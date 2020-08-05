using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.Devices.Input;
using Windows.UI.Input;
using System.Threading.Tasks;
using Windows.UI;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartCrossHairBehavior enables viewing of informations related to Chart coordinates, at mouse over position or at touch contact point inside a Chart.
    /// </summary>
    /// <remarks>
    /// ChartCrossHairBehavior displays a vertical line, horizontal line and a popup like control displaying information about the data point
    /// at touch contact point or at mouse over position. You can also customize the look of cross hair and information displayed in a label.
    /// </remarks>
    public partial class ChartCrossHairBehavior : ChartBehavior
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalAxisLabelAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalAxisLabelAlignmentProperty =
            DependencyProperty.Register(
                "VerticalAxisLabelAlignment",
                typeof(ChartAlignment),
                typeof(ChartCrossHairBehavior),
                new PropertyMetadata(ChartAlignment.Center));

        /// <summary>
        ///  The DependencyProperty for <see cref="HorizontalAxisLabelAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisLabelAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalAxisLabelAlignment",
                typeof(ChartAlignment),
                typeof(ChartCrossHairBehavior),
                new PropertyMetadata(ChartAlignment.Center));

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalLineStyleProperty =
            DependencyProperty.Register(
                "HorizontalLineStyle",
                typeof(Style),
                typeof(ChartCrossHairBehavior),
#if UNIVERSALWINDOWS
                null);
#else
                new PropertyMetadata(ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"]));
#endif

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalLineStyleProperty =
            DependencyProperty.Register(
                "VerticalLineStyle",
                typeof(Style),
                typeof(ChartCrossHairBehavior),
#if UNIVERSALWINDOWS
                null);
#else
                new PropertyMetadata(ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"]));
#endif

        #endregion

        #region Fields

        #region Protected Internal Fields

        protected internal Point CurrentPoint;

        #endregion

        #region Private Fields
        
        private Line verticalLine;
        private Line horizontalLine;
        private int fingerCount = 0;
        private bool isActivated = false;
        private List<ContentControl> labelElements;
        private ObservableCollection<ChartPointInfo> pointInfos;
        private List<FrameworkElement> elements;
        private string labelXValue;
        private string labelYValue;

        #endregion

        #endregion
        
        #region Constructor

        /// <summary>
        /// Called when instance created for ChartCrossHairBehaviour
        /// </summary>
        public ChartCrossHairBehavior()
        {
#if UNIVERSALWINDOWS
          HorizontalLineStyle = ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"] as Style;
          VerticalLineStyle = ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"] as Style;
#endif
            elements = new List<FrameworkElement>();
            verticalLine = new Line();
            horizontalLine = new Line();
            labelElements = new List<ContentControl>();
            pointInfos = new ObservableCollection<ChartPointInfo>();
        }

        #endregion
        
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the alignment for the label appearing in horizontal axis.
        /// </summary>
        public ChartAlignment VerticalAxisLabelAlignment
        {
            get { return (ChartAlignment)GetValue(VerticalAxisLabelAlignmentProperty); }
            set { SetValue(VerticalAxisLabelAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the alignment for the label appearing in vertical axis.
        /// </summary>
        public ChartAlignment HorizontalAxisLabelAlignment
        {
            get { return (ChartAlignment)GetValue(HorizontalAxisLabelAlignmentProperty); }
            set { SetValue(HorizontalAxisLabelAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets the collection of ChartPointInfo
        /// </summary>
        public ObservableCollection<ChartPointInfo> PointInfos
        {
            get
            {
                return pointInfos;
            }

            internal set
            {
                pointInfos = value;
            }
        }

        /// <summary>
        /// Gets or sets the style for horizontal line.
        /// </summary>
        public Style HorizontalLineStyle
        {
            get { return (Style)GetValue(HorizontalLineStyleProperty); }
            set { SetValue(HorizontalLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for vertical line.
        /// </summary>
        public Style VerticalLineStyle
        {
            get { return (Style)GetValue(VerticalLineStyleProperty); }
            set { SetValue(VerticalLineStyleProperty, value); }
        }

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets a value indicating whether the cross hair IsActivated
        /// </summary>
        protected internal bool IsActivated
        {
            get
            {
                return isActivated;
            }

            set
            {
                isActivated = value;
                Activate(isActivated);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        #region Protected Internal Override Methods

        /// <summary>
        /// Method implementation for DetachElements
        /// </summary>
        protected internal override void DetachElements()
        {
            if (this.AdorningCanvas != null)
            {
                foreach (var element in elements)
                {
                    this.AdorningCanvas.Children.Remove(element);
                }

                elements.Clear();
            }
        }

        /// <summary>
        /// Called when Size Changed
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnSizeChanged(SizeChangedEventArgs e)
        {
            if (ChartArea != null && !string.IsNullOrEmpty(labelXValue) && !string.IsNullOrEmpty(labelYValue))
            {
                double y1 = this.ChartArea.ValueToPoint(this.ChartArea.InternalSecondaryAxis, (Convert.ToDouble(labelYValue)));
                double x1 = this.ChartArea.ValueToPoint(this.ChartArea.InternalPrimaryAxis, (Convert.ToDouble(labelXValue)));
                if (!double.IsNaN(y1) && !double.IsNaN(x1))
                {
                    foreach (ContentControl control in labelElements)
                    {
                        DetachElement(control);
                    }

                    this.labelElements.Clear();
                    this.pointInfos.Clear();
                    elements.Clear();
                    this.SetPosition(new Point(x1, y1));
                }
            }
        }

        /// <summary>
        /// Called when Holding the Focus in UIElement
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnHolding(HoldingRoutedEventArgs e)
        {
            if(ChartArea == null)
            {
                return;
            }

            IsActivated = true;

            if (e.PointerDeviceType == PointerDeviceType.Touch)
                ChartArea.HoldUpdate = true;
            if (this.ChartArea != null && this.ChartArea.VisibleSeries.Count > 0 && this.ChartArea.VisibleSeries[0] is CartesianSeries && IsActivated)
            {
                Point point = e.GetPosition(this.AdorningCanvas);

                if (this.ChartArea.SeriesClipRect.Contains(point))
                {
                    foreach (ContentControl control in labelElements)
                    {
                        DetachElement(control);
                    }

                    labelElements.Clear();

                    pointInfos.Clear();

                    elements.Clear();
                    SetPosition(point);
                }
            }
        }

        /// <summary>
        /// Called when Pointer pressed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                IsActivated = false;
            else
                fingerCount++;
        }

        /// <summary>
        /// Called when Pointer moved in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            PointerPoint pointer = e.GetCurrentPoint(this.AdorningCanvas);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                if (fingerCount > 1) return;
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse &&
                 !pointer.Properties.IsLeftButtonPressed
				 || e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                IsActivated = true;

            if (this.ChartArea != null && this.ChartArea.AreaType == ChartAreaType.CartesianAxes && IsActivated)
            {
                CurrentPoint = new Point(pointer.Position.X, pointer.Position.Y);

                if (this.ChartArea.SeriesClipRect.Contains(CurrentPoint))
                {
                    foreach (ContentControl control in labelElements)
                    {
                        DetachElement(control);
                    }

                    labelElements.Clear();

                    pointInfos.Clear();

                    elements.Clear();
                    SetPosition(CurrentPoint);
                }
                else
                {
                    IsActivated = false;
                }
            }
        }

        /// <summary>
        /// Called when PointerExited from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (IsActivated)
            {
                IsActivated = false;
            }

            fingerCount = 0;
        }

        /// <summary>
        /// Called when chart layout updated from chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnLayoutUpdated()
        {
            if (this.ChartArea == null)
            {
                return;
            }

            if (IsActivated)
            {
                foreach (ContentControl control in labelElements)
                {
                    DetachElement(control);
                }

                labelElements.Clear();

                pointInfos.Clear();

                if (this.ChartArea.SeriesClipRect.Contains(CurrentPoint))
                {
                    SetPosition(CurrentPoint);
                }
                else
                {
                    foreach (var element in elements)
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Called when Pointer Released in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (ChartArea == null)
            {
                return;
            }

            if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
            {
                if (IsActivated)
                {
                    IsActivated = false;
                    ChartArea.HoldUpdate = false;
                }

                fingerCount--;
            }
        }
        
        protected internal override void AlignDefaultLabel(
            ChartAlignment verticalAlignemnt, 
            ChartAlignment horizontalAlignment,
            double x, 
            double y, 
            ContentControl control)
        {
            if (horizontalAlignment == ChartAlignment.Near)
            {
                x = x - control.DesiredSize.Width;
                if (control is ContentControl)
                    ((control as ContentControl).Content as ChartPointInfo).X = x;
            }
            else if (horizontalAlignment == ChartAlignment.Center)
            {
                x = x - control.DesiredSize.Width / 2;
                if (control is ContentControl)
                    ((control as ContentControl).Content as ChartPointInfo).X = x;
            }

            if (verticalAlignemnt == ChartAlignment.Near)
            {
                y = y - control.DesiredSize.Height;
                if (control is ContentControl)
                    ((control as ContentControl).Content as ChartPointInfo).Y = y;
            }
            else if (verticalAlignemnt == ChartAlignment.Center)
            {
                y = y - control.DesiredSize.Height / 2;
                if (control is ContentControl)
                    ((control as ContentControl).Content as ChartPointInfo).Y = y;
            }

            Canvas.SetLeft(control, x);
            Canvas.SetTop(control, y);
        }

        #endregion

        #region Protected Internal Virtual Methods

        /// <summary>
        /// Method implementation for Set positions for given point
        /// </summary>
        /// <param name="point"></param>
        protected internal virtual void SetPosition(Point point)
        {
            if (AdorningCanvas == null || double.IsNaN(point.X) || double.IsNaN(point.Y) || !IsActivated) return;

            var seriesLeft = ChartArea.SeriesClipRect.Left;
            var seriesTop = ChartArea.SeriesClipRect.Top;

            double x = point.X;
            double y = point.Y;

            foreach (var element in elements)
            {
                element.Visibility = Visibility.Visible;
            }

            verticalLine.X1 = verticalLine.X2 = x > this.ChartArea.SeriesClipRect.Right ? this.ChartArea.SeriesClipRect.Right : x;
            verticalLine.Y1 = seriesTop;
            verticalLine.Y2 = this.ChartArea.SeriesClipRect.Height + seriesTop;
            elements.Add(verticalLine);

            horizontalLine.Y1 = horizontalLine.Y2 = y;
            horizontalLine.X1 = seriesLeft;
            horizontalLine.X2 = seriesLeft + this.ChartArea.SeriesClipRect.Width;
            elements.Add(horizontalLine);

            foreach (ChartAxis axis in ChartArea.Axes)
            {
                if ((axis.RenderedRect.Left <= point.X && axis.RenderedRect.Right >= point.X)
                    || axis.RenderedRect.Top <= point.Y && axis.RenderedRect.Bottom >= point.Y)
                {
                    double val = this.ChartArea.PointToValue(axis, new Point(point.X - seriesLeft, point.Y - seriesTop));
                    if (!double.IsNaN(val))
                    {
                        ChartPointInfo pointInfo = new ChartPointInfo();
                        pointInfo.Axis = axis;
                        var isDateTimeAxis = axis is DateTimeAxis;

                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            if (ChartArea.VisibleSeries.Count > 0 && ChartArea.VisibleSeries[0].IsIndexed && !ChartArea.VisibleSeries[0].IsActualTransposed)
                            {
                                pointInfo.ValueX = axis.GetLabelContent((int)Math.Round(val)).ToString();
                                var x1 = this.ChartArea.ValueToPoint(axis, Math.Round(val));
                                x1 += seriesLeft;
                                pointInfo.X = verticalLine.X1 = verticalLine.X2 = x1 > this.ChartArea.SeriesClipRect.Right ?
                                    this.ChartArea.SeriesClipRect.Right : x1 < this.ChartArea.SeriesClipRect.Left ? this.ChartArea.SeriesClipRect.Left : x1;
                                pointInfo.BaseX = pointInfo.X;
                            }
                            else
                            {
                                pointInfo.ValueX = isDateTimeAxis ? axis.GetLabelContent(val).ToString() : axis.GetLabelContent(Math.Round(val, 2)).ToString();
                                pointInfo.X = point.X;
                                pointInfo.BaseX = pointInfo.X;
                            }

                            labelXValue = val.ToString();
                        }
                        else
                        {
                            if (ChartArea.VisibleSeries.Count > 0 && ChartArea.VisibleSeries[0].IsIndexed && ChartArea.VisibleSeries[0].IsActualTransposed)
                            {
                                pointInfo.ValueY = axis.GetLabelContent((int)Math.Round(val)).ToString();
                                var y1 = this.ChartArea.ValueToPoint(axis, Math.Round(val));
                                y1 += seriesTop;
                                pointInfo.Y = horizontalLine.Y1 = horizontalLine.Y2 = y1 > this.ChartArea.SeriesClipRect.Bottom ?
                                    this.ChartArea.SeriesClipRect.Bottom : y1 < this.ChartArea.SeriesClipRect.Top ? this.ChartArea.SeriesClipRect.Top : y1;
                                pointInfo.BaseY = pointInfo.Y;
                            }
                            else
                            {
                                pointInfo.ValueY = isDateTimeAxis ? axis.GetLabelContent(val).ToString() : axis.GetLabelContent(Math.Round(val, 2)).ToString();
                                pointInfo.Y = point.Y;
                                pointInfo.BaseY = pointInfo.Y;
                            }

                            labelYValue = val.ToString();
                        }

                        GenerateLabel(pointInfo, axis);
                    }
                }
            }
        }

        #endregion

        #region Protected Override Methods
        
        /// <summary>
        /// Method implementation for AttachElements
        /// </summary>
        protected override void AttachElements()
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("VerticalLineStyle");
            verticalLine.SetBinding(Line.StyleProperty, binding);

            horizontalLine = new Line(); binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("HorizontalLineStyle");
            horizontalLine.SetBinding(Line.StyleProperty, binding);

            if (this.AdorningCanvas != null && !this.AdorningCanvas.Children.Contains(verticalLine))
            {
                this.AdorningCanvas.Children.Add(verticalLine);
                elements.Add(verticalLine);
            }

            if (this.AdorningCanvas != null && !this.AdorningCanvas.Children.Contains(horizontalLine))
            {
                this.AdorningCanvas.Children.Add(horizontalLine);
                elements.Add(horizontalLine);
            }

            IsActivated = false;
        }
        
        protected override DependencyObject CloneBehavior(DependencyObject obj)
        {
            return base.CloneBehavior(new ChartCrossHairBehavior()
            {
                CurrentPoint = this.CurrentPoint,
                HorizontalAxisLabelAlignment = this.HorizontalAxisLabelAlignment,
                VerticalAxisLabelAlignment = this.VerticalAxisLabelAlignment,
                HorizontalLineStyle = this.HorizontalLineStyle,
                VerticalLineStyle = this.VerticalLineStyle,
            });
        }

        #endregion

        #region Protected Virutal Methods

        /// <summary>
        /// Method implementation for GenerateLabel for axis
        /// </summary>
        /// <param name="pointInfo"></param>
        /// <param name="axis"></param>
        protected virtual void GenerateLabel(ChartPointInfo pointInfo, ChartAxis axis)
        {
            if (axis.ShowTrackBallInfo)
            {
                double scrollbar = 0;
                chartAxis = axis;
                DataTemplate axisCrosshairLabelTemplate = axis.CrosshairLabelTemplate ??
                             ChartDictionaries.GenericCommonDictionary["axisCrosshairLabel"] as DataTemplate;

                var chartAxisBase2D = axis as ChartAxisBase2D;
#if WINDOWS_UAP && CHECKLATER
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
#endif
                if (chartAxisBase2D.EnableScrollBar && !chartAxisBase2D.EnableTouchMode)
                {
                    if (axis.Orientation == Orientation.Vertical)
                        scrollbar = chartAxisBase2D.sfChartResizableBar.DesiredSize.Width;
                    else
                        scrollbar = chartAxisBase2D.sfChartResizableBar.DesiredSize.Height;
                }

                if (axis.Orientation == Orientation.Vertical)
                {
                    pointInfo.X = axis.OpposedPosition ? axis.ArrangeRect.Left + scrollbar : axis.ArrangeRect.Right - scrollbar;
                    AddLabel(
                        pointInfo, 
                        VerticalAxisLabelAlignment,
                        GetChartAlignment(axis.OpposedPosition, ChartAlignment.Near),
                        axisCrosshairLabelTemplate);
                }
                else
                {
                    pointInfo.Y = axis.OpposedPosition ? axis.ArrangeRect.Bottom - scrollbar : axis.ArrangeRect.Top + scrollbar;
                    AddLabel(
                        pointInfo, 
                        GetChartAlignment(axis.OpposedPosition, ChartAlignment.Far),
                        HorizontalAxisLabelAlignment,
                        axisCrosshairLabelTemplate);
                }
            }
        }

        /// <summary>
        /// Method implementation for add labels for CrossHair
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="verticalAlignemnt"></param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="template"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected virtual void AddLabel(
            object obj, 
            ChartAlignment verticalAlignemnt,
            ChartAlignment horizontalAlignment,
            DataTemplate template,
            double x,
            double y)
        {
            ContentControl control = new ContentControl();
            control.Content = obj;
            control.ContentTemplate = template;
            AddElement(control);
            labelElements.Add(control);
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            AlignAxisToolTipPolygon(control, verticalAlignemnt, horizontalAlignment, x, y, this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method implementatin for 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="verticalAlignment"></param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="template"></param>
        protected void AddLabel(ChartPointInfo obj, ChartAlignment verticalAlignment, ChartAlignment horizontalAlignment, DataTemplate template)
        {
            if (obj != null && template != null)
            {
                AddLabel(obj, verticalAlignment, horizontalAlignment, template, obj.X, obj.Y);
            }
        }

        /// <summary>
        /// Method implementation for Add elements in UIElement
        /// </summary>
        /// <param name="element"></param>
        protected void AddElement(UIElement element)
        {
            if (!this.AdorningCanvas.Children.Contains(element))
            {
                this.AdorningCanvas.Children.Add(element);
                elements.Add(element as FrameworkElement);
            }
        }

        #endregion

        #region Private Static Methods

        private static ChartAlignment GetChartAlignment(bool isOpposed, ChartAlignment alignment)
        {
            if (isOpposed)
            {
                if (alignment == ChartAlignment.Near)
                    return ChartAlignment.Far;
                else if (alignment == ChartAlignment.Far)
                    return ChartAlignment.Near;
                else
                    return ChartAlignment.Center;
            }
            else
                return alignment;
        }

        #endregion

        #region Private Methods

        private void Activate(bool activate)
        {
            foreach (UIElement element in elements)
            {
                element.Visibility = activate ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #endregion
    }
}
