// <copyright file="XyDataSeries3D.cs" company="Syncfusion. Inc">
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
    using System.Linq;
    using System.Reflection;
    using Windows.ApplicationModel;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Class implementation for <see cref="XyDataSeries3D"/> 
    /// </summary>
    public abstract partial class XyDataSeries3D : CartesianSeries3D
    {
        #region Dependency Property Registration

        /// <summary>
        ///  The DependencyProperty for <see cref="YBindingPath"/> property.
        /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register(
                "YBindingPath",
                typeof(string),
                typeof(XyDataSeries3D),
                new PropertyMetadata(null, OnYBindingPathChanged));

        #endregion
        
        #region Constructor.

        /// <summary>
        /// Initializes a new instance of the <see cref="XyDataSeries3D"/> class.
        /// </summary>
        protected XyDataSeries3D()
        {
            this.YValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the binding path for y axis.
        /// </summary>
        public string YBindingPath
        {
            get { return (string)GetValue(YBindingPathProperty); }
            set { this.SetValue(YBindingPathProperty, value); }
        }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets YValues
        /// </summary>
        protected internal IList<double> YValues
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Method used to get the chart data at an given point
        /// </summary>
        /// <param name="axis">The Axis</param>
        /// <param name="point">The Point</param>
        /// <returns>The point to value.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is a public method")]       
        public double GetPointToValue(ChartAxis axis, Point point)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return axis.CoefficientToValueCalc((point.X - (axis.RenderedRect.Left - axis.Area.SeriesClipRect.Left)) / axis.RenderedRect.Width);
                }

                return axis.CoefficientToValueCalc(1d - ((point.Y - (axis.RenderedRect.Top - axis.Area.SeriesClipRect.Top)) / axis.RenderedRect.Height));
            }

            return double.NaN;
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Updates the series on label property changed.
        /// </summary>
        internal void OnLabelPropertyChanged()
        {
            var area3D = ActualArea;
            if (area3D != null && area3D.InternalDepthAxis != null && (area3D.InternalDepthAxis as ChartAxisBase3D).IsManhattanAxis)
            {
                area3D.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Calculates the segment depth.
        /// </summary>
        /// <param name="depth">The Depth</param>
        /// <returns>Returns the calculated segment depth.</returns>
        internal DoubleRange GetSegmentDepth(double depth)
        {
            var actualDepth = depth;
            double start, end;

            if (Area.SideBySideSeriesPlacement && this.IsSideBySide)
            {
                var space = actualDepth / 4;
                start = space;
                end = space * 3;
            }
            else
            {
                var index = Area.VisibleSeries.IndexOf(this);
                var count = Area.VisibleSeries.Count;
                var space = actualDepth / ((count * 2) + count + 1);
                start = space + (space * index * 3);
                end = start + space * 2;
            }

            return new DoubleRange(start, end);
        }
        
        /// <summary>
        /// Validate the data points for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var yValue in this.YValues)
            {
                if (double.IsNaN(yValue) && this.ShowEmptyPoints)
                {
                    this.ValidateDataPoints(this.YValues);
                    break;
                }
            }
        }
        
        /// <summary>
        /// Re-validate the data points for segment implementation.
        /// </summary>
        /// <param name="emptyPointIndex">Empty Point Index</param>
        internal override void ReValidateYValues(List<int>[] emptyPointIndex)
        {
            foreach (var item in emptyPointIndex)
            {
                foreach (var index in item)
                {
                    this.YValues[index] = double.NaN;
                }
            }
        }

        #endregion

        #region Protected Internal Methods
        
        /// <summary>
        /// Method for Generate Points for XYDataSeries.
        /// </summary>
        protected internal override void GeneratePoints()
        {
            this.GeneratePoints(new[] { this.YBindingPath }, this.YValues);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue">The Old Value</param>
        /// <param name="newValue">The New Value</param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            this.YValues.Clear();
            this.GeneratePoints(new[] { this.YBindingPath }, this.YValues);
            this.UpdateArea();
        }

        /// <summary>
        /// Raises the <see>
        /// <cref>E:BindingPathChanged</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="args">The Event Arguments.</param>
        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            this.YValues.Clear();
            base.OnBindingPathChanged(args);
        }

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var xyDataSeries = obj as XyDataSeries;
            if (xyDataSeries != null)
            {
                xyDataSeries.YBindingPath = this.YBindingPath;
            }

            return base.CloneSeries(obj);
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Updates the series on y binding path changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnYBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var xyDataSeries3D = d as XyDataSeries3D;

            if (xyDataSeries3D != null)
            {
                xyDataSeries3D.OnBindingPathChanged(e);
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for <see cref="XyzDataSeries3D"/> 
    /// </summary>
    public abstract partial class XyzDataSeries3D : XyDataSeries3D
    {
        #region Dependency Propety Registration

        // Using a DependencyProperty as the backing store for ZBindingPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZBindingPathProperty =
            DependencyProperty.Register(
                "ZBindingPath",
                typeof(string),
                typeof(ChartSeries3D),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnBindingPathZChanged)));

        #endregion

        #region Fields

        private ChartValueType zValueType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="XyzDataSeries3D"/> class.
        /// </summary>
        public XyzDataSeries3D() : base()
        {
            this.ZValues = this.ActualZValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the z-axis range. 
        /// </summary>
        public DoubleRange ZRange { get; internal set; }
        
        /// <summary>
        /// Gets or sets the binding path for z axis.
        /// </summary>
        public string ZBindingPath
        {
            get { return (string)GetValue(ZBindingPathProperty); }
            set { this.SetValue(ZBindingPathProperty, value); }
        }

        #endregion

        #region Internal Properties
        
        /// <summary>
        /// Gets or sets the z complex paths.
        /// </summary>
        internal string[] ZComplexPaths { get; set; }
        
        /// <summary>
        /// Gets or sets the z axis side by side information.
        /// </summary>
        internal DoubleRange ZSideBySideInfoRangePad { get; set; }

        /// <summary>
        /// Gets or sets the z axis value type.
        /// </summary>
        internal ChartValueType ZAxisValueType
        {
            get
            {
                return this.zValueType;
            }

            set
            {
                this.zValueType = value;
            }
        }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets a value indicating whether to treat z values as categories. 
        /// </summary>
        protected internal bool IsIndexedZAxis
        {
            get { return this.ActualZAxis is CategoryAxis3D || this.ActualZAxis is DateTimeCategoryAxis; }
        }

        /// <summary>
        /// Gets the actual z axis.
        /// </summary>
        protected internal ChartAxis ActualZAxis
        {
            get
            {
                return (this.ActualArea != null && this is ISupportAxes) ? ActualArea.InternalDepthAxis : null;
            }
        }

        /// <summary>
        /// Gets or sets the sorted values, if the IsSortData is true.
        /// </summary>
        protected internal IEnumerable ActualZValues
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the x values in an unsorted order or in the order the data has been added to series.
        /// </summary>
        protected internal IEnumerable ZValues
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Generates the property points
        /// </summary>
        /// <param name="yPaths">The Y Paths</param>
        /// <param name="yLists">The Y Lists</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Needed For Point Generation Logic")]
        internal override void GeneratePropertyPoints(string[] yPaths, IList<double>[] yLists)
        {
            var xyzSeries = this as XyzDataSeries3D;
            if (xyzSeries == null || (xyzSeries != null && (xyzSeries.ZBindingPath == null || xyzSeries.ZBindingPath.Length == 0)))
            {
                base.GeneratePropertyPoints(yPaths, yLists);
            }
            else
            {
                IEnumerator enumerator = ItemsSource.GetEnumerator();
                PropertyInfo xPropertyInfo, yPropertyInfo, zPropertyInfo;

                if (enumerator.MoveNext())
                {
                    {
                        for (int i = 0; i < this.UpdateStartedIndex; i++)
                        {
                            enumerator.MoveNext();
                        }

                        xPropertyInfo = ChartDataUtils.GetPropertyInfo(enumerator.Current, this.XBindingPath);

                        zPropertyInfo = xyzSeries.ZBindingPath != null ? ChartDataUtils.GetPropertyInfo(enumerator.Current, xyzSeries.ZBindingPath) : null;
                        IPropertyAccessor xPropertyAccessor = null;
                        IPropertyAccessor zPropertyAccessor = null;
                        if (xPropertyInfo != null)
                        {
                            xPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                        }

                        if (zPropertyInfo != null)
                        {
                            zPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(zPropertyInfo);
                        }

                        if (xPropertyAccessor == null || zPropertyAccessor == null)
                        {
                            return;
                        }

                        Func<object, object> xGetMethod = xPropertyAccessor.GetMethod;
                        Func<object, object> zGetMethod = zPropertyAccessor.GetMethod;

                        if (xGetMethod(enumerator.Current) != null && xGetMethod(enumerator.Current).GetType().IsArray)
                        {
                            return;
                        }

                        this.XAxisValueType = GetDataType(xPropertyAccessor, ItemsSource as IEnumerable);
                        if (zPropertyAccessor != null)
                        {
                            this.ZAxisValueType = GetDataType(zPropertyAccessor, ItemsSource as IEnumerable);
                        }

                        if (this.XAxisValueType == ChartValueType.DateTime || this.XAxisValueType == ChartValueType.Double ||
                            this.XAxisValueType == ChartValueType.Logarithmic || this.XAxisValueType == ChartValueType.TimeSpan)
                        {
                            if (!(ActualXValues is List<double>))
                            {
                                this.ActualXValues = this.XValues = new List<double>();
                            }
                        }
                        else
                        {
                            if (!(ActualXValues is List<string>))
                            {
                                this.ActualXValues = this.XValues = new List<string>();
                            }
                        }

                        if (this.ZAxisValueType == ChartValueType.DateTime || this.ZAxisValueType == ChartValueType.Double ||
                                    this.ZAxisValueType == ChartValueType.Logarithmic || this.ZAxisValueType == ChartValueType.TimeSpan)
                        {
                            if (!(this.ActualZValues is List<double>))
                            {
                                this.ActualZValues = this.ZValues = new List<double>();
                            }
                        }
                        else
                        {
                            if (!(this.ActualZValues is List<string>))
                            {
                                this.ActualZValues = this.ZValues = new List<string>();
                            }
                        }

                        if (string.IsNullOrEmpty(yPaths[0]))
                        {
                            return;
                        }

                        yPropertyInfo = ChartDataUtils.GetPropertyInfo(enumerator.Current, yPaths[0]);
                        IPropertyAccessor yPropertyAccessor = null;
                        if (yPropertyInfo != null)
                        {
                            yPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(yPropertyInfo);
                        }

                        if (yPropertyAccessor == null)
                        {
                            return;
                        }

                        IList<double> yValue = yLists[0];
                        if (yPropertyAccessor == null)
                        {
                            return;
                        }

                        Func<object, object> yGetMethod = yPropertyAccessor.GetMethod;
                        if (yGetMethod(enumerator.Current) != null && yGetMethod(enumerator.Current).GetType().IsArray)
                        {
                            return;
                        }

                        var zDoubleValues = this.ZValues as List<double>;
                        if (this.XAxisValueType == ChartValueType.String)
                        {
                            IList<string> xValue = this.XValues as List<string>;

                            switch (this.ZAxisValueType)
                            {
                                case ChartValueType.DateTime:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);
                                        xValue.Add(xVal != null ? (string)xVal : string.Empty);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(((DateTime)zVal).ToOADate());
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                case ChartValueType.TimeSpan:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);
                                        xValue.Add(xVal != null ? (string)xVal : string.Empty);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(((TimeSpan)zVal).TotalMilliseconds);
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                case ChartValueType.Double:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);
                                        xValue.Add(xVal != null ? (string)xVal : string.Empty);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(Convert.ToDouble(zVal));
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                default:
                                    object zValue = this.ZValues;

                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);
                                        xValue.Add(xVal != null ? (string)xVal : string.Empty);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        (zValue as List<string>).Add((string)zVal);
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;
                            }
                        }
                        else if (this.XAxisValueType == ChartValueType.Double ||
                                 this.XAxisValueType == ChartValueType.Logarithmic)
                        {
                            IList<double> xValue = this.XValues as List<double>;

                            switch (this.ZAxisValueType)
                            {
                                case ChartValueType.DateTime:

                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = Convert.ToDouble(xVal != null ? xVal : double.NaN);

                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(((DateTime)zVal).ToOADate());
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                case ChartValueType.TimeSpan:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = Convert.ToDouble(xVal != null ? xVal : double.NaN);

                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(((TimeSpan)zVal).TotalMilliseconds);
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                case ChartValueType.Double:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = Convert.ToDouble(xVal != null ? xVal : double.NaN);

                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(Convert.ToDouble(zVal));
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;
                                default:
                                    object zValue = this.ZValues;
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = Convert.ToDouble(xVal != null ? xVal : double.NaN);

                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        (zValue as List<string>).Add((string)zVal);
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;

                                    break;
                            }
                        }
                        else if (this.XAxisValueType == ChartValueType.DateTime)
                        {
                            IList<double> xValue = this.XValues as List<double>;
                            
                            switch (this.ZAxisValueType)
                            {
                                case ChartValueType.DateTime:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = ((DateTime)xVal).ToOADate();
                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(((DateTime)zVal).ToOADate());
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                case ChartValueType.TimeSpan:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = ((DateTime)xVal).ToOADate();
                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(((TimeSpan)zVal).TotalMilliseconds);
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                case ChartValueType.Double:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = ((DateTime)xVal).ToOADate();
                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        zDoubleValues.Add(Convert.ToDouble(zVal));
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;
                                default:
                                    object zValue = this.ZValues;
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = ((DateTime)xVal).ToOADate();
                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));
                                        (zValue as List<string>).Add((string)zVal);
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;
                            }
                        }
                        else if (this.XAxisValueType == ChartValueType.TimeSpan)
                        {
                            IList<double> xValue = this.XValues as List<double>;
                            
                            switch (this.ZAxisValueType)
                            {
                                case ChartValueType.DateTime:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = ((TimeSpan)xVal).TotalMilliseconds;
                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));

                                        zDoubleValues.Add(((DateTime)zVal).ToOADate());
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                case ChartValueType.TimeSpan:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = ((TimeSpan)xVal).TotalMilliseconds;
                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));

                                        zDoubleValues.Add(((TimeSpan)zVal).TotalMilliseconds);
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                case ChartValueType.Double:
                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = ((TimeSpan)xVal).TotalMilliseconds;
                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));

                                        zDoubleValues.Add(Convert.ToDouble(zVal));
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;

                                default:
                                    object zValue = this.ZValues;

                                    do
                                    {
                                        object xVal = xGetMethod(enumerator.Current);
                                        object yVal = yGetMethod(enumerator.Current);
                                        object zVal = zGetMethod(enumerator.Current);

                                        this.XData = ((TimeSpan)xVal).TotalMilliseconds;
                                        if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                        {
                                            this.isLinearData = false;
                                        }

                                        xValue.Add(this.XData);
                                        yValue.Add(Convert.ToDouble(yVal != null ? yVal : double.NaN));

                                        (zValue as List<string>).Add((string)zVal);
                                        ActualData.Add(enumerator.Current);
                                    }
                                    while (enumerator.MoveNext());
                                    this.DataCount = xValue.Count;
                                    break;
                            }
                        }

                        this.HookPropertyChangedEvent(this.ListenPropertyChange);
                    }
                }

                this.IsPointGenerated = true;
            }
        }

        /// <summary>
        /// Generates the complex property points
        /// </summary>
        /// <param name="yPaths">The Y Paths</param>
        /// <param name="yLists">The Y Lists</param>
        /// <param name="getPropertyValue">The Reflected Property Value</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Needed For Point Generation Logic")]
        internal override void GenerateComplexPropertyPoints(string[] yPaths, IList<double>[] yLists, GetReflectedProperty getPropertyValue)
        {
            var xyzSeries = this as XyzDataSeries3D;
            if (xyzSeries == null || (xyzSeries != null && (xyzSeries.ZBindingPath == null || xyzSeries.ZBindingPath.Length == 0)))
            {
                base.GenerateComplexPropertyPoints(yPaths, yLists, getPropertyValue);
            }
            else
            {
                IEnumerator enumerator = (ItemsSource as IEnumerable).GetEnumerator();
                if (enumerator.MoveNext())
                {
                    for (int i = 0; i < this.UpdateStartedIndex; i++)
                    {
                        enumerator.MoveNext();
                    }

                    this.XAxisValueType = this.GetDataType(ItemsSource as IEnumerable, this.XComplexPaths);
                    if (this.XAxisValueType == ChartValueType.DateTime || this.XAxisValueType == ChartValueType.Double ||
                        this.XAxisValueType == ChartValueType.Logarithmic || this.XAxisValueType == ChartValueType.TimeSpan)
                    {
                        if (!(XValues is List<double>))
                        {
                            this.ActualXValues = this.XValues = new List<double>();
                        }
                    }
                    else
                    {
                        if (!(XValues is List<string>))
                        {
                            this.ActualXValues = this.XValues = new List<string>();
                        }
                    }

                    if (this.ZAxisValueType == ChartValueType.DateTime || this.ZAxisValueType == ChartValueType.Double ||
                     this.ZAxisValueType == ChartValueType.Logarithmic || this.ZAxisValueType == ChartValueType.TimeSpan)
                    {
                        if (!(this.ZValues is List<double>))
                        {
                            this.ActualZValues = this.ZValues = new List<double>();
                        }
                    }
                    else
                    {
                        if (!(this.ZValues is List<string>))
                        {
                            this.ActualZValues = this.ZValues = new List<string>();
                        }
                    }

                    string[] tempYPath = YComplexPaths[0];
                    if (string.IsNullOrEmpty(yPaths[0]))
                    {
                        return;
                    }

                    IList<double> yValue = yLists[0];
                    object xVal = null, yVal = null, zVal = null;
                    var zDoubleValues = this.ZValues as List<double>;

                    if (this.XAxisValueType == ChartValueType.String)
                    {
                        IList<string> xValue = this.XValues as List<string>;
                        switch (this.ZAxisValueType)
                        {
                            case ChartValueType.DateTime:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);
                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    xValue.Add((string)xVal);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(((DateTime)zVal).ToOADate());
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;

                            case ChartValueType.TimeSpan:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);
                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    xValue.Add((string)xVal);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(((TimeSpan)zVal).TotalMilliseconds);
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;

                            case ChartValueType.Double:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);
                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    xValue.Add((string)xVal);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(Convert.ToDouble(zVal));
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;
                            default:
                                object zValue = this.ZValues;

                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    xValue.Add((string)xVal);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    (zValue as List<string>).Add((string)zVal);
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;
                        }
                    }
                    else if (this.XAxisValueType == ChartValueType.Double ||
                        this.XAxisValueType == ChartValueType.Logarithmic)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        
                        switch (this.ZAxisValueType)
                        {
                            case ChartValueType.DateTime:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = Convert.ToDouble(xVal);

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(((DateTime)zVal).ToOADate());
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;

                            case ChartValueType.TimeSpan:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = Convert.ToDouble(xVal);

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(((TimeSpan)zVal).TotalMilliseconds);
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;

                            case ChartValueType.Double:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = Convert.ToDouble(xVal);

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(Convert.ToDouble(zVal));
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;
                            default:
                                object zValue = this.ZValues;

                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = Convert.ToDouble(xVal);

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    (zValue as List<string>).Add((string)zVal);
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;
                        }
                    }
                    else if (this.XAxisValueType == ChartValueType.DateTime)
                    {
                        IList<double> xValue = this.XValues as List<double>;
                        
                        switch (this.ZAxisValueType)
                        {
                            case ChartValueType.DateTime:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = ((DateTime)xVal).ToOADate();

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(((DateTime)zVal).ToOADate());
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;

                            case ChartValueType.TimeSpan:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = ((DateTime)xVal).ToOADate();

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(((TimeSpan)zVal).TotalMilliseconds);
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;

                            case ChartValueType.Double:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = ((DateTime)xVal).ToOADate();

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(Convert.ToDouble(zVal));
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;
                            default:
                                object zValue = this.ZValues;

                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = ((DateTime)xVal).ToOADate();

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    (zValue as List<string>).Add((string)zVal);
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;
                        }
                    }
                    else if (this.XAxisValueType == ChartValueType.TimeSpan)
                    {
                        IList<double> xValue = this.XValues as List<double>;

                        switch (this.ZAxisValueType)
                        {
                            case ChartValueType.DateTime:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = ((TimeSpan)xVal).TotalMilliseconds;

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(((DateTime)zVal).ToOADate());
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;

                            case ChartValueType.TimeSpan:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = ((TimeSpan)xVal).TotalMilliseconds;

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(((TimeSpan)zVal).TotalMilliseconds);
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;

                            case ChartValueType.Double:
                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = ((TimeSpan)xVal).TotalMilliseconds;

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    zDoubleValues.Add(Convert.ToDouble(zVal));
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;
                            default:
                                object zValue = this.ZValues;

                                do
                                {
                                    xVal = getPropertyValue(enumerator.Current, this.XComplexPaths);
                                    yVal = getPropertyValue(enumerator.Current, tempYPath);
                                    zVal = getPropertyValue(enumerator.Current, this.ZComplexPaths);

                                    if (xVal == null)
                                    {
                                        return;
                                    }

                                    this.XData = ((TimeSpan)xVal).TotalMilliseconds;

                                    // check the Data Collection is linear or not
                                    if (this.isLinearData && xValue.Count > 0 && this.XData < xValue[xValue.Count - 1])
                                    {
                                        this.isLinearData = false;
                                    }

                                    xValue.Add(this.XData);
                                    yValue.Add(Convert.ToDouble(yVal ?? double.NaN));
                                    (zValue as List<string>).Add((string)zVal);
                                    ActualData.Add(enumerator.Current);
                                }
                                while (enumerator.MoveNext());
                                this.DataCount = xValue.Count;
                                break;
                        }
                    }

                    this.HookPropertyChangedEvent(this.ListenPropertyChange);
                }

                this.IsPointGenerated = true;
            }
        }

        /// <summary>
        /// Gets the adornment z position on rotation. 
        /// </summary>
        /// <param name="start">The Start Angle</param>
        /// <param name="end">The End Angle</param>
        /// <returns>Returns the z position to place the adornments.</returns>
        internal double GetZAdornmentAnglePosition(double start, double end)
        {
            var rotation = this.Area.ActualRotationAngle;
            if (rotation >= 0 && rotation < 45 || rotation >= 315 && rotation < 360)
            {
                return start;
            }
            else if (rotation >= 45 && rotation < 135 || rotation >= 225 && rotation < 315)
            {
                return start + (end - start) * 0.5;
            }
            else
            {
                return end + 2;
            }
        }

        /// <summary>
        /// Gets the adornment z position on rotation. 
        /// </summary>
        /// <param name="z">The Z Value</param>
        /// <param name="zsbsInfo">The Z Axis Side By Side Information</param>
        /// <returns>Returns the z position to place the adornments.</returns>
        internal double GetZAdornmentAnglePosition(double z, DoubleRange zsbsInfo)
        {
            var rotation = this.Area.ActualRotationAngle;
            if ((rotation >= 0 && rotation < 45) || (rotation >= 315 && rotation < 360))
            {
                return z + zsbsInfo.Start;
            }
            else if ((rotation >= 45 && rotation < 135) || (rotation >= 225 && rotation < 315))
            {
                return z + zsbsInfo.Median;
            }
            else
            {
                return z + zsbsInfo.End;
            }
        }

        /// <summary>
        /// Gets the adornment x position on rotation.
        /// </summary>
        /// <param name="x">The X Value</param>
        /// <param name="sbsInfo">Side By Side Information</param>
        /// <returns>Returns the x position to place the adornments.</returns>
        internal double GetXAdornmentAnglePosition(double x, DoubleRange sbsInfo)
        {
            var rotation = this.Area.ActualRotationAngle;
            if (this.IsActualTransposed)
            {
                return x + sbsInfo.Start;
            }
            else
            {
                if ((rotation >= 0 && rotation < 45) || (rotation >= 315 && rotation < 360) || (rotation >= 135 && rotation < 225))
                {
                    return x + sbsInfo.Start;
                }
                else if (rotation >= 45 && rotation < 135)
                {
                    return x + sbsInfo.Median;
                }
                else
                {
                    return x + sbsInfo.Start - Math.Abs(sbsInfo.Delta) / 2;
                }
            }
        }

        #endregion
        
        #region Protected Internal Methods

        /// <summary>
        /// Gets the z values.
        /// </summary>
        /// <returns>Returns the z values.</returns>
        protected internal List<double> GetZValues()
        {
            double zIndexValues = 0d;
            if (this.ActualZValues == null)
            {
                return null;
            }

            List<double> zValues = this.ActualZValues as List<double>;
            if (this.IsIndexedZAxis || zValues == null)
            {
                zValues = zValues != null ? (from val in (zValues) select (zIndexValues++)).ToList()
                      : (from val in (this.ActualZValues as List<string>) select (zIndexValues++)).ToList();
            }

            return zValues;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the series on data source changed.
        /// </summary>
        /// <param name="oldValue">The Old Value</param>
        /// <param name="newValue">The New Value</param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (this.ActualZValues != null)
            {
                if (this.ActualZValues is IList<double>)
                {
                    (this.ZValues as IList<double>).Clear();
                    (this.ActualZValues as IList<double>).Clear();
                }
                else if (this.ActualZValues is IList<string>)
                {
                    (this.ZValues as IList<string>).Clear();
                    (this.ActualZValues as IList<string>).Clear();
                }
            }

            base.OnDataSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Method implementation for Set points to given index
        /// </summary>
        /// <param name="index">The Index</param>
        /// <param name="obj">The Object</param>
        /// <param name="replace">Is Replace Required</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Needed For Point Generation Logic")]
        protected override void SetIndividualPoint(int index, object obj, bool replace)
        {
            var xyzSeries = this as XyzDataSeries3D;
            if (xyzSeries == null || (xyzSeries != null && (xyzSeries.ZBindingPath == null || xyzSeries.ZBindingPath.Length == 0)))
            {
                base.SetIndividualPoint(index, obj, replace);
            }
            else
            {
                if (this.SeriesYValues != null && this.YPaths != null && this.ItemsSource != null)
                {
                    object xvalueType = null;
                    xvalueType = this.GetArrayPropertyValue(obj, this.XComplexPaths);
                    if (xvalueType != null)
                    {
                        this.XAxisValueType = GetDataType(xvalueType);
                    }

                    string[] tempYPath = YComplexPaths[0];
                    IList<double> yValue = SeriesYValues[0];
                    if (this.XAxisValueType == ChartValueType.String)
                    {
                        if (!(this.XValues is List<string>))
                        {
                            this.XValues = this.ActualXValues = new List<string>();
                        }

                        IList<string> xValue = this.XValues as List<string>;
                        object xVal = GetArrayPropertyValue(obj, XComplexPaths);
                        object yVal = GetArrayPropertyValue(obj, tempYPath);
                        object zVal = GetArrayPropertyValue(obj, this.ZComplexPaths);

                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = Convert.ToString(xVal);
                        }
                        else
                        {
                            xValue.Insert(index, Convert.ToString(xVal));
                        }

                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal != null ? yVal : double.NaN);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal != null ? yVal : double.NaN));
                        }

                        switch (this.ZAxisValueType)
                        {
                            case ChartValueType.DateTime:
                                IList zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = ((DateTime)zVal).ToOADate();
                                }
                                else
                                {
                                    zValue.Insert(index, ((DateTime)zVal).ToOADate());
                                }

                                break;

                            case ChartValueType.TimeSpan:
                                zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = ((TimeSpan)zVal).TotalMilliseconds;
                                }
                                else
                                {
                                    zValue.Insert(index, ((TimeSpan)zVal).TotalMilliseconds);
                                }

                                break;

                            case ChartValueType.Double:
                                zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = Convert.ToDouble(zVal);
                                }
                                else
                                {
                                    zValue.Insert(index, Convert.ToDouble(zVal));
                                }

                                break;
                            default:
                                zValue = this.ZValues as List<string>;
                                var zString = (string)zVal;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = zString;
                                }
                                else
                                {
                                    zValue.Insert(index, zString);
                                }

                                break;
                        }

                        this.DataCount = xValue.Count;
                    }
                    else if (this.XAxisValueType == ChartValueType.Double ||
                        this.XAxisValueType == ChartValueType.Logarithmic)
                    {
                        if (!(this.XValues is List<double>))
                        {
                            this.XValues = this.ActualXValues = new List<double>();
                        }

                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetArrayPropertyValue(obj, XComplexPaths);
                        object yVal = GetArrayPropertyValue(obj, tempYPath);
                        object zVal = GetArrayPropertyValue(obj, this.ZComplexPaths);

                        this.XData = (xVal != null ? Convert.ToDouble(xVal) : double.NaN);

                        // check the Data Collection is linear or not
                        if (this.isLinearData && index > 0 && this.XData < xValue[index - 1])
                        {
                            this.isLinearData = false;
                        }

                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = this.XData;
                        }
                        else
                        {
                            xValue.Insert(index, this.XData);
                        }

                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal != null ? yVal : double.NaN);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal != null ? yVal : double.NaN));
                        }

                        switch (this.ZAxisValueType)
                        {
                            case ChartValueType.DateTime:
                                IList zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = ((DateTime)zVal).ToOADate();
                                }
                                else
                                {
                                    zValue.Insert(index, ((DateTime)zVal).ToOADate());
                                }

                                break;

                            case ChartValueType.TimeSpan:
                                zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = ((TimeSpan)zVal).TotalMilliseconds;
                                }
                                else
                                {
                                    zValue.Insert(index, ((TimeSpan)zVal).TotalMilliseconds);
                                }

                                break;

                            case ChartValueType.Double:
                                zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = Convert.ToDouble(zVal);
                                }
                                else
                                {
                                    zValue.Insert(index, Convert.ToDouble(zVal));
                                }

                                break;
                            default:
                                zValue = this.ZValues as List<string>;
                                var zString = (string)zVal;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = zString;
                                }
                                else
                                {
                                    zValue.Insert(index, zString);
                                }

                                break;
                        }

                        this.DataCount = xValue.Count;
                    }
                    else if (this.XAxisValueType == ChartValueType.DateTime)
                    {
                        if (!(this.XValues is List<double>))
                        {
                            this.XValues = this.ActualXValues = new List<double>();
                        }

                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetArrayPropertyValue(obj, XComplexPaths);
                        object yVal = GetArrayPropertyValue(obj, tempYPath);
                        object zVal = GetArrayPropertyValue(obj, this.ZComplexPaths);

                        this.XData = Convert.ToDateTime(xVal).ToOADate();

                        // check the Data Collection is linear or not
                        if (isLinearData && index > 0 && this.XData < xValue[index - 1])
                        {
                            this.isLinearData = false;
                        }

                        if (replace && xValue.Count > index)
                        {
                            xValue[index] = this.XData;
                        }
                        else
                        {
                            xValue.Insert(index, this.XData);
                        }

                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal != null ? yVal : double.NaN);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal != null ? yVal : double.NaN));
                        }

                        switch (this.ZAxisValueType)
                        {
                            case ChartValueType.DateTime:
                                IList zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = ((DateTime)zVal).ToOADate();
                                }
                                else
                                {
                                    zValue.Insert(index, ((DateTime)zVal).ToOADate());
                                }

                                break;

                            case ChartValueType.TimeSpan:
                                zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = ((TimeSpan)zVal).TotalMilliseconds;
                                }
                                else
                                {
                                    zValue.Insert(index, ((TimeSpan)zVal).TotalMilliseconds);
                                }

                                break;

                            case ChartValueType.Double:
                                zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = Convert.ToDouble(zVal);
                                }
                                else
                                {
                                    zValue.Insert(index, Convert.ToDouble(zVal));
                                }

                                break;
                            default:
                                zValue = this.ZValues as List<string>;
                                var zString = (string)zVal;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = zString;
                                }
                                else
                                {
                                    zValue.Insert(index, zString);
                                }

                                break;
                        }

                        this.DataCount = xValue.Count;
                    }
                    else if (this.XAxisValueType == ChartValueType.TimeSpan)
                    {
                        if (!(this.XValues is List<double>))
                        {
                            this.XValues = this.ActualXValues = new List<double>();
                        }

                        IList<double> xValue = this.XValues as List<double>;
                        object xVal = GetArrayPropertyValue(obj, XComplexPaths);
                        object yVal = GetArrayPropertyValue(obj, tempYPath);
                        object zVal = GetArrayPropertyValue(obj, this.ZComplexPaths);

                        this.XData = ((TimeSpan)xVal).TotalMilliseconds;

                        // check the Data Collection is linear or not
                        if (this.isLinearData && index > 0 && this.XData < xValue[index - 1])
                        {
                            this.isLinearData = false;
                        }

                        if (xVal != null && replace && xValue.Count > index)
                        {
                            xValue[index] = this.XData;
                        }
                        else if (xVal != null)
                        {
                            xValue.Insert(index, this.XData);
                        }

                        if (replace && yValue.Count > index)
                        {
                            yValue[index] = Convert.ToDouble(yVal != null ? yVal : double.NaN);
                        }
                        else
                        {
                            yValue.Insert(index, Convert.ToDouble(yVal != null ? yVal : double.NaN));
                        }
                        
                        switch (this.ZAxisValueType)
                        {
                            case ChartValueType.DateTime:
                                IList zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = ((DateTime)zVal).ToOADate();
                                }
                                else
                                {
                                    zValue.Insert(index, ((DateTime)zVal).ToOADate());
                                }

                                break;

                            case ChartValueType.TimeSpan:
                                zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = ((TimeSpan)zVal).TotalMilliseconds;
                                }
                                else
                                {
                                    zValue.Insert(index, ((TimeSpan)zVal).TotalMilliseconds);
                                }

                                break;

                            case ChartValueType.Double:
                                zValue = this.ZValues as List<double>;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = Convert.ToDouble(zVal);
                                }
                                else
                                {
                                    zValue.Insert(index, Convert.ToDouble(zVal));
                                }

                                break;
                            default:
                                zValue = this.ZValues as List<string>;
                                var zString = (string)zVal;
                                if (replace && zValue.Count > index)
                                {
                                    zValue[index] = zString;
                                }
                                else
                                {
                                    zValue.Insert(index, zString);
                                }

                                break;
                        }

                        this.DataCount = xValue.Count;
                    }

                    if (replace && ActualData.Count > index)
                    {
                        this.ActualData[index] = obj;
                    }
                    else if (ActualData.Count == index)
                    {
                        ActualData.Add(obj);
                    }
                    else
                    {
                        ActualData.Insert(index, obj);
                    }

                    this.totalCalculated = false;
                }

                this.UpdateEmptyPoints(index);

                this.HookPropertyChangedEvent(this.ListenPropertyChange, obj);
            }
        }
        
        /// <summary>
        /// Updates the series on binding path changed.
        /// </summary>
        /// <param name="args">The Event Arguments</param>
        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ActualZValues != null)
            {
                if (this.ActualZValues is IList<double>)
                {
                    (this.ZValues as IList<double>).Clear();
                    (this.ActualZValues as IList<double>).Clear();
                }
                else if (this.ActualZValues is IList<string>)
                {
                    (this.ZValues as IList<string>).Clear();
                    (this.ActualZValues as IList<string>).Clear();
                }
            }

            base.OnBindingPathChanged(args);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the series on z binding path changed.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnBindingPathZChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var series = (obj as XyzDataSeries3D);
            if (args.NewValue != null)
            {
                series.ZComplexPaths = args.NewValue.ToString().Split('.');
            }

            series.OnBindingPathChanged(args);
        }

        #endregion

        #endregion
    }
}
