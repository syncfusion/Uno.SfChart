// <copyright file="AreaSeries3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Class implementation for AreaSeries3D
    /// </summary>
    public partial class AreaSeries3D : XyDataSeries3D
    {
        #region Fields

        private Point hitPoint = new Point();

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSeries3D"/> class.
        /// </summary>
        public AreaSeries3D()
        {
            IsAnimated = false;
        }

        #endregion
        
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="AreaSegment3D"/> of this series.
        /// </summary>
        public AreaSegment3D Segment { get; set; }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="AreaSeries3D"/> is animated.
        /// </summary>
        internal bool IsAnimated { get; set; }
        
        #endregion

        #region Protected Internal Properties

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
        ///  Returns <c>true</c> if its linear, otherwise it returns <c>false</c>.
        /// </remarks>
        protected internal override bool IsAreaTypeSeries
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
        /// It returns <see cref="List{ChartSegment}"/>.
        /// </returns>
        protected internal override List<ChartSegment> SelectedSegments
        {
            get
            {
                if (SelectedSegmentsIndexes.Count > 0)
                {
                    selectedSegments.Clear();
                    foreach (var index in this.SelectedSegmentsIndexes)
                    {
                        selectedSegments.Add(this.GetDataPoint(index));
                    }

                    return this.selectedSegments;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the selected segment in this series, when we enable the single selection.
        /// </summary>
        /// <returns>
        /// It returns <c>ChartSegment</c>.
        /// </returns>
        protected internal override ChartSegment SelectedSegment
        {
            get
            {
                return this.GetDataPoint(this.SelectedIndex);
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Creates the segments of AreaSeries3D.
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = null;
            bool isGrouping = this.ActualXAxis is CategoryAxis3D && !(this.ActualXAxis as CategoryAxis3D).IsIndexed;
            if (isGrouping)
            {
                xValues = this.GroupedXValuesIndexes;
            }
            else
            {
                xValues = this.GetXValues();
            }

            if (xValues == null)
            {
                return;
            }

            var depthInfo = GetSegmentDepth(this.Area.Depth);
            if (isGrouping)
            {
                Segments.Clear();
                Adornments.Clear();
                if (this.GroupedSeriesYValues != null && GroupedSeriesYValues[0].Contains(double.NaN))
                {
                    List<List<double>> yValList;
                    List<List<double>> xValList;
                    this.CreateEmptyPointSegments(this.GroupedSeriesYValues[0], out yValList, out xValList);
                }
                else if (xValues != null)
                {
                    if (this.Segment == null || Segments.Count == 0)
                    {
                        this.Segment = new AreaSegment3D(xValues, GroupedSeriesYValues[0], depthInfo.Start, depthInfo.End, this);
                        this.Segment.SetData(xValues, this.GroupedSeriesYValues[0], depthInfo.Start, depthInfo.End);
                        Segments.Add(this.Segment);
                    }
                }

                for (var i = 0; i < xValues.Count; i++)
                {
                    if (this.AdornmentsInfo != null)
                    {
                        this.AddAdornments(xValues[i], this.GroupedSeriesYValues[0][i], i, depthInfo.Start);
                    }
                }
            }
            else
            {
                this.ClearUnUsedSegments(this.DataCount);
                this.ClearUnUsedAdornments(this.DataCount);
                if (YValues.Contains(double.NaN))
                {
                    List<List<double>> yValList;
                    List<List<double>> xValList;
                    this.CreateEmptyPointSegments(this.YValues, out yValList, out xValList);
                }
                else if (xValues != null)
                {
                    if (this.Segment == null || Segments.Count == 0)
                    {
                        this.Segment = new AreaSegment3D(xValues, YValues, depthInfo.Start, depthInfo.End, this);
                        this.Segment.SetData(xValues, this.YValues, depthInfo.Start, depthInfo.End);
                        Segments.Add(this.Segment);
                    }
                    else
                    {
                        this.Segment.SetData(xValues, this.YValues, depthInfo.Start, depthInfo.End);
                    }
                }
                
                for (var i = 0; i < this.DataCount; i++)
                {
                    if (this.AdornmentsInfo != null)
                    {
                        this.AddAdornments(xValues[i], this.YValues[i], i, depthInfo.Start);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the empty segments.
        /// </summary>
        /// <param name="yValues">The y values</param>
        /// <param name="yValList">The y value list</param>
        /// <param name="xValList">The x value list</param>
        public override void CreateEmptyPointSegments(IList<double> yValues, out List<List<double>> yValList, out List<List<double>> xValList)
        {            
            IList<double> xValues = (ActualXValues is IList<double> && !IsIndexed) ? ActualXValues as IList<double> : GetXValues();
            var depthInfo = GetSegmentDepth(this.Area.ActualDepth);
            base.CreateEmptyPointSegments(yValues, out yValList, out xValList);
            int j = 0;

            // EmptyPoint calculation 
            if (Segments.Count != yValList.Count)
            {
                Segments.Clear();
            }

            if (this.Segment == null || Segments.Count == 0)
            {
                for (int i = 0; i < yValList.Count && i < xValList.Count; i++)
                {
                    if (i < xValList.Count && i < yValList.Count && xValList[i].Count > 0 && yValList[i].Count > 0)
                    {
                        this.Segment = new AreaSegment3D(xValList[i], yValList[i], depthInfo.Start, depthInfo.End, this);
                        this.Segment.SetData(xValList[i], yValList[i], depthInfo.Start, depthInfo.End);
                        Segments.Add(this.Segment);
                    }
                }
            }
            else if (xValues != null)
            {
                foreach (var segment in this.Segments)
                {
                    if (j < xValList.Count && j < yValList.Count && xValList[j].Count > 0 && yValList[j].Count > 0)
                    {
                        segment.SetData(xValList[j], yValList[j], depthInfo.Start, depthInfo.End);
                    }

                    j++;
                }
            }
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Updates the series when selected segment index collection changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        internal override void SelectedSegmentsIndexes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && !((ActualArea as SfChart3D).SelectionStyle == SelectionStyle3D.Single))
                    {
                        int oldIndex = PreviousSelectedIndex;

                        int newIndex = (int)e.NewItems[0];

                        if (newIndex >= 0 && (ActualArea as SfChart3D).EnableSegmentSelection)
                        {
                            if (Segments.Count > 0)
                            {
                                chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                {
                                    SelectedSegment = this.SelectedSegment,
                                    SelectedSegments = Area.SelectedSegments,
                                    SelectedSeries = this,
                                    SelectedIndex = newIndex,
                                    PreviousSelectedIndex = oldIndex,
                                    IsSelected = true
                                };

                                chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                                if (oldIndex != -1 && oldIndex < Segments.Count)
                                {
                                    chartSelectionChangedEventArgs.PreviousSelectedSegment = this.Segments[0];
                                    chartSelectionChangedEventArgs.OldPointInfo = this.GetDataPoint(oldIndex);
                                }

                                (ActualArea as SfChart3D).OnSelectionChanged(chartSelectionChangedEventArgs);
                                this.PreviousSelectedIndex = newIndex;
                            }
                            else if (Segments.Count == 0)
                            {
                                this.triggerSelectionChangedEventOnLoad = true;
                            }
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    if (e.OldItems != null && !((ActualArea as SfChart3D).SelectionStyle == SelectionStyle3D.Single))
                    {
                        int newIndex = (int)e.OldItems[0];

                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = null,
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = null,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = PreviousSelectedIndex,
                            PreviousSelectedSeries = this,
                            PreviousSelectedSegment = null,
                            IsSelected = false
                        };

                        if (this.PreviousSelectedIndex != -1 && this.PreviousSelectedIndex < Segments.Count)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = this.Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = this.GetDataPoint(this.PreviousSelectedIndex);
                        }

                        (ActualArea as SfChart3D).OnSelectionChanged(chartSelectionChangedEventArgs);
                        this.PreviousSelectedIndex = newIndex;
                    }

                    break;
            }
        }

        /// <summary>
        /// Checks the animation active state.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> property which indicates whether animation is active.</returns>
        internal override bool GetAnimationIsActive()
        {
            return this.AnimationStoryboard != null && AnimationStoryboard.GetCurrentState() == ClockState.Active;
        }

        /// <summary>
        /// Animates the <see cref="AreaSeries3D"/>.
        /// </summary>
        internal override void Animate()
        {
            // WPF-25124 Animation not working properly when resize the window.
            if (this.AnimationStoryboard != null)
            {
                AnimationStoryboard.Stop();
                if (!this.canAnimate)
                {
                    this.ResetAdornmentAnimationState();
                    return;
                }
            }

            this.AnimationStoryboard = new Storyboard();
            foreach (AreaSegment3D segment in this.Segments)
            {
                var dblAnimationKeyFrames = new DoubleAnimationUsingKeyFrames();

                var keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = this.ActualYAxis.VisibleRange.Start
                };
                dblAnimationKeyFrames.KeyFrames.Add(keyFrame);
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                    Value = this.Area.InternalSecondaryAxis.VisibleRange.End
                };
                EasingDoubleKeyFrame ease = new EasingDoubleKeyFrame();
                ease.Value = this.YValues.Max();
                ease.KeyTime = this.AnimationDuration;
                ease.EasingFunction = new CircleEase();
                var keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;
                dblAnimationKeyFrames.KeyFrames.Add(ease);
                dblAnimationKeyFrames.KeyFrames.Add(keyFrame);
                Storyboard.SetTargetProperty(dblAnimationKeyFrames, "AreaSegment3D.Y");
                dblAnimationKeyFrames.EnableDependentAnimation = true;
                Storyboard.SetTarget(dblAnimationKeyFrames, segment);
                AnimationStoryboard.Children.Add(dblAnimationKeyFrames);
            }

            AnimationStoryboard.Begin();
        }

        /// <summary>
        /// This method used to gets the chart data point at a position.
        /// </summary>
        /// <param name="point">The Mouse Position</param>
        /// <returns>Returns the data point.</returns>
        internal override ChartDataPointInfo GetDataPoint(Point point)
        {
            var frontPlane = new Polygon3D(new Vector3D(0, 0, 1), 0);
            var transform = (ActualArea as SfChart3D).Graphics3D.Transform;
            frontPlane.Transform(transform.View);

            var position = transform.ToPlane(point, frontPlane);

            Rect rect;
            int startIndex, endIndex;
            List<int> hitIndexes = new List<int>();
            IList<double> xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            this.CalculateHittestRect(new Point(position.X, position.Y), out startIndex, out endIndex, out rect);

            for (int i = startIndex; i <= endIndex; i++)
            {
                this.hitPoint.X = this.IsIndexed ? i : xValues[i];
                this.hitPoint.Y = this.YValues[i];

                if (rect.Contains(this.hitPoint))
                {
                    hitIndexes.Add(i);
                }
            }

            if (hitIndexes.Count > 0)
            {
                int i = hitIndexes[hitIndexes.Count / 2];
                hitIndexes = null;

                this.dataPoint = new ChartDataPointInfo();
                dataPoint.Index = i;
                dataPoint.Series = this;
                dataPoint.XData = xValues[i];
                dataPoint.YData = this.YValues[i];
                if (ActualData.Count > i)
                {
                    dataPoint.Item = this.ActualData[i];
                }

                return this.dataPoint;
            }
            else
            {
                return this.dataPoint;
            }
        }

        #endregion

        #region Protected Internal Methods
        
        /// <summary>
        /// Updates the series when mouse moved.
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="pos">The Position</param>
        protected internal override void OnSeriesMouseMove(object source, Point pos)
        {
            if (this.SelectionMode == SelectionMode.MouseMove)
            {
                this.OnMouseMoveSelection(source as FrameworkElement, pos);
            }

            this.UpdateToolTip(source, pos);
        }

        /// <summary>
        /// Method used to set SegmentSelectionBrush to selected index chart segment and trigger chart selection event
        /// </summary>
        /// <param name="newIndex">The New Index</param>
        /// <param name="oldIndex">The Old Index</param>
        protected internal override void SelectedIndexChanged(int newIndex, int oldIndex)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            if (this.ActualArea != null)
            {
                if (newIndex >= 0 && (ActualArea as SfChart3D).EnableSegmentSelection)
                {
                    if (Segments.Count > 0)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = this.SelectedSegment,
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            IsSelected = true
                        };

                        chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                        if (oldIndex != -1 && oldIndex < Segments.Count)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = this.Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = this.GetDataPoint(oldIndex);
                        }

                        (ActualArea as SfChart3D).OnSelectionChanged(chartSelectionChangedEventArgs);
                        this.PreviousSelectedIndex = newIndex;
                    }
                    else if (Segments.Count == 0)
                    {
                        this.triggerSelectionChangedEventOnLoad = true;
                    }
                }
                else if (newIndex == -1 && this.ActualArea != null)
                {
                    chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                    {
                        SelectedSegment = null,
                        SelectedSegments = Area.SelectedSegments,
                        SelectedSeries = null,
                        SelectedIndex = newIndex,
                        PreviousSelectedIndex = oldIndex,
                        PreviousSelectedSeries = this,
                        PreviousSelectedSegment = null,
                        IsSelected = false
                    };

                    if (oldIndex != -1 && oldIndex < Segments.Count)
                    {
                        chartSelectionChangedEventArgs.PreviousSelectedSegment = this.Segments[0];
                        chartSelectionChangedEventArgs.OldPointInfo = this.GetDataPoint(oldIndex);
                    }

                    (ActualArea as SfChart3D).OnSelectionChanged(chartSelectionChangedEventArgs);
                    this.PreviousSelectedIndex = newIndex;
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned series</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new AreaSeries3D()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the tooltip for the series.
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="pos">The Position</param>
        private void UpdateToolTip(object source, Point pos)
        {
            {
                if (this.ShowTooltip)
                {
                    object customTag = null;
                    this.SetTooltipDuration();
                    var canvas = this.Area.GetAdorningCanvas();
                    this.mousePos = pos;
                    double xVal = 0;
                    double yVal = 0;
                    double stackedYValue = double.NaN;
                    object data = null;
                    int index = 0;

                    var shape = source as Shape;
                    if ((shape != null) && shape.Tag is ChartSegment3D)
                    {
                        customTag = shape.Tag;
                    }
                    else
                    {
                        customTag = this.Segments[0];
                    }

                    if (this.Area.SeriesClipRect.Contains(this.mousePos))
                    {
                        var point = new Point(
                            mousePos.X - this.Area.SeriesClipRect.Left,
                            mousePos.Y - this.Area.SeriesClipRect.Top);

                        this.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);
                        if (double.IsNaN(xVal))
                        {
                            return;
                        }

                        if (ActualXAxis is CategoryAxis3D && !(ActualXAxis as CategoryAxis3D).IsIndexed)
                        {
                            index = (int)GroupedXValuesIndexes[(int)xVal];
                        }
                        else
                        {
                            index = this.GetXValues().IndexOf(xVal);
                        }

                        data = this.ActualData[index];
                    }

                    if (this.Area.Tooltip == null)
                    {
                        this.Area.Tooltip = new ChartTooltip();
                    }

                    var chartTooltip = this.Area.Tooltip as ChartTooltip;
                    var areaSegment3D = customTag as AreaSegment3D;
                    areaSegment3D.Item = data;
                    areaSegment3D.XData = xVal;
                    areaSegment3D.YData = yVal;
                    if (chartTooltip != null)
                    {
                        chartTooltip.ClearValue(ChartTooltip.ContentProperty);
                        chartTooltip.ClearValue(ChartTooltip.ContentTemplateProperty);
                        if (canvas.Children.Count == 0 || (canvas.Children.Count > 0 && !this.IsTooltipAvailable(canvas)))
                        {
                            chartTooltip.Content = customTag;
                            if (chartTooltip.Content == null)
                            {
                                return;
                            }

                            if (ChartTooltip.GetInitialShowDelay(this) == 0)
                            {
                                canvas.Children.Add(chartTooltip);
                            }

                            chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                            this.AddTooltip();

                            if (ChartTooltip.GetEnableAnimation(this))
                            {
                                this.SetDoubleAnimation(chartTooltip);
                                storyBoard.Children.Add(this.leftDoubleAnimation);
                                storyBoard.Children.Add(this.topDoubleAnimation);
                                Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                                Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                                _stopwatch.Start();
                            }
                            else
                            {
                                Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                                Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                                _stopwatch.Start();
                            }
                        }
                        else
                        {
                            foreach (var child in canvas.Children)
                            {
                                var tooltip = child as ChartTooltip;
                                if (tooltip != null)
                                {
                                    chartTooltip = tooltip;
                                }
                            }

                            chartTooltip.Content = customTag;
                            if (chartTooltip.Content == null)
                            {
                                this.RemoveTooltip();
                                return;
                            }

                            chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                            this.AddTooltip();
                            if (_stopwatch.ElapsedMilliseconds > 100)
                            {
                                if (ChartTooltip.GetEnableAnimation(this))
                                {
                                    _stopwatch.Restart();
                                    if (this.leftDoubleAnimation == null || this.topDoubleAnimation == null || this.storyBoard == null)
                                    {
                                        this.SetDoubleAnimation(chartTooltip);
                                    }
                                    else
                                    {
                                        leftDoubleAnimation.To = chartTooltip.LeftOffset;
                                        topDoubleAnimation.To = chartTooltip.TopOffset;
                                        storyBoard.Begin();
                                    }
#if WPF
                            }
#endif
                                }
                                else
                                {
                                    _stopwatch.Restart();
                                    Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                                    Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                                }
                            }
                            else if (this.EnableAnimation == false)
                            {
                                Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                                Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
