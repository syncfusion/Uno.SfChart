using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for DateTimeAxis
    /// </summary>
    public partial class DateTimeAxis : RangeAxisBase
    {
        #region Dependency Property Registration
        
        // Using a DependencyProperty as the backing store for AutoScrollingDeltaType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollingDeltaTypeProperty =
            DependencyProperty.Register(
                "AutoScrollingDeltaType", 
                typeof(DateTimeIntervalType),
                typeof(DateTimeAxis),
                new PropertyMetadata(DateTimeIntervalType.Auto, OnAutoScrollingDeltaTypeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableBusinessHours"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableBusinessHoursProperty =
            DependencyProperty.Register(
                "EnableBusinessHours",
                typeof(bool), 
                typeof(DateTimeAxis),
                new PropertyMetadata(false, OnEnableBusinessHoursChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="OpenTime"/> property.
        /// </summary>
        public static readonly DependencyProperty OpenTimeProperty =
            DependencyProperty.Register(
                "OpenTime", 
                typeof(double), 
                typeof(DateTimeAxis),
                new PropertyMetadata(0d));

        /// <summary>
        /// The DependencyProperty for <see cref="CloseTime"/> property.
        /// </summary>
        public static readonly DependencyProperty CloseTimeProperty =
            DependencyProperty.Register(
                "CloseTime", 
                typeof(double), 
                typeof(DateTimeAxis),
                new PropertyMetadata(24d));

        /// <summary>
        /// The DependencyProperty for <see cref="WorkingDays"/> property.
        /// </summary>
        public static readonly DependencyProperty WorkingDaysProperty =
            DependencyProperty.Register(
                "WorkingDays",
                typeof(Day),
                typeof(DateTimeAxis),
                new PropertyMetadata(Day.Monday | Day.Tuesday | Day.Wednesday | Day.Thursday | Day.Friday, OnWorkDaysChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(object), typeof(DateTimeAxis), new PropertyMetadata(null, OnMinimumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(object), typeof(DateTimeAxis), new PropertyMetadata(null, OnMaximumChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(double),
                typeof(DateTimeAxis),
                new PropertyMetadata(0d, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="RangePadding"/> property.
        /// </summary>
        public static readonly DependencyProperty RangePaddingProperty =
            DependencyProperty.Register(
                "RangePadding",
                typeof(DateTimeRangePadding),
                typeof(DateTimeAxis),
                new PropertyMetadata(DateTimeRangePadding.Auto, new PropertyChangedCallback(OnRangePaddingChanged)));

        /// <summary>
        ///  The DependencyProperty for <see cref="IntervalType"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalTypeProperty =
            DependencyProperty.Register(
                "IntervalType", 
                typeof(DateTimeIntervalType),
                typeof(DateTimeAxis),
                new PropertyMetadata(
                    DateTimeIntervalType.Auto,
                    new PropertyChangedCallback(OnIntervalTypeChanged)));

        #endregion

        #region Fields

        #region Internal Fields

        internal double TotalNonWorkingHours;

        internal List<string> NonWorkingDays;

        #endregion

        #region Private Fields
        
        private DateTimeIntervalType dateTimeIntervalType = DateTimeIntervalType.Auto;

        private double nonWorkingHoursPerDay;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the date time unit of the value specified in the <c>AutoScrollingDelta</c> property.
        /// </summary>
        public DateTimeIntervalType AutoScrollingDeltaType
        {
            get { return (DateTimeIntervalType)GetValue(AutoScrollingDeltaTypeProperty); }
            set { SetValue(AutoScrollingDeltaTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable only the business hours for the DateTime axis.
        /// </summary>
        public bool EnableBusinessHours
        {
            get { return (bool)GetValue(EnableBusinessHoursProperty); }
            set { SetValue(EnableBusinessHoursProperty, value); }
        }

        /// <summary>
        /// Gets or sets the business open time.
        /// </summary>
        public double OpenTime
        {
            get { return (double)GetValue(OpenTimeProperty); }
            set { SetValue(OpenTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the business closing time.
        /// </summary>
        public double CloseTime
        {
            get { return (double)GetValue(CloseTimeProperty); }
            set { SetValue(CloseTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the flagged enum to selected the list of working days in a business week.
        /// </summary>
        public Day WorkingDays
        {
            get { return (Day)GetValue(WorkingDaysProperty); }
            set { SetValue(WorkingDaysProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum value for the axis range.
        /// </summary>
        /// <remarks>
        ///     If we didn't set the minimum value, it will be calculate from the underlying collection.
        /// </remarks>
        public object Minimum
        {
            get { return GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum value for the axis range.
        /// </summary>
        /// <remarks>
        ///     If we didn't set the maximum value, it will be calculate from the underlying collection.
        /// </remarks>
        public object Maximum
        {
            get { return GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that determines the interval between labels.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the padding used to shift the DateTimeAxis range inside or outside.
        /// </summary>
        /// <value>
        ///     <c>Additional</c>, to extend the range,
        ///     <c>Round</c>, to round-off the range,
        ///     <c>None</c>, do nothing,
        ///     <c>Auto</c>, auto range based on type of series.
        /// </value>
        public DateTimeRangePadding RangePadding
        {
            get { return (DateTimeRangePadding)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of interval to be displayed in axis.
        /// </summary>
        /// <remarks>
        ///     This property hold the values ranges from Year, Months to Milliseconds.
        /// </remarks>
        public DateTimeIntervalType IntervalType
        {
            get { return (DateTimeIntervalType)GetValue(IntervalTypeProperty); }
            set { SetValue(IntervalTypeProperty, value); }
        }

        #endregion

        #region Internal Properties

        internal string InternalWorkingDays { get; set; }

        internal DateTimeIntervalType ActualIntervalType
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Public Metheds

        /// <summary>
        /// Calculate the non working hours between two dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="workingDays"></param>
        /// <param name="nonWorkingHoursPerDay"></param>
        /// <returns></returns>
        public double CalcNonWorkingHours(DateTime startDate, DateTime endDate, string workingDays, double nonWorkingHoursPerDay)
        {
            double totalNonWorkinghours;
            var trimStart = DateTime.MinValue;
            var trimEnd = DateTime.MinValue;
            double totalWeek = 0;
            var nonWorkingHours = 0;
            var lastWeekEndCount = 0;
            if (startDate == endDate)
                return 0;

            for (var i = startDate; i <= endDate.AddHours(new TimeSpan(23, 59, 59).TotalHours - endDate.Hour); i = i.AddDays(1))
            {
                if (i.DayOfWeek != DayOfWeek.Saturday)
                {
                    if (!workingDays.Contains(i.DayOfWeek.ToString()) && i != startDate)
                    {
                        lastWeekEndCount++;
                    }
                    else if (i != startDate)
                    {
                        nonWorkingHours++;
                    }

                    continue;
                }

                if (i != startDate)
                {
                    trimStart = i;
                    break;
                }
            }

            for (var i = endDate; i >= startDate.AddHours(-startDate.Hour); i = i.AddDays(-1))
            {
                if (i.DayOfWeek != DayOfWeek.Friday)
                {
                    if (!workingDays.Contains(i.DayOfWeek.ToString()) && (int)i.ToOADate() != (int)startDate.ToOADate() && i != endDate && trimStart != DateTime.MinValue)
                    {
                        lastWeekEndCount++;
                    }
                    else if ((int)i.ToOADate() != (int)startDate.ToOADate() && trimStart != DateTime.MinValue)
                    {
                        nonWorkingHours++;
                    }

                    continue;
                }

                trimEnd = i;
                break;
            }

            if (trimEnd != DateTime.MinValue && trimStart != DateTime.MinValue)
            {
                totalWeek =
                    Math.Round((double.IsNegativeInfinity(trimEnd.ToOADate() - trimStart.ToOADate())
                        ? 0
                        : trimEnd.ToOADate() - trimStart.ToOADate()) / 7);
            }

            totalNonWorkinghours = ((totalWeek * NonWorkingDays.Count) * 24 + (lastWeekEndCount * 24) +
                                    (totalWeek * (7 - NonWorkingDays.Count)) * nonWorkingHoursPerDay +
                                    nonWorkingHours * nonWorkingHoursPerDay);
            return totalNonWorkinghours;
        }

        #endregion

        #region Public Override Methods

        /// <summary>
        /// Return object value from the given double value
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            if (CustomLabels.Count > 0 || LabelsSource != null)
            {
                return GetCustomLabelContent(position) ?? GetActualLabelContent(position);
            }
            else
                return GetActualLabelContent(position);
        }

        #endregion

        #region Internal Overide Methods
        
        internal override void UpdateAutoScrollingDelta(double autoScrollingDelta)
        {
            var dateTime = ActualRange.End.FromOADate();

            if (double.IsNaN(AutoScrollingDelta)) return;

            var scrollingDelta = (double)AutoScrollingDelta;

            switch (GetActualAutoScrollingDeltaType())
            {
                case DateTimeIntervalType.Years:
                    var value = dateTime.AddYears((int)-AutoScrollingDelta);
                    autoScrollingDelta = ActualRange.End - value.ToOADate();
                    break;
                case DateTimeIntervalType.Months:
                    value = dateTime.AddMonths((int)-AutoScrollingDelta);
                    autoScrollingDelta = ActualRange.End - value.ToOADate();
                    break;
                case DateTimeIntervalType.Days:
                    autoScrollingDelta = scrollingDelta;
                    break;
                case DateTimeIntervalType.Hours:
                    autoScrollingDelta = TimeSpan.FromHours(scrollingDelta).TotalDays;
                    break;
                case DateTimeIntervalType.Minutes:
                    autoScrollingDelta = TimeSpan.FromMinutes(scrollingDelta).TotalDays;
                    break;
                case DateTimeIntervalType.Seconds:
                    autoScrollingDelta = TimeSpan.FromSeconds(scrollingDelta).TotalDays;
                    break;
                case DateTimeIntervalType.Milliseconds:
                    autoScrollingDelta = TimeSpan.FromMilliseconds(scrollingDelta).TotalDays;
                    break;
            }

            base.UpdateAutoScrollingDelta(autoScrollingDelta);
        }

        internal override object GetActualLabelContent(double position)
        {
            return position.FromOADate().ToString(this.LabelFormat, CultureInfo.CurrentCulture);
        }

        #endregion

        #region Protected Internal Override Methods

        protected internal override double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {
            DateTime dateTimeMin = actualRange.Start.FromOADate();
            DateTime dateTimeMax = actualRange.End.FromOADate();
            TimeSpan timeSpan = dateTimeMax.Subtract(dateTimeMin);
            double interval = 0;
            switch (IntervalType)
            {
                case DateTimeIntervalType.Years:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 365), availableSize);
                    break;
                case DateTimeIntervalType.Months:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize);
                    break;
                case DateTimeIntervalType.Days:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize);
                    break;
                case DateTimeIntervalType.Hours:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize);
                    break;
                case DateTimeIntervalType.Minutes:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize);
                    break;
                case DateTimeIntervalType.Seconds:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize);
                    break;
                case DateTimeIntervalType.Milliseconds:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize);
                    break;
                case DateTimeIntervalType.Auto:
                    DoubleRange range = new DoubleRange(0, timeSpan.TotalDays / 365);
                    interval = base.CalculateNiceInterval(range, availableSize);

                    if (interval >= 1)
                    {
                        ActualIntervalType = DateTimeIntervalType.Years;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize);

                    if (interval >= 1)
                    {
                        ActualIntervalType = DateTimeIntervalType.Months;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize);

                    if (interval >= 1)
                    {
                        ActualIntervalType = DateTimeIntervalType.Days;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize);

                    if (interval >= 1)
                    {
                        ActualIntervalType = DateTimeIntervalType.Hours;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize);

                    if (interval >= 1)
                    {
                        ActualIntervalType = DateTimeIntervalType.Minutes;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize);

                    if (interval >= 1)
                    {
                        ActualIntervalType = DateTimeIntervalType.Seconds;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize);

                    ActualIntervalType = DateTimeIntervalType.Milliseconds;

                    break;
            }

            return interval;
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (EnableBusinessHours)
            {
                CalculateNonWorkingDays(range);
                range = new DoubleRange(range.Start, range.End - nonWorkingHoursPerDay / 24);
            }

            if (Interval == 0 || double.IsNaN(Interval))
                return CalculateNiceInterval(range, availableSize);
            else if (IntervalType == DateTimeIntervalType.Auto)
                CalculateNiceInterval(range, availableSize);
            return Interval;
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal override void CalculateVisibleRange(Size availableSize)
        {
            base.CalculateVisibleRange(availableSize);
            if (EnableBusinessHours)
            {
                var actualStart = ActualRange.Start.FromOADate();
                var actualEnd = ActualRange.End.FromOADate();

                // If actual range greater/less than business hours then change range start/end value as business range
                while (NonWorkingDays.Contains(actualStart.DayOfWeek.ToString()))
                {
                    var date = actualStart.AddDays(-1);
                    actualStart = date.AddHours(CloseTime == 24 ? 0 : CloseTime);
                }

                while (NonWorkingDays.Contains(actualEnd.DayOfWeek.ToString()))
                {
                    var date = actualEnd.AddDays(-1);
                    actualEnd = date.AddHours(CloseTime == 24 ? 0 : CloseTime);
                }

                if (actualStart.TimeOfDay.TotalHours < OpenTime)
                {
                    actualStart = actualStart.AddHours(OpenTime - actualStart.TimeOfDay.TotalHours);
                }
                else if (actualStart.TimeOfDay.TotalHours > CloseTime)
                {
                    actualStart = actualStart.AddHours(-(actualStart.TimeOfDay.TotalHours - CloseTime));
                }

                if (actualEnd.TimeOfDay.TotalHours < OpenTime)
                {
                    actualEnd = actualEnd.AddHours(OpenTime - actualEnd.TimeOfDay.TotalHours);
                }
                else if (actualEnd.TimeOfDay.TotalHours > CloseTime)
                {
                    actualEnd = actualEnd.AddHours(-(actualEnd.TimeOfDay.TotalHours - CloseTime));
                }

                if (ZoomPosition > 0 || ZoomFactor < 1)
                {
                    var diff = CloseTime - OpenTime; // hours differce from endtime to open time
                    var totalWorkingHours = CalcNonWorkingHours(actualStart, actualEnd, InternalWorkingDays.ToString(), nonWorkingHoursPerDay);
                    var currentWorkingHours = ((ActualRange.Delta * 24) - totalWorkingHours) * ZoomPosition; // current working hours for zoom postion value
                    var totalDays = currentWorkingHours / diff; // total days without nonworking hours
                    var remainderHours = (totalDays - (int)totalDays);
                    var startDate = actualStart.AddDays((int)totalDays).AddHours(remainderHours * diff);

                    if (NonWorkingDays.Count > 0)
                    {
                        var weekEndCount = CalculateWeekEndDayCount(NonWorkingDays[0], actualStart, startDate); // find the weekend occurrence from start and end date
                        startDate = startDate.AddDays(weekEndCount * NonWorkingDays.Count);
                        while (NonWorkingDays.Contains(startDate.DayOfWeek.ToString()))
                        {
                            startDate = startDate.AddDays(1);
                        }
                    }

                    var currentWorkingHours1 = ((ActualRange.Delta * 24) - totalWorkingHours) * ZoomFactor; // working hours for zoomfactor value
                    var totalDays1 = currentWorkingHours1 / diff;
                    var remainderHours1 = totalDays1 - (int)totalDays1;
                    var endDate = startDate.AddDays((int)totalDays1);
                    endDate = (remainderHours1 * diff) > (CloseTime - endDate.TimeOfDay.TotalHours) ?
                        new DateTime(endDate.Year, endDate.Month, endDate.Day).AddDays(1).AddHours(OpenTime + ((remainderHours1 * diff) - (CloseTime - endDate.TimeOfDay.TotalHours))) :
                        endDate.AddHours(remainderHours1 * diff); // calculate end date

                    if (NonWorkingDays.Count > 0)
                    {
                        var weekEndCount = CalculateWeekEndDayCount(NonWorkingDays[0], startDate, endDate);
                        endDate = endDate.AddDays(weekEndCount * NonWorkingDays.Count);
                        while (NonWorkingDays.Contains(endDate.DayOfWeek.ToString()))
                        {
                            endDate = endDate.AddDays(1);
                        }
                    }

                    VisibleRange = new DoubleRange(startDate.ToOADate(), endDate.ToOADate()); // set the visible range
                }
                else
                    VisibleRange = new DoubleRange(actualStart.ToOADate(), actualEnd.ToOADate());
            }

            DateTimeAxisHelper.CalculateVisibleRange(this, availableSize, Interval);
            if (EnableBusinessHours)
            {
                CalculateNonWorkingDays(VisibleRange);
            }
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Called when Maximum property changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        /// <summary>
        /// Called when minimum property Changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            OnMinMaxChanged();
        }

        /// <summary>
        /// Called when Interval property changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        #endregion

        #region Protected Override Methods


        /// <summary>
        /// Method implementation for Create VisibleLabels for DateTime axis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            SetRangeForAxisStyle();
            DateTimeAxisHelper.GenerateVisibleLabels(this, Minimum, Maximum, ActualIntervalType);
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected override DoubleRange CalculateActualRange()
        {
            if (ActualRange.IsEmpty) // Executes when Minimum and Maximum aren set.
            {
                DoubleRange range = base.CalculateActualRange();
                if (IncludeAnnotationRange && Area != null)
                {
                    foreach (var annotation in (Area as SfChart).Annotations)
                    {
                        if (Orientation == Orientation.Horizontal && annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.XAxis == this)
                            range += new DoubleRange(
                                (Annotation.ConvertData(annotation.X1, this) == 0d 
                                ? range.Start : Annotation.ConvertData(annotation.X1, this)),
                                (annotation is TextAnnotation 
                                ? (Annotation.ConvertData(annotation.X1, this) == 0d 
                                ? range.Start : Annotation.ConvertData(annotation.X1, this)) 
                                : annotation is ImageAnnotation 
                                ? (Annotation.ConvertData((annotation as ImageAnnotation).X2, this) == 0d 
                                ? range.Start : Annotation.ConvertData((annotation as ImageAnnotation).X2, this)) 
                                : (Annotation.ConvertData((annotation as ShapeAnnotation).X2, this) == 0d 
                                ? range.Start : Annotation.ConvertData((annotation as ShapeAnnotation).X2, this))));
                    }
                }

                return range;
            }
            else if (Minimum != null && Maximum != null) // Executes when Minimum and Maximum aren't set.
                return ActualRange;
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange range = base.CalculateActualRange();
                if (Minimum != null)
                    return new DoubleRange(ActualRange.Start, double.IsNaN(range.End) ? ActualRange.Start + 1 : range.End);
                else if (Maximum != null)
                    return new DoubleRange(double.IsNaN(range.Start) ? ActualRange.End - 1 : range.Start, ActualRange.End);
                else
                {
                    if (IncludeAnnotationRange && Area != null)
                    {
                        foreach (var annotation in (Area as SfChart).Annotations)
                        {
                            if (Orientation == Orientation.Horizontal && annotation.CoordinateUnit == CoordinateUnit.Axis && annotation.XAxis == this)
                                range += new DoubleRange(
                                    (Annotation.ConvertData(annotation.X1, this) == 0d
                                    ? range.Start : Annotation.ConvertData(annotation.X1, this)),
                                    (annotation is TextAnnotation 
                                    ? (Annotation.ConvertData(annotation.X1, this) == 0d 
                                    ? range.Start : Annotation.ConvertData(annotation.X1, this)) 
                                    : annotation is ImageAnnotation
                                    ? (Annotation.ConvertData((annotation as ImageAnnotation).X2, this) == 0d 
                                    ? range.Start : Annotation.ConvertData((annotation as ImageAnnotation).X2, this)) 
                                    : (Annotation.ConvertData((annotation as ShapeAnnotation).X2, this) == 0d 
                                    ? range.Start : Annotation.ConvertData((annotation as ShapeAnnotation).X2, this))));
                        }
                    }

                    return range;
                }
            }
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            if (Minimum == null && Maximum == null) // Executes when Minimum and Maximum aren't set.
                return DateTimeAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, RangePadding, ActualIntervalType);
            else if (Minimum != null && Maximum != null) // Executes when  Minimum and Maximum are set.
                return range;
            else
            {
                // Executes when either Minimum or Maximum is set.
                DoubleRange baseRange = DateTimeAxisHelper.ApplyRangePadding(this, base.ApplyRangePadding(range, interval), interval, RangePadding, ActualIntervalType);
                return Minimum != null ? new DoubleRange(range.Start, baseRange.End) : new DoubleRange(baseRange.Start, range.End);
            }
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.Interval = this.Interval;
            dateTimeAxis.Minimum = this.Minimum;
            dateTimeAxis.Maximum = this.Maximum;
            dateTimeAxis.IntervalType = this.IntervalType;
            dateTimeAxis.EnableBusinessHours = this.EnableBusinessHours;
            dateTimeAxis.OpenTime = this.OpenTime;
            dateTimeAxis.CloseTime = this.CloseTime;
            dateTimeAxis.WorkingDays = this.WorkingDays;
            dateTimeAxis.RangePadding = this.RangePadding;
            obj = dateTimeAxis;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Static Methods
        
        private static void OnAutoScrollingDeltaTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimeAxis = d as DateTimeAxis;

            if(dateTimeAxis != null && dateTimeAxis.Area != null)
            {
                dateTimeAxis.CanAutoScroll = true;
                dateTimeAxis.OnPropertyChanged();
            }
        }

        /// <summary>
        /// calculate the count of day which is occur with in start and end date
        /// </summary>
        /// <param name="dayName"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        static int CalculateWeekEndDayCount(string dayName, DateTime start, DateTime end)
        {
            DayOfWeek day = dayName.Equals("Monday") ? DayOfWeek.Monday : dayName.Equals("Tuesday") ? DayOfWeek.Tuesday : dayName.Equals("Wednesday") ? DayOfWeek.Wednesday : dayName.Equals("Thursday") ? DayOfWeek.Thursday : dayName.Equals("Friday") ? DayOfWeek.Friday : dayName.Equals("Saturday") ? DayOfWeek.Saturday : DayOfWeek.Sunday;
            TimeSpan diff = end - start;
            int daycount = (int)Math.Floor(diff.TotalDays / 7);  // count the week
            int remainder = (int)(diff.TotalDays % 7); // remainig days   
            int diffday = (int)(end.DayOfWeek - day);
            if (diffday < 0) diffday += 7;
            if (remainder >= diffday) daycount++;
            return daycount;
        }

        private static void OnEnableBusinessHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as DateTimeAxis).OnEnableBusinessHoursChanged((bool)args.NewValue);
        }

        private static void OnWorkDaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((DateTimeAxis)d).InternalWorkingDays = args.NewValue.ToString();
        }

        private static void OnRangePaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (d as DateTimeAxis).OnPropertyChanged();
        }

        private static void OnIntervalTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimeAxis = d as DateTimeAxis;
            if (dateTimeAxis != null && e.NewValue != null)
            {
                dateTimeAxis.ActualIntervalType = dateTimeAxis.IntervalType;
                dateTimeAxis.OnPropertyChanged();
            }
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DateTimeAxis).OnMinimumChanged(e);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DateTimeAxis).OnMaximumChanged(e);
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DateTimeAxis).OnIntervalChanged(e);
        }

        #endregion

        #region Private Methods

        private DateTimeIntervalType GetActualAutoScrollingDeltaType()
        {
            if (AutoScrollingDeltaType == DateTimeIntervalType.Auto)
            {
                CalculateDateTimeIntervalType(ActualRange, AvailableSize, ref dateTimeIntervalType);
                return dateTimeIntervalType;               
            }
            else
            {
                return AutoScrollingDeltaType;
            }
        }

        private object CalculateDateTimeIntervalType(DoubleRange actualRange, Size availableSize, ref DateTimeIntervalType dateTimeIntervalType)
        {
            var dateTimeMin = ChartExtensionUtils.FromOADate(actualRange.Start);
            var dateTimeMax = ChartExtensionUtils.FromOADate(actualRange.End);
            var timeSpan = dateTimeMax.Subtract(dateTimeMin);
            double interval = 0;

            switch (dateTimeIntervalType)
            {
                case DateTimeIntervalType.Years:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 365), availableSize);
                    break;
                case DateTimeIntervalType.Months:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize);
                    break;
                case DateTimeIntervalType.Days:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize);
                    break;
                case DateTimeIntervalType.Hours:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize);
                    break;
                case DateTimeIntervalType.Minutes:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize);
                    break;
                case DateTimeIntervalType.Seconds:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize);
                    break;
                case DateTimeIntervalType.Milliseconds:
                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize);
                    break;
                case DateTimeIntervalType.Auto:
                    var range = new DoubleRange(0, timeSpan.TotalDays / 365);
                    interval = base.CalculateNiceInterval(range, availableSize);

                    if (interval >= 1)
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Years;

                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize);

                    if (interval >= 1)
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Months;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize);

                    if (interval >= 1)
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Days;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize);

                    if (interval >= 1)
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Hours;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize);

                    if (interval >= 1)
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Minutes;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize);

                    if (interval >= 1)
                    {
                        dateTimeIntervalType = DateTimeIntervalType.Seconds;
                        return interval;
                    }

                    interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize);

                    dateTimeIntervalType = DateTimeIntervalType.Milliseconds;

                    break;
            }

            return interval;
        }

        private void OnMinMaxChanged()
        {
            DateTimeAxisHelper.OnMinMaxChanged(this, Minimum, Maximum);
        }

        private void OnEnableBusinessHoursChanged(bool value)
        {
            if (value)
            {
                ValueToCoefficientCalc = ValueToBusinesshoursCoefficient;
                CoefficientToValueCalc = CoefficientToBusinesshoursValue;
            }
            else
            {
                ValueToCoefficientCalc = ValueToCoefficient;
                CoefficientToValueCalc = CoefficientToValue;
            }

            if (Area != null)
                Area.ScheduleUpdate();
        }

        /// <summary>
        /// Calculate the NonWorking days for the  range
        /// </summary>
        /// <param name="range"></param>
        private void CalculateNonWorkingDays(DoubleRange range)
        {
            var days = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            NonWorkingDays = new List<string>();
            if (double.IsNaN(range.Start)) return;
            var startDate = range.Start.FromOADate();
            var endDate = range.End.FromOADate();
            InternalWorkingDays = WorkingDays.ToString();
            foreach (var day in days)
            {
                if (!InternalWorkingDays.Contains(day))
                {
                    NonWorkingDays.Add(day);
                }
            }

            nonWorkingHoursPerDay = 24 - (CloseTime - OpenTime);
            TotalNonWorkingHours = CalcNonWorkingHours(startDate, endDate, InternalWorkingDays, nonWorkingHoursPerDay);
        }

        private double CoefficientToBusinesshoursValue(double value)
        {
            var diff = CloseTime - OpenTime;
            var start = VisibleRange.Start;
            var end = VisibleRange.End - (TotalNonWorkingHours / 24);
            var delta = end - start;
            var totalDays = ((delta * value) * 24) / diff;
            var remainderHours = (totalDays - (int)totalDays);
            var date = start.FromOADate().AddDays((int)totalDays).AddHours(remainderHours * diff);

            if (NonWorkingDays.Count > 0)
            {
                var weekEndCount = CalculateWeekEndDayCount(NonWorkingDays[0], start.FromOADate(), date); // find the weekend occurrence from start and end date
                date = date.AddDays(weekEndCount * NonWorkingDays.Count);
                if (NonWorkingDays.Contains(date.DayOfWeek.ToString()))
                {
                    date = date.AddDays(NonWorkingDays.Count);
                }
            }

            return date.ToOADate();
        }

        private double ValueToBusinesshoursCoefficient(double value)
        {
            double result = double.NaN;
            var start = VisibleRange.Start;
            var end = VisibleRange.End - (TotalNonWorkingHours / 24);
            var delta = end - start;
            if (!double.IsNaN(value))
            {
                value -=
                    CalcNonWorkingHours(
                        start.FromOADate(), 
                        value.FromOADate(), 
                        InternalWorkingDays,
                        nonWorkingHoursPerDay) / 24; // Calculate the nonworking hours between start date to value date and reduce that hours from value

                result = delta == 0 ? 0 : (value - start) / delta;
            }

            return isInversed ? 1d - result : result;
        }

        #endregion

        #endregion
    }
}
