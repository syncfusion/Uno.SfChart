// <copyright file="CartesianSeries3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Class implementation for CartesianSeries
    /// </summary>
    public abstract partial class CartesianSeries3D : ChartSeries3D, ISupportAxes3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="IsTransposed"/> property.
        /// </summary>
        public static readonly DependencyProperty IsTransposedProperty =
            DependencyProperty.Register(
                "IsTransposed",
                typeof(bool),
                typeof(CartesianSeries3D),
                new PropertyMetadata(false, OnTransposeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="XAxis"/> property.
        /// </summary>
        internal static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register(
                "XAxis",
                typeof(ChartAxisBase3D),
                typeof(CartesianSeries3D),
                new PropertyMetadata(null, OnXAxisChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="YAxis"/> property.
        /// </summary>
        internal static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register(
                "YAxis",
                typeof(RangeAxisBase3D),
                typeof(CartesianSeries3D),
                new PropertyMetadata(null, OnYAxisChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the x-axis range. 
        /// </summary>
        public DoubleRange XRange { get; internal set; }

        /// <summary>
        /// Gets the y-axis range. 
        /// </summary>
        public DoubleRange YRange { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether to change the orientation of the series.
        /// </summary>
        /// <value>
        ///     <c>True</c> exchanges the horizontal axis to vertical and vice versa. 
        ///     <c>False</c> is the default behavior.
        /// </value>
        public bool IsTransposed
        {
            get { return (bool)GetValue(IsTransposedProperty); }
            set { this.SetValue(IsTransposedProperty, value); }
        }
        
        /// <summary>
        /// Gets the actual x-axis instance.
        /// </summary>
        ChartAxis ISupportAxes.ActualXAxis
        {
            get { return this.ActualXAxis; }
        }

        /// <summary>
        /// Gets the actual y-axis instance.
        /// </summary>
        ChartAxis ISupportAxes.ActualYAxis
        {
            get { return this.ActualYAxis; }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the additional x-axis(horizontal) for the series.
        /// </summary>
        internal ChartAxisBase3D XAxis
        {
            get { return (ChartAxisBase3D)GetValue(XAxisProperty); }
            set { this.SetValue(XAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the additional y-axis(vertical) for the series.
        /// </summary>
        internal RangeAxisBase3D YAxis
        {
            get { return (RangeAxisBase3D)GetValue(YAxisProperty); }
            set { this.SetValue(YAxisProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Updates the range of the series.
        /// </summary>
        internal override void UpdateRange()
        {
            this.XRange = DoubleRange.Empty;
            this.YRange = DoubleRange.Empty;
            var xyzDataSeries = this as XyzDataSeries3D;
            bool isZAxis = xyzDataSeries != null && !string.IsNullOrEmpty(xyzDataSeries.ZBindingPath);

            if (isZAxis)
            {
                xyzDataSeries.ZRange = DoubleRange.Empty;
            }

            foreach (var segment in this.Segments)
            {
                this.XRange += segment.XRange;
                this.YRange += segment.YRange;
                if (isZAxis)
                {
                    xyzDataSeries.ZRange += (segment as ChartSegment3D).ZRange;
                }
            }

            if (this.IsSideBySide)
            {
                if (this.SideBySideInfoRangePad != null && !SideBySideInfoRangePad.IsEmpty)
                {
                    bool isAlterRange = ((this.ActualXAxis is NumericalAxis3D && (this.ActualXAxis as NumericalAxis3D).RangePadding == NumericalPadding.None)
                   || (this.ActualXAxis is DateTimeAxis3D && (this.ActualXAxis as DateTimeAxis3D).RangePadding == DateTimeRangePadding.None));
                    this.XRange = isAlterRange ? new DoubleRange(this.XRange.Start - SideBySideInfoRangePad.Start, this.XRange.End - SideBySideInfoRangePad.End)
                        : new DoubleRange(this.XRange.Start + SideBySideInfoRangePad.Start, this.XRange.End + SideBySideInfoRangePad.End);

                    if (isZAxis)
                    {
                        bool isAlterZRange = ((xyzDataSeries.ActualZAxis is NumericalAxis3D && (xyzDataSeries.ActualZAxis as NumericalAxis3D).RangePadding == NumericalPadding.None)
                    || (xyzDataSeries.ActualZAxis is DateTimeAxis3D && (xyzDataSeries.ActualZAxis as DateTimeAxis3D).RangePadding == DateTimeRangePadding.None));
                        xyzDataSeries.ZRange = isAlterZRange ? new DoubleRange(xyzDataSeries.ZRange.Start - xyzDataSeries.ZSideBySideInfoRangePad.Start, xyzDataSeries.ZRange.End - xyzDataSeries.ZSideBySideInfoRangePad.End)
                            : new DoubleRange(xyzDataSeries.ZRange.Start + xyzDataSeries.ZSideBySideInfoRangePad.Start, xyzDataSeries.ZRange.End + xyzDataSeries.ZSideBySideInfoRangePad.End);
                    }
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when instance created for YAxis Changed 
        /// </summary>
        /// <param name="oldAxis">The Old Axis</param>
        /// <param name="newAxis">The New Axis</param>
        protected virtual void OnYAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (newAxis != null && !newAxis.RegisteredSeries.Contains(this))
            {
                if (this.Area != null && !Area.Axes.Contains(newAxis))
                {
                    Area.Axes.Add(newAxis);
                }

                newAxis.Area = this.Area;
                newAxis.Orientation = Orientation.Vertical;
                newAxis.RegisteredSeries.Add(this);
            }

            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                {
                    oldAxis.RegisteredSeries.Remove(this);
                }

                if (this.Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                    {
                        Area.Axes.Remove(oldAxis);
                    }
                }
            }

            if (this.Area != null)
            {
                Area.ScheduleUpdate();
            }

            if (newAxis != null)
            {
                newAxis.Orientation = this.IsActualTransposed ? Orientation.Horizontal : Orientation.Vertical;
            }
        }

        /// <summary>
        /// Called when instance created for XAxis changed
        /// </summary>
        /// <param name="oldAxis">The Old Axis</param>
        /// <param name="newAxis">The New Axis</param>
        protected virtual void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                oldAxis.VisibleRangeChanged -= this.OnVisibleRangeChanged;

                if (oldAxis.RegisteredSeries.Contains(this))
                {
                    oldAxis.RegisteredSeries.Remove(this);
                }

                if (this.Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                    {
                        Area.Axes.Remove(oldAxis);
                    }
                }
            }

            if (newAxis != null)
            {
                if (this.Area != null && !Area.Axes.Contains(newAxis) && newAxis != Area.InternalPrimaryAxis)
                {
                    Area.Axes.Add(newAxis);
                }

                newAxis.Area = this.Area;
                newAxis.Orientation = Orientation.Horizontal;
                if (!newAxis.RegisteredSeries.Contains(this))
                {
                    newAxis.RegisteredSeries.Add(this);
                }

                newAxis.VisibleRangeChanged += this.OnVisibleRangeChanged;
            }

            if (this.Area != null)
            {
                this.Area.ScheduleUpdate();
            }

            if (newAxis != null)
            {
                newAxis.Orientation = this.IsActualTransposed ? Orientation.Vertical : Orientation.Horizontal;
            }
        }

        /// <summary>
        /// Called when VisibleRange property changed
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected virtual void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
        }

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            if ((obj as CartesianSeries != null))
            {
                (obj as CartesianSeries3D).IsTransposed = this.IsTransposed;
            }

            return base.CloneSeries(obj);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the series when <see cref="IsTransposed"/> property changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnTransposeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CartesianSeries3D).OnTransposeChanged((bool)e.NewValue);
        }

        /// <summary>
        /// Updates the series when x axis changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CartesianSeries3D)d).OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        /// <summary>
        /// Updates the series when the y axis changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CartesianSeries3D)d).OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the series when visible range changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        private void OnVisibleRangeChanged(object sender, VisibleRangeChangedEventArgs e)
        {
            this.OnVisibleRangeChanged(e);
        }

        #endregion

        #endregion
    }
}
