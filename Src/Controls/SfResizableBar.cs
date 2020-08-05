// <copyright file="SfResizableBar.cs" company="Syncfusion. Inc">
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
    using System.Net;
    using System.Windows;
#if NETFX_CORE
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
#endif

    /// <summary>
    /// Represents the <see cref="SfChartResizableBar"/> class.
    /// </summary>
    public partial class SfChartResizableBar : ResizableScrollBar
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="ZoomPosition"/> property.
        /// </summary>
        internal static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register(
                "ZoomPosition",
                typeof(double),
                typeof(SfChartResizableBar),
                new PropertyMetadata(0d, OnZoomPositionChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ThumbLabelTemplate"/> property.
        /// </summary>
        internal static readonly DependencyProperty ThumbLabelTemplateProperty =
            DependencyProperty.Register(
                "ThumbLabelTemplate",
                typeof(DataTemplate),
                typeof(SfChartResizableBar),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="ThumbLabelVisibility"/> property.
        /// </summary>
        internal static readonly DependencyProperty ThumbLabelVisibilityProperty =
            DependencyProperty.Register(
                "EnableThumbLabel",
                typeof(Visibility),
                typeof(SfChartResizableBar),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        ///  The DependencyProperty for <see cref="ZoomFactor"/> property.
        /// </summary>
        internal static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register(
                "ZoomFactor",
                typeof(double),
                typeof(SfChartResizableBar),
                new PropertyMetadata(1d, OnZoomFactorChanged));

        #endregion

        #region Constants

        private const double Interval = 0.5, TransformSize = 20, RectCoordinate = 15;

        #endregion

        #region Fields

        private double previousZoomFactor;

        private DispatcherTimer timer;

        private ChartAxisBase2D axis;

        private bool onButtonPressed = false, isValueChanged = false;

        private DependencyObject parent;

        private ContentControl nearHandContentControl, farHandContentControl;

        private object handler;

        private ZoomChangingEventArgs zoomChangingEventArgs;

        private ZoomChangedEventArgs zoomChangedEventArgs;

        private PanChangingEventArgs panChangingEventArgs;

        private PanChangedEventArgs panChangedEventArgs;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SfChartResizableBar"/> class.
        /// </summary>
        public SfChartResizableBar()
        {          
            zoomChangingEventArgs = new ZoomChangingEventArgs();
            zoomChangedEventArgs = new ZoomChangedEventArgs();
            panChangingEventArgs = new PanChangingEventArgs();
            panChangedEventArgs = new PanChangedEventArgs();

#if NETFX_CORE
            this.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;
#endif
        }

        #endregion

        #region Properties

        #region Internal Properties

        /// <summary>
        /// Gets or sets the axis of the <see cref="SfChartResizableBar"/>.
        /// </summary>
        internal ChartAxisBase2D Axis
        {
            get
            {
                return axis;
            }

            set
            {
                if (axis != null)
                    DetachTouchModeEvents();
                axis = value;
                AttachTouchModeEvents();
                BindProperties();
            }
        }

        /// <summary>
        /// Gets or sets zoom position. Value must fall within 0 to 1. It determines starting value of visible range
        /// </summary>
        internal double ZoomPosition
        {
            get { return (double)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets Template For Visible Range Label View.
        /// </summary>
        internal DataTemplate ThumbLabelTemplate
        {
            get { return (DataTemplate)GetValue(ThumbLabelTemplateProperty); }
            set { SetValue(ThumbLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of Range Label View.
        /// </summary>
        internal Visibility ThumbLabelVisibility
        {
            get { return (Visibility)GetValue(ThumbLabelVisibilityProperty); }
            set { SetValue(ThumbLabelVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets zoom factor. Value must fall within 0 to 1. It determines delta of visible range.
        /// </summary>
        internal double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods


        internal void Dispose()
        {
            DetachTouchModeEvents();

#if NETFX_CORE
            if (handler != null)
            {
                if (parent is Page)
                    (parent as Page).RemoveHandler(Page.PointerPressedEvent, handler);

            }
#endif

            DiposeEvents();
            axis = null;
        }

        private void DiposeEvents()
        {
            if (MiddleThumb != null)
            {
                MiddleThumb.DragCompleted -= DragCompleted;
            }

            if (NearHand != null)
            {
                NearHand.DragCompleted -= DragCompleted;
            }

            if (FarHand != null)
            {
                FarHand.DragCompleted -= DragCompleted;
            }

            if (timer != null)
            {
                timer.Tick -= OnTimeout;
            }
        }

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/>.
        /// </summary>
        /// <param name="isVisible"><see cref="bool"/> value to indicate the visibility</param>
        internal void UpdateResizable(bool isVisible)
        {
            if (NearHand != null && FarHand != null)
            {
                if (EnableTouchMode)
                {
                    if (isVisible)
                    {
                        NearHand.Visibility = Visibility.Visible;
                        FarHand.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        NearHand.Visibility = Visibility.Collapsed;
                        FarHand.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    NearHand.Visibility = Visibility.Visible;
                    FarHand.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion
        
        #region Protected Methods

        /// <summary>
        /// Applies the template for the control.
        /// </summary>
#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (Axis != null)
                UpdateResizable(Axis.EnableScrollBarResizing);

            if (this.EnableTouchMode)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    nearHandContentControl = this.GetTemplateChild("HorizontalNearHandContent") as ContentControl;
                    farHandContentControl = this.GetTemplateChild("HorizontalFarHandContent") as ContentControl;
                }
                else
                {
                    nearHandContentControl = this.GetTemplateChild("VerticalNearHandContent") as ContentControl;
                    farHandContentControl = this.GetTemplateChild("VerticalFarHandContent") as ContentControl;
                }
            }

            if (MiddleThumb != null && NearHand != null && FarHand != null)
            {
                MiddleThumb.DragCompleted += DragCompleted;
                NearHand.DragCompleted += DragCompleted;
                FarHand.DragCompleted += DragCompleted;
            }
        }

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on value change.
        /// </summary>
        protected override void OnValueChanged()
        {
            var prevPosition = axis.ZoomPosition;
            var prevFactor = axis.ZoomFactor;
            var newZoomPosition = RangeStart;
            var newZoomFactor = (RangeEnd - RangeStart) >= SmallChange ? (RangeEnd - RangeStart) : ZoomFactor;

            if (isNearDragged || isFarDragged)
            {
                RaiseZoomChangingEvent(newZoomPosition, newZoomFactor);
            }

            if (canDrag)
            {
                RaisePanChangingEvent(prevPosition, newZoomPosition);
            }

            if (!Axis.DeferredScrolling)
            {
                if (!zoomChangingEventArgs.Cancel && (isNearDragged || isFarDragged))
                {
                    SetZoomingChanges(newZoomPosition, newZoomFactor);

                    RaiseZoomChangedEvent(prevPosition, prevFactor);
                }

                if (!panChangingEventArgs.Cancel && canDrag)
                {
                    // UWP-439-Zoom factor is getting changed while clicking on scroll bar's increase button
                    // While panning newZoomFactor is updated with newZoomPostion,It is resolved by passing the existing ZoomFactor                    
                    SetZoomingChanges(newZoomPosition, ZoomFactor);
                    RaisePanChangedevent(newZoomPosition);
                }
            }

            ResetTimer();
            isValueChanged = true;
            previousZoomFactor = 0;

            base.OnValueChanged();
        }

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on far hand dragged.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        protected override void OnFarHandDragged(object sender, DragDeltaEventArgs e)
        {
            if (this.Axis.EnableScrollBarResizing)
            {
                axis.IsScrolling = true;
                base.OnFarHandDragged(sender, e);
                if (farHandContentControl != null)
                    farHandContentControl.Visibility = (isValueChanged) ? Visibility.Visible : Visibility.Collapsed;
                onButtonPressed = true;
                if (this.EnableTouchMode)
                    Translate(farHandContentControl, RangeEnd);
                isValueChanged = false;
            }
            else
            {
                OnThumbDragged(sender, e);
            }
        }

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on near hand dragged.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        protected override void OnNearHandDragged(object sender, DragDeltaEventArgs e)
        {
            if (this.Axis.EnableScrollBarResizing)
            {
                axis.IsScrolling = true;
                base.OnNearHandDragged(sender, e);
                if (nearHandContentControl != null)
                    nearHandContentControl.Visibility = (isValueChanged) ? Visibility.Visible : Visibility.Collapsed;
                onButtonPressed = true;
                if (this.EnableTouchMode)
                    Translate(nearHandContentControl, RangeStart);
                isValueChanged = false;
            }
            else
            {
                OnThumbDragged(sender, e);
            }
        }

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on thumb dragged.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        protected override void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            axis.IsScrolling = true;
            base.OnThumbDragged(sender, e);
            onButtonPressed = true;
        }

        #endregion
        
        #region Private Static Methods

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on zoom factor changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChartResizableBar).ChangeZoomFactor((double)e.NewValue);
        }

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on zoom position changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnZoomPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChartResizableBar).ChangeZoomPosition((double)e.NewValue);
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Middles the thumb drag completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DragCompletedEventArgs"/> instance containing the event data.</param>
        private void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (axis != null)
                axis.IsScrolling = false;
        }

        /// <summary>
        /// Attaches the touch mode events.
        /// </summary>
        private void AttachTouchModeEvents()
        {
#if NETFX_CORE 
            Axis.Loaded += OnAxisLoaded;
            Axis.Unloaded += OnAxisUnloaded;
#else
            // This is modified to fix the issue thumb viewing while switch over the theme. 
            this.Loaded += OnAxisLoaded;
            this.Unloaded += OnAxisUnloaded;
#endif
        }

        /// <summary>
        /// Detaches the touch mode events.
        /// </summary>
        private void DetachTouchModeEvents()
        {
            if (Axis != null)
            {

#if NETFX_CORE || UNIVERSAL
            Axis.PointerEntered -= OnAxisPointerEntered;
            Axis.PointerExited -= OnAxisPointerExited;
            Axis.Loaded -= OnAxisLoaded;
            Axis.Unloaded -= OnAxisUnloaded;
#else
            Axis.MouseEnter -= OnAxisMouseEnter;
            Axis.MouseLeave -= OnAxisMouseLeave;
            this.Loaded -= OnAxisLoaded;
            this.Unloaded -= OnAxisUnloaded;
#endif
            }
        }

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on axis unloaded.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnAxisUnloaded(object sender, RoutedEventArgs e)
        {
#if NETFX_CORE
            if (Axis != null)
            {
                Axis.PointerEntered -= OnAxisPointerEntered;
                Axis.PointerExited -= OnAxisPointerExited;
            }

            if (parent is Page)
                (parent as Page).RemoveHandler(Page.PointerPressedEvent, handler);
#endif
        }

        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on axis loaded. 
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnAxisLoaded(object sender, RoutedEventArgs e)
        {
#if NETFX_CORE
            if (Axis != null)
            {
                Axis.PointerEntered += OnAxisPointerEntered;
                Axis.PointerExited += OnAxisPointerExited;
            }
#endif
            if (Axis.Area != null)
            {
                parent = VisualTreeHelper.GetParent(Axis.Area);
                if (parent != null)
                {
                    while (VisualTreeHelper.GetParent(parent) != null)
                    {
                        parent = VisualTreeHelper.GetParent(parent);
#if NETFX_CORE 
                        if (parent is Page)
                            break;
#endif
                    }
#if NETFX_CORE
                    handler = new PointerEventHandler(OnPointerPressed);
                    if (parent is Page)
                        (parent as Page).AddHandler(Page.PointerPressedEvent, handler, true);
#endif
                }
            }
        }

#if NETFX_CORE
        /// <summary>
        /// Updates the <see cref="SfChartResizableBar"/> on axis loaded. 
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (this.Axis != null && this.Axis.Area != null)
            {
                Point currentPosition = (Point)e.GetCurrentPoint(Axis.Area).Position;
                if (CheckRegion(currentPosition))
                {
                    VisualStateManager.GoToState(this, "OnView", true);
                    onButtonPressed = true;
                }
                else
                {
                    VisualStateManager.GoToState(this, "OnLostFocus", true);
                    onButtonPressed = false;
                }
            }
        }

        private void OnAxisPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!Axis.isManipulated && !onButtonPressed)
                VisualStateManager.GoToState(this, "OnFocus", true);
        }

        private void OnAxisPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!onButtonPressed)
                VisualStateManager.GoToState(this, "OnExit", true);
        }
#else
        /// <summary>
        /// Updates the mouse down interactive features of the <see cref="SfChartResizableBar"/>.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Axis == null || Axis.Area == null)
            {
                return;
            }

            Point currentPosition = (Point)e.GetPosition(Axis.Area);
            if (CheckRegion(currentPosition))
            {
                VisualStateManager.GoToState(this, "OnView", true);
                onButtonPressed = true;
            }
            else
            {
                VisualStateManager.GoToState(this, "OnLostFocus", true);
                onButtonPressed = false;
            }
        }

        /// <summary>
        /// Updates the interactive features of the <see cref="SfChartResizableBar"/> when mouse leaves the axis.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnAxisMouseLeave(object sender, MouseEventArgs e)
        {
            if (!onButtonPressed)
                VisualStateManager.GoToState(this, "OnExit", true);
        }

        /// <summary>
        /// Updates the interactive features of the <see cref="SfChartResizableBar"/> when mouse enters the axis.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnAxisMouseEnter(object sender, MouseEventArgs e)
        {
            if (!Axis.isManipulated && !onButtonPressed)
                VisualStateManager.GoToState(this, "OnFocus", true);
        }

#endif
        /// <summary>
        /// Checks whether the point is inside the axis.
        /// </summary>
        /// <param name="position">The Position</param>
        /// <returns>Returns the indication whether the pointer is inside the axis.</returns>
        private bool CheckRegion(Point position)
        {
            if (Axis.EnableScrollBar && Axis.Visibility != Visibility.Collapsed)
            {
                Rect rect = new Rect();
                rect = Axis.ArrangeRect;
                if (this.Orientation == Orientation.Horizontal)
                {
                    rect.X = rect.X - RectCoordinate;
                    rect.Y = (Axis.OpposedPosition) ? rect.Y + RectCoordinate : rect.Y - RectCoordinate;
                }
                else
                {
                    rect.X = (Axis.OpposedPosition) ? rect.X - RectCoordinate : rect.X + RectCoordinate;
                    rect.Y = rect.Y - RectCoordinate;
                }

                if (axis.EnableTouchMode == true && FarHand != null && NearHand != null && MiddleThumb != null)
                {
                    if (rect.Contains(position) || FarHand.IsDragging || NearHand.IsDragging || MiddleThumb.IsDragging)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Zoom factor and zoom position is set.
        /// </summary>
        /// <param name="newZoomPosition">The New Zoom Position</param>
        /// <param name="newZoomFactor">The New Zoom Factor</param>
        private void SetZoomingChanges(double newZoomPosition, double newZoomFactor)
        {
            ZoomPosition = newZoomPosition;
            ZoomFactor = newZoomFactor;
        }

        /// <summary>
        /// Raised when zooming occurs.
        /// </summary>
        /// <param name="newPosition">The New Position</param>
        /// <param name="newFactor">The New Factor</param>
        private void RaiseZoomChangingEvent(double newPosition, double newFactor)
        {
            if (axis.Area != null)
            {
                zoomChangingEventArgs.Axis = axis;
                zoomChangingEventArgs.CurrentFactor = newFactor;
                zoomChangingEventArgs.CurrentPosition = newPosition;
                zoomChangingEventArgs.PreviousFactor = ZoomFactor;
                zoomChangingEventArgs.PreviousPosition = ZoomPosition;
                if (axis is DateTimeAxis || axis is DateTimeCategoryAxis)
                {
                    zoomChangingEventArgs.OldRange = new DateTimeRange(axis.VisibleRange.Start.FromOADate(), axis.VisibleRange.End.FromOADate());
                }
                else
                {
                    zoomChangingEventArgs.OldRange = new DoubleRange(axis.VisibleRange.Start, axis.VisibleRange.End);
                }

                zoomChangingEventArgs.Cancel = false;
                (axis.Area as SfChart).OnZoomChanging(zoomChangingEventArgs);
            }
        }

        /// <summary>
        /// Raised when zooming is done.
        /// </summary>
        /// <param name="prevPosition">The Previous Position</param>
        /// <param name="prevFactor">The Previous Factor</param>
        private void RaiseZoomChangedEvent(double prevPosition, double prevFactor)
        {
            if (axis.Area != null)
            {
                var newRange = axis.CalculateRange(axis.ActualRange, ZoomPosition, ZoomFactor);
                zoomChangedEventArgs.Axis = axis;
                zoomChangedEventArgs.CurrentFactor = ZoomFactor;
                zoomChangedEventArgs.CurrentPosition = ZoomPosition;
                zoomChangedEventArgs.PreviousFactor = prevFactor;
                zoomChangedEventArgs.PreviousPosition = prevPosition;
                if (axis is DateTimeAxis || axis is DateTimeCategoryAxis)
                {
                    zoomChangedEventArgs.NewRange = new DateTimeRange(newRange.Start.FromOADate(), newRange.End.FromOADate());
                    zoomChangedEventArgs.OldRange = new DateTimeRange(axis.VisibleRange.Start.FromOADate(), axis.VisibleRange.End.FromOADate());
                }
                else
                {
                    zoomChangedEventArgs.NewRange = newRange;
                    zoomChangedEventArgs.OldRange = new DoubleRange(axis.VisibleRange.Start, axis.VisibleRange.End);
                }

                (axis.Area as SfChart).OnZoomChanged(zoomChangedEventArgs);
            }
        }

        /// <summary>
        /// Raised when panning is changed.
        /// </summary>
        /// <param name="zoomPosition">The Zoom Position</param>
        private void RaisePanChangedevent(double zoomPosition)
        {
            if (axis.Area != null)
            {
                panChangedEventArgs.Axis = axis;
                panChangedEventArgs.NewZoomPosition = zoomPosition;

                (axis.Area as SfChart).OnPanChanged(panChangedEventArgs);
            }
        }

        /// <summary>
        /// Update the range according to the zoom position.
        /// </summary>
        /// <param name="value">The Value</param>
        private void ChangeZoomPosition(double value)
        {
            IsValueChangedTrigger = false;
            RangeStart = value;
            if (previousZoomFactor == ZoomFactor)
                RangeEnd = RangeStart + ZoomFactor;
            previousZoomFactor = ZoomFactor;
        }

        /// <summary>
        /// Raised when panning occurs.
        /// </summary>
        /// <param name="prevPosition">The Previous Position</param>
        /// <param name="newPosition">The New Position</param>
        private void RaisePanChangingEvent(double prevPosition, double newPosition)
        {
            if (axis.Area != null)
            {
                panChangingEventArgs.Axis = axis;
                panChangingEventArgs.OldZoomPosition = prevPosition;
                panChangingEventArgs.NewZoomPosition = newPosition;
                panChangingEventArgs.Cancel = false;

                (axis.Area as SfChart).OnPanChanging(panChangingEventArgs);
            }
        }

        /// <summary>
        /// Update the range according to the zoom factor.
        /// </summary>
        /// <param name="value">The Value</param>
        private void ChangeZoomFactor(double value)
        {
            IsValueChangedTrigger = false;
            RangeEnd = RangeStart + value;
            RangeStart = ZoomPosition;
            previousZoomFactor = ZoomFactor;
        }

        /// <summary>
        /// Binds the resizable bar with the axis.
        /// </summary>
        private void BindProperties()
        {
#if WINDOWS_UAP && CHECKLATER
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
#endif
            {
                Binding zoomPositionBind = new Binding();
                zoomPositionBind.Source = Axis;
                zoomPositionBind.Path = new PropertyPath("ZoomPosition");
                zoomPositionBind.Mode = BindingMode.TwoWay;
                this.SetBinding(SfChartResizableBar.ZoomPositionProperty, zoomPositionBind);
                Binding zoomFactorBind = new Binding();
                zoomFactorBind.Source = Axis;
                zoomFactorBind.Path = new PropertyPath("ZoomFactor");
                zoomFactorBind.Mode = BindingMode.TwoWay;
                this.SetBinding(SfChartResizableBar.ZoomFactorProperty, zoomFactorBind);
                Binding enableTouchModeBind = new Binding();
                enableTouchModeBind.Source = Axis;
                enableTouchModeBind.Path = new PropertyPath("EnableTouchMode");
                enableTouchModeBind.Mode = BindingMode.TwoWay;
                this.SetBinding(SfChartResizableBar.EnableTouchModeProperty, enableTouchModeBind);

                Binding thumbLabelBind = new Binding();
                thumbLabelBind.Source = Axis;
                thumbLabelBind.Path = new PropertyPath("ThumbLabelTemplate");
                this.SetBinding(SfChartResizableBar.ThumbLabelTemplateProperty, thumbLabelBind);
                Binding enableThumbLabelBind = new Binding();
                enableThumbLabelBind.Source = Axis;
                enableThumbLabelBind.Path = new PropertyPath("ThumbLabelVisibility");
                this.SetBinding(SfChartResizableBar.ThumbLabelVisibilityProperty, enableThumbLabelBind);
            }
        }

        /// <summary>
        /// Translate the thumb label templates.
        /// </summary>
        /// <param name="contentControl">The Thumb Label</param>
        /// <param name="rangeValue">The Range Value</param>
        private void Translate(ContentControl contentControl, double rangeValue)
        {
            if (ThumbLabelVisibility == Visibility.Visible)
            {
                contentControl.ContentTemplate = ThumbLabelTemplate;
                contentControl.Content = Axis is NumericalAxis ? Convert.ToDecimal(Axis.GetLabelContent(Axis.CoefficientToActualValue(rangeValue))).ToString("0.##") : Convert.ToString(Axis.GetLabelContent(Axis.CoefficientToActualValue(rangeValue)));
                TranslateTransform translate = new TranslateTransform();
                if (Orientation == Orientation.Horizontal)
                {
                    translate.X = -contentControl.ActualWidth / 2;
                    translate.Y = Axis.OpposedPosition ? TransformSize : -TransformSize;
                }
                else
                {
                    translate.X = Axis.OpposedPosition ? -(2 * TransformSize) : TransformSize;
                    translate.Y = -contentControl.ActualHeight / 2;
                }

                contentControl.RenderTransform = translate;
            }
            else
                contentControl.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        private void ResetTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Interval = TimeSpan.FromSeconds(Interval);
                timer.Start();
            }
            else
            {
                timer = new DispatcherTimer();
                timer.Tick += OnTimeout;
            }
        }

        /// <summary>
        /// Time outs the timer operations.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnTimeout(object sender, object e)
        {
            if (Axis.DeferredScrolling == true)
            {
                ZoomPosition = RangeStart;
                ZoomFactor = (RangeEnd - RangeStart) >= SmallChange ? (RangeEnd - RangeStart) : (ZoomFactor == Maximum) ? SmallChange : ZoomFactor;
            }

            if (nearHandContentControl != null && farHandContentControl != null)
            {
                nearHandContentControl.Visibility = Visibility.Collapsed;
                farHandContentControl.Visibility = Visibility.Collapsed;
            }

            if (timer != null)
                timer.Stop();
            timer.Tick -= OnTimeout;
            timer = null;
        }
    }
    #endregion

    #endregion
}
