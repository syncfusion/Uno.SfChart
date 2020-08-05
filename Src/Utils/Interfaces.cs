using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Inteface implementation for IRangeAxis
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRangeAxis<T> where T : IComparable
    {
        /// <summary>
        /// Gets or sets Minimum property
        /// </summary>
        T Minimum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Maximum property
        /// </summary>
        T Maximum
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface implementation for IRangeAxis
    /// </summary>
    public interface IRangeAxis
    {
        /// <summary>
        /// Gets Range property
        /// </summary>
        DoubleRange Range { get; }
    }

    /// <summary>
    /// Interface implementation for IChartAxis
    /// </summary>
    public interface IChartAxis
    {
        /// <summary>
        /// Gets or sets VisibleLabels property
        /// </summary>
        ChartAxisLabelCollection VisibleLabels
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface implementation for IChartSeries
    /// </summary>
    public interface IChartSeries
    {
        /// <summary>
        /// Gets or sets ItemsSource property
        /// </summary>
        IEnumerable ItemsSource
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface implementation for ISupportAxes
    /// </summary>
    public interface ISupportAxes
    {
        /// <summary>
        /// Gets XRange property
        /// </summary>
        DoubleRange XRange
        {
            get;
        }

        /// <summary>
        /// Gets YRange property
        /// </summary>
        DoubleRange YRange
        {
            get;
        }

        /// <summary>
        /// Gets ActualXAxis property
        /// </summary>
        ChartAxis ActualXAxis { get; }
        /// <summary>
        /// Gets ActualYAxis property
        /// </summary>
        ChartAxis ActualYAxis { get; }
    }

    public interface ISupportAxes2D : ISupportAxes
    {
        ///<summary>
        /// Gets or sets YAxis property
        /// </summary>
        RangeAxisBase YAxis
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets XAxis property
        /// </summary>
        ChartAxisBase2D XAxis
        {
            get;
            set;
        }
    }

    public interface ISupportAxes3D : ISupportAxes
    {
        
    }


    /// <summary>
    /// Interface implementation for ICloneable
    /// </summary>
    public interface ICloneable
    {
        DependencyObject Clone();
    }

}
