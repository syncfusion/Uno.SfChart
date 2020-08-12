using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
#if SyncfusionLicense
using Syncfusion.Licensing;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartBase is a base class for chart. Which represents a chart control with basic presentation characteristics. 
    /// </summary>
    public abstract partial class ChartBase : Control, ICloneable, INotifyPropertyChanged
    {
        #region Dependency Property Registrations
        
        /// <summary>
        /// The DependencyProperty for <see cref="AxisThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty AxisThicknessProperty =
            DependencyProperty.Register(
                "AxisThickness", 
                typeof(Thickness), 
                typeof(ChartBase),
                new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// The DependencyProperty for <see cref="Row"/> property.
        /// </summary>
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached(
                "Row",
                typeof(int), 
                typeof(ChartBase),
                new PropertyMetadata(0, OnRowColumnChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ColumnSpan"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached(
                "ColumnSpan",
                typeof(int),
                typeof(ChartBase),
                new PropertyMetadata(1, OnRowColumnChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="RowSpan"/> property.
        /// </summary>
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached(
                "RowSpan", 
                typeof(int), 
                typeof(ChartBase),
                new PropertyMetadata(1, OnRowColumnChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="VisibleSeries"/> property.
        /// </summary>
        public static readonly DependencyProperty VisibleSeriesProperty =
            DependencyProperty.Register(
                "VisibleSeries", 
                typeof(ChartVisibleSeriesCollection),
                typeof(ChartBase), 
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="Palette"/> property.
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register(
                "Palette",
                typeof(ChartColorPalette), 
                typeof(ChartBase),
                new PropertyMetadata(ChartColorPalette.Metro, OnPaletteChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SeriesSelectedIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty SeriesSelectedIndexProperty =
            DependencyProperty.Register(
                "SeriesSelectedIndex", 
                typeof(int),
                typeof(ChartBase),
                new PropertyMetadata(-1, OnSeriesSelectedIndexChanged));
              
        /// <summary>
        /// The DependencyProperty for <see cref="SideBySideSeriesPlacement"/> property.
        /// </summary>
        public static readonly DependencyProperty SideBySideSeriesPlacementProperty =
            DependencyProperty.Register(
                "SideBySideSeriesPlacement",
                typeof(bool), 
                typeof(ChartBase),
                new PropertyMetadata(true, OnSideBySideSeriesPlacementProperty));

        /// <summary>
        /// The DependencyProperty for <see cref="Header"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ChartBase), new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalHeaderAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalHeaderAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalHeaderAlignment",
                typeof(HorizontalAlignment), 
                typeof(ChartBase),
                new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalHeaderAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalHeaderAlignmentProperty =
            DependencyProperty.Register(
                "VerticalHeaderAlignment",
                typeof(VerticalAlignment), 
                typeof(ChartBase),
                new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// The DependencyProperty for <see cref="ColorModel"/> property.
        /// </summary>
        public static readonly DependencyProperty ColorModelProperty =
            DependencyProperty.Register(
                "ColorModel", 
                typeof(ChartColorModel), 
                typeof(ChartBase),
                new PropertyMetadata(null, OnColorModelChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Column"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached(
                "Column", 
                typeof(int),
                typeof(ChartBase),
                new PropertyMetadata(0, OnRowColumnChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Tooltip"/> property.
        /// </summary>
        internal static readonly DependencyProperty TooltipProperty =
            DependencyProperty.Register("Tooltip", typeof(ChartTooltip), typeof(ChartBase), new PropertyMetadata(null));

        #endregion

        #region Fields

        #region Internal Fields

        internal IAsyncAction updateAreaAction;

        internal double InternalDoughnutHoleSize = 0.5;

        internal Canvas AdorningCanvas;

        internal Canvas ToolkitCanvas;

        internal bool ShowTooltip = false;

        internal bool IsTemplateApplied = false;

        internal bool IsUpdateLegend = false;

        internal List<ChartSeriesBase> ActualSeries = new List<ChartSeriesBase>();

        internal List<ChartSeriesBase> SelectedSeriesCollection = new List<ChartSeriesBase>();

        internal ChartSeriesBase CurrentSelectedSeries, PreviousSelectedSeries;

        #endregion

#region Protected Fields
#if !Uno
        protected Printing Printing;
#endif

#endregion

#region Private Fields

#if WINDOWS_UAP
        const double imageResolution = 96.0;
#endif

        private ChartRowDefinitions rowDefinitions;
        
        private ChartColumnDefinitions columnDefinitions;

        private ILayoutCalculator gridLinesLayout;

        private Rect seriesClipRect;

        private ILayoutCalculator chartAxisLayoutPanel;
        
        private double m_minPointsDelta = double.NaN;

        private bool isSbsWithOneData = false;
        
        private Size? rootPanelDesiredSize;

        private ChartAreaType areaType = ChartAreaType.CartesianAxes;

        private ChartSelectionBehavior selectionBehavior;

        private Dictionary<object, int> seriesPosition = new Dictionary<object, int>();

        private bool isLoading = true;

#if NETFX_CORE && !Uno

        private SerializableSfChart serializationController;

#endif
#endregion

#endregion

#region Events

        /// <summary>
        /// Event correspond to series selection. It invokes once selection changed from a series.
        /// </summary>
        /// <remarks>
        ///     <see cref="ChartSelectionChangedEventArgs"/>
        /// </remarks>
        public event EventHandler<ChartSelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Event correspond to series selection. It invokes before selection changing from a series.
        /// </summary>
        /// <remarks>
        ///     <see cref="ChartSelectionChangingEventArgs"/>
        /// </remarks>
        public event EventHandler<ChartSelectionChangingEventArgs> SelectionChanging;

        /// <summary>
        /// Event correspond to plot area bound. It invokes when the plot area size changes.
        /// </summary>
        /// <remarks>
        ///     <see cref="ChartSeriesBoundsEventArgs"/>
        /// </remarks>
        public event EventHandler<ChartSeriesBoundsEventArgs> SeriesBoundsChanged;

        public event PropertyChangedEventHandler PropertyChanged;

#endregion

#region Properties

#region Public Properties

        /// <summary>
        /// Gets or sets thickness to the axis
        /// </summary>
        public Thickness AxisThickness
        {
            get { return (Thickness)GetValue(AxisThicknessProperty); }
            internal set { SetValue(AxisThicknessProperty, value); }
        }      
        
        /// <summary>
        /// Gets a bounding rectangle of chart excluding axis and chart header.
        /// </summary>
        public Rect SeriesClipRect
        {
            get
            {
                return seriesClipRect;
            }
            internal set
            {
                if (seriesClipRect == value) return;
                var oldRect = seriesClipRect;
                seriesClipRect = value;
                OnSeriesBoundsChanged(new ChartSeriesBoundsEventArgs { OldBounds = oldRect, NewBounds = value });
                OnPropertyChanged("SeriesClipRect");
            }
        }
        
        /// <summary>
        /// Gets visible series of chart area.
        /// </summary>
        /// <remarks>
        /// This property is intended to be used for custom <see>
        /// <cref>ChartArea</cref>
        /// </see>
        /// templates.
        /// </remarks>        
        public ChartVisibleSeriesCollection VisibleSeries
        {
            get { return (ChartVisibleSeriesCollection)GetValue(VisibleSeriesProperty); }
            internal set { SetValue(VisibleSeriesProperty, value); }
        }

        /// <summary>
        /// Gets or sets ChartPalette. By default, it is Metro.
        /// </summary>        
        public ChartColorPalette Palette
        {
            get { return (ChartColorPalette)GetValue(PaletteProperty); }
            set { SetValue(PaletteProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the index to select the series.
        /// </summary>        
        public int SeriesSelectedIndex
        {
            get { return (int)GetValue(SeriesSelectedIndexProperty); }
            set { SetValue(SeriesSelectedIndexProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the collection of ChartColumnDefinition objects defined in Chart.
        /// </summary>        
        public ChartColumnDefinitions ColumnDefinitions
        {
            get
            {
                if (columnDefinitions != null) return columnDefinitions;
                columnDefinitions = new ChartColumnDefinitions();
                columnDefinitions.CollectionChanged += OnRowColChanged;

                return columnDefinitions;
            }
            set
            {
                if (columnDefinitions != null)
                {
                    columnDefinitions.CollectionChanged -= OnRowColChanged;
                }
                columnDefinitions = value;
                if (columnDefinitions != null)
                {
                    columnDefinitions.CollectionChanged -= OnRowColChanged;
                }
                ScheduleUpdate();
            }
        }
        
        /// <summary>
        /// Gets or sets the collection of ChartRowDefinition objects defined in Chart
        /// </summary>        
        public ChartRowDefinitions RowDefinitions
        {
            get
            {
                if (rowDefinitions != null) return rowDefinitions;
                rowDefinitions = new ChartRowDefinitions();
                rowDefinitions.CollectionChanged += OnRowColChanged;

                return rowDefinitions;
            }
            set
            {
                if (rowDefinitions != null)
                {
                    rowDefinitions.CollectionChanged -= OnRowColChanged;
                }
                rowDefinitions = value;
                if (rowDefinitions != null)
                {
                    rowDefinitions.CollectionChanged += OnRowColChanged;
                }
                ScheduleUpdate();
            }
        }
        
        /// <summary>
        /// Gets the collection of horizontal and vertical axis.
        /// </summary>
        public ChartAxisCollection Axes { get; internal set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether ChartSeries added to area should be plotted side-by-side.
        /// </summary>        
        public bool SideBySideSeriesPlacement
        {
            get { return (bool)GetValue(SideBySideSeriesPlacementProperty); }
            set { SetValue(SideBySideSeriesPlacementProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets title for chart
        /// </summary>        
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets a value to set the horizontal alignment for the header
        /// </summary>
        public HorizontalAlignment HorizontalHeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalHeaderAlignmentProperty); }
            set { SetValue(HorizontalHeaderAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value to set the vertical alignment for the header
        /// </summary>
        public VerticalAlignment VerticalHeaderAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalHeaderAlignmentProperty); }
            set { SetValue(VerticalHeaderAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets ChartColorModel for entire chart
        /// </summary>        
        public ChartColorModel ColorModel
        {
            get { return (ChartColorModel)GetValue(ColorModelProperty); }
            set { SetValue(ColorModelProperty, value); }
        }

#endregion

#region Internal Properties

        /// <summary>
        /// Gets or sets the intermediate PrimaryAxis object used for internal calculation
        /// </summary>
        internal ChartAxis InternalPrimaryAxis { get; set; }

        /// <summary>
        /// Gets or sets the intermediate DepthAxis object used for internal calculation
        /// </summary>
        internal ChartAxis InternalDepthAxis { get; set; }

        /// <summary>
        /// Gets or sets the intermediate SecondaryAxis object used for internal calculation
        /// </summary>
        internal ChartAxis InternalSecondaryAxis { get; set; }

        internal bool IsChartLoaded { get; set; }

        internal ChartDockPanel ChartDockPanel { get; set; }

        internal List<ChartAxis> DependentSeriesAxes { get; set; }

        internal ChartSelectionBehavior SelectionBehaviour
        {
            get
            {
                if (selectionBehavior == null)
                    SetSelectionBehaviour();
                return selectionBehavior;
            }
            set
            {
                selectionBehavior = value;
            }
        }

        internal bool SBSInfoCalculated //sbs - sidebyside
        {
            get;
            set;
        }
        
        internal Size? RootPanelDesiredSize
        {
            get { return rootPanelDesiredSize; }
            set
            {
                if (rootPanelDesiredSize == value) return;
                rootPanelDesiredSize = value;

                OnRootPanelSizeChanged(value != null ? value.Value : new Size());
            }
        }

        internal Size AvailableSize
        {
            get;
            set;
        }

        internal UpdateAction UpdateAction
        {
            get;
            set;
        }

        internal Dictionary<object, int> SeriesPosition
        {
            get { return seriesPosition; }
            set { seriesPosition = value; }
        }

        internal Dictionary<object, StackingValues> StackedValues { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
        internal int[,] SbsSeriesCount   //sbs  - sidebyside
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the calclulated minimum delta value.
        /// </summary>
        internal double MinPointsDelta
        {
            get
            {
                m_minPointsDelta = double.MaxValue;

                foreach (var series in VisibleSeries)
                {
                    var xValues = (series.ActualXValues as List<double>);
                    if (!series.IsIndexed && xValues != null && series.IsSideBySide) // ColumnSegment width is changed while adding LineSeries dynamically-WPF-19670                 
                        GetMinPointsDelta(xValues, ref m_minPointsDelta, series, series.IsIndexed);
                }

                if (VisibleSeries.Count > 1 && isSbsWithOneData && m_minPointsDelta == double.MaxValue)
                {
                    foreach (var series in VisibleSeries)
                    {
                        var xValues = (series.ActualXValues as List<double>);
                        if (!series.IsIndexed && xValues != null && !series.IsSideBySide)
                            GetMinPointsDelta(xValues, ref m_minPointsDelta, series, series.IsIndexed);
                    }
                    isSbsWithOneData = false;
                }

                m_minPointsDelta = ((m_minPointsDelta == double.MaxValue || m_minPointsDelta < 0) ? 1 : m_minPointsDelta);

                return m_minPointsDelta;
            }
        }

        internal ILayoutCalculator GridLinesLayout
        {
            get { return gridLinesLayout; }
            set { gridLinesLayout = value; }
        }

        /// <summary>
        /// Gets or sets the type of the area.
        /// </summary>
        /// <value>
        /// The type of the area.
        /// </value>
        internal ChartAreaType AreaType
        {
            get
            {
                return areaType;
            }
            set
            {
                if (areaType == value) return;
                areaType = value;
                OnAreaTypeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the chart axis layout panel.
        /// </summary>
        /// <value>
        /// The chart axis layout panel.
        /// </value>
        internal ILayoutCalculator ChartAxisLayoutPanel
        {
            get
            {
                return chartAxisLayoutPanel;
            }
            set
            {
                chartAxisLayoutPanel = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the Current Tooltip object, which is displaying in Chart
        /// </summary>
        internal ChartTooltip Tooltip
        {
            get { return (ChartTooltip)GetValue(TooltipProperty); }
            set { SetValue(TooltipProperty, value); }
        }

        internal bool IsLoading
        {
            get
            {
                return isLoading;
            }

            set
            {
                if (isLoading == value)
                {
                    return;
                }

                isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }
#if !Uno
#if NETFX_CORE
        internal SerializableSfChart SerializationController
        {
            get
            {
                return serializationController;
            }
            set
            {
                serializationController = value;
            }
        }
#endif
#endif
#endregion

#region Protected Internal Properties

        /// <summary>
        /// Gets the selected segments in this series, when we enable the multiple selection.
        /// </summary>
        /// <returns>
        /// It returns <c>List<ChartSegment></c>.
        /// </returns>
        protected internal virtual List<ChartSegment> SelectedSegments
        {
            get
            {
                if (VisibleSeries.Count > 0)
                {
                    return VisibleSeries.Where(series => series.SelectedSegments != null).
                        SelectMany(series => series.SelectedSegments).ToList();
                }
                else
                    return null;
            }
        }

#endregion

#endregion

#region Methods

#region Public Static Methods

        /// <summary>
        /// Return int value from the given ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetRow(UIElement obj)
        {
            return (int)obj.GetValue(RowProperty);
        }

        /// <summary>
        /// Method implementation for Set Row value to ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>        
        public static void SetRow(UIElement obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        /// <summary>
        /// Get the column value from the given ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>        
        public static int GetColumn(UIElement obj)
        {
            return (int)obj.GetValue(ColumnProperty);
        }

        /// <summary>
        /// Gets the value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property from a given UIElement. 
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property.</returns>        
        public static int GetColumnSpan(UIElement element)
        {
            return (int)element.GetValue(ColumnSpanProperty);
        }

        /// <summary>
        /// Gets the value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property from a given UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property.</returns>        
        public static int GetRowSpan(UIElement element)
        {
            return (int)element.GetValue(RowSpanProperty);
        }

        /// <summary>
        /// Set column to ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>        
        public static void SetColumn(UIElement obj, int value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        /// <summary>
        /// Sets the value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property
        //     to a given UIElement.
        /// </summary>
        /// <param name="element"> The element on which to set the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property.</param>
        /// <param name="value">The property value to set.</param>        
        public static void SetColumnSpan(UIElement element, int value)
        {
            element.SetValue(ColumnSpanProperty, value);
        }

        /// <summary>
        /// Sets the value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property
        //     to a given UIElement.
        /// </summary>
        /// <param name="element"> The element on which to set the Syncfusion.UI.Xaml.Charts.RowSpan attached property.</param>
        /// <param name="value">The property value to set.</param>        
        public static void SetRowSpan(UIElement element, int value)
        {
            element.SetValue(RowSpanProperty, value);
        }

#endregion

#region Public Methods

        /// <summary>
        /// This method will suspend all the series from updating the data till ResumeNotification is called. This is specifically used when we need to append collection of datas.
        /// </summary>        
        public void SuspendSeriesNotification()
        {
            if (ActualSeries != null)
                foreach (ChartSeriesBase series in this.ActualSeries)
                {
                    series.SuspendNotification();
                }
        }
        
        /// <summary>
        /// Processes the data that is added to data source after SuspendSeriesNotification.
        /// </summary>        
        public void ResumeSeriesNotification()
        {
            if (ActualSeries != null)
                foreach (ChartSeriesBase series in this.ActualSeries)
                {
                    series.ResumeNotification();
                }
        }

        /// <summary>
        /// Clone the entire chart control
        /// </summary>
        public DependencyObject Clone()
        {
            return CloneChart();
        }

        /// <summary>
        /// Returns the stacked value of the series.
        /// </summary>
        /// <param name="series">ChartSeries</param>
        /// <param name="reqNegStack">RequiresNegativeStack</param>
        /// <returns>StackedYValues collection</returns>
        public List<double> GetCumulativeStackInfo(ChartSeriesBase series, bool reqNegStack)
        {
            if (series != null)
            {
                var y = series.ActualYAxis.Origin;
                double currtY;
                var calcYValues = new List<double>();

                foreach (var ser in VisibleSeries)
                {
                    var yValues = ((XyDataSeries)ser).YValues;
                    if (ser.ActualXValues != null)
                    {
                        if (calcYValues.Count > 0)
                        {
                            for (var i = 0; i < ser.DataCount; i++)
                            {
                                currtY = reqNegStack ? Math.Abs(yValues[i]) : yValues[i];
                                if (i < calcYValues.Count)
                                    calcYValues[i] += currtY + y;
                                else
                                    calcYValues.Add(currtY + y);
                            }
                        }
                        else
                        {
                            for (var i = 0; i < ser.DataCount; i++)
                            {
                                currtY = reqNegStack ? Math.Abs(yValues[i]) : yValues[i];
                                calcYValues.Add(currtY + y);
                            }
                        }
                        if (series == ser)
                            break;
                    }
                }
                return calcYValues;
            }
            return null;
        }
#if !Uno
#if NETFX_CORE

        public void Print()
        {
            Printing.Print();
        }

#endif

        /// <summary>
        /// Method used to generate a serialize file in default location
        /// </summary>
        /// <param name="file"></param>
        public void Serialize(StorageFile file)
        { 
            this.SerializationController = new SerializableSfChart(this as SfChart);
            this.serializationController.Serialize(file);
        }

        /// <summary>
        /// Method used to generate a serialize file in specified location
        /// </summary>
        /// <param name="stream"></param>
        public void Serialize(Stream stream)
        {
            this.SerializationController = new SerializableSfChart(this as SfChart);
            this.serializationController.Serialize(stream);
        }

        /// <summary>
        /// Method used to deserialize the serialized file
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        public object Deserialize(Stream stream)
        {
            SerializationController = new SerializableSfChart();
            this.serializationController.Deserialize(stream);
            return serializationController.Chart;
        }

        /// <summary>
        /// Method used to deserialize the serialized file
        /// </summary>
        /// <param name="storageFile"></param>
        /// <returns></returns>
        public object Deserialize(StorageFile storageFile)
        {
            SerializationController = new SerializableSfChart();
            SerializationController.Deserialize(storageFile);
            //To hold the process until Chart control deserialized
            while (!serializationController.TaskDone)
                Task.Delay(10);
            return SerializationController.Chart;
        }

#if WINDOWS_UAP
        /// <summary>
        /// Exports the SfChart image in the selected location by using FileSavePicker
        /// </summary>
        public async void Save()
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(this);
            var pixels = await renderTargetBitmap.GetPixelsAsync();

            //Initialize fileSavePicker with the image formates and its default file name
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("BMP", new List<string>() { ".bmp" });
            fileSavePicker.FileTypeChoices.Add("GIF", new List<string>() { ".gif" });
            fileSavePicker.FileTypeChoices.Add("PNG", new List<string>() { ".png" });
            fileSavePicker.FileTypeChoices.Add("JPG", new List<string>() { ".jpg" });
            fileSavePicker.FileTypeChoices.Add("JPG-XR", new List<string>() { ".jxr" });
            fileSavePicker.FileTypeChoices.Add("TIFF", new List<string>() { ".tiff" });
            fileSavePicker.SuggestedFileName = SfChartResourceWrapper.FileName;

            var file = await fileSavePicker.PickSaveFileAsync();
            if (file != null)
            {
                Guid encoderId = GetBitmapEncoderId(file.FileType);
                using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
                    encoder.SetPixelData(
                        BitmapPixelFormat.Bgra8, 
                        BitmapAlphaMode.Ignore, 
                        (uint)renderTargetBitmap.PixelWidth,
                        (uint)renderTargetBitmap.PixelHeight, 
                        imageResolution,
                        imageResolution,
                        pixels.ToArray());
                    await encoder.FlushAsync();
                }
            }
        }
        
        /// <summary>
        /// Export the SfChart image with the given name and the specified location.
        /// </summary>
        /// <param name="fileName">Name of the image file.</param>
        /// <param name="folderLocation">Specifies the location to save. Default location:Installed Location.</param>
        public async void Save(string fileName, StorageFolder folderLocation)
        {
            StorageFolder storageFolder;
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(this);
            var pixels = await renderTargetBitmap.GetPixelsAsync();
#if WINDOWS_UAP && CHECKLATER
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                storageFolder = folderLocation != null ? folderLocation : KnownFolders.PicturesLibrary;
            else
                storageFolder = folderLocation != null ? folderLocation : Windows.ApplicationModel.Package.Current.InstalledLocation;
#else
            storageFolder = folderLocation != null ? folderLocation : Windows.ApplicationModel.Package.Current.InstalledLocation;
#endif
            var file = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            Guid encoderId = GetBitmapEncoderId(file.FileType.ToLower());
            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8, 
                    BitmapAlphaMode.Ignore, 
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight, 
                    imageResolution,
                    imageResolution,
                    pixels.ToArray());
                await encoder.FlushAsync();
            }
        }

        /// <summary>
        /// Export the SfChart image using the stream and encoder.
        /// </summary>
        /// <param name="stream">Image Stream</param>
        /// <param name="bitmapEncoderID">BitmapEncoder ID</param>
        public async void Save(IRandomAccessStream stream, Guid bitmapEncoderID)
        {
            if (stream == null) return;
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(this);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            var dataWriter = new Windows.Storage.Streams.DataWriter(stream);
            var encoder = await BitmapEncoder.CreateAsync(bitmapEncoderID, stream);
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Ignore, 
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight, 
                imageResolution, 
                imageResolution, 
                pixelBuffer.ToArray());
            await encoder.FlushAsync();
        }

#endif
#endif
        #endregion

        #region Public Virtual Methods

        /// <summary>
        /// Method used to highlight selected index series.
        /// </summary>
        /// <param name="args"></param>        
        public virtual void SeriesSelectedIndexChanged(int newIndex, int oldIndex)
        {

        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>        
        public virtual double ValueToPoint(ChartAxis axis, double value)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    if (axis.ActualWidth == 0)
                        return axis.ValueToCoefficientCalc(value) * axis.Area.SeriesClipRect.Width;
                    return (axis.RenderedRect.Left - axis.Area.SeriesClipRect.Left)
                        + (axis.ValueToCoefficientCalc(value) * axis.RenderedRect.Width);
                }
                return (axis.RenderedRect.Top - axis.Area.SeriesClipRect.Top) + (1 - axis.ValueToCoefficientCalc(value)) * axis.RenderedRect.Height;
            }
            return double.NaN;
        }

        /// <summary>
        /// Converts point to value.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="point">The point.</param>
        /// <returns>The double point to value</returns>        
        public virtual double PointToValue(ChartAxis axis, Point point)
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

#region Internal Static Methods

#if UNIVERSALWINDOWS && SyncfusionLicense

        internal static async void ValidateLicense()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return;

            var licenseMessage = FusionLicenseProvider.GetLicenseType(Platform.UWP);
            if (!string.IsNullOrEmpty(licenseMessage))
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(licenseMessage);
                dialog.Title = "Syncfusion License";
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK", (action) => { }));
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("HELP", (action) =>
                {
                    OpenHelpLink();

                }));
                await dialog.ShowAsync();
            }
        }

#endif

#endregion

#region Internal Virtual Methods

        /// <summary>
        /// Clone the entire chart
        /// </summary>
        internal virtual DependencyObject CloneChart()
        {
            return null;
        }

        /// <summary>
        /// Update the chart area
        /// </summary>
        internal virtual void UpdateArea(bool forceUpdate)
        {

        }
        
        /// <summary>
        /// Converts Value to Log point.
        /// </summary>
        /// <param name="axis">The Logarithmic axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>        
        internal virtual double ValueToLogPoint(ChartAxis axis, double value)
        {
            return double.NaN;
        }

        /// <summary>
        /// Updates the entire chart series and axis
        /// </summary>
        internal virtual void UpdateAxisLayoutPanels()
        {

        }

#endregion

#region Internal Methods

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void DisposeRowColumnsDefinitions()
        {
            if (rowDefinitions != null)
            {
                foreach (var row in rowDefinitions)
                {
                    row.Dispose();
                }

                rowDefinitions.CollectionChanged -= OnRowColChanged;
                rowDefinitions.Clear();
            }

            if (columnDefinitions != null)
            {
                foreach (var column in columnDefinitions)
                {
                    column.Dispose();
                }

                columnDefinitions.Clear();
                columnDefinitions.CollectionChanged -= OnRowColChanged;
            }
        }
        
        internal void DisposeSelectionEvents()
        {
            if(SelectionChanged != null)
            {
                foreach(var handler in SelectionChanged.GetInvocationList())
                {
                    SelectionChanged -= handler as EventHandler<ChartSelectionChangedEventArgs>;
                }

                SelectionChanged = null;              
            }

            if(SelectionChanging != null)
            {
                foreach(var handler in SelectionChanging.GetInvocationList())
                {
                    SelectionChanging -= handler as EventHandler<ChartSelectionChangingEventArgs>;
                }

                SelectionChanging = null;
            }

            if(SeriesBoundsChanged != null)
            {
                foreach(var handler in SeriesBoundsChanged.GetInvocationList())
                {
                    SeriesBoundsChanged -= handler as EventHandler<ChartSeriesBoundsEventArgs>;
                }

                SeriesBoundsChanged = null;
            }
        }

        /// <summary>
        /// Return actual row value from the given ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal int GetActualRow(UIElement obj)
        {
            var actualPos = RowDefinitions.Count;
            var pos = GetRow(obj);
            var result = pos >= actualPos ? actualPos - 1 : (pos < 0 ? 0 : pos);
            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// Return actual column value from the given ChartAxis
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>        
        internal int GetActualColumn(UIElement obj)
        {
            var actualPos = ColumnDefinitions.Count;
            var pos = GetColumn(obj);
            var result = pos >= actualPos ? actualPos - 1 : (pos < 0 ? 0 : pos);
            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// Gets the actual value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property from a given UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Syncfusion.UI.Xaml.Charts.ColumnSpan attached property.</returns>        
        internal int GetActualColumnSpan(UIElement element)
        {
            var count = ColumnDefinitions.Count;
            var span = GetColumnSpan(element);
            return span > count ? count : (span < 0 ? 0 : span);
        }
        /// <summary>
        /// Gets the actual value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property from a given UIElement.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Syncfusion.UI.Xaml.Charts.RowSpan attached property.</returns>        
        internal int GetActualRowSpan(UIElement obj)
        {
            var count = RowDefinitions.Count;
            var span = GetRowSpan(obj);
            return span > count ? count : (span < 0 ? 0 : span);
        }

        internal void GetMinPointsDelta(List<double> values, ref double minPointsDelta, ChartSeriesBase series, bool isIndexed)
        {
            if (!series.isLinearData) // WPF 17950 Series is not rendered properly while adding data statically and dynamically between the DateTime Range
            {
                values = values.ToList();
                values.Sort();
            }

            if (values.Count == 1)
                isSbsWithOneData = true;

            for (var i = 1; i < values.Count; i++)
            {
                var delta = values[i] - values[i - 1];
                if (delta != 0 && !double.IsNaN(delta))
                {
                    minPointsDelta = Math.Min(minPointsDelta, delta);
                }
            }
        }

        internal Canvas GetAdorningCanvas()
        {
            return AdorningCanvas;
        }

        /// <summary>
        /// Method used to get brush for series selection.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal Brush GetSeriesSelectionBrush(ChartSeriesBase series)
        {
            if (this is SfChart && SelectionBehaviour != null)
                return SelectionBehaviour.GetSeriesSelectionBrush(series);

            return null;
        }

        /// <summary>
        /// Method used to get EnableSeriesSelection property value.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal bool GetEnableSeriesSelection()
        {
            if (this is SfChart && SelectionBehaviour != null)
                return SelectionBehaviour.EnableSeriesSelection;

            return false;
        }

        /// <summary>
        /// Method used to get EnableSegmentSelection property value.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal bool GetEnableSegmentSelection()
        {
            if (this is SfChart && SelectionBehaviour != null)
                return SelectionBehaviour.EnableSegmentSelection;

            return false;
        }

        internal int GetSeriesIndex(ChartSeriesBase series)
        {
            return ActualSeries.IndexOf(series);
        }

        /// <summary>
        /// Method used to Set selection behavior.
        /// </summary>
        /// <returns></returns>
        internal void SetSelectionBehaviour()
        {
            var behaviors = (from behavior in (this as SfChart).Behaviors
                             where behavior is ChartSelectionBehavior
                             select behavior).ToList();

            if (behaviors != null && behaviors.Count > 0)
                SelectionBehaviour = behaviors[0] as ChartSelectionBehavior;
            else
                SelectionBehaviour = null;
        }

        internal void ScheduleUpdate()
        {
            var _isInDesignMode = DesignMode.DesignModeEnabled;
            if (updateAreaAction == null && !_isInDesignMode)
            {
                updateAreaAction = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateArea);
            }
            else if (_isInDesignMode)
                UpdateArea(true);
        }


#if NETFX_CORE
        /// <summary>
        /// Renders the chart using composition rendering.
        /// </summary>
        internal void CompositionScheduleUpdate()
        {
            if (updateAreaAction == null)
            {
                CompositionTarget.Rendering -= this.SmoothCompositeRendering;
                CompositionTarget.Rendering += this.SmoothCompositeRendering;
            }
        }
       
#endif

        internal void UpdateArea()
        {
            UpdateArea(false);
        }

        /// <summary>
        /// Method to raise SelectionChanged event when SeriesSelectedIndex is set at chart load time.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal void RaiseSeriesSelectionChangedEvent()
        {
            ChartSeriesBase series = null;
            if (SeriesSelectedIndex < (this as SfChart).Series.Count)
                series = (this as SfChart).Series[this.SeriesSelectedIndex] as ChartSeriesBase;
            OnSelectionChanged(new ChartSelectionChangedEventArgs()
            {
                SelectedSegment = null,
                SelectedSeries = series,
                SelectedSeriesCollection = SelectedSeriesCollection,
                SelectedIndex = SeriesSelectedIndex,
                PreviousSelectedIndex = -1,
                IsDataPointSelection = false
            });
        }

#endregion
        
#region Protected Internal Virtual Methods

        protected internal virtual void OnSeriesBoundsChanged(ChartSeriesBoundsEventArgs args)
        {
            if (SeriesBoundsChanged != null && args != null)
                SeriesBoundsChanged(this, args);
        }
        
        /// <summary>
        /// Called when Selection changed in SfChart
        /// </summary>
        /// <param name="eventArgs"></param>
        protected internal virtual void OnSelectionChanged(ChartSelectionChangedEventArgs eventArgs)
        {

            if (this is SfChart && SelectionBehaviour != null)
            {
                if (SelectionChanged != null && eventArgs != null)
                    SelectionChanged(this, eventArgs);

                SelectionBehaviour.OnSelectionChanged(eventArgs);
            }
        }

        /// <summary>
        /// Called when Selection changed in SfChart
        /// </summary>
        /// <param name="eventArgs"></param>
        protected internal virtual void OnBoxSelectionChanged(ChartSelectionChangedEventArgs eventArgs)
        {
            if (SelectionChanged != null && eventArgs != null)
                SelectionChanged(this, eventArgs);

            if (SelectionBehaviour != null)
                SelectionBehaviour.OnSelectionChanged(eventArgs);

        }

        /// <summary>
        /// It's a preview event before SelectionChanged.
        /// </summary>
        /// <param name="eventArgs"></param>
        protected internal virtual void OnSelectionChanging(ChartSelectionChangingEventArgs eventArgs)
        {
            if (SelectionChanging != null && eventArgs != null)
                SelectionChanging(this, eventArgs);

            if (this is SfChart)
            {
                if (SelectionBehaviour != null)
                    SelectionBehaviour.OnSelectionChanging(eventArgs);
            }
        }

#endregion

#region Protected Virtual Methods

        /// <summary>
        /// Called when [root panel size changed].
        /// </summary>
        /// <param name="size">The size.</param>
        protected virtual void OnRootPanelSizeChanged(Size size)
        {
            if (!IsTemplateApplied || !RootPanelDesiredSize.HasValue) return;
            UpdateAction |= UpdateAction.LayoutAndRender;

            UpdateArea(true);
        }

#endregion

#region Private Static Methods

#if UNIVERSALWINDOWS && SyncfusionLicense

        private static async void OpenHelpLink()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(@"https://help.syncfusion.com/es/licensing/"));
        }

#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnRowColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d is ChartAxis ? (d as ChartAxis).Area
                            : (d is ChartLegend) ? (d as ChartLegend).ChartArea : null;
            if (chartArea != null)
                chartArea.ScheduleUpdate();
        }

        private static void OnSeriesSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartBase chartBase = d as ChartBase;
            chartBase.SeriesSelectedIndexChanged((int)args.NewValue, (int)args.OldValue);

        }

        private static void OnSideBySideSeriesPlacementProperty(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((ChartBase)d).ScheduleUpdate();
        }

        private static void OnColorModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartBase = d as ChartBase;
            if (chartBase.ColorModel != null)
            {
                chartBase.ColorModel.Palette = chartBase.Palette;
                chartBase.ColorModel.ChartArea = chartBase;
            }
        }

        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartBase)d).OnPaletteChanged(e);
        }

#endregion

#region Private Methods

#if WINDOWS_UAP

  private static Guid GetBitmapEncoderId(string type)
        {
            switch (type)
            {
                case ".bmp":
                    return BitmapEncoder.BmpEncoderId;
                case ".jpg":
                case ".jpeg":
                    return BitmapEncoder.JpegEncoderId;
                case ".jxr":
                    return BitmapEncoder.JpegXREncoderId;
                case ".png":
                    return BitmapEncoder.PngEncoderId;
                case ".tif":
                case ".tiff":
                    return BitmapEncoder.TiffEncoderId;
            }
            return BitmapEncoder.BmpEncoderId;
        }

#endif

        private void OnPaletteChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ColorModel != null)
                ColorModel.Palette = Palette;
            else
                ColorModel = new ChartColorModel(Palette);

            if (this.VisibleSeries.Count > 0)//ColorModel custom brush dynamic update not working in native control.-WP-610
            {
                for (int index = 0; index < VisibleSeries.Count; index++)
                {
                    (this.VisibleSeries[index] as ChartSeriesBase).Segments.Clear();
                }
                IsUpdateLegend = true;
                ScheduleUpdate();
            }
        }
        
#if NETFX_CORE
        
        /// <summary>
        /// Handler for the composition rendering.
        /// </summary>
        /// <param name="sender">The source of the composition rendering.</param>
        /// <param name="e">The details of the composition rendering.</param>
        private void SmoothCompositeRendering(object sender, object e)
        {
            CompositionTarget.Rendering -= this.SmoothCompositeRendering;
            updateAreaAction = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateArea);
        }
#endif

        private void OnAreaTypeChanged()
        {
            UpdateAxisLayoutPanels();
        }

        void OnRowColChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleUpdate();
        }

#endregion

#endregion

    }
}
