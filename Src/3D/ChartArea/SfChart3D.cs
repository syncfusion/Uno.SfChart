// <copyright file="SfChart3D.cs" company="Syncfusion. Inc">
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
    using Windows.ApplicationModel;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Represents the 3DChart control which is used to visualize the data graphically in three dimension.
    /// </summary>
    /// <remarks>
    /// The Chart is often used to make it easier to
    /// understand large amount of data and the relationship between different parts
    /// of the data. Chart can usually be read more quickly than the raw data that they
    /// come from. Certain <see cref="ChartSeries3D" /> are more useful for
    /// presenting a given data set than others. For example, data that presents
    /// percentages in different groups (such as "satisfied, not satisfied, unsure") are
    /// often displayed in a <see cref="PieSeries3D" /> chart, but are more easily
    /// understood when presented in a horizontal <see cref="BarSeries3D" /> chart.
    /// </remarks>
    /// <seealso cref="ChartSeries3D"/>
    /// <seealso cref="ChartLegend"/>
    /// <seealso cref="ChartAxisBase3D"/>

    [ContentProperty(Name = "Series")]
    public partial class SfChart3D : ChartBase 
    {
        #region Dependency Property Registration
        
        /// <summary>
        ///  The DependencyProperty for <see cref="DepthAxis"/> property.
        /// </summary>
        public static readonly DependencyProperty DepthAxisProperty =
            DependencyProperty.Register(
                "DepthAxis", 
                typeof(ChartAxisBase3D), 
                typeof(SfChart3D),
                new PropertyMetadata(null, OnDepthAxisChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="WallSize"/> property.
        /// </summary>
        public static readonly DependencyProperty WallSizeProperty =
            DependencyProperty.Register(
                "WallSize", 
                typeof(double), 
                typeof(SfChart3D), 
                new PropertyMetadata(10d));
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableRotation"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableRotationProperty =
            DependencyProperty.Register(
                "EnableRotation", 
                typeof(bool), 
                typeof(SfChart3D), 
                new PropertyMetadata(false));
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableSeriesSelection"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableSeriesSelectionProperty =
            DependencyProperty.Register(
                "EnableSeriesSelection", 
                typeof(bool), 
                typeof(SfChart3D), 
                new PropertyMetadata(false, OnEnableSeriesSelectionChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableSegmentSelection"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableSegmentSelectionProperty =
            DependencyProperty.Register(
                "EnableSegmentSelection", 
                typeof(bool), 
                typeof(SfChart3D), 
                new PropertyMetadata(true, OnEnableSegmentSelectionChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SelectionStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionStyleProperty =
           DependencyProperty.Register(
               "SelectionStyle", 
               typeof(SelectionStyle3D), 
               typeof(SfChart3D), 
               new PropertyMetadata(SelectionStyle3D.Single, OnSelectionStyleChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="TopWallBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty TopWallBrushProperty =
           DependencyProperty.Register(
               "TopWallBrush", 
               typeof(Brush), 
               typeof(SfChart3D), 
               new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 211, 211, 211)), OnTopWallBrushChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="BottomWallBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty BottomWallBrushProperty =
            DependencyProperty.Register(
                "BottomWallBrush", 
                typeof(Brush), 
                typeof(SfChart3D), 
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 211, 211, 211)), OnBottomWallBrushChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="RightWallBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty RightWallBrushProperty =
            DependencyProperty.Register(
                "RightWallBrush", 
                typeof(Brush), 
                typeof(SfChart3D), 
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 211, 211, 211)), OnRightWallBrushChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="LeftWallBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty LeftWallBrushProperty =
            DependencyProperty.Register(
                "LeftWallBrush",
                typeof(Brush), 
                typeof(SfChart3D), 
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 211, 211, 211)), OnLeftWallBrushChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="BackWallBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty BackWallBrushProperty =
            DependencyProperty.Register(
                "BackWallBrush", 
                typeof(Brush), 
                typeof(SfChart3D), 
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 211, 211, 211)), OnBackWallBrushChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="PerspectiveAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty PerspectiveAngleProperty =
            DependencyProperty.Register(
                "PerspectiveAngle", 
                typeof(double), 
                typeof(SfChart3D), 
                new PropertyMetadata(90d, OnPerspectiveAngleChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="PrimaryAxis"/> property.
        /// </summary>
        public static readonly DependencyProperty PrimaryAxisProperty =
            DependencyProperty.Register(
                "PrimaryAxis", 
                typeof(ChartAxisBase3D),
                typeof(SfChart3D), 
                new PropertyMetadata(null, OnPrimaryAxisChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="SecondaryAxis"/> property.
        /// </summary>
        public static readonly DependencyProperty SecondaryAxisProperty =
            DependencyProperty.Register(
                "SecondaryAxis",
                typeof(RangeAxisBase3D), 
                typeof(SfChart3D), 
                new PropertyMetadata(null, OnSecondaryAxisChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="Series"/> property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                "Series", 
                typeof(ChartSeries3DCollection),
                typeof(SfChart3D), 
                new PropertyMetadata(null, OnSeriesPropertyCollectionChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="Tilt"/> property.
        /// </summary>
        public static readonly DependencyProperty TiltProperty =
            DependencyProperty.Register(
                "Tilt", 
                typeof(double),
                typeof(SfChart3D),
                new PropertyMetadata(0d, OnTiltPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="Depth"/> property.
        /// </summary>
        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register(
                "Depth", 
                typeof(double), 
                typeof(SfChart3D), 
                new PropertyMetadata(100d, OnDepthPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="Rotation"/> property.
        /// </summary>
        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register(
                "Rotation", 
                typeof(double), 
                typeof(SfChart3D), 
                new PropertyMetadata(0d, OnRotationPropertyChanged));

        #if WINDOWS_UAP

        /// <summary>
        /// The DependencyProperty for <see cref="SelectionCursor"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionCursorProperty =
            DependencyProperty.Register("SelectionCursor", typeof(CoreCursorType), typeof(SfChart3D), new PropertyMetadata(CoreCursorType.Arrow));


        #endif

        #endregion

        #region Fields

        #region Private Fields

        /// <summary>
        /// Backing store for ZMinPointsDelta/>
        /// </summary>
        private double zminPointsDelta = double.NaN;
        
        /// <summary>
        /// Plans of the left side wall.
        /// </summary>
        private Polygon3D[] leftSideWallPlans = null;

        /// <summary>
        /// Plans of the bottom side wall.
        /// </summary>
        private Polygon3D[] bottomSideWallPlans = null;

        /// <summary>
        /// Plans of the top side wall.
        /// </summary>
        private Polygon3D[] topSideWallPlans = null;

        /// <summary>
        /// Plans of the right side wall.
        /// </summary>
        private Polygon3D[] rightSideWallPlans = null;

        /// <summary>
        /// Represents the current series.
        /// </summary>
        private ChartSeries3D currentSeries = null;

        /// <summary>
        /// Used to temporarily store the series for display tooltip hovering on adornment or adornment label.
        /// </summary>       
        private object series = null;

        /// <summary>
        /// Holds the previous chart position.
        /// </summary>
        private Point previousChartPosition;

        /// <summary>
        /// Checks the rotation activation
        /// </summary>
        private bool rotationActivated;
        
        /// <summary>
        /// Holds the previous auto depth.
        /// </summary>
        private double previousAutoDepth;

        /// <summary>
        /// Holds the sum by index.
        /// </summary>
        private Dictionary<int, double> sumByIndex = new Dictionary<int, double>();
        
        /// <summary>
        /// Checks whether the 3D schedule is updated.
        /// </summary>
        private bool is3DUpdateScheduled;

        /// <summary>
        /// Represents the segment on which the mouse is moved.
        /// </summary>
        private ChartSegment mouseMoveSegment;
        
        /// <summary>
        /// Represents the controls presenter.
        /// </summary>
        private Panel controlsPresenter;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SfChart3D"/> class.
        /// </summary>
        public SfChart3D()
        {

#if UNIVERSALWINDOWS && SyncfusionLicense
            SfChart3D.ValidateLicense();
#endif

            this.Graphics3D = new Graphics3D();
            this.ActualRotationAngle = 0d;
            this.ActualTiltAngle = 0d;
            this.DefaultStyleKey = typeof(SfChart3D);

            this.UpdateAction = UpdateAction.Invalidate;
            this.Series = new ChartSeries3DCollection();
            this.VisibleSeries = new ChartVisibleSeriesCollection();
            this.Axes = new ChartAxisCollection();


            this.ManipulationMode = ManipulationModes.Scale
                               | ManipulationModes.TranslateRailsX
                               | ManipulationModes.TranslateRailsY
                               | ManipulationModes.TranslateX
                               | ManipulationModes.TranslateY
                               | ManipulationModes.TranslateInertia
                               | ManipulationModes.Rotate;


            this.ColorModel = new ChartColorModel(Palette);

        }

#endregion

#region Properties

#region Public Properties

        /// <summary>
        /// Gets or sets the horizontal axis(Z) for the <c>SfChart3D</c>.
        /// </summary>
        public ChartAxisBase3D DepthAxis
        {
            get { return (ChartAxisBase3D)GetValue(DepthAxisProperty); }
            set { this.SetValue(DepthAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the axis wall. 
        /// </summary>
        public double WallSize
        {
            get { return (double)GetValue(WallSizeProperty); }
            set { this.SetValue(WallSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the rotation is enabled for <c>SfChart3D</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if rotation is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRotation
        {
            get { return (bool)GetValue(EnableRotationProperty); }
            set { this.SetValue(EnableRotationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the series selection is enabled or not.
        /// </summary>
        public bool EnableSeriesSelection
        {
            get { return (bool)GetValue(EnableSeriesSelectionProperty); }
            set { this.SetValue(EnableSeriesSelectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the segment (or) data point selection is enabled or not.
        /// </summary>
        public bool EnableSegmentSelection
        {
            get { return (bool)GetValue(EnableSegmentSelectionProperty); }
            set { this.SetValue(EnableSegmentSelectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of selection. By Default Single Selection is enabled.
        /// </summary>
        /// <value>
        ///     <c>SelectionStyle3D.Single</c> for selecting single point/series; 
        ///     <c>SelectionStylr3D.Multiple</c> for selecting multiple point/series. 
        /// </value>
        public SelectionStyle3D SelectionStyle
        {
            get { return (SelectionStyle3D)GetValue(SelectionStyleProperty); }
            set { this.SetValue(SelectionStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pointer cursor for the series, which indicates that this series is selectable
        /// </summary>
        /// <remarks>
        /// Default value is Arrow
        /// </remarks>
        public CoreCursorType SelectionCursor
        {
            get { return (CoreCursorType)GetValue(SelectionCursorProperty); }
            set { this.SetValue(SelectionCursorProperty, value); }
        }


        /// <summary>
        /// Gets or sets the brush for the Top wall.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush TopWallBrush
        {
            get { return (Brush)GetValue(TopWallBrushProperty); }
            set { this.SetValue(TopWallBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for the Bottom wall.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BottomWallBrush
        {
            get { return (Brush)GetValue(BottomWallBrushProperty); }
            set { this.SetValue(BottomWallBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets brush for the Right wall.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush RightWallBrush
        {
            get { return (Brush)GetValue(RightWallBrushProperty); }
            set { this.SetValue(RightWallBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for Left wall.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush LeftWallBrush
        {
            get { return (Brush)GetValue(LeftWallBrushProperty); }
            set { this.SetValue(LeftWallBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for the Back wall.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BackWallBrush
        {
            get { return (Brush)GetValue(BackWallBrushProperty); }
            set { this.SetValue(BackWallBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the angle for the perspective view. By default its 90 degree.
        /// </summary>
        public double PerspectiveAngle
        {
            get { return (double)GetValue(PerspectiveAngleProperty); }
            set { this.SetValue(PerspectiveAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal axis(X) for the <c>SfChart3D</c>.
        /// </summary>
        public ChartAxisBase3D PrimaryAxis
        {
            get { return (ChartAxisBase3D)GetValue(PrimaryAxisProperty); }
            set { this.SetValue(PrimaryAxisProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the vertical axis(Y) for the <c>SfChart3D</c>.
        /// </summary>
        public RangeAxisBase3D SecondaryAxis
        {
            get { return (RangeAxisBase3D)GetValue(SecondaryAxisProperty); }
            set { this.SetValue(SecondaryAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the series added to the <c>SfChart3D</c>.
        /// </summary>
        /// <example>
        /// The following is an example for initializing the <c>Series</c>
        /// <code language="XAML">
        ///     &lt;syncfusion:SfChart&gt;
        ///         &lt;syncfusion:AreaSeries3d/&gt;
        ///         &lt;syncfusion:AreaSeries3d/&gt;
        ///     &lt;/syncfusion:SfChart&gt;
        /// </code>
        /// <code language="C#">
        ///     SfChart sfChart = new SfChart();
        ///     AreaSeries3D areaSereis1 = new AreaSeries3D();
        ///     AreaSeries3D areaSereis2 = new AreaSeries3D();
        ///     sfChart.Series.Add(areaSereis1);
        ///     sfChart.Series.Add(areaSereis2);
        /// </code>
        /// </example>
        public ChartSeries3DCollection Series
        {
            get { return (ChartSeries3DCollection)GetValue(SeriesProperty); }
            set { this.SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Tilt angle for the 3D charts.
        /// </summary>
        /// <remark>
        ///     The default angle is 0d.
        /// </remark>
        public double Tilt
        {
            get { return (double)GetValue(TiltProperty); }
            set { this.SetValue(TiltProperty, value); }
        }

        /// <summary>
        /// Gets or sets the depth of field for 3D view.
        /// </summary>
        /// <remark>
        ///     The default value is 100d.
        /// </remark>
        public double Depth
        {
            get { return (double)GetValue(DepthProperty); }
            set { this.SetValue(DepthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rotation angle for the 3D charts.
        /// </summary>
        /// <remark>
        ///     The default angle is 0d.
        /// </remark>
        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            set { this.SetValue(RotationProperty, value); }
        }

#endregion

#region Internal Properties

        /// <summary>
        /// Gets or sets the action for rendering the series.
        /// </summary>
        internal IAsyncAction RenderSeriesAction { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Graphics3D"/> for render the chart.
        /// </summary>
        internal Graphics3D Graphics3D { get; set; }
        
        /// <summary>
        /// Gets the spacing for the column typed series.
        /// </summary>
        internal double ZMinPointsDelta
        {
            get
            {
                this.zminPointsDelta = double.MaxValue;

                var xyzDataSeriesCollection = VisibleSeries.Where(x => x is XyzDataSeries3D);
                foreach (XyzDataSeries3D xyzDataSeries in xyzDataSeriesCollection)
                {
                    var zValues = xyzDataSeries.ActualZValues as List<double>;
                    if (xyzDataSeries.IsIndexed && zValues != null && xyzDataSeries.IsSideBySide)
                    {
                        this.GetMinPointsDelta(zValues, ref this.zminPointsDelta, xyzDataSeries, xyzDataSeries.IsIndexedZAxis);
                    }
                }

                this.zminPointsDelta = (this.zminPointsDelta == double.MaxValue || this.zminPointsDelta < 0) ? 1 : this.zminPointsDelta;

                return this.zminPointsDelta;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto depth need to be set.
        /// </summary>
        internal bool IsAutoDepth { get; set; }

        /// <summary>
        /// Gets or sets the root panel of the chart.
        /// </summary>
        internal Canvas RootPanel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the rotation schedule update is required.
        /// </summary>
        internal bool IsRotationScheduleUpdate { get; set; }

        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        internal double ActualRotationAngle { get; set; }

        /// <summary>
        /// Gets or sets the tilt angle.
        /// </summary>
        internal double ActualTiltAngle { get; set; }

        /// <summary>
        /// Gets or sets the actual depth of the chart.
        /// </summary>
        internal double ActualDepth { get; set; }

#endregion Internal Properties

#endregion

#region Methods

#region Public Methods

        /// <summary>
        ///  Method used to highlight selected index series.
        /// </summary>
        /// <param name="newIndex">The New Index</param>
        /// <param name="oldIndex">The Old Index</param>
        public override void SeriesSelectedIndexChanged(int newIndex, int oldIndex)
        {
            // Reset the oldIndex series Interior
            if (oldIndex < this.Series.Count && oldIndex >= 0 && this.SelectionStyle == SelectionStyle3D.Single)
            {
                ChartSeriesBase series = this.Series[oldIndex];

                if (SelectedSeriesCollection.Contains(series))
                {
                    SelectedSeriesCollection.Remove(series);
                }

                SfChart3D.OnResetSeries(series);
            }

            if (newIndex >= 0 && newIndex < this.Series.Count && this.GetEnableSeriesSelection())
            {
                ChartSeriesBase series = this.Series[newIndex];

                if (!SelectedSeriesCollection.Contains(series))
                {
                    SelectedSeriesCollection.Add(series);
                }

                // Set the SeriestSelectionBrush to newIndex series Interior
                foreach (ChartSegment3D segment in series.Segments)
                {
                    segment.BindProperties();

                    foreach (var item in segment.Polygons)
                    {
                        item.Fill = segment.Interior;
                        item.ReDraw();
                    }
                }

                ChartSelectionChangedEventArgs selectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                {
                    SelectedSegment = null,
                    SelectedSeries = series,
                    SelectedSeriesCollection = SelectedSeriesCollection,
                    SelectedIndex = newIndex,
                    PreviousSelectedIndex = oldIndex,
                    IsDataPointSelection = false,
                    IsSelected = true,
                    PreviousSelectedSeries = null,
                    PreviousSelectedSegment = null
                };

                if (oldIndex != -1)
                {
                    selectionChangedEventArgs.PreviousSelectedSeries = this.Series[oldIndex];
                }

                this.OnSelectionChanged(selectionChangedEventArgs);
            }
            else if (newIndex == -1)
            {
                this.OnSelectionChanged(new ChartSelectionChangedEventArgs()
                {
                    SelectedSegment = null,
                    SelectedSeries = null,
                    SelectedSeriesCollection = SelectedSeriesCollection,
                    SelectedIndex = newIndex,
                    PreviousSelectedIndex = oldIndex,
                    IsDataPointSelection = false,
                    PreviousSelectedSeries = this.Series[oldIndex],
                    PreviousSelectedSegment = null,
                    IsSelected = false
                });
            }
        }
        
        /// <summary>
        /// Converts point to value.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        /// The double point to value
        /// </returns>
        public override double PointToValue(ChartAxis axis, Point point)
        {
            var frontPlane = new Polygon3D(new Vector3D(0, 0, 1), 0);
            var transform = this.Graphics3D.Transform;
            frontPlane.Transform(transform.View);
            var actualPosition = transform.ToPlane(point, frontPlane);
            return base.PointToValue(axis, new Point(actualPosition.X, actualPosition.Y));
        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart Axis.</param>
        /// <param name="value">The Value.</param>
        /// <returns>
        /// The double value to point
        /// </returns>
        public override double ValueToPoint(ChartAxis axis, double value)
        {
            var actualValue = base.ValueToPoint(axis, value);
            if (axis.Orientation == Orientation.Horizontal)
            {
                return this.Graphics3D.Transform.ToScreen(new Vector3D(actualValue, 0, 0)).X;
            }
            else
            {
                return this.Graphics3D.Transform.ToScreen(new Vector3D(0, actualValue, 0)).Y;
            }
        }

#endregion

#region Internal Static Methods
        
        /// <summary>
        /// Updates the chart when reset the series.
        /// </summary>
        /// <param name="series">The Series</param>
        internal static void OnResetSeries(ChartSeriesBase series)
        {
            foreach (ChartSegment3D segment in series.Segments)
            {
                segment.BindProperties();

                foreach (var item in segment.Polygons)
                {
                    item.Fill = segment.Interior;
                    item.ReDraw();
                }
            }
        }

#endregion

#region Internal Methods

        /// <summary>
        ///  Set default axes for <see cref="SfChart3D"/>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Linq Statements")]
        internal void InitializeDefaultAxes()
        {
            if (this.PrimaryAxis == null || this.PrimaryAxis.IsDefault)
            {
                if (this.Series != null && this.Series.Count == 0)
                {
                    if (this.PrimaryAxis == null)
                    {
                        this.PrimaryAxis = new NumericalAxis3D() { IsDefault = true };
                    }
                }
                else
                {
                    // Get the XAxisValueType from the each series in Series collection which are having ActualXAxis as null
                    var valueTypes = (from series in this.Series
                                      where (series.ActualXAxis == null || series.ActualXAxis.IsDefault
                                      || !Axes.Contains(series.ActualXAxis))
                                      select series.XAxisValueType).ToList();

                    if (valueTypes.Count > 0)
                    {
                        this.SetPrimaryAxis(valueTypes[0]); // Set PrimaryAxis for SfChart based on XAxisValueType
                    }
                    else
                    {
                        this.InternalPrimaryAxis = this.Series[0].ActualXAxis;
                    }
                }
            }

            if (this.SecondaryAxis == null || this.SecondaryAxis.IsDefault)
            {
                if (this.Series != null && this.Series.Count == 0)
                {
                    if (this.SecondaryAxis == null)
                    {
                        this.SecondaryAxis = new NumericalAxis3D() { IsDefault = true };
                    }
                }
                else
                {
                    var seriesCollection = (from series in this.Series
                                            where (series.ActualYAxis == null || series.ActualYAxis.IsDefault
                                            || !Axes.Contains(series.ActualYAxis))
                                            select series).ToList();

                    if (seriesCollection.Count > 0 && this.SecondaryAxis == null)
                    {
                        this.SecondaryAxis = new NumericalAxis3D() { IsDefault = true };
                    }
                    else
                    {
                        this.InternalSecondaryAxis = this.Series[0].ActualYAxis;
                    }
                }
            }

            if (this.DepthAxis == null || this.DepthAxis.IsDefault)
            {
                if (this.Series.Count > 0 && this.Series.Any(x => x is XyzDataSeries3D && !string.IsNullOrEmpty((x as XyzDataSeries3D).ZBindingPath)))
                {
                    if (this.Series != null && this.Series.Count == 0)
                    {
                        if (this.DepthAxis == null)
                        {
                            this.DepthAxis = new NumericalAxis3D() { IsDefault = true };
                        }
                    }
                    else
                    {
                        // get the ZAxisValueType from the each series in Series collection which are having ActualZAxis as null
                        var valueTypes = (from series in this.Series
                                          where (series is XyzDataSeries3D && (series as XyzDataSeries3D).ActualZAxis == null
                                          || (series as XyzDataSeries3D).ActualZAxis.IsDefault
                                          || !Axes.Contains((series as XyzDataSeries3D).ActualZAxis))
                                          select (series as XyzDataSeries3D).ZAxisValueType).ToList();

                        if (valueTypes.Count > 0)
                        {
                            this.SetDepthAxis(valueTypes[0]); // Set DepthAxis for SfChart based on ZAxisValueType
                        }
                        else
                        {
                            this.InternalDepthAxis = (this.Series[0] as XyzDataSeries3D).ActualZAxis;
                        }
                    }
                }
            }
            else if (this.IsManhattanAxis())
            {
                if (this.DepthAxis != null && !(this.DepthAxis is CategoryAxis3D))
                {
                    this.DepthAxis = new CategoryAxis3D() { IsDefault = true };
                }
            }

            this.IsUpdateLegend = true;
        }

        /// <summary>
        /// Checks for the manhattan axis.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> value indicating whether Manhattan Axis Required.</returns>
        internal bool IsManhattanAxis()
        {
            return (VisibleSeries.Count > 1 && VisibleSeries.All((series => series is AreaSeries3D || series is LineSeries3D)));
        }

        /// <summary>
        /// Set PrimaryAxis for <see cref="SfChart3D"/>
        /// </summary>
        /// <param name="type">The Type</param>
        internal void SetPrimaryAxis(ChartValueType type)
        {
            switch (type)
            {
                case ChartValueType.Double:
                    if (this.PrimaryAxis == null || this.PrimaryAxis.GetType() != typeof(NumericalAxis3D))
                    {
                        this.PrimaryAxis = new NumericalAxis3D() { IsDefault = true };
                    }

                    break;

                case ChartValueType.DateTime:
                    if (this.PrimaryAxis == null || this.PrimaryAxis.GetType() != typeof(DateTimeAxis3D))
                    {
                        this.PrimaryAxis = new DateTimeAxis3D() { IsDefault = true };
                    }

                    break;

                case ChartValueType.String:
                    if (this.PrimaryAxis == null || this.PrimaryAxis.GetType() != typeof(CategoryAxis3D))
                    {
                        this.PrimaryAxis = new CategoryAxis3D() { IsDefault = true };
                    }

                    break;

                case ChartValueType.TimeSpan:
                    if (this.PrimaryAxis == null || this.PrimaryAxis.GetType() != typeof(TimeSpanAxis3D))
                    {
                        this.PrimaryAxis = new TimeSpanAxis3D() { IsDefault = true };
                    }

                    break;
            }
        }

        // Individual methods are used since it is not refered properly.

        /// <summary>
        /// Set DepthAxis for <see cref="SfChart3D"/>
        /// </summary>
        /// <param name="type">The Type</param>
        internal void SetDepthAxis(ChartValueType type)
        {
            switch (type)
            {
                case ChartValueType.Double:
                    if (this.DepthAxis == null || this.DepthAxis.GetType() != typeof(NumericalAxis3D))
                    {
                        this.DepthAxis = new NumericalAxis3D() { IsDefault = true };
                    }

                    break;

                case ChartValueType.DateTime:
                    if (this.DepthAxis == null || this.DepthAxis.GetType() != typeof(DateTimeAxis3D))
                    {
                        this.DepthAxis = new DateTimeAxis3D() { IsDefault = true };
                    }

                    break;

                case ChartValueType.String:
                    if (this.DepthAxis == null || this.DepthAxis.GetType() != typeof(CategoryAxis3D))
                    {
                        this.DepthAxis = new CategoryAxis3D() { IsDefault = true };
                    }

                    break;

                case ChartValueType.TimeSpan:
                    if (this.DepthAxis == null || this.DepthAxis.GetType() != typeof(TimeSpanAxis3D))
                    {
                        this.DepthAxis = new TimeSpanAxis3D() { IsDefault = true };
                    }

                    break;
            }
        }
                
        /// <summary>
        /// Clone the entire chart
        /// </summary>
        /// <returns>The Cloned Chart.</returns>
        internal override DependencyObject CloneChart()
        {
            var chart = new SfChart3D();
            ChartCloning.CloneControl(this, chart);
            chart.Height = double.IsNaN(this.Height) ? this.ActualHeight : this.Height;
            chart.Width = double.IsNaN(this.Width) ? this.ActualWidth : this.Width;
            chart.Header = this.Header;
            chart.Palette = this.Palette;
            chart.SideBySideSeriesPlacement = this.SideBySideSeriesPlacement;
            chart.EnableRotation = this.EnableRotation;
            chart.EnableSegmentSelection = this.EnableSegmentSelection;
            chart.EnableSeriesSelection = this.EnableSeriesSelection;
            chart.LeftWallBrush = this.LeftWallBrush;
            chart.SelectionStyle = this.SelectionStyle;
            chart.SelectionCursor = this.SelectionCursor;
            if (this.PrimaryAxis != null && this.PrimaryAxis is ChartAxisBase3D)
            {
                chart.PrimaryAxis = (ChartAxisBase3D)(this.PrimaryAxis as ICloneable).Clone();
            }

            if (this.SecondaryAxis != null && this.SecondaryAxis is RangeAxisBase3D)
            {
                chart.SecondaryAxis = (RangeAxisBase3D)(this.SecondaryAxis as ICloneable).Clone();
            }

            if (this.Legend != null)
            {
                chart.Legend = (ChartLegend)(Legend as ICloneable).Clone();
            }

            foreach (ChartSeriesBase series in this.Series)
            {
                chart.Series.Add((ChartSeries3D)(series as ICloneable).Clone());
            }

            foreach (var rowDefinition in this.RowDefinitions)
            {
                chart.RowDefinitions.Add((ChartRowDefinition)(rowDefinition as ICloneable).Clone());
            }

            foreach (var columnDefinition in this.ColumnDefinitions)
            {
                chart.ColumnDefinitions.Add((ChartColumnDefinition)(columnDefinition as ICloneable).Clone());
            }

            chart.UpdateArea(true);
            return chart;
        }

        /// <summary>
        /// Gets the percent by index.
        /// </summary>
        /// <param name="series">The Series</param>
        /// <param name="index">The Index</param>
        /// <param name="value">The Value</param>
        /// <param name="isReCalculation">Is Recalculation Required</param>
        /// <returns>The Percent</returns>
        internal double GetPercentByIndex(List<StackingSeriesBase3D> series, int index, double value, bool isReCalculation)
        {
            if (this.sumByIndex.Keys.Contains(index) && !isReCalculation)
            {
                return value / this.sumByIndex[index] * 100;
            }

            var result = series.Sum(item => item.YValues.Count != 0d && index < item.YValues.Count ? Math.Abs(double.IsNaN(item.YValues[index]) ? 0 : item.YValues[index]) : 0d);
            this.sumByIndex[index] = result;
            return value / this.sumByIndex[index] * 100;
        }

        /// <summary>
        /// Renders the series.
        /// </summary>
        internal void RenderSeries()
        {
            if (this.RootPanelDesiredSize != null)
            {
                this.Update3DWall();

                var size = RootPanelDesiredSize.Value;

                if (this.VisibleSeries != null)
                {
                    foreach (ChartSeries3D series in this.VisibleSeries)
                    {
                        series.UpdateOnSeriesBoundChanged(size);
                        if (series.AdornmentsInfo != null)
                        {
                            series.AdornmentsInfo.Arrange(size);
                        }

                        foreach (ChartSegment3D segment in series.Segments.OfType<ChartSegment3D>())
                        {
                            segment.Polygons.Clear();
                        }
                    }
                }

                this.Graphics3D.PrepareView(this.PerspectiveAngle, this.ActualDepth, this.ActualRotationAngle, this.ActualTiltAngle, size);
                this.Graphics3D.View(this.RootPanel);

                foreach (var item in VisibleSeries.Where(item => item.CanAnimate && item.Segments.Count > 0))
                {
                    item.Animate();
                    item.CanAnimate = false;
                }
            }


            this.RenderSeriesAction = null;
            this.StackedValues = null;
        }

        /// <summary>
        /// Updates the right wall.
        /// </summary>
        internal void UpdateRightWall()
        {
            Rect rightRect = new Rect(-(this.ActualDepth), SeriesClipRect.Top, this.ActualDepth, SeriesClipRect.Height);
            var tlfVector = new Vector3D(rightRect.Left, rightRect.Top, SeriesClipRect.Right + 1.5);
            var brbVector = new Vector3D(rightRect.Right, rightRect.Bottom, SeriesClipRect.Right + this.WallSize);
            this.rightSideWallPlans = Polygon3D.CreateBox(tlfVector, brbVector, this, 0, this.Graphics3D, this.RightWallBrush, this.RightWallBrush, 0, false, "RightWallBrush");

            foreach (Polygon3D plan in this.rightSideWallPlans)
            {
                plan.Transform(Matrix3D.Turn((float)(-Math.PI / 2)));
            }
        }

        /// <summary>
        /// Updates the left wall.
        /// </summary>
        internal void UpdateLeftWall()
        {
            Rect leftRect = new Rect(-(this.ActualDepth), SeriesClipRect.Top, this.ActualDepth, SeriesClipRect.Height);
            double offset = SeriesClipRect.Left;
            var tlfVector = new Vector3D(leftRect.Left, leftRect.Top, offset - 0.1);
            var brbVector = new Vector3D(leftRect.Right, leftRect.Bottom, offset - this.WallSize);
            this.leftSideWallPlans = Polygon3D.CreateBox(tlfVector, brbVector, this, 0, this.Graphics3D, this.LeftWallBrush, this.LeftWallBrush, 0, false, "LeftWallBrush");

            foreach (Polygon3D plan in this.leftSideWallPlans)
            {
                plan.Transform(Matrix3D.Turn((float)(-Math.PI / 2)));
            }
        }

        /// <summary>
        /// Updates the top wall.
        /// </summary>
        internal void UpdateTopWall()
        {
            Rect topRect = new Rect(SeriesClipRect.Left, -(int)this.ActualDepth, SeriesClipRect.Width, (int)this.ActualDepth);
            double offset = 0d;

            if (this.WallSize < SeriesClipRect.Top)
            {
                offset = SeriesClipRect.Top - this.WallSize;
            }
            else
            {
                offset = -(this.WallSize - SeriesClipRect.Top);
            }

            var tlfVector = new Vector3D(topRect.Right, topRect.Top, SeriesClipRect.Top - 0.1);
            var brbVector = new Vector3D(topRect.Left, topRect.Bottom - 0.1, offset);
            this.topSideWallPlans = Polygon3D.CreateBox(tlfVector, brbVector, this, 0, this.Graphics3D, this.TopWallBrush, this.TopWallBrush, 0, false, "TopWallBrush");
            foreach (Polygon3D plan in this.topSideWallPlans)
            {
                plan.Transform(Matrix3D.Tilt((float)(Math.PI / 2)));
            }
        }

        /// <summary>
        /// Updates the bottom wall.
        /// </summary>
        internal void UpdateBottomWall()
        {
            Rect bottomRect = new Rect(SeriesClipRect.Left, -(int)this.ActualDepth, SeriesClipRect.Width, (int)this.ActualDepth);
            var tlfVector = new Vector3D(bottomRect.Right, bottomRect.Top, this.WallSize + SeriesClipRect.Height);
            var brbVector = new Vector3D(bottomRect.Left, bottomRect.Bottom - 0.1, SeriesClipRect.Top + SeriesClipRect.Height + 1);
            this.bottomSideWallPlans = Polygon3D.CreateBox(brbVector, tlfVector, this, 0, this.Graphics3D, this.BottomWallBrush, this.BottomWallBrush, 0, false, "BottomWallBrush");

            foreach (Polygon3D plan in this.bottomSideWallPlans)
            {
                plan.Transform(Matrix3D.Tilt((float)(Math.PI / 2)));
            }
        }

        /// <summary>
        /// Updates the back wall.
        /// </summary>
        internal void UpdateBackWall()
        {
            var actualSeriesRect = SeriesClipRect;
            var tlfVector = new Vector3D(actualSeriesRect.Left, actualSeriesRect.Top, this.ActualDepth == 0d ? 1.5 : this.ActualDepth + this.WallSize);
            var brbVector = new Vector3D(actualSeriesRect.Right, actualSeriesRect.Bottom, this.ActualDepth == 0d ? 1.5 : this.ActualDepth);
            Polygon3D.CreateBox(tlfVector, brbVector, this, 0, this.Graphics3D, this.BackWallBrush, this.BackWallBrush, 0, false, "BackWallBrush");
        }

        /// <summary>
        /// Checks whether the chart is rotated.
        /// </summary>
        /// <returns>Indicates a <see cref="bool"/> value whether the chart is Rotated</returns>
        internal bool IsChartRotated()
        {
            var actualTiltView = Math.Abs(this.Tilt % 360);
            var actualRotateView = Math.Abs(this.Rotation % 360);

            if ((actualTiltView > 90 && actualTiltView < 270) ^ (actualRotateView > 90 && actualRotateView < 270))
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Updates the entire chart series and axis
        /// </summary>
        internal override void UpdateAxisLayoutPanels()
        {
            if (this.AreaType == ChartAreaType.CartesianAxes)
            {
                this.ChartAxisLayoutPanel = new ChartCartesianAxisLayoutPanel(this.controlsPresenter)
                {
                    Area = this
                };

                this.GridLinesLayout = new ChartCartesianGridLinesPanel(null)
                {
                    Area = this
                };
            }
            else
            {
                this.ChartAxisLayoutPanel = null;
                this.GridLinesLayout = null;
            }
        }
        
        /// <summary>
        /// Update the chart area
        /// </summary>
        /// <param name="forceUpdate">Triggers Force Update</param>
        internal override void UpdateArea(bool forceUpdate)
        {

            if (this.updateAreaAction != null || forceUpdate)
            {
                this.sumByIndex.Clear();
                this.Graphics3D.ClearVisual();
                if (this.AreaType == ChartAreaType.CartesianAxes)
                {
                    if (ColumnDefinitions.Count == 0)
                    {
                        ColumnDefinitions.Add(new ChartColumnDefinition());
                    }

                    if (RowDefinitions.Count == 0)
                    {
                        RowDefinitions.Add(new ChartRowDefinition());
                    }
                }

                if (this.VisibleSeries == null)
                {
                    return;
                }

                if ((this.UpdateAction & UpdateAction.Create) == UpdateAction.Create)
                {
                    foreach (var series in this.VisibleSeries)
                    {
                        if (!series.IsPointGenerated)
                        {
                            series.GeneratePoints();
                        }

                        if (series.ShowTooltip)
                        {
                            this.ShowTooltip = true;
                        }
                    }

                    // For stacked grouping scenario
                    if (this.AreaType == ChartAreaType.CartesianAxes)
                    {
                        foreach (ChartSeriesBase series in this.VisibleSeries)
                        {
                            ISupportAxes cartesianSeries = series as ISupportAxes;
                            if (this.InternalPrimaryAxis != null && !InternalPrimaryAxis.RegisteredSeries.Contains(cartesianSeries))
                            {
                                InternalPrimaryAxis.RegisteredSeries.Add(cartesianSeries);
                            }

                            if (this.InternalSecondaryAxis != null && !InternalSecondaryAxis.RegisteredSeries.Contains(cartesianSeries))
                            {
                                InternalSecondaryAxis.RegisteredSeries.Add(cartesianSeries);
                            }

                            if (this.InternalDepthAxis != null && !InternalDepthAxis.RegisteredSeries.Contains(cartesianSeries))
                            {
                                InternalDepthAxis.RegisteredSeries.Add(cartesianSeries);
                            }
                        }
                    }

                    // Initialize default axes for SfChart when PrimaryAxis or SecondayAxis is not set
                    this.InitializeDefaultAxes();

                    // Add selected index while loading 
                    if (!this.IsChartLoaded)
                    {
                        foreach (ChartSeries3D chartSeries3D in this.VisibleSeries)
                        {                           
                            if (chartSeries3D.SelectedIndex >= 0
                                && this.GetEnableSegmentSelection())
                            {
                                int index = chartSeries3D.SelectedIndex;
                                if (!chartSeries3D.SelectedSegmentsIndexes.Contains(index))
                                {
                                    chartSeries3D.SelectedSegmentsIndexes.Add(index);
                                }
                            }
                        }

                        if (this.GetEnableSeriesSelection() && this.SeriesSelectedIndex >= 0)
                        {
                            ChartSeriesBase series = VisibleSeries[SeriesSelectedIndex];

                            if (!SelectedSeriesCollection.Contains(series))
                            {
                                SelectedSeriesCollection.Add(series);
                            }
                        }
                    }

                    if ((InternalPrimaryAxis is CategoryAxis3D) && !(InternalPrimaryAxis as CategoryAxis3D).IsIndexed)
                    {
                        CategoryAxisHelper.GroupData(this.VisibleSeries);
                    }

                    foreach (var series in this.VisibleSeries)
                    {
                        series.Invalidate();
                    }

                    if (this.ShowTooltip && this.Tooltip == null)
                    {
                        this.Tooltip = new ChartTooltip();
                    }
                }

                if (this.IsUpdateLegend && (this.ChartDockPanel != null))
                {
                    this.UpdateLegend(this.Legend, false);
                    this.IsUpdateLegend = false;
                }

                if ((this.UpdateAction & UpdateAction.UpdateRange) == UpdateAction.UpdateRange)
                {
                    foreach (var series in this.VisibleSeries)
                    {
                        series.UpdateRange();
                    }
                }

                if (this.RootPanelDesiredSize != null)
                {
                    if ((this.UpdateAction & UpdateAction.Layout) == UpdateAction.Layout)
                    {
                        this.LayoutAxis(RootPanelDesiredSize.Value);
                    }

                    this.UpdateLegendArrangeRect();

                    if ((this.UpdateAction & UpdateAction.Render) == UpdateAction.Render)
                    {
                        if (!this.IsChartLoaded)
                        {
                            this.ScheduleRenderSeries();
                            this.IsChartLoaded = true;

                            // Raise the SelectionChanged event when SeriesSelectedIndex is set at chart load time.
                            if (this.SeriesSelectedIndex >= 0 && VisibleSeries.Count > 0 && this.GetEnableSeriesSelection())
                            {
                                this.RaiseSeriesSelectionChangedEvent();
                            }
                        }
                        else if (this.RenderSeriesAction == null)
                        {
                            this.RenderSeries();
                        }
                    }
                }

                this.UpdateAction = UpdateAction.Invalidate;
                this.updateAreaAction = null;
                this.IsRotationScheduleUpdate = false;
            }
        }

#endregion
        
#region Protected Methods

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The Available Size</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                this.SizeChanged -= this.OnSizeChanged;
                this.SizeChanged += this.OnSizeChanged;
                this.AvailableSize = new Size(ActualWidth == 0d ? 500d : ActualWidth, ActualHeight == 0d ? 500d : ActualHeight);
            }
            else
            {
                this.AvailableSize = availableSize;
            }

            return base.MeasureOverride(this.AvailableSize);
        }
        
        /// <summary>
        /// Updates the chart when tapped.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            var position = e.GetPosition(this.AdorningCanvas);
            this.ExplodeOnMouseClick(element, position);
            base.OnTapped(e);
        }

        /// <summary>
        /// Called before the PointerMoved event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            this.ChartMouseMove(e.OriginalSource, e.GetCurrentPoint(this.AdorningCanvas).Position);
            this.RotateChart(e.GetCurrentPoint(this).Position);
            base.OnPointerMoved(e);
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            this.ChartMouseUp(e.OriginalSource, e.GetCurrentPoint(this).Position, e.Pointer);
            this.ReleasePointerCapture(e.Pointer);
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            this.ChartMouseDown(e.OriginalSource, e.GetCurrentPoint(this).Position, e.Pointer);
            this.CapturePointer(e.Pointer);
        }

        /// <summary>
        /// Called when Pointer Leave in the Chart
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
        }


            /// <summary>
            /// Invoke to render <see cref="SfChart3D"/>
            /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.RootPanel = GetTemplateChild("PART_3DPanel") as Canvas;
            this.AdorningCanvas = GetTemplateChild("PART_adorningCanvas") as Canvas;
            this.ChartDockPanel = GetTemplateChild("Part_DockPanel") as ChartDockPanel;
            var layout = GetTemplateChild("Part_LayoutRoot") as ChartRootPanel;
            layout.Area = this;
            this.controlsPresenter = GetTemplateChild("Part_ControlsPanel") as Panel;
            this.UpdateAxisLayoutPanels();

            foreach (var visibleSeries in VisibleSeries.Where(visibleSeries => this.controlsPresenter != null))
            {
                this.controlsPresenter.Children.Add(visibleSeries);
            }

            var chartLegendCollection = Legend as ChartLegendCollection;
            if (chartLegendCollection != null)
            {
                this.LegendItems = new ObservableCollection<ObservableCollection<LegendItem>>();

                for (int i = 0; i < chartLegendCollection.Count; i++)
                {
                    var legendcollection = new ObservableCollection<LegendItem>();
                    LegendItems.Add(legendcollection);
                }
            }
            else
            {
                this.LegendItems = new ObservableCollection<ObservableCollection<LegendItem>>() { new ObservableCollection<LegendItem>() };
            }

            this.UpdateLegend(this.Legend, true);
            this.IsTemplateApplied = true;
        }

#endregion

#region Private Methods

#region Private Static Methods
        
        /// <summary>
        /// To prevent the call of update area each time the angle is changed.
        /// </summary>
        /// <param name="actualRotation">The Actual Rotation Angle</param>
        /// <param name="previousRotation">The Previous Rotation Angle</param>
        /// <returns>Is rotation angle at the same quadrant.</returns>
        private static bool CheckRotationAngleSameQuadrant(double actualRotation, double previousRotation)
        {
            return
                (actualRotation >= 0 && actualRotation < 45) && (previousRotation >= 0 && previousRotation < 45)
                || (actualRotation >= 45 && actualRotation < 90) && (previousRotation >= 45 && previousRotation < 90)
                || (actualRotation >= 90 && actualRotation < 135) && (previousRotation >= 90 && previousRotation < 135)
                || (actualRotation >= 135 && actualRotation < 180) && (previousRotation >= 135 && previousRotation < 180)
                || (actualRotation >= 180 && actualRotation < 225) && (previousRotation >= 180 && previousRotation < 225)
                || (actualRotation >= 225 && actualRotation < 270) && (previousRotation >= 225 && previousRotation < 270)
                || (actualRotation >= 270 && actualRotation < 315) && (previousRotation >= 270 && previousRotation < 315)
                || (actualRotation >= 315 && actualRotation < 360) && (previousRotation >= 315 && previousRotation < 360);
        }

        /// <summary>
        /// To prevent the call of update area each time the tilt is changed.
        /// </summary>
        /// <param name="actualtilt">The Actual Rotation Angle</param>
        /// <param name="previousTilt">The Previous Rotation Angle</param>
        /// <returns>Is tilt angle at the same quadrant.</returns>
        private static bool CheckTiltAngleSameQuadrant(double actualtilt, double previousTilt)
        {
            return
                (actualtilt >= 0 && actualtilt < 45) && (previousTilt >= 0 && previousTilt < 45)
                || (actualtilt >= 45 && actualtilt < 90) && (previousTilt >= 45 && previousTilt < 90)
                || (actualtilt >= 90 && actualtilt < 135) && (previousTilt >= 90 && previousTilt < 135)
                || (actualtilt >= 135 && actualtilt < 180) && (previousTilt >= 135 && previousTilt < 180)
                || (actualtilt >= 180 && actualtilt < 225) && (previousTilt >= 180 && previousTilt < 225)
                || (actualtilt >= 225 && actualtilt < 270) && (previousTilt >= 225 && previousTilt < 270)
                || (actualtilt >= 270 && actualtilt < 315) && (previousTilt >= 270 && previousTilt < 315)
                || (actualtilt >= 315 && actualtilt < 360) && (previousTilt >= 315 && previousTilt < 360);
        }

        /// <summary>
        /// Checks the series transposition.
        /// </summary>
        /// <param name="series">The Series</param>
        private static void CheckSeriesTransposition(ChartSeries3D series)
        {
            if (series.ActualXAxis == null || series.ActualYAxis == null)
            {
                return;
            }

            series.ActualXAxis.Orientation = series.IsActualTransposed ? Orientation.Vertical : Orientation.Horizontal;
            series.ActualYAxis.Orientation = series.IsActualTransposed ? Orientation.Horizontal : Orientation.Vertical;
        }

        /// <summary>
        /// This method is used to return the <see cref="bool"/> value when the AdornmentTemplate is selected.
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="series">The Series</param>
        /// <returns>Returns the <see cref="bool"/> value indicating whether the series event triggered.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Child Checking Logic")]
        private static bool IsSeriesEventTrigger(object source, ChartSeriesBase series)
        {
            var element = source as FrameworkElement;
            if (series != null && series.Adornments.Count > 0)
            {
                while (!(element is ChartAdornmentContainer) && element != null)
                {
                    element = VisualTreeHelper.GetParent(element) as FrameworkElement;
                    var contentControl = element as ContentControl;
                    if (contentControl != null && contentControl.Tag != null)
                    {
                        if (series.adornmentInfo.LabelPresenters.Count > 0 &&
                        series.adornmentInfo.LabelPresenters.Contains(element))
                        {
                            return true;
                        }
                    }
                    else if (element is ChartAdornmentContainer)
                    {
                        return series.adornmentInfo.adormentContainers.Count > 0 &&
                   series.adornmentInfo.adormentContainers.Contains(element);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the chart when depth axis is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnDepthAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = e.NewValue as ChartAxisBase3D;
            var chartArea = d as SfChart3D;

            if (chartAxis != null)
            {
                chartAxis.IsZAxis = true;
                chartAxis.Orientation = Orientation.Horizontal;
                chartArea.InternalDepthAxis = (ChartAxis)e.NewValue;
            }

            if (chartArea != null)
            {
                chartArea.OnAxisChanged(e);
            }
        }
        
        /// <summary>
        /// PropertyChangeCallback for EnableSeriesSelection property.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs</param>
        private static void OnEnableSeriesSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfChart3D chart = d as SfChart3D;

            if (chart != null && !(bool)e.NewValue)
            {
                foreach (ChartSeries3D series in chart.VisibleSeries)
                {
                    if (chart.SelectedSeriesCollection.Contains(series))
                    {
                        chart.SelectedSeriesCollection.Remove(series);
                        SfChart3D.OnResetSeries(series);
                    }
                }

                chart.SeriesSelectedIndex = -1;
                chart.SelectedSeriesCollection.Clear();
            }
            else if (chart != null && (bool)e.NewValue && chart.SeriesSelectedIndex != -1)
            {
                chart.SeriesSelectedIndexChanged(chart.SeriesSelectedIndex, -1);
            }
        }
        
        /// <summary>
        /// PropertyChangeCallback for EnableSegmentSelection.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs</param>
        private static void OnEnableSegmentSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = d as SfChart3D;
            if (chart == null)
            {
                return;
            }

            if (!(bool)e.NewValue)
            {
                foreach (var series in chart.VisibleSeries)
                {
                    for (int i = 0; i < series.ActualData.Count; i++)
                    {
                        if (series.SelectedSegmentsIndexes.Contains(i))
                        {
                            series.SelectedSegmentsIndexes.Remove(i);
                            series.OnResetSegment(i);
                        }
                    }

                    var chartSeries3D = series as ChartSeries3D;
                    if (chartSeries3D != null)
                    {
                        chartSeries3D.SelectedIndex = -1;
                    }

                    series.SelectedSegmentsIndexes.Clear();
                }
            }
            else
            {
                for (int index = 0; index < chart.VisibleSeries.Count; index++)
                {
                    ChartSeriesBase series = chart.VisibleSeries[index];

                    var chartSeries3D = series as ChartSeries3D;
                    if (chartSeries3D != null && chartSeries3D.SelectedIndex != -1)
                    {
                        series.SelectedIndexChanged(chartSeries3D.SelectedIndex, -1);
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates the chart style when style is set.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSelectionStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfChart3D chartBase = d as SfChart3D;

            if (chartBase == null || chartBase.Series == null || chartBase.Series.Count == 0)
            {
                return;
            }

            foreach (ChartSeries3D series in chartBase.Series)
            {
                if (chartBase.SelectedSeriesCollection.Contains(series))
                {
                    chartBase.SelectedSeriesCollection.Remove(series);
                    SfChart3D.OnResetSeries(series);
                }

                for (int i = 0; i < series.ActualData.Count; i++)
                {
                    if (series.SelectedSegmentsIndexes.Contains(i))
                    {
                        series.SelectedSegmentsIndexes.Remove(i);
                        series.OnResetSegment(i);
                    }
                }

                series.SelectedIndex = -1;
            }

            chartBase.SeriesSelectedIndex = -1;
        }
        
        /// <summary>
        /// Updates the top wall color when top wall brush changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnTopWallBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var chart = d as SfChart3D;
            if (chart != null && args.NewValue != args.OldValue)
            {
                chart.OnTopWallBrushChanged();
            }
        }
        
        /// <summary>
        /// Updates the bottom wall color when top wall brush changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnBottomWallBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var chart = d as SfChart3D;
            if (chart != null && args.NewValue != args.OldValue)
            {
                chart.OnBottomWallBrushChanged();
            }
        }
        
        /// <summary>
        /// Update the right wall color when top wall brush changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnRightWallBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var chart = d as SfChart3D;
            if (chart != null && args.NewValue != args.OldValue)
            {
                chart.OnRightWallBrushChanged();
            }
        }

        /// <summary>
        /// Updates the left wall color when left wall brush changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnLeftWallBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var chart = d as SfChart3D;
            if (chart != null && args.NewValue != args.OldValue)
            {
                chart.OnLeftWallBrushChanged();
            }
        }

        /// <summary>
        /// Updates the back wall color when back wall brush is applied.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnBackWallBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var chart = d as SfChart3D;
            if (chart != null && args.NewValue != args.OldValue)
            {
                chart.OnBackWallBrushChanged();
            }
        }

        /// <summary>
        /// Updates the chart when PerspectiveAngle is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnPerspectiveAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var chart = d as SfChart3D;
            if (chart != null)
            {
                chart.OnPerspectiveAngleChanged();
            }
        }

        /// <summary>
        /// Updates the chart when the primary axis is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnPrimaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = e.NewValue as ChartAxis;
            var chartArea = d as SfChart3D;

            if (chartAxis != null)
            {
                chartAxis.Orientation = Orientation.Horizontal;

                if (chartArea != null)
                {
                    chartArea.InternalPrimaryAxis = (ChartAxis)e.NewValue;
                }
            }

            if (chartArea != null)
            {
                chartArea.OnAxisChanged(e);
            }
        }

        /// <summary>
        /// Updates the chart when the secondary axis is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSecondaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as SfChart3D;

            if (chartArea == null)
            {
                return;
            }

            if (e.NewValue != null)
            {
                chartArea.InternalSecondaryAxis = (ChartAxis)e.NewValue;
                ((ChartAxis)e.NewValue).Orientation = Orientation.Vertical;
            }

            chartArea.OnAxisChanged(e);
        }

        /// <summary>
        /// Updates the chart when the series property collection changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSeriesPropertyCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SfChart3D)d).OnSeriesPropertyCollectionChanged(e);
        }

        /// <summary>
        /// Updates the chart when the depth property is changed.
        /// </summary>
        /// <param name="d">The Dependency Property</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnDepthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((SfChart3D)d).ScheduleUpdate();
        }

        /// <summary>
        /// Updates the chart when the rotation property changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnRotationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var area3D = (SfChart3D)d;
            area3D.IsRotationScheduleUpdate = true;
            var temprotationangle = (double)e.NewValue % 360;
            var actualRotationAngle = area3D.ActualRotationAngle = (temprotationangle < 0 ? temprotationangle + 360 : temprotationangle);

            temprotationangle = (double)e.OldValue % 360;
            var previousRotationAngle = (temprotationangle < 0 ? temprotationangle + 360 : temprotationangle);

            // This condition is to prevent the call of update area each time the angle is changed and also for the circular series.
            if (area3D.ChartAxisLayoutPanel == null || SfChart3D.CheckRotationAngleSameQuadrant(actualRotationAngle, previousRotationAngle))
            {
                area3D.Schedule3DUpdate();
            }
            else if (area3D != null)
            {
                area3D.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Updates the chart when tilt property changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnTiltPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var area3D = (SfChart3D)d;

            var tempTiltAngle = (double)e.NewValue % 360;
            var actualTiltAngle = area3D.ActualTiltAngle = (tempTiltAngle < 0 ? tempTiltAngle + 360 : tempTiltAngle);

            tempTiltAngle = (double)e.OldValue % 360;
            var previousRotationAngle = (tempTiltAngle < 0 ? tempTiltAngle + 360 : tempTiltAngle);

            // This condition is to prevent the call of update area each time the angle is changed and also for the circular series.
            if (area3D.ChartAxisLayoutPanel == null || SfChart3D.CheckTiltAngleSameQuadrant(actualTiltAngle, previousRotationAngle))
            {
                area3D.Schedule3DUpdate();
            }
            else if (area3D != null)
            {
                area3D.ScheduleUpdate();
            }
        }

#endregion

#region Private Methods

#region Update Wall Brush

        /// <summary>
        /// Update Back Wall Brush
        /// </summary>
        private void OnBackWallBrushChanged()
        {
            if (this.Graphics3D != null && this.Graphics3D.GetVisualCount() > 0)
            {
                var visualChild = this.Graphics3D.GetVisual();
                var getBackPolygons = visualChild.Where(item => (item.Name != null && item.Name.Contains("BackWallBrush")));
                foreach (Polygon3D item in getBackPolygons.ToList())
                {
                    visualChild.Remove(item);
                }

                this.UpdateBackWall();
                this.Graphics3D.PrepareView();
                this.Graphics3D.View(this.RootPanel);
            }
        }

        /// <summary>
        /// Update Left Wall Brush
        /// </summary>
        private void OnLeftWallBrushChanged()
        {
            if (this.leftSideWallPlans != null && this.Graphics3D != null && this.Graphics3D.GetVisualCount() > 0)
            {
                var visualChild = this.Graphics3D.GetVisual();
                var getBackPolygons = visualChild.Where(item => (item.Name != null && item.Name.Contains("LeftWallBrush")));
                foreach (Polygon3D item in getBackPolygons.ToList())
                {
                    visualChild.Remove(item);
                }

                this.UpdateLeftWall();
                this.Graphics3D.PrepareView();
                this.Graphics3D.View(this.RootPanel);
            }
        }

        /// <summary>
        /// Update Right Wall Brush
        /// </summary>
        private void OnRightWallBrushChanged()
        {
            if (this.rightSideWallPlans != null && this.Graphics3D != null && this.Graphics3D.GetVisualCount() > 0)
            {
                var visualChild = this.Graphics3D.GetVisual();
                var getBackPolygons = visualChild.Where(item => (item.Name != null && item.Name.Contains("RightWallBrush")));
                foreach (Polygon3D item in getBackPolygons.ToList())
                {
                    visualChild.Remove(item);
                }

                this.UpdateRightWall();
                this.Graphics3D.PrepareView();
                this.Graphics3D.View(this.RootPanel);
            }
        }

        /// <summary>
        /// Update Bottom Wall Brush
        /// </summary>
        private void OnBottomWallBrushChanged()
        {
            if (this.bottomSideWallPlans != null && this.Graphics3D != null && this.Graphics3D.GetVisualCount() > 0)
            {
                var visualChild = this.Graphics3D.GetVisual();
                var getBackPolygons = visualChild.Where(item => (item.Name != null && item.Name.Contains("BottomWallBrush")));
                foreach (Polygon3D item in getBackPolygons.ToList())
                {
                    visualChild.Remove(item);
                }

                this.UpdateBottomWall();
                this.Graphics3D.PrepareView();
                this.Graphics3D.View(this.RootPanel);
            }
        }

        /// <summary>
        /// Update Top Wall Brush
        /// </summary>
        private void OnTopWallBrushChanged()
        {
            if (this.topSideWallPlans != null && this.Graphics3D != null && this.Graphics3D.GetVisualCount() > 0)
            {
                var visualChild = this.Graphics3D.GetVisual();
                var getBackPolygons = visualChild.Where(item => (item.Name != null && item.Name.Contains("TopWallBrush")));
                foreach (Polygon3D item in getBackPolygons.ToList())
                {
                    visualChild.Remove(item);
                }

                this.UpdateTopWall();
                this.Graphics3D.PrepareView();
                this.Graphics3D.View(this.RootPanel);
            }
        }

#endregion
        
        /// <summary>
        /// Unregisters the series.
        /// </summary>
        /// <param name="series">The Series</param>
        private void UnRegisterSeries(ISupportAxes series)
        {
            if (this.InternalPrimaryAxis == null || this.InternalSecondaryAxis == null)
            {
                return;
            }

            if (InternalPrimaryAxis.RegisteredSeries.Contains(series))
            {
                InternalPrimaryAxis.RegisteredSeries.Remove(series);
            }

            if (InternalSecondaryAxis.RegisteredSeries.Contains(series))
            {
                InternalSecondaryAxis.RegisteredSeries.Remove(series);
            }

            // To reset the axis orientation when the transposed series is cleared.
            if (InternalSecondaryAxis.RegisteredSeries.Count == 0
                                && InternalPrimaryAxis.RegisteredSeries.Count == 0)
            {
                InternalPrimaryAxis.Orientation = Orientation.Horizontal;
                InternalSecondaryAxis.Orientation = Orientation.Vertical;
            }

            if (this.InternalDepthAxis == null)
            {
                return;
            }

            if (InternalDepthAxis.RegisteredSeries.Contains(series))
            {
                InternalDepthAxis.RegisteredSeries.Remove(series);

                // Dynamic axis series clearing case. The z axis has to be removed.
                if (InternalDepthAxis.IsDefault)
                {
                    this.InternalDepthAxis = null;
                    this.DepthAxis = null;
                }
            }
        }

        /// <summary>
        /// Updates the visible series.
        /// </summary>
        /// <param name="seriesColl">The Series Collection</param>
        private void UpdateVisibleSeries(IList seriesColl)
        {
            foreach (ChartSeries3D series in seriesColl)
            {
                series.UpdateLegendIconTemplate(false);
                series.Area = this;
                SfChart3D.CheckSeriesTransposition(series);

                if (series.ActualXAxis != null && !this.Axes.Contains(series.ActualXAxis))
                {
                    series.ActualXAxis.Area = this;
                    Axes.Add(series.ActualXAxis);
                }

                if (series.ActualYAxis != null && !this.Axes.Contains(series.ActualYAxis))
                {
                    series.ActualYAxis.Area = this;
                    Axes.Add(series.ActualYAxis);
                }

                var zseries = series as XyzDataSeries3D;

                // Fourth condition is to prevent the unwanted addition of the z axis.
                if (zseries != null && zseries.ActualZAxis != null && !this.Axes.Contains(zseries.ActualZAxis) && (zseries != null && !string.IsNullOrEmpty(zseries.ZBindingPath)))
                {
                    zseries.ActualZAxis.Area = this;
                    Axes.Add(zseries.ActualZAxis);
                }

                if (this.controlsPresenter != null && !this.controlsPresenter.Children.Contains(series))
                {
                    this.controlsPresenter.Children.Add(series);
                }

                if (series.IsSeriesVisible)
                {
                    if (this.AreaType == ChartAreaType.None && series is CircularSeriesBase3D)
                    {
                        VisibleSeries.Add(series);
                    }
                    else if (this.AreaType == ChartAreaType.CartesianAxes && series is CartesianSeries3D)
                    {
                        VisibleSeries.Add(series);
                    }
                }

                ActualSeries.Add(series);
            }
        }

        /// <summary>
        /// Updates the chart when the series collection changed.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        private void OnSeriesPropertyCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((ChartSeries3DCollection)e.OldValue).Clear();
                ((ChartSeries3DCollection)e.OldValue).CollectionChanged -= this.OnSeriesCollectionChanged;
                InternalPrimaryAxis.RegisteredSeries.Clear();
                InternalSecondaryAxis.RegisteredSeries.Clear();
                VisibleSeries.Clear();
                ActualSeries.Clear();
            }

            if (this.Series == null)
            {
                return;
            }

            this.Series.CollectionChanged += this.OnSeriesCollectionChanged;
            if (this.Series.Count <= 0)
            {
                return;
            }

            if (this.Series[0] is CircularSeriesBase3D)
            {
                this.AreaType = ChartAreaType.None;
            }
            else
            {
                this.AreaType = ChartAreaType.CartesianAxes;
            }

            this.UpdateVisibleSeries(this.Series);
            this.UpdateLegend(this.Legend, false);
            this.ScheduleUpdate();
        }

        /// <summary>
        /// Updates the chart when the perspective angle is changed.
        /// </summary>
        private void OnPerspectiveAngleChanged()
        {
            if (this.RootPanel == null || this.RootPanelDesiredSize == null)
            {
                return;
            }

            this.Graphics3D.View(this.RootPanel, this.ActualRotationAngle, this.ActualTiltAngle, RootPanelDesiredSize.Value, this.PerspectiveAngle, this.ActualDepth);
        }

        /// <summary>
        /// Updates the chart when the series collection is changed.
        /// </summary>
        /// <param name="sender">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    {
                        if (this.controlsPresenter != null)
                        {
                            for (var i = this.controlsPresenter.Children.Count - 1; i >= 0; i--)
                            {
                                if (this.controlsPresenter.Children[i] is ChartSeries3D)
                                {
                                    var series = this.controlsPresenter.Children[i] as ChartSeries3D;
                                    this.controlsPresenter.Children.RemoveAt(i);

                                    if (series.GetAnimationIsActive())
                                    {
                                        series.AnimationStoryboard.Stop();
                                        series.AnimationStoryboard = null;
                                    }

                                    var isupportAxes = series as ISupportAxes;
                                    if (isupportAxes != null)
                                    {
                                        this.UnRegisterSeries(isupportAxes);
                                    }
                                }
                            }
                        }

                        if (this.Legend != null && this.LegendItems != null)
                        {
                            if (Legend is ChartLegendCollection)
                            {
                                foreach (var item in this.LegendItems)
                                {
                                    item.Clear();
                                }
                            }
                            else if (LegendItems.Count > 0)
                            {
                                LegendItems[0].Clear();
                            }
                        }

                        ActualSeries.Clear();
                        VisibleSeries.Clear();
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        var series = e.OldItems[0] as ChartSeriesBase;

                        var isupportAxex = series as ISupportAxes;
                        if (isupportAxex != null)
                        {
                            this.UnRegisterSeries(isupportAxex);
                        }

                        var chartSeries3D = series as ChartSeries3D;
                        if (chartSeries3D != null && series.GetAnimationIsActive())
                        {
                            chartSeries3D.AnimationStoryboard.Stop();
                            chartSeries3D.AnimationStoryboard = null;
                        }

                        if (VisibleSeries.Contains(series))
                        {
                            VisibleSeries.Remove(series);
                        }

                        if (ActualSeries.Contains(series))
                        {
                            ActualSeries.Remove(series);
                        }

                        this.controlsPresenter.Children.Remove(series);
                        series.RemoveTooltip();

                        if (VisibleSeries.Count == 0 && this.Series.Count > 0)
                        {
                            if (this.Series[0] is CircularSeriesBase3D)
                            {
                                this.AreaType = ChartAreaType.None;
                            }
                            else
                            {
                                this.AreaType = ChartAreaType.CartesianAxes;
                            }

                            this.UpdateVisibleSeries(this.Series);
                        }

                        if (this.Legend != null && this.LegendItems != null)
                        {
                            if (Legend is ChartLegendCollection)
                            {
                                foreach (var item in this.LegendItems)
                                {
                                    item.Clear();
                                }
                            }
                            else if (LegendItems.Count > 0)
                            {
                                LegendItems[0].Clear();
                            }
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewStartingIndex == 0)
                        {
                            if (this.Series[0] is CircularSeriesBase3D)
                            {
                                this.AreaType = ChartAreaType.None;
                            }
                            else
                            {
                                this.AreaType = ChartAreaType.CartesianAxes;
                            }
                        }

                        this.UpdateVisibleSeries(e.NewItems);
                        break;
                    }
            }

            var canvas = GetAdorningCanvas();
            if (canvas != null)
            {
                var children = canvas.Children.OfType<ChartTooltip>().ToList();
                foreach (var item in children)
                {
                    canvas.Children.Remove(item);
                }
            }

            this.IsUpdateLegend = true;
            this.ScheduleUpdate();
            this.SBSInfoCalculated = false;
        }

        /// <summary>
        /// Updates the chart on chart size changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != this.AvailableSize)
            {
                this.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Rotates the chart.
        /// </summary>
        /// <param name="updatedPosition">The Position</param>
        private void RotateChart(Point updatedPosition)
        {
            if (this.rotationActivated)
            {
                foreach (var series in this.VisibleSeries)
                {
                    series.RemoveTooltip();
                }

                var difY = this.previousChartPosition.Y - updatedPosition.Y;
                var difX = this.previousChartPosition.X - updatedPosition.X;
                this.Tilt -= difY;
                this.Rotation += difX;
                this.previousChartPosition = updatedPosition;
            }
        }

        /// <summary>
        /// Updates the front wall.
        /// </summary>
        private void UpdateFrontWall()
        {
            var actualSeriesRect = SeriesClipRect;
            var tlfVector = new Vector3D(actualSeriesRect.Left, actualSeriesRect.Top, this.ActualDepth == 0d ? -1.5 : -this.WallSize - 1);
            var brbVector = new Vector3D(actualSeriesRect.Right, actualSeriesRect.Bottom, this.ActualDepth == 0d ? -1.5 : 0 - 1);
            Polygon3D.CreateBox(tlfVector, brbVector, this, 0, this.Graphics3D, this.BackWallBrush, this.BackWallBrush, 0, false, "BackWallBrush");
        }

        /// <summary>
        /// Update3D the wall.
        /// </summary>
        private void Update3DWall()
        {
            if (this.AreaType != ChartAreaType.CartesianAxes)
            {
                return;
            }

            var verticalAxis = InternalSecondaryAxis.Orientation == Orientation.Vertical ? InternalSecondaryAxis : InternalPrimaryAxis;

            if (!verticalAxis.OpposedPosition && this.ActualRotationAngle >= 90 && this.ActualRotationAngle < 270)
            {
                this.UpdateFrontWall();
            }
            else
            {
                this.UpdateBackWall();
            }

            this.leftSideWallPlans = null;
            this.bottomSideWallPlans = null;
            this.topSideWallPlans = null;
            this.rightSideWallPlans = null;
            bool isHorizWallUpdated = false;

            foreach (var item in this.Axes)
            {
                if (item.Orientation == Orientation.Vertical)
                {
                    if (item.OpposedPosition || (!item.OpposedPosition && this.ActualRotationAngle >= 180 && this.ActualRotationAngle < 360) && this.rightSideWallPlans == null)
                    {
                        this.UpdateRightWall();
                    }
                    else if (this.leftSideWallPlans == null)
                    {
                        this.UpdateLeftWall();
                    }
                }
                else
                {
                    if (!isHorizWallUpdated)
                    {
                        if ((item.OpposedPosition || item.Area.Axes.Where(x => (x is ChartAxisBase3D) && x.Orientation == Orientation.Horizontal && x.OpposedPosition).Any()) && this.topSideWallPlans == null)
                        {
                            this.UpdateTopWall();
                        }
                        else if (this.bottomSideWallPlans == null)
                        {
                            this.UpdateBottomWall();
                        }
                    }

                    isHorizWallUpdated = true;
                }
            }
        }

        /// <summary>
        /// Update the 3D view.
        /// </summary>
        private void Update3DView()
        {
            this.is3DUpdateScheduled = false;

            if (this.RootPanelDesiredSize != null)
            {
                foreach (var segment in this.Series.SelectMany(series => series.Segments.OfType<ChartSegment3D>()))
                {
                    ((ChartSegment3D)segment).Polygons.Clear();
                }

                if (this.RootPanel == null)
                {
                    return;
                }

                if (this.IsAutoDepth)
                {
                    if (this.AutoDepthAdjust())
                    {
                        this.Graphics3D.PrepareView(this.PerspectiveAngle, this.Depth, this.Rotation, this.Tilt, RootPanelDesiredSize.Value);
                    }
                }

                this.Graphics3D.View(this.RootPanel, this.Rotation, this.Tilt, RootPanelDesiredSize.Value, this.PerspectiveAngle, this.ActualDepth);
            }
        }

        /// <summary>
        /// Automatics the depth adjust.
        /// </summary>
        /// <returns>Need auto depth adjust</returns>
        private bool AutoDepthAdjust()
        {
            const double DepthSpacing = 5d;
            var depth = 0d;

            if (this.IsChartRotated())
            {
                depth = this.ActualDepth + DepthSpacing;
            }

            if (this.previousAutoDepth == depth)
            {
                return false;
            }

            foreach (var item in this.Graphics3D.GetVisual())
            {
                if (!(item is UIElement3D) && !(item is PolyLine3D))
                {
                    continue;
                }

                var count = 0;
                var updatedVector = new Vector3D[item.VectorPoints.Length];
                foreach (var vectorPoint in item.VectorPoints)
                {
                    updatedVector[count] = new Vector3D(vectorPoint.X, vectorPoint.Y, depth);
                    count++;
                }

                item.VectorPoints = updatedVector;
            }

            this.previousAutoDepth = depth;
            return true;
        }

        /// <summary>
        /// Updates the interactions when chart is moved.
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        private void ChartMouseMove(object source, Point position)
        {
            var element = source as FrameworkElement;
            var segment = element != null ? element.Tag as ChartSegment : null;
            if (segment != null)
            {
                if ((this.EnableSegmentSelection && !segment.Series.IsLinear) || this.EnableSeriesSelection)
                {
#if WINDOWS_UAP && CHECKLATER
                    if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                    {
                        if (Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor.Type == CoreCursorType.Arrow)
                        {
                            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(this.SelectionCursor, 1);
                        }
                    }
#endif
                }

                if (this.mouseMoveSegment != null && this.mouseMoveSegment != segment
                    && ((ChartSeries3D)this.mouseMoveSegment.Series).SelectionMode == SelectionMode.MouseMove)
                {
                    ((ChartSeries3D)this.mouseMoveSegment.Series).SelectedIndex = -1;
                }

                this.currentSeries = (ChartSeries3D)segment.Series;
                ((ChartSeries3D)segment.Series).OnSeriesMouseMove(source, position);
                this.mouseMoveSegment = segment;
            }
            else if (this.mouseMoveSegment != null && ((ChartSeries3D)this.mouseMoveSegment.Series).SelectionMode == SelectionMode.MouseMove)
            {
                bool isCancel;

                if (this.EnableSeriesSelection)
                {
                    isCancel = ((ChartSeries3D)this.mouseMoveSegment.Series).RaiseSelectionChanging(-1, this.SeriesSelectedIndex);
                }
                else
                {
                    isCancel = ((ChartSeries3D)this.mouseMoveSegment.Series).RaiseSelectionChanging(-1, ((ChartSeries3D)this.mouseMoveSegment.Series).SelectedIndex);
                }

                if (!isCancel)
                {
                    ((ChartSeries3D)this.mouseMoveSegment.Series).SelectedIndex = -1;
                    this.SeriesSelectedIndex = -1;
                    this.mouseMoveSegment = null;
                }
            }
            else if (segment == null && this.currentSeries != null && !(element.DataContext is LegendItem))
            {
#if WINDOWS_UAP && CHECKLATER
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                {
                    if (Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow)
                    {
                        Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 2);
                    }
                }
#endif
                this.currentSeries.OnSeriesMouseLeave(source, position);
            }

            if (element.DataContext is ChartAdornmentContainer)
            {
                ChartAdornmentContainer container = element.DataContext as ChartAdornmentContainer;
                (container.Adornment.Series as ChartSeries3D).OnSeriesMouseMove(element, position);
                this.series = container.Adornment.Series;
            }
            else if (VisualTreeHelper.GetParent(element) is ContentPresenter)
            {
                ContentPresenter contentpresenter = VisualTreeHelper.GetParent(element) as ContentPresenter;
                if (contentpresenter.Content is ChartAdornment3D)
                {
                    ((contentpresenter.Content as ChartAdornment3D).Series as ChartSeries3D).OnSeriesMouseMove(element, position);
                    this.series = (contentpresenter.Content as ChartAdornment3D).Series;
                }
            }
            else if (this.series is ChartSeries3D)
            {
                (this.series as ChartSeries3D).OnSeriesMouseLeave(element, position);
                this.series = null;
            }
        }

        /// <summary>
        /// Updates the interactions when the chart mouse button is down.
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <param name="pointer">The pointer</param>
        private void ChartMouseDown(object source, Point position, object pointer)
        {
            this.previousChartPosition = position;
            this.rotationActivated = this.EnableRotation;
            var element = source as FrameworkElement;
            this.CapturePointer(pointer as Pointer);
            var segment = element != null ? element.Tag as ChartSegment : null;

            if (segment != null)
            {
                ((ChartSeries3D)segment.Series).OnSeriesMouseDown(source, position);
            }
            else if (this.Series != null)
            {
                foreach (var series in this.Series)
                {
                    if (SfChart3D.IsSeriesEventTrigger(source, series))
                    {
                        series.OnSeriesMouseDown(source, position);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the interactions when the chart mouse button is up.
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <param name="pointer">The Pointer</param>
        private void ChartMouseUp(object source, Point position, object pointer)
        {
            this.rotationActivated = false;
            this.ReleasePointerCapture(pointer as Pointer);
        }

        /// <summary>
        /// Explodes the chart on mouse click.
        /// </summary>
        /// <param name="element">The Element</param>
        /// <param name="position">The Position</param>
        private void ExplodeOnMouseClick(FrameworkElement element, Point position)
        {
            var segment = element != null ? element.Tag as ChartSegment : null;
            if (segment != null)
            {
                ((ChartSeries3D)segment.Series).OnSeriesMouseUp(element, position);
            }
            else if (this.Series != null)
            {
                foreach (var series in this.Series)
                {
                    if (SfChart3D.IsSeriesEventTrigger(element, series))
                    {
                        series.OnSeriesMouseUp(element, position);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Schedule the 3d update.
        /// </summary>
        private void Schedule3DUpdate()
        {
            if (this.is3DUpdateScheduled)
            {
                return;
            }
#if NETFX_CORE
            if (DesignMode.DesignModeEnabled)
            {
                this.Update3DView();
            }
            else
            {
               //This code is to call Update3DView() method when rotating 3D chart.
               var update3DViewAction = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Update3DView);
            }
#endif
            this.is3DUpdateScheduled = true;
        }

        /// <summary>
        /// Raises the <see>
        /// <cref>E:AxisChanged</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" />Instance containing the event data.</param>
        private void OnAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = e.NewValue as ChartAxis;

            var oldAxis = e.OldValue as ChartAxis;

            if (oldAxis != null && Axes.Contains(oldAxis))
            {
                Axes.Remove(oldAxis);
                oldAxis.RegisteredSeries.Clear();
            }

            if (this.Axes != null && chartAxis != null && !Axes.Contains(chartAxis))
            {
                chartAxis.Area = this;
                Axes.Insert(0, chartAxis);
            }

            if (this.Series != null && chartAxis != null)
            {
                foreach (var series in this.Series)
                {
                    if (series is CartesianSeries3D)
                    {
                        SfChart3D.CheckSeriesTransposition(series); // For adding the bar series with the default axis.
                        chartAxis.RegisteredSeries.Add((ISupportAxes)series);
                    }
                }
            }

            this.ScheduleUpdate();
        }

        /// <summary>
        /// Layouts the axis.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        private void LayoutAxis(Size availableSize)
        {
            if (this.ChartAxisLayoutPanel != null)
            {
                ChartAxisLayoutPanel.UpdateElements();
                ChartAxisLayoutPanel.Measure(availableSize);
                ChartAxisLayoutPanel.Arrange(availableSize);
            }

            if (this.GridLinesLayout == null)
            {
                return;
            }

            GridLinesLayout.UpdateElements();
            GridLinesLayout.Measure(availableSize);
            ((ChartCartesianGridLinesPanel)GridLinesLayout).Arrange3D(availableSize);
        }

        /// <summary>
        /// Renders the segment at the given schedules.
        /// </summary>
        private void ScheduleRenderSeries()
        {
            if (DesignMode.DesignModeEnabled)
            {
                this.RenderSeries();
            }
            else if (this.RenderSeriesAction == null)
            {
                this.RenderSeriesAction = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, this.RenderSeries);
            }
        }

#endregion

#endregion
                      
#endregion
    }
}
