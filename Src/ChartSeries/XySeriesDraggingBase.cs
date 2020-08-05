using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Devices.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// XySeriesDraggingBase is abstract class which is used to allow, drag a chart series in chart area.
    /// </summary>
    /// <seealso cref="EnableSeriesDragging"/>
    public abstract partial class XySeriesDraggingBase : XySegmentDraggingBase
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableSeriesDragging"/> property.       .
        /// </summary>
        public static readonly DependencyProperty EnableSeriesDraggingProperty =
            DependencyProperty.Register(
                "EnableSeriesDragging",
                typeof(bool),
                typeof(XySeriesDraggingBase),
                new PropertyMetadata(false, OnEnableDraggingChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to enable the series dragging. We can drag the series, if its <c>true</c>.
        /// </summary>
        public bool EnableSeriesDragging
        {
            get { return (bool)GetValue(EnableSeriesDraggingProperty); }
            set { SetValue(EnableSeriesDraggingProperty, value); }
        }

        #endregion

        #region Internal Properties

        internal Ellipse DraggingPointIndicator { get; set; }

        internal UIElement PreviewSeries { get; set; }

        internal ChartSegment DraggingSegment { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal Virtual Methods

        internal virtual void UpdatePreivewSeriesDragging(Point mousePos)
        {
        }

        internal virtual void UpdatePreviewSegmentDragging(Point mousePos)
        {
        }

        #endregion

        #region Internal Override Methods

        internal override void ActivateDragging(Point mousePos, object element)
        {
#if NETFX_CORE
            Focus(FocusState.Keyboard);
#endif
            KeyDown += CoreWindow_KeyDown;
            double x, y, stackedValue;
            FindNearestChartPoint(mousePos, out x, out y, out stackedValue);
            delta = 0;
            SegmentIndex = (int)(IsIndexed || ActualXValues is IList<string> ? x : ((IList<double>)ActualXValues).IndexOf(x));
            if (SegmentIndex < 0) return;
            var dragEventArgs = new ChartDragStartEventArgs { BaseXValue = GetActualXValue(SegmentIndex) };

            if (EmptyPointIndexes != null)
            {
                var emptyPointIndex = EmptyPointIndexes[0];
                for (var i = 0; i < emptyPointIndex.Count; i++)
                    if (SegmentIndex == emptyPointIndex[i])
                    {
                        dragEventArgs.EmptyPoint = true;
                        break;
                    }
            }

            RaiseDragStart(dragEventArgs);
            if (dragEventArgs.Cancel)
            {
                ResetDraggingElements("Cancel", true);
                SegmentIndex = -1;
                return;
            }

            var eumerator = Area.Behaviors.OfType<ChartZoomPanBehavior>();
            foreach (var behavior in eumerator)
            {
                behavior.InternalEnablePanning = false;
                behavior.InternalEnableSelectionZooming = false;
            }
        }

        #endregion

        #region Internal Methods

        internal void UpdateSeriesDragValueToolTip(Point pos, Brush brush, double newValue, double baseValue, double offsetX)
        {
            if (!EnableDragTooltip) return;
            double start;
            if (Tooltip == null)
            {
                DragInfo = new ChartDragSeriesInfo();
                Tooltip = new ContentControl();
                Tooltip.Content = DragInfo;
                Tooltip.ContentTemplate = IsActualTransposed ? ChartDictionaries.GenericCommonDictionary["SeriesDragInfoHorizontal"] as DataTemplate :
                    ChartDictionaries.GenericCommonDictionary["SeriesDragInfoVertical"] as DataTemplate;
                SeriesPanel.Children.Add(Tooltip);
            }

            if (IsActualTransposed)
            {
                double offset = 50;
                start = Area.ValueToLogPoint(ActualXAxis, baseValue);
                var end = Area.ValueToLogPoint(ActualXAxis, newValue + baseValue);
                ((ChartDragSeriesInfo)DragInfo).OffsetY = Tooltip.Width = Math.Abs(end - start);
                DragInfo.IsNegative = !(newValue < 0);
                DragInfo.Delta = newValue;
                DragInfo.Brush = brush;
                if (DragInfo.IsNegative)
                    Canvas.SetLeft(Tooltip, offsetX);
                else
                    Canvas.SetLeft(Tooltip, offsetX - Tooltip.Width);
                Canvas.SetTop(Tooltip, start - offset);
            }
            else
            {
                start = Area.ValueToLogPoint(ActualYAxis, baseValue);
                var end = Area.ValueToLogPoint(ActualYAxis, newValue + baseValue);
                ((ChartDragSeriesInfo)DragInfo).OffsetY = Tooltip.Height = Math.Abs(start - end);
                DragInfo.IsNegative = !(newValue > 0);
                DragInfo.Delta = newValue;
                DragInfo.Brush = brush;
                if (DragInfo.IsNegative)
                    Canvas.SetTop(Tooltip, start);
                else
                    Canvas.SetTop(Tooltip, start - Tooltip.Height);
                Canvas.SetLeft(Tooltip, offsetX);
            }
        }

        #endregion

        #region Protected Override Methods

#if NETFX_CORE

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if ((EnableSegmentDragging || EnableSeriesDragging) && PreviewSeries == null && DraggingSegment == null && e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                SeriesPanel.CapturePointer(e.Pointer);
                var mousePos = e.GetCurrentPoint(SeriesPanel).Position;
                var element = e.OriginalSource as FrameworkElement;
                if (element != null && element.Tag is ChartSegment)
                {
                    if (Math.Abs(mousePos.X - mousePos.X) < 20 && Math.Abs(mousePos.Y - mousePos.Y) < 20)
                        ActivateDragging(mousePos, e.OriginalSource);
                }
            }
            else if (EnableSeriesDragging)
            {
                OnChartDragDelta(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            }
            else
                base.OnPointerMoved(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (EnableSeriesDragging)
            {
                SeriesPanel.CapturePointer(e.Pointer);
                OnChartDragStart(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            }
            else
                base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (EnableSeriesDragging)
                OnChartDragEnd(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            else
                base.OnPointerReleased(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (EnableSeriesDragging)
                OnChartDragEntered(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            else
                base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (EnableSeriesDragging)
                OnChartDragExited(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            else
                base.OnPointerExited(e);
        }

#endif

        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            ResetDraggingindicators();
            base.ResetDraggingElements(reason, dragEndEvent);
        }

        protected override void OnChartDragDelta(Point mousePos, object originalSource)
        {
            if (PreviewSeries != null)
            {
                ResetDraggingindicators();
                UpdatePreivewSeriesDragging(mousePos);
            }
            else if (DraggingSegment != null)
            {
                ResetDraggingindicators();
                UpdatePreviewSegmentDragging(mousePos);
            }
        }

        protected override void OnChartDragEntered(Point mousePos, object originalSource)
        {
            var frameworkElement = originalSource as FrameworkElement;
            if ((EnableSegmentDragging || EnableSeriesDragging)
                && (frameworkElement != null && (frameworkElement.Tag is ChartSegment || frameworkElement.DataContext is ChartAdornmentContainer)))
                UpdatePreviewIndicatorPosition(mousePos);
            base.OnChartDragEntered(mousePos, originalSource);
        }

        protected override void OnChartDragExited(Point mousePos, object originalSource)
        {
            if (EnableSegmentDragging || EnableSeriesDragging)
                ResetDraggingindicators();
            base.OnChartDragExited(mousePos, originalSource);
        }

        #endregion

        #region Protected Methods

        protected void UpdateUnderLayingModel(string path, IList<double> updatedDatas)
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
                    yPropertyAccessor.SetValue(enumerator.Current, updatedDatas[i]);
                    i++;
                }
                while (enumerator.MoveNext());
            }
        }

        #endregion

        #region Private Static Methods

        private static void OnEnableDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
                ((XySeriesDraggingBase)d).ResetDraggingElements("OnPropertyChanged", false);
        }

        #endregion

        #region Private Methods

        private void AddSegmentIndicator()
        {
            DraggingPointIndicator = new Ellipse
            {
                Height = 15,
                Width = 15,
                Fill = Segments[SegmentIndex == 0 ? 0 : SegmentIndex - 1].Interior,
            };
            SeriesPanel.Children.Add(DraggingPointIndicator);
        }

        private void UpdatePreviewIndicatorPosition(Point mousePos)
        {
            if (PreviewSeries == null && DraggingSegment == null && EnableSegmentDragging)
            {
                double x, y, stackedValue, positionY;
                FindNearestChartPoint(mousePos, out x, out y, out stackedValue);
                if (double.IsNaN(y))
                    return;

                // WPF_18250 DragDelta Event returns undesired delta value.
                // Here we have reset the prevDraggedValue when dragged segment is changed.
                int previousIndex = SegmentIndex;
                SegmentIndex = (int)(IsIndexed || ActualXValues is IList<string> ? x : ((IList<double>)ActualXValues).IndexOf(x));
                if (previousIndex != SegmentIndex)
                    prevDraggedValue = 0;

                XySegmentEnterEventArgs args = new XySegmentEnterEventArgs
                {
                    XValue = GetActualXValue(SegmentIndex),
                    SegmentIndex = SegmentIndex,
                    CanDrag = true,
                    YValue = YValues[SegmentIndex]
                };
                RaiseDragEnter(args);
                if (!args.CanDrag) return;
                positionY = Area.ValueToLogPoint(ActualYAxis, y);
                var positionX = Area.ValueToLogPoint(ActualXAxis, x);

                if (AdornmentsInfo == null)
                {
                    if (DraggingPointIndicator == null)
                        AddSegmentIndicator();
                    if (this.IsActualTransposed)
                    {
                        Canvas.SetTop(DraggingPointIndicator, positionX - DraggingPointIndicator.Width / 2);
                        Canvas.SetLeft(DraggingPointIndicator, positionY - DraggingPointIndicator.Height / 2);
                    }
                    else
                    {
                        Canvas.SetLeft(DraggingPointIndicator, positionX - DraggingPointIndicator.Width / 2);
                        Canvas.SetTop(DraggingPointIndicator, positionY - DraggingPointIndicator.Height / 2);
                    }

                    DraggingPointIndicator.Tag = Math.Abs(Segments.Count - SegmentIndex) > 0
                        ? Segments[(int)SegmentIndex]
                        : Segments[(int)SegmentIndex - 1];
                    AddAnimationEllipse(
                        ChartDictionaries.GenericSymbolDictionary["AnimationEllipseTemplate"] as ControlTemplate,
                        DraggingPointIndicator.Height,
                        DraggingPointIndicator.Width,
                        positionX, 
                        positionY,
                        null,
                        false);
                }
                else if (AdornmentsInfo != null && AdornmentsInfo.Symbol != ChartSymbol.Custom)
                {
                    AddAnimationEllipse(
                        ChartDictionaries.GenericSymbolDictionary["Animation" + AdornmentsInfo.Symbol.ToString()+ "Template"] as ControlTemplate,
                        AdornmentsInfo.SymbolHeight,
                        AdornmentsInfo.SymbolWidth,
                        positionX,
                        positionY,
                        null,
                        true);
                }
            }
        }

        private void ResetDraggingindicators()
        {
            if (DraggingPointIndicator != null)
            {
                SeriesPanel.Children.Remove(DraggingPointIndicator);
                DraggingPointIndicator = null;
            }

            if (AnimationElement != null && SeriesPanel.Children.Contains(AnimationElement))
            {
                SeriesPanel.Children.Remove(AnimationElement);
                AnimationElement = null;
            }

            DraggingPointIndicator = null;
        }

        #endregion

        #endregion
    }

    public partial class XySeriesDragEventArgs : DragDelta
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets base y value
        /// </summary>
        public object BaseXValue { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a ChartDragPoint that includes a offsety value. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.ChartDragPointinfo" />
    public partial class ChartDragSeriesInfo : ChartDragPointinfo
    {
        #region Fields

        #region Private Fields

        private double offsetY;


        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets value of y offset
        /// </summary>
        public double OffsetY
        {
            get
            {
                return offsetY;
            }

            set
            {
                offsetY = value;
                OnPropertyChanged("OffsetY");
            }
        }

        #endregion

        #endregion
    }

    public partial class XySegmentEnterEventArgs : EventArgs
    {
        #region Properites

        #region Public Properites

        /// <summary>
        /// Gets or sets x value
        /// </summary>
        public object XValue { get; set; }

        /// <summary>
        /// Gets or sets y value
        /// </summary>
        public object YValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to Enable or disable dragging
        /// </summary>
        public bool CanDrag { get; set; }

        /// <summary>
        /// Gets or sets the segment index
        /// </summary>
        public int SegmentIndex { get; set; }

        #endregion

        #endregion
    }
}
