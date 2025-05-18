# Krdn.Masking

ASP.NET Core 애플리케이션을 위한 고성능 데이터 마스킹 라이브러리입니다. 특히 여행 프로젝트에서 개인정보 보호를 위한 마스킹 처리에 최적화되어 있습니다.

https://claude.ai/public/artifacts/f5fc5a01-11ec-40be-aad9-619440f2de38

## 주요 기능

- **다양한 마스킹 지원**: 이메일, 전화번호, 이름, 여권번호, 신용카드 번호 등
- **두 가지 사용 방식**:
  - 직접 함수 호출 방식
  - Attribute 선언 방식
- **성능 최적화**:
  - 캐싱 메커니즘
  - 표현식 트리를 활용한 동적 메서드 생성
  - 병렬 처리 지원
- **확장성**: 커스텀 마스킹 로직 쉽게 추가 가능

## 설치

NuGet 패키지 관리자를 통해 설치할 수 있습니다:

```bash
dotnet add package Krdn.Masking
```

또는 Package Manager Console을 사용:

```powershell
Install-Package Krdn.Masking
```

## 사용 방법

### 1. 직접 함수 호출 방식

```csharp
using Krdn.Masking.Services;

// 서비스 인스턴스 생성
var maskingService = new MaskingService();

// 마스킹 적용
string maskedEmail = maskingService.MaskEmail("honggildong@example.com");     // ho*********@example.com
string maskedPhone = maskingService.MaskPhoneNumber("010-1234-5678");        // 010-****-5678
string maskedName = maskingService.MaskName("홍길동");                        // 홍**
string maskedCard = maskingService.MaskCreditCard("1234-5678-9012-3456");    // 1234-5678-****-3456
string maskedPassport = maskingService.MaskPassport("M12345678");            // M1******8
```

### 2. 문자열 확장 메서드 사용

```csharp
using Krdn.Masking.Extensions;

string email = "honggildong@example.com";
string maskedEmail = email.MaskEmail(); // ho*********@example.com

string phone = "010-1234-5678";
string maskedPhone = phone.MaskPhoneNumber(); // 010-****-5678
```

### 3. Attribute 선언 방식

```csharp
using Krdn.Masking.Attributes;
using Krdn.Masking.Extensions;

// 모델 클래스 정의
public class CustomerInfo
{
    public int Id { get; set; }
    
    [NameMasking]
    public string Name { get; set; }
    
    [EmailMasking(visibleCharCount: 3)] // 첫 3글자 표시
    public string Email { get; set; }
    
    [PhoneMasking]
    public string PhoneNumber { get; set; }
    
    [CreditCardMasking]
    public string CreditCardNumber { get; set; }
    
    [PassportMasking]
    public string PassportNumber { get; set; }
}

// 사용 예시
var customer = new CustomerInfo
{
    Id = 1,
    Name = "홍길동",
    Email = "hong.gildong@example.com",
    PhoneNumber = "010-1234-5678",
    CreditCardNumber = "1234-5678-9012-3456",
    PassportNumber = "M12345678"
};

// 마스킹 적용
var maskedCustomer = customer.Mask();
```

## 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다.
