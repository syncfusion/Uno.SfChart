using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a ChartSeries that displays data in a customizable chart.
    /// </summary>
    public abstract partial class AdornmentSeries : ChartSeries
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="AdornmentsInfo"/> property.
        /// </summary>
        public static readonly DependencyProperty AdornmentsInfoProperty =
          DependencyProperty.Register("AdornmentsInfo", typeof(ChartAdornmentInfo), typeof(ChartSeriesBase),
          new PropertyMetadata(null, OnAdornmentsInfoChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// <para>Gets or sets data labels for the series.</para> <para>This allows us to customize the appearance of a data point 
        /// by displaying labels, shapes and connector lines.</para>
        /// </summary>
        /// <value>
        /// The <see cref="ChartAdornmentInfo" /> value.
        /// </value>
        public ChartAdornmentInfo AdornmentsInfo
        {
            get
            {
                return (ChartAdornmentInfo)GetValue(AdornmentsInfoProperty);
            }

            set
            {
                SetValue(AdornmentsInfoProperty, value);
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// An abstract method which will be called over to create segments.
        /// </summary>
        public override void CreateSegments()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Internal Override Methods

        internal override void UpdateOnSeriesBoundChanged(Size size)
        {
            if (AdornmentPresenter != null && AdornmentsInfo != null)
            {
                AdornmentsInfo.UpdateElements();
            }

            base.UpdateOnSeriesBoundChanged(size);

            if (AdornmentPresenter != null && AdornmentsInfo != null)
            {
                AdornmentPresenter.Update(size);
                AdornmentPresenter.Arrange(size);
            }
        }

        internal override void CalculateSegments()
        {
            base.CalculateSegments();
            if (DataCount == 0)
            {
                if (AdornmentsInfo != null)
                {
                    if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        ClearUnUsedAdornments(this.DataCount * 4);
                    else
                        ClearUnUsedAdornments(this.DataCount * 2);
                }
            }
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method implementation  for GeneratePoints for Adornments
        /// </summary>
        protected internal override void GeneratePoints()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Method implementation for Create Adornments
        /// </summary>
        /// <param name="series"></param>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        protected virtual ChartAdornment CreateAdornment(AdornmentSeries series, double xVal, double yVal, double xPos, double yPos)
        {
            ChartAdornment adornment = new ChartAdornment(xVal, yVal, xPos, yPos, series);
            adornment.XData = xVal;
            adornment.YData = yVal;
            adornment.XPos = xPos;
            adornment.YPos = yPos;
            adornment.Series = series;
            return adornment;
        }

        /// <summary>
        /// Method implementation for Add ColumnAdornments in Chart
        /// </summary>
        /// <param name="values"></param>
        protected virtual void AddColumnAdornments(params double[] values)
        {
            ////values[0] -->   xData
            ////values[1] -->   yData
            ////values[2] -->   xPos
            ////values[3] -->   yPos
            ////values[4] -->   data point index
            ////values[5] -->   Median value.

            double adornposX = values[2] + values[5], adornposY = values[3];
            int pointIndex = (int)values[4];
            if ((EmptyPointIndexes != null && EmptyPointIndexes.Any() && EmptyPointIndexes[0].Contains(pointIndex) &&
                 (EmptyPointStyle == Charts.EmptyPointStyle.Symbol ||
                  EmptyPointStyle == Charts.EmptyPointStyle.SymbolAndInterior)))
                if (this is StackingSeriesBase)
                    adornposY = (EmptyPointValue == EmptyPointValue.Average) ? values[3] : values[1];
                else
                    adornposY = values[1]; // WPF-13874-EmptyPoint segment adornmentinfo positioning wrongly when EmptyPointValues is Average
            if (pointIndex < Adornments.Count)
            {
                Adornments[pointIndex].SetData(values[0], values[1], adornposX, adornposY);
            }
            else
            {
                Adornments.Add(CreateAdornment(this, values[0], values[1], adornposX, adornposY));
            }

            if (!(this is HistogramSeries))
            {
                if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed
                    && this.GroupedActualData.Count > 0)
                    Adornments[pointIndex].Item = this.GroupedActualData[pointIndex];
                else
                    Adornments[pointIndex].Item = ActualData[pointIndex];
            }
        }

        /// <summary>
        /// Method implementation for Add Adornments at XY
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pointindex"></param>
        protected virtual void AddAdornmentAtXY(double x, double y, int pointindex)
        {
            double adornposX = x, adornposY = y;

            if (pointindex < Adornments.Count)
            {
                Adornments[pointindex].SetData(x, y, adornposX, adornposY);
            }
            else
            {
                Adornments.Add(CreateAdornment(this, x, y, adornposX, adornposY));
            }

            if (pointindex < ActualData.Count)
                Adornments[pointindex].Item = ActualData[pointindex];
        }

        /// <summary>
        /// Method implementation for Add AreaAdornments in ChartAdornments
        /// </summary>
        /// <param name="values"></param>
        protected virtual void AddAreaAdornments(params IList<double>[] values)
        {
            IList<double> yValues = values[0];
            List<double> xValues = new List<double>();
            if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();

            if (values.Length == 1)
            {
                int i;
                for (i = 0; i < DataCount; i++)
                {
                    if (i < xValues.Count && i < yValues.Count)
                    {
                        double adornX = xValues[i];
                        double adornY = yValues[i];

                        if (i < Adornments.Count)
                        {
                            Adornments[i].SetData(xValues[i], yValues[i], adornX, adornY);
                        }
                        else
                        {
                            Adornments.Add(CreateAdornment(this, xValues[i], yValues[i], adornX, adornY));
                        }

                        Adornments[i].Item = ActualData[i];
                    }
                }
            }
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// call this method when Adornments render on Series
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!(this is ErrorBarSeries))
            {
                AdornmentPresenter.Series = this;
                if (Area != null && AdornmentsInfo != null)
                {
                    ////Panel panel = Area.GetMarkerPresenter();
                    Panel panel = AdornmentPresenter;
                    if (panel != null)
                    {
                        AdornmentsInfo.PanelChanged(panel);
                    }
                }
            }
        }

        /// <summary>
        /// Called when DataSource property changed 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            if (AdornmentsInfo != null)
            {
                Adornments.Clear();
                AdornmentsInfo.UpdateElements();
            }

            base.OnDataSourceChanged(oldValue, newValue);

            var area = this.Area;
            if(area != null)
            {
                area.IsUpdateLegend = area.HasDataPointBasedLegend();
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            if (this.AdornmentsInfo != null)
                (obj as AdornmentSeries).AdornmentsInfo = (ChartAdornmentInfo)this.AdornmentsInfo.Clone();
            return base.CloneSeries(obj);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method implementation for Clear Unused Adornments
        /// </summary>
        /// <param name="startIndex"></param>
        protected void ClearUnUsedAdornments(int startIndex)
        {
            if (Adornments.Count > startIndex)
            {
                int count = Adornments.Count;

                for (int i = startIndex; i < count; i++)
                {
                    Adornments.RemoveAt(startIndex);
                }
            }
        }

        #endregion

        #region Private Static Methods

        private static void OnAdornmentsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as AdornmentSeries;

            if (e.OldValue != null)
            {
                var adornmentInfo = e.OldValue as ChartAdornmentInfo;
                if (series != null) series.Adornments.Clear();

                if (adornmentInfo != null)
                {
                    adornmentInfo.ClearChildren();
                    adornmentInfo.Series = null;
                }
            }

            if (e.NewValue != null)
            {
                if (series != null)
                {
                    series.adornmentInfo = e.NewValue as ChartAdornmentInfo;
                    series.AdornmentsInfo.Series = series;
                    if (series.Area != null && series.AdornmentsInfo != null)
                    {
                        ////Panel panel = series.Area.GetMarkerPresenter();
                        Panel panel = series.AdornmentPresenter;
                        if (panel != null)
                        {
                            series.AdornmentsInfo.PanelChanged(panel);
                            series.Area.ScheduleUpdate();
                        }
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
