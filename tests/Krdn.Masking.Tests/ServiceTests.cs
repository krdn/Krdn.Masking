using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Krdn.Masking.Extensions;
using Krdn.Masking.Services;
using Xunit;
using Xunit.Abstractions;

namespace Krdn.Masking.Tests
{
    public class ServiceTests
    {
        private readonly MaskingService _service;
        private readonly ITestOutputHelper _output;
        
        public ServiceTests(ITestOutputHelper output)
        {
            _service = new MaskingService();
            _output = output;
        }
        
        [Fact]
        public void MaskEmail_ShouldMaskCorrectly()
        {
            // Arrange
            var email = "test@example.com";
            
            // Act
            var masked = _service.MaskEmail(email);
            
            // Assert
            Assert.Equal("te**@example.com", masked);
        }
        
        [Fact]
        public void MaskPhoneNumber_ShouldMaskMiddlePart()
        {
            // Arrange
            var phone = "010-1234-5678";
            
            // Act
            var masked = _service.MaskPhoneNumber(phone);
            
            // Assert
            Assert.Equal("010-****-5678", masked);
        }
        
        [Fact]
        public void MaskName_ShouldMaskAllButFirstChar()
        {
            // Arrange
            var name = "홍길동";
            
            // Act
            var masked = _service.MaskName(name);
            
            // Assert
            Assert.Equal("홍**", masked);
        }
        
        [Fact]
        public void ExtensionMethods_ShouldWorkCorrectly()
        {
            // Arrange
            var email = "test@example.com";
            var phone = "010-1234-5678";
            var name = "홍길동";
            
            // Act
            var maskedEmail = email.MaskEmail();
            var maskedPhone = phone.MaskPhoneNumber();
            var maskedName = name.MaskName();
            
            // Assert
            Assert.Equal("te**@example.com", maskedEmail);
            Assert.Equal("010-****-5678", maskedPhone);
            Assert.Equal("홍**", maskedName);
        }
        
        [Fact]
        public void CustomMaskingProvider_ShouldBeUsable()
        {
            // Arrange
            var customProvider = new CustomMaskingProvider();
            var customService = new MaskingService(customProvider);
            var email = "test@example.com";
            
            // Act
            var masked = customService.MaskEmail(email);
            
            // Assert
            Assert.Equal("t***@example.com", masked);
        }
        
        // 커스텀 마스킹 제공자 구현
        private class CustomMaskingProvider : DefaultMaskingProvider
        {
            public override string MaskEmail(string email, int visibleCharCount = 2)
            {
                // 기본 구현을 재정의하여 1글자만 표시
                return base.MaskEmail(email, 1);
            }
        }
    }
}
