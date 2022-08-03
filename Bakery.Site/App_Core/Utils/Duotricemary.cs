using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace Bakery.Utils
{

    /// <summary>
    /// 三十二进制
    /// </summary>
    /// <remarks>更多的方法可以创建，比如两个三十二进制数相减等等</remarks>
    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct Duotricemary
    {

        #region 字段和属性
        /// <summary>
        /// 代表字符串格式的三十二进制所有组成的字符
        /// </summary>
        const string CHARS = "0123456789ABCDEFGHJKLMNPQRTUVWXY";

        private string m_StringValue;
        /// <summary>
        /// 字符串格式的值
        /// </summary>
        /// <remarks></remarks>
        public string StringValue
        {
            get { return m_StringValue; }
            set { m_StringValue = value; }
        }

        private ulong? m_Int64Value;
        /// <summary>
        /// 十进制的值
        /// </summary>
        public ulong? Int64Value
        {
            get { return m_Int64Value; }
            set { m_Int64Value = value; }
        }
        #endregion

        #region 构造函数与创建实例的静态方法等
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stringValue"></param>
        public Duotricemary(string stringValue)
        {
            m_StringValue = stringValue;
            m_Int64Value = null;
            m_Int64Value = ToInt64(stringValue);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="intValue"></param>
        public Duotricemary(ulong intValue)
        {
            m_Int64Value = intValue;
            m_StringValue = null;
            m_StringValue = ToDuotricemaryString(intValue);
        }

        /// <summary>
        /// 通过字符串比如EG创建三十二进制的实例
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static Duotricemary FromString(string stringValue)
        {
            return new Duotricemary(stringValue);
        }

        /// <summary>
        /// 通过非负整形创建三十二进制的实例
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static Duotricemary FromInt64(ulong intValue)
        {
            return new Duotricemary(intValue);
        }

        #endregion

        #region 转换方法

        /// <summary>
        /// 转换为无符号整型类型
        /// </summary>
        /// <returns></returns>
        public ulong ToInt64()
        {
            if (!this.Int64Value.HasValue)
            {
                this.Int64Value = this.ToInt64(this.StringValue);
            }
            return this.Int64Value.Value;
        }

        /// <summary>
        /// 将字符串表示的三十二进制数字转换为整形
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        private ulong ToInt64(string stringValue)
        {
            ulong value = 0;
            if (!string.IsNullOrEmpty(stringValue))
            {
                int j = 0;
                for (int i = stringValue.Length; i > 0; i--, j++)
                {
                    char c = stringValue[i - 1];
                    int index = Duotricemary.CHARS.IndexOf(c);
                    if (index == -1)
                    {
                        throw new FormatException("Unrecognizable duotricemary format.");
                    }
                    value += (ulong)(Math.Pow(32, j) * (index));

                }
            }
            return value;
        }

        /// <summary>
        /// 将十进制非负Int值转换为三十二进制
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string ToDuotricemaryString(ulong intValue)
        {
            if (intValue < 32)
            {
                return intValue.ToString();
            }
            else
            {
                Stack<char> chars = new Stack<char>();
                ulong temp = intValue;
                int n = 0;
                while (temp > 0)
                {
                    n = (int)(temp % 32);
                    temp = (ulong)(temp / 32);
                    chars.Push(Duotricemary.CHARS[n]);
                }
                StringBuilder sb = new StringBuilder();
                int totalChars = chars.Count;
                for (int j = 0; j < totalChars; j++)
                {
                    sb.Append(chars.Pop());
                }
                return sb.ToString(); ;
            }
        }
        #endregion

        #region 操作符
        ///// <summary>
        ///// 将ulong类型强制转换为Duotricemary
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static explicit operator Duotricemary(ulong value)
        //{
        //    return new Duotricemary(value);
        //}

        /// <summary>
        /// 将ulong类型隐式转换为Duotricemary
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Duotricemary(ulong value)
        {
            return new Duotricemary(value);
        }

        ///// <summary>
        ///// 将字符串类型强制转换为Duotricemary
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static explicit operator Duotricemary(string value)
        //{
        //    return new Duotricemary(value);
        //}

        /// <summary>
        /// 将字符串类型隐式转换为Duotricemary
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Duotricemary(string value)
        {
            return new Duotricemary(value);
        }

        /// <summary>
        /// 加
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Duotricemary operator +(Duotricemary d, ulong value)
        {
            ulong i = d.ToInt64();
            return new Duotricemary(i + value);
        }

        /// <summary>
        /// 减
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Duotricemary operator -(Duotricemary d, ulong value)
        {
            ulong i = d.ToInt64();
            return new Duotricemary(i - value);
        }

        #endregion

        #region 方法重载
        /// <summary>
        /// 重载ToStirng方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.StringValue))
            {
                if (this.Int64Value.HasValue)
                {
                    this.StringValue = this.ToDuotricemaryString(this.Int64Value.Value);
                }
                else
                {
                    this.Int64Value = 0;
                    this.StringValue = "0";
                }
            }
            return this.StringValue;
        }
        #endregion
    }
}
