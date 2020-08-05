// <copyright file="ResizableScrollBar.cs" company="Syncfusion. Inc">
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
    using System.Net;
    using System.Windows;
#if NETFX_CORE
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;
    using Popup = Windows.UI.Xaml.Controls.Primitives.Popup;
#endif

    /// <summary>
    /// Represents the <see cref="ResizableScrollBar"/> class.
    /// </summary>
    [TemplatePartAttribute(Name = "VerticalRoot", Type = typeof(FrameworkElement))]
    [TemplatePartAttribute(Name = "HorizontalRoot", Type = typeof(FrameworkElement))]
    [TemplatePartAttribute(Name = "HorizontalLargeIncrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "HorizontalLargeDecrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "HorizontalSmallDecrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "HorizontalSmallIncrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "HorizontalThumb", Type = typeof(Thumb))]
    [TemplatePartAttribute(Name = "HorizontalThumbHand1", Type = typeof(Popup))]
    [TemplatePartAttribute(Name = "HorizontalThumbHand2", Type = typeof(Popup))]
    [TemplatePartAttribute(Name = "VerticalLargeIncrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "VerticalLargeDecrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "VerticalSmallIncrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "VerticalSmallDecrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "VerticalThumb", Type = typeof(Thumb))]
    [TemplatePartAttribute(Name = "VerticalThumbHand1", Type = typeof(Thumb))]
    [TemplatePartAttribute(Name = "VerticalThumbHand2", Type = typeof(Thumb))]
    [TemplateVisualStateAttribute(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "OnFocus", GroupName = "TouchMode")]
    [TemplateVisualStateAttribute(Name = "OnLostFocus", GroupName = "TouchMode")]
    [TemplateVisualStateAttribute(Name = "OnExit", GroupName = "TouchMode")]
    [TemplateVisualStateAttribute(Name = "OnView", GroupName = "TouchMode")]
    public partial class ResizableScrollBar : ContentControl
    {
        #region Dependency Property Registration

        /// <summary>
        ///  The DependencyProperty for <see cref="Orientation"/> property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(ResizableScrollBar),
                new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(double),
                typeof(ResizableScrollBar),
                new PropertyMetadata(1d));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(double),
                typeof(ResizableScrollBar),
                new PropertyMetadata(0d));

        /// <summary>
        ///  The DependencyProperty for <see cref="ViewSizePort"/> property.
        /// </summary>
        public static readonly DependencyProperty ViewSizePortProperty =
            DependencyProperty.Register(
                "ViewSizePort",
                typeof(double),
                typeof(ResizableScrollBar),
                new PropertyMetadata(0d));

        /// <summary>
        /// The DependencyProperty for <see cref="SmallChange"/> property.
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(
                "SmallChange",
                typeof(double),
                typeof(ResizableScrollBar),
                new PropertyMetadata(0.01d));

        /// <summary>
        /// The DependencyProperty for <see cref="LargeChange"/> property.
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register(
                "LargeChange",
                typeof(double),
                typeof(ResizableScrollBar),
                new PropertyMetadata(0.1d));

        /// <summary>
        ///  The DependencyProperty for <see cref="Scale"/> property.
        /// </summary>
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                "Scale",
                typeof(double),
                typeof(ResizableScrollBar),
                new PropertyMetadata(1d));

        /// <summary>
        /// The DependencyProperty for <see cref="RangeStart"/> property.
        /// </summary>
        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register(
                "RangeStart",
                typeof(double),
                typeof(ResizableScrollBar),
                new PropertyMetadata(0d, OnRangeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="RangeEnd"/> property.
        /// </summary>
        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register(
                "RangeEnd",
                typeof(double),
                typeof(ResizableScrollBar),
                new PropertyMetadata(1d, OnRangeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ScrollButtonVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty ScrollButtonVisibilityProperty =
            DependencyProperty.Register(
                "ScrollButtonVisibility",
                typeof(Visibility),
                typeof(ResizableScrollBar),
                new PropertyMetadata(Visibility.Visible, OnIncreaseDecreaseVisibilityChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableTouchMode"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableTouchModeProperty =
            DependencyProperty.Register(
                "EnableTouchMode",
                typeof(bool),
                typeof(ResizableScrollBar),
                new PropertyMetadata(false));

        #endregion
                
        #region Fields

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "This is a public property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a public property")]
        public bool isFarDragged = false;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "This is a public property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a public property")]
        public bool isNearDragged = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "When upper case property is used it hides the CanDrag control property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "When upper case property is used it hides the CanDrag control property")]
        internal bool canDrag = false;
        
        #region Constants

        private const double GapSize = 4, MinimumThumbSize = 0, ResizableBarSize = 15, MinimumDiff = 0, optimizeEnd = 1.5;

        #endregion

        private Grid horizontalRoot, verticalRoot;
        private Size desiredSize, availableSize;
        private double resizeThumbSize, smallThumbSize, actualTrackSize, actualSize, previousThumbSize, rangeDiff;
        private double middleThumbSize, largeDecreaseThumbSize, largeIncreaseThumbSize, actualLargeThumbSize;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizableScrollBar"/> class.
        /// </summary>
        public ResizableScrollBar()
        {
            this.DefaultStyleKey = typeof(ResizableScrollBar);
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the Orientation for the Scroll Bar
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets Maximum Value for Scroll Bar
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets Minimum Value for Scroll Bar
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets ViewSizePort Value for Scroll Bar
        /// </summary>
        public double ViewSizePort
        {
            get { return (double)GetValue(ViewSizePortProperty); }
            set { SetValue(ViewSizePortProperty, value); }
        }

        /// <summary>
        /// Gets or sets SmallChange Value for Scroll Bar Thumb Change When the Small Increase and Decrease Button is Clicked.
        /// </summary>
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets LargeChange Value for Scroll Bar Thumb Change When the Large Increase and Decrease Button is Clicked.
        /// </summary>
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that determines how far the scroll content is scaled. 
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        /// <summary>
        /// Gets or sets RangeStart Value for Scroll Bar.
        /// </summary>
        public double RangeStart
        {
            get { return (double)GetValue(RangeStartProperty); }
            set { SetValue(RangeStartProperty, value); }
        }

        /// <summary>
        /// Gets or sets RangeEnd Value for Scroll Bar.
        /// </summary>
        public double RangeEnd
        {
            get { return (double)GetValue(RangeEndProperty); }
            set { SetValue(RangeEndProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of scroll buttons.
        /// </summary>
        public Visibility ScrollButtonVisibility
        {
            get { return (Visibility)GetValue(ScrollButtonVisibilityProperty); }
            set { SetValue(ScrollButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to EnableTouchMode.
        /// </summary>
        public bool EnableTouchMode
        {
            get { return (bool)GetValue(EnableTouchModeProperty); }
            set { SetValue(EnableTouchModeProperty, value); }
        }

        /// <summary>
        /// Gets the resizable thumb size.
        /// </summary>
        public double ResizableThumbSize
        {
            get
            {
                return resizeThumbSize;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the available size.
        /// </summary>
        internal Size AvailabeSize
        {
            get
            {
                return availableSize;
            }
        }

        /// <summary>
        /// Gets the track size.
        /// </summary>
        internal double TrackSize
        {
            get
            {
                return actualTrackSize;
            }
        }
               
        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets a value indicating whether the value changed is triggered.
        /// </summary>
        protected internal bool IsValueChangedTrigger { get; set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the near hand.
        /// </summary>
        protected Thumb NearHand { get; set; }

        /// <summary>
        /// Gets or sets the far hand.
        /// </summary>
        protected Thumb FarHand { get; set; }

        /// <summary>
        /// Gets or sets the middle thumb.
        /// </summary>
        protected Thumb MiddleThumb { get; set; }

        /// <summary>
        /// Gets or sets the small decrease.
        /// </summary>
        protected RepeatButton SmallDecrease { get; set; }

        /// <summary>
        /// Gets or sets the large decrease.
        /// </summary>
        protected RepeatButton LargeDecrease { get; set; }

        /// <summary>
        /// Gets or sets the large increase.
        /// </summary>
        protected RepeatButton LargeIncrease { get; set; }

        /// <summary>
        /// Gets or sets the small increase.
        /// </summary>
        protected RepeatButton SmallIncrease { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Applied the required templates for the control.
        /// </summary>
#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            ApplyOrientationTemplate();
        }

        /// <summary>
        /// Applies the required orientation template on orientation changed.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyOrientationTemplate();
        }

        /// <summary>
        /// Updates the <see cref="ResizableBarSize"/> on range value changed.
        /// </summary>
        protected virtual void OnRangeValueChanged()
        {
            RangeMinMax();
            Scale = (Maximum - Minimum) / (RangeEnd - RangeStart);
            InvalidateArrange();
            if (IsValueChangedTrigger)
                OnValueChanged();
            IsValueChangedTrigger = (canDrag) ? false : true;
        }

        /// <summary>
        /// Measures the control.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the measure size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            this.availableSize = ChartLayoutUtils.CheckSize(availableSize);
            if (this.Orientation == Orientation.Horizontal && NearHand != null && FarHand != null)
            {
                desiredSize.Width = this.availableSize.Width;
                desiredSize.Height = ResizableBarSize;
                if (NearHand.Visibility != Visibility.Collapsed)
                {
                    NearHand.Measure(availableSize);
                    Size thumbSize = NearHand.DesiredSize;
                    if (thumbSize.Width == 0)
                        resizeThumbSize = NearHand.MinWidth;
                    else
                        resizeThumbSize = thumbSize.Width;
                    NearHand.Width = resizeThumbSize;
                    FarHand.Width = resizeThumbSize;
                }
                else
                {
                    resizeThumbSize = 0d;
                }

                if (this.ScrollButtonVisibility != Visibility.Collapsed)
                {
                    SmallIncrease.Measure(availableSize);
                    Size scrollButtonSize = SmallIncrease.DesiredSize;
                    if (scrollButtonSize.Width == 0)
                        smallThumbSize = SmallIncrease.MinWidth;
                    else
                        smallThumbSize = (smallThumbSize == 0) ? scrollButtonSize.Width : Math.Min(smallThumbSize, scrollButtonSize.Width);
                    SmallDecrease.Width = smallThumbSize;
                    SmallIncrease.Width = smallThumbSize;
                }
                else
                {
                    SmallDecrease.Width = smallThumbSize = 0;
                    SmallIncrease.Width = smallThumbSize = 0;
                }
            }
            else if (this.Orientation == Orientation.Vertical && FarHand != null && NearHand != null)
            {
                desiredSize.Height = this.availableSize.Height;
                desiredSize.Width = ResizableBarSize;
                NearHand.Measure(availableSize);
                if (NearHand.Visibility != Visibility.Collapsed)
                {
                    Size thumbSize = NearHand.DesiredSize;
                    if (thumbSize.Height == 0)
                        resizeThumbSize = NearHand.MinHeight;
                    else
                        resizeThumbSize = thumbSize.Height;
                    NearHand.Height = resizeThumbSize;
                    FarHand.Height = resizeThumbSize;
                }
                else
                {
                    resizeThumbSize = 0d;
                }

                if (this.ScrollButtonVisibility != Visibility.Collapsed)
                {
                    SmallIncrease.Measure(availableSize);
                    Size scrollButtonSize = SmallIncrease.DesiredSize;
                    if (scrollButtonSize.Width == 0)
                        smallThumbSize = SmallIncrease.MinHeight;
                    else
                        smallThumbSize = smallThumbSize = (smallThumbSize == 0) ? scrollButtonSize.Height : Math.Min(smallThumbSize, scrollButtonSize.Height);
                    SmallDecrease.Height = smallThumbSize;
                    SmallIncrease.Height = smallThumbSize;
                }
                else
                {
                    SmallDecrease.Height = smallThumbSize = 0;
                    SmallIncrease.Height = smallThumbSize = 0;
                }
            }

            return desiredSize;
        }

        /// <summary>
        /// Arranges the elements in the control.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        /// <returns>Returns the arrange size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
#if WINDOWS_UAP
            // This is a temprorary fix , for the issue WPF-21714, in viewing the vertical scroll bar resizable thumb in UWP platform.
            // When the children total height equals to desired height then one pixel is added to render size height. 
            actualSize = (this.Orientation == Orientation.Horizontal) ? finalSize.Width : this.EnableTouchMode ? finalSize.Height - 1 : finalSize.Height;
#else
            actualSize = (this.Orientation == Orientation.Horizontal) ? finalSize.Width : finalSize.Height;
#endif
            actualTrackSize = actualSize - (2 * smallThumbSize) - (this.EnableTouchMode ? 0 : 2 * resizeThumbSize);
            CalculateSize();
            actualLargeThumbSize = this.EnableTouchMode ? actualSize : actualTrackSize;
            ThumbMinMax();
            rangeDiff = rangeDiff == 0 ? SmallChange : rangeDiff;
            if (this.Orientation == Orientation.Horizontal)
            {
                MiddleThumb.Width = middleThumbSize;
                LargeDecrease.Width = largeDecreaseThumbSize;
                LargeIncrease.Width = largeIncreaseThumbSize;
#if NETFX_CORE 
                LargeDecrease.Height = finalSize.Height;
                LargeIncrease.Height = finalSize.Height;
#endif
            }
            else if (this.Orientation == Orientation.Vertical)
            {
                MiddleThumb.Height = middleThumbSize;
                LargeDecrease.Height = largeDecreaseThumbSize;
                LargeIncrease.Height = largeIncreaseThumbSize;
#if NETFX_CORE 
                LargeDecrease.Width = finalSize.Width;
                LargeIncrease.Width = finalSize.Width;
#endif
            }

            return finalSize;
        }

        /// <summary>
        /// Updates the scroll bar when the thumb is dragged.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        protected virtual void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            double thumbSize = (this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height;
            if (!(RangeStart == Minimum && RangeEnd == Maximum))
            {
                CalculateRangeDifference();
                double change = (this.Orientation == Orientation.Horizontal) ? e.HorizontalChange : -e.VerticalChange;
                if (change < 0 && RangeStart != Minimum)
                    canDrag = true;
                else if (change > 0 && RangeEnd != Maximum)
                    canDrag = true;
                if (canDrag)
                {
                    double newChange = (change * (Maximum - Minimum)) / actualTrackSize;
                    IsValueChangedTrigger = false;
                    CalculateChange(newChange, newChange);
                    RangeStart = (RangeStart > Maximum - rangeDiff) ? Maximum - rangeDiff : RangeStart;
                    RangeEnd = (RangeEnd < rangeDiff) ? rangeDiff : RangeEnd;
                    isFarDragged = false;
                    isNearDragged = false;
                    OnValueChanged();
                    CalculateOperations();
                }
            }
        }

        /// <summary>
        /// Updates the scroll bar on far hand dragged.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        protected virtual void OnFarHandDragged(object sender, DragDeltaEventArgs e)
        {
            double change = (this.Orientation == Orientation.Horizontal) ? e.HorizontalChange : -e.VerticalChange;
            change = ((actualTrackSize) * change) / (actualTrackSize - (this.EnableTouchMode ? 0 : 2 * resizeThumbSize));
            double newChange = (change * (Maximum - Minimum)) / actualSize;
            double thumbSize = (this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height;

            bool canChange = false;

            if (RangeEnd + newChange <= Maximum)
            {
                if (((RangeEnd + newChange) - RangeStart) >= (MinimumDiff * SmallChange))
                {
                    canChange = true;
                }
            }
            else if (RangeEnd < Maximum)
            {
                RangeEnd = Maximum;
                canChange = true;
            }

            if (canChange)
            {
                if (thumbSize != MinimumThumbSize || change > 0)
                {
                    IsValueChangedTrigger = false;
                    rangeDiff = (MinimumDiff * SmallChange);
                    CalculateChange(0, newChange);
                    Scale = (Maximum - Minimum) / (RangeEnd - RangeStart);
                    CalculateOperations();
                    isFarDragged = true;
                    isNearDragged = false;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Updates the scroll bar on near hand dragged.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        protected virtual void OnNearHandDragged(object sender, DragDeltaEventArgs e)
        {
            double change = (this.Orientation == Orientation.Horizontal) ? e.HorizontalChange : -e.VerticalChange;
            change = ((actualTrackSize) * change) / (actualTrackSize - (this.EnableTouchMode ? 0 : 2 * resizeThumbSize));
            double newChange = (change * (Maximum - Minimum)) / actualSize;
            double thumbSize = (this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height;
            bool canChange = false;

            if (RangeStart + newChange >= Minimum)
            {
                if ((RangeEnd - (RangeStart + newChange)) >= (MinimumDiff * SmallChange))
                {
                    canChange = true;
                }
            }
            else if (RangeStart > Minimum)
            {
                RangeStart = Minimum;
                canChange = true;
            }
            
            if (canChange)
            {
                if (thumbSize != MinimumThumbSize || change < 0)
                {
                    IsValueChangedTrigger = false;
                    rangeDiff = (MinimumDiff * SmallChange);
                    CalculateChange(newChange, 0);
                    Scale = (Maximum - Minimum) / (RangeEnd - RangeStart);
                    CalculateOperations();
                    isFarDragged = false;
                    isNearDragged = true;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="ResizableScrollBar"/> on it's value change.
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
                IsValueChangedTrigger = true;
            }

            canDrag = false;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the <see cref="ResizableScrollBar"/> on orientation changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResizableScrollBar).OnOrientationChanged(e);
        }

        /// <summary>
        /// Updates the <see cref="ResizableScrollBar"/> on orientation changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResizableScrollBar).OnRangeValueChanged();
        }

        /// <summary>
        /// Updates the <see cref="ResizableScrollBar"/> on scroll button visibility changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnIncreaseDecreaseVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var resizableScrollBar = d as ResizableScrollBar;
            resizableScrollBar.MeasureOverride(resizableScrollBar.AvailabeSize);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the orientation templates.
        /// </summary>
        private void ApplyOrientationTemplate()
        {
            horizontalRoot = this.GetTemplateChild("HorizontalRoot") as Grid;
            verticalRoot = this.GetTemplateChild("VerticalRoot") as Grid;

            if (LargeIncrease != null)
                LargeIncrease.Click -= OnLargeIncreaseClick;
            if (LargeDecrease != null)
                LargeDecrease.Click -= OnLargeDecreaseClick;
            if (SmallDecrease != null)
                SmallDecrease.Click -= OnSmallDecreaseClick;
            if (SmallIncrease != null)
                SmallIncrease.Click -= OnSmallIncreaseClick;
            if (NearHand != null)
                NearHand.DragDelta -= OnNearHandDragged;

            if (FarHand != null)
                FarHand.DragDelta -= OnFarHandDragged;

            if (MiddleThumb != null)
                MiddleThumb.DragDelta -= OnThumbDragged;

            if (this.Orientation == Orientation.Horizontal && horizontalRoot != null)
            {
                horizontalRoot.Visibility = Visibility.Visible;
                verticalRoot.Visibility = Visibility.Collapsed;
                NearHand = this.GetTemplateChild("HorizontalThumbHand1") as Thumb;
                FarHand = this.GetTemplateChild("HorizontalThumbHand2") as Thumb;
                MiddleThumb = this.GetTemplateChild("HorizontalThumb") as Thumb;
                SmallDecrease = this.GetTemplateChild("HorizontalSmallDecrease") as RepeatButton;
                LargeDecrease = this.GetTemplateChild("HorizontalLargeDecrease") as RepeatButton;
                LargeIncrease = this.GetTemplateChild("HorizontalLargeIncrease") as RepeatButton;
                SmallIncrease = this.GetTemplateChild("HorizontalSmallIncrease") as RepeatButton;
                NearHand.DragDelta += OnNearHandDragged;
                FarHand.DragDelta += OnFarHandDragged;
                MiddleThumb.DragDelta += OnThumbDragged;
                LargeIncrease.Click += OnLargeIncreaseClick;
                LargeDecrease.Click += OnLargeDecreaseClick;
                SmallDecrease.Click += OnSmallDecreaseClick;
                SmallIncrease.Click += OnSmallIncreaseClick;
            }
            else if (verticalRoot != null)
            {
                horizontalRoot.Visibility = Visibility.Collapsed;
                verticalRoot.Visibility = Visibility.Visible;
                NearHand = this.GetTemplateChild("VerticalThumbHand1") as Thumb;
                FarHand = this.GetTemplateChild("VerticalThumbHand2") as Thumb;
                MiddleThumb = this.GetTemplateChild("VerticalThumb") as Thumb;
                SmallDecrease = this.GetTemplateChild("VerticalSmallDecrease") as RepeatButton;
                LargeDecrease = this.GetTemplateChild("VerticalLargeDecrease") as RepeatButton;
                LargeIncrease = this.GetTemplateChild("VerticalLargeIncrease") as RepeatButton;
                SmallIncrease = this.GetTemplateChild("VerticalSmallIncrease") as RepeatButton;
                NearHand.DragDelta += OnNearHandDragged;
                FarHand.DragDelta += OnFarHandDragged;
                MiddleThumb.DragDelta += OnThumbDragged;
                LargeIncrease.Click += OnLargeIncreaseClick;
                LargeDecrease.Click += OnLargeDecreaseClick;
                SmallDecrease.Click += OnSmallDecreaseClick;
                SmallIncrease.Click += OnSmallIncreaseClick;
            }

            IsValueChangedTrigger = true;
        }

        /// <summary>
        /// Updates the <see cref="ResizableScrollBar"/> on small increase click.
        /// </summary>
        /// <param name="sender">The Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSmallIncreaseClick(object sender, RoutedEventArgs e)
        {
            if (RangeEnd != Maximum)
            {
                IncreaseDecreaseOperation(SmallChange, SmallChange);
            }
        }
        
        /// <summary>
        /// Updates the <see cref="ResizableScrollBar"/> on small decrease click.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSmallDecreaseClick(object sender, RoutedEventArgs e)
        {
            if (RangeStart != Minimum)
            {
                IncreaseDecreaseOperation(-SmallChange, -SmallChange);
            }
        }

        /// <summary>
        /// Updates the <see cref="ResizableScrollBar"/> on large decrease click.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnLargeDecreaseClick(object sender, RoutedEventArgs e)
        {
            if (!(RangeStart == Minimum))
            {
                IncreaseDecreaseOperation(-LargeChange, -LargeChange);
            }
        }

        /// <summary>
        /// Updates the <see cref="ResizableScrollBar"/> on large increase click.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnLargeIncreaseClick(object sender, RoutedEventArgs e)
        {
            if (!(RangeEnd == Maximum))
            {
                IncreaseDecreaseOperation(LargeChange, LargeChange);
            }
        }

        /// <summary>
        /// Calculates the thumbs sizes.
        /// </summary>
        private void CalculateSize()
        {
            middleThumbSize = ((actualTrackSize) * (((RangeEnd - RangeStart) / (Maximum - Minimum)))) - ((this.ScrollButtonVisibility != Visibility.Collapsed) ? GapSize : 0);
            largeDecreaseThumbSize = (actualTrackSize) * ((RangeStart - Minimum) / (Maximum - Minimum));
            largeIncreaseThumbSize = ((actualTrackSize) * ((Maximum - RangeEnd) / (Maximum - Minimum)));
        }

        /// <summary>
        /// Calculates the thumbs minimum and maximum sizes.
        /// </summary>
        private void ThumbMinMax()
        {
            middleThumbSize = middleThumbSize < MinimumThumbSize ? MinimumThumbSize : ((middleThumbSize > actualLargeThumbSize) ? actualLargeThumbSize : middleThumbSize);
            largeDecreaseThumbSize = largeDecreaseThumbSize <= 0 ? 0 : ((largeDecreaseThumbSize > actualLargeThumbSize) ? actualLargeThumbSize : largeDecreaseThumbSize);
            largeIncreaseThumbSize = largeIncreaseThumbSize <= 0 ? 0 : ((largeIncreaseThumbSize > actualLargeThumbSize) ? actualLargeThumbSize : largeIncreaseThumbSize);
        }

        /// <summary>
        /// Calculates the ranges when thumbs are dragged.
        /// </summary>
        /// <param name="startChange">The Start Range</param>
        /// <param name="endChange">The End Range</param>
        private void CalculateChange(double startChange, double endChange)
        {
            RangeStart += startChange;
            RangeEnd += endChange;
            RangeMinMax();
        }

        /// <summary>
        /// Calculates the minimum and the maximum range.
        /// </summary>
        private void RangeMinMax()
        {
            RangeStart = (RangeStart < Minimum) ? Minimum : (RangeStart > Maximum - (Maximum / 100) && this.ScrollButtonVisibility != Visibility.Collapsed) ? Maximum - (Maximum / 100) : (EnableTouchMode && RangeStart > Maximum - (1.5 * SmallChange)) ? Maximum - (1.5 * SmallChange) : RangeStart;
            RangeEnd = (RangeEnd > Maximum) ? Maximum : ((RangeEnd < Minimum && this.ScrollButtonVisibility == Visibility.Collapsed) ? Minimum : (RangeEnd < (Maximum / 100) && this.ScrollButtonVisibility == Visibility.Visible) ? (Maximum / 100) : RangeEnd);
        }

        /// <summary>
        /// Calculates the large thumb size.
        /// </summary>
        private void SetLargeThumbSize()
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                MiddleThumb.Width = middleThumbSize;
                LargeDecrease.Width = largeDecreaseThumbSize;
                LargeIncrease.Width = largeIncreaseThumbSize;
            }
            else
            {
                MiddleThumb.Height = middleThumbSize;
                LargeDecrease.Height = largeDecreaseThumbSize;
                LargeIncrease.Height = largeIncreaseThumbSize;
            }
        }

        /// <summary>
        /// Calculates the range difference.
        /// </summary>
        private void CalculateRangeDifference()
        {
            if (previousThumbSize != ((this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height))
            {
                previousThumbSize = ((this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height);
                rangeDiff = RangeEnd - RangeStart;
            }
        }

        /// <summary>
        /// Calculates the range when <see cref="ResizableScrollBar"/> is clicked at a range.
        /// </summary>
        /// <param name="startChange">The Start Change</param>
        /// <param name="endChange">The End Change</param>
        private void IncreaseDecreaseOperation(double startChange, double endChange)
        {
            canDrag = true;
            IsValueChangedTrigger = false;
            CalculateRangeDifference();
            CalculateChange(startChange, endChange);
            RangeStart = (RangeStart > Maximum - rangeDiff) ? Maximum - rangeDiff : RangeStart;
            RangeEnd = (RangeEnd < rangeDiff) ? rangeDiff : RangeEnd;
            CalculateOperations();
            OnValueChanged();
        }

        /// <summary>
        /// Calculates the thumb size.
        /// </summary>
        private void CalculateOperations()
        {
            CalculateSize();
            ThumbMinMax();
            SetLargeThumbSize();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Override the ResizableScrollBar for Range navigator thumb selector
    /// </summary>
    public partial class RangeNavigatorSelector : ResizableScrollBar
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="OverlayBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register(
                "OverlayBrush",
                typeof(Brush),
                typeof(ResizableScrollBar),
                new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        #endregion

        #region Properties

        #region Public Properties
        /// <summary>
        /// Gets or sets the overlay brush color
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        #endregion

        #endregion
    }
}