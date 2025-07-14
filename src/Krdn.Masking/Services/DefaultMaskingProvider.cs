using System;
using Krdn.Masking.Attributes;

namespace Krdn.Masking.Services
{
    /// <summary>
    /// 기본 마스킹 제공자 구현
    /// </summary>
    public class DefaultMaskingProvider : IMaskingProvider
    {
        /// <inheritdoc/>
        public virtual string MaskEmail(string email, int visibleCharCount = 2)
        {
            return new EmailMaskingAttribute(visibleCharCount).Mask(email);
        }
        
        /// <inheritdoc/>
        public virtual string MaskPhoneNumber(string phoneNumber)
        {
            return new PhoneMaskingAttribute().Mask(phoneNumber);
        }
        
        /// <inheritdoc/>
        public virtual string MaskName(string name, int visibleCharCount = 1)
        {
            return new NameMaskingAttribute(visibleCharCount).Mask(name);
        }
        
        /// <inheritdoc/>
        public virtual string MaskCreditCard(string cardNumber)
        {
            return new CreditCardMaskingAttribute().Mask(cardNumber);
        }
        
        /// <inheritdoc/>
        public virtual string MaskPassport(string passportNumber)
        {
            return new PassportMaskingAttribute().Mask(passportNumber);
        }
        
        /// <inheritdoc/>
        public virtual string MaskWithAttribute(string value, MaskingAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute), "마스킹 속성이 null입니다.");
                
            if (string.IsNullOrEmpty(value))
                return value;
                
            try
            {
                return attribute.Mask(value);
            }
            catch (Exception ex)
            {
                // 로그 기록 후 원본 값 반환
                System.Diagnostics.Debug.WriteLine($"마스킹 오류: {ex.Message}");
                return value;
            }
        }
    }
}
