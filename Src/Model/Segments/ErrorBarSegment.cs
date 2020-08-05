using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a ChartSegment which displays the error bar type series. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.ChartSegment" />
    public partial class ErrorBarSegment : ChartSegment
    {
        #region Fields

        #region Internal Fields

        internal Line HorLine;

        internal Line VerLine;

        internal Line HorLeftCapLine;

        internal Line HorRightCapLine;

        internal Line VerTopCapLine;

        internal Line VerBottomCapLine;

        #endregion

        #region Private Fields

        private readonly ErrorBarSeries _parentSeries;

        private Canvas _canvas;

        private ChartPoint _verToppoint;

        private ChartPoint _verBottompoint;

        private ChartPoint _horLeftpoint;

        private ChartPoint _horRightpoint;

        #endregion

        #endregion

        #region Constructors

        public ErrorBarSegment()
        {
        }

        [Obsolete("Use  ErrorBarSegment(ChartPoint hlpoint, ChartPoint hrpoint, ChartPoint vtpoint, ChartPoint vbpoint, ErrorBarSeries series, object item)")]
        public ErrorBarSegment(Point hlpoint, Point hrpoint, Point vtpoint, Point vbpoint, ErrorBarSeries series, object item)
        {
            base.Series = series;
            _parentSeries = series;
            base.Item = item;
        }

        public ErrorBarSegment(ChartPoint hlpoint, ChartPoint hrpoint, ChartPoint vtpoint, ChartPoint vbpoint, ErrorBarSeries series, object item)
        {
            base.Series = series;
            _parentSeries = series;
            base.Item = item;
        }

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="hlpoint"></param>
        /// <param name="hrpoint"></param>
        /// <param name="vtpoint"></param>
        /// <param name="vbpoint"></param>
        [Obsolete("Use  SetData(ChartPoint hlpoint, ChartPoint hrpoint, ChartPoint vtpoint, ChartPoint vbpoint)")]
        public override void SetData(Point hlpoint, Point hrpoint, Point vtpoint, Point vbpoint)
        {
            _horLeftpoint = new ChartPoint(hlpoint.X, hlpoint.Y);
            _horRightpoint = new ChartPoint(hrpoint.X, hrpoint.Y);
            _verToppoint = new ChartPoint(vtpoint.X, vtpoint.Y);
            _verBottompoint = new ChartPoint(vbpoint.X, vbpoint.Y);

            switch (_parentSeries.Mode)
            {
                case ErrorBarMode.Horizontal:
                    XRange = new DoubleRange(ChartMath.Min(hlpoint.X, hrpoint.X), ChartMath.Max(hlpoint.X, hrpoint.X));
                    YRange = DoubleRange.Empty;
                    break;
                case ErrorBarMode.Vertical:
                    YRange = new DoubleRange(vbpoint.Y, vtpoint.Y);
                    XRange = DoubleRange.Empty;
                    break;
                default:
                    XRange = new DoubleRange(ChartMath.Min(hlpoint.X, hrpoint.X), ChartMath.Max(hlpoint.X, hrpoint.X));
                    YRange = new DoubleRange(vbpoint.Y, vtpoint.Y);
                    break;
            }
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>returns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return _canvas;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            if (transformer != null)
            {
                _canvas.Children.Clear();
                if (HorLine != null && _parentSeries.Mode != ErrorBarMode.Vertical && !(double.IsNaN(_horLeftpoint.Y) || double.IsNaN(_horRightpoint.Y)))
                {
                    var hLPoint = transformer.TransformToVisible(_horLeftpoint.X, _horLeftpoint.Y);
                    var hRPoint = transformer.TransformToVisible(_horRightpoint.X, _horRightpoint.Y);

                    HorLine.X1 = hLPoint.X;
                    HorLine.Y1 = hLPoint.Y;
                    HorLine.X2 = hRPoint.X;
                    HorLine.Y2 = hRPoint.Y;
                    _canvas.Children.Add(HorLine);

                    if (_parentSeries.HorizontalCapLineStyle.Visibility == Visibility.Visible)
                    {
                        var horWidth = _parentSeries.HorizontalCapLineStyle.LineWidth / 2;
                        if (_parentSeries.IsTransposed)
                        {
                            HorLeftCapLine.X1 = HorLine.X1 - horWidth;
                            HorLeftCapLine.Y1 = HorLine.Y1;
                            HorLeftCapLine.X2 = HorLine.X1 + horWidth;
                            HorLeftCapLine.Y2 = HorLine.Y1;
                        }
                        else
                        {
                            HorLeftCapLine.X1 = HorLine.X1;
                            HorLeftCapLine.Y1 = HorLine.Y1 + horWidth;
                            HorLeftCapLine.X2 = HorLine.X1;
                            HorLeftCapLine.Y2 = HorLine.Y1 - horWidth;
                        }
                        _canvas.Children.Add(HorLeftCapLine);
                        if (_parentSeries.HorizontalDirection == ErrorBarDirection.Plus)
                            HorLeftCapLine.Visibility = Visibility.Collapsed;
                        else
                            HorLeftCapLine.Visibility = Visibility.Visible;
                        if (_parentSeries.IsTransposed)
                        {
                            HorRightCapLine.X1 = HorLine.X2 - horWidth;
                            HorRightCapLine.Y1 = HorLine.Y2;
                            HorRightCapLine.X2 = HorLine.X2 + horWidth;
                            HorRightCapLine.Y2 = HorLine.Y2;
                        }
                        else
                        {
                            HorRightCapLine.X1 = HorLine.X2;
                            HorRightCapLine.Y1 = HorLine.Y2 + horWidth;
                            HorRightCapLine.X2 = HorLine.X2;
                            HorRightCapLine.Y2 = HorLine.Y2 - horWidth;
                        }
                        _canvas.Children.Add(HorRightCapLine);
                        if (_parentSeries.HorizontalDirection == ErrorBarDirection.Minus)
                            HorRightCapLine.Visibility = Visibility.Collapsed;
                        else
                            HorRightCapLine.Visibility = Visibility.Visible;
                    }
                }

                if (VerLine != null && _parentSeries.Mode != ErrorBarMode.Horizontal && !(double.IsNaN(_verToppoint.Y) || double.IsNaN(_verBottompoint.Y)))
                {
                    var vTPoint = transformer.TransformToVisible(_verToppoint.X, _verToppoint.Y);
                    var vBPoint = transformer.TransformToVisible(_verBottompoint.X, _verBottompoint.Y);

                    this.VerLine.X1 = vTPoint.X;
                    this.VerLine.Y1 = vTPoint.Y;
                    this.VerLine.X2 = vBPoint.X;
                    this.VerLine.Y2 = vBPoint.Y;

                    _canvas.Children.Add(VerLine);
                    if (_parentSeries.VerticalCapLineStyle.Visibility == Visibility.Visible)
                    {
                        var halfwidth = _parentSeries.VerticalCapLineStyle.LineWidth / 2;
                        if (_parentSeries.IsTransposed)
                        {
                            VerBottomCapLine.X1 = VerLine.X1;
                            VerBottomCapLine.Y1 = VerLine.Y1 + halfwidth;
                            VerBottomCapLine.X2 = VerLine.X1;
                            VerBottomCapLine.Y2 = VerLine.Y1 - halfwidth;
                        }
                        else
                        {
                            VerBottomCapLine.X1 = VerLine.X1 - halfwidth;
                            VerBottomCapLine.Y1 = VerLine.Y1;
                            VerBottomCapLine.X2 = VerLine.X1 + halfwidth;
                            VerBottomCapLine.Y2 = VerLine.Y1;
                        }
                        _canvas.Children.Add(VerBottomCapLine);
                        if (_parentSeries.VerticalDirection == ErrorBarDirection.Plus)
                            VerBottomCapLine.Visibility = Visibility.Collapsed;
                        else
                            VerBottomCapLine.Visibility = Visibility.Visible;
                        if (_parentSeries.IsTransposed)
                        {
                            VerTopCapLine.X1 = VerLine.X2;
                            VerTopCapLine.Y1 = VerLine.Y2 + halfwidth;
                            VerTopCapLine.X2 = VerLine.X2;
                            VerTopCapLine.Y2 = VerLine.Y2 - halfwidth;
                        }
                        else
                        {
                            VerTopCapLine.X1 = VerLine.X1 - halfwidth;
                            VerTopCapLine.Y1 = VerLine.Y2;
                            VerTopCapLine.X2 = VerLine.X1 + halfwidth;
                            VerTopCapLine.Y2 = VerLine.Y2;
                        }
                        _canvas.Children.Add(VerTopCapLine);
                        if (_parentSeries.VerticalDirection == ErrorBarDirection.Minus)
                            VerTopCapLine.Visibility = Visibility.Collapsed;
                        else
                            VerTopCapLine.Visibility = Visibility.Visible;
                    }
                }
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

        public override void SetData(ChartPoint hlpoint, ChartPoint hrpoint, ChartPoint vtpoint, ChartPoint vbpoint)
        {
            _horLeftpoint = hlpoint;
            _horRightpoint = hrpoint;
            _verToppoint = vtpoint;
            _verBottompoint = vbpoint;

            switch (_parentSeries.Mode)
            {
                case ErrorBarMode.Horizontal:
                    XRange = new DoubleRange(ChartMath.Min(hlpoint.X, hrpoint.X), ChartMath.Max(hlpoint.X, hrpoint.X));
                    YRange = DoubleRange.Empty;
                    break;
                case ErrorBarMode.Vertical:
                    YRange = new DoubleRange(vbpoint.Y, vtpoint.Y);
                    XRange = DoubleRange.Empty;
                    break;
                default:
                    XRange = new DoubleRange(ChartMath.Min(hlpoint.X, hrpoint.X), ChartMath.Max(hlpoint.X, hrpoint.X));
                    YRange = new DoubleRange(vbpoint.Y, vtpoint.Y);
                    break;
            }
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// returns UI Element
        /// </returns>
        public override UIElement CreateVisual(Size size)
        {
            _canvas = new Canvas();

            HorLine = new Line();
            _canvas.Children.Add(HorLine);
            HorLine.Tag = this;

            HorLeftCapLine = new Line();
            _canvas.Children.Add(HorLeftCapLine);

            HorRightCapLine = new Line();
            _canvas.Children.Add(HorRightCapLine);

            HorRightCapLine.Tag = HorLeftCapLine.Tag = this;

            VerLine = new Line();
            _canvas.Children.Add(VerLine);
            VerLine.Tag = this;

            VerBottomCapLine = new Line();
            _canvas.Children.Add(VerBottomCapLine);

            VerTopCapLine = new Line();
            _canvas.Children.Add(VerTopCapLine);

            VerTopCapLine.Tag = VerBottomCapLine.Tag = this;

            UpdateVisualBinding();

            return _canvas;
        }

        #endregion

        #region Internal Methods

        internal Point DateTimeIntervalCalculation(double errorvalue, DateTimeIntervalType type)
        {
            DateTime date = Convert.ToDouble(_horLeftpoint.X).FromOADate();
            DateTime date1 = Convert.ToDouble(_horRightpoint.X).FromOADate();
            if (_parentSeries.HorizontalDirection == ErrorBarDirection.Plus)
            {
                _horLeftpoint.X = DateTimeAxisHelper.IncreaseInterval(date, 0, type).ToOADate();
                _horRightpoint.X = DateTimeAxisHelper.IncreaseInterval(date1, errorvalue, type).ToOADate();
            }
            else if (_parentSeries.HorizontalDirection == ErrorBarDirection.Minus)
            {
                _horLeftpoint.X = DateTimeAxisHelper.IncreaseInterval(date, -errorvalue, type).ToOADate();
                _horRightpoint.X = DateTimeAxisHelper.IncreaseInterval(date1, 0, type).ToOADate();
            }
            else
            {
                _horLeftpoint.X = DateTimeAxisHelper.IncreaseInterval(date, -errorvalue, type).ToOADate();
                _horRightpoint.X = DateTimeAxisHelper.IncreaseInterval(date1, errorvalue, type).ToOADate();
            }
            return new Point(_horLeftpoint.X, _horRightpoint.X);
        }

        internal void UpdateVisualBinding()
        {
            SetVisualBindings(HorLine, _parentSeries.HorizontalLineStyle);
            SetVisualBindings(HorLeftCapLine, _parentSeries.HorizontalCapLineStyle);
            SetVisualBindings(HorRightCapLine, _parentSeries.HorizontalCapLineStyle);
            SetVisualBindings(VerLine, _parentSeries.VerticalLineStyle);
            SetVisualBindings(VerBottomCapLine, _parentSeries.VerticalCapLineStyle);
            SetVisualBindings(VerTopCapLine, _parentSeries.VerticalCapLineStyle);
        }

        #endregion

        #endregion

        #region Private Methods

        void SetVisualBindings(Shape element, DependencyObject linestyle)
        {
            Binding binding;
            var check = element != HorLine && element != VerLine;
            if (check)
            {
                binding = new Binding { Source = linestyle, Path = new PropertyPath("Visibility") };
                element.SetBinding(Shape.VisibilityProperty, binding);
            }

            binding = new Binding { Source = linestyle, Path = new PropertyPath("Stroke") };
            element.SetBinding(Shape.StrokeProperty, binding);

            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeThickness") };
            element.SetBinding(Shape.StrokeThicknessProperty, binding);

            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeDashCap") };
            element.SetBinding(Shape.StrokeDashCapProperty, binding);

            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeEndLineCap") };
            element.SetBinding(Shape.StrokeEndLineCapProperty, binding);

            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeLineJoin") };
            element.SetBinding(Shape.StrokeLineJoinProperty, binding);

            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeMiterLimit") };
            element.SetBinding(Shape.StrokeMiterLimitProperty, binding);

            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeDashOffset") };
            element.SetBinding(Shape.StrokeDashOffsetProperty, binding);

            var lineStyle = linestyle as LineStyle;
            var collection = lineStyle.StrokeDashArray;
            if (collection != null && collection.Count > 0)
            {
                var doubleCollection = new DoubleCollection();
                foreach (double value in collection)
                {
                    doubleCollection.Add(value);
                }
                element.StrokeDashArray = doubleCollection;
            }
        }

        #endregion

    }
}
