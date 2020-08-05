using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    ///  Represents a resource wrapper for SfChart control.
    /// </summary>
    public partial class SfChartResourceWrapper
    {      

        /// <summary>
        /// Gets the culture based value to represent tooltip for zoom out option.
        /// </summary>
        public static string ZoomOut
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ZoomOut");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip for zoom in option.
        /// </summary>
        public static string ZoomIn
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "ZoomIn");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip for reset option.
        /// </summary>
        public static string Reset
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Reset");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip for rect zooming option.
        /// </summary>
        public static string BoxSelectionZoom
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "BoxSelectionZoom");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip for panning option.
        /// </summary>
        public static string Pan
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Pan");
            }
        }

        /// <summary>
        ///  Gets the culture based value to represent week. 
        /// </summary>
        public static string Week
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Week");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent W.
        /// </summary>
        public static string W
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "W");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent quarter.
        /// </summary>
        public static string Quarter
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Quarter");
            }
        }        

        /// <summary>
        /// Gets the culture based value to represent Q.
        /// </summary>
        public static string Q
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Q");
            }
        }
       
        /// <summary>
        /// Gets the culture based value to represent  tooltip's high.
        /// </summary>
        public static string High
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "High");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip's low.
        /// </summary>
        public static string Low
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Low");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip's open. 
        /// </summary>
        public static string Open
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Open");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip's close.
        /// </summary>
        public static string Close
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Close");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip's maximum.
        /// </summary>
        public static string Maximum
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Maximum");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip's minimum.
        /// </summary>
        public static string Minimum
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Minimum");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip's median.
        /// </summary>
        public static string Median
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Median");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip's Q1.
        /// </summary>
        public static string Q1
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Q1");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent tooltip's Q3.
        /// </summary>
        public static string Q3
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Q3");
            }
        }
        /// <summary>
        /// Gets the culture based value to represent tooltip's size.
        /// </summary>
        public static string Size
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Size");
            }
        }

        /// <summary>
        ///  Gets the culture based value to represent tooltip's Yvalue.
        /// </summary>
        public static string YValue
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "YValue");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent legend Increase.
        /// </summary>
        public static string Increase
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Increase");
            }
        }

        /// <summary>
        ///  Gets the culture based value to represent legend decrease.
        /// </summary>
        public static string Decrease
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Decrease");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent legend total.
        /// </summary>
        public static string Total
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Total");
            }
        }       

        /// <summary>
        /// Gets the culture based value to represent file name.
        /// </summary>
        public static string FileName
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "FileName");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent print message.
        /// </summary>
        public static string PrintingExceptionMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "PrintingExceptionMessage");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent incompatible exeception.
        /// </summary>
        public static string AxisIncompatibleExceptionMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "AxisIncompatibleExceptionMessage");
            }
        }
        
        /// <summary>
        /// Gets the culture based value to represent print error message.
        /// </summary>
        public static string PrintErrorMessage
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "PrintErrorMessage");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent indicator signal line.
        /// </summary>
        public static string SignalLine
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "SignalLine");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent indicator upper line.
        /// </summary>
        public static string UpperLine
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "UpperLine");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent indicator lower line.
        /// </summary>
        public static string LowerLine
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "LowerLine");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent indicator period line.
        /// </summary>
        public static string PeriodLine
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "PeriodLine");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent indicator histogram.
        /// </summary>
        public static string Histogram
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Histogram");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent indicator MACD.
        /// </summary>
        public static string MACD
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "MACD");
            }
        }

        /// <summary>
        /// Gets the culture based value to represent segments grouping last legend label.
        /// </summary>
        internal static string Others
        {
            get
            {
                return SR.GetString(CultureInfo.CurrentUICulture, "Others");
            }
        }
    }
}


