using System;
using System.Collections.Generic;
using Krdn.Masking.Attributes;

namespace Krdn.Masking.Services
{
    /// <summary>
    /// 마스킹 제공자 인터페이스 - 다양한 데이터 타입에 대한 마스킹 구현 제공
    /// </summary>
    public interface IMaskingProvider
    {
        /// <summary>
        /// 이메일 주소 마스킹
        /// </summary>
        /// <param name="email">원본 이메일</param>
        /// <param name="visibleCharCount">ID 부분에서 표시할 문자 수</param>
        /// <returns>마스킹된 이메일</returns>
        string MaskEmail(string email, int visibleCharCount = 2);
        
        /// <summary>
        /// 전화번호 마스킹
        /// </summary>
        /// <param name="phoneNumber">원본 전화번호</param>
        /// <returns>마스킹된 전화번호</returns>
        string MaskPhoneNumber(string phoneNumber);
        
        /// <summary>
        /// 이름 마스킹
        /// </summary>
        /// <param name="name">원본 이름</param>
        /// <param name="visibleCharCount">표시할 문자 수</param>
        /// <returns>마스킹된 이름</returns>
        string MaskName(string name, int visibleCharCount = 1);
        
        /// <summary>
        /// 신용카드 번호 마스킹
        /// </summary>
        /// <param name="cardNumber">원본 카드번호</param>
        /// <returns>마스킹된 카드번호</returns>
        string MaskCreditCard(string cardNumber);
        
        /// <summary>
        /// 여권 번호 마스킹
        /// </summary>
        /// <param name="passportNumber">원본 여권번호</param>
        /// <returns>마스킹된 여권번호</returns>
        string MaskPassport(string passportNumber);
        
        /// <summary>
        /// 속성에 지정된 마스킹 속성을 사용하여 문자열 마스킹
        /// </summary>
        /// <param name="value">마스킹할 값</param>
        /// <param name="attribute">적용할 마스킹 속성</param>
        /// <returns>마스킹된 값</returns>
        string MaskWithAttribute(string value, MaskingAttribute attribute);
    }
}
