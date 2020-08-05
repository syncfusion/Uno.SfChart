using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Windows;

namespace Syncfusion.UI.Xaml.Charts
{
     /// <summary>
     ///ChartColorModel contains a number of predefined color palette and have custom brushes collection to populate a custom palette.
     /// </summary>
    
    public partial class ChartColorModel
    {
        #region Fields

        #region Private Fields

        private List<Brush> metroBrushes;

        private List<Brush> currentBrushes;

        private ChartColorPalette palette;

        private List<Brush> customBrushes;

        private List<Brush> autumnBrightsBrushes;

        private List<Brush> floraHuesBrushes;

        private List<Brush> pineappleBrushes;

        private List<Brush> tomatoSpectrumBrushes;

        private List<Brush> redChromeBrushes;

        private List<Brush> purpleChromeBrushes;

        private List<Brush> blueChromeBrushes;

        private List<Brush> greenChromeBrushes;

        private List<Brush> eliteBrushes;

        private List<Brush> sandyBeachBrushes;

        private List<Brush> lightCandyBrushes;

        #endregion

        #endregion
        
        #region Constructor

        /// <summary>
        /// Called when instance created for ChartColorModel
        /// </summary>
        public ChartColorModel()
        {
            CustomBrushes = new List<Brush>();
            this.ApplyPalette(ChartColorPalette.Metro);
        }

        /// <summary>
        /// Called when instance created for ChartColorModel with single arguments
        /// </summary>
        /// <param name="palette"></param>
        public ChartColorModel(ChartColorPalette palette)
        {
            if (Palette == palette)
                this.ApplyPalette(palette);
            Palette = palette;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the custom brushes to be used to paint the interiors of each segment or series.
        /// </summary>
        public List<Brush> CustomBrushes
        {
            get
            {
                return customBrushes;
            }
            set
            {
                customBrushes = value;
                if (Palette == ChartColorPalette.Custom)
                {
                    ApplyPalette(Palette);
                }

                //ColorModel custom brush dynamic update not working in when binding context changed.
                if (this.Series != null)
                {
                    if (Palette == ChartColorPalette.Custom)
                    {
                        if(Series.Segments != null)
                        {
                            Series.Segments.Clear();
                        }

                        if (Series.ActualArea != null)
                        {
                            Series.ActualArea.IsUpdateLegend = true;
                            Series.ActualArea.ScheduleUpdate();
                        }
                    }
                }
                else if (this.ChartArea != null)
                {
                    if (ChartArea.VisibleSeries.Count > 0)
                    {
                        for (int index = 0; index < ChartArea.VisibleSeries.Count; index++)
                        {
                            var visibleSeries = ChartArea.VisibleSeries[index];

                            if (visibleSeries.Segments != null)
                            {
                                visibleSeries.Segments.Clear();
                            }
                        }

                        ChartArea.IsUpdateLegend = true;
                        ChartArea.ScheduleUpdate();
                    }
                }
            }
        }

        #endregion

        #region Internal Properties

        internal ChartColorPalette Palette
        {
            get
            {
                return palette;
            }
            set
            {
                if (value != palette)
                {
                    palette = value;
                    ApplyPalette(palette);
                }
            }
        }

        internal ChartBase ChartArea { get; set; }

        internal ChartSeriesBase Series { get; set; }

        #endregion

        #endregion

        #region Methods

        #region  Public Methods

        /// <summary>
        /// Returns the collection of brushes for specified pallete
        /// </summary>
        /// <param name="palette">ChartColorPalette</param>
        /// <returns>List of brushes</returns>
        public List<Brush> GetBrushes(ChartColorPalette palette)
        {
            switch (palette)
            {
                case ChartColorPalette.Metro:
                    return GetMetroBrushes();
                case ChartColorPalette.AutumnBrights:
                    return GetAutumnBrushes();
                case ChartColorPalette.BlueChrome:
                    return GetBlueChromeBrushes();
                case ChartColorPalette.Elite:
                    return GetEliteBrushes();
                case ChartColorPalette.FloraHues:
                    return GetFloraHuesBrushes();
                case ChartColorPalette.GreenChrome:
                    return GetGreenChromeBrushes();
                case ChartColorPalette.LightCandy:
                    return GetLightCandyBrushes();
                case ChartColorPalette.Pineapple:
                    return GetPineappleBrushes();
                case ChartColorPalette.PurpleChrome:
                    return GetPurpleChromeBrushes();
                case ChartColorPalette.RedChrome:
                    return GetRedChromeBrushes();
                case ChartColorPalette.SandyBeach:
                    return GetSandyBeachBrushes();
#if WINDOWS_UAP
                case ChartColorPalette.TomatoSpectrum:
#else
                case ChartColorPalette.TomotoSpectrum:
#endif
                    return GetTomatoSpectrumBrushes();
                case ChartColorPalette.Custom:
                    return CustomBrushes;
                case ChartColorPalette.None:
                    return new List<Brush>();
            }

            return null;
        }

        /// <summary>
        /// Returns the brushes used for metro palette.
        /// </summary>
        /// <returns> Metro Brushes</returns>
        public List<Brush> GetMetroBrushes()
        {
            if (metroBrushes == null)
            {
                metroBrushes = new List<Brush>()
                {
                    new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xA0, 0x50, 0x00)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x99, 0x33)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0xC1, 0x39)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0x00, 0x73)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0x96, 0x09)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0x71, 0xB8)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x00, 0xFF)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0x14, 0x00)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAB, 0xA9))
                };
            }

            return metroBrushes;
        }

        /// <summary>
        /// Returns the brushes used for AutumnBrights palette.
        /// </summary>
        /// <returns>AutumnBrights Brushes</returns>
        public List<Brush> GetAutumnBrushes()
        {
            if (autumnBrightsBrushes == null)
            {
                autumnBrightsBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0xEF,0xF2,0xCB)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xFF,0xE4,0x63)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xFE,0xAA,0x0F)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xDB,0x64,0x0F)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x35,0x45,0x2B)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xC4,0xDB,0xAB)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x96,0x35,0x0a)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xfe,0xa0,0x33)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x77,0x9e,0x5c)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xfe,0xed,0x2b))

               };
            }
            return autumnBrightsBrushes;
        }

        /// <summary>
        /// Returns the brushes used for FloraHues palette.
        /// </summary>
        /// <returns>FloraHues Brushes</returns>
        public List<Brush> GetFloraHuesBrushes()
        {
            if (floraHuesBrushes == null)
            {
                floraHuesBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0xDD,0xDA,0xE8)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xEC,0x79,0xA8)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xA6,0x40,0x5A)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x46,0x4E,0x3E)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xFF,0x57,0x4C)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xFF,0xD7,0x91)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x7c,0x0a,0x28)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xe6,0x01,0x34)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x2f,0x2a,0x12)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xfa,0x9d,0x60))

               };
            }
            return floraHuesBrushes;
        }

        /// <summary>
        /// Returns the brushes used for Pineapple palette.
        /// </summary>
        /// <returns></returns>
        public List<Brush> GetPineappleBrushes()
        {
            if (pineappleBrushes == null)
            {
                pineappleBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0xC2,0xDF,0xDB)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x65,0x72,0x81)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x47,0x39,0x39)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x3E,0x56,0x3F)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x81,0xB5,0x64)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xDB,0xD7,0xA7)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x9f,0x5e,0x44)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x29,0x4e,0x11)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x72,0x3b,0x0e)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x6e,0x4a,0x42))

               };
            }
            return pineappleBrushes;
        }

        /// <summary>
        /// Returns the brushes used for TomatoSpectrum palette.
        /// </summary>
        /// <returns>TomatoSpectrum Brushes</returns>
        [Obsolete("Use GetTomatoSpectrumBrushes")]
        public List<Brush> GetTomotoSpectramBrushes()
        {
            if (tomatoSpectrumBrushes == null)
            {
                tomatoSpectrumBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0xE6,0xDC,0x96)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xB4,0xB5,0x65)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x57,0x6B,0x52)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x41,0x66,0x75)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xD9,0x6D,0x32)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xFD,0xA9,0x4E)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xa3,0x9f,0x10)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xf8,0x94,0x0b)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xdb,0xd5,0x4b)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xbd,0x49,0x16))

               };
            }
            return tomatoSpectrumBrushes;
        }


        /// <summary>
        /// Returns the brushes used for TomatoSpectrum palette.
        /// </summary>
        /// <returns>TomatoSpectrum Brushes</returns>
        public List<Brush> GetTomatoSpectrumBrushes()
        {
            if (tomatoSpectrumBrushes == null)
            {
                tomatoSpectrumBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0xE6,0xDC,0x96)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xB4,0xB5,0x65)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x57,0x6B,0x52)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x41,0x66,0x75)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xD9,0x6D,0x32)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xFD,0xA9,0x4E)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xa3,0x9f,0x10)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xf8,0x94,0x0b)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xdb,0xd5,0x4b)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xbd,0x49,0x16))

               };
            }
            return tomatoSpectrumBrushes;
        }

        /// <summary>
        /// Returns the brushes used for RedChrome palette.
        /// </summary>
        /// <returns>RedChrome Brushes</returns>
        public List<Brush> GetRedChromeBrushes()
        {
            if (redChromeBrushes == null)
            {
                redChromeBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0xb7,0x1c,0x1c)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xc6,0x28,0x28)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xd3,0x2f,0x2f)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xe5,0x39,0x35)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xf4,0x43,0x36)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xef,0x53,0x50)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xe5,0x73,0x73)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xef,0x9a,0x9a)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xff,0xcd,0xd2)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xde,0xdb,0xde))

               };
            }
            return redChromeBrushes;
        }

        /// <summary>
        /// Returns the brushes used for PurpleChrome palette.
        /// </summary>
        /// <returns>PurpleChrome Brushes</returns>
        public List<Brush> GetPurpleChromeBrushes()
        {
            if (purpleChromeBrushes == null)
            {
                purpleChromeBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0x4a,0x14,0x8c)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x6a,0x1b,0x9a)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x7b,0x1f,0xa2)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x8e,0x24,0xaa)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x9c,0x27,0xb0)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xab,0x47,0xbc)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xba,0x68,0xc8)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xce,0x93,0xd8)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xe1,0xbe,0xe7)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xF3,0xE5,0xF5))

               };
            }
            return purpleChromeBrushes;
        }

        /// <summary>
        /// Returns the brushes used for BlueChrome palette.
        /// </summary>
        /// <returns>BlueChrome Brushes</returns>
        public List<Brush> GetBlueChromeBrushes()
        {
            if (blueChromeBrushes == null)
            {
                blueChromeBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0x0d,0x47,0xa1)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x15,0x65,0xc0)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x19,0x76,0xd2)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x1e,0x88,0xe5)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x21,0x96,0xf3)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x42,0xa5,0xf5)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x64,0xb5,0xf6)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x90,0xca,0xf9)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xbb,0xde,0xfb)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xe0,0xf2,0xff))

               };
            }
            return blueChromeBrushes;
        }

        /// <summary>
        /// Returns the brushes used for GreenChrome palette.
        /// </summary>
        /// <returns>GreenChrome Brushes</returns>
        public List<Brush> GetGreenChromeBrushes()
        {
            if (greenChromeBrushes == null)
            {
                greenChromeBrushes = new List<Brush>()
               {
                   new SolidColorBrush (Color.FromArgb(0xFF,0x1b,0x5e,0x20)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x2e,0x7d,0x32)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x38,0x8e,0x3c)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x43,0xa0,0x47)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x4c,0xaf,0x50)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x66,0xbb,0x6a)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0x81,0xc7,0x84)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xa5,0xd6,0xa7)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xc8,0xe6,0xc9)),
                   new SolidColorBrush (Color.FromArgb(0xFF,0xe8,0xf5,0xe9))

               };
            }
            return greenChromeBrushes;
        }

        /// <summary>
        /// Returns the brushes used for Elite palette.
        /// </summary>
        /// <returns>Elite Brushes</returns>
        public List<Brush> GetEliteBrushes()
        {
            if (eliteBrushes == null)
            {
                eliteBrushes = new List<Brush>();

                LinearGradientBrush lineargradientbrush1 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop1 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf9, 0xb6, 0xdc), Offset = 0 };
                GradientStop stop2 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xea, 0x0e, 0x8c), Offset = 0.5 };
                lineargradientbrush1.GradientStops.Add(stop1);
                lineargradientbrush1.GradientStops.Add(stop2);
                eliteBrushes.Add(lineargradientbrush1);

                LinearGradientBrush lineargradientbrush2 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop3 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xb4, 0xde, 0xec), Offset = 0 };
                GradientStop stop4 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x06, 0x91, 0xc1), Offset = 0.5 };
                lineargradientbrush2.GradientStops.Add(stop3);
                lineargradientbrush2.GradientStops.Add(stop4);
                eliteBrushes.Add(lineargradientbrush2);

                LinearGradientBrush lineargradientbrush3 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop5 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xd3, 0xbd, 0xdb), Offset = 0 };
                GradientStop stop6 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x92, 0x5c, 0xa7), Offset = 0.5 };
                lineargradientbrush3.GradientStops.Add(stop5);
                lineargradientbrush3.GradientStops.Add(stop6);
                eliteBrushes.Add(lineargradientbrush3);

                LinearGradientBrush lineargradientbrush4 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop7 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xff, 0xf0, 0xb4), Offset = 0 };
                GradientStop stop8 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xfe, 0xcf, 0x0d), Offset = 0.5 };
                lineargradientbrush4.GradientStops.Add(stop7);
                lineargradientbrush4.GradientStops.Add(stop8);
                eliteBrushes.Add(lineargradientbrush4);

                LinearGradientBrush lineargradientbrush5 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop9 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xc1, 0xe4, 0xbb), Offset = 0 };
                GradientStop stop10 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x69, 0xbd, 0x5b), Offset = 0.5 };
                lineargradientbrush5.GradientStops.Add(stop9);
                lineargradientbrush5.GradientStops.Add(stop10);
                eliteBrushes.Add(lineargradientbrush5);

                LinearGradientBrush lineargradientbrush6 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop11 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf1, 0xb3, 0x74), Offset = 0 };
                GradientStop stop12 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf1, 0x5a, 0x24), Offset = 0.5 };
                lineargradientbrush6.GradientStops.Add(stop11);
                lineargradientbrush6.GradientStops.Add(stop12);
                eliteBrushes.Add(lineargradientbrush6);

                LinearGradientBrush lineargradientbrush7 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop13 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xee, 0xb8, 0xec), Offset = 0 };
                GradientStop stop14 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xdd, 0x74, 0xda), Offset = 0.5 };
                lineargradientbrush7.GradientStops.Add(stop13);
                lineargradientbrush7.GradientStops.Add(stop14);
                eliteBrushes.Add(lineargradientbrush7);

                LinearGradientBrush lineargradientbrush8 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop15 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xb2, 0xe3, 0xf6), Offset = 0 };
                GradientStop stop16 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x68, 0xc8, 0xed), Offset = 0.5 };
                lineargradientbrush8.GradientStops.Add(stop15);
                lineargradientbrush8.GradientStops.Add(stop16);
                eliteBrushes.Add(lineargradientbrush8);

                LinearGradientBrush lineargradientbrush9 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop17 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xe5, 0xe6, 0x9b), Offset = 0 };
                GradientStop stop18 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xcc, 0xcf, 0x3a), Offset = 0.5 };
                lineargradientbrush9.GradientStops.Add(stop17);
                lineargradientbrush9.GradientStops.Add(stop18);
                eliteBrushes.Add(lineargradientbrush9);

                LinearGradientBrush lineargradientbrush10 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop19 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xe1, 0x95, 0xb4), Offset = 0 };
                GradientStop stop20 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xc7, 0x36, 0x72), Offset = 0.5 };
                lineargradientbrush10.GradientStops.Add(stop19);
                lineargradientbrush10.GradientStops.Add(stop20);
                eliteBrushes.Add(lineargradientbrush10);
            }
            return eliteBrushes;
        }

        /// <summary>
        /// Returns the brushes used for SandyBeach palette.
        /// </summary>
        /// <returns>SandyBeach Brushes</returns>
        public List<Brush> GetSandyBeachBrushes()
        {
            if (sandyBeachBrushes == null)
            {
                sandyBeachBrushes = new List<Brush>();

                LinearGradientBrush lineargradientbrush1 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop1 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xff, 0xce, 0x80), Offset = 0 };
                GradientStop stop2 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xff, 0x9e, 0x02), Offset = 0.5 };
                lineargradientbrush1.GradientStops.Add(stop1);
                lineargradientbrush1.GradientStops.Add(stop2);
                sandyBeachBrushes.Add(lineargradientbrush1);

                LinearGradientBrush lineargradientbrush2 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop3 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xd2, 0xa5, 0x7f), Offset = 0 };
                GradientStop stop4 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xa7, 0x4e, 0x03), Offset = 0.5 };
                lineargradientbrush2.GradientStops.Add(stop3);
                lineargradientbrush2.GradientStops.Add(stop4);
                sandyBeachBrushes.Add(lineargradientbrush2);

                LinearGradientBrush lineargradientbrush3 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop5 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xdf, 0xb7, 0x7f), Offset = 0 };
                GradientStop stop6 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xc0, 0x70, 0x02), Offset = 0.5 };
                lineargradientbrush3.GradientStops.Add(stop5);
                lineargradientbrush3.GradientStops.Add(stop6);
                sandyBeachBrushes.Add(lineargradientbrush3);

                LinearGradientBrush lineargradientbrush4 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop7 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xeb, 0xc9, 0x8e), Offset = 0 };
                GradientStop stop8 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xd7, 0x94, 0x20), Offset = 0.5 };
                lineargradientbrush4.GradientStops.Add(stop7);
                lineargradientbrush4.GradientStops.Add(stop8);
                sandyBeachBrushes.Add(lineargradientbrush4);

                LinearGradientBrush lineargradientbrush5 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop9 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf8, 0xe1, 0xbf), Offset = 0 };
                GradientStop stop10 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf2, 0xc3, 0x80), Offset = 0.5 };
                lineargradientbrush5.GradientStops.Add(stop9);
                lineargradientbrush5.GradientStops.Add(stop10);
                sandyBeachBrushes.Add(lineargradientbrush5);

                LinearGradientBrush lineargradientbrush6 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop11 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf8, 0xe6, 0xcc), Offset = 0 };
                GradientStop stop12 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf2, 0xce, 0x9b), Offset = 0.5 };
                lineargradientbrush6.GradientStops.Add(stop11);
                lineargradientbrush6.GradientStops.Add(stop12);
                sandyBeachBrushes.Add(lineargradientbrush6);

                LinearGradientBrush lineargradientbrush7 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop13 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xdb, 0xec, 0xd9), Offset = 0 };
                GradientStop stop14 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xba, 0xdc, 0xb7), Offset = 0.5 };
                lineargradientbrush7.GradientStops.Add(stop13);
                lineargradientbrush7.GradientStops.Add(stop14);
                sandyBeachBrushes.Add(lineargradientbrush7);

                LinearGradientBrush lineargradientbrush8 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop15 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xd1, 0xe8, 0xb8), Offset = 0 };
                GradientStop stop16 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xa8, 0xd4, 0x77), Offset = 0.5 };
                lineargradientbrush8.GradientStops.Add(stop15);
                lineargradientbrush8.GradientStops.Add(stop16);
                sandyBeachBrushes.Add(lineargradientbrush8);

                LinearGradientBrush lineargradientbrush9 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop17 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf6, 0xe8, 0xaf), Offset = 0 };
                GradientStop stop18 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xed, 0xd4, 0x66), Offset = 0.5 };
                lineargradientbrush9.GradientStops.Add(stop17);
                lineargradientbrush9.GradientStops.Add(stop18);
                sandyBeachBrushes.Add(lineargradientbrush9);

                LinearGradientBrush lineargradientbrush10 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop19 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xfe, 0xc6, 0xae), Offset = 0 };
                GradientStop stop20 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xfd, 0x92, 0x64), Offset = 0.5 };
                lineargradientbrush10.GradientStops.Add(stop19);
                lineargradientbrush10.GradientStops.Add(stop20);
                sandyBeachBrushes.Add(lineargradientbrush10);
            }
            return sandyBeachBrushes;
        }

        /// <summary>
        /// Returns the brushes used for LightCandy palette.
        /// </summary>
        /// <returns>LightCandy Brushes</returns>
        public List<Brush> GetLightCandyBrushes()
        {
            if (lightCandyBrushes == null)
            {
                lightCandyBrushes = new List<Brush>();

                LinearGradientBrush lineargradientbrush1 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop1 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xb2, 0xdf, 0xc3), Offset = 0 };
                GradientStop stop2 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x66, 0xc0, 0x88), Offset = 0.5 };
                lineargradientbrush1.GradientStops.Add(stop1);
                lineargradientbrush1.GradientStops.Add(stop2);
                lightCandyBrushes.Add(lineargradientbrush1);

                LinearGradientBrush lineargradientbrush2 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop3 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xa3, 0xab, 0xb3), Offset = 0 };
                GradientStop stop4 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x4a, 0x5b, 0x69), Offset = 0.5 };
                lineargradientbrush2.GradientStops.Add(stop3);
                lineargradientbrush2.GradientStops.Add(stop4);
                lightCandyBrushes.Add(lineargradientbrush2);

                LinearGradientBrush lineargradientbrush3 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop5 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf7, 0xb0, 0xb0), Offset = 0 };
                GradientStop stop6 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xf0, 0x63, 0x62), Offset = 0.5 };
                lineargradientbrush3.GradientStops.Add(stop5);
                lineargradientbrush3.GradientStops.Add(stop6);
                lightCandyBrushes.Add(lineargradientbrush3);

                LinearGradientBrush lineargradientbrush4 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop7 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xa0, 0xe2, 0xf8), Offset = 0 };
                GradientStop stop8 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x43, 0xc5, 0xf1), Offset = 0.5 };
                lineargradientbrush4.GradientStops.Add(stop7);
                lineargradientbrush4.GradientStops.Add(stop8);
                lightCandyBrushes.Add(lineargradientbrush4);

                LinearGradientBrush lineargradientbrush5 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop9 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xe9, 0xb1, 0x8f), Offset = 0 };
                GradientStop stop10 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xd3, 0x65, 0x21), Offset = 0.5 };
                lineargradientbrush5.GradientStops.Add(stop9);
                lineargradientbrush5.GradientStops.Add(stop10);
                lightCandyBrushes.Add(lineargradientbrush5);

                LinearGradientBrush lineargradientbrush6 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop11 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xff, 0xea, 0x9f), Offset = 0 };
                GradientStop stop12 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xff, 0xd6, 0x41), Offset = 0.5 };
                lineargradientbrush6.GradientStops.Add(stop11);
                lineargradientbrush6.GradientStops.Add(stop12);
                lightCandyBrushes.Add(lineargradientbrush6);

                LinearGradientBrush lineargradientbrush7 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop13 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xb8, 0xb4, 0xd1), Offset = 0 };
                GradientStop stop14 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x78, 0x71, 0xa7), Offset = 0.5 };
                lineargradientbrush7.GradientStops.Add(stop13);
                lineargradientbrush7.GradientStops.Add(stop14);
                lightCandyBrushes.Add(lineargradientbrush7);

                LinearGradientBrush lineargradientbrush8 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop15 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x9b, 0xd1, 0xc7), Offset = 0 };
                GradientStop stop16 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x42, 0xa7, 0x95), Offset = 0.5 };
                lineargradientbrush8.GradientStops.Add(stop15);
                lineargradientbrush8.GradientStops.Add(stop16);
                lightCandyBrushes.Add(lineargradientbrush8);

                LinearGradientBrush lineargradientbrush9 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop17 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xca, 0xcb, 0xde), Offset = 0 };
                GradientStop stop18 = new GradientStop() { Color = Color.FromArgb(0xFF, 0x9a, 0x9c, 0xc0), Offset = 0.5 };
                lineargradientbrush9.GradientStops.Add(stop17);
                lineargradientbrush9.GradientStops.Add(stop18);
                lightCandyBrushes.Add(lineargradientbrush9);

                LinearGradientBrush lineargradientbrush10 = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
                GradientStop stop19 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xdd, 0xd5, 0xb4), Offset = 0 };
                GradientStop stop20 = new GradientStop() { Color = Color.FromArgb(0xFF, 0xbf, 0xaf, 0x72), Offset = 0.5 };
                lineargradientbrush10.GradientStops.Add(stop19);
                lineargradientbrush10.GradientStops.Add(stop20);
                lightCandyBrushes.Add(lineargradientbrush10);
            }
            return lightCandyBrushes;
        }

        /// <summary>
        /// Returns the brush at the specified index for current palette
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        /// <param name="colorIndex"></param>
        /// <returns></returns>
        public Brush GetBrush(int colorIndex)
        {
            if (this.currentBrushes != null && currentBrushes.Count > 0)
                return this.currentBrushes[colorIndex % currentBrushes.Count()];
            return new SolidColorBrush(Colors.Transparent);
        }

        #endregion
        
        #region Internal Methods

        internal void ApplyPalette(ChartColorPalette palette)
        {
            switch (palette)
            {
                case ChartColorPalette.Metro:
                    currentBrushes = GetMetroBrushes();
                    break;
                case ChartColorPalette.Custom:
                    currentBrushes = CustomBrushes;
                    break;
                case ChartColorPalette.AutumnBrights:
                    currentBrushes = GetAutumnBrushes();
                    break;
                case ChartColorPalette.FloraHues:
                    currentBrushes = GetFloraHuesBrushes();
                    break;
                case ChartColorPalette.Pineapple:
                    currentBrushes = GetPineappleBrushes();
                    break;
#if WINDOWS_UAP
                case ChartColorPalette.TomatoSpectrum:
#else
                case ChartColorPalette.TomotoSpectrum:
#endif
                    currentBrushes = GetTomatoSpectrumBrushes();
                    break;
                case ChartColorPalette.RedChrome:
                    currentBrushes = GetRedChromeBrushes();
                    break;
                case ChartColorPalette.PurpleChrome:
                    currentBrushes = GetPurpleChromeBrushes();
                    break;
                case ChartColorPalette.BlueChrome :
                    currentBrushes = GetBlueChromeBrushes();
                    break;
                case ChartColorPalette.GreenChrome:
                    currentBrushes = GetGreenChromeBrushes();
                    break;
                case ChartColorPalette.Elite:
                    currentBrushes = GetEliteBrushes();
                    break;
                case ChartColorPalette.SandyBeach:
                    currentBrushes = GetSandyBeachBrushes();
                    break;
                case ChartColorPalette.LightCandy:
                    currentBrushes = GetLightCandyBrushes();
                    break; 
            }
        }

        #endregion

        #endregion
    }
}
