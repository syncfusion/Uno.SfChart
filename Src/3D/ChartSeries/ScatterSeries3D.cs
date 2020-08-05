// <copyright file="ScatterSeries3D.cs" company="Syncfusion. Inc">
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
    using System.Linq;
    using System.Text;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// Class implementation for the <see cref="ScatterSeries3D"/>.
    /// </summary>
    public partial class ScatterSeries3D : XyzDataSeries3D
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="ScatterWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty ScatterWidthProperty =
            DependencyProperty.Register(
                "ScatterWidth", 
                typeof(double), 
                typeof(ScatterSeries3D),
                new PropertyMetadata(20d, OnSizeChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ScatterHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty ScatterHeightProperty =
            DependencyProperty.Register(
                "ScatterHeight", 
                typeof(double), 
                typeof(ScatterSeries3D),
                new PropertyMetadata(20d, OnSizeChanged));
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterSeries3D"/> class.
        /// </summary>
        public ScatterSeries3D()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="ScatterSegment3D"/> property.
        /// </summary>
        public ScatterSegment3D Segment { get; set; }
        
        /// <summary>
        /// Gets or sets the width of the scatter points.
        /// </summary>
        public double ScatterWidth
        {
            get { return (double)GetValue(ScatterWidthProperty); }
            set { this.SetValue(ScatterWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the scatter points.
        /// </summary>
        public double ScatterHeight
        {
            get { return (double)GetValue(ScatterHeightProperty); }
            set { this.SetValue(ScatterHeightProperty, value); }
        }

        #endregion

        #region Methods
       
        #region Public Methods
        
        /// <summary>
        /// Creates the segments of ScatterSeries3D.
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = null;
            bool isGrouping = this.ActualXAxis is CategoryAxis3D && !(this.ActualXAxis as CategoryAxis3D).IsIndexed;
            if (isGrouping)
                xValues = this.GroupedXValuesIndexes;
            else
                xValues = this.GetXValues();
            if (xValues == null) return;

            var zValues = GetZValues();
            bool isZAxis = zValues != null && zValues.Count > 0;
            var area3D = this.ActualArea as SfChart3D;
            var depthInfo = GetSegmentDepth(isZAxis ? area3D.ActualDepth : area3D.Depth);

            double z1, z2;
            if (isGrouping)
            {
                Segments.Clear();
                Adornments.Clear();
                for (var i = 0; i < xValues.Count; i++)
                {
                    if (i >= xValues.Count) continue;
                    if (isZAxis)
                    {
                        z1 = zValues[i];
                        z2 = zValues[i];
                    }
                    else
                    {
                        z1 = depthInfo.Start;
                        z2 = depthInfo.End;
                    }

                    if (i < xValues.Count)
                    {
                        Segments.Add(new ScatterSegment3D(xValues[i], GroupedSeriesYValues[0][i], z1, z2, this)
                        {
                            XData = xValues[i],
                            YData = GroupedSeriesYValues[0][i],
                            Item = ActualData[i]
                        });
                        if (this.AdornmentsInfo != null)
                            this.AddAdornments(xValues[i], this.GroupedSeriesYValues[0][i], i, z1);
                    }
                }
            }
            else
            {
                this.ClearUnUsedSegments(this.DataCount);
                this.ClearUnUsedAdornments(this.DataCount);
                for (var i = 0; i < this.DataCount; i++)
                {
                    if (isZAxis)
                    {
                        z1 = zValues[i];
                        z2 = zValues[i];
                    }
                    else
                    {
                        z1 = depthInfo.Start;
                        z2 = depthInfo.End;
                    }

                    if (i >= this.DataCount) continue;
                    if (i < Segments.Count)
                    {
                        (Segments[i] as ScatterSegment3D).SetData(xValues[i], this.YValues[i], z1, z2);
                        (Segments[i] as ScatterSegment3D).YData = this.YValues[i];
                        (Segments[i] as ScatterSegment3D).XData = xValues[i];
                        (Segments[i] as ScatterSegment3D).ZData = isZAxis ? zValues[i] : 0;
                        (Segments[i] as ScatterSegment3D).Item = ActualData[i];
                        ((ScatterSegment3D)Segments[i]).Plans = null;
                    }
                    else
                    {
                        Segments.Add(new ScatterSegment3D(xValues[i], YValues[i], z1, z2, this)
                        {
                            XData = xValues[i],
                            YData = YValues[i],
                            ZData = isZAxis ? zValues[i] : 0,
                            Item = ActualData[i]
                        });
                    }

                    if (this.AdornmentsInfo != null)
                    {
                        this.AddAdornments(xValues[i], this.YValues[i], i, z1);
                    }
                }
            }

            if (this.ShowEmptyPoints)
                this.UpdateEmptyPointSegments(xValues, false);
        }

        #endregion
        
        #region Internal Methods
        
        /// <summary>
        /// Checks the animation active state.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> value indicating animation state.</returns>
        internal override bool GetAnimationIsActive()
        {
            return this.AnimationStoryboard != null && AnimationStoryboard.GetCurrentState() == ClockState.Active;
        }

        /// <summary>
        /// Animates the <see cref="ScatterSeries3D"/>.
        /// </summary>
        internal override void Animate()
        {
            int i = 0;
            Random rand = new Random();

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
            ScatterSegment3D segment = null;
            foreach (ChartSegment scatterSegment in this.Segments)
            {
                if (scatterSegment is EmptyPointSegment) continue;
                segment = scatterSegment as ScatterSegment3D;
                if (double.IsNaN(segment.Y)) continue;
                int randomValue = Segments.IndexOf(segment) == 0 ? 0 : rand.Next(1, 20);
                TimeSpan beginTime = TimeSpan.FromMilliseconds(randomValue * 20);

                var dblAnimationKeyFrames = new DoubleAnimationUsingKeyFrames();
                var keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0d
                };
                dblAnimationKeyFrames.KeyFrames.Add(keyFrame);
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                    Value = segment.ScatterHeight
                };
                var keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;
                dblAnimationKeyFrames.KeyFrames.Add(keyFrame);
                Storyboard.SetTargetProperty(dblAnimationKeyFrames, "ScatterSegment3D.ScatterHeight");
                dblAnimationKeyFrames.EnableDependentAnimation = true;
                Storyboard.SetTarget(dblAnimationKeyFrames, segment);
                AnimationStoryboard.Children.Add(dblAnimationKeyFrames);

                dblAnimationKeyFrames = new DoubleAnimationUsingKeyFrames();
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0d
                };
                dblAnimationKeyFrames.KeyFrames.Add(keyFrame);
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                    Value = segment.ScatterWidth
                };
                keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;
                dblAnimationKeyFrames.KeyFrames.Add(keyFrame);
                Storyboard.SetTargetProperty(dblAnimationKeyFrames, "ScatterSegment3D.ScatterWidth");
                dblAnimationKeyFrames.EnableDependentAnimation = true;
                Storyboard.SetTarget(dblAnimationKeyFrames, segment);
                AnimationStoryboard.Children.Add(dblAnimationKeyFrames);

                if (this.AdornmentsInfo != null && AdornmentsInfo.ShowLabel)
                {
                    UIElement label = this.AdornmentsInfo.LabelPresenters[i];
                    label.Opacity = 0;

                    DoubleAnimation animation = new DoubleAnimation() { To = 1, From = 0, BeginTime = TimeSpan.FromSeconds(beginTime.TotalSeconds + (beginTime.Seconds * 90) / 100), Duration = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 50) / 100) };
                    Storyboard.SetTargetProperty(animation, "FrameworkElement.Opacity");
                    Storyboard.SetTarget(animation, label);
                    AnimationStoryboard.Children.Add(animation);
                }

                i++;
            }

            AnimationStoryboard.Begin();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new ScatterSeries3D()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                ScatterHeight = this.ScatterHeight,
                ScatterWidth = this.ScatterWidth,
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the series when size changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scatterSeries3D = d as ScatterSeries3D;
            if (scatterSeries3D.ActualArea != null)
                (scatterSeries3D.ActualArea as SfChart3D).ScheduleUpdate();
        }
        
        #endregion

        #endregion
    }
}
