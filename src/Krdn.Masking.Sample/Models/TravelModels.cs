using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Krdn.Masking.Attributes;

namespace Krdn.Masking.Sample.Models
{
    /// <summary>
    /// 고객 정보
    /// </summary>
    public class CustomerInfo
    {
        /// <summary>
        /// 고객 ID
        /// </summary>
        public int CustomerId { get; set; }
        
        /// <summary>
        /// 고객 이름
        /// </summary>
        [NameMasking]
        public string Name { get; set; }
        
        /// <summary>
        /// 이메일 주소
        /// </summary>
        [EmailMasking]
        public string Email { get; set; }
        
        /// <summary>
        /// 전화번호
        /// </summary>
        [PhoneMasking]
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// 여권 번호
        /// </summary>
        [PassportMasking]
        public string PassportNumber { get; set; }
    }
    
    /// <summary>
    /// 여행 패키지 정보
    /// </summary>
    public class TravelPackage
    {
        /// <summary>
        /// 패키지 ID
        /// </summary>
        public int PackageId { get; set; }
        
        /// <summary>
        /// 여행지
        /// </summary>
        public string Destination { get; set; }
        
        /// <summary>
        /// 출발일
        /// </summary>
        public DateTime DepartureDate { get; set; }
        
        /// <summary>
        /// 도착일
        /// </summary>
        public DateTime ReturnDate { get; set; }
        
        /// <summary>
        /// 가격
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// 설명
        /// </summary>
        public string Description { get; set; }
    }
    
    /// <summary>
    /// 여행 예약 요청
    /// </summary>
    public class TravelBookingRequest
    {
        /// <summary>
        /// 예약 ID
        /// </summary>
        public int BookingId { get; set; }
        
        /// <summary>
        /// 예약일
        /// </summary>
        public DateTime BookingDate { get; set; }
        
        /// <summary>
        /// 고객 정보
        /// </summary>
        public CustomerInfo Customer { get; set; }
        
        /// <summary>
        /// 여행 패키지
        /// </summary>
        public TravelPackage Package { get; set; }
        
        /// <summary>
        /// 총 인원
        /// </summary>
        public int NumberOfTravelers { get; set; }
        
        /// <summary>
        /// 결제 정보 - 신용카드 번호
        /// </summary>
        [CreditCardMasking]
        public string CreditCardNumber { get; set; }
        
        /// <summary>
        /// 예약 상태
        /// </summary>
        public string Status { get; set; }
    }
    
    /// <summary>
    /// 여행 예약 응답
    /// </summary>
    public class TravelBookingResponse
    {
        /// <summary>
        /// 예약 ID
        /// </summary>
        public int BookingId { get; set; }
        
        /// <summary>
        /// 예약 확인 코드
        /// </summary>
        public string ConfirmationCode { get; set; }
        
        /// <summary>
        /// 예약 상태
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// 고객 정보
        /// </summary>
        public CustomerInfo Customer { get; set; }
        
        /// <summary>
        /// 여행 패키지
        /// </summary>
        public TravelPackage Package { get; set; }
        
        /// <summary>
        /// 총 가격
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}
