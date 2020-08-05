using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{

    /// <summary>
    /// Represents chart stacking area segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="StackingAreaSeries"/>
   
    public partial class StackingAreaSegment:ChartSegment
    {
        #region Fields

        #region Private Fields

        bool segmentUpdated;

        bool isEmpty;

        private List<double> XValues;

        private List<double> YValues;

        private StackingAreaSeries containerSeries;

        private Canvas segmentCanvas;

        private Path segPath;

        PathGeometry strokeGeometry;

        PathFigure strokeFigure;

        PolyLineSegment strokePolyLine;

        Path strokePath;

        private double _xData;

        private double _yData;

        #endregion

        #endregion

        #region Constructors

        public StackingAreaSegment()
        {

        }

        /// <summary>
        /// Called when instance created for StackingAreaSegment
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="series"></param>
        public StackingAreaSegment(List<double> xValues, List<double> yValues, StackingAreaSeries series)
        {
            containerSeries = series;
        }

        /// <summary>
        /// Called when instance created for StackingAreaSegment
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        public StackingAreaSegment(List<double> xValues, List<double> yValues)
        {
        }

        #endregion

        #region Properties

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
            return null;
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        public override void SetData(IList<double> xValues, IList<double> yValues)
        {
            XValues = xValues as List<double>;
            YValues = yValues as List<double>;
            double xMax = xValues.Max();
            double yMax = yValues.Max();
            double xMin = xValues.Min();
            double _Min = YValues.Min();
            isEmpty = double.IsNaN(_Min);
            double yMin = isEmpty ? YValues.Where(e => !double.IsNaN(e)).Min() : _Min;
            XRange = new DoubleRange(xMin, xMax);
            YRange = new DoubleRange(yMin, yMax);
            if (!isEmpty && segPath != null)
            {
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("StrokeThickness");
                segPath.SetBinding(Path.StrokeThicknessProperty, binding);
            }
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            if (transformer != null)
            {
                if (segmentUpdated)
                    Series.SeriesRootPanel.Clip = null;
                PathFigure figure = new PathFigure();
                PathGeometry segmentGeometry = new PathGeometry();
                WindowsLinesegment lineSegment;
                double origin = containerSeries.ActualXAxis.Origin;
                if (containerSeries.ActualXAxis != null && containerSeries.ActualXAxis.Origin == 0
                    && containerSeries.ActualYAxis is LogarithmicAxis && (containerSeries.ActualYAxis as LogarithmicAxis).Minimum != null)
                    origin = (double)(containerSeries.ActualYAxis as LogarithmicAxis).Minimum;
                figure.StartPoint = transformer.TransformToVisible(XValues[0], origin);

                strokeGeometry = new PathGeometry();
                strokeFigure = new PathFigure();
                strokePolyLine = new PolyLineSegment();
                strokePath = new Path();


                if (!containerSeries.IsClosed && !double.IsNaN(YValues[YValues.Count / 2]))
                {
                    AddStroke(transformer.TransformToVisible(XValues[YValues.Count / 2], YValues[YValues.Count / 2]));
                }
                else if (isEmpty)
                {
                    AddStroke(transformer.TransformToVisible(XValues[YValues.Count / 2 - 1], YValues[YValues.Count / 2 - 1]));
                }


                for (int index = 0; index < XValues.Count; index++)
                {
                    if (!double.IsNaN(YValues[index]))
                    {
                        Point point = transformer.TransformToVisible(XValues[index], YValues[index]);
                        lineSegment = new WindowsLinesegment();
                        lineSegment.Point = point;
                        figure.Segments.Add(lineSegment);
                        if ((isEmpty && !Series.ShowEmptyPoints) || !containerSeries.IsClosed)
                        {
                            if (index > (int)(YValues.Count / 2 - 1))
                            {
                                if (double.IsNaN(YValues[index - 1]))
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
                    }
                    else if (XValues.Count > 1)
                    {
                        if (index > 0 && index < XValues.Count - 1)
                        {
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index - 1], origin);
                            figure.Segments.Add(lineSegment);

                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index], origin);
                            figure.Segments.Add(lineSegment);

                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index + 1], origin);
                            figure.Segments.Add(lineSegment);
                        }
                        else if (index == YValues.Count - 1)
                        {
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index - 1], origin);

                            figure.Segments.Add(lineSegment);
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index], origin);
                            figure.Segments.Add(lineSegment);
                        }
                        else
                        {
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index], origin);
                            figure.Segments.Add(lineSegment);
                            lineSegment = new WindowsLinesegment();
                            lineSegment.Point = transformer.TransformToVisible(XValues[index + 1], origin);

                            figure.Segments.Add(lineSegment);
                        }

                    }
                }

                lineSegment = new WindowsLinesegment();
                lineSegment.Point = transformer.TransformToVisible(XValues[XValues.Count - 1], origin);
                figure.Segments.Add(lineSegment);

                if (containerSeries.IsClosed && isEmpty && !double.IsNaN(YValues[YValues.Count - 1]))
                    strokePolyLine.Points.Add(lineSegment.Point);
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
        /// Called to add the stroke for stacking area series.
        /// </summary>
        /// <param name="startPoint"></param>
        private void AddStroke(Point startPoint)
        {
            if (segmentCanvas.Children.Count > 1)
                segmentCanvas.Children.RemoveAt(1); //remove the existing stroke path 
            segPath.StrokeThickness = 0;

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
