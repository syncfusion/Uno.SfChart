// <copyright file="LabelBarStyle.cs" company="Syncfusion. Inc">
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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Represents a dependency object that defines the style for axis label. 
    /// </summary>
    /// <seealso cref="System.Windows.DependencyObject" />
    public partial class LabelBarStyle : DependencyObject
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedLabelBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedLabelBrushProperty =
            DependencyProperty.Register(
                "SelectedLabelBrush",
                typeof(SolidColorBrush),
                typeof(LabelBarStyle),
                new PropertyMetadata(new SolidColorBrush(Colors.White), OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelHorizontalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
            DependencyProperty.Register(
                "LabelHorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(LabelBarStyle),
                new PropertyMetadata(HorizontalAlignment.Center, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Background"/> property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                "Background",
                typeof(Brush),
                typeof(LabelBarStyle),
                new PropertyMetadata(DefaultBackground, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedLabelStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedLabelStyleProperty =
            DependencyProperty.Register(
                "SelectedLabelStyle",
                typeof(Style),
                typeof(LabelBarStyle),
                new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Position"/> property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                "Position",
                typeof(BarPosition),
                typeof(LabelBarStyle),
                new PropertyMetadata(BarPosition.Outside, OnPropertyChanged));

        #endregion

        #region Fields

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Readonly propery in wpf platform")]
        private static SolidColorBrush DefaultBackground = Application.Current.RequestedTheme == ApplicationTheme.Dark
            ? new SolidColorBrush(Color.FromArgb(255, 34, 34, 34))
            : new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelBarStyle"/> class.
        /// </summary>
        public LabelBarStyle()
        {
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the HorizontalAlignment of the labels inside the label bar.
        /// </summary>
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Background the label bar.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the labels inside the selected region.
        /// </summary>
        public SolidColorBrush SelectedLabelBrush
        {
            get { return (SolidColorBrush)GetValue(SelectedLabelBrushProperty); }
            set { SetValue(SelectedLabelBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for labels in the selected region.
        /// </summary>
        public Style SelectedLabelStyle
        {
            get { return (Style)GetValue(SelectedLabelStyleProperty); }
            set { SetValue(SelectedLabelStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the position which is used to place the upper and lower labels inside or outside of the label bar.
        /// </summary>
        public BarPosition Position
        {
            get { return (BarPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the date time range navigator.
        /// </summary>
        internal SfDateTimeRangeNavigator DateTimeRangeNavigator { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Private Static Methods

        /// <summary>
        /// Updates the label bar style on it's property change.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var labelBarStyle = d as LabelBarStyle;
            if (labelBarStyle.DateTimeRangeNavigator != null)
            {
                labelBarStyle.DateTimeRangeNavigator.SetLabelPosition();
                labelBarStyle.DateTimeRangeNavigator.Scheduleupdate();
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents the <see cref="LabelStyle"/> class.
    /// </summary>
    public partial class LabelStyle : DependencyObject
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="FontFamily"/> property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                "FontFamily",
                typeof(FontFamily),
                typeof(LabelStyle),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="Foreground"/> property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground",
                typeof(SolidColorBrush),
                typeof(LabelStyle),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="FontSize"/> property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                "FontSize",
                typeof(double),
                typeof(LabelStyle),
                new PropertyMetadata(null));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelStyle"/> class.
        /// </summary>
        public LabelStyle()
        {
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the font family for label
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the foreground color for label
        /// </summary>
        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font size
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        #endregion

        #endregion
    }
}
