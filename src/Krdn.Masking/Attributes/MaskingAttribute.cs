using System;
using System.Linq;

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

            try
            {
                var parts = value.Split('@');
                if (parts.Length != 2)
                    return value;
                    
                var name = parts[0];
                var domain = parts[1];
                
                // 도메인이 비어있으면 원본 반환
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(domain))
                    return value;
                
                if (name.Length <= _visibleCharCount)
                    return value;
                    
                var masked = name.Substring(0, _visibleCharCount) + new string('*', name.Length - _visibleCharCount);
                return $"{masked}@{domain}";
            }
            catch (Exception)
            {
                // 예외 발생 시 원본 값 반환
                return value;
            }
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
                
            try
            {
                // 공백 제거 후 검증
                var trimmedValue = value.Trim();
                if (string.IsNullOrEmpty(trimmedValue))
                    return value;
                    
                // 한국 전화번호 형식 (010-1234-5678)
                var result = System.Text.RegularExpressions.Regex.Replace(
                    trimmedValue, 
                    @"(\d{3})-(\d{4})-(\d{4})", 
                    "$1-****-$3");
                    
                // 패턴이 매치되지 않으면 원본 반환
                return result == trimmedValue ? value : result;
            }
            catch (Exception)
            {
                // 예외 발생 시 원본 값 반환
                return value;
            }
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
            if (string.IsNullOrEmpty(value))
                return value;
                
            try
            {
                var trimmedValue = value.Trim();
                if (string.IsNullOrEmpty(trimmedValue) || trimmedValue.Length <= _visibleCharCount)
                    return value;
                    
                return trimmedValue.Substring(0, _visibleCharCount) + new string('*', trimmedValue.Length - _visibleCharCount);
            }
            catch (Exception)
            {
                // 예외 발생 시 원본 값 반환
                return value;
            }
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
                
            try
            {
                var trimmedValue = value.Trim();
                if (string.IsNullOrEmpty(trimmedValue))
                    return value;
                    
                // 표준 신용카드 형식 (1234-5678-9012-3456 또는 1234567890123456)
                var normalized = trimmedValue.Replace("-", "").Replace(" ", "");
                
                // 15자리(아멕스) 또는 16자리 카드 지원
                if (normalized.Length != 15 && normalized.Length != 16)
                    return value;
                    
                // 모든 문자가 숫자인지 확인
                if (!normalized.All(char.IsDigit))
                    return value;
                
                if (normalized.Length == 15) // 아멕스
                {
                    var formatted15 = string.Format("{0}-{1}-{2}-{3}", 
                        normalized.Substring(0, 4),
                        normalized.Substring(4, 6),
                        "*****",
                        normalized.Substring(11, 4));
                    return formatted15;
                }
                else // 16자리
                {
                    var formatted16 = string.Format("{0}-{1}-{2}-{3}", 
                        normalized.Substring(0, 4),
                        normalized.Substring(4, 4),
                        "****",
                        normalized.Substring(12, 4));
                    return formatted16;
                }
            }
            catch (Exception)
            {
                // 예외 발생 시 원본 값 반환
                return value;
            }
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
                
            try
            {
                var trimmedValue = value.Trim().ToUpper();
                if (string.IsNullOrEmpty(trimmedValue))
                    return value;
                    
                // 기본 여권 번호 형식 (M12345678) 우선 처리
                var result = System.Text.RegularExpressions.Regex.Replace(
                    trimmedValue, 
                    @"^([A-Z]\d{1})(\d{6})(\d{1})$", 
                    "$1******$3");
                
                // 기본 패턴이 매치되지 않으면 확장 패턴 시도
                if (result == trimmedValue)
                {
                    result = System.Text.RegularExpressions.Regex.Replace(
                        trimmedValue, 
                        @"^([A-Z]{1,2}\d{1,2})(\d{4,6})(\d{1,2})$", 
                        match => 
                        {
                            var prefix = match.Groups[1].Value;
                            var middle = match.Groups[2].Value;
                            var suffix = match.Groups[3].Value;
                            return prefix + new string('*', middle.Length) + suffix;
                        });
                }
                    
                // 패턴이 매치되지 않으면 원본 반환
                return result == trimmedValue ? value : result;
            }
            catch (Exception)
            {
                // 예외 발생 시 원본 값 반환
                return value;
            }
        }
        
        /// <inheritdoc/>
        public override string MaskingType => "Passport";
    }
}
