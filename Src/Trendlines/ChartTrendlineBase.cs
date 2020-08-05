using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract partial class TrendlineBase : Control
    {
        #region fields

        private IList<double> _xValues;

        private double _xMin;

        private double _xMax;

       
        private IList<double> trendXValues;
        private IList<double> trendYValues;

        private List<double> trendXSegmentValues;
        private List<double> trendYSegmentValues;

        private IList<double> xNonEmptyValues;
        private IList<double> yNonEmptyValues;
       
        #endregion

        #region constructor
        public TrendlineBase()
        {
#if UNIVERSALWINDOWS
            Stroke = new SolidColorBrush(Colors.Blue);
#endif
            DefaultStyleKey = typeof(TrendlineBase);
            TrendlineSegments = new ObservableCollection<ChartSegment>();
        }
        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the slope.
        /// </summary>
        public double Slope { get; private set; }

        /// <summary>
        /// Gets or sets the intercept.
        /// </summary>
        public double Intercept { get; private set; }

        internal ChartSeries Series
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the polynomial slopes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Public property exposed.")]
        public double[] PolynomialSlopes { get; private set; }

        internal ChartTrendlinePanel TrendlinePanel { get; set; }

        internal ObservableCollection<ChartSegment> TrendlineSegments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <c>Trendline</c> is visible.
        /// </summary>
        /// <value>
        ///     if its <c>true</c>, trendline will be visible.
        /// </value>
        public bool IsTrendlineVisible
        {
            get { return (bool)GetValue(IsTrendlineVisibleProperty); }
            set { SetValue(IsTrendlineVisibleProperty, value); }
        }


        /// <summary>
        /// The DependencyProperty for <see cref="IsTrendlineVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty IsTrendlineVisibleProperty =
            DependencyProperty.Register("IsTrendlineVisible", typeof(bool), typeof(TrendlineBase), 
            new PropertyMetadata(true, OnIsTrendlineVisibleChanged));

        private static void OnIsTrendlineVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as TrendlineBase;

            if (instance == null) return;

            if ((bool)args.NewValue)
                instance.Visibility = Visibility.Visible;
            else
                instance.Visibility = Visibility.Collapsed;

            var series = instance.Series as CartesianSeries;

            if (series != null && series.ActualArea != null)
                series.ActualArea.ScheduleUpdate(); 
        }


        /// <summary>
        /// Gets or sets a value that determines whether to create a legend item for this trendline. 
        /// </summary>
        /// <remarks>
        /// By default, legend will be visible for this trendline.
        /// </remarks>
        /// <value>
        /// <see cref="System.Windows.Visibility"/>
        /// </value>
        public Visibility VisibilityOnLegend
        {
            get { return (Visibility)GetValue(VisibilityOnLegendProperty); }
            set { SetValue(VisibilityOnLegendProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="VisibilityOnLegend"/> property.
        /// </summary>
        public static readonly DependencyProperty VisibilityOnLegendProperty =
           DependencyProperty.Register("VisibilityOnLegend", typeof(Visibility), typeof(TrendlineBase),
           new PropertyMetadata(Visibility.Visible, OnVisibilityOnLegendChanged));
 
        private static void OnVisibilityOnLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrendlineBase trendLine = d as TrendlineBase;

            if (trendLine.Series != null && trendLine.Series.ActualArea!=null)
            {
                trendLine.Series.ActualArea.IsUpdateLegend = true;
                trendLine.Series.ActualArea.ScheduleUpdate();
            }
                
        }


        /// <summary>
        /// Gets or sets the custom template for the legend icons.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>.
        /// </value>
        public DataTemplate LegendIconTemplate
        {
            get { return (DataTemplate)GetValue(LegendIconTemplateProperty); }
            set { SetValue(LegendIconTemplateProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="LegendIconTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty LegendIconTemplateProperty =
            DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(TrendlineBase),
            new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the type of icon to be displayed in legend item.
        /// </summary>
        /// <remarks>
        /// By default, the icon shape will represent the series type.
        /// </remarks>
        /// <value>
        /// The value can be Circle, Rectangle, etc. See <see cref="Syncfusion.UI.Xaml.Charts.ChartLegendIcon"/>.
        /// </value>
        public ChartLegendIcon LegendIcon
        {
            get { return (ChartLegendIcon)GetValue(LegendIconProperty); }
            set { SetValue(LegendIconProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="LegendIcon"/> property.
        /// </summary>
        public static readonly DependencyProperty LegendIconProperty =
            DependencyProperty.Register("LegendIcon", typeof(ChartLegendIcon), typeof(TrendlineBase),
            new PropertyMetadata(ChartLegendIcon.SeriesType, OnLegendIconChanged));

        private static void OnLegendIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TrendlineBase;
            if (instance != null && instance.Series != null)
                instance.UpdateLegendIconTemplate(true);
        }


        /// <summary>
        /// Gets or sets the label that will be displayed in the associated legend item.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Label"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TrendlineBase),
            new PropertyMetadata(string.Empty));


        /// <summary>
        /// Gets or sets the type of the trendline.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.TrendlineType"/>.
        /// </value>
        public TrendlineType Type
        {
            get { return (TrendlineType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Type"/> property.
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(TrendlineType), typeof(TrendlineBase), 
            new PropertyMetadata(TrendlineType.Linear, OnTypeChanged));

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TrendlineBase;
            if (instance != null && instance.Series != null && e.NewValue != null)
            {
                instance.Series.ActualArea.ScheduleUpdate();
            }
        }


        /// <summary>
        /// Gets or sets the brush to paint the stroke of the trendline.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(TrendlineBase),

#if UNIVERSALWINDOWS
            new PropertyMetadata(null, OnStrokeChanged));
#else
            new PropertyMetadata(new SolidColorBrush(Colors.Blue), OnStrokeChanged));
#endif

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as Trendline;
            if (e.NewValue != null && instance != null && instance.TrendlineSegments != null)
                foreach (var segment in instance.TrendlineSegments)
                    segment.Interior = (Brush)e.NewValue;
        }


        /// <summary>
        /// Gets or sets the thickness for the trendline.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(TrendlineBase),
            new PropertyMetadata(2d, OnStrokeThicknessChanged));

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as Trendline;
            if (e.NewValue != null)
                foreach (ChartSegment segment in instance.TrendlineSegments)
                {
                    segment.StrokeThickness = (double)e.NewValue;
                }
        }


        /// <summary>
        /// Gets or sets a collection of Double values that indicates the pattern of
        /// dashes and gaps that is used to outline shapes.
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(TrendlineBase), 
            new PropertyMetadata(null, OnStrokeDashArrayChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TrendlineBase;
            if (e.NewValue != null)
            {
                foreach (var segment in instance.TrendlineSegments)
                {
                    var collection = (DoubleCollection)e.NewValue;
                    if (collection != null && collection.Count > 0)
                    {
                        var doubleCollection = new DoubleCollection();
                        foreach (var value in collection)
                        {
                            doubleCollection.Add(value);
                        }
                        if (segment is SplineSegment)
                            (segment as SplineSegment).StrokeDashArray = doubleCollection;
                        else if (segment is LineSegment)
                            (segment as LineSegment).StrokeDashArray = doubleCollection;
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the range of trend to be estimated from the future.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <value>
        /// The double value.
        /// </value>
        public double ForwardForecast
        {
            get { return (double)GetValue(ForwardForecastProperty); }
            set { SetValue(ForwardForecastProperty, value); }
        }


        /// <summary>
        /// The DependencyProperty for <see cref=" ForwardForecast"/> property.
        /// </summary>
        public static readonly DependencyProperty ForwardForecastProperty =
            DependencyProperty.Register("ForwardForecast", typeof(double), typeof(TrendlineBase),
            new PropertyMetadata(0d, OnTypeChanged));

        /// <summary>
        /// Gets or sets the range of trend to be estimated from the past.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <value>
        /// The double value.
        /// </value>
        public double BackwardForecast
        {
            get { return (double)GetValue(BackwardForecastProperty); }
            set { SetValue(BackwardForecastProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="BackwardForecast"/> property.
        /// </summary>
        public static readonly DependencyProperty BackwardForecastProperty =
            DependencyProperty.Register("BackwardForecast", typeof(double), typeof(TrendlineBase),
            new PropertyMetadata(0d, OnTypeChanged));


        /// <summary>
        /// Gets or sets the Polynomial Order for the polynomial trendline, 
        /// it calculate the order based equation..
        /// </summary>
        /// <value>
        ///     It accepts integer value ranging from 2 to 6.
        /// </value>
        public int PolynomialOrder
        {
            get { return (int)GetValue(PolynomialOrderProperty); }
            set { SetValue(PolynomialOrderProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="PolynomialOrder"/> property.
        /// </summary>
        public static readonly DependencyProperty PolynomialOrderProperty =
            DependencyProperty.Register("PolynomialOrder", typeof(int), typeof(TrendlineBase), 
            new PropertyMetadata(2, OnPolynomialOrderChanged));

        private static void OnPolynomialOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TrendlineBase;
            if (instance != null && instance.Series != null)
            {
                if (e.NewValue != null  && instance.Type == TrendlineType.Polynomial && (instance.Series.ActualData.Count > 2 && instance.Series.ActualData.Count > (int)e.NewValue))
                {
                    instance.Series.ActualArea.ScheduleUpdate();
                }
            }
        }

        #endregion

        #region Methods
        
        #region Major Methods

        /// <summary>
        /// Generates the continuous list without empty points.
        /// </summary>
        /// <param name="yValues"></param>
        private void GenerateNonEmptyXandYValues()
        {
            IList<double> yValues = null;

            yNonEmptyValues = new List<double>();
            xNonEmptyValues = new List<double>();

            if (Series is FinancialSeriesBase)
                yValues = (Series as FinancialSeriesBase).CloseValues;
            else if (Series is RangeSeriesBase)
                yValues = (Series as RangeSeriesBase).LowValues;
            else
                yValues = (Series as XyDataSeries).YValues;
            
            if (yValues != null && yValues.Contains(double.NaN))
            {
                for (int i = 0; i < yValues.Count; i++)
                    if (!double.IsNaN(yValues[i]))
                    {
                        yNonEmptyValues.Add(yValues[i]);
                        xNonEmptyValues.Add(_xValues[i]);
                    }
            }
            else
            {
                yNonEmptyValues = yValues;
                xNonEmptyValues = _xValues;
            }
        }

        /// <summary>
        /// Invoke to render trendline.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TrendlinePanel = this.GetTemplateChild("trendlinePanel") as ChartTrendlinePanel;
            if (TrendlinePanel != null)
            {
                TrendlinePanel.Trend = this;
                foreach (var segment in TrendlineSegments)
                {
                    if (!segment.IsAddedToVisualTree)
                    {
                        UIElement element = segment.CreateVisual(Size.Empty);
                        if (element != null)
                        {
                            TrendlinePanel.Children.Add(element);
                            segment.IsAddedToVisualTree = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update Legend Ion Template method
        /// </summary>
        /// <param name="iconChanged"></param>
        internal void UpdateLegendIconTemplate(bool iconChanged)
        {
            var legendIcon = LegendIcon.ToString();

            if (LegendIcon == ChartLegendIcon.SeriesType)
                legendIcon = this.Type.ToString().Replace("Trend", "");

            if ((LegendIconTemplate == null || iconChanged) && ChartDictionaries.GenericLegendDictionary.Keys.Contains(legendIcon))
            {
                LegendIconTemplate = ChartDictionaries.GenericLegendDictionary[legendIcon] as DataTemplate;
            }
        }

        /// <summary>
        /// Update Trendline elements method
        /// </summary>
        internal void UpdateElements()
        {
            if (Series == null)
            {
                return;
            }
			
            _xValues = Series.GetXValues();
               
            TrendlineSegments.Clear();

            //Generates the non empty x and y lists.
            GenerateNonEmptyXandYValues();

            if(xNonEmptyValues.Count >= 2)
            {
                if ((Series.ActualXAxis is DateTimeAxis) && (Series.ActualXAxis as DateTimeAxis).IntervalType == DateTimeIntervalType.Auto)
                    (Series.ActualXAxis as DateTimeAxis).ActualRangeChanged += Trendline_ActualRangeChanged;
                else if (Series.ActualXAxis is DateTimeAxis) 
                    (Series.ActualXAxis as DateTimeAxis).ActualRangeChanged -= Trendline_ActualRangeChanged;
                _xMin = _xValues.Min();
                _xMax = _xValues.Max();

                //Check Trend line type to Calculate
                CheckTrendlineType(true);
            }
            UpdateLegendIconTemplate(true);
        }

        void CheckTrendlineType(bool update)
        {
            switch (Type)
            {
                case TrendlineType.Linear:
                    UpdateTrendSource();
                    CalculateLinearTrendline();
                    break;
                case TrendlineType.Exponential:
                    UpdateExponentialTrendSource();
                    CalculateExponentialTrendline();
                    break;
                case TrendlineType.Power:
                    UpdatePowerTrendSource();
                    CalculatePowerTrendline();
                    break;
                case TrendlineType.Logarithmic:
                    UpdateLogarithmicTrendSource();
                    CalculateLogarithmicTrendline();
                    break;
                case TrendlineType.Polynomial:
                    if (Series.ActualData.Count >= PolynomialOrder && PolynomialOrder > 1 && PolynomialOrder <= 6)
                    {
                        UpdatePolynomialTrendSource();
                        CalculatePolynomialTrendLine();
                    }
                    break;
            }
        }

        protected virtual DependencyObject CloneTrendline(DependencyObject obj)
        {
            TrendlineBase trendline = new Trendline ();
            trendline.IsTrendlineVisible = this.IsTrendlineVisible;
            trendline.Type = this.Type;
            trendline.Label = this.Label;
            trendline.Stroke = this.Stroke;
            trendline.StrokeDashArray = this.StrokeDashArray;
            trendline.StrokeThickness = this.StrokeThickness;
            trendline.PolynomialOrder = this.PolynomialOrder;
            return trendline;
        }

        /// <summary>
        /// Clone the trendline
        /// </summary>
        /// <returns></returns>
        public DependencyObject Clone()
        {
            return CloneTrendline(null);
        }

        #endregion

        #region Polynomial Trendline

        /// <summary>
        /// Update Polynomial trend source
        /// </summary>
        private void UpdatePolynomialTrendSource()
        {
            trendXValues = new List<double>();
            trendYValues = yNonEmptyValues;

            for (int i = 0; i < trendYValues.Count; i++)
            {
                if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                    trendXValues.Add(xNonEmptyValues[i]+1);
                else
                    trendXValues.Add(xNonEmptyValues[i]);
            }
        }

        /// <summary>
        /// Calculate Gauss Jordan Eliminiation value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
        private static bool GaussJordanEliminiation(double[,] a, double[] b)
        {
            var length = a.GetLength(0);
            var numArray1 = new int[length];
            var numArray2 = new int[length];
            var numArray3 = new int[length];
            for (var index = 0; index < length; ++index)
                numArray3[index] = 0;
            for (var index1 = 0; index1 < length; ++index1)
            {
                var num1 = 0.0;
                var index2 = 0;
                var index3 = 0;
                for (var index4 = 0; index4 < length; ++index4)
                {
                    if (numArray3[index4] != 1)
                    {
                        for (var index5 = 0; index5 < length; ++index5)
                        {
                            if (numArray3[index5] == 0 && Math.Abs(a[index4, index5]) >= num1)
                            {
                                num1 = Math.Abs(a[index4, index5]);
                                index2 = index4;
                                index3 = index5;
                            }
                        }
                    }
                }
                ++numArray3[index3];
                if (index2 != index3)
                {
                    for (var index4 = 0; index4 < length; ++index4)
                    {
                        var num2 = a[index2, index4];
                        a[index2, index4] = a[index3, index4];
                        a[index3, index4] = num2;
                    }
                    var num3 = b[index2];
                    b[index2] = b[index3];
                    b[index3] = num3;
                }
                numArray2[index1] = index2;
                numArray1[index1] = index3;
                if (a[index3, index3] == 0.0)
                    return false;
                double num4 = 1.0 / a[index3, index3];
                a[index3, index3] = 1.0;
               
                for (var index4 = 0; index4 < length; ++index4)
                    a[index3, index4] *= num4;
                
                b[index3] *= num4;
                
                for (var index4 = 0; index4 < length; ++index4)
                {
                    if (index4 != index3)
                    {
                        var num2 = a[index4, index3];
                        a[index4, index3] = 0.0;
                        for (var index5 = 0; index5 < length; ++index5)
                            a[index4, index5] -= a[index3, index5] * num2;
                        b[index4] -= b[index3] * num2;
                    }
                }
            }
            for (var index1 = length - 1; index1 >= 0; --index1)
            {
                if (numArray2[index1] != numArray1[index1])
                {
                    for (var index2 = 0; index2 < length; ++index2)
                    {
                        var num = a[index2, numArray2[index1]];
                        a[index2, numArray2[index1]] = a[index2, numArray1[index1]];
                        a[index2, numArray1[index1]] = num;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calculate Polynomial Trendline with order
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
        private void CalculatePolynomialTrendLine()
        {
            var power = PolynomialOrder;

            // Calculate sum of y datapoints 1 X power matrix

            PolynomialSlopes = new double[power + 1];
            for (var index1 = 0; index1 < Series.DataCount; index1++)
            {
                var num2 = trendXValues[index1];
                var yval = trendYValues[index1];
                if (!double.IsNaN(num2) && !double.IsNaN(yval))
                {
                    for (var index2 = 0; index2 <= power; ++index2)
                        PolynomialSlopes[index2] += Math.Pow(num2, index2) * yval;
                }
            }

            // Calculate sum matrix of x datapoints

            var numArray = new double[1 + 2 * power];

            var matrixOfA = new double[power + 1, power + 1];
            var num1 = 0;
            for (var index1 = 0; index1 < trendXValues.Count; ++index1)
            {
                var num2 = 1.0;
                var d = trendXValues[index1];
                if (!double.IsNaN(d) && !double.IsNaN(trendYValues[index1]))
                {
                    for (var index2 = 0; index2 < numArray.Length; ++index2)
                    {
                        numArray[index2] += num2;
                        num2 *= d;
                        ++num1;
                    }
                }
            }

            for (var index1 = 0; index1 <= power; ++index1)
            {
                for (var index2 = 0; index2 <= power; ++index2)
                    matrixOfA[index1, index2] = numArray[index1 + index2];
            }


            //Calculation Gauss jordan eliminiation value of a and b matrix
            if (!GaussJordanEliminiation(matrixOfA, PolynomialSlopes))
                PolynomialSlopes = null;

            //Create segments methods
            CreatePolynomialSegments();

        }

        /// <summary>
        /// Create the polynomial segments
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void CreatePolynomialSegments()
        {
            trendXSegmentValues = new List<double>();
            trendYSegmentValues = new List<double>();

            double count = trendXValues.Count;
            double x = 1, xValue = 0.0;
            if (PolynomialSlopes != null)
            {
                for (var i = 1; i <= PolynomialSlopes.Length; i++)
                {
                    var axis = Series.ActualXAxis;
                   
                    if (i == 1)
                    {
                        xValue = _xMin - BackwardForecast;
                        if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                        {
                            trendXSegmentValues.Add(xValue);
                            trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, i - ((count - 1) * (BackwardForecast / (_xMax - _xMin)))));
                        }
                        else
                        {
                            var dateTimeAxis = axis as DateTimeAxis;
                            if (dateTimeAxis != null)
                            {
                                double foreCastStartValue = CalculateDateTimeForecastValue(_xMin, -BackwardForecast, dateTimeAxis.IntervalType);
                                trendXSegmentValues.Add(foreCastStartValue);
                                xValue = foreCastStartValue;
                            }
                            else
                                trendXSegmentValues.Add(xValue);
                            trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, xValue));
                        }
                    }
                    else if (i == PolynomialSlopes.Length)
                    {
                        xValue = _xMax + ForwardForecast;
                        if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                        {
                            trendXSegmentValues.Add(xValue);
                            trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, count + ((count - 1) * (ForwardForecast / (_xMax - _xMin)))));
                        }
                        else
                        {
                            var dateTimeAxis = axis as DateTimeAxis;
                            if (dateTimeAxis != null)
                            {
                                var foreCastEndValue = CalculateDateTimeForecastValue(_xMax, ForwardForecast, dateTimeAxis.IntervalType);
                                trendXSegmentValues.Add(foreCastEndValue);
                                xValue = foreCastEndValue;
                            }
                            else
                                trendXSegmentValues.Add(xValue);
                            trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, xValue));
                        }                     
                    }
                    else
                    {
                        x += (count + ((count - 1) * (ForwardForecast / (_xMax - _xMin)))) / PolynomialSlopes.Length;
                        if (!(count == x))
                        {
                            if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                            {
                                trendXSegmentValues.Add(Math.Ceiling(x) - 1);
                                trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, Math.Ceiling(Math.Ceiling(x))));
                            }
                            else
                            {
                                if (count > x)
                                {
                                    xValue = _xValues[(int)Math.Ceiling(x) - 1];
                                    trendXSegmentValues.Add(xValue);
                                    trendYSegmentValues.Add(GetPolynomialYValue(PolynomialSlopes, Math.Ceiling(xValue)));
                                }
                            }                            
                        }
                    }
                }
            }

            //create spline segments
            CreateSpline();
        }
        
        #endregion

        #region Logarithmic Trendline
       
        /// <summary>
        /// Update Logarithmic Trend Source
        /// </summary>
        private void UpdateLogarithmicTrendSource()
        {
            trendXValues = new List<double>();
            trendYValues = yNonEmptyValues;

            for (int i = 0; i < trendYValues.Count; i++)
            {
                if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                    trendXValues.Add(Math.Log(xNonEmptyValues[i] + 1));
                else
                    trendXValues.Add(Math.Log(xNonEmptyValues[i]));
            }            
            CalculateSumXAndYValue();
        }

        /// <summary>
        /// Caluculate Logarithmic Value and Draw Trendline
        /// </summary>
        private void CalculateLogarithmicTrendline()
        {
            var n = xNonEmptyValues.Count;

            if (n > 1)
            {
                CalculateTrendXSegment(_xValues.Count);

                if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                {
                    trendYSegmentValues.Add(GetLogarithmicYValue(1));
                    trendYSegmentValues.Add(GetLogarithmicYValue(Math.Round((double)n / 2)));
                    trendYSegmentValues.Add(GetLogarithmicYValue(n + ForwardForecast));
                }
                else 
                {
                    trendYSegmentValues.Add(GetLogarithmicYValue(trendXSegmentValues[0]));
                    trendYSegmentValues.Add(GetLogarithmicYValue(trendXSegmentValues[1]));
                    trendYSegmentValues.Add(GetLogarithmicYValue(trendXSegmentValues[2]));
                }

                CreateSpline();

            }
        }

        #endregion 

        #region Exponential Trendline

        /// <summary>
        /// Update Exponential Trend Source
        /// </summary>
        private void UpdateExponentialTrendSource()
        {
            trendXValues = new List<double>();
            trendYValues = new List<double>();

            for (int i = 0; i < yNonEmptyValues.Count; i++)
            {
                trendYValues.Add(Math.Log(yNonEmptyValues[i]));
                if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                    trendXValues.Add(xNonEmptyValues[i] + 1);
                else
                    trendXValues.Add(xNonEmptyValues[i]);
            }

            CalculateSumXAndYValue();
        }

        /// <summary>
        /// Calculate Exponential Value and Draw Trendline
        /// </summary>
        private void CalculateExponentialTrendline()
        {
            var n = xNonEmptyValues.Count;
            if (n > 1)
            {
                CalculateTrendXSegment(_xValues.Count);

                trendYSegmentValues.Add(GetExponentialYValue(trendXSegmentValues[0]));
                trendYSegmentValues.Add(GetExponentialYValue(trendXSegmentValues[1]));
                trendYSegmentValues.Add(GetExponentialYValue(trendXSegmentValues[2]));

                CreateSpline();
            }
        }

#endregion

        #region Power Trendline
        /// <summary>
        /// Update Power TrendSource
        /// </summary>
        private void UpdatePowerTrendSource()
        {
            trendXValues = new List<double>();
            trendYValues = new List<double>();
            
            for (int i = 0; i < yNonEmptyValues.Count; i++)
            {
                trendYValues.Add(Math.Log(yNonEmptyValues[i]));
                if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                    trendXValues.Add(Math.Log(xNonEmptyValues[i] + 1));
                else
                    trendXValues.Add(Math.Log(xNonEmptyValues[i]));
            }
            CalculateSumXAndYValue();
        }

        /// <summary>
        /// Calculate Power Value and Draw Trendline
        /// </summary>
        private void CalculatePowerTrendline()
        {
            var n = xNonEmptyValues.Count;
            if (n > 1)
            {
                CalculateTrendXSegment(_xValues.Count);

                if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                {
                    trendYSegmentValues.Add(GetPowerYValue(1));
                    trendYSegmentValues.Add(GetPowerYValue(Math.Round((double)n / 2)));
                    trendYSegmentValues.Add(GetPowerYValue(n + ForwardForecast));
                }
                else
                {
                    trendYSegmentValues.Add(GetPowerYValue(trendXSegmentValues[0]));
                    trendYSegmentValues.Add(GetPowerYValue(trendXSegmentValues[1]));
                    trendYSegmentValues.Add(GetPowerYValue(trendXSegmentValues[2]));
                }
                CreateSpline();

            }
        }
        #endregion

        #region Linear TrendLine
        /// <summary>
        /// Update Linear Trend Source
        /// </summary>
        private void UpdateTrendSource()
        {
            trendXValues = new List<double>();
            trendYValues = yNonEmptyValues;

            for (int i = 0; i < yNonEmptyValues.Count; i++)
            {
               trendXValues.Add(xNonEmptyValues[i]);
            }

            CalculateSumXAndYValue();
        }

        /// <summary>
        /// Calculate Linear Value and Draw Trendline
        /// </summary>
        private void CalculateLinearTrendline()
        {
            var count =_xValues.Count;

            if (count > 0)
            {
                LineSegment linesegemnt;
                var startYValue = 0.0;
                var endYValue = 0.0;
                if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
                {
                    linesegemnt = new LineSegment() { Interior = this.Stroke, Stroke = this.Stroke, StrokeThickness = this.StrokeThickness, StrokeDashArray = this.StrokeDashArray };
                    linesegemnt.X1 = 0 - BackwardForecast;
                    linesegemnt.X2 = (count - 1) + ForwardForecast;
                    startYValue = GetLinearYValue(linesegemnt.X1);
                    endYValue = GetLinearYValue(linesegemnt.X2);
                    linesegemnt.Y1 = startYValue;
                    linesegemnt.Y2 = endYValue;
                    linesegemnt.Item = this;
                    linesegemnt.SetData(linesegemnt.X1, startYValue, linesegemnt.X2, endYValue);
                }
                else
                {
                    var axis = Series.ActualXAxis as DateTimeAxis;
                    if (axis != null)
                    {
                       

                        double foreCastStartValue = CalculateDateTimeForecastValue(_xMin, -BackwardForecast,axis.IntervalType );

                        double foreCastEndValue = CalculateDateTimeForecastValue(_xMax, ForwardForecast, axis.IntervalType);

                        linesegemnt = new LineSegment()
                        {
                            Interior = this.Stroke,
                            Stroke = this.Stroke,
                            StrokeThickness = this.StrokeThickness,
                            StrokeDashArray = this.StrokeDashArray
                        };
                        linesegemnt.X1 = foreCastStartValue;
                        linesegemnt.X2 = foreCastEndValue;
                        startYValue = GetLinearYValue(foreCastStartValue);
                        endYValue = GetLinearYValue(foreCastEndValue);
                        linesegemnt.Y1 = startYValue;
                        linesegemnt.Y2 = linesegemnt.Y2Value;
                        linesegemnt.Item = this;
                        linesegemnt.SetData(foreCastStartValue, startYValue, foreCastEndValue, endYValue);

                    }
                    else
                    {
                        linesegemnt = new LineSegment()
                        {
                            Interior = this.Stroke,
                            Stroke = this.Stroke,
                            StrokeThickness = this.StrokeThickness,
                            StrokeDashArray = this.StrokeDashArray
                        };
                        linesegemnt.X1 = _xMin - BackwardForecast;
                        linesegemnt.X2 = _xMax + ForwardForecast;
                        startYValue = GetLinearYValue(_xMin - BackwardForecast);
                        endYValue = GetLinearYValue(_xMax + ForwardForecast);
                        linesegemnt.Y1 = startYValue;
                        linesegemnt.Y2 = endYValue;
                        linesegemnt.Item = this;
                        linesegemnt.SetData(_xMin - BackwardForecast, startYValue, _xMax + ForwardForecast, endYValue);
                    }
                }
                linesegemnt.Series = this.Series;
                TrendlineSegments.Add(linesegemnt);

            }

        }

        #endregion

        #region Trendline calculation method

        /// <summary>
        /// Calculate Sum of x and y values
        /// </summary>
        private void CalculateSumXAndYValue()
        {
            var count = trendXValues.Count;
            var sumX = trendXValues.Sum(x => x);
            var sumX2 = trendXValues.Sum(x => x * x);
            var sumY = trendYValues.Sum(y => !double.IsNaN(y)?y:0);
            double sumXY = 0;
            for (var i = 0; i < count; i++)
            {
                if (!double.IsNaN(trendYValues[i]))
                sumXY += trendXValues[i] * trendYValues[i];
            }
            Slope = ((sumXY * count) - (sumX * sumY)) / ((sumX2 * count) - (sumX * sumX));
            if (Type == TrendlineType.Exponential || Type == TrendlineType.Power)
              Intercept=  Math.Exp((sumY - (Slope * sumX)) / count);
            else
                Intercept = (sumY - (Slope*sumX))/count;
        }

        /// <summary>
        /// Calculate Trend Segment X values
        /// </summary>
        /// <param name="n"></param>
        private void CalculateTrendXSegment(int n)
        {
            trendXSegmentValues = new List<double>();
            trendYSegmentValues = new List<double>();

            if (Series.ActualXAxis is CategoryAxis || Series.ActualXAxis is DateTimeCategoryAxis)
            {
                trendXSegmentValues.Add(-BackwardForecast);
                trendXSegmentValues.Add(Math.Round((double)n / 2) - 1);
                trendXSegmentValues.Add((n - 1) + ForwardForecast);
            }
            else
            {
                var axis = Series.ActualXAxis as DateTimeAxis;
                if (axis != null)
                {
                    
                    double foreCastStartValue = CalculateDateTimeForecastValue(_xMin, -BackwardForecast,axis.IntervalType);

                    trendXSegmentValues.Add(foreCastStartValue);

                    trendXSegmentValues.Add(_xMin + ((_xMax - _xMin) / 2));

                    double foreCastEndValue = CalculateDateTimeForecastValue(_xMax, ForwardForecast, axis.IntervalType);

                    trendXSegmentValues.Add(foreCastEndValue);
                }
                else
                {
                    trendXSegmentValues.Add(_xMin - BackwardForecast);
                    trendXSegmentValues.Add(_xMin + ((_xMax - _xMin) / 2));
                    trendXSegmentValues.Add(_xMax + ForwardForecast);
                }
            }
        }

        private static double CalculateDateTimeForecastValue(double value, double forecastValue, DateTimeIntervalType type)
        {
            DateTime start = Convert.ToDouble(value).FromOADate();
            DateTime foreCastStartDate = DateTimeAxisHelper.IncreaseInterval(start, forecastValue,type);
            double foreCastValue = Convert.ToDateTime(foreCastStartDate).ToOADate();
            return foreCastValue;
        }

        void Trendline_ActualRangeChanged(object sender, ActualRangeChangedEventArgs e)
        {
            var dateTimeAxis = sender as DateTimeAxis;
            if (dateTimeAxis != null && dateTimeAxis.IntervalType == DateTimeIntervalType.Auto)
            {
                if (Type == TrendlineType.Linear)
                {
                    double foreCastStartValue = CalculateDateTimeForecastValue(_xMin, -BackwardForecast,
                        dateTimeAxis.ActualIntervalType);

                    double foreCastEndValue = CalculateDateTimeForecastValue(_xMax, ForwardForecast,
                        dateTimeAxis.ActualIntervalType);

                    var startYValue = GetLinearYValue(1 - BackwardForecast);
                    var endYValue = GetLinearYValue(_xValues.Count + ForwardForecast);

                    TrendlineSegments[0].SetData(foreCastStartValue, startYValue, foreCastEndValue, endYValue);
                }
                else
                {
                    trendXSegmentValues[0] = CalculateDateTimeForecastValue(_xMin, -BackwardForecast,
                        dateTimeAxis.ActualIntervalType);
                    trendXSegmentValues[trendXSegmentValues.Count - 1] = CalculateDateTimeForecastValue(_xMax,
                        ForwardForecast, dateTimeAxis.ActualIntervalType);
                    CreateSpline();

                }

                var min = TrendlineSegments[0].XRange.Start;
                var max = TrendlineSegments[TrendlineSegments.Count - 1].XRange.End;
                var date = (DateTime)e.ActualMinimum;
                if (min < date.ToOADate())
                    e.ActualMinimum = Convert.ToDouble(min).FromOADate();
                date = (DateTime)e.ActualMaximum;
                if (max > date.ToOADate())
                    e.ActualMaximum = Convert.ToDouble(max).FromOADate();
            }
        }
        
        #endregion

        #region Trendline Y Equation
        /// <summary>
        /// Get Linear Y Value
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>Segment y values</returns>
        private double GetLinearYValue(double xValue)
        {
            return Intercept + Slope * xValue;
        }
        /// <summary>
        /// Get Logarithmic Y Value
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>Segment y values</returns>
        private double GetLogarithmicYValue(double xValue)
        {
            return Intercept + Slope * Math.Log(xValue);
        }
        /// <summary>
        /// Get Exponential Y Value
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>Segment y values</returns>
        private double GetExponentialYValue(double xValue)
        {
            return (Intercept * Math.Exp(Slope * xValue));
        }
        /// <summary>
        /// Get Power Y Value
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>Segment y values </returns>
        private double GetPowerYValue(double xValue)
        {
            return (Intercept * Math.Pow(xValue, Slope));
        }
        /// <summary>
        /// Get polynomial y value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns>Segment y values</returns>
        private static double GetPolynomialYValue(double[] a, double x)
        {
            return a.Select((t, index) => t*Math.Pow(x, (double) index)).Sum();
        }

        #endregion

        #region Spline Segment Method
        /// <summary>
        /// Create Spline Segment of Trendline
        /// </summary>
        private void CreateSpline()
        {
            TrendlineSegments.Clear();
            int index = -1;
            double[] yCoef = null;

            NaturalSpline(trendXSegmentValues, trendYSegmentValues, out yCoef);
            for (int i = 0; i < trendXSegmentValues.Count; i++)
            {
                index = i + 1;

                var startPoint = new ChartPoint(trendXSegmentValues[i], trendYSegmentValues[i]);
                if (index < trendXSegmentValues.Count)
                {
                    var endPoint = new ChartPoint(trendXSegmentValues[index], trendYSegmentValues[index]);
                    ChartPoint startControlPoint;
                    ChartPoint endControlPoint;

                    GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);
                   
                    var splineSegment = new TrendlineSegment(startPoint, startControlPoint, endControlPoint, endPoint, this.Series)
                    {
                       
                        Interior = this.Stroke,
                        StrokeDashArray = this.StrokeDashArray,
                        StrokeThickness = this.StrokeThickness,
                        Series = this.Series
                    };
                    splineSegment.SetData(startPoint, startControlPoint, endControlPoint, endPoint);
                    TrendlineSegments.Add(splineSegment);

                }
            }
        }

        /// <summary>
        /// Coefficient Of Natural Spline Segment
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="ys2"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected void NaturalSpline(List<double> xValues, List<double> yValues, out double[] ys2)
        {
            var count = (int)xValues.Count;

            ys2 = new double[count];

            double a = 6;
            double[] u = new double[count];
            double p;

            ys2[0] = u[0] = 0;
            ys2[count - 1] = 0;

            for (int i = 1; i < count - 1; i++)
            {
                double d1 = xValues[i] - xValues[i - 1];
                double d2 = xValues[i + 1] - xValues[i - 1];
                double d3 = xValues[i + 1] - xValues[i];
                double dy1 = yValues[i + 1] - yValues[i];
                double dy2 = yValues[i] - yValues[i - 1];

                if (xValues[i] == xValues[i - 1] || xValues[i] == xValues[i + 1])
                {
                    ys2[i] = 0;
                    u[i] = 0;
                }
                else
                {
                    p = 1 / (d1 * ys2[i - 1] + 2 * d2);
                    ys2[i] = -p * d3;
                    u[i] = p * (a * (dy1 / d3 - dy2 / d1) - d1 * u[i - 1]);
                }
            }

            for (int k = count - 2; k >= 0; k--)
            {
                ys2[k] = ys2[k] * ys2[k + 1] + u[k];
            }
        }

        /// <summary>
        /// Returns the controlPoints of the curve
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="ys1"></param>
        /// <param name="ys2"></param>
        /// <param name="controlPoint1"></param>
        /// <param name="controlPoint2"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        protected void GetBezierControlPoints(ChartPoint point1, ChartPoint point2, double ys1, double ys2, out ChartPoint controlPoint1, out ChartPoint controlPoint2)
        {
            const double One_thrid = 1 / 3.0d;

            double deltaX2 = point2.X - point1.X;

            deltaX2 = deltaX2 * deltaX2;

            double dx1 = 2 * point1.X + point2.X;
            double dx2 = point1.X + 2 * point2.X;

            double dy1 = 2 * point1.Y + point2.Y;
            double dy2 = point1.Y + 2 * point2.Y;

            double y1 = One_thrid * (dy1 - One_thrid * deltaX2 * (ys1 + 0.5f * ys2));
            double y2 = One_thrid * (dy2 - One_thrid * deltaX2 * (0.5f * ys1 + ys2));

            controlPoint1 = new ChartPoint(dx1 * One_thrid, y1);
            controlPoint2 = new ChartPoint(dx2 * One_thrid, y2);
        }
        #endregion

        #endregion
    }

}
