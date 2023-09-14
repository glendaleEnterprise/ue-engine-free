using System;

namespace Glendale.Design.DataAnnotations
{
    /// <summary>
    /// Set the decimal precision of a decimal sql data type
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DecimalPrecisionAttribute : Attribute
    {
        /// <summary>
        /// Specify the precision - the number of digits both left and right of the decimal
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Specify the scale - the number of digits to the right of the decimal
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// Set the decimal precision of a decimal sql data type
        /// </summary>
        /// <param name="precision">Specify the precision - the number of digits both left and right of the decimal</param>
        /// <param name="scale">Specify the scale - the number of digits to the right of the decimal</param>
        public DecimalPrecisionAttribute(int precision = 18, int scale = 2)
        {
            Precision = precision;
            Scale = scale;
            if (Precision < 1 || Precision > 38)
            {
                throw new InvalidOperationException("精度必须在1和38之间.");
            }
            if (Scale < 1 || Scale > 38)
            {
                throw new InvalidOperationException("刻度必须在1和38之间.");
            }
        }

        public DecimalPrecisionAttribute(int[] values)
        {
            Precision = values[0];
            Scale = values[1];
            if (Precision < 1 || Precision > 38)
            {
                throw new InvalidOperationException("精度必须在1和38之间.");
            }
            if (Scale < 1 || Scale > 38)
            {
                throw new InvalidOperationException("刻度必须在1和38之间.");
            }
        }
    }
}
