// <copyright file="SfDateTimeRangeNavigator.cs" company="Syncfusion. Inc">
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
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Windows;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    // The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235
    /// <summary>
    /// Represents the <see cref="SfDateTimeRangeNavigator"/> class.
    /// </summary>
    public partial class SfDateTimeRangeNavigator : SfRangeNavigator
    {
        #region Dependency Property Registration

        /// <summary>
        ///  The DependencyProperty for <see cref="LeftThumbStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LeftThumbStyleProperty =
            DependencyProperty.Register(
                "LeftThumbStyle",
                typeof(ThumbStyle),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnThumbLineStyleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="RightThumbStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty RightThumbStyleProperty =
            DependencyProperty.Register(
                "RightThumbStyle",
                typeof(ThumbStyle),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnThumbLineStyleChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="HigherBarTickLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty HigherBarTickLineStyleProperty =
            DependencyProperty.Register(
                "HigherBarTickLineStyle",
                typeof(Style),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnTickLineStyleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HigherBarGridLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty HigherBarGridLineStyleProperty =
            DependencyProperty.Register(
                "HigherBarGridLineStyle",
                typeof(Style),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnGridLineStyleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LowerBarTickLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LowerBarTickLineStyleProperty =
            DependencyProperty.Register(
                "LowerBarTickLineStyle",
                typeof(Style),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnTickLineStyleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LowerBarGridLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LowerBarGridLineStyleProperty =
            DependencyProperty.Register(
                "LowerBarGridLineStyle",
                typeof(Style),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnGridLineStyleChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="EnableDeferredUpdate"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableDeferredUpdateProperty =
            DependencyProperty.Register(
                "EnableDeferredUpdate",
                typeof(bool),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(false, OnEnableDeferredUpdatePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="DeferredUpdateDelay"/> property
        /// </summary>
        public static readonly DependencyProperty DeferredUpdateDurationProperty =
            DependencyProperty.Register(
                "DeferredUpdateDuration",
                typeof(double),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(0.5, OnDeferredUpdateDurationPropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="Intervals"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalsProperty =
            DependencyProperty.Register(
                "Intervals",
                typeof(Intervals),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnIntervalChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(object),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(DateTime.MinValue, OnMinimumMaximumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(object),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(DateTime.MinValue, OnMinimumMaximumChanged));

        /// <summary>
        ///  Identifies the ItemsSource dependency property.
        ///  The DependencyProperty for <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(SfDateTimeRangeNavigator), 
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemSourceChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ShowToolTip"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowToolTipProperty =
            DependencyProperty.Register(
                "ShowToolTip",
                typeof(bool),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(false, OnShowToolTipChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LeftToolTipTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty LeftToolTipTemplateProperty =
            DependencyProperty.Register(
                "LeftToolTipTemplate",
                typeof(DataTemplate),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnLeftToolTipTemplateChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="RightToolTipTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty RightToolTipTemplateProperty =
            DependencyProperty.Register(
                "RightToolTipTemplate",
                typeof(DataTemplate),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnRightToolTipTemplateChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="ToolTipLabelFormat"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolTipLabelFormatProperty =
            DependencyProperty.Register(
                "ToolTipLabelFormat",
                typeof(string),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata("yyyy/MMM/dd", OnToolTipFormatChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="XBindingPath"/> property.
        /// </summary>
        public static readonly DependencyProperty XBindingPathProperty =
            DependencyProperty.Register(
                "XBindingPath",
                typeof(string),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(string.Empty, OnXBindingPathChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="LowerLevelBarStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LowerLevelBarStyleProperty =
            DependencyProperty.Register(
                "LowerLevelBarStyle",
                typeof(LabelBarStyle),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnLabelStyleChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="HigherLevelBarStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty HigherLevelBarStyleProperty =
            DependencyProperty.Register(
                "HigherLevelBarStyle",
                typeof(LabelBarStyle),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnLabelStyleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HigherLabelStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty HigherLabelStyleProperty =
            DependencyProperty.Register(
                "HigherLabelStyle",
                typeof(Style),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnLabelStyleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LowerLabelStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LowerLabelStyleProperty =
            DependencyProperty.Register(
                "LowerLabelStyle",
                typeof(Style),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null, OnLabelStyleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ShowGridLines"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register(
                "ShowGridLines",
                typeof(bool),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(true, OnShowGridlinesChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="RangePadding"/> property.
        /// </summary>
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register(
                "RangePadding",
                typeof(NavigatorRangePadding),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(NavigatorRangePadding.Round));

        /// <summary>
        /// The DependencyProperty for <see cref="LowerBarVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty LowerBarVisibilityProperty =
            DependencyProperty.Register(
                "LowerBarVisibility",
                typeof(Visibility),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(Visibility.Visible, OnLowerBarVisibilityChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="UpperLabelBarVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty HigherBarVisibilityProperty =
            DependencyProperty.Register(
                "HigherBarVisibility",
                typeof(Visibility),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(Visibility.Visible, OnHigherBarVisibilityChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedData"/> property.
        /// </summary>
        internal static readonly DependencyProperty SelectedDataProperty =
            DependencyProperty.Register(
                "SelectedData",
                typeof(object),
                typeof(SfDateTimeRangeNavigator),
                new PropertyMetadata(null));

        #endregion

        #region Fields

        private bool isRightSet = false;

        private bool isUpdate = false;

        private Panel hover;

        private Panel tool;

        private UIElementsRecycler<TextBlock> upperLabelRecycler;

        private DispatcherTimer timer;

        private UIElementsRecycler<TextBlock> lowerLabelRecycler;

        private UIElementsRecycler<Line> upperTickLineRecycler;

        private UIElementsRecycler<Line> lowerGridLineRecycler;

        private UIElementsRecycler<Line> lowerTickLineRecycler;

        private UIElementsRecycler<Line> upperGridLineRecycler;

        private DataTemplate leftTemplate;

        private DataTemplate rightTemplate;

        private Panel navigatorPanel;

        private Border uppperBorder;

        private Border lowerBorder;

        private Panel upperLabelBar;

        private Panel lowerLabelBar;

        private Panel lowerLineBar;

        private Panel upperLineBar;

        private Rect[] labelElementBounds;

        private ObservableCollection<double> lowerLabelBounds;

        private ObservableCollection<double> upperLabelBounds;

        private ObservableCollection<ChartAxisLabel> upperBarLabels;

        private ObservableCollection<ChartAxisLabel> lowerBarLabels;

        private ObservableCollection<string> navigatorIntervals;

        private DateTime maximumDateTimeValue = DateTime.MinValue;

        private DateTime minimumDateTimeValue = DateTime.MinValue;

        private double totalNoofDays;

        private Line leftThumbLine, rightThumbLine;

        private ContentPresenter leftThumbSymbol, rightThumbSymbol;

        private bool isMinMaxSet;

        private ObservableCollection<double> daysvalue = new ObservableCollection<double>();

        private double txtblockwidth;

        private List<ObservableCollection<string>> formatters;

        private bool isLeftButtonPressed;

        private bool isDragged;

        private IAsyncAction updateAction;

        private Thumb leftThumb, rightThumb;

        private bool isFormatterEmpty;

        private Panel innerGridlines;

        private string dockPosition = "Lower";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SfDateTimeRangeNavigator"/> class.
        /// </summary>
        public SfDateTimeRangeNavigator()
        {
            this.DefaultStyleKey = typeof(SfDateTimeRangeNavigator);
            upperBarLabels = new ObservableCollection<ChartAxisLabel>();
            lowerBarLabels = new ObservableCollection<ChartAxisLabel>();
            this.Loaded += OnSfDateTimeRangeNavigatorLoaded;
            Intervals = new Intervals();
            Intervals.CollectionChanged += OnIntervalsCollectionChanged;
        }

        #endregion

        #region Event

        /// <summary>
        /// Occurs when the lower bar labels are created.
        /// </summary>
        public event EventHandler<LowerBarLabelsCreatedEventArgs> LowerBarLabelsCreated;

        /// <summary>
        /// Occurs when the upper bar labels are created.
        /// </summary>
        public event EventHandler<HigherBarLabelsCreatedEventArgs> HigherBarLabelsCreated;
     
        #endregion

        #region Properties

        #region Public Properties

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed. Suppression is OK here")]
        internal ResourceManager ResourceManager
        {
            set
            {
                SR.ResourceManager = value;
            }
        }

        /// <summary>
        /// Gets or sets the thumb style for left thumb.
        /// </summary>
        public ThumbStyle LeftThumbStyle
        {
            get { return (ThumbStyle)GetValue(LeftThumbStyleProperty); }
            set { SetValue(LeftThumbStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the right thumb style.
        /// </summary>
        public ThumbStyle RightThumbStyle
        {
            get { return (ThumbStyle)GetValue(RightThumbStyleProperty); }
            set { SetValue(RightThumbStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for tick lines inside the upper bar.
        /// </summary>
        public Style HigherBarTickLineStyle
        {
            get { return (Style)GetValue(HigherBarTickLineStyleProperty); }
            set { SetValue(HigherBarTickLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for upper bar gridlines.
        /// </summary>
        public Style HigherBarGridLineStyle
        {
            get { return (Style)GetValue(HigherBarGridLineStyleProperty); }
            set { SetValue(HigherBarGridLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for tick lines in lower bar.
        /// </summary>
        public Style LowerBarTickLineStyle
        {
            get { return (Style)GetValue(LowerBarTickLineStyleProperty); }
            set { SetValue(LowerBarTickLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for lower bar gridlines.
        /// </summary>
        public Style LowerBarGridLineStyle
        {
            get { return (Style)GetValue(LowerBarGridLineStyleProperty); }
            set { SetValue(LowerBarGridLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to defer the ValueChanged notification. 
        /// </summary>
        public bool EnableDeferredUpdate
        {
            get { return (bool)GetValue(EnableDeferredUpdateProperty); }
            set { SetValue(EnableDeferredUpdateProperty, value); }
        }

        /// <summary>
        /// Gets or sets double interval value to reset the timer when EnableDeferredUpdate is true
        /// </summary>
        public double DeferredUpdateDelay
        {
            get { return (double)GetValue(DeferredUpdateDurationProperty); }
            set { SetValue(DeferredUpdateDurationProperty, value); }
        }

        /// <summary>
        /// Gets or sets intervals collection to render labels of <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        public Intervals Intervals
        {
            get { return (Intervals)GetValue(IntervalsProperty); }
            set { SetValue(IntervalsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Minimum Starting Range of the <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        public object Minimum
        {
            get { return (object)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Maximum Ending Range of the <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        public object Maximum
        {
            get { return (object)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets an IEnumerable source used to render range.
        /// </summary>
        /// <value>The DataSource value.</value>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }

            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
      
        /// <summary>
        /// Gets an IEnumerable source for the particular selected region
        /// </summary>
        public object SelectedData
        {
            get { return (object)GetValue(SelectedDataProperty); }
            private set { SetValue(SelectedDataProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show ToolTip.
        /// </summary>
        public bool ShowToolTip
        {
            get { return (bool)GetValue(ShowToolTipProperty); }
            set { SetValue(ShowToolTipProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value for indicating whether the visibility of the lower label bar.
        /// </summary>
        public Visibility LowerBarVisibility
        {
            get { return (Visibility)GetValue(LowerBarVisibilityProperty); }
            set { SetValue(LowerBarVisibilityProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value for indicating whether the visibility of the upper label bar.
        /// </summary>
        public Visibility HigherBarVisibility
        {
            get { return (Visibility)GetValue(HigherBarVisibilityProperty); }
            set { SetValue(HigherBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets template for the left side ToolTip.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate LeftToolTipTemplate
        {
            get { return (DataTemplate)GetValue(LeftToolTipTemplateProperty); }
            set { SetValue(LeftToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets template for the right side ToolTip.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate RightToolTipTemplate
        {
            get { return (DataTemplate)GetValue(RightToolTipTemplateProperty); }
            set { SetValue(RightToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets label format for ToolTip.
        /// </summary>
        public string ToolTipLabelFormat
        {
            get { return (string)GetValue(ToolTipLabelFormatProperty); }
            set { SetValue(ToolTipLabelFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property path of the x data in ItemsSource.
        /// </summary>
        public string XBindingPath
        {
            get { return (string)GetValue(XBindingPathProperty); }
            set { SetValue(XBindingPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the styles for the lower label bar of <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        public LabelBarStyle LowerLevelBarStyle
        {
            get { return (LabelBarStyle)GetValue(LowerLevelBarStyleProperty); }
            set { SetValue(LowerLevelBarStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the styles for the higher label bar of <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        public LabelBarStyle HigherLevelBarStyle
        {
            get { return (LabelBarStyle)GetValue(HigherLevelBarStyleProperty); }
            set { SetValue(HigherLevelBarStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the higher label style.
        /// </summary>
        /// <value>
        /// The higher label style.
        /// </value>
        public Style HigherLabelStyle
        {
            get { return (Style)GetValue(HigherLabelStyleProperty); }
            set { SetValue(HigherLabelStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the lower label style.
        /// </summary>
        /// <value>
        /// The lower label style.
        /// </value>
        public Style LowerLabelStyle
        {
            get { return (Style)GetValue(LowerLabelStyleProperty); }
            set { SetValue(LowerLabelStyleProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to show grid lines inside the content.
        /// </summary>
        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        /// <summary>
        /// Gets or sets <see cref="NavigatorRangePadding"/> to shift the <see cref="SfDateTimeRangeNavigator"/> axis range inside or outside.
        /// </summary>
        public NavigatorRangePadding RangePadding
        {
            get { return (NavigatorRangePadding)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Schedule the <see cref="SfDateTimeRangeNavigator"/> update.
        /// </summary>
        internal void Scheduleupdate()
        {
            if (updateAction == null)
                updateAction = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Update);
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        internal void Update()
        {
            if (minimumDateTimeValue == DateTime.MinValue || maximumDateTimeValue == DateTime.MinValue) return;
            navigatorIntervals = null;
            formatters = null;
            navigatorIntervals = new ObservableCollection<string>();
            formatters = new List<ObservableCollection<string>>();
            string currentdockPos = "Upper";
            if (isMinMaxSet)
            {
                if (Navigator != null)
                    Navigator.IsValueChangedTrigger = true;

                bool isLowerGenerated = false;
                if (Intervals != null && Intervals.Count > 0 && upperLabelBar != null && lowerLabelBar != null)
                {
                    foreach (Interval navinterval in Intervals)
                    {
                        navigatorIntervals.Add(navinterval.IntervalType.ToString());
                        formatters.Add(navinterval.LabelFormatters);
                    }

                    if (navigatorIntervals.Contains("Year"))
                    {
                        SetYearInterval(0, currentdockPos, formatters[navigatorIntervals.IndexOf("Year")]);
                        currentdockPos = "Lower";
                    }

                    if (navigatorIntervals.Contains("Quarter"))
                    {
                        SetQuarterInterval(0, currentdockPos, formatters[navigatorIntervals.IndexOf("Quarter")]);
                        if (currentdockPos == "Lower")
                            isLowerGenerated = true;
                        else
                            currentdockPos = "Lower";
                    }

                    if (navigatorIntervals.Contains("Month") && !isLowerGenerated)
                    {
                        SetMonthInterval(0, currentdockPos, formatters[navigatorIntervals.IndexOf("Month")]);
                        if (currentdockPos == "Lower")
                            isLowerGenerated = true;
                        else
                            currentdockPos = "Lower";
                    }

                    if (navigatorIntervals.Contains("Week") && !isLowerGenerated)
                    {
                        SetWeekInterval(0, currentdockPos, formatters[navigatorIntervals.IndexOf("Week")]);
                        if (currentdockPos == "Lower")
                            isLowerGenerated = true;
                        else
                            currentdockPos = "Lower";
                    }

                    if (navigatorIntervals.Contains("Day") && !isLowerGenerated)
                    {
                        SetDayInterval(0, currentdockPos, formatters[navigatorIntervals.IndexOf("Day")]);
                        if (currentdockPos == "Lower")
                            isLowerGenerated = true;
                        else
                            currentdockPos = "Lower";
                    }

                    if (navigatorIntervals.Contains("Hour") && !isLowerGenerated)
                    {
                        SetHourInterval(0, currentdockPos, formatters[navigatorIntervals.IndexOf("Hour")]);
                        if (currentdockPos == "Lower")
                            isLowerGenerated = true;
                        else
                            currentdockPos = "Lower";
                    }

                    if (upperLabelRecycler.Count == 0 && upperLabelBar != null)
                        SetYearInterval(0, "Upper", null);
                    if (lowerLabelBar != null && !isLowerGenerated)
                    {
                        if (navigatorIntervals.Contains("Year"))
                            SetYearInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Year")]);
                        if (navigatorIntervals.Contains("Quarter"))
                            SetQuarterInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Quarter")]);
                        if (navigatorIntervals.Contains("Month"))
                            SetMonthInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Month")]);
                        if (navigatorIntervals.Contains("Week"))
                            SetWeekInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Week")]);
                        if (navigatorIntervals.Contains("Day"))
                            SetDayInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Day")]);
                        if (navigatorIntervals.Contains("Hour"))
                            SetHourInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Hour")]);
                    }
                }
                else
                {
                    if (upperLabelBar != null)
                        SetYearInterval(0, "Upper", null);
                    if (lowerLabelBar != null)
                        SetQuarterInterval(0, dockPosition, null);
                }
            }

            UpdateTooltip();
            updateAction = null;
        }

        /// <summary>
        /// Sets the thumb style.
        /// </summary>
        internal void SetThumbStyle()
        {
            if (LeftThumbStyle != null)
            {
                if (leftThumbLine != null && LeftThumbStyle.LineStyle != null)
                    leftThumbLine.Style = LeftThumbStyle.LineStyle;
                if (leftThumbSymbol != null && LeftThumbStyle.SymbolTemplate != null)
                    leftThumbSymbol.ContentTemplate = LeftThumbStyle.SymbolTemplate;
            }

            if (RightThumbStyle != null)
            {
                if (rightThumbLine != null && RightThumbStyle.LineStyle != null)
                    rightThumbLine.Style = RightThumbStyle.LineStyle;
                if (rightThumbSymbol != null && RightThumbStyle.SymbolTemplate != null)
                    rightThumbSymbol.ContentTemplate = RightThumbStyle.SymbolTemplate;
            }

            if (leftThumb != null)
            {
                leftThumbLine.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                leftThumb.Width = leftThumbLine.DesiredSize.Width;
            }

            if (rightThumb != null)
            {
                rightThumbLine.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                rightThumb.Width = rightThumbLine.DesiredSize.Width;
            }

            if (leftThumbSymbol != null && LeftThumbStyle != null)
            {
                if (LeftThumbStyle.SymbolTemplate != null)
                {
                    leftThumbSymbol.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    leftThumbSymbol.Margin = new Thickness(-leftThumbSymbol.DesiredSize.Width - 15, 0, -leftThumbSymbol.DesiredSize.Width - 15, 0);
                }
                else
                {
                    leftThumbSymbol.ClearValue(ContentPresenter.MarginProperty);
                }
            }

            if (rightThumbSymbol != null && RightThumbStyle != null)
            {
                if (RightThumbStyle.SymbolTemplate != null)
                {
                    rightThumbSymbol.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    rightThumbSymbol.Margin = new Thickness(-rightThumbSymbol.DesiredSize.Width - 15, 0, -rightThumbSymbol.DesiredSize.Width - 15, 0);
                }
                else
                {
                    rightThumbSymbol.ClearValue(ContentPresenter.MarginProperty);
                }
            }
        }

        /// <summary>
        /// Generates the data points.
        /// </summary>
        internal void GeneratePoints()
        {
            bool isPointsGenerated = false;
            if (this.ItemsSource != null && !string.IsNullOrEmpty(this.XBindingPath))
            {
                GeneratePropertyPoints();
                isPointsGenerated = true;
            }

            if (XValues != null && XValues is IList<DateTime> && (XValues as IList<DateTime>).Count != 0 && isPointsGenerated)
            {
                if (RangePadding == NavigatorRangePadding.Round)
                {
                    maximumDateTimeValue =
                        (XValues as IList<DateTime>).Max().AddHours(new TimeSpan(0, 23, 59, 59).TotalHours -
                                                                    (XValues as IList<DateTime>).Max().Hour);

                    minimumDateTimeValue =
                        (XValues as IList<DateTime>).Min().AddHours(new TimeSpan(0, 0, 00, 01).TotalHours -
                                                                    (XValues as IList<DateTime>).Min().Hour);
                }
                else
                {
                    maximumDateTimeValue = (XValues as IList<DateTime>).Max();
                    minimumDateTimeValue = (XValues as IList<DateTime>).Min();
                }
            }
            else if (Convert.ToString(Minimum) != "0" && Convert.ToString(Maximum) != "1")
            {
                maximumDateTimeValue = Convert.ToDateTime(Maximum, CultureInfo.InvariantCulture);
                minimumDateTimeValue = Convert.ToDateTime(Minimum, CultureInfo.InvariantCulture);
            }

            if (maximumDateTimeValue != DateTime.MinValue && minimumDateTimeValue != DateTime.MinValue)
            {
                isMinMaxSet = true;
                totalNoofDays = (maximumDateTimeValue.ToOADate() - minimumDateTimeValue.ToOADate()) == 0 ? 1 : (maximumDateTimeValue.ToOADate() - minimumDateTimeValue.ToOADate());
                if ((upperLabelRecycler != null && upperLabelRecycler.Count > 0) || (lowerLabelRecycler != null && lowerLabelRecycler.Count > 0))
                    CalculateSelectedData();
            }
            else
            {
                isMinMaxSet = false;
                totalNoofDays = 0;
            }

            if (Navigator != null && (upperLabelBar != null && Navigator.TrackSize != 0) || (lowerLabelBar != null && Navigator.TrackSize != 0))
                Update();
        }

        /// <summary>
        /// Gets the selected data.
        /// </summary>
        internal override void CalculateSelectedData()
        {
            Selected = new ObservableCollection<object>();
            if (Navigator != null)
            {
                Navigator.IsValueChangedTrigger = false;
                if (this.ItemsSource != null)
                {
                    var itemSource = this.ItemsSource.GetEnumerator();
                    var dateTime = XValues.GetEnumerator();
                    while (itemSource.MoveNext() && dateTime.MoveNext())
                    {
                        if ((DateTime)dateTime.Current >= Convert.ToDateTime(ViewRangeStart) &&
                            (DateTime)dateTime.Current <= Convert.ToDateTime(ViewRangeEnd))
                        {
                            Selected.Add(itemSource.Current);
                        }
                    }

                    this.SelectedData = Selected;
                }

                Navigator.IsValueChangedTrigger = false;
                this.ZoomFactor = Navigator.RangeEnd - Navigator.RangeStart;
                Navigator.IsValueChangedTrigger = false;
                this.ZoomPosition = Navigator.RangeStart;
                isUpdate = false;
            }

            SetSelectedDataStyle();
        }

        /// <summary>
        /// Sets the label position.
        /// </summary>
        internal void SetLabelPosition()
        {
            if (navigatorPanel != null)
            {
                if (HigherLevelBarStyle != null && HigherLevelBarStyle.Position == BarPosition.Inside)
                {
                    uppperBorder.VerticalAlignment = VerticalAlignment.Top;
                    Grid.SetRow(uppperBorder, 1);
                    uppperBorder.Background = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    Grid.SetRow(uppperBorder, 0);
                    uppperBorder.Background = HigherLevelBarStyle.Background;
                }

                if (LowerLevelBarStyle != null && LowerLevelBarStyle.Position == BarPosition.Inside)
                {
                    lowerBorder.VerticalAlignment = VerticalAlignment.Bottom;
                    Grid.SetRow(lowerBorder, 1);
                    lowerBorder.Background = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    lowerBorder.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(lowerBorder, 2);
                    lowerBorder.Background = LowerLevelBarStyle.Background;
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on view range start changed.
        /// </summary>
        internal override void OnViewRangeStartChanged()
        {
            if (!(ViewRangeStart is double) && Navigator != null && Navigator.TrackSize != 0 && totalNoofDays != 0d)
            {
                var noofdays = (Convert.ToDateTime(ViewRangeStart) - minimumDateTimeValue).TotalDays;
                var onedayInterval = Navigator.TrackSize / totalNoofDays;
                var startleft = noofdays * onedayInterval;
                var startrange = 1 * (startleft / (Navigator.TrackSize));
                Navigator.IsValueChangedTrigger = false;
                Navigator.RangeStart = startrange < 0 ? 0 : startrange;
            }

            CalculateSelectedData();
            UpdateTooltip();
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on view range end changed.
        /// </summary>
        internal override void OnViewRangeEndChanged()
        {
            if (!(ViewRangeEnd is double) && Navigator != null && Navigator.TrackSize != 0 && totalNoofDays != 0d)
            {
                var noofdays = (Convert.ToDateTime(ViewRangeEnd) - this.minimumDateTimeValue).TotalDays;
                var onedayInterval = (Navigator.TrackSize / this.totalNoofDays);
                var endright = noofdays * onedayInterval;
                var endrange = 1 * (endright / (Navigator.TrackSize));
                Navigator.IsValueChangedTrigger = false;
                Navigator.RangeEnd = endrange > 1 ? 1 : endrange;
            }

            CalculateSelectedData();
            UpdateTooltip();
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on zoom factor changed.
        /// </summary>
        /// <param name="newValue">The New Value</param>
        /// <param name="oldValue">The Old Value</param>
        internal override void OnZoomFactorChanged(double newValue, double oldValue)
        {
            this.zoomFactor = newValue;
            if (Navigator == null)
            {
                base.OnZoomFactorChanged(newValue, oldValue);
            }
            else if (Navigator != null)
            {
                isUpdate = true;
                Navigator.IsValueChangedTrigger = false;

                // WPF-22553 Intially the DateTime RangeNavigator is not reset. So the newValue != oldValue is checked.
                if ((Navigator.RangeEnd != Navigator.Maximum && Navigator.isFarDragged) || (newValue != oldValue))
                {
                    Navigator.RangeEnd = ZoomPosition + newValue;
                }

                isUpdate = false;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on zoom position changed.
        /// </summary>
        /// <param name="newValue">The New Value</param>
        internal override void OnZoomPositionChanged(double newValue)
        {
            this.zoomPosition = newValue;
            if (Navigator != null)
            {
                Navigator.IsValueChangedTrigger = false;
                Navigator.RangeEnd = ZoomFactor + newValue;
                Navigator.IsValueChangedTrigger = false;
                Navigator.RangeStart = newValue;
                UpdateTooltip();
                ChangeViewRange();
            }
            else if (Navigator == null)
            {
                base.OnZoomPositionChanged(newValue);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. 
        /// In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            navigatorPanel = this.GetTemplateChild("PART_RangeNavigatorPanel") as Grid;
#if WINDOWS_UAP && CHECKLATER
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                if (navigatorPanel != null)
                {
                    navigatorPanel.Children.Remove(this.GetTemplateChild("Part_Scroll") as ResizableScrollBar);
                }
            }
#endif
            uppperBorder = this.GetTemplateChild("Part_UpperBorder") as Border;
            lowerBorder = this.GetTemplateChild("Part_Border") as Border;
            upperLabelBar = this.GetTemplateChild("PART_UPPERBAR") as Panel;
            upperLineBar = this.GetTemplateChild("PART_UPPERLINE") as Panel;
            lowerLabelBar = this.GetTemplateChild("PART_LOWERBAR") as Panel;
            lowerLineBar = this.GetTemplateChild("PART_LOWERLINE") as Panel;
            hover = this.GetTemplateChild("Part_Hover") as Panel;
            tool = this.GetTemplateChild("Part_Tooltip") as Panel;
            innerGridlines = this.GetTemplateChild("Part_Content_line") as Panel;
            //// Update the visibility of upper label bar
            this.UpdateHigherBarVisibility();
            //// Update the visibility of lower label bar
            this.UpdateLowerBarVisibility();
            this.leftTemplate = ChartDictionaries.GenericCommonDictionary["leftTooltipTemplate"] as DataTemplate;
            this.rightTemplate = ChartDictionaries.GenericCommonDictionary["rightTooltipTemplate"] as DataTemplate;
            if (upperLabelBar != null)
                upperLabelRecycler = new UIElementsRecycler<TextBlock>(upperLabelBar);
            if (lowerLabelBar != null)
                lowerLabelRecycler = new UIElementsRecycler<TextBlock>(lowerLabelBar);
            if (innerGridlines != null)
            {
                lowerGridLineRecycler = new UIElementsRecycler<Line>(innerGridlines);
                upperGridLineRecycler = new UIElementsRecycler<Line>(innerGridlines);
            }

            if (upperLineBar != null)
                upperTickLineRecycler = new UIElementsRecycler<Line>(upperLineBar);
            if (lowerLineBar != null)
                lowerTickLineRecycler = new UIElementsRecycler<Line>(lowerLineBar);

            if (upperLabelBar != null)
            {
                upperLabelBar.AddHandler(PointerMovedEvent, new PointerEventHandler(UpperLabelBar_PointerMoved), true);
                upperLabelBar.AddHandler(PointerExitedEvent, new PointerEventHandler(UpperLabelBar_PointerExited), true);
                upperLabelBar.AddHandler(PointerPressedEvent, new PointerEventHandler(UpperLabelBar_PointerPressed), true);
            }

            if (lowerLabelBar != null)
            {
                lowerLabelBar.PointerMoved += LowerLabelBar_PointerMoved;
                lowerLabelBar.PointerExited += LowerLabelBar_PointerExited;
                lowerLabelBar.PointerPressed += LowerLabelBar_PointerPressed;
            }

            this.AddHandler(PointerPressedEvent, new PointerEventHandler(DateTimeRangeNavigator_PointerPressed), true);

            if (uppperBorder != null)
                Canvas.SetZIndex(uppperBorder, 1);
            if (HigherLevelBarStyle != null)
                HigherLevelBarStyle.DateTimeRangeNavigator = this;
            if (LowerLevelBarStyle != null)
                LowerLevelBarStyle.DateTimeRangeNavigator = this;
            SetLabelPosition();
            GeneratePoints();
            UpdateTooltipVisibility();
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on data source changed.
        /// </summary>
        /// <param name="args">The Event Arguments.</param>
        protected void OnDataSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            SetDefaultRange();
            CalculateRange();
            Refresh();
            UpdateTooltip();

            if (args.OldValue is INotifyCollectionChanged)
            {
                (args.OldValue as INotifyCollectionChanged).CollectionChanged -= OnSfRangeNavigatorCollectionChanged;
            }

            if (args.NewValue is INotifyCollectionChanged)
            {
                (args.NewValue as INotifyCollectionChanged).CollectionChanged += OnSfRangeNavigatorCollectionChanged;
            }
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> pointer released operations.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (EnableDeferredUpdate)
            {
                if (isLeftButtonPressed && isDragged)
                {
                    OnInternalValueChanged();
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Tick -= OnTimeout;
                    }

                    timer = null;
                }

                isLeftButtonPressed = false;
                isDragged = false;
            }         
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> pointer moved operations.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (EnableDeferredUpdate && isLeftButtonPressed)
                isDragged = true;
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on scroll bar value changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        protected override void OnScrollbarValueChanged(object sender, EventArgs e)
        {
            base.OnScrollbarValueChanged(sender, e);
            if (Navigator.Content != null)
            {
                double newmargin = XRange + Navigator.ResizableThumbSize;
                double size = XRange == 0 ? Navigator.DesiredSize.Width : Navigator.TrackSize;
                if (!Scrollbar.isFarDragged)
                {
                    if (tool != null)
                    {
                        tool.Width = size * Scrollbar.Scale;
                        tool.Margin = new Thickness(newmargin, 0, 0, 0);
                    }

                    upperLabelBar.Width = size * Scrollbar.Scale;
                    upperLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    upperLineBar.Width = size * Scrollbar.Scale;
                    upperLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    lowerLabelBar.Width = size * Scrollbar.Scale;
                    lowerLineBar.Width = size * Scrollbar.Scale;
                    lowerLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    lowerLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                }
                else if (!Scrollbar.isNearDragged && XRange != 0)
                {
                    if (tool != null)
                    {
                        tool.Width = Navigator.TrackSize * Scrollbar.Scale;
                        tool.Margin = new Thickness(newmargin, 0, 0, 0);
                    }

                    upperLabelBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                    upperLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    upperLineBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                    upperLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    lowerLabelBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                    lowerLineBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                    lowerLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    lowerLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                }

                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
                SetThumbStyle();
            }

            UpdateTooltip();
        }
        
        /// <summary>
        /// Updates the tooltip on time line value changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        protected override void OnTimeLineValueChanged(object sender, EventArgs e)
        {
            UpdateTooltip();

            if (!EnableDeferredUpdate)
            {
                OnInternalValueChanged();
            }
            else
            {
                ResetTimer();
            }
        }
        
        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> interval on time line changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        protected override void OnTimeLineSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Navigator != null)
                Update();
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on it's value change.
        /// </summary>
        protected override void OnValueChanged()
        {
            ChangeViewRange();
            base.OnValueChanged();
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Updates the tooltip when the left tooltip template changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnLeftToolTipTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeRangeNavigator navigator = (d as SfDateTimeRangeNavigator);
            if (navigator != null && navigator.tool != null && navigator.ShowToolTip &&
                navigator.tool.Children.Count > 0)
            {
                (navigator.tool.Children[0] as ContentControl).ContentTemplate = (DataTemplate)e.NewValue;
                navigator.UpdateTooltip();
            }
        }
        
        /// <summary>
        /// Updates the thumb style when it is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnThumbLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var navigator = d as SfDateTimeRangeNavigator;
            if (navigator != null)
                navigator.ThumbStyleChanged(navigator, e);
        }

        /// <summary>
        /// Updates the gridlines when it's style changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnGridLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var navigator = d as SfDateTimeRangeNavigator;
            if (navigator != null && (navigator.lowerGridLineRecycler != null || navigator.upperGridLineRecycler != null))
                navigator.Scheduleupdate();
        }

        /// <summary>
        /// Updates the tick lines when it's style changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnTickLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var navigator = d as SfDateTimeRangeNavigator;
            if (navigator != null && (navigator.upperTickLineRecycler != null || navigator.lowerTickLineRecycler != null))
                navigator.Scheduleupdate();
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> when the deferred update property is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnEnableDeferredUpdatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTImeRangeNavigator = d as SfDateTimeRangeNavigator;
            if (dateTImeRangeNavigator != null)
                dateTImeRangeNavigator.Scheduleupdate();
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on deferred update duration changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnDeferredUpdateDurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimeRangeNavigator = d as SfDateTimeRangeNavigator;
            if (dateTimeRangeNavigator != null)
                dateTimeRangeNavigator.Scheduleupdate();
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> when the interval property is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeRangeNavigator rangeNavigator = (d as SfDateTimeRangeNavigator);
            if (rangeNavigator != null)
            {
                if (e.OldValue != null)
                    (e.OldValue as Intervals).CollectionChanged -= rangeNavigator.OnIntervalsCollectionChanged;
                (e.NewValue as Intervals).CollectionChanged += rangeNavigator.OnIntervalsCollectionChanged;
                if ((rangeNavigator.upperLabelBar != null && rangeNavigator.Navigator.TrackSize != 0) || (rangeNavigator.lowerLabelBar != null && rangeNavigator.Navigator.TrackSize != 0))
                    rangeNavigator.Scheduleupdate();
            }
        }

        /// <summary>
        /// Updates the range when the maximum or the minimum value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Argument</param>
        private static void OnMinimumMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var dateTimeRangeNavigator = d as SfDateTimeRangeNavigator;
            if (dateTimeRangeNavigator != null)
                dateTimeRangeNavigator.OnMinimumMaximumChanged();
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on items source changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimeRangeNavigator = d as SfDateTimeRangeNavigator;
            if (e.NewValue == null)
            {
                dateTimeRangeNavigator.ClearLabels();
                dateTimeRangeNavigator.SetDefaultRange();
            }
            else
                dateTimeRangeNavigator.OnDataSourceChanged(e);
        }

        /// <summary>
        /// Updates the tooltip visibility.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnShowToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDateTimeRangeNavigator).UpdateTooltipVisibility();
        }

        /// <summary>
        /// Updates the right tooltip when it's template changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnRightToolTipTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfDateTimeRangeNavigator navigator = (d as SfDateTimeRangeNavigator);
            if (navigator != null && navigator.tool != null && navigator.ShowToolTip && navigator.tool.Children.Count > 0)
            {
                (navigator.tool.Children[1] as ContentControl).ContentTemplate = (DataTemplate)e.NewValue;
                navigator.UpdateTooltip();
            }
        }

        /// <summary>
        /// Updates the tooltip when it's format changed
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnToolTipFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDateTimeRangeNavigator).UpdateTooltip();
        }

        /// <summary>
        /// Generates the points when the x binding path changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnXBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            SfDateTimeRangeNavigator navigator = d as SfDateTimeRangeNavigator;
            if (navigator.ItemsSource != null)
                navigator.GeneratePoints();
        }

        /// <summary>
        /// Updates the label when it's style is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnLabelStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var navigator = d as SfDateTimeRangeNavigator;
            navigator.LabelStyleChanged();
        }

        /// <summary>
        /// Updates the gridlines when <see cref="ShowGridLines"/> property is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnShowGridlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDateTimeRangeNavigator).Scheduleupdate();
        }

        /// <summary>
        /// Updates the lower label bar visibility when <see cref="LowerBarVisibility"/> property is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnLowerBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDateTimeRangeNavigator).UpdateLowerBarVisibility();
        }

        /// <summary>
        /// Updates the upper label bar visibility when <see cref="UpperLabelBarVisibility"/> property is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnHigherBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfDateTimeRangeNavigator).UpdateHigherBarVisibility();
        }

        /// <summary>
        /// Gets the week number.
        /// </summary>
        /// <param name="datePassed">The Date</param>
        /// <returns>Returns the week number.</returns>
        private static int GetWeekNumber(DateTime datePassed)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            int weekNum = cultureInfo.Calendar.GetWeekOfYear(datePassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
        
        /// <summary>
        /// Sets the minute interval
        /// </summary>
        /// <param name="minuteinterval">The Minute Interval</param>
        /// <param name="dockposition">The Dock Position</param>
        private static void SetMinuteInterval(int minuteinterval, string dockposition)
        {
        }

        /// <summary>
        /// Method called to style for the labels.
        /// </summary>
        /// <param name="labelBarPanel">The Label Bar Panel</param>
        /// <param name="index">The Index</param>
        /// <param name="labelBarStyle">The Label Bar Style</param>
        private static void SetLabelStyle(Panel labelBarPanel, int index, LabelBarStyle labelBarStyle)
        {
            if (labelBarPanel.Children.Count <= index)
                return;
            if (labelBarStyle.SelectedLabelStyle == null)
            {
                (labelBarPanel.Children[index] as FrameworkElement).ClearValue(TextBlock.StyleProperty);
                (labelBarPanel.Children[index] as TextBlock).Foreground = labelBarStyle.SelectedLabelBrush;
            }
            else
            {
                (labelBarPanel.Children[index] as FrameworkElement).ClearValue(TextBlock.FontSizeProperty);
                (labelBarPanel.Children[index] as FrameworkElement).ClearValue(TextBlock.ForegroundProperty);
                (labelBarPanel.Children[index] as FrameworkElement).ClearValue(TextBlock.StyleProperty);
                (labelBarPanel.Children[index] as TextBlock).Style = labelBarStyle.SelectedLabelStyle;
            }
        }
        
        /// <summary>
        /// Sets the second interval.
        /// </summary>
        private static void SetSecondInterval()
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the thumb style when it is changed.
        /// </summary>
        /// <param name="navigator">The Range Navigator</param>
        /// <param name="e">The Event Arguments</param>
        private void ThumbStyleChanged(SfDateTimeRangeNavigator navigator, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as ThumbStyle).Navigator = this;

            navigator.SetThumbStyle();
        }

        /// <summary>
        /// Updates the range when the maximum or the minimum value changed.
        /// </summary>
        private void OnMinimumMaximumChanged()
        {
            if (Convert.ToDateTime(Minimum, CultureInfo.InvariantCulture) != DateTime.MinValue && Convert.ToDateTime(Maximum, CultureInfo.InvariantCulture) != DateTime.MinValue)
            {
                Refresh();
                CalculateRange();
                UpdateTooltip();
                if (ViewRangeStart != null)
                    OnViewRangeStartChanged();
                if (ViewRangeEnd != null)
                    OnViewRangeEndChanged();
            }
        }

        /// <summary>
        /// Method called to reset default range when items source is changed.
        /// </summary>
        private void SetDefaultRange()
        {
            if (Navigator != null)
            {
                DateTime actualRangeStart = Convert.ToDateTime(ViewRangeStart, CultureInfo.InvariantCulture);
                DateTime actualRangeEnd = Convert.ToDateTime(ViewRangeEnd, CultureInfo.InvariantCulture);
                if (!(actualRangeStart >= minimumDateTimeValue && actualRangeStart <= maximumDateTimeValue))
                {
                    ViewRangeStart = null;
                    Navigator.RangeStart = 0;
                }

                if (!(actualRangeEnd >= minimumDateTimeValue && actualRangeEnd <= maximumDateTimeValue))
                {
                    ViewRangeEnd = null;
                    Navigator.RangeEnd = 1;
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on collection changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSfRangeNavigatorCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset && (XValues as IList<DateTime>) != null)
            {
                (XValues as IList<DateTime>).Clear();
                ClearLabels();
            }

            Refresh();
            if (ViewRangeEnd != null && ViewRangeStart != null)
            {
                OnViewRangeStartChanged();
                OnViewRangeEndChanged();
            }
        }
        
        /// <summary>
        /// Refreshes the points and the layout.
        /// </summary>
        private void Refresh()
        {
            GeneratePoints();
            UpdateLayout();
        }

        /// <summary>
        /// Updates the label when it's style is changed.
        /// </summary>
        private void LabelStyleChanged()
        {
            if ((HigherLevelBarStyle != null
                 || LowerLevelBarStyle != null || HigherLabelStyle != null ||
                 LowerLabelStyle != null)
                && (upperLabelBar != null || lowerLabelBar != null))
            {
                SetLabelPosition();
                SetSelectedDataStyle();
                Scheduleupdate();
            }
        }

        /// <summary>
        /// Updating the upper label bar visibility.
        /// </summary>
        private void UpdateHigherBarVisibility()
        {
            if (upperLabelBar != null)
            {
                upperLabelBar.Visibility = HigherBarVisibility;
            }
        }
        
        /// <summary>
        /// Updating the lower label bar visibility..
        /// </summary>
        private void UpdateLowerBarVisibility()
        {
            if (lowerLabelBar != null)
            {
                lowerLabelBar.Visibility = LowerBarVisibility;
            }
        }

        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> when the interval collection changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnIntervalsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Interval interval in e.OldItems)
                {
                    if (interval.LabelFormatters != null)
                        interval.LabelFormatters.CollectionChanged -= LabelFormatters_CollectionChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (Interval interval in e.NewItems)
                {
                    if (interval.LabelFormatters != null)
                        interval.LabelFormatters.CollectionChanged += LabelFormatters_CollectionChanged;
                }
            }

            if (Navigator != null && (upperLabelBar != null && Navigator.TrackSize != 0) || (lowerLabelBar != null && Navigator.TrackSize != 0))
            {
                if (e.Action == NotifyCollectionChangedAction.Reset && (upperLabelRecycler.Count > 0 || lowerLabelRecycler.Count > 0))
                    ClearNavigatorLabels();
                Scheduleupdate();
            }
        }

        /// <summary>
        /// Updates the label format when the label formatter collection changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void LabelFormatters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Scheduleupdate();
        }
        
        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> on it's size changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSfDateTimeRangeNavigatorSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Scrollbar != null && Navigator != null)
            {
                Navigator.ClearValue(ResizableScrollBar.WidthProperty);
                this.Navigator.Width = this.ActualWidth * Scrollbar.Scale;
                XRange = -((this.ActualWidth * Scrollbar.Scale) * Scrollbar.RangeStart);

                if (Navigator.Content != null)
                {
                    double newmargin = XRange >= -Navigator.ResizableThumbSize ? XRange : XRange + Navigator.ResizableThumbSize;

                    if (!Scrollbar.isFarDragged && XRange != 0)
                    {
                        upperLabelBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                        upperLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        upperLineBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                        upperLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        lowerLabelBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                        lowerLineBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                        lowerLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        lowerLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    }
                    else if (!Scrollbar.isNearDragged && XRange != 0)
                    {
                        upperLabelBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                        upperLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        upperLineBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                        upperLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        lowerLabelBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                        lowerLineBar.Width = Navigator.TrackSize * Scrollbar.Scale;
                        lowerLineBar.Margin = new Thickness(newmargin, 0, 0, 0);
                        lowerLabelBar.Margin = new Thickness(newmargin, 0, 0, 0);
                    }

                    if (tool != null && Navigator != null)
                    {
                        tool.Width = Navigator.TrackSize * Scrollbar.Scale;
                        tool.Margin = new Thickness(newmargin, 0, 0, 0);
                    }
                }
            }

            this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
            SetThumbStyle();
        }
        
        /// <summary>
        /// Updates the <see cref="SfDateTimeRangeNavigator"/> when it is loaded.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSfDateTimeRangeNavigatorLoaded(object sender, RoutedEventArgs e)
        {
            if (Navigator != null)
            {
                if (upperLabelBar != null)
                {
                    upperLabelBar.Margin = new Thickness(Navigator.ResizableThumbSize, 0, 0, 0);
                    upperLineBar.Margin = new Thickness(Navigator.ResizableThumbSize, 0, 0, 0);
                }

                if (lowerLabelBar != null)
                {
                    lowerLabelBar.Margin = new Thickness(Navigator.ResizableThumbSize, 0, 0, 0);
                    lowerLineBar.Margin = new Thickness(Navigator.ResizableThumbSize, 0, 0, 0);
                }

                if (hover != null)
                    hover.Margin = new Thickness(Navigator.ResizableThumbSize, 0, 0, 0);
                if (innerGridlines != null)
                    innerGridlines.Margin = new Thickness(Navigator.ResizableThumbSize, 0, 0, 0);

                GetVisualChildren();
                SetThumbStyle();
            }

            CalculateRange();

            if (Navigator != null)
                Update();

            this.SizeChanged += OnSfDateTimeRangeNavigatorSizeChanged;
        }
        
        /// <summary>
        /// Calculates the range for the <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        private void CalculateRange()
        {
            if (this.ViewRangeStart != null && this.ViewRangeEnd != null && totalNoofDays != 0 && !(this.ViewRangeStart is double) && !(this.ViewRangeEnd is double) && Navigator != null && minimumDateTimeValue != DateTime.MinValue)
            {
                var ondayinterval = (Navigator.TrackSize / totalNoofDays);
                var startleft = (Convert.ToDateTime(this.ViewRangeStart) - minimumDateTimeValue).TotalDays * ondayinterval;
                var endright = (Convert.ToDateTime(this.ViewRangeEnd) - minimumDateTimeValue).TotalDays * ondayinterval;

                if (Convert.ToDateTime(ViewRangeStart) != minimumDateTimeValue)
                {
                    Navigator.IsValueChangedTrigger = false;
                    var rangeStart = 1 * (startleft / (Navigator.TrackSize));

                    if (!double.IsNaN(rangeStart))
                    {
                        Navigator.RangeStart = rangeStart;
                    }
                }

                if (Convert.ToDateTime(ViewRangeEnd) != maximumDateTimeValue)
                {
                    Navigator.IsValueChangedTrigger = false;
                    var rangeEnd = 1 * (endright / (Navigator.TrackSize));

                    if (!double.IsNaN(rangeEnd))
                    {
                        Navigator.RangeEnd = rangeEnd;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the tooltip visibility.
        /// </summary>
        private void UpdateTooltipVisibility()
        {
            if (tool == null) return;
            if (ShowToolTip)
            {
                (tool.Children[0] as UIElement).Visibility = Visibility.Visible;
                (tool.Children[1] as UIElement).Visibility = Visibility.Visible;
                UpdateTooltip();
            }
            else
            {
                (tool.Children[0] as UIElement).Visibility = Visibility.Collapsed;
                (tool.Children[1] as UIElement).Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Updates the pointer pressed interactions for the <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void DateTimeRangeNavigator_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (EnableDeferredUpdate)
                isLeftButtonPressed = true;
        }              

        /// <summary>
        /// Gets the visual children
        /// </summary>
        private void GetVisualChildren()
        {
            if (Navigator != null && VisualTreeHelper.GetChildrenCount(Navigator) > 0)
            {
                // If there is any changes in the template ,the changes has to reflected in this method.
                Grid root = VisualTreeHelper.GetChild(Navigator, 0) as Grid;
                if (root != null)
                {
                    Grid horizonatlRoot = VisualTreeHelper.GetChild(root, 0) as Grid;
                    if (horizonatlRoot != null)
                    {
                        leftThumb = VisualTreeHelper.GetChild(horizonatlRoot, 6) as Thumb;
                        rightThumb = VisualTreeHelper.GetChild(horizonatlRoot, 7) as Thumb;
                        if (leftThumb != null)
                        {
                            Grid grid1 = VisualTreeHelper.GetChild(leftThumb, 0) as Grid;
                            if (grid1 != null)
                            {
                                leftThumbLine = VisualTreeHelper.GetChild(grid1, 0) as Line;
                                leftThumbSymbol = VisualTreeHelper.GetChild(grid1, 1) as ContentPresenter;
                            }
                        }

                        if (rightThumb != null)
                        {
                            Grid grid2 = VisualTreeHelper.GetChild(rightThumb, 0) as Grid;
                            if (grid2 != null)
                            {
                                rightThumbLine = VisualTreeHelper.GetChild(grid2, 0) as Line;
                                rightThumbSymbol = VisualTreeHelper.GetChild(grid2, 1) as ContentPresenter;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the day values.
        /// </summary>
        /// <param name="currentDate">The Current Date</param>
        private void AddDaysValues(DateTime currentDate)
        {
            if (currentDate <= maximumDateTimeValue)
                daysvalue.Add((currentDate - minimumDateTimeValue).TotalDays);
            else
                daysvalue.Add((maximumDateTimeValue - minimumDateTimeValue).TotalDays);
        }

        /// <summary>
        /// Generates the property points.
        /// </summary>
        private void GeneratePropertyPoints()
        {
            DataEnd = 0;
            IEnumerator enumerator = this.ItemsSource.GetEnumerator();
            PropertyInfo xPropertyInfo;

            if (enumerator.MoveNext())
            {
                xPropertyInfo = ChartDataUtils.GetPropertyInfo(enumerator.Current, this.XBindingPath);
                IPropertyAccessor xPropertyAccessor = null;
                if (xPropertyInfo != null)
                {
                    xPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                    Func<object, object> xGetMethod = xPropertyAccessor.GetMethod;
                    this.XValues = new List<DateTime>();
                    IList<DateTime> xValue = XValues as List<DateTime>;
                    do
                    {
                        object xVal = xGetMethod(enumerator.Current);
                        xValue.Add((DateTime)xVal);
                        DataEnd++;
                    }
                    while (enumerator.MoveNext());
                }
            }
        }

        /// <summary>
        /// Updates the tooltip for the <see cref="SfDateTimeRangeNavigator"/>
        /// </summary>
        private void UpdateTooltip()
        {
            if (tool != null && ShowToolTip && isMinMaxSet && Navigator != null && Navigator.TrackSize != 0)
            {
                tool.Visibility = Visibility.Visible;
                var onedayInterval = this.Navigator.TrackSize / this.totalNoofDays;
                var leftTooltip = tool.Children[0] as ContentControl;
                var rightTooltip = tool.Children[1] as ContentControl;
                var noofdays = (this.Navigator.RangeEnd * this.Navigator.TrackSize) / onedayInterval;
                var chkdays = (this.Navigator.RangeStart * this.Navigator.TrackSize) / onedayInterval;
                leftTooltip.ContentTemplate = this.LeftToolTipTemplate == null ? this.leftTemplate : this.LeftToolTipTemplate;
                rightTooltip.ContentTemplate = this.RightToolTipTemplate == null ? this.rightTemplate : this.RightToolTipTemplate;
                leftTooltip.Content = this.minimumDateTimeValue.AddDays(chkdays).ToString(this.ToolTipLabelFormat, CultureInfo.CurrentCulture);
                rightTooltip.Content = this.minimumDateTimeValue.AddDays(noofdays).ToString(this.ToolTipLabelFormat, CultureInfo.CurrentCulture);
                leftTooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                rightTooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                var leftwidth = leftTooltip.DesiredSize.Width;
                var rightwidth = rightTooltip.DesiredSize.Width;
                Canvas.SetLeft((tool.Children[0] as FrameworkElement), (this.Navigator.RangeStart * this.Navigator.TrackSize) - leftwidth);
                Canvas.SetLeft((tool.Children[1] as FrameworkElement), (this.Navigator.RangeEnd * this.Navigator.TrackSize));
                isRightSet = false;
                leftTooltip.Margin = new Thickness(0, 0, 0, 0);
                rightTooltip.Margin = new Thickness(0, 0, 0, 0);
                if (Canvas.GetLeft(leftTooltip) <= 0)
                {
                    leftTooltip.ContentTemplate = this.LeftToolTipTemplate == null ? this.rightTemplate : this.LeftToolTipTemplate;
                    Canvas.SetLeft((tool.Children[0] as FrameworkElement), (this.Navigator.RangeStart * this.Navigator.ActualWidth));
                    isRightSet = true;
                }

                if (Canvas.GetLeft(rightTooltip) + rightwidth >= Navigator.ActualWidth)
                {
                    rightTooltip.ContentTemplate = this.RightToolTipTemplate == null ? this.leftTemplate : this.RightToolTipTemplate;
                    Canvas.SetLeft((tool.Children[1] as FrameworkElement), (this.Navigator.RangeEnd * (this.Navigator.ActualWidth)) - rightwidth);
                }

                if (Scrollbar != null)
                {
                    if (Canvas.GetLeft(leftTooltip) / Navigator.TrackSize <= Scrollbar.RangeStart)
                    {
                        leftTooltip.ContentTemplate = this.LeftToolTipTemplate == null ? this.rightTemplate : this.LeftToolTipTemplate;
                        if (Navigator.RangeStart <= Scrollbar.RangeStart)
                            Canvas.SetLeft((tool.Children[0] as FrameworkElement), (this.Scrollbar.RangeStart * this.Navigator.TrackSize));
                        else
                            Canvas.SetLeft((tool.Children[0] as FrameworkElement), ((Canvas.GetLeft(leftTooltip) / Navigator.TrackSize) * this.Navigator.TrackSize) + leftwidth);
                        if (Navigator.RangeEnd <= Scrollbar.RangeStart)
                            Canvas.SetLeft((tool.Children[1] as FrameworkElement), (this.Scrollbar.RangeStart * this.Navigator.TrackSize));
                        isRightSet = true;
                    }

                    if ((Canvas.GetLeft(rightTooltip) + rightwidth) / Navigator.ActualWidth >= Scrollbar.RangeEnd)
                    {
                        rightTooltip.ContentTemplate = this.RightToolTipTemplate == null ? this.leftTemplate : this.RightToolTipTemplate;
                        if (Math.Round(Navigator.RangeEnd) >= Scrollbar.RangeEnd)
                            Canvas.SetLeft(tool.Children[1] as FrameworkElement, (this.Scrollbar.RangeEnd * this.Navigator.TrackSize + Navigator.ResizableThumbSize) - rightwidth);
                        else
                            Canvas.SetLeft(tool.Children[1] as FrameworkElement, ((Canvas.GetLeft(rightTooltip) / Navigator.ActualWidth) * this.Navigator.ActualWidth) - rightwidth);
                        if (Navigator.RangeStart >= Scrollbar.RangeEnd)
                            Canvas.SetLeft(tool.Children[0] as FrameworkElement, (this.Scrollbar.RangeEnd * this.Navigator.TrackSize) - leftwidth);
                    }
                }

                if (Canvas.GetLeft(leftTooltip) + leftwidth >= Canvas.GetLeft(rightTooltip))
                {
                    if (!isRightSet)
                        leftTooltip.Margin = new Thickness(0, 30, 0, 0);
                    else
                        rightTooltip.Margin = new Thickness(0, 30, 0, 0);
                }
                else
                {
                    if (!isRightSet)
                        leftTooltip.Margin = new Thickness(0, 0, 0, 0);
                    else
                        rightTooltip.Margin = new Thickness(0, 0, 0, 0);
                }
            }

            if (!isMinMaxSet && tool != null)
                tool.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Changes view range.
        /// </summary>
        private void ChangeViewRange()
        {
            if (Navigator != null && this.Navigator.TrackSize > 0 && totalNoofDays != 0d)
            {
                var onedayInterval = this.Navigator.TrackSize / this.totalNoofDays;
                var noofdays = (this.Navigator.RangeStart * this.Navigator.TrackSize) / onedayInterval;
                this.ViewRangeStart = this.minimumDateTimeValue.AddDays(noofdays);
                noofdays = (this.Navigator.RangeEnd * this.Navigator.TrackSize) / onedayInterval;
                this.ViewRangeEnd = this.minimumDateTimeValue.AddDays(noofdays);
            }
        }
        
        /// <summary>
        /// Method called to set the style for upper and lower labels.
        /// </summary>
        private void SetSelectedDataStyle()
        {
            int labelcount = -1;
            if (upperLabelBar != null && upperLabelBounds != null)
            {
                for (int i = 0; i < upperLabelBounds.Count; i++)
                {
                    if (Navigator.RangeStart * Navigator.TrackSize < upperLabelBounds[i] && Navigator.RangeEnd * Navigator.TrackSize >= upperLabelBounds[i])
                    {
                        SfDateTimeRangeNavigator.SetLabelStyle(upperLabelBar, i, HigherLevelBarStyle);
                        labelcount = i;
                    }
                    else if (i == 0 && (Navigator.RangeStart * Navigator.TrackSize >= 0 && Navigator.RangeStart * Navigator.TrackSize <= upperLabelBounds[i]) && (Navigator.RangeEnd * Navigator.TrackSize >= 0 && Navigator.RangeEnd * Navigator.TrackSize <= upperLabelBounds[i]))
                    {
                        SfDateTimeRangeNavigator.SetLabelStyle(upperLabelBar, i, HigherLevelBarStyle);
                    }
                    else if (i != 0 && (Navigator.RangeStart * Navigator.TrackSize >= upperLabelBounds[i - 1] && Navigator.RangeStart * Navigator.TrackSize <= upperLabelBounds[i]) && (Navigator.RangeEnd * Navigator.TrackSize >= upperLabelBounds[i - 1] && Navigator.RangeEnd * Navigator.TrackSize <= upperLabelBounds[i]))
                    {
                        SfDateTimeRangeNavigator.SetLabelStyle(upperLabelBar, i, HigherLevelBarStyle);
                    }
                    else
                    {
                        (upperLabelBar.Children[i] as FrameworkElement).ClearValue(TextBlock.ForegroundProperty);
                        (upperLabelBar.Children[i] as TextBlock).Style = HigherLabelStyle;
                    }

                    if (labelcount != -1 && Navigator.RangeEnd * Navigator.TrackSize > upperLabelBounds[labelcount] && labelcount + 1 != upperLabelBar.Children.Count)
                    {
                        SfDateTimeRangeNavigator.SetLabelStyle(upperLabelBar, labelcount + 1, HigherLevelBarStyle);
                    }
                }
            }

            labelcount = -1;
            if (lowerLabelBar != null && lowerLabelBounds != null && lowerLabelBar.Children.Count == lowerLabelBounds.Count)
            {
                for (int i = 0; i < lowerLabelBounds.Count; i++)
                {
                    if (Navigator.RangeStart * Navigator.TrackSize < lowerLabelBounds[i] && Navigator.RangeEnd * Navigator.TrackSize >= lowerLabelBounds[i])
                    {
                        SfDateTimeRangeNavigator.SetLabelStyle(lowerLabelBar, i, LowerLevelBarStyle);
                        labelcount = i;
                    }
                    else if (i == 0 && (Navigator.RangeStart * Navigator.TrackSize >= 0 && Navigator.RangeStart * Navigator.TrackSize <= lowerLabelBounds[i]) && (Navigator.RangeEnd * Navigator.TrackSize >= 0 && Navigator.RangeEnd * Navigator.TrackSize <= lowerLabelBounds[i]))
                    {
                        SfDateTimeRangeNavigator.SetLabelStyle(lowerLabelBar, i, LowerLevelBarStyle);
                    }
                    else if (i != 0 && (Navigator.RangeStart * Navigator.TrackSize >= lowerLabelBounds[i - 1] && Navigator.RangeStart * Navigator.TrackSize <= lowerLabelBounds[i]) && (Navigator.RangeEnd * Navigator.TrackSize >= lowerLabelBounds[i - 1] && Navigator.RangeEnd * Navigator.TrackSize <= lowerLabelBounds[i]))
                    {
                        SfDateTimeRangeNavigator.SetLabelStyle(lowerLabelBar, i, LowerLevelBarStyle);
                    }
                    else
                    {
                        (lowerLabelBar.Children[i] as FrameworkElement).ClearValue(TextBlock.ForegroundProperty);
                        (lowerLabelBar.Children[i] as TextBlock).Style = LowerLabelStyle;
                    }

                    if (labelcount != -1 && Navigator.RangeEnd * Navigator.TrackSize > lowerLabelBounds[labelcount] && labelcount + 1 != lowerLabelBar.Children.Count)
                    {
                        SfDateTimeRangeNavigator.SetLabelStyle(lowerLabelBar, labelcount + 1, LowerLevelBarStyle);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the mouse left button down interaction of the lower label bar.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void LowerLabelBar_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (EnableDeferredUpdate)
            {
                timer = new DispatcherTimer();
                timer.Tick += OnTimeout;
            }

            Windows.UI.Input.PointerPoint pointer = e.GetCurrentPoint(lowerLabelBar);
            Point pt = new Point(pointer.Position.X, pointer.Position.Y);

            if (lowerLabelBounds != null && lowerLabelBounds.Count > 1)
                for (int i = 0; i < lowerLabelBounds.Count; i++)
                {
                    if (pt.X > lowerLabelBounds[i] && i + 1 == lowerLabelBounds.Count)
                    {
                        double startleft = lowerLabelBounds[i];
                        double endright = 1;
                        LabelSelection(startleft, endright);
                    }
                    else if (pt.X > lowerLabelBounds[i] && pt.X < lowerLabelBounds[i + 1])
                    {
                        double startleft = lowerLabelBounds[i];
                        double endright = lowerLabelBounds[i + 1];
                        LabelSelection(startleft, endright);
                        break;
                    }
                    else if (pt.X < lowerLabelBounds[i])
                    {
                        double startleft = 0;
                        double endright = lowerLabelBounds[0];
                        LabelSelection(startleft, endright);
                    }
                }
        }

        /// <summary>
        /// Selects the label.
        /// </summary>
        /// <param name="startleft">The Start Left Range</param>
        /// <param name="endright">The End Right Range</param>
        private void LabelSelection(double startleft, double endright)
        {
            var rangeEnd = 1 * (endright / (Navigator.TrackSize));
            Navigator.IsValueChangedTrigger = rangeEnd == Navigator.RangeEnd;
            Navigator.RangeStart = (1 * (startleft / (Navigator.TrackSize))) == Navigator.Maximum ? 0.999 : 1 * (startleft / (Navigator.TrackSize));
            Navigator.IsValueChangedTrigger = true;
            Navigator.RangeEnd = Navigator.RangeStart > rangeEnd ? Navigator.Maximum : rangeEnd;
            SetSelectedDataStyle();
        }

        /// <summary>
        /// Updates the mouse left button down interaction of the lower label bar.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void UpperLabelBar_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (EnableDeferredUpdate)
            {
                timer = new DispatcherTimer();
                timer.Tick += OnTimeout;
            }

            Windows.UI.Input.PointerPoint pointer = e.GetCurrentPoint(upperLabelBar);
            Point pt = new Point(pointer.Position.X, pointer.Position.Y);

            if (upperLabelBounds != null && upperLabelBounds.Count > 1)
                for (int i = 0; i < upperLabelBounds.Count; i++)
                {
                    if (pt.X > upperLabelBounds[i] && i + 1 == upperLabelBounds.Count)
                    {
                        double startleft = upperLabelBounds[i];
                        double endright = 1;
                        LabelSelection(startleft, endright);
                    }
                    else if (pt.X > upperLabelBounds[i] && pt.X < upperLabelBounds[i + 1])
                    {
                        double startleft = upperLabelBounds[i];
                        double endright = upperLabelBounds[i + 1];
                        LabelSelection(startleft, endright);
                        break;
                    }
                    else if (pt.X < upperLabelBounds[i])
                    {
                        double startleft = 0;
                        double endright = upperLabelBounds[0];
                        LabelSelection(startleft, endright);
                    }
                }
        }

        /// <summary>
        /// Updates the mouse leave interaction of the lower label bar.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void LowerLabelBar_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (hover != null)
                (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Updates the mouse move interaction of the lower label bar.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void LowerLabelBar_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint pointer = e.GetCurrentPoint(lowerLabelBar);
            Point pt = new Point(pointer.Position.X, pointer.Position.Y);

            if (lowerLabelBounds != null)
                for (int i = 0; i < lowerLabelBounds.Count; i++)
                {
                    if (i + 1 < lowerLabelBounds.Count && pt.X > lowerLabelBounds[i] && pt.X < lowerLabelBounds[i + 1])
                    {
                        Canvas.SetLeft((hover.Children[0] as Rectangle), lowerLabelBounds[i]);
                        (hover.Children[0] as Rectangle).Height = Navigator.ActualHeight;
                        (hover.Children[0] as Rectangle).Width = lowerLabelBounds[i + 1] - lowerLabelBounds[i];
                        (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0x7E, 0x00, 0x00, 0x00));
                        hover.Opacity = 0.5;
                        break;
                    }
                    else if (pt.X < lowerLabelBounds[i])
                    {
                        Canvas.SetLeft((hover.Children[0] as Rectangle), 0);
                        (hover.Children[0] as Rectangle).Height = Navigator.ActualHeight;
                        (hover.Children[0] as Rectangle).Width = lowerLabelBounds[0];
                        (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0x7E, 0x00, 0x00, 0x00));
                        hover.Opacity = 0.5;
                    }
                }
        }

        /// <summary>
        /// Updates the mouse leave interactions of the upper label bar.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        private void UpperLabelBar_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (hover != null)
                (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Updates the mouse move interaction of the upper label bar.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void UpperLabelBar_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint pointer = e.GetCurrentPoint(upperLabelBar);
            Point pt = new Point(pointer.Position.X, pointer.Position.Y);

            if (upperLabelBounds != null)
                for (int i = 0; i < upperLabelBounds.Count; i++)
                {
                    if (i + 1 < upperLabelBounds.Count && pt.X > upperLabelBounds[i] && pt.X < upperLabelBounds[i + 1])
                    {
                        Canvas.SetLeft((hover.Children[0] as Rectangle), upperLabelBounds[i]);
                        (hover.Children[0] as Rectangle).Height = Navigator.ActualHeight;
                        (hover.Children[0] as Rectangle).Width = upperLabelBounds[i + 1] - upperLabelBounds[i];
                        (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0x7E, 0x00, 0x00, 0x00));
                        hover.Opacity = 0.5;
                        break;
                    }
                    else if (pt.X < upperLabelBounds[i])
                    {
                        Canvas.SetLeft((hover.Children[0] as Rectangle), 0);
                        (hover.Children[0] as Rectangle).Height = Navigator.ActualHeight;
                        (hover.Children[0] as Rectangle).Width = upperLabelBounds[0];
                        (hover.Children[0] as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0x7E, 0x00, 0x00, 0x00));
                        hover.Opacity = 0.5;
                    }
                }
        }

        /// <summary>
        /// Inserts the labels at the specified dock position.
        /// </summary>
        /// <param name="dockposition">The Dock Position.</param>
        private void InsertLabels(string dockposition)
        {
            int count = 0;
            if (dockposition == "Lower" && lowerLabelBar != null)
            {
                double eachinterval = Navigator.TrackSize / totalNoofDays;
                double lineleft = daysvalue[count] * eachinterval;
                double setleft = 0;
                if (LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Center || LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Stretch)
                    setleft = (lineleft / 2) - (txtblockwidth / 2);
                else if (LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Left)
                    setleft = 0;
                else
                    setleft = lineleft - txtblockwidth;
                labelElementBounds = new Rect[lowerLabelRecycler.Count];
                lowerLabelBounds = new ObservableCollection<double>();
                foreach (TextBlock txt in lowerLabelRecycler)
                {
                    txt.Visibility = Visibility.Visible;
                    txt.FontSize = 11;

                    if (setleft < 0)
                        txt.Visibility = Visibility.Collapsed;
                    Canvas.SetLeft(txt, setleft);
                    Line ln1 = lowerTickLineRecycler[count];
#if NETFX_CORE 
                    Canvas.SetTop(txt, 2);
#endif
                    ln1.X1 = Math.Round(lineleft);
                    ln1.X2 = Math.Round(lineleft);
                    ln1.Y1 = 0;
                    ln1.Y2 = lowerBorder.DesiredSize.Height;
                    ln1.Style = LowerBarTickLineStyle;
                    ChartExtensionUtils.SetStrokeDashArray(lowerTickLineRecycler);

                    if (ShowGridLines)
                    {
                        Line ln = lowerGridLineRecycler[count];
                        ln.X1 = Math.Round(lineleft);
                        ln.X2 = Math.Round(lineleft);
                        ln.Y1 = 0;
                        ln.Y2 = Navigator.ActualHeight;
                        ln.Style = LowerBarGridLineStyle;
                        ChartExtensionUtils.SetStrokeDashArray(lowerGridLineRecycler);
                    }
                    else
                    {
                        lowerGridLineRecycler[count].ClearUIValues();
                    }

                    lowerLabelBounds.Add(lineleft);
                    labelElementBounds[count] = new Rect(setleft, 0, txt.ActualWidth, 17);

                    if (count + 1 < daysvalue.Count)
                        lineleft = daysvalue[count + 1] * eachinterval;
                    if (LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Center || LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Stretch)
                        setleft = ((lineleft - daysvalue[count] * eachinterval) / 2) - (txtblockwidth / 2) + (daysvalue[count] * eachinterval);
                    else if (LowerLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Left)
                        setleft = (daysvalue[count] * eachinterval);
                    else
                        setleft = lineleft - txtblockwidth;
                    if (count != 0 && ((daysvalue[count] * eachinterval) - (daysvalue[count - 1] * eachinterval) < txtblockwidth))
                    {
                        txt.Visibility = Visibility.Collapsed;
                    }

                    count++;
                }
            }
            else
            {
                upperLabelBounds = new ObservableCollection<double>();
                double eachinterval = 0;
                if (XValues != null && (XValues as IList<DateTime>).Count > 0)
                {
                    if ((XValues as IList<DateTime>).Count == 1)
                        eachinterval = Navigator.TrackSize / maximumDateTimeValue.ToOADate();
                    else
                        eachinterval = Navigator.TrackSize /
                                       (maximumDateTimeValue.ToOADate() -
                                        minimumDateTimeValue.ToOADate());
                }
                else if (Convert.ToString(Minimum) != "0" && Convert.ToString(Maximum) != "1")
                    eachinterval = Navigator.TrackSize / (Convert.ToDateTime(Maximum, CultureInfo.InvariantCulture).ToOADate() - Convert.ToDateTime(Minimum, CultureInfo.InvariantCulture).ToOADate());

                double lineleft = daysvalue[count] * eachinterval;
                double setleft = 0;
                if (HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Center || HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Stretch)
                    setleft = (lineleft / 2) - (txtblockwidth / 2);
                else if (HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Left)
                    setleft = 0;
                else
                    setleft = lineleft - txtblockwidth;

                labelElementBounds = new Rect[upperLabelRecycler.Count];
                foreach (TextBlock txt in upperLabelRecycler)
                {
                    txt.Visibility = Visibility.Visible;
                    txt.FontSize = 11;

                    if (setleft < 0)
                        txt.Visibility = Visibility.Collapsed;
                    Canvas.SetLeft(txt, setleft);
                    Line ln = upperTickLineRecycler[count];
                    ln.X1 = lineleft;
                    ln.X2 = lineleft;
                    ln.Y1 = 0;
                    ln.Y2 = uppperBorder.DesiredSize.Height;
                    ln.Style = HigherBarTickLineStyle;

                    ChartExtensionUtils.SetStrokeDashArray(upperTickLineRecycler);

                    Line gridline = upperGridLineRecycler[count];
                    gridline.X1 = Math.Round(lineleft);
                    gridline.X2 = Math.Round(lineleft);
                    gridline.Y1 = 0;
                    gridline.Y2 = Navigator.ActualHeight;
                    if (HigherBarGridLineStyle != null)
                    {
                        gridline.Style = HigherBarGridLineStyle;
                        ChartExtensionUtils.SetStrokeDashArray(upperGridLineRecycler);
                    }
                    else
                        gridline.ClearValue(Line.StyleProperty);

                    labelElementBounds[count] = new Rect(setleft, 0, txt.ActualWidth, 17);
                    upperLabelBounds.Add(lineleft);
                    if (count + 1 < daysvalue.Count)
                        lineleft = daysvalue[count + 1] * eachinterval;
                    if (HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Center || HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Stretch)
                        setleft = ((lineleft - daysvalue[count] * eachinterval) / 2) - (txtblockwidth / 2) + (daysvalue[count] * eachinterval);
                    else if (HigherLevelBarStyle.LabelHorizontalAlignment == HorizontalAlignment.Left)
                        setleft = (daysvalue[count] * eachinterval) + ln.StrokeThickness;
                    else
                        setleft = lineleft - txtblockwidth - ln.StrokeThickness;

                    if (count != 0 && ((daysvalue[count] * eachinterval) - (daysvalue[count - 1] * eachinterval) < txtblockwidth))
                    {
                        txt.Visibility = Visibility.Collapsed;
                    }

                    count++;
                }
            }

            CalculateSelectedData();
        }

        /// <summary>
        /// Sets the hour interval.
        /// </summary>
        /// <param name="hourinterval">The Hour Interval</param>
        /// <param name="dockposition">The Dock Position</param>
        /// <param name="formatter">The Formatter</param>
        private void SetHourInterval(int hourinterval, string dockposition, ObservableCollection<string> formatter)
        {
            string format = string.Empty;
            bool isadded;
            if (formatter != null && formatter.Count == 0)
            {
                isFormatterEmpty = true;
            }

            if (formatter != null && !isFormatterEmpty)
            {
                for (int i = 0; i < formatter.Count; i++)
                {
                    isadded = AddLabels(formatter[i].ToString(), dockposition, "Hour");
                    if (isadded)
                    {
                        format = formatter[i].ToString();
                        break;
                    }
                }
            }

            daysvalue.Clear();
            txtblockwidth = 0;
            if (dockposition == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();
            labelElementBounds = null;
            string content = string.Empty;
            var currentDate = minimumDateTimeValue;
            switch (hourinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("hh tt", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(1);
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                    {
                        if (navigatorIntervals.Contains("Hour"))
                            SetHourInterval(1, dockposition, formatters[navigatorIntervals.IndexOf("Hour")]);
                        else
                            SetHourInterval(1, dockposition, null);
                    }

                    break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("hh tt", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(1);
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                    {
                        if (navigatorIntervals.Contains("Hour"))
                            SetHourInterval(2, dockposition, formatters[navigatorIntervals.IndexOf("Hour")]);
                        else
                            SetHourInterval(2, dockposition, null);
                    }

                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("hh tt", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(2);
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                    {
                        if (navigatorIntervals.Contains("Hour"))
                            SetHourInterval(3, dockposition, formatters[navigatorIntervals.IndexOf("Hour")]);
                        else
                            SetHourInterval(3, dockposition, null);
                    }

                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("ht", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(4);
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                        ClearLabels(dockposition);
                    break;
            }

            if (isFormatterEmpty)
                isFormatterEmpty = false;
        }

        /// <summary>
        /// Sets the day interval.
        /// </summary>
        /// <param name="dayinterval">The Day Interval</param>
        /// <param name="dockposition">The Dock Position</param>
        /// <param name="formatter">The Formatter</param>
        private void SetDayInterval(int dayinterval, string dockposition, ObservableCollection<string> formatter)
        {
            string format = string.Empty;
            bool isadded;

            if (formatter != null && formatter.Count == 0)
            {
                isFormatterEmpty = true;
            }

            if (formatter != null && !isFormatterEmpty)
            {
                for (int i = 0; i < formatter.Count; i++)
                {
                    isadded = AddLabels(formatter[i].ToString(), dockposition, "Day");
                    if (isadded)
                    {
                        format = formatter[i].ToString();
                        break;
                    }
                }
            }

            daysvalue.Clear();
            txtblockwidth = 0;
            if (dockposition == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();
            string content = string.Empty;
            labelElementBounds = null;
            var currentDate = minimumDateTimeValue;
            switch (dayinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("dddd, MMMM d, yyyy", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day).AddDays(1);
                        AddDaysValues(currentDate);
                    }

                    if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                    {
                        InsertLabels(dockposition);
                        if (Navigator.TrackSize - (txtblockwidth * lowerLabelBar.Children.Count) > (txtblockwidth / 2))
                        {
                            if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Hour"))
                            {
                                SetDayInterval(0, "Upper", null);
                            }

                            if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Hour"))
                            {
                                if (navigatorIntervals.Contains("Hour"))
                                    SetHourInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Hour")]);
                                else
                                    SetHourInterval(0, "Lower", null);
                            }
                        }
                    }
                    else if (dockposition == "Upper" && GenerateLabelContainers(dockposition))
                    {
                        InsertLabels(dockposition);
                    }
                    else
                    {
                        if (navigatorIntervals.Contains("Day"))
                            SetDayInterval(1, dockposition, formatters[navigatorIntervals.IndexOf("Day")]);
                        else
                            SetDayInterval(1, dockposition, null);
                    }

                    break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("ddd, MMM d, yyyy", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day).AddDays(1);
                        AddDaysValues(currentDate);
                    }

                    if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                    {
                        if (navigatorIntervals.Contains("Day"))
                            SetDayInterval(2, dockposition, formatters[navigatorIntervals.IndexOf("Day")]);
                        else
                            SetDayInterval(2, dockposition, null);
                    }

                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("dddd, d", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                        {
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        }

                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day).AddDays(1);
                        AddDaysValues(currentDate);
                    }

                    if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                    {
                        if (navigatorIntervals.Contains("Day"))
                            SetDayInterval(3, dockposition, formatters[navigatorIntervals.IndexOf("Day")]);
                        else
                            SetDayInterval(3, dockposition, null);
                    }

                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.Day.ToString();
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day).AddDays(1);
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Week"))
                    {
                        if (navigatorIntervals.Contains("Week"))
                            SetWeekInterval(0, dockposition, formatters[navigatorIntervals.IndexOf("Week")]);
                        else
                            SetWeekInterval(0, dockposition, null);
                    }
                    else
                        ClearLabels(dockposition);

                    break;
                default:
                    break;
            }

            if (isFormatterEmpty)
                isFormatterEmpty = false;
        }

        /// <summary>
        /// Method to check whether to add the format to labels.
        /// </summary>
        /// <param name="format">The Format</param>
        /// <param name="dockposition">The Dock Position.</param>
        /// <param name="interval">The Interval</param>
        /// <returns>Returns a value indicating whether the total text block width is less than the track size.</returns>
        private bool AddLabels(string format, string dockposition, string interval)
        {
            var currentDate = minimumDateTimeValue;
            string content = string.Empty;
            while (currentDate <= maximumDateTimeValue)
            {
                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                if (dockposition == "Lower")
                    lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                else
                    upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                if (interval == "Day")
                {
                    currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day).AddDays(1);
                }
                else if (interval == "Year")
                {
                    if ((currentDate - minimumDateTimeValue).TotalDays != 0)
                        daysvalue.Add((currentDate - minimumDateTimeValue).TotalDays);
                    currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                }
                else if (interval == "Quarter")
                {
                    if (currentDate.Month >= 1 && currentDate.Month <= 3)
                    {
                        currentDate = new DateTime(currentDate.Year, 4, 1);
                    }
                    else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                    {
                        currentDate = new DateTime(currentDate.Year, 7, 1);
                    }
                    else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                    {
                        currentDate = new DateTime(currentDate.Year, 10, 1);
                    }
                    else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                    {
                        currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                    }
                }
                else if (interval == "Month")
                {
                    if (currentDate == minimumDateTimeValue)
                    {
                        if (currentDate.Month == 12)
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                        else
                            currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                    }
                    else
                        currentDate = currentDate.AddMonths(1);
                }
                else if (interval == "Week")
                {
                    if (currentDate.DayOfWeek != DayOfWeek.Monday)
                    {
                        while (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            currentDate = currentDate.AddDays(1);
                        }

                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                    }
                    else
                    {
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        currentDate = currentDate.AddDays(7);
                    }
                }
                else
                    currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, 0, 0).AddHours(1);

                if (interval == "Year")
                    daysvalue.Add((maximumDateTimeValue - minimumDateTimeValue).TotalDays);
                else
                    AddDaysValues(currentDate);
            }

            if (GenerateLabelContainers(dockposition))
            {
                ClearLabels(dockposition);
                return true;
            }

            ClearLabels(dockposition);
            return false;
        }

        /// <summary>
        /// Sets the week interval.
        /// </summary>
        /// <param name="weekinterval">The Week Interval</param>
        /// <param name="dockposition">The Dock Position</param>
        /// <param name="formatter">The Formatter</param>
        private void SetWeekInterval(int weekinterval, string dockposition, ObservableCollection<string> formatter)
        {
            string format = string.Empty;
            bool isadded;
            if (formatter != null)
            {
                for (int i = 0; i < formatter.Count; i++)
                {
                    isadded = AddLabels(formatter[i].ToString(), dockposition, "Week");
                    if (isadded)
                    {
                        format = formatter[i].ToString();
                        break;
                    }
                }
            }

            txtblockwidth = 0;
            daysvalue.Clear();
            if (dockposition == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();

            labelElementBounds = null;
            var currentDate = minimumDateTimeValue;
            string content = string.Empty;
            while (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                currentDate = currentDate.AddDays(-1);
            }

            switch (weekinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        else
                            content = SfChartResourceWrapper.Week + SfDateTimeRangeNavigator.GetWeekNumber(currentDate) + currentDate.ToString(" MMMM, yyyy", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        if (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            while (currentDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                currentDate = currentDate.AddDays(1);
                            }

                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        }
                        else
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                            currentDate = currentDate.AddDays(7);
                        }

                        AddDaysValues(currentDate);
                    }

                    if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                    {
                        InsertLabels(dockposition);
                        if (Navigator.TrackSize - (txtblockwidth * lowerLabelBar.Children.Count) > (txtblockwidth / 2))
                        {
                            if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Day"))
                                SetWeekInterval(0, "Upper", null);
                            if (navigatorIntervals.Contains("Day"))
                                SetDayInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Day")]);
                            else if (navigatorIntervals.Contains("Hour"))
                                SetHourInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Hour")]);
                            else if (navigatorIntervals.Count == 0)
                                SetDayInterval(0, "Lower", null);
                        }
                    }
                    else if (dockposition == "Upper" && GenerateLabelContainers(dockposition))
                    {
                        InsertLabels(dockposition);
                    }
                    else
                    {
                        if (navigatorIntervals.Contains("Week"))
                            SetWeekInterval(1, dockposition, formatters[navigatorIntervals.IndexOf("Week")]);
                        else
                            SetWeekInterval(1, dockposition, null);
                    }

                    break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        else
                            content = SfChartResourceWrapper.Week + SfDateTimeRangeNavigator.GetWeekNumber(currentDate) + currentDate.ToString(" MMMM, yyyy", CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        if (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            while (currentDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                currentDate = currentDate.AddDays(1);
                            }

                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        }
                        else
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                            currentDate = currentDate.AddDays(7);
                        }

                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                    {
                        if (navigatorIntervals.Contains("Week"))
                            SetWeekInterval(2, dockposition, formatters[navigatorIntervals.IndexOf("Week")]);
                        else
                            SetWeekInterval(2, dockposition, null);
                    }

                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        else
                            content = SfChartResourceWrapper.Week + SfDateTimeRangeNavigator.GetWeekNumber(currentDate);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        if (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            while (currentDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                currentDate = currentDate.AddDays(1);
                            }

                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        }
                        else
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                            currentDate = currentDate.AddDays(7);
                        }

                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                    {
                        InsertLabels(dockposition);
                    }
                    else
                    {
                        if (navigatorIntervals.Contains("Week"))
                            SetWeekInterval(3, dockposition, formatters[navigatorIntervals.IndexOf("Week")]);
                        else
                            SetWeekInterval(3, dockposition, null);
                    }

                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        else
                            content = SfChartResourceWrapper.W + SfDateTimeRangeNavigator.GetWeekNumber(currentDate);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        if (currentDate.DayOfWeek != DayOfWeek.Monday)
                        {
                            while (currentDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                currentDate = currentDate.AddDays(1);
                            }

                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                        }
                        else
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                            currentDate = currentDate.AddDays(7);
                        }

                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Month"))
                    {
                        if (navigatorIntervals.Contains("Month"))
                            SetMonthInterval(0, dockposition, formatters[navigatorIntervals.IndexOf("Month")]);
                        else
                            SetMonthInterval(0, dockposition, null);
                    }
                    else
                        ClearLabels(dockposition);

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the month interval
        /// </summary>
        /// <param name="monthinterval">The Month Interval</param>
        /// <param name="dockposition">The Dock Position</param>
        /// <param name="formatter">The Formatter</param>
        private void SetMonthInterval(int monthinterval, string dockposition, ObservableCollection<string> formatter)
        {
            string format = string.Empty;
            bool isadded;
            if (formatter != null && formatter.Count == 0)
            {
                isFormatterEmpty = true;
            }

            if (formatter != null && !isFormatterEmpty)
            {
                for (int i = 0; i < formatter.Count; i++)
                {
                    isadded = AddLabels(formatter[i].ToString(), dockposition, "Month");
                    if (isadded)
                    {
                        format = formatter[i].ToString();
                        break;
                    }
                }
            }

            txtblockwidth = 0;
            daysvalue.Clear();
            if (dockposition == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();
            labelElementBounds = null;
            var currentDate = minimumDateTimeValue;
            string content = string.Empty;
            switch (monthinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("MMMM, yyyy", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                        {
                            lowerBarLabels.Add(new ChartAxisLabel { LabelContent = content });
                        }
                        else
                        {
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        }

                        if (currentDate == minimumDateTimeValue)
                        {
                            if (currentDate.Month == 12)
                                currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            else
                                currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                        }
                        else
                            currentDate = currentDate.AddMonths(1);
                        AddDaysValues(currentDate);
                    }

                    if (dockposition == "Lower" && GenerateLabelContainers(dockposition))
                    {
                        InsertLabels(dockposition);
                        if (Navigator.TrackSize - (txtblockwidth * lowerLabelBar.Children.Count) > (txtblockwidth * 30))
                        {
                            if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Week"))
                            {
                                if (navigatorIntervals.Contains("Month"))
                                    SetMonthInterval(0, "Upper", formatters[navigatorIntervals.IndexOf("Month")]);
                                else
                                    SetMonthInterval(0, "Upper", null);
                            }

                            if (navigatorIntervals.Contains("Week"))
                                SetWeekInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Week")]);
                            else if (navigatorIntervals.Contains("Day"))
                                SetDayInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Day")]);
                            else if (navigatorIntervals.Count == 0)
                                SetWeekInterval(0, "Lower", null);
                        }
                    }
                    else if (dockposition == "Upper" && GenerateLabelContainers(dockposition))
                    {
                        InsertLabels(dockposition);
                    }
                    else
                    {
                        if (navigatorIntervals.Contains("Month"))
                            SetMonthInterval(1, dockposition, formatters[navigatorIntervals.IndexOf("Month")]);
                        else
                            SetMonthInterval(1, dockposition, null);
                    }

                    break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("MMMM", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);

                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        if (currentDate == minimumDateTimeValue)
                        {
                            if (currentDate.Month == 12)
                                currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            else
                                currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                        }
                        else
                            currentDate = currentDate.AddMonths(1);
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                    {
                        if (navigatorIntervals.Contains("Month"))
                            SetMonthInterval(2, dockposition, formatters[navigatorIntervals.IndexOf("Month")]);
                        else
                            SetMonthInterval(2, dockposition, null);
                    }

                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("MMM", CultureInfo.CurrentCulture);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        if (currentDate == minimumDateTimeValue)
                        {
                            if (currentDate.Month == 12)
                                currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            else
                                currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                        }
                        else
                            currentDate = currentDate.AddMonths(1);
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else
                    {
                        if (navigatorIntervals.Contains("Month"))
                            SetMonthInterval(3, dockposition, formatters[navigatorIntervals.IndexOf("Month")]);
                        else
                            SetMonthInterval(3, dockposition, null);
                    }

                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (formatter == null || isFormatterEmpty)
                            content = currentDate.ToString("MMM", CultureInfo.CurrentCulture).Substring(0, 1);
                        else
                            if (!string.IsNullOrEmpty(format))
                            content = currentDate.ToString(format, CultureInfo.CurrentCulture);

                        if (dockposition == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        if (currentDate == minimumDateTimeValue)
                        {
                            if (currentDate.Month == 12)
                                currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            else
                                currentDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                        }
                        else
                            currentDate = currentDate.AddMonths(1);
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockposition))
                        InsertLabels(dockposition);
                    else if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Quarter"))
                    {
                        if (navigatorIntervals.Contains("Quarter"))
                            SetQuarterInterval(0, dockposition, formatters[navigatorIntervals.IndexOf("Quarter")]);
                        else
                            SetQuarterInterval(0, dockposition, null);
                    }
                    else
                        ClearLabels(dockposition);
                    break;
                default:
                    break;
            }

            if (isFormatterEmpty)
                isFormatterEmpty = false;
        }

        /// <summary>
        /// Clears the labels according to the dock position.
        /// </summary>
        /// <param name="dockposition">The Dock Position</param>
        private void ClearLabels(string dockposition)
        {
            if (dockposition == "Lower")
            {
                lowerBarLabels.Clear();
                if (lowerLabelBounds != null)
                    lowerLabelBounds.Clear();
            }
            else
            {
                upperBarLabels.Clear();
                if (upperLabelBounds != null)
                    upperLabelBounds.Clear();
            }

            GenerateLabelContainers(dockposition);
        }

        /// <summary>
        /// Sets the quarter interval.
        /// </summary>
        /// <param name="quarterinterval">The Quarter Interval.</param>
        /// <param name="dockpostion">The Dock Position</param>
        /// <param name="formatter">The Formatter</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        private void SetQuarterInterval(int quarterinterval, string dockpostion, ObservableCollection<string> formatter)
        {
            string format = string.Empty;
            bool isadded;
            if (formatter != null && formatter.Count == 0)
            {
                isFormatterEmpty = true;
            }

            if (formatter != null && !isFormatterEmpty)
            {
                for (int i = 0; i < formatter.Count; i++)
                {
                    isadded = AddLabels(formatter[i].ToString(), dockpostion, "Quarter");
                    if (isadded)
                    {
                        format = formatter[i].ToString();
                        break;
                    }
                }
            }

            txtblockwidth = 0;
            daysvalue.Clear();
            if (dockpostion == "Lower")
                lowerBarLabels.Clear();
            else
                upperBarLabels.Clear();

            labelElementBounds = null;
            var currentDate = minimumDateTimeValue;
            string content = string.Empty;

            switch (quarterinterval)
            {
                case 0:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (currentDate.Month >= 1 && currentDate.Month <= 3)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Quarter + " " + "1" + ", " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 4, 1);
                        }
                        else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Quarter + " " + "2" + ", " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 7, 1);
                        }
                        else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Quarter + " " + "3" + ", " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 10, 1);
                        }
                        else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Quarter + " " + "4" + ", " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                        }

                        if (dockpostion == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });

                        AddDaysValues(currentDate);
                    }

                    if (dockpostion == "Lower" && GenerateLabelContainers(dockpostion))
                    {
                        InsertLabels(dockpostion);
                        if (Navigator.TrackSize - (txtblockwidth * lowerLabelBar.Children.Count) > txtblockwidth / 2)
                        {
                            if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Month"))
                                SetQuarterInterval(0, "Upper", null);
                            if (navigatorIntervals.Contains("Month"))
                                SetMonthInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Month")]);
                            else if (navigatorIntervals.Contains("Week"))
                                SetWeekInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Week")]);
                            else if (navigatorIntervals.Contains("Day"))
                                SetDayInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Day")] as ObservableCollection<string>);
                            else if (navigatorIntervals.Count == 0)
                                SetMonthInterval(0, "Lower", null);
                        }
                    }
                    else if (dockpostion == "Upper" && GenerateLabelContainers(dockpostion))
                    {
                        InsertLabels(dockpostion);
                    }
                    else
                    {
                        if (navigatorIntervals.Contains("Quarter"))
                            SetQuarterInterval(1, dockpostion, formatters[navigatorIntervals.IndexOf("Quarter")]);
                        else
                            SetQuarterInterval(1, dockpostion, null);
                    }

                    break;
                case 1:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (currentDate.Month >= 1 && currentDate.Month <= 3)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Quarter + " " + "1" + ", " + currentDate.ToString("yy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 4, 1);
                        }
                        else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Quarter + " " + "2" + ", " + currentDate.ToString("yy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 7, 1);
                        }
                        else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Quarter + " " + "3" + ", " + currentDate.ToString("yy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 10, 1);
                        }
                        else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Quarter + " " + "4" + ", " + currentDate.ToString("yy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                        }

                        if (dockpostion == "Lower")
                        {
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        }
                        else
                        {
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        }

                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockpostion))
                        InsertLabels(dockpostion);
                    else
                    {
                        if (navigatorIntervals.Contains("Quarter"))
                            SetQuarterInterval(2, dockpostion, formatters[navigatorIntervals.IndexOf("Quarter")]);
                        else
                            SetQuarterInterval(2, dockpostion, null);
                    }

                    break;
                case 2:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (currentDate.Month >= 1 && currentDate.Month <= 3)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Q + "1" + ", " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 4, 1);
                        }
                        else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Q + "2" + ", " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 7, 1);
                        }
                        else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Q + "3" + ", " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 10, 1);
                        }
                        else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Q + "4" + ", " + currentDate.ToString("yyyy", CultureInfo.CurrentCulture);
                            else
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                        }

                        if (dockpostion == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockpostion))
                        InsertLabels(dockpostion);
                    else
                    {
                        if (navigatorIntervals.Contains("Quarter"))
                            SetQuarterInterval(3, dockpostion, formatters[navigatorIntervals.IndexOf("Quarter")]);
                        else
                            SetQuarterInterval(3, dockpostion, null);
                    }

                    break;
                case 3:
                    while (currentDate <= maximumDateTimeValue)
                    {
                        if (currentDate.Month >= 1 && currentDate.Month <= 3)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Q + "1";
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 4, 1);
                        }
                        else if (currentDate.Month >= 4 && currentDate.Month <= 6)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Q + "2";
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 7, 1);
                        }
                        else if (currentDate.Month >= 7 && currentDate.Month <= 9)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Q + "3";
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year, 10, 1);
                        }
                        else if (currentDate.Month >= 10 && currentDate.Month <= 12)
                        {
                            if (formatter == null || isFormatterEmpty)
                                content = SfChartResourceWrapper.Q + "4";
                            else
                                if (!string.IsNullOrEmpty(format))
                                content = currentDate.ToString(format, CultureInfo.CurrentCulture);
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                        }

                        if (dockpostion == "Lower")
                            lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        else
                            upperBarLabels.Add(new ChartAxisLabel() { LabelContent = content });
                        AddDaysValues(currentDate);
                    }

                    if (GenerateLabelContainers(dockpostion))
                        InsertLabels(dockpostion);
                    else if (navigatorIntervals.Count == 0 || navigatorIntervals.Contains("Year"))
                    {
                        ClearLabels("Upper");
                        if (navigatorIntervals.Contains("Year"))
                            SetYearInterval(0, "Lower", formatters[navigatorIntervals.IndexOf("Year")]);
                        else
                            SetYearInterval(0, "Lower", null);
                    }
                    else
                        ClearLabels(dockpostion);
                    break;
                default:
                    break;
            }

            if (isFormatterEmpty)
                isFormatterEmpty = false;
        }

        /// <summary>
        /// Sets the year interval
        /// </summary>
        /// <param name="yearInterval">The Year Interval</param>
        /// <param name="dockposition">The Dock Position</param>
        /// <param name="formatter">The Formatter</param>
        private void SetYearInterval(int yearInterval, string dockposition, ObservableCollection<string> formatter)
        {
            if (isMinMaxSet)
            {
                bool isLastCategory = false;
                string format = string.Empty;
                bool isadded;

                if (formatter != null && formatter.Count == 0)
                {
                    isFormatterEmpty = true;
                }

                if (formatter != null && !isFormatterEmpty)
                {
                    for (int i = 0; i < formatter.Count; i++)
                    {
                        isadded = AddLabels(formatter[i].ToString(), dockposition, "Year");
                        if (isadded)
                        {
                            format = formatter[i].ToString();
                            break;
                        }
                    }
                }

                if (dockposition == "Upper")
                    upperBarLabels.Clear();
                else
                    lowerBarLabels.Clear();
                daysvalue.Clear();
                var currentDate = minimumDateTimeValue;
                switch (yearInterval)
                {
                    case 0:
                        while (currentDate <= maximumDateTimeValue)
                        {
                            if (dockposition == "Upper")
                            {
                                if (formatter == null || isFormatterEmpty)
                                    upperBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString("yyyy", CultureInfo.CurrentCulture) });
                                else
                                    if (!string.IsNullOrEmpty(format))
                                    upperBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString(format, CultureInfo.CurrentCulture) });
                            }
                            else
                            {
                                if (formatter == null || isFormatterEmpty)
                                    lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString("yyyy", CultureInfo.CurrentCulture) });
                                else
                                    if (!string.IsNullOrEmpty(format))
                                    lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString(format, CultureInfo.CurrentCulture) });
                            }

                            if ((currentDate - minimumDateTimeValue).TotalDays != 0)
                                daysvalue.Add((currentDate - minimumDateTimeValue).TotalDays);
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                        }

                        break;
                    case 1:
                        while (currentDate <= maximumDateTimeValue)
                        {
                            if (dockposition == "Upper")
                            {
                                if (formatter == null || isFormatterEmpty)
                                    upperBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString("yy", CultureInfo.CurrentCulture) });
                                else
                                    if (!string.IsNullOrEmpty(format))
                                    upperBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString(format, CultureInfo.CurrentCulture) });
                            }
                            else
                            {
                                if (formatter == null || isFormatterEmpty)
                                    lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString("yy", CultureInfo.CurrentCulture) });
                                else
                                    if (!string.IsNullOrEmpty(format))
                                    lowerBarLabels.Add(new ChartAxisLabel() { LabelContent = currentDate.ToString(format, CultureInfo.CurrentCulture) });
                            }

                            if ((currentDate - minimumDateTimeValue).TotalDays != 0)
                                daysvalue.Add((currentDate - minimumDateTimeValue).TotalDays);
                            currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                            isLastCategory = true;
                        }

                        break;
                    default:
                        break;
                }

                daysvalue.Add((maximumDateTimeValue - minimumDateTimeValue).TotalDays);
                if (GenerateLabelContainers(dockposition))
                    InsertLabels(dockposition);
                else if (!isLastCategory)
                {
                    if (navigatorIntervals.Contains("Year"))
                        SetYearInterval(1, dockposition, formatters[navigatorIntervals.IndexOf("Year")]);
                    else
                        SetYearInterval(1, dockposition, null);
                }
                else
                    ClearLabels(dockposition);
            }

            if (isFormatterEmpty)
                isFormatterEmpty = false;

            if (navigatorIntervals.Contains("Quarter") && dockPosition != "Lower")
                SetQuarterInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Quarter")]);
            else if (navigatorIntervals.Contains("Month") && dockPosition != "Lower")
                SetMonthInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Month")]);
            else if (navigatorIntervals.Contains("Week") && dockPosition != "Lower")
                SetWeekInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Week")]);
            else if (navigatorIntervals.Contains("Day") && dockPosition != "Lower")
                SetDayInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Day")]);
            else if (navigatorIntervals.Contains("Hour") && dockPosition != "Lower")
                SetHourInterval(0, dockPosition, formatters[navigatorIntervals.IndexOf("Hour")]);
        }

        /// <summary>
        /// Generates the label containers.
        /// </summary>
        /// <param name="postion">The Position</param>
        /// <returns>Returns a value indicating whether the total text block width is less than the track size.</returns>
        private bool GenerateLabelContainers(string postion)
        {
            txtblockwidth = 0;
            int i = 0;
            ObservableCollection<ChartAxisLabel> labels = new ObservableCollection<ChartAxisLabel>();
            UIElementsRecycler<TextBlock> labelsRecyler = null;
            Panel panel;
            if (postion == "Upper")
            {
                labelsRecyler = upperLabelRecycler;
                labels = upperBarLabels;
                panel = upperLabelBar;
                upperTickLineRecycler.GenerateElements(upperBarLabels.Count);
                upperGridLineRecycler.GenerateElements(upperBarLabels.Count);
            }
            else
            {
                labelsRecyler = lowerLabelRecycler;
                labels = lowerBarLabels;
                panel = lowerLabelBar;
                lowerTickLineRecycler.GenerateElements(lowerBarLabels.Count);
                lowerGridLineRecycler.GenerateElements(lowerBarLabels.Count);
            }

            labelsRecyler.GenerateElements(labels.Count);
            var labelsCount = labels.Count;
            if (postion == "Upper")
            {
                var higherBarLabelsCreatedEventArgs = new HigherBarLabelsCreatedEventArgs();
                foreach (var label in labels)
                {
                    higherBarLabelsCreatedEventArgs.HigherBarLabels.Add(new RangeNavigatorLabel { Content = label.LabelContent });
                }
                
				if(HigherBarLabelsCreated != null)
				{
					HigherBarLabelsCreated.Invoke(this, higherBarLabelsCreatedEventArgs);
				}

				for (int j = 0; j < labelsCount; j++)
                {
                    labels[j].LabelContent = higherBarLabelsCreatedEventArgs.HigherBarLabels[j].Content;
                }
                
            }
            else
            {
                var lowerBarLabelsCreatedEventArgs = new LowerBarLabelsCreatedEventArgs();
                foreach (var label in labels)
                {
                    lowerBarLabelsCreatedEventArgs.LowerBarLabels.Add(new RangeNavigatorLabel { Content = label.LabelContent });
                }
                
				if(LowerBarLabelsCreated != null)
				{
					LowerBarLabelsCreated.Invoke(this, lowerBarLabelsCreatedEventArgs);
				}

				for (int j = 0; j < labelsCount; j++)
                {
                    labels[j].LabelContent = lowerBarLabelsCreatedEventArgs.LowerBarLabels[j].Content;
                }
                
            }

            foreach (var label in labels)
            {
                TextBlock textBlock = labelsRecyler[i];
                textBlock.Text = label.LabelContent.ToString();
                textBlock.Visibility = Visibility.Visible;
                //// textBlock.Style = labelStyle;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                i++;
                textBlock.Measure(Navigator.DesiredSize);
                txtblockwidth = Math.Max(textBlock.ActualWidth, txtblockwidth);
            }

            return (txtblockwidth * panel.Children.Count < Navigator.TrackSize);
        }

        /// <summary>
        /// Method to calculate the selected data.
        /// </summary>
        private void OnInternalValueChanged()
        {
            if (!isUpdate)
            {
                CalculateSelectedData();
            }

            OnValueChanged();
        }

        /// <summary>
        /// Method called to reset the timer.
        /// </summary>
        private void ResetTimer()
        {
            if (timer != null)
            {
                timer.Stop();

                timer.Interval = TimeSpan.FromSeconds(DeferredUpdateDelay);

                timer.Start();
            }
            else
            {
                timer = new DispatcherTimer();
                timer.Tick += OnTimeout;
            }
        }

        /// <summary>
        /// Method called on every tick of timer.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnTimeout(object sender, object e)
        {
            if (EnableDeferredUpdate)
            {
                OnInternalValueChanged();
            }

            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= OnTimeout;
            }

            timer = null;
        }

        /// <summary>
        /// Clears the navigator labels.
        /// </summary>
        private void ClearLabels()
        {
            minimumDateTimeValue = DateTime.MinValue;
            maximumDateTimeValue = DateTime.MinValue;
            ClearNavigatorLabels();
            if (upperLabelBar != null && lowerLabelBar != null)
            {
                GenerateLabelContainers("Upper");
                GenerateLabelContainers("Lower");
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Clears the navigator labels.
        /// </summary>
        private void ClearNavigatorLabels()
        {
            if (upperBarLabels != null)
                upperBarLabels.Clear();
            if (lowerBarLabels != null)
                lowerBarLabels.Clear();
            if (lowerLabelBounds != null)
                lowerLabelBounds.Clear();
            if (upperLabelBounds != null)
                upperLabelBounds.Clear();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents the <see cref="ThumbStyle"/> class.
    /// </summary>
    public partial class ThumbStyle : FrameworkElement
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="LineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register(
                "LineStyle",
                typeof(Style),
                typeof(ThumbStyle),
                new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SymbolTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty SymbolTemplateProperty =
            DependencyProperty.Register(
                "SymbolTemplate",
                typeof(DataTemplate),
                typeof(ThumbStyle),
                new PropertyMetadata(null, OnPropertyChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the style for the thumb line.
        /// </summary>
        public Style LineStyle
        {
            get { return (Style)GetValue(LineStyleProperty); }
            set { SetValue(LineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data template for the symbol.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate SymbolTemplate
        {
            get { return (DataTemplate)GetValue(SymbolTemplateProperty); }
            set { SetValue(SymbolTemplateProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the <see cref="SfDateTimeRangeNavigator"/>.
        /// </summary>
        internal SfDateTimeRangeNavigator Navigator
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Private Static Methods

        /// <summary>
        /// Updates the thumb style on it's property change.
        /// </summary>
        /// <param name="d">The Dependency Property</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thumbStyle = d as ThumbStyle;
            if (thumbStyle.Navigator != null)
            {
                thumbStyle.Navigator.SetThumbStyle();
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents the <see cref="ResizeCanvas"/> class.
    /// </summary>
    public partial class ResizeCanvas : Canvas
    {
        #region Methods

        #region Protected Methods

        /// <summary>
        /// Measures the resize canvas elements.
        /// </summary>
        /// <param name="constraint">The Size Constraint</param>
        /// <returns>Returns the size to arrange the children.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            double maxHeight = 0;
            double maxWidth = 0;
            foreach (UIElement element in Children)
            {
                if (element != null)
                {
                    element.Measure(availableSize);
                    double left = Canvas.GetLeft(element);
                    double top = element.DesiredSize.Height;
                    left += element.DesiredSize.Width;
                    maxWidth = maxWidth < left ? left : maxWidth;
                    maxHeight = maxHeight < top ? top : maxHeight;
                }
            }

            return new Size { Height = maxHeight == 0 ? 17 : maxHeight, Width = constraint.Width };
        }

        /// <summary>
        /// Arranges the resize canvas elements.
        /// </summary>
        /// <param name="arrangeSize">The Arrange Size</param>
        /// <returns>Returns the arranged size.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement element in Children)
            {
                if (element is TextBlock)
                {
                    double pos = (arrangeSize.Height / 2) - (element.DesiredSize.Height / 2);
                    Canvas.SetTop(element, pos);
                }
            }

            return base.ArrangeOverride(arrangeSize);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a collection of Interval. 
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection{Syncfusion.UI.Xaml.Charts.Interval}" />
    public partial class Intervals : ObservableCollection<Interval>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Intervals"/> class.
        /// </summary>
        public Intervals()
        {
        }

        #endregion
    }

    /// <summary>
    /// Represents a dependency object that contains the types of interval for axis. 
    /// </summary>
    /// <seealso cref="System.Windows.DependencyObject" />
    public partial class Interval : FrameworkElement
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="IntervalType"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalTypeProperty =
            DependencyProperty.Register(
                "IntervalType",
                typeof(NavigatorIntervalType),
                typeof(Interval),
                new PropertyMetadata(NavigatorIntervalType.Year));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelFormatters"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelFormattersProperty =
            DependencyProperty.Register(
                "LabelFormatters",
                typeof(ObservableCollection<string>),
                typeof(Interval),
                new PropertyMetadata(null));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> class.
        /// </summary>
        public Interval()
        {
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets interval type in which the navigator values should be displayed.
        /// </summary>
        public NavigatorIntervalType IntervalType
        {
            get { return (NavigatorIntervalType)GetValue(IntervalTypeProperty); }
            set { SetValue(IntervalTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets string collection to set the label format for the navigator labels.
        /// </summary>
        public ObservableCollection<string> LabelFormatters
        {
            get { return (ObservableCollection<string>)GetValue(LabelFormattersProperty); }
            set { SetValue(LabelFormattersProperty, value); }
        }

        #endregion

        #endregion        
    }

    /// <summary>
    /// Lower bar labels created event arguments.
    /// </summary>
    public partial class LowerBarLabelsCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the lower bar labels.
        /// </summary>
        public List<RangeNavigatorLabel> LowerBarLabels { get; internal set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="LowerBarLabelsCreatedEventArgs"/> class.
        /// </summary>
        public LowerBarLabelsCreatedEventArgs()
        {
            LowerBarLabels = new List<RangeNavigatorLabel>();
        }
    }

    /// <summary>
    /// Higher bar labels created event arguments.
    /// </summary>
    public partial class HigherBarLabelsCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets and sets the higher bar labels.
        /// </summary>
        public List<RangeNavigatorLabel> HigherBarLabels { get; internal set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HigherBarLabelsCreatedEventArgs"/> class.
        /// </summary>
        public HigherBarLabelsCreatedEventArgs()
        {
            HigherBarLabels = new List<RangeNavigatorLabel>();
        }
    }

    /// <summary>
    /// Serves as the label type for MinorScale and MajorScale labels.
    /// </summary>
    public partial class RangeNavigatorLabel
    {
        /// <summary>
        /// Gets and sets the labels content.
        /// </summary>
        public object Content { get; set; }
    }
}