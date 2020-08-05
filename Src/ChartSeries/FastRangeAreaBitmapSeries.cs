// <copyright file="FastRangeAreaBitmapSeries.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastRangeAreaBitmapSeries is another version of RangeAreaSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    /// </remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    /// <seealso cref="FastHiLoBitmapSeries"/>
    public partial class FastRangeAreaBitmapSeries : RangeSeriesBase, ISegmentSelectable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="EnableAntiAliasing"/> property. 
        /// </summary>
        public static readonly DependencyProperty EnableAntiAliasingProperty =
            DependencyProperty.Register(
                "EnableAntiAliasing",
                typeof(bool),
                typeof(FastRangeAreaBitmapSeries),
                new PropertyMetadata(false, OnSeriesPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.       
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush",
                typeof(Brush),
                typeof(FastRangeAreaBitmapSeries),
                new PropertyMetadata(null, OnSeriesPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(FastRangeAreaBitmapSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HighValueInterior"/> property.       
        /// </summary>
        public static readonly DependencyProperty HighValueInteriorProperty =
            DependencyProperty.Register(
                "HighValueInterior",
                typeof(Brush),
                typeof(FastRangeAreaBitmapSeries),
                new PropertyMetadata(null, OnSeriesPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LowValueInterior"/> property.       
        /// </summary>
        public static readonly DependencyProperty LowValueInteriorProperty =
            DependencyProperty.Register(
                "LowValueInterior",
                typeof(Brush),
                typeof(FastRangeAreaBitmapSeries),
                new PropertyMetadata(null, OnSeriesPropertyChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to enable the anti aliasing for the bitmap series, to draw smooth edges.
        /// </summary>
        public bool EnableAntiAliasing
        {
            get { return (bool)GetValue(EnableAntiAliasingProperty); }
            set { SetValue(EnableAntiAliasingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior(brush) for the selected segment(s).
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        /// <example>
        ///     <code>
        ///     series.SegmentSelectionBrush = new SolidColorBrush(Colors.Red);
        ///     </code>
        /// </example>
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected segment.
        /// </summary>
        /// <value>
        /// <c>Int</c> value represents the index of the data point(or segment) to be selected. 
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior brush for the high value segment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush HighValueInterior
        {
            get { return (Brush)GetValue(HighValueInteriorProperty); }
            set { SetValue(HighValueInteriorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior brush for the low value segment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush LowValueInterior
        {
            get { return (Brush)GetValue(LowValueInteriorProperty); }
            set { SetValue(LowValueInteriorProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Used to indicate whether multipleYValues is needed,will be set internally.
        /// </summary>
        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// This indicates whether its a bitmap series or not.
        /// </summary>
        protected internal override bool IsBitmapSeries
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the selected segments in this series, when we enable the multiple selection.
        /// </summary>
        /// <returns>
        /// It returns <see cref="ChartSegment"/>.
        /// </returns>
        protected internal override List<ChartSegment> SelectedSegments
        {
            get
            {
                if (SelectedSegmentsIndexes.Count > 0)
                {
                    selectedSegments.Clear();

                    foreach (var index in SelectedSegmentsIndexes)
                    {
                        selectedSegments.Add(GetDataPoint(index));
                    }

                    return selectedSegments;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// The property confirms the linearity of this series.
        /// </summary>
        /// <remarks>
        ///  Returns <c>true</c> if its linear, otherwise it returns <c>false</c>.
        /// </remarks>
        protected internal override bool IsLinear
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// This property used to confirm whether it is area typed series.
        /// </summary>
        /// <remarks>
        ///  Returns <c>true</c> if its area type series, otherwise it returns <c>false</c>.
        /// </remarks>
        protected internal override bool IsAreaTypeSeries
        {
            get
            {
                return true;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Creates the segments of <see cref="FastRangeAreaBitmapSeries"/>.
        /// </summary>
        public override void CreateSegments()
        {
            ChartPoint point1;
            ChartPoint point2;
            ChartPoint point3;
            ChartPoint point4;

            ChartPoint? crossPoint = null;
            List<ChartPoint> segPoints = new List<ChartPoint>();

            List<double> xValues = null;
            if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();

            if (xValues != null)
            {
                bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
                if (!isGrouping)
                {
                    Segments.Clear();
                    Adornments.Clear();
                    if (double.IsNaN(GroupedSeriesYValues[1][0]) || !double.IsNaN(GroupedSeriesYValues[0][0]))
                    {
                        segPoints.Add(new ChartPoint(xValues[0], GroupedSeriesYValues[1][0]));
                        segPoints.Add(new ChartPoint(xValues[0], GroupedSeriesYValues[0][0]));
                        var segment = new FastRangeAreaSegment(segPoints, false, this)
                        {
                            High = GroupedSeriesYValues[0][0],
                            Low = GroupedSeriesYValues[1][0],
                            Item = ActualData[0]
                        };

                        AddSegment(segment, segPoints);
                    }

                    segPoints = new List<ChartPoint>();
                    int i;

                    for (i = 0; i < xValues.Count - 1; i++)
                    {
                        if (!double.IsNaN(GroupedSeriesYValues[1][i]) && !double.IsNaN(GroupedSeriesYValues[0][i]))
                        {
                            point1 = new ChartPoint(xValues[i], GroupedSeriesYValues[1][i]);
                            point3 = new ChartPoint(xValues[i], GroupedSeriesYValues[0][i]);

                            if (i == 0 || (i < xValues.Count - 1 && (double.IsNaN(GroupedSeriesYValues[1][i - 1]) || double.IsNaN(GroupedSeriesYValues[0][i - 1]))))
                            {
                                segPoints.Add(point1);
                                segPoints.Add(point3);
                            }

                            if (!double.IsNaN(GroupedSeriesYValues[1][i + 1]) && !double.IsNaN(GroupedSeriesYValues[0][i + 1]))
                            {
                                point2 = new ChartPoint(xValues[i + 1], GroupedSeriesYValues[1][i + 1]);
                                point4 = new ChartPoint(xValues[i + 1], GroupedSeriesYValues[0][i + 1]);

                                // UWP-8718 Use ChartMath.GetCrossPoint since it returns the ChartDataPoint withou rounding the values.
                                crossPoint = ChartMath.GetCrossPoint(point1, point2, point3, point4);

                                if (crossPoint != null)
                                {
                                    var crossPointValue = new ChartPoint(crossPoint.Value.X, crossPoint.Value.Y);
                                    segPoints.Add(crossPointValue);
                                    segPoints.Add(crossPointValue);
                                    var segment = new FastRangeAreaSegment(segPoints, (GroupedSeriesYValues[1][i] > GroupedSeriesYValues[0][i]), this)
                                    {
                                        High = GroupedSeriesYValues[0][i],
                                        Low = GroupedSeriesYValues[1][i],
                                        Item = ActualData[i]
                                    };

                                    AddSegment(segment, segPoints);
                                    segPoints = new List<ChartPoint>();
                                    segPoints.Add(crossPointValue);
                                    segPoints.Add(crossPointValue);
                                }

                                segPoints.Add(point2);
                                segPoints.Add(point4);
                            }
                            else if (i != 0 && !double.IsNaN(GroupedSeriesYValues[1][i - 1]) && !double.IsNaN(GroupedSeriesYValues[0][i - 1]))
                            {
                                segPoints.Add(point1);
                                segPoints.Add(point3);
                            }
                        }
                        else
                        {
                            if (segPoints.Count > 0)
                            {
                                if (!double.IsNaN(GroupedSeriesYValues[1][i - 1]) && !double.IsNaN(GroupedSeriesYValues[0][i - 1]))
                                {
                                    var segment = new FastRangeAreaSegment(segPoints, false, this)
                                    {
                                        High = GroupedSeriesYValues[0][i - 1],
                                        Low = GroupedSeriesYValues[1][i - 1],
                                        Item = ActualData[i - 1]
                                    };

                                    AddSegment(segment, segPoints);
                                }
                            }

                            segPoints = new List<ChartPoint>();
                        }
                    }

                    if (segPoints.Count > 0)
                    {
                        var segment = new FastRangeAreaSegment(segPoints, (GroupedSeriesYValues[1][i] > GroupedSeriesYValues[0][i]), this)
                        {
                            High = GroupedSeriesYValues[0][i],
                            Low = GroupedSeriesYValues[1][i],
                            Item = ActualData[i]
                        };

                        AddSegment(segment, segPoints);
                    }
                    else if (i == xValues.Count - 1 && (double.IsNaN(GroupedSeriesYValues[1][i]) || double.IsNaN(GroupedSeriesYValues[0][i])))
                    {
                        segPoints.Add(new ChartPoint(xValues[i], GroupedSeriesYValues[1][i]));
                        segPoints.Add(new ChartPoint(xValues[i], GroupedSeriesYValues[0][i]));
                        var segment = new FastRangeAreaSegment(segPoints, false, this)
                        {
                            High = GroupedSeriesYValues[0][i],
                            Low = GroupedSeriesYValues[1][i],
                            Item = ActualData[i]
                        };

                        AddSegment(segment, segPoints);
                    }

                    for (int j = 0; j < xValues.Count; j++)
                    {
                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[j], 0, GroupedSeriesYValues[0][j], GroupedSeriesYValues[1][j], j);
                    }
                }
                else
                {
                    Segments.Clear();
                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 2);
                        else
                            ClearUnUsedAdornments(this.DataCount);
                    }

                    if (xValues != null)
                    {
                        if (double.IsNaN(LowValues[0]) || double.IsNaN(HighValues[0]))
                        {
                            segPoints.Add(new ChartPoint(xValues[0], LowValues[0]));
                            segPoints.Add(new ChartPoint(xValues[0], HighValues[0]));
                            var segment = new FastRangeAreaSegment(segPoints, false, this)
                            {
                                High = HighValues[0],
                                Low = LowValues[0],
                                Item = ActualData[0]
                            };

                            AddSegment(segment, segPoints);
                        }

                        segPoints = new List<ChartPoint>();
                        int i;
                        for (i = 0; i < DataCount - 1; i++)
                        {
                            if (!double.IsNaN(LowValues[i]) && !double.IsNaN(HighValues[i]))
                            {
                                point1 = new ChartPoint(xValues[i], LowValues[i]);
                                point3 = new ChartPoint(xValues[i], HighValues[i]);

                                if (i == 0 || (i < DataCount - 1 && (double.IsNaN(LowValues[i - 1]) || double.IsNaN(HighValues[i - 1]))))
                                {
                                    segPoints.Add(point1);
                                    segPoints.Add(point3);
                                }

                                if (!double.IsNaN(LowValues[i + 1]) && !double.IsNaN(HighValues[i + 1]))
                                {
                                    point2 = new ChartPoint(xValues[i + 1], LowValues[i + 1]);
                                    point4 = new ChartPoint(xValues[i + 1], HighValues[i + 1]);

                                    // UWP-8718 Use ChartMath.GetCrossPoint since it returns the ChartDataPoint withou rounding the values.
                                    crossPoint = ChartMath.GetCrossPoint(point1, point2, point3, point4);

                                    if (crossPoint != null)
                                    {
                                        var crossPointValue = new ChartPoint(crossPoint.Value.X, crossPoint.Value.Y);
                                        segPoints.Add(crossPointValue);
                                        segPoints.Add(crossPointValue);
                                        var segment = new FastRangeAreaSegment(segPoints, (LowValues[i] > HighValues[i]), this)
                                        {
                                            High = HighValues[i],
                                            Low = LowValues[i],
                                            Item = ActualData[i]
                                        };

                                        AddSegment(segment, segPoints);
                                        segPoints = new List<ChartPoint>();
                                        segPoints.Add(crossPointValue);
                                        segPoints.Add(crossPointValue);
                                    }

                                    segPoints.Add(point2);
                                    segPoints.Add(point4);
                                }
                            }
                            else
                            {
                                if (segPoints.Count > 0)
                                {
                                    if (!double.IsNaN(LowValues[i - 1]) && !double.IsNaN(HighValues[i - 1]))
                                    {
                                        var segment = new FastRangeAreaSegment(segPoints, false, this)
                                        {
                                            High = HighValues[i - 1],
                                            Low = LowValues[i - 1],
                                            Item = ActualData[i - 1]
                                        };

                                        AddSegment(segment, segPoints);
                                    }
                                }

                                segPoints = new List<ChartPoint>();
                            }
                        }

                        if (segPoints.Count > 0)
                        {
                            var segment = new FastRangeAreaSegment(segPoints, (LowValues[i] > HighValues[i]), this)
                            {
                                High = HighValues[i],
                                Low = LowValues[i],
                                Item = ActualData[i]
                            };

                            AddSegment(segment, segPoints);
                        }
                        else if (i == DataCount - 1 && (double.IsNaN(LowValues[i]) || double.IsNaN(HighValues[i])))
                        {
                            segPoints.Add(new ChartPoint(xValues[i], LowValues[i]));
                            segPoints.Add(new ChartPoint(xValues[i], HighValues[i]));
                            var segment = new FastRangeAreaSegment(segPoints, false, this)
                            {
                                High = HighValues[i],
                                Low = LowValues[i],
                                Item = ActualData[i]
                            };

                            AddSegment(segment, segPoints);
                        }
                    }

                    for (int i = 0; i < xValues.Count; i++)
                    {
                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], 0, HighValues[i], LowValues[i], i);
                    }
                }
            }

            // Updates the stroke rendering for empty points.
            if (Segments.Count > 1)
                UpdateEmptyPointSegments();
        }

        #endregion

        #region Internal Override Methods

        /// <summary>
        /// This method used to gets the chart data point at a position.
        /// </summary>
        /// <param name="mousePos">The mouse position.</param>
        /// <returns>Returns the data point nearest to the mouse position.</returns>
        internal override ChartDataPointInfo GetDataPoint(Point mousePos)
        {
            Rect rect;
            int startIndex, endIndex;
            List<int> hitIndexes = new List<int>();
            IList<double> xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            CalculateHittestRect(mousePos, out startIndex, out endIndex, out rect);

            Point y1Value = new Point();
            Point y2Value = new Point();

            for (int i = startIndex; i <= endIndex; i++)
            {
                y1Value.X = IsIndexed ? i : xValues[i];
                y1Value.Y = ActualSeriesYValues[0][i];

                y2Value.X = IsIndexed ? i : xValues[i];
                y2Value.Y = ActualSeriesYValues[1][i];

                if (rect.Contains(y1Value) || rect.Contains(y2Value))
                    hitIndexes.Add(i);
            }

            if (hitIndexes.Count > 0)
            {
                int i = hitIndexes[hitIndexes.Count / 2];
                hitIndexes = null;

                dataPoint = new ChartDataPointInfo();
                dataPoint.Index = i;
                dataPoint.XData = xValues[i];
                dataPoint.High = ActualSeriesYValues[0][i];
                dataPoint.Low = ActualSeriesYValues[1][i];
                dataPoint.Series = this;
                if (i > -1 && ActualData.Count > i)
                    dataPoint.Item = ActualData[i];

                return dataPoint;
            }
            else
                return dataPoint;
        }

        /// <summary>
        /// This method used to get the chart data index at an <see cref="SfChart"/> co-ordinates
        /// </summary>
        /// <param name="point">The point to be passed to get the data point index.</param>
        /// <returns>Returns the data point index.</returns>
        internal override int GetDataPointIndex(Point point)
        {
            Canvas canvas = Area.GetAdorningCanvas();
            double left = Area.ActualWidth - canvas.ActualWidth;
            double top = Area.ActualHeight - canvas.ActualHeight;
            ChartDataPointInfo data = null;
            point.X = point.X - left + Area.Margin.Left;
            point.Y = point.Y - top + Area.Margin.Top;

            Point mousePos = new Point(point.X - Area.SeriesClipRect.Left, point.Y - Area.SeriesClipRect.Top);

            double currentBitmapPixel = (Area.fastRenderSurface.PixelWidth * (int)mousePos.Y + (int)mousePos.X);

            if (!Area.isBitmapPixelsConverted)
                Area.ConvertBitmapPixels();

            if (Pixels.Contains((int)currentBitmapPixel))
                data = GetDataPoint(point);

            if (data != null)
                return data.Index;
            else
                return -1;
        }

        /// <summary>
        /// Updates the selection when selected index collection changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        internal override void SelectedSegmentsIndexes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    if (e.NewItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int oldIndex = PreviousSelectedIndex;

                        int newIndex = (int)e.NewItems[0];

                        // For adornment selection implementation
                        if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                        {
                            dataPoint = GetDataPoint(newIndex);

                            if (dataPoint != null && SegmentSelectionBrush != null)
                            {
                                // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                                if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                                {
                                    UpdateAdornmentSelection(newIndex);
                                }

                                // Generate pixels for the particular data point
                                if (Segments.Count > 0) GeneratePixels();

                                // Set the SegmentSelectionBrush to the selected segment pixels
                                OnBitmapSelection(selectedSegmentPixels, SegmentSelectionBrush, true);
                            }

                            // trigger the SelectionChanged event
                            if (ActualArea != null && Segments.Count > 0)
                            {
                                chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                {
                                    SelectedSegment = Segments[0],
                                    SelectedSegments = Area.SelectedSegments,
                                    SelectedSeries = this,
                                    SelectedIndex = newIndex,
                                    PreviousSelectedIndex = oldIndex,
                                    NewPointInfo = GetDataPoint(newIndex),
                                    IsSelected = true
                                };

                                chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                                if (oldIndex != -1)
                                {
                                    chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                                    chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                                }

                                (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                                PreviousSelectedIndex = newIndex;
                            }
                            else if (Segments.Count == 0)
                            {
                                triggerSelectionChangedEventOnLoad = true;
                            }
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    if (e.OldItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int newIndex = (int)e.OldItems[0];

                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = null,
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = null,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = PreviousSelectedIndex,
                            PreviousSelectedSegment = null,
                            PreviousSelectedSeries = this,
                            IsSelected = false
                        };

                        if (PreviousSelectedIndex != -1)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(PreviousSelectedIndex);
                        }

                            (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                        OnResetSegment(newIndex);
                        PreviousSelectedIndex = newIndex;
                    }

                    break;
            }
        }

        /// <summary>
        /// Updates the segment when reset.
        /// </summary>
        /// <param name="index">The index on reset.</param>
        internal override void OnResetSegment(int index)
        {
            if (index >= 0)
            {
                dataPoint = GetDataPoint(index);

                if (dataPoint != null)
                {
                    // Resets the adornment selection when the mouse pointer moved away from the adornment or clicked the same adornment.
                    if (adornmentInfo != null)
                        AdornmentPresenter.ResetAdornmentSelection(index, false);

                    if (SegmentSelectionBrush != null)
                    {
                        // Generate pixels for the particular data point
                        if (Segments.Count > 0) GeneratePixels();

                        // Reset the segment pixels
                        OnBitmapSelection(selectedSegmentPixels, null, false);

                        selectedSegmentPixels.Clear();
                        dataPoint = null;
                    }
                }
            }
        }
        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Method used to set SegmentSelectionBrush to SelectedIndex segment
        /// </summary>
        /// <param name="newIndex">The new index passed.</param>
        /// <param name="oldIndex">The old index passed.</param>
        protected internal override void SelectedIndexChanged(int newIndex, int oldIndex)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            if (ActualArea != null && ActualArea.SelectionBehaviour != null && !ActualArea.GetEnableSeriesSelection())
            {
                // Reset the old segment
                if (ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                {
                    if (SelectedSegmentsIndexes.Contains(oldIndex))
                        SelectedSegmentsIndexes.Remove(oldIndex);

                    OnResetSegment(oldIndex);
                }

                if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                {
                    if (!SelectedSegmentsIndexes.Contains(newIndex))
                        SelectedSegmentsIndexes.Add(newIndex);

                    dataPoint = GetDataPoint(newIndex);

                    if (dataPoint != null && SegmentSelectionBrush != null)
                    {
                        // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                        if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                        {
                            UpdateAdornmentSelection(newIndex);
                        }

                        // Generate pixels for the particular data point
                        if (Segments.Count > 0) GeneratePixels();

                        // Set the SegmentSelectionBrush to the selected segment pixels
                        OnBitmapSelection(selectedSegmentPixels, SegmentSelectionBrush, true);
                    }

                    // trigger the SelectionChanged event
                    if (ActualArea != null && Segments.Count > 0)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = Segments[0],
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            NewPointInfo = GetDataPoint(newIndex),
                            IsSelected = true
                        };

                        chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                        if (oldIndex != -1)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                        }

                        (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                        PreviousSelectedIndex = newIndex;
                    }
                    else if (Segments.Count == 0)
                    {
                        triggerSelectionChangedEventOnLoad = true;
                    }
                }
                else if (newIndex == -1)
                {
                    chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                    {
                        SelectedSegment = null,
                        SelectedSegments = Area.SelectedSegments,
                        SelectedSeries = null,
                        SelectedIndex = newIndex,
                        PreviousSelectedIndex = oldIndex,
                        PreviousSelectedSegment = null,
                        PreviousSelectedSeries = this,
                        IsSelected = false
                    };

                    if (oldIndex != -1)
                    {
                        chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                        chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                    }

                    (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                    PreviousSelectedIndex = newIndex;
                }
            }
            else if (newIndex >= 0 && Segments.Count == 0)
            {
                triggerSelectionChangedEventOnLoad = true;
            }
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue">The old value passed for the items source.</param>
        /// <param name="newValue">The new value passed for the items source.</param>
        protected override void OnDataSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            Segment = null;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (ShowTooltip)
            {
                Canvas canvas = ActualArea.GetAdorningCanvas();
                mousePos = e.GetCurrentPoint(canvas).Position;
                ChartDataPointInfo dataPoint = new ChartDataPointInfo();
                int index = ChartExtensionUtils.GetAdornmentIndex(e.OriginalSource);
                if (index > -1)
                {
                    if ((ActualXAxis is CategoryAxis) && (!(ActualXAxis as CategoryAxis).IsIndexed))
                    {
                        if (GroupedSeriesYValues[0].Count > index)
                            dataPoint.High = GroupedSeriesYValues[0][index];

                        if (GroupedSeriesYValues[1].Count > index)
                            dataPoint.Low = GroupedSeriesYValues[1][index];
                    }
                    else
                    {
                        if (ActualSeriesYValues[0].Count > index)
                            dataPoint.High = ActualSeriesYValues[0][index];
                        if (ActualSeriesYValues[1].Count > index)
                            dataPoint.Low = ActualSeriesYValues[1][index];
                    }

                    dataPoint.Index = index;
                    dataPoint.Series = this;
                    if (ActualData.Count > index)
                        dataPoint.Item = ActualData[index];
                    UpdateSeriesTooltip(dataPoint);
                }
            }
        }      

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new RangeAreaSeries()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                HighValueInterior = this.HighValueInterior,
                LowValueInterior = this.LowValueInterior
            });
        }

        #endregion

        #region Static Private Methods

        /// <summary>
        /// Updates the series when the series property changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastRangeAreaBitmapSeries).UpdateArea();
        }

        /// <summary>
        /// Updates the selection index.
        /// </summary>
        /// <param name="d">The Dependency Property</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.ActualArea == null || series.ActualArea.SelectionBehaviour == null) return;
            if (series.ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                series.SelectedIndexChanged((int)e.NewValue, (int)e.OldValue);
            else if ((int)e.NewValue != -1)
                series.SelectedSegmentsIndexes.Add((int)e.NewValue);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the created segment to the segment collection.
        /// </summary>
        /// <param name="segment">The segment to be added.</param>
        /// <param name="segPoints">The segment values used to render the <see cref="FastRangeAreaSegment"/>.</param>
        private void AddSegment(FastRangeAreaSegment segment, List<ChartPoint> segPoints)
        {
            segment.Series = this;
            segment.SetData(segPoints);
            Segments.Add(segment);
        }

        /// <summary>
        /// Updates the stroke rendering for empty points.
        /// </summary>
        private void UpdateEmptyPointSegments()
        {
            FastRangeAreaSegment previousEmptySegment = null;

            for (int i = 0; i < Segments.Count; i++)
            {
                FastRangeAreaSegment segment = Segments[i] as FastRangeAreaSegment;

                int count = segment.AreaValues.Count(value => !double.IsNaN(value.Y));

                if (count > 1 && count == segment.AreaValues.Count)
                {
                    if (previousEmptySegment == null)
                    {
                        segment.EmptyStroke = EmptyStroke.Right;
                    }
                    else if (previousEmptySegment.EmptyStroke == EmptyStroke.Right)
                    {
                        segment.EmptyStroke = EmptyStroke.Left;
                    }
                    else if (previousEmptySegment.EmptyStroke == EmptyStroke.Left)
                    {
                        previousEmptySegment.EmptyStroke = EmptyStroke.Both;
                        segment.EmptyStroke = EmptyStroke.Left;
                    }

                    previousEmptySegment = segment;
                }
            }
        }
        #endregion

        #endregion
    }
}
