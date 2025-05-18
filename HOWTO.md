# 프로젝트 실행 방법

이 문서는 Krdn.Masking 라이브러리와 샘플 프로젝트를 실행하는 방법을 설명합니다.

## 1. 필수 요구사항

- .NET 8.0 SDK 설치 ([다운로드](https://dotnet.microsoft.com/download/dotnet/8.0))
- Visual Studio 2022 또는 VS Code와 C# 확장 프로그램

## 2. 솔루션 빌드 및 실행

### Visual Studio 2022를 사용하는 경우

1. Krdn.Masking.sln 파일을 열기
2. 솔루션 빌드 (Ctrl+Shift+B)
3. 시작 프로젝트로 Krdn.Masking.Sample 선택
4. F5 키를 눌러 애플리케이션 실행

### 명령줄에서 실행하는 경우

```bash
# 솔루션 빌드
dotnet build

# 코어 라이브러리 테스트 실행
dotnet test tests/Krdn.Masking.Tests/Krdn.Masking.Tests.csproj

# 샘플 애플리케이션 실행
cd src/Krdn.Masking.Sample
dotnet run
```

실행 후 웹 브라우저에서 `https://localhost:5001/swagger` 주소로 접속하여 Swagger UI를 통해 API를 테스트할 수 있습니다.

## 3. 테스트 시나리오

### 3.1. 여행 패키지 조회

- GET `/api/Travel/packages` - 모든 여행 패키지 조회
- GET `/api/Travel/packages/{id}` - ID로 여행 패키지 조회

### 3.2. 예약 생성

POST `/api/Travel/bookings`를 사용하여 여행 예약을 생성할 수 있습니다. 요청 예시:

```json
{
  "customer": {
    "customerId": 1,
    "name": "홍길동",
    "email": "hong@example.com",
    "phoneNumber": "010-1234-5678",
    "passportNumber": "M12345678"
  },
  "package": {
    "packageId": 1
  },
  "numberOfTravelers": 2,
  "creditCardNumber": "1234-5678-9012-3456"
}
```

응답으로 마스킹된 데이터가 반환됩니다.

### 3.3. 예약 조회

- GET `/api/Travel/bookings` - 모든 예약 조회 (직접 함수 호출 방식으로 마스킹)
- GET `/api/Travel/bookings/{id}` - ID로 예약 조회 (확장 메서드 방식으로 마스킹)

### 3.4. 성능 테스트

GET `/api/Travel/performance-test?count=1000`를 사용하여 다양한 마스킹 방식의 성능을 비교할 수 있습니다.

## 4. NuGet 패키지 빌드 및 배포

```bash
# 릴리스 모드로 빌드
dotnet build -c Release

# NuGet 패키지 생성
cd src/Krdn.Masking
dotnet pack -c Release

# NuGet 패키지 게시 (API 키 필요)
dotnet nuget push bin/Release/Krdn.Masking.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

## 5. 사용 방법

### 5.1. 패키지 설치

```bash
dotnet add package Krdn.Masking
```

### 5.2. 직접 함수 호출 방식

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

### 5.3. 문자열 확장 메서드 방식

```csharp
using Krdn.Masking.Extensions;

string email = "honggildong@example.com";
string maskedEmail = email.MaskEmail(); // ho*********@example.com

string phone = "010-1234-5678";
string maskedPhone = phone.MaskPhoneNumber(); // 010-****-5678
```

### 5.4. Attribute 선언 방식

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
