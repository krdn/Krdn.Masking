using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Krdn.Masking.Attributes;

namespace Krdn.Masking.Utils
{
    /// <summary>
    /// 속성 정보 캐싱을 위한 유틸리티 클래스
    /// </summary>
    internal static class AttributeCache
    {
        // 타입별 속성 정보 캐시
        private static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, MaskingAttribute>> _cache =
            new ConcurrentDictionary<Type, Dictionary<PropertyInfo, MaskingAttribute>>();
            
        /// <summary>
        /// 지정된 타입의 모든 속성에 대해 마스킹 속성 정보를 검색하고 캐싱합니다.
        /// </summary>
        /// <param name="type">검색할 타입</param>
        /// <returns>속성 정보와 해당 마스킹 속성의 매핑</returns>
        /// <exception cref="ArgumentNullException">type이 null인 경우</exception>
        public static Dictionary<PropertyInfo, MaskingAttribute> GetMaskingAttributes(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
                
            return _cache.GetOrAdd(type, t =>
            {
                var result = new Dictionary<PropertyInfo, MaskingAttribute>();
                
                try
                {
                    // 해당 타입의 모든 public 속성을 가져옵니다
                    var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    if (properties == null)
                        return result;
                    
                    foreach (var property in properties)
                    {
                        try
                        {
                            // 속성이 null이거나 문자열 타입이 아니면 건너뜀
                            if (property == null || property.PropertyType != typeof(string))
                                continue;
                                
                            // MaskingAttribute가 지정된 속성을 찾습니다
                            var maskingAttribute = property
                                .GetCustomAttributes<MaskingAttribute>(true)
                                .FirstOrDefault();
                                
                            if (maskingAttribute != null)
                            {
                                result.Add(property, maskingAttribute);
                            }
                        }
                        catch (Exception ex)
                        {
                            // 개별 속성 처리 실패 시 로그 기록 후 계속
                            System.Diagnostics.Debug.WriteLine($"속성 {property?.Name} 처리 오류: {ex.Message}");
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 전체 처리 실패 시 로그 기록
                    System.Diagnostics.Debug.WriteLine($"타입 {t.Name} 속성 처리 오류: {ex.Message}");
                }
                
                return result;
            });
        }
        
        /// <summary>
        /// 캐시를 지웁니다.
        /// </summary>
        public static void ClearCache()
        {
            _cache.Clear();
        }
    }
}
