using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Input;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartZoomPanBehavior enables zooming and panning operations over a Chart.
    /// </summary>
    /// <remarks>
    /// Zooming and panning operations can be initiated and can be restored backed to
    /// the original position by performing zoom out operation or by enabling <see
    /// cref="ChartZoomPanBehavior.ResetOnDoubleTap"/> property. 
    /// <para>Chart can also be zoomed, without adding ChartZoomPanBehavior, by
    /// specifying following properties <see cref="ChartAxis.ZoomFactor"/> and <see
    /// cref="ChartAxis.ZoomPosition"/> for the ChartAxis. By specifying zooming mode
    /// using <see cref="ChartZoomPanBehavior.ZoomMode"/> property, zooming operation
    /// can be performed along horizontal or along vertical or along both directions in
    /// a Chart.</para>
    /// </remarks>
    public partial class ChartZoomPanBehavior : ChartBehavior
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarItems"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarItemsProperty =
            DependencyProperty.Register(
                "ToolBarItems",
                typeof(ZoomToolBarItems),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(ZoomToolBarItems.All, OnToolBarItemsChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableZoomingToolBar"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableZoomingToolBarProperty =
            DependencyProperty.Register(
                "EnableZoomingToolBar",
                typeof(bool),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(false, new PropertyChangedCallback(OnToolbarPropertyChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarItemHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarItemHeightProperty =
            DependencyProperty.Register(
                "ToolBarItemHeight",
                typeof(double),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(25d));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarItemWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarItemWidthProperty =
            DependencyProperty.Register(
                "ToolBarItemWidth",
                typeof(double),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(25d));


        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarItemMargin"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarItemMarginProperty =
            DependencyProperty.Register(
                "ToolBarItemMargin",
                typeof(Thickness),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(new Thickness(7)));

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalPositionProperty =
            DependencyProperty.Register(
                "HorizontalPosition",
                typeof(HorizontalAlignment),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(HorizontalAlignment.Right, new PropertyChangedCallback(OnAlignmentPropertyChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalPositionProperty =
            DependencyProperty.Register(
                "VerticalPosition",
                typeof(VerticalAlignment),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(VerticalAlignment.Top, new PropertyChangedCallback(OnAlignmentPropertyChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarOrientation"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarOrientationProperty =
            DependencyProperty.Register(
                "ToolBarOrientation",
                typeof(Orientation),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarBackground"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarBackgroundProperty =
            DependencyProperty.Register(
                "ToolBarBackground",
                typeof(SolidColorBrush),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(null, OnToolBarBackgroundChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ZoomRelativeToCursor"/> property.
        /// </summary>
        public static readonly DependencyProperty ZoomRelativeToCursorProperty =
            DependencyProperty.Register(
                "ZoomRelativeToCursor",
                typeof(bool),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(true));

        /// <summary>
        /// The DependencyProperty for <see cref="EnablePinchZooming"/> property.
        /// </summary>
        public static readonly DependencyProperty EnablePinchZoomingProperty =
            DependencyProperty.Register(
                "EnablePinchZooming",
                typeof(bool),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(true));

        /// <summary>
        /// The DependencyProperty for <see cref="ZoomMode"/> property.
        /// </summary>
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register(
                "ZoomMode",
                typeof(ZoomMode),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(ZoomMode.XY));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableDirectionalZooming"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableDirectionalZoomingProperty =
            DependencyProperty.Register(
                "EnableDirectionalZooming",
                typeof(bool),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(false));

        /// <summary>
        /// The DependencyProperty for <see cref="EnablePanning"/> property.
        /// </summary>
        public static readonly DependencyProperty EnablePanningProperty =
            DependencyProperty.Register(
                "EnablePanning",
                typeof(bool),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(true));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness",
                typeof(double),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(1d));

        /// <summary>
        /// The DependencyProperty for <see cref="MaximumZoomLevel"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumZoomLevelProperty =
            DependencyProperty.Register(
                "MaximumZoomLevel",
                typeof(double),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(double.NaN));

        /// <summary>
        /// The DependencyProperty for <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke",
                typeof(Brush),
                typeof(ChartZoomPanBehavior),
#if UNIVERSALWINDOWS
                null);
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
#endif

        /// <summary>
        /// The DependencyProperty for <see cref="Fill"/> property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                "Fill",
                typeof(Brush),
                typeof(ChartZoomPanBehavior),
#if UNIVERSALWINDOWS
                null);
#else
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(100, 150, 200, 200))));
#endif

        /// <summary>
        /// The DependencyProperty for <see cref="EnableSelectionZooming"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableSelectionZoomingProperty =
            DependencyProperty.Register(
                "EnableSelectionZooming",
                typeof(bool),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(false, new PropertyChangedCallback(OnZoomSelectionChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ResetOnDoubleTap"/> property.
        /// </summary>
        public static readonly DependencyProperty ResetOnDoubleTapProperty =
            DependencyProperty.Register(
                "ResetOnDoubleTap",
                typeof(bool),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(true));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableMouseWheelZooming"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableMouseWheelZoomingProperty =
            DependencyProperty.Register(
                "EnableMouseWheelZooming",
                typeof(bool),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(true));

        /// <summary>
        /// The DependencyProperty for <see cref="KeyModifiers"/> property.
        /// </summary>
        public static readonly DependencyProperty KeyModifiersProperty =
            DependencyProperty.Register(
                "KeyModifiers",
                typeof(VirtualKeyModifiers),
                typeof(ChartZoomPanBehavior),
                new PropertyMetadata(VirtualKeyModifiers.None));

        #endregion

        #region Fields

        #region internal Fields

        internal bool IsActive; // maintain the state for zooming toolbar

        #endregion

        #region private Fields

        private bool enablePanning = true;

        private bool enableSelectionZooming = true;

        private Rectangle selectionRectangle;

        private ZoomChangingEventArgs zoomChangingEventArgs;

        private ZoomChangedEventArgs zoomChangedEventArgs;

        private PanChangingEventArgs panChangingEventArgs;

        private PanChangedEventArgs panChangedEventArgs;

        private SelectionZoomingStartEventArgs sel_ZoomingStartEventArgs;

        private SelectionZoomingDeltaEventArgs sel_ZoomingDeltaEventArgs;

        private SelectionZoomingEndEventArgs sel_ZoomingEndEventArgs;

        private ResetZoomEventArgs zoomingResetEventArgs;

        private bool isPanningChanged;

        private bool isReset;

        private Point startPoint;

        private bool isZooming;

        bool isTransposed = false;

        Rect zoomRect;

        private double previousScale;

        private Rect areaRect;

        private double angle;

        private List<PointerPoint> pointers = new List<PointerPoint>();

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for ChartZoomPanBehavior
        /// </summary>
        public ChartZoomPanBehavior()
        {
#if UNIVERSALWINDOWS
            Fill = new SolidColorBrush(Color.FromArgb(100, 150, 200, 200));
            Stroke = new SolidColorBrush(Colors.Gray);
#endif
            zoomChangingEventArgs = new ZoomChangingEventArgs();
            zoomChangedEventArgs = new ZoomChangedEventArgs();
            panChangedEventArgs = new PanChangedEventArgs();
            panChangingEventArgs = new PanChangingEventArgs();
            sel_ZoomingStartEventArgs = new SelectionZoomingStartEventArgs();
            sel_ZoomingDeltaEventArgs = new SelectionZoomingDeltaEventArgs();
            sel_ZoomingEndEventArgs = new SelectionZoomingEndEventArgs();
            zoomingResetEventArgs = new ResetZoomEventArgs();
            selectionRectangle = new Rectangle { IsHitTestVisible = false };
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value which is used to select the tool bar items respectively
        /// </summary>
        public ZoomToolBarItems ToolBarItems
        {
            get { return (ZoomToolBarItems)GetValue(ToolBarItemsProperty); }
            set { SetValue(ToolBarItemsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show/hide the zooming tool bar.
        /// </summary>
        /// /// <value>
        ///   <c>true</c> if [enable zooming toolbar]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableZoomingToolBar
        {
            get { return (bool)GetValue(EnableZoomingToolBarProperty); }
            set { SetValue(EnableZoomingToolBarProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Height for the ZoomingToolBar items.
        /// </summary>
        public double ToolBarItemHeight
        {
            get { return (double)GetValue(ToolBarItemHeightProperty); }
            set { SetValue(ToolBarItemHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Width for the ZoomingToolBar items.
        /// </summary>
        public double ToolBarItemWidth
        {
            get { return (double)GetValue(ToolBarItemWidthProperty); }
            set { SetValue(ToolBarItemWidthProperty, value); }
        }

        /// <summary>
        ///  Gets or sets the Margin for the ZoomingToolBar items.
        /// </summary>
        public Thickness ToolBarItemMargin
        {
            get { return (Thickness)GetValue(ToolBarItemMarginProperty); }
            set { SetValue(ToolBarItemMarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal position for the tool bar
        /// </summary>
        public HorizontalAlignment HorizontalPosition
        {
            get { return (HorizontalAlignment)GetValue(HorizontalPositionProperty); }
            set { SetValue(HorizontalPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical position for the tool bar.
        /// </summary>
        public VerticalAlignment VerticalPosition
        {
            get { return (VerticalAlignment)GetValue(VerticalPositionProperty); }
            set { SetValue(VerticalPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tool bar orientation
        /// </summary>
        public Orientation ToolBarOrientation
        {
            get { return (Orientation)GetValue(ToolBarOrientationProperty); }
            set { SetValue(ToolBarOrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tool bar background
        /// </summary>
        public SolidColorBrush ToolBarBackground
        {
            get { return (SolidColorBrush)GetValue(ToolBarBackgroundProperty); }
            set { SetValue(ToolBarBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [zoom relative to mouse pointer] and this is applicable only for mouse wheel zooming.
        /// </summary>
        /// <value>
        /// <c>true</c> if [zoom relative to mouse pointer]; otherwise, <c>false</c>.
        /// </value>
        public bool ZoomRelativeToCursor
        {
            get { return (bool)GetValue(ZoomRelativeToCursorProperty); }
            set { SetValue(ZoomRelativeToCursorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the finger gesture is enabled.
        /// </summary>
        /// <value>This property takes the boolean value, and its default value is true.
        /// If this property is true, zooming is performed based on pinch gesture of the user.
        /// If this property is false, zooming is performed based on the mouse wheel of the user.
        /// </value>
        public bool EnablePinchZooming
        {
            get { return (bool)GetValue(EnablePinchZoomingProperty); }
            set { SetValue(EnablePinchZoomingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the zoom mode.
        /// </summary>
        public ZoomMode ZoomMode
        {
            get { return (ZoomMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value that indicates whether directional zooming is enabled.
        /// </summary>
        /// <remarks>
        /// If this property is false, zooming is performed based on <see cref="ZoomMode"/> property. If this property is true, zooming is performed based on pinch direction of the user.
        /// This property having effect only with <see cref="ZoomMode"/> value as <c>XY</c>
        /// </remarks>
        public bool EnableDirectionalZooming
        {
            get { return (bool)GetValue(EnableDirectionalZoomingProperty); }
            set { SetValue(EnableDirectionalZoomingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Enable/Disable panning. 
        /// </summary>
        public bool EnablePanning
        {
            get { return (bool)GetValue(EnablePanningProperty); }
            set { SetValue(EnablePanningProperty, value); }
        }

        /// <summary>
        /// Gets or sets stroke thickness for selection rectangle.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets Maximum Zoom Level.
        /// </summary>
        public double MaximumZoomLevel
        {
            get { return (double)GetValue(MaximumZoomLevelProperty); }
            set { SetValue(MaximumZoomLevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets stroke for selection rectangle.
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
        /// Gets or sets the background for selection rectangle.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable zooming chart using selection rectangle.
        /// </summary>
        public bool EnableSelectionZooming
        {
            get { return (bool)GetValue(EnableSelectionZoomingProperty); }
            set { SetValue(EnableSelectionZoomingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to reset the zooming when press the mouse left button twice
        /// </summary>
        public bool ResetOnDoubleTap
        {
            get { return (bool)GetValue(ResetOnDoubleTapProperty); }
            set { SetValue(ResetOnDoubleTapProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether mouse wheel zooming is enabled.
        /// </summary>
        public bool EnableMouseWheelZooming
        {
            get { return (bool)GetValue(EnableMouseWheelZoomingProperty); }
            set { SetValue(EnableMouseWheelZoomingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the key modifiers used for zooming.
        /// </summary>
        public VirtualKeyModifiers KeyModifiers
        {
            get { return (VirtualKeyModifiers)GetValue(KeyModifiersProperty); }
            set { SetValue(KeyModifiersProperty, value); }
        }
        #endregion

        #region Internal Properties
        
        internal bool InternalEnablePanning
        {
            get { return enablePanning && EnablePanning; }
            set { enablePanning = value; }
        }
        
        internal ZoomingToolBar ChartZoomingToolBar
        {
            get;
            set;
        }
        
        internal bool InternalEnableSelectionZooming
        {
            get { return enableSelectionZooming && EnableSelectionZooming; }
            set { enableSelectionZooming = value; }
        }

        #endregion

        #region Private Properties

        private bool IsMaxZoomLevel
        {
            get { return !double.IsNaN(MaximumZoomLevel); }
        }
        
        private bool HorizontalMode
        {
            get { return (ZoomMode == ZoomMode.X && !isTransposed) || (ZoomMode == ZoomMode.Y && isTransposed); }
        }

        private bool VerticalMode
        {
            get { return (ZoomMode == ZoomMode.Y && !isTransposed) || (ZoomMode == ZoomMode.X && isTransposed); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Virtual Methods

        /// <summary>
        /// Return bool value from the given ChartAxis
        /// </summary>
        /// <param name="cumulativeScale"></param>
        /// <param name="origin"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public virtual bool Zoom(double cumulativeScale, double origin, ChartAxisBase2D axis)
        {
            ////if (cumulativeScale >= 1d && cumulativeScale <= 4d)
            if (cumulativeScale >= 1d && axis != null)
            {
                double calcZoomPos = 0;
                double calcZoomFactor = 0;
                double previousPosition = axis.ZoomPosition;
                double previousFactor = axis.ZoomFactor;

                CalZoomFactors(cumulativeScale, origin, previousFactor, previousPosition, ref calcZoomPos, ref calcZoomFactor);

                var newZoomFactor = (calcZoomPos + calcZoomFactor) > 1 ? 1 - calcZoomPos : calcZoomFactor;

                RaiseZoomChangingEvent(calcZoomPos, newZoomFactor, axis);

                if (!zoomChangingEventArgs.Cancel && (axis.ZoomPosition != calcZoomPos || axis.ZoomFactor != calcZoomFactor))
                {
                    axis.ZoomPosition = calcZoomPos;
                    axis.ZoomFactor = newZoomFactor;
                    RaiseZoomChangedEvent(previousFactor, previousPosition, axis);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Zooms the specified cumulative scale.
        /// </summary>
        /// <param name="cumulativeScale">The cumulative scale.</param>
        /// <param name="axis">The axis.</param>
        /// <returns></returns>
        public bool Zoom(double cumulativeScale, ChartAxisBase2D axis)
        {
            return Zoom(cumulativeScale, 0.5, axis);
        }

        /// <summary>
        /// Resets the zoom factor and zoom position for all the axis.
        /// </summary>
        public void Reset()
        {
            foreach (ChartAxisBase2D axis in ChartArea.Axes)
            {
                RaiseResetZoomingEvent(axis);

                if (zoomingResetEventArgs.Cancel)
                    return;

                axis.ZoomPosition = 0d;
                axis.ZoomFactor = 1d;
                axis.IsScrolling = false;

                RaiseZoomChangedEvent(axis.ZoomFactor, axis.ZoomPosition, axis);
            }
        }

        #endregion

        #region Internal Methods
        
        internal void DisposeZoomEventArguments()
        {
            if (zoomChangedEventArgs != null)
            {
                zoomChangedEventArgs.Axis = null;
                zoomChangedEventArgs = null;
            }

            if (zoomChangingEventArgs != null)
            {
                zoomChangingEventArgs.Axis = null;
                zoomChangingEventArgs = null;
            }
    
            if (panChangedEventArgs != null)
            {
                panChangedEventArgs.Axis = null;
                panChangedEventArgs = null;
            }

            if (panChangingEventArgs != null)
            {
                panChangingEventArgs.Axis = null;
                panChangingEventArgs = null;
            }
        }

        /// <summary>
        /// Zooming applied in the chartaxis based start and end datetime range.
        /// </summary>
        /// <param name="chartAxis">ChartAxisBase2D instance</param>
        /// <param name="start">Start DateTime Range</param>
        /// <param name="end">End DateTime Range</param>
        internal void ZoomByRange(ChartAxisBase2D chartAxis, DateTime start, DateTime end)
        {
            this.ZoomByRange(chartAxis, start.ToOADate(), end.ToOADate());
        }

        /// <summary>
        /// Zooming applied in the chartaxis based start and end range.
        /// </summary>
        /// <param name="chartAxis">ChartAxisBase2D instance</param>
        /// <param name="start">Start Range</param>
        /// <param name="end">End Range</param>
        internal void ZoomByRange(ChartAxisBase2D chartAxis, double start, double end)
        {
            if (CanZoom(chartAxis))
            {
                if (start > end)
                {
                    double temp = start;
                    start = end;
                    end = temp;
                }

                chartAxis.ZoomPosition = (start - chartAxis.ActualRange.Start) / chartAxis.ActualRange.Delta;
                chartAxis.ZoomFactor = (end - start) / chartAxis.ActualRange.Delta;
            }
        }

        /// <summary>
        /// Zooming applied in the chartaxis upto zoom factor in the zoom position.
        /// </summary>
        /// <param name="chartAxis">ChartAxisBase2D instance</param>
        /// <param name="zoomPosition">Zoom Position</param>
        /// <param name="zoomFactor">Zoom Factor</param>
        internal void ZoomToFactor(ChartAxisBase2D chartAxis, double zoomPosition, double zoomFactor)
        {
            if (CanZoom(chartAxis))
            {
                chartAxis.ZoomFactor = zoomFactor;
                chartAxis.ZoomPosition = zoomPosition;
            }
        }

        /// <summary>
        /// Used to zoom in chart
        /// </summary>
        internal void ZoomIn()
        {
            foreach (ChartAxisBase2D chartAxis in ChartArea.Axes)
            {
                if (chartAxis != null && CanZoom(chartAxis))
                {
                    double currentScale = Math.Max(1 / ChartMath.MinMax(chartAxis.ZoomFactor, 0, 1), 1);
                    double cumulativeScale = 0;
                    double factor = 0.25;
                    cumulativeScale = ValMaxScaleLevel(Math.Max(currentScale + factor, 1));
                    Zoom(cumulativeScale, 0.5f, chartAxis);
                }
            }
        }
	
	    /// <summary>
        /// Used to zoom out chart
        /// </summary>
        internal void ZoomOut()
        {
            foreach (ChartAxisBase2D chartAxis in ChartArea.Axes)
            {
                if (chartAxis != null && CanZoom(chartAxis))
                {
                    double currentScale = Math.Max(1 / ChartMath.MinMax(chartAxis.ZoomFactor, 0, 1), 1);
                    double cumulativeScale = 0;
                    double factor = -0.25;
                    cumulativeScale = ValMaxScaleLevel(Math.Max(currentScale + factor, 1));
                    Zoom(cumulativeScale, 0.5f, chartAxis);
                }
            }
        }

	    /// <summary>
        /// Used to zoom in chart for an given ZoomFactor
        /// </summary>
        /// <param name="zoomFactor">ZoomFactor</param>
        internal void Zoom(double zoomFactor)
        {
            foreach (ChartAxisBase2D chartAxis in ChartArea.Axes)
            {
                if (CanZoom(chartAxis))
                {

                    if (chartAxis.ZoomFactor <= 1 && chartAxis.ZoomFactor >= 0.1)
                    {
                        chartAxis.ZoomFactor = zoomFactor;
                        chartAxis.ZoomPosition = 0.5f;
                    }
                }
            }
        }

        /// <summary>
        /// Used to zoom in chart for an given rectangle.
        /// </summary>
        /// <param name="zoomRect"></param>
        internal void Zoom(Rect zoomRect)
        {
            foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
            {
                Zoom(zoomRect, axis);
            }
        }
        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method implementation for detatch elements
        /// </summary>
        protected internal override void DetachElements()
        {
            if (this.ChartZoomingToolBar != null && this.ChartArea != null && this.ChartArea.ToolkitCanvas != null
                && this.ChartArea.AreaType == ChartAreaType.CartesianAxes &&
                this.ChartArea.ToolkitCanvas.Children.Contains(ChartZoomingToolBar))
            {
                this.ChartArea.ToolkitCanvas.Children.Remove(ChartZoomingToolBar);
            }
        }
       
        protected internal override void OnLayoutUpdated()
        {
            if (this.ChartZoomingToolBar != null && this.ChartArea != null && this.ChartZoomingToolBar.Visibility != Visibility.Collapsed)
            {
                if (this.HorizontalPosition == HorizontalAlignment.Left)
                    Canvas.SetLeft(ChartZoomingToolBar, this.ChartArea.SeriesClipRect.Left + 5);
                else if (this.HorizontalPosition == HorizontalAlignment.Center)
                    Canvas.SetLeft(ChartZoomingToolBar, ((this.ChartArea.SeriesClipRect.Width / 2) - (ChartZoomingToolBar.DesiredSize.Width / 2)));
                else
                    Canvas.SetLeft(ChartZoomingToolBar, (this.ChartArea.SeriesClipRect.Right - ChartZoomingToolBar.DesiredSize.Width) - 5);

                if (this.VerticalPosition == VerticalAlignment.Bottom)
                    Canvas.SetTop(ChartZoomingToolBar, (this.ChartArea.SeriesClipRect.Bottom - ChartZoomingToolBar.DesiredSize.Height) - 5);
                else if (this.VerticalPosition == VerticalAlignment.Center)
                    Canvas.SetTop(ChartZoomingToolBar, ((this.ChartArea.SeriesClipRect.Height / 2) - (ChartZoomingToolBar.DesiredSize.Height / 2)));
                else
                    Canvas.SetTop(ChartZoomingToolBar, (this.ChartArea.SeriesClipRect.Top + 5));
            }
        }

        /// <summary>
        /// Method implementation for OnDoubleTapped
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            if (ChartArea != null && ChartArea.SeriesClipRect.Contains(e.GetPosition(AdorningCanvas)))
            {
                isReset = true;
                isZooming = false;
            }
        }

        /// <summary>
        /// Occurs PointerWheel changed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            if (ChartArea == null || ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;

            if (EnableMouseWheelZooming
                && e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && this.KeyModifiers == e.KeyModifiers)
            {
                var point = e.GetCurrentPoint(AdorningCanvas);
                double direction = point.Properties.MouseWheelDelta > 0 ? 1 : -1;
                MouseWheelZoom(point.Position, direction);
            }
        }

        /// <summary>
        /// Called when PointerPressed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (ChartArea != null)
            {

                isPanningChanged = false;
                if (ChartArea.AreaType != ChartAreaType.CartesianAxes)
                    return;

                if (EnableDirectionalZooming && EnablePinchZooming)
                {
                    pointers.Add(e.GetCurrentPoint(AdorningCanvas));
                    if (pointers.Count == 2)
                    {
                        angle = ChartMath.GetAngle(pointers[0].Position, pointers[1].Position);
                    }
                }

                foreach (var axis in ChartArea.Axes)
                {
                    axis.IsScrolling = true;
                }

                if (InternalEnableSelectionZooming)
                {
                    PointerPoint point = e.GetCurrentPoint(this.AdorningCanvas);

                    if (ChartArea.SeriesClipRect.Contains(point.Position))
                    {
                        zoomRect = Rect.Empty;
                        Canvas.SetLeft(selectionRectangle, point.Position.X);
                        Canvas.SetTop(selectionRectangle, point.Position.Y);

                        sel_ZoomingStartEventArgs.ZoomRect = zoomRect;

                        this.ChartArea.OnSelectionZoomingStart(sel_ZoomingStartEventArgs);

                        startPoint = point.Position;
                        AdorningCanvas.CapturePointer(e.Pointer);
                        isZooming = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called when PointerMoved in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (ChartArea != null)
            {
                if (pointers.Count == 2)
                {
                    PointerPoint point = e.GetCurrentPoint(this.AdorningCanvas);

                    for (int i = 0; i < pointers.Count; i++)
                    {
                        if (pointers[i].PointerId == e.Pointer.PointerId)
                        {
                            pointers[i] = point;
                        }
                    }
                    
                    angle = ChartMath.GetAngle(pointers[0].Position, pointers[1].Position);
                }

                isTransposed = (from series in ChartArea.Series where series.IsActualTransposed select series).Any();

                if (isZooming)
                {                 
                    PointerPoint point = e.GetCurrentPoint(this.AdorningCanvas);
                    Point pt = new Point(point.Position.X, point.Position.Y);

                    if (ChartArea.IsMultipleArea)
                    {
                        bool isPointInsideRect = false;
                        foreach (var axis in ChartArea.Axes)
                        {
                            areaRect = ChartExtensionUtils.GetAxisArrangeRect(startPoint, axis, out isPointInsideRect);
                            if(isPointInsideRect)
                            {
                                break;
                            }
                        }

                        if(!isPointInsideRect)
                        {
                            areaRect = ChartArea.SeriesClipRect;
                        }

                        pt = ChartBehavior.ValidatePoint(pt, areaRect);
                        pt.X = ChartMath.MinMax(pt.X, areaRect.X, areaRect.Right);
                        pt.Y = ChartMath.MinMax(pt.Y, areaRect.Y, areaRect.Bottom);
                    }
                    else
                    {
                        areaRect = ChartArea.SeriesClipRect;
                        pt = ChartBehavior.ValidatePoint(pt, areaRect);
                        pt.X = ChartMath.MinMax(pt.X, 0, this.AdorningCanvas.Width);
                        pt.Y = ChartMath.MinMax(pt.Y, 0, this.AdorningCanvas.Height);
                    }

                    if (HorizontalMode)
                    {
                        zoomRect = new Rect(
                            new Point(startPoint.X, ChartArea.SeriesClipRect.Top),
                            new Point(pt.X, ChartArea.SeriesClipRect.Bottom));
                    }
                    else if (VerticalMode)
                    {
                        zoomRect = new Rect(
                            new Point(ChartArea.SeriesClipRect.Left, startPoint.Y),
                            new Point(ChartArea.SeriesClipRect.Right, pt.Y));
                    }
                    else
                    {
                        zoomRect = new Rect(startPoint, pt);
                    }

                    RaiseSelectionZoomDeltaEvent();

                    if (!sel_ZoomingDeltaEventArgs.Cancel)
                    {
                        selectionRectangle.Height = zoomRect.Height;
                        selectionRectangle.Width = zoomRect.Width;

                        Canvas.SetLeft(selectionRectangle, zoomRect.X);
                        Canvas.SetTop(selectionRectangle, zoomRect.Y);
                    }
                }
            }
        }

        /// <summary>
        /// Called when pointer released in chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (ChartArea != null)
            {
                foreach (ChartAxisBase2D axis in ChartArea.Axes)
                {
                    axis.IsScrolling = false;
                    if (isPanningChanged)
                        RaisePanChangedEvent(axis);
                }

                AdorningCanvas.ReleasePointerCapture(e.Pointer);

                if (isZooming)
                {
                    isZooming = false;
                    selectionRectangle.Width = 0;
                    selectionRectangle.Height = 0;

                    if (zoomRect.Width > 0 && zoomRect.Height > 0 && !sel_ZoomingDeltaEventArgs.Cancel)
                    {
                        foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
                        {
                            Zoom(zoomRect, axis);
                        }

                        sel_ZoomingEndEventArgs.ZoomRect = zoomRect;

                        this.ChartArea.OnSelectionZoomingEnd(sel_ZoomingEndEventArgs);
                    }
                }

                if (ResetOnDoubleTap && isReset)
                {
                    Reset();
                    isReset = false;
                }
            }

            foreach (var pointer in pointers)
            {
                if (pointer.PointerId == e.Pointer.PointerId)
                {
                    pointers.Remove(pointer);
                    return;
                }
            }
        }

        private bool CanDirectionalZoom(ChartAxisBase2D axis)
        {            
            if (EnableDirectionalZooming && ZoomMode == ZoomMode.XY)
            {
                bool isXDirection = (angle >= 340 && angle <= 360) || (angle >= 0 && angle <= 20) || (angle >= 160 && angle <= 200);
                bool isYDirection = (angle >= 70 && angle <= 110) || (angle >= 250 && angle <= 290);
                bool isBothDirection = (angle > 20 && angle < 70) || (angle > 110 && angle < 160) || (angle > 200 && angle < 250) || (angle > 290 && angle < 340);

                return (axis.Orientation == Orientation.Horizontal && isXDirection) || (axis.Orientation == Orientation.Vertical && isYDirection) || isBothDirection;
            }

            return ZoomMode == ZoomMode.XY;
        }
        
        /// <summary>
        /// Called when Manipulation Started
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            previousScale = e.Cumulative.Scale;
        }

        /// <summary>
        /// Called when Manipulation delta is changed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {           
            if(ChartArea == null)
            {
                return;
            }

            if (ChartArea.HoldUpdate || ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;
            foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
            {
                ////double currentScale = ChartMath.MinMax(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1, 4);

                double currentScale = Math.Max(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1);

                double cumulativeScale = 0;

                double factor = currentScale * ((e.Cumulative.Scale - previousScale) / previousScale);

                if (EnablePinchZooming && e.Cumulative.Scale != previousScale &&
                    ((axis.Orientation == Orientation.Vertical && VerticalMode) ||
                     (axis.Orientation == Orientation.Horizontal && HorizontalMode) || CanDirectionalZoom(axis)))
                {
                    ////cumulativeScale = ChartMath.MinMax(currentScale + factor, 1, 4);
                    if (!double.IsNaN(factor) && !double.IsInfinity(factor))
                    {
                        cumulativeScale = ValMaxScaleLevel(Math.Max(currentScale + factor, 1));

                        if (cumulativeScale >= 1d)
                        {
                            double origin = 0.5;

                            if (axis.Orientation == Orientation.Horizontal)
                            {
                                origin = e.Position.X / this.ChartArea.ActualWidth;
                            }
                            else
                            {
                                origin = 1 - (e.Position.Y / this.ChartArea.ActualHeight);
                            }

                            Zoom(cumulativeScale, origin, axis);
                        }
                    }
                }
                //The above condition failed and panning is activated at the end of pinch action. Checked e.Cumulative.Scale value to avoid panning at that time.
                else if (InternalEnablePanning && !isZooming && !isReset && e.Cumulative.Scale == 1)
                {
                    if (axis.EnableTouchMode)
                    {
                        if (axis.Area.SeriesClipRect.Contains(e.Position))
                        {
                            Translate(axis, e.Delta.Translation.X, e.Delta.Translation.Y, currentScale);
                            axis.isManipulated = true;
                        }
                    }
                    else
                    {
                        Translate(axis, e.Delta.Translation.X, e.Delta.Translation.Y, currentScale);
                    }
                }
            }

            previousScale = e.Cumulative.Scale;
        }

        protected internal override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            if (ChartArea == null)
            {
                return;
            }

            foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
            {
                if (isPanningChanged)
                    RaisePanChangedEvent(axis);
                axis.isManipulated = false;
            }
        }

        #endregion

        #region Protected Internal Methods

        protected internal void AddZoomingToolBar()
        {
            if (EnableZoomingToolBar && this.ChartZoomingToolBar != null && this.ChartArea != null
                 && this.ChartArea.ToolkitCanvas != null && this.ChartArea.AreaType == ChartAreaType.CartesianAxes &&
                   !this.ChartArea.ToolkitCanvas.Children.Contains(ChartZoomingToolBar))
            {
                this.ChartArea.AddZoomToolBar(ChartZoomingToolBar, this);
            }
        }
        
        protected internal void RemoveZoomingToolBar()
        {
            if (!EnableZoomingToolBar && this.ChartZoomingToolBar != null && this.ChartArea != null
                && this.ChartArea.ToolkitCanvas != null &&
                 this.ChartArea.ToolkitCanvas.Children.Contains(ChartZoomingToolBar))
            {
                this.ChartArea.RemoveZoomToolBar(ChartZoomingToolBar);
            }
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method implementation for AttachElements
        /// </summary>
        protected override void AttachElements()
        {
            SelectionRectangleBinding();
            if (EnableSelectionZooming && this.AdorningCanvas != null &&
                !this.AdorningCanvas.Children.Contains(selectionRectangle))
            {
                this.AdorningCanvas.Children.Add(selectionRectangle);
            }

            AddZoomingToolBar();
        }
        
        protected override DependencyObject CloneBehavior(DependencyObject obj)
        {
            return base.CloneBehavior(new ChartZoomPanBehavior()
            {
                EnableSelectionZooming = this.EnableSelectionZooming,
                EnablePanning = this.EnablePanning,
                EnableZoomingToolBar = this.EnableZoomingToolBar,
                ResetOnDoubleTap = this.ResetOnDoubleTap,
                MaximumZoomLevel = this.MaximumZoomLevel,
                EnableMouseWheelZooming = this.EnableMouseWheelZooming,
                EnablePinchZooming = this.EnablePinchZooming,
                HorizontalPosition = this.HorizontalPosition,
                VerticalPosition = this.VerticalPosition,
                ZoomMode = this.ZoomMode,
                ToolBarOrientation = this.ToolBarOrientation,
                ToolBarBackground = this.ToolBarBackground,
                ToolBarItems = this.ToolBarItems,
                ZoomRelativeToCursor = this.ZoomRelativeToCursor,
            });
        }

        #endregion

        #region Private Static Methods
        
        private static void OnToolBarItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartZoomPanBehavior = d as ChartZoomPanBehavior;
            if (chartZoomPanBehavior != null && chartZoomPanBehavior.ChartZoomingToolBar != null)
            {
                chartZoomPanBehavior.ChartZoomingToolBar.SetItemSource();
            }
        }

        private static void OnToolbarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartZoomPanBehavior behavior = d as ChartZoomPanBehavior;

            if ((bool)e.NewValue)
            {
                if (behavior.ChartZoomingToolBar == null)
                    behavior.ChartZoomingToolBar = new ZoomingToolBar() { ZoomBehavior = behavior };

                behavior.AddZoomingToolBar();
            }
            else
                behavior.RemoveZoomingToolBar();
        }

        private static void OnAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartZoomPanBehavior).OnLayoutUpdated();
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartZoomPanBehavior behavior = d as ChartZoomPanBehavior;
            if (behavior.ChartZoomingToolBar != null)
                behavior.ChartZoomingToolBar.ChangedOrientation();
        }

        private static void OnToolBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartZoomPanBehavior behavior = d as ChartZoomPanBehavior;
            if (behavior.ChartZoomingToolBar != null)
                behavior.ChartZoomingToolBar.ChangeBackground();
        }

        private static void OnZoomSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartZoomPanBehavior behavior = d as ChartZoomPanBehavior;

            if (behavior.ChartZoomingToolBar != null)
            {
                behavior.ChartZoomingToolBar.SetItemSource();
            }

            if ((bool)e.NewValue)
            {
                if (behavior.AdorningCanvas != null && !behavior.AdorningCanvas.Children.Contains(behavior.selectionRectangle))
                {
                    behavior.AdorningCanvas.Children.Add(behavior.selectionRectangle);
                }
            }
            else
            {
                behavior.DetachElement(behavior.selectionRectangle);
            }
        }

        /// <summary>
        /// Calculates ZoomFactor and ZoomPosition using the cumulative scale..
        /// </summary>
        /// <param name="cumulativeScale">Cumulative scale since the starting of the manipulation</param>
        /// <param name="origin">center of manipulation</param>
        /// <param name="currentZoomFactor">Current axis's ZoomFactor</param>
        /// <param name="currentZoomPos">Current axis's ZoomPosition</param>
        /// <param name="calcZoomPos">Calculated ZoomPosition</param>
        /// <param name="calcZoomFactor">Calculated ZoomFactor</param>
        private static void CalZoomFactors(
            double cumulativeScale,
            double origin,
            double currentZoomFactor,
            double currentZoomPos,
            ref double calcZoomPos,
            ref double calcZoomFactor)
        {
            if (cumulativeScale == 1)
            {
                calcZoomFactor = 1;
                calcZoomPos = 0;
            }
            else
            {
                calcZoomFactor = ChartMath.MinMax(1 / cumulativeScale, 0, 1);
                calcZoomPos = currentZoomPos + ((currentZoomFactor - calcZoomFactor) * origin);
            }
        }

        #endregion

        #region Private Methods

        private void SelectionRectangleBinding()
        {
            if (selectionRectangle != null)
            {
                Binding binding = new Binding();
                binding.Path = new PropertyPath("StrokeThickness");
                binding.Source = this;
                selectionRectangle.SetBinding(Rectangle.StrokeThicknessProperty, binding);

                binding = new Binding();
                binding.Path = new PropertyPath("Fill");
                binding.Source = this;
                selectionRectangle.SetBinding(Rectangle.FillProperty, binding);

                binding = new Binding();
                binding.Path = new PropertyPath("Stroke");
                binding.Source = this;
                selectionRectangle.SetBinding(Rectangle.StrokeProperty, binding);
            }
        }


        private void MouseWheelZoom(Point mousePoint, double direction)
        {
            if (ChartArea != null)
            {
                double origin;
                bool canUpdate = false;
                var seriesClipRect = ChartArea.SeriesClipRect;
                mousePoint = new Point(mousePoint.X - seriesClipRect.Left, mousePoint.Y - seriesClipRect.Top);

                bool canZoom = false;
                foreach (ChartAxisBase2D axis in this.ChartArea.Axes)
                {
                    if (axis.RegisteredSeries.Count > 0 && (axis.RegisteredSeries[0] as CartesianSeries) != null &&
                        (axis.RegisteredSeries[0] as CartesianSeries).IsActualTransposed)
                    {
                        if ((axis.Orientation == Orientation.Horizontal &&
                             (ZoomMode == ZoomMode.Y || ZoomMode == ZoomMode.XY)) ||
                            (axis.Orientation == Orientation.Vertical &&
                             (ZoomMode == ZoomMode.X || ZoomMode == ZoomMode.XY)))
                        {
                            canZoom = true;
                        }
                    }
                    else
                    {
                        if ((axis.Orientation == Orientation.Vertical &&
                             (ZoomMode == ZoomMode.Y || ZoomMode == ZoomMode.XY)) ||
                            (axis.Orientation == Orientation.Horizontal &&
                             (ZoomMode == ZoomMode.X || ZoomMode == ZoomMode.XY)))
                        {
                            canZoom = true;
                        }
                    }

                    if (canZoom)
                    {
                        origin = 0.5;
                        ////double currentScale = ChartMath.MinMax(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1, 4);

                        double currentScale = Math.Max(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1);

                        ////double cumulativeScale = ChartMath.MinMax(currentScale + (0.25 * direction), 1, 4);

                        double cumulativeScale = ValMaxScaleLevel(Math.Max(currentScale + (0.25 * direction), 1));

                        if (ZoomRelativeToCursor)
                        {
                            if (axis.Orientation == Orientation.Horizontal)
                                origin = mousePoint.X / seriesClipRect.Width;
                            else
                                origin = 1d - (mousePoint.Y / seriesClipRect.Height);
                        }

                        var value = axis.IsInversed ? 1d - origin : origin;
                        canUpdate = canUpdate | Zoom(cumulativeScale, value > 1d ? 1d : value < 0d ? 0d : value, axis);
                    }

                    canZoom = false;
                }

                if (canUpdate)
                    UpdateArea();
            }
        }

        /// <summary>
        /// Called when selection zooming occurs.
        /// </summary>
        private void RaiseSelectionZoomDeltaEvent()
        {
            sel_ZoomingDeltaEventArgs.ZoomRect = zoomRect;
            sel_ZoomingDeltaEventArgs.Cancel = false;

            this.ChartArea.OnSelectionZoomingDelta(sel_ZoomingDeltaEventArgs);
        }

        private void Translate(ChartAxisBase2D axis, double translateX, double translateY, double currentScale)
        {
            double prevZoomPosition = axis.ZoomPosition;

            double offset, newZoomPosition;
            if (axis.Orientation == Orientation.Horizontal)
            {
                offset = translateX / this.AdorningCanvas.ActualWidth / currentScale;
                newZoomPosition = ChartMath.MinMax(
                    axis.IsInversed == true? axis.ZoomPosition + offset : axis.ZoomPosition - offset, 
                    0, 
                    (1 - axis.ZoomFactor));
            }
            else
            {
                offset = translateY / this.AdorningCanvas.ActualHeight / currentScale;
                newZoomPosition = ChartMath.MinMax(
                    axis.IsInversed == true ? axis.ZoomPosition - offset: axis.ZoomPosition + offset, 
                    0, 
                    (1 - axis.ZoomFactor));
            }

            if (prevZoomPosition == newZoomPosition) return;

            RaisePanChangingEvent(axis, prevZoomPosition, newZoomPosition);

            if (!panChangingEventArgs.Cancel)
            {
                axis.ZoomPosition = newZoomPosition;
                isPanningChanged = true;
            }
        }

        /// <summary>
        /// Called when panning takes place.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="preZoomPosition"></param>
        /// <param name="newZoomPosition"></param>
        private void RaisePanChangingEvent(ChartAxisBase2D axis, double preZoomPosition, double newZoomPosition)
        {
            panChangingEventArgs.Axis = axis;
            panChangingEventArgs.NewZoomPosition = newZoomPosition;
            panChangingEventArgs.OldZoomPosition = preZoomPosition;
            panChangingEventArgs.Cancel = false;

            this.ChartArea.OnPanChanging(panChangingEventArgs);
        }

        /// <summary>
        /// Called when panning is completed.
        /// </summary>
        /// <param name="axis"></param>
        private void RaisePanChangedEvent(ChartAxisBase2D axis)
        {
            panChangedEventArgs.Axis = axis;
            panChangedEventArgs.NewZoomPosition = axis.ZoomPosition;

            this.ChartArea.OnPanChanged(panChangedEventArgs);
        }

        /// <summary>
        /// Event is raised when zooming is completed.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="zoomPosition"></param>
        /// <param name="axis"></param>
        private void RaiseZoomChangedEvent(double zoomFactor, double zoomPosition, ChartAxisBase2D axis)
        {
            var newRange = axis.CalculateRange(axis.ActualRange, axis.ZoomPosition, axis.ZoomFactor);
            zoomChangedEventArgs.Axis = axis;
            zoomChangedEventArgs.CurrentFactor = axis.ZoomFactor;
            zoomChangedEventArgs.CurrentPosition = axis.ZoomPosition;
            if (axis is DateTimeAxis || axis is DateTimeCategoryAxis)
            {
                zoomChangedEventArgs.OldRange = new DateTimeRange(
                    axis.VisibleRange.Start.FromOADate(),
                    axis.VisibleRange.End.FromOADate());
                zoomChangedEventArgs.NewRange = new DateTimeRange(newRange.Start.FromOADate(), newRange.End.FromOADate());
            }
            else
            {
                zoomChangedEventArgs.OldRange = new DoubleRange(axis.VisibleRange.Start, axis.VisibleRange.End);
                zoomChangedEventArgs.NewRange = newRange;
            }

            zoomChangedEventArgs.PreviousFactor = zoomFactor;
            zoomChangedEventArgs.PreviousPosition = zoomPosition;

            this.ChartArea.OnZoomChanged(zoomChangedEventArgs);
        }

        /// <summary>
        /// Event is raised when zooming takes place. 
        /// </summary>
        /// <param name="zoomPosition"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="axis"></param>
        private void RaiseZoomChangingEvent(double zoomPosition, double zoomFactor, ChartAxisBase2D axis)
        {
            zoomChangingEventArgs.Axis = axis;
            zoomChangingEventArgs.CurrentFactor = zoomFactor;
            zoomChangingEventArgs.PreviousFactor = axis.ZoomFactor;
            zoomChangingEventArgs.PreviousPosition = axis.ZoomPosition;
            zoomChangingEventArgs.CurrentPosition = zoomPosition;
            if (axis is DateTimeAxis || axis is DateTimeCategoryAxis)
            {
                zoomChangingEventArgs.OldRange = new DateTimeRange(axis.VisibleRange.Start.FromOADate(), axis.VisibleRange.End.FromOADate());
            }
            else
                zoomChangingEventArgs.OldRange = new DoubleRange(axis.VisibleRange.Start, axis.VisibleRange.End);
            zoomChangingEventArgs.Cancel = false;
            this.ChartArea.OnZoomChanging(zoomChangingEventArgs);
        }

        void Zoom(Rect zoomRect, ChartAxisBase2D axis)
        {
            double previousZoomFactor = axis.ZoomFactor;
            double previousZoomPosition = axis.ZoomPosition;
            double currentZoomFactor = 0d;
            var clipRect = new Rect();
            bool isPointInsideRect = false;
            double clipWidth = 0d;
            double clipHeight = 0d;

            if (axis.Orientation == Orientation.Horizontal)
            {
                if (ChartArea.IsMultipleArea)
                {
                    ChartExtensionUtils.GetAxisArrangeRect(startPoint, axis, out isPointInsideRect);
                    if (!isPointInsideRect)
                    {
                        return;
                    }

                    clipWidth = areaRect.Width - 2 * axis.ActualPlotOffset;
                    clipWidth = clipWidth > 0 ? clipWidth : 0;

                    clipRect = new Rect(
                        new Point(areaRect.X + axis.ActualPlotOffset, areaRect.Y),
                        new Size(clipWidth, areaRect.Height));
                }
                else
                {
                    clipWidth = ChartArea.SeriesClipRect.Width - (2 * axis.ActualPlotOffset);
                    clipWidth = clipWidth > 0 ? clipWidth : 0;

                    clipRect = new Rect(
                        new Point(ChartArea.SeriesClipRect.X + (axis.ActualPlotOffset), ChartArea.SeriesClipRect.Y),
                        new Size(clipWidth, ChartArea.SeriesClipRect.Height));
                }

                if (zoomRect.X < clipRect.X)
                {
                    var zoomWidth = zoomRect.Width - (clipRect.X - zoomRect.X);
                    zoomWidth = zoomWidth > 0 ? zoomWidth : ((StrokeThickness) > 0) ? StrokeThickness : 1;
                    zoomRect = new Rect(new Point(clipRect.X, zoomRect.Y), new Size(zoomWidth, zoomRect.Height));
                }

                if (zoomRect.Right > clipRect.Right)
                {
                    var zoomWidth = zoomRect.Width - (zoomRect.Right - clipRect.Right);
                    zoomWidth = zoomWidth > 0 ? zoomWidth : ((StrokeThickness) > 0) ? StrokeThickness : 1;
                    zoomRect = new Rect(new Point(zoomRect.X, zoomRect.Y), new Size(zoomWidth, zoomRect.Height));
                }

                currentZoomFactor = previousZoomFactor * (zoomRect.Width / clipRect.Width);

                if (currentZoomFactor != previousZoomFactor && ValMaxZoomLevel(currentZoomFactor))
                {
                    axis.ZoomFactor = !VerticalMode
                    ? currentZoomFactor
                    : 1;
                    axis.ZoomPosition = !VerticalMode
                        ? (previousZoomPosition +

                       Math.Abs((axis.IsInversed ? clipRect.Right - zoomRect.Right
                                                  : zoomRect.X - clipRect.Left)
                                                  / clipRect.Width) * previousZoomFactor) : 0;
                }
            }
            else
            {
                if (ChartArea.IsMultipleArea)
                {
                    ChartExtensionUtils.GetAxisArrangeRect(startPoint, axis, out isPointInsideRect);

                    if (!isPointInsideRect)
                    {
                        return;
                    }

                    clipHeight = areaRect.Height - 2 * axis.ActualPlotOffset;
                    clipHeight = clipHeight > 0 ? clipHeight : 0;

                    clipRect = new Rect(
                               new Point(areaRect.X, areaRect.Y + axis.ActualPlotOffset),
                               new Size(areaRect.Width, clipHeight));
                }
                else
                {
                    clipHeight = ChartArea.SeriesClipRect.Height - (2 * axis.ActualPlotOffset);
                    clipHeight = clipHeight > 0 ? clipHeight : 0;

                    clipRect = new Rect(
                               new Point(ChartArea.SeriesClipRect.X, ChartArea.SeriesClipRect.Y + (axis.ActualPlotOffset)),
                               new Size(ChartArea.SeriesClipRect.Width, clipHeight));
                }

                if (zoomRect.Y < clipRect.Y)
                {
                    var zoomHeight = zoomRect.Height - (clipRect.Y - zoomRect.Y);
                    zoomHeight = zoomHeight > 0 ? zoomHeight : StrokeThickness > 0 ? StrokeThickness : 1;
                    zoomRect = new Rect(new Point(zoomRect.X, clipRect.Y), new Size(zoomRect.Width, zoomHeight));
                }

                if (clipRect.Bottom < zoomRect.Bottom)
                {
                    var zoomHeight = zoomRect.Height - (zoomRect.Bottom - clipRect.Bottom);
                    zoomHeight = zoomHeight > 0 ? zoomHeight : StrokeThickness > 0 ? StrokeThickness : 1;
                    zoomRect = new Rect(new Point(zoomRect.X, zoomRect.Y), new Size(zoomRect.Width, zoomHeight));
                }

                currentZoomFactor = previousZoomFactor * zoomRect.Height / clipRect.Height;

                if (currentZoomFactor != previousZoomFactor && ValMaxZoomLevel(currentZoomFactor))
                {
                    axis.ZoomFactor = !HorizontalMode
                   ? currentZoomFactor
                   : 1;
                    axis.ZoomPosition = !HorizontalMode
                        ? previousZoomPosition +
                       (1 - Math.Abs(((axis.IsInversed ? zoomRect.Top - clipRect.Bottom
                                                         : zoomRect.Bottom - clipRect.Top))
                                                         / clipRect.Height)) * previousZoomFactor : 0;
                }
            }
        }

        bool CanZoom(ChartAxisBase2D axis)
        {
            bool canZoom = false;

            if (axis.RegisteredSeries.Count > 0 && (axis.RegisteredSeries[0] as CartesianSeries) != null && (axis.RegisteredSeries[0] as CartesianSeries).IsActualTransposed)
            {
                if ((axis.Orientation == Orientation.Horizontal && (ZoomMode == ZoomMode.Y || ZoomMode == ZoomMode.XY)) ||
                    (axis.Orientation == Orientation.Vertical && (ZoomMode == ZoomMode.X || ZoomMode == ZoomMode.XY)))
                    canZoom = true;
            }
            else
            {
                if ((axis.Orientation == Orientation.Vertical && (ZoomMode == ZoomMode.Y || ZoomMode == ZoomMode.XY)) ||
                    (axis.Orientation == Orientation.Horizontal && (ZoomMode == ZoomMode.X || ZoomMode == ZoomMode.XY)))
                    canZoom = true;
            }

            return canZoom;
        }

        /// <summary>
        /// Event is raised when zoom is reset.
        /// </summary>
        /// <param name="axis"></param>
        private void RaiseResetZoomingEvent(ChartAxisBase2D axis)
        {
            zoomingResetEventArgs.Axis = axis;
            if (axis is DateTimeAxis || axis is DateTimeCategoryAxis)
                zoomingResetEventArgs.PreviousZoomRange = new DateTimeRange(
                    axis.VisibleRange.Start.FromOADate(),
                    axis.VisibleRange.End.FromOADate());
            else
                zoomingResetEventArgs.PreviousZoomRange = new DoubleRange(axis.VisibleRange.Start, axis.VisibleRange.End);

            this.zoomingResetEventArgs.Cancel = false;
            this.ChartArea.OnResetZoom(zoomingResetEventArgs);
        }

        bool ValMaxZoomLevel(double currentZoomFactor)
        {
            if (IsMaxZoomLevel) // Selection Zooming not select the correct region -WPF-18131
                return ((1 / currentZoomFactor) <= MaximumZoomLevel ? true : false);
            return true;
        }

        double ValMaxScaleLevel(double cumulativeScale)
        {
            if (IsMaxZoomLevel)
                return ((cumulativeScale <= MaximumZoomLevel) ? cumulativeScale : MaximumZoomLevel);
            return cumulativeScale;
        }
 
        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for zooming event arguments.
    /// </summary>
    public partial class ZoomEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the previous zoomposition.
        /// </summary>
        public double PreviousPosition { get; set; }

        /// <summary>
        /// Gets or sets the current zoom position.
        /// </summary>
        public double CurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the old visible range.
        /// </summary>
        public object OldRange { get; set; }

        /// <summary>
        /// Gets or sets the previous zoomfactor.
        /// </summary>
        public double PreviousFactor { get; internal set; }

        /// <summary>
        /// Gets or sets the current zoomfactor.
        /// </summary>
        public double CurrentFactor { get; internal set; }

        /// <summary>
        /// Gets or sets the ChartAxis.
        /// </summary>
        public ChartAxisBase2D Axis { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for ZoomChanging event arguments.
    /// </summary>
    public partial class ZoomChangingEventArgs : ZoomEventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the event.
        /// </summary>
        public bool Cancel { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for ZoomChanged event arguments.
    /// </summary>
    public partial class ZoomChangedEventArgs : ZoomEventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the new visible range .
        /// </summary>
        public object NewRange { get; internal set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for selection zooming event arguments.
    /// </summary>
    public partial class SelectionZoomingEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the bounds of the rectangle.
        /// </summary>
        public Rect ZoomRect { get; set; }
        
        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for SelectionZoomingStart event arguments.
    /// </summary>
    public partial class SelectionZoomingStartEventArgs : SelectionZoomingEventArgs
    {
    }

    /// <summary>
    /// Class implementation for SelectionZoomingDelta event arguments.
    /// </summary>
    public partial class SelectionZoomingDeltaEventArgs : SelectionZoomingEventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the event.
        /// </summary>
        public bool Cancel { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for SelectionZoomingEnd event arguments.
    /// </summary>
    public partial class SelectionZoomingEndEventArgs : SelectionZoomingEventArgs
    {
    }

    /// <summary>
    /// Class implementation for panning event arguments.
    /// </summary>
    public partial class PanningEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the new zoom position.
        /// </summary>
        public double NewZoomPosition { get; internal set; }

        /// <summary>
        /// Gets or sets the ChartAxis.
        /// </summary>
        public ChartAxisBase2D Axis { get; internal set; }
        
        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for PanChanging event arguments.
    /// </summary>
    public partial class PanChangingEventArgs : PanningEventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the previous zoom position.
        /// </summary>
        public double OldZoomPosition { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the event.
        /// </summary>
        public bool Cancel { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for PanChanged event arguments.
    /// </summary>
    public partial class PanChangedEventArgs : PanningEventArgs
    {
    }

    /// <summary>
    /// Class implementation for ResetZooming event arguments.
    /// </summary>
    public partial class ResetZoomEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the previous visible range.
        /// </summary>
        public object PreviousZoomRange { get; internal set; }

        /// <summary>
        /// Gets or sets the ChartAxis.
        /// </summary>
        public ChartAxisBase2D Axis { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the event.
        /// </summary>
        public bool Cancel { get; set; }

        #endregion

        #endregion
    }
}
