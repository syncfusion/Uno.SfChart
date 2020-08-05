using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart SplineArea segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="SplineAreaSeries"/>
    public partial class SplineAreaSegment : AreaSegment
    {
        #region Fields

        #region Internal Fields

        internal List<ChartPoint> segmentPoints = new List<ChartPoint>();
        
        #endregion

        #region Private Fields

        private SplineAreaSeries containerSeries;

        private Canvas segmentCanvas;

        bool isEmpty;

        Path segPath;

        private bool isSegmentUpdated;

        PathGeometry strokeGeometry;

        PathFigure strokeFigure;

        Path strokePath;

        #endregion

        #endregion

        #region Constructor

        public SplineAreaSegment()
        {

        }

        /// <summary>
        /// Constructor for <c>SplineAreaSegment</c>.
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="series"></param>
        [Obsolete("Use SplineAreaSegment(List<ChartPoint> Points, List<double> xValues, IList<double> yValues, SplineAreaSeries series): base(xValues, yValues)")]
        public SplineAreaSegment(List<Point> Points, List<double> xValues, IList<double> yValues, SplineAreaSeries series)
            : base(xValues, yValues)
        {
            Series = containerSeries = series;
            base.Item = series.ActualData;
            SetData(Points, xValues, yValues);
        }

        public SplineAreaSegment(List<ChartPoint> Points, List<double> xValues, IList<double> yValues, SplineAreaSeries series)
          : base(xValues, yValues)
        {
            Series = containerSeries = series;
            base.Item = series.ActualData;
            SetData(Points, xValues, yValues);
        }

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
            if (isSegmentUpdated)
                Series.SeriesRootPanel.Clip = null;
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
            DoubleRange range = cartesianTransformer.XAxis.VisibleRange;

            PathFigure figure = new PathFigure();
            PathGeometry segmentGeometry = new PathGeometry();
            double origin = containerSeries.ActualXAxis != null ? containerSeries.ActualXAxis.Origin : 0;//setting origin value for splinearea segment
            WindowsLineSegment lineSegment;
            figure.StartPoint = transformer.TransformToVisible(segmentPoints[0].X, segmentPoints[0].Y);
            lineSegment = new WindowsLineSegment();
            lineSegment.Point = transformer.TransformToVisible(segmentPoints[1].X, segmentPoints[1].Y);
            figure.Segments.Add(lineSegment);

            strokeGeometry = new PathGeometry();
            strokeFigure = new PathFigure();
            strokePath = new Path();

            if (containerSeries.IsClosed && isEmpty)
            {
                AddStroke(figure.StartPoint);
                lineSegment = new WindowsLineSegment();
                lineSegment.Point = transformer.TransformToVisible(segmentPoints[1].X, segmentPoints[1].Y);
                strokeFigure.Segments.Add(lineSegment);
            }
            else if (!containerSeries.IsClosed)
            {
                AddStroke(transformer.TransformToVisible(segmentPoints[1].X, segmentPoints[1].Y));
            }

            int i;
            for (i = 2; i < segmentPoints.Count; i += 3)
            {
                double xVal = segmentPoints[i].X;
                if (xVal >= range.Start && xVal <= range.End || xEnd >= range.Start && xEnd <= range.End)
                {
                    if (Series.ShowEmptyPoints || ((!double.IsNaN(segmentPoints[i].Y) && !double.IsNaN(segmentPoints[i + 1].Y) && !double.IsNaN(segmentPoints[i + 2].Y))))
                    {
                        BezierSegment segment = new BezierSegment();
                        segment.Point1 = transformer.TransformToVisible(segmentPoints[i].X, segmentPoints[i].Y);
                        segment.Point2 = transformer.TransformToVisible(segmentPoints[i + 1].X, segmentPoints[i + 1].Y);
                        segment.Point3 = transformer.TransformToVisible(segmentPoints[i + 2].X, segmentPoints[i + 2].Y);
                        figure.Segments.Add(segment);
                        if ((isEmpty && !Series.ShowEmptyPoints) || !containerSeries.IsClosed)
                        {
                            BezierSegment strokeSegment = new BezierSegment();
                            strokeSegment.Point1 = segment.Point1;
                            strokeSegment.Point2 = segment.Point2;
                            strokeSegment.Point3 = segment.Point3;
                            strokeFigure.Segments.Add(strokeSegment);
                        }
                    }
                    else
                    {
                        if ((double.IsNaN(segmentPoints[i].Y) && double.IsNaN(segmentPoints[i + 1].Y) && double.IsNaN(segmentPoints[i + 2].Y)))
                        {
                            lineSegment = new WindowsLineSegment();
                            lineSegment.Point = transformer.TransformToVisible(segmentPoints[i - 1].X, origin);
                            figure.Segments.Add(lineSegment);
                            lineSegment = new WindowsLineSegment();
                            lineSegment.Point = transformer.TransformToVisible(segmentPoints[i + 2].X, origin);
                            figure.Segments.Add(lineSegment);
                        }
                        else if (i > 0 && (double.IsNaN(segmentPoints[i - 1].Y) || double.IsNaN(segmentPoints[i].Y)))
                        {
                            lineSegment = new WindowsLineSegment();
                            lineSegment.Point = transformer.TransformToVisible(segmentPoints[i + 2].X, origin);

                            figure.Segments.Add(lineSegment);
                            lineSegment = new WindowsLineSegment();
                            lineSegment.Point = transformer.TransformToVisible(segmentPoints[i + 2].X, double.IsNaN(segmentPoints[i + 2].Y) ? origin : segmentPoints[i + 2].Y);
                            if ((!Series.ShowEmptyPoints && !double.IsNaN(segmentPoints[i + 2].Y)) || !containerSeries.IsClosed)
                            {
                                strokeFigure = new PathFigure();
                                strokeFigure.StartPoint = lineSegment.Point;
                                strokeGeometry.Figures.Add(strokeFigure);
                            }
                            figure.Segments.Add(lineSegment);
                        }
                    }
                }
            }
            Point point = transformer.TransformToVisible(segmentPoints[i - 1].X, origin);
            lineSegment = new WindowsLineSegment();
            lineSegment.Point = point;
            figure.Segments.Add(lineSegment);
            if (containerSeries.IsClosed && !double.IsNaN(segmentPoints[i - 1].Y))
            {
                lineSegment = new WindowsLineSegment();
                lineSegment.Point = point;
                strokeFigure.Segments.Add(lineSegment);
            }
            //figure.IsClosed = true;
            isSegmentUpdated = true;
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

        #region Public Methods

        /// <summary>
        /// Method implementation for SetData
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        [Obsolete("Use SetData(List<ChartPoint> points, List<double> xValues, IList<double> yValues)")]
        public void SetData(List<Point> points, List<double> xValues, IList<double> yValues)
        {
            base.SetData(xValues, yValues);
            var pointCollection = new List<ChartPoint>();
            foreach (var point in points)
                pointCollection.Add(new ChartPoint(point.X, point.Y));
            segmentPoints = pointCollection;
            double _Min = points.Min(item => item.Y);
            double Y_MIN = double.IsNaN(_Min) ? points.Where(item => !double.IsNaN(item.Y)).Min(item => item.Y) : _Min;
            isEmpty = double.IsNaN(_Min);
            XRange = new DoubleRange(points.Min(item => item.X), points.Max(item => item.X));
            YRange = new DoubleRange(Y_MIN, points.Max(item => item.Y));
            if (!isEmpty && segPath != null)
            {
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("StrokeThickness");
                segPath.SetBinding(Path.StrokeThicknessProperty, binding);
            }
        }

        public void SetData(List<ChartPoint> points, List<double> xValues, IList<double> yValues)
        {
            base.SetData(xValues, yValues);
            segmentPoints = points;
            double _Min = points.Min(item => item.Y);
            double Y_MIN = double.IsNaN(_Min) ? points.Where(item => !double.IsNaN(item.Y)).Min(item => item.Y) : _Min;
            isEmpty = double.IsNaN(_Min);
            XRange = new DoubleRange(points.Min(item => item.X), points.Max(item => item.X));
            YRange = new DoubleRange(Y_MIN, points.Max(item => item.Y));
            if (!isEmpty && segPath != null)
            {
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("StrokeThickness");
                segPath.SetBinding(Path.StrokeThicknessProperty, binding);
            }
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

        #region Private Methods


        /// <summary>
        /// Called to add the stroke for spline area series.
        /// </summary>
        /// <param name="startPoint"></param>
        private void AddStroke(Point startPoint)
        {
            if (segmentCanvas.Children.Count > 1)
                segmentCanvas.Children.RemoveAt(1); //remove the existing stroke path 
            segPath.StrokeThickness = 0;

            strokeFigure.StartPoint = startPoint;

            strokeGeometry.Figures.Add(strokeFigure);

            strokePath.Data = strokeGeometry;

            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            strokePath.SetBinding(Path.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            strokePath.SetBinding(Path.StrokeThicknessProperty, binding);
            segmentCanvas.Children.Add(strokePath);
        }

        #endregion

        internal override void Dispose()
        {
            if(segmentCanvas != null)
            {
                segmentCanvas.Children.Clear();
                segmentCanvas = null;
            }
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
