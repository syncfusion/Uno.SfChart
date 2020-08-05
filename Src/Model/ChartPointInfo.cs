using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    ///ChartPointInfo contains information about the displaying series data points.
    /// </summary>
    [Windows.UI.Xaml.Data.Bindable]
    public partial class ChartPointInfo: INotifyPropertyChanged
    {
        #region Fields

        #region Internal Fields

        internal bool isOutlier;
        
        #endregion

        #region Private Fields

        private ChartSeriesBase series;
        private ChartAxis axis;
        private string valueX;
        private string valueY;
        private string high;
        private string low;
        private string open;
        private string close;
        private string upperLine;
        private string lowerLine;
        private string signalLine;
        private Brush interior;
        private Brush foreground;
        private Brush borderBrush;
        private PointCollection polygonPoints;
        private object item;
        private ObservableCollection<string> seriesvalues;
        private ChartAlignment verticalAlignment;
        private ChartAlignment horizontalAlignment;
        private string median;

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the  SeriesValues.
        /// </summary>
        public ObservableCollection<string> SeriesValues
        {
            get
            {
                if(seriesvalues==null)
                {
                    seriesvalues=new ObservableCollection<string>();
                }
                return seriesvalues;
            }
            set
            {
                value = seriesvalues;
            }
        }
        
        /// <summary>
        /// Gets or sets the associated series.
        /// </summary>
        public ChartSeriesBase Series
        {
            get
            {
                return series;
            }
            set
            {
                if (value != series)
                {
                    series = value;
                    OnPropertyChanged("Series");
                }
            }
        }

        /// <summary>
        /// Gets or sets the associated axis.
        /// </summary>
        public ChartAxis Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
            }
        }
        public object Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
                OnPropertyChanged("Item");
            }
        }
        /// <summary>
        /// Gets or sets the interior color of this data point.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Interior
        {
            get
            {
                return interior;
            }
            set
            {
                interior = value;
                OnPropertyChanged("Interior");
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of this data point.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Foreground
        {
            get
            {
                return foreground;
            }
            set
            {
                foreground = value;
                OnPropertyChanged("Foreground");
            }
        }

        /// <summary>
        /// Gets or sets the border color of this data point.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BorderBrush
        {
            get
            {
                return borderBrush;
            }
            set
            {
                borderBrush = value;
                OnPropertyChanged("BorderBrush");
            }
        }
        
        /// <summary>
        /// Gets or sets the x value
        /// </summary>
        public string ValueX
        {
            get
            {
                return valueX;
            }
            set
            {
                if (value != valueX)
                {
                    valueX = value;
                    OnPropertyChanged("ValueX");
                }
            }
        }

        /// <summary>
        /// Gets or sets the y value.
        /// </summary>
        public string ValueY
        {
            get
            {
                return valueY;
            }
            set
            {
                if (value != valueY)
                {
                    valueY = value;
                    OnPropertyChanged("ValueY");
                }
            }
        }


         /// <summary>
        /// Gets or sets the high value.
        /// </summary>
        public string High
        {
            get
            {
                return high;
            }
            set
            {
                if (value != high)
                {
                    high = value;
                    OnPropertyChanged("High");
                }
            }
        }

        /// <summary>
        /// Gets or sets the low value.
        /// </summary>
        public string Low
        {
            get
            {
                return low;
            }
            set
            {
                if (value != low)
                {
                    low = value;
                    OnPropertyChanged("Low");
                }
            }
        }

        /// <summary>
        /// Gets or sets the open value.
        /// </summary>
        public string Open
        {
            get
            {
                return open;
            }
            set
            {
                if (value != open)
                {
                    open = value;
                    OnPropertyChanged("Open");
                }
            }
        }


        /// <summary>
        /// Gets or sets the close value.
        /// </summary>
        public string Close
        {
            get
            {
                return close;
            }
            set
            {
                if (value != close)
                {
                    close = value;
                    OnPropertyChanged("Close");
                }
            }
        }

         /// <summary>
        /// Gets or sets the median value.
        /// </summary>
        public string Median
        {
            get
            {
                return median;
            }
            set
            {
                if (value != median)
                {
                    median = value;
                    OnPropertyChanged("Median");
                }
            }
        }
        /// <summary>
        /// Gets or sets the y value of the indicator segment.
        /// </summary>
        public string UpperLine
        {
            get
            {
                return upperLine;
            }
            set
            {
                if (value != upperLine)
                {
                    upperLine = value;
                    OnPropertyChanged("UpperLine");
                }
            }
        }

        /// <summary>
        /// Gets or sets the y value of the indicator segment.
        /// </summary>
        public string LowerLine
        {
            get
            {
                return lowerLine;
            }
            set
            {
                if (value != lowerLine)
                {
                    lowerLine = value;
                    OnPropertyChanged("LowerLine");
                }
            }
        }

        /// <summary>
        /// Gets or sets the y value of the signal line in the indicator.
        /// </summary>
        public string SignalLine
        {
            get
            {
                return signalLine;
            }
            set
            {
                if (value != signalLine)
                {
                    signalLine = value;
                    OnPropertyChanged("SignalLine");
                }
            }
        }

        /// <summary>
        /// Gets or sets the x initial coordinate.
        /// </summary>
        public double BaseX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the y initial coordinate
        /// </summary>
        public double BaseY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the x coordinate
        /// </summary>
        public double X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the y coordinate
        /// </summary>
        public double Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point collection.
        /// </summary>
        public PointCollection PolygonPoints
        {
            get
            {
                return polygonPoints;
            }
            set
            {
                polygonPoints = value;
                OnPropertyChanged("PolygonPoints");
            }
        }

        #endregion

        #region Internal Properties

        internal ChartAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set { verticalAlignment = value; }
        }

        internal ChartAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set { horizontalAlignment = value; }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Called when property changed
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #endregion
    }
}
