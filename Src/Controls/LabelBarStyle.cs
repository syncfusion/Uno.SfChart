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
