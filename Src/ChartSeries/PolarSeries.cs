using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// PolarSeries displays data points that are grouped by category on a 360-degree circle. 
    /// </summary>
    /// <remarks>
    /// Polar charts are most commonly used to plot polar data, where each data point is determined by an angle and a distance.
    /// </remarks>
    /// <seealso cref="RadarSeries"/>
    public partial class PolarSeries : PolarRadarSeriesBase
    {
        #region Methods

        #region Public Override Methods
        
        /// <summary>
        /// Creates the segments of PolarSeries.
        /// </summary>
        public override void CreateSegments()
        {
            List<double> tempYValues = new List<double>();
            Segments.Clear(); Segment = null;

            if (DrawType == ChartSeriesDrawType.Area)
            {
                double Origin = this.ActualXAxis != null ? this.ActualXAxis.Origin : 0;
                List<double> xValues = GetXValues().ToList();
                tempYValues = (from val in YValues select val).ToList();

                if (xValues != null)
                {
                    if (!IsClosed)
                    {
                        xValues.Insert((int)DataCount - 1, xValues[(int)DataCount - 1]);
                        xValues.Insert(0, xValues[0]);
                        tempYValues.Insert(0, Origin);
                        tempYValues.Insert(tempYValues.Count, Origin);
                    }
                    else
                    {
                        xValues.Insert(0, xValues[0]);
                        tempYValues.Insert(0, YValues[0]);
                        xValues.Insert(0, xValues[(int)DataCount]);
                        tempYValues.Insert(0, YValues[(int)DataCount - 1]);
                    }

                    if (Segment == null)
                    {
                        Segment = new AreaSegment(xValues, tempYValues, this, null);
                        Segment.SetData(xValues, tempYValues);
                        Segments.Add(Segment);
                    }
                    else
                        Segment.SetData(xValues, tempYValues);

                    if (AdornmentsInfo != null)
                        AddAreaAdornments(YValues);
                }
            }
            else if (DrawType == ChartSeriesDrawType.Line)
            {
                int index = -1;
                int i = 0;
                double xIndexValues = 0d;
                List<double> xValues = ActualXValues as List<double>;

                if (IsIndexed || xValues == null)
                {
                    xValues = xValues != null ? (from val in (xValues) select (xIndexValues++)).ToList()
                          : (from val in (ActualXValues as List<string>) select (xIndexValues++)).ToList();
                }

                if (xValues != null)
                {
                    for (i = 0; i < this.DataCount; i++)
                    {
                        index = i + 1;

                        if (index < DataCount)
                        {
                            if (i < Segments.Count)
                            {
                                (Segments[i]).SetData(xValues[i], YValues[i], xValues[index], YValues[index]);
                            }
                            else
                            {
                                LineSegment line = new LineSegment(xValues[i], YValues[i], xValues[index], YValues[index], this, this);
                                Segments.Add(line);
                            }
                        }

                        if (AdornmentsInfo != null)
                        {
                            if (i < Adornments.Count)
                            {
                                Adornments[i].SetData(xValues[i], YValues[i], xValues[i], YValues[i]);
                            }
                            else
                            {
                                Adornments.Add(this.CreateAdornment(this, xValues[i], YValues[i], xValues[i], YValues[i]));
                            }
                        }
                    }

                    if (IsClosed)
                    {
                        LineSegment line = new LineSegment(xValues[0], YValues[0], xValues[i - 1], YValues[i - 1], this, this);
                        Segments.Add(line);
                    }

                    if (ShowEmptyPoints)
                        UpdateEmptyPointSegments(xValues, false);
                }
            }
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
                ChartTransformer = ChartTransform.CreatePolar(new Rect(new Point(), size), this);
            }

            return ChartTransformer;
        }

        #endregion

        #region Protected Override Methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new PolarSeries());
        }

        #endregion

        #endregion
    }
}