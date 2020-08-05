using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart Spline segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="SplineSeries"/>   
    public partial class SplineSegment:ChartSegment
    {
        #region Fields

        #region Private Fields

        ChartPoint Point1;

        ChartPoint Point2;

        ChartPoint Point3;

        ChartPoint Point4;

        Path segPath;

        private ContentControl control;

        private DataTemplate customTemplate;

        private PathGeometry data;

        private double _yData, _y1Data, _xData, _x1Data;

        private Point p1, q1, q2, p2;

        private bool isSegmentUpdated;

        #endregion

        #endregion

        #region Constructor

        public SplineSegment()
        {

        }

        /// <summary>
        /// Called when instance created for SplineSegment
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="series"></param>
        [Obsolete("Use SplineSegment(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4, SplineSeries series)")]
        public SplineSegment(Point point1, Point point2, Point point3, Point point4, SplineSeries series)
        {
            base.Series = series;
            customTemplate = series.CustomTemplate;
        }

        public SplineSegment(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4, SplineSeries series)
        {
            base.Series = series;
            customTemplate = series.CustomTemplate;
        }

        [Obsolete("Use SplineSegment(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4, ChartSeriesBase series)")]
        public SplineSegment(Point point1, Point point2, Point point3, Point point4, ChartSeriesBase series)
        {

        }

        public SplineSegment(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4, ChartSeriesBase series)
        {

        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the x1 point of this segment.
        /// </summary>
        public double X1 { get; set; }

        /// <summary>
        /// Gets or sets the x2 point of this segment.
        /// </summary>
        public double X2 { get; set; }

        /// <summary>
        /// Gets or sets the y1 point of this segment.
        /// </summary>
        public double Y1 { get; set; }

        /// <summary>
        /// Gets or sets the y2 point of this segment.
        /// </summary>
        public double Y2 { get; set; }


        /// <summary>
        /// Gets or sets the start point of the bezier segment.
        /// </summary>
        public Point P1
        {
            get { return p1; }
            set
            {
                p1 = value;
                OnPropertyChanged("P1");
            }
        }


        /// <summary>
        /// Gets or sets the first control point for the bezier segment.
        /// </summary>
        public Point Q1
        {
            get { return q1; }
            set
            {
                q1 = value;
                OnPropertyChanged("Q1");
            }
        }


        /// <summary>
        /// Gets or sets the second control point for the bezier segment.
        /// </summary>
        public Point Q2
        {
            get { return q2; }
            set
            {
                q2 = value;
                OnPropertyChanged("Q2");
            }
        }


        /// <summary>
        /// Gets or sets the end point for the bezier segment.
        /// </summary>
        public Point P2
        {
            get { return p2; }
            set
            {
                p2 = value;
                OnPropertyChanged("P2");
            }
        }


        /// <summary>
        /// Gets or sets x1 value.
        /// </summary>
        public double X1Data
        {
            get { return _x1Data; }
            set
            {
                _x1Data = value;
                OnPropertyChanged("X1Data");
            }
        }

        /// <summary>
        /// Gets or sets the data point value, bind with x for this segment.
        /// </summary>
        public double XData
        {
            get { return _xData; }
            set
            {
                _xData = value;
                OnPropertyChanged("XData");
            }
        }


        /// <summary>
        /// Gets or sets the data point value, bind with y for this segment.
        /// </summary>
        public double YData
        {
            get { return _yData; }
            set
            {
                _yData = value;
                OnPropertyChanged("YData");
            }
        }


        /// <summary>
        /// Gets or sets the y1 value.
        /// </summary>
        public double Y1Data
        {
            get { return _y1Data; }
            set
            {
                _y1Data = value;
                OnPropertyChanged("Y1Data");
            }
        }


        /// <summary>
        /// Gets or sets the segment geometry for this segment.
        /// </summary>
        public PathGeometry Data
        {
            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged("Data");
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods


        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        [Obsolete("Use SetData(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4)")]
        public override void SetData(Point point1, Point point2, Point point3, Point point4)
        {
            XData = point1.X;
            X1Data = point4.X;
            YData = point1.Y;
            Y1Data = point4.Y;
            Point1 = new ChartPoint(point1.X, point1.Y);
            Point2 = new ChartPoint(point2.X, point2.Y);
            Point3 = new ChartPoint(point3.X, point2.Y);
            Point4 = new ChartPoint(point4.X, point4.Y);
            XRange = new DoubleRange(point1.X, point4.X);
            //WP-581 - YRange differ when using NaN 
            YRange = GetYRange(point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y, point4.X, point4.Y);
        }

        public override void SetData(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4)
        {
            XData = point1.X;
            X1Data = point4.X;
            YData = point1.Y;
            Y1Data = point4.Y;
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
            Point4 = point4;
            XRange = new DoubleRange(point1.X, point4.X);
            //WP-581 - YRange differ when using NaN 
            YRange = GetYRange(point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y, point4.X, point4.Y);
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>

        public override UIElement CreateVisual(Size size)
        {
            if (customTemplate == null)
            {
                segPath = new Path();
                segPath.Tag = this;
                SetVisualBindings(segPath);
                return segPath;
            }
            control = new ContentControl { Content = this, Tag = this, ContentTemplate = customTemplate };
            return control;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            if (customTemplate == null)
                return segPath;
            return control;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>

        public override void Update(IChartTransformer transformer)
        {
            if (transformer != null)
            {
                if (isSegmentUpdated)
                    Series.SeriesRootPanel.Clip = null;
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double left = xIsLogarithmic ? Math.Log(Point1.X, xBase) : Point1.X;
                double right = xIsLogarithmic ? Math.Log(Point4.X, xBase) : Point4.X;

                if ((left <= xEnd && right >= xStart))
                {
                    PathFigure figure = new PathFigure();
                    BezierSegment bezierSeg = new BezierSegment();
                    PathGeometry segGeometry = new PathGeometry();

                    P1 = figure.StartPoint = transformer.TransformToVisible(Point1.X, Point1.Y);
                    Q1 = bezierSeg.Point1 = transformer.TransformToVisible(Point2.X, Point2.Y);
                    Q2 = bezierSeg.Point2 = transformer.TransformToVisible(Point3.X, Point3.Y);
                    P2 = bezierSeg.Point3 = transformer.TransformToVisible(Point4.X, Point4.Y);
                    figure.Segments.Add(bezierSeg);
                    segGeometry.Figures = new PathFigureCollection() { figure };

                    var path = this.segPath;
                    if (path != null) path.Data = segGeometry;
                    else Data = segGeometry;
                }
                else
                {
                    if (segPath != null)
                        this.segPath.Data = null;
                    else if (Data != null)
                        Data = null;
                }
                isSegmentUpdated = true;
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>

        public override void OnSizeChanged(Size size)
        {

        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// return doubleRange values from the given values
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected internal DoubleRange GetYRange(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            DoubleRange range = DoubleRange.Union(new double[] { y1, y2, y3, y4 });

            double cy = 3 * (y2 - y1);

            double by = 3 * (y3 - y3) - cy;

            double ay = y4 - y1 - by - cy;

            double r1, r2;

            if (ChartMath.SolveQuadraticEquation(3 * ay, 2 * by, cy, out r1, out r2))
            {
                if (r1 >= 0 && r1 <= 1)
                {
                    double y = ay * r1 * r1 * r1 + by * r1 * r1 + cy * r1 + y1;
                    range = DoubleRange.Union(range, y);
                }

                if (r2 >= 0 && r2 <= 1)
                {
                    double y = ay * r2 * r2 * r2 + by * r2 * r2 + cy * r2 + y1;
                    range = DoubleRange.Union(range, y);
                }
            }

            return range;
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
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
            if (this is TrendlineSegment)
            {
                DoubleCollection collection = this.StrokeDashArray;
                if (collection != null)
                {
                    DoubleCollection doubleCollection = new DoubleCollection();
                    foreach (double value in collection)
                    {
                        doubleCollection.Add(value);
                    }

                    element.StrokeDashArray = doubleCollection;
                }
            }
            else
            {
                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("StrokeDashArray");
                BindingOperations.SetBinding(this, SplineSegment.StrokeDashArrayProperty, binding);
            }
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
