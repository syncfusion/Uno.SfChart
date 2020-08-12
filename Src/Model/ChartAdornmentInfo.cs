using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
using WindowsPolyLineSegment = Windows.UI.Xaml.Media.PolyLineSegment;
using Windows.UI.Text;
using Windows.UI;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the class used for configuring chart adornments for chart.
    /// </summary>
    public abstract partial class ChartAdornmentInfoBase : DependencyObject, ICloneable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="LabelRotationAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelRotationAngleProperty =
            DependencyProperty.Register(
                "LabelRotationAngle", 
                typeof(double), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(0d, new PropertyChangedCallback(OnLabelRotationAngleChanged)));

        /// <summary>
        ///  The DependencyProperty for <see cref="Background"/> property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                "Background", 
                typeof(Brush), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(null, OnColorPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="BorderThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness",
                typeof(Thickness), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(new Thickness(0), OnDefaultAdornmentChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="BorderBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                "BorderBrush", 
                typeof(Brush), 
                typeof(ChartAdornmentInfoBase),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnStylingPropertyChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnStylingPropertyChanged));
#endif

        /// <summary>
        /// The DependencyProperty for <see cref="Margin"/> property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(
                "Margin", 
                typeof(Thickness),
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(new Thickness(5), OnStylingPropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="FontStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register(
                "FontStyle", 
                typeof(FontStyle), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(
                    TextBlock.FontStyleProperty.GetMetadata(typeof(TextBlock)).DefaultValue, 
                    OnFontStylePropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="FontSize"/> property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                "FontSize", 
                typeof(double),
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(
                    TextBlock.FontSizeProperty.GetMetadata(typeof(TextBlock)).DefaultValue,
                    OnStylingPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="Foreground"/> property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground",
                typeof(Brush), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(null, OnLabelsPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="UseSeriesPalette"/> property.
        /// </summary>
        public static readonly DependencyProperty UseSeriesPaletteProperty =
            DependencyProperty.Register(
                "UseSeriesPalette",
                typeof(bool), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(false, OnDefaultAdornmentChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="LabelPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register(
                "LabelPosition", 
                typeof(AdornmentsLabelPosition),
                typeof(ChartAdornmentInfoBase), 
                new PropertyMetadata(
                    AdornmentsLabelPosition.Default,
                    OnAdornmentPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="HighlightOnSelection"/> property.
        /// </summary>
        public static readonly DependencyProperty HighlightOnSelectionProperty =
            DependencyProperty.Register(
                "HighlightOnSelection", 
                typeof(bool), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(false, OnHighlightOnSelectionChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
          DependencyProperty.Register(
              "HorizontalAlignment",
              typeof(HorizontalAlignment),
              typeof(ChartAdornmentInfoBase), 
              new PropertyMetadata(
                  HorizontalAlignment.Center,
                  OnAdornmentPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="VerticalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
         DependencyProperty.Register(
             "VerticalAlignment", 
             typeof(VerticalAlignment),
             typeof(ChartAdornmentInfoBase), 
             new PropertyMetadata(
                 VerticalAlignment.Center,
                 OnAdornmentPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ConnectorHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty ConnectorHeightProperty =
            DependencyProperty.Register(
                "ConnectorHeight",
                typeof(double), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(0d, OnAdornmentPositionChanged)); //WPF-14304 ConnectorRotationAngle and connectorHeight properties not working dynamically. 
        
        /// <summary>
        /// The DependencyProperty for <see cref=" ConnectorRotationAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty ConnectorRotationAngleProperty =
            DependencyProperty.Register(
                "ConnectorRotationAngle", 
                typeof(double), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(double.NaN, OnAdornmentPositionChanged)); //WPF-14304 ConnectorRotationAngle and connectorHeight properties not working dynamically. 
        
        /// <summary>
        ///  The DependencyProperty for <see cref="ConnectorLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty ConnectorLineStyleProperty =
            DependencyProperty.Register(
                "ConnectorLineStyle", 
                typeof(Style),
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(null, OnShowConnectingLine));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="ShowConnectorLine"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowConnectorLineProperty =
            DependencyProperty.Register(
                "ShowConnectorLine",
                typeof(bool), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(false, OnShowConnectingLine));
        
        /// <summary>
        /// The DependencyProperty for <see cref="LabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register(
                "LabelTemplate",
                typeof(DataTemplate), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(null, OnLabelChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="Symbol"/> property.
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(
                "Symbol", 
                typeof(ChartSymbol), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(ChartSymbol.Custom, OnSymbolTypeChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="SymbolWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty SymbolWidthProperty =
            DependencyProperty.Register(
                "SymbolWidth", 
                typeof(double), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(20d, OnSymbolPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="SymbolHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty SymbolHeightProperty =
            DependencyProperty.Register(
                "SymbolHeight", 
                typeof(double),
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(20d, OnSymbolPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="SymbolTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty SymbolTemplateProperty =
            DependencyProperty.Register(
                "SymbolTemplate", 
                typeof(DataTemplate), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(null, OnSymbolPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SymbolInterior"/> property.
        /// </summary>
        public static readonly DependencyProperty SymbolInteriorProperty =
            DependencyProperty.Register(
                "SymbolInterior", 
                typeof(Brush),
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(null, OnSymbolInteriorChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="SymbolStroke"/> property.
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeProperty =
            DependencyProperty.Register(
                "SymbolStroke", 
                typeof(Brush), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="FontFamily"/> property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty
               = DependencyProperty.Register(
                    "FontFamily",
                    typeof(FontFamily), 
                    typeof(ChartAdornmentInfoBase),
#if UNIVERSALWINDOWS
                    new PropertyMetadata(null, OnStylingPropertyChanged));
#else
                    new PropertyMetadata(TextBlock.FontFamilyProperty.GetMetadata(typeof(TextBlock)).DefaultValue, OnStylingPropertyChanged));
#endif

        /// <summary>
        ///  The DependencyProperty for <see cref="AdornmentsPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty AdornmentsPositionProperty =
            DependencyProperty.Register(
                "AdornmentsPosition", 
                typeof(AdornmentsPosition),
                typeof(ChartAdornmentInfoBase), 
                new PropertyMetadata(AdornmentsPosition.Top, OnAdornmentPositionChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentLabelContent"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelContentProperty =
            DependencyProperty.Register(
                "SegmentLabelContent",
                typeof(LabelContent),
                typeof(ChartAdornmentInfoBase), 
                new PropertyMetadata(LabelContent.YValue, OnLabelChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentLabelFormat"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelFormatProperty =
            DependencyProperty.Register(
                "SegmentLabelFormat", 
                typeof(string), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(string.Empty, OnLabelChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ShowMarker"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowMarkerProperty =
            DependencyProperty.Register(
                "ShowMarker", 
                typeof(bool),
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(true, OnShowMarker));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ShowLabel"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLabelProperty =
            DependencyProperty.Register(
                "ShowLabel", 
                typeof(bool), 
                typeof(ChartAdornmentInfoBase),
                new PropertyMetadata(false, OnLabelChanged));
        
        #endregion

        #region Fields
        
        #region Internal Fields

        internal UIElementsRecycler<ChartAdornmentContainer> adormentContainers;

        internal ChartSeriesBase series;

        #endregion

        #region Private Fields

        private double labelPadding = 3;
        
        private double offsetX = 0;
        
        private double offsetY = 0;

        #endregion

        #endregion

        #region Constructor

        public ChartAdornmentInfoBase()
        {
#if UNIVERSALWINDOWS
            BorderBrush = new SolidColorBrush(Colors.Transparent);
            FontFamily = TextBlock.FontFamilyProperty.GetMetadata(typeof(TextBlock)).DefaultValue as FontFamily;
#endif
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the rotation angle to the adornment label content.
        /// </summary>
        /// <remarks>
        /// The default value is 0 and the label will be rotated with center as origin.
        /// </remarks>
        public double LabelRotationAngle
        {
            get { return (double)GetValue(LabelRotationAngleProperty); }
            set { SetValue(LabelRotationAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background brush for the adornment label.
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
        /// Gets or sets the outer border thickness of the label.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }
        /// <summary>
        /// Gets or sets the brush that draws the outer border stroke brush. 
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
        /// Gets or sets the top-level font style for the label.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.FontStyle"/>
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
        /// Gets or sets foreground brush to apply to the label content.
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
        /// Gets or sets a value indicating whether the adornment should reflect the series interior.
        /// </summary>
        /// <value>
        /// if its <c>true</c>, the data point color will be applied for the respective adornments. 
        /// By default, its <c>false</c>.
        /// </value>
        public bool UseSeriesPalette
        {
            get { return (bool)GetValue(UseSeriesPaletteProperty); }
            set { SetValue(UseSeriesPaletteProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label position for the adornment.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.AdornmentsLabelPosition"/>
        /// </value>
        /// <remarks>
        /// By defining its value as <c>AdornmentsLabelPosition.Auto</c>, the adornment label will position smartly based 
        /// on type of the series.
        /// </remarks>
        public AdornmentsLabelPosition LabelPosition
        {
            get { return (AdornmentsLabelPosition)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the selection for the adornments.
        /// </summary>
        /// <value>
        /// if its <c>true</c>, we can selected the data point by selecting adornments.
        /// </value>
        /// <remarks>
        /// This feature will be useful for the continuous series like FastLine, Area, etc.
        /// </remarks>
        public bool HighlightOnSelection
        {
            get { return (bool)GetValue(HighlightOnSelectionProperty); }
            set { SetValue(HighlightOnSelectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment for the label.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.HorizontalAlignment"/>
        /// </value>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical alignment for the label.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.VerticalAlignment"/>
        /// </value>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the connector line height of the adornment.
        /// </summary>
        public double ConnectorHeight
        {
            get { return (double)GetValue(ConnectorHeightProperty); }
            set { SetValue(ConnectorHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rotation angle for the connector line.
        /// </summary>
        public double ConnectorRotationAngle
        {
            get { return (double)GetValue(ConnectorRotationAngleProperty); }
            set { SetValue(ConnectorRotationAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the connector line style.
        /// </summary>
        /// <remarks>
        /// <see cref="System.Windows.Style"/>
        /// </remarks>
        public Style ConnectorLineStyle
        {
            get { return (Style)GetValue(ConnectorLineStyleProperty); }
            set { SetValue(ConnectorLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show or hide the connector line.
        /// </summary>
        public bool ShowConnectorLine
        {
            get { return (bool)GetValue(ShowConnectorLineProperty); }
            set { SetValue(ShowConnectorLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the adornment label.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of symbol to be displayed as adornment.
        /// </summary>
        /// <remarks>
        /// By default, symbol will not be displayed. We need to define the required shape.
        /// </remarks>
        /// <value>
        /// The value can be Circle, Rectangle, etc. See <see cref="Syncfusion.UI.Xaml.Charts.ChartSymbol"/>.
        /// </value>
        public ChartSymbol Symbol
        {
            get { return (ChartSymbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the adornment symbol.
        /// </summary>
        /// <remarks>
        /// The default value is 20.
        /// </remarks>
        public double SymbolWidth
        {
            get { return (double)GetValue(SymbolWidthProperty); }
            set { SetValue(SymbolWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the adornment symbol.
        /// </summary>
        /// <remarks>
        /// The default value is 20.
        /// </remarks>
        public double SymbolHeight
        {
            get { return (double)GetValue(SymbolHeightProperty); }
            set { SetValue(SymbolHeightProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the custom template for the adornment symbol.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate SymbolTemplate
        {
            get { return (DataTemplate)GetValue(SymbolTemplateProperty); }
            set { SetValue(SymbolTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background of the adornment symbol.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush SymbolInterior
        {
            get { return (Brush)GetValue(SymbolInteriorProperty); }
            set { SetValue(SymbolInteriorProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the stroke of the adornment symbol.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush SymbolStroke
        {
            get { return (Brush)GetValue(SymbolStrokeProperty); }
            set { SetValue(SymbolStrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the adornment label font family.
        /// </summary>
        /// <remarks>
        /// Identifies font family that should be used to display adornment's text.
        /// </remarks>
        /// <value>
        /// <see cref="System.Windows.Media.FontFamily"/>
        /// </value>
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)GetValue(FontFamilyProperty);
            }
            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }
        
        /// <summary>
        /// Gets the associated series of this adornment.
        /// </summary>
        public ChartSeriesBase Series
        {
            get
            {
                return series;
            }
            internal set
            {
                series = value;
                if (series != null)
                {
                    if (adormentContainers != null)
                    {
                        adormentContainers.GenerateElements(series.Adornments.Count);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the adornments.
        /// </summary>
        /// <value>
        /// <c>AdornmentsPosition.Top</c> 
        /// <c>AdornmentsPosition.Bottom</c>
        /// <c>AdornmentsPosition.TopAndBottom</c>
        /// </value>
        public AdornmentsPosition AdornmentsPosition
        {
            get { return (AdornmentsPosition)GetValue(AdornmentsPositionProperty); }
            set { SetValue(AdornmentsPositionProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the actual label content to be displayed as adornment.
        /// </summary>
        /// <remarks>
        /// This property is used to define the value to be displayed in adornment label 
        /// like x value or any other value from underlying model object.
        /// </remarks>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.LabelContent"/>
        /// </value>
        public LabelContent SegmentLabelContent
        {
            get { return (LabelContent)GetValue(SegmentLabelContentProperty); }
            set { SetValue(SegmentLabelContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the string formatting for the adornment labels.
        /// </summary>
        public string SegmentLabelFormat
        {
            get { return (string)GetValue(SegmentLabelFormatProperty); }
            set { SetValue(SegmentLabelFormatProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to show or hide the marker symbol.
        /// </summary>
        public bool ShowMarker
        {
            get { return (bool)GetValue(ShowMarkerProperty); }
            set { SetValue(ShowMarkerProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to show or hide the adornment label.
        /// </summary>
        public bool ShowLabel
        {
            get { return (bool)GetValue(ShowLabelProperty); }
            set { SetValue(ShowLabelProperty, value); }
        }

        #endregion

        #region Internal Properties

        internal UIElementsRecycler<FrameworkElement> LabelPresenters { get; set; }

        internal UIElementsRecycler<Path> ConnectorLines { get; set; }

        internal Size AdornmentInfoSize { get; set; }

        internal bool IsStraightConnectorLine2D { get; set; }

        internal bool ShowMarkerAtEdge2D { get; set; }

        /// <summary>
        /// Gets a value indicating whether to generate the adornment containers.
        /// </summary>
        internal bool IsMarkerRequired
        {
            get
            {
                return (ShowMarker && ((Symbol == ChartSymbol.Custom && SymbolTemplate != null)
                                      || (Symbol != ChartSymbol.Custom)));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the adornment label is rendered with textblock.
        /// </summary>
        internal bool IsTextRequired
        {
            get
            {
                return Background == null && BorderThickness == new Thickness(0)
                       && !UseSeriesPalette && LabelTemplate == null;
            }
        }

        internal double LabelPadding
        {
            get { return labelPadding; }
            set
            {
                labelPadding = value;
                OnAdornmentPropertyChanged();
            }
        }

        internal double OffsetX
        {
            get { return offsetX; }
            set
            {
                offsetX = value;
                OnAdornmentPropertyChanged();
            }
        }

        internal double OffsetY
        {
            get { return offsetY; }
            set
            {
                offsetY = value;
                OnAdornmentPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        internal void Dispose()
        {
            if (LabelPresenters != null && LabelPresenters.Count > 0 && LabelPresenters[0] is ContentControl)
            {
                foreach (var label in LabelPresenters)
                {
                    label.SetValue(ContentControl.ContentProperty, null);
                }

                LabelPresenters.Clear();
            }

            if (adormentContainers != null)
            {
                foreach (var adornmentContainer in adormentContainers)
                {
                    adornmentContainer.Dispose();
                }

                adormentContainers.Clear();
            }

            Series = null;
        }

        public DependencyObject Clone()
        {
            return CloneAdornmentInfo();
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Gets the bezier approximation.
        /// </summary>
        /// <param name="controlPoints">The control points.</param>
        /// <param name="outputSegmentCount">The output segment count.</param>
        /// <returns></returns>
        internal static List<Point> GetBezierApproximation(IList<Point> controlPoints, int outputSegmentCount)
        {
            var points = new List<Point>();
            for (var i = 0; i <= outputSegmentCount; i++)
            {
                var t = (double)i / outputSegmentCount;
                points.Add(GetBezierPoint(t, controlPoints, 0, controlPoints.Count));
            }
            return points;
        }

        /// <summary>
        /// Aligns the element.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="verticalAlignment">The vertical alignment.</param>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        internal static void AlignElement(
            FrameworkElement control,
            ChartAlignment verticalAlignment,
            ChartAlignment horizontalAlignment,
            double x,
            double y)
        {
            if (horizontalAlignment == ChartAlignment.Near)
            {
                x = x - control.DesiredSize.Width;
            }
            else if (horizontalAlignment == ChartAlignment.Center)
            {
                x = x - control.DesiredSize.Width / 2;
            }

            if (verticalAlignment == ChartAlignment.Near)
            {
                y = y - control.DesiredSize.Height;
            }
            else if (verticalAlignment == ChartAlignment.Center)
            {
                y = y - control.DesiredSize.Height / 2;
            }

            Canvas.SetLeft(control, x);
            Canvas.SetTop(control, y);
        }

        #endregion

        #region Internal Virtual Methods

        internal virtual void Arrange(Size finalSize)
        {

        }

        internal virtual DependencyObject CloneAdornmentInfo()
        {
            return null;
        }

        #endregion

        #region Internal Methods

        internal void UpdateLabels()
        {
            if (ShowLabel && LabelPresenters != null && series != null)
            {
                RotateTransform rotate = new RotateTransform();
                Binding lblRotationBinding = new Binding() { Source = this, Path = new PropertyPath("LabelRotationAngle"), Mode = BindingMode.TwoWay };
                var labelTemplate = LabelTemplate ?? (UseSeriesPalette ? ChartDictionaries.GenericCommonDictionary["AdornmentLabelTemplate"] as DataTemplate : ChartDictionaries.GenericCommonDictionary["AdornmentDefaultLabelTemplate"] as DataTemplate);

                Style textStyle = new Style();
                var isTextRequired = IsTextRequired;
                if (isTextRequired)
                {
                    textStyle.TargetType = typeof(TextBlock);
                    textStyle.Setters.Add(new Setter() { Property = TextBlock.FontStyleProperty, Value = FontStyle });
                    textStyle.Setters.Add(new Setter() { Property = TextBlock.FontFamilyProperty, Value = FontFamily });
                    textStyle.Setters.Add(new Setter() { Property = TextBlock.FontSizeProperty, Value = FontSize });
                    textStyle.Setters.Add(new Setter() { Property = TextBlock.MarginProperty, Value = Margin });
                    if (this.Foreground != null)
                        textStyle.Setters.Add(new Setter() { Property = TextBlock.ForegroundProperty, Value = Foreground });
                    if (LabelPresenters.Count > 0 && LabelPresenters[0] is ContentControl)
                        LabelPresenters.Clear();
                    LabelPresenters.GenerateElementsOfType(series.Adornments.Count, typeof(TextBlock));
                }
                else
                {
                    if (LabelPresenters.Count > 0 && LabelPresenters[0] is TextBlock)
                        LabelPresenters.Clear();

                    LabelPresenters.GenerateElementsOfType(series.Adornments.Count, typeof(ContentControl));
                }

                for (var i = 0; i < LabelPresenters.Count; i++)
                {
                    var label = LabelPresenters[i];
                    var adornment = Series.Adornments[i];
                    var adornmentItem = adornment.Item;

                    if (label.Visibility == Visibility.Collapsed)
                        label.Visibility = Visibility.Visible; // WRT-5653 - Adornment label is not positioned properly

                    //UWP - 5829 - Fix for the label irregular positioning in dynamic loading of series.
                    if (!this.Series.ActualArea.IsChartLoaded)
                        label.Visibility = Visibility.Collapsed;

                    if(series is CircularSeriesBase && !double.IsNaN(((CircularSeriesBase)series).GroupTo))
                    {
                        label.Tag = i;
                    }
                    else if (series.ActualXAxis is CategoryAxis && !(series.ActualXAxis as CategoryAxis).IsIndexed
                       && (Series.IsSideBySide && (!(Series is RangeSeriesBase)) && (!(Series is FinancialSeriesBase))
                       && !(Series is WaterfallSeries)))
                        label.Tag = series.GroupedActualData.IndexOf(adornmentItem);
                    else
                        label.Tag = Series.ActualData.IndexOf(adornmentItem);

                    if (isTextRequired)
                    {
                        label.Style = textStyle;
                        var text = label as TextBlock;
                        text.Text = adornment.GetTextContent().ToString();
                        text.IsHitTestVisible = false;
                    }
                    else
                    {
                        var binding = CreateAdormentBinding("ActualContent", Series.Adornments[i]);
                        label.SetBinding(ContentControl.ContentProperty, binding);

                        if (LabelTemplate == null)//WPF-14307-Adornmentinfo SegmentLabelContent and SegmentLabelFormat properties not working dynamically, while setting UseSeriesPalette as true. 
                        {
                            label.ClearValue(ContentControl.ContentTemplateProperty);
                        }

                        (label as ContentControl).ContentTemplate = labelTemplate;
                    }

                    var transformGroup = new TransformGroup();
                    label.RenderTransform = transformGroup;

                    if (LabelRotationAngle != 0)
                    {
                        BindingOperations.SetBinding(rotate, RotateTransform.AngleProperty, lblRotationBinding);
                        label.RenderTransformOrigin = new Point(0.5, 0.5);
                        transformGroup.Children.Add(rotate);
                    }
                }
            }
            else if (LabelPresenters != null)
            {
                LabelPresenters.GenerateElements(0);
            }
        }

        /// <summary>
        /// Updates the adornment connecting lines.
        /// </summary>
        internal void UpdateConnectingLines()
        {
            if (ShowConnectorLine && ConnectorLines != null && series != null)
            {
                ConnectorLines.GenerateElements(Series.Adornments.Count);
                var circularseries = series as CircularSeriesBase;

                for (var i = 0; i < ConnectorLines.Count; i++)
                {
                    if (double.IsNaN(Series.Adornments[i].YData))
                        ConnectorLines[i].Visibility = Visibility.Collapsed;
                    else
                    {
                        var connectorStyle = ConnectorLineStyle ?? ChartDictionaries.GenericCommonDictionary["pathStyle"] as Style;
                        int selectedSegmentIndex = circularseries != null && !double.IsNaN(circularseries.GroupTo) ? series.Segments.IndexOf(series.Segments[i]) : series.ActualData.IndexOf(series.Adornments[i].Item);
                        ConnectorLines[i].Visibility = Visibility.Visible;

                        //Checking SeriesSelection behavior and setting SeriesSelectionBrush property. 
                        if (series.ActualArea.SelectedSeriesCollection.Contains(series)
                        && series.ActualArea.GetSeriesSelectionBrush(series) != null && series.adornmentInfo.HighlightOnSelection
                        && Series.ActualArea.GetEnableSeriesSelection() && series is ChartSeries)
                        {
                            ConnectorLines[i].Stroke = series.ActualArea.GetSeriesSelectionBrush(series);
                        }
                        //Checking SegmentSelection behavior and setting SegmentSelectionBrush property. 
                        else if (series.SelectedSegmentsIndexes.Contains(selectedSegmentIndex)
                            && series is ISegmentSelectable && series.adornmentInfo.HighlightOnSelection
                            && (series as ISegmentSelectable).SegmentSelectionBrush != null
                            && Series.ActualArea.GetEnableSegmentSelection())
                        {
                            ConnectorLines[i].Stroke = (series as ISegmentSelectable).SegmentSelectionBrush;
                        }
                        //Setting connector line stroke.
                        else if (UseSeriesPalette && !CheckStrokeAppliedInStyle(ConnectorLineStyle))
                        {
                            var binding = new Binding
                            {
                                Source = series.Adornments[i],
                                Path = new PropertyPath("Interior")
                            };
                            ConnectorLines[i].SetBinding(Shape.StrokeProperty, binding);
                        }
                        else
                            ConnectorLines[i].ClearValue(Shape.StrokeProperty);

                        ConnectorLines[i].Style = connectorStyle;
                    }
                }

                //StrokeDashArray applied only for the first element when it is applied through style. 
                //It is bug in the framework.
                //And hence manually setting stroke dash array for each and every connector line.
                if (ConnectorLines.Count > 0)
                {
                    DoubleCollection collection = ConnectorLines[0].StrokeDashArray;
                    if (collection != null && collection.Count > 0)
                    {
                        foreach (Path path in ConnectorLines)
                        {
                            DoubleCollection doubleCollection = new DoubleCollection();
                            foreach (double value in collection)
                            {
                                doubleCollection.Add(value);
                            }
                            path.StrokeDashArray = doubleCollection;
                        }
                    }
                }
            }
            else if (ConnectorLines != null)
            {
                ConnectorLines.GenerateElements(0);
            }
        }

        /// <summary>
        /// Adornment element's properties have updated.
        /// </summary>
        internal void UpdateElements()
        {
            UpdateAdornments();
            UpdateLabels();
            UpdateConnectingLines();
        }

        internal void Measure(Size availableSize, Panel panel)
        {
            if (LabelPresenters == null && panel != null)
                LabelPresenters = new UIElementsRecycler<FrameworkElement>(panel);

            if (ConnectorLines == null && panel != null)
                ConnectorLines = new UIElementsRecycler<Path>(panel);

            if (adormentContainers == null && panel != null)
                adormentContainers = new UIElementsRecycler<ChartAdornmentContainer>(panel);

            int adornmentIndex = 0;
            if (this.adormentContainers != null)
                foreach (ChartAdornmentContainer element in this.adormentContainers)
                {
                    element.Adornment = series.Adornments[adornmentIndex];
                    element.Measure(availableSize);
                    adornmentIndex++;
                }

            int i = 0;
            if (ShowLabel)
            {
                for (; i < this.LabelPresenters.Count; i++)
                {
                    var label = this.LabelPresenters[i];

                    // UWP-5829 - Fix for the label irregular positioning in dynamic loading of series.
                    if (this.Series.ActualArea.IsChartLoaded && label.Visibility == Visibility.Collapsed)
                        label.Visibility = Visibility.Visible;

                    label.Measure(availableSize);
                    Canvas.SetZIndex(label, 4);

                    if (double.IsNaN(Series.Adornments[i].YData))
                        label.Visibility = Visibility.Collapsed;
                    else
                        label.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Updates the spider labels.
        /// </summary>
        /// <param name="pieLeft">The pie left.</param>
        /// <param name="pieRight">The pie right.</param>
        /// <param name="finalSize"></param>
        internal void UpdateSpiderLabels(double pieLeft, double pieRight, Size finalSize, double radius)
        {
            var orderedAdornments = GetOrderedAdornments();

            var previousRectColl = new List<Rect>();
            var previousRect = new Rect();

            var coefficient =  series is PieSeries ? ((PieSeries)series).InternalPieCoefficient : ((DoughnutSeries)series).InternalDoughnutCoefficient;
            var center = (series as CircularSeriesBase).Center;
            double baseRight = pieRight, baseLeft = pieLeft;

            pieLeft = (finalSize.Width / 2 - radius) - radius * 0.5;
            pieRight = (finalSize.Width / 2 + radius) + radius * 0.5;

            var connectorHeight = radius * 0.2;

            pieRight = pieRight > baseRight ? baseRight : pieRight;
            pieLeft = pieLeft < baseLeft ? baseLeft : pieLeft;

            double explodRadius =0, angle;
            ConnectorMode connectorMode;
            int explodeIndex = -1;
            var circularSeriesBase = series as CircularSeriesBase;
            if (circularSeriesBase != null)
            {
                explodeIndex = circularSeriesBase.ExplodeAll ? -2 : circularSeriesBase.ExplodeIndex;
                explodRadius = circularSeriesBase.ExplodeRadius;
                connectorMode = circularSeriesBase.ConnectorType;
            }

            for (var i = 0; i < orderedAdornments.Count(); i++)
            {
                var renderingPoints = new List<Point>();
                ChartAdornment adornment;
                int adornmentIndex;
                adornment = orderedAdornments[i];
                adornmentIndex = series.Adornments.IndexOf(adornment);

                if (ConnectorLines.Count > adornmentIndex)
                {
                    if (adornment.CanHideLabel)
                    {
                        ConnectorLines[adornmentIndex].Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ConnectorLines[adornmentIndex].Visibility = Visibility.Visible;
                    }
                }

                double explodedRadius = adornmentIndex == explodeIndex ? explodRadius : explodeIndex == -2 ? explodRadius : 0d;
                angle = adornment.ConnectorRotationAngle;
                var label = LabelPresenters[adornmentIndex];
                var x = center.X + (Math.Cos(angle) * radius);
                var y = center.Y + (Math.Sin(angle) * radius);
                if (IsStraightConnectorLine2D)
                {
                    renderingPoints.Add((new Point(x, y)));
                }
                else
                {
                    x = x + (Math.Cos(angle) * (explodedRadius - (radius * coefficient) / 10));
                    y = y + (Math.Sin(angle) * (explodedRadius - (radius * coefficient) / 10));
                    renderingPoints.Add((new Point(x, y)));
                    x = x + (Math.Cos(angle) * connectorHeight);
                    y = y + (Math.Sin(angle) * connectorHeight);

                    renderingPoints.Add(new Point(x, y));
                }

                var pieAngle = angle % (Math.PI * 2);
                var isLeft = pieAngle > 1.57 && pieAngle < 4.71;
                if (label != null)
                {
                    double connectorLineEdge;
                    if (isLeft)
                    {
                        x = pieLeft - (label.DesiredSize.Width / 2);
                        connectorLineEdge = +label.DesiredSize.Width / 2;
                    }
                    else
                    {
                        x = pieRight + (label.DesiredSize.Width / 2);
                        connectorLineEdge = -label.DesiredSize.Width / 2;
                    }
                    var distanceFromOrigin = (Math.Sqrt(Math.Pow(adornment.X - x, 2) + Math.Pow(adornment.Y - y, 2))) / 10;
                    x = isLeft ? x + distanceFromOrigin : x - distanceFromOrigin;
                    var currRect = new Rect(x, y, label.DesiredSize.Width, label.DesiredSize.Height);

                    if (previousRectColl.IntersectWith(currRect))
                    {
                        renderingPoints.Add(isLeft
                            ? new Point(x + connectorHeight + connectorLineEdge, y)
                            : new Point(x - connectorHeight + connectorLineEdge, y));
                        y = previousRect.Bottom + 2;
                    }

                    var lineEdge = connectorLineEdge;
                    if (ShowMarkerAtEdge2D && ShowMarker && adormentContainers[adornmentIndex] != null)
                    {
                        x += lineEdge;
                        lineEdge = 0;
                        x += renderingPoints.Last().X < center.X ? SymbolWidth / 2 : -SymbolWidth / 2;
                    }

                    renderingPoints.Add(new Point(x + lineEdge, y));
                    currRect.Y = y;
                    previousRect = currRect;
                    previousRectColl.Add(currRect);
                }

                if (!ShowLabel) continue;
                var chartAdornmentInfo = this as ChartAdornmentInfo;
                if (chartAdornmentInfo != null)
                {
                    if (ShowMarker && ShowMarkerAtEdge2D && circularSeriesBase != null)
                    {
                        ChartAdornmentContainer adornmentSymbol = null;
                        if (adormentContainers != null && adormentContainers.Count > adornmentIndex)
                        {
                            adornmentSymbol = adormentContainers[adornmentIndex];
                        }

                        AlignStraightConnectorLineLabel(label, center, LabelPosition, adornmentSymbol, x, y, circularSeriesBase.EnableSmartLabels, circularSeriesBase.LabelPosition);
                    }
                    else
                    {
                        if (LabelPosition == AdornmentsLabelPosition.Default)
                            AlignElement(label, GetVerticalAlignment(VerticalAlignment), GetHorizontalAlignment(HorizontalAlignment), x, y);
                        else
                            chartAdornmentInfo.AlignAdornmentLabelPosition(label, LabelPosition, x + OffsetX, y + OffsetY, adornmentIndex);
                    }
                }

                if (ShowMarkerAtEdge2D && circularSeriesBase != null && ShowMarker && adormentContainers != null && adornmentIndex < adormentContainers.Count)
                {
                    SetSymbolPosition(new Point(renderingPoints.Last().X, renderingPoints.Last().Y), adormentContainers[adornmentIndex]);
                }
            }
        }

        /// <summary>
        /// Align label position for straight connector line for circular series.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="center"></param>
        /// <param name="adornmentsPosition"></param>
        /// <param name="adornmentSymbol"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="enableSmartLabel"></param>
        internal void AlignStraightConnectorLineLabel(FrameworkElement label, Point center, AdornmentsLabelPosition adornmentsPosition, ChartAdornmentContainer adornmentSymbol, double x, double y, bool enableSmartLabel, CircularSeriesLabelPosition labelPosition)
        {
            double adornmentSymbolWidth = 0d, adornmentSymbolHeight = 0d;
            if (adornmentSymbol != null && ShowMarker && ShowMarkerAtEdge2D)
            {
                if (SymbolTemplate == null)
                {
                    adornmentSymbolWidth = SymbolWidth;
                    adornmentSymbolHeight = SymbolHeight;
                }
                else
                {
                    adornmentSymbolWidth = adornmentSymbol.DesiredSize.Width;
                    adornmentSymbolHeight = adornmentSymbol.DesiredSize.Height;
                }
            }

            bool isRight = x > center.X;
            if (!enableSmartLabel)
            {
                if (adornmentsPosition == AdornmentsLabelPosition.Auto || adornmentsPosition == AdornmentsLabelPosition.Inner)
                {
                    x = isRight ? x - label.DesiredSize.Width - (adornmentSymbolWidth / 2) : x + (adornmentSymbolWidth / 2);
                    y = y - label.DesiredSize.Height;
                }
                else if (adornmentsPosition == AdornmentsLabelPosition.Outer || adornmentsPosition == AdornmentsLabelPosition.Default)
                {
                    x = isRight ? x + (adornmentSymbolWidth / 2) : x - (label.DesiredSize.Width + (adornmentSymbolWidth / 2));
                    y = y - label.DesiredSize.Height / 2;
                }
                else
                {
                    x = x - (label.DesiredSize.Width / 2);
                    y = y - label.DesiredSize.Height - adornmentSymbolHeight / 2;
                }
            }
            else if (enableSmartLabel && labelPosition == CircularSeriesLabelPosition.OutsideExtended)
            {
                x = isRight ? x + (adornmentSymbolWidth / 2) : x - (label.DesiredSize.Width + (adornmentSymbolWidth / 2));
                y = y - label.DesiredSize.Height / 2;
            }
            else
            {
                x = isRight ? x - ((label.DesiredSize.Width / 2) - (adornmentSymbolWidth / 2)) : x - ((label.DesiredSize.Width / 2) + (adornmentSymbolWidth / 2));
                y = y - label.DesiredSize.Height / 2;
            }

            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);
        }


        internal static void SetSymbolPosition(Point ConnectorEndPoint, ChartAdornmentContainer adornmentPresenter)
        {
            var adornmentRect = new Rect(new Point(), (adornmentPresenter.DesiredSize))
            {
                X = ConnectorEndPoint.X - adornmentPresenter.SymbolOffset.X,
                Y = ConnectorEndPoint.Y - adornmentPresenter.SymbolOffset.Y
            };

            Canvas.SetLeft(adornmentPresenter, adornmentRect.Left);
            Canvas.SetTop(adornmentPresenter, adornmentRect.Top);
        }

        /// <summary>
        /// Draws the connecotr line.
        /// </summary>
        /// <param name="connectorIndex">Index of the connector.</param>
        /// <param name="drawingPoints">The drawing points.</param>
        /// <param name="connectorLineMode">The connector line mode.</param>
        internal void DrawConnectorLine(int connectorIndex, List<Point> drawingPoints, ConnectorMode connectorLineMode, bool is3DChart, double depth)
        {
            if (ConnectorLines.Count <= connectorIndex) return;
            var element = ConnectorLines[connectorIndex];
            if (connectorLineMode == ConnectorMode.Bezier)
                drawingPoints = GetBezierApproximation(drawingPoints, 256);
            (this as ChartAdornmentInfo).DrawLineSegment(drawingPoints, element);
        }

        /// <summary>
        /// Panels the changed.
        /// </summary>
        /// <param name="panel">The panel.</param>
        internal void PanelChanged(Panel panel)
        {
            if (LabelPresenters == null)
                LabelPresenters = new UIElementsRecycler<FrameworkElement>(panel);

            if (ConnectorLines == null)
                ConnectorLines = new UIElementsRecycler<Path>(panel);

            if (adormentContainers == null)
                adormentContainers = new UIElementsRecycler<ChartAdornmentContainer>(panel);

            UpdateAdornments();
            UpdateLabels();
            UpdateConnectingLines();
        }

        internal void ClearChildren()
        {
            if (LabelPresenters != null)
            {
                LabelPresenters.Clear();
            }

            if (adormentContainers != null)
            {
                adormentContainers.Clear();
            }

            if (ConnectorLines != null)
                ConnectorLines.Clear();
        }

        internal void AddAdornment(UIElement element, Panel panel)
        {
            if (adormentContainers == null)
                adormentContainers = new UIElementsRecycler<ChartAdornmentContainer>(panel);
            adormentContainers.Add(element as ChartAdornmentContainer);
        }

        internal void RemoveAdornment(UIElement element)
        {
            adormentContainers.Remove(element as ChartAdornmentContainer);
        }

        /// <summary>
        /// Gets the adornment positions.
        /// </summary>
        /// <param name="pieRadius">The pie radius.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="finalSize">The final size.</param>
        /// <param name="adornment">The adornment.</param>
        /// <param name="labelIndex">Index of the label.</param>
        /// <param name="pieLeft">The pie left.</param>
        /// <param name="pieRight">The pie right.</param>
        /// <param name="label">The label.</param>
        /// <param name="series">The series.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        internal List<Point> GetAdornmentPositions(double pieRadius, IList<Rect> bounds, Size finalSize, ChartAdornment adornment, int labelIndex, double pieLeft, double pieRight, FrameworkElement label, ChartSeriesBase series, ref double x, ref double y, double angle, bool isPie)
        {
            var connectorHeight = ShowConnectorLine ? adornment.ConnectorHeight : 0d;
            var labelRadiusFromOrigin = connectorHeight;

            var drawingPoints = new List<Point> { new Point(x, y) };
            var center = new Point(x, y);
            //We need to do some rotation if the series is circular series
            if (isPie)
            {
                drawingPoints.Clear();
                double explodedRadius =-1;
                int explodeIndex =0;
                CircularSeriesLabelPosition labelPosition = CircularSeriesLabelPosition.Inside;
                bool enableSmartLabels = false;
                //Get the values like explode radius, index.., from pie series
                var circularSeriesBase = series as CircularSeriesBase;
                if (circularSeriesBase != null)
                {
                    explodedRadius = circularSeriesBase.ExplodeRadius;
                    explodeIndex = circularSeriesBase.ExplodeIndex;
                    labelPosition = circularSeriesBase.LabelPosition;
                    explodeIndex = circularSeriesBase.ExplodeAll ? -2 : explodeIndex;
                    enableSmartLabels = circularSeriesBase.EnableSmartLabels && ShowLabel && label != null;
                    double seriesCount = Series.ActualArea.VisibleSeries.Count;
                    if (seriesCount == 1)
                        center = circularSeriesBase.Center;
                    else
                        center = new Point(finalSize.Width / 2, finalSize.Height / 2);
                }
                else
                {

                }

                explodedRadius = explodeIndex == labelIndex || explodeIndex == -2 ? explodedRadius : 0d;
                labelRadiusFromOrigin = pieRadius / 2 + connectorHeight;
                if (labelPosition != CircularSeriesLabelPosition.Inside)
                {
                    labelRadiusFromOrigin = pieRadius + connectorHeight;
                    center.X = center.X + (Math.Cos((angle)) * explodedRadius);
                    center.Y = center.Y + (Math.Sin((angle)) * explodedRadius);

                    drawingPoints.Add(new Point(center.X + (Math.Cos((angle)) * pieRadius), center.Y + (Math.Sin((angle)) * pieRadius)));

                    if (labelPosition != CircularSeriesLabelPosition.OutsideExtended && !IsStraightConnectorLine2D)
                    {
                        x = center.X + (Math.Cos((angle)) * (labelRadiusFromOrigin));
                        y = center.Y + (Math.Sin((angle)) * (labelRadiusFromOrigin));

                        if (LabelPosition == AdornmentsLabelPosition.Auto)
                        {
                            x = (x < 0) ? 0 : (x > center.X * 2) ? center.X * 2 : x;
                            y = (y < 0) ? 0 : (y > center.Y * 2) ? center.Y * 2 : y;
                        }

                        drawingPoints.Add(new Point(x, y));
                    }

                    if (labelPosition == CircularSeriesLabelPosition.Outside && IsStraightConnectorLine2D)
                    {
                        x = GetStraightLineXPosition(center, angle, x, connectorHeight);
                        y = center.Y + (Math.Sin((angle)) * pieRadius);
                        drawingPoints.Add(new Point(x, y));
                    }
                }
                else
                {
                    x = x + (Math.Cos((angle)) * explodedRadius);
                    y = y + (Math.Sin((angle)) * explodedRadius);
                    drawingPoints.Add(new Point(x, y));
                    if (!IsStraightConnectorLine2D)
                    {
                        x = x + (Math.Cos((angle)) * connectorHeight);
                        y = y + (Math.Sin((angle)) * connectorHeight);

                        if (LabelPosition == AdornmentsLabelPosition.Auto)
                        {
                            x = (x < 0) ? 0 : (x > center.X * 2) ? center.X * 2 : x;
                            y = (y < 0) ? 0 : (y > center.Y * 2) ? center.Y * 2 : y;
                        }
                    }
                    else
                    {
                        x = GetStraightLineXPosition(center, angle, x, connectorHeight);
                    }

                    drawingPoints.Add(new Point(x, y));
                }
                //If the smart labels are enabled means we have to check for overlap else just place the labels in calculated positions
                if (enableSmartLabels)
                {
                    var currRect = new Rect(x, y, label.DesiredSize.Width, label.DesiredSize.Height);
                    switch (labelPosition)
                    {
                        case CircularSeriesLabelPosition.Inside:
                            {
                                var point = SmartLabelsForInside(adornment, bounds, label, connectorHeight, labelRadiusFromOrigin, pieRadius + explodedRadius, drawingPoints, center, currRect);
                                x = point.X;
                                y = point.Y;
                            }
                            break;
                        case CircularSeriesLabelPosition.Outside:
                            {
                                var point = SmartLabelsForOutside(bounds, drawingPoints, currRect, label, center, labelRadiusFromOrigin, connectorHeight, explodedRadius, adornment);
                                x = point.X;
                                y = point.Y;
                            }
                            break;
                    }
                }
                else if (labelPosition == CircularSeriesLabelPosition.OutsideExtended)
                {
                    //Calculation for default outside extended adornment position width
                    double baseRight = pieRight, baseLeft = pieLeft;
                    pieLeft = (finalSize.Width / 2 - pieRadius) - pieRadius;
                    pieRight = (finalSize.Width / 2 + pieRadius) + pieRadius;
                    if (!IsStraightConnectorLine2D)
                    {
                        x = center.X + (Math.Cos((angle)) * (pieRadius + pieRadius * 0.2));
                        y = center.Y + (Math.Sin((angle)) * (pieRadius + pieRadius * 0.2));
                        drawingPoints.Add(new Point(x, y));
                    }

                    var markerWidth = ShowMarkerAtEdge2D && this.ShowMarker ? SymbolWidth / 2 : 0d;
                    pieRight = pieRight > baseRight ? baseRight : pieRight;
                    pieLeft = pieLeft < baseLeft ? baseLeft : pieLeft;
                    var pieAngle = angle % (Math.PI * 2);
                    if ((pieAngle <= 1.57 && pieAngle >= 0) || pieAngle >= 4.71)
                    {
                        x = x < pieRight ? pieRight : x;
                    }
                    else
                    {
                        x = x > pieLeft ? pieLeft : x;
                    }

                    x = center.X < x ? x - markerWidth : x + markerWidth;
                    y = IsStraightConnectorLine2D ? center.Y + (Math.Sin((angle)) * pieRadius) : y;
                    drawingPoints.Add(new Point(x, y));
                }
            }
            else if (Series is TriangularSeriesBase)
            {
                x = x + (Math.Cos((angle)) * ConnectorHeight);
                y = y + (Math.Sin((angle)) * ConnectorHeight);

                if (LabelPosition == AdornmentsLabelPosition.Auto)
                {
                    x = (x < 0) ? 0 : (x > center.X * 2) ? center.X * 2 : x;
                    y = (y < 0) ? 0 : (y > center.Y * 2) ? center.Y * 2 : y;
                }
                drawingPoints.Add(new Point(x, y));
            }
            else
            {
                var newPoint = CalculateConnectorLinePoint(ref x, ref y, adornment, angle, labelIndex);
                drawingPoints.Add(newPoint);
            }
            return drawingPoints;
        }

        private double GetStraightLineXPosition(Point center, double angle, double x, double labelRadiusFromOrigin)
        {
            double circularAngle = Math.Abs(angle) % (Math.PI * 2);
            bool isLeft = circularAngle > 1.57 && circularAngle < 4.71;
            var markerWidth =ShowMarkerAtEdge2D && this.ShowMarker ? SymbolWidth : 0d;
            ////For straight horizontal connector line fixed angle as 180 for left side line and 0 as right side line.
            var connectorLineAngle = isLeft ? 180 * 0.0174532925f : 0;
            return (x + (Math.Cos((connectorLineAngle)) * (labelRadiusFromOrigin - (markerWidth / 2))));
        }

        internal bool IsTop(int index)
        {
            double yValue = Series.Adornments[index].YData;
            double nextYValue = 0.0;
            double previousYValue = 0.0;

            if (Series.Adornments.Count - 1 > index)
                nextYValue = Series.Adornments[index + 1].YData;

            if (index > 0)
                previousYValue = Series.Adornments[index - 1].YData;

            if (index == 0)
            {
                if (double.IsNaN(nextYValue))
                    return true;
                else
                    return yValue > nextYValue ? true : false;
            }

            if (index == Series.Adornments.Count - 1)
            {
                if (double.IsNaN(previousYValue))
                    return true;
                else
                    return yValue > previousYValue ? true : false;
            }
            else
            {
                if (double.IsNaN(nextYValue) && double.IsNaN(previousYValue))
                    return true;
                else if (double.IsNaN(nextYValue))
                    return previousYValue > yValue ? false : true;
                else if (double.IsNaN(previousYValue))
                    return nextYValue > yValue ? false : true;
                else
                {
                    double previousXValue = index - 1;
                    double nextXValue = index + 1;
                    double xValue = index;

                    double slope = (nextYValue - previousYValue) / (nextXValue - previousXValue);
                    double yIntercept = nextYValue - (slope * nextXValue);
                    double intersectY = (slope * xValue) + yIntercept;

                    return intersectY < yValue ? true : false;
                }
            }
        }

        /// <summary>
        /// Gets the actual label position when the chart is inversed or y values less than 0.
        /// </summary>
        internal void GetActualLabelPosition(ChartAdornment adornment)
        {
            if (this.Series is RangeSeriesBase || this.Series is FinancialSeriesBase
                 || this.Series is AccumulationSeriesBase
                || this.Series is PolarRadarSeriesBase || this.Series is BoxAndWhiskerSeries)
            {
                //The label positioning has been already algned for the above series.
                return;
            }
            else
            {
                if (adornment.Series.IsActualTransposed)
                {
                    adornment.ActualLabelPosition = this.AdornmentsPosition == AdornmentsPosition.Bottom
                        ? adornment.Series.ActualYAxis.IsInversed ^ adornment.YData < 0 ? ActualLabelPosition.Right : ActualLabelPosition.Left
                        : adornment.Series.ActualYAxis.IsInversed ^ adornment.YData < 0 ? ActualLabelPosition.Left : ActualLabelPosition.Right;
                }
                else
                {
                    adornment.ActualLabelPosition = this.AdornmentsPosition == AdornmentsPosition.Bottom
                        ? adornment.Series.ActualYAxis.IsInversed ^ adornment.YData < 0 ? ActualLabelPosition.Top : ActualLabelPosition.Bottom
                        : adornment.Series.ActualYAxis.IsInversed ^ adornment.YData < 0 ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;
                }
            }
        }

        internal void OnAdornmentPropertyChanged()
        {
            if (this is ChartAdornmentInfo)
            {
                if ((adormentContainers != null && adormentContainers.Count > 0)
                    || (ConnectorLines != null && ConnectorLines.Count > 0)
                    || LabelPresenters != null && LabelPresenters.Count > 0)
                {
                    this.Measure(AdornmentInfoSize, null);
                    this.Arrange(AdornmentInfoSize);
                }
            }
            else if (series != null && series.ActualArea != null)
            {
                series.ActualArea.ScheduleUpdate();
            }
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Draws the line segment.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="path">The path.</param>
        protected virtual void DrawLineSegment(List<Point> points, Path path)
        {

        }

        #endregion

        #region Protected Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected ChartAlignment GetVerticalAlignment(VerticalAlignment alignment)
        {
            if (alignment == VerticalAlignment.Bottom)
                return ChartAlignment.Far;
            if (alignment == VerticalAlignment.Top)
                return ChartAlignment.Near;
            return ChartAlignment.Center;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected ChartAlignment GetHorizontalAlignment(HorizontalAlignment alignment)
        {
            if (alignment == HorizontalAlignment.Right)
                return ChartAlignment.Far;
            if (alignment == HorizontalAlignment.Left)
                return ChartAlignment.Near;
            return ChartAlignment.Center;
        }

        #endregion

        #region Private Static Methods

        private static Binding CreateAdormentBinding(string path, object source)
        {
            Binding bindingProvider = new Binding();
            bindingProvider.Path = new PropertyPath(path);
            bindingProvider.Source = source;
            bindingProvider.Mode = BindingMode.OneWay;
            return bindingProvider;
        }

        private static void OnShowConnectingLine(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAdornmentInfoBase adornmentInfo = (d as ChartAdornmentInfoBase);

            adornmentInfo.UpdateConnectingLines();
            adornmentInfo.OnAdornmentPropertyChanged();
        }

        private static void OnShowMarker(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAdornmentInfoBase adornmentInfo = (d as ChartAdornmentInfoBase);

            if (!((bool)(e.NewValue)) && adornmentInfo.adormentContainers != null)
                adornmentInfo.adormentContainers.GenerateElements(0);
                adornmentInfo.UpdateAdornments();
                adornmentInfo.OnAdornmentPropertyChanged();
        }

        /// <summary>
        /// Gets the bezier point.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="controlPoints">The control points.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        static Point GetBezierPoint(double t, IList<Point> controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var p0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var p1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Point((1 - t) * p0.X + t * p1.X, (1 - t) * p0.Y + t * p1.Y);
        }

        /// <summary>
        /// Updates the adornment symbol symbol type is changed.
        /// </summary>
        /// <param name="d">The dependency object</param>
        /// <param name="e">The event arguments</param>
        private static void OnSymbolTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).OnSymbolTypeChanged();
        }

        /// <summary>
        /// Updates the font style properties.
        /// </summary>
        /// <param name="d">The dependency object</param>
        /// <param name="e">The event arguments</param>
        private static void OnFontStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).OnFontStylePropertyChanged();
        }

        /// <summary>
        /// Updates the labels and position.
        /// </summary>
        /// <param name="d">The dependency object</param>
        /// <param name="e">The event arguments</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAdornmentInfoBase adornmentInfo = (d as ChartAdornmentInfoBase);

            adornmentInfo.UpdateLabels();
            adornmentInfo.OnAdornmentPropertyChanged();
        }

        /// <summary>
        /// Updates the styling properties.
        /// </summary>
        /// <param name="d">The dependency object</param>
        /// <param name="e">The event arguments</param>
        private static void OnStylingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).OnStylingPropertyChanged();
        }

        /// <summary>
        /// Updates the label.
        /// </summary>
        /// <param name="d">The dependency object</param>
        /// <param name="e">The event arguments</param>
        private static void OnLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateLabels();
        }

        /// <summary>
        /// Updates the label color properties.
        /// </summary>
        /// <param name="d">The dependency object</param>
        /// <param name="e">The event arguments</param>
        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).OnColorPropertyChanged();
        }

        private static void OnAdornmentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateArea();
        }

        private static void OnLabelRotationAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateLabels();
        }
        
        /// <summary>
        /// Updates all the adornment properties.
        /// </summary>
        /// <param name="d">The dependency object</param>
        /// <param name="args">The event arguments</param>
        private static void OnDefaultAdornmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as ChartAdornmentInfoBase).OnDefaultAdornmentChanged();
        }

        private static void OnHighlightOnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).UpdateSelection(e);
        }

        private static void OnSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAdornmentInfoBase = d as ChartAdornmentInfoBase;
            if (chartAdornmentInfoBase != null)
            {
                if (chartAdornmentInfoBase.adormentContainers != null)
                    chartAdornmentInfoBase.adormentContainers.GenerateElements(0);
                chartAdornmentInfoBase.UpdateAdornments();
                chartAdornmentInfoBase.OnAdornmentPropertyChanged();
            }
        }

        private static void OnAdornmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentInfoBase).OnAdornmentPropertyChanged();
        }

        private static void OnSymbolInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e.OldValue == null)
            {
                var adornmentInfo = (d as ChartAdornmentInfoBase);
                if (adornmentInfo.adormentContainers != null)
                {
                    foreach (var container in adornmentInfo.adormentContainers)
                    {
                        container.UpdateContainers(true);
                    }
                }
            }
        }
        
        private static bool CheckStrokeAppliedInStyle(Style connectorStyle)
        {
            var isStrokeApplied = false;
            if (connectorStyle != null)
            {
                foreach (Setter setter in connectorStyle.Setters)
                {
#if UNIVERSALWINDOWS
                    if (setter.Property.GetType().Name == "Stroke")
#else
                    if (setter.Property.Name == "Stroke")
#endif
                    {
                        if (setter.Value != null)
                        {
                            isStrokeApplied = true;
                        }

                        break;
                    }
                }
            }

            return isStrokeApplied;
        }

        #endregion

        #region Private Methods

        void UpdateArea()
        {
            if (this.Series != null && this.Series.ActualArea != null)
                this.Series.ActualArea.ScheduleUpdate();
        }

        private void UpdateAdornments()
        {
            if (adormentContainers != null && series != null && IsMarkerRequired)
            {
                adormentContainers.GenerateElements(series.Adornments.Count);
            }
            else if (adormentContainers != null)
            {
                adormentContainers.GenerateElements(0);
            }
            if (series == null) return;
            foreach (var adornment in series.Adornments)
            {
                adornment.Series = series;
                adornment.BindColorProperties();
                Binding binding = new Binding { Source = this, Path = new PropertyPath("ConnectorHeight") };
                BindingOperations.SetBinding(adornment, ChartAdornment.ConnectorHeightProperty, binding);
                if (series is CircularSeriesBase)
                    binding = new Binding { Source = adornment, Path = new PropertyPath("Angle") };
                else
                    binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("ConnectorRotationAngle"),
                        Converter = new ConnectorRotationAngleConverter(series)
                    };
                BindingOperations.SetBinding(adornment, ChartAdornment.ConnectorRotationAngleProperty, binding);
                binding = new Binding { Source = this, Path = new PropertyPath("Foreground") };
                BindingOperations.SetBinding(adornment, ChartAdornment.ForegroundProperty, binding);
                binding = new Binding { Source = this, Path = new PropertyPath("FontFamily") };
                BindingOperations.SetBinding(adornment, ChartAdornment.FontFamilyProperty, binding);
                binding = new Binding { Source = this, Path = new PropertyPath("FontSize") };
                BindingOperations.SetBinding(adornment, ChartAdornment.FontSizeProperty, binding);
                binding = new Binding { Source = this, Path = new PropertyPath("FontStyle") };
                BindingOperations.SetBinding(adornment, ChartAdornment.FontStyleProperty, binding);
                binding = new Binding { Source = this, Path = new PropertyPath("Margin") };
                BindingOperations.SetBinding(adornment, ChartAdornment.MarginProperty, binding);
                binding = new Binding { Source = this, Path = new PropertyPath("BorderBrush") };
                BindingOperations.SetBinding(adornment, ChartAdornment.BorderBrushProperty, binding);
                binding = new Binding { Source = this, Path = new PropertyPath("BorderThickness") };
                BindingOperations.SetBinding(adornment, ChartAdornment.BorderThicknessProperty, binding);
                binding = new Binding { Source = this, Path = new PropertyPath("Background") };
                BindingOperations.SetBinding(adornment, ChartAdornment.BackgroundProperty, binding);
            }

            if (adormentContainers != null)
            {
                for (int i = 0; i < adormentContainers.Count; i++)
                {
                    var adornmentItem = series is HistogramSeries ? Series.ActualData[i] : Series.Adornments[i].Item;
                    if (series is CircularSeriesBase && !double.IsNaN(((CircularSeriesBase)series).GroupTo))
                        adormentContainers[i].Tag = i;
                    else if (series.ActualXAxis is CategoryAxis && !(series.ActualXAxis as CategoryAxis).IsIndexed
                       && (Series.IsSideBySide && (!(Series is RangeSeriesBase)) && (!(Series is FinancialSeriesBase))
                       && !(Series is WaterfallSeries)))
                        adormentContainers[i].Tag = Series.GroupedActualData.IndexOf(adornmentItem);
                    else
                        adormentContainers[i].Tag = Series.ActualData.IndexOf(adornmentItem);
                }
            }
        }

        private Point CalculateConnectorLinePoint(ref double x, ref double y, ChartAdornment adornment, double angle, int index)
        {
            var actualLabelPos =  (Series as AdornmentSeries).Adornments[index].ActualLabelPosition;
            switch (actualLabelPos)
            {
                case ActualLabelPosition.Top:
                    x = x + (Math.Cos((angle)) * ConnectorHeight);
                    y = y + (Math.Sin((angle)) * ConnectorHeight);
                    break;

                case ActualLabelPosition.Left:
                    y = y + (Math.Sin((angle)) * ConnectorHeight);
                    x = x - (Math.Cos((angle)) * ConnectorHeight);
                    break;

                case ActualLabelPosition.Bottom:
                    x = x + (Math.Cos((angle)) * ConnectorHeight);
                    y = y + (Math.Sin((-angle)) * ConnectorHeight);
                    break;

                case ActualLabelPosition.Right:
                    y = y + (Math.Sin((angle)) * ConnectorHeight);
                    x = x + (Math.Cos((angle)) * ConnectorHeight);
                    break;
            }

            if (LabelPosition == AdornmentsLabelPosition.Auto
                && (!((Series.ActualYAxis as ChartAxisBase2D).ZoomFactor < 1
                || (Series.ActualXAxis as ChartAxisBase2D).ZoomFactor < 1)))
            {
                if (Series is PolarRadarSeriesBase)
                {
                    x = (x < 0) ? 0 : (x > Series.ActualArea.SeriesClipRect.Width) ? Series.ActualArea.SeriesClipRect.Width : x;
                    y = (y < 0) ? 0 : (y > Series.ActualArea.SeriesClipRect.Height) ? Series.ActualArea.SeriesClipRect.Height : y;
                }
                else
                {
                    x = (x < 0) ? 0 : (x > Series.Clip.Bounds.Width) ? Series.Clip.Bounds.Width : x;
                    y = (y < 0) ? 0 : (y > Series.Clip.Bounds.Height) ? Series.Clip.Bounds.Height : y;
                }
            }
            return new Point(x, y);
        }

        /// <summary>
        /// Smarts the labels for outside.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="drawingPoints">The drawing points.</param>
        /// <param name="currRect">The curr rect.</param>
        /// <param name="label">The label.</param>
        /// <param name="center">The center.</param>
        /// <param name="labelRadiusFromOrigin">The label radius from origin.</param>
        /// <param name="connectorHeight">Height of the connector.</param>
        /// <param name="explodedRadius">The exploded radius.</param>
        /// <param name="pieAdornment">The pie adornment.</param>
        /// <returns></returns>
        private Point SmartLabelsForOutside(
            IList<Rect> bounds, 
            IList<Point> drawingPoints,
            Rect currRect,
            FrameworkElement label,
            Point center,
            double labelRadiusFromOrigin,
            double connectorHeight, 
            double explodedRadius,
            ChartAdornment pieAdornment)
        {
            double x, y;
            var startAngle = 0d;
            var angle = pieAdornment.ConnectorRotationAngle;
            if (pieAdornment.Series is CircularSeriesBase)
                startAngle = ((CircularSeriesBase)pieAdornment.Series).StartAngle * Math.PI / 180;
            var baseAngle = angle;
            bool isIntersected = false, isIntersectedLabel;
            //Since  no need to draw the lines to edges its need to like hipen.
            drawingPoints.RemoveAt(1);
            do
            {
                isIntersectedLabel = false;
                if (!bounds.IntersectWith(currRect)) continue;
                isIntersected = isIntersectedLabel = true;
                //If the label don’t have a place in chart area means we need to collapse the lables.
                if (angle > Math.PI * 2 + startAngle)
                {
                    label.Visibility = Visibility.Collapsed;
                    isIntersected = isIntersectedLabel = false;
                    var labelIndex = LabelPresenters.IndexOf(label);
                    if (ConnectorLines.Count > labelIndex)
                    {
                        ConnectorLines[labelIndex].Visibility = Visibility.Collapsed;
                    }
                }
                angle += 0.01;
                x = center.X + (Math.Cos((angle)) * labelRadiusFromOrigin);
                y = center.Y + (Math.Sin((angle)) * labelRadiusFromOrigin);
                currRect.X = x;
                currRect.Y = y;
            } while (isIntersectedLabel);

            x = currRect.X;
            y = currRect.Y;
            bounds.Add(currRect);
            drawingPoints.Add(isIntersected
                ? new Point(
                    pieAdornment.X + (Math.Cos((baseAngle)) * (connectorHeight + explodedRadius - connectorHeight / 1.5)),
                    pieAdornment.Y + (Math.Sin((baseAngle)) * (connectorHeight + explodedRadius - connectorHeight / 1.5)))
                : new Point(x, y));
            drawingPoints.Add(new Point(x, y));

            if (ShowConnectorLine && connectorHeight != 0)
            {
                var pieAngle = angle % (Math.PI * 2);
                //Checks, whether this labels placed over right side or left side
                var isRight = ((pieAngle <= (isIntersected ? 1.35 : 1.55) && pieAngle >= 0) || (pieAngle >= (isIntersected ? 4.51 : 4.71)));
                var hipen = connectorHeight / 5;
                x += isRight ? hipen : -hipen;
                drawingPoints.Add(new Point(x, y));
                //Label will be placed at the edge of the connector lines
                x += isRight ? label.DesiredSize.Width / 2 : -label.DesiredSize.Width / 2;
            }
            return new Point(x, y);
        }

        /// <summary>
        /// Smarts the labels for inside.
        /// </summary>
        /// <param name="adornment">The adornment.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="label">The label.</param>
        /// <param name="connectorHeight">Height of the connector.</param>
        /// <param name="labelRadiusFromOrigin">The label radius from origin.</param>
        /// <param name="pieRadius">The pie radius.</param>
        /// <param name="drawingPoints">The drawing points.</param>
        /// <param name="center">The center.</param>
        /// <param name="currRect">The curr rect.</param>
        /// <returns></returns>
        private Point SmartLabelsForInside(ChartAdornment adornment, IList<Rect> bounds, FrameworkElement label, double connectorHeight, double labelRadiusFromOrigin, double pieRadius, List<Point> drawingPoints, Point center, Rect currRect)
        {
            bool isIntersectedLabel, isIntersected = false;

            labelRadiusFromOrigin = pieRadius + connectorHeight + (label.DesiredSize.Width + label.DesiredSize.Height) / 2;
            var startAngle = 0d;
            var angle = adornment.ConnectorRotationAngle;
            var firstSegmentAngle = 0.0;
            if (adornment.Series.Segments.Count > 0)
            {
                if (adornment.Series.Segments[0] is PieSegment)
                    firstSegmentAngle = (adornment.Series.Segments[0] as PieSegment).AngleOfSlice / 2;
                else if (adornment.Series.Segments[0] is DoughnutSegment)
                    firstSegmentAngle = (adornment.Series.Segments[0] as DoughnutSegment).AngleOfSlice / 2;
            }
            if (adornment.Series is CircularSeriesBase)
                startAngle = ((CircularSeriesBase)adornment.Series).StartAngle * Math.PI / 180;
            double x = currRect.X, y = currRect.Y, baseAngle = angle;

            var labelIndex = LabelPresenters.IndexOf(label);

                do
                {
                isIntersectedLabel = false;

                if (!bounds.IntersectWith(currRect))
                    continue;

                isIntersected = isIntersectedLabel = true;
                //If the label don’t have a place in chart area means we need to collapse the lables.
                if (baseAngle > Math.PI * 2 + (startAngle + firstSegmentAngle))
                {
                    label.Visibility = Visibility.Collapsed;
                    isIntersected = isIntersectedLabel = false;
                    if (ConnectorLines.Count > labelIndex)
                    {
                        ConnectorLines[labelIndex].Visibility = Visibility.Collapsed;
                    }
                }
                //Increment the angle by radiant to  check with the overlap.
                baseAngle += 0.01;
                x = center.X + (Math.Cos((baseAngle)) * labelRadiusFromOrigin);
                y = center.Y + (Math.Sin((baseAngle)) * labelRadiusFromOrigin);
                currRect.X = x;
                currRect.Y = y;
            } while (isIntersectedLabel);

            if (isIntersected)
            {
                //If the labels is intersected means, we need to draw connector line and it should be position as like outside
                drawingPoints.Clear();
                drawingPoints.Add(new Point(center.X + (Math.Cos((angle)) * pieRadius), center.Y + (Math.Sin((angle)) * pieRadius)));
                drawingPoints.Add(new Point(center.X + (Math.Cos((angle)) * (pieRadius)), center.Y + (Math.Sin((angle)) * (pieRadius))));
            }

            drawingPoints.Add(new Point(x, y));
            bounds.Add(currRect);
            if (isIntersected && ShowConnectorLine && connectorHeight != 0)
            {
                //If the labels is intersected means, we need to position the labels outside with connector lines form edges.
                var isRight = ((angle % (Math.PI * 2) <= 1.55 && angle % (Math.PI * 2) >= 0) || (angle % (Math.PI * 2) >= 4.71));
                x += isRight ? label.DesiredSize.Width / 2 : -(label.DesiredSize.Width / 2);
            }
            return new Point(x, y);
        }

        private void OnSymbolTypeChanged()
        {
            UpdateAdornments();
            OnAdornmentPropertyChanged();
        }

        /// <summary>
        /// Updates the font style properties.
        /// </summary>
        private void OnFontStylePropertyChanged()
        {
            if (IsTextRequired)
                UpdateLabels();
        }

        private void OnStylingPropertyChanged()
        {
            if (IsTextRequired)
            {
                UpdateLabels();
                OnAdornmentPropertyChanged();
            }
            else
            {
                if (this is ChartAdornmentInfo)
                    UpdateArea();
                else
                    OnDefaultAdornmentChanged();
            }
        }

        /// <summary>
        /// Updates the label color properties.
        /// </summary>
        private void OnColorPropertyChanged()
        {
            // Check for the transition change.
            var isText = LabelPresenters != null && LabelPresenters.Count > 0
                         && LabelPresenters[0] is TextBlock;

            var needUpdate = IsTextRequired ^ isText;
            if (needUpdate)
                UpdateArea();
            else
                UpdateLabels();
        }

        /// <summary>
        /// Updates all the adornment properties.
        /// </summary>
        private void OnDefaultAdornmentChanged()
        {
            UpdateLabels();
            UpdateConnectingLines();
            UpdateAdornments();
            OnAdornmentPropertyChanged();
        }

        private void UpdateSelection(DependencyPropertyChangedEventArgs e)
        {
            if (Series != null && Series.ActualArea != null)
            {
                if ((bool)e.NewValue)
                {
                    if (Series.ActualArea.GetEnableSeriesSelection() && Series.ActualArea.SeriesSelectedIndex > -1
                        && Series is ChartSeries)
                    {
                        List<int> indexes = (from adorment in series.Adornments
                                             select series.Adornments.IndexOf(adorment)).ToList();

                        Series.AdornmentPresenter.UpdateAdornmentSelection(indexes, false);
                    }
                    else if (Series is ISegmentSelectable && (Series as ISegmentSelectable).SelectedIndex > -1)
                    {
                        Series.UpdateAdornmentSelection((Series as ISegmentSelectable).SelectedIndex);
                    }
                }
                else
                {
                    if (Series.ActualArea.SeriesSelectedIndex > -1 && Series.ActualArea.GetEnableSeriesSelection()
                        && Series.ActualArea.GetSeriesSelectionBrush(Series) != null)
                    {
                        Series.AdornmentPresenter.ResetAdornmentSelection(null, true);
                    }
                    else if ((Series is ISegmentSelectable) && (Series as ISegmentSelectable).SelectedIndex > -1
                        && Series.ActualArea.GetEnableSegmentSelection()
                        && (Series as ISegmentSelectable).SegmentSelectionBrush != null)
                    {
                        Series.AdornmentPresenter.ResetAdornmentSelection((Series as ISegmentSelectable).SelectedIndex, false);
                    }
                }
            }
        }
        
        private IList<ChartAdornment> GetOrderedAdornments()
        {
            return series.Adornments.Where(item => item.ConnectorRotationAngle % (Math.PI * 2) > 1.57 &&
                                                   item.ConnectorRotationAngle %
                                                   (Math.PI * 2) < 4.71).ToList().OrderBy(item => item.Y).Union
                (series.Adornments.Where(item => item.ConnectorRotationAngle %
                                                 (Math.PI * 2) <= 1.57 || item.ConnectorRotationAngle %
                                                 (Math.PI * 2) >= 4.71).ToList().OrderBy(item => item.Y)).ToList();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents the class used for configuring chart adornments for 2D chart.
    /// </summary>
    /// <remarks>
    /// Chart adornments are used to show additional information about the data point.
    /// </remarks>
    /// <example>
    /// This example, we are using <see cref="PieSeries"/>.
    /// </example>
    /// <example>
    ///     <code language="XAML">
    ///         &lt;syncfusion:PieSeries&gt;
    ///             &lt;syncfusion:PieSeries.AdornmentInfo&gt;
    ///                 &lt;syncfusion:ChartAdornmentInfo&gt;
    ///             &lt;/syncfusion:PieSeries.AdornmentInfo&gt;
    ///         &lt;syncfusion:PieSeries&gt;
    ///     </code>
    ///     <code language="C#">
    ///         ChartAdornmentInfo chartAdornmentInfo = new ChartAdornmentInfo();
    ///         pieSeries.AdornmentInfo = chartAdornmentInfo;
    ///     </code>
    /// </example>
    public sealed class ChartAdornmentInfo : ChartAdornmentInfoBase
    {
        internal Point ConnectorEndPoint { get; set; }
        #region Methods

        #region Internal Override Methods

        internal override void Arrange(Size finalSize)
        {
            double pieLeft = 0d, pieRight = 0d, pieRadius = 0d;
            var circularSeriesBase = series as CircularSeriesBase;
            var isPieSeriesExtendedLabels = circularSeriesBase != null && circularSeriesBase.LabelPosition == CircularSeriesLabelPosition.OutsideExtended && circularSeriesBase.EnableSmartLabels;
            AdornmentInfoSize = finalSize;
            IsStraightConnectorLine2D = circularSeriesBase != null && circularSeriesBase.ConnectorType == ConnectorMode.StraightLine;
            ShowMarkerAtEdge2D = circularSeriesBase != null && circularSeriesBase.ShowMarkerAtLineEnd;
            var index = 0;
            if (LabelPresenters != null && LabelPresenters.Count > 0 && circularSeriesBase != null)
            {
                pieRadius = circularSeriesBase.Radius;
                foreach (var pieAdornment in Series.Adornments.Select(adornment => adornment as ChartPieAdornment))
                {
                    if (pieAdornment.ConnectorRotationAngle % (Math.PI * 2) <= 1.57 || pieAdornment.ConnectorRotationAngle % (Math.PI * 2) >= 4.71)
                    {
                        pieLeft = Math.Max(pieLeft, LabelPresenters[index].DesiredSize.Width);
                    }
                    else
                    {
                        pieRight = Math.Max(pieRight, LabelPresenters[index].DesiredSize.Width);
                    }

                    index++;
                }
                pieRight = finalSize.Width - pieRight;
            }

            var adornmentIndex = 0;
            var labelBounds = new List<Rect>();
            var doughnutSeries = this.Series as DoughnutSeries;
            bool isMultipleDoughnut = doughnutSeries != null && doughnutSeries.IsStackedDoughnut;

            foreach (var adornment in series.Adornments)
            {
                var transformer = Series.CreateTransformer(finalSize, false);
                adornment.Update(transformer);

                var pieAdornment = adornment as ChartPieAdornment;
                if (adormentContainers != null && adornmentIndex < adormentContainers.Count)
                {
                    var adornmentPresenter = adormentContainers[adornmentIndex];

                    if (!double.IsNaN(adornment.YData) && !double.IsNaN(adornment.XData))
                    {
                        //WPF-22703 - Adornment symbol render wrongly with empty points 
                        adornmentPresenter.Visibility = Visibility.Visible;

                        var adornmentRect = new Rect();

                        // Adorment position for the multiple doughnut series case.
                        if (isMultipleDoughnut)
                        {
                            // Symbol is set at the end.
                            var doughnutSegment = doughnutSeries.Segments[adornmentIndex] as DoughnutSegment;
                            adornmentRect = new Rect(new Point(), (adornmentPresenter.DesiredSize))
                            {
                                X = doughnutSeries.Center.X + (pieAdornment.Radius - (pieAdornment.Radius - pieAdornment.InnerDoughnutRadius) / 2) * Math.Cos(doughnutSegment.EndAngle) - adornmentPresenter.SymbolOffset.X,
                                Y = doughnutSeries.Center.Y + (pieAdornment.Radius - (pieAdornment.Radius - pieAdornment.InnerDoughnutRadius) / 2) * Math.Sin(doughnutSegment.EndAngle) - adornmentPresenter.SymbolOffset.Y
                            };                          
                        }
                        else
                        {
                            adornmentRect = new Rect(new Point(), (adornmentPresenter.DesiredSize))
                            {
                                X = adornment.X - adornmentPresenter.SymbolOffset.X,
                                Y = adornment.Y - adornmentPresenter.SymbolOffset.Y
                            };
                        }

                        Canvas.SetZIndex(adornmentPresenter, 3);
                        Canvas.SetLeft(adornmentPresenter, adornmentRect.Left);
                        Canvas.SetTop(adornmentPresenter, adornmentRect.Top);
                    }
                    else
                        adornmentPresenter.Visibility = Visibility.Collapsed;
                }
                //Update the outside and inside labels
                if (!isPieSeriesExtendedLabels)
                {
                    var segmentRadius = isMultipleDoughnut ? pieAdornment.Radius : pieRadius;
                    UpdateLabelPos(segmentRadius, labelBounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight);
                }

                adornmentIndex++;
            }
            //Update the outside extended labels
            if (isPieSeriesExtendedLabels)
            {
                UpdateSpiderLabels(pieLeft, pieRight, finalSize, pieRadius);
            }

            if (ConnectorLines != null)
                foreach (var line in ConnectorLines)
                {
                    Canvas.SetLeft(line, 0);
                    Canvas.SetTop(line, 0);
                }
        }

        internal override DependencyObject CloneAdornmentInfo()
        {
            var adornment = new ChartAdornmentInfo
            {
                ShowLabel = ShowLabel,
                ShowMarker = ShowMarker,
                Symbol = Symbol,
                SymbolHeight = SymbolHeight,
                SymbolInterior = SymbolInterior,
                SymbolTemplate = SymbolTemplate,
                SymbolWidth = SymbolWidth,
                ShowConnectorLine = ShowConnectorLine,
                SegmentLabelFormat = SegmentLabelFormat,
                SegmentLabelContent = SegmentLabelContent,
                LabelTemplate = LabelTemplate,
                HorizontalAlignment = HorizontalAlignment,
                ConnectorLineStyle = ConnectorLineStyle,
                LabelPosition = LabelPosition,
                UseSeriesPalette = UseSeriesPalette
            };
            return adornment;
        }

        #endregion

        #region Internal Methods

        internal void AlignAdornmentLabelPosition(FrameworkElement control, AdornmentsLabelPosition labelPosition, double x, double y, int index)
        {

            Point point = new Point(x, y);
            double padding = !(ShowConnectorLine) || (ConnectorHeight <= 0) ? LabelPadding : 0.0;

            switch (labelPosition)
            {
                case AdornmentsLabelPosition.Auto:
                    if (Series is StackingColumnSeries || Series is StackingBarSeries || Series is StackingAreaSeries ||
                        Series is FastStackingColumnBitmapSeries)
                        point = AlignInnerLabelPosition(control, index, x, y);

                    else if (Series is BubbleSeries || Series is FinancialSeriesBase || Series is TriangularSeriesBase)
                    {
                        point.X = x - control.DesiredSize.Width / 2;
                        point.Y = y - control.DesiredSize.Height / 2;
                    }
                    else if (Series is LineSeries || Series is SplineSeries || Series is FastLineSeries || Series is StepLineSeries
                             || Series is FastStepLineBitmapSeries || series is FastLineBitmapSeries)
                    {
                        if (Series.ActualYAxis.IsInversed)
                        {
                            if ((Series as CartesianSeries).IsTransposed)
                            {
                                point.Y = y - control.DesiredSize.Height / 2;
                                point.X = IsTop(index) ? x - control.DesiredSize.Width - padding : x + padding;
                            }
                            else
                            {
                                point.X = x - control.DesiredSize.Width / 2;
                                point.Y = IsTop(index) ? y + padding : y - control.DesiredSize.Height - padding;
                            }
                        }
                        else
                        {
                            if ((Series as CartesianSeries).IsTransposed)
                            {
                                point.Y = y - control.DesiredSize.Height / 2;
                                point.X = IsTop(index) ? x + padding : x - control.DesiredSize.Width - padding;
                            }
                            else
                            {
                                point.X = x - control.DesiredSize.Width / 2;
                                point.Y = IsTop(index) ? y - control.DesiredSize.Height - padding : y + padding;
                            }
                        }
                    }
                    else if (Series is CircularSeriesBase || Series is PolarRadarSeriesBase)
                    {
                        point.X -= control.DesiredSize.Width / 2;
                        point.Y -= control.DesiredSize.Height / 2;
                    }
                    else
                        point = AlignOuterLabelPosition(control, index, x, y);
                    if (Series is TriangularSeriesBase)
                    {
                        point.X = (point.X < 0) ? padding : (point.X + control.DesiredSize.Width > Series.GetAvailableSize().Width) ? Series.GetAvailableSize().Width - control.DesiredSize.Width - padding : point.X;
                        point.Y = (point.Y < 0) ? padding : (point.Y + control.DesiredSize.Height > Series.GetAvailableSize().Height) ? Series.GetAvailableSize().Height - control.DesiredSize.Height - padding : point.Y;
                    }
                    else if (Series is CircularSeriesBase || !((Series.ActualYAxis as ChartAxisBase2D).ZoomFactor < 1 || (Series.ActualXAxis as ChartAxisBase2D).ZoomFactor < 1))
                    {
                        double seriesHeight = 0.0;
                        double seriesWidth = 0.0;

                        if (Series is PolarRadarSeriesBase && Series.Segments.Count > 0)
                        {
                            seriesHeight = Series.ActualArea.SeriesClipRect.Height;
                            seriesWidth = Series.ActualArea.SeriesClipRect.Width;
                        }
                        else
                        {
                            seriesHeight = (Series is CircularSeriesBase) ? (Series as CircularSeriesBase).Center.Y * 2 : Series.Clip.Bounds.Height;
                            seriesWidth = (Series is CircularSeriesBase) ? (Series as CircularSeriesBase).Center.X * 2 : Series.Clip.Bounds.Width;
                        }

                        if ((point.Y < 0 || point.Y + control.DesiredSize.Height > seriesHeight) && Series is ColumnSeries)
                            point = AlignInnerLabelPosition(control, index, x, y);

                        if ((point.X < 0 || point.X + control.DesiredSize.Width > seriesWidth) && Series is BarSeries)
                            point = AlignInnerLabelPosition(control, index, x, y);

                        point.Y = (point.Y < 0) ? padding : (point.Y + control.DesiredSize.Height > seriesHeight) ? seriesHeight - control.DesiredSize.Height - padding : point.Y;
                        point.X = (point.X < 0) ? padding : (point.X + control.DesiredSize.Width > seriesWidth) ? seriesWidth - control.DesiredSize.Width - padding : point.X;

                    }
                    break;

                case AdornmentsLabelPosition.Inner:
                    if (Series is LineSeries || Series is SplineSeries || Series is FastLineSeries || Series is StepLineSeries
                        || Series is FastStepLineBitmapSeries || Series is FastLineBitmapSeries)
                    {
                        if (Series.ActualYAxis.IsInversed)
                        {
                            if ((Series as CartesianSeries).IsTransposed)
                            {
                                point.Y = y - control.DesiredSize.Height / 2;
                                point.X = IsTop(index) ? x + padding : x - control.DesiredSize.Width - padding;
                            }
                            else
                            {
                                point.X = x - control.DesiredSize.Width / 2;
                                point.Y = IsTop(index) ? y - control.DesiredSize.Height - padding : y + padding;
                            }
                        }
                        else
                        {
                            if ((Series as CartesianSeries).IsTransposed)
                            {
                                point.Y = y - control.DesiredSize.Height / 2;
                                point.X = IsTop(index) ? x - control.DesiredSize.Width - padding : x + padding;
                            }
                            else
                            {
                                point.X = x - control.DesiredSize.Width / 2;
                                point.Y = IsTop(index) ? y + padding : y - control.DesiredSize.Height - padding;
                            }
                        }
                    }
                    else
                        point = AlignInnerLabelPosition(control, index, x, y);
                    break;

                case AdornmentsLabelPosition.Center:
                    point.Y = point.Y - control.DesiredSize.Height / 2;
                    point.X = point.X - control.DesiredSize.Width / 2;
                    break;

                default:
                    if (Series is LineSeries || Series is SplineSeries || Series is FastLineSeries || Series is StepLineSeries
                        || Series is FastStepLineBitmapSeries || Series is FastLineBitmapSeries)
                    {
                        if (Series.ActualYAxis.IsInversed)
                        {
                            if ((Series as CartesianSeries).IsTransposed)
                            {
                                point.Y = y - control.DesiredSize.Height / 2;
                                point.X = IsTop(index) ? x - control.DesiredSize.Width - padding : x + padding;
                            }
                            else
                            {
                                point.X = x - control.DesiredSize.Width / 2;
                                point.Y = IsTop(index) ? y + padding : y - control.DesiredSize.Height - padding;
                            }
                        }
                        else
                        {
                            if ((Series as CartesianSeries).IsTransposed)
                            {
                                point.Y = y - control.DesiredSize.Height / 2;
                                point.X = IsTop(index) ? x + padding : x - control.DesiredSize.Width - padding;
                            }
                            else
                            {
                                point.X = x - control.DesiredSize.Width / 2;
                                point.Y = IsTop(index) ? y - control.DesiredSize.Height - padding : y + padding;
                            }
                        }
                    }
                    else
                    {
                        point = AlignOuterLabelPosition(control, index, x, y);
                    }
                    break;
            }
            Canvas.SetLeft(control, point.X);
            Canvas.SetTop(control, point.Y);
        }

        #endregion

        #region Protected Override Methods

        protected override void DrawLineSegment(List<Point> points, Path path)
        {
            if (points.Count < 1 || path == null) return;
            var pathFigure = new PathFigure();
            var pathGeometry = new PathGeometry();

            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            pathFigure.StartPoint = points[0];
            var segment = new PolyLineSegment { Points = new PointCollection() };
            foreach (var item in points)
            {
                segment.Points.Add(item);
            }
            pathFigure.Segments.Add(segment);
        }

        #endregion

        #region Private Static Methods
        
        private static Point GetMultipleDoughnutLabelPoints(DoughnutSeries doughnutSeries, DoughnutSegment doughnutSegment, double width, double height, double padding, Point center, int index, double x, double y, bool isOuter, double angle)
        {
            Point doughnutPoint = new Point();

            if (doughnutSeries.LabelPosition == CircularSeriesLabelPosition.Inside)
            {
                doughnutPoint.X = x;
                doughnutPoint.Y = y;
            }
            else
            {
                var isClockwisePlacement = !(doughnutSegment.EndAngle > doughnutSegment.StartAngle ^ isOuter);
                var pieAdornment = (doughnutSeries.Adornments[index] as ChartPieAdornment);
                var segmentRadius = pieAdornment.Radius - pieAdornment.InnerDoughnutRadius;
                var middleRadius = pieAdornment.InnerDoughnutRadius + segmentRadius / 2;
                var xAngle = 0d;
                var yAngle = 0d;


                if (doughnutSeries.LabelPosition == CircularSeriesLabelPosition.OutsideExtended)
                {
                    xAngle = isClockwisePlacement ? doughnutSegment.EndAngle + ((width / 2 + padding)) / middleRadius : doughnutSegment.EndAngle - ((width / 2 + padding)) / middleRadius;
                    yAngle = isClockwisePlacement ? doughnutSegment.EndAngle + ((height / 2 + padding)) / middleRadius : doughnutSegment.EndAngle - ((height / 2 + padding)) / middleRadius;
                }
                else
                {
                    xAngle = isClockwisePlacement ? doughnutSegment.StartAngle - ((width / 2 + padding)) / middleRadius : doughnutSegment.StartAngle + ((width / 2 + padding)) / middleRadius;
                    yAngle = isClockwisePlacement ? doughnutSegment.StartAngle - ((height / 2 + padding)) / middleRadius : doughnutSegment.StartAngle + ((height / 2 + padding)) / middleRadius;
                }

                doughnutPoint.X = center.X + middleRadius * Math.Cos(xAngle) - width / 2;
                doughnutPoint.Y = center.Y + middleRadius * Math.Sin(yAngle) - height / 2;
            }

            return doughnutPoint;
        }

        #endregion

        #region Private Methods

        private void UpdateLabelPos(double pieRadius, IList<Rect> bounds, Size finalSize, ChartAdornment adornment, int adornmentIndex, double pieLeft, double pieRight)
        {
            if (adornment == null) return;
            double x = adornment.X, y = adornment.Y;
            var label = LabelPresenters != null && LabelPresenters.Count > 0 ? LabelPresenters[adornmentIndex] : null;
            if (label != null && ShowLabel)
            {
                if (adornment.CanHideLabel || double.IsNaN(x) || double.IsNaN(y))
                {
                    label.Visibility = Visibility.Collapsed;
                    return;
                }
                label.Visibility = Visibility.Visible;
            }

            GetActualLabelPosition(adornment);
            //Reset the visibility if the visibility is collapsed from collision.
            if (ConnectorLines.Count > adornmentIndex)
            {
                if (adornment.CanHideLabel)
                {
                    ConnectorLines[adornmentIndex].Visibility = Visibility.Collapsed;
                }
                else
                {
                    ConnectorLines[adornmentIndex].Visibility = Visibility.Visible;
                }
            }

            var circularSeriesBase = series as CircularSeriesBase;
            var pieAdornment = adornment as ChartPieAdornment;

            if (ShowConnectorLine || (circularSeriesBase != null && circularSeriesBase.EnableSmartLabels))
            {
                var connectorLineMode = circularSeriesBase != null ? circularSeriesBase.ConnectorType : ConnectorMode.Line;
                var isPie = pieAdornment != null;
                if (label != null && LabelPosition != AdornmentsLabelPosition.Default && ConnectorHeight > 0)
                {
                    if (Series is BubbleSeries || Series is ScatterSeries)
                    {
                        double bubbleRadius = Series is BubbleSeries ? (Series.Segments[adornmentIndex] as BubbleSegment).SegmentRadius
                                                                    : (Series.Segments[adornmentIndex] as ScatterSegment).ScatterHeight / 2;
                        var angle = (6.28 * (1 - (adornment.ConnectorRotationAngle / 360.0)));
                        if (LabelPosition == AdornmentsLabelPosition.Outer)
                        {
                            if (this.Series.ActualYAxis.IsInversed ^ (adornment.YData < 0 || AdornmentsPosition == Charts.AdornmentsPosition.Bottom))
                            {
                                x = x - (Math.Cos(angle) * bubbleRadius);
                                y = y - (Math.Sin(angle) * bubbleRadius);
                            }
                            else
                            {
                                x = x + (Math.Cos(angle) * bubbleRadius);
                                y = y + (Math.Sin(angle) * bubbleRadius);
                            }
                        }
                        else if (LabelPosition == AdornmentsLabelPosition.Inner && (bubbleRadius - label.DesiredSize.Height / 2 > 0))
                        {
                            if (this.Series.ActualYAxis.IsInversed ^ (adornment.YData < 0 || AdornmentsPosition == Charts.AdornmentsPosition.Bottom))
                            {
                                x = x - (Math.Cos(angle) * (bubbleRadius - label.DesiredSize.Height / 2));
                                y = y - (Math.Sin(angle) * (bubbleRadius - label.DesiredSize.Height / 2));
                            }
                            else
                            {
                                x = x + (Math.Cos(angle) * (bubbleRadius - label.DesiredSize.Height / 2));
                                y = y + (Math.Sin(angle) * (bubbleRadius - label.DesiredSize.Height / 2));
                            }
                        }
                    }
                    else if (Series is TriangularSeriesBase)
                    {
                        double height;
                        if (Series is PyramidSeries)
                        {
                            PyramidSegment segment = Series.Segments[adornmentIndex] as PyramidSegment;
                            height = segment.height;
                        }
                        else
                        {
                            FunnelSegment segment = Series.Segments[adornmentIndex] as FunnelSegment;
                            height = segment.height;
                        }
                        if (LabelPosition == AdornmentsLabelPosition.Inner)
                        {
                            y = y + height - label.DesiredSize.Height / 2;
                        }
                        else if (LabelPosition == AdornmentsLabelPosition.Outer)
                        {
                            y = y - height + label.DesiredSize.Height / 2;
                        }
                    }
                }

                var connectorAngle = isPie ? adornment.ConnectorRotationAngle : (6.28 * (1 - (adornment.ConnectorRotationAngle / 360.0)));
                var points = GetAdornmentPositions(pieRadius, bounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight, label, circularSeriesBase, ref x, ref y, connectorAngle, isPie);
                DrawConnectorLine(adornmentIndex, points, connectorLineMode, false, 0);
                if (ShowMarkerAtEdge2D && circularSeriesBase != null && ShowMarker && adormentContainers != null && adornmentIndex < adormentContainers.Count)
                {
                    SetSymbolPosition(new Point(points.Last().X, points.Last().Y), adormentContainers[adornmentIndex]);
                }
            }

            if (!ShowLabel || double.IsNaN(y) || label == null) return;
            // For straight line circular adornment the label is set top.
            if (circularSeriesBase != null && circularSeriesBase.ConnectorLinePosition == ConnectorLinePosition.Auto && IsStraightConnectorLine2D)
            {
                var center = circularSeriesBase != null ? circularSeriesBase.Center : new Point(finalSize.Width / 2, finalSize.Height / 2);
                ChartAdornmentContainer adornmentSymbol = null;
                if (adormentContainers != null && adormentContainers.Count > adornmentIndex)
                {
                    adornmentSymbol = adormentContainers[adornmentIndex];
                }

                AlignStraightConnectorLineLabel(circularSeriesBase.Center, adornmentSymbol, ref x, ref y, label);

                if (circularSeriesBase.ConnectorLinePosition == ConnectorLinePosition.Auto)
                {
                    var labelBounds = new Rect(Canvas.GetLeft(label), Canvas.GetTop(label), label.DesiredSize.Width, label.DesiredSize.Height);
                    var doughnutSegmentPath = series.Segments[adornmentIndex].GetRenderedVisual() as Path;
                    var midAngle = pieAdornment.Angle;
                    var bottomAngle = midAngle;
                    var topAngle = midAngle;
                    bool isBottomAlign = true;

                    PieSegment pieSegment = Series.Segments[adornmentIndex] as PieSegment;
                    DoughnutSegment doughnutSegment = Series.Segments[adornmentIndex] as DoughnutSegment;

                    double startAngle = doughnutSegment != null ? doughnutSegment.StartAngle : pieSegment.StartAngle;
                    double endAngle = doughnutSegment != null ? doughnutSegment.EndAngle : pieSegment.EndAngle;

                    while (IsLabelCollidedWithSegment(x, y, label, circularSeriesBase.Center, finalSize, pieRadius))
                    {
                        if (isBottomAlign)
                        {
                            if (bottomAngle + 0.01 > midAngle && bottomAngle + 0.01 < endAngle)
                            {
                                bottomAngle += 0.01;
                                UpdateLabelPositionAndConnectorLine(pieRadius, bounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight, label, circularSeriesBase, ref x, ref y, circularSeriesBase.ConnectorType, false, 0, bottomAngle, true);
                                AlignStraightConnectorLineLabel(circularSeriesBase.Center, adornmentSymbol, ref x, ref y, label);
                            }
                            else
                            {
                                isBottomAlign = false;
                            }
                        }
                        else
                        {
                            if (topAngle - 0.01 < midAngle && topAngle - 0.01 > startAngle)
                            {
                                topAngle += 0.01;
                                UpdateLabelPositionAndConnectorLine(pieRadius, bounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight, label, circularSeriesBase, ref x, ref y, circularSeriesBase.ConnectorType, false, 0, topAngle, true);
                                AlignStraightConnectorLineLabel(circularSeriesBase.Center, adornmentSymbol, ref x, ref y, label);
                            }
                            else
                            {
                                // Placing at center default position if no space is available
                                UpdateLabelPositionAndConnectorLine(pieRadius, bounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight, label, circularSeriesBase, ref x, ref y, circularSeriesBase.ConnectorType, false, 0, midAngle, true);
                                AlignStraightConnectorLineLabel(circularSeriesBase.Center, adornmentSymbol, ref x, ref y, label);
                                break;
                            }
                        }
                    }
                }

                Canvas.SetLeft(label, x);
                Canvas.SetTop(label, y);
            }
            else if (ShowMarkerAtEdge2D && ShowMarker && circularSeriesBase != null)
            {
                ChartAdornmentContainer adornmentSymbol = null;
                if (adormentContainers != null && adormentContainers.Count > adornmentIndex)
                {
                    adornmentSymbol = adormentContainers[adornmentIndex];
                }

                var center = circularSeriesBase != null ? circularSeriesBase.Center : new Point(finalSize.Width / 2, finalSize.Height / 2);
                AlignStraightConnectorLineLabel(label, center, LabelPosition, adornmentSymbol, x, y, circularSeriesBase.EnableSmartLabels, circularSeriesBase.LabelPosition);
            }
            else
            {
                if (LabelPosition == AdornmentsLabelPosition.Default)
                    AlignElement(label, GetVerticalAlignment(VerticalAlignment), GetHorizontalAlignment(HorizontalAlignment), x, y);
                else
                    AlignAdornmentLabelPosition(label, LabelPosition, x + OffsetX, y + OffsetY, adornmentIndex);
            }
        }

        /// <summary>
        /// Define auto position for the straight line.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="adornmentSymbol"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="label"></param>
        internal void AlignStraightConnectorLineLabel(Point center, ChartAdornmentContainer adornmentSymbol, ref double x, ref double y, FrameworkElement label)
        {
            double adornmentSymbolWidth = 0d, adornmentSymbolHeight = 0d;

            if (adornmentSymbol != null)
            {
                if (SymbolTemplate == null)
                {
                    adornmentSymbolWidth = SymbolWidth;
                    adornmentSymbolHeight = SymbolHeight;
                }
                else
                {
                    adornmentSymbolWidth = adornmentSymbol.DesiredSize.Width;
                    adornmentSymbolHeight = adornmentSymbol.DesiredSize.Height;
                }
            }

            x = (x > center.X) ? x - label.DesiredSize.Width - adornmentSymbolWidth / 2 : x + adornmentSymbolWidth / 2;
            y = y - label.DesiredSize.Height - adornmentSymbolHeight / 2;
        }

        private void UpdateLabelPositionAndConnectorLine(double pieRadius, IList<Rect> bounds, Size finalSize, ChartAdornment adornment, int adornmentIndex, double pieLeft, double pieRight, FrameworkElement label, CircularSeriesBase circularSeriesBase, ref double x, ref double y, ConnectorMode connectorLineMode, bool is3D, int value, double connectorAngle, bool isPie)
        {
            var points = GetAdornmentPositions(pieRadius, bounds, finalSize, adornment, adornmentIndex, pieLeft, pieRight, label, circularSeriesBase, ref x, ref y, connectorAngle, isPie);
            DrawConnectorLine(adornmentIndex, points, connectorLineMode, is3D, value);
            ConnectorEndPoint = points[points.Count - 1];
            if (ShowMarkerAtEdge2D && circularSeriesBase != null && ShowMarker && adormentContainers != null && adornmentIndex < adormentContainers.Count)
            {
                SetSymbolPosition(new Point(points.Last().X, points.Last().Y), adormentContainers[adornmentIndex]);
            }
        }

        private static bool IsLabelCollidedWithSegment(double x, double y, FrameworkElement label, Point center, Size finalSize, double pieRadius)
        {
            var labelBoundsRect = new Rect(x, y, label.DesiredSize.Width, label.DesiredSize.Height);
            return ChartMath.IsPointInsideCircle(center, pieRadius, new Point(x, y))
                   || ChartMath.IsPointInsideCircle(center, pieRadius, new Point(x + label.DesiredSize.Width, y))
                   || ChartMath.IsPointInsideCircle(center, pieRadius, new Point(x, y + label.DesiredSize.Height))
                   || ChartMath.IsPointInsideCircle(center, pieRadius, new Point(x + label.DesiredSize.Width, y + label.DesiredSize.Height));
        }

        private Point AlignOuterLabelPosition(FrameworkElement control, int index, double x, double y)
        {
            double padding = !(ShowConnectorLine) || ConnectorHeight <= 0 ? LabelPadding : 0.0;
            double height = control.DesiredSize.Height;
            double width = control.DesiredSize.Width;

            if (Series is ScatterSeries)
            {
                ScatterSegment segment = Series.Segments[index] as ScatterSegment;
                padding = padding + (!ShowConnectorLine || ConnectorHeight <= 0 ? segment.ScatterHeight / 2 : 0);
            }
            else if (Series is BubbleSeries)
            {
                BubbleSegment segment = Series.Segments[index] as BubbleSegment;
                padding = padding + (!ShowConnectorLine || ConnectorHeight <= 0 ? segment.SegmentRadius : 0);
            }

            if (Series is CircularSeriesBase)
            {
                var pieSeries = Series as CircularSeriesBase;
                Point center = pieSeries.Center;

                PieSegment pieSegment = Series.Segments[index] as PieSegment;
                DoughnutSegment doughnutSegment = Series.Segments[index] as DoughnutSegment;

                double angle = (Series is PieSeries) ? (pieSegment.StartAngle + pieSegment.EndAngle) / 2 :
                                (doughnutSegment.StartAngle + doughnutSegment.EndAngle) / 2;

                //Default positioning. 
                x = x - width / 2;
                y = y - height / 2;
                
                if (!pieSeries.EnableSmartLabels)
                {
                    if (pieSeries.LabelPosition == CircularSeriesLabelPosition.OutsideExtended && ShowConnectorLine)
                    {
                        x = (x > center.X) ? x + width / 2 : x - width / 2;
                    }
                    else
                    {
                        var doughnutSeries = Series as DoughnutSeries;
                        if (doughnutSeries != null && doughnutSeries.IsStackedDoughnut)
                        {
                            var doughnutLabelPoint = GetMultipleDoughnutLabelPoints(doughnutSeries, doughnutSegment, width, height, padding, center, index, x, y, true, angle);
                            x = doughnutLabelPoint.X;
                            y = doughnutLabelPoint.Y;
                        }
                        else
                        {
                            x = x + Math.Cos(angle) * (width / 2 + padding);
                            y = y + Math.Sin(angle) * (height / 2 + padding);
                        }
                    }
                }
            }
            else if (Series is PyramidSeries)
            {
                x = x - width / 2;
                PyramidSegment segment = Series.Segments[index] as PyramidSegment;
                y = y + padding - (ConnectorHeight <= 0 ? segment.height : 0);
            }
            else if (Series is FunnelSeries)
            {
                x = x - width / 2;
                if (index < Series.Segments.Count)
                {
                    FunnelSegment segment = Series.Segments[index] as FunnelSegment;
                    y = y - padding + (ConnectorHeight <= 0 ? segment.height : 0) - height;
                }
            }
            else if (Series is PolarRadarSeriesBase)
            {
                if(!ShowConnectorLine)
                {
                    Point vectorPoint = ChartTransform.ValueToVector(Series.ActualXAxis, (Series as AdornmentSeries).Adornments[index].XData);
                    x = (width / 2 * vectorPoint.X + x) - width / 2 + padding;
                    y = (height / 2 * vectorPoint.Y + y) - height / 2 + padding;
                }
                else
                {
                    x = x - width / 2;
                    y = y - height / 2;

                    var polarRadarSeries = Series as PolarRadarSeriesBase;
                    var centerX = polarRadarSeries.Area.SeriesClipRect.Width / 2;
                    var centerY = polarRadarSeries.Area.SeriesClipRect.Height / 2;
                    centerX = centerX - width / 2;
                    centerY = centerY - height / 2;
                    bool isLeft = x < centerX, isBottom = y > centerY;

                    if (x == centerX)
                    {
                        y = isBottom ? y + (height / 2) + padding : y - (height / 2) - padding;
                    }
                    else if (y == centerY)
                    {
                        x = isLeft ? x - (width / 2) - padding : x + (width / 2) + padding;
                    }
                    else if (isLeft)
                    {
                        x = x - (width / 2) - padding;
                        y = isBottom ? y + (height / 2) + padding : y - (height / 2) - padding;
                    }
                    else
                    {
                        x = x + (width / 2) + padding;
                        y = isBottom ? y + (height / 2) + padding : y - (height / 2) - padding;
                    }
                }
            }
            else
            {
                if (series is RangeColumnSeries && !series.IsMultipleYPathRequired)
                {
                    x = x - width / 2;
                    y = y - height / 2;
                    return new Point(x, y);
                }

                switch ((Series as AdornmentSeries).Adornments[index].ActualLabelPosition)
                {
                    case ActualLabelPosition.Top:
                        y = y - height - padding;
                        x = x - width / 2;
                        break;
                    case ActualLabelPosition.Left:
                        x = x - width - padding;
                        y = y - height / 2;
                        break;
                    case ActualLabelPosition.Bottom:
                        y = y + padding;
                        x = x - width / 2;
                        break;
                    case ActualLabelPosition.Right:
                        x = x + padding;
                        y = y - height / 2;
                        break;
                }
            }

            return new Point(x, y);
        }

        private Point AlignInnerLabelPosition(FrameworkElement control, int index, double x, double y)
        {
            double padding = !(ShowConnectorLine) || ConnectorHeight <= 0 ? LabelPadding : 0.0;
            double height = control.DesiredSize.Height;
            double width = control.DesiredSize.Width;

            if (Series is ScatterSeries)
            {
                ScatterSegment segment = Series.Segments[index] as ScatterSegment;
                padding = padding - (!ShowConnectorLine || ConnectorHeight <= 0 ? segment.ScatterHeight / 2 : 0);
            }
            else if (Series is BubbleSeries)
            {
                BubbleSegment segment = Series.Segments[index] as BubbleSegment;
                padding = padding - (!ShowConnectorLine || ConnectorHeight <= 0 ? segment.SegmentRadius : 0);
            }

            if (Series is CircularSeriesBase)
            {
                var pieSeries = series as CircularSeriesBase;
                Point center = pieSeries.Center;
                PieSegment pieSegment = Series.Segments[index] as PieSegment;
                DoughnutSegment douSegment = Series.Segments[index] as DoughnutSegment;

                double angle = (Series is PieSeries) ? (pieSegment.StartAngle + pieSegment.EndAngle) / 2 :
                                (douSegment.StartAngle + douSegment.EndAngle) / 2;

                //Default positioning.
                x = x - width / 2;
                y = y - height / 2;
                            
                if (!pieSeries.EnableSmartLabels)
                {
                    if (pieSeries.LabelPosition == CircularSeriesLabelPosition.OutsideExtended && ShowConnectorLine)
                    {
                        x = (x > center.X) ? x - width / 2 : x + width / 2;
                    }
                    else
                    {
                        var doughnutSeries = Series as DoughnutSeries;
                        if (doughnutSeries != null && doughnutSeries.IsStackedDoughnut)
                        {
                            var doughnutPoint = GetMultipleDoughnutLabelPoints(doughnutSeries, douSegment, width, height, padding, center, index, x, y, false, angle);

                            x = doughnutPoint.X;
                            y = doughnutPoint.Y;                           
                        }
                        else
                        {
                            x = x - Math.Cos(angle) * (width / 2 + padding);
                            y = y - Math.Sin(angle) * (height / 2 + padding);
                        }
                    }
                }
            }
            else if (Series is PyramidSeries)
            {
                x = x - width / 2;
                PyramidSegment segment = Series.Segments[index] as PyramidSegment;
                y = y - padding + (ConnectorHeight <= 0 ? segment.height : 0) - height;
            }
            else if (Series is FunnelSeries)
            {
                x = x - width / 2;
                if (index < Series.Segments.Count)
                {
                    FunnelSegment segment = Series.Segments[index] as FunnelSegment;
                    y = y - padding + (ConnectorHeight <= 0 ? segment.height : 0) - height;
                }
            }
            else if (Series is PolarRadarSeriesBase)
            {
                if(!ShowConnectorLine)
                {
                    Point vectorPoint = ChartTransform.ValueToVector(Series.ActualXAxis, (Series as AdornmentSeries).Adornments[index].XData);
                    x = (-width / 2 * vectorPoint.X + x) - width / 2 + padding;
                    y = (-height / 2 * vectorPoint.Y + y) - height / 2 + padding;
                }
                else
                {
                    x = x - width / 2;
                    y = y - height / 2;

                    var polarRadarSeries = Series as PolarRadarSeriesBase;
                    var centerX = polarRadarSeries.Area.SeriesClipRect.Width / 2;
                    var centerY = polarRadarSeries.Area.SeriesClipRect.Height / 2;
                    centerX = centerX - width / 2;
                    centerY = centerY - height / 2;
                    bool isLeft = x < centerX, isBottom = y > centerY;

                    if (x == centerX)
                    {
                        y = isBottom ? y - (height / 2) - padding : y + (height / 2) + padding;
                    }
                    else if (y == centerY)
                    {
                        x = isLeft ? x + (width / 2) + padding : x - (width / 2) - padding;
                    }
                    else if (isLeft)
                    {
                        x = x + (width / 2) + padding;
                        y = isBottom ? y - (height / 2) - padding : y + (height / 2) + padding;
                    }
                    else
                    {
                        x = x - (width / 2) - padding;
                        y = isBottom ? y - (height / 2) - padding : y + (height / 2) + padding;
                    }
                }
            }
            else
            {
                if (series is RangeColumnSeries && !series.IsMultipleYPathRequired)
                {
                    x = x - width / 2;
                    y = y - height / 2;
                    return new Point(x, y);
                }

                switch ((Series as AdornmentSeries).Adornments[index].ActualLabelPosition)
                {
                    case ActualLabelPosition.Top:
                        y = y + padding;
                        x = x - width / 2;
                        break;
                    case ActualLabelPosition.Left:
                        x = x + padding;
                        y = y - height / 2;
                        break;
                    case ActualLabelPosition.Bottom:
                        y = y - height - padding;
                        x = x - width / 2;
                        break;
                    case ActualLabelPosition.Right:
                        x = x - width - padding;
                        y = y - height / 2;
                        break;
                }
            }
            return new Point(x, y);
        }

        #endregion

        #endregion
    }

}
