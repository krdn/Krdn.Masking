using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Krdn.Masking.Attributes;
using Krdn.Masking.Extensions;
using Krdn.Masking.Services;
using Xunit;
using Xunit.Abstractions;

namespace Krdn.Masking.Tests
{
    public class PerformanceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly MaskingService _service;
        private const int TestIterations = 10000;
        
        public PerformanceTests(ITestOutputHelper output)
        {
            _output = output;
            _service = new MaskingService();
        }
        
        [Fact]
        public void ComparePerformance_DirectVsAttribute()
        {
            // 테스트 데이터 생성
            var testData = new List<TestModel>();
            for (int i = 0; i < TestIterations; i++)
            {
                testData.Add(new TestModel
                {
                    Id = i,
                    Name = "홍길동",
                    Email = "test@example.com",
                    PhoneNumber = "010-1234-5678",
                    CreditCard = "1234-5678-9012-3456",
                    PassportNumber = "M12345678"
                });
            }
            
            // 1. 직접 마스킹 함수 호출 방식
            var directWatch = Stopwatch.StartNew();
            
            var directResults = testData.Select(model => new TestModelDto
            {
                Id = model.Id,
                Name = _service.MaskName(model.Name),
                Email = _service.MaskEmail(model.Email),
                PhoneNumber = _service.MaskPhoneNumber(model.PhoneNumber),
                CreditCard = _service.MaskCreditCard(model.CreditCard),
                PassportNumber = _service.MaskPassport(model.PassportNumber)
            }).ToList();
            
            directWatch.Stop();
            _output.WriteLine($"직접 함수 호출 방식: {directWatch.ElapsedMilliseconds}ms");
            
            // 2. 속성 기반 마스킹 방식
            var attributeWatch = Stopwatch.StartNew();
            
            var attributeResults = testData.Select(model => model.Mask()).ToList();
            
            attributeWatch.Stop();
            _output.WriteLine($"속성 기반 마스킹 방식: {attributeWatch.ElapsedMilliseconds}ms");
            
            // 3. 병렬 처리 속성 기반 마스킹 방식
            var parallelWatch = Stopwatch.StartNew();
            
            var parallelResults = testData.MaskAllParallel().ToList();
            
            parallelWatch.Stop();
            _output.WriteLine($"병렬 처리 속성 기반 마스킹 방식: {parallelWatch.ElapsedMilliseconds}ms");
            
            // 검증
            Assert.Equal(TestIterations, directResults.Count);
            Assert.Equal(TestIterations, attributeResults.Count);
            Assert.Equal(TestIterations, parallelResults.Count);
            
            // 샘플 결과 비교
            var directSample = directResults.First();
            var attributeSample = attributeResults.First();
            
            Assert.Equal(directSample.Name, attributeSample.Name);
            Assert.Equal(directSample.Email, attributeSample.Email);
            Assert.Equal(directSample.PhoneNumber, attributeSample.PhoneNumber);
        }
        
        // 테스트용 모델 클래스
        private class TestModel
        {
            public int Id { get; set; }
            
            [NameMasking]
            public string Name { get; set; }
            
            [EmailMasking]
            public string Email { get; set; }
            
            [PhoneMasking]
            public string PhoneNumber { get; set; }
            
            [CreditCardMasking]
            public string CreditCard { get; set; }
            
            [PassportMasking]
            public string PassportNumber { get; set; }
        }
        
        // 마스킹 결과를 저장할 DTO
        private class TestModelDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string CreditCard { get; set; }
            public string PassportNumber { get; set; }
        }
    }
}
