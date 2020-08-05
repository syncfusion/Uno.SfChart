// <copyright file="SfRangeNavigator.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;
#if SyncfusionLicense
    using Syncfusion.Licensing;
#endif
    // The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

    /// <summary>
    /// Represents the <see cref="SfRangeNavigator"/> class.
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class SfRangeNavigator : Control
    {
        #region Dependenc Property Registration

        /// <summary>
        ///  The DependencyProperty for <see cref="ZoomFactor"/> property.
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register(
                "ZoomFactor",
                typeof(double),
                typeof(SfRangeNavigator),
                new PropertyMetadata(1d, OnZoomFactorChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ZoomPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register(
                "ZoomPosition",
                typeof(double),
                typeof(SfRangeNavigator),
                new PropertyMetadata(0d, OnZoomPositionChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ViewRangeStart"/> property.
        /// </summary>
        public static readonly DependencyProperty ViewRangeStartProperty =
            DependencyProperty.Register(
                "ViewRangeStart",
                typeof(object),
                typeof(SfRangeNavigator),
                new PropertyMetadata(null, new PropertyChangedCallback(OnViewRangeStartChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ViewRangeEnd"/> property.
        /// </summary>
        public static readonly DependencyProperty ViewRangeEndProperty =
            DependencyProperty.Register(
                "ViewRangeEnd",
                typeof(object),
                typeof(SfRangeNavigator),
                new PropertyMetadata(null, new PropertyChangedCallback(OnViewRangeEndChanged)));

        /// <summary>
        ///  The DependencyProperty for <see cref="Content"/> property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(SfRangeNavigator),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="OverlayBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register(
                "OverlayBrush",
                typeof(Brush),
                typeof(SfRangeNavigator),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="ScrollbarVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty ScrollbarVisibilityProperty =
            DependencyProperty.Register(
                "ScrollbarVisibility",
                typeof(Visibility),
                typeof(SfRangeNavigator),
                new PropertyMetadata(Visibility.Visible));

        #endregion

        #region Fields
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Already a property with an upper case letter is found.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Already a property with an upper case letter is found.")]
        internal double zoomPosition = 0, zoomFactor = 1;
        
        private bool isViewRangeSet = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SfRangeNavigator"/> class.
        /// </summary>
        public SfRangeNavigator()
        {
#if SyncfusionLicense
#if NETCORE
            Windows.Shared.LicenseHelper.ValidateLicense();
            SfRangeNavigator.ValidateLicense();
#endif
#endif
            DataStart = 0;
            DataEnd = 0;
            this.DefaultStyleKey = typeof(SfRangeNavigator);
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets zoom factor. Value must fall within 0 to 1. It determines delta of visible range.
        /// </summary>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        /// <summary>
        /// Gets or sets zoom position. Value must fall within 0 to 1. It determines starting value of visible range
        /// </summary>
        public double ZoomPosition
        {
            get { return (double)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets Navigator's Start Thumb value, Value can be DateTime if Minimum and Maximum are set as DateTime values.
        /// </summary>
        public object ViewRangeStart
        {
            get { return (object)GetValue(ViewRangeStartProperty); }
            set { SetValue(ViewRangeStartProperty, value); }
        }

        /// <summary>
        /// Gets or sets Navigator's End Thumb value, Value can be DateTime if Minimum and Maximum are set as DateTime values.
        /// </summary>
        public object ViewRangeEnd
        {
            get { return (object)GetValue(ViewRangeEndProperty); }
            set { SetValue(ViewRangeEndProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content that needs to be hosted inside the Navigator, the content can be any UI element.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the overlay brush color.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of the scrollbar.
        /// </summary>
        public Visibility ScrollbarVisibility
        {
            get { return (Visibility)GetValue(ScrollbarVisibilityProperty); }
            set { SetValue(ScrollbarVisibilityProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the x range.
        /// </summary>
        internal double XRange { get; set; }

        /// <summary>
        /// Gets or sets the navigator.
        /// </summary>
        internal ResizableScrollBar Navigator { get; set; }

        /// <summary>
        /// Gets or sets the scrollbar.
        /// </summary>
        internal ResizableScrollBar Scrollbar { get; set; }

        /// <summary>
        /// Gets or sets the data start.
        /// </summary>
        internal double DataStart { get; set; }

        /// <summary>
        /// Gets or sets the data end.
        /// </summary>
        internal double DataEnd { get; set; }

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        internal ObservableCollection<object> Selected { get; set; }

        /// <summary>
        /// Gets or sets the x values.
        /// </summary>
        internal IEnumerable XValues { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        #endregion

        #region Internal Static Methods

#if UNIVERSALWINDOWS && SyncfusionLicense

        internal static async void ValidateLicense()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return;

            var licenseMessage = FusionLicenseProvider.GetLicenseType(Platform.UWP);
            if (!string.IsNullOrEmpty(licenseMessage))
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(licenseMessage);
                dialog.Title = "Syncfusion License";
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK", (action) => { }));
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("HELP", (action) =>
                {
                    OpenHelpLink();

                }));
                await dialog.ShowAsync();
            }
        }

#endif

        #endregion

        #region Internal Methods

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on zoom factor changed.
        /// </summary>
        /// <param name="newValue">The New Value</param>
        /// <param name="oldValue">The Old Value</param>
        internal virtual void OnZoomFactorChanged(double newValue, double oldValue)
        {
            zoomFactor = newValue;
            if (Navigator != null)
            {
                Navigator.IsValueChangedTrigger = false;
                Navigator.RangeEnd = ZoomPosition + newValue;
            }
        }

        /// <summary>
        /// Updates the range navigator on zoom position changed.
        /// </summary>
        /// <param name="newValue">The New Value</param>
        internal virtual void OnZoomPositionChanged(double newValue)
        {
            if (Navigator != null && this is SfRangeNavigator)
            {
                Navigator.IsValueChangedTrigger = false;
                Navigator.RangeEnd = ZoomFactor + newValue;
                Navigator.IsValueChangedTrigger = false;
                Navigator.RangeStart = newValue;
            }

            zoomPosition = newValue;
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on view range end changed.
        /// </summary>
        internal virtual void OnViewRangeEndChanged()
        {
            if (Navigator != null && Navigator.TrackSize != 0)
                if (!(ViewRangeEnd is DateTime))
                {
                    Navigator.IsValueChangedTrigger = false;
                    Navigator.RangeEnd = Convert.ToDouble(ViewRangeEnd);
                }
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on view range start changed.
        /// </summary>
        internal virtual void OnViewRangeStartChanged()
        {
            if (Navigator != null && Navigator.TrackSize != 0)
                if (!(ViewRangeStart is DateTime))
                {
                    Navigator.IsValueChangedTrigger = false;
                    Navigator.RangeStart = Convert.ToDouble(ViewRangeStart);
                }
        }

        /// <summary>
        /// Calculates the range for the selected data.
        /// </summary>
        internal virtual void CalculateSelectedData()
        {
            if (isViewRangeSet || ViewRangeStart == null && ViewRangeEnd == null)
            {
                this.ZoomFactor = (Navigator.RangeEnd - Navigator.RangeStart);
                this.ZoomPosition = Navigator.RangeStart;
                this.ViewRangeEnd = Navigator.RangeEnd;
                this.ViewRangeStart = Navigator.RangeStart;
            }
            else if (Convert.ToDouble(ViewRangeStart) >= Navigator.Minimum && Convert.ToDouble(ViewRangeEnd) <= Navigator.Maximum)
            {
                Navigator.RangeStart = Convert.ToDouble(ViewRangeStart);
                Navigator.RangeEnd = Convert.ToDouble(ViewRangeEnd);
                this.ZoomFactor = (Navigator.RangeEnd - Navigator.RangeStart);
                this.ZoomPosition = Navigator.RangeStart;
            }
            else
            {
                isViewRangeSet = true;
                CalculateSelectedData();
            }

            isViewRangeSet = true;
        }

        #endregion

        #region Protected Properties
        
        /// <summary>
        /// Applies the templates for the control.
        /// </summary>
#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            ResizableScrollBar oldNavigator = null;
            if (Navigator != null)
            {
                Navigator.SizeChanged -= OnTimeLineSizeChanged;
                oldNavigator = Navigator;
            }

            Navigator = this.GetTemplateChild("Part_RangePicker") as ResizableScrollBar;

            if (oldNavigator != null)
            {
                Navigator.RangeStart = oldNavigator.RangeStart;
                Navigator.RangeEnd = oldNavigator.RangeEnd;
            }
            else
            {
                Navigator.RangeStart = this.zoomPosition;
                Navigator.RangeEnd = this.zoomPosition + this.zoomFactor;
            }

            {
                ResizableScrollBar oldScrollBar = null;
                if (Scrollbar != null)
                {
                    Scrollbar.ValueChanged -= OnScrollbarValueChanged;
                    oldScrollBar = Scrollbar;
                }

                Scrollbar = this.GetTemplateChild("Part_Scroll") as ResizableScrollBar;

                if (oldScrollBar != null)
                {
                    Scrollbar.RangeStart = oldScrollBar.RangeStart;
                    Scrollbar.RangeEnd = oldScrollBar.RangeEnd;
                    ClipNavigator(); // Added to reset issue with the rangenavigator on applying themes.
                }

                if (Scrollbar != null)
                {
                    Scrollbar.ValueChanged += OnScrollbarValueChanged;
                }
            }

            if (Navigator != null)
            {
                Navigator.ScrollButtonVisibility = Visibility.Collapsed;
                Navigator.SizeChanged += OnTimeLineSizeChanged;
            }

            Loaded -= OnSfRangeNavigatorLoaded;
            Loaded += OnSfRangeNavigatorLoaded;
            this.SizeChanged -= SfRangeNavigator_SizeChanged;
            this.SizeChanged += SfRangeNavigator_SizeChanged;
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on it's value change.
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on scroll bar value changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        protected virtual void OnScrollbarValueChanged(object sender, EventArgs e)
        {
            ClipNavigator();
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on time line value changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments.</param>
        protected virtual void OnTimeLineValueChanged(object sender, EventArgs e)
        {
            CalculateSelectedData();
            OnValueChanged();
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on time line size changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Argument</param>
        protected virtual void OnTimeLineSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        #endregion

        #region Private Static Methods


#if UNIVERSALWINDOWS && SyncfusionLicense

        private static async void OpenHelpLink()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(@"https://help.syncfusion.com/es/licensing/"));
        }

#endif

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on zoom factor changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfRangeNavigator).OnZoomFactorChanged(Convert.ToDouble(e.NewValue), Convert.ToDouble(e.OldValue));
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on zoom position changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnZoomPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfRangeNavigator).OnZoomPositionChanged(Convert.ToDouble(e.NewValue));
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on view range start changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnViewRangeStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeNavigator = d as SfRangeNavigator;
            if (rangeNavigator != null && e.NewValue != null)
                rangeNavigator.OnViewRangeStartChanged();
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on view range end changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnViewRangeEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeNavigator = d as SfRangeNavigator;
            if (rangeNavigator != null && e.NewValue != null)
                rangeNavigator.OnViewRangeEndChanged();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on size changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void SfRangeNavigator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipNavigator();
        }

        /// <summary>
        /// Updates the <see cref="SfRangeNavigator"/> on loaded.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSfRangeNavigatorLoaded(object sender, RoutedEventArgs e)
        {
            if (Navigator != null)
            {
                Navigator.ValueChanged -= OnTimeLineValueChanged;
                Navigator.ValueChanged += OnTimeLineValueChanged;
                CalculateSelectedData();
            }
        }

        /// <summary>
        /// Clips the <see cref="SfRangeNavigator"/> with the given range.
        /// </summary>
        private void ClipNavigator()
        {
            if (Navigator != null && Navigator.Content != null && Scrollbar != null)
            {
                XRange = -((ActualWidth * Scrollbar.Scale) * Scrollbar.RangeStart);
                Navigator.Width = ActualWidth * Scrollbar.Scale;
                Navigator.Margin = new Thickness(XRange, 0, 0, 0);
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
            }
        }

        #endregion

        #endregion
    }
}
