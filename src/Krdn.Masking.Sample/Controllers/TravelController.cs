using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Krdn.Masking.Extensions;
using Krdn.Masking.Sample.Models;
using Krdn.Masking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Krdn.Masking.Sample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class TravelController : ControllerBase
    {
        private readonly ILogger<TravelController> _logger;
        private readonly MaskingService _maskingService;
        
        // 샘플 데이터
        private static readonly List<TravelPackage> _packages = new List<TravelPackage>
        {
            new TravelPackage
            {
                PackageId = 1,
                Destination = "제주도",
                DepartureDate = DateTime.Parse("2025-07-01"),
                ReturnDate = DateTime.Parse("2025-07-05"),
                Price = 550000,
                Description = "제주도 4박 5일 패키지"
            },
            new TravelPackage
            {
                PackageId = 2,
                Destination = "방콕",
                DepartureDate = DateTime.Parse("2025-08-15"),
                ReturnDate = DateTime.Parse("2025-08-20"),
                Price = 1200000,
                Description = "방콕 5박 6일 패키지"
            },
            new TravelPackage
            {
                PackageId = 3,
                Destination = "오사카",
                DepartureDate = DateTime.Parse("2025-09-10"),
                ReturnDate = DateTime.Parse("2025-09-14"),
                Price = 850000,
                Description = "오사카 4박 5일 패키지"
            }
        };
        
        private static readonly List<TravelBookingRequest> _bookings = new List<TravelBookingRequest>();
        private static int _nextBookingId = 1;
        
        public TravelController(ILogger<TravelController> logger)
        {
            _logger = logger;
            _maskingService = new MaskingService();
        }
        
        /// <summary>
        /// 모든 여행 패키지를 가져옵니다.
        /// </summary>
        [HttpGet("packages")]
        public ActionResult<IEnumerable<TravelPackage>> GetPackages()
        {
            return Ok(_packages);
        }
        
        /// <summary>
        /// ID로 여행 패키지를 가져옵니다.
        /// </summary>
        [HttpGet("packages/{id}")]
        public ActionResult<TravelPackage> GetPackage(int id)
        {
            var package = _packages.FirstOrDefault(p => p.PackageId == id);
            if (package == null)
                return NotFound();
                
            return Ok(package);
        }
        
        /// <summary>
        /// 여행 예약을 생성합니다.
        /// </summary>
        [HttpPost("bookings")]
        public ActionResult<TravelBookingResponse> CreateBooking(TravelBookingRequest request)
        {
            if (request.Customer == null || string.IsNullOrEmpty(request.CreditCardNumber))
                return BadRequest("고객 정보 및 결제 정보는 필수입니다.");
                
            var package = _packages.FirstOrDefault(p => p.PackageId == request.Package?.PackageId);
            if (package == null)
                return BadRequest("유효한 여행 패키지를 선택해주세요.");
                
            // 새 예약 생성
            request.BookingId = _nextBookingId++;
            request.BookingDate = DateTime.Now;
            request.Package = package;
            request.Status = "확정";
            
            // 예약 저장
            _bookings.Add(request);
            
            // 응답 객체 생성
            var response = new TravelBookingResponse
            {
                BookingId = request.BookingId,
                ConfirmationCode = $"BK{request.BookingId:D6}",
                Status = request.Status,
                Customer = request.Customer,
                Package = request.Package,
                TotalPrice = request.Package.Price * request.NumberOfTravelers
            };
            
            // 속성 기반 마스킹 적용 - 확장 메서드 사용
            return Ok(response.Mask());
        }
        
        /// <summary>
        /// 모든 예약 정보를 가져옵니다.
        /// 직접 마스킹 함수 호출 방식을 시연합니다.
        /// </summary>
        [HttpGet("bookings")]
        public ActionResult<IEnumerable<TravelBookingResponse>> GetBookings()
        {
            var responses = _bookings.Select(booking => new TravelBookingResponse
            {
                BookingId = booking.BookingId,
                ConfirmationCode = $"BK{booking.BookingId:D6}",
                Status = booking.Status,
                Customer = new CustomerInfo
                {
                    CustomerId = booking.Customer.CustomerId,
                    // 직접 마스킹 함수 호출 방식
                    Name = _maskingService.MaskName(booking.Customer.Name),
                    Email = _maskingService.MaskEmail(booking.Customer.Email),
                    PhoneNumber = _maskingService.MaskPhoneNumber(booking.Customer.PhoneNumber),
                    PassportNumber = _maskingService.MaskPassport(booking.Customer.PassportNumber)
                },
                Package = booking.Package,
                TotalPrice = booking.Package.Price * booking.NumberOfTravelers
            }).ToList();
            
            return Ok(responses);
        }
        
        /// <summary>
        /// ID로 예약 정보를 가져옵니다.
        /// </summary>
        [HttpGet("bookings/{id}")]
        public ActionResult<TravelBookingResponse> GetBooking(int id)
        {
            var booking = _bookings.FirstOrDefault(b => b.BookingId == id);
            if (booking == null)
                return NotFound();
                
            // 응답 객체 생성
            var response = new TravelBookingResponse
            {
                BookingId = booking.BookingId,
                ConfirmationCode = $"BK{booking.BookingId:D6}",
                Status = booking.Status,
                Customer = booking.Customer,
                Package = booking.Package,
                TotalPrice = booking.Package.Price * booking.NumberOfTravelers
            };
            
            // 문자열 확장 메서드를 사용한 마스킹 예시
            // 특정 마스킹이 필요한 경우에 유용
            if (response.Customer != null)
            {
                response.Customer.Email = response.Customer.Email.MaskEmail(3); // 첫 3글자 표시
                response.Customer.PhoneNumber = response.Customer.PhoneNumber.MaskPhoneNumber();
            }
            
            return Ok(response);
        }
    }
}
