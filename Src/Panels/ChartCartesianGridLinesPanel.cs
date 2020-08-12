// <copyright file="ChartCartesianGridLinesPanel.cs" company="Syncfusion. Inc">
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
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents <see cref="ChartCartesianGridLinesPanel"/>.
    /// </summary>
    public partial class ChartCartesianGridLinesPanel : ILayoutCalculator
    {
        #region Fields

        private Panel panel;

        private Size desiredSize;

        private readonly UIElementsRecycler<Border> stripLines;

        private Line xOriginLine;

        private Line yOriginLine;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCartesianGridLinesPanel"/> class.
        /// </summary>
        /// <param name="panel">The Panel</param>
        /// <exception cref="ArgumentNullException"><see cref="ArgumentNullException"/> may be thrown</exception>
        public ChartCartesianGridLinesPanel(Panel panel)
        {
            // if (panel == null)
            //    throw new ArgumentNullException();

            this.panel = panel;

            stripLines = new UIElementsRecycler<Border>(this.panel);
            xOriginLine = new Line();
            yOriginLine = new Line();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the desired size of the panel.
        /// </summary>
        public Size DesiredSize
        {
            get { return desiredSize; }
        }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <value>
        /// The panel.
        /// </value>
        public Panel Panel
        {
            get { return panel; }
        }

        /// <summary>
        /// Gets the children count in the panel.
        /// </summary>
        public List<UIElement> Children
        {
            get
            {
                if (panel != null)
                {
                    return panel.Children.Cast<UIElement>().ToList();
                }

                return null;
            }
        }

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

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the chart area.
        /// </summary>
        internal ChartBase Area { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods
        
        /// <summary>
        /// Draws the Gridlines at definite intervals in <see cref="ChartAxis"/>
        /// </summary>
        /// <param name="axis">Relevant ChartAxis</param>
        public void DrawGridLines(ChartAxis axis)
        {
            if (axis == null)
                return;

            double left = axis.RenderedRect.Left - Area.AxisThickness.Left;
            double right = axis.RenderedRect.Right - Area.AxisThickness.Left;
            double top = axis.RenderedRect.Top - Area.AxisThickness.Top;
            double bottom = axis.RenderedRect.Bottom - Area.AxisThickness.Top;

            double width = 0d;
            double height = 0d;


            var values = (from label in axis.VisibleLabels
                          select label.Position).ToArray();

            var categoryAxis = axis as CategoryAxis;
            if (categoryAxis != null && categoryAxis.LabelPlacement == LabelPlacement.BetweenTicks)
                values = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();

            if (axis.Orientation == Orientation.Horizontal)
            {
                width = right - left;
                IEnumerable<ChartAxis> selectedAxes = null;
                if (axis.RegisteredSeries.Count > 0)
                {
                    selectedAxes = Area.RowDefinitions.Count > 1 ? axis.AssociatedAxes : axis.AssociatedAxes.DistinctBy(Area.GetActualRow);
                }
                else
                {
                    if (Area.InternalPrimaryAxis != null)
                        selectedAxes = new List<ChartAxis> { Area.InternalSecondaryAxis };
                }

                int index = 0;
                int smallTickIndex = 0;
                if (selectedAxes != null)
                    foreach (ChartAxis supportAxis in selectedAxes)
                    {
                        if (supportAxis == null)
                            continue;
                        var count = axis.RegisteredSeries.Where(item => (item.ActualXAxis == supportAxis || item.ActualYAxis == supportAxis)).Count();
                        if (count == 0) count = 1;
                        if (count > 0)
                        {
                            top = supportAxis.ArrangeRect.Top - Area.AxisThickness.Top;
                            height = top + supportAxis.ArrangeRect.Height;
                            if (values.Length > 0)
                                DrawGridLines(axis, axis.GridLinesRecycler, left, top, width, height, values, true, index);
                            if (axis.smallTicksRequired)
                            {
                                var smallTickvalues = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();
                                if (smallTickvalues.Length > 0)
                                    DrawGridLines(axis, axis.MinorGridLinesRecycler, left, top, width, height, smallTickvalues, false, smallTickIndex);
                                smallTickIndex += smallTickvalues.Length;
                            }
                            index += values.Length;
                        }
                    }
            }
            else
            {
                height = bottom - top;

                IEnumerable<ChartAxis> selectedAxes = null;
                if (axis.RegisteredSeries.Count > 0)
                {
                    selectedAxes = Area.ColumnDefinitions.Count > 1 ? axis.AssociatedAxes : axis.AssociatedAxes.DistinctBy(Area.GetActualColumn);
                }
                else
                {
                    if (Area.InternalPrimaryAxis != null)
                        selectedAxes = new List<ChartAxis> { Area.InternalPrimaryAxis };
                }

                int index = 0;
                int smallTickIndex = 0;
                if (selectedAxes != null)
                    foreach (ChartAxis supportAxis in selectedAxes)
                    {
                        var count = axis.RegisteredSeries.Where(item => (item.ActualXAxis == supportAxis || item.ActualYAxis == supportAxis)).Count();
                        if (count == 0) count = 1;
                        if (count > 0)
                        {
                            left = supportAxis.ArrangeRect.Left - Area.AxisThickness.Left;
                            width = left + supportAxis.ArrangeRect.Width;
                            if (values.Length > 0)
                                DrawGridLines(axis, axis.GridLinesRecycler, left, top, width, height, values, true, index);
                            if (axis.smallTicksRequired)
                            {
                                var smallTickvalues = (from pointValues in axis.SmallTickPoints select pointValues).ToArray();
                                if (smallTickvalues.Length > 0)
                                    DrawGridLines(axis, axis.MinorGridLinesRecycler, left, top, width, height, smallTickvalues, false, smallTickIndex);
                                smallTickIndex += smallTickvalues.Length;
                            }
                            index += values.Length;
                        }
                    }
            }
        }



        /// <summary>
        /// Measures the elements in the panel.
        /// </summary>
        /// <param name="availableSize">Available size of the panel.</param>
        /// <returns>Returns Size</returns>
        public Size Measure(Size availableSize)
        {
            desiredSize = availableSize;
            return availableSize;
        }

        /// <summary>
        /// Arrranges the elements inside a panel.
        /// </summary>
        /// <param name="finalSize">final size of the panel.</param>
        /// <returns>Returns Size</returns>
        public Size Arrange(Size finalSize)
        {
            foreach (ChartAxis axis in Area.Axes)
            {
                if (axis.ShowGridLines)
                    DrawGridLines(axis);
            }
            return finalSize;
        }


        /// <summary>
        /// Seek the elements.
        /// </summary>
        public void DetachElements()
        {
            panel.Children.Clear();
            panel = null;

            if (stripLines != null)
                stripLines.Clear();
        }

        /// <summary>
        /// Adds the elements in the panel.
        /// </summary>
        public void UpdateElements()
        {
            foreach (ChartAxis axis in Area.Axes)
            {
                UpdateGridLines(axis);
            }

            UpdateStripLines();
        }

        /// <summary>
        /// Adds the Gridlines for the axis.
        /// </summary>
        /// <param name="axis">The Axis</param>       
        public void UpdateGridLines(ChartAxis axis)
        {
            if (axis == null)
                return;
            if (axis.GridLinesRecycler == null)
                axis.CreateLineRecycler();
            int axesCount = 1;
            if (axis.RegisteredSeries.Count > 0)
            {
                axesCount = axis.Orientation == Orientation.Horizontal
                    ? Area.RowDefinitions.Count > 1 ? axis.AssociatedAxes.Count : (axis.AssociatedAxes.DistinctBy(Area.GetActualRow)).Count()
                    : Area.ColumnDefinitions.Count > 1 ? axis.AssociatedAxes.Count : (axis.AssociatedAxes.DistinctBy(Area.GetActualColumn)).Count();
            }

            int tickCount = axis.SmallTickPoints.Count * axesCount;

            var categoryAxis = axis as CategoryAxis;
            if (!(categoryAxis != null && categoryAxis.LabelPlacement == LabelPlacement.BetweenTicks))
                tickCount = axis.VisibleLabels.Count * axesCount;

            if (axis.smallTicksRequired)
                ChartCartesianGridLinesPanel.UpdateGridlines(axis, axis.MinorGridLinesRecycler, axis.SmallTickPoints.Count * axesCount, false, false);

            ChartCartesianGridLinesPanel.UpdateGridlines(axis, axis.GridLinesRecycler, tickCount, true, true);                      
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Updates the strip lines.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal void UpdateStripLines()
        {

            stripLines.Clear();

            foreach (
                ChartAxisBase2D axis in
                    Area.Axes.Where(
                        axis =>
                            axis != null && !axis.VisibleRange.IsEmpty && (axis as ChartAxisBase2D).StripLines != null && (axis as ChartAxisBase2D).StripLines.Count > 0))
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    UpdateHorizontalStripLine(axis);
                }
                else
                {
                    UpdateVerticalStripLine(axis);
                }
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the gridlines.
        /// </summary>
        /// <param name="axis">The Relevant Axis</param>
        /// <param name="linesRecycler">The Line Recycler</param>
        /// <param name="requiredLinescount">The Required Lines Count</param>
        /// <param name="isMajor">Check For Marjor Axis</param>
        /// <param name="checkOrginFlag">Check For Origin Flag</param>
        private static void UpdateGridlines(ChartAxis axis, UIElementsRecycler<Line> linesRecycler, int requiredLinescount, bool isMajor, bool checkOrginFlag)
        {
            if (linesRecycler == null || axis == null)
                return;

            foreach (var line in linesRecycler)
            {
                line.ClearValue(FrameworkElement.StyleProperty);
            }

            int totalLinesCount = requiredLinescount;

            if (axis.ShowOrigin && checkOrginFlag)
                totalLinesCount += 1;

            if (!linesRecycler.BindingProvider.Keys.Contains(FrameworkElement.StyleProperty))
            {
                linesRecycler.BindingProvider.Add(FrameworkElement.StyleProperty, GetGridLineBinding(axis, isMajor));
            }

			if (linesRecycler.Count > 0)
            {
                foreach (var line in linesRecycler)
                {
                    line.SetBinding(FrameworkElement.StyleProperty, GetGridLineBinding(axis, isMajor));
                }
            }
			
            linesRecycler.GenerateElements(totalLinesCount);
            var rangeStyles = axis.RangeStyles;
            if (rangeStyles != null && rangeStyles.Count > 0)
            {
                var values = !isMajor ? axis.SmallTickPoints :
                    (from label in axis.VisibleLabels select label.Position).ToList();

                for (int i = 0; i < values.Count; i++)
                {
                    foreach (var chartAxisRangeStyle in rangeStyles)
                    {
                        var range = chartAxisRangeStyle.Range;
                        var style = isMajor ? chartAxisRangeStyle.MajorGridLineStyle : chartAxisRangeStyle.MinorGridLineStyle;
                        if (range.Start <= values[i] && range.End >= values[i] && style != null)
                        {
							linesRecycler[i].SetBinding(FrameworkElement.StyleProperty, GetGridLineBinding(chartAxisRangeStyle, isMajor));
                            break;
                        }
                    }
                }
            }

            //StrokeDashArray applied only for the first line element when it is applied through style. 
            //It is bug in the framework.
            //And hence manually setting stroke dash array for each and every grid line.
            ChartExtensionUtils.SetStrokeDashArray(linesRecycler);
        }

        /// <summary>
        /// Creates the binding provider with the specifed path and source.
        /// </summary>
        /// <param name="path">The Path</param>
        /// <param name="source">The Source</param>
        /// <returns>Returns the binding provider.</returns>
        private static Binding CreateBinding(string path, object source)
        {
            var bindingProvider = new Binding
            {
                Path = new PropertyPath(path),
                Source = source,
                Mode = BindingMode.OneWay
            };
            return bindingProvider;
        }
		
		private static Binding GetGridLineBinding(object source, bool isMajor)
        {
            Binding binding = new Binding();
            binding.Path = isMajor ? new PropertyPath("MajorGridLineStyle") :
                new PropertyPath("MinorGridLineStyle");
            binding.Source = source;
            return binding;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Draws the gridlines with the specified values.
        /// </summary>
        /// <param name="axis">The Axis</param>
        /// <param name="lines">The Lines</param>
        /// <param name="left">The Left</param>
        /// <param name="top">The Top</param>
        /// <param name="width">The Width</param>
        /// <param name="height">The Height</param>
        /// <param name="values">The Values</param>
        /// <param name="drawOrigin">Check For Draw Origin</param>
        /// <param name="index">The Index</param>
        private void DrawGridLines(ChartAxis axis, UIElementsRecycler<Line> lines, double left, double top, double width, double height, double[] values, bool drawOrigin, int index)
        {
            if (lines == null || axis == null)
                return;

            int labelsCount = values.Length;
            int linesCount = lines.Count;
            Orientation orienatation = axis.Orientation;
            if (orienatation == Orientation.Horizontal)
            {
                int i;
                for (i = 0; i < labelsCount; i++)
                {
                    if (i < linesCount)
                    {
                        Line line = lines[index];
                        double value = axis.ValueToCoefficientCalc(values[i]);
                        value = double.IsNaN(value) ? 0 : value;
                        //line.X1 = (value=Math.Round(width * value) + 0.5)>width ? value-0.5:value;
                        line.X1 = (Math.Round(width * value)) + left;
                        line.Y1 = top;
                        line.X2 = line.X1;
                        line.Y2 = height;
                    }
                    index++;
                }

                if (!axis.ShowOrigin && panel.Children.Contains(xOriginLine))
                    panel.Children.Remove(xOriginLine);
                if (axis.ShowOrigin && drawOrigin)
                {
                    double value = Area.InternalSecondaryAxis.ValueToCoefficientCalc(axis.Origin);
                    value = double.IsNaN(value) ? 0 : value;
                    xOriginLine.X1 = 0;
                    xOriginLine.Y1 = xOriginLine.Y2 = Math.Round(height * (1 - value));
                    xOriginLine.X2 = width;
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("OriginLineStyle");
                    binding.Source = axis;
                    xOriginLine.SetBinding(Line.StyleProperty, binding);
                    if (!panel.Children.Contains(xOriginLine))
                        panel.Children.Add(xOriginLine);
                }
            }
            else
            {
                int i;
                for (i = 0; i < labelsCount; i++)
                {
                    if (i < linesCount)
                    {
                        Line line = lines[index];
                        double value = axis.ValueToCoefficientCalc(values[i]);
                        value = double.IsNaN(value) ? 0 : value;
                        line.X1 = left;
                        line.Y1 = Math.Round(height * (1 - value)) + 0.5 + top;
                        line.X2 = width;
                        line.Y2 = line.Y1;
                    }
                    index++;
                }

                if (!axis.ShowOrigin && panel.Children.Contains(yOriginLine))
                    panel.Children.Remove(yOriginLine);

                if (axis.ShowOrigin && axis.VisibleRange.Delta > 0 && drawOrigin)
                {
                    double value = Area.InternalPrimaryAxis.ValueToCoefficientCalc(axis.Origin);
                    value = double.IsNaN(value) ? 0 : value;
                    yOriginLine.X1 = yOriginLine.X2 = Math.Round(width * (value));
                    yOriginLine.Y1 = 0;
                    yOriginLine.Y2 = height;
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("OriginLineStyle");
                    binding.Source = axis;
                    yOriginLine.SetBinding(Line.StyleProperty, binding);
                    if (!panel.Children.Contains(yOriginLine))
                        panel.Children.Add(yOriginLine);
                }
            }
        }


        /// <summary>
        /// Renders the stripline.
        /// </summary>
        /// <param name="stripRect">The Strip <see cref="Rect"/></param>
        /// <param name="stripLine">The Strip Line</param>
        private void RenderStripLine(Rect stripRect, ChartStripLine stripLine)
        {
            if (stripRect.IsEmpty) return;
            var border = stripLines.CreateNewInstance();
            var control = new ContentControl();

            border.SetBinding(Border.BackgroundProperty, ChartCartesianGridLinesPanel.CreateBinding("Background", stripLine));
            border.SetBinding(Border.BorderBrushProperty, ChartCartesianGridLinesPanel.CreateBinding("BorderBrush", stripLine));
            border.SetBinding(Border.BorderThicknessProperty, ChartCartesianGridLinesPanel.CreateBinding("BorderThickness", stripLine));
            border.SetBinding(Border.OpacityProperty, ChartCartesianGridLinesPanel.CreateBinding("Opacity", stripLine));
            border.SetBinding(Border.VisibilityProperty, ChartCartesianGridLinesPanel.CreateBinding("Visibility", stripLine));
            control.SetBinding(ContentControl.ContentProperty, ChartCartesianGridLinesPanel.CreateBinding("Label", stripLine));
            control.SetBinding(ContentControl.ContentTemplateProperty, ChartCartesianGridLinesPanel.CreateBinding("LabelTemplate", stripLine));
            control.SetBinding(ContentControl.OpacityProperty, ChartCartesianGridLinesPanel.CreateBinding("Opacity", stripLine));
            control.RenderTransformOrigin = new Point(0.5, 0.5);
            control.RenderTransform = new RotateTransform
            {
                Angle = stripLine.LabelAngle
            };
            control.SetBinding(FrameworkElement.HorizontalAlignmentProperty, ChartCartesianGridLinesPanel.CreateBinding("LabelHorizontalAlignment", stripLine));
            control.SetBinding(FrameworkElement.VerticalAlignmentProperty, ChartCartesianGridLinesPanel.CreateBinding("LabelVerticalAlignment", stripLine));

            border.Child = control;

            border.Width = stripRect.Width;
            border.Height = stripRect.Height;
            Canvas.SetLeft(border, double.IsNaN(stripRect.Left) ? 0 : stripRect.Left);
            Canvas.SetTop(border, double.IsNaN(stripRect.Top) ? 0 : stripRect.Top);
        }

        /// <summary>
        /// Updates the horizontal stripline.
        /// </summary>
        /// <param name="axis">The Relevant Axis</param>
        private void UpdateHorizontalStripLine(ChartAxisBase2D axis)
        {
            var seriesRect = Area.SeriesClipRect;
            var visibleRange = axis.VisibleRange;
            Rect xAxisRect = axis.ArrangeRect;
            axis.StriplineXRange = DoubleRange.Empty;
            axis.StriplineYRange = DoubleRange.Empty;
            foreach (var stripLine in axis.StripLines)
            {
                IEnumerable<ChartAxis> yAxes;
                ChartAxis cusAxis = Area.Axes[stripLine.SegmentAxisName];
                if (cusAxis != null)
                {
                    yAxes = new List<ChartAxis> { cusAxis };
                }
                else if (axis.RegisteredSeries.Count > 0)
                {
                    yAxes = (Area.RowDefinitions.Count > 1) ? axis.AssociatedAxes : axis.AssociatedAxes.DistinctBy(Area.GetActualRow); //WPF-13976-StripLine not working while using RowDefinitions              
                }
                else
                {
                    yAxes = new List<ChartAxis> { Area.InternalSecondaryAxis };
                }

                foreach (var currAxis in yAxes)
                {
                    var count = axis.RegisteredSeries.Where(item => (item.ActualXAxis == currAxis || item.ActualYAxis == currAxis)).Count(); //stripline drawn for all the yaxis in the chart area .hence it is filtered using its associated axis -WPF-13976
                    if (count == 0 && axis.RegisteredSeries.Count == 0) count = 1;
                    if (count > 0)
                    {
                        var yAxisRect = currAxis.ArrangeRect;
                        Rect stripRect;
                        if (!stripLine.IsSegmented)
                        {
                            double startStrip = stripLine.Start;
                            double endStrip = double.IsNaN(stripLine.RepeatUntil)
                                ? visibleRange.End
                                : stripLine.RepeatUntil;
                            double periodStrip = stripLine.RepeatEvery;

                            do
                            {
                                double stripStart = startStrip;
                                double stripEnd = (!double.IsNaN(stripLine.RepeatUntil) ? (startStrip < stripLine.RepeatUntil) ? startStrip + stripLine.Width
                                            : startStrip - stripLine.Width : startStrip + stripLine.Width);
                                double x1 = Area.ValueToPoint(axis, startStrip);

                                double x2;
                                if (stripLine.IsPixelWidth)
                                {
                                    x2 = x1 + stripLine.Width;

                                }
                                else
                                {
                                    x2 = Area.ValueToPoint(axis, stripEnd);
                                }
                                axis.StriplineXRange += new DoubleRange(stripStart, Area.PointToValue(axis, new Point(x2, 0)));
                                axis.StriplineXRange += currAxis.StriplineXRange;
                                double left = xAxisRect.Width + xAxisRect.Left - seriesRect.Left;
                                x2 = left > x2 ? x2 : left;//WPF-12981-Striplines are cut when we set plotoffset to primaryaxis
                                stripRect = new Rect(new Point(x1, yAxisRect.Top - seriesRect.Top),
                                    new Point(x2, yAxisRect.Height + yAxisRect.Top - seriesRect.Top));
                                RenderStripLine(stripRect, stripLine);

                                startStrip = startStrip < endStrip ? startStrip + periodStrip : startStrip - periodStrip;
                            }
                            while (periodStrip != 0 && (Double.IsNaN(stripLine.RepeatUntil) ? startStrip < endStrip : (stripLine.Start < stripLine.RepeatUntil ? startStrip < endStrip
                                      : startStrip > endStrip)));
                        }
                        else
                        {
                            double startStrip = stripLine.Start;
                            double endStrip = double.IsNaN(stripLine.RepeatUntil)
                                ? visibleRange.End
                                : stripLine.RepeatUntil;
                            double periodStrip = stripLine.RepeatEvery;
                            if (!double.IsNaN(stripLine.SegmentStartValue) &&
                                !double.IsNaN(stripLine.SegmentEndValue))
                            {
                                do
                                {
                                    double x1 = Area.ValueToPoint(axis, startStrip);
                                    double x2;
                                    if (stripLine.IsPixelWidth)
                                    {
                                        x2 = x1 + stripLine.Width;

                                    }
                                    else
                                    {
                                        double stripEnd = (!double.IsNaN(stripLine.RepeatUntil) ? (startStrip < stripLine.RepeatUntil) ? startStrip + stripLine.Width
                                            : startStrip - stripLine.Width : startStrip + stripLine.Width);
                                        x2 = Area.ValueToPoint(axis, stripEnd);
                                    }

                                    axis.StriplineXRange += new DoubleRange(startStrip, Area.PointToValue(axis, new Point(x2, 0)));
                                    axis.StriplineXRange += currAxis.StriplineXRange;
                                    if (axis.Area != null)
                                    {
                                        double startVal = Area.ValueToPoint(currAxis,
                                            stripLine.SegmentStartValue);
                                        double endVal = Area.ValueToPoint(currAxis,
                                            stripLine.SegmentEndValue);

                                        stripRect = new Rect(new Point(x1, startVal),
                                            new Point(x2, endVal));
                                        RenderStripLine(stripRect, stripLine);
                                        if ((axis as ChartAxisBase2D).IncludeStripLineRange)
                                            currAxis.StriplineYRange += new DoubleRange(stripLine.SegmentStartValue, stripLine.SegmentEndValue);
                                    }

                                    startStrip = startStrip < endStrip ? startStrip + periodStrip : startStrip - periodStrip;
                                }
                                while (periodStrip != 0 && (Double.IsNaN(stripLine.RepeatUntil) ? startStrip < endStrip : (stripLine.Start < stripLine.RepeatUntil ? startStrip < endStrip
                                      : startStrip > endStrip)));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the vertical stripline.
        /// </summary>
        /// <param name="axis">The Axis</param>
        private void UpdateVerticalStripLine(ChartAxisBase2D axis)
        {
            var visibleRange = axis.VisibleRange;
            var seriesRect = Area.SeriesClipRect;
            axis.StriplineXRange = DoubleRange.Empty;
            axis.StriplineYRange = DoubleRange.Empty;
            foreach (var stripLine in axis.StripLines)
            {
                IEnumerable<ChartAxis> xAxes;
                ChartAxis cusAxis = Area.Axes[stripLine.SegmentAxisName];
                if (cusAxis != null)
                {
                    xAxes = new List<ChartAxis> { cusAxis };
                }
                else if (axis.RegisteredSeries.Count > 0)
                {
                    xAxes = (Area.ColumnDefinitions.Count > 1) ? axis.AssociatedAxes : axis.AssociatedAxes.DistinctBy(Area.GetActualColumn);//WPF-13976-StripLine not working while using RowDefinitions
                }
                else
                {
                    xAxes = new List<ChartAxis> { Area.InternalPrimaryAxis };
                }

                foreach (var xAxis in xAxes)
                {
                    var count = axis.RegisteredSeries.Where(item => item.ActualXAxis == xAxis || item.ActualYAxis == xAxis).Count();//stripline drawn for all the xaxis in the chart area .hence it is filtered using its associated axis- WPF-13976
                    if (count == 0 && axis.RegisteredSeries.Count == 0) count = 1;
                    if (count > 0)
                    {
                        var xAxisRect = xAxis.ArrangeRect;
                        var yAxisRect = axis.ArrangeRect;
                        Rect stripRect;
                        if (!stripLine.IsSegmented)
                        {
                            double startStrip = stripLine.Start;
                            double endStrip = double.IsNaN(stripLine.RepeatUntil)
                                ? visibleRange.End
                                : stripLine.RepeatUntil;
                            double periodStrip = stripLine.RepeatEvery;

                            do
                            {
                                double y1 = Area.ValueToPoint(axis, startStrip);
                                double y2;
                                if (stripLine.IsPixelWidth)
                                {
                                    y2 = stripLine.Width < yAxisRect.Height
                                        ? y1 - stripLine.Width
                                        : y1 - yAxisRect.Height;

                                }
                                else
                                {
                                    double stripEnd = (!double.IsNaN(stripLine.RepeatUntil) ? (startStrip < stripLine.RepeatUntil) ? startStrip + stripLine.Width
                                            : startStrip - stripLine.Width : startStrip + stripLine.Width);
                                    y2 = Area.ValueToPoint(axis, stripEnd);
                                }
                                axis.StriplineYRange += new DoubleRange(startStrip, Area.PointToValue(axis, new Point(0, y2)));

                                double top = yAxisRect.Top - seriesRect.Top;
                                y2 = y2 > top ? y2 : top;
                                stripRect = new Rect(new Point(xAxisRect.Left - seriesRect.Left, y1),
                                    new Point(xAxisRect.Width + xAxisRect.Left - seriesRect.Left, y2));
                                RenderStripLine(stripRect, stripLine);

                                startStrip = startStrip < endStrip ? startStrip + periodStrip : startStrip - periodStrip;
                            }
                            while (periodStrip != 0 && (Double.IsNaN(stripLine.RepeatUntil) ? startStrip < endStrip : (stripLine.Start < stripLine.RepeatUntil ? startStrip < endStrip
                                      : startStrip > endStrip)));
                        }
                        else
                        {
                            double startStrip = stripLine.Start;
                            double endStrip = double.IsNaN(stripLine.RepeatUntil)
                                ? visibleRange.End
                                : stripLine.RepeatUntil;
                            double periodStrip = stripLine.RepeatEvery;

                            if (!double.IsNaN(stripLine.SegmentStartValue) &&
                                !double.IsNaN(stripLine.SegmentEndValue))
                            {
                                do
                                {

                                    double y1 = Area.ValueToPoint(axis, startStrip);
                                    double y2;
                                    if (stripLine.IsPixelWidth)
                                    {
                                        y2 = y1 - stripLine.Width;
                                    }
                                    else
                                    {
                                        double stripEnd = (!double.IsNaN(stripLine.RepeatUntil) ? (startStrip < stripLine.RepeatUntil) ? startStrip + stripLine.Width
                                            : startStrip - stripLine.Width : startStrip + stripLine.Width);
                                        y2 = Area.ValueToPoint(axis, stripEnd);
                                    }

                                    axis.StriplineYRange += new DoubleRange(startStrip, Area.PointToValue(axis, new Point(0, y2)));

                                    if (axis.Area != null && axis.Area.InternalPrimaryAxis != null)
                                    {
                                        double startVal = Area.ValueToPoint(xAxis,
                                            stripLine.SegmentStartValue);
                                        double endVal = Area.ValueToPoint(xAxis,
                                            stripLine.SegmentEndValue);

                                        stripRect = new Rect(new Point(startVal, y1),
                                            new Point(endVal, y2));
                                        RenderStripLine(stripRect, stripLine);
                                        if ((axis as ChartAxisBase2D).IncludeStripLineRange)
                                            axis.StriplineXRange += new DoubleRange(stripLine.SegmentStartValue, stripLine.SegmentEndValue);
                                    }
                                    startStrip = startStrip < endStrip ? startStrip + periodStrip : startStrip - periodStrip;
                                }
                                while (periodStrip != 0 && (Double.IsNaN(stripLine.RepeatUntil) ? startStrip < endStrip : (stripLine.Start < stripLine.RepeatUntil ? startStrip < endStrip
                                      : startStrip > endStrip)));
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
