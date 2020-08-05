// <copyright file="Annotation.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Windows.Foundation;
    using Windows.UI.Text;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Provides a lightweight control for displaying overlay element in <see cref="SfChart"/>.
    /// </summary>
    /// <seealso cref="FrameworkElement" />
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.ICloneable" />
    public abstract partial class Annotation : FrameworkElement, ICloneable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Text"/> property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(Annotation),
                new PropertyMetadata(string.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textAnnotation = d as Annotation;
            if (textAnnotation != null && textAnnotation.TextElement != null)
                textAnnotation.TextElement.Content = (string)e.NewValue;
        }

        /// <summary>
        /// The DependencyProperty for <see cref="EnableEditing"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableEditingProperty =
            DependencyProperty.Register(
                "EnableEditing",
                typeof(bool),
                typeof(Annotation),
                new PropertyMetadata(false, OnEnableEditingChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ContentTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(Annotation),
                new PropertyMetadata(null, OnContentTemplateChanged));

        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textAnnotation = d as Annotation;
            if (textAnnotation != null && textAnnotation.TextElement != null)
                textAnnotation.TextElement.ContentTemplate = (DataTemplate)e.NewValue;
        }

        /// <summary>
        /// The DependencyProperty for <see cref="EnableClipping"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableClippingProperty =
            DependencyProperty.Register(
                "EnableClipping",
                typeof(bool),
                typeof(Annotation),
                new PropertyMetadata(false, OnEnableClippingPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ShowTooltip"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowToolTipProperty =
            DependencyProperty.Register(
                "ShowToolTip",
                typeof(bool),
                typeof(Annotation),
                new PropertyMetadata(false));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolTipContent"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolTipContentProperty =
            DependencyProperty.Register(
                "ToolTipContent",
                typeof(object),
                typeof(Annotation),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolTipShowDuration"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolTipShowDurationProperty =
            DependencyProperty.Register(
                "ToolTipShowDuration",
                typeof(double),
                typeof(Annotation),
                new PropertyMetadata(double.NaN));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolTipTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolTipTemplateProperty =
            DependencyProperty.Register(
                "ToolTipTemplate",
                typeof(DataTemplate),
                typeof(Annotation),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="ToolTipPlacement"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolTipPlacementProperty =
            DependencyProperty.Register(
                "ToolTipPlacement",
                typeof(ToolTipLabelPlacement),
                typeof(Annotation),
                new PropertyMetadata(ToolTipLabelPlacement.Right));

        /// <summary>
        /// The DependencyProperty for <see cref="CoordinateUnit"/> property.
        /// </summary>
        public static readonly DependencyProperty CoordinateUnitProperty =
            DependencyProperty.Register(
                "CoordinateUnit",
                typeof(CoordinateUnit),
                typeof(Annotation),
                new PropertyMetadata(CoordinateUnit.Axis, OnCoordinatePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="XAxisName"/> property.
        /// </summary>
        public static readonly DependencyProperty XAxisNameProperty =
            DependencyProperty.Register(
                "XAxisName",
                typeof(string),
                typeof(Annotation),
                new PropertyMetadata(string.Empty, OnAxisNameChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="XAxisName"/> property.
        /// </summary>
        public static readonly DependencyProperty YAxisNameProperty =
            DependencyProperty.Register(
                "YAxisName",
                typeof(string),
                typeof(Annotation),
                new PropertyMetadata(string.Empty, OnAxisNameChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="X1"/> property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(
                "X1",
                typeof(object),
                typeof(Annotation),
                new PropertyMetadata(null, OnUpdatePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Y1"/> property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(
                "Y1",
                typeof(object),
                typeof(Annotation),
                new PropertyMetadata(null, OnUpdatePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="FontSize"/> property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                "FontSize",
                typeof(double),
                typeof(Annotation),
                new PropertyMetadata(TextBlock.FontSizeProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// The DependencyProperty for <see cref="FontFamily"/> property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                "FontFamily",
                typeof(FontFamily),
                typeof(Annotation),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null));
#else
                new PropertyMetadata(TextBlock.FontFamilyProperty.GetMetadata(typeof(TextBlock)).DefaultValue));
#endif

        /// <summary>
        /// The DependencyProperty for <see cref="FontStretch"/> property.
        /// </summary>
        public static readonly DependencyProperty FontStretchProperty =
            DependencyProperty.Register(
                "FontStretch",
                typeof(FontStretch),
                typeof(Annotation),
                new PropertyMetadata(TextBlock.FontStretchProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// The DependencyProperty for <see cref="FontStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register(
                "FontStyle",
                typeof(FontStyle),
                typeof(Annotation),
                new PropertyMetadata(TextBlock.FontStyleProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// The DependencyProperty for <see cref="FontWeight"/> property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(
                "FontWeight",
                typeof(FontWeight),
                typeof(Annotation),
                new PropertyMetadata(TextBlock.FontWeightProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// The DependencyProperty for <see cref="Foreground"/> property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground",
                typeof(Brush),
                typeof(Annotation),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null));
#else
                new PropertyMetadata(TextBlock.ForegroundProperty.GetMetadata(typeof(TextBlock)).DefaultValue));
#endif

        /// <summary>
        /// The DependencyProperty for <see cref="InternalHorizontalAlignment"/> property.
        /// </summary>
        internal static readonly DependencyProperty InternalHorizontalAlignmentProperty =
            DependencyProperty.Register(
                "InternalHorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(Annotation),
                new PropertyMetadata(HorizontalAlignment.Right, OnUpdatePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="InternalVerticalAlignment"/> property.
        /// </summary>
        internal static readonly DependencyProperty InternalVerticalAlignmentProperty =
            DependencyProperty.Register(
                "InternalVerticalAlignment",
                typeof(VerticalAlignment),
                typeof(Annotation),
                new PropertyMetadata(VerticalAlignment.Bottom, OnUpdatePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="InternalVisibility"/> property.
        /// </summary>
        internal static readonly DependencyProperty InternalVisibilityProperty =
            DependencyProperty.Register(
                "InternalVisibility",
                typeof(Visibility),
                typeof(Annotation),
                new PropertyMetadata(Visibility.Visible, OnVisibiltyChanged));

        #endregion

        #region Fields

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "This is a backing store for the property named Chart")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a backing store for the property named Chart")]
        internal SfChart chart;

        private Matrix transformation;

        private Rect transformedDesiredElement;

        private DataTemplate contentTemplate = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation"/> class.
        /// </summary>
        public Annotation()
        {
            this.AnnotationElement = new Grid();
            this.TextElementCanvas = new Canvas();
            this.TextElement = new ContentControl();

            TextElement.PointerPressed += TextElement_PointerPressed;
            FontFamily = TextBlock.FontFamilyProperty.GetMetadata(typeof(TextBlock)).DefaultValue as FontFamily;
            Foreground = TextBlock.ForegroundProperty.GetMetadata(typeof(TextBlock)).DefaultValue as Brush;
        }

        #endregion

        #region Events

        #region Public Events

        /// <summary>
        /// Occurs when the pointer device initiates a Press action within this element.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
          Justification = "Used defaut PointerEventHandler")]
        public new event PointerEventHandler PointerPressed
        {
            add
            {
                this.AnnotationElement.PointerPressed += value;
            }

            remove
            {
                this.AnnotationElement.PointerPressed -= value;
            }
        }

        /// <summary>
        ///  Occurs when the pointer device that previously initiated a Press action is released, while within this element.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
         Justification = "Used defaut PointerEventHandler")]
        public new event PointerEventHandler PointerReleased
        {
            add
            {
                this.AnnotationElement.PointerReleased += value;
            }

            remove
            {
                this.AnnotationElement.PointerReleased -= value;
            }
        }

        /// <summary>
        /// Occurs when a pointer moves while the pointer remains within the hit test area of this element.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
         Justification = "Used defaut PointerEventHandler")]
        public new event PointerEventHandler PointerMoved
        {
            add
            {
                this.AnnotationElement.PointerMoved += value;
            }

            remove
            {
                this.AnnotationElement.PointerMoved -= value;
            }
        }

        /// <summary>
        /// Occurs when <see cref="Syncfusion.UI.Xaml.Charts.Annotation"/> becomes selected.
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        /// Occurs when <see cref="Syncfusion.UI.Xaml.Charts.Annotation"/> becomes unselected.
        /// </summary>
        public event EventHandler UnSelected;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the description text for Annotation.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text in <c>TextAnnotation</c> can be edited or not.
        /// </summary>
        /// <value>
        ///     <c>true</c> enable the editing option in TextAnnotation.
        /// </value>
        public bool EnableEditing
        {
            get { return (bool)GetValue(EnableEditingProperty); }
            set { SetValue(EnableEditingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for <see cref="Text"/>.
        /// </summary>
        /// <value>
        /// This accepts a DataTemplate.
        /// <see cref="DataTemplate"/>
        /// </value>
        /// <example>
        ///     <code language="XAML">
        ///         &lt;syncfusion:SfChart.Annotations&gt;
        ///             &lt;syncfusion:Annotation ContentTemplate="{StaticResource contentTemplate}"&gt;
        ///         &lt;/syncfusion:SfChart.Annotations&gt;
        ///     </code>
        ///     <code language="C#">
        ///         annotation.ContentTemplate = dataTemplate;
        ///     </code>
        /// </example>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether annotation should clip while crossing with boundary.
        /// </summary>
        public bool EnableClipping
        {
            get { return (bool)GetValue(EnableClippingProperty); }
            set { SetValue(EnableClippingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <c>ToolTip</c> can be displayed in Annotation.
        /// </summary>
        /// <value>
        /// The <c>true</c> enables the ToolTip for the annotation.
        /// </value>
        public bool ShowToolTip
        {
            get { return (bool)GetValue(ShowToolTipProperty); }
            set { SetValue(ShowToolTipProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content to be displayed in annotation tooltip.
        /// </summary>
        /// <value>
        /// This accepts all arbitrary .net objects.
        /// </value>
        public object ToolTipContent
        {
            get { return (object)GetValue(ToolTipContentProperty); }
            set { SetValue(ToolTipContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the elapse time for the ToolTip.
        /// </summary>
        /// <value>
        /// It accepts the <c>double</c> value.
        /// </value>
        public double ToolTipShowDuration
        {
            get { return (double)GetValue(ToolTipShowDurationProperty); }
            set { SetValue(ToolTipShowDurationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the ToolTip.
        /// </summary>
        /// <value>
        /// This accepts the DataTemplate.
        /// <see cref="DataTemplate"/>
        /// </value>
        /// <example>
        ///     <code language="XAML">
        ///         &lt;syncfusion:SfChart.Annotations&gt;
        ///             &lt;syncfusion:Annotation ShowToolTip="true" ToolTipTemplate="{StaticResource toolTipTemplate}"&gt;
        ///         &lt;/syncfusion:SfChart.Annotations&gt;
        ///     </code>
        ///     <code language="C#">
        ///         annotation.ShowToolTip = true; 
        ///         annotation.ToolTipTemplate = dataTemplate;
        ///     </code>
        /// </example>
        public DataTemplate ToolTipTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
            set { SetValue(ToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the position of the ToolTip.
        /// </summary>
        /// <value>
        ///     <code>ToolTipLabelPlacement.Left</code> will place the ToolTip left.
        ///     <code>ToolTipLabelPlacement.Right</code> will place the ToolTip right.
        ///     <code>ToolTipLabelPlacement.Top</code> will place the ToolTip top.
        ///     <code>ToolTipLabelPlacement.Bottom</code> will place the ToolTip bottom.
        /// </value>
        public ToolTipLabelPlacement ToolTipPlacement
        {
            get { return (ToolTipLabelPlacement)GetValue(ToolTipPlacementProperty); }
            set { SetValue(ToolTipPlacementProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property which identifies whether the annotation positioned w.r.t pixel or axis coordinate. 
        /// </summary>
        /// <value>
        ///     <code>CoordinateUnit.Pixel</code> - position the annotation based on screen coordinates.
        ///     <code>CoordinateUnit.Axis</code> - position the annotation based on axis values.
        /// </value>
        public CoordinateUnit CoordinateUnit
        {
            get { return (CoordinateUnit)GetValue(CoordinateUnitProperty); }
            set { SetValue(CoordinateUnitProperty, value); }
        }

        /// <summary>
        /// Gets or sets the axis(horizontal) in which this annotation associated.
        /// </summary>
        /// <remarks>
        /// This property works only with <see cref="CoordinateUnit"/> as <c>CoordinateUnit.Axis</c>.
        /// </remarks>
        public string XAxisName
        {
            get { return (string)GetValue(XAxisNameProperty); }
            set { SetValue(XAxisNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the axis(vertical) in which this annotation associated.
        /// </summary>
        /// <remarks>
        /// This property works only with <see cref="CoordinateUnit"/> as <c>CoordinateUnit.Axis</c>.
        /// </remarks>
        public string YAxisName
        {
            get { return (string)GetValue(YAxisNameProperty); }
            set { SetValue(YAxisNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the annotation X1 position.
        /// </summary>
        public object X1
        {
            get { return (object)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        /// <summary>
        /// Gets or sets the annotation Y1 position.
        /// </summary>
        public object Y1
        {
            get { return (object)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        /// <summary>
        /// Gets or sets the font size of the annotation description.
        /// </summary>
        /// <value>
        /// It accepts the double value.
        /// </value>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font family for the annotation description.
        /// </summary>
        /// <value>
        /// This accepts all the <see cref="Windows.UI.Xaml.Media.FontFamily"/> .
        /// </value>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font stretch for the annotation description.
        /// </summary>
        /// <value>
        /// This property of type <see cref="Windows.UI.Text.FontStretch"/>.
        /// </value>
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font style for the annotation description.
        /// </summary>
        /// <value>
        /// This property of type <see cref="Windows.UI.Text.FontStyle"/>.
        /// </value>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font weight for the annotation description.
        /// </summary>
        /// <value>
        /// This property of type <see cref="Windows.UI.Text.FontWeight"/> property.
        /// </value>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground for the annotation description.
        /// </summary>
        /// <value>
        /// The <see cref="Brush"/> value.
        /// </value>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the chart for the annotation.
        /// </summary>
        internal virtual SfChart Chart
        {
            get
            {
                return chart;
            }

            set
            {
                chart = value;
                SetAxisFromName();
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment internally.
        /// </summary>
        internal HorizontalAlignment InternalHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(InternalHorizontalAlignmentProperty); }
            set { SetValue(InternalHorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical alignment internally.
        /// </summary>
        internal VerticalAlignment InternalVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(InternalVerticalAlignmentProperty); }
            set { SetValue(InternalVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the annotation visibility internally.
        /// </summary>
        internal Visibility InternalVisibility
        {
            get { return (Visibility)GetValue(InternalVisibilityProperty); }
            set { SetValue(InternalVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rotated <see cref="Rect"/> of the annotation.
        /// </summary>
        internal Rect RotatedRect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the annotation is selected.
        /// </summary>
        internal bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the annotation is resizing.
        /// </summary>
        internal bool IsResizing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the visibility is changed for the annotation.
        /// </summary>
        internal bool IsVisbilityChanged { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="XAxis"/> of the annotation.
        /// </summary>
        internal ChartAxis XAxis { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="YAxis"/> of the annotation.
        /// </summary>
        internal ChartAxis YAxis { get; set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the x 1 value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "The name X1 is used other classes.")]
        protected double x1 { get; set; }

        /// <summary>
        /// Gets or sets the y 1 value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "The name Y1 is used other classes.")]
        protected double y1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the UI is cleared.
        /// </summary>
        protected bool IsUiCleared { get; set; }

        /// <summary>
        /// Gets or sets the annotation grid.
        /// </summary>
        protected Grid AnnotationElement { get; set; }

        /// <summary>
        /// Gets or sets the text element canvas.
        /// </summary>
        protected Canvas TextElementCanvas { get; set; }

        /// <summary>
        /// Gets or sets the text element.
        /// </summary>
        protected ContentControl TextElement { get; set; }

        /// <summary>
        /// Gets or sets the rotated <see cref="Rect"/> of the annotations.
        /// </summary>
        protected Rect RotatedTextRect { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods
        
        /// <summary>
        /// Returns the clone annotation
        /// </summary>
        /// <returns>Returns the cloned annotation.</returns>
        public DependencyObject Clone()
        {
            return CloneAnnotation(null);
        }

        /// <summary>
        /// Gets the rendered annotation.
        /// </summary>
        /// <returns> A UI Element</returns>
        public virtual UIElement GetRenderedAnnotation()
        {
            return AnnotationElement;
        }

        /// <summary>
        /// Updates the annotation.
        /// </summary>
        public virtual void UpdateAnnotation()
        {
            SetData();
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Updates the annotation on text alignment changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="args">The dependency property changed event arguments.</param>
        internal static void OnTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as Annotation).UpdateAnnotation();
        }

        /// <summary>
        /// Converts the data to the required format.
        /// </summary>
        /// <param name="data">The data passed for conversion.</param>
        /// <param name="axis">The relevant axis for the conversion.</param>
        /// <returns>Returns the converted data.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static double ConvertData(object data, ChartAxis axis)
        {
            if (axis is NumericalAxis)
                return Convert.ToDouble(data);
            if (axis is DateTimeAxis)
            {
                if (data is DateTime)
                    return ((DateTime)data).ToOADate();
                else if (data is string)
                    return (Convert.ToDateTime(data.ToString())).ToOADate();
                else
                    return Convert.ToDouble(data);
            }

            if (axis is TimeSpanAxis)
            {
                if (data is TimeSpan)
                    return ((TimeSpan)data).TotalMilliseconds;
                else if (data is string)
                    return (TimeSpan.Parse(data.ToString())).TotalMilliseconds;
                else
                    return Convert.ToDouble(data);
            }

            var logarithmicAxis = axis as LogarithmicAxis;
            if (logarithmicAxis != null)
            {
                data = Convert.ToDouble(data) > 0 ? data : 1;
                return Math.Log(Convert.ToDouble(data), logarithmicAxis.LogarithmicBase);
            }

            return Convert.ToDouble(data);
        }

        /// <summary>
        /// Converts to required value from the passing data.
        /// </summary>
        /// <param name="data">The data to be converted.</param>
        /// <param name="axis">The relevant axis for the conversion.</param>
        /// <returns>Returns the converted data.</returns>
        internal static object ConvertToObject(double data, ChartAxis axis)
        {
            if (axis is DateTimeAxis)
                return (data).FromOADate();
            if (axis is TimeSpanAxis)
                return TimeSpan.FromMilliseconds(data);

            var logarithmicAxis = axis as LogarithmicAxis;
            if (logarithmicAxis != null)
                return Math.Pow(logarithmicAxis.LogarithmicBase, data);
            return Convert.ToDouble(data);
        }

        #endregion

        #region Internal Methods

        internal void Dispose()
        {
            var annotationResizer = this as AnnotationResizer;

            if (annotationResizer != null &&
                annotationResizer.ResizerControl != null)
            {
                annotationResizer.ResizerControl.Dispose();
                annotationResizer.ResizerControl = null;
            }
            Chart = null;
            XAxis = null;
            YAxis = null;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Updates the annotation on visibility property changed.
        /// </summary>
        internal virtual void OnVisibilityChanged()
        {
            if (this.chart != null && this.chart.AnnotationManager != null && this.chart.Annotations != null && this.chart.Annotations.Contains(this))
            {
                var shapeAnnotation = this as ShapeAnnotation;
                bool isResizer = (shapeAnnotation != null && chart.AnnotationManager.SelectedAnnotation == this
                    && shapeAnnotation.CanResize);
                IsVisbilityChanged = true;
 
                if (Visibility.Equals(Visibility.Collapsed))
                {
                    this.chart.AnnotationManager.AddOrRemoveAnnotations(this, true);
                    if (isResizer && this.chart.AnnotationManager.AnnotationResizer != null)
                    {
                        this.chart.AnnotationManager.AnnotationResizer.IsVisbilityChanged = true;
                        this.chart.AnnotationManager.AddOrRemoveAnnotationResizer(this.chart.AnnotationManager.AnnotationResizer, true);
                    }
                }
                else
                {
                    this.chart.AnnotationManager.AddOrRemoveAnnotations(this, false);
                    if (isResizer && this.chart.AnnotationManager.AnnotationResizer != null)
                    {
                        this.chart.AnnotationManager.AnnotationResizer.IsVisbilityChanged = true;
                        this.chart.AnnotationManager.AddOrRemoveAnnotationResizer(this.chart.AnnotationManager.AnnotationResizer, false);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the chart when annotation property changed.
        /// </summary>
        /// <param name="args">The dependency property changed event arguments.</param>
        internal void UpdatePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Chart != null && Chart.AnnotationManager != null && Chart.AnnotationManager.SelectedAnnotation != null &&
                 this is AnnotationResizer && args.OldValue != null)
            {
                Chart.AnnotationManager.SelectedAnnotation.Y1 = this.Y1;
                Chart.AnnotationManager.SelectedAnnotation.X1 = this.X1;
            }

            UpdateAnnotation();
            if (Chart != null && this.CoordinateUnit == CoordinateUnit.Axis && CanUpdateRange(this.X1, this.Y1))
                Chart.ScheduleUpdate();
        }

        /// <summary>
        /// Creates the annotation.
        /// </summary>
        /// <returns>Returns the annotation <see cref="UIElement"/></returns>
        internal virtual UIElement CreateAnnotation()
        {
            return AnnotationElement;
        }

        /// <summary>
        /// Sets the axis from the name.
        /// </summary>
        internal void SetAxisFromName()
        {
            if (Chart != null)
            {
                this.XAxis = Chart.Axes[this.XAxisName] ?? Chart.InternalPrimaryAxis;
                this.YAxis = Chart.Axes[this.YAxisName] ?? Chart.InternalSecondaryAxis;
            }
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Invoked when an unhandled Selected event reaches an element in its route that is derived from this class. Implement
        /// this method to add class handling for this event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected internal virtual void OnSelected(EventArgs args)
        {
            if (Selected != null)
                Selected(this, args);
        }

        /// <summary>
        /// Invoked when an unhandled UnSelected event reaches an element in its route that is derived from this class. Implement
        /// this method to add class handling for this event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected internal virtual void OnUnSelected(EventArgs args)
        {
            if (UnSelected != null)
                UnSelected(this, args);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Checks for the update requirement.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>Returns the value indicating whether the range update required.</returns>
        protected bool CanUpdateRange(object x, object y)
        {
            var isUpdateXAxis = false;
            var isUpdateYAxis = false;
            if (x != null)
            {
                var xVal = ConvertData(x, XAxis);
                if (xVal < XAxis.VisibleRange.Start || xVal > XAxis.VisibleRange.End)
                    isUpdateXAxis = true;
            }

            if (y != null)
            {
                var yVal = ConvertData(y, YAxis);
                if (yVal < YAxis.VisibleRange.Start || yVal > YAxis.VisibleRange.End)
                    isUpdateYAxis = true;
            }

            return isUpdateXAxis || isUpdateYAxis;
        }

        /// <summary>
        /// Gets the element position.
        /// </summary>
        /// <param name="desiredSize">The desired size to get alignment position.</param>
        /// <param name="originalPosition">The original position.</param>
        /// <returns>Returns the element position.</returns>
        protected Point GetElementPosition(Size desiredSize, Point originalPosition)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.InternalHorizontalAlignment;
            VerticalAlignment verticalAlignment = this.InternalVerticalAlignment;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X -= (desiredSize.Width / 2);
                    break;
                case HorizontalAlignment.Left:
                    point.X -= (desiredSize.Width);
                    break;
                case HorizontalAlignment.Right:
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    break;
                case VerticalAlignment.Center:
                    point.Y -= (desiredSize.Height / 2);
                    break;
                case VerticalAlignment.Top:
                    point.Y -= (desiredSize.Height);
                    break;
            }

            return point;
        }

        /// <summary>
        /// Gets the element position.
        /// </summary>
        /// <param name="element">The annotation <see cref="FrameworkElement"/>.</param>
        /// <param name="originalPosition">The original position.</param>
        /// <returns>Returns the element position.</returns>
        protected Point GetElementPosition(FrameworkElement element, Point originalPosition)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.InternalHorizontalAlignment;
            VerticalAlignment verticalAlignment = this.InternalVerticalAlignment;
            Size desiredSize = new Size(element.ActualWidth, element.ActualHeight);

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X -= (desiredSize.Width / 2);
                    break;
                case HorizontalAlignment.Left:
                    point.X -= (desiredSize.Width);
                    break;
                case HorizontalAlignment.Right:
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    break;
                case VerticalAlignment.Center:
                    point.Y -= (desiredSize.Height / 2);
                    break;
                case VerticalAlignment.Top:
                    point.Y -= (desiredSize.Height);
                    break;
            }

            return point;
        }

        /// <summary>
        /// Checks whether the given two <see cref="Windows.Foundation.Rect"/> intersects.
        /// </summary>
        /// <param name="r1">The first <see cref="Windows.Foundation.Rect"/></param>
        /// <param name="r2">The second <see cref="Windows.Foundation.Rect"/></param>
        /// <returns>Returns a <see cref="bool"/> value indicating whether the two <see cref="Rect"/> are intersecting.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "Proteced Methods")]
        protected bool IntersectsWith(Rect r1, Rect r2)
        {
            return !(r2.Left > r1.Right ||
                     r2.Right < r1.Left ||
                     r2.Top > r1.Bottom ||
                     r2.Bottom < r1.Top);
        }

        /// <summary>
        /// This method is used to modify the value for clipping out of axis
        /// </summary>
        /// <param name="value">The value to be checked in clipping range.</param>
        /// <param name="axis">The axis to get the visible range.</param>
        /// <returns>Returns the clipped values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "Proteced Methods")]
        protected double GetClippingValues(double value, ChartAxis axis)
        {
            DoubleRange range = axis.VisibleRange;

            if (value < range.Start)
                return range.Start;
            else if (value > range.End)
                return range.End;
            else
                return value;
        }

        /// <summary>
        /// Sets the required x and y values.
        /// </summary>
        protected void SetData()
        {
            SetAxisFromName();
            if (XAxis != null && YAxis != null)
            {
                x1 = ConvertData(X1, XAxis);
                y1 = ConvertData(Y1, YAxis);
            }
        }

        /// <summary>
        /// Sets the annotation bindings to the required elements.
        /// </summary>
        protected virtual void SetBindings()
        {
            Binding horizontalAlignBinding = new Binding { Path = new PropertyPath("HorizontalAlignment"), Source = this };
            this.SetBinding(Annotation.InternalHorizontalAlignmentProperty, horizontalAlignBinding);
            Binding verticalAlignBinding = new Binding { Path = new PropertyPath("VerticalAlignment"), Source = this };
            this.SetBinding(Annotation.InternalVerticalAlignmentProperty, verticalAlignBinding);
            Binding visibilityBinding = new Binding { Path = new PropertyPath("Visibility"), Source = this };
            this.SetBinding(Annotation.InternalVisibilityProperty, visibilityBinding);
            if (TextElement != null)
            {
                Binding textBinding = new Binding { Path = new PropertyPath("Text"), Source = this };
                TextElement.SetBinding(ContentControl.ContentProperty, textBinding);
                Binding fontSizeBinding = new Binding { Source = this, Path = new PropertyPath("FontSize") };
                TextElement.SetBinding(ContentControl.FontSizeProperty, fontSizeBinding);
                Binding fontStyleBinding = new Binding { Source = this, Path = new PropertyPath("FontStyle") };
                TextElement.SetBinding(ContentControl.FontStyleProperty, fontStyleBinding);
                Binding fontStretchBinding = new Binding { Source = this, Path = new PropertyPath("FontStretch") };
                TextElement.SetBinding(ContentControl.FontStretchProperty, fontStretchBinding);
                Binding fontFamilyBinding = new Binding { Source = this, Path = new PropertyPath("FontFamily") };
                TextElement.SetBinding(ContentControl.FontFamilyProperty, fontFamilyBinding);
                Binding fontWeightBinding = new Binding { Source = this, Path = new PropertyPath("FontWeight") };
                TextElement.SetBinding(ContentControl.FontWeightProperty, fontWeightBinding);
                Binding foregroundBinding = new Binding { Source = this, Path = new PropertyPath("Foreground") };
                TextElement.SetBinding(ContentControl.ForegroundProperty, foregroundBinding);
                Binding templateBinding = new Binding { Source = this, Path = new PropertyPath("ContentTemplate") };
                TextElement.SetBinding(ContentControl.ContentTemplateProperty, templateBinding);
            }
        }

        /// <summary>
        /// Rotates the element to the specified angle.
        /// </summary>
        /// <param name="angle">The angle specified for rotation.</param>
        /// <param name="item">The <see cref="FrameworkElement"/>.</param>
        /// <returns>Returns the transformed element.</returns>
        protected Rect RotateElement(double angle, FrameworkElement item)
        {
            double angleRadians = (2 * Math.PI * angle) / 360;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);
            var matrix = new MatrixTransform();
            transformation = new Matrix(cos, sin, -sin, cos, 0, 0);
            double offsety = item.ActualHeight / 2;
            transformedDesiredElement = Annotation.ElementTransform(new Rect(0, 0, item.ActualWidth, item.ActualHeight), transformation);
            matrix.Matrix = new Matrix(cos, sin, -sin, cos, 0, (transformedDesiredElement.Height / 2) - offsety);

            return transformedDesiredElement;
        }

        /// <summary>
        /// Gets the rotated points.
        /// </summary>
        /// <param name="angle">The specified angle.</param>
        /// <param name="item">The <see cref="FrameworkElement"/>.</param>
        /// <param name="originalPoint">The original point.</param>
        /// <returns>Returns the matrix transformed.</returns>
        protected Point GetRotatePoint(double angle, FrameworkElement item, Point originalPoint)
        {
            double angleRadians = (2 * Math.PI * angle) / 360;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);
            var trfmGroup = new TransformGroup();
            var matrix = new MatrixTransform();
            transformation = new Matrix(cos, sin, -sin, cos, 0, 0);
            double offsety = item.ActualHeight / 2;
            transformedDesiredElement = Annotation.ElementTransform(new Rect(0, 0, item.ActualWidth, item.ActualHeight), transformation);
            matrix.Matrix = new Matrix(cos, sin, -sin, cos, 0, (transformedDesiredElement.Height / 2) - offsety);
            item.RenderTransformOrigin = new Point(0.5, 0.5);
            trfmGroup.Children.Add(matrix);
            item.RenderTransform = trfmGroup;
#if !NETFX_CORE
            return matrix.Transform(originalPoint);
#else
            return matrix.Matrix.Transform(originalPoint);
#endif
        }

        /// <summary>
        /// Rotates the element to the specified angle.
        /// </summary>
        /// <param name="angle">The angle specified for rotation.</param>
        /// <param name="item">The <see cref="FrameworkElement"/>.</param>
        /// <param name="itemSize">The <see cref="FrameworkElement"/> size.</param>
        /// <returns>Returns the transformed element.</returns>
        protected Rect RotateElement(double angle, FrameworkElement item, Size itemSize)
        {
            double angleRadians = (2 * Math.PI * angle) / 360;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);
            var trfmGroup = new TransformGroup();
            var matrix = new MatrixTransform();
            transformation = new Matrix(cos, sin, -sin, cos, 0, 0);
            double offsety = itemSize.Height / 2;
            transformedDesiredElement = Annotation.ElementTransform(new Rect(0, 0, itemSize.Width, itemSize.Height), transformation);
            matrix.Matrix = new Matrix(cos, sin, -sin, cos, 0, (transformedDesiredElement.Height / 2) - offsety);
            trfmGroup.Children.Add(matrix);

            return transformedDesiredElement;
        }

        /// <summary>
        /// Calculates the ensure point.
        /// </summary>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <returns>Returns the ensure point.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMemberAsStatic", Justification = "Proteced Methods")]
        protected Point EnsurePoint(Point point1, Point point2)
        {
            double x = point1.X;
            double y = point1.Y;
            x = Math.Min(x, point2.X);
            y = Math.Min(y, point2.Y);

            return new Point(x, y);
        }

        /// <summary>
        /// Clones the annotation.
        /// </summary>
        /// <param name="annotation">The annotation cloned is updated.</param>
        /// <returns>Returns the cloned annotation.</returns>
        protected virtual DependencyObject CloneAnnotation(Annotation annotation)
        {
            annotation.ContentTemplate = this.ContentTemplate;
            annotation.CoordinateUnit = this.CoordinateUnit;
            annotation.FontFamily = this.FontFamily;
            annotation.FontSize = this.FontSize;
            annotation.FontStyle = this.FontStyle;
            annotation.FontWeight = this.FontWeight;
            annotation.Foreground = this.Foreground;
            annotation.InternalHorizontalAlignment = this.InternalHorizontalAlignment;
            annotation.Text = this.Text;
            annotation.EnableEditing = this.EnableEditing;
            annotation.InternalVerticalAlignment = this.InternalVerticalAlignment;
            annotation.X1 = this.X1;
            annotation.Y1 = this.Y1;
            annotation.XAxisName = this.XAxisName;
            annotation.YAxisName = this.YAxisName;
            annotation.ShowToolTip = this.ShowToolTip;
            annotation.ToolTipContent = this.ToolTipContent;
            annotation.ToolTipPlacement = this.ToolTipPlacement;
            annotation.ToolTipShowDuration = this.ToolTipShowDuration;
            annotation.ToolTipTemplate = this.ToolTipTemplate;
            return annotation;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the annotation when axis name changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="args">The dependency property changed event arguments.</param>
        private static void OnAxisNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null)
                annotation.UpdateAnnotation();
        }

        /// <summary>
        /// Updates the chart and annotation on property changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="args">The dependency property changed event arguments.</param>
        private static void OnUpdatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null) annotation.UpdatePropertyChanged(args);
        }

        /// <summary>
        /// Updates the annotation on visibility changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void OnVisibiltyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!e.OldValue.Equals(e.NewValue))
                (d as Annotation).OnVisibilityChanged();
        }

        /// <summary>
        /// Updates the editing when <see cref="EnableEditing"/> property changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void OnEnableEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = d as Annotation;
            if (annotation != null)
                annotation.OnEditing(annotation);
        }

        /// <summary>
        /// Updates the annotation when <see cref="EnableClipping"/> property changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void OnEnableClippingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Annotation annotation = d as Annotation;
            if (annotation != null) annotation.UpdateAnnotation();
        }

        /// <summary>
        /// Updates the annotation positioning when <see cref="CoordinateUnit"/> property changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void OnCoordinatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = d as Annotation;
            if (e.OldValue.Equals(e.NewValue)) return;
            if (annotation != null && annotation.chart != null && annotation.chart.AnnotationManager != null)
            {
                annotation.chart.AnnotationManager.AddOrRemoveAnnotations(annotation, true);
                annotation.UpdatePropertyChanged(e);
            }
        }

        /// <summary>
        /// Element Transformation takes place.
        /// </summary>
        /// <param name="rect">The <see cref="Windows.Foundation.Rect"/></param>
        /// <param name="matrix">The matrix passed for the transform.</param>
        /// <returns>Returns the transformed element.</returns>
        private static Rect ElementTransform(Rect rect, Matrix matrix)
        {
            Point leftTop = matrix.Transform(new Point(rect.Left, rect.Top));
            Point rightTop = matrix.Transform(new Point(rect.Right, rect.Top));
            Point leftBottom = matrix.Transform(new Point(rect.Left, rect.Bottom));
            Point rightBottom = matrix.Transform(new Point(rect.Right, rect.Bottom));
            double left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            double top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            double right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            double bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));
            return new Rect(left, top, right - left, bottom - top);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the editing when <see cref="EnableEditing"/> property changed.
        /// </summary>
        /// <param name="annotation">The annotation to be edited.</param>
        private void OnEditing(Annotation annotation)
        {
            if (Chart != null && !(Chart.AnnotationManager.TextAnnotation != annotation))
                Chart.AnnotationManager.OnTextEditing();
        }

        /// <summary>
        /// Updates the text element when pointer pressed.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The pointer routed event arguments.</param>
        private void TextElement_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // to disable editing when previous annotation is in Editing Mode
            if (Chart.AnnotationManager.IsEditing)
                Chart.AnnotationManager.OnTextEditing();

            // to enable ediding in annoatation 
            if (EnableEditing && ContentTemplate == null)
            {
                e.Handled = true;
                OnTextEditingMode(TextElement);
            }
        }

        /// <summary>
        /// Sets the text element binding.
        /// </summary>
        /// <param name="textElement">The text element to be bind.</param>
        private void SetTextElementBinding(TextBox textElement)
        {
            if (textElement != null)
            {
                Binding textBinding = new Binding { Path = new PropertyPath("Text"), Source = this };
                textElement.SetBinding(TextBox.TextProperty, textBinding);
                Binding fontSizeBinding = new Binding { Source = this, Path = new PropertyPath("FontSize") };
                textElement.SetBinding(TextBox.FontSizeProperty, fontSizeBinding);
                Binding fontStyleBinding = new Binding { Source = this, Path = new PropertyPath("FontStyle") };
                textElement.SetBinding(TextBox.FontStyleProperty, fontStyleBinding);
                Binding fontStretchBinding = new Binding { Source = this, Path = new PropertyPath("FontStretch") };
                textElement.SetBinding(TextBox.FontStretchProperty, fontStretchBinding);
                Binding fontFamilyBinding = new Binding { Source = this, Path = new PropertyPath("FontFamily") };
                textElement.SetBinding(TextBox.FontFamilyProperty, fontFamilyBinding);
                Binding fontWeightBinding = new Binding { Source = this, Path = new PropertyPath("FontWeight") };
                textElement.SetBinding(TextBox.FontWeightProperty, fontWeightBinding);
                Binding foregroundBinding = new Binding { Source = this, Path = new PropertyPath("Foreground") };
                textElement.SetBinding(TextBox.ForegroundProperty, foregroundBinding);
            }
        }

        /// <summary>
        /// Replace the <see cref="TextBlock"/> to <see cref="TextBox"/> while editing text
        /// </summary>
        /// <param name="textElement">The text element.</param>
        private void OnTextEditingMode(ContentControl textElement)
        {
            if (chart.AnnotationManager.SelectedAnnotation != null)
                chart.AnnotationManager.SelectedAnnotation = null;
            if (textElement != null)
                Chart.AnnotationManager.EditAnnotation = textElement;
            contentTemplate = ChartDictionaries.GenericCommonDictionary["textBoxAnnotation"] as DataTemplate;
            var textBox = contentTemplate.LoadContent() as TextBox;
            textElement.SetValue(ContentControl.ContentProperty, textBox);

            SetTextElementBinding(textBox);

            Chart.AnnotationManager.TextBox = textBox;
            Chart.AnnotationManager.IsEditing = true;
            Chart.AnnotationManager.TextAnnotation = this;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for Annotation DragDelta/ResizeDelta event arguments
    /// </summary>
    public partial class AnnotationDragDeltaEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new <see cref="Position"/>.
        /// </summary>
        public Position NewValue { get; set; }

        /// <summary>
        /// Gets or sets the old <see cref="Position"/>
        /// </summary>
        public Position OldValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the event.
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// Class implementation for Annotation DragCompleted/ResizeCompleted event arguments
    /// </summary>
    public partial class AnnotationDragCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new <see cref="Position"/>
        /// </summary>
        public Position NewValue { get; set; }
    }

    /// <summary>
    /// Class implementation for Annotation positioning points 
    /// </summary>
    public partial class Position
    {
        /// <summary>
        /// Gets or sets the x1 position.
        /// </summary>
        public object X1 { get; set; }

        /// <summary>
        /// Gets or sets the x2 position.
        /// </summary>
        public object X2 { get; set; }

        /// <summary>
        /// Gets or sets the y1 position.
        /// </summary>
        public object Y1 { get; set; }

        /// <summary>
        /// Gets or sets the y2 position.
        /// </summary>
        public object Y2 { get; set; }
    }
}
