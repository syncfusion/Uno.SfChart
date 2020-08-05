using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a DateTime indexed axis. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.ChartAxisBase2D" />
    public partial class DateTimeCategoryAxis : ChartAxisBase2D
    {
        #region Dependency Property Registrations

        /// <summary>
        ///  The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(double), 
                typeof(DateTimeCategoryAxis),
            new PropertyMetadata(0d, OnIntervalChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="IntervalType"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalTypeProperty =
            DependencyProperty.Register(
                "IntervalType", 
                typeof(DateTimeIntervalType),
                typeof(DateTimeCategoryAxis),
                new PropertyMetadata(
                    DateTimeIntervalType.Auto,
                    new PropertyChangedCallback(OnIntervalTypeChanged)));

        #endregion

        #region Properties

        #region Public Properties

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

        internal DateTimeIntervalType ActualIntervalType
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Method implementation for Get LabelContent for given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            ChartSeriesBase actualSeries =
                Area.VisibleSeries
                .Where(series => series.ActualXAxis == this)
                .Max(filteredSeries => filteredSeries.DataCount);

            if (actualSeries != null)
            {
                if (CustomLabels.Count > 0 || LabelsSource != null)
                {
                    return GetCustomLabelContent(position) ?? GetLabelContent((int)Math.Round(position), actualSeries);
                }
                else
                    return GetLabelContent((int)Math.Round(position), actualSeries);
            }

            return position;
        }

        #endregion

        #region Protected Internal Override Methods
        
        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            if (Interval == 0 || double.IsNaN(Interval))
                return CalculateNiceInterval(range, availableSize);
            return Interval;
        }

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
                    interval = actualRange.Delta / GetActualDesiredIntervalsCount(availableSize);
                    if (interval <= base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 365), availableSize))
                    {
                        interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 365), availableSize);
                        ActualIntervalType = DateTimeIntervalType.Years;
                    }
                    else if (interval <= base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize))
                    {
                        interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays / 30), availableSize);
                        ActualIntervalType = DateTimeIntervalType.Months;
                    }
                    else if (interval <= base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize))
                    {
                        interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalDays), availableSize);
                        ActualIntervalType = DateTimeIntervalType.Days;
                    }
                    else if (interval <= base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize))
                    {
                        interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalHours), availableSize);
                        ActualIntervalType = DateTimeIntervalType.Hours;
                    }
                    else if (interval <= base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize))
                    {
                        interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMinutes), availableSize);
                        ActualIntervalType = DateTimeIntervalType.Minutes;
                    }
                    else if (interval <= base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize))
                    {
                        interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalSeconds), availableSize);
                        ActualIntervalType = DateTimeIntervalType.Seconds;
                    }
                    else if (interval <= base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize))
                    {
                        interval = base.CalculateNiceInterval(new DoubleRange(0, timeSpan.TotalMilliseconds), availableSize);
                        ActualIntervalType = DateTimeIntervalType.Milliseconds;
                    }

                    break;
            }

            return Math.Max(1d, interval);
        }

        #endregion

        #region Protected Virtual Methods
        
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

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var dateTimeCategoryAxis = new DateTimeCategoryAxis();
            dateTimeCategoryAxis.Interval = this.Interval;
            dateTimeCategoryAxis.IntervalType = this.IntervalType;
            obj = dateTimeCategoryAxis;
            return base.CloneAxis(obj);
        }

        /// <summary>
        /// Method implementation for Create VisibleLabels for DateTime axis
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        protected override void GenerateVisibleLabels()
        {
            ChartSeriesBase actualSeries =
                Area.VisibleSeries
                .Where(series => series.ActualXAxis == this)
                .Max(filteredSeries => filteredSeries.DataCount);

            actualSeries = actualSeries != null ? actualSeries : Area is SfChart ? (Area as SfChart).TechnicalIndicators.Where(series => series.ActualXAxis == this)
                        .Max(filteredSeries => filteredSeries.DataCount) : null;

            SetRangeForAxisStyle();

            if (actualSeries != null)
            {
                var XValuesCount = actualSeries.ActualXValues as List<double>;
                if (XValuesCount != null && XValuesCount.Count > 0)
                {
                    double interval = Interval != 0 ? Interval : ActualInterval;
                    double position = VisibleRange.Start - (VisibleRange.Start % ActualInterval);
                    var xValues = actualSeries.ActualXValues as List<double>;
                    var xStartValue = xValues[0].FromOADate();
                    double distinctDate = 0d;
                    int year = xStartValue.Year,
                        month = xStartValue.Month,
                        hour = xStartValue.Hour,
                        mins = xStartValue.Minute,
                        secs = xStartValue.Second,
                        millisecs = xStartValue.Millisecond;
                    DateTime date = xStartValue.Date;

                    switch (ActualIntervalType)
                    {
                        case DateTimeIntervalType.Months:
                            for (; position <= VisibleRange.End; position++)
                            {
                                int pos = ((int)Math.Round(position));
                                if (VisibleRange.Inside(position) && pos > -1 && pos < xValues.Count)
                                {
                                    var xValue = xValues[pos].FromOADate();
                                    if (xValue.Year > year)
                                    {
                                        year = xValue.Year;
                                        month = xValue.Month;
                                    }

                                    if (xValue.Year == year)
                                    {
                                        if (xValue.Month > month)
                                        {
                                            month = xValue.Month;
                                        }

                                        if (xValue.Month == month)
                                        {
                                            object obj = GetLabelContent(pos, actualSeries);
                                            var dateTimeAxisLabel = new DateTimeAxisLabel(pos, obj, xValues[pos]);
                                            dateTimeAxisLabel.IntervalType = ActualIntervalType;
                                            dateTimeAxisLabel.IsTransition = DateTimeAxisHelper.GetTransitionState(ref distinctDate, xValue, ActualIntervalType);
                                            VisibleLabels.Add(dateTimeAxisLabel);
                                            month = xValue.AddMonths((int)interval).Month;
                                            year = xValue.AddMonths((int)interval).Year;
                                        }
                                    }
                                }
                            }

                            break;

                        case DateTimeIntervalType.Years:
                            for (; position <= VisibleRange.End; position++)
                            {
                                int pos = ((int)Math.Round(position));
                                if (VisibleRange.Inside(position) && pos > -1 && pos < xValues.Count)
                                {
                                    var xValue = xValues[pos].FromOADate();
                                    if (xValue.Year > year)
                                        year = xValue.Year;
                                    if (xValue.Year == year)
                                    {
                                        object obj = GetLabelContent(pos, actualSeries);
                                        var dateTimeAxisLabel = new DateTimeAxisLabel(pos, obj, xValues[pos]);
                                        dateTimeAxisLabel.IntervalType = ActualIntervalType;
                                        dateTimeAxisLabel.IsTransition = DateTimeAxisHelper.GetTransitionState(ref distinctDate, xValue, ActualIntervalType);
                                        VisibleLabels.Add(dateTimeAxisLabel);
                                        year = xValue.AddYears((int)interval).Year;
                                    }
                                }
                            }

                            break;

                        case DateTimeIntervalType.Days:
                            for (; position <= VisibleRange.End; position++)
                            {
                                int pos = ((int)Math.Round(position));
                                if (VisibleRange.Inside(position) && pos > -1 && pos < xValues.Count)
                                {
                                    var xValue = xValues[pos].FromOADate();
                                    if (xValue.Date > date)
                                    {
                                        date = xValue.Date;
                                    }

                                    if (xValue.Date == date)
                                    {
                                        object obj = GetLabelContent(pos, actualSeries);
                                        var dateTimeAxisLabel = new DateTimeAxisLabel(pos, obj, xValues[pos]);
                                        dateTimeAxisLabel.IntervalType = ActualIntervalType;
                                        dateTimeAxisLabel.IsTransition = DateTimeAxisHelper.GetTransitionState(ref distinctDate, xValue, ActualIntervalType);
                                        VisibleLabels.Add(dateTimeAxisLabel);
                                        date = xValue.AddDays((int)interval).Date;
                                    }
                                }
                            }

                            break;

                        case DateTimeIntervalType.Hours:
                            for (; position <= VisibleRange.End; position++)
                            {
                                int pos = ((int)Math.Round(position));
                                if (VisibleRange.Inside(position) && pos > -1 && pos < xValues.Count)
                                {
                                    var xValue = xValues[pos].FromOADate();
                                    if (xValue.Date > date)
                                    {
                                        date = xValue.Date;
                                        hour = xValue.Hour;
                                    }

                                    if (xValue.Date == date)
                                    {
                                        if (xValue.Hour > hour)
                                        {
                                            hour = xValue.Hour;
                                        }

                                        if (xValue.Hour == hour)
                                        {
                                            object obj = GetLabelContent(pos, actualSeries);
                                            var dateTimeAxisLabel = new DateTimeAxisLabel(pos, obj, xValues[pos]);
                                            dateTimeAxisLabel.IntervalType = ActualIntervalType;
                                            dateTimeAxisLabel.IsTransition = DateTimeAxisHelper.GetTransitionState(ref distinctDate, xValue, ActualIntervalType);
                                            VisibleLabels.Add(dateTimeAxisLabel);
                                            hour = xValue.AddHours((int)interval).Hour;
                                            date = xValue.AddHours((int)interval).Date;
                                        }
                                    }
                                }
                            }

                            break;

                        case DateTimeIntervalType.Minutes:
                            for (; position <= VisibleRange.End; position++)
                            {
                                int pos = ((int)Math.Round(position));

                                if (VisibleRange.Inside(position) && pos > -1 && pos < xValues.Count)
                                {
                                    var xValue = xValues[pos].FromOADate();
                                    if (xValue.Date > date)
                                    {
                                        date = xValue.Date;
                                        hour = xValue.Hour;
                                    }

                                    if (xValue.Date == date)
                                    {
                                        if (xValue.Hour > hour)
                                        {
                                            hour = xValue.Hour;
                                            mins = xValue.Minute;
                                        }

                                        if (xValue.Hour == hour)
                                        {
                                            if (xValue.Minute > mins)
                                            {
                                                mins = xValue.Minute;
                                            }

                                            if (xValue.Minute == mins)
                                            {
                                                object obj = GetLabelContent(pos, actualSeries);
                                                var dateTimeAxisLabel = new DateTimeAxisLabel(pos, obj, xValues[pos]);
                                                dateTimeAxisLabel.IntervalType = ActualIntervalType;
                                                dateTimeAxisLabel.IsTransition = DateTimeAxisHelper.GetTransitionState(ref distinctDate, xValue, ActualIntervalType);
                                                VisibleLabels.Add(dateTimeAxisLabel);
                                                hour = xValue.AddMinutes((int)interval).Hour;
                                                mins = xValue.AddMinutes((int)interval).Minute;
                                                date = xValue.AddMinutes((int)interval).Date;
                                            }
                                        }
                                    }
                                }
                            }

                            break;

                        case DateTimeIntervalType.Seconds:
                            for (; position <= VisibleRange.End; position++)
                            {
                                int pos = ((int)Math.Round(position));
                                if (VisibleRange.Inside(position) && pos > -1 && pos < xValues.Count)
                                {
                                    var xValue = xValues[pos].FromOADate();
                                    if (xValue.Date > date)
                                    {
                                        date = xValue.Date;
                                        hour = xValue.Hour;
                                    }

                                    if (xValue.Date == date)
                                    {
                                        if (xValue.Hour > hour)
                                        {
                                            hour = xValue.Hour;
                                            mins = xValue.Minute;
                                        }

                                        if (xValue.Hour == hour)
                                        {
                                            if (xValue.Minute > mins)
                                            {
                                                mins = xValue.Minute;
                                                secs = xValue.Second;
                                            }

                                            if (xValue.Minute == mins)
                                            {
                                                if (xValue.Second > secs)
                                                {
                                                    secs = xValue.Second;
                                                }

                                                if (xValue.Second == secs)
                                                {
                                                    object obj = GetLabelContent(pos, actualSeries);
                                                    var dateTimeAxisLabel = new DateTimeAxisLabel(pos, obj, xValues[pos]);
                                                    dateTimeAxisLabel.IntervalType = ActualIntervalType;
                                                    dateTimeAxisLabel.IsTransition = DateTimeAxisHelper.GetTransitionState(ref distinctDate, xValue, ActualIntervalType);
                                                    VisibleLabels.Add(dateTimeAxisLabel);
                                                    hour = xValue.AddSeconds((int)interval).Hour;
                                                    mins = xValue.AddSeconds((int)interval).Minute;
                                                    date = xValue.AddSeconds((int)interval).Date;
                                                    secs = xValue.AddSeconds((int)interval).Second;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;

                        case DateTimeIntervalType.Milliseconds:
                            for (; position <= VisibleRange.End; position++)
                            {
                                int pos = ((int)Math.Round(position));
                                if (VisibleRange.Inside(position) && pos > -1 && pos < xValues.Count)
                                {
                                    var xValue = xValues[pos].FromOADate();
                                    if (xValue.Date > date)
                                    {
                                        date = xValue.Date;
                                        hour = xValue.Hour;
                                    }

                                    if (xValue.Date == date)
                                    {
                                        if (xValue.Hour > hour)
                                        {
                                            hour = xValue.Hour;
                                            mins = xValue.Minute;
                                        }

                                        if (xValue.Hour == hour)
                                        {
                                            if (xValue.Minute > mins)
                                            {
                                                mins = xValue.Minute;
                                                secs = xValue.Second;
                                            }

                                            if (xValue.Minute == mins)
                                            {
                                                if (xValue.Second > secs)
                                                {
                                                    secs = xValue.Second;
                                                    millisecs = xValue.Millisecond;
                                                }

                                                if (xValue.Second == secs)
                                                {
                                                    if (xValue.Millisecond > millisecs)
                                                    {
                                                        millisecs = xValue.Millisecond;
                                                    }

                                                    if (xValue.Millisecond == millisecs)
                                                    {
                                                        object obj = GetLabelContent(pos, actualSeries);
                                                        var dateTimeAxisLabel = new DateTimeAxisLabel(pos, obj, xValues[pos]);
                                                        dateTimeAxisLabel.IntervalType = ActualIntervalType;
                                                        dateTimeAxisLabel.IsTransition = DateTimeAxisHelper.GetTransitionState(ref distinctDate, xValue, ActualIntervalType);
                                                        VisibleLabels.Add(dateTimeAxisLabel);
                                                        hour = xValue.AddMilliseconds((int)interval).Hour;
                                                        mins = xValue.AddMilliseconds((int)interval).Minute;
                                                        date = xValue.AddMilliseconds((int)interval).Date;
                                                        secs = xValue.AddMilliseconds((int)interval).Second;
                                                        millisecs = xValue.AddMilliseconds((int)interval).Millisecond;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;

                        case DateTimeIntervalType.Auto:
                            for (; position <= VisibleRange.End; position += interval)
                            {
                                if (VisibleRange.Inside(position) && position < actualSeries.DataCount && position > -1)
                                {
                                    int pos = ((int)Math.Round(position));
                                    var xValue = xValues[pos].FromOADate();
                                    object obj = GetLabelContent(pos, actualSeries);
                                    var dateTimeAxisLabel = new DateTimeAxisLabel(pos, obj, xValues[pos]);
                                    dateTimeAxisLabel.IntervalType = ActualIntervalType;
                                    dateTimeAxisLabel.IsTransition = DateTimeAxisHelper.GetTransitionState(ref distinctDate, xValue, ActualIntervalType);
                                    VisibleLabels.Add(dateTimeAxisLabel);
                                }
                            }

                            break;
                    }
                }
            }
        }

        #endregion

        #region Private Static Methods

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DateTimeCategoryAxis).OnIntervalChanged(e);
        }

        private static void OnIntervalTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimeCategoryAxis = d as DateTimeCategoryAxis;
            if (dateTimeCategoryAxis != null && e.NewValue != null && e.NewValue != e.OldValue)
            {
                dateTimeCategoryAxis.ActualIntervalType = dateTimeCategoryAxis.IntervalType;
                dateTimeCategoryAxis.OnIntervalChanged(e);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Return object value from the given double value
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private object GetLabelContent(int pos, ChartSeriesBase actualSeries)
        {
            if (actualSeries != null)
            {
                var xValues = actualSeries.ActualXValues as List<double>;

                if (xValues != null && pos < xValues.Count && pos >= 0)
                {
                    DateTime xDateTime = xValues[pos].FromOADate();
                    return xDateTime.ToString(this.LabelFormat, CultureInfo.CurrentCulture);
                }
                else
                {
                    var xStrValues = actualSeries.ActualXValues as List<string>;
                    if (xStrValues != null && pos < xStrValues.Count && pos >= 0)
                        return xStrValues[pos];
                }
            }

            return pos;
        }

        #endregion

        #endregion        
    }
}
