// <copyright file="ChartPolarGridLinesPanel.cs" company="Syncfusion. Inc">
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
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;

    /// <summary>
    /// Represents ChartPolarGridLinesPanel
    /// </summary>
    public partial class ChartPolarGridLinesPanel : ILayoutCalculator
    {
        #region Fields

        private Size desiredSize;
        private Panel panel;
        private UIElementsRecycler<Ellipse> ellipseRecycler;
        private UIElementsRecycler<Line> linesRecycler;
        private UIElementsRecycler<Line> ylinesRecycler;
        private UIElementsRecycler<Path> pathRecycler;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPolarGridLinesPanel"/> class.
        /// </summary>
        /// <param name="panel">The Required Panel</param>
        /// <exception cref="ArgumentNullException"><see cref="ArgumentNullException"/> may be thrown</exception>
        public ChartPolarGridLinesPanel(Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException();

            this.panel = panel;
            ellipseRecycler = new UIElementsRecycler<Ellipse>(panel);
            linesRecycler = new UIElementsRecycler<Line>(panel);
            ylinesRecycler = new UIElementsRecycler<Line>(panel);
            pathRecycler = new UIElementsRecycler<Path>(panel);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the Series is Radar/Polar Series type.
        /// </summary>
        public bool IsRadar
        {
            get
            {
                if (Area != null && Area.VisibleSeries != null
                    && Area.VisibleSeries.Count > 0)
                {
                    return Area.VisibleSeries[0] is RadarSeries;
                }

                return false;
            }
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
        /// Gets or sets the chart area.
        /// </summary>
        internal SfChart Area
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the x-axis of the chart.
        /// </summary>      
        public ChartAxis XAxis
        {
            get
            {
                return Area.InternalPrimaryAxis;
            }
        }

        /// <summary>
        ///Gets the y-axis of the chart.
        /// </summary>       
        public ChartAxis YAxis
        {
            get
            {
                return Area.InternalSecondaryAxis;
            }
        }

        /// <summary>
        /// Gets the desired position of the panel.
        /// </summary>       
        public Size DesiredSize
        {
            get { return desiredSize; }
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

        #region Methods

        #region Public Methods  

        /// <summary>
        /// Measures the elements of a panel.
        /// </summary>
        /// <param name="availableSize">available size of the panel.</param>
        /// <returns>returns Size.</returns>      
        public Size Measure(Size availableSize)
        {
            desiredSize = new Size(Area.SeriesClipRect.Width, Area.SeriesClipRect.Height);

            if (!IsRadar)
                RenderCircles();

            return availableSize;
        }

        /// <summary>
        /// Arranges the elements of a panel.
        /// </summary>
        /// <param name="finalSize">final size of the panel.</param>
        /// <returns>returns Size</returns>
        public Size Arrange(Size finalSize)
        {
            RenderGridLines();
            return finalSize;
        }

        /// <summary>
        /// Seek the elements from the panel.
        /// </summary>
        public void DetachElements()
        {
            if (ellipseRecycler != null)
                ellipseRecycler.Clear();

            if (linesRecycler != null)
                linesRecycler.Clear();
            if (ylinesRecycler != null)
                ylinesRecycler.Clear();
            panel = null;
        }

        /// <summary>
        /// Adds the elements to the panel.
        /// </summary>
        public void UpdateElements()
        {
            int count = 0;
            if (this.YAxis != null)
                count = this.YAxis.VisibleLabels.Count;

            int totalLinesCount = 0;
            if (!linesRecycler.BindingProvider.Keys.Contains(Line.StyleProperty) && this.Area.InternalPrimaryAxis != null)
            {
                Binding binding = new Binding();
                binding.Path = new PropertyPath("MajorGridLineStyle");
                binding.Source = this.Area.InternalPrimaryAxis;
                linesRecycler.BindingProvider.Add(Line.StyleProperty, binding);
            }

            if (!IsRadar)
            {
                if (this.Area.InternalSecondaryAxis != null && this.Area.InternalSecondaryAxis.MajorGridLineStyle.TargetType == typeof(Ellipse) && !ellipseRecycler.BindingProvider.Keys.Contains(Ellipse.StyleProperty))
                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("MajorGridLineStyle");
                    binding.Source = this.Area.InternalSecondaryAxis;
                    ellipseRecycler.BindingProvider.Add(Ellipse.StyleProperty, binding);
                    ellipseRecycler.GenerateElements(count);
                }
                else
                {
                    ylinesRecycler.Clear();
                    ellipseRecycler.GenerateElements(count);
                    foreach (Ellipse ellipse in ellipseRecycler)
                    {
                        ellipse.Stroke = new SolidColorBrush(Colors.Gray);
                        ellipse.StrokeThickness = 1;
                    }

                }
            }
            else if (this.XAxis != null)
            {
                ellipseRecycler.Clear();
                totalLinesCount = count * this.XAxis.VisibleLabels.Count;
                if (this.Area.InternalSecondaryAxis != null && !ylinesRecycler.BindingProvider.Keys.Contains(Line.StyleProperty))
                {
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("MajorGridLineStyle");
                    binding.Source = this.Area.InternalSecondaryAxis;
                    ylinesRecycler.BindingProvider.Add(Line.StyleProperty, binding);
                }

                ylinesRecycler.GenerateElements(totalLinesCount);
            }

            if (this.XAxis != null)
                count = this.XAxis.VisibleLabels.Count;

            linesRecycler.GenerateElements(count);
        }

        #endregion

        #region Internal Methods

        internal void Dispose()
        {
            Area = null;
            if (Children.Count > 0)
            {
                Children.Clear();
            }

            if (ellipseRecycler != null && ellipseRecycler.Count > 0)
            {
                ellipseRecycler.Clear();
            }

            if (linesRecycler != null && linesRecycler.Count > 0)
            {
                linesRecycler.Clear();
            }

            if (ylinesRecycler != null && ylinesRecycler.Count > 0)
            {
                ylinesRecycler.Clear();
            }

            if (pathRecycler != null && pathRecycler.Count > 0)
            {
                pathRecycler.Clear();
            }

            ellipseRecycler = null;
            linesRecycler = null;
            ylinesRecycler = null;
            pathRecycler = null;
        }

        /// <summary>
        /// Renders the circles.
        /// </summary>
        internal void RenderCircles()
        {
            ChartAxis yAxis = this.YAxis;

            double bigRadius = Math.Min(this.DesiredSize.Width, this.DesiredSize.Height) / 2;
            Point center = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);

            if (yAxis != null && yAxis.ShowGridLines
                && ellipseRecycler.Count > 0)
            {
                int pos = 0;
                foreach (ChartAxisLabel label in yAxis.VisibleLabels)
                {
                    double radius = bigRadius * yAxis.ValueToCoefficientCalc(label.Position);
                    Ellipse ellipse = ellipseRecycler[pos];
                    ellipse.Width = radius * 2;
                    ellipse.Height = radius * 2;

                    Canvas.SetLeft(ellipse, center.X - radius);
                    Canvas.SetTop(ellipse, center.Y - radius);

                    pos++;
                }
            }
        }

        /// <summary>
        /// Updates the striplines.
        /// </summary>
        internal void UpdateStripLines()
        {
            pathRecycler.Clear();
            if (!IsRadar)
                RenderPolarStripLines();
            else
                RenderRadarStripLines();
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Creates the binding provider with the provided path and source.
        /// </summary>
        /// <param name="path">The path for binding</param>
        /// <param name="source">The source for binding</param>
        /// <returns>Returns the required binding provider.</returns>
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

        /// <summary>
        /// Renders the ellipse.
        /// </summary>
        /// <param name="center">The center point</param>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="group">The geometry group</param>
        private static void RenderEllipse(Point center, double start, double end, out GeometryGroup group)
        {
            EllipseGeometry outerEllipse = new EllipseGeometry() { Center = center, RadiusX = end, RadiusY = end };
            EllipseGeometry innerEllipse = new EllipseGeometry() { Center = center, RadiusX = start, RadiusY = start };
            group = new GeometryGroup();
            group.Children.Add(innerEllipse);
            group.Children.Add(outerEllipse);
        }

        /// <summary>
        /// Renders the segment path.
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="angle">The angle</param>
        /// <param name="center">The Cneter point</param>
        /// <param name="vector1">The first vector point</param>
        /// <param name="vector2">The second vector point</param>
        /// <param name="innerArc">The inner arc</param>
        /// <param name="outerArc">the outer arc</param>
        /// <param name="pathGeometry">The path geometry</param>
        private static void RenderSegmentedPath(double start, double end, double angle, Point center, Point vector1, Point vector2, ArcSegment innerArc, ArcSegment outerArc, out PathGeometry pathGeometry)
        {
            Point innerArcStart = new Point(center.X + start * vector1.X, center.Y + start * vector1.Y);
            Point innerArcEnd = new Point(center.X + start * vector2.X, center.Y + start * vector2.Y);
            Point outerArcStart = new Point(center.X + end * vector1.X, center.Y + end * vector1.Y);
            Point outerArcEnd = new Point(center.X + end * vector2.X, center.Y + end * vector2.Y);

            pathGeometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = innerArcStart;
            ChartPolarGridLinesPanel.RenderArc(start, angle, innerArcEnd, out innerArc, SweepDirection.Clockwise);
            WindowsLinesegment line = new WindowsLinesegment();
            line.Point = outerArcEnd;
            ChartPolarGridLinesPanel.RenderArc(end, angle, outerArcStart, out outerArc, SweepDirection.Counterclockwise);
            figure.IsClosed = true;
            figure.Segments.Add(innerArc);
            figure.Segments.Add(line);
            figure.Segments.Add(outerArc);
            pathGeometry.Figures.Add(figure);
        }

        /// <summary>
        /// Calculates the angle between two vectors.
        /// </summary>
        /// <param name="vector1">The first vector</param>
        /// <param name="vector2">The second vector</param>
        /// <param name="angle">The angle</param>
        private static void CalculateAngle(Point vector1, Point vector2, out double angle)
        {
            angle = Math.Atan2(vector2.Y, vector2.X) - Math.Atan2(vector1.Y, vector1.X);
            angle = (angle * 180) / Math.PI;
            if (angle < 0)
                angle = angle + 360;
        }

        /// <summary>
        /// Renders the arc
        /// </summary>
        /// <param name="radius">The radius</param>
        /// <param name="angle">The angle</param>
        /// <param name="point">The point</param>
        /// <param name="arc">The arc</param>
        /// <param name="direction">The direction</param>
        private static void RenderArc(double radius, double angle, Point point, out ArcSegment arc, SweepDirection direction)
        {
            arc = new ArcSegment();
            arc.Size = new Size(radius, radius);
            arc.SweepDirection = direction;
            if (angle > 180)
                arc.IsLargeArc = true;
            arc.Point = point;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Renders the striplines.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="stripLine">The stripline</param>
        private void RenderStripLine(Geometry data, ChartStripLine stripLine)
        {
            var path = pathRecycler.CreateNewInstance();
            path.Data = data;
            path.SetBinding(Path.FillProperty, ChartPolarGridLinesPanel.CreateBinding("Background", stripLine));
            path.SetBinding(Path.StrokeProperty, ChartPolarGridLinesPanel.CreateBinding("BorderBrush", stripLine));
            path.SetBinding(Path.StrokeThicknessProperty, ChartPolarGridLinesPanel.CreateBinding("BorderThickness", stripLine));
            path.SetBinding(Path.OpacityProperty, ChartPolarGridLinesPanel.CreateBinding("Opacity", stripLine));
            path.SetBinding(Path.VisibilityProperty, ChartPolarGridLinesPanel.CreateBinding("Visibility", stripLine));
        }

        /// <summary>
        /// Adds stripline for the secondary axis of polar series.
        /// </summary>
        /// <param name="axis">Vertically oriented axis</param>
        private void UpdatePolarVerticalStripLine(ChartAxis axis)
        {
            GeometryGroup group;
            double angle;

            double polarRadius = Math.Min(this.DesiredSize.Width, this.DesiredSize.Height) / 2;
            Point center = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);

            double rangeStart = polarRadius * axis.ValueToCoefficientCalc(axis.VisibleRange.Start);
            double rangeEnd = polarRadius * axis.ValueToCoefficientCalc(axis.VisibleRange.End);

            foreach (var stripline in (axis as ChartAxisBase2D).StripLines)
            {
                double stripStart = stripline.Start;
                if (double.IsNaN(stripStart)) return;
                stripStart = stripStart > axis.VisibleRange.End ? axis.VisibleRange.End : stripStart;
                double endStrip = double.IsNaN(stripline.RepeatUntil)
                        ? axis.VisibleRange.End
                        : stripline.RepeatUntil;
                endStrip = endStrip > axis.VisibleRange.End ? axis.VisibleRange.End :
                                 endStrip < axis.VisibleRange.Start ? axis.VisibleRange.Start : endStrip;
                double periodStrip = stripline.RepeatEvery;
                double stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                   : stripStart - stripline.Width : stripStart + stripline.Width);
                stripEnd = stripEnd < axis.VisibleRange.End ? stripEnd : axis.VisibleRange.End;

                if (!stripline.IsSegmented)
                    do
                    {
                        double start = polarRadius * axis.ValueToCoefficientCalc(stripStart);
                        double end = polarRadius * axis.ValueToCoefficientCalc(stripEnd);

                        if (stripline.IsPixelWidth)
                            end = end > start ? start + stripline.Width : start - stripline.Width;

                        start = start <= rangeStart ? rangeStart : start >= rangeEnd ? rangeEnd : start;
                        end = end <= rangeStart ? rangeStart : end >= rangeEnd ? rangeEnd : end;

                        ChartPolarGridLinesPanel.RenderEllipse(center, start, end, out group);
                        RenderStripLine(group, stripline);

                        stripStart = stripStart < endStrip ? stripStart + periodStrip : stripStart - periodStrip;
                        stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                       : stripStart - stripline.Width : stripStart + stripline.Width);
                        stripEnd = stripEnd < axis.VisibleRange.End ? stripEnd : axis.VisibleRange.End;
                    }
                    while (periodStrip != 0 && !double.IsNaN(stripline.RepeatUntil) && (stripline.Start < stripline.RepeatUntil ? (stripStart < endStrip && stripEnd <= endStrip)
                           : (stripStart > endStrip && stripEnd >= endStrip)));
                else
                {
                    do
                    {
                        ArcSegment innerArc = null;
                        ArcSegment outerArc = null;
                        PathGeometry pathGeometry;
                        double endRange = XAxis.VisibleRange.End + (XAxis.VisibleRange.Delta / (XAxis.VisibleLabels.Count - 1));
                        double startRange = XAxis.VisibleRange.Start;
                        double start = polarRadius * axis.ValueToCoefficientCalc(stripStart);
                        double end = polarRadius * axis.ValueToCoefficientCalc(stripEnd);
                        double segmentStart = stripline.SegmentStartValue;
                        double segmentEnd = stripline.SegmentEndValue;

                        segmentStart = !(XAxis is NumericalAxis) ? segmentStart > endRange ? endRange : segmentStart < startRange ? startRange : segmentStart
                            : segmentStart > XAxis.VisibleRange.End ? XAxis.VisibleRange.End : segmentStart < startRange ? startRange : segmentStart;

                        segmentEnd = !(XAxis is NumericalAxis) ? segmentEnd > endRange ? endRange : segmentEnd < startRange ? startRange : segmentEnd
                           : segmentEnd > XAxis.VisibleRange.End ? XAxis.VisibleRange.End : segmentEnd < startRange ? startRange : segmentEnd;

                        if (stripline.IsPixelWidth)
                            end = end > start ? start + stripline.Width : start - stripline.Width;

                        start = start <= rangeStart ? rangeStart : start >= rangeEnd ? rangeEnd : start;
                        end = end <= rangeStart ? rangeStart : end >= rangeEnd ? rangeEnd : end;
                        if (segmentStart > segmentEnd)
                        {
                            double temp = segmentStart;
                            segmentStart = segmentEnd;
                            segmentEnd = temp;
                        }

                        Point vector1 = ChartTransform.ValueToVector(XAxis, segmentStart);
                        Point vector2 = ChartTransform.ValueToVector(XAxis, segmentEnd);
                        ChartPolarGridLinesPanel.CalculateAngle(vector1, vector2, out angle);
                        if (angle == 360)
                        {
                            ChartPolarGridLinesPanel.RenderEllipse(center, start, end, out group);
                            RenderStripLine(group, stripline);
                        }
                        else
                        {
                            ChartPolarGridLinesPanel.RenderSegmentedPath(start, end, angle, center, vector1, vector2, innerArc, outerArc, out pathGeometry);
                            RenderStripLine(pathGeometry, stripline);
                        }

                        stripStart = stripStart < endStrip ? stripStart + periodStrip : stripStart - periodStrip;
                        stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                       : stripStart - stripline.Width : stripStart + stripline.Width);
                        stripEnd = stripEnd < axis.VisibleRange.End ? stripEnd : axis.VisibleRange.End;
                    }
                    while (periodStrip != 0 && !double.IsNaN(stripline.RepeatUntil) && (stripline.Start < stripline.RepeatUntil ? (stripStart < endStrip && stripEnd <= endStrip)
                                                   : (stripStart > endStrip && stripEnd >= endStrip)));
                }
            }
        }

        /// <summary>
        /// Adds stripline for the primary axis of polar series.
        /// </summary>
        /// <param name="axis">Horizontally oriented axis</param>
        private void UpdatePolarHorizontalStripLine(ChartAxis axis)
        {
            PathGeometry pathGeometry;
            PathFigure figure;
            double angle;
            double endRange = axis.VisibleRange.End + (axis.VisibleRange.Delta / (axis.VisibleLabels.Count - 1));
            double polarRadius = Math.Min(this.DesiredSize.Width, this.DesiredSize.Height) / 2;
            Point center = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);

            foreach (var stripline in (axis as ChartAxisBase2D).StripLines)
            {
                var isNumericalAxis = axis is NumericalAxis;
                double stripStart = stripline.Start;
                if (double.IsNaN(stripStart)) return;
                stripStart = stripStart < axis.VisibleRange.Start ? axis.VisibleRange.Start :
                    stripStart > axis.VisibleRange.End ? !isNumericalAxis ? axis.VisibleRange.End + 1 :
                               axis.VisibleRange.End : stripStart;
                double endStrip = double.IsNaN(stripline.RepeatUntil)
                        ? axis.VisibleRange.End
                        : stripline.RepeatUntil;

                endStrip = !isNumericalAxis ? endStrip > endRange ? endRange : endStrip
                              : endStrip > axis.VisibleRange.End ? axis.VisibleRange.End : endStrip;

                double periodStrip = stripline.RepeatEvery;

                double stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                   : stripStart - stripline.Width : stripStart + stripline.Width);

                stripEnd = !isNumericalAxis ? stripEnd > endRange ? endRange : stripEnd
                                  : stripEnd > axis.VisibleRange.End ? axis.VisibleRange.End : stripEnd;

                if (!stripline.IsSegmented)
                    do
                    {
                        Point vector1 = ChartTransform.ValueToVector(XAxis, stripStart);
                        Point vector2 = ChartTransform.ValueToVector(XAxis, stripEnd);
                        if (stripStart > stripEnd)
                        {
                            Point temp = vector1;
                            vector1 = vector2;
                            vector2 = temp;
                        }
                        figure = new PathFigure();
                        ArcSegment arc = new ArcSegment();
                        WindowsLinesegment lineSegment;
                        ChartPolarGridLinesPanel.CalculateAngle(vector1, vector2, out angle);

                        if (angle == 360)
                        {
                            EllipseGeometry ellipse = new EllipseGeometry() { Center = center, RadiusX = polarRadius, RadiusY = polarRadius };
                            RenderStripLine(ellipse, stripline);
                        }
                        else
                        {
                            lineSegment = new WindowsLinesegment();
                            pathGeometry = new PathGeometry();
                            lineSegment.Point = new Point(center.X + polarRadius * vector1.X, center.Y + polarRadius * vector1.Y);
                            figure.StartPoint = new Point(center.X, center.Y);
                            Point arcPoint = new Point(center.X + polarRadius * vector2.X, center.Y + polarRadius * vector2.Y);
                            ChartPolarGridLinesPanel.RenderArc(polarRadius, angle, arcPoint, out arc, SweepDirection.Clockwise);
                            figure.IsClosed = true;
                            figure.Segments.Add(lineSegment);
                            figure.Segments.Add(arc);
                            pathGeometry.Figures.Add(figure);
                            RenderStripLine(pathGeometry, stripline);
                        }

                        stripStart = stripStart < endStrip ? stripStart + periodStrip : stripStart - periodStrip;
                        stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                       : stripStart - stripline.Width : stripStart + stripline.Width);

                        stripEnd = !isNumericalAxis ? stripEnd > endRange ? endRange : stripEnd
                                  : stripEnd > axis.VisibleRange.End ? axis.VisibleRange.End : stripEnd;

                    }
                    while (periodStrip != 0 && !double.IsNaN(stripline.RepeatUntil) && (stripline.Start < stripline.RepeatUntil ? (stripStart < endStrip && stripEnd <= endStrip)
                           : (stripStart > endStrip && stripEnd >= endStrip)));
                else
                {
                    do
                    {
                        ArcSegment innerArc = null;
                        ArcSegment outerArc = null;
                        GeometryGroup group;
                        double segmentStart = stripline.SegmentStartValue;
                        double segmentEnd = stripline.SegmentEndValue;
                        segmentEnd = segmentEnd > YAxis.VisibleRange.End ? YAxis.VisibleRange.End :
                            segmentEnd < YAxis.VisibleRange.Start ? YAxis.VisibleRange.Start : segmentEnd;
                        segmentStart = segmentStart < YAxis.VisibleRange.Start ? YAxis.VisibleRange.Start :
                            segmentStart > YAxis.VisibleRange.End ? YAxis.VisibleRange.End : segmentStart;

                        Point vector1 = ChartTransform.ValueToVector(XAxis, stripStart);
                        Point vector2 = ChartTransform.ValueToVector(XAxis, stripEnd);
                        if (stripStart > stripEnd)
                        {
                            Point temp = vector1;
                            vector1 = vector2;
                            vector2 = temp;
                        }
                        double start = polarRadius * YAxis.ValueToCoefficientCalc(segmentStart);
                        double end = polarRadius * YAxis.ValueToCoefficientCalc(segmentEnd);

                        start = start < 0 ? 0 : start;
                        end = end < 0 ? 0 : end;

                        ChartPolarGridLinesPanel.CalculateAngle(vector1, vector2, out angle);

                        if (angle == 360)
                        {
                            ChartPolarGridLinesPanel.RenderEllipse(center, start, end, out group);
                            RenderStripLine(group, stripline);
                        }

                        else
                        {
                            ChartPolarGridLinesPanel.RenderSegmentedPath(start, end, angle, center, vector1, vector2, innerArc, outerArc, out pathGeometry);
                            RenderStripLine(pathGeometry, stripline);
                        }
                        stripStart = stripStart < endStrip ? stripStart + periodStrip : stripStart - periodStrip;
                        stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                       : stripStart - stripline.Width : stripStart + stripline.Width);
                        stripEnd = stripEnd > axis.VisibleRange.End ? !isNumericalAxis ? stripEnd : axis.VisibleRange.End
                                   : stripEnd;

                    }
                    while (periodStrip != 0 && !double.IsNaN(stripline.RepeatUntil) && (stripline.Start < stripline.RepeatUntil ? (stripStart < endStrip && stripEnd <= endStrip)
                                                  : (stripStart > endStrip && stripEnd >= endStrip)));
                }
            }
        }

        /// <summary>
        /// Renders the polar strip lines.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void RenderPolarStripLines()
        {
            foreach (var axis in Area.Axes.Where(axis => axis != null && !axis.VisibleRange.IsEmpty
            && (axis as ChartAxisBase2D).StripLines != null && (axis as ChartAxisBase2D).StripLines.Count > 0))

            {
                if (axis.Orientation == Orientation.Vertical)
                {
                    UpdatePolarVerticalStripLine(axis);
                }

                else if (axis.Orientation == Orientation.Horizontal)
                {
                    UpdatePolarHorizontalStripLine(axis);
                }
            }
        }

        /// <summary>
        /// Renders the grid lines.
        /// </summary>
        private void RenderGridLines()
        {
            ChartAxis xAxis = this.XAxis;
            ChartAxis yAxis = this.YAxis;

            double bigRadius = Math.Min(this.DesiredSize.Width, this.DesiredSize.Height) / 2;
            Point center = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
            int pos = 0;
            if (IsRadar && yAxis != null && yAxis.ShowGridLines)
            {
                foreach (ChartAxisLabel label in yAxis.VisibleLabels)
                {
                    double radius = bigRadius * yAxis.ValueToCoefficientCalc(label.Position);
                    for (int i = 0; i < xAxis.VisibleLabels.Count; i++)
                    {
                        Point vector = ChartTransform.ValueToVector(xAxis, xAxis.VisibleLabels[i].Position);
                        Point vector2 = new Point();
                        if ((i + 1) < xAxis.VisibleLabels.Count)
                        {
                            vector2 = ChartTransform.ValueToVector(xAxis, xAxis.VisibleLabels[i + 1].Position);
                        }
                        else
                        {
                            vector2 = ChartTransform.ValueToVector(xAxis, xAxis.VisibleLabels[0].Position);
                        }

                        Point connectPoint = new Point(center.X + radius * vector.X, center.Y + radius * vector.Y);
                        Point endPoint = new Point(center.X + radius * vector2.X, center.Y + radius * vector2.Y);

                        Line line = ylinesRecycler[pos];
                        line.X1 = connectPoint.X;
                        line.Y1 = connectPoint.Y;
                        line.X2 = endPoint.X;
                        line.Y2 = endPoint.Y;
                        pos++;
                    }
                }
            }

            if (xAxis != null && xAxis.ShowGridLines)
            {
                int pos1 = 0;

                foreach (ChartAxisLabel label in xAxis.VisibleLabels)
                {
                    Point vector = ChartTransform.ValueToVector(xAxis, label.Position);
                    Line line = linesRecycler[pos1];
                    line.X1 = center.X;
                    line.Y1 = center.Y;
                    line.X2 = center.X + bigRadius * vector.X;
                    line.Y2 = center.Y + bigRadius * vector.Y;
                    pos1++;
                }
            }

            UpdateStripLines();
        }

        /// <summary>
        /// Render radar striplines.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void RenderRadarStripLines()
        {
            foreach (var axis in Area.Axes.Where(axis => axis != null && !axis.VisibleRange.IsEmpty
             && (axis as ChartAxisBase2D).StripLines != null && (axis as ChartAxisBase2D).StripLines.Count > 0))
            {
                if (axis.Orientation == Orientation.Vertical)
                {
                    UpdateRadarVerticalStripLine(axis);

                }
                else if (axis.Orientation == Orientation.Horizontal)
                {
                    UpdateRadarHorizontalStripLine(axis);
                }
            }
        }

        /// <summary>
        /// Adds stripline for the primary axis of radar series.
        /// </summary>
        /// <param name="axis">Horizontally oriented axis</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        private void UpdateRadarHorizontalStripLine(ChartAxis axis)
        {
            double radarRadius = Math.Min(this.DesiredSize.Width, this.DesiredSize.Height) / 2;
            Point center = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
            PathFigure figure1;
            PathGeometry pathGeometry1;
            WindowsLinesegment lineSegment2;
            WindowsLinesegment lineSegment1;
            var isXAxisNumerical = XAxis is NumericalAxis;
            foreach (var stripline in (axis as ChartAxisBase2D).StripLines)
            {
                var isCurrentAxisNumerical = axis is NumericalAxis;
                int startIndex = 0;
                int endIndex = 0;
                int count = 0;
                bool isFullRadar = false;
                double endRange = XAxis.VisibleRange.End + (XAxis.VisibleRange.Delta / (XAxis.VisibleLabels.Count - 1));
                double stripStart = stripline.Start;
                if (double.IsNaN(stripStart)) return;
                stripStart = stripStart < axis.VisibleRange.Start ? axis.VisibleRange.Start : stripStart;

                double endStrip = double.IsNaN(stripline.RepeatUntil) ? !isCurrentAxisNumerical ? endRange : axis.VisibleRange.End
                                : stripline.RepeatUntil;

                endStrip = !isXAxisNumerical ? endStrip > endRange ? endRange : endStrip
                            : endStrip > XAxis.VisibleRange.End ? XAxis.VisibleRange.End : endStrip;

                double periodStrip = stripline.RepeatEvery;
                double stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                   : stripStart - stripline.Width : stripStart + stripline.Width);

                stripEnd = !isCurrentAxisNumerical ? stripEnd > endRange ? endRange : stripEnd
                                 : stripEnd > axis.VisibleRange.End ? axis.VisibleRange.End : stripEnd;

                if (!stripline.IsSegmented)
                    do
                    {
                        count = 0;
                        foreach (var labels in XAxis.VisibleLabels)
                        {
                            if (Math.Abs(stripStart - labels.Position) < 0.001)
                                startIndex = XAxis.VisibleLabels.IndexOf(labels);

                            if (Math.Abs(stripEnd - labels.Position) < 0.001)
                                endIndex = XAxis.VisibleLabels.IndexOf(labels);
                        }

                        figure1 = new PathFigure();
                        pathGeometry1 = new PathGeometry();

                        if (!isXAxisNumerical && stripStart <= XAxis.VisibleRange.End)
                            endIndex = stripEnd > XAxis.VisibleRange.End ? XAxis.VisibleLabels.Count : endIndex;

                        if (stripStart > stripEnd)
                        {
                            int temp = startIndex;
                            startIndex = endIndex;
                            endIndex = temp;
                        }

                        if (!isXAxisNumerical && stripStart == endRange && stripEnd == XAxis.VisibleRange.End)
                            endIndex = XAxis.VisibleLabels.Count;

                        for (int i = startIndex; i < endIndex; i++)
                        {
                            int lineStart = i;
                            int temp = lineStart;
                            if (lineStart >= XAxis.VisibleLabels.Count - 1)
                            {
                                lineStart = lineStart - (XAxis.VisibleLabels.Count - 1);
                                isFullRadar = true;
                            }

                            int lineEnd = lineStart + 1;

                            if (!isXAxisNumerical && ((stripStart == axis.VisibleRange.End && stripEnd > axis.VisibleRange.End) || (stripStart == endRange && stripEnd == axis.VisibleRange.End)))
                            {
                                lineStart = axis.VisibleLabels.Count - 1;
                                lineEnd = 0;
                                isFullRadar = false;
                            }

                            Point vector = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineStart].Position);
                            Point vector2 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineEnd].Position);

                            Point startPoint = new Point(center.X + radarRadius * vector.X, center.Y + radarRadius * vector.Y);
                            Point endPoint = new Point(center.X + radarRadius * vector2.X, center.Y + radarRadius * vector2.Y);

                            lineSegment1 = new WindowsLinesegment();
                            if (count == 0)
                                figure1.StartPoint = center;
                            lineSegment1.Point = startPoint;
                            figure1.Segments.Add(lineSegment1);

                            if (!isFullRadar)
                            {
                                lineSegment2 = new WindowsLinesegment();
                                lineSegment2.Point = endPoint;
                                figure1.Segments.Add(lineSegment2);
                            }

                            count++;
                            lineStart = temp;
                            isFullRadar = false;
                        }

                        pathGeometry1.Figures.Add(figure1);
                        figure1.IsClosed = true;
                        RenderStripLine(pathGeometry1, stripline);

                        stripStart = stripStart < endStrip ? stripStart + periodStrip : stripStart - periodStrip;
                        stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                       : stripStart - stripline.Width : stripStart + stripline.Width);

                        stripEnd = !isCurrentAxisNumerical ? stripEnd > endRange ? endRange : stripEnd
                                : stripEnd > axis.VisibleRange.End ? axis.VisibleRange.End : stripEnd;
                    }
                    while (periodStrip != 0 && !double.IsNaN(stripline.RepeatUntil) && (stripline.Start < stripline.RepeatUntil ? (stripStart < endStrip && stripEnd <= endStrip)
                           : (stripStart > endStrip && stripEnd >= endStrip)));
                else
                {
                    do
                    {
                        Point innerLineStart;
                        Point innerLineEnd;
                        Point outerLineStart = new Point(0, 0);
                        Point outerLineEnd = new Point(0, 0);
                        figure1 = new PathFigure();
                        pathGeometry1 = new PathGeometry();
                        bool lastSegment = false;
                        count = 0;

                        foreach (var labels in XAxis.VisibleLabels)
                        {
                            if (Math.Abs(stripStart - labels.Position) < 0.001)
                                startIndex = XAxis.VisibleLabels.IndexOf(labels);
                            if (Math.Abs(stripEnd - labels.Position) < 0.001)
                                endIndex = XAxis.VisibleLabels.IndexOf(labels);
                        }

                        if (!isXAxisNumerical && stripStart <= XAxis.VisibleRange.End)
                            endIndex = stripEnd > XAxis.VisibleRange.End ? XAxis.VisibleLabels.Count : endIndex;

                        if (stripStart > stripEnd)
                        {
                            int temp = startIndex;
                            startIndex = endIndex;
                            endIndex = temp;
                        }

                        if (!isXAxisNumerical && stripStart == endRange && stripEnd == XAxis.VisibleRange.End)
                            endIndex = XAxis.VisibleLabels.Count;

                        double segmentStart = stripline.SegmentStartValue;
                        double segmentEnd = stripline.SegmentEndValue;
                        segmentEnd = segmentEnd > YAxis.VisibleRange.End ? YAxis.VisibleRange.End :
                                        segmentEnd < YAxis.VisibleRange.Start ? YAxis.VisibleRange.Start : segmentEnd;
                        segmentStart = segmentStart < YAxis.VisibleRange.Start ? YAxis.VisibleRange.Start :
                                           segmentStart > YAxis.VisibleRange.End ? YAxis.VisibleRange.End : segmentStart;

                        double start = radarRadius * YAxis.ValueToCoefficientCalc(segmentStart);
                        double end = radarRadius * YAxis.ValueToCoefficientCalc(segmentEnd);


                        for (int i = startIndex; i < endIndex; i++)
                        {
                            int lineStart = i;
                            isFullRadar = false;
                            int temp = lineStart;
                            if (lineStart >= XAxis.VisibleLabels.Count - 1)
                            {
                                lineStart = lineStart - (XAxis.VisibleLabels.Count - 1);
                                if (!isXAxisNumerical)
                                    isFullRadar = true;
                            }
                            int lineEnd = lineStart + 1;
                            start = start < 0 ? 0 : start;
                            end = end < 0 ? 0 : end;
                            if (!isXAxisNumerical && ((stripStart == axis.VisibleRange.End && stripEnd > axis.VisibleRange.End) || (stripStart == endRange && stripEnd == axis.VisibleRange.End)))
                            {
                                lineStart = axis.VisibleLabels.Count - 1;
                                lineEnd = 0;
                                isFullRadar = false;
                            }

                            Point vector1 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineStart].Position);
                            Point vector2 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineEnd].Position);
                            innerLineStart = new Point(center.X + start * vector1.X, center.Y + start * vector1.Y);
                            innerLineEnd = new Point(center.X + start * vector2.X, center.Y + start * vector2.Y);
                            outerLineStart = new Point(center.X + end * vector1.X, center.Y + end * vector1.Y);
                            outerLineEnd = new Point(center.X + end * vector2.X, center.Y + end * vector2.Y);

                            lineSegment1 = new WindowsLinesegment();
                            if (!isFullRadar)
                                lineSegment1.Point = innerLineEnd;
                            else
                                lineSegment1.Point = innerLineStart;
                            if (count == 0)
                                figure1.StartPoint = innerLineStart;
                            figure1.Segments.Add(lineSegment1);
                            count++;
                            lineStart = temp;
                        }

                        lineSegment1 = new WindowsLinesegment();
                        if (!isXAxisNumerical && isFullRadar)
                            lineSegment1.Point = outerLineStart;
                        else
                            lineSegment1.Point = outerLineEnd;
                        figure1.Segments.Add(lineSegment1);

                        for (int i = endIndex; i > startIndex; i--)
                        {
                            int lineEnd = i;
                            int temp = lineEnd;
                            isFullRadar = false;
                            if (lineEnd > XAxis.VisibleLabels.Count - 1)
                            {
                                lineEnd = lineEnd - (XAxis.VisibleLabels.Count - 1);
                                if (!isXAxisNumerical)
                                    isFullRadar = true;
                            }
                            int lineStart = lineEnd - 1;
                            if (!isXAxisNumerical && ((stripStart == axis.VisibleRange.End && (stripEnd == axis.VisibleRange.Start || stripEnd > axis.VisibleRange.End)) || (stripStart == endRange && stripEnd == axis.VisibleRange.End)))
                            {
                                lineStart = 0;
                                lineEnd = axis.VisibleLabels.Count - 1;
                                lastSegment = true;
                                isFullRadar = false;
                            }

                            Point vector1 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineEnd].Position);
                            Point vector2 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineStart].Position);
                            outerLineStart = new Point(center.X + end * vector1.X, center.Y + end * vector1.Y);
                            outerLineEnd = new Point(center.X + end * vector2.X, center.Y + end * vector2.Y);

                            lineSegment1 = new WindowsLinesegment();
                            if (!isXAxisNumerical && !isFullRadar)
                                lineSegment1.Point = outerLineStart;
                            else
                                lineSegment1.Point = outerLineEnd;
                            figure1.Segments.Add(lineSegment1);
                            lineEnd = temp;
                            isFullRadar = false;
                        }
                        if (!isXAxisNumerical && !lastSegment)
                        {
                            lineSegment1 = new WindowsLinesegment();
                            lineSegment1.Point = outerLineEnd;
                            figure1.Segments.Add(lineSegment1);
                        }

                        figure1.IsClosed = true;
                        pathGeometry1.Figures.Add(figure1);
                        RenderStripLine(pathGeometry1, stripline);

                        stripStart = stripStart < endStrip ? stripStart + periodStrip : stripStart - periodStrip;
                        stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                       : stripStart - stripline.Width : stripStart + stripline.Width);

                        stripEnd = !isCurrentAxisNumerical ? stripEnd > endRange ? endRange : stripEnd
                               : stripEnd > axis.VisibleRange.End ? axis.VisibleRange.End : stripEnd;

                    }
                    while (periodStrip != 0 && !double.IsNaN(stripline.RepeatUntil) && (stripline.Start < stripline.RepeatUntil ? (stripStart < endStrip && stripEnd <= endStrip)
                                                   : (stripStart > endStrip && stripEnd >= endStrip)));
                }
            }
        }

        /// <summary>
        /// Adds stripline for the secondary axis of radar series.
        /// </summary>
        /// <param name="axis">Vertically oriented axis</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        private void UpdateRadarVerticalStripLine(ChartAxis axis)
        {
            double radarRadius = Math.Min(this.DesiredSize.Width, this.DesiredSize.Height) / 2;
            Point center = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
            GeometryGroup group;
            WindowsLinesegment lineSegment2;
            WindowsLinesegment lineSegment1;
            PathFigure figure1;
            PathFigure figure2;
            PathGeometry pathGeometry1;
            PathGeometry pathGeometry2;

            double rangeStart = radarRadius * axis.ValueToCoefficientCalc(axis.VisibleRange.Start);
            double rangeEnd = radarRadius * axis.ValueToCoefficientCalc(axis.VisibleRange.End);
            var isXAxisNumerical = XAxis is NumericalAxis;
            foreach (var stripline in (axis as ChartAxisBase2D).StripLines)
            {
                double stripStart = stripline.Start;
                if (double.IsNaN(stripStart)) return;
                stripStart = stripStart > axis.VisibleRange.End ? axis.VisibleRange.End : stripStart;
                double endStrip = double.IsNaN(stripline.RepeatUntil)
                        ? axis.VisibleRange.End
                        : stripline.RepeatUntil;
                double periodStrip = stripline.RepeatEvery;
                endStrip = endStrip > axis.VisibleRange.End ? axis.VisibleRange.End : endStrip;
                double stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                   : stripStart - stripline.Width : stripStart + stripline.Width);
                stripEnd = stripEnd < axis.VisibleRange.End ? stripEnd : axis.VisibleRange.End;

                if (!stripline.IsSegmented)
                    do
                    {
                        figure1 = new PathFigure();
                        figure2 = new PathFigure();
                        pathGeometry1 = new PathGeometry();
                        pathGeometry2 = new PathGeometry();
                        group = new GeometryGroup();
                        for (int i = 0; i < XAxis.VisibleLabels.Count; i++)
                        {
                            double innerRadius = radarRadius * axis.ValueToCoefficientCalc(stripStart);
                            double outerRadius = radarRadius * axis.ValueToCoefficientCalc(stripEnd);
                            if (stripline.IsPixelWidth)
                                outerRadius = innerRadius + stripline.Width;
                            innerRadius = innerRadius <= rangeStart ? rangeStart : innerRadius >= rangeEnd ? rangeEnd : innerRadius;
                            outerRadius = outerRadius <= rangeStart ? rangeStart : outerRadius >= rangeEnd ? rangeEnd : outerRadius;
                            Point vector = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[i].Position);
                            Point vector2 = new Point();
                            Point startVector = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[0].Position);
                            Point startPoint = new Point(center.X + innerRadius * startVector.X, center.Y + innerRadius * startVector.Y);
                            Point startPoint1 = new Point(center.X + outerRadius * startVector.X, center.Y + outerRadius * startVector.Y);
                            if ((i + 1) < XAxis.VisibleLabels.Count)
                            {
                                vector2 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[i + 1].Position);
                            }
                            else
                            {
                                vector2 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[0].Position);
                            }
                            Point connectPoint = new Point(center.X + innerRadius * vector.X, center.Y + innerRadius * vector.Y);
                            Point endPoint = new Point(center.X + innerRadius * vector2.X, center.Y + innerRadius * vector2.Y);
                            Point connectPoint1 = new Point(center.X + outerRadius * vector.X, center.Y + outerRadius * vector.Y);
                            Point endPoint1 = new Point(center.X + outerRadius * vector2.X, center.Y + outerRadius * vector2.Y);

                            lineSegment2 = new WindowsLinesegment();
                            lineSegment2.Point = connectPoint1;
                            figure2.Segments.Add(lineSegment2);
                            lineSegment2 = new WindowsLinesegment();
                            lineSegment2.Point = endPoint1;
                            figure2.StartPoint = startPoint1;
                            figure2.Segments.Add(lineSegment2);

                            lineSegment1 = new WindowsLinesegment();
                            lineSegment1.Point = connectPoint;
                            figure1.Segments.Add(lineSegment1);
                            lineSegment1 = new WindowsLinesegment();
                            lineSegment1.Point = endPoint;
                            figure1.StartPoint = startPoint;
                            figure1.Segments.Add(lineSegment1);
                        }
                        stripStart = stripStart < endStrip ? stripStart + periodStrip : stripStart - periodStrip;
                        stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                       : stripStart - stripline.Width : stripStart + stripline.Width);

                        pathGeometry1.Figures.Add(figure1);
                        pathGeometry2.Figures.Add(figure2);
                        group.Children.Add(pathGeometry1);
                        group.Children.Add(pathGeometry2);
                        RenderStripLine(group, stripline);

                        stripEnd = stripEnd < axis.VisibleRange.End ? stripEnd : axis.VisibleRange.End;
                    }
                    while (periodStrip != 0 && !double.IsNaN(stripline.RepeatUntil) && (stripline.Start < stripline.RepeatUntil ? (stripStart < endStrip && stripEnd <= endStrip)
                           : (stripStart > endStrip && stripEnd >= endStrip)));
                else
                {
                    do
                    {
                        Point innerLineStart;
                        Point innerLineEnd;
                        Point outerLineStart = new Point(0, 0);
                        Point outerLineEnd = new Point(0, 0);
                        int startIndex = 0;
                        int endIndex = 0;
                        int count = 0;
                        figure1 = new PathFigure();
                        pathGeometry1 = new PathGeometry();
                        bool isFullSegment = false;
                        double segmentStart = stripline.SegmentStartValue;
                        double segmentEnd = stripline.SegmentEndValue;
                        segmentStart = !isXAxisNumerical ? segmentStart > XAxis.VisibleRange.End + (XAxis.VisibleRange.Delta / (XAxis.VisibleLabels.Count - 1)) ? XAxis.VisibleRange.End + (XAxis.VisibleRange.Delta / (XAxis.VisibleLabels.Count - 1)) : segmentStart
                             : segmentStart > XAxis.VisibleRange.End ? XAxis.VisibleRange.End : segmentStart;
                        segmentEnd = !isXAxisNumerical ? segmentEnd > XAxis.VisibleRange.End + (XAxis.VisibleRange.Delta / (XAxis.VisibleLabels.Count - 1)) ? XAxis.VisibleRange.End + (XAxis.VisibleRange.Delta / (XAxis.VisibleLabels.Count - 1)) : segmentEnd
                            : segmentEnd > XAxis.VisibleRange.End ? XAxis.VisibleRange.End : segmentEnd;
                        if (segmentStart > segmentEnd)
                        {
                            double temp = segmentStart;
                            segmentStart = segmentEnd;
                            segmentEnd = temp;
                        }

                        foreach (var labels in XAxis.VisibleLabels)
                        {
                            if (Math.Abs(segmentStart - labels.Position) < 0.001)
                                startIndex = XAxis.VisibleLabels.IndexOf(labels);

                            if (Math.Abs(segmentEnd - labels.Position) < 0.001)
                                endIndex = XAxis.VisibleLabels.IndexOf(labels);
                        }

                        if (!isXAxisNumerical && segmentStart <= XAxis.VisibleRange.End)
                            endIndex = segmentEnd > XAxis.VisibleRange.End ? XAxis.VisibleLabels.Count : endIndex;
                        double start = radarRadius * axis.ValueToCoefficientCalc(stripStart);
                        double end = radarRadius * axis.ValueToCoefficientCalc(stripEnd);
                        for (int i = startIndex; i < endIndex; i++)
                        {
                            int lineStart = i;
                            int temp = lineStart;
                            isFullSegment = false;
                            if (lineStart >= XAxis.VisibleLabels.Count - 1)
                            {
                                lineStart = lineStart - (XAxis.VisibleLabels.Count - 1);
                                if (!isXAxisNumerical)
                                    isFullSegment = true;
                            }
                            if (stripline.IsPixelWidth)
                                end = end > start ? start + stripline.Width : start - stripline.Width;
                            int lineEnd = lineStart + 1;

                            if (!isXAxisNumerical && startIndex == XAxis.VisibleRange.End && endIndex > XAxis.VisibleRange.End)
                            {
                                lineStart = startIndex;
                                lineEnd = 0;
                            }
                            start = start <= rangeStart ? rangeStart : start >= rangeEnd ? rangeEnd : start;
                            end = end <= rangeStart ? rangeStart : end >= rangeEnd ? rangeEnd : end;

                            Point vector1 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineStart].Position);
                            Point vector2 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineEnd].Position);
                            innerLineStart = new Point(center.X + start * vector1.X, center.Y + start * vector1.Y);
                            innerLineEnd = new Point(center.X + start * vector2.X, center.Y + start * vector2.Y);
                            outerLineStart = new Point(center.X + end * vector1.X, center.Y + end * vector1.Y);
                            outerLineEnd = new Point(center.X + end * vector2.X, center.Y + end * vector2.Y);

                            lineSegment1 = new WindowsLinesegment();
                            if (!isXAxisNumerical && lineStart == 0 && isFullSegment)
                                lineSegment1.Point = innerLineStart;
                            else
                                lineSegment1.Point = innerLineEnd;
                            if (count == 0)
                                figure1.StartPoint = innerLineStart;
                            figure1.Segments.Add(lineSegment1);
                            count++;
                            lineStart = temp;
                        }

                        lineSegment1 = new WindowsLinesegment();
                        if (!isXAxisNumerical && isFullSegment && startIndex != XAxis.VisibleRange.End)
                            lineSegment1.Point = outerLineStart;
                        else
                            lineSegment1.Point = outerLineEnd;
                        figure1.Segments.Add(lineSegment1);
                        isFullSegment = false;

                        for (int i = endIndex; i > startIndex; i--)
                        {
                            int lineStart = i;
                            int temp = lineStart;
                            if (lineStart >= XAxis.VisibleLabels.Count)
                            {
                                lineStart = lineStart - (XAxis.VisibleLabels.Count - 1);
                            }

                            if (!isXAxisNumerical && lineStart == XAxis.VisibleLabels.Count - 1)
                                isFullSegment = true;
                            if (stripline.IsPixelWidth)
                                end = end > start ? start + stripline.Width : start - stripline.Width;
                            end = end <= rangeStart ? rangeStart : end >= rangeEnd ? rangeEnd : end;
                            int lineEnd = lineStart - 1;
                            if (!isXAxisNumerical && startIndex == XAxis.VisibleRange.End && endIndex > XAxis.VisibleRange.End)
                            {
                                lineStart = 0;
                                lineEnd = XAxis.VisibleLabels.Count - 1;
                            }

                            Point vector1 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineStart].Position);
                            Point vector2 = ChartTransform.ValueToVector(XAxis, XAxis.VisibleLabels[lineEnd].Position);
                            outerLineStart = new Point(center.X + end * vector1.X, center.Y + end * vector1.Y);
                            outerLineEnd = new Point(center.X + end * vector2.X, center.Y + end * vector2.Y);
                            lineSegment1 = new WindowsLinesegment();
                            if (!isXAxisNumerical && isFullSegment)
                                lineSegment1.Point = outerLineStart;
                            else
                                lineSegment1.Point = outerLineEnd;
                            figure1.Segments.Add(lineSegment1);
                            lineStart = temp;
                        }

                        if (!isXAxisNumerical)
                        {
                            lineSegment1 = new WindowsLinesegment();
                            lineSegment1.Point = outerLineEnd;
                            figure1.Segments.Add(lineSegment1);
                        }

                        figure1.IsClosed = true;
                        pathGeometry1.Figures.Add(figure1);
                        RenderStripLine(pathGeometry1, stripline);

                        stripStart = stripStart < endStrip ? stripStart + periodStrip : stripStart - periodStrip;
                        stripEnd = (!double.IsNaN(stripline.RepeatUntil) ? stripStart == stripline.RepeatUntil ? stripStart : (stripStart < stripline.RepeatUntil) ? stripStart + stripline.Width
                                       : stripStart - stripline.Width : stripStart + stripline.Width);
                        stripEnd = stripEnd < axis.VisibleRange.End ? stripEnd : axis.VisibleRange.End;
                    }
                    while (periodStrip != 0 && !double.IsNaN(stripline.RepeatUntil) && (stripline.Start < stripline.RepeatUntil ? (stripStart < endStrip && stripEnd <= endStrip)
                                                   : (stripStart > endStrip && stripEnd >= endStrip)));
                }
            }
        }

        #endregion

        #endregion
    }
}
