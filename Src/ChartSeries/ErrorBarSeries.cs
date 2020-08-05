using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a control that represents a error bar type series. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.XyDataSeries" />
    public partial class ErrorBarSeries : XyDataSeries
    {
        #region Dependency Property Registration
        
        /// <summary>
        ///   The DependencyProperty for <see cref="HorizontalErrorPath"/> property. 
        /// </summary>
        public static readonly DependencyProperty HorizontalErrorPathProperty =
            DependencyProperty.Register("HorizontalErrorPath", typeof(string), typeof(ErrorBarSeries),
            new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref=" VerticalErrorPath"/> property. 
        /// </summary>
        public static readonly DependencyProperty VerticalErrorPathProperty =
            DependencyProperty.Register("VerticalErrorPath", typeof(string), typeof(ErrorBarSeries),
            new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref=" HorizontalLineStyle"/> property. 
        /// </summary>
        public static readonly DependencyProperty HorizontalLineStyleProperty =
            DependencyProperty.Register("HorizontalLineStyle", typeof(LineStyle), typeof(ErrorBarSeries),
            new PropertyMetadata(null, OnHorizontalPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalLineStyle"/> property. 
        /// </summary>
        public static readonly DependencyProperty VerticalLineStyleProperty =
            DependencyProperty.Register("VerticalLineStyle", typeof(LineStyle), typeof(ErrorBarSeries),
            new PropertyMetadata(null, OnVerticalPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalCapLineStyle"/> property. 
        /// </summary>
        public static readonly DependencyProperty HorizontalCapLineStyleProperty =
            DependencyProperty.Register("HorizontalCapLineStyle", typeof(CapLineStyle), typeof(ErrorBarSeries),
            new PropertyMetadata(null, OnHorizontalCapPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalCapLineStyle"/> property. 
        /// </summary>
        public static readonly DependencyProperty VerticalCapLineStyleProperty =
            DependencyProperty.Register("VerticalCapLineStyle", typeof(CapLineStyle), typeof(ErrorBarSeries),
            new PropertyMetadata(null, OnVerticalCapPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalErrorValue"/> property. 
        /// </summary>
        public static readonly DependencyProperty HorizontalErrorValueProperty =
            DependencyProperty.Register("HorizontalErrorValue", typeof(double), typeof(ErrorBarSeries),
            new PropertyMetadata(0d, OnHorizontalErrorValuePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalErrorValue"/> property. 
        /// </summary>
        public static readonly DependencyProperty VerticalErrorValueProperty =
            DependencyProperty.Register("VerticalErrorValue", typeof(double), typeof(ErrorBarSeries),
            new PropertyMetadata(0d, OnVerticalErrorValuePropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="Mode"/> property. 
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(ErrorBarMode), typeof(ErrorBarSeries),
            new PropertyMetadata(ErrorBarMode.Both, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Type"/> property. 
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(ErrorBarType), typeof(ErrorBarSeries),
            new PropertyMetadata(ErrorBarType.Fixed, OnPropertyChanged));

        public static readonly DependencyProperty HorizontalDirectionProperty =
            DependencyProperty.Register("HorizontalDirection", typeof(ErrorBarDirection), typeof(ErrorBarSeries),
                new PropertyMetadata(ErrorBarDirection.Both, OnPropertyChanged));

        public static readonly DependencyProperty VerticalDirectionProperty =
            DependencyProperty.Register("VerticalDirection", typeof(ErrorBarDirection), typeof(ErrorBarSeries),
                new PropertyMetadata(ErrorBarDirection.Both, OnPropertyChanged));

        #endregion

        #region Fields

        #region Pivate Fields

        double _horizontalErrorValue;

        #endregion

        #endregion

        #region Contructor

        /// <summary>
        /// Called when instance created for ErrorBarSeries
        /// </summary>
        public ErrorBarSeries()
        {
            DefaultStyleKey = typeof(ErrorBarSeries);
            HorizontalCustomValues = new List<double>();
            VerticalCustomValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the property path to be bind with horizontal error value.
        /// </summary>
        public string HorizontalErrorPath
        {
            get { return (string)GetValue(HorizontalErrorPathProperty); }
            set { SetValue(HorizontalErrorPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property path to be bind with vertical error value.
        /// </summary>
        public string VerticalErrorPath
        {
            get { return (string)GetValue(VerticalErrorPathProperty); }
            set { SetValue(VerticalErrorPathProperty, value); }
        }


        /// <summary>
        /// Gets or sets the style for the horizontal line in error bar.
        /// </summary>
        /// <value>
        /// <see cref="System.UI.Xaml.Charts.LineStyle"/>.
        /// </value>
        public LineStyle HorizontalLineStyle
        {
            get { return (LineStyle)GetValue(HorizontalLineStyleProperty); }
            set { SetValue(HorizontalLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the vertical line in error bar.
        /// </summary>
        /// <value>
        /// <see cref="System.UI.Xaml.Charts.LineStyle"/>.
        /// </value>
        public LineStyle VerticalLineStyle
        {
            get { return (LineStyle)GetValue(VerticalLineStyleProperty); }
            set { SetValue(VerticalLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end cap style for the horizontal error line.
        /// </summary>
        /// <value>
        ///     <see cref="Syncfusion.UI.Xaml.Charts.CapLineStyle"/>.
        /// </value>
        public CapLineStyle HorizontalCapLineStyle
        {
            get { return (CapLineStyle)GetValue(HorizontalCapLineStyleProperty); }
            set { SetValue(HorizontalCapLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end cap style for the vertical error line.
        /// </summary>
        /// <value>
        ///     <see cref="Syncfusion.UI.Xaml.Charts.CapLineStyle"/>.
        /// </value>
        public CapLineStyle VerticalCapLineStyle
        {
            get { return (CapLineStyle)GetValue(VerticalCapLineStyleProperty); }
            set { SetValue(VerticalCapLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the error or varying value along x value.
        /// </summary>
        public double HorizontalErrorValue
        {
            get { return (double)GetValue(HorizontalErrorValueProperty); }
            set { SetValue(HorizontalErrorValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the error or varying value along y value.
        /// </summary>
        public double VerticalErrorValue
        {
            get { return (double)GetValue(VerticalErrorValueProperty); }
            set { SetValue(VerticalErrorValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to draw error bar in horizontal or vertical or both directions.
        /// </summary>
        /// <value>
        ///     <see cref="Syncfusion.UI.Xaml.Charts.ErrorBarMode"/>.
        /// </value>
        public ErrorBarMode Mode
        {
            get { return (ErrorBarMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the standard types of error bar to be drawn.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.ErrorBarType"/>
        /// </value>
        public ErrorBarType Type
        {
            get { return (ErrorBarType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public ErrorBarDirection HorizontalDirection
        {
            get { return (ErrorBarDirection)GetValue(HorizontalDirectionProperty); }
            set { SetValue(HorizontalDirectionProperty, value); }
        }

        public ErrorBarDirection VerticalDirection
        {
            get { return (ErrorBarDirection)GetValue(VerticalDirectionProperty); }
            set { SetValue(VerticalDirectionProperty, value); }
        }

        #endregion

        #region Internal Override Properties

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Internal Properties

        internal string HorizontalErrorMemberPath { get; set; }

        internal string VerticalErrorMemberPath { get; set; }

        #endregion

        #region Protected Internal Properties

        protected internal IList<double> HorizontalCustomValues
        {
            get;
            set;
        }

        protected internal IList<double> VerticalCustomValues
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of ErrorBarSeries
        /// </summary>
        public override void CreateSegments()
        {
            var xValues = GetXValues();

            if (xValues != null)
            {
                var horSdvalue = GetSdErrorValue(xValues);
                var verSdvalue = GetSdErrorValue(YValues);

                double verticalErrorValue;

                if (Type == ErrorBarType.StandardErrors || Type == ErrorBarType.StandardDeviation)
                {
                    _horizontalErrorValue = horSdvalue[1];
                    verticalErrorValue = verSdvalue[1];
                }
                else
                {
                    _horizontalErrorValue = HorizontalErrorValue;
                    verticalErrorValue = VerticalErrorValue;
                }

                if (Segments.Count > this.DataCount)
                {
                    ClearUnUsedSegments(this.DataCount);
                }

                var dateTimeAxis = ActualXAxis as DateTimeAxis;

                if (dateTimeAxis != null && dateTimeAxis.IntervalType == DateTimeIntervalType.Auto)
                    dateTimeAxis.ActualRangeChanged += ErrorBarSeries_ActualRangeChanged;
                else if (dateTimeAxis != null && dateTimeAxis.IntervalType != DateTimeIntervalType.Auto)
                    dateTimeAxis.ActualRangeChanged -= ErrorBarSeries_ActualRangeChanged;

                for (var i = 0; i < this.DataCount; i++)
                {
                    switch (Type)
                    {
                        case ErrorBarType.Percentage:
                            if ((ActualXAxis is DateTimeAxis) || (ActualXAxis is CategoryAxis) || (ActualXAxis is DateTimeCategoryAxis))
                            {
                                _horizontalErrorValue = GetPercentageErrorBarValue(i + 1, HorizontalErrorValue);
                                verticalErrorValue = GetPercentageErrorBarValue(YValues[i], VerticalErrorValue);
                            }
                            else
                            {
                                _horizontalErrorValue = GetPercentageErrorBarValue(xValues[i], HorizontalErrorValue);
                                verticalErrorValue = GetPercentageErrorBarValue(YValues[i], VerticalErrorValue);
                            }

                            break;

                        case ErrorBarType.Custom:
                            _horizontalErrorValue = HorizontalCustomValues.Count > 0 ? HorizontalCustomValues[i] : 0;
                            verticalErrorValue = VerticalCustomValues.Count > 0 ? VerticalCustomValues[i] : 0;
                            break;
                    }

                    ChartPoint leftPt;
                    ChartPoint rightPt;
                    ChartPoint topPt;
                    ChartPoint bottomPt;
                    if (Type == ErrorBarType.StandardDeviation)
                    {
                        if (HorizontalDirection == ErrorBarDirection.Plus)
                        {
                            leftPt = new ChartPoint(horSdvalue[0], YValues[i]);
                            rightPt = new ChartPoint(GetPlusValue(horSdvalue[0], _horizontalErrorValue, true), YValues[i]);
                        }
                        else if (HorizontalDirection == ErrorBarDirection.Minus)
                        {
                            leftPt = new ChartPoint(GetMinusValue(horSdvalue[0], _horizontalErrorValue, true), YValues[i]);
                            rightPt = new ChartPoint(horSdvalue[0], YValues[i]);
                        }
                        else
                        {
                            leftPt = new ChartPoint(GetMinusValue(horSdvalue[0], _horizontalErrorValue, true), YValues[i]);
                            rightPt = new ChartPoint(GetPlusValue(horSdvalue[0], _horizontalErrorValue, true), YValues[i]);
                        }

                        if (VerticalDirection == ErrorBarDirection.Plus)
                        {
                            topPt = new ChartPoint(xValues[i], verSdvalue[0]);
                            bottomPt = new ChartPoint(xValues[i], GetPlusValue(verSdvalue[0], verticalErrorValue, false));
                        }
                        else if (VerticalDirection == ErrorBarDirection.Minus)
                        {
                            topPt = new ChartPoint(xValues[i], GetMinusValue(verSdvalue[0], verticalErrorValue, false));
                            bottomPt = new ChartPoint(xValues[i], verSdvalue[0]);
                        }
                        else
                        {
                            topPt = new ChartPoint(xValues[i], GetMinusValue(verSdvalue[0], verticalErrorValue, false));
                            bottomPt = new ChartPoint(xValues[i], GetPlusValue(verSdvalue[0], verticalErrorValue, false));
                        }
                    }
                    else
                    {
                        if (HorizontalDirection == ErrorBarDirection.Plus)
                        {
                            leftPt = new ChartPoint(xValues[i], YValues[i]);
                            rightPt = new ChartPoint(GetPlusValue(xValues[i], _horizontalErrorValue, true), YValues[i]);
                        }
                        else if (HorizontalDirection == ErrorBarDirection.Minus)
                        {
                            leftPt = new ChartPoint(GetMinusValue(xValues[i], _horizontalErrorValue, true), YValues[i]);
                            rightPt = new ChartPoint(xValues[i], YValues[i]);
                        }
                        else
                        {
                            leftPt = new ChartPoint(GetMinusValue(xValues[i], _horizontalErrorValue, true), YValues[i]);
                            rightPt = new ChartPoint(GetPlusValue(xValues[i], _horizontalErrorValue, true), YValues[i]);
                        }

                        if (VerticalDirection == ErrorBarDirection.Plus)
                        {
                            topPt = new ChartPoint(xValues[i], YValues[i]);
                            bottomPt = new ChartPoint(xValues[i], GetPlusValue(YValues[i], verticalErrorValue, false));
                        }
                        else if (VerticalDirection == ErrorBarDirection.Minus)
                        {
                            topPt = new ChartPoint(xValues[i], GetMinusValue(YValues[i], verticalErrorValue, false));
                            bottomPt = new ChartPoint(xValues[i], YValues[i]);
                        }
                        else
                        {
                            topPt = new ChartPoint(xValues[i], GetMinusValue(YValues[i], verticalErrorValue, false));
                            bottomPt = new ChartPoint(xValues[i], GetPlusValue(YValues[i], verticalErrorValue, false));
                        }
                    }

                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(leftPt, rightPt, topPt, bottomPt);
                    }
                    else
                    {
                        var errorBar = new ErrorBarSegment(leftPt, rightPt, topPt, bottomPt, this,
                            ActualData[i]);
                        errorBar.SetData(leftPt, rightPt, topPt, bottomPt);
                        Segments.Add(errorBar);
                    }
                }
            }
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            HorizontalCustomValues.Clear();
            VerticalCustomValues.Clear();
            YValues.Clear();
            if (YBindingPath != null && HorizontalErrorPath != null && VerticalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, HorizontalErrorPath, VerticalErrorPath }, YValues, HorizontalCustomValues, VerticalCustomValues);
            else if (YBindingPath != null && HorizontalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, HorizontalErrorPath }, YValues, HorizontalCustomValues);
            else if (YBindingPath != null && VerticalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, VerticalErrorPath }, YValues, VerticalCustomValues);
            else if (YBindingPath != null)
            {
                GeneratePoints(new[] { YBindingPath }, YValues);
            }
        }

        #endregion

        #region Protected Override Methods

        protected override void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            base.OnXAxisChanged(oldAxis, newAxis);
            if (newAxis is DateTimeAxis)
                newAxis.ActualRangeChanged += ErrorBarSeries_ActualRangeChanged;
            if (oldAxis is DateTimeAxis)
                oldAxis.ActualRangeChanged -= ErrorBarSeries_ActualRangeChanged;
        }

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            HorizontalCustomValues.Clear();
            VerticalCustomValues.Clear();
            YValues.Clear();
            if (YBindingPath != null && HorizontalErrorPath != null && VerticalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, HorizontalErrorPath, VerticalErrorPath }, YValues, HorizontalCustomValues, VerticalCustomValues);
            else if (YBindingPath != null && HorizontalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, HorizontalErrorPath }, YValues, HorizontalCustomValues);
            else if (YBindingPath != null && VerticalErrorPath != null)
                GeneratePoints(new[] { YBindingPath, VerticalErrorPath }, YValues, VerticalCustomValues);
            else if (YBindingPath != null)
            {
                GeneratePoints(new[] { YBindingPath }, YValues);
            }

            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            HorizontalCustomValues.Clear();
            VerticalCustomValues.Clear();
            YValues.Clear();
            base.OnBindingPathChanged(args);
        }

        /// <summary>
        /// Clone method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var errorBarSeries = obj as ErrorBarSeries;
            if (errorBarSeries != null)
            {
                errorBarSeries.HorizontalErrorPath = HorizontalErrorPath;
                errorBarSeries.VerticalErrorPath = VerticalErrorPath;
                errorBarSeries.Type = Type;
                errorBarSeries.Mode = Mode;
                errorBarSeries.HorizontalErrorValue = HorizontalErrorValue;
                errorBarSeries.VerticalErrorValue = VerticalErrorValue;
                errorBarSeries.HorizontalLineStyle = HorizontalLineStyle;
                errorBarSeries.VerticalLineStyle = VerticalLineStyle;
                errorBarSeries.HorizontalCapLineStyle = HorizontalCapLineStyle;
                errorBarSeries.VerticalCapLineStyle = VerticalCapLineStyle;
            }

            return base.CloneSeries(obj);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Get Percentage ErrorBar Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        private static double GetPercentageErrorBarValue(double value, double errorValue)
        {
            return (value * (errorValue / 100));
        }

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var errorBarSeries = d as ErrorBarSeries;
            if (errorBarSeries != null && e.NewValue != null) errorBarSeries.OnBindingPathChanged(e);
        }


        private static void OnHorizontalErrorValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.Area != null && (instance.Type == ErrorBarType.Fixed || instance.Type == ErrorBarType.Percentage) && instance.Mode != ErrorBarMode.Vertical)
                instance.Area.ScheduleUpdate();
        }

        private static void OnVerticalErrorValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.Area != null && (instance.Type == ErrorBarType.Fixed || instance.Type == ErrorBarType.Percentage) && instance.Mode != ErrorBarMode.Horizontal)
                instance.Area.ScheduleUpdate();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.Area != null)
                instance.Area.ScheduleUpdate();
        }

        private static void OnHorizontalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.HorizontalLineStyle != null)
            {
                instance.HorizontalLineStyle.Series = instance;
                foreach (ErrorBarSegment item in instance.Segments)
                    item.UpdateVisualBinding();
                if (instance.Area != null)
                    instance.Area.ScheduleUpdate();
            }
        }

        private static void OnHorizontalCapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.HorizontalCapLineStyle != null)
            {
                instance.HorizontalCapLineStyle.Series = instance;
                foreach (ErrorBarSegment item in instance.Segments)
                    item.UpdateVisualBinding();
                if (instance.Area != null)
                    instance.Area.ScheduleUpdate();
            }
        }

        private static void OnVerticalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.VerticalLineStyle != null)
            {
                instance.VerticalLineStyle.Series = instance;
                foreach (ErrorBarSegment item in instance.Segments)
                    item.UpdateVisualBinding();
                if (instance.Area != null)
                    instance.Area.ScheduleUpdate();
            }
        }

        private static void OnVerticalCapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ErrorBarSeries;
            if (instance != null && instance.VerticalCapLineStyle != null)
            {
                instance.VerticalCapLineStyle.Series = instance;
                foreach (ErrorBarSegment item in instance.Segments)
                    item.UpdateVisualBinding();
                if (instance.Area != null)
                    instance.Area.ScheduleUpdate();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Actual Range Event hooked here for the suppose of DateTimeAxis with Auto type errorbar calculation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ErrorBarSeries_ActualRangeChanged(object sender, ActualRangeChangedEventArgs e)
        {
            var dateTimeAxis = sender as DateTimeAxis;
            if (dateTimeAxis != null && dateTimeAxis.IntervalType == DateTimeIntervalType.Auto)
            {
                double minrange = ((DateTime)e.ActualMinimum).ToOADate();
                double maxrange = ((DateTime)e.ActualMaximum).ToOADate();
                for (int i = 0; i < Segments.Count; i++)
                {
                    var chartSegment = Segments[i] as ErrorBarSegment;
                    Point point;
                    if (Type == ErrorBarType.Custom)
                        point = chartSegment.DateTimeIntervalCalculation(HorizontalCustomValues.Count > 0 ? HorizontalCustomValues[i] : 0, dateTimeAxis.ActualIntervalType);
                    else
                        point = chartSegment.DateTimeIntervalCalculation(_horizontalErrorValue, dateTimeAxis.ActualIntervalType);
                    if (minrange > point.X)
                        minrange = point.X;
                    if (maxrange < point.Y)
                        maxrange = point.Y;
                }

                var date = (DateTime)e.ActualMinimum;
                if (minrange < date.ToOADate())
                    e.ActualMinimum = Convert.ToDouble(minrange).FromOADate();
                date = (DateTime)e.ActualMaximum;
                if (maxrange > date.ToOADate())
                    e.ActualMaximum = Convert.ToDouble(maxrange).FromOADate();
            }
        }

        /// <summary>
        /// Calculate StandardDeviation and StandardError value
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private double[] GetSdErrorValue(IList<double> values)
        {
            var sum = values.Sum();
            var mean = sum / values.Count;
            var dev = new List<double>();
            var sQDev = new List<double>();

            for (var i = 0; i < values.Count; i++)
            {
                dev.Add(values[i] - mean);
                sQDev.Add(dev[i] * dev[i]);
            }

            var sumSqDev = sQDev.Sum(x => x);
            var valueDoubles = new double[2];

            var sDValue = Math.Sqrt(sumSqDev / (values.Count - 1));
            var sDerrorValue = sDValue / Math.Sqrt(DataCount);
            valueDoubles[0] = mean;
            valueDoubles[1] = Type == ErrorBarType.StandardDeviation ? sDValue : sDerrorValue;
            return valueDoubles;
        }

        /// <summary>
        /// Calculate the Plus value of line
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorvalue"></param>
        /// <param name="axischeck"></param>
        /// <returns></returns>
        private double GetPlusValue(double value, double errorvalue, bool axischeck)
        {
            if ((ActualXAxis is DateTimeAxis) && axischeck)
            {
                var dateaxis = ActualXAxis as DateTimeAxis;
                DateTime verDate = Convert.ToDouble(value).FromOADate();
                return DateTimeAxisHelper.IncreaseInterval(verDate, errorvalue, dateaxis.IntervalType).ToOADate();
            }

            return value + errorvalue;
        }

        /// <summary>
        /// Calculate the Minus Value of line
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorvalue"></param>
        /// <param name="axischeck"></param>
        /// <returns></returns>
        private double GetMinusValue(double value, double errorvalue, bool axischeck)
        {
            if ((ActualXAxis is DateTimeAxis) && axischeck)
            {
                var dateaxis = ActualXAxis as DateTimeAxis;
                DateTime verDate = Convert.ToDouble(value).FromOADate();
                return DateTimeAxisHelper.IncreaseInterval(verDate, -errorvalue, dateaxis.IntervalType).ToOADate();
            }

            return value - errorvalue;
        }

        #endregion

        #endregion        
    }
}
