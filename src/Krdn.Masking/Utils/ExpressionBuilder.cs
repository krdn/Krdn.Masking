using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Krdn.Masking.Utils
{
    /// <summary>
    /// 표현식 트리를 사용한 빠른 속성 접근을 위한 유틸리티 클래스
    /// </summary>
    internal static class ExpressionBuilder
    {
        // 게터와 세터 델리게이트 캐시
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> _getterCache =
            new ConcurrentDictionary<PropertyInfo, Func<object, object>>();
            
        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> _setterCache =
            new ConcurrentDictionary<PropertyInfo, Action<object, object>>();
            
        /// <summary>
        /// 지정된 속성에 대한 게터 델리게이트를 가져옵니다.
        /// </summary>
        /// <param name="property">대상 속성</param>
        /// <returns>속성 값을 가져오는 델리게이트</returns>
        public static Func<object, object> GetPropertyGetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
                
            return _getterCache.GetOrAdd(property, prop =>
            {
                try
                {
                    // GetMethod가 없는 경우(예: 읽기 전용 속성)
                    if (prop.GetMethod == null)
                        return obj => null;
                        
                    // DeclaringType 검증
                    if (prop.DeclaringType == null)
                        return obj => null;
                        
                    // 파라미터: 속성 값을 가져올 개체 인스턴스
                    var objParam = Expression.Parameter(typeof(object), "obj");
                    
                    // 개체를 속성을 포함하는 타입으로 캐스팅
                    var typedObj = Expression.Convert(objParam, prop.DeclaringType);
                    
                    // 속성 값 가져오기
                    var propAccess = Expression.Property(typedObj, prop);
                    
                    // 속성 값을 object 타입으로 변환
                    var convertedProp = Expression.Convert(propAccess, typeof(object));
                    
                    // 람다 식 생성 및 컴파일
                    var lambda = Expression.Lambda<Func<object, object>>(convertedProp, objParam);
                    return lambda.Compile();
                }
                catch (Exception ex)
                {
                    // 예외 발생 시 널 반환 함수 제공
                    System.Diagnostics.Debug.WriteLine($"게터 생성 오류: {ex.Message}");
                    return obj => null;
                }
            });
        }
        
        /// <summary>
        /// 지정된 속성에 대한 세터 델리게이트를 가져옵니다.
        /// </summary>
        /// <param name="property">대상 속성</param>
        /// <returns>속성 값을 설정하는 델리게이트</returns>
        public static Action<object, object> GetPropertySetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
                
            return _setterCache.GetOrAdd(property, prop =>
            {
                try
                {
                    // SetMethod가 없는 경우(예: 읽기 전용 속성)
                    if (prop.SetMethod == null)
                        return (obj, value) => { };
                        
                    // DeclaringType 검증
                    if (prop.DeclaringType == null)
                        return (obj, value) => { };
                        
                    // 파라미터: 속성을 설정할 개체와 새 값
                    var objParam = Expression.Parameter(typeof(object), "obj");
                    var valueParam = Expression.Parameter(typeof(object), "value");
                    
                    // 개체를 속성을 포함하는 타입으로 캐스팅
                    var typedObj = Expression.Convert(objParam, prop.DeclaringType);
                    
                    // 값을 속성 타입으로 캐스팅
                    var typedValue = Expression.Convert(valueParam, prop.PropertyType);
                    
                    // 속성 설정
                    var propAssign = Expression.Call(typedObj, prop.SetMethod, typedValue);
                    
                    // 람다 식 생성 및 컴파일
                    var lambda = Expression.Lambda<Action<object, object>>(propAssign, objParam, valueParam);
                    return lambda.Compile();
                }
                catch (Exception ex)
                {
                    // 예외 발생 시 빈 액션 제공
                    System.Diagnostics.Debug.WriteLine($"세터 생성 오류: {ex.Message}");
                    return (obj, value) => { };
                }
            });
        }
        
        /// <summary>
        /// 캐시를 지웁니다.
        /// </summary>
        public static void ClearCache()
        {
            _getterCache.Clear();
            _setterCache.Clear();
        }
    }
}
