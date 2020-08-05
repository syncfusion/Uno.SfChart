using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart range area segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="RangeAreaSeries"/>
    public partial class RangeAreaSegment : ChartSegment
    {
        #region Fields

        #region Private Fields

        private List<ChartPoint> AreaPoints;

        private bool isHighLow;

        private Path segPath;

        private Brush hiValueInterior, loValueInterior;

        /// <summary>
        /// Gets or sets the high(top) value bind with this segment.
        /// </summary>
        private double _high;

        /// <summary>
        /// Gets or sets the low(bottom) value bind with this segment.
        /// </summary>
        private double _low;

        #endregion

        #endregion

        #region Constructor

        public RangeAreaSegment()
        {

        }

        /// <summary>
        /// Called when instance created for rangeAreaSegments
        /// </summary>
        /// <param name="AreaPoints"></param>
        /// <param name="isHighLow"></param>
        /// <param name="series"></param>
        [Obsolete("Use RangeAreaSegment(List<ChartPoint> AreaPoints, bool isHighLow, RangeAreaSeries series)")]
        public RangeAreaSegment(List<Point> AreaPoints, bool isHighLow, RangeAreaSeries series)
        {
            this.isHighLow = isHighLow;
        }

        public RangeAreaSegment(List<ChartPoint> AreaPoints, bool isHighLow, RangeAreaSeries series)
        {
            this.isHighLow = isHighLow;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the actual color used to paint the interior of the segment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush ActualInterior
        {
            get
            {
                return isHighLow
                    ? HighValueInterior : LowValueInterior;
            }
        }

        /// <summary>
        /// Gets or sets the high value interior brush of this segment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush HighValueInterior
        {
            get
            {
                return hiValueInterior == null
                    ? this.Interior : hiValueInterior;
            }
            set
            {
                if (hiValueInterior != value)
                {
                    hiValueInterior = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        /// <summary>
        /// Gets or sets the low value interior brush of this segment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush LowValueInterior
        {
            get
            {
                return loValueInterior == null
                    ? this.Interior : loValueInterior;
            }
            set
            {
                if (loValueInterior != value)
                {
                    loValueInterior = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        public double High
        {
            get { return _high; }
            set
            {
                _high = value;
                OnPropertyChanged("High");
            }
        }

        public double Low
        {
            get { return _low; }
            set
            {
                _low = value;
                OnPropertyChanged("Low");
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="AreaPoints"></param>
        [Obsolete("Use SetData(List<ChartPoint> AreaPoints)")]
        public override void SetData(List<Point> AreaPoints)
        {
            var areaPoints = new List<ChartPoint>();
            foreach (Point point in AreaPoints)
                areaPoints.Add(new ChartPoint(point.X, point.Y));
            this.AreaPoints = areaPoints;
            double X_MAX = AreaPoints.Max(x => x.X);
            double Y_MAX = AreaPoints.Max(y => y.Y);
            double X_MIN = AreaPoints.Min(x => x.X);
            double _Min = AreaPoints.Min(item => item.Y);
            double Y_MIN;
            if (double.IsNaN(_Min))
            {
                var yVal = AreaPoints.Where(item => !double.IsNaN(item.Y));
                Y_MIN = (!yVal.Any()) ? 0 : yVal.Min(item => item.Y);
            }
            else
            {
                Y_MIN = _Min;
            }
            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);
        }

        public override void SetData(List<ChartPoint> AreaPoints)
        {
            this.AreaPoints = AreaPoints;
            double X_MAX = AreaPoints.Max(x => x.X);
            double Y_MAX = AreaPoints.Max(y => y.Y);
            double X_MIN = AreaPoints.Min(x => x.X);
            double _Min = AreaPoints.Min(item => item.Y);
            double Y_MIN;
            if (double.IsNaN(_Min))
            {
                var yVal = AreaPoints.Where(item => !double.IsNaN(item.Y));
                Y_MIN = (!yVal.Any()) ? 0 : yVal.Min(item => item.Y);
            }
            else
            {
                Y_MIN = _Min;
            }
            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// returns UIElement
        /// </returns>
        public override UIElement CreateVisual(Size size)
        {
            segPath = new Path();
            segPath.Tag = this;
            SetVisualBindings(segPath);
            return segPath;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>returns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return segPath;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            PathFigure figure = new PathFigure();

            int startIndex = 0;
            int endIndex = AreaPoints.Count - 1;

            if (AreaPoints.Count > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(AreaPoints[0].X, AreaPoints[0].Y);

                for (int i = startIndex; i < AreaPoints.Count; i += 2)
                {
                    WindowsLinesegment lineSeg = new WindowsLinesegment();
                    lineSeg.Point = transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y);
                    figure.Segments.Add(lineSeg);
                }

                for (int i = endIndex; i >= 1; i -= 2)
                {
                    WindowsLinesegment lineSeg = new WindowsLinesegment();
                    lineSeg.Point = transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y);
                    figure.Segments.Add(lineSeg);

                }
                figure.IsClosed = true;
                Series.SeriesRootPanel.Clip = null;
            }
            PathGeometry segmentGeometry = new PathGeometry();
            segmentGeometry.Figures.Add(figure);
            segPath.Data = segmentGeometry;
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        public override void OnSizeChanged(Size size)
        {

        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method Implementation for set Binding to ChartSegments properties.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualInterior");
            element.SetBinding(Shape.FillProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualInterior");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
        }



        /// <summary>
        /// Called when Property changed 
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanged(string name)
        {
            if (name == "Interior")
                name = "ActualInterior";
            base.OnPropertyChanged(name);
        }

        #endregion

        internal override void Dispose()
        {
            if(segPath != null)
            {
                segPath.Tag = null;
                segPath = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
