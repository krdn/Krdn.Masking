# Claude Code Setup for Krdn.Masking

## Project Information
- **Type**: .NET Class Library (.NET Standard 2.0) with ASP.NET Core Sample App
- **Purpose**: High-performance data masking library for personal information protection
- **Framework**: .NET Standard 2.0 (library), .NET 8.0 (sample app and tests)

## Development Environment Setup

### Prerequisites
- .NET 8.0 SDK installed at `/home/krdnn/.dotnet/`
- PATH configured to include .NET CLI: `export PATH="$PATH:/home/krdnn/.dotnet"`

### Build Commands
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run sample application
cd src/Krdn.Masking.Sample
dotnet run
```

### Project Structure
- `src/Krdn.Masking/` - Main library (.NET Standard 2.0)
- `src/Krdn.Masking.Sample/` - Sample ASP.NET Core application (.NET 8.0)
- `tests/Krdn.Masking.Tests/` - Unit tests (.NET 8.0)

### Key Features
- Email, phone, name, credit card, and passport masking
- Attribute-based declarative masking
- High-performance expression tree compilation
- Parallel processing support
- Caching mechanisms

### Fixed Issues During Setup
1. Added `virtual` modifiers to `DefaultMaskingProvider` methods to enable inheritance
2. Added `partial` modifier to `TravelController` class for split file structure  
3. Fixed test assertion for email masking character count
4. **Enhanced Input Validation** (Latest Update):
   - Added comprehensive null/empty checks across all masking methods
   - Implemented proper exception handling with fallback to original values
   - Added argument validation in service methods (negative parameters)
   - Enhanced credit card masking to support 15-digit Amex cards
   - Improved passport masking with flexible pattern support
   - Added robust error handling in utility classes
   - Created 14 new validation tests covering edge cases

### Testing
All 26 tests pass successfully, covering:
- Attribute-based masking
- Service-based masking  
- Performance characteristics
- Extension methods
- Input validation and error handling
- Edge cases and malformed input
- Exception handling scenarios

### Sample Usage
The library provides two main usage patterns:
1. Direct service calls: `maskingService.MaskEmail("test@example.com")`
2. Attribute declarations: `[EmailMasking] public string Email { get; set; }`