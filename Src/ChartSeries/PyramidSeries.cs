using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// PyramidSeries displays data as a proportion of the whole.PyramidSeries are most commonly used to make comparisons among a set of given data.
    /// </summary>
    /// <remarks>
    /// PyramidSeries does not have any axis. The segments in PyramidSeries can be explode to a certain distance from the center using <see>
    /// <cref>PyramidSeries.ExplodeIndex</cref>
    /// </see>
    /// property.
    /// The segments can be filled with a custom set of colors using <see cref="ChartColorModel.CustomBrushes"/> property.
    /// </remarks>
    /// <seealso cref="PyramidSegment"/>
    /// <seealso cref="FunnelSegment"/>
    /// <seealso cref="FunnelSeries"/>    
    public partial class PyramidSeries : TriangularSeriesBase, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="PyramidMode"/> property.       
        /// </summary>
        public static readonly DependencyProperty PyramidModeProperty =
            DependencyProperty.Register(
                "PyramidMode", 
                typeof(ChartPyramidMode), 
                typeof(PyramidSeries),
                new PropertyMetadata(ChartPyramidMode.Linear, OnPyramidModeChanged));

        #endregion

        #region Fields

        double currY = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for PyramidSeries
        /// </summary>
        public PyramidSeries()
        {
            DefaultStyleKey = typeof(PyramidSeries);
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the y value should interpret the length or surface of the pyramid block.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.ChartPyramidMode"/>
        /// </value>
        public ChartPyramidMode PyramidMode
        {
            get { return (ChartPyramidMode)GetValue(PyramidModeProperty); }
            set { SetValue(PyramidModeProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Static Methods

        /// <summary>
        /// To get the SurfaceHeight for PyramidSeries.
        /// </summary>
        public static double GetSurfaceHeight(double y, double surface)
        {
            double r1, r2;
            if (ChartMath.SolveQuadraticEquation(1, 2 * y, -surface, out r1, out r2))
            {
                return Math.Max(r1, r2);
            }

            return double.NaN;
        }

        #endregion

        #region Public Override Methods

        /// <summary>
        /// Creates the segment of PyramidSeries.
        /// </summary>
        public override void CreateSegments()
        {
            Adornments.Clear();
            this.Segments.Clear();
            int count = DataCount;
            List<double> xValues = GetXValues();
            IList<double> toggledYValues = null;
            if (ToggledLegendIndex.Count > 0)
                toggledYValues = GetYValues();
            else
                toggledYValues = YValues;
            double sumValues = 0;
            double gapRatio = this.GapRatio;
            ChartPyramidMode pyramidMode = this.PyramidMode;

            for (int i = 0; i < count; i++)
            {
                sumValues += Math.Max(0, Math.Abs(double.IsNaN(toggledYValues[i]) ? 0 : toggledYValues[i]));
            }

            double gapHeight = gapRatio / (count - 1);
            if (pyramidMode == ChartPyramidMode.Linear)
                this.CalculateLinearSegments(sumValues, gapRatio, count, xValues);
            else
                this.CalculateSurfaceSegments(sumValues, count, gapHeight, xValues);

            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues, false);

            if (ActualArea != null)
                ActualArea.IsUpdateLegend = true;
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Return IChartTranform value based upon the given size
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
        /// Creates the adornment of PyramidSeries.
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
                foreach (PyramidSegment segment in Segments)
                {
                    int index = ActualData.IndexOf(segment.Item);
                    if (i == index)
                    {
                        segment.isExploded = !segment.isExploded;
                        base.UpdateSegments(i, NotifyCollectionChangedAction.Remove);
                    }
                    else if (i == -1)
                    {
                        segment.isExploded = false;
                        base.UpdateSegments(i, NotifyCollectionChangedAction.Remove);
                    }
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new PyramidSeries() { PyramidMode = this.PyramidMode });
        }

        #endregion

        #region Private Static Methods

        private static void OnPyramidModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PyramidSeries pyramidSeries = d as PyramidSeries;
            if (pyramidSeries != null && pyramidSeries.Area != null)
                pyramidSeries.Area.ScheduleUpdate();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To calculate the segments if the pyramid mode is linear
        /// </summary>
        private void CalculateLinearSegments(double sumValues, double gapRatio, int count, List<double> xValues)
        {
            var toggledYValues = YValues.ToList();
            if (ToggledLegendIndex.Count > 0)
                toggledYValues = GetYValues();
            currY = 0;
            double coef = 1d / (sumValues * (1 + gapRatio / (1 - gapRatio)));

            for (int i = 0; i < count; i++)
            {
                double height = 0;
                if (!double.IsNaN(YValues[i]))
                {
                    height = coef * Math.Abs(double.IsNaN(toggledYValues[i]) ? 0 : toggledYValues[i]);
                    PyramidSegment pyramidSegment = new PyramidSegment(currY, height, this.ExplodeOffset, this, i == ExplodeIndex || this.ExplodeAll ? true : false);
                    pyramidSegment.Item = ActualData[i];
                    pyramidSegment.XData = xValues[i];
                    pyramidSegment.YData = Math.Abs(YValues[i]);
                    if (ToggledLegendIndex.Contains(i))
                        pyramidSegment.IsSegmentVisible = false;
                    else
                        pyramidSegment.IsSegmentVisible = true;
                    this.Segments.Add(pyramidSegment);
                    currY += (gapRatio / (count - 1)) + height;
                    if (AdornmentsInfo != null)
                    {
                        Adornments.Add(this.CreateAdornment(this, xValues[i], toggledYValues[i], 0, double.IsNaN(currY) ? 1 - height / 2 : currY - height / 2));
                        Adornments[Segments.Count - 1].Item = ActualData[i];
                    }
                }
            }
        }

        /// <summary>
        /// To calculate the segments if the pyramid mode is surface
        /// </summary>
        private void CalculateSurfaceSegments(double sumValues, int count, double gapHeight, List<double> xValues)
        {
            var toggledYValues = YValues.ToList();
            if (ToggledLegendIndex.Count > 0)
                toggledYValues = GetYValues();
            currY = 0;
            double[] y = new double[count];
            double[] height = new double[count];
            double preSum = GetSurfaceHeight(0, sumValues);

            for (int i = 0; i < count; i++)
            {
                y[i] = currY;
                height[i] = GetSurfaceHeight(currY, Math.Abs(double.IsNaN(toggledYValues[i]) ? 0 : toggledYValues[i]));
                currY += height[i] + gapHeight * preSum;
            }

            double coef = 1 / (currY - gapHeight * preSum);
            for (int i = 0; i < count; i++)
            {
                double currHeight = 0;
                if (!double.IsNaN(YValues[i]))
                {
                    currHeight = coef * y[i];
                    PyramidSegment pyramidSegment = new PyramidSegment(currHeight, coef * height[i], this.ExplodeOffset, this, i == this.ExplodeIndex || this.ExplodeAll ? true : false);
                    pyramidSegment.Item = ActualData[i];
                    pyramidSegment.XData = xValues[i];
                    pyramidSegment.YData = Math.Abs(YValues[i]);
                    if (ToggledLegendIndex.Contains(i))
                        pyramidSegment.IsSegmentVisible = false;
                    else
                        pyramidSegment.IsSegmentVisible = true;
                    this.Segments.Add(pyramidSegment);

                    if (AdornmentsInfo != null)
                    {
                        Adornments.Add(this.CreateAdornment(this, xValues[i], toggledYValues[i], 0, double.IsNaN(currHeight) ? 1 - height[i] / 2 : currHeight + coef * height[i] / 2));
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
