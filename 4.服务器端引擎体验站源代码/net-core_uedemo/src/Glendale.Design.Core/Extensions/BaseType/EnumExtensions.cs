using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Glendale.Design
{
    /// <summary>
    /// 枚举扩展方法
    /// </summary>
    public static partial class EnumExtensions
    {
        /// <summary>
        /// 根据枚举int值获取枚举名称
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="status">枚举值</param>
        /// <returns></returns>
        public static string GetEnumName<T>(this int status)
        {
            return Enum.GetName(typeof(T), status);
        }


        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(this Enum obj)
        {
            return GetDescription(obj, false);
        }

        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <param name="isTop">是否改变为返回该类、枚举类型的头 Description 属性，而不是当前的属性或枚举变量值的 Description 属性</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(this Enum obj, bool isTop)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            Type enumType = obj.GetType();
            DescriptionAttribute dna;
            if (isTop)
            {
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute));
            }
            else
            {
                FieldInfo fi = enumType.GetField(System.Enum.GetName(enumType, obj));
                dna = (DescriptionAttribute)Attribute.GetCustomAttribute(
                   fi, typeof(DescriptionAttribute));
            }
            if ((dna != null)
                && (string.IsNullOrEmpty(dna.Description) == false))
            {
                return dna.Description;
            }
            return obj.ToString();
        }
    }
}