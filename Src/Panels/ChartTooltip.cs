// <copyright file="ChartTooltip.cs" company="Syncfusion. Inc">
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
    using System.Threading;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// Represents a content control that display a information about focused element. 
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ContentControl" />
    public partial class ChartTooltip : ContentControl
    {
        #region Dependency Property Registration

        /// <summary>
        ///  The DependencyProperty for <see cref="ShowDuration"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowDurationProperty =
            DependencyProperty.RegisterAttached(
                "ShowDuration",
                typeof(int),
                typeof(ChartTooltip),
                new PropertyMetadata(1000));

        /// <summary>
        ///  The DependencyProperty for <see cref="InitialShowDelay"/> property.
        /// </summary>
        public static readonly DependencyProperty InitialShowDelayProperty =
            DependencyProperty.RegisterAttached(
                "InitialShowDelay",
                typeof(int),
                typeof(ChartTooltip),
                new PropertyMetadata(0));

        /// <summary>
        ///  The DependencyProperty for <see cref="HorizontalOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalOffset",
                typeof(double),
                typeof(ChartTooltip),
                new PropertyMetadata(0d));

        /// <summary>
        ///  The DependencyProperty for <see cref="VerticalOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "VerticalOffset",
                typeof(double),
                typeof(ChartTooltip),
                new PropertyMetadata(0d));

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalAlignment"/> property.
        /// </summary>
        public static new readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(ChartTooltip),
                new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalAlignment"/> property.
        /// </summary>
        public static new readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "VerticalAlignment",
                typeof(VerticalAlignment),
                typeof(ChartTooltip),
                new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableAnimation"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.RegisterAttached(
                "EnableAnimation",
                typeof(bool),
                typeof(ChartTooltip),
                new PropertyMetadata(false));

        /// <summary>
        /// The DependencyProperty for <see cref="TooltipMargin"/> property.
        /// </summary>
        public static readonly DependencyProperty TooltipMarginProperty =
            DependencyProperty.RegisterAttached(
                "TooltipMargin",
                typeof(Thickness),
                typeof(ChartTooltip),
                new PropertyMetadata(new Thickness(0)));

        /// <summary>
        ///  The DependencyProperty for <see cref="TopOffset"/> property.
        /// </summary>
        internal static readonly DependencyProperty TopOffsetProperty =
            DependencyProperty.Register(
                "TopOffset",
                typeof(double),
                typeof(ChartTooltip),
                new PropertyMetadata(0d));

        /// <summary>
        ///  The DependencyProperty for <see cref="LeftOffset"/> property.
        /// </summary>
        internal static readonly DependencyProperty LeftOffsetProperty =
            DependencyProperty.Register(
                "LeftOffset",
                typeof(double),
                typeof(ChartTooltip),
                new PropertyMetadata(0d));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTooltip"/> class.
        /// </summary>
        public ChartTooltip()
        {
            this.IsHitTestVisible = false;

            Canvas.SetLeft(this, 0d);
            Canvas.SetTop(this, 0d);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the left offset.
        /// </summary>
        internal double LeftOffset
        {
            get { return (double)GetValue(LeftOffsetProperty); }
            set { SetValue(LeftOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the top offset.
        /// </summary>
        internal double TopOffset
        {
            get { return (double)GetValue(TopOffsetProperty); }
            set { SetValue(TopOffsetProperty, value); }
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Get the Enable Animation 
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns a value indicating whether the animation is enabled.</returns>
        public static bool GetEnableAnimation(UIElement obj)
        {
            return (bool)obj.GetValue(EnableAnimationProperty);
        }

        /// <summary>
        /// Set the Enable Animation
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <param name="value">The Value</param>
        public static void SetEnableAnimation(UIElement obj, bool value)
        {
            obj.SetValue(EnableAnimationProperty, value);
        }

        /// <summary>
        /// Get the HorizontalAlignment
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the horizontal alignment.</returns>
        public static HorizontalAlignment GetHorizontalAlignment(UIElement obj)
        {
            return (HorizontalAlignment)obj.GetValue(HorizontalAlignmentProperty);
        }

        /// <summary>
        /// Set the HorizontalAlignment
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <param name="value">The Value</param>
        public static void SetHorizontalAlignment(UIElement obj, HorizontalAlignment value)
        {
            obj.SetValue(HorizontalAlignmentProperty, value);
        }

        /// <summary>
        /// Get the VerticalAlignment
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the vertical alignment.</returns>
        public static VerticalAlignment GetVerticalAlignment(UIElement obj)
        {
            return (VerticalAlignment)obj.GetValue(VerticalAlignmentProperty);
        }

        /// <summary>
        /// Set the VerticalAlignment
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <param name="value">The Value</param>
        public static void SetVerticalAlignment(UIElement obj, VerticalAlignment value)
        {
            obj.SetValue(VerticalAlignmentProperty, value);
        }

        /// <summary>
        /// Get TooltipMargin value
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the <see cref="ChartTooltip"/> margin.</returns>
        public static Thickness GetTooltipMargin(UIElement obj)
        {
            return (Thickness)obj.GetValue(TooltipMarginProperty);
        }

        /// <summary>
        /// Set TooltipMargin value
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <param name="value">The Value</param>
        public static void SetTooltipMargin(UIElement obj, Thickness value)
        {
            obj.SetValue(TooltipMarginProperty, value);
        }

        /// <summary>
        /// Get ShowDuration value
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <returns>Returns the show duration.</returns>
        public static int GetShowDuration(DependencyObject obj)
        {
            return (int)obj.GetValue(ShowDurationProperty);
        }

        /// <summary>
        /// Set ShowDuration value
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <param name="value">The Value</param>
        public static void SetShowDuration(DependencyObject obj, int value)
        {
            obj.SetValue(ShowDurationProperty, value);
        }

        /// <summary>
        /// Get InitialShowDelay value
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <returns>Returns the show delay.</returns>
        public static int GetInitialShowDelay(DependencyObject obj)
        {
            return (int)obj.GetValue(InitialShowDelayProperty);
        }

        /// <summary>
        /// Set InitialShowDelay value
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <param name="value">The Value</param>
        public static void SetInitialShowDelay(DependencyObject obj, int value)
        {
            obj.SetValue(InitialShowDelayProperty, value);
        }

        /// <summary>
        /// Get HorizontalOffset value
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <returns>Returns the horizontal offset.</returns>
        public static double GetHorizontalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(HorizontalOffsetProperty);
        }

        /// <summary>
        /// Set HorizontalOffset value
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <param name="value">The Value</param>
        public static void SetHorizontalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(HorizontalOffsetProperty, value);
        }

        /// <summary>
        /// Get VerticalOffset value
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <returns>Returns the vertical offset.</returns>
        public static double GetVerticalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(VerticalOffsetProperty);
        }

        /// <summary>
        /// Set VerticalOffset value
        /// </summary>
        /// <param name="obj">The Dependency Object</param>
        /// <param name="value">The Value</param>
        public static void SetVerticalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(VerticalOffsetProperty, value);
        }

        #endregion

        #endregion
    }
}
