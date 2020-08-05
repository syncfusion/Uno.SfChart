using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Data;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart area segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="AreaSeries"/>
    public partial class AreaSegment : ChartSegment
    {
        #region Fields

        #region Private Fields

        private IList<double> XValues, YValues;

        private ChartSeriesBase containerSeries;

        private Canvas segmentCanvas;

        bool isEmpty;

        Path segPath;

        bool segmentUpdated;

        PathGeometry strokeGeometry;

        PathFigure strokeFigure;

        PolyLineSegment strokePolyLine;

        Path strokePath;

        private double _xData;

        private double _yData;

        #endregion

        #endregion

        #region Propertes

        #region Public Properties

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

        #endregion

        #endregion
        
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public AreaSegment()
        {

        }

        /// <summary>
        /// Called when instance created for AreaSegments
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="series"></param>
        public AreaSegment(List<double> xValues, List<double> yValues, AdornmentSeries series, object item)
        {
            base.Series = series;
            containerSeries = series;
            base.Item = item;
        }

        /// <summary>
        /// Called when instance created for AreaSegments 
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        public AreaSegment(List<double> xValues, IList<double> yValues)
        {
            
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="XValues"></param>
        /// <param name="YValues"></param>
        public override void SetData(IList<double> XValues, IList<double> YValues)
        {
            this.XValues = XValues;
            this.YValues = YValues;
            double _Min = YValues.Min();
            isEmpty = double.IsNaN(_Min);
            double Y_MIN;
            if (double.IsNaN(_Min))
            {
                var yVal = YValues.Where(e => !double.IsNaN(e));
                Y_MIN = (!yVal.Any()) ? 0 : yVal.Min();
            }
            else
            {
                Y_MIN = _Min;
            }

            XRange = new DoubleRange(XValues.Min(), XValues.Max());
            YRange = new DoubleRange(Y_MIN, YValues.Max());
            if(!isEmpty && segPath!=null)
            {
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("StrokeThickness");
                segPath.SetBinding(Path.StrokeThicknessProperty, binding);
            }
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
            return segmentCanvas;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            ChartAxis xAxis = this.Series.ActualXAxis;
            double xValue = 0;
            int lastIndex = 0;


            if (transformer != null)
            {
                if (segmentUpdated)
                    Series.SeriesRootPanel.Clip = null;

                PathGeometry segmentGeometry = new PathGeometry();

                PathFigure figure = new PathFigure();

                WindowsLinesegment lineSegment;
                double start = containerSeries.ActualYAxis.ActualRange.Start;
                double origin = containerSeries.ActualXAxis.Origin;
                if (containerSeries.ActualXAxis != null && containerSeries.ActualXAxis.Origin == 0
                    && containerSeries.ActualYAxis is LogarithmicAxis && (containerSeries.ActualYAxis as LogarithmicAxis).Minimum != null)
                    origin = (double)(containerSeries.ActualYAxis as LogarithmicAxis).Minimum;
                origin = origin == 0d ? start < 0d ? 0d : start : origin;
                figure.StartPoint = transformer.TransformToVisible(XValues[0], origin);

                ResetStrokeShapes();

                if (Series is PolarRadarSeriesBase && !double.IsNaN(YValues[0]))
                {
                    figure.StartPoint = transformer.TransformToVisible(XValues[0], YValues[0]);
                }

                if ((Series is AreaSeries) && !(Series as AreaSeries).IsClosed && !double.IsNaN(YValues[0]))
                {
                    this.AddStroke(transformer.TransformToVisible(XValues[0], YValues[0]));
                }
                else if (isEmpty)
                {
                    this.AddStroke(figure.StartPoint);
                }

                var logarithmicXAxis = xAxis as LogarithmicAxis;
                double rangeStart = logarithmicXAxis != null ? Math.Pow(logarithmicXAxis.LogarithmicBase, xAxis.VisibleRange.Start) : xAxis.VisibleRange.Start;
                double rangeEnd = logarithmicXAxis != null ? Math.Pow(logarithmicXAxis.LogarithmicBase, xAxis.VisibleRange.End) : xAxis.VisibleRange.End;
                for (int index = 0; index < XValues.Count; index++)
                {
                    // UWP - 256 Aea series is fuzzy on chart if it is in zoomed state
                    xValue = XValues[index];
                    if (!(rangeStart <= xValue && rangeEnd >= xValue))
                        if (rangeStart >= xValue && index + 1 < XValues.Count
                            && rangeStart <= XValues[index + 1])
                        {
                            figure.StartPoint = transformer.TransformToVisible(xValue, origin);
                            //To avoid stroke blur when IsClosed & Empty point stroke is false.
                            if ((Series is AreaSeries) && !(Series as AreaSeries).IsClosed && !double.IsNaN(YValues[0]))
                            {
                                ResetStrokeShapes();
                                this.AddStroke(transformer.TransformToVisible(xValue, YValues[index]));
                            }
                            else if (isEmpty)
                            {
                                ResetStrokeShapes();
                                this.AddStroke(figure.StartPoint);
                            }
                        }
                        else if (!(rangeEnd <= xValue && index - 1 > -1
                            && rangeEnd >= XValues[index - 1]))
                        {
                            continue;
                        }

                    if (!double.IsNaN(YValues[index]))
                    {
                        Point point = transformer.TransformToVisible(xValue, YValues[index]);
                        lineSegment = new WindowsLinesegment();
                        lineSegment.Point = point;
                        figure.Segments.Add(lineSegment);

                        if ((isEmpty && !Series.ShowEmptyPoints) || (Series is AreaSeries && !(Series as AreaSeries).IsClosed))
                        {
                            if (index > 0 && double.IsNaN(YValues[index - 1]))
                            {
                                strokeFigure = new PathFigure();
                                strokePolyLine = new PolyLineSegment();
                                strokeFigure.StartPoint = point;
                                strokeGeometry.Figures.Add(strokeFigure);
                                strokeFigure.Segments.Add(strokePolyLine);
                            }
                            strokePolyLine.Points.Add(point);
                        }
                    }
                    else if (XValues.Count > 1)
                    {
                        if (index > 0 && index < XValues.Count - 1)
                        {
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index - 1], origin);
                            figure.Segments.Add(lineSegment);

                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(xValue, origin);
                            figure.Segments.Add(lineSegment);

                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index + 1], origin);
                            figure.Segments.Add(lineSegment);
                        }
                        else if (index == 0)
                        {
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(xValue, origin);
                            figure.Segments.Add(lineSegment);
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index + 1], origin);

                            figure.Segments.Add(lineSegment);
                        }
                        else
                        {
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index - 1], origin);
                            figure.Segments.Add(lineSegment);

                        }

                    }
                    lastIndex = index;
                }
                if (!(Series is PolarRadarSeriesBase))
                {

                    lineSegment = new WindowsLinesegment();
                    lineSegment.Point = transformer.TransformToVisible(XValues[lastIndex], origin);
                    figure.Segments.Add(lineSegment);

                    if ((Series is AreaSeries && (Series as AreaSeries).IsClosed) && isEmpty && !double.IsNaN(YValues[lastIndex]))
                        strokePolyLine.Points.Add(lineSegment.Point);
                }

                segmentGeometry.Figures.Add(figure);
                this.segPath.Data = segmentGeometry;
                segmentUpdated = true;
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

        #region Protected Methods

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

        void ResetStrokeShapes()
        {
            strokeGeometry = new PathGeometry();
            strokeFigure = new PathFigure();
            strokePolyLine = new PolyLineSegment();
            strokePath = new Path();
        }

        /// <summary>
        /// Called to add the stroke for area series.
        /// </summary>
        /// <param name="startPoint"></param>
        private void AddStroke(Point startPoint)
        {
            if (segmentCanvas.Children.Count > 1)
                segmentCanvas.Children.RemoveAt(1); //remove the existing stroke path 
            segPath.StrokeThickness = 0;

            strokePath = new Path();

            strokeFigure.StartPoint = startPoint;

            strokeFigure.Segments.Add(strokePolyLine);
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

        internal override void Dispose()
        {
            if(segmentCanvas != null)
            {
                segmentCanvas.Children.Clear();
                segmentCanvas.Tag = null;
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

        #endregion
    }
}
