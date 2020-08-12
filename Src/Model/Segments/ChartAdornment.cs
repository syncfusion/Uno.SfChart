using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.UI;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart adornment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="ChartAdornmentInfo"/>

    public partial class ChartAdornment : ChartSegment
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Background"/> property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ChartAdornment),
            new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="BorderThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(ChartAdornment),
            new PropertyMetadata(new Thickness(0)));

        /// <summary>
        ///The DependencyProperty for <see cref="BorderBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ChartAdornment),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// The DependencyProperty for <see cref="Margin"/> property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(Thickness), typeof(ChartAdornment),
            new PropertyMetadata(new Thickness(5)));

        /// <summary>
        /// The DependencyProperty for <see cref="FontFamily"/> property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
                DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(ChartAdornment),
                new PropertyMetadata(new FontFamily("Times New Roman")));

        /// <summary>
        /// The DependencyProperty for <see cref="FontStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(ChartAdornment), new PropertyMetadata(FontStyle.Normal));

        /// <summary>
        /// The DependencyProperty for <see cref="FontSize"/> property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(ChartAdornment),
            new PropertyMetadata(0d));

        /// <summary>
        /// The DependencyProperty for <see cref="Foreground"/> property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(ChartAdornment),
            new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="ConnectorRotationAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty ConnectorRotationAngleProperty =
            DependencyProperty.Register("ConnectorRotationAngle", typeof(double), typeof(ChartAdornment),
            new PropertyMetadata(0d));

        /// <summary>
        ///  The DependencyProperty for <see cref="ConnectorHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty ConnectorHeightProperty =
            DependencyProperty.Register("ConnectorHeight", typeof(double), typeof(ChartAdornment),
            new PropertyMetadata(0d));

        #endregion

        #region Fields

        #region Internal Fields

        internal ChartAdornmentContainer adormentContainer;

        #endregion

        #region Private Fields

        double x;

        double y;

        double xpos, ypos;

        double xData;

        double yData;

        private ActualLabelPosition actualLabelPosition;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ChartAdornment()
        {

        }

        /// <summary>
        /// Called when instance created for ChartAdornment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="series"></param>
        public ChartAdornment(double xVal, double yVal, double x, double y, ChartSeriesBase series)
        {

        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets series property
        /// </summary>
        public new ChartSeriesBase Series
        {
            get;
            protected internal set;
        }

        /// <summary>
        /// Gets or sets the background brush to apply to the label content.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the outer border thickness of the adornment label.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush that draws the outer border color. 
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the outer margin of a label.
        /// </summary>
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font family for the adornment label.
        /// </summary>
        /// <remarks>
        /// Identifies font family that should be used to display adornment's text.
        /// </remarks>
        /// <value>
        /// <see cref="System.Windows.Media.FontFamily"/>
        /// </value>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font style for the adornment.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Media.FontStyle"/>
        /// </value>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font size for the label.
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground brush to be applied for the label content.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rotation angle for the connectors.
        /// </summary>
        public double ConnectorRotationAngle
        {
            get { return (double)GetValue(ConnectorRotationAngleProperty); }
            set { SetValue(ConnectorRotationAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the connector line.
        /// </summary>
        public double ConnectorHeight
        {
            get { return (double)GetValue(ConnectorHeightProperty); }
            set { SetValue(ConnectorHeightProperty, value); }
        }

        /// <summary>
        /// Gets the actual content displayed visually. Actual content is resolved based on
        /// <see cref="ChartAdornmentInfo.SegmentLabelContent"/>
        /// </summary>
        public object ActualContent
        {
            get
            {
                if (Series.adornmentInfo == null) return null;
                string labelFormat = Series.adornmentInfo.SegmentLabelFormat;
                var content = Series.adornmentInfo.SegmentLabelContent;
                //WPF-14375 : segmentLabelContent not working properly, while setting Adornmentinfo labelTemplate and UseSeriesPalette as true
                if (Series.adornmentInfo.LabelTemplate == null)
                {
                    content = LabelContent.LabelContentPath;
                }
                return CalculateLabelContent(content, labelFormat);

            }
        }

        /// <summary>
        /// Gets or sets the x-value to be bind in ChartAdornment.
        /// </summary>
        public double XData
        {
            get
            {
                return xData;
            }
            set
            {
                xData = value;
                OnPropertyChanged("XData");
                OnPropertyChanged("ActualContent");
            }
        }

        /// <summary>
        /// Gets or sets the y-value to be bind in ChartAdornment.
        /// </summary>
        public double YData
        {
            get
            {
                return yData;
            }
            set
            {
                yData = value;
                OnPropertyChanged("YData");
                OnPropertyChanged("ActualContent");
            }
        }

        /// <summary>
        /// Gets or sets the x screen coordinate relative to series
        /// </summary>
        public double X
        {
            get
            {
                return x;
            }
            internal set
            {
                x = value;
            }
        }

        /// <summary>
        /// Gets or sets the y screen coordinate relative to series
        /// </summary>
        public double Y
        {
            get
            {
                return y;
            }
            internal set
            {
                y = value;
            }
        }

        #endregion

        #region Internal Properties

        internal bool CanHideLabel
        {
            get
            {
                return ((YData == 0 || double.IsNaN(YData)) && (this.Series is AccumulationSeriesBase));
            }
        }

        internal double XPos
        {
            get
            {
                return xpos;
            }
            set
            {
                xpos = value;
                OnPropertyChanged("XPos");
            }
        }

        internal double YPos
        {
            get
            {
                return ypos;
            }
            set
            {
                ypos = value;
                OnPropertyChanged("YPos");
            }
        }

        internal ActualLabelPosition ActualLabelPosition
        {
            get
            {
                return actualLabelPosition;
            }
            set
            {
                actualLabelPosition = value;
            }
        }

        internal List<object> Data
        {
            get
            {
                if(Series is HistogramSeries)
                {
                    var index = Series.Adornments.IndexOf(this);
                    return (Series.Segments[index] as HistogramSegment).Data;
                }

                return null;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        public override void SetData(params double[] Values)
        {
            XData = Values[0];
            YData = Values[1];
            XPos = Values[2];
            YPos = Values[3];
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>
        public override UIElement CreateVisual(Size size)
        {
            if (adormentContainer == null)
                adormentContainer = new ChartAdornmentContainer(this);

            return adormentContainer;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>

        public override UIElement GetRenderedVisual()
        {
            return adormentContainer;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>

        public override void Update(IChartTransformer transformer)
        {

            if (Series is RangeColumnSeries && !Series.IsMultipleYPathRequired)
            {
                YPos = (Series.ActualYAxis.VisibleRange.End - Math.Abs(Series.ActualYAxis.VisibleRange.Start)) / 2;
            }

            Point point = transformer.TransformToVisible(XPos, YPos);
            this.X = point.X;
            this.Y = point.Y;

        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>

        public override void OnSizeChanged(Size size)
        {

        }

        #endregion

        #region Internal Virtual Methods

        internal virtual ChartAdornmentContainer GetAdornmentContainer()
        {
            return adormentContainer;
        }

        #endregion

        #region Internal Override Methods

        internal override UIElement CreateSegmentVisual(Size size)
        {
            BindColorProperties();
            return CreateVisual(size);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Updated the text for the adornment label TextBlock.
        /// </summary>      
        internal object GetTextContent()
        {
            if (this.CanHideLabel)
                return "";

            string format = this.Series.adornmentInfo.SegmentLabelFormat;
            var content = this.Series.adornmentInfo.SegmentLabelContent;

            if (content == LabelContent.LabelContentPath)
            {
                content = LabelContent.YValue;
            }

            return this.CalculateLabelContent(content, format);
        }

        internal object CalculateLabelContent(LabelContent content, string labelFormat)
        {
            switch (content)
            {
                case LabelContent.XValue:
                    return this.XData.ToString(labelFormat, CultureInfo.CurrentCulture);
                case LabelContent.YValue:
                    if (!double.IsNaN(YData))
                    {
                        labelFormat = string.IsNullOrEmpty(labelFormat) ? "0.##" : labelFormat;//Round off default decimal point to two digits while segment dragging-WRT-4212
                        return this.YData.ToString(labelFormat, CultureInfo.CurrentCulture);
                    }
                    else
                        return "";
                case LabelContent.Percentage:
                    double grandPercentage;

                    grandPercentage = (Series is HistogramSeries) ?
                        this.Series.GetGrandTotal((Series as HistogramSeries).ActualYValues) :
                        this.Series.GetGrandTotal(this.Series.ActualSeriesYValues[0]);

                    labelFormat = string.IsNullOrEmpty(labelFormat) ? "0.##" : labelFormat; //WP-559 - decimal digits round-off to 2 positions by default
                    return (this.YData / grandPercentage * 100).ToString(labelFormat, CultureInfo.CurrentCulture) + "%";
                case LabelContent.YofTot:
                    double grandTotal;

                    grandTotal = (Series is HistogramSeries) ?
                        this.Series.GetGrandTotal((Series as HistogramSeries).ActualYValues) :
                        this.Series.GetGrandTotal(this.Series.ActualSeriesYValues[0]);

                    labelFormat = string.IsNullOrEmpty(labelFormat) ? "0.##" : labelFormat;
                    return this.YData.ToString(labelFormat, CultureInfo.CurrentCulture) + " of " + grandTotal.ToString(labelFormat, CultureInfo.CurrentCulture);
                case LabelContent.DateTime:
                    object datetimeContent = null;
                    if (this.Series.IsIndexed)
                    {
                        List<double> dateValues = this.Series.ActualXValues as List<double>;
                        if (dateValues != null)
                            datetimeContent = dateValues[(int)this.XData].FromOADate().ToString(this.Series.adornmentInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                        else
                        {
                            List<string> stringValues = this.Series.ActualXValues as List<string>;
                            DateTime date = DateTime.MinValue;
                            DateTime.TryParse(stringValues[(int)this.XData], out date);
                            datetimeContent = date.ToString(this.Series.adornmentInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                        }

                    }
                    else
                    {
                        datetimeContent = (Series is HistogramSeries) ?
                            this.XPos.FromOADate().ToString(this.Series.adornmentInfo.SegmentLabelFormat,
                            CultureInfo.CurrentCulture) :
                            this.XData.FromOADate().ToString(this.Series.adornmentInfo.SegmentLabelFormat,
                            CultureInfo.CurrentCulture);
                    }
                    return datetimeContent;
                case LabelContent.LabelContentPath:

                    return this;
                default:
                    if (!double.IsNaN(YData))
                    {
                        labelFormat = string.IsNullOrEmpty(labelFormat) ? "0.##" : labelFormat;
                        return this.YData.ToString(labelFormat, CultureInfo.CurrentCulture);
                    }
                    else
                        return "";
            }
        }

        internal void BindColorProperties()
        {
            var isGrouping = (Series.ActualXAxis is CategoryAxis) ? (Series.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (Series is WaterfallSeries)
            {
                var segment = Series.Segments.FirstOrDefault(seg => seg.Item == Item) as WaterfallSegment;

                if (segment is WaterfallSegment)
                    BindWaterfallSegmentInterior(segment);
            }
            else if (!this.IsEmptySegmentInterior)
            {
                Binding binding1 = new Binding();
                binding1.Source = Series;
                binding1.Path = new PropertyPath("Interior");
                binding1.Converter = new InteriorConverter(Series);
                if (Series is CircularSeriesBase && !double.IsNaN(((CircularSeriesBase)Series).GroupTo))
                    binding1.ConverterParameter = Series.Adornments.IndexOf(this);
                else if (!isGrouping && (Series.IsSideBySide && (!(Series is RangeSeriesBase)) && (!(Series is FinancialSeriesBase))))
                    binding1.ConverterParameter = Series.GroupedActualData.IndexOf(Item);
                else
                    binding1.ConverterParameter = Series is HistogramSeries ? Series.Adornments.IndexOf(this) : Series.ActualData.IndexOf(Item);
                BindingOperations.SetBinding(this, ChartSegment.InteriorProperty, binding1);
            }
            else
            {
                Binding binding1 = new Binding();
                binding1.Source = Series;
                binding1.ConverterParameter = Series.Interior;
                binding1.Path = new PropertyPath("EmptyPointInterior");
                binding1.Converter = new MultiInteriorConverter();
                BindingOperations.SetBinding(this, ChartSegment.InteriorProperty, binding1);
            }

            Binding binding = new Binding();
            binding.Source = Series;
            binding.Path = new PropertyPath("Stroke");
            BindingOperations.SetBinding(this, ChartSegment.StrokeProperty, binding);

            binding = new Binding();
            binding.Source = Series;
            binding.Path = new PropertyPath("StrokeThickness");
            BindingOperations.SetBinding(this, ChartSegment.StrokeThicknessProperty, binding);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents chart adornment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>

    public partial class ChartPieAdornment : ChartAdornment
    {
        #region Fields

        #region Private Fields

        double angle;

        double radius;

        int pieIndex;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for ChartPieAdornment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <param name="series"></param>
        public ChartPieAdornment(double xVal, double yVal, double angle, double radius, AdornmentSeries series)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Angle property 
        /// </summary>
        public double Angle
        {
            get
            {
                return angle;
            }
            internal set
            {
                angle = value;
                OnPropertyChanged("Angle");
            }
        }

        /// <summary>
        /// Gets or sets Radius property
        /// </summary>
        public double Radius
        {
            get
            {
                return radius;
            }
            internal set
            {
                radius = value;
                OnPropertyChanged("Radius");
            }
        }

        internal double InnerDoughnutRadius
        {
            get; set;
        }

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        public override void SetData(params double[] Values)
        {
            XPos = XData = Values[0];
            YPos = YData = Values[1];
            Angle = Values[2];
            Radius = Values[3];
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            double radius = Radius;
            double actualRadius = Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
            if (Series is PieSeries)
            {
                var hostSeries = Series as PieSeries;
                double pieSeriesCount = hostSeries.GetPieSeriesCount();
                double equalParts = actualRadius / pieSeriesCount;
                double innerRadius = equalParts * pieIndex;

                Point center;
                if (pieSeriesCount == 1)
                    center = hostSeries.Center;
                else
                    center = ChartLayoutUtils.GetCenter(transformer.Viewport);

                if (hostSeries != null && hostSeries.LabelPosition == CircularSeriesLabelPosition.Inside)
                {
                    if (pieIndex > 0)
                    {
                        double difference = (radius - innerRadius) / 2;
                        radius = radius - difference;
                    }
                    else
                    {
                        radius = radius / 2;
                    }
                }

                this.X = center.X + radius * Math.Cos(Angle);
                this.Y = center.Y + radius * Math.Sin(Angle);
            }
            else if (Series is DoughnutSeries)
            {
                var hostSeries = Series as DoughnutSeries;

                actualRadius *= hostSeries.InternalDoughnutSize;

                Point center;
                double remainingWidth = 0d, equalParts = 0d, innerRadius = 0d;
                var adornmentAngle = Angle;

                if (hostSeries.IsStackedDoughnut)
                {
                    var adornmentIndex = Series.Adornments.IndexOf(this);
                    var doughnutSegment = hostSeries.Segments[adornmentIndex] as DoughnutSegment;
                    int doughnutSegmentsCount = hostSeries.Segments.Count;
                    center = hostSeries.Center;
                    remainingWidth = actualRadius - (actualRadius * Series.ActualArea.InternalDoughnutHoleSize);
                    equalParts = (remainingWidth / doughnutSegmentsCount) * hostSeries.InternalDoughnutCoefficient;
                    radius = actualRadius - (equalParts * (doughnutSegmentsCount - (doughnutSegment.DoughnutSegmentIndex + 1)));
                    InnerDoughnutRadius = innerRadius = radius - equalParts;
                    radius = radius - equalParts * hostSeries.SegmentSpacing;
                    Radius = radius;
                    innerRadius = ChartMath.MaxZero(innerRadius);
                    
                    double difference = (radius - innerRadius) / 2;
                    radius = radius - difference;

                    adornmentAngle = hostSeries.LabelPosition == CircularSeriesLabelPosition.Outside ? doughnutSegment.StartAngle : hostSeries.LabelPosition == CircularSeriesLabelPosition.OutsideExtended ? doughnutSegment.EndAngle : Angle;
                }
                else
                {
                    int doughnutSeriesCount = hostSeries.GetDoughnutSeriesCount();
                    center = doughnutSeriesCount == 1 ? hostSeries.Center : ChartLayoutUtils.GetCenter(transformer.Viewport);
                    remainingWidth = actualRadius - (actualRadius * Series.ActualArea.InternalDoughnutHoleSize);
                    equalParts = remainingWidth / doughnutSeriesCount;
                    InnerDoughnutRadius = innerRadius = radius - (equalParts * hostSeries.InternalDoughnutCoefficient);
                    innerRadius = ChartMath.MaxZero(innerRadius);

                    if (hostSeries != null && hostSeries.LabelPosition == CircularSeriesLabelPosition.Inside)
                    {
                        double difference = (radius - innerRadius) / 2;
                        radius = radius - difference;
                    }
                }
                
                this.X = center.X + radius * Math.Cos(adornmentAngle);
                this.Y = center.Y + radius * Math.Sin(adornmentAngle);
            }
        }

        #endregion

        #region Internal Methods

        internal void SetValues(double xVal, double yVal, double angle, double radius, AdornmentSeries series)
        {
            XPos = XData = xVal;
            YPos = YData = yVal;
            Radius = radius;
            Angle = angle;
            Series = base.Series = series;
            pieIndex = (from pieSeries in Series.ActualArea.VisibleSeries where pieSeries is CircularSeriesBase select pieSeries).ToList().IndexOf(series);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for triangularAdornments
    /// </summary>
    public partial class TriangularAdornment : ChartAdornment
    {
        #region Fields

        #region Private Fields

        double CurrY, Height;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for TriangularAdornment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="currY"></param>
        /// <param name="height"></param>
        /// <param name="series"></param>

        public TriangularAdornment(double xVal, double yVal, double currY, double height, AdornmentSeries series)
        {
            XPos = XData = xVal;
            YPos = YData = yVal;
            CurrY = currY;
            Height = height;
            Series = base.Series = series;
        }

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        public override void SetData(params double[] Values)
        {
            XPos = XData = Values[0];
            YPos = YData = Values[1];
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>

        public override void Update(IChartTransformer transformer)
        {
            double bottom;
            Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
            bottom = center.Y;
            center.Y = 0;
            center.Y += ((CurrY * bottom) * 2) - (Height / 2) * 4;
            this.X = center.X;
            this.Y = center.Y;
        }
        #endregion

        #endregion
    }

}
