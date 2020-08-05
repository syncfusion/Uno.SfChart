using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class ChartMultiLevelLabel : DependencyObject, INotifyPropertyChanged, ICloneable
    {        
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty of <see cref="Start"/> property
        /// </summary>
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(
                "Start",
                typeof(object),
                typeof(ChartMultiLevelLabel),
                new PropertyMetadata(null, OnStartPropertyChanged));

        /// <summary>
        /// The DependencyProperty of <see cref="End"/> property
        /// </summary>
        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register(
                "End",
                typeof(object),
                typeof(ChartMultiLevelLabel),
                new PropertyMetadata(null, OnEndPropertyChanged));

        /// <summary>
        /// The DependencyProperty of <see cref="Level"/> property
        /// </summary>
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register(
                "Level", 
                typeof(int),
                typeof(ChartMultiLevelLabel),
                new PropertyMetadata(0, OnLevelPropertyChanged));

        /// <summary>
        /// The DependencyProperty of <see cref="Text"/> property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(ChartMultiLevelLabel),
                new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        /// <summary>
        /// The DependencyProperty of <see cref="FontSize"/> property
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                "FontSize",
                typeof(double), 
                typeof(ChartMultiLevelLabel),
                new PropertyMetadata(12d, OnFontSizePropertyChanged));

        /// <summary>
        /// The DependencyProperty of <see cref="Foreground"/> property
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
         DependencyProperty.Register(
             "Foreground", 
             typeof(Brush),
             typeof(ChartMultiLevelLabel),
             new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// The DependencyProperty of <see cref="FontFamily"/> property
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
           DependencyProperty.Register(
               "FontFamily", 
               typeof(FontFamily),
               typeof(ChartMultiLevelLabel),
               new PropertyMetadata(new FontFamily("Segoe UI")));

        /// <summary>
        /// The DependencyProperty of <see cref="LabelAlignment"/> property
        /// </summary>
        public static readonly DependencyProperty LabelAlignmentProperty =
            DependencyProperty.Register(
                "LabelAlignment",
                typeof(LabelAlignment),
                typeof(ChartMultiLevelLabel),
                new PropertyMetadata(LabelAlignment.Center, OnLabelAlignmentPropertyChanged));

        #endregion
        
        #region Constructor

        /// <summary>
        /// Intializes a new instance for <see cref="ChartMultiLevelLabel"/> class
        /// </summary>
        public ChartMultiLevelLabel()
        {
        }

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
        /// Gets or sets the start value for label
        /// </summary>
        public object Start
        {
            get { return (object)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end value for label
        /// </summary>
        public object End
        {
            get { return (object)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label level
        /// </summary>
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the fontsize for label
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for label's foreground
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font family for label
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets alignment for label placement
        /// </summary>
        public LabelAlignment LabelAlignment
        {
            get { return (LabelAlignment)GetValue(LabelAlignmentProperty); }
            set { SetValue(LabelAlignmentProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// To clone the ChartMultiAxisLabel
        /// </summary>
        /// <returns></returns>
        public DependencyObject Clone()
        {
            return CloneMultiAxisLabel(null);
        }

        #endregion

        #region Internal Methods

        // To set the properties of corresponding textblock and border
        internal void SetVisualBinding(TextBlock textBlock, Border border, ChartAxisBase2D axis)
        {
            Binding binding = new Binding() { Source = this, Path = new PropertyPath("FontSize") };
            textBlock.SetBinding(TextBlock.FontSizeProperty, binding);
            binding = new Binding() { Source = this, Path = new PropertyPath("Text") };
            textBlock.SetBinding(TextBlock.TextProperty, binding);
            binding = new Binding() { Source = this, Path = new PropertyPath("Foreground") };
            textBlock.SetBinding(TextBlock.ForegroundProperty, binding);
            binding = new Binding() { Source = this, Path = new PropertyPath("FontFamily") };
            textBlock.SetBinding(TextBlock.FontFamilyProperty, binding);
            textBlock.Tag = this;
            binding = new Binding() { Source = axis, Path = new PropertyPath("LabelBorderBrush") };
            border.SetBinding(Border.BorderBrushProperty, binding);
        }

        // To set the properties of corresponding textblock and brace's polyline
        internal void SetBraceVisualBinding(TextBlock textBlock, Shape polyline1, Shape polyline2, ChartAxisBase2D axis)
        {
            Binding binding = new Binding() { Source = this, Path = new PropertyPath("FontSize") };
            textBlock.SetBinding(TextBlock.FontSizeProperty, binding);
            binding = new Binding() { Source = this, Path = new PropertyPath("Foreground") };
            textBlock.SetBinding(TextBlock.ForegroundProperty, binding);
            binding = new Binding() { Source = this, Path = new PropertyPath("FontFamily") };
            textBlock.SetBinding(TextBlock.FontFamilyProperty, binding);
            binding = new Binding() { Source = this, Path = new PropertyPath("Text") };
            textBlock.SetBinding(TextBlock.TextProperty, binding);
            textBlock.Tag = this;
            binding = new Binding() { Source = axis, Path = new PropertyPath("LabelBorderBrush") };
            polyline1.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding() { Source = axis, Path = new PropertyPath("LabelBorderBrush") };
            polyline2.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding() { Source = axis, Path = new PropertyPath("LabelBorderWidth") };
            polyline1.SetBinding(Shape.StrokeThicknessProperty, binding);
            binding = new Binding() { Source = axis, Path = new PropertyPath("LabelBorderWidth") };
            polyline2.SetBinding(Shape.StrokeThicknessProperty, binding);
        }

        #endregion

        #region Protected Virtual Methods
        
        // To Clone the multi level label properties
        protected virtual DependencyObject CloneMultiAxisLabel(DependencyObject obj)
        {
            ChartMultiLevelLabel multiLevelLabel = new ChartMultiLevelLabel();
            multiLevelLabel.Start = this.Start;
            multiLevelLabel.End = this.End;
            multiLevelLabel.Level = this.Level;
            multiLevelLabel.Text = this.Text;
            multiLevelLabel.FontSize = this.FontSize;
            multiLevelLabel.Foreground = this.Foreground;
            multiLevelLabel.FontFamily = this.FontFamily;
            multiLevelLabel.LabelAlignment = this.LabelAlignment;
            return multiLevelLabel;
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// <see cref="Start"/> property changed event call back.
        /// </summary>
        /// <param name="d">Dependency Object</param>
        /// <param name="e">Event Args</param>
        private static void OnStartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartMultiLevelLabel).OnPropertyChanged("Start");
        }

        /// <summary>
        /// <see cref="End"/> property changed event call back.
        /// </summary>
        /// <param name="d">Dependency Object</param>
        /// <param name="e">Event Args</param>
        private static void OnEndPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartMultiLevelLabel).OnPropertyChanged("End");
        }

        /// <summary>
        /// <see cref="Level"/> property changed event call back.
        /// </summary>
        /// <param name="d">Dependency Object</param>
        /// <param name="e">Event Args</param>
        private static void OnLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartMultiLevelLabel).OnPropertyChanged("Level");
        }

        /// <summary>
        /// <see cref="Text"/> property changed event call back.
        /// </summary>
        /// <param name="d">Dependency Object</param>
        /// <param name="e">Event Args</param>
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartMultiLevelLabel).OnPropertyChanged("Text");
        }

        /// <summary>
        /// <see cref="FontSize"/> property changed event call back.
        /// </summary>
        /// <param name="d">Dependency Object</param>
        /// <param name="e">Event Args</param>
        private static void OnFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartMultiLevelLabel).OnPropertyChanged("FontSize");
        }

        /// <summary>
        /// <see cref="LabelAlignment"/> property changed event call back.
        /// </summary>
        /// <param name="d">Dependency Object</param>
        /// <param name="e">Event Args</param>
        private static void OnLabelAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartMultiLevelLabel).OnPropertyChanged("LabelAlignment");
        }

        #endregion

        #region Private
        
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
