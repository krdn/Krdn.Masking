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
        public static Dictionary<PropertyInfo, MaskingAttribute> GetMaskingAttributes(Type type)
        {
            return _cache.GetOrAdd(type, t =>
            {
                var result = new Dictionary<PropertyInfo, MaskingAttribute>();
                
                // 해당 타입의 모든 public 속성을 가져옵니다
                var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                
                foreach (var property in properties)
                {
                    // 문자열 타입의 속성만 마스킹을 적용합니다
                    if (property.PropertyType == typeof(string))
                    {
                        // MaskingAttribute가 지정된 속성을 찾습니다
                        var maskingAttribute = property
                            .GetCustomAttributes<MaskingAttribute>(true)
                            .FirstOrDefault();
                            
                        if (maskingAttribute != null)
                        {
                            result.Add(property, maskingAttribute);
                        }
                    }
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
