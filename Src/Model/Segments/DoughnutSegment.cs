using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for DoughnutSegment
    /// </summary>
    public partial class DoughnutSegment:ChartSegment
    {
        #region Dependency Property Registration
        
        // Using a DependencyProperty as the backing store for TrackBorderWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackBorderWidthProperty =
            DependencyProperty.Register("TrackBorderWidth", typeof(double), typeof(DoughnutSegment), new PropertyMetadata(0d));

        // Using a DependencyProperty as the backing store for brushTrackBorderColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackBorderColorProperty =
            DependencyProperty.Register("TrackBorderColor", typeof(Brush), typeof(DoughnutSegment), new PropertyMetadata(null));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ActualStartAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty ActualStartAngleProperty =
            DependencyProperty.Register("ActualStartAngle", typeof(double), typeof(DoughnutSegment),
            new PropertyMetadata(0d, OnAngleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ActualEndAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty ActualEndAngleProperty =
            DependencyProperty.Register("ActualEndAngle", typeof(double), typeof(DoughnutSegment),
            new PropertyMetadata(0d, OnAngleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="IsExploded"/> property.
        /// </summary>
        public static readonly DependencyProperty IsExplodedProperty =
            DependencyProperty.Register("IsExploded", typeof(bool), typeof(DoughnutSegment),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsExplodedChaned)));

        #endregion

        #region Fields

        #region Internal Fields

        internal Point startPoint;

        #endregion

        #region Private Fields

        private const double CurveDepth = 1.5;

        private double startAngle;
        
        private double endAngle;

        Path segmentPath;

        private double xData, yData, angleOfSlice;

        private DoughnutSeries parentSeries;

        private int doughnutSeriesIndex, doughnutSeriesCount;

        private int doughnutSegmentsCount;
        
        private bool isInitializing = true;

        private Geometry pathGeometry;

        private Geometry circularPathGeometry;

        private double trackOpacity;

        private Brush trackColor;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for DoughnutSegment
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="series"></param>
        public DoughnutSegment(double startAngle, double endAngle, DoughnutSeries series)
        {

        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the track area border width.
        /// </summary>
        public double TrackBorderWidth
        {
            get { return (double)GetValue(TrackBorderWidthProperty); }
            set { SetValue(TrackBorderWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the track area border color.
        /// </summary>
        public Brush TrackBorderColor
        {
            get { return (Brush)GetValue(TrackBorderColorProperty); }
            set { SetValue(TrackBorderColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the opacity for the track area.
        /// </summary>
        public double TrackOpacity
        {
            get
            {
                return trackOpacity;
            }

            set
            {
                trackOpacity = value;
                OnPropertyChanged("TrackOpacity");
            }
        }

        public Brush TrackColor
        {
            get
            {
                return trackColor;
            }

            set
            {
                trackColor = value;
                OnPropertyChanged("TrackColor");
            }
        }

        /// <summary>
        /// Gets or sets the start angle of this segment slice.
        /// </summary>
        public double ActualStartAngle
        {
            get { return (double)GetValue(ActualStartAngleProperty); }
            set { SetValue(ActualStartAngleProperty, value); }
        }


        /// <summary>
        /// Gets or sets the end angle of this segment slice.
        /// </summary>
        public double ActualEndAngle
        {
            get { return (double)GetValue(ActualEndAngleProperty); }
            set { SetValue(ActualEndAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this segment can be exploded or not.
        /// </summary>
        public bool IsExploded
        {
            get { return (bool)GetValue(IsExplodedProperty); }
            set { SetValue(IsExplodedProperty, value); }
        }

        /// <summary>
        /// Gets the start angle of the <see cref="DoughnutSegment"/>.
        /// </summary>
        public double StartAngle
        {
            get
            {
                return startAngle;
            }
            internal set
            {
                startAngle = value;
                if (Series != null && !Series.CanAnimate)
                    ActualStartAngle = value;
                OnPropertyChanged("StartAngle");
            }
        }

        /// <summary>
        /// Gets the end angle of the <see cref="DoughnutSegment"/>.
        /// </summary>
        public double EndAngle
        {
            get
            {
                return endAngle;
            }
            internal set
            {
                endAngle = value;
                if (Series != null && !Series.CanAnimate)
                    ActualEndAngle = value;
                OnPropertyChanged("EndAngle");
            }
        }

        /// <summary>
        /// Gets the actual angle the <see cref="DoughnutSegment"/> slice.
        /// </summary>
        public double AngleOfSlice
        {
            get
            {
                return angleOfSlice;
            }
            internal set
            {
                angleOfSlice = value;
                OnPropertyChanged("AngleOfSlice");
            }
        }

        /// <summary>
        /// Gets the data point value, bind with x for this segment.
        /// </summary>
        public double XData
        {
            get
            {
                return xData;
            }
            internal set
            {
                xData = value;
                OnPropertyChanged("XData");
            }
        }

        /// <summary>
        /// Gets the data point value, bind with x for this segment.
        /// </summary>
        public double YData
        {
            get
            {
                return yData;
            }
            internal set
            {
                yData = value;
                OnPropertyChanged("YData");
            }
        }

        internal Path CircularDoughnutPath { get; set; }

        private bool IsMultipleCircleDoughnut
        {
            get
            {
                return doughnutSeriesCount == 1 && parentSeries.IsStackedDoughnut;
            }
        }

        internal bool IsEndValueExceed { get; set; }

        internal int DoughnutSegmentIndex { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>       
        public override UIElement CreateVisual(Size size)
        {
            this.CircularDoughnutPath = new Path();
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("TrackColor");
            CircularDoughnutPath.SetBinding(Shape.FillProperty, binding);
            
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("TrackOpacity");
            CircularDoughnutPath.SetBinding(Shape.OpacityProperty, binding);
            
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("TrackBorderColor");
            CircularDoughnutPath.SetBinding(Shape.StrokeProperty, binding);
            
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("TrackBorderWidth");
            CircularDoughnutPath.SetBinding(Shape.StrokeThicknessProperty, binding);

            segmentPath = new Path();
            SetVisualBindings(segmentPath);
            segmentPath.Tag = this;
            return segmentPath;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>       
        public override UIElement GetRenderedVisual()
        {
            return segmentPath;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            if (Series == null || Series.ActualArea == null)
            {
                return;
            }

            if (!this.IsSegmentVisible)
                segmentPath.Visibility = Visibility.Collapsed;
            else
                segmentPath.Visibility = Visibility.Visible;

            if (IsMultipleCircleDoughnut)
            {
                DrawMultipleDoughnut(transformer);
            }
            else
            {
                DrawSingleDoughnut(transformer);
            }
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

        #region Internal Methods

        internal void UpdateTrackInterior(int segmentIndex)
        {
            if (parentSeries.TrackColor != null)
            {
                this.TrackColor = parentSeries.TrackColor;
                this.TrackOpacity = 1;
            }
            else
            {
                this.TrackColor = ChartExtensionUtils.GetInterior(parentSeries, segmentIndex);
                this.TrackOpacity = 0.2;
            }
        }

        internal void SetData(double arcStartAngle, double arcEndAngle, DoughnutSeries doughnutSeries)
        {
            base.Series = doughnutSeries;
            this.StartAngle = arcStartAngle;
            this.EndAngle = arcEndAngle;
            this.parentSeries = doughnutSeries;
            doughnutSeriesCount = doughnutSeries.GetDoughnutSeriesCount();
            doughnutSeriesIndex = GetDoughnutSeriesIndex(doughnutSeries);
            doughnutSegmentsCount = doughnutSeries.DataCount;
            isInitializing = false;
        }

        /// <summary>
        /// Method used to check the given co-ordinates lies in doughnut segment or not
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal bool IsPointInDoughnutSegment(double x, double y)
        {
            Canvas canvas = Series.ActualArea.GetAdorningCanvas();
            Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            Point center = new Point(size.Width * 0.5, size.Height * 0.5);
            double circleRadius = Math.Min(center.X, center.Y) * 0.8;
            double innerRadius = circleRadius * (Series as DoughnutSeries).InternalDoughnutCoefficient;
            double dx = x - center.X;
            double dy = y - center.Y;
            double pointLength = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            if (pointLength < circleRadius && pointLength > innerRadius)
            {
                double angle = Math.Atan2(dy, dx);
                double arcLength = 3.1415 * 2;
                double start = StartAngle;
                double end = EndAngle;

                if (angle < 0)
                {
                    double degreeangle = (angle / (Math.PI / 180)) + 360;
                    angle = degreeangle * Math.PI / 180;
                }

                if (StartAngle > 0 && end > arcLength && angle < StartAngle)
                {
                    angle = angle + arcLength;
                }

                if (angle > start && angle < end)
                {
                    return true;
                }
            }

            return false;
        }

        internal int GetDoughnutSeriesIndex(ChartSeriesBase currentSeries)
        {
            int index = 0;
            var doughnutSeries = (from series in parentSeries.Area.VisibleSeries where series is DoughnutSeries select series).ToList();//Exception is thrown when we get the index of the segment in custom doughnut series-WPF-18315
            return (index = doughnutSeries.IndexOf(currentSeries)) >= 0 ? index : -1;
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
            element.SetBinding(Shape.FillProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
        }

        #endregion

        #region Private Static Methods

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DoughnutSegment).OnAngleChanged(e);
        }
        
        private static void OnIsExplodedChaned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DoughnutSegment).OnPropertyChanged("IsExploded");
        }
        
        private static GeometryGroup GetEllipseGeometry(Point center, double radius, double innerRadius)
        {
            GeometryGroup geometryGroup = new GeometryGroup();
            geometryGroup.FillRule = FillRule.EvenOdd;

            geometryGroup.Children.Add(new EllipseGeometry()
            {
                Center = center,
                RadiusX = radius,
                RadiusY = radius
            });

            geometryGroup.Children.Add(new EllipseGeometry()
            {
                Center = center,
                RadiusX = innerRadius,
                RadiusY = innerRadius
            });

            return geometryGroup;
        }
        
        private static void SuppressAngleForSmallDifference(ref double outerSegmentStartAngle, ref double outerSegmentEndAngle, ref double innerSegmentStartAngle, ref double innerSegmentEndAngle, double segmentStartAngle, double segmentEndAngle, bool isClockWise)
        {
            double middleAngle = 0d;

            if (isClockWise)
            {
                middleAngle = segmentStartAngle + (segmentEndAngle - segmentStartAngle) / 2;

                if (outerSegmentStartAngle > middleAngle)
                {
                    outerSegmentStartAngle = middleAngle;
                }

                if (innerSegmentStartAngle > middleAngle)
                {
                    innerSegmentStartAngle = middleAngle;
                }

                if (outerSegmentEndAngle < middleAngle)
                {
                    outerSegmentEndAngle = middleAngle;
                }

                if (innerSegmentEndAngle < middleAngle)
                {
                    innerSegmentEndAngle = middleAngle;
                }
            }
            else
            {
                middleAngle = segmentEndAngle + (segmentStartAngle - segmentEndAngle) / 2;

                if (outerSegmentStartAngle < middleAngle)
                {
                    outerSegmentStartAngle = middleAngle;
                }

                if (innerSegmentStartAngle < middleAngle)
                {
                    innerSegmentStartAngle = middleAngle;
                }

                if (outerSegmentEndAngle > middleAngle)
                {
                    outerSegmentEndAngle = middleAngle;
                }

                if (innerSegmentEndAngle > middleAngle)
                {
                    innerSegmentEndAngle = middleAngle;
                }
            }
        }

        #endregion

        #region Private Methods

        private void DrawSingleDoughnut(IChartTransformer transformer)
        {
            var segmentStartAngle = ActualStartAngle;
            var segmentEndAngle = ActualEndAngle;
            Point center;

            if (doughnutSeriesCount > 1)
            {
                center = new Point(0.5 * transformer.Viewport.Width, 0.5 * transformer.Viewport.Height);
            }
            else
            {
                center = parentSeries.Center;
            }

            CalculateGapRatioAngle(ref segmentEndAngle,ref segmentStartAngle);

            double actualRadius = parentSeries.InternalDoughnutSize * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
            double remainingWidth = actualRadius - (actualRadius * Series.ActualArea.InternalDoughnutHoleSize);
            double equalParts = remainingWidth / doughnutSeriesCount;
            double radius = parentSeries.Radius = actualRadius - (equalParts * (doughnutSeriesCount - (doughnutSeriesIndex + 1)));
            double innerRadius = radius - (equalParts * parentSeries.InternalDoughnutCoefficient);
            innerRadius = ChartMath.MaxZero(innerRadius);

            if (parentSeries.Segments.IndexOf(this) == 0)
                parentSeries.InnerRadius = innerRadius;

            this.pathGeometry = GetDoughnutGeometry(center, radius, innerRadius, segmentStartAngle, segmentEndAngle, false);
            this.segmentPath.Data = pathGeometry;

            this.circularPathGeometry = null;
            this.CircularDoughnutPath.Data = null;
        }

        private void DrawMultipleDoughnut(IChartTransformer transformer)
        {
            if (IsSegmentVisible)
            {
                var segmentStartAngle = ActualStartAngle;
                var segmentEndAngle = ActualEndAngle;
                var center = parentSeries.Center;

                double actualRadius = parentSeries.InternalDoughnutSize * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
                double remainingWidth = actualRadius - (actualRadius * Series.ActualArea.InternalDoughnutHoleSize);
                double equalParts = (remainingWidth / doughnutSegmentsCount) * parentSeries.InternalDoughnutCoefficient;
                double radius = actualRadius - (equalParts * (doughnutSegmentsCount - (DoughnutSegmentIndex + 1)));
                parentSeries.Radius = actualRadius - equalParts;
                double innerRadius = radius - equalParts;
                double outerRadius = radius - equalParts * parentSeries.SegmentSpacing;
                innerRadius = ChartMath.MaxZero(innerRadius);

                if (parentSeries.Segments.IndexOf(this) == 0)
                    parentSeries.InnerRadius = innerRadius;

                this.pathGeometry = GetDoughnutGeometry(center, outerRadius, innerRadius, segmentStartAngle, segmentEndAngle, false);
                this.segmentPath.Data = pathGeometry;

                //Rendering the back segments.                     
                var seriesStartAngle = parentSeries.DegreeToRadianConverter(parentSeries.StartAngle);
                var seriesEndAngle = parentSeries.DegreeToRadianConverter(parentSeries.EndAngle);

                var totalArcLength = Math.PI * 2;
                var arcLength = seriesEndAngle - seriesStartAngle;
                if (Math.Abs(Math.Round(arcLength, 2)) > totalArcLength)
                    arcLength = arcLength % totalArcLength;

                seriesEndAngle = arcLength + seriesStartAngle;

                this.circularPathGeometry = GetDoughnutGeometry(center, outerRadius, innerRadius, seriesStartAngle, seriesEndAngle, true);
                this.CircularDoughnutPath.Data = circularPathGeometry;
            }
            else
            {
                this.pathGeometry = null;
                this.segmentPath.Data = null;

                this.circularPathGeometry = null;
                this.CircularDoughnutPath.Data = null;
            }
        }

        private Geometry GetDoughnutGeometry(Point center, double radius, double innerRadius, double segmentStartAngle, double segmentEndAngle, bool isUnfilledPath)
        {
            Geometry geometry = null;

            if((this.parentSeries.CapStyle == DoughnutCapStyle.BothFlat || isUnfilledPath || IsEndValueExceed) && Math.Round(segmentEndAngle - segmentStartAngle, 2) == 6.28)
            {
                geometry = GetEllipseGeometry(center, radius, innerRadius);
            }
            else
            {
                geometry = GetArcGeometry(center, radius, innerRadius, segmentStartAngle, segmentEndAngle, isUnfilledPath);
            }

            return geometry;
        }

        private PathGeometry GetArcGeometry(Point center, double radius, double innerRadius, double segmentStartAngle, double segmentEndAngle, bool isUnfilledPath)
        {
            if (this.IsExploded)
            {
                center = new Point(center.X + (parentSeries.ExplodeRadius * Math.Cos(AngleOfSlice)), center.Y + (parentSeries.ExplodeRadius * Math.Sin(AngleOfSlice)));
            }

            var isclockWise = segmentEndAngle > segmentStartAngle;

            var outerSegmentStartAngle = segmentStartAngle;
            var outerSegmentEndAngle = segmentEndAngle;
            var innerSegmentStartAngle = segmentStartAngle;
            var innerSegmentEndAngle = segmentEndAngle;

            double midRadius = 0d;

            if (parentSeries.CapStyle != DoughnutCapStyle.BothFlat && !isUnfilledPath)
            {
                var segmentRadius = (radius - innerRadius) / 2;
                midRadius = radius - segmentRadius;
                UpdateSegmentAngleForCurvePosition(ref outerSegmentStartAngle, ref outerSegmentEndAngle, ref innerSegmentStartAngle, ref innerSegmentEndAngle, segmentStartAngle, segmentEndAngle, radius, innerRadius, segmentRadius, isclockWise);
            }

            startPoint = new Point(center.X + radius * Math.Cos(outerSegmentStartAngle), center.Y + radius * Math.Sin(outerSegmentStartAngle));
            Point endPoint = new Point(center.X + radius * Math.Cos(outerSegmentEndAngle), center.Y + radius * Math.Sin(outerSegmentEndAngle));
            Point startDPoint = new Point(center.X + innerRadius * Math.Cos(innerSegmentStartAngle), center.Y + innerRadius * Math.Sin(innerSegmentStartAngle));
            Point endDPoint = new Point(center.X + innerRadius * Math.Cos(innerSegmentEndAngle), center.Y + innerRadius * Math.Sin(innerSegmentEndAngle));

            var isOuterDirectionClockWise = outerSegmentEndAngle > outerSegmentStartAngle;
            var isInnerDirectionClockWise = innerSegmentEndAngle > innerSegmentStartAngle;

            PathFigure figure = new PathFigure();
            figure.StartPoint = startPoint;
            ArcSegment seg = new ArcSegment();
            seg.Point = endPoint;
            seg.Size = new Size(radius, radius);
            seg.RotationAngle = outerSegmentEndAngle - outerSegmentStartAngle;
            seg.IsLargeArc = (!isOuterDirectionClockWise ? outerSegmentStartAngle - outerSegmentEndAngle : outerSegmentEndAngle - outerSegmentStartAngle) > Math.PI;
            seg.SweepDirection = !isOuterDirectionClockWise ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
            figure.Segments.Add(seg);

            if ((parentSeries.CapStyle == DoughnutCapStyle.EndCurve || parentSeries.CapStyle == DoughnutCapStyle.BothCurve) && !isUnfilledPath)
            {
                Point midEndPoint = new Point(center.X + midRadius * Math.Cos(segmentEndAngle), center.Y + midRadius * Math.Sin(segmentEndAngle));
                var bezierEndPoint = new Point(center.X + radius * Math.Cos(segmentEndAngle), center.Y + radius * Math.Sin(segmentEndAngle));
                var bezSeg = new QuadraticBezierSegment() { Point1 = bezierEndPoint, Point2 = midEndPoint };
                figure.Segments.Add(bezSeg);

                var bezierEndDPoint = new Point(center.X + innerRadius * Math.Cos(segmentEndAngle), center.Y + innerRadius * Math.Sin(segmentEndAngle));
                bezSeg = new QuadraticBezierSegment() { Point1 = bezierEndDPoint, Point2 = endDPoint };
                figure.Segments.Add(bezSeg);
            }
            else
            {
                WindowsLineSegment line = new WindowsLineSegment() { Point = endDPoint };
                figure.Segments.Add(line);
            }

            seg = new ArcSegment();
            seg.Point = startDPoint;
            seg.Size = new Size(innerRadius, innerRadius);
            seg.RotationAngle = innerSegmentEndAngle - innerSegmentStartAngle;
            seg.IsLargeArc = (!isInnerDirectionClockWise ? innerSegmentStartAngle - innerSegmentEndAngle : innerSegmentEndAngle - innerSegmentStartAngle) > Math.PI;
            seg.SweepDirection = isInnerDirectionClockWise ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
            figure.Segments.Add(seg);

            if ((parentSeries.CapStyle == DoughnutCapStyle.StartCurve || parentSeries.CapStyle == DoughnutCapStyle.BothCurve) && !isUnfilledPath)
            {
                Point midStartPoint = new Point(center.X + midRadius * Math.Cos(segmentStartAngle), center.Y + midRadius * Math.Sin(segmentStartAngle));
                var bezierStartDPoint = new Point(center.X + innerRadius * Math.Cos(segmentStartAngle), center.Y + innerRadius * Math.Sin(segmentStartAngle));
                var bezSeg = new QuadraticBezierSegment() { Point1 = bezierStartDPoint, Point2 = midStartPoint };
                figure.Segments.Add(bezSeg);

                var bezierStartPoint = new Point(center.X + radius * Math.Cos(segmentStartAngle), center.Y + radius * Math.Sin(segmentStartAngle));
                bezSeg = new QuadraticBezierSegment() { Point1 = bezierStartPoint, Point2 = startPoint };
                figure.Segments.Add(bezSeg);
            }

            figure.IsClosed = true;
            var geometry = new PathGeometry();
            geometry.Figures = new PathFigureCollection() { figure };
            return geometry;
        }

        private void CalculateGapRatioAngle(ref double segmentEndAngle, ref double segmentStartAngle)
        {
            if (parentSeries.SegmentSpacing != 0)
            {
                // Clockwise condition gap ratio check.
                if (segmentEndAngle > segmentStartAngle)
                {
                    segmentEndAngle = segmentEndAngle - parentSeries.SegmentGapAngle;
                    segmentStartAngle = segmentStartAngle + parentSeries.SegmentGapAngle;

                    if(segmentEndAngle < segmentStartAngle)
                    {
                        segmentStartAngle = segmentEndAngle = 0;
                    }
                }
                else if (segmentEndAngle < segmentStartAngle)
                {
                    segmentEndAngle = segmentEndAngle + parentSeries.SegmentGapAngle;
                    segmentStartAngle = segmentStartAngle - parentSeries.SegmentGapAngle;
                    
                    if (segmentEndAngle > segmentStartAngle)
                    {
                        segmentStartAngle =  segmentEndAngle = 0;
                    }
                }
            }
        }
        
        private void UpdateSegmentAngleForCurvePosition(ref double outerSegmentStartAngle, ref double outerSegmentEndAngle, ref double innerSegmentStartAngle, ref double innerSegmentEndAngle, double segmentStartAngle, double segmentEndAngle, double radius, double innerRadius, double segmentRadius, bool isClockwise)
        {
            // To prevent argument exception(Nan) when radius is 0.
            if (radius == 0)
            {
                return;
            }

            if (parentSeries.CapStyle != DoughnutCapStyle.EndCurve)
            {
                outerSegmentStartAngle = isClockwise ? segmentStartAngle + (segmentRadius * CurveDepth) / radius : segmentStartAngle - (segmentRadius * CurveDepth) / radius;
                innerSegmentStartAngle = isClockwise ? segmentStartAngle + (segmentRadius * CurveDepth) / innerRadius : segmentStartAngle - (segmentRadius * CurveDepth) / innerRadius;
            }

            if (parentSeries.CapStyle != DoughnutCapStyle.StartCurve)
            {
                outerSegmentEndAngle = !isClockwise ? segmentEndAngle + (segmentRadius * CurveDepth) / radius : segmentEndAngle - (segmentRadius * CurveDepth) / radius;
                innerSegmentEndAngle = !isClockwise ? segmentEndAngle + (segmentRadius * CurveDepth) / innerRadius : segmentEndAngle - (segmentRadius * CurveDepth) / innerRadius;
            }

            SuppressAngleForSmallDifference(ref outerSegmentStartAngle, ref outerSegmentEndAngle, ref innerSegmentStartAngle, ref innerSegmentEndAngle, segmentStartAngle, segmentEndAngle, isClockwise);
        }

        private void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Series != null && !isInitializing)
            {
                this.Update(this.Series.CreateTransformer(new Size(), false));
            }
        }

        #endregion

        internal override void Dispose()
        {
            if(segmentPath != null)
            {
                segmentPath.Tag = null;
                segmentPath = null;
            }
            base.Dispose();
        }

        #endregion

    }
}
