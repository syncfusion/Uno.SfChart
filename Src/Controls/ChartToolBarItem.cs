// <copyright file="ChartToolBarItem.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Devices.Input;
    using Windows.Foundation;
    using Windows.System;
    using Windows.UI;
    using Windows.UI.Core;
    using Windows.UI.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    #region ToolBarItems    

    /// <summary>
    /// ChartToolBarItem class implementation. 
    /// </summary>
    public abstract partial class ChartToolBarItem
    {
    }

    /// <summary>
    /// <see cref="ZoomingToolBarItem"/> class implementation.
    /// </summary>
    public partial class ZoomingToolBarItem : ContentControl, INotifyPropertyChanged
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="IconBackground"/> property.
        /// </summary>
        public static readonly DependencyProperty IconBackgroundProperty =
            DependencyProperty.Register(
                "IconBackground",
                typeof(Color),
                typeof(ZoomingToolBarItem),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableColor"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableColorProperty =
            DependencyProperty.Register(
                "EnableColor",
                typeof(Color),
                typeof(ZoomingToolBarItem),
                new PropertyMetadata(Color.FromArgb(250, 0x0c, 0x9e, 0xef)));

        /// <summary>
        /// The DependencyProperty for <see cref="DisableColor"/> property.
        /// </summary>
        public static readonly DependencyProperty DisableColorProperty =
            DependencyProperty.Register(
                "DisableColor",
                typeof(Color),
                typeof(ZoomingToolBarItem),
                new PropertyMetadata(Color.FromArgb(250, 0xb3, 0xb3, 0xb3)));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarIconHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarIconHeightProperty =
            DependencyProperty.Register(
                "ToolBarIconHeight",
                typeof(double),
                typeof(ZoomingToolBarItem),
                new PropertyMetadata(0d, OnSizeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarIconWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarIconWidthProperty =
            DependencyProperty.Register(
                "ToolBarIconWidth",
                typeof(double),
                typeof(ZoomingToolBarItem),
                new PropertyMetadata(0d, OnSizeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolBarIconMargin"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolBarIconMarginProperty =
            DependencyProperty.Register(
                "ToolBarIconMargin",
                typeof(Thickness),
                typeof(ZoomingToolBarItem),
                new PropertyMetadata(new Thickness(0), OnSizeChanged));

        #endregion

        #region Fields

        private ChartZoomPanBehavior source;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the icon background 
        /// </summary>
        public Color IconBackground
        {
            get { return (Color)GetValue(IconBackgroundProperty); }
            set { SetValue(IconBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the toolbar color.
        /// </summary>
        public Color EnableColor
        {
            get { return (Color)GetValue(EnableColorProperty); }
            set { SetValue(EnableColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the toolbar disable color.
        /// </summary>
        public Color DisableColor
        {
            get { return (Color)GetValue(DisableColorProperty); }
            set { SetValue(DisableColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Height for the ZoomingToolbar items.
        /// </summary>
        public double ToolBarIconHeight
        {
            get { return (double)GetValue(ToolBarIconHeightProperty); }
            set { SetValue(ToolBarIconHeightProperty, value); }
        }

        /// <summary>
        ///  Gets or sets the Width for the ZoomingToolbar items.
        /// </summary>
        public double ToolBarIconWidth
        {
            get { return (double)GetValue(ToolBarIconWidthProperty); }
            set { SetValue(ToolBarIconWidthProperty, value); }
        }

        /// <summary>
        ///  Gets or sets the Margin for the ZoomingToolbar items.
        /// </summary>
        public Thickness ToolBarIconMargin
        {
            get { return (Thickness)GetValue(ToolBarIconMarginProperty); }
            set { SetValue(ToolBarIconMarginProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the <see cref="ChartZoomPanBehavior"/>.
        /// </summary>
        internal ChartZoomPanBehavior Source
        {
            get
            {
                return source;
            }

            set
            {
                source = value;
                if (source != null)
                {
                    BindingToolbarItems(source);
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods
        
        internal void Dispose()
        {
            source = null;

            if (PropertyChanged != null)
            {
                foreach (var handler in PropertyChanged.GetInvocationList())
                {
                    PropertyChanged -= handler as PropertyChangedEventHandler;
                }

                PropertyChanged = null;
            }

            this.Content = null;
            this.DataContext = null;
        }

        /// <summary>
        /// Updates the <see cref="ZoomingToolBarItem"/> when it's property changes.
        /// </summary>
        /// <param name="propertyName">The Property Name</param>
        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

       protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
       {
           e.Handled = true;
       }
        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the <see cref="ZoomingToolBarItem"/> when the size changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomingToolBarItem item = d as ZoomingToolBarItem;
            item.ScheduleUpdate();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Binds the tool bar item.
        /// </summary>
        /// <param name="dataSource">The Required Source.</param>
        private void BindingToolbarItems(ChartZoomPanBehavior dataSource)
        {
            Binding binding = new Binding();
            binding.Source = dataSource;
            binding.Path = new PropertyPath("ToolBarItemHeight");
            BindingOperations.SetBinding(this, ZoomingToolBarItem.ToolBarIconHeightProperty, binding);

            binding = new Binding();
            binding.Source = dataSource;
            binding.Path = new PropertyPath("ToolBarItemWidth");
            BindingOperations.SetBinding(this, ZoomingToolBarItem.ToolBarIconWidthProperty, binding);

            binding = new Binding();
            binding.Source = dataSource;
            binding.Path = new PropertyPath("ToolBarItemMargin");
            BindingOperations.SetBinding(this, ZoomingToolBarItem.ToolBarIconMarginProperty, binding);
        }

        /// <summary>
        /// Schedule the <see cref="ZoomingToolBarItem"/> update.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "UpdateToolBarAction", Justification = "Reviewed")]
        private void ScheduleUpdate()
        {
            IAsyncAction UpdateToolBarAction = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateToolBarPosition);
        }

        /// <summary>
        /// Updates the toolbar position.
        /// </summary>
        private void UpdateToolBarPosition()
        {
            source.ChartZoomingToolBar.UpdateLayout();
            source.OnLayoutUpdated();
        }

        #endregion

        #endregion     
    }

    /// <summary>
    /// <see cref="ZoomIn"/> class Implementation.
    /// </summary>
    public partial class ZoomIn : ZoomingToolBarItem
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomIn"/> class.
        /// </summary>
        public ZoomIn()
        {
            this.DataContext = this;
            this.DefaultStyleKey = typeof(ZoomIn);
            Tag = "ZoomIn";
            ToolTipService.SetToolTip(this, SfChartResourceWrapper.ZoomIn);
        }

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Updates the <see cref="ZoomIn"/> icon.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (this.Source.ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;
            if (this.Source.ChartArea != null)
            {
                bool canZoom = false;
                foreach (ChartAxisBase2D axis in this.Source.ChartArea.Axes)
                {
                    if (axis.RegisteredSeries.Count > 0 && (axis.RegisteredSeries[0] as CartesianSeries) != null && (axis.RegisteredSeries[0] as CartesianSeries).IsActualTransposed)
                    {
                        if ((axis.Orientation == Orientation.Horizontal && (Source.ZoomMode == ZoomMode.Y || Source.ZoomMode == ZoomMode.XY)) ||
                            (axis.Orientation == Orientation.Vertical && (Source.ZoomMode == ZoomMode.X || Source.ZoomMode == ZoomMode.XY)))
                        {
                            canZoom = true;
                        }
                    }
                    else
                    {
                        if ((axis.Orientation == Orientation.Vertical && (Source.ZoomMode == ZoomMode.Y || Source.ZoomMode == ZoomMode.XY)) ||
                            (axis.Orientation == Orientation.Horizontal && (Source.ZoomMode == ZoomMode.X || Source.ZoomMode == ZoomMode.XY)))
                        {
                            canZoom = true;
                        }
                    }

                    if (canZoom)
                    {
                        var origin = 0.5;
                        double currentScale = Math.Max(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1);
                        double cumulativeScale = Math.Max(currentScale + (0.25 * 1), 1);
                        Source.Zoom(cumulativeScale, origin > 1d ? 1d : origin < 0d ? 0d : origin, axis);
                    }

                    canZoom = false;
                }
            }

            e.Handled = true;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// <see cref="ZoomOut"/> class Implementation.
    /// </summary>
    public partial class ZoomOut : ZoomingToolBarItem
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomOut"/> class.
        /// </summary>
        public ZoomOut()
        {
            this.DataContext = this;
            this.DefaultStyleKey = typeof(ZoomOut);
            Tag = "ZoomOut";
            ToolTipService.SetToolTip(this, SfChartResourceWrapper.ZoomOut);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the <see cref="ZoomOut"/> icon.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (this.Source.ChartArea.AreaType != ChartAreaType.CartesianAxes)
                return;
            bool canZoom = false;
            int updatedAxes = 0;
            foreach (ChartAxisBase2D axis in this.Source.ChartArea.Axes)
            {
                if (axis.RegisteredSeries.Count > 0 && (axis.RegisteredSeries[0] as CartesianSeries) != null && (axis.RegisteredSeries[0] as CartesianSeries).IsActualTransposed)
                {
                    if ((axis.Orientation == Orientation.Horizontal && (Source.ZoomMode == ZoomMode.Y || Source.ZoomMode == ZoomMode.XY)) ||
                        (axis.Orientation == Orientation.Vertical && (Source.ZoomMode == ZoomMode.X || Source.ZoomMode == ZoomMode.XY)))
                    {
                        canZoom = true;
                    }
                }
                else
                {
                    if ((axis.Orientation == Orientation.Vertical && (Source.ZoomMode == ZoomMode.Y || Source.ZoomMode == ZoomMode.XY)) ||
                        (axis.Orientation == Orientation.Horizontal && (Source.ZoomMode == ZoomMode.X || Source.ZoomMode == ZoomMode.XY)))
                    {
                        canZoom = true;
                    }
                }

                if (canZoom)
                {
                    var origin = 0.5;
                    double currentScale = Math.Max(1 / ChartMath.MinMax(axis.ZoomFactor, 0, 1), 1);
                    double cumulativeScale = Math.Max(currentScale + (0.25 * -1), 1);
                    Source.Zoom(cumulativeScale, origin > 1d ? 1d : origin < 0d ? 0d : origin, axis);
                }

                canZoom = false;
                if (axis.ZoomFactor == 1 || axis.ZoomPosition == 0)
                    updatedAxes++;
            }

            e.Handled = true;
        }

        #endregion
    }

    /// <summary>
    /// <see cref="ZoomReset"/> class Implementation.
    /// </summary>
    public partial class ZoomReset : ZoomingToolBarItem
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomReset"/> class.
        /// </summary>
        public ZoomReset()
        {
            this.DataContext = this;
            this.DefaultStyleKey = typeof(ZoomReset);
            Tag = "Reset";
            ToolTipService.SetToolTip(this, SfChartResourceWrapper.Reset);
        }

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Updates the <see cref="ZoomReset"/> icon.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            this.Source.Reset();

            e.Handled = true;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// <see cref="ZoomPan"/> class Implementation.
    /// </summary>
    public partial class ZoomPan : ZoomingToolBarItem
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomPan"/> class.
        /// </summary>
        public ZoomPan()
        {
            this.DataContext = this;
            this.DefaultStyleKey = typeof(ZoomPan);
            Tag = "Panning";
            ToolTipService.SetToolTip(this, SfChartResourceWrapper.Pan);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the <see cref="ZoomPan"/> icon.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (Source.IsActive)
            {
                Source.EnablePanning = false;
                foreach (ZoomingToolBarItem item in Source.ChartZoomingToolBar.ItemsSource as IEnumerable)
                {
                    if (item is ZoomPan)
                    {
                        item.IconBackground = item.DisableColor;
                    }
                    else if (item is SelectionZoom)
                    {
                        item.IconBackground = item.EnableColor;
                        Source.InternalEnableSelectionZooming = true;
                    }
                }

                Source.IsActive = false;
            }
            else
            {
                Source.EnablePanning = true;
                foreach (ZoomingToolBarItem item in Source.ChartZoomingToolBar.ItemsSource as IEnumerable)
                {
                    if (item is ZoomPan)
                    {
                        item.IconBackground = item.EnableColor;
                    }

                    if (item is SelectionZoom)
                    {
                        item.IconBackground = item.DisableColor;
                        Source.InternalEnableSelectionZooming = false;
                    }
                }

                Source.IsActive = true;
            }

            e.Handled = true;
        }

        #endregion
    }

    /// <summary>
    /// <see cref="SelectionZoom"/> class Implementation.
    /// </summary>
    public partial class SelectionZoom : ZoomingToolBarItem
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionZoom"/> class.
        /// </summary>
        public SelectionZoom()
        {
            this.DataContext = this;
            this.DefaultStyleKey = typeof(SelectionZoom);
            Tag = "Selection";
            ToolTipService.SetToolTip(this, SfChartResourceWrapper.BoxSelectionZoom);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the <see cref="SelectionZoom"/> icon.
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (!Source.IsActive)
            {
                Source.InternalEnableSelectionZooming = false;
                foreach (ZoomingToolBarItem item in Source.ChartZoomingToolBar.ItemsSource as IEnumerable)
                {
                    if (item is ZoomPan)
                    {
                        item.IconBackground = item.EnableColor;
                        Source.EnablePanning = true;
                    }

                    if (item is SelectionZoom)
                    {
                        item.IconBackground = item.DisableColor;
                    }
                }

                Source.IsActive = true;
            }
            else
            {
                Source.InternalEnableSelectionZooming = true;
                foreach (ZoomingToolBarItem item in Source.ChartZoomingToolBar.ItemsSource as IEnumerable)
                {
                    if (item is ZoomPan)
                    {
                        item.IconBackground = item.DisableColor;
                        Source.EnablePanning = false;
                    }

                    if (item is SelectionZoom)
                    {
                        item.IconBackground = item.EnableColor;                            
                    }
                }

                Source.IsActive = false;
            }
        }

        #endregion
    }

    #endregion
}
