// <copyright file="ColumnSeries3D.cs" company="Syncfusion. Inc">
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
    using System.ComponentModel;
    using System.Linq;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// ColumnSeries displays its data points using a set of vertical bars.
    /// </summary>
    public partial class ColumnSeries3D : XyzDataSeries3D, ISegmentSpacing
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register(
                "SegmentSpacing", 
                typeof(double), 
                typeof(ColumnSeries3D),
                new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets the spacing between the segments across the series in cluster mode.
        /// </summary>
        /// <value>
        ///     The value ranges from 0 to 1.
        /// </value>
        public double SegmentSpacing
        {
            get { return (double)GetValue(SegmentSpacingProperty); }
            set { this.SetValue(SegmentSpacingProperty, value); }
        }
        
#if NETFX_CORE
        /// <summary>
        /// Gets or sets the segment for internal usage
        /// </summary>
        public ColumnSegment3D Segment { get; set; }
#endif
        
        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets a value indicating whether this series is placed side by side.
        /// </summary>
        /// <returns>
        /// It returns <c>true</c>, if the series is placed side by side [cluster mode].
        /// </returns>
        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        #endregion

        #endregion

        #region methods

        #region Public Properties
        
        /// <summary>
        /// Creates the segments of ColumnSeries.
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

            var zValues = GetZValues();
            var isZAxis = zValues != null && zValues.Count > 0;
            double median;
            if (xValues == null)
            {
                return;
            }

            var area3D = this.ActualArea as SfChart3D;
            var depthInfo = GetSegmentDepth(isZAxis ? area3D.ActualDepth : area3D.Depth);
            var sbsInfo = GetSideBySideInfo(this);
            DoubleRange zsbsInfo = DoubleRange.Empty;

            if (isZAxis)
            {
                zsbsInfo = this.GetZSideBySideInfo(this);
            }

            median = sbsInfo.Delta / 2;
            double z1, z2;
            double start = depthInfo.Start;
            double end = depthInfo.End;
            int segmentCount = 0;
            if (isGrouping && this.GroupedSeriesYValues[0] != null && GroupedSeriesYValues[0].Count > 0)
            {
                Segments.Clear();
                Adornments.Clear();
                GroupedActualData.Clear();
                for (int i = 0; i < DistinctValuesIndexes.Count; i++)
                {
                    var list = (from index in DistinctValuesIndexes[i]
                                select new List<double> { GroupedSeriesYValues[0][index], index }).ToList();
                    double startValue = isZAxis ? zsbsInfo.Start : depthInfo.Start;

                    var divider = zsbsInfo.Delta / list.Count;
                    var segmentWidth = divider * 0.75;
                    var segmentSpace = divider * 0.25;

                    for (int j = 0; j < list.Count; j++)
                    {
                        var yValue = list[j][0];
                        var x1 = i + sbsInfo.Start;
                        var x2 = i + sbsInfo.End;
                        var y1 = yValue;
                        var y2 = ActualXAxis != null ? ActualXAxis.Origin : 0;
                        var actualStart = 0d;
                        var actualEnd = 0d;

                        GroupedActualData.Add(this.ActualData[(int)list[j][1]]);
                        if (list.Count > 1)
                        {
                            if (isZAxis)
                            {
                                start = startValue;
                                end = start + segmentWidth;
                                startValue = end + segmentSpace;

                                actualStart = start;
                                actualEnd = end;

                                start = zValues[i] + start;
                                end = zValues[i] + end;
                            }
                            else
                            {
                                var count = list.Count;
                                var space = depthInfo.End / ((count * 2) + count + 1);
                                start = startValue + space;
                                end = start + depthInfo.End / (count * 2);
                                startValue = end;
                            }
                        }
                        else
                        {
                            if (isZAxis)
                            {
                                actualStart = zsbsInfo.Start;
                                actualEnd = zsbsInfo.End;

                                start = zValues[i] + zsbsInfo.Start;
                                end = zValues[i] + zsbsInfo.End;
                            }
                            else
                            {
                                start = depthInfo.Start;
                                end = depthInfo.End;
                            }
                        }

                        Segments.Add(new ColumnSegment3D(x1, y1, x2, y2, start, end, this)
                        {
                            XData = xValues[j],
                            YData = yValue,
                            Item = GroupedActualData[segmentCount]
                        });

                        if (this.AdornmentsInfo == null)
                        {
                            continue;
                        }

                        double xAdornmentPosition = 0d;
                        double zAdornmentPosition = 0d;

                        xAdornmentPosition = this.GetXAdornmentAnglePosition(i, sbsInfo);
                        if (isZAxis)
                        {
                            zAdornmentPosition = this.GetZAdornmentAnglePosition(zValues[i], new DoubleRange(actualStart, actualEnd));
                        }
                        else
                        {
                            zAdornmentPosition = this.GetZAdornmentAnglePosition(start, end);
                        }

                        switch (AdornmentsInfo.AdornmentsPosition)
                        {
                            case AdornmentsPosition.Top:
                                this.AddColumnAdornments(i, yValue, xAdornmentPosition, y1, segmentCount, median, zAdornmentPosition);
                                break;
                            case AdornmentsPosition.Bottom:
                                this.AddColumnAdornments(i, yValue, xAdornmentPosition, y2, segmentCount, median, zAdornmentPosition);
                                break;
                            default:
                                this.AddColumnAdornments(i, yValue, xAdornmentPosition, y1 + (y2 - y1) / 2, segmentCount, median, zAdornmentPosition);
                                break;
                        }

                        segmentCount++;
                    }
                }
            }
            else
            {
                this.ClearUnUsedSegments(this.DataCount);
                this.ClearUnUsedAdornments(this.DataCount);
                for (var i = 0; i < this.DataCount; i++)
                {
                    if (i >= this.DataCount)
                    {
                        continue;
                    }

                    var x1 = xValues[i] + sbsInfo.Start;
                    var x2 = xValues[i] + sbsInfo.End;
                    var y1 = YValues[i];
                    var y2 = ActualXAxis != null ? ActualXAxis.Origin : 0;

                    if (isZAxis)
                    {
                        z1 = zValues[i] + zsbsInfo.Start;
                        z2 = zValues[i] + zsbsInfo.End;
                    }
                    else
                    {
                        z1 = depthInfo.Start;
                        z2 = depthInfo.End;
                    }

                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(x1, y1, x2, y2, z1, z2);
                        ((ColumnSegment3D)Segments[i]).YData = this.YValues[i];
                        ((ColumnSegment3D)Segments[i]).Plans = null;
                        ((ColumnSegment3D)Segments[i]).XData = xValues[i];
                        if (isZAxis)
                        {
                            ((ColumnSegment3D)Segments[i]).ZData = zValues[i];
                        }

                        ((ColumnSegment3D)Segments[i]).Item = this.ActualData[i];
                    }
                    else
                    {
                        Segments.Add(new ColumnSegment3D(x1, y1, x2, y2, z1, z2, this)
                        {
                            XData = xValues[i],
                            YData = YValues[i],
                            ZData = isZAxis ? zValues[i] : 0,
                            Item = ActualData[i]
                        });
                    }

                    if (this.AdornmentsInfo == null)
                    {
                        continue;
                    }

                    double xAdornmentPosition = 0d;
                    double zAdornmentPosition = 0d;

                    xAdornmentPosition = this.GetXAdornmentAnglePosition(xValues[i], sbsInfo);
                    if (isZAxis)
                    {
                        zAdornmentPosition = this.GetZAdornmentAnglePosition(zValues[i], zsbsInfo);
                    }
                    else
                    {
                        zAdornmentPosition = this.GetZAdornmentAnglePosition(start, end);
                    }

                    switch (AdornmentsInfo.AdornmentsPosition)
                    {
                        case AdornmentsPosition.Top:
                            this.AddColumnAdornments(xValues[i], this.YValues[i], xAdornmentPosition, y1, i, median, zAdornmentPosition);
                            break;
                        case AdornmentsPosition.Bottom:
                            this.AddColumnAdornments(xValues[i], this.YValues[i], xAdornmentPosition, y2, i, median, zAdornmentPosition);
                            break;
                        default:
                            this.AddColumnAdornments(xValues[i], this.YValues[i], xAdornmentPosition, y1 + (y2 - y1) / 2, i, median, zAdornmentPosition);
                            break;
                    }
                }
            }

            if (this.ShowEmptyPoints)
            {
                this.UpdateEmptyPointSegments(xValues, true);
            }
        }
        
        /// <summary>
        /// Calculates the segment spacing.
        /// </summary>
        /// <param name="spacing">The Spacing</param>
        /// <param name="right">The Right</param>
        /// <param name="left">The Left</param>
        /// <returns>Returns the calculated segment space.</returns>
        double ISegmentSpacing.CalculateSegmentSpacing(double spacing, double right, double left)
        {
            return ColumnSeries3D.CalculateSegmentSpacing(spacing, right, left);
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Updates the series when transpose changed.
        /// </summary>
        /// <param name="val">The Value</param>
        internal override void OnTransposeChanged(bool val)
        {
            this.IsActualTransposed = val;
        }
        
        /// <summary>
        /// Gets the animation is active.
        /// </summary>
        /// <returns>Returns the animation state whether it is active or not.</returns>
        internal override bool GetAnimationIsActive()
        {
            return this.AnimationStoryboard != null && AnimationStoryboard.GetCurrentState() == ClockState.Active;
        }

        /// <summary>
        /// Animates the series.
        /// </summary>
        internal override void Animate()
        {
            if (Segments.Count <= 0)
            {
                return;
            }

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
            ColumnSegment3D segment = null;
            var top = ActualYAxis.VisibleRange.End;
            foreach (ChartSegment columnsSegment in this.Segments)
            {
                if (columnsSegment is EmptyPointSegment)
                {
                    continue;
                }

                segment = columnsSegment as ColumnSegment3D;
                var segmentTop = top < segment.Top ? top : segment.Top;
                if (double.IsNaN(segmentTop))
                {
                    continue;
                }

                var keyFrames = new DoubleAnimationUsingKeyFrames();
                var keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = segment.Bottom
                };
                keyFrames.KeyFrames.Add(keyFrame);

                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                    Value = segmentTop
                };
                var keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;

                keyFrames.KeyFrames.Add(keyFrame);
                Storyboard.SetTargetProperty(keyFrames, "ColumnSegment3D.Top");
                keyFrames.EnableDependentAnimation = true;
                Storyboard.SetTarget(keyFrames, segment);
                AnimationStoryboard.Children.Add(keyFrames);
            }

            AnimationStoryboard.Begin();
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Calculates the segment spacing.
        /// </summary>
        /// <param name="spacing">The Spacing.</param>
        /// <param name="right">The Right Position.</param>
        /// <param name="left">The Left Position.</param>
        /// <returns>Returns the calculated segment space.</returns>
        protected static double CalculateSegmentSpacing(double spacing, double right, double left)
        {
            double diff = right - left;
            double totalspacing = diff * spacing / 2;
            left = left + totalspacing;
            right = right - totalspacing;
            return left;
        }
        
        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new ColumnSeries3D()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                SegmentSpacing = this.SegmentSpacing
            });
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Updates the segments when segment spacing value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as ColumnSeries3D;
            if (series.Area != null)
            {
                series.Area.ScheduleUpdate();
            }
        }
        
        #endregion

        #endregion
    }
}
