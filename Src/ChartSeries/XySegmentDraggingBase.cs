using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Text;
using Windows.UI;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// XySegmentDraggingBase is abstract class which is used to allow, drag a segment in a chart series.
    /// </summary>
    /// <seealso cref="EnableSegmentDragging"/>
    public abstract partial class XySegmentDraggingBase : XyDataSeries
    {
        #region Dependency Property Registration

        // Using a DependencyProperty as the backing store for DragTooltipStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragTooltipStyleProperty =
            DependencyProperty.Register("DragTooltipStyle", typeof(ChartDragTooltipStyle), typeof(XySegmentDraggingBase), new PropertyMetadata(null));
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableDragTooltip"/> property.       .
        /// </summary>
        public static readonly DependencyProperty EnableDragTooltipProperty =
            DependencyProperty.Register(
                "EnableDragTooltip", 
                typeof(bool), 
                typeof(XySegmentDraggingBase),
                new PropertyMetadata(true));
        
        /// <summary>
        /// The DependencyProperty for <see cref="DragTooltipTemplate"/> property.       .
        /// </summary>
        public static readonly DependencyProperty DragTooltipTemplateProperty =
            DependencyProperty.Register(
                "DragTooltipTemplate",
                typeof(DataTemplate),
                typeof(XySegmentDraggingBase),
                new PropertyMetadata(null));
        
        /// <summary>
        /// The DependencyProperty for <see cref="RoundToDecimal"/> property.       .
        /// </summary>
        public static readonly DependencyProperty RoundToDecimalProperty =
            DependencyProperty.Register(
                "RoundToDecimal",
                typeof(int),
                typeof(XySegmentDraggingBase),
                new PropertyMetadata(0));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SnapToPoint"/> property.       .
        /// </summary>
        public static readonly DependencyProperty SnapToPointProperty =
            DependencyProperty.Register(
                "SnapToPoint",
                typeof(SnapToPoint),
                typeof(XySegmentDraggingBase),
                new PropertyMetadata(SnapToPoint.None));
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableSegmentDragging"/> property.       .
        /// </summary>
        public static readonly DependencyProperty EnableSegmentDraggingProperty =
            DependencyProperty.Register(
                "EnableSegmentDragging",
                typeof(bool), 
                typeof(XySegmentDraggingBase),
                new PropertyMetadata(false));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="UpdateSource"/> property.       .
        /// </summary>
        public static readonly DependencyProperty UpdateSourceProperty =
            DependencyProperty.Register(
                "UpdateSource",
                typeof(bool), 
                typeof(XySegmentDraggingBase),
                new PropertyMetadata(false));

        /// <summary>
        /// The DependencyProperty for <see cref="DragCancelKeyModifiers"/> property.
        /// </summary>
        public static readonly DependencyProperty DragCancelKeyModifiersProperty =
            DependencyProperty.Register(
                "DragCancelKeyModifiers", 
                typeof(VirtualKeyModifiers), 
                typeof(XySegmentDraggingBase),
                new PropertyMetadata(VirtualKeyModifiers.None));


#endregion

        #region Fields

        #region Internal Fields

        internal double prevDraggedXValue;

        internal double prevDraggedValue;

        internal double delta;

        internal double DeltaX = 0;
        
        #endregion
        
        #region Private Fields

        private double tooltipX;

        private double tooltipY;

        private Storyboard ellipseAnimation;

        DataTemplate oppRightTootip, normalTooltip, oppLeftTooltip;

        #endregion

        #endregion
        
        #region Events

        /// <summary>
        /// Occurs when [segment enter].
        /// </summary>
        public event EventHandler<XySegmentEnterEventArgs> SegmentEnter;

        /// <summary>
        /// Occurs when [drag start].
        /// </summary>
        public event EventHandler<ChartDragStartEventArgs> DragStart;

        /// <summary>
        /// Occurs when [drag delta].
        /// </summary>
        public event EventHandler<DragDelta> DragDelta;

        /// <summary>
        /// Occurs when [drag end].
        /// </summary>
        public event EventHandler<ChartDragEndEventArgs> DragEnd;

        /// <summary>
        /// Occurs when [preview drag end].
        /// </summary>
        public event EventHandler<XyPreviewEndEventArgs> PreviewDragEnd;

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets dragging tooltip style.
        /// </summary>
        public ChartDragTooltipStyle DragTooltipStyle
        {
            get { return (ChartDragTooltipStyle)GetValue(DragTooltipStyleProperty); }
            set { SetValue(DragTooltipStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dragging tooltip is enabled or not.
        /// </summary>
        public bool EnableDragTooltip
        {
            get { return (bool)GetValue(EnableDragTooltipProperty); }
            set { SetValue(EnableDragTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for dragging tooltip/>.
        /// </summary>
        /// <value>
        /// This accepts a DataTemplate.
        /// </value>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate DragTooltipTemplate
        {
            get { return (DataTemplate)GetValue(DragTooltipTemplateProperty); }
            set { SetValue(DragTooltipTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a property used to round the decimal value.
        /// </summary>
        public int RoundToDecimal
        {
            get { return (int)GetValue(RoundToDecimalProperty); }
            set { SetValue(RoundToDecimalProperty, value); }
        }

        /// <summary>
        /// Gets or sets snap point.
        /// </summary>
        public SnapToPoint SnapToPoint
        {
            get { return (SnapToPoint)GetValue(SnapToPointProperty); }
            set { SetValue(SnapToPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether segment dragging is enabled or not.
        /// </summary>
        public bool EnableSegmentDragging
        {
            get { return (bool)GetValue(EnableSegmentDraggingProperty); }
            set { SetValue(EnableSegmentDraggingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update the dragging values in source
        /// </summary>
        public bool UpdateSource
        {
            get { return (bool)GetValue(UpdateSourceProperty); }
            set { SetValue(UpdateSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the dragging.
        /// </summary>
        public VirtualKeyModifiers DragCancelKeyModifiers
        {
            get { return (VirtualKeyModifiers)GetValue(DragCancelKeyModifiersProperty); }
            set { SetValue(DragCancelKeyModifiersProperty, value); }
        }
        
        #endregion

        #region Internal Properties

        internal FrameworkElement AnimationElement { get; set; }
        
        #endregion

        #region Protected Properties

        protected int SegmentIndex { get; set; }

        protected ContentControl DragSpliter { get; set; }

        protected double DraggedXValue { get; set; }

        protected double DraggedValue { get; set; }

        protected ContentControl Tooltip { get; set; }

        protected ChartDragPointinfo DragInfo { get; set; }

        protected Storyboard EllipseAnimation
        {
            get { return ellipseAnimation; }
            set { ellipseAnimation = value; }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Static Methods

        internal static ChartSegment GetDraggingSegment(object element)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;
            if (frameworkElement == null) return null;

            return frameworkElement.Tag as ChartSegment
                ?? (!(frameworkElement.DataContext is ChartAdornment) ? frameworkElement.DataContext as ChartSegment : null);
        }

        #endregion

        #region Internal Virtual Methods

        /// <summary>
        /// Updates the drag spliter.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="position">The position.</param>
        internal virtual void UpdateDragSpliter(FrameworkElement rect, ChartSegment indexSegment, string position)
        {
            int index = Segments.IndexOf(indexSegment);
            XySegmentEnterEventArgs args = new XySegmentEnterEventArgs
            {
                XValue = GetActualXValue(index),
                SegmentIndex = index,
                CanDrag = true,
                YValue = YValues[index]
            };
            RaiseDragEnter(args);
            if (!args.CanDrag) return;

            if (DragSpliter == null)
            {
                DragSpliter = new ContentControl();
                SeriesPanel.Children.Add(DragSpliter);
                if (position == "Left" || position == "Right")
                    DragSpliter.Template = ChartDictionaries.GenericCommonDictionary["DragSpliterLeft"] as ControlTemplate;
                else
                    DragSpliter.Template = ChartDictionaries.GenericCommonDictionary["DragSpliterTop"] as ControlTemplate;
            }

            double canvasLeft = 0d, canvasTop = 0d, spliterHeight = 0d, spliterWidth = 0d, margin;

            if (position == "Top")
            {
                var top = Canvas.GetTop(rect);
                var width = rect.Width;
                margin = width / 3;
                spliterWidth = width - margin * 2;
                DragSpliter.Margin = new Thickness(margin, 0, margin, 0);
                canvasLeft = Canvas.GetLeft(rect);
                canvasTop = top + 7;
                spliterHeight = spliterWidth / 5;
            }
            else if (position == "Bottom")
            {
                var bottom = Canvas.GetTop(rect) + rect.Height;
                var width = rect.Width;
                margin = width / 3;
                spliterWidth = width - margin * 2;
                DragSpliter.Margin = new Thickness(margin, 0, margin, 0);

                canvasLeft = Canvas.GetLeft(rect);
                canvasTop = bottom - 7;

                spliterHeight = spliterWidth / 5;
            }
            else if (position == "Right")
            {
                var left = Canvas.GetLeft(rect) + rect.Width;
                var height = rect.Height;
                margin = height / 3;
                spliterHeight = height - margin * 2;
                DragSpliter.Margin = new Thickness(0, margin, 0, margin);
                canvasTop = Canvas.GetTop(rect);
                canvasLeft = left - 20;
                spliterWidth = spliterHeight / 5;
            }
            else if (position == "Left")
            {
                var left = Canvas.GetLeft(rect);
                var height = rect.Height;
                margin = height / 3;
                spliterHeight = height - margin * 2;
                DragSpliter.Margin = new Thickness(0, margin, 0, margin);
                canvasTop = Canvas.GetTop(rect);
                canvasLeft = left + 10;
                spliterWidth = spliterHeight / 5;
            }

            DragSpliter.SetValue(Canvas.LeftProperty, canvasLeft);
            DragSpliter.SetValue(Canvas.TopProperty, canvasTop);
            DragSpliter.Height = spliterHeight;
            DragSpliter.Width = spliterWidth;
        }



        /// <summary>
        /// Activates the dragging.
        /// </summary>
        /// <param name="mousePos">The mouse position.</param>
        /// <param name="element">The element.</param>
        internal virtual void ActivateDragging(Point mousePos, object element)
        {
#if NETFX_CORE
            Focus(FocusState.Keyboard);
#endif
            delta = 0;
            var segment = GetDraggingSegment(element);

            if (segment == null) return;

            // WPF_18250 DragDelta Event returns undesired delta value
            // Here we have reset the prevDraggedValue when dragged segment is changed.
            int previousIndex = SegmentIndex;

            if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                SegmentIndex = ActualData.IndexOf(segment.Item);
            else
                SegmentIndex = Segments.IndexOf(segment);

            if (previousIndex != SegmentIndex)
                prevDraggedValue = 0;

            KeyDown += CoreWindow_KeyDown;

            ChartDragStartEventArgs dragEventArgs;

            dragEventArgs = new ChartDragStartEventArgs { BaseXValue = GetActualXValue(SegmentIndex) };

            if (EmptyPointIndexes != null)
            {
                var emptyPointIndex = EmptyPointIndexes[0];
                foreach (var index in emptyPointIndex)
                    if (SegmentIndex == index)
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
            }

            UnHoldPanning(false);
        }

        #endregion

        #region Internal Methods

        internal void CoreWindow_KeyDown(object sender, KeyRoutedEventArgs e)
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
        
        internal void UnHoldPanning(bool value)
        {
            var eumerator = Area.Behaviors.OfType<ChartZoomPanBehavior>();
            foreach (var behavior in eumerator)
            {
                behavior.InternalEnablePanning = value;
                behavior.InternalEnableSelectionZooming = value;
            }
        }

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

        internal void UpdateSegmentDragValueToolTip(Point pos, ChartSegment segment, double newXValue, double newYValue, double offsetX, double offsetY)
        {
            if (!EnableDragTooltip) return;
            var scatterSeries = this as ScatterSeries;
            if (Tooltip == null)
            {
                DragInfo = new ChartDragSegmentInfo();
                Tooltip = new ContentControl { Content = DragInfo };
                SeriesPanel.Children.Add(Tooltip);

                if (DragTooltipStyle != null)
                {
                    DragInfo.FontFamily = DragTooltipStyle.FontFamily;
                    DragInfo.FontSize = DragTooltipStyle.FontSize;
                    DragInfo.FontStyle = DragTooltipStyle.FontStyle;
                    DragInfo.Foreground = DragTooltipStyle.Foreground;
                }
                else
                {
                    // Assigning Default tooltip style Values.
                    DragInfo.FontFamily = TextBlock.FontFamilyProperty.GetMetadata(typeof(TextBlock)).DefaultValue as FontFamily;
                    DragInfo.FontSize = 20;
                    DragInfo.FontStyle = FontStyle.Normal;
                    DragInfo.Foreground = new SolidColorBrush(Colors.White);
                }

                if (DragTooltipTemplate != null)
                {
                    normalTooltip = oppLeftTooltip = oppRightTootip = DragTooltipTemplate;
                }
                else
                {
                    string templateString = "";

                    if (scatterSeries != null && scatterSeries.DragDirection == DragType.XY)
                    {
                        templateString = "Xy";

                        DragInfo.PrefixLabelTemplate = ActualYAxis.PrefixLabelTemplate;
                        DragInfo.PostfixLabelTemplate = ActualYAxis.PostfixLabelTemplate;

                        DragInfo.PrefixLabelTemplateX = ActualXAxis.PrefixLabelTemplate;
                        DragInfo.PostfixLabelTemplateX = ActualXAxis.PostfixLabelTemplate;
                    }
                    else if (scatterSeries != null && scatterSeries.DragDirection == DragType.X)
                    {
                        DragInfo.PrefixLabelTemplate = ActualXAxis.PrefixLabelTemplate;
                        DragInfo.PostfixLabelTemplate = ActualXAxis.PostfixLabelTemplate;
                    }
                    else
                    {
                        DragInfo.PrefixLabelTemplate = ActualYAxis.PrefixLabelTemplate;
                        DragInfo.PostfixLabelTemplate = ActualYAxis.PostfixLabelTemplate;
                    }

                    normalTooltip = ChartDictionaries.GenericCommonDictionary[templateString + "SegmentDragInfo"] as DataTemplate;
                    oppLeftTooltip =
                        ChartDictionaries.GenericCommonDictionary[templateString + "SegmentDragInfoOppLeft"] as DataTemplate;
                    oppRightTootip =
                        ChartDictionaries.GenericCommonDictionary[templateString + "SegmentDragInfoOppRight"] as DataTemplate;

                    if (IsActualTransposed)
                        RightAlignTooltip(pos.X, pos.Y, offsetX);
                    else
                        TopAlignTooltip(pos.X, pos.Y, offsetY);
                }
            }

            DragInfo.Brush = DragTooltipStyle != null && DragTooltipStyle.Background != null ? DragTooltipStyle.Background : segment.Interior;
            DragInfo.ScreenCoordinates = pos;

            var dragSegmentInfo = DragInfo as ChartDragSegmentInfo;

            if (scatterSeries != null && scatterSeries.DragDirection == DragType.XY)
            {
                dragSegmentInfo.NewXValue = this.ActualXAxis.GetLabelContent(newXValue);
                dragSegmentInfo.NewValue = newYValue;
            }
            else if (scatterSeries != null && scatterSeries.DragDirection == DragType.X)
                dragSegmentInfo.NewValue = this.ActualXAxis.GetLabelContent(newXValue);
            else
                dragSegmentInfo.NewValue = newYValue;

            DragInfo.Segment = segment;

            // Initializing a template is used to identify for the position correctly.                     
            double seriesPanelX = 0;
            double seriesPanelY = 0;
            double totalWidth = ActualWidth;
            double totalHeight = ActualHeight;
            bool isColumnBarSeries = this is ColumnSeries || this is BarSeries;

            // Arranging the tooltip according to the boundaries.
            if (IsActualTransposed)
            {
                if (pos.X + offsetX + Tooltip.DesiredSize.Width >= totalWidth)
                {
                    if (isColumnBarSeries)
                    {
                        // Aligning the position at the top.
                        tooltipY = pos.Y - offsetY - Tooltip.DesiredSize.Height;
                        Canvas.SetTop(Tooltip, pos.Y + offsetY);
                    }
                    else
                        LeftAlignTooltip(pos.X, pos.Y, offsetX);
                }
                else
                    RightAlignTooltip(pos.X, pos.Y, offsetX);
            }
            else
            {
                if (pos.X - Tooltip.DesiredSize.Width / 2 <= seriesPanelX)
                    RightAlignTooltip(pos.X, pos.Y, offsetX);
                else if (pos.X + Tooltip.DesiredSize.Width / 2 >= totalWidth
                        || pos.Y - offsetY - Tooltip.DesiredSize.Height <= seriesPanelY
                        || (isColumnBarSeries && pos.Y >= totalHeight))
                    LeftAlignTooltip(pos.X, pos.Y, offsetX);
                else
                    TopAlignTooltip(pos.X, pos.Y, offsetY);
            }

            // Stopping the tooltip at the boundaries
            if (tooltipX <= seriesPanelX)
                Canvas.SetLeft(Tooltip, seriesPanelX);
            else if (tooltipX + Tooltip.DesiredSize.Width >= totalWidth)
                Canvas.SetLeft(Tooltip, totalWidth - Tooltip.DesiredSize.Width);
            else
                Canvas.SetLeft(Tooltip, tooltipX);

            if (tooltipY <= seriesPanelY)
                Canvas.SetTop(Tooltip, seriesPanelY);
            else if (tooltipY + Tooltip.DesiredSize.Height >= totalHeight)
                Canvas.SetTop(Tooltip, totalHeight - Tooltip.DesiredSize.Height);
            else
                Canvas.SetTop(Tooltip, tooltipY);
        }


        // WPF_18250 DragDelta Event returns undesired delta value.
        // Calculates and returns the delta value.
        internal double GetActualDelta()
        {
            delta += (prevDraggedValue != 0 ? DraggedValue - prevDraggedValue : DraggedValue - YValues[SegmentIndex]);
            return delta;
        }

        internal object GetActualXDelta(double prevDraggedXValue, double draggedXValue, ref double delta)
        {
            delta += (prevDraggedXValue != 0 ? draggedXValue - prevDraggedXValue : draggedXValue - ((IList<double>)ActualXValues)[SegmentIndex]);
            if (XAxisValueType == ChartValueType.DateTime || XAxisValueType == ChartValueType.TimeSpan)
                return TimeSpan.FromMilliseconds(delta);
            else
                return delta;
        }

        internal object GetDraggedActualXValue(double deltaX)
        {
            object objectDeltaX;
            switch (XAxisValueType)
            {
                case ChartValueType.DateTime:
                    objectDeltaX = deltaX.FromOADate();
                    break;

                case ChartValueType.TimeSpan:
                    objectDeltaX = TimeSpan.FromMilliseconds(deltaX);
                    break;

                default:
                    objectDeltaX = deltaX;
                    break;
            }

            return objectDeltaX;
        }

        internal void EllipseIdealAnimation(UIElement ellipse)
        {
            EllipseAnimation = new Storyboard();
#if NETFX_CORE
            var ellipseAnimation = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(ellipseAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
#else
            var ellipseAnimation = new DoubleAnimationUsingKeyFrames() { RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(ellipseAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#endif
            Storyboard.SetTarget(ellipseAnimation, ellipse);

            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 });
            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 3.6, EasingFunction = new CircleEase() });
            EllipseAnimation.Children.Add(ellipseAnimation);
#if NETFX_CORE
            ellipseAnimation = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(ellipseAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
#else
            ellipseAnimation = new DoubleAnimationUsingKeyFrames() { RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(ellipseAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#endif
            Storyboard.SetTarget(ellipseAnimation, ellipse);

            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 });
            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 3.6, EasingFunction = new CircleEase() });
            EllipseAnimation.Children.Add(ellipseAnimation);
#if NETFX_CORE
            ellipseAnimation = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(ellipseAnimation, "(UIElement.Opacity)");
#else
            ellipseAnimation = new DoubleAnimationUsingKeyFrames() { RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(ellipseAnimation, new PropertyPath("(UIElement.Opacity)"));
#endif
            Storyboard.SetTarget(ellipseAnimation, ellipse);

            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 });
            ellipseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0, EasingFunction = new CircleEase() });
            EllipseAnimation.Children.Add(ellipseAnimation);
        }

        internal void AddAnimationEllipse(ControlTemplate template, double height, double width, double left, double top, object bindingObject, bool isAdornment)
        {
            if (AnimationElement == null)
            {
                AnimationElement = new ContentControl
                {
                    Background = this.Segments[SegmentIndex == 0 ? 0 : SegmentIndex - 1].Interior,
                    RenderTransform = new ScaleTransform(),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    DataContext = this,
                    Template = template,
                };

                EllipseIdealAnimation(AnimationElement);
                this.SeriesPanel.Children.Add(AnimationElement);
                AnimationElement.Height = height - height / 2;
                AnimationElement.Width = width - width / 2;

                if (this is ScatterSeries)
                {
                    AnimationElement.IsHitTestVisible = false;
                    Binding colorBinding = null;

                    if (isAdornment)
                    {
                        colorBinding = new Binding()
                        {
                            Source = (this as AdornmentSeries).AdornmentsInfo,
                            Path = new PropertyPath("SymbolInterior")
                        };
                    }
                    else
                    {
                        // The color has to be bind for the mouse move selection.
                        colorBinding = new Binding()
                        {
                            Source = bindingObject,
                            Path = new PropertyPath("Interior")
                        };
                    }

                    AnimationElement.SetBinding(ContentControl.BackgroundProperty, colorBinding);
                }

                ellipseAnimation.Begin();
            }

            if (this.IsActualTransposed)
            {
                Canvas.SetTop(AnimationElement, left - AnimationElement.Width / 2);
                Canvas.SetLeft(AnimationElement, top - AnimationElement.Height / 2);
            }
            else
            {
                Canvas.SetLeft(AnimationElement, left - AnimationElement.Width / 2);
                Canvas.SetTop(AnimationElement, top - AnimationElement.Height / 2);
            }
        }

        internal void AnimateSegmentTemplate(double positionX, double positionY, ChartSegment segment)
        {
            var scatterSeries = this as ScatterSeries;
            if (scatterSeries == null) return;

            var canvas = scatterSeries.CustomTemplate.LoadContent() as Canvas;
            if (canvas != null)
            {
                if (canvas.Children.Count < 1) return;
                AnimationElement = canvas;
                SeriesPanel.Children.Add(AnimationElement);

                for (int i = 0; i < canvas.Children.Count; i++)
                {
                    var innerElement = canvas.Children[i] as FrameworkElement;

                    if (innerElement != null)
                    {
                        innerElement.DataContext = segment;
                        EllipseIdealAnimation(innerElement);
                        innerElement.RenderTransform = new ScaleTransform();
                        innerElement.RenderTransformOrigin = new Point(0.5, 0.5);
                        innerElement.Height -= innerElement.ActualHeight / 2;
                        innerElement.Width -= innerElement.ActualWidth / 2;
                        EllipseAnimation.Begin();

                        if (IsActualTransposed)
                        {
                            Canvas.SetLeft(innerElement, positionY - innerElement.Width / 2);
                            Canvas.SetTop(innerElement, positionX - innerElement.Height / 2);
                        }
                        else
                        {
                            Canvas.SetLeft(innerElement, positionX - innerElement.Width / 2);
                            Canvas.SetTop(innerElement, positionY - innerElement.Height / 2);
                        }
                    }
                }
            }
        }

        internal void AnimateAdornmentSymbolTemplate(double positionX, double positionY)
        {
            // When layout is updated only the child elements are get.
            if (AdornmentsInfo.SymbolTemplate == null) return;
            AnimationElement = AdornmentsInfo.SymbolTemplate.LoadContent() as Shape;
            if (AnimationElement == null) return;

            SeriesPanel.Children.Add(AnimationElement);
            AnimationElement.IsHitTestVisible = false;

            // The color has to be bind for the mouse move selection for the newly created segment.
            var colorBinding = new Binding()
            {
                Source = Adornments[SegmentIndex],
                Path = new PropertyPath("Interior")
            };
            BindingOperations.SetBinding((AnimationElement as Shape), Shape.FillProperty, colorBinding);

            AnimationElement.DataContext = Adornments[SegmentIndex];
            if (AnimationElement == null) return;

            EllipseIdealAnimation(AnimationElement);
            AnimationElement.RenderTransform = new ScaleTransform();
            AnimationElement.RenderTransformOrigin = new Point(0.5, 0.5);
            AnimationElement.Height -= AnimationElement.ActualHeight / 2;
            AnimationElement.Width -= AnimationElement.ActualWidth / 2;
            EllipseAnimation.Begin();

            if (IsActualTransposed)
            {
                Canvas.SetLeft(AnimationElement, positionY - AnimationElement.Width / 2);
                Canvas.SetTop(AnimationElement, positionX - AnimationElement.Height / 2);
            }
            else
            {
                Canvas.SetLeft(AnimationElement, positionX - AnimationElement.Width / 2);
                Canvas.SetTop(AnimationElement, positionY - AnimationElement.Height / 2);
            }
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Resets the dragging elements.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="dragEndEvent">if set to <c>true</c> [drag end event].</param>
        protected virtual void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            KeyDown -= CoreWindow_KeyDown;
            UnHoldPanning(true);
            if (dragEndEvent)
                if (this is ScatterSeries)
                    RaiseDragEnd(new ChartXyDragEndEventArgs());
                else
                    RaiseDragEnd(new ChartDragEndEventArgs());
            ResetSegmentDragTooltipInfo();
        }

        /// <summary>
        /// Resets the drag spliter.
        /// </summary>
        protected virtual void ResetDragSpliter()
        {
            if (DragSpliter != null)
            {
                SeriesPanel.Children.Remove(DragSpliter);
                DragSpliter = null;
            }
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
            {
                OnChartDragDelta(e.GetCurrentPoint(SeriesPanel).Position, e.OriginalSource);
            }

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
        /// Raises the drag start.
        /// </summary>
        /// <param name="args">The <see cref="ChartDragStartEventArgs"/> instance containing the event data.</param>
        protected void RaiseDragStart(ChartDragStartEventArgs args)
        {
            if (DragStart != null)
                DragStart(this, args);
        }

        /// <summary>
        /// Raises the drag end.
        /// </summary>
        /// <param name="args">The <see cref="ChartDragEndEventArgs"/> instance containing the event data.</param>
        protected void RaiseDragEnd(ChartDragEndEventArgs args)
        {
            if (DragEnd != null)
                DragEnd(this, args);
        }

        /// <summary>
        /// Raises the drag delta.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void RaiseDragDelta(DragDelta args)
        {
            if (DragDelta != null)
                DragDelta(this, args);
        }

        /// <summary>
        /// Raises the drag enter.
        /// </summary>
        /// <param name="args">The <see cref="XySegmentEnterEventArgs"/> instance containing the event data.</param>
        protected void RaiseDragEnter(XySegmentEnterEventArgs args)
        {
            if (SegmentEnter != null)
                SegmentEnter(this, args);
        }

        /// <summary>
        /// Raises the preview end.
        /// </summary>
        /// <param name="args">The <see cref="XyPreviewEndEventArgs"/> instance containing the event data.</param>
        protected void RaisePreviewEnd(XyPreviewEndEventArgs args)
        {
            if (PreviewDragEnd != null)
                PreviewDragEnd(this, args);
        }

        /// <summary>
        /// Updates the under laying model.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="index">The index.</param>
        /// <param name="updatedData">The updated data.</param>
        protected void UpdateUnderLayingModel(string path, int index, object updatedData)
        {
            var enumerator = ItemsSource.GetEnumerator();

            if (enumerator.MoveNext())
            {
                int i = 0;
                do
                {
                    if (i == index)
                    {
                        SetPropertyValue(enumerator.Current, path.Split('.'), updatedData);
                        break;
                    }

                    i++;
                }
                while (enumerator.MoveNext());
            }
        }

        #endregion

        #region Private Methods

        private void ResetSegmentDragTooltipInfo()
        {
            if (Tooltip != null)
            {
                SeriesPanel.Children.Remove(Tooltip);
                Tooltip = null;
                DragInfo = null;
            }
        }


        private void TopAlignTooltip(double x, double y, double offsetY)
        {
            Tooltip.ContentTemplate = normalTooltip;
            UpdateTooltip();
            tooltipX = x - Tooltip.DesiredSize.Width / 2;
            tooltipY = y - Tooltip.DesiredSize.Height - offsetY;
        }

        private void LeftAlignTooltip(double x, double y, double offsetX)
        {
            Tooltip.ContentTemplate = oppLeftTooltip;
            UpdateTooltip();
            tooltipX = x - Tooltip.DesiredSize.Width - offsetX;
            tooltipY = y - Tooltip.DesiredSize.Height / 2;
        }

        private void RightAlignTooltip(double x, double y, double offsetX)
        {
            Tooltip.ContentTemplate = oppRightTootip;
            UpdateTooltip();
            tooltipX = x + offsetX;
            tooltipY = y - Tooltip.DesiredSize.Height / 2;
        }

        private void UpdateTooltip()
        {
            // In UWP the measure measures the width correctly but in wpf the measure miscalculates the width and height.
            Tooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        #endregion

        #endregion   
    }

    /// <summary>
    /// Represents EventArgs that a cancel option for abort the operation. 
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public partial class DragDelta : EventArgs
    {
        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets or sets double value to delta. 
        /// </summary>
        public double Delta { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel. 
        /// </summary>
        public bool Cancel { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a events args that contain event data, and provides a
    //     value to use for events that do not include event data.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public partial class ChartDragStartEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether empty point. 
        /// </summary>
        public bool EmptyPoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel. 
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets x value. 
        /// </summary>
        public object BaseXValue { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a events args that contain event data, and provides a
    //     value to use for events that do not include event data.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public partial class ChartDragEndEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets y value. 
        /// </summary>
        public double BaseYValue { get; set; }

        /// <summary>
        /// Gets or sets new value. 
        /// </summary>
        public double NewYValue { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a ChartDragPoint that includes a current and existing value. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.ChartDragPointinfo" />
    public partial class ChartDragSegmentInfo : ChartDragPointinfo
    {
        #region Fields

        #region Private Fields

        private object newValue;

        private object newXValue;

        private double baseValue;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets new value. 
        /// </summary>
        public object NewValue
        {
            get
            {
                return newValue;
            }

            set
            {
                newValue = value;
                OnPropertyChanged("NewValue");
            }
        }
        
        /// <summary>
        /// Gets or sets new x value. 
        /// </summary>
        public object NewXValue
        {
            get
            {
                return newXValue;
            }

            set
            {
                newXValue = value;
                OnPropertyChanged("NewXValue");
            }
        }
        
        /// <summary>
        /// Gets or sets base value. 
        /// </summary>
        public double BaseValue
        {
            get
            {
                return baseValue;
            }

            set
            {
                baseValue = value;
                OnPropertyChanged("BaseValue");
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a data point that used to display the drag data point to user. 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public partial class ChartDragPointinfo : INotifyPropertyChanged
    {
        #region Fields

        #region Private Fields

        private FontFamily fontFamily;

        private double fontSize;

        private FontStyle fontStyle;

        private Brush foreground;

        private Brush brush;

        private ChartSegment segment;

        private Point screenCoordinates;

        private double delta;

        private bool isNegative;

        #endregion

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the font family for dragging tooltip text.
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                return fontFamily;
            }

            set
            {
                if (fontFamily != value)
                {
                    fontFamily = value;
                    OnPropertyChanged("FontFamily");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the font size for dragging tooltip text.
        /// </summary>
        public double FontSize
        {
            get
            {
                return fontSize;
            }

            set
            {
                if (fontSize != value)
                {
                    fontSize = value;
                    OnPropertyChanged("FontSize");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the font style for dragging tooltip text.
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }

            set
            {
                if (fontStyle != value)
                {
                    fontStyle = value;
                    OnPropertyChanged("FontStyle");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the Brush for dragging tooltip text.
        /// </summary>
        public Brush Foreground
        {
            get
            {
                return foreground;
            }

            set
            {
                if (foreground != value)
                {
                    foreground = value;
                    OnPropertyChanged("Foreground");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets brush. 
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Brush
        {
            get
            {
                return brush;
            }

            set
            {
                if (brush != value)
                {
                    brush = value;
                    OnPropertyChanged("Brush");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets chart segment. 
        /// </summary>
        public ChartSegment Segment
        {
            get
            {
                return segment;
            }

            set
            {
                segment = value;
                OnPropertyChanged("Segment");
            }
        }

        /// <summary>
        /// Gets or sets the double value to delta. 
        /// </summary>
        public double Delta
        {
            get
            {
                return delta;
            }

            set
            {
                delta = value;
                OnPropertyChanged("Delta");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Enable or disable IsNegative. 
        /// </summary>
        public bool IsNegative
        {
            get
            {
                return isNegative;
            }

            set
            {
                isNegative = value;
                OnPropertyChanged("IsNegative");
            }
        }

        /// <summary>
        /// Gets or sets the points to screen co-ordinates. 
        /// </summary>
        public Point ScreenCoordinates
        {
            get
            {
                return screenCoordinates;
            }

            set
            {
                screenCoordinates = value;
                OnPropertyChanged("ScreenCoordinates");
            }
        }

        /// <summary>
        /// Gets or sets the template for prefix label. 
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate PrefixLabelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for postfix label. 
        /// </summary>
        public DataTemplate PostfixLabelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for prefix x label. 
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate PrefixLabelTemplateX { get; set; }

        /// <summary>
        /// Gets or sets the template for postfix x label. 
        /// </summary>
        public DataTemplate PostfixLabelTemplateX { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Protected Virtual Methods

        /// <summary>
        /// Called when Property changed 
        /// </summary>
        /// <param name="name"></param>
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #endregion
    }

    public partial class XySegmentDragEventArgs : DragDelta
    {
        #region Properites

        #region Public Properties

        /// <summary>
        /// Gets or sets base y value. 
        /// </summary>
        public double BaseYValue { get; set; }

        /// <summary>
        /// Gets or sets new y value. 
        /// </summary>
        public double NewYValue { get; set; }

        /// <summary>
        /// Gets or sets chart segment. 
        /// </summary>
        public ChartSegment Segment { get; set; }

        #endregion

        #endregion
    }

    public partial class XyPreviewEndEventArgs : CancelEventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets base y value. 
        /// </summary>
        public double BaseYValue { get; set; }

        /// <summary>
        /// Gets or sets new y value. 
        /// </summary>
        public double NewYValue { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines the ChartXyDragStart event arguments.
    /// </summary>
    public partial class ChartXyDragStartEventArgs : ChartDragStartEventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets base y value. 
        /// </summary>
        public double BaseYValue { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines the XyDeltaDrag event arguments.
    /// </summary>
    public partial class XyDeltaDragEventArgs : XySegmentDragEventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets base x value. 
        /// </summary>
        public object BaseXValue { get; set; }

        /// <summary>
        /// Gets or sets new x value. 
        /// </summary>
        public object NewXValue { get; set; }

        /// <summary>
        /// Gets or sets x value to delta. 
        /// </summary>
        public object DeltaX { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines the ChartXyDragEnd event arguments.
    /// </summary>
    public partial class ChartXyDragEndEventArgs : ChartDragEndEventArgs
    {
        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets base x value. 
        /// </summary>
        public object BaseXValue { get; set; }

        /// <summary>
        /// Gets or sets new x value. 
        /// </summary>
        public object NewXValue { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents the class for configuring dragging tooltip Style.
    /// </summary>    
    public partial class ChartDragTooltipStyle : DependencyObject
    {
        #region Dependency Property Registration
        
        // Using a DependencyProperty as the backing store for FontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilyProperty =
              DependencyProperty.Register(
                  "FontFamily", 
                  typeof(FontFamily), 
                  typeof(ChartDragTooltipStyle),
                  new PropertyMetadata(TextBlock.FontFamilyProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(ChartDragTooltipStyle), new PropertyMetadata(20d));
        
        // Using a DependencyProperty as the backing store for FontStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontStyleProperty =
          DependencyProperty.Register(
              "FontStyle", 
              typeof(FontStyle), 
              typeof(ChartDragTooltipStyle),
              new PropertyMetadata(TextBlock.FontStyleProperty.GetMetadata(typeof(TextBlock)).DefaultValue));
        
        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(ChartDragTooltipStyle), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        
        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ChartDragTooltipStyle), new PropertyMetadata(null));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the font family for dragging tooltip text.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font size for dragging tooltip text.
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font style for dragging tooltip text.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Brush for dragging tooltip text.
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background Brush for dragging tooltip.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion

        #endregion
    }
}
