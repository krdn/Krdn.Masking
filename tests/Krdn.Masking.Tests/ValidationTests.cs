using System;
using Krdn.Masking.Attributes;
using Krdn.Masking.Extensions;
using Krdn.Masking.Services;
using Xunit;

namespace Krdn.Masking.Tests
{
    /// <summary>
    /// 입력 검증 및 예외 처리 테스트
    /// </summary>
    public class ValidationTests
    {
        private readonly MaskingService _service;
        
        public ValidationTests()
        {
            _service = new MaskingService();
        }
        
        [Fact]
        public void MaskingService_Constructor_ThrowsOnNullProvider()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MaskingService(null));
        }
        
        [Fact]
        public void MaskingService_MaskEmail_ThrowsOnNegativeVisibleCharCount()
        {
            // Arrange
            var email = "test@example.com";
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.MaskEmail(email, -1));
        }
        
        [Fact]
        public void MaskingService_MaskName_ThrowsOnNegativeVisibleCharCount()
        {
            // Arrange
            var name = "홍길동";
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.MaskName(name, -1));
        }
        
        [Fact]
        public void EmailMaskingAttribute_HandlesNullAndEmpty()
        {
            // Arrange
            var attribute = new EmailMaskingAttribute();
            
            // Act & Assert
            Assert.Null(attribute.Mask(null));
            Assert.Equal("", attribute.Mask(""));
            Assert.Equal("   ", attribute.Mask("   "));
        }
        
        [Fact]
        public void EmailMaskingAttribute_HandlesInvalidFormat()
        {
            // Arrange
            var attribute = new EmailMaskingAttribute();
            
            // Act & Assert
            Assert.Equal("notanemail", attribute.Mask("notanemail"));
            Assert.Equal("@", attribute.Mask("@"));
            Assert.Equal("test@", attribute.Mask("test@"));
            Assert.Equal("@example.com", attribute.Mask("@example.com"));
        }
        
        [Fact]
        public void PhoneMaskingAttribute_HandlesInvalidFormat()
        {
            // Arrange
            var attribute = new PhoneMaskingAttribute();
            
            // Act & Assert
            Assert.Equal("123456", attribute.Mask("123456")); // 패턴에 맞지 않음
            Assert.Equal("abc-def-ghij", attribute.Mask("abc-def-ghij")); // 숫자가 아님
        }
        
        [Fact]
        public void CreditCardMaskingAttribute_HandlesInvalidLength()
        {
            // Arrange
            var attribute = new CreditCardMaskingAttribute();
            
            // Act & Assert
            Assert.Equal("12345", attribute.Mask("12345")); // 너무 짧음
            Assert.Equal("12345678901234567", attribute.Mask("12345678901234567")); // 너무 김
        }
        
        [Fact]
        public void CreditCardMaskingAttribute_HandlesNonNumeric()
        {
            // Arrange
            var attribute = new CreditCardMaskingAttribute();
            
            // Act & Assert
            Assert.Equal("abcd-efgh-ijkl-mnop", attribute.Mask("abcd-efgh-ijkl-mnop"));
        }
        
        [Fact]
        public void CreditCardMaskingAttribute_Handles15DigitAmex()
        {
            // Arrange
            var attribute = new CreditCardMaskingAttribute();
            
            // Act
            var masked = attribute.Mask("123456789012345");
            
            // Assert
            Assert.Equal("1234-567890-*****-2345", masked);
        }
        
        [Fact]
        public void PassportMaskingAttribute_HandlesInvalidFormat()
        {
            // Arrange
            var attribute = new PassportMaskingAttribute();
            
            // Act & Assert
            Assert.Equal("INVALID", attribute.Mask("INVALID"));
            Assert.Equal("123456789", attribute.Mask("123456789")); // 숫자만
        }
        
        [Fact]
        public void PassportMaskingAttribute_HandlesFlexibleFormat()
        {
            // Arrange
            var attribute = new PassportMaskingAttribute();
            
            // Act & Assert
            Assert.Equal("AB12****7", attribute.Mask("AB1234567")); // 유연한 패턴
            Assert.Equal("M1******8", attribute.Mask("M12345678")); // 기존 패턴
        }
        
        [Fact]
        public void MaskingExtensions_HandlesNullObject()
        {
            // Arrange
            TestModel obj = null;
            
            // Act
            var result = obj.Mask();
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void MaskingExtensions_HandlesObjectWithoutParameterlessConstructor()
        {
            // Arrange
            var obj = new ObjectWithoutParameterlessConstructor("test");
            
            // Act & Assert - 예외가 발생하지 않고 원본 객체가 반환되어야 함
            var result = obj.Mask();
            Assert.NotNull(result);
        }
        
        [Fact]
        public void DefaultMaskingProvider_MaskWithAttribute_ThrowsOnNullAttribute()
        {
            // Arrange
            var provider = new DefaultMaskingProvider();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                provider.MaskWithAttribute("test", null));
        }
        
        // 테스트용 모델 클래스
        private class TestModel
        {
            [EmailMasking]
            public string Email { get; set; }
            
            [NameMasking]
            public string Name { get; set; }
        }
        
        // 매개변수 없는 생성자가 없는 클래스
        private class ObjectWithoutParameterlessConstructor
        {
            public string Value { get; }
            
            public ObjectWithoutParameterlessConstructor(string value)
            {
                Value = value;
            }
        }
    }
}