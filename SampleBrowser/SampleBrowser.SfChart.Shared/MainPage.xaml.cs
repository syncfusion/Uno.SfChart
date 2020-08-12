using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleBrowser.SfChart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            navigationView.PaneHeader = new TextBlock()
            {
                Text = "Syncfusion Uno Chart",
                FontSize = 18,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigatePage(sender);
        }

        void NavigatePage(object sender)
        {
            syncLabel.Visibility = Visibility.Collapsed;
            string item = (string)(sender as NavigationViewItem).Tag;
            switch (item)
            {
                case "Column":
                    contentFrame.Navigate(typeof(ColumnChartView));
                    break;
                case "Bar":
                    contentFrame.Navigate(typeof(BarChartView));
                    break;
                case "Line":
                    contentFrame.Navigate(typeof(LineChartView));
                    break;
                case "Area":
                    contentFrame.Navigate(typeof(AreaChartView));
                    break;
                case "Scatter":
                    contentFrame.Navigate(typeof(ScatterChartView));
                    break;
                case "Pie":
                    contentFrame.Navigate(typeof(PieChartView));
                    break;
                case "Doughnut":
                    contentFrame.Navigate(typeof(DoughtnutChartView));
                    break;
                case "Bubble":
                    contentFrame.Navigate(typeof(BubbleChartView));
                    break;
                case "Axis":
                    contentFrame.Navigate(typeof(ChartAxisType));
                    break;
            }
        }


        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigatePage(args.SelectedItem);
        }
    }


}



