using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
#if WINDOWS_UAP
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class BoxAndWhiskerSegment : ChartSegment
    {
        #region Fields

#region Private Fields

        #region Constants

        private const int averagePathSize = 10;

#endregion

        private Line lowerQuartileLine;

        private Line upperQuartileLine;

        private Line medianLine;

        private Line maximumLine;

        private Line minimumLine;

        private Canvas segmentCanvas;

        private Rectangle rectangle;

        private Path averagePath;

        private double average;

        #endregion

        #endregion

        #region Constructor

        public BoxAndWhiskerSegment(ChartSeriesBase series)
        {
            this.Series = series;
            Outliers = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the actual stroke for box and whisker segment.
        /// </summary>
        public Brush ActualStroke
        {
            get
            {
                var series = this.Series as ChartSeries;
                if (series.Stroke != null)
                    return series.Stroke;
                else
                    return ChartColorModifier.GetDarkenedColor(this.Interior, 0.6d);
            }
        }

        /// <summary>
        /// Gets or sets minimum value for segment. 
        /// </summary>
        public double Minimum
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets maximum value for segment. 
        /// </summary>
        public double Maximum
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets median value for segment. 
        /// </summary>
        public double Median
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets lower quartile value for segment. 
        /// </summary>
        public double LowerQuartile
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets upper quartile value for segment. 
        /// </summary>
        public double UppperQuartile
        {
            get;
            internal set;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the whisker edge width.
        /// </summary>
        internal double WhiskerWidth
        {
            get;
            set;
        }

        internal double Left
        {
            get;
            set;
        }

        internal double Right
        {
            get;
            set;
        }

        internal double Top
        {
            get;
            set;
        }

        internal double Bottom
        {
            get;
            set;
        }

        internal double Center
        {
            get;
            set;
        }

        internal List<double> Outliers
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the value for box and whisker segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        public override void SetData(params double[] Values)
        {
            Left = Values[0];
            Right = Values[1];
            Top = Values[8];
            Bottom = Values[2];

            Minimum = Values[3];
            LowerQuartile = Values[4];
            Median = Values[5];
            UppperQuartile = Values[6];
            Maximum = Values[7];
            Center = Values[9];
            average = Values[10];

            XRange = new DoubleRange(Left, Right);
            YRange = new DoubleRange(Top, Bottom);
        }

        /// <summary>
        /// Creates the visaul for box and whisker segment. 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public override UIElement CreateVisual(Size size)
        {
            segmentCanvas = new Canvas();
            lowerQuartileLine = new Line();
            SetVisualBindings(lowerQuartileLine);
            segmentCanvas.Children.Add(lowerQuartileLine);
            lowerQuartileLine.Tag = this;

            upperQuartileLine = new Line();
            SetVisualBindings(upperQuartileLine);
            segmentCanvas.Children.Add(upperQuartileLine);
            upperQuartileLine.Tag = this;

            medianLine = new Line();
            SetVisualBindings(medianLine);
            medianLine.Tag = this;
            Canvas.SetZIndex(medianLine, 1);
            segmentCanvas.Children.Add(medianLine);

            maximumLine = new Line();
            SetVisualBindings(maximumLine);
            maximumLine.Tag = this;
            segmentCanvas.Children.Add(maximumLine);

            minimumLine = new Line();
            SetVisualBindings(minimumLine);
            minimumLine.Tag = this;
            segmentCanvas.Children.Add(minimumLine);

            rectangle = new Rectangle();
            rectangle.Tag = this;
            SetVisualBindings(rectangle);
            segmentCanvas.Children.Add(rectangle);

            averagePath = new Path();
            averagePath.Tag = this;
            averagePath.Stretch = Stretch.Fill;
            DrawPathGeometry();
            SetVisualBindings(averagePath);

            return segmentCanvas;
        }

        /// <summary>
        /// Gets the rendered visaul for box and whisker segment. 
        /// </summary>
        /// <returns></returns>
        public override UIElement GetRenderedVisual()
        {
            return segmentCanvas;
        }

        public override void OnSizeChanged(Size size)
        {

        }

        /// <summary>
        /// Updates the box and whisker segment based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            if (double.IsNaN(Minimum) && double.IsNaN(LowerQuartile) && double.IsNaN(Median) && double.IsNaN(UppperQuartile) && double.IsNaN(Maximum)) return;

            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            double xStart = Math.Floor(cartesianTransformer.XAxis.VisibleRange.Start);
            double xEnd = Math.Ceiling(cartesianTransformer.XAxis.VisibleRange.End);
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
            double left = xIsLogarithmic ? Math.Log(Left, xBase) : Left;
            double right = xIsLogarithmic ? Math.Log(Right, xBase) : Right;

            if ((left <= xEnd && right >= xStart))
            {
                lowerQuartileLine.Visibility = Visibility.Visible;
                upperQuartileLine.Visibility = Visibility.Visible;
                medianLine.Visibility = Visibility.Visible;
                minimumLine.Visibility = Visibility.Visible;
                maximumLine.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Visible;

                Point minimumPoint = transformer.TransformToVisible(Center, Minimum);
                Point lowerQuartilePoint = transformer.TransformToVisible(Center, LowerQuartile);
                Point medianPoint = transformer.TransformToVisible(Center, Median);
                Point upperQuartilePoint = transformer.TransformToVisible(Center, UppperQuartile);
                Point maximumPoint = transformer.TransformToVisible(Center, Maximum);

                var tlPoint = transformer.TransformToVisible(Left, UppperQuartile);
                var brPoint = transformer.TransformToVisible(Right, LowerQuartile);

                double spacing = (Series as ISegmentSpacing).SegmentSpacing;
                rect = new Rect(tlPoint, brPoint);

                if (Series.IsActualTransposed)
                {
                    if (spacing > 0 && spacing <= 1)
                    {
                        rect.Y = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, rect.Bottom, rect.Top);
                        rect.Height = (1 - spacing) * rect.Height;
                    }

                    var percentHeight = rect.Height / 2 * WhiskerWidth;
                    var centerLinePosition = rect.Y + rect.Height / 2;
                    var topPercentWidth = centerLinePosition - percentHeight;
                    var bottomPercentWidth = centerLinePosition + percentHeight;

                    this.medianLine.X1 = medianPoint.X;
                    this.medianLine.X2 = medianPoint.X;
                    this.medianLine.Y1 = rect.Top;
                    this.medianLine.Y2 = rect.Bottom;

                    this.maximumLine.X1 = maximumPoint.X;
                    this.maximumLine.Y1 = topPercentWidth;
                    this.maximumLine.X2 = maximumPoint.X;
                    this.maximumLine.Y2 = bottomPercentWidth;

                    this.minimumLine.X1 = minimumPoint.X;
                    this.minimumLine.Y1 = topPercentWidth;
                    this.minimumLine.X2 = minimumPoint.X;
                    this.minimumLine.Y2 = bottomPercentWidth;
                }
                else
                {
                    if (spacing > 0 && spacing <= 1)
                    {
                        rect.X = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, rect.Right, rect.Left);
                        rect.Width = (1 - spacing) * rect.Width;
                    }

                    var percentWidth = rect.Width / 2 * WhiskerWidth;
                    var centerLinePosition = rect.X + rect.Width / 2;
                    var leftPercentWidth = centerLinePosition - percentWidth;
                    var rightPercentWidth = centerLinePosition + percentWidth;

                    this.medianLine.X1 = rect.Left;
                    this.medianLine.X2 = rect.Right;
                    this.medianLine.Y1 = medianPoint.Y;
                    this.medianLine.Y2 = medianPoint.Y;

                    this.maximumLine.X1 = leftPercentWidth;
                    this.maximumLine.Y1 = maximumPoint.Y;
                    this.maximumLine.X2 = rightPercentWidth;
                    this.maximumLine.Y2 = maximumPoint.Y;

                    this.minimumLine.X1 = leftPercentWidth;
                    this.minimumLine.Y1 = minimumPoint.Y;
                    this.minimumLine.X2 = rightPercentWidth;
                    this.minimumLine.Y2 = minimumPoint.Y;
                }

                rectangle.Width = rect.Width;
                rectangle.Height = rect.Height;
                rectangle.SetValue(Canvas.LeftProperty, rect.X);
                rectangle.SetValue(Canvas.TopProperty, rect.Y);

                this.lowerQuartileLine.X1 = minimumPoint.X;
                this.lowerQuartileLine.X2 = lowerQuartilePoint.X;
                this.lowerQuartileLine.Y1 = minimumPoint.Y;
                this.lowerQuartileLine.Y2 = lowerQuartilePoint.Y;

                this.upperQuartileLine.X1 = upperQuartilePoint.X;
                this.upperQuartileLine.X2 = maximumPoint.X;
                this.upperQuartileLine.Y1 = upperQuartilePoint.Y;
                this.upperQuartileLine.Y2 = maximumPoint.Y;

                UpdateMeanSymbol(cartesianTransformer, (Series as BoxAndWhiskerSeries).ShowMedian);
            }
            else
            {
                medianLine.Visibility = Visibility.Collapsed;
                minimumLine.Visibility = Visibility.Collapsed;
                maximumLine.Visibility = Visibility.Collapsed;
                lowerQuartileLine.Visibility = Visibility.Collapsed;
                upperQuartileLine.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Collapsed;
                averagePath.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Internal Override Methods

        internal void UpdateMeanSymbol(IChartTransformer cartesianTransformer, bool showMedian)
        {
            if (showMedian)
            {
                averagePath.Visibility = Visibility.Visible;
                Point averagePoint = cartesianTransformer.TransformToVisible(Center, average);
                double sizeRatio = 0.25, CenterX, CenterY;

                if (this.Series.IsActualTransposed)
                {
                    //To retain with the path size when the size is increased.
                    double widthFactor = rect.Height * sizeRatio;
                    widthFactor = widthFactor > averagePathSize ? averagePathSize : widthFactor;

                    averagePath.Width = widthFactor;
                    averagePath.Height = widthFactor;

                    CenterX = averagePoint.X;
                    CenterY = (rect.Top + rect.Height / 2);
                }
                else
                {
                    double widthFactor = rect.Width * sizeRatio;
                    widthFactor = widthFactor > averagePathSize ? averagePathSize : widthFactor;

                    averagePath.Width = widthFactor;
                    averagePath.Height = widthFactor;

                    CenterX = (rect.X + rect.Width / 2);
                    CenterY = averagePoint.Y;
                }

                Canvas.SetLeft(averagePath, CenterX - averagePath.Width / 2);
                Canvas.SetTop(averagePath, CenterY - averagePath.Height / 2);

                if (!segmentCanvas.Children.Contains(averagePath))
                    segmentCanvas.Children.Add(averagePath);
            }
            else
            {
                if (segmentCanvas.Children.Contains(averagePath))
                    segmentCanvas.Children.Remove(averagePath);
            }
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method implementation for Set Bindings to properties in ColumnSegement.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            if (!(element is Path))
                base.SetVisualBindings(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualStroke");
            element.SetBinding(Shape.StrokeProperty, binding);
        }

        protected override void OnPropertyChanged(string name)
        {
            //UWP - 3938 Fix for the stroke color not getting changed when switched color from palette to interior in box and whisker series. 
            //The palette is checked when interior is changed.

            if (name == "Interior")
                base.OnPropertyChanged("ActualStroke");
            else if (name == "Stroke")
                name = "ActualStroke";
            base.OnPropertyChanged(name);
        }

        #endregion

        #region Private Methods

        private void DrawPathGeometry()
        {
            //When the segment is resized the size is maintained.

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, 0);

            WindowsLineSegment lineSegment1 = new WindowsLineSegment();
            WindowsLineSegment lineSegment2 = new WindowsLineSegment();

            lineSegment1.Point = new Point(averagePathSize, averagePathSize);
            pathFigure.Segments.Add(lineSegment1);

            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.StartPoint = new Point(averagePathSize, 0);

            lineSegment2.Point = new Point(0, averagePathSize);
            pathFigure2.Segments.Add(lineSegment2);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(pathFigure);
            geometry.Figures.Add(pathFigure2);

            averagePath.Data = geometry;
        }

        #endregion

        #endregion
    }
}
