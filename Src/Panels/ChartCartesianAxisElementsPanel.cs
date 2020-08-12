// <copyright file="ChartCartesianAxisElementsPanel.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections.Generic;
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

    /// <summary>
    /// Represents ChartCartesianAxisElementsPanel.
    /// </summary>
    /// <remarks>
    /// The elements inside the panel comprises of <see cref="ChartAxis"/> axis line, major ticklines and minor ticklines.
    /// </remarks>
    public partial class ChartCartesianAxisElementsPanel : ILayoutCalculator
    {
        #region Fields

        private ChartAxis axis;

        private UIElementsRecycler<Line> majorTicksRecycler;

        private UIElementsRecycler<Line> minorTicksRecycler;

        private Size desiredSize;

        private Panel labelsPanels;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCartesianAxisElementsPanel"/> class.
        /// </summary>
        /// <param name="panel">The Panel</param>
        public ChartCartesianAxisElementsPanel(Panel panel)
        {
            this.labelsPanels = panel;
            MainAxisLine = new Line();
            if (panel != null)
                panel.Children.Add(MainAxisLine);
            majorTicksRecycler = new UIElementsRecycler<Line>(panel);
            minorTicksRecycler = new UIElementsRecycler<Line>(panel);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public double Left
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public double Top
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <value>
        /// The panel.
        /// </value>
        public Panel Panel
        {
            get { return labelsPanels; }
        }

        /// <summary>
        /// Gets the desired size.
        /// </summary>
        public Size DesiredSize
        {
            get
            {
                return desiredSize;
            }
        }

        /// <summary>
        /// Gets the Children count in the panel.
        /// </summary>
        public List<UIElement> Children
        {
            get
            {
                if (labelsPanels != null)
                {
                    return labelsPanels.Children.Cast<UIElement>().ToList();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        internal ChartAxis Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
                SetAxisLineBinding();
            }
        }

        /// <summary>
        /// Gets or sets the main axis line.
        /// </summary>
        internal Line MainAxisLine { get; set; }

        /// <summary>
        /// Gets the desired size of the panel.
        /// </summary>

        #endregion

        #region Methods

        #region Public Methods
            
        /// <summary>
        /// Method declaration for Measure
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the size available for arranging the elements.</returns>
        public Size Measure(Size availableSize)
        {
            Size size = Size.Empty;
            double smallTickLineSize = (Axis is CategoryAxis || Axis is DateTimeCategoryAxis) ? 5 : (Axis as RangeAxisBase).SmallTickLineSize;
            if (Axis.Orientation == Orientation.Horizontal)
            {
                size = new Size(availableSize.Width, Math.Max(Math.Max(Axis.TickLineSize, smallTickLineSize), 0) + MainAxisLine.StrokeThickness);
            }
            else
            {
                size = new Size(Math.Max(Math.Max(Axis.TickLineSize, smallTickLineSize), 0) + MainAxisLine.StrokeThickness, availableSize.Height);
            }

            desiredSize = size;

            return size;
        }

        /// <summary>
        /// Seek the elements.
        /// </summary>
        public void DetachElements()
        {
            if (MainAxisLine != null && labelsPanels != null && labelsPanels.Children != null
                && labelsPanels.Children.Contains(MainAxisLine))
                labelsPanels.Children.Remove(MainAxisLine);
            if (majorTicksRecycler != null)
                majorTicksRecycler.Clear();

            if (minorTicksRecycler != null)
                minorTicksRecycler.Clear();

            labelsPanels = null;
        }

        /// <summary>
        /// Method declaration for Arrange
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        /// <returns>Returns the arranged size.</returns>
        public Size Arrange(Size finalSize)
        {
            double[] values = (from val in Axis.VisibleLabels
                               select val.Position).ToArray();

            if (Axis.Area is SfChart)
            {
                RenderAxisLine(finalSize);
                if (Axis is CategoryAxis && (Axis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks)
                    values = (from val in Axis.SmallTickPoints
                              select val).ToArray();
                RenderTicks(finalSize, majorTicksRecycler, Axis.Orientation, Axis.TickLineSize, Axis.TickLinesPosition, values);

                if (Axis.smallTicksRequired)
                {
                    values = (from val in Axis.SmallTickPoints
                              select val).ToArray();
                    RenderTicks(finalSize, minorTicksRecycler, Axis.Orientation, (Axis as RangeAxisBase).SmallTickLineSize, (Axis as RangeAxisBase).SmallTickLinesPosition, values);
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Method declaration for UpdateElements
        /// </summary>
        public void UpdateElements()
        {
            UpdateTicks();
        }

        #endregion

        #region Internal Methods

        internal void Dispose()
        {
            axis = null;

            if (majorTicksRecycler != null && majorTicksRecycler.Count > 0)
            {
                majorTicksRecycler.Clear();
                majorTicksRecycler = null;
            }

            if (minorTicksRecycler != null && minorTicksRecycler.Count > 0)
            {
                minorTicksRecycler.Clear();
                minorTicksRecycler = null;
            }
        }

        /// <summary>
        /// Updates the tick lines.
        /// </summary>
        internal void UpdateTicks()
        {
            int tickCount = Axis.SmallTickPoints.Count;
            if ((!(Axis is CategoryAxis && (Axis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks)))
                tickCount = Axis.VisibleLabels.Count;

            UpdateTicks(tickCount, majorTicksRecycler, "MajorTickLineStyle");

            if (Axis.smallTicksRequired)
                UpdateTicks(Axis.SmallTickPoints.Count, minorTicksRecycler, "MinorTickLineStyle");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Binds the axis line style with the <see cref="Line"/> <see cref="Shape"/> .
        /// </summary>
        private void SetAxisLineBinding()
        {
            Binding binding = new Binding();
            binding.Source = axis;
            binding.Path = new PropertyPath("AxisLineStyle");
            MainAxisLine.SetBinding(Line.StyleProperty, binding);
        }

        /// <summary>
        /// Updates the tick lines.
        /// </summary>
        /// <param name="linescount">The Tick Lines Count</param>
        /// <param name="lineRecycler">The Tick Lines Recycler</param>
        /// <param name="lineStylePath">The Tick Lines Style</param>
        private void UpdateTicks(int linescount, UIElementsRecycler<Line> lineRecycler, String lineStylePath)
        {
            int linesCount = linescount;

            foreach (var line in lineRecycler)
            {
                line.ClearValue(FrameworkElement.StyleProperty);
            }

            if (!lineRecycler.BindingProvider.Keys.Contains(Line.StyleProperty))
            {
                lineRecycler.BindingProvider.Add(Line.StyleProperty, GetTickLineBinding(Axis, lineStylePath));
                lineRecycler.BindingProvider.Add(Line.VisibilityProperty, GetTickLineBinding(Axis, "Visibility"));
            }

			if (lineRecycler.Count > 0)
            {
                foreach (var line in lineRecycler)
                {
                    line.SetBinding(FrameworkElement.StyleProperty, GetTickLineBinding(Axis, lineStylePath));
                    line.SetBinding(Line.VisibilityProperty, GetTickLineBinding(Axis, "Visibility"));
                }
            }
			
            lineRecycler.GenerateElements(linesCount);
            var rangeStyles = axis.RangeStyles;
            if (rangeStyles != null && rangeStyles.Count > 0)
            {
                var values = !lineStylePath.Equals("MajorTickLineStyle") ? axis.SmallTickPoints :
                    (from label in axis.VisibleLabels select label.Position).ToList();

                for (int i = 0; i < values.Count; i++)
                {
                    foreach (var chartAxisRangeStyle in rangeStyles)
                    {
                        var range = chartAxisRangeStyle.Range;
                        var style = lineStylePath.Equals("MajorTickLineStyle") ? chartAxisRangeStyle.MajorTickLineStyle : chartAxisRangeStyle.MinorTickLineStyle;
                        if (range.Start <= values[i] && range.End >= values[i] && style != null)
                        {
                            lineRecycler[i].SetBinding(FrameworkElement.StyleProperty, GetTickLineBinding(chartAxisRangeStyle, lineStylePath));
                            lineRecycler[i].SetBinding(Line.VisibilityProperty, GetTickLineBinding(axis, "Visibility"));
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Renders the axis line.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        private void RenderAxisLine(Size finalSize)
        {
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            double width = finalSize.Width;
            double height = finalSize.Height;
            double strokeHalfFactor = MainAxisLine.StrokeThickness / 2;
            bool isTickInside = Axis.TickLinesPosition == AxisElementPosition.Inside;
            Orientation orientaion = Axis.Orientation;
            bool isOpposed = Axis.OpposedPosition;

            if (orientaion == Orientation.Horizontal)
            {
                x1 = this.Axis.AxisLineOffset;
                x2 = width - this.Axis.AxisLineOffset;

                // Positioning axis line according to it's corner, not to it's center.
                y2 = y1 = isOpposed ? isTickInside ? -strokeHalfFactor : height - strokeHalfFactor
                                    : isTickInside ? y1 = strokeHalfFactor + height : strokeHalfFactor;
            }
            else
            {
                // Positioning axis line according to it's corner, not to it's center.
                x2 = x1 = isOpposed ? isTickInside ? strokeHalfFactor + width : strokeHalfFactor
                                    : isTickInside ? -strokeHalfFactor : width - strokeHalfFactor;
                y1 = this.Axis.AxisLineOffset;
                y2 = height - this.Axis.AxisLineOffset;
            }

            if (MainAxisLine != null)
            {
                MainAxisLine.X1 = x1;
                MainAxisLine.Y1 = y1;
                MainAxisLine.X2 = x2;
                MainAxisLine.Y2 = y2;
            }
        }

        /// <summary>
        /// Renders the tick lines.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        /// <param name="linesRecycler">The Line Recycler</param>
        /// <param name="orientation">The Orientation</param>
        /// <param name="tickSize">The Tick Size</param>
        /// <param name="tickPosition">The Tick Postion</param>
        /// <param name="Values">The Values</param>
        private void RenderTicks(
            Size finalSize,
            UIElementsRecycler<Line> linesRecycler,
            Orientation orientation,
            double tickSize,
            AxisElementPosition tickPosition,
            double[] Values)
        {

            int labelsCount = Values.Length;
            int linesCount = linesRecycler.Count;
            double width = finalSize.Width;
            double height = finalSize.Height;

            for (int i = 0; i < labelsCount; i++)
            {
                if (i < linesCount)
                {
                    double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    Line line = linesRecycler[i];
                    double value = this.Axis.ValueToCoefficientCalc(Values[i]);
                    value = double.IsNaN(value) ? 0 : value;

                    if (orientation == Orientation.Horizontal)
                    {
                        x1 = x2 = Axis.ActualPlotOffset + Math.Round((this.Axis.RenderedRect.Width * value));
                    }
                    else
                    {
                        y1 = y2 = (Axis.ActualPlotOffset + Math.Round(this.Axis.RenderedRect.Height * (1 - value)) + 0.5);
                    }

                    CalculatePosition(tickPosition, tickSize, width, height, ref x1, ref y1, ref x2, ref y2);

                    line.X1 = x1;
                    line.X2 = x2;
                    line.Y1 = y1;
                    line.Y2 = y2;
                }
            }
        }

        /// <summary>
        /// Calcuates the tick position.
        /// </summary>
        /// <param name="ticksPosition">The Tick Position</param>
        /// <param name="tickSize">The Tick Size</param>
        /// <param name="width">The Weight</param>
        /// <param name="height">The Height</param>
        /// <param name="x1">The x 1 value</param>
        /// <param name="y1">The y 1 value</param>
        /// <param name="x2">The x 2 value</param>
        /// <param name="y2">The y 2 value</param>
        private void CalculatePosition(
            AxisElementPosition ticksPosition,
            double tickSize,
            double width,
            double height,
            ref double x1,
            ref double y1,
            ref double x2,
            ref double y2)
        {
            Orientation orientaion = Axis.Orientation;
            bool isOpposed = Axis.OpposedPosition;
            double strokeHalfFactor = MainAxisLine.StrokeThickness / 2;
            bool isTickInside = ticksPosition == AxisElementPosition.Inside;

            // Positioning ticksize according to the axis stroke thickness size.
            if (orientaion == Orientation.Horizontal)
            {
                y1 = isOpposed ? !isTickInside ? MainAxisLine.Y1 - strokeHalfFactor : MainAxisLine.Y1 + strokeHalfFactor
                             : isTickInside ? MainAxisLine.Y1 - strokeHalfFactor : MainAxisLine.Y1 + strokeHalfFactor;

                y2 = isOpposed ? !isTickInside ? y1 - tickSize : y1 + tickSize
                             : isTickInside ? y1 - tickSize : y1 + tickSize;
            }
            else
            {
                x1 = isOpposed ? !isTickInside ? MainAxisLine.X1 + strokeHalfFactor : MainAxisLine.X1 - strokeHalfFactor
                              : isTickInside ? MainAxisLine.X1 + strokeHalfFactor : MainAxisLine.X1 - strokeHalfFactor;
                x2 = isOpposed ? !isTickInside ? x1 + tickSize : x1 - tickSize
                              : isTickInside ? x1 + tickSize : x1 - tickSize;
            }
        }


		private Binding GetTickLineBinding(object source, string propertyPath)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyPath);
            binding.Source = source;
            return binding;
        }
		
        #endregion

        #endregion
    }
}
