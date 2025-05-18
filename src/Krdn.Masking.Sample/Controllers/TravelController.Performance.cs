using System;
using System.Collections.Generic;
using System.Linq;
using Krdn.Masking.Extensions;
using Krdn.Masking.Sample.Models;
using Krdn.Masking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Krdn.Masking.Sample.Controllers
{
    public partial class TravelController
    {
        /// <summary>
        /// 성능 비교를 위한 대량 예약 생성 및 마스킹 (테스트용)
        /// </summary>
        [HttpGet("performance-test")]
        public ActionResult PerformanceTest([FromQuery] int count = 1000)
        {
            // 테스트 결과 저장용
            var results = new Dictionary<string, long>();
            
            // 테스트 데이터 생성
            var testBookings = new List<TravelBookingRequest>();
            for (int i = 0; i < count; i++)
            {
                testBookings.Add(new TravelBookingRequest
                {
                    BookingId = i,
                    BookingDate = DateTime.Now,
                    Customer = new CustomerInfo
                    {
                        CustomerId = i,
                        Name = "홍길동",
                        Email = "hong.gildong@example.com",
                        PhoneNumber = "010-1234-5678",
                        PassportNumber = "M12345678"
                    },
                    Package = _packages[i % _packages.Count],
                    NumberOfTravelers = 2,
                    CreditCardNumber = "1234-5678-9012-3456",
                    Status = "확정"
                });
            }
            
            // 1. 직접 함수 호출 방식 성능 측정
            var directWatch = System.Diagnostics.Stopwatch.StartNew();
            
            var directResults = testBookings.Select(booking => new TravelBookingResponse
            {
                BookingId = booking.BookingId,
                Customer = new CustomerInfo
                {
                    CustomerId = booking.Customer.CustomerId,
                    Name = _maskingService.MaskName(booking.Customer.Name),
                    Email = _maskingService.MaskEmail(booking.Customer.Email),
                    PhoneNumber = _maskingService.MaskPhoneNumber(booking.Customer.PhoneNumber),
                    PassportNumber = _maskingService.MaskPassport(booking.Customer.PassportNumber)
                },
                Package = booking.Package
            }).ToList();
            
            directWatch.Stop();
            results["직접 함수 호출 방식"] = directWatch.ElapsedMilliseconds;
            
            // 2. 속성 기반 마스킹 방식 성능 측정
            var attributeWatch = System.Diagnostics.Stopwatch.StartNew();
            
            var attributeResults = testBookings.Select(booking => new TravelBookingResponse
            {
                BookingId = booking.BookingId,
                Customer = booking.Customer,
                Package = booking.Package
            }.Mask()).ToList();
            
            attributeWatch.Stop();
            results["속성 기반 마스킹 방식"] = attributeWatch.ElapsedMilliseconds;
            
            // 3. 병렬 처리 속성 기반 마스킹 방식 성능 측정
            var parallelWatch = System.Diagnostics.Stopwatch.StartNew();
            
            var responses = testBookings.Select(booking => new TravelBookingResponse
            {
                BookingId = booking.BookingId,
                Customer = booking.Customer,
                Package = booking.Package
            }).ToList();
            
            var parallelResults = responses.MaskAllParallel().ToList();
            
            parallelWatch.Stop();
            results["병렬 처리 속성 기반 마스킹 방식"] = parallelWatch.ElapsedMilliseconds;
            
            return Ok(new
            {
                TestCount = count,
                Results = results,
                // 샘플 결과
                DirectSample = directResults.First(),
                AttributeSample = attributeResults.First(),
                ParallelSample = parallelResults.First()
            });
        }
    }
}
