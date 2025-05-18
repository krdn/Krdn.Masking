using System;

namespace Krdn.Masking.Attributes
{
    /// <summary>
    /// 데이터 마스킹을 위한 기본 추상 속성 클래스
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class MaskingAttribute : Attribute
    {
        /// <summary>
        /// 주어진 문자열 값에 마스킹을 적용합니다.
        /// </summary>
        /// <param name="value">마스킹할 원본 문자열</param>
        /// <returns>마스킹된 문자열</returns>
        public abstract string Mask(string value);
        
        /// <summary>
        /// 디버그에 표시하기 위한 마스킹 유형 이름
        /// </summary>
        public abstract string MaskingType { get; }
    }
    
    /// <summary>
    /// 이메일 주소에 대한 마스킹 속성
    /// </summary>
    public class EmailMaskingAttribute : MaskingAttribute
    {
        private readonly int _visibleCharCount;
        
        /// <summary>
        /// 이메일 마스킹 속성 초기화
        /// </summary>
        /// <param name="visibleCharCount">ID 부분에서 표시할 문자 수 (기본값: 2)</param>
        public EmailMaskingAttribute(int visibleCharCount = 2)
        {
            _visibleCharCount = Math.Max(0, visibleCharCount);
        }
        
        /// <inheritdoc/>
        public override string Mask(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var parts = value.Split('@');
            if (parts.Length != 2)
                return value;
                
            var name = parts[0];
            var domain = parts[1];
            
            if (name.Length <= _visibleCharCount)
                return value;
                
            var masked = name.Substring(0, _visibleCharCount) + new string('*', name.Length - _visibleCharCount);
            return $"{masked}@{domain}";
        }
        
        /// <inheritdoc/>
        public override string MaskingType => "Email";
    }
    
    /// <summary>
    /// 전화번호에 대한 마스킹 속성
    /// </summary>
    public class PhoneMaskingAttribute : MaskingAttribute
    {
        /// <inheritdoc/>
        public override string Mask(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
                
            // 한국 전화번호 형식 (010-1234-5678)
            return System.Text.RegularExpressions.Regex.Replace(
                value, 
                @"(\d{3})-(\d{4})-(\d{4})", 
                "$1-****-$3");
        }
        
        /// <inheritdoc/>
        public override string MaskingType => "Phone";
    }
    
    /// <summary>
    /// 이름에 대한 마스킹 속성
    /// </summary>
    public class NameMaskingAttribute : MaskingAttribute
    {
        private readonly int _visibleCharCount;
        
        /// <summary>
        /// 이름 마스킹 속성 초기화
        /// </summary>
        /// <param name="visibleCharCount">표시할 문자 수 (기본값: 1)</param>
        public NameMaskingAttribute(int visibleCharCount = 1)
        {
            _visibleCharCount = Math.Max(0, visibleCharCount);
        }
        
        /// <inheritdoc/>
        public override string Mask(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= _visibleCharCount)
                return value;
                
            return value.Substring(0, _visibleCharCount) + new string('*', value.Length - _visibleCharCount);
        }
        
        /// <inheritdoc/>
        public override string MaskingType => "Name";
    }
    
    /// <summary>
    /// 신용카드 번호에 대한 마스킹 속성
    /// </summary>
    public class CreditCardMaskingAttribute : MaskingAttribute
    {
        /// <inheritdoc/>
        public override string Mask(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
                
            // 표준 신용카드 형식 (1234-5678-9012-3456 또는 1234567890123456)
            var normalized = value.Replace("-", "");
            
            if (normalized.Length != 16)
                return value;
                
            var formatted = string.Format("{0}-{1}-{2}-{3}", 
                normalized.Substring(0, 4),
                normalized.Substring(4, 4),
                "****",
                normalized.Substring(12, 4));
                
            return formatted;
        }
        
        /// <inheritdoc/>
        public override string MaskingType => "CreditCard";
    }
    
    /// <summary>
    /// 여권 번호에 대한 마스킹 속성
    /// </summary>
    public class PassportMaskingAttribute : MaskingAttribute
    {
        /// <inheritdoc/>
        public override string Mask(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
                
            // 여권 번호 형식 (M12345678)
            return System.Text.RegularExpressions.Regex.Replace(
                value, 
                @"^([A-Z]\d{1})(\d{6})(\d{1})$", 
                "$1******$3");
        }
        
        /// <inheritdoc/>
        public override string MaskingType => "Passport";
    }
}
