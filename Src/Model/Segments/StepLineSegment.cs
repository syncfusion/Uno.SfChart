using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart StepLine segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="StepLineSeries"/>
    public partial class StepLineSegment : ChartSegment
    {
        #region Fields

        #region Private Fields

        private Polyline poly;

        private ChartPoint pointStart;

        private ChartPoint pointEnd;

        private ChartPoint stepMidPoint;

        private DataTemplate customTemplate;

        private double _yData, _y1Data, _xData, _x1Data;

        private ContentControl control;

        private PointCollection points;

        bool isSegmentUpdated;

        private PointCollection stepPoints;

        private double x1, x2, y1, y2, x3, y3;

        #endregion

        #endregion
        
        #region Constructor

        public StepLineSegment()
        {

        }

        /// <summary>
        /// Called when instance created for SteplineSegment
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="stepPoint"></param>
        /// <param name="point2"></param>
        /// <param name="series"></param>
        [Obsolete("Use StepLineSegment(ChartPoint point1, ChartPoint stepPoint, ChartPoint point2, StepLineSeries series)")]
        public StepLineSegment(Point point1, Point stepPoint, Point point2, StepLineSeries series)
        {
            customTemplate = series.CustomTemplate;
        }

        public StepLineSegment(ChartPoint point1, ChartPoint stepPoint, ChartPoint point2, StepLineSeries series)
        {
            customTemplate = series.CustomTemplate;
        }

        #endregion

        #region Properties

#region Public Properties

        /// <summary>
        /// Gets or sets x1 point of this segment.
        /// </summary>
        public double X1
        {
            get { return x1; } 
            set 
            { 
                x1 = value;
                OnPropertyChanged("X1"); 
            }
        }

        /// <summary>
        /// Gets or sets the x2 point of this segment
        /// </summary>
        public double X2 
        { 
            get { return x2; }
            set
            {
                x2 = value; 
                OnPropertyChanged("X2");
            } 
        }

        /// <summary>
        /// Gets or sets the y1 point of this segment.
        /// </summary>
        public double Y1
        {
            get { return y1; }
            set
            {
                y1 = value; 
                OnPropertyChanged("Y1");
            }
        }

        /// <summary>
        /// Gets or sets the y2 point of this segment.
        /// </summary>
        public double Y2
        {
            get { return y2; }
            set
            {
                y2 = value; 
                OnPropertyChanged("Y2");
            }
        }

        /// <summary>
        /// Gets or sets the step x point of this segment.
        /// </summary>
        public double X3
        {
            get { return x3; }
            set
            {
                x3 = value;
                OnPropertyChanged("X3");
            }
        }

        /// <summary>
        /// Gets or sets the step y point of this segment.
        /// </summary>
        public double Y3
        {
            get { return y3; }
            set
            {
                y3 = value;
                OnPropertyChanged("Y3");
            }
        }

        /// <summary>
        /// Gets or sets the X value of point1 in a step line segment.
        /// </summary>
        public double X1Value { get; set; }

        /// <summary>
        /// Gets or sets the Y value of point1 in a step line segment.
        /// </summary>
        public double Y1Value { get; set; }

        /// <summary>
        /// Gets or sets the X value of point2 in a step line segment.
        /// </summary>
        public double X2Value { get; set; }

        /// <summary>
        /// Gets or sets the Y value of point2 in a step line segment.
        /// </summary>
        public double Y2Value { get; set; }

        /// <summary>
        /// Gets or sets the ending y value, bind with this segment.
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
        /// Gets or sets the starting x value, bind with this segment.
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
        /// Gets or sets the ending y value, bind with this segment.
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
        /// Gets or sets the starting y value, bind with this segment.
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
        /// Gets or sets the point collection corresponds to this segment.
        /// </summary>
        /// <remarks>
        /// Each <c>StepLineSegment</c> consists of 3 points(start point, end point and intermediate step point).
        /// </remarks>
        public PointCollection Points
        {
            get { return stepPoints; }
            set
            {
                stepPoints = value;
                OnPropertyChanged("Points");
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
        /// <param name="linePoints"></param>
        [Obsolete("Use SetData(List<ChartPoint> linePoints)")]
        public override void SetData(List<Point> linePoints)
        {
            pointStart = new ChartPoint(linePoints[0].X, linePoints[0].Y);
            stepMidPoint = new ChartPoint(linePoints[1].X, linePoints[1].Y);
            pointEnd = new ChartPoint(linePoints[2].X, linePoints[2].Y);
            X1Value = pointStart.X;
            X2Value = pointEnd.X;
            Y1Value = pointStart.Y;
            Y2Value = pointEnd.Y;
            XData = pointStart.X;
            X1Data = stepMidPoint.X;
            YData = pointStart.Y;
            Y1Data = stepMidPoint.Y;
            XRange = new DoubleRange(pointStart.X, pointEnd.X);
            YRange = new DoubleRange(pointStart.Y, stepMidPoint.Y);
        }

        public override void SetData(List<ChartPoint> linePoints)
        {
            pointStart = linePoints[0];
            stepMidPoint = linePoints[1];
            pointEnd = linePoints[2];
            X1Value = pointStart.X;
            X2Value = pointEnd.X;
            Y1Value = pointStart.Y;
            Y2Value = pointEnd.Y;
            XData = pointStart.X;
            X1Data = stepMidPoint.X;
            YData = pointStart.Y;
            Y1Data = stepMidPoint.Y;
            XRange = new DoubleRange(pointStart.X, pointEnd.X);
            YRange = new DoubleRange(pointStart.Y, stepMidPoint.Y);
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
            if (customTemplate == null)
            {
                poly = new Polyline();
                SetVisualBindings(poly);
                poly.Fill = new SolidColorBrush(Colors.Transparent);
                poly.Tag = this;
                poly.StrokeEndLineCap = PenLineCap.Round;
                return poly;
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
                return poly;
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
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;

            if (isSegmentUpdated)
                Series.SeriesRootPanel.Clip = null;

            double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
            double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
            double xBase = cartesianTransformer.XAxis.IsLogarithmic
                ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase
                : 1;
            bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
            double left = xIsLogarithmic ? Math.Log(X1Value, xBase) : X1Value;
            double right = xIsLogarithmic ? Math.Log(X2Value, xBase) : X2Value;

            if (left <= xEnd && right >= xStart)
            {
                if (poly != null)
                    poly.Visibility = Visibility.Visible;

                Point point1 = transformer.TransformToVisible(pointStart.X, pointStart.Y);
                Point point2 = transformer.TransformToVisible(pointEnd.X, pointEnd.Y);
                Point stepPoint = transformer.TransformToVisible(stepMidPoint.X, stepMidPoint.Y);
                points = new PointCollection();

                if (X1 != point1.X || X2 != point2.X || Y1 != point1.Y || Y2 != point2.Y || X3 != stepPoint.X ||
                    Y3 != stepPoint.Y)
                {
                    this.X1 = point1.X;
                    this.X2 = point2.X;
                    this.Y1 = point1.Y;
                    this.Y2 = point2.Y;
                    this.X3 = stepPoint.X;
                    this.Y3 = stepPoint.Y;
                    points.Add(point1);
                    points.Add(point2);
                    points.Add(stepPoint);

                    if (poly != null) poly.Points = points;
                    else Points = points;
                }
            }
            else
            {
                if (poly != null)
                {
                    poly.ClearUIValues();
                    poly.Visibility = Visibility.Collapsed;
                }
            }
            isSegmentUpdated = true;
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

        internal override void Dispose()
        {
            if(poly != null)
            {
                poly.Tag = null;
                poly = null;
            }
            base.Dispose();
        }

        #endregion

    }
}
