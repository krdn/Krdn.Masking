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
        public string MaskEmail(string email, int visibleCharCount = 2)
        {
            return new EmailMaskingAttribute(visibleCharCount).Mask(email);
        }
        
        /// <inheritdoc/>
        public string MaskPhoneNumber(string phoneNumber)
        {
            return new PhoneMaskingAttribute().Mask(phoneNumber);
        }
        
        /// <inheritdoc/>
        public string MaskName(string name, int visibleCharCount = 1)
        {
            return new NameMaskingAttribute(visibleCharCount).Mask(name);
        }
        
        /// <inheritdoc/>
        public string MaskCreditCard(string cardNumber)
        {
            return new CreditCardMaskingAttribute().Mask(cardNumber);
        }
        
        /// <inheritdoc/>
        public string MaskPassport(string passportNumber)
        {
            return new PassportMaskingAttribute().Mask(passportNumber);
        }
        
        /// <inheritdoc/>
        public string MaskWithAttribute(string value, MaskingAttribute attribute)
        {
            if (attribute == null || string.IsNullOrEmpty(value))
                return value;
                
            return attribute.Mask(value);
        }
    }
}
