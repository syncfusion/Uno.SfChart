using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Defines a custom DoubleRange data type for<see cref="SfChart"/> library.
    /// </summary>
   
    public struct DoubleRange
    {
        #region Members
        /// <summary>
        /// Initializes c_empty
        /// </summary>
        private static readonly DoubleRange c_empty = new DoubleRange(double.NaN, double.NaN);
        private bool m_isempty;

        /// <summary>
        /// Initializes m_start
        /// </summary>
        private double m_start;

        /// <summary>
        /// Initializes m_end
        /// </summary>
        private double m_end;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the Empty value
        /// </summary>
       
        public static DoubleRange Empty
        {
            get
            {
                return c_empty;
            }
        }

        /// <summary>
        /// Gets the Start value
        /// </summary>
     
        public double Start
        {
            get
            {
                return m_start;
            }
        }

        /// <summary>
        /// Gets the End value
        /// </summary>
       
        public double End
        {
            get
            {
                return m_end;
            }
        }

        /// <summary>
        /// Gets the Delta value
        /// </summary>
        
        public double Delta
        {
            get
            {
                return m_end - m_start;
            }
        }

        /// <summary>
        /// Gets the median.
        /// </summary>
        /// <value>The median.</value>
     
        public double Median
        {
            get
            {
                return (m_start + m_end) / 2d;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsEmpty 
        /// </summary>
        
        public bool IsEmpty
        {
            get
            {
                return m_isempty;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleRange"/> struct.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        public DoubleRange(double start, double end)
        {
            if (!double.IsNaN(start) && !double.IsNaN(end))
            {
                this.m_isempty = false;
            }
            else
            {
                this.m_isempty = true;
            }

            if (start > end)
            {
                m_start = end;
                m_end = start;
            }
            else
            {
                m_start = start;
                m_end = end;
            }
        }
        #endregion

        #region Operators
        /// <summary>
        /// Union operator
        /// </summary>
        /// <param name="leftRange">First double range</param>
        /// <param name="rightRange">Second double range</param>
        /// <returns>The Union value</returns>
        public static DoubleRange operator +(DoubleRange leftRange, DoubleRange rightRange)
        {
            return Union(leftRange, rightRange);
        }

        /// <summary>
        /// Union operator
        /// </summary>
        /// <param name="range">First double range</param>
        /// <param name="value">Second double range</param>
        /// <returns>The Union value</returns>
        public static DoubleRange operator +(DoubleRange range, double value)
        {
            return Union(range, value);
        }

        /// <summary>
        /// The operator
        /// </summary>
        /// <param name="range">The DoubleRange </param>
        /// <param name="value">The double value</param>
        /// <returns>The range value</returns>
        public static bool operator >(DoubleRange range, double value)
        {
            return range.m_start > value;
        }

        /// <summary>
        /// Return bool value from the given DoubleRange
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator >(DoubleRange range, DoubleRange value)
        {
            return range.m_start > value.m_start && range.m_end > value.m_end;
        }

        /// <summary>
        /// return Bool value from doublerange
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator <(DoubleRange range, DoubleRange value)
        {
            return range.m_start < value.m_start && range.m_end < value.m_end;
        }


        /// <summary>
        /// The operator
        /// </summary>
        /// <param name="range">The DoubleRange </param>
        /// <param name="value">The double value</param>
        /// <returns>The range value</returns>
        public static bool operator <(DoubleRange range, double value)
        {
            return range.m_end < value;
        }

        /// <summary>
        /// The operator method
        /// </summary>
        /// <param name="leftRange">The left DoubleRange</param>
        /// <param name="rightRange">The right DoubleRange</param>
        /// <returns>The left range</returns>
        public static bool operator ==(DoubleRange leftRange, DoubleRange rightRange)
        {
            return leftRange.Equals(rightRange);
        }

        /// <summary>
        /// The operator method
        /// </summary>
        /// <param name="leftRange">The left range</param>
        /// <param name="rightRange">The right range</param>
        /// <returns>The inverse left range</returns>
        public static bool operator !=(DoubleRange leftRange, DoubleRange rightRange)
        {
            return !leftRange.Equals(rightRange);
        }
        #endregion

        #region Public methods
        

        /// <summary>
        /// Create range by array of double.
        /// </summary>
        /// <param name="values">The values</param>
        /// <returns>The DoubleRange</returns>
        public static DoubleRange Union(params double[] values)
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (double val in values)
            {
                if (double.IsNaN(val))
                {
                    min = val;
                }
                else
                if (min > val)
                {
                    min = val;
                }

                if (max < val)
                {
                    max = val;
                }
            }

            return new DoubleRange(min, max);
        }

        /// <summary>
        /// Unions the specified left range with right range.
        /// </summary>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns>The DoubleRange</returns>
        public static DoubleRange Union(DoubleRange leftRange, DoubleRange rightRange)
        {
            if (leftRange.IsEmpty)
            {
                return rightRange;
            }
            else if (rightRange.IsEmpty)
            {
                return leftRange;
            }

            return new DoubleRange(Math.Min(leftRange.m_start, rightRange.m_start), Math.Max(leftRange.m_end, rightRange.m_end));
        }

        /// <summary>
        /// Unions the specified range with value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The DoubleRange</returns>
        public static DoubleRange Union(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return new DoubleRange(value, value);
            }

            return new DoubleRange(Math.Min(range.m_start, value), Math.Max(range.m_end, value));
        }

        /// <summary>
        /// Scales the specified range by value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The DoubleRange</returns>
        public static DoubleRange Scale(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return range;
            }

            return new DoubleRange(range.m_start - value * range.Delta, range.m_end + value * range.Delta);
        }

        /// <summary>
        /// Offsets the specified range by value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        /// <returns>The DoubleRange</returns>
        public static DoubleRange Offset(DoubleRange range, double value)
        {
            if (range.IsEmpty)
            {
                return range;
            }

            return new DoubleRange(range.m_start + value, range.m_end + value);
        }

        /// <summary>
        /// Excludes the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="excluder">The excluder.</param>
        /// <param name="leftRange">The left range.</param>
        /// <param name="rightRange">The right range.</param>
        /// <returns>True if empty</returns>
        public static bool Exclude(DoubleRange range, DoubleRange excluder, out DoubleRange leftRange, out DoubleRange rightRange)
        {
            leftRange = DoubleRange.Empty;
            rightRange = DoubleRange.Empty;

            if (!(range.IsEmpty || excluder.IsEmpty))
            {
                if (excluder.m_end < range.m_start)
                {
                    if (excluder.m_end > range.m_start)
                    {
                        leftRange = new DoubleRange(excluder.m_start, range.m_start);
                    }
                    else
                    {
                        leftRange = excluder;
                    }
                }

                if (excluder.m_end > range.m_end)
                {
                    if (excluder.m_start < range.m_end)
                    {
                        rightRange = new DoubleRange(range.m_end, excluder.m_end);
                    }
                    else
                    {
                        rightRange = excluder;
                    }
                }
            }

            return !(leftRange.IsEmpty && rightRange.IsEmpty);
        }

        /// <summary>
        /// Checks whether intersection region of two ranges is not empty.
        /// </summary>
        /// <param name="range">the DoubleRange</param>
        /// <returns><b>true</b> if  intersection is not empty</returns>
        public bool Intersects(DoubleRange range)
        {
            if (this.IsEmpty || this.IsEmpty)
            {
                return false;
            }

            return this.Inside(range.m_start) || this.Inside(range.m_end) || range.Inside(this.m_start) || range.Inside(this.m_end);
        }

        /// <summary>
        /// Checks whether intersection region of two ranges is not empty.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <returns> true if  intersection is not empty</returns>
        public bool Intersects(double start, double end)
        {
            return this.Intersects(new DoubleRange(start, end));
        }

        /// <summary>
        /// Checks whether the given value is inside the axis range
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if value is inside</returns>
        public bool Inside(double value)
        {
            if (this.IsEmpty)
            {
                return false;
            }

            return (value <= m_end) && (value >= m_start);
        }

        /// <summary>
        /// Checks whether the given range is inside the axis range
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>True is range is inside</returns>
        public bool Inside(DoubleRange range)
        {
            if (this.IsEmpty)
            {
                return false;
            }

            return m_start <= range.m_start && m_end >= range.m_end;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is DoubleRange)
            {
                DoubleRange range = (DoubleRange)obj;
                return (m_start == range.m_start) && (m_end == range.m_end);
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return m_start.GetHashCode() ^ m_end.GetHashCode();
        }
        #endregion
    }
}
