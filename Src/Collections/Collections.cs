using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// A collection class which holds chart legend
    /// </summary>
    public partial class ChartLegendCollection : ObservableCollection<ChartLegend>
    { 
    
    }
	
	/// <summary>
    /// A collection class which holds ChartAxisScaleBreak
    /// </summary>

    public partial class ChartAxisScaleBreaks : ObservableCollection<ChartAxisScaleBreak>
    {
    }

    /// <summary>
    /// A collection class which holds ChartStripLine
    /// </summary>
   
    public partial class ChartStripLines : ObservableCollection<ChartStripLine>
    {
    }

    /// <summary>
    /// A collection class which holds ChartMultiLevelLabels
    /// </summary>
    public partial class ChartMultiLevelLabels : ObservableCollection<ChartMultiLevelLabel>
    {
       
    }
         
    /// <summary>
    /// A collection class which holds ChartAxis.
    /// </summary>
   
    public partial class ChartAxisCollection : ObservableCollection<ChartAxis>
    {
        /// <summary>
        /// return ChartAxis value from the given string
        /// </summary>
        /// <param name="name"></param>
        public ChartAxis this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                foreach (ChartAxis axis in this)
                {
                    if (axis.Name == name)
                    {
                        return axis;
                    }
                }

                return null;
            }
        }
        protected override void InsertItem(int index, ChartAxis item)
        {
            if (item != null && !Contains(item))
            {
                base.InsertItem(index, item);
            }
            
            if (Contains(item) && item.Area.DependentSeriesAxes != null)
            {
                if (item.Area.DependentSeriesAxes.Contains(item))
                {
                    item.Area.DependentSeriesAxes.Remove(item);
                }
            }
        }

        internal void RemoveItem(ChartAxis axis, bool flag)
        {
            if (flag)
            {
                Remove(axis);
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartTrendLine.
    /// </summary>

    public partial class ChartTrendLineCollection : ObservableCollection<Trendline>
    {
        /// <summary>
        /// Called when instance created for ChartTrendLineCollection
        /// </summary>
        public ChartTrendLineCollection()
        {
            
        }
        /// <summary>
        /// return ChartTrendLine from the given string
        /// </summary>
        /// <param name="name"></param>
        public TrendlineBase this[string name]
        {
            get
            {
                foreach (var trend in this)
                {
                    if (trend.Name == name)
                    {
                        return trend;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartSeries.
    /// </summary>

    public partial class ChartSeriesCollection : ObservableCollection<ChartSeries>
    {
        /// <summary>
        /// return ChartSeries from the given string
        /// </summary>
        /// <param name="name"></param>
        public ChartSeries this[string name]
        {
            get
            {
                foreach (ChartSeries series in this)
                {
                    if (series.Name == name)
                    {
                        return series;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartSeries 2D.
    /// </summary>
   
    public partial class ChartVisibleSeriesCollection : ObservableCollection<ChartSeriesBase>
    {
        /// <summary>
        /// return ChartSeries from the given string
        /// </summary>
        /// <param name="name"></param>
        public ChartSeriesBase this[string name]
        {
            get
            {
                foreach (ChartSeriesBase series in this)
                {
                    if (series.Name == name)
                    {
                        return series;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartSeries 3D.
    /// </summary>
   
    public partial class ChartSeries3DCollection : ObservableCollection<ChartSeries3D>
    {
        /// <summary>
        /// return ChartSeries from the given string
        /// </summary>
        /// <param name="name"></param>
        public ChartSeries3D this[string name]
        {
            get
            {
                foreach (ChartSeries3D series in this)
                {
                    if (series.Name == name)
                    {
                        return series;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartRowDefinitions
    /// </summary>
   
    public partial class ChartRowDefinitions : ObservableCollection<ChartRowDefinition> 
    {
    }

    /// <summary>
    /// A collection class which holds ChartColumnDefinitions
    /// </summary>
   
    public partial class ChartColumnDefinitions : ObservableCollection<ChartColumnDefinition> 
    {
    }

    /// <summary>
    /// A collection class that holds ChartAxisRangeStyle.
    /// </summary>
    public partial class ChartAxisRangeStyleCollection : ObservableCollection<ChartAxisRangeStyle>
    {
    }
}
