// <copyright file="StackingColumnSeries3D.cs" company="Syncfusion. Inc">
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
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// StackingColumnSeries3D is typically preferred in cases of multiple series of type <see cref="ColumnSeries3D" />.
    /// Each series is then stacked vertically one above the other.
    /// If there exists only single series, it will resemble like a simple <see cref="ColumnSeries3D" /> chart.
    /// </summary>
    public partial class StackingColumnSeries3D : StackingSeriesBase3D, ISegmentSpacing
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register(
                "SegmentSpacing", 
                typeof(double),
                typeof(StackingColumnSeries3D),
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
        public StackingColumnSegment3D Segment { get; set; }
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
            get { return true; }
        }

        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets a value indicating whether this series is placed side by side.
        /// </summary>
        /// <returns>
        /// It returns <c>true</c>, if the series is placed side by side [cluster mode].
        /// </returns>
        protected override bool IsStacked
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
        /// creates the segments of StackingColumnSeries3D.
        /// </summary>
        public override void CreateSegments()
        {
            var sbsInfo = GetSideBySideInfo(this);
            var origin = ActualXAxis.Origin;
            if (this.ActualXAxis != null && ActualXAxis.Origin == 0 && ActualYAxis is LogarithmicAxis &&
                (ActualYAxis as LogarithmicAxis).Minimum != null)
            {
                origin = (double)(ActualYAxis as LogarithmicAxis).Minimum;
            }

            var median = sbsInfo.Delta / 2;
            var stackingValues = GetCumulativeStackValues(this);
            if (stackingValues == null)
            {
                return;
            }

            this.YRangeStartValues = stackingValues.StartValues;
            this.YRangeEndValues = stackingValues.EndValues;
            bool isIndexed = ActualXAxis is CategoryAxis3D && !(ActualXAxis as CategoryAxis3D).IsIndexed;
            var xValues = isIndexed ? GroupedXValuesIndexes : GetXValues();
            var zValues = GetZValues();
            var isZAxis = zValues != null && zValues.Count > 0;

            var space = (isZAxis ? Area.ActualDepth : Area.Depth) / 4;
            var start = space;
            var end = space * 3;

            double z1, z2;
            DoubleRange zsbsInfo = DoubleRange.Empty;
            if (isZAxis)
            {
                zsbsInfo = this.GetZSideBySideInfo(this);
            }

            if (this.YRangeStartValues == null)
            {
                this.YRangeStartValues = (from val in xValues select origin).ToList();
            }

            if (this.Area.VisibleSeries != null)
            {
                var stackingseries = (from series in Area.VisibleSeries where (series is StackingColumnSeries3D || series is StackingColumn100Series3D || series is StackingBarSeries3D || series is StackingBar100Series3D) select series).ToList();
                if (stackingseries.Count > 0)
                {
                    double spacing = (stackingseries[0] as StackingColumnSeries3D).SegmentSpacing;
                    for (int index = 0; index < stackingseries.Count; index++)
                    {
                        double seriesspacing = (stackingseries[index] as StackingColumnSeries3D).SegmentSpacing;
                        if ((spacing <= 0 || spacing > 1) && seriesspacing > spacing)
                        {
                            spacing = seriesspacing;
                        }
                    }

                    for (int index = 0; index < stackingseries.Count; index++)
                    {
                        (stackingseries[index] as StackingColumnSeries3D).SegmentSpacing = spacing;
                    }
                }
            }

            if (xValues == null)
            {
                return;
            }

            if (isIndexed)
            {
                Segments.Clear();
                Adornments.Clear();
                int segmentCount = 0;
                for (int i = 0; i < DistinctValuesIndexes.Count; i++)
                {
                    for (int j = 0; j < DistinctValuesIndexes[i].Count; j++)
                    {
                        var x1 = i + sbsInfo.Start;
                        var x2 = i + sbsInfo.End;
                        var y2 = double.IsNaN(YRangeStartValues[segmentCount]) ? origin : YRangeStartValues[segmentCount];
                        var y1 = double.IsNaN(YRangeEndValues[segmentCount]) ? origin : YRangeEndValues[segmentCount];
                        if (isZAxis)
                        {
                            z1 = zValues[i] + zsbsInfo.Start;
                            z2 = zValues[i] + zsbsInfo.End;
                        }
                        else
                        {
                            z1 = start;
                            z2 = end;
                        }

                        Segments.Add(new StackingColumnSegment3D(x1, y1, x2, y2, z1, z2, this)
                        {
                            XData = xValues[segmentCount],
                            YData = GroupedSeriesYValues[0][segmentCount],
                            Item = GroupedActualData[segmentCount]
                        });
                        if (this.AdornmentsInfo == null)
                        {
                            continue;
                        }

                        var xAdornmentPosition = GetXAdornmentAnglePosition(i, sbsInfo);

                        double zAdornmentPosition = 0d;
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
                                this.AddColumnAdornments(i, this.GroupedSeriesYValues[0][segmentCount], xAdornmentPosition, y1, segmentCount, median, zAdornmentPosition);
                                break;
                            case AdornmentsPosition.Bottom:
                                this.AddColumnAdornments(i, this.GroupedSeriesYValues[0][segmentCount], xAdornmentPosition, y2, segmentCount, median, zAdornmentPosition);
                                break;
                            default:
                                this.AddColumnAdornments(i, this.GroupedSeriesYValues[0][segmentCount], xAdornmentPosition, y1 + (y2 - y1) / 2, segmentCount, median, zAdornmentPosition);
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
                    var x1 = xValues[i] + sbsInfo.Start;
                    var x2 = xValues[i] + sbsInfo.End;
                    var y2 = double.IsNaN(YRangeStartValues[i]) ? origin : YRangeStartValues[i];
                    var y1 = double.IsNaN(YRangeEndValues[i]) ? origin : YRangeEndValues[i];
                    z1 = isZAxis ? zValues[i] + zsbsInfo.Start : start;
                    z2 = isZAxis ? zValues[i] + zsbsInfo.End : end;

                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(x1, y1, x2, y2, z1, z2);
                        ((StackingColumnSegment3D)Segments[i]).YData = this.ActualSeriesYValues[0][i];
                        ((StackingColumnSegment3D)Segments[i]).XData = xValues[i];
                        if (isZAxis)
                        {
                            ((StackingColumnSegment3D)Segments[i]).ZData = zValues[i];
                        }

                        ((StackingColumnSegment3D)Segments[i]).Item = this.ActualData[i];
                    }
                    else
                    {
                        Segments.Add(new StackingColumnSegment3D(x1, y1, x2, y2, z1, z2, this)
                        {
                            XData = xValues[i],
                            YData = ActualSeriesYValues[0][i],
                            ZData = isZAxis ? zValues[i] : 0,
                            Item = ActualData[i],
                        });
                    }

                    if (this.AdornmentsInfo == null)
                    {
                        continue;
                    }

                    var xAdornmentPosition = GetXAdornmentAnglePosition(xValues[i], sbsInfo);

                    double zAdornmentPosition = 0d;
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
        /// <param name="spacing">The Spacing.</param>
        /// <param name="right">The Right Position.</param>
        /// <param name="left">The Left Position.</param>
        /// <returns>Returns the calculated segment space.</returns>
        double ISegmentSpacing.CalculateSegmentSpacing(double spacing, double right, double left)
        {
            return StackingColumnSeries3D.CalculateSegmentSpacing(spacing, right, left);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Checks the animation active state.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> property which indicates whether animation is active.</returns>
        internal override bool GetAnimationIsActive()
        {
            return this.AnimationStoryboard != null && AnimationStoryboard.GetCurrentState() == ClockState.Active;
        }

        /// <summary>
        /// Animates the series.
        /// </summary>
        internal override void Animate()
        {
            this.canAnimate = false;
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

            StackingColumnSegment3D segment = null;
            int count = 0;
            var durationInSeconds = AnimationDuration.TotalSeconds; // stackingSeries.Count;
            foreach (ChartSegment stackingColumnSegment in this.Segments)
            {
                if (stackingColumnSegment is EmptyPointSegment)
                {
                    continue;
                }

                segment = stackingColumnSegment as StackingColumnSegment3D;

                var segmentTop = segment.InternalTop;
                var segmentBottom = segment.InternalBottom;
                if (double.IsNaN(segmentTop) || double.IsNaN(segmentBottom))
                {
                    continue;
                }

                var keyFrames = new DoubleAnimationUsingKeyFrames
                {
                };
                var keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0d
                };
                keyFrames.KeyFrames.Add(keyFrame);

                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(durationInSeconds)),
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

                keyFrames = new DoubleAnimationUsingKeyFrames
                {
                    // BeginTime = TimeSpan.FromSeconds(beginTime)
                };
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0d
                };
                keyFrames.KeyFrames.Add(keyFrame);

                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(durationInSeconds)),
                    Value = segmentBottom
                };
                keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;

                keyFrames.KeyFrames.Add(keyFrame);
                Storyboard.SetTargetProperty(keyFrames, "ColumnSegment3D.Bottom");
                keyFrames.EnableDependentAnimation = true;
                Storyboard.SetTarget(keyFrames, segment);
                
                AnimationStoryboard.Children.Add(keyFrames);
                count++;
            }

            AnimationStoryboard.Begin();
        }

        #endregion

        #region Protected Methods

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
            return base.CloneSeries(new StackingColumnSeries3D()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SegmentSpacing = this.SegmentSpacing,
                SelectedIndex = this.SelectedIndex
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
            var series = d as StackingColumnSeries3D;
            if (series.Area != null)
            {
                series.Area.ScheduleUpdate();
            }
        }

        #endregion

        #endregion
    }
}
