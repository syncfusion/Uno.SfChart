// <copyright file="ChartToolBar.cs" company="Syncfusion. Inc">
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
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Devices.Input;
    using Windows.Foundation;
    using Windows.System;
    using Windows.UI;
    using Windows.UI.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    #region Chart TooBar    

    /// <summary>
    /// ChartToolBar class implementation that represents a ItemsControl. 
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ItemsControl" />
    public abstract partial class ChartToolBar : ItemsControl
    {
    }

    /// <summary>
    /// Represents the <see cref="ZoomingToolBar"/> class.
    /// </summary>
    public partial class ZoomingToolBar : ChartToolBar
    {
        #region Fields

        private ChartZoomPanBehavior behavior;

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomingToolBar"/> class.
        /// </summary>
        public ZoomingToolBar()
        {
            this.DefaultStyleKey = typeof(ZoomingToolBar);
            Loaded += ZoomingToolBar_Loaded;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="ChartZoomPanBehavior"/>.
        /// </summary>
        internal ChartZoomPanBehavior ZoomBehavior
        {
            get
            {
                return behavior;
            }

            set
            {
                behavior = value;
            }
        }

        #endregion

        #region Methods

        #region Pubic Methods
        
        /// <summary>
        /// Changes the background color.
        /// </summary>
        public void ChangeBackground()
        {
            if (this.ZoomBehavior.ToolBarBackground != null)
                this.Background = this.ZoomBehavior.ToolBarBackground;
        }

        #endregion

        #region Internal Methods

        internal void Dispose()
        {
            this.behavior = null;

            var toolbarItems = ItemsSource as List<ZoomingToolBarItem>;
            if (toolbarItems != null)
            {
                foreach (var toolbarItem in toolbarItems)
                {
                    toolbarItem.Dispose();
                }

                toolbarItems.Clear();
                ItemsSource = null;
            }
        }

        /// <summary>
        /// Change the ItemsPanel orientation 
        /// </summary>
        internal void ChangedOrientation()
        {
            ItemsPresenter itemsPresenter = ChartLayoutUtils.GetVisualChild<ItemsPresenter>(this);
            if (itemsPresenter != null)
            {
                if (VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
                {
                    StackPanel itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 1) as StackPanel;

                    if (itemsPanel != null)
                    {
                        itemsPanel.Orientation = ZoomBehavior.ToolBarOrientation;
                        this.UpdateLayout();
                        ZoomBehavior.OnLayoutUpdated();
                    }
                }
            }
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Set Items for ToolBar.
        /// </summary>
        protected internal void SetItemSource()
        {
            var values = new string[] { "ZoomIn", "ZoomOut", "Reset", "SelectZooming" };
            var itemSource = new List<ZoomingToolBarItem>();
            var items = from item in ZoomBehavior.ToolBarItems.ToString().Split(',') select item.Trim();
            if (items.Any(item => item.Equals("All")))
                items = values;
            foreach (string item in items)
            {
                switch (item)
                {
                    case "ZoomIn":
                        itemSource.Add(new ZoomIn() { Source = this.ZoomBehavior });
                        break;
                    case "ZoomOut":
                        itemSource.Add(new ZoomOut() { Source = this.ZoomBehavior, IsEnabled = false });
                        break;
                    case "Reset":
                        itemSource.Add(new ZoomReset() { Source = this.ZoomBehavior, IsEnabled = false });
                        break;
                    case "SelectZooming":
                        if (ZoomBehavior.EnableSelectionZooming)
                        {
                            itemSource.Add(new ZoomPan() { Source = this.ZoomBehavior });
                            itemSource.Add(new SelectionZoom() { Source = this.ZoomBehavior });
                        }

                        break;
                }
            }

            this.ItemsSource = itemSource;
            ChangedOrientation();
        }

        #endregion

        #region Protected Override Methods

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the <see cref="ZoomingToolBar"/> when loaded.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments.</param>
        private void ZoomingToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeBackground();
            SetItemSource();
            if (this.ZoomBehavior.ChartArea != null)
            {
                if (this.ZoomBehavior.ChartArea.Axes.Count > 0)
                {
                    foreach (ChartAxisBase2D axis in this.ZoomBehavior.ChartArea.Axes)
                    {
                        if (axis.ZoomFactor < 1 || axis.ZoomPosition > 0)
                            this.ZoomBehavior.ChartArea.ChangeToolBarState();
                    }
                }
            }
        }

        #endregion

        #endregion
    }

    #endregion
}
