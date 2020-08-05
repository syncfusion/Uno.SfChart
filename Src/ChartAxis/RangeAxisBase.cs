// <copyright file="RangeAxisBase.cs" company="Syncfusion. Inc">
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
    using System.Text;
    using System.Threading;
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for RangeAxisBase
    /// </summary>
    public abstract partial class RangeAxisBase : ChartAxisBase2D, IRangeAxis
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="IncludeAnnotationRange"/> property.
        /// </summary>
        public static readonly DependencyProperty IncludeAnnotationRangeProperty =
            DependencyProperty.Register(
                "IncludeAnnotationRange",
                typeof(bool),
                typeof(RangeAxisBase),
                new PropertyMetadata(false, OnIncludeAnnotationRangeChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="SmallTicksPerInterval"/> property.
        /// </summary>
        public static readonly DependencyProperty SmallTicksPerIntervalProperty =
            DependencyProperty.Register(
                "SmallTicksPerInterval",
                typeof(int),
                typeof(ChartAxis),
                new PropertyMetadata(0, new PropertyChangedCallback(OnSmallTicksPerIntervalPropertyChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="SmallTickLineSize"/> property.
        /// </summary>
        public static readonly DependencyProperty SmallTickLineSizeProperty =
            DependencyProperty.Register(
                "SmallTickLineSize",
                typeof(double),
                typeof(ChartAxis),
                new PropertyMetadata(5d, OnSmallTicksPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SmallTickLinesPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty SmallTickLinesPositionProperty =
            DependencyProperty.Register(
                "SmallTickLinesPosition",
                typeof(AxisElementPosition),
                typeof(ChartAxis),
                new PropertyMetadata(AxisElementPosition.Outside, OnSmallTicksPropertyChanged));

        #endregion

        #region Properties

#region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to enable the axis to include the annotation range while calculating the axis range.
        /// </summary>
        public bool IncludeAnnotationRange
        {
            get { return (bool)GetValue(IncludeAnnotationRangeProperty); }
            set { SetValue(IncludeAnnotationRangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the small tick lines interval.
        /// </summary>
        public int SmallTicksPerInterval
        {
            get { return (int)GetValue(SmallTicksPerIntervalProperty); }
            set { SetValue(SmallTicksPerIntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets minor tick line size.
        /// </summary>
        public double SmallTickLineSize
        {
            get { return (double)GetValue(SmallTickLineSizeProperty); }
            set { SetValue(SmallTickLineSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets small tick lines position, either inside or outside.
        /// </summary>
        public AxisElementPosition SmallTickLinesPosition
        {
            get { return (AxisElementPosition)GetValue(SmallTickLinesPositionProperty); }
            set { SetValue(SmallTickLinesPositionProperty, value); }
        }
        
        DoubleRange IRangeAxis.Range
        {
            get { return ActualRange; }
        }

        #endregion

        #region Protected Properties

        protected DoubleRange Range
        {
            get { return ActualRange; }
        }

        #endregion
        
        #endregion

        #region Methods

        #region Protected Internal Methods

        /// <summary>
        /// Method implementation for Add SamllTicksPoint
        /// </summary>
        /// <param name="position"></param>
        protected internal override void AddSmallTicksPoint(double position)
        {
            RangeAxisBaseHelper.AddSmallTicksPoint(this, position, VisibleInterval, SmallTicksPerInterval);
        }

        /// <summary>
        /// Method implementation for Add smallTicks to axis
        /// </summary>
        /// <param name="position"></param>
        /// <param name="interval"></param>
        protected internal override void AddSmallTicksPoint(double position, double interval)
        {
            RangeAxisBaseHelper.AddSmallTicksPoint(this, position, interval, SmallTicksPerInterval);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            RangeAxisBaseHelper.GenerateVisibleLabels(this, SmallTickLineSize);
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var rangeAxisBase = obj as RangeAxisBase;
            rangeAxisBase.SmallTicksPerInterval = this.SmallTicksPerInterval;
            rangeAxisBase.SmallTickLinesPosition = this.SmallTickLinesPosition;
            rangeAxisBase.SmallTickLineSize = this.SmallTickLineSize;
            rangeAxisBase.IncludeAnnotationRange = this.IncludeAnnotationRange;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Static Methods

        private static void OnIncludeAnnotationRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        private static void OnSmallTicksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        private static void OnSmallTicksPerIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.smallTicksRequired = (int)e.NewValue > 0 ? true : axis.smallTicksRequired;
                if (axis.Area != null)
                    axis.Area.ScheduleUpdate();
            }
        }

        #endregion

        #endregion
    }
}
