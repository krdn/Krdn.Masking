using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Krdn.Masking.Attributes;
using Krdn.Masking.Services;
using Krdn.Masking.Utils;

namespace Krdn.Masking.Extensions
{
    /// <summary>
    /// 문자열 마스킹을 위한 확장 메서드
    /// </summary>
    public static class StringExtensions
    {
        private static readonly IMaskingProvider _defaultProvider = new DefaultMaskingProvider();
        
        /// <summary>
        /// 이메일 주소를 마스킹합니다.
        /// </summary>
        /// <param name="email">원본 이메일</param>
        /// <param name="visibleCharCount">표시할 문자 수</param>
        /// <returns>마스킹된 이메일</returns>
        public static string MaskEmail(this string email, int visibleCharCount = 2)
        {
            return _defaultProvider.MaskEmail(email, visibleCharCount);
        }
        
        /// <summary>
        /// 전화번호를 마스킹합니다.
        /// </summary>
        /// <param name="phoneNumber">원본 전화번호</param>
        /// <returns>마스킹된 전화번호</returns>
        public static string MaskPhoneNumber(this string phoneNumber)
        {
            return _defaultProvider.MaskPhoneNumber(phoneNumber);
        }
        
        /// <summary>
        /// 이름을 마스킹합니다.
        /// </summary>
        /// <param name="name">원본 이름</param>
        /// <param name="visibleCharCount">표시할 문자 수</param>
        /// <returns>마스킹된 이름</returns>
        public static string MaskName(this string name, int visibleCharCount = 1)
        {
            return _defaultProvider.MaskName(name, visibleCharCount);
        }
        
        /// <summary>
        /// 신용카드 번호를 마스킹합니다.
        /// </summary>
        /// <param name="cardNumber">원본 카드번호</param>
        /// <returns>마스킹된 카드번호</returns>
        public static string MaskCreditCard(this string cardNumber)
        {
            return _defaultProvider.MaskCreditCard(cardNumber);
        }
        
        /// <summary>
        /// 여권 번호를 마스킹합니다.
        /// </summary>
        /// <param name="passportNumber">원본 여권번호</param>
        /// <returns>마스킹된 여권번호</returns>
        public static string MaskPassport(this string passportNumber)
        {
            return _defaultProvider.MaskPassport(passportNumber);
        }
    }
    
    /// <summary>
    /// 객체 마스킹을 위한 확장 메서드
    /// </summary>
    public static class MaskingExtensions
    {
        private static readonly IMaskingProvider _defaultProvider = new DefaultMaskingProvider();
        
        /// <summary>
        /// 객체의 마스킹 속성이 지정된 모든 속성에 마스킹을 적용합니다.
        /// </summary>
        /// <typeparam name="T">마스킹할 객체 타입</typeparam>
        /// <param name="obj">마스킹할 객체</param>
        /// <returns>마스킹된 객체의 복사본</returns>
        public static T Mask<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;
                
            try
            {
                // 객체의 얕은 복사본 생성
                T clone = obj.ShallowCopy();
                if (clone == null)
                    return obj; // 복사 실패 시 원본 반환
                
                // 모든 마스킹 속성 가져오기
                var maskingAttributes = AttributeCache.GetMaskingAttributes(typeof(T));
                if (maskingAttributes == null)
                    return clone;
                
                // 각 마스킹 속성에 대해 마스킹 적용
                foreach (var entry in maskingAttributes)
                {
                    try
                    {
                        var property = entry.Key;
                        var attribute = entry.Value;
                        
                        if (property == null || attribute == null)
                            continue;
                        
                        // 표현식 트리로 생성된 빠른 게터와 세터 가져오기
                        var getter = ExpressionBuilder.GetPropertyGetter(property);
                        var setter = ExpressionBuilder.GetPropertySetter(property);
                        
                        if (getter == null || setter == null)
                            continue;
                        
                        // 속성 값 가져오기
                        var value = getter(clone) as string;
                        
                        if (!string.IsNullOrEmpty(value))
                        {
                            // 마스킹 적용 및 결과 설정
                            var maskedValue = attribute.Mask(value);
                            setter(clone, maskedValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 개별 속성 마스킹 실패 시 로그 기록 후 계속
                        System.Diagnostics.Debug.WriteLine($"속성 마스킹 오류: {ex.Message}");
                        continue;
                    }
                }
                
                return clone;
            }
            catch (Exception ex)
            {
                // 전체 마스킹 실패 시 원본 객체 반환
                System.Diagnostics.Debug.WriteLine($"객체 마스킹 오류: {ex.Message}");
                return obj;
            }
        }
        
        /// <summary>
        /// 객체 컬렉션의 모든 항목에 마스킹을 적용합니다.
        /// </summary>
        /// <typeparam name="T">마스킹할 객체 타입</typeparam>
        /// <param name="items">마스킹할 객체 컬렉션</param>
        /// <returns>마스킹된 객체의 컬렉션</returns>
        public static IEnumerable<T> MaskAll<T>(this IEnumerable<T> items) where T : class
        {
            if (items == null)
                return Enumerable.Empty<T>();
                
            return items.Select(item => item.Mask());
        }
        
        /// <summary>
        /// 객체 컬렉션의 모든 항목에 병렬로 마스킹을 적용합니다.
        /// </summary>
        /// <typeparam name="T">마스킹할 객체 타입</typeparam>
        /// <param name="items">마스킹할 객체 컬렉션</param>
        /// <returns>마스킹된 객체의 컬렉션</returns>
        public static IEnumerable<T> MaskAllParallel<T>(this IEnumerable<T> items) where T : class
        {
            if (items == null)
                return Enumerable.Empty<T>();
                
            return items.AsParallel().Select(item => item.Mask()).ToList();
        }
        
        /// <summary>
        /// 객체의 얕은 복사본을 만듭니다.
        /// </summary>
        /// <typeparam name="T">객체 타입</typeparam>
        /// <param name="source">원본 객체</param>
        /// <returns>객체의 복사본</returns>
        /// <exception cref="InvalidOperationException">복사에 실패한 경우</exception>
        private static T ShallowCopy<T>(this T source) where T : class
        {
            if (source == null)
                return null;
                
            try
            {
                // 새 인스턴스 생성을 위한 타입 정보
                Type type = typeof(T);
                
                // 매개변수 없는 생성자 확인
                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                {
                    throw new InvalidOperationException($"타입 {type.Name}에 매개변수 없는 생성자가 없습니다.");
                }
                
                // 새 인스턴스 생성
                T target = (T)Activator.CreateInstance(type);
                if (target == null)
                {
                    throw new InvalidOperationException($"타입 {type.Name}의 인스턴스 생성에 실패했습니다.");
                }
                
                // 모든 속성 복사
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite);
                    
                foreach (var property in properties)
                {
                    try
                    {
                        // 원본에서 값 읽기
                        var value = property.GetValue(source);
                        // 대상에 값 쓰기
                        property.SetValue(target, value);
                    }
                    catch (Exception ex)
                    {
                        // 개별 속성 복사 실패 시 로그 기록 후 계속
                        System.Diagnostics.Debug.WriteLine($"속성 {property.Name} 복사 오류: {ex.Message}");
                        continue;
                    }
                }
                
                return target;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"객체 복사 중 오류가 발생했습니다: {ex.Message}", ex);
            }
        }
    }
}
