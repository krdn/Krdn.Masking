using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Krdn.Masking.Attributes;
using Krdn.Masking.Extensions;
using Krdn.Masking.Services;
using Xunit;
using Xunit.Abstractions;

namespace Krdn.Masking.Tests
{
    public class AttributeTests
    {
        private readonly ITestOutputHelper _output;
        
        public AttributeTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void EmailMasking_ShouldMaskCorrectly()
        {
            // Arrange
            var attribute = new EmailMaskingAttribute(2);
            var email = "honggildong@example.com";
            
            // Act
            var masked = attribute.Mask(email);
            
            // Assert
            Assert.Equal("ho*********@example.com", masked);
        }
        
        [Fact]
        public void PhoneMasking_ShouldMaskMiddlePart()
        {
            // Arrange
            var attribute = new PhoneMaskingAttribute();
            var phone = "010-1234-5678";
            
            // Act
            var masked = attribute.Mask(phone);
            
            // Assert
            Assert.Equal("010-****-5678", masked);
        }
        
        [Fact]
        public void NameMasking_ShouldMaskAllButFirstChar()
        {
            // Arrange
            var attribute = new NameMaskingAttribute(1);
            var name = "홍길동";
            
            // Act
            var masked = attribute.Mask(name);
            
            // Assert
            Assert.Equal("홍**", masked);
        }
        
        [Fact]
        public void CreditCardMasking_ShouldMaskMiddlePart()
        {
            // Arrange
            var attribute = new CreditCardMaskingAttribute();
            var cardNumber = "1234-5678-9012-3456";
            
            // Act
            var masked = attribute.Mask(cardNumber);
            
            // Assert
            Assert.Equal("1234-5678-****-3456", masked);
        }
        
        [Fact]
        public void PassportMasking_ShouldMaskMiddlePart()
        {
            // Arrange
            var attribute = new PassportMaskingAttribute();
            var passportNumber = "M12345678";
            
            // Act
            var masked = attribute.Mask(passportNumber);
            
            // Assert
            Assert.Equal("M1******8", masked);
        }
        
        [Fact]
        public void Mask_OnModelWithAttributes_ShouldMaskAllMarkedProperties()
        {
            // Arrange
            var model = new TestModel
            {
                Id = 1,
                Name = "홍길동",
                Email = "hong@example.com",
                PhoneNumber = "010-1234-5678",
                Address = "서울시 강남구" // 마스킹 속성 없음
            };
            
            // Act
            var masked = model.Mask();
            
            // Assert
            Assert.Equal(1, masked.Id);
            Assert.Equal("홍**", masked.Name);
            Assert.Equal("ho**@example.com", masked.Email);
            Assert.Equal("010-****-5678", masked.PhoneNumber);
            Assert.Equal("서울시 강남구", masked.Address); // 변경되지 않아야 함
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
            
            public string Address { get; set; }
        }
    }
}
