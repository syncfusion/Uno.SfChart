using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Chart enables the user to break the scale of <see cref="ChartAxis"/> by adding scale breaks to it.
    /// </summary>
    /// <remarks>
    /// The scale break appearance can be customized.
    /// </remarks>
    public partial class ChartAxisScaleBreak : FrameworkElement, INotifyPropertyChanged
    {
        #region Dependency Property Registration

        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(
                "Start", 
                typeof(double), 
                typeof(ChartAxisScaleBreak),
                new PropertyMetadata(double.NaN, OnStartPropertyChanged));

        public static readonly DependencyProperty EndProperty =
                DependencyProperty.Register(
                    "End",
                    typeof(double),
                    typeof(ChartAxisScaleBreak),
                    new PropertyMetadata(double.NaN, OnEndPropertyChanged));

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                "Fill",
                typeof(Brush), 
                typeof(ChartAxisScaleBreak),

#if UNIVERSALWINDOWS
                new PropertyMetadata(null));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.White)));
#endif

        public static readonly DependencyProperty LineTypeProperty =
            DependencyProperty.Register(
                "LineType", 
                typeof(BreakLineType),
                typeof(ChartAxisScaleBreak),
                new PropertyMetadata(BreakLineType.StraightLine, OnLineTypeChanged));

        public static readonly DependencyProperty BreakSpacingProperty =
            DependencyProperty.Register(
                "BreakSpacing",
                typeof(double),
                typeof(ChartAxisScaleBreak),
                new PropertyMetadata(5d, OnBreakSpacingChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="BreakPercent"/> property.
        /// </summary>
        public static readonly DependencyProperty BreakPercentProperty =
            DependencyProperty.Register(
                "BreakPercent", 
                typeof(double), 
                typeof(ChartAxisScaleBreak),
                new PropertyMetadata(50d, OnBreakPercentChanged));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness", 
                typeof(double), 
                typeof(ChartAxisScaleBreak),
                new PropertyMetadata(1d));

        /// <summary>
        /// The DependencyProperty for <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke",
                typeof(Brush), 
                typeof(ChartAxisScaleBreak),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));                
#endif

#endregion

#region Constructor

        public ChartAxisScaleBreak()
        {
#if UNIVERSALWINDOWS
            Stroke = new SolidColorBrush(Colors.Black);
            Fill = new SolidColorBrush(Colors.White);
#endif
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion

        #region Properties

        #region Public Properties

        public double Start
        {
            get { return (double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }
        
        public double End
        {
            get { return (double)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public BreakLineType LineType
        {
            get { return (BreakLineType)GetValue(LineTypeProperty); }
            set { SetValue(LineTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scale break spacing.
        /// </summary>
        public double BreakSpacing
        {
            get { return (double)GetValue(BreakSpacingProperty); }
            set { SetValue(BreakSpacingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the percent where breaks will be positioned at the specified percent of the area.
        /// This will be considered when the enum BreakPosition is set to Percent mode.
        /// </summary>
        public double BreakPercent
        {
            get { return (double)GetValue(BreakPercentProperty); }
            set { SetValue(BreakPercentProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods
        
        /// <summary>
        /// Clone the axis breaks
        /// </summary>
        /// <returns></returns>
        public DependencyObject Clone()
        {
            return CloneAxisBreaks(null);
        }

        #endregion

        #region Internal Static Methods

        internal static void DrawPath(NumericalAxis axis)
        {
            ISupportAxes regSeries = null;
            if (axis.RegisteredSeries != null && axis.RegisteredSeries.Count > 0)
                regSeries = axis.RegisteredSeries[0];

            double top = 0, bottom = 0, left = 0, right = 0;

            if (axis.BreakRanges == null) return;

            var rect = axis.Area.SeriesClipRect;
            var plotOffset = axis.ActualPlotOffset;
            double width = axis.RenderedRect.Width;
            double height = axis.RenderedRect.Height;

            foreach (var scaleBreak in axis.BreakRangesInfo)
            {
                if (!(scaleBreak.Key.Start > axis.VisibleRange.Start && scaleBreak.Key.End < axis.VisibleRange.End))
                    continue;
                var panel = (axis.axisElementsPanel as ChartCartesianAxisElementsPanel);
                double axisThickness = panel.MainAxisLine.StrokeThickness;

                var cartesianSeries = regSeries as CartesianSeries;
                if (axis.Orientation == Orientation.Horizontal)
                {
                    left = plotOffset + rect.Left + Math.Round(width * regSeries.ActualYAxis.ValueToCoefficientCalc(scaleBreak.Key.Start));
                    right = plotOffset + rect.Left + Math.Round(width * regSeries.ActualYAxis.ValueToCoefficientCalc(scaleBreak.Key.End));
                    if (cartesianSeries.YAxis != null)
                    {
                        DrawBreakLineOnAxis(left, top, right, bottom, axis, panel, scaleBreak);
                    }

                    if (axis.OpposedPosition)
                    {
                        top = rect.Top - axisThickness;
                        bottom = rect.Top + rect.Height + 1.5;
                    }
                    else
                    {
                        top = rect.Top;
                        bottom = rect.Top + rect.Height + axisThickness + 1.5;
                    }
                }
                else
                {
                    top = rect.Top + plotOffset + 0.5 + Math.Round(height * (1 - regSeries.ActualYAxis.ValueToCoefficientCalc(scaleBreak.Key.End)));
                    bottom = rect.Top + plotOffset + 0.5 + Math.Round(height * (1 - regSeries.ActualYAxis.ValueToCoefficientCalc(scaleBreak.Key.Start)));
                    if (cartesianSeries.YAxis != null)
                    {
                        DrawBreakLineOnAxis(left, top, right, bottom, axis, panel, scaleBreak);
                    }

                    if (axis.OpposedPosition)
                    {
                        left = rect.Left - 0.5;
                        right = rect.Left + rect.Width + axisThickness + 0.5;
                    }
                    else
                    {
                        left = rect.Left - axisThickness - 0.5;
                        right = rect.Left + rect.Width + 0.5;
                    }
                }

                CalculateDrawingPoints(axis, scaleBreak, left, top, right, bottom);
            }
        }

        #endregion

        #region Protected Virtual Methods

        protected virtual DependencyObject CloneAxisBreaks(DependencyObject obj)
        {
            ChartAxisScaleBreak scaleBreak = new ChartAxisScaleBreak();
            scaleBreak.Start = this.Start;
            scaleBreak.End = this.End;
            scaleBreak.Fill = this.Fill;
            scaleBreak.LineType = this.LineType;
            scaleBreak.BreakSpacing = this.BreakSpacing;
            scaleBreak.BreakPercent = this.BreakPercent;
            scaleBreak.StrokeThickness = this.StrokeThickness;
            scaleBreak.Stroke = this.Stroke;
            return scaleBreak;
        }

        #endregion

        #region Private Static Methods
        
        private static void OnStartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxisScaleBreak).OnPropertyChanged("Start");
        }

        private static void OnEndPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxisScaleBreak).OnPropertyChanged("End");
        }

        private static void OnLineTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxisScaleBreak).OnPropertyChanged("LineType");
        }

        private static void OnBreakSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxisScaleBreak).OnPropertyChanged("BreakSpacing");
        }

        private static void OnBreakPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxisScaleBreak).OnPropertyChanged("BreakPercent");
        }

        private static void DrawLine(double left, double top, double right, double bottom, NumericalAxis axis, KeyValuePair<DoubleRange, ChartAxisScaleBreak> scaleBreak)
        {
            Polyline line1 = new Polyline();
            Polyline line2 = new Polyline();
            Polyline line3 = new Polyline();

            if (axis.Orientation == Orientation.Horizontal)
            {
                line1.Points.Add(new Point(left, top));
                line1.Points.Add(new Point(left, bottom));

                line2.Points.Add(new Point(right, top));
                line2.Points.Add(new Point(right, bottom));

                line3.Points.Add(new Point(left, top));
                line3.Points.Add(new Point(left, bottom));

                line3.Points.Add(new Point(right, bottom));
                line3.Points.Add(new Point(right, top));
            }
            else
            {
                line1.Points.Add(new Point(left, top));
                line1.Points.Add(new Point(right, top));

                line2.Points.Add(new Point(left, bottom));
                line2.Points.Add(new Point(right, bottom));

                line3.Points.Add(new Point(left, top));
                line3.Points.Add(new Point(right, top));

                line3.Points.Add(new Point(right, bottom));
                line3.Points.Add(new Point(left, bottom));
            }

            line1.SetBinding(Shape.StrokeProperty, CreateBinding("Stroke", scaleBreak.Value));
            line2.SetBinding(Shape.StrokeProperty, CreateBinding("Stroke", scaleBreak.Value));

            line1.SetBinding(Shape.StrokeThicknessProperty, CreateBinding("StrokeThickness", scaleBreak.Value));
            line2.SetBinding(Shape.StrokeThicknessProperty, CreateBinding("StrokeThickness", scaleBreak.Value));
            line3.SetBinding(Shape.FillProperty, CreateBinding("Fill", scaleBreak.Value));

            if (!(axis.Area.AdorningCanvas.Children.Contains(line3)))
                axis.Area.AdorningCanvas.Children.Add(line3);
            if (!(axis.Area.AdorningCanvas.Children.Contains(line2)))
                axis.Area.AdorningCanvas.Children.Add(line2);
            if (!(axis.Area.AdorningCanvas.Children.Contains(line1)))
                axis.Area.AdorningCanvas.Children.Add(line1);
            axis.BreakShapes.Add(line1);
            axis.BreakShapes.Add(line2);
            axis.BreakShapes.Add(line3);
        }

        private static void CalculateDrawingPoints(NumericalAxis axis, KeyValuePair<DoubleRange, ChartAxisScaleBreak> scaleBreak, double left, double top, double right, double bottom)
        {
            if (scaleBreak.Value.LineType == BreakLineType.StraightLine)
            {
                DrawLine(left, top, right, bottom, axis, scaleBreak);
            }
            else
            {
                Path path = new Path();
                Point[] points;
                Point from;
                Point to;
                if (axis.Orientation == Orientation.Vertical)
                {
                    if (axis.IsInversed)
                    {
                        from = new Point(left, bottom);
                        to = new Point(right, bottom);
                    }
                    else
                    {
                        from = new Point(left, top);
                        to = new Point(right, top);
                    }

                    points = axis.OpposedPosition
                        ? GetWaveBeziersPoints(to, from, 10, 5)
                        : GetWaveBeziersPoints(from, to, 10, 5);
                }
                else
                {
                    if (axis.IsInversed)
                    {
                        from = new Point(right, top);
                        to = new Point(right, bottom);
                    }
                    else
                    {
                        from = new Point(left, top);
                        to = new Point(left, bottom);
                    }

                    points = axis.OpposedPosition
                        ? GetWaveBeziersPoints(from, to, 10, 5)
                        : GetWaveBeziersPoints(to, from, 10, 5);
                }

                Point[] nearPoints = new Point[points.Length];
                Point[] farPoints = new Point[points.Length];
                double dx = to.X - from.X;
                double dy = to.Y - from.Y;
                double d = Math.Sqrt(dx * dx + dy * dy);
                double offset = scaleBreak.Value.BreakSpacing;
                double offsetX = offset * dy / d;
                double offsetY = offset * dx / d;

                for (int i = 0; i < points.Length; i++)
                {
                    nearPoints[i] = new Point(points[i].X, points[i].Y);
                    farPoints[points.Length - i - 1] = new Point(points[i].X + offsetX, points[i].Y + offsetY);
                }

                PathFigure figure1 = new PathFigure { StartPoint = new Point(nearPoints[0].X, nearPoints[0].Y) };
                Point point1, point2, point3;
                for (int i = 1; i < nearPoints.Length; i = i + 3)
                {
                    point1 = new Point(nearPoints[i].X, nearPoints[i].Y);
                    point2 = new Point(nearPoints[i + 1].X, nearPoints[i + 1].Y);
                    point3 = new Point(nearPoints[i + 2].X, nearPoints[i + 2].Y);
                    figure1.Segments.Add(new BezierSegment { Point1 = point1, Point2 = point2, Point3 = point3 });
                }

                WindowsLinesegment line = new WindowsLinesegment { Point = new Point(farPoints[0].X, farPoints[0].Y) };
                figure1.Segments.Add(line);
                for (int i = 1; i < nearPoints.Length; i = i + 3)
                {
                    point1 = new Point(farPoints[i].X, farPoints[i].Y);
                    point2 = new Point(farPoints[i + 1].X, farPoints[i + 1].Y);
                    point3 = new Point(farPoints[i + 2].X, farPoints[i + 2].Y);
                    figure1.Segments.Add(new BezierSegment { Point1 = point1, Point2 = point2, Point3 = point3 });
                }

                var geometry1 = new PathGeometry();
                geometry1.Figures.Add(figure1);
                path.Data = geometry1;
                path.SetBinding(Shape.StrokeProperty, CreateBinding("Stroke", scaleBreak.Value));
                path.SetBinding(Shape.StrokeThicknessProperty, CreateBinding("StrokeThickness", scaleBreak.Value));
                path.SetBinding(Shape.FillProperty, CreateBinding("Fill", scaleBreak.Value));
                if (!(axis.Area.AdorningCanvas.Children.Contains(path)))
                    axis.Area.AdorningCanvas.Children.Add(path);
                axis.BreakShapes.Add(path);
            }
        }

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

        private static void DrawBreakLineOnAxis(double left, double top, double right, double bottom, NumericalAxis axis, ChartCartesianAxisElementsPanel panel, KeyValuePair<DoubleRange, ChartAxisScaleBreak> scaleBreak)
        {
            if (panel == null)
                return;
            var rect = axis.ArrangeRect;
            double width = axis.axisLabelsPanel.DesiredSize.Width;
            double height = axis.axisLabelsPanel.DesiredSize.Height;
            var line = panel.MainAxisLine;

            if (axis.Orientation == Orientation.Horizontal)
            {
                if (axis.OpposedPosition)
                {
                    if (axis.LabelsPosition == AxisElementPosition.Inside)
                    {
                        top = rect.Top + line.Y1 - 5;
                        bottom = rect.Top + line.Y2 + 5;
                    }
                    else
                    {
                        top = rect.Top + line.Y1 + height - 5;
                        bottom = rect.Top + line.Y2 + height + 5;
                    }
                }
                else
                {
                    if (axis.LabelsPosition == AxisElementPosition.Outside)
                    {
                        top = rect.Top + line.Y1 - 5;
                        bottom = rect.Top + line.Y2 + 5;
                    }
                    else
                    {
                        top = rect.Top + line.Y1 + height - 5;
                        bottom = rect.Top + line.Y2 + height + 5;
                    }
                }
            }
            else
            {
                if (axis.OpposedPosition)
                {
                    if (axis.LabelsPosition == AxisElementPosition.Outside)
                    {
                        left = rect.Left + line.X1 - 5;
                        right = rect.Left + panel.MainAxisLine.X2 + 5;
                    }
                    else
                    {
                        left = rect.Left + line.X1 + width - 5;
                        right = rect.Left + line.X2 + width + 5;
                    }
                }
                else
                {
                    if (axis.LabelsPosition == AxisElementPosition.Inside)
                    {
                        left = rect.Left + line.X1 - 5;
                        right = rect.Left + line.X2 + 5;
                    }
                    else
                    {
                        left = rect.Left + line.X1 + width - 5;
                        right = rect.Left + line.X2 + width + 5;
                    }
                }
            }

            DrawLine(left, top, right, bottom, axis, scaleBreak);
        }

        private static Point[] GetWaveBeziersPoints(Point pt1, Point pt2, int count, float fault)
        {
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);
            double nx = fault * dy / length;
            double ny = fault * dx / length;
            double sx = dx / count;
            double sy = dy / count;

            Point[] points = new Point[3 * count + 1];

            for (int i = 0; i < count; i++)
            {
                points[3 * i] = new Point(pt1.X + sx * i, pt1.Y + sy * i);
                points[3 * i + 1] = new Point(pt1.X + sx * (i + 0.5f) + nx, pt1.Y + sy * (i + 0.5f) + ny);
                points[3 * i + 2] = new Point(pt1.X + sx * (i + 0.5f) - nx, pt1.Y + sy * (i + 0.5f) - ny);
            }

            points[points.Length - 1] = pt2;
            return points;
        }

        #endregion

        #region Private Methods
        
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        
        #endregion

        #endregion
               
    }
}
