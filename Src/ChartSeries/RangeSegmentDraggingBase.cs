using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract partial class RangeSegmentDraggingBase : RangeSeriesBase
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableDragTooltip"/> property.       
        /// </summary>
        public static readonly DependencyProperty EnableDragTooltipProperty =
            DependencyProperty.Register(
                "EnableDragTooltip", 
                typeof(bool), 
                typeof(RangeSegmentDraggingBase),
                new PropertyMetadata(true));

        /// <summary>
        /// The DependencyProperty for <see cref="DragTooltipTemplate"/> property.       
        /// </summary>
        public static readonly DependencyProperty DragTooltipTemplateProperty =
            DependencyProperty.Register(
                "DragTooltipTemplate", 
                typeof(DataTemplate),
                typeof(RangeSegmentDraggingBase),
                new PropertyMetadata(null));

        /// <summary>
        ///  The DependencyProperty for <see cref="RoundToDecimal"/> property.       
        /// </summary>
        public static readonly DependencyProperty RoundToDecimalProperty =
            DependencyProperty.Register(
                "RoundToDecimal",
                typeof(int),
                typeof(RangeSegmentDraggingBase),
                new PropertyMetadata(0));

        /// <summary>
        ///  The DependencyProperty for <see cref="SnapToPoint"/> property.       
        /// </summary>
        public static readonly DependencyProperty SnapToPointProperty =
            DependencyProperty.Register(
                "SnapToPoint",
                typeof(SnapToPoint), 
                typeof(RangeSegmentDraggingBase),
                new PropertyMetadata(SnapToPoint.None));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableSegmentDragging"/> property.       
        /// </summary>
        public static readonly DependencyProperty EnableSegmentDraggingProperty =
            DependencyProperty.Register(
                "EnableSegmentDragging",
                typeof(bool),
                typeof(RangeSegmentDraggingBase),
                new PropertyMetadata(false));

        /// <summary>
        /// The DependencyProperty for <see cref="UpdateSource"/> property.       
        /// </summary>
        public static readonly DependencyProperty UpdateSourceProperty =
            DependencyProperty.Register(
                "UpdateSource", 
                typeof(bool),
                typeof(RangeSegmentDraggingBase),
                new PropertyMetadata(false));

        /// <summary>
        /// The DependencyProperty for <see cref="DragCancelKeyModifiers"/> property. 
        /// </summary>
        public static readonly DependencyProperty DragCancelKeyModifiersProperty =
            DependencyProperty.Register(
                "DragCancelKeyModifiers",
                typeof(VirtualKeyModifiers), 
                typeof(RangeSegmentDraggingBase),
                new PropertyMetadata(VirtualKeyModifiers.None));

        #endregion

        #region Fields

        #region Private Fields

        ContentControl highTooltip, lowTooltip;

        ChartDragPointinfo highDragInfo, lowDragInfo;

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [segment enter].
        /// </summary>
        public event EventHandler<RangeSegmentEnterEventArgs> SegmentEnter;

        /// <summary>
        /// Occurs when [drag start].
        /// </summary>
        public event EventHandler<ChartDragStartEventArgs> DragStart;

        /// <summary>
        /// Occurs when [drag delta].
        /// </summary>
        public event EventHandler<RangeDragEventArgs> DragDelta;

        /// <summary>
        /// Occurs when [drag end].
        /// </summary>
        public event EventHandler<RangeDragEndEventArgs> DragEnd;

        /// <summary>
        /// Occurs when [preview drag end]..
        /// </summary>
        public event EventHandler<RangeDragEventArgs> PreviewDragEnd;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to enable the ToolTip while dragging segment.
        /// </summary>
        public bool EnableDragTooltip
        {
            get { return (bool)GetValue(EnableDragTooltipProperty); }
            set { SetValue(EnableDragTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the dragging tooltip.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate DragTooltipTemplate
        {
            get { return (DataTemplate)GetValue(DragTooltipTemplateProperty); }
            set { SetValue(DragTooltipTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the round off decimal places for the ToolTip value.
        /// </summary>
        public int RoundToDecimal
        {
            get { return (int)GetValue(RoundToDecimalProperty); }
            set { SetValue(RoundToDecimalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the segment dropping mechanism as per requirement.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.SnapToPoint"/>
        /// </value>
        public SnapToPoint SnapToPoint
        {
            get { return (SnapToPoint)GetValue(SnapToPointProperty); }
            set { SetValue(SnapToPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the segment dragging for this series.
        /// </summary>
        public bool EnableSegmentDragging
        {
            get { return (bool)GetValue(EnableSegmentDraggingProperty); }
            set { SetValue(EnableSegmentDraggingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update the underlying source value while dragging.
        /// </summary>
        public bool UpdateSource
        {
            get { return (bool)GetValue(UpdateSourceProperty); }
            set { SetValue(UpdateSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the key modifier to cancel the dragging.
        /// </summary>
        /// <value>
        /// The <c>VirtualKeyModifiers</c> value.
        /// </value>
        public VirtualKeyModifiers DragCancelKeyModifiers
        {
            get { return (VirtualKeyModifiers)GetValue(DragCancelKeyModifiersProperty); }
            set { SetValue(DragCancelKeyModifiersProperty, value); }
        }

        #endregion

        #region Protected Properties

        protected int SegmentIndex { get; set; }

        protected ContentControl DragSpliterHigh { get; set; }

        protected ContentControl DragSpliterLow { get; set; }

        protected double DraggedValue { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal Virtual Methods

        internal virtual void ResetDragSpliter()
        {
            if (DragSpliterHigh != null)
            {
                SeriesPanel.Children.Remove(DragSpliterHigh);
                DragSpliterHigh = null;
            }

            if (DragSpliterLow != null)
            {
                SeriesPanel.Children.Remove(DragSpliterLow);
                DragSpliterLow = null;
            }
        }
        internal virtual void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            KeyDown -= CoreWindow_KeyDown;
            UnHoldPanning(true);
            if (dragEndEvent)
                RaiseDragEnd(new RangeDragEndEventArgs());
            ResetSegmentDragTooltipInfo();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the snap to point.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <returns></returns>
        internal double GetSnapToPoint(double actualValue)
        {
            var outValue = actualValue;

            switch (SnapToPoint)
            {
                case SnapToPoint.Round:
                    {
                        outValue = Math.Round(actualValue, RoundToDecimal);
                        break;
                    }

                case SnapToPoint.Ceil:
                    {
                        outValue = Math.Ceiling(actualValue);
                        break;
                    }

                case SnapToPoint.Floor:
                    {
                        outValue = Math.Floor(actualValue);
                        break;
                    }
            }

            return outValue;
        }

        internal void UnHoldPanning(bool value)
        {
            var eumerator = Area.Behaviors.OfType<ChartZoomPanBehavior>();
            foreach (var behavior in eumerator)
            {
                behavior.InternalEnablePanning = value;
                behavior.InternalEnableSelectionZooming = value;
            }
        }

        internal void UpdateSegmentDragValueToolTipHigh(Point pos, ChartSegment segment, double newValue, double offsetY)
        {
            if (!EnableDragTooltip) return;
            if (highTooltip == null)
            {
                highDragInfo = new ChartDragSegmentInfo() { PostfixLabelTemplate = ActualYAxis.PostfixLabelTemplate, PrefixLabelTemplate = ActualYAxis.PostfixLabelTemplate };
                highTooltip = new ContentControl { Content = highDragInfo };
                SeriesPanel.Children.Add(highTooltip);
                if (DragTooltipTemplate == null)
                    highTooltip.ContentTemplate = IsActualTransposed ? ChartDictionaries.GenericCommonDictionary["SegmentDragInfoOppRight"] as DataTemplate :
                        ChartDictionaries.GenericCommonDictionary["SegmentDragInfo"] as DataTemplate;
                else
                    highTooltip.ContentTemplate = DragTooltipTemplate;
            }

            highDragInfo.Segment = segment;
            highDragInfo.Brush = segment.Interior;
            highDragInfo.ScreenCoordinates = pos;
            ((ChartDragSegmentInfo)highDragInfo).NewValue = newValue;
            highDragInfo.Segment = segment;
            highTooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (IsActualTransposed)
            {
                if (pos.X < 0)
                {
                    Canvas.SetTop(highTooltip, pos.Y - highTooltip.DesiredSize.Height / 2);
                    Canvas.SetLeft(highTooltip, 0);
                }
                else
                {
                    Canvas.SetTop(highTooltip, pos.Y - highTooltip.DesiredSize.Height / 2);
                    Canvas.SetLeft(highTooltip, pos.X);
                }
            }
            else
            {
                double posY = pos.Y - highTooltip.DesiredSize.Height;
                if (posY < 0)
                {
                    Canvas.SetTop(highTooltip, 0);
                    Canvas.SetLeft(highTooltip, pos.X - highTooltip.DesiredSize.Width / 2);
                }
                else
                {
                    Canvas.SetTop(highTooltip, posY);
                    Canvas.SetLeft(highTooltip, pos.X - highTooltip.DesiredSize.Width / 2);
                }
            }
        }

        internal void UpdateSegmentDragValueToolTipLow(Point pos, ChartSegment segment, double newValue)
        {
            if (!EnableDragTooltip) return;
            if (lowTooltip == null)
            {
                lowDragInfo = new ChartDragSegmentInfo() { PostfixLabelTemplate = ActualYAxis.PostfixLabelTemplate, PrefixLabelTemplate = ActualYAxis.PostfixLabelTemplate };
                lowTooltip = new ContentControl { Content = lowDragInfo };
                SeriesPanel.Children.Add(lowTooltip);
                if (DragTooltipTemplate == null)
                    lowTooltip.ContentTemplate = IsActualTransposed ? ChartDictionaries.GenericCommonDictionary["SegmentDragInfoOppLeft"] as DataTemplate :
                        ChartDictionaries.GenericCommonDictionary["SegmentDragInfoOppBottom"] as DataTemplate;
                else
                    lowTooltip.ContentTemplate = DragTooltipTemplate;
            }

            lowDragInfo.Segment = segment;
            lowDragInfo.ScreenCoordinates = pos;
            lowDragInfo.Brush = segment.Interior;
            ((ChartDragSegmentInfo)lowDragInfo).NewValue = newValue;
            lowDragInfo.Segment = segment;
            lowTooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (IsActualTransposed)
            {
                if ((pos.X + lowTooltip.DesiredSize.Width > ActualWidth))
                {
                    Canvas.SetTop(lowTooltip, ActualWidth - lowTooltip.DesiredSize.Width);
                    Canvas.SetLeft(lowTooltip, pos.X - lowTooltip.DesiredSize.Height / 2);
                }
                else
                {
                    Canvas.SetTop(lowTooltip, pos.Y - lowTooltip.DesiredSize.Height / 2);
                    Canvas.SetLeft(lowTooltip, pos.X - lowTooltip.DesiredSize.Width);
                }
            }
            else
            {
                if ((pos.Y + lowTooltip.DesiredSize.Height > ActualHeight))
                {
                    Canvas.SetTop(lowTooltip, ActualHeight - lowTooltip.DesiredSize.Height);
                    Canvas.SetLeft(lowTooltip, pos.X - lowTooltip.DesiredSize.Width / 2);
                }
                else
                {
                    Canvas.SetTop(lowTooltip, pos.Y);
                    Canvas.SetLeft(lowTooltip, pos.X - lowTooltip.DesiredSize.Width / 2);
                }
            }
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Updates the drag spliter high.
        /// </summary>
        /// <param name="rect">The rect.</param>
        protected virtual void UpdateDragSpliterHigh(Rectangle rect)
        {
            int index = Segments.IndexOf(rect.Tag as ChartSegment);
            var args = new RangeSegmentEnterEventArgs()
            {
                XValue = GetActualXValue(index),
                SegmentIndex = index,
                CanDrag = true,
                HighValue = HighValues[index],
                LowValue = LowValues[index]
            };
            RaiseDragEnter(args);
            if (!args.CanDrag) return;

            if (DragSpliterHigh == null)
            {
                DragSpliterHigh = new ContentControl();
                SeriesPanel.Children.Add(DragSpliterHigh);
                if (IsActualTransposed)
                    DragSpliterHigh.Template = ChartDictionaries.GenericCommonDictionary["DragSpliterLeft"] as ControlTemplate;
                else
                    DragSpliterHigh.Template = ChartDictionaries.GenericCommonDictionary["DragSpliterTop"] as ControlTemplate;
            }

            double canvasLeft, canvasTop, spliterHeight, spliterWidth, margin;

            if (IsActualTransposed)
            {
                canvasTop = Canvas.GetTop(rect);
                double height = rect.Height;
                margin = height / 3;
                spliterHeight = height - margin * 2;
                DragSpliterHigh.Margin = new Thickness(0, margin, 0, margin);
                canvasLeft = Canvas.GetLeft(rect) + rect.Width - margin / 2;
                spliterWidth = spliterHeight / 5;
            }
            else
            {
                double top = Canvas.GetTop(rect);
                double width = rect.Width;
                margin = width / 3;
                spliterWidth = width - margin * 2;
                DragSpliterHigh.Margin = new Thickness(margin, 0, margin, 0);
                canvasLeft = Canvas.GetLeft(rect);
                canvasTop = top + 7;
                spliterHeight = spliterWidth / 5;
            }

            DragSpliterHigh.SetValue(Canvas.LeftProperty, canvasLeft);
            DragSpliterHigh.SetValue(Canvas.TopProperty, canvasTop);
            DragSpliterHigh.Height = spliterHeight;
            DragSpliterHigh.Width = spliterWidth;
        }

        /// <summary>
        /// Updates the drag spliter low.
        /// </summary>
        /// <param name="rect">The rect.</param>
        protected virtual void UpdateDragSpliterLow(Rectangle rect)
        {
            int index = Segments.IndexOf(rect.Tag as ChartSegment);
            var args = new RangeSegmentEnterEventArgs()
            {
                XValue = GetActualXValue(index),
                SegmentIndex = index,
                CanDrag = true,
                HighValue = HighValues[index],
                LowValue = LowValues[index]
            };
            RaiseDragEnter(args);
            if (!args.CanDrag) return;

            if (DragSpliterLow == null)
            {
                DragSpliterLow = new ContentControl();
                SeriesPanel.Children.Add(DragSpliterLow);
                if (IsActualTransposed)
                    DragSpliterLow.Template = ChartDictionaries.GenericCommonDictionary["DragSpliterLeft"] as ControlTemplate;
                else
                    DragSpliterLow.Template = ChartDictionaries.GenericCommonDictionary["DragSpliterTop"] as ControlTemplate;
            }

            double canvasLeft, canvasTop, spliterHeight, spliterWidth, margin;
            if (IsActualTransposed)
            {
                canvasTop = Canvas.GetTop(rect);
                double height = rect.Height;
                margin = height / 3;
                spliterHeight = height - margin * 2;
                DragSpliterLow.Margin = new Thickness(0, margin, 0, margin);
                canvasLeft = Canvas.GetLeft(rect) + margin / 3;
                spliterWidth = spliterHeight / 5;
            }
            else
            {
                double bottom = Canvas.GetTop(rect) + rect.Height;
                double width = rect.Width;
                margin = width / 3;
                spliterWidth = width - margin * 2;
                DragSpliterLow.Margin = new Thickness(margin, 0, margin, 0);
                canvasLeft = Canvas.GetLeft(rect);
                canvasTop = bottom - 7;
                spliterHeight = spliterWidth / 5;
            }

            DragSpliterLow.SetValue(Canvas.LeftProperty, canvasLeft);
            DragSpliterLow.SetValue(Canvas.TopProperty, canvasTop);
            DragSpliterLow.Height = spliterHeight;
            DragSpliterLow.Width = spliterWidth;
        }

        protected virtual void OnChartDragStart(Point mousePos, object originalSource)
        {
        }

        protected virtual void OnChartDragDelta(Point mousePos, object originalSource)
        {
        }

        protected virtual void OnChartDragEnd(Point mousePos, object originalSource)
        {
        }

        protected virtual void OnChartDragEntered(Point mousePos, object originalSource)
        {
        }

        protected virtual void OnChartDragExited(Point mousePos, object originalSource)
        {
            ResetDragSpliter();
        }

        #endregion

        #region Protected Override Methods

#if NETFX_CORE

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragDelta(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            base.OnPointerMoved(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
            {
                OnChartDragStart(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
                SeriesPanel.CapturePointer(e.Pointer);
            }

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
            {
                OnChartDragEnd(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
                SeriesPanel.ReleasePointerCapture(e.Pointer);
            }

            base.OnPointerReleased(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragEntered(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (EnableSegmentDragging)
                OnChartDragExited(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            base.OnPointerExited(e);
        }

#endif

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the under laying model.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="index">The index.</param>
        /// <param name="updatedData">The updated data.</param>
        protected void UpdateUnderLayingModel(string path, int index, object updatedData)
        {
            var enumerator = ItemsSource.GetEnumerator();
            PropertyInfo yPropertyInfo;

            if (enumerator.MoveNext())
            {
                yPropertyInfo = ChartDataUtils.GetPropertyInfo(enumerator.Current, path);
                IPropertyAccessor yPropertyAccessor = null;
                if (yPropertyInfo != null)
                    yPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(yPropertyInfo);
                int i = 0;
                do
                {
                    if (i == index)
                    {
                        yPropertyAccessor.SetValue(enumerator.Current, updatedData);
                        break;
                    }

                    i++;
                }
                while (enumerator.MoveNext());
            }
        }

        protected void RaisePreviewEnd(RangeDragEventArgs args)
        {
            if (PreviewDragEnd != null)
                PreviewDragEnd(this, args);
        }

        protected void RaiseDragStart(ChartDragStartEventArgs args)
        {
            if (DragStart != null)
                DragStart(this, args);
        }

        protected void RaiseDragEnd(RangeDragEndEventArgs args)
        {
            if (DragEnd != null)
                DragEnd(this, args);
        }

        protected void RaiseDragDelta(RangeDragEventArgs args)
        {
            if (DragDelta != null)
                DragDelta(this, args);
        }

        protected void RaiseDragEnter(RangeSegmentEnterEventArgs args)
        {
            if (SegmentEnter != null)
                SegmentEnter(this, args);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Resets the segment drag tooltip information.
        /// </summary>
        private void ResetSegmentDragTooltipInfo()
        {
            if (highTooltip != null)
            {
                SeriesPanel.Children.Remove(highTooltip);
                highTooltip = null;
                highDragInfo = null;
            }

            if (lowTooltip != null)
            {
                SeriesPanel.Children.Remove(lowTooltip);
                lowTooltip = null;
                lowDragInfo = null;
            }
        }
        

        void CoreWindow_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var value = false;
#if NETFX_CORE 
            if (e.Key == VirtualKey.Escape)
            {
                switch (DragCancelKeyModifiers)
                {
                    case VirtualKeyModifiers.Shift:
                        {
                            if (Windows.UI.Xaml.Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Shift) ==
                                CoreVirtualKeyStates.Down)
                                value = true;
                        }

                        break;

                    case VirtualKeyModifiers.Menu:
                        {
                            if (Windows.UI.Xaml.Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Menu) == CoreVirtualKeyStates.Down)
                                value = true;
                        }

                        break;

                    case VirtualKeyModifiers.Windows:
                        {
                            if (Windows.UI.Xaml.Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.LeftWindows) ==
                                CoreVirtualKeyStates.Down)
                                value = true;
                        }

                        break;

                    case VirtualKeyModifiers.Control:
                        {
                            if (Windows.UI.Xaml.Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control) ==
                                CoreVirtualKeyStates.Down)
                                value = true;
                        }

                        break;

                    case VirtualKeyModifiers.None:
                        value = true;
                        break;
                }
            }
#endif
            if (!value) return;
#if NETFX_CORE
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
#endif
            ResetDraggingElements("EscapeKey", true);
        }

        #endregion

        #endregion        
    }

    public partial class RangeDragEventArgs : RangeDragEndEventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to disable/revert the dragging before completing the dragging.
        /// </summary>
        public bool Cancel { get; set; }

        #endregion

        #endregion
    }

    public partial class RangeDragEndEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the current High value.
        /// </summary>
        public double BaseHighValue { get; set; }

        /// <summary>
        /// Gets or sets the current Low value.
        /// </summary>
        public double BaseLowValue { get; set; }

        /// <summary>
        /// Gets or sets the new High value.
        /// </summary>
        public double NewHighValue { get; set; }

        /// <summary>
        /// Gets or sets the new High value.
        /// </summary>
        public double NewLowValue { get; set; }

        #endregion

        #endregion
    }

    public partial class RangeSegmentEnterEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        public object XValue { get; set; }

        public object HighValue { get; set; }

        public object LowValue { get; set; }

        public bool CanDrag { get; set; }

        public int SegmentIndex { get; set; }

        #endregion

        #endregion
    }
}
