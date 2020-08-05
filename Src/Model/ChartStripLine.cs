using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{

    /// <summary>
    /// Chart enables the user to highlight a specific region of <see cref="ChartAxis"/> by adding strip lines to it.
    /// </summary>
    /// <remarks>
    /// The strip lines length and width can be customized,a text label can be specified and also the look and feel can be customized too.
    /// </remarks>
 
    public partial class ChartStripLine : FrameworkElement, INotifyPropertyChanged
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="Background"/> property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                "Background",
                typeof(Brush),
                typeof(ChartStripLine),
                new PropertyMetadata(null));
        
        /// <summary>
        /// The DependencyProperty for <see cref="BorderBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                "BorderBrush",
                typeof(Brush),
                typeof(ChartStripLine),
                new PropertyMetadata(null));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="BorderThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness", 
                typeof(Thickness), 
                typeof(ChartStripLine),
                new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// The DependencyProperty for <see cref="Start"/> property.
        /// </summary>
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(
                "Start",
                typeof(double),
                typeof(ChartStripLine),
                new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnStartPropertChanged)));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentStartValue"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentStartValueProperty =
            DependencyProperty.Register(
                "SegmentStartValue", 
                typeof(double),
                typeof(ChartStripLine),
                new PropertyMetadata(double.NaN, OnSegmentStartValueChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentEndValue"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentEndValueProperty =
            DependencyProperty.Register(
                "SegmentEndValue",
                typeof(double), 
                typeof(ChartStripLine),
                new PropertyMetadata(double.NaN, OnSegmentEndValueChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentAxisName"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentAxisNameProperty =
            DependencyProperty.Register(
                "SegmentAxisName",
                typeof(string), 
                typeof(ChartStripLine),
                new PropertyMetadata(string.Empty, OnSegmentAxisNameChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="IsSegmented"/> property.
        /// </summary>
        public static readonly DependencyProperty IsSegmentedProperty =
            DependencyProperty.Register(
                "IsSegmented", 
                typeof(bool), 
                typeof(ChartStripLine),
                new PropertyMetadata(false, OnIsSegmentedPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="RepeatEvery"/> property.
        /// </summary>
        public static readonly DependencyProperty RepeatEveryProperty =
            DependencyProperty.Register(
                "RepeatEvery", 
                typeof(double), 
                typeof(ChartStripLine),
                new PropertyMetadata(0d, OnRepeatEveryPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="RepeatUntil"/> property.
        /// </summary>
        public static readonly DependencyProperty RepeatUntilProperty =
            DependencyProperty.Register(
                "RepeatUntil", 
                typeof(double),
                typeof(ChartStripLine),
                new PropertyMetadata(double.NaN, OnRepeatUntilPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="Label"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                "Label", 
                typeof(object), 
                typeof(ChartStripLine),
                new PropertyMetadata(null));
        
        /// <summary>
        /// The DependencyProperty for <see cref="LabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register(
                "LabelTemplate",
                typeof(DataTemplate),
                typeof(ChartStripLine),
                new PropertyMetadata(null));
        
        /// <summary>
        /// The DependencyProperty for <see cref="Width"/> property.
        /// </summary>
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                "Width", 
                typeof(double), 
                typeof(ChartStripLine),
                new PropertyMetadata(0d, new PropertyChangedCallback(OnWidthPropertyChanged)));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="LabelAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelAngleProperty =
            DependencyProperty.Register(
                "LabelAngle", 
                typeof(double),
                typeof(ChartStripLine),
                new PropertyMetadata(0d, OnLabelAnglePropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="IsPixelWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty IsPixelWidthProperty =
            DependencyProperty.Register(
                "IsPixelWidth",
                typeof(bool),
                typeof(ChartStripLine),
                new PropertyMetadata(false, OnIsPixelWidthPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="LabelHorizontalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
            DependencyProperty.Register(
                "LabelHorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(ChartStripLine), 
                new PropertyMetadata(HorizontalAlignment.Center));
        
        /// <summary>
        /// The DependencyProperty for <see cref="LabelVerticalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty =
            DependencyProperty.Register(
                "LabelVerticalAlignment", 
                typeof(VerticalAlignment),
                typeof(ChartStripLine), 
                new PropertyMetadata(VerticalAlignment.Center));

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the start range for the Stripline.
        /// </summary>
        public double Start
        {
            get { return (double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        /// <summary>
        /// Gets or sets the fill color for this Stripline.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
      
        /// <summary>
        /// Gets or sets the border brush of the Stripline.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }
       
        /// <summary>
        /// Gets or sets the start value for the Stripline, when <see cref="IsSegmented"/> is set as <c>true</c>..
        /// </summary>
        public double SegmentStartValue
        {
            get { return (double)GetValue(SegmentStartValueProperty); }
            set { SetValue(SegmentStartValueProperty, value); }
        }
              
        /// <summary>
        /// Gets or sets the end value for the Stripline, when <see cref="IsSegmented"/> is set as <c>true</c>..
        /// </summary>
        public double SegmentEndValue
        {
            get { return (double)GetValue(SegmentEndValueProperty); }
            set { SetValue(SegmentEndValueProperty, value); }
        }
                     
        /// <summary>
        ///  Gets or sets the name of the axis associated with the segmented Stripline.
        /// </summary>
        public string SegmentAxisName
        {
            get { return (string)GetValue(SegmentAxisNameProperty); }
            set { SetValue(SegmentAxisNameProperty, value); }
        }
               
        /// <summary>
        /// Gets or sets a value indicating whether to enable the segmented stripline.
        /// </summary>
        public bool IsSegmented
        {
            get { return (bool)GetValue(IsSegmentedProperty); }
            set { SetValue(IsSegmentedProperty, value); }
        }
                       
        /// <summary>
        /// Gets or sets the stripline interval.
        /// </summary>
        /// <remarks>
        /// This property used to draw multiple striplines repeatedly.
        /// </remarks>
        public double RepeatEvery
        {
            get { return (double)GetValue(RepeatEveryProperty); }
            set { SetValue(RepeatEveryProperty, value); }
        }
                
        /// <summary>
        /// Gets or sets the end value, till that striplines will be drawn.
        /// </summary>
        public double RepeatUntil
        {
            get { return (double)GetValue(RepeatUntilProperty); }
            set { SetValue(RepeatUntilProperty, value); }
        }
                
        /// <summary>
        /// Gets or sets the label to be displayed inside the Stripline.
        /// </summary>
        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the custom template for the Stripline label.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }
       
        /// <summary>
        /// Gets or sets the width of the Stripline.
        /// </summary>
        public new double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
               
        /// <summary>
        /// Gets or sets rotation angle for the Stripline angle.
        /// </summary>
        public double LabelAngle
        {
            get { return (double)GetValue(LabelAngleProperty); }
            set { SetValue(LabelAngleProperty, value); }
        }
                      
        /// <summary>
        /// Gets or sets a value indicating whether the value specified in Width property should be measured in pixels.
        /// </summary>
        public bool IsPixelWidth
        {
            get { return (bool)GetValue(IsPixelWidthProperty); }
            set { SetValue(IsPixelWidthProperty, value); }
        }
                
        /// <summary>
        /// Gets or sets horizontal alignment of stripline label.
        /// </summary>
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }
       
        /// <summary>
        /// Gets or sets vertical alignment of the Stripline label.
        /// </summary>
        public VerticalAlignment LabelVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Clone the strip line
        /// </summary>
        /// <returns></returns>
        public DependencyObject Clone()
        {
            return CloneStripline(null);
        }

        #endregion

        #region Protected Virtual Methods

        protected virtual DependencyObject CloneStripline(DependencyObject obj)
        {
            ChartStripLine stripline = new ChartStripLine();
            stripline.Start = this.Start;
            stripline.Label = this.Label;
            stripline.Background = this.Background;
            stripline.BorderBrush = this.BorderBrush;
            stripline.BorderThickness = this.BorderThickness;
            stripline.SegmentEndValue = this.SegmentEndValue;
            stripline.SegmentStartValue = this.SegmentStartValue;
            stripline.SegmentAxisName = this.SegmentAxisName;
            stripline.IsSegmented = this.IsSegmented;
            stripline.RepeatEvery = this.RepeatEvery;
            stripline.RepeatUntil = this.RepeatUntil;
            stripline.LabelTemplate = this.LabelTemplate;
            stripline.LabelAngle = this.LabelAngle;
            stripline.LabelVerticalAlignment = this.LabelVerticalAlignment;
            stripline.LabelHorizontalAlignment = this.LabelHorizontalAlignment;
            stripline.IsPixelWidth = stripline.IsPixelWidth;
            stripline.Width = this.Width;
            return stripline;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Called when StartX property changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnStartPropertChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("Start");
        }

        private static void OnSegmentStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("SegmentStartValue");
        }

        private static void OnSegmentEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("SegmentEndValue");
        }
        
        private static void OnSegmentAxisNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("SegmentAxisName");
        }

        private static void OnIsSegmentedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("IsSegmented");
        }

        private static void OnRepeatEveryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("RepeatEvery");
        }

        private static void OnRepeatUntilPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("RepeatUntil");
        }
        
        private static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("Width");
        }

        private static void OnLabelAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("LabelAngle");
        }

        private static void OnIsPixelWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("IsPixelWidth");
        }

        #endregion

        #region Private Methods

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #endregion        
    }
}
