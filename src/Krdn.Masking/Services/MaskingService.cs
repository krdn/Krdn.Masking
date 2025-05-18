using System;

namespace Krdn.Masking.Services
{
    /// <summary>
    /// 마스킹 서비스 - 마스킹 작업의 통합 진입점
    /// </summary>
    public class MaskingService
    {
        private readonly IMaskingProvider _maskingProvider;
        
        /// <summary>
        /// 기본 제공자를 사용하여 마스킹 서비스를 초기화합니다.
        /// </summary>
        public MaskingService() : this(new DefaultMaskingProvider())
        {
        }
        
        /// <summary>
        /// 지정된 제공자를 사용하여 마스킹 서비스를 초기화합니다.
        /// </summary>
        /// <param name="maskingProvider">사용할 마스킹 제공자</param>
        public MaskingService(IMaskingProvider maskingProvider)
        {
            _maskingProvider = maskingProvider ?? throw new ArgumentNullException(nameof(maskingProvider));
        }
        
        /// <summary>
        /// 이메일 주소를 마스킹합니다.
        /// </summary>
        /// <param name="email">원본 이메일</param>
        /// <param name="visibleCharCount">표시할 문자 수</param>
        /// <returns>마스킹된 이메일</returns>
        public string MaskEmail(string email, int visibleCharCount = 2)
        {
            return _maskingProvider.MaskEmail(email, visibleCharCount);
        }
        
        /// <summary>
        /// 전화번호를 마스킹합니다.
        /// </summary>
        /// <param name="phoneNumber">원본 전화번호</param>
        /// <returns>마스킹된 전화번호</returns>
        public string MaskPhoneNumber(string phoneNumber)
        {
            return _maskingProvider.MaskPhoneNumber(phoneNumber);
        }
        
        /// <summary>
        /// 이름을 마스킹합니다.
        /// </summary>
        /// <param name="name">원본 이름</param>
        /// <param name="visibleCharCount">표시할 문자 수</param>
        /// <returns>마스킹된 이름</returns>
        public string MaskName(string name, int visibleCharCount = 1)
        {
            return _maskingProvider.MaskName(name, visibleCharCount);
        }
        
        /// <summary>
        /// 신용카드 번호를 마스킹합니다.
        /// </summary>
        /// <param name="cardNumber">원본 카드번호</param>
        /// <returns>마스킹된 카드번호</returns>
        public string MaskCreditCard(string cardNumber)
        {
            return _maskingProvider.MaskCreditCard(cardNumber);
        }
        
        /// <summary>
        /// 여권 번호를 마스킹합니다.
        /// </summary>
        /// <param name="passportNumber">원본 여권번호</param>
        /// <returns>마스킹된 여권번호</returns>
        public string MaskPassport(string passportNumber)
        {
            return _maskingProvider.MaskPassport(passportNumber);
        }
    }
}
