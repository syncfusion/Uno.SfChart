using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for TriangularSeriesBase
    /// </summary>
    public abstract partial class TriangularSeriesBase : AccumulationSeriesBase
    {

        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="GapRatio"/> property.       .
        /// </summary>
        public static readonly DependencyProperty GapRatioProperty =
            DependencyProperty.Register(
                "GapRatio",
                typeof(double),
                typeof(TriangularSeriesBase),
                new PropertyMetadata(0d, new PropertyChangedCallback(OnGapRatioChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ExplodeOffset"/> property.       .
        /// </summary>
        public static readonly DependencyProperty ExplodeOffsetProperty =
            DependencyProperty.Register(
                "ExplodeOffset", 
                typeof(double), 
                typeof(TriangularSeriesBase),
                new PropertyMetadata(40d, new PropertyChangedCallback(OnExplodeOffsetChanged)));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the ratio of distance between the funnel or pyramid segment blocks. This is a bindable property.
        /// </summary>
        public double GapRatio
        {
            get { return (double)GetValue(GapRatioProperty); }
            set { SetValue(GapRatioProperty, value); }
        }

        /// <summary>
        /// Gets or sets the offset distance when exploding the funnel or pyramid segment. This is a bindable property.
        /// </summary>
        public double ExplodeOffset
        {
            get { return (double)GetValue(ExplodeOffsetProperty); }
            set { SetValue(ExplodeOffsetProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Override Methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var triangularSeriesBase = obj as TriangularSeriesBase;

            if (triangularSeriesBase != null)
            {
                triangularSeriesBase.GapRatio = this.GapRatio;
                triangularSeriesBase.ExplodeOffset = this.ExplodeOffset;
            }

            return base.CloneSeries(obj);
        }

        #endregion

        #region Private Static Methods

        private static void OnGapRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var triangularSeriesBase = d as TriangularSeriesBase;
            if (triangularSeriesBase != null && triangularSeriesBase.Area != null)
                triangularSeriesBase.Area.ScheduleUpdate();
        }
        
        private static void OnExplodeOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var triangularSeriesBase = d as TriangularSeriesBase;
            if (triangularSeriesBase != null && triangularSeriesBase.Area != null)
            {
                triangularSeriesBase.Area.ScheduleUpdate();
            }
        }

        #endregion

        #endregion
    }
}
