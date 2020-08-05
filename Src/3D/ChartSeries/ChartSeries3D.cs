// <copyright file="ChartSeries3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Apply Template overriding access modifier changes in wpf platform")]

    /// <summary>
    /// Class implementation for ChartSeries3D
    /// </summary>
    public abstract partial class ChartSeries3D : ChartSeriesBase
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush", 
                typeof(Brush), 
                typeof(ChartSeries3D),
                new PropertyMetadata(null, OnSegmentSelectionBrush));
        
        /// <summary>
        /// The DependencyProperty for <see cref="AdornmentsInfo"/> property.
        /// </summary>
        public static readonly DependencyProperty AdornmentsInfoProperty =
          DependencyProperty.Register(
              "AdornmentsInfo", 
              typeof(ChartAdornmentInfo3D), 
              typeof(ChartSeries3D),
              new PropertyMetadata(null, OnAdornmentsInfoChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex", 
                typeof(int), 
                typeof(ChartSeries3D),
                new PropertyMetadata(-1, OnSelectedIndexChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SelectionMode"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode", 
                typeof(SelectionMode), 
                typeof(ChartSeries3D),
                new PropertyMetadata(SelectionMode.MouseClick));
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeries3D"/> class.
        /// </summary>
        public ChartSeries3D()
        {
            PrevSelectedIndex = -1;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the interior (brush) for the selected segment(s).
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        /// <example>
        ///     <code language="C#">
        ///         series.SegmentSelectionBrush = new SolidColorBrush(Colors.Red);
        ///     </code>
        /// </example>
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { this.SetValue(SegmentSelectionBrushProperty, value); }
        }
        
        /// <summary>
        /// <para>Gets or sets data labels for the series.</para> <para>This allows us to customize the appearance of a data point 
        /// by displaying labels, shapes and connector lines.</para>
        /// </summary>
        /// <value>
        /// The <see cref="ChartAdornmentInfo3D" /> value.
        /// </value>
        /// <example>
        ///     <code language="XAML">
        ///         &lt;syncfusion:ColumnSeries3D.AdornmentsInfo&gt;
        ///             &lt;syncfusion:ChartAdornmentInfo3D ShowMarker="True" Symbol="Ellipse"&gt;
        ///         &lt;/syncfusion:ColumnSeries3D.AdornmentsInfo&gt;
        ///     </code>
        ///     <code language="C#">
        ///         ChartAdornmentInfo3D chartAdornmentInfo3D = new ChartAdornmentInfo3D();
        ///         chartAdornmentInfo3D.ShowMarker = true;
        ///         chartAdornmentInfo3D.Symbol = ChartSymbol.Ellipse;
        ///         ColumnSeries3D columnSeries3D = new ColumnSeries3D();
        ///         columnSeries3D.AdornmentsInfo = chartAdornmentInfo3D;
        ///     </code>
        /// </example>
        public ChartAdornmentInfo3D AdornmentsInfo
        {
            get
            {
                return (ChartAdornmentInfo3D)GetValue(AdornmentsInfoProperty);
            }

            set
            {
                this.SetValue(AdornmentsInfoProperty, value);
            }
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
            set { this.SetValue(SelectedIndexProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the property which defines the way of selection.
        /// </summary>
        /// <value>
        ///     <c>SelectionMode.MouseClick</c> will select the segment(s) using mouse or pointer click.
        ///     <c>SelectionMode.MouseMove</c> will select the segment while hovering.
        /// </value>
        /// <remark>
        /// Note : With <see cref="SelectionStyle"/> as Multiple, MouseMove will not support.
        /// </remark>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { this.SetValue(SelectionModeProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the previous selected index.
        /// </summary>
        internal int PrevSelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Storyboard"/> for the series animation.
        /// </summary>
        internal Storyboard AnimationStoryboard { get; set; }

        /// <summary>
        /// Gets or sets the chart area <see cref="SfChart3D"/>.
        /// </summary>
        internal SfChart3D Area
        {
            get { return ActualArea as SfChart3D; }
            set { this.ActualArea = value; }
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
                    return (from index in SelectedSegmentsIndexes
                            where index < Segments.Count
                            select Segments[index]).ToList();
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
                if (this.SelectedIndex >= 0 && this.SelectedIndex < Segments.Count)
                {
                    return this.Segments[this.SelectedIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Method used to get selection brush for series selection.
        /// </summary>
        /// <param name="series">The Series</param>
        /// <returns>The series selection brush.</returns>
        public virtual Brush GetSeriesSelectionBrush(ChartSeriesBase series)
        {
            return series.SeriesSelectionBrush;
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Returns the value of side by side position for a series.
        /// </summary>
        /// <param name="currentseries">The ChartSeries.</param>
        /// <returns>The DoubleRange side by side Info</returns>
        internal DoubleRange GetZSideBySideInfo(ChartSeriesBase currentseries)
        {
            if (this.ActualArea != null)
            {
                if (this.ActualArea.InternalPrimaryAxis == null || this.ActualArea.InternalSecondaryAxis == null
                    || this.ActualArea.InternalDepthAxis == null)
                {
                    return DoubleRange.Empty;
                }

                if (!this.ActualArea.SBSInfoCalculated || !this.ActualArea.SeriesPosition.ContainsKey(currentseries))
                {
                    this.CalculateSideBySidePositions(false);
                }

                double width = 1 - ChartSeriesBase.GetSpacing(this);
                double minWidth = 0d;
                int all = 0;

                // MinPointsDelta is assigned to field since whenever the value is get the MinPointsDelta is calculated.
                double minPointsDelta = (this.ActualArea as SfChart3D).ZMinPointsDelta;
                if (!double.IsNaN(minPointsDelta))
                {
                    minWidth = minPointsDelta;
                }

                var xyzDataSeries = currentseries as XyzDataSeries3D;

                int rowPos = currentseries.IsActualTransposed
                    ? ActualArea.GetActualRow(xyzDataSeries.ActualZAxis)
                    : ActualArea.GetActualRow(xyzDataSeries.ActualYAxis);
                int columnPos = currentseries.IsActualTransposed
                    ? ActualArea.GetActualColumn(xyzDataSeries.ActualYAxis)
                    : ActualArea.GetActualColumn(xyzDataSeries.ActualZAxis);

                var rowID = currentseries.ActualYAxis == null ? 0 : rowPos;
                var colID = xyzDataSeries.ActualZAxis == null ? 0 : columnPos;
                if ((rowID < this.ActualArea.SbsSeriesCount.GetLength(0)) && (colID < this.ActualArea.SbsSeriesCount.GetLength(1)))
                {
                    all = this.ActualArea.SbsSeriesCount[rowID, colID];
                }
                else
                {
                    return DoubleRange.Empty;
                }

                if (!this.ActualArea.SeriesPosition.ContainsKey(currentseries))
                {
                    return DoubleRange.Empty;
                }

                int pos = this.ActualArea.SeriesPosition[currentseries];
                if (all == 0)
                {
                    all = 1;
                    pos = 1;
                }

                double div = minWidth * width / all;
                double start = div * (pos - 1) - minWidth * width / 2;
                double end = start + div;

                // For adding additional space on both ends of side by side info series.
                this.CalculateSideBySideInfoPadding(minWidth, all, pos, false);

                return new DoubleRange(start, end);
            }
            else
            {
                return DoubleRange.Empty;
            }
        }
        
        /// <summary>
        /// Updates the chart when the selected segment index collection changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Argument</param>
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
                            // Set the SegmentSelectionBrush to newIndex segment Interior
                            if (newIndex < Segments.Count && this.SegmentSelectionBrush != null)
                            {
                                ChartSegment3D segment = Segments[newIndex] as ChartSegment3D;
                                segment.BindProperties();

                                foreach (var item in segment.Polygons)
                                {
                                    item.Fill = this.SegmentSelectionBrush;
                                    item.ReDraw();
                                }
                            }

                            if (newIndex < Segments.Count)
                            {
                                chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                {
                                    SelectedSegment = this.SelectedSegment,
                                    SelectedSegments = this.Area.SelectedSegments,
                                    SelectedSeries = this,
                                    SelectedIndex = newIndex,
                                    PreviousSelectedIndex = oldIndex,
                                    NewPointInfo = Segments[newIndex].Item,
                                    PreviousSelectedSegment = null,
                                    PreviousSelectedSeries = null,
                                    IsSelected = true
                                };

                                chartSelectionChangedEventArgs.PreviousSelectedSeries = this.Area.PreviousSelectedSeries;

                                if (oldIndex != -1 && oldIndex < Segments.Count)
                                {
                                    chartSelectionChangedEventArgs.PreviousSelectedSegment = this.Segments[oldIndex];
                                    chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex].Item;
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
                            SelectedSegments = this.Area.SelectedSegments,
                            SelectedSeries = null,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = PreviousSelectedIndex,
                            PreviousSelectedSegment = null,
                            PreviousSelectedSeries = this,
                            IsSelected = false
                        };

                        if (this.PreviousSelectedIndex != -1 && this.PreviousSelectedIndex < Segments.Count)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = this.Segments[this.PreviousSelectedIndex];
                            chartSelectionChangedEventArgs.OldPointInfo = Segments[PreviousSelectedIndex].Item;
                        }

                        (ActualArea as SfChart3D).OnSelectionChanged(chartSelectionChangedEventArgs);
                        this.PreviousSelectedIndex = newIndex;
                    }

                    break;
            }
        }

        /// <summary>
        /// Updates the <see cref="ChartSeries3D"/> when the segment is reset.
        /// </summary>
        /// <param name="index">The Index</param>
        internal override void OnResetSegment(int index)
        {
            if (index < Segments.Count && index >= 0)
            {
                ChartSegment3D segment = Segments[index] as ChartSegment3D;

                segment.BindProperties();

                foreach (var item in segment.Polygons)
                {
                    item.Fill = segment.Interior;
                    item.ReDraw();
                }
            }
        }
        
        /// <summary>
        /// Method is used to raises the selection changing event.
        /// </summary>
        /// <param name="newIndex">The New Index</param>
        /// <param name="oldIndex">The Old Index</param>
        /// <returns>Returns a <see cref="bool"/> property to indicate the argument cancel.</returns>
        internal override bool RaiseSelectionChanging(int newIndex, int oldIndex)
        {
            selectionChangingEventArgs.SelectedSegments = null;

            if (this is AreaSeries3D || this is LineSeries3D)
            {
                selectionChangingEventArgs.SelectedSegment = this.GetDataPoint(newIndex);
            }
            else
            {
                if (newIndex >= 0 && newIndex < Segments.Count)
                {
                    selectionChangingEventArgs.SelectedSegment = this.Segments[newIndex];
                }
                else
                {
                    selectionChangingEventArgs.SelectedSegment = null;
                }
            }

            this.SetSelectionChangingEventArgs(newIndex, oldIndex);

            ActualArea.OnSelectionChanging(this.selectionChangingEventArgs);

            return selectionChangingEventArgs.Cancel;
        }

        /// <summary>
        /// Updates the series when mouse moved.
        /// </summary>
        /// <param name="element">The Element</param>
        /// <param name="pos">The Position</param>
        internal void OnMouseMoveSelection(FrameworkElement element, Point pos)
        {
            if (element == null || !(element.Tag is ChartSegment3D))
            {
                return;
            }

            var segment = element.Tag as ChartSegment3D;
            int currentIndex = -1;

            if (this.IsSideBySide || segment.Series is CircularSeriesBase3D)
            {
                currentIndex = Segments.IndexOf(segment);
            }
            else
            {
                this.dataPoint = this.GetDataPoint(pos);
                if (this.dataPoint != null)
                {
                    currentIndex = dataPoint.Index;
                }
            }

            bool seriesSelection = this.Area.EnableSeriesSelection
              && (segment.Series.IsSideBySide || segment.Series is ScatterSeries3D
              || segment.Series is CircularSeriesBase3D
              || (!segment.Series.IsSideBySide
              && ((!this.Area.EnableSegmentSelection)
              || (this.Area.EnableSegmentSelection && currentIndex == -1))));

            if (seriesSelection)
            {
                int index = this.Area.Series.IndexOf(segment.Series as ChartSeries3D);

                if (index > -1)
                {
                    bool isCancel = this.RaiseSelectionChanging(index, this.Area.SeriesSelectedIndex);

                    if (!isCancel)
                    {
                        this.Area.SeriesSelectedIndex = index;
                        this.Area.PreviousSelectedSeries = this;
                    }
                }
            }
            else if (this.Area.EnableSegmentSelection)
            {
                if (currentIndex > -1)
                {
                    bool isCancel = this.RaiseSelectionChanging(currentIndex, this.SelectedIndex);

                    if (!isCancel)
                    {
                        if (this.Area.SelectionStyle == SelectionStyle3D.Single)
                        {
                            this.SelectedIndex = currentIndex;
                        }
                        else
                        {
                            SelectedSegmentsIndexes.Add(currentIndex);
                        }

                        this.Area.PreviousSelectedSeries = this;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="ChartSeries3D"/> on mouse down selection.
        /// </summary>
        /// <param name="currentIndex">The Current Selected Index</param>
        internal void OnMouseDownSelection(int currentIndex)
        {           
            bool seriesSelection = this.Area.EnableSeriesSelection
              && (IsSideBySide || this is ScatterSeries3D
              || this is CircularSeriesBase3D
              || (!this.IsSideBySide
              && ((!this.Area.EnableSegmentSelection)
              || (this.Area.EnableSegmentSelection && currentIndex == -1))));

            if (seriesSelection)
            {
                int index = this.Area.Series.IndexOf(this as ChartSeries3D);
                selectionChangingEventArgs.IsDataPointSelection = false;
                bool isCancel = this.RaiseSelectionChanging(index, this.Area.SeriesSelectedIndex);

                if (!isCancel)
                {
                    if (this.Area.SelectionStyle != SelectionStyle3D.Single && this.Area.SelectedSeriesCollection.Contains(this))
                    {
                        this.Area.SelectedSeriesCollection.Remove(this);

                        if (this.Area.SeriesSelectedIndex == index)
                        {
                            this.Area.SeriesSelectedIndex = -1;
                        }

                        SfChart3D.OnResetSeries(this);
                    }
                    else if (this.Area.SeriesSelectedIndex == index)
                    {
                        this.Area.SeriesSelectedIndex = -1;
                    }
                    else if (index > -1)
                    {
                        this.Area.SeriesSelectedIndex = index;
                        this.Area.PreviousSelectedSeries = this;
                    }
                }
            }
            else if (this.Area.EnableSegmentSelection)
            {
                selectionChangingEventArgs.IsDataPointSelection = true;
                bool isCancel = this.RaiseSelectionChanging(currentIndex, this.SelectedIndex);

                if (!isCancel)
                {
                    if (this.Area.SelectionStyle != SelectionStyle3D.Single && SelectedSegmentsIndexes.Contains(currentIndex))
                    {
                        SelectedSegmentsIndexes.Remove(currentIndex);

                        if (this.SelectedIndex == currentIndex)
                        {
                            this.SelectedIndex = -1;
                        }

                        this.OnResetSegment(currentIndex);
                    }
                    else if (this.SelectedIndex == currentIndex)
                    {
                        this.SelectedIndex = -1;
                    }
                    else if (currentIndex > -1)
                    {
                        this.SelectedIndex = currentIndex;
                        this.Area.PreviousSelectedSeries = this;
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates the on series bound changed.
        /// </summary>
        /// <param name="size">The size.</param>
        internal override void UpdateOnSeriesBoundChanged(Size size)
        {
            if (this.AdornmentsInfo != null)
            {
                this.AdornmentsInfo.UpdateElements();
                this.AdornmentsInfo.Measure(size, null);
            }

            var isupportAxes = this as ISupportAxes;
            var canUpdate = isupportAxes == null || isupportAxes != null && ActualXAxis != null && ActualYAxis != null;

            if (!canUpdate)
            {
                return;
            }

            var chartTransformer = CreateTransformer(size, true);
            foreach (var segment in this.Segments)
            {
                segment.CreateSegmentVisual(size);
                segment.Update(chartTransformer);
            }
        }
        
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        internal override void CalculateSegments()
        {
            base.CalculateSegments();
            if (this.DataCount == 0)
            {
                if (this.AdornmentsInfo != null)
                {
                    this.ClearUnUsedAdornments(this.DataCount);
                }
            }
        }

        #endregion

        #region Protected Internal Methods
        
        /// <summary>
        /// Method used to set SegmentSelectionBrush to <see cref="SelectedIndex"/> <see cref="ChartSegment3D"/> 
        /// and trigger chart selection event.
        /// </summary>
        /// <param name="newIndex">The New Index</param>
        /// <param name="oldIndex">The Old Index</param>
        protected internal override void SelectedIndexChanged(int newIndex, int oldIndex)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            if (this.ActualArea != null)
            {
                ChartSegment3D segment;

                // Reset the oldIndex segment Interior
                if (this.Area.SelectionStyle == SelectionStyle3D.Single)
                {
                    if (SelectedSegmentsIndexes.Contains(oldIndex))
                    {
                        SelectedSegmentsIndexes.Remove(oldIndex);
                    }

                    this.OnResetSegment(oldIndex);
                }

                if (newIndex >= 0 && (ActualArea as SfChart3D).EnableSegmentSelection)
                {
                    if (!SelectedSegmentsIndexes.Contains(newIndex))
                    {
                        SelectedSegmentsIndexes.Add(newIndex);
                    }

                    // Set the SegmentSelectionBrush to newIndex segment Interior
                    if (newIndex < Segments.Count && this.SegmentSelectionBrush != null)
                    {
                        segment = Segments[newIndex] as ChartSegment3D;
                        segment.BindProperties();

                        foreach (var item in segment.Polygons)
                        {
                            item.Fill = this.SegmentSelectionBrush;
                            item.ReDraw();
                        }
                    }

                    if (newIndex < Segments.Count)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = this.SelectedSegment,
                            SelectedSegments = this.Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            NewPointInfo = Segments[newIndex].Item,
                            PreviousSelectedSegment = null,
                            PreviousSelectedSeries = null,
                            IsSelected = true
                        };

                        chartSelectionChangedEventArgs.PreviousSelectedSeries = this.Area.PreviousSelectedSeries;

                        if (oldIndex >= 0 && oldIndex < Segments.Count)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = this.Segments[oldIndex];
                            chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex].Item;
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
                        SelectedSegments = this.Area.SelectedSegments,
                        SelectedSeries = null,
                        SelectedIndex = newIndex,
                        PreviousSelectedIndex = oldIndex,
                        PreviousSelectedSegment = null,
                        PreviousSelectedSeries = this,
                        IsSelected = false
                    };

                    if (oldIndex != -1 && oldIndex < Segments.Count)
                    {
                        chartSelectionChangedEventArgs.PreviousSelectedSegment = this.Segments[oldIndex];
                        chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex].Item;
                    }

                    (ActualArea as SfChart3D).OnSelectionChanged(chartSelectionChangedEventArgs);
                    this.PreviousSelectedIndex = newIndex;
                }
            }
        }
        
        /// <summary>
        /// Called when [series mouse down].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="position">The position.</param>
        protected internal override void OnSeriesMouseDown(object source, Point position)
        {
        }
        
        /// <summary>
        /// Called when [series mouse up].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="position">The position.</param>
        protected internal override void OnSeriesMouseUp(object source, Point position)
        {
            FrameworkElement element = source as FrameworkElement;
            if (SelectionMode == SelectionMode.MouseClick)
            {
                if (element == null || !(element.Tag is ChartSegment3D))
                {
                    return;
                }

                var segment = element.Tag as ChartSegment3D;
                int currentIndex = -1;

                if (this.IsSideBySide || segment.Series is CircularSeriesBase3D)
                {
                    currentIndex = Segments.IndexOf(segment);
                }
                else
                {
                    this.dataPoint = this.GetDataPoint(position);
                    if (this.dataPoint != null)
                    {
                        currentIndex = dataPoint.Index;
                    }
                }

                this.OnMouseDownSelection(currentIndex);
            }
        }

        /// <summary>
        /// Called when [series mouse move].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal virtual void OnSeriesMouseMove(object source, Point pos)
        {
            if (SelectionMode == SelectionMode.MouseMove && this.Area.SelectionStyle == SelectionStyle3D.Single)
            {
                this.OnMouseMoveSelection(source as FrameworkElement, pos);
            }

            this.mousePos = pos;
            this.UpdateTooltip(source);
        }
        
        /// <summary>
        /// Called when [series mouse leave].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal virtual void OnSeriesMouseLeave(object source, Point pos)
        {
            this.RemoveTooltip();
            Timer.Stop();
            if (ChartTooltip.GetInitialShowDelay(this) > 0)
            {
                InitialDelayTimer.Stop();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Invoke to render 3D series
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        
        /// <summary>
        /// Method implementation for Create Adornments
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="xVal">The x value.</param>
        /// <param name="yVal">The y value.</param>
        /// <param name="xPos">The x position.</param>
        /// <param name="yPos">The y position.</param>
        /// <param name="startDepth">The start depth.</param>
        /// <returns>Returns the created adornment.</returns>
        protected virtual ChartAdornment CreateAdornment(ChartSeriesBase series, double xVal, double yVal, double xPos, double yPos, double startDepth)
        {
            return new ChartAdornment3D(xVal, yVal, xPos, yPos, startDepth, series);
        }
        
        /// <summary>
        /// Adds the adornment to the adornments collection.
        /// </summary>
        /// <param name="xValue">The X Value</param>
        /// <param name="yValue">The Y Value</param>
        /// <param name="index">The Index</param>
        /// <param name="depth">The Depth</param>
        protected virtual void AddAdornments(double xValue, double yValue, int index, double depth)
        {
            double adornX = 0d, adornY = 0d;
            adornX = xValue;
            adornY = yValue;
            if (index < Adornments.Count)
            {
                Adornments[index].SetData(adornX, adornY, adornX, adornY, depth);
            }
            else
            {
                Adornments.Add(this.CreateAdornment(this, adornX, adornY, adornX, adornY, depth));
            }

            Adornments[index].Item = this.ActualData[index];
        }
        
        /// <summary>
        /// Method implementation for Add ColumnAdornments in Chart.
        /// </summary>
        /// <param name="values">The Value</param>
        protected virtual void AddColumnAdornments(params double[] values)
        {
            // values[0] -->   xData
            // values[1] -->   yData
            // values[2] -->   xPos
            // values[3] -->   yPos
            // values[4] -->   data point index
            // values[5] -->   Median value.
            double adornposX = values[2] + values[5], adornposY = values[3];
            var pointIndex = (int)values[4];

            if (pointIndex < Adornments.Count)
            {
                Adornments[pointIndex].SetData(values[0], values[1], adornposX, adornposY, values[6]);
            }
            else
            {
                Adornments.Add(this.CreateAdornment(this, values[0], values[1], adornposX, adornposY, values[6]));
            }

            if (ActualXAxis is CategoryAxis3D && !(ActualXAxis as CategoryAxis3D).IsIndexed)
            {
                Adornments[pointIndex].Item = this.GroupedActualData[pointIndex];
            }
            else
            {
                Adornments[pointIndex].Item = this.ActualData[pointIndex];
            }
        }

        /// <summary>
        /// Method implementation for Add Adornments at XY
        /// </summary>
        /// <param name="x">The X Value</param>
        /// <param name="y">The Y Value</param>
        /// <param name="pointindex">The Point Index</param>
        /// <param name="startDepth">The Start Depth</param>
        protected virtual void AddAdornmentAtXY(double x, double y, int pointindex, double startDepth)
        {
            double adornposX = x, adornposY = y;

            if (pointindex < Adornments.Count)
            {
                Adornments[pointindex].SetData(x, y, adornposX, adornposY);
            }
            else
            {
                Adornments.Add(this.CreateAdornment(this, x, y, adornposX, adornposY, startDepth));
            }
        }

        /// <summary>
        /// Called when DataSource property changed 
        /// </summary>
        /// <param name="oldValue">The Old Value</param>
        /// <param name="newValue">The New Value</param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (this.AdornmentsInfo != null)
            {
                Adornments.Clear();
                this.AdornmentsInfo.UpdateElements();
            }

            base.OnDataSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Method implementation for Clear Unused Adornments
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        protected void ClearUnUsedAdornments(int startIndex)
        {
            if (Adornments.Count <= startIndex)
            {
                return;
            }

            var count = Adornments.Count;

            for (var i = startIndex; i < count; i++)
            {
                Adornments.RemoveAt(startIndex);
            }
        }

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A DependencyObject.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var chartSeries3D = (ChartSeries3D)obj;

            if (this.AdornmentsInfo != null)
            {
                chartSeries3D.AdornmentsInfo = (ChartAdornmentInfo3D)this.AdornmentsInfo.Clone();
            }

            chartSeries3D.SegmentSelectionBrush = this.SegmentSelectionBrush;
            chartSeries3D.SelectedIndex = this.SelectedIndex;
            return base.CloneSeries(obj);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the segment selection color for the series.
        /// </summary>
        /// <param name="d">The Dependency Property</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((ChartSeries3D)d).UpdateArea();
        }
        
        /// <summary>
        /// Updates the series adornments when the <see cref="AdornmentsInfo"/> changed.
        /// </summary>
        /// <param name="d">The Dependency Property</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnAdornmentsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as ChartSeries3D;

            if (e.OldValue != null)
            {
                var adornmentInfo = e.OldValue as ChartAdornmentInfoBase;
                if (series != null)
                {
                    series.Adornments.Clear();
                }

                if (adornmentInfo != null)
                {
                    adornmentInfo.ClearChildren();
                    adornmentInfo.Series = null;
                }
            }

            if (e.NewValue == null)
            {
                return;
            }

            if (series == null)
            {
                return;
            }

            series.adornmentInfo = e.NewValue as ChartAdornmentInfoBase;
            if (series.AdornmentsInfo == null)
            {
                return;
            }

            series.AdornmentsInfo.Series = series;
            series.AdornmentsInfo.PanelChanged(null);
            if (series.Area != null)
            {
                series.Area.ScheduleUpdate();
            }
        }
        
        /// <summary>
        /// Updates the series when selected index changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries3D series = d as ChartSeries3D;
            if (series.ActualArea == null)
            {
                return;
            }

            if ((series.ActualArea as SfChart3D).SelectionStyle == SelectionStyle3D.Single)
            {
                series.SelectedIndexChanged((int)e.NewValue, (int)e.OldValue);
            }
            else if ((int)e.NewValue != -1)
            {
                series.SelectedSegmentsIndexes.Add((int)e.NewValue);
            }
        }
        
        #endregion

        #endregion
    }
}
