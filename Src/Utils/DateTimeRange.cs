using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Defines memebers and methods to handle DateTime type range in <see cref="ChartAxis"/>.
    /// </summary>
   
    public struct DateTimeRange :IEquatable<DateTimeRange>
    {
        #region Members
        /// <summary>
        /// Initilaizes m_start
        /// </summary>
        private DateTime m_start;

        /// <summary>
        /// Initilaizes m_end
        /// </summary>
        private DateTime m_end;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRange"/> struct.
        /// </summary>
        /// <param name="rangeStart">The range start.</param>
        /// <param name="rangeEnd">The range end.</param>
        public DateTimeRange(DateTime rangeStart, DateTime rangeEnd)
        {
            m_start = rangeStart;
            m_end = rangeEnd;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
       
        public bool IsEmpty
        {
            get
            {
                return m_end <= m_start;
            }
        }

        /// <summary>
        /// Gets the start.
        /// </summary>
        /// <value>The start.</value>
      
        public DateTime Start
        {
            get
            {
                return m_start;
            }
        }

        /// <summary>
        /// Gets the end.
        /// </summary>
        /// <value>The end value.</value>
       
        public DateTime End
        {
            get
            {
                return m_end;
            }
        }
        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if(!(obj is DateTimeRange))
            {
                return false;
            }

            return Equals((DateTimeRange)obj);
        }

        public bool Equals(DateTimeRange other)
        {
            if(Start != other.Start)
            {
                return false;
            }

            return End == other.End;
        }

        public static bool operator ==(DateTimeRange point1, DateTimeRange point2)
        {
            return point1.Equals(point2);
        }

        public static bool operator !=(DateTimeRange point1, DateTimeRange point2)
        {
            return !point1.Equals(point2);
        }

        #endregion
    }
}
