using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FunnelSeries displays its path using a set of data's.
    /// </summary>
    /// <seealso cref="FunnelSegment"/>
    /// <seealso cref="PyramidSegment"/>
    /// <seealso cref="PyramidSeries"/>  
    public partial class FunnelSeries : TriangularSeriesBase, ISegmentSelectable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="FunnelMode"/> property. 
        /// </summary>
        public static readonly DependencyProperty FunnelModeProperty =
            DependencyProperty.Register(
                "FunnelMode", 
                typeof(ChartFunnelMode),
                typeof(FunnelSeries),
                new PropertyMetadata(ChartFunnelMode.ValueIsHeight, OnFunnelModeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="MinWidth"/> property. 
        /// </summary>
        public new static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                "MinWidth",
                typeof(double), 
                typeof(FunnelSeries),
                new PropertyMetadata(40d, OnMinWidthChanged));

        #endregion

        #region Fields

        #region Private Fields

        double currY = 0d;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FunnelSeries()
        {
            DefaultStyleKey = typeof(FunnelSeries);
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the y value should interpret the height or width of the funnel block.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.FunnelMode"/>
        /// </value>
        public ChartFunnelMode FunnelMode
        {
            get { return (ChartFunnelMode)GetValue(FunnelModeProperty); }
            set { SetValue(FunnelModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum width for the funnel block.
        /// </summary>
        public new double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of FunnelSeries.
        /// </summary>
        public override void CreateSegments()
        {
            Segments.Clear();
            Adornments.Clear();
            List<double> xValues = GetXValues();
            double sumValues = 0d, gapRatio = this.GapRatio;
            int count = DataCount;
            int explodedIndex = this.ExplodeIndex;
            ChartFunnelMode funnelmode = this.FunnelMode;

            IList<double> toggledYValues = null;

            if (ToggledLegendIndex.Count > 0)
                toggledYValues = GetYValues();
            else
                toggledYValues = YValues;

            for (int i = 0; i < count; i++)
            {
                sumValues += Math.Max(0, Math.Abs(double.IsNaN(toggledYValues[i]) ? 0 : toggledYValues[i]));
            }

            if (funnelmode == ChartFunnelMode.ValueIsHeight)
                this.CalculateValueIsHeightSegments(toggledYValues, xValues, sumValues, gapRatio, count, explodedIndex);
            else
                this.CalculateValueIsWidthSegments(toggledYValues, xValues, sumValues, gapRatio, count, explodedIndex);

            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues, false);
            if (ActualArea != null)
                ActualArea.IsUpdateLegend = true;
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method implementation for Create Transform
        /// </summary>
        /// <param name="size"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        protected internal override IChartTransformer CreateTransformer(Size size, bool create)
        {
            if (create || ChartTransformer == null)
            {
                ChartTransformer = ChartTransform.CreateSimple(size);
            }

            return ChartTransformer;
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Creates the adornment of FunnelSeries.
        /// </summary>
        protected override ChartAdornment CreateAdornment(AdornmentSeries series, double xVal, double yVal, double height, double currY)
        {
            return new TriangularAdornment(xVal, yVal, currY, height, series);
        }

        /// <summary>
        /// Method implementation for ExplodeIndex
        /// </summary>
        /// <param name="i"></param>
        protected override void SetExplodeIndex(int i)
        {
            if (Segments.Count > 0)
            {
                foreach (FunnelSegment segment in Segments)
                {
                    int index = ActualData.IndexOf(segment.Item);
                    if (i == index)
                    {
                        segment.IsExploded = !segment.IsExploded;
                        base.UpdateSegments(i, NotifyCollectionChangedAction.Remove);
                    }
                    else if (i == -1)
                    {
                        segment.IsExploded = false;
                        base.UpdateSegments(i, NotifyCollectionChangedAction.Remove);
                    }
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FunnelSeries()
            {
                FunnelMode = this.FunnelMode,
                GapRatio = this.GapRatio,
                ExplodeOffset = this.ExplodeOffset,
                MinWidth = this.MinWidth
            });
        }

        #endregion

        #region Private Static Methods


        private static void OnFunnelModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunnelSeries funnelSeries = d as FunnelSeries;
            if (funnelSeries != null && funnelSeries.Area != null)
            {
                funnelSeries.Area.IsUpdateLegend = true;
                funnelSeries.Area.ScheduleUpdate();
            }
        }

        private static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunnelSeries funnelSeries = d as FunnelSeries;
            if (funnelSeries != null && funnelSeries.Area != null)
                funnelSeries.Area.ScheduleUpdate();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To calculate the segments if the pyramid mode is vlaueisHeight.
        /// </summary>
        private void CalculateValueIsHeightSegments(IList<double> yValues, List<double> xValues, double sumValues, double gapRatio, int dataCount, int explodedIndex)
        {
            currY = 0d;
            double coefHeight = 1 / sumValues;
            double spacing = gapRatio / (DataCount - 1);

            for (int i = DataCount - 1; i >= 0; i--)
            {
                double height = 0;
                if (!double.IsNaN(YValues[i]))
                {
                    height = Math.Abs(double.IsNaN(yValues[i]) ? 0 : yValues[i]) * coefHeight;
                    FunnelSegment funnelSegment = new FunnelSegment(currY, height, this, explodedIndex == i || this.ExplodeAll ? true : false);
                    funnelSegment.Item = ActualData[i]; // WPF-14426 Funnel series legend and segment colour is changing while setting emptypoint
                    funnelSegment.XData = xValues[i];
                    funnelSegment.YData = YValues[i];

                    if (ToggledLegendIndex.Contains(i))
                        funnelSegment.IsSegmentVisible = false;
                    else
                        funnelSegment.IsSegmentVisible = true;
                    Segments.Add(funnelSegment);

                    if (AdornmentsInfo != null)
                    {
                        ChartAdornment adornment = (this.CreateAdornment(this, xValues[i], yValues[i], 0, double.IsNaN(currY) ? 0 : currY + (height + spacing) / 2));
                        adornment.Item = ActualData[i];
                        Adornments.Add(adornment);
                    }

                    currY += height + spacing;
                }
            }
        }

        /// <summary>
        /// To calculate the segments if the pyramid mode is valueisWidth.
        /// </summary>
        private void CalculateValueIsWidthSegments(IList<double> yValues, List<double> xValues, double sumValues, double gapRatio, int count, int explodedIndex)
        {
            currY = 0d;
            if (ToggledLegendIndex.Count > 0)
                count = YValues.Count - ToggledLegendIndex.Count;
            double offset = 1d / (count - 1);
            double height = (1 - gapRatio) / (count - 1);
            for (int i = DataCount - 1; i > 0; i--)
            {
                if (!double.IsNaN(YValues[i]))
                {
                    double w1 = Math.Abs(YValues[i]);
                    double w2 = 0;
                    if (ToggledLegendIndex.Contains(i - 1))
                    {
                        for (int k = i - 2; k >= 0; k--)
                        {
                            if (!(ToggledLegendIndex.Contains(k)))
                            {
                                w2 = Math.Abs(YValues[k]);
                                break;
                            }
                        }
                    }
                    else
                        w2 = Math.Abs(YValues[i - 1]);

                    if (ToggledLegendIndex.Contains(i))
                    {
                        height = 0;
                        w2 = w1;
                    }
                    else
                        height = (1 - gapRatio) / (count - 1);

                    FunnelSegment funnelSegment = new FunnelSegment(currY, height, w1 / sumValues, w2 / sumValues, this, explodedIndex == i || this.ExplodeAll ? true : false);
                    funnelSegment.Item = ActualData[i];
                    funnelSegment.XData = xValues[i];
                    funnelSegment.YData = YValues[i];

                    if (ToggledLegendIndex.Contains(i))
                        funnelSegment.IsSegmentVisible = false;
                    else
                        funnelSegment.IsSegmentVisible = true;

                    Segments.Add(funnelSegment);
                    if (AdornmentsInfo != null)
                    {
                        Adornments.Add(this.CreateAdornment(this, xValues[i], yValues[i], height, currY));
                        Adornments[Adornments.Count - 1].Item = ActualData[i];
                    }

                    if (!(ToggledLegendIndex.Contains(i)))
                        currY += offset;
                }
            }

            if (AdornmentsInfo != null && DataCount > 0)
            {
                Adornments.Add(this.CreateAdornment(this, xValues[0], yValues[0], height, currY));
                Adornments[Adornments.Count - 1].Item = ActualData[0];
            }
        }

        #endregion

        #endregion
    }
}
