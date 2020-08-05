using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class SplineRangeAreaSegment : RangeAreaSegment
    {
        #region Fields

        #region Private Properties

        private List<ChartPoint> AreaPoints;

        private Path segPath;

        private Canvas segmentCanvas;

        #endregion

        #endregion

        #region Constructor
        
        /// <summary>
        /// Called when instance created for SplineRangeAreaSegments
        /// </summary>
        /// <param name="AreaPoints"></param>
        /// <param name="series"></param>
        [Obsolete("Use SplineRangeAreaSegment(List<ChartPoint> AreaPoints, SplineRangeAreaSeries series)")]
        public SplineRangeAreaSegment(List<Point> AreaPoints, SplineRangeAreaSeries series)
        {
            this.Series = series;
        }

        public SplineRangeAreaSegment(List<ChartPoint> AreaPoints, SplineRangeAreaSeries series)
        {
            this.Series = series;
        }

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
            segmentCanvas = new Canvas();
            segPath = new Path();
            segPath.Tag = this;
            SetVisualBindings(segPath);
            segmentCanvas.Children.Add(segPath);
            return segmentCanvas;
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

            if (AreaPoints.Count > 1)
            {
                int endIndex = AreaPoints.Count - 1;
                WindowsLineSegment lineSegment = new WindowsLineSegment();
                figure.StartPoint = transformer.TransformToVisible(AreaPoints[0].X, AreaPoints[0].Y);
                lineSegment.Point = transformer.TransformToVisible(AreaPoints[1].X, AreaPoints[1].Y);
                figure.Segments.Add(lineSegment);

                for (int i = 2; i < AreaPoints.Count - 1; i += 6)
                {
                    BezierSegment segment = new BezierSegment();
                    segment.Point1 = transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y);
                    segment.Point2 = transformer.TransformToVisible(AreaPoints[i + 1].X, AreaPoints[i + 1].Y);
                    segment.Point3 = transformer.TransformToVisible(AreaPoints[i + 2].X, AreaPoints[i + 2].Y);
                    figure.Segments.Add(segment);
                }

                lineSegment = new WindowsLineSegment();
                lineSegment.Point = transformer.TransformToVisible(AreaPoints[endIndex].X, AreaPoints[endIndex].Y);
                figure.Segments.Add(lineSegment);

                for (int i = endIndex - 1; i > 1; i -= 6)
                {
                    BezierSegment segment = new BezierSegment();
                    segment.Point1 = transformer.TransformToVisible(AreaPoints[i].X, AreaPoints[i].Y);
                    segment.Point2 = transformer.TransformToVisible(AreaPoints[i - 1].X, AreaPoints[i - 1].Y);
                    segment.Point3 = transformer.TransformToVisible(AreaPoints[i - 2].X, AreaPoints[i - 2].Y);
                    figure.Segments.Add(segment);
                }
            }

            PathGeometry segmentGeometry = new PathGeometry();
            segmentGeometry.Figures.Add(figure);
            this.segPath.Data = segmentGeometry;
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
            base.SetVisualBindings(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
        }

        #endregion

        internal override void Dispose()
        {
            if (segmentCanvas != null)
            {
                segmentCanvas.Children.Clear();
                segmentCanvas = null;
            }
            if (segPath != null)
            {
                segPath.Tag = null;
                segPath = null;
            }
            base.Dispose();
        }

        #endregion        
    }
}
