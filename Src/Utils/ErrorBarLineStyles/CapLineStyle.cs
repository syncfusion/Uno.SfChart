using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a dependency object that display a cap style in error bar. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.LineStyle" />
    public partial class CapLineStyle : LineStyle
    {

        public CapLineStyle(ChartSeriesBase series)
            : base(series)
        {
        }

        public CapLineStyle()
        {
            
        }
        /// <summary>
        /// The DependencyProperty for <see cref="Visibility"/> property.
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
         DependencyProperty.Register("Visibility", typeof(Visibility), typeof(CapLineStyle), 
         new PropertyMetadata(Visibility.Visible, OnPropertyChange));

        /// <summary>
        /// Gets or sets the visiblity for the line
        /// </summary>
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }
        /// <summary>
        /// The DependencyProperty for <see cref="LineWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(double), typeof(CapLineStyle),
            new PropertyMetadata(10d, OnPropertyChange));

        private static void OnPropertyChange(DependencyObject obj, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var chartSeries = (obj as CapLineStyle).Series;
            if (chartSeries != null) chartSeries.ActualArea.ScheduleUpdate();
        }

        /// <summary>
        /// Gets or sets a width for the line
        /// </summary>
        public double LineWidth
        {
            get { return (double)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }
       
    }
}
