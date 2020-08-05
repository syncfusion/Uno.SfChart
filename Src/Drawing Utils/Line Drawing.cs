using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.Storage.Streams;
#if WINDOWS_PHONE
using System.Windows.Media.Imaging;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains utility methods to draw shapes using WritableBitmap API's.
    /// </summary>
    public static partial class WriteableBitmapExtensions
    {

        #region general

        /// <summary>
        /// Slope Calculation Method
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double Slope(double x1, double y1, double x2, double y2)
        {
            return ((y2 - y1) / (x2 - x1));
        }

        /// <summary>
        /// Intersect Calculation Method(c = y- mx)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="slope"></param>
        /// <returns></returns>
        public static double Intersect(double x, double y, double slope)
        {
            return (y - slope * x);
        }

        #endregion

        #region DrawLine

        /// <summary>
        /// Method implementation for Clear values
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="stream"></param>
        /// <param name="pixels"></param>
        public static void Clear(this WriteableBitmap bmp, Stream stream, List<int> pixels)
        {
            foreach (int pixel in pixels)
            {
                stream.Position = pixel;
                stream.WriteByte(0);
                stream.WriteByte(0);
                stream.WriteByte(0);
                stream.WriteByte(0);
            }
        }

        /// <summary>
        /// Method implementation for Clear values
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="stream"></param>
        /// <param name="pixels"></param>
        public static void Clear(this WriteableBitmap bmp, Stream stream, byte[] pixels)
        {
            stream.Position = 0;
            stream.Write(pixels, 0, pixels.Count());
            stream.Position = 0;
        }

        /// <summary>
        /// Draws a colored line by connecting two points using the Bresenham algorithm.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="stream"></param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="uiColor">The color for the line.</param>
        /// <param name="pixels"></param>
        public static void DrawLineBresenham(this WriteableBitmap bmp, Stream stream, int x1, int y1, int x2, int y2, Color uiColor, List<int> pixels)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            //int[] pixels = new int[w * h];

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine sign for direction x
            int incx = 0;
            if (dx < 0)
            {
                dx = -dx;
                incx = -1;
            }
            else if (dx > 0)
            {
                incx = 1;
            }

            // Determine sign for direction y
            int incy = 0;
            if (dy < 0)
            {
                dy = -dy;
                incy = -1;
            }
            else if (dy > 0)
            {
                incy = 1;
            }

            // Which gradient is larger
            int pdx, pdy, odx, ody, es, el;
            if (dx > dy)
            {
                pdx = incx;
                pdy = 0;
                odx = incx;
                ody = incy;
                es = dy;
                el = dx;
            }
            else
            {
                pdx = 0;
                pdy = incy;
                odx = incx;
                ody = incy;
                es = dx;
                el = dy;
            }

            // Init start
            int x = x1;
            int y = y1;
            int error = el >> 1;
            if (y < h && y >= 0 && x < w && x >= 0)
            {
                //pixels[y * w + x] = color;
                int posInStream = (y * w + x) * 4;
                //pixels.Add(posInStream);
                stream.Position = posInStream;
                stream.WriteByte(uiColor.B);
                stream.WriteByte(uiColor.G);
                stream.WriteByte(uiColor.R);
                stream.WriteByte(uiColor.A);
            }

            // Walk the line!
            for (int i = 0; i < el; i++)
            {
                // Update error term
                error -= es;

                // Decide which coord to use
                if (error < 0)
                {
                    error += el;
                    x += odx;
                    y += ody;
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                // Set pixel
                if (y < h && y >= 0 && x < w && x >= 0)
                {
                    //pixels[y * w + x] = color;
                    int posInStream = (y * w + x) * 4;
                    //pixels.Add(posInStream);
                    stream.Position = posInStream;
                    stream.WriteByte(uiColor.B);
                    stream.WriteByte(uiColor.G);
                    stream.WriteByte(uiColor.R);
                    stream.WriteByte(uiColor.A);
                }
            }
        }

        /// <summary>
        /// Method implementation for DrawLine
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="leftThickness"></param>
        /// <param name="rightThickness"></param>
        /// <param name="uiColor"></param>
        public static void DrawLineBresenham(this WriteableBitmap bmp, byte[] buffer, int width,
           int height, int x1, int y1, int x2, int y2, Color uiColor, List<int> seriesPixels)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            int w = width;
            int h = height;
            //int[] pixels = new int[w * h];

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine sign for direction x
            int incx = 0;
            if (dx < 0)
            {
                dx = -dx;
                incx = -1;
            }
            else if (dx > 0)
            {
                incx = 1;
            }

            // Determine sign for direction y
            int incy = 0;
            if (dy < 0)
            {
                dy = -dy;
                incy = -1;
            }
            else if (dy > 0)
            {
                incy = 1;
            }

            // Which gradient is larger
            int pdx, pdy, odx, ody, es, el;
            if (dx > dy)
            {
                pdx = incx;
                pdy = 0;
                odx = incx;
                ody = incy;
                es = dy;
                el = dx;
            }
            else
            {
                pdx = 0;
                pdy = incy;
                odx = incx;
                ody = incy;
                es = dx;
                el = dy;
            }

            // Init start
            int x = x1;
            int y = y1;
            int error = el >> 1;
            if (y < h && y >= 0 && x < w && x >= 0)
            {
                int index = y * w + x;
                int posInStream = index * 4;

                buffer[posInStream] = uiColor.B;
                seriesPixels.Add(posInStream);

                buffer[++posInStream] = uiColor.G;
                seriesPixels.Add(posInStream);

                buffer[++posInStream] = uiColor.R;
                seriesPixels.Add(posInStream);

                buffer[++posInStream] = uiColor.A;
                seriesPixels.Add(posInStream);
            }

            // Walk the line!
            for (int i = 0; i < el; i++)
            {
                // Update error term
                error -= es;

                // Decide which coord to use
                if (error < 0)
                {
                    error += el;
                    x += odx;
                    y += ody;
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                // Set pixel
                if (y < h && y >= 0 && x < w && x >= 0)
                {
                    int index = y * w + x;
                    int posInStream = index * 4;

                    buffer[posInStream] = uiColor.B;
                    seriesPixels.Add(posInStream);

                    buffer[++posInStream] = uiColor.G;
                    seriesPixels.Add(posInStream);

                    buffer[++posInStream] = uiColor.R;
                    seriesPixels.Add(posInStream);

                    buffer[++posInStream] = uiColor.A;
                    seriesPixels.Add(posInStream);
                }
            }
        }

        /// <summary>
        /// Method implementation for DrawLine
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="uiColor"></param>
        /// <param name="seriesPixels"></param>
        /// <param name="clip"></param>
        public static void DrawLineBresenham(this WriteableBitmap bmp, byte[] buffer,int width, 
            int height, int x1, int y1, int x2, int y2, Color uiColor,List<int> seriesPixels,Rect clip )
        {
            // Use refs for faster access (really important!) speeds up a lot!
            int w = width;
            int h = height;
            int ct = (int)clip.Top;
            int cb = (int)clip.Bottom;
            int cl = (int)clip.Left;
            int cr = (int)clip.Right;
            //int[] pixels = new int[w * h];

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine sign for direction x
            int incx = 0;
            if (dx < 0)
            {
                dx = -dx;
                incx = -1;
            }
            else if (dx > 0)
            {
                incx = 1;
            }

            // Determine sign for direction y
            int incy = 0;
            if (dy < 0)
            {
                dy = -dy;
                incy = -1;
            }
            else if (dy > 0)
            {
                incy = 1;
            }

            // Which gradient is larger
            int pdx, pdy, odx, ody, es, el;
            if (dx > dy)
            {
                pdx = incx;
                pdy = 0;
                odx = incx;
                ody = incy;
                es = dy;
                el = dx;
            }
            else
            {
                pdx = 0;
                pdy = incy;
                odx = incx;
                ody = incy;
                es = dx;
                el = dy;
            }

            // Init start
            int x = x1;
            int y = y1;
            int error = el >> 1;
            if (y < h && y >= 0 && x < w && x >= 0 && y < cb && y >= ct && x < cr && x >= cl)
            {
                int index = y * w + x;
                int posInStream =  index * 4;

                buffer[posInStream] = uiColor.B;
                    seriesPixels.Add(posInStream);
               
                buffer[++posInStream] = uiColor.G;
                seriesPixels.Add(posInStream);
               
                buffer[++posInStream] = uiColor.R;
                seriesPixels.Add(posInStream);
               
                buffer[++posInStream] = uiColor.A;
                seriesPixels.Add(posInStream);
            }

            // Walk the line!
            for (int i = 0; i < el; i++)
            {
                // Update error term
                error -= es;

                // Decide which coord to use
                if (error < 0)
                {
                    error += el;
                    x += odx;
                    y += ody;
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                // Set pixel
                if (y < h && y >= 0 && x < w && x >= 0 && y < cb && y > ct && x < cr && x >= cl)
                {
                    int index = y * w + x;
                    int posInStream = index * 4;

                    buffer[posInStream] = uiColor.B;
                    seriesPixels.Add(posInStream);

                    buffer[++posInStream] = uiColor.G;
                    seriesPixels.Add(posInStream);

                    buffer[++posInStream] = uiColor.R;
                    seriesPixels.Add(posInStream);

                    buffer[++posInStream] = uiColor.A;
                    seriesPixels.Add(posInStream);
                }
            }
        }

        /// <summary>
        /// Method implementation for drawLine
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="buffer"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="uiColor"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        public static void DrawLine(this WriteableBitmap bmp, byte[] buffer, int x1, int y1, int x2, int y2, Color uiColor)
        {
            int pixelWidth = bmp.PixelWidth;
            int pixelHeight = bmp.PixelHeight;

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            const int PRECISION_SHIFT = 8;

            // Determine slope (absoulte value)
            int lenX, lenY;
            if (dy >= 0)
            {
                lenY = dy;
            }
            else
            {
                lenY = -dy;
            }

            if (dx >= 0)
            {
                lenX = dx;
            }
            else
            {
                lenX = -dx;
            }

            if (lenX > lenY)
            { // x increases by +/- 1
                if (dx < 0)
                {
                    int t = x1;
                    x1 = x2;
                    x2 = t;
                    t = y1;
                    y1 = y2;
                    y2 = t;
                }

                // Init steps and start
                int incy = (dy << PRECISION_SHIFT) / dx;

                int y1s = y1 << PRECISION_SHIFT;
                int y2s = y2 << PRECISION_SHIFT;
                int hs = pixelHeight << PRECISION_SHIFT;

                if (y1 < y2)
                {
                    if (y1 >= pixelHeight || y2 < 0)
                    {
                        return;
                    }
                    if (y1s < 0)
                    {
                        if (incy == 0)
                        {
                            return;
                        }
                        int oldy1s = y1s;
                        // Find lowest y1s that is greater or equal than 0.
                        y1s = incy - 1 + ((y1s + 1) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s >= hs)
                    {
                        if (incy != 0)
                        {
                            // Find highest y2s that is less or equal than ws - 1.
                            // y2s = y1s + n * incy. Find n.
                            y2s = hs - 1 - (hs - 1 - y1s) % incy;
                            x2 = x1 + (y2s - y1s) / incy;
                        }
                    }
                }
                else
                {
                    if (y2 >= pixelHeight || y1 < 0)
                    {
                        return;
                    }
                    if (y1s >= hs)
                    {
                        if (incy == 0)
                        {
                            return;
                        }
                        int oldy1s = y1s;
                        // Find highest y1s that is less or equal than ws - 1.
                        // y1s = oldy1s + n * incy. Find n.
                        y1s = hs - 1 + (incy - (hs - 1 - oldy1s) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s < 0)
                    {
                        if (incy != 0)
                        {
                            // Find lowest y2s that is greater or equal than 0.
                            // y2s = y1s + n * incy. Find n.
                            y2s = y1s % incy;
                            x2 = x1 + (y2s - y1s) / incy;
                        }
                    }
                }

                if (x1 < 0)
                {
                    y1s -= incy * x1;
                    x1 = 0;
                }
                if (x2 >= pixelWidth)
                {
                    x2 = pixelWidth - 1;
                }

                int ys = y1s;

                // Walk the line!
                int y = ys >> PRECISION_SHIFT;
                int previousY = y;
                int index = x1 + y * pixelWidth;
                int k = incy < 0 ? 1 - pixelWidth : 1 + pixelWidth;
                for (int x = x1; x <= x2; ++x)
                {
                    int posInStream = index * 4;
                    buffer[posInStream] = uiColor.B;
                    buffer[++posInStream] = uiColor.G;
                    buffer[++posInStream] = uiColor.R;
                    buffer[++posInStream] = uiColor.A;
                    //pixels[index] = color;
                    ys += incy;
                    y = ys >> PRECISION_SHIFT;
                    if (y != previousY)
                    {
                        previousY = y;
                        index += k;
                    }
                    else
                    {
                        ++index;
                    }
                }
            }
            else
            {
                // Prevent divison by zero
                if (lenY == 0)
                {
                    return;
                }
                if (dy < 0)
                {
                    int t = x1;
                    x1 = x2;
                    x2 = t;
                    t = y1;
                    y1 = y2;
                    y2 = t;
                }

                // Init steps and start
                int x1s = x1 << PRECISION_SHIFT;
                int x2s = x2 << PRECISION_SHIFT;
                int ws = pixelWidth << PRECISION_SHIFT;

                int incx = (dx << PRECISION_SHIFT) / dy;

                if (x1 < x2)
                {
                    if (x1 >= pixelWidth || x2 < 0)
                    {
                        return;
                    }
                    if (x1s < 0)
                    {
                        if (incx == 0)
                        {
                            return;
                        }
                        int oldx1s = x1s;
                        // Find lowest x1s that is greater or equal than 0.
                        x1s = incx - 1 + ((x1s + 1) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s >= ws)
                    {
                        if (incx != 0)
                        {
                            // Find highest x2s that is less or equal than ws - 1.
                            // x2s = x1s + n * incx. Find n.
                            x2s = ws - 1 - (ws - 1 - x1s) % incx;
                            y2 = y1 + (x2s - x1s) / incx;
                        }
                    }
                }
                else
                {
                    if (x2 >= pixelWidth || x1 < 0)
                    {
                        return;
                    }
                    if (x1s >= ws)
                    {
                        if (incx == 0)
                        {
                            return;
                        }
                        int oldx1s = x1s;
                        // Find highest x1s that is less or equal than ws - 1.
                        // x1s = oldx1s + n * incx. Find n.
                        x1s = ws - 1 + (incx - (ws - 1 - oldx1s) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s < 0)
                    {
                        if (incx != 0)
                        {
                            // Find lowest x2s that is greater or equal than 0.
                            // x2s = x1s + n * incx. Find n.
                            x2s = x1s % incx;
                            y2 = y1 + (x2s - x1s) / incx;
                        }
                    }
                }

                if (y1 < 0)
                {
                    x1s -= incx * y1;
                    y1 = 0;
                }
                if (y2 >= pixelHeight)
                {
                    y2 = pixelHeight - 1;
                }

                int index = x1s + ((y1 * pixelWidth) << PRECISION_SHIFT);

                // Walk the line!
                var inc = (pixelWidth << PRECISION_SHIFT) + incx;
                for (int y = y1; y <= y2; ++y)
                {
                    int posInStream = (index >> PRECISION_SHIFT) * 4;
                    buffer[posInStream] = uiColor.B;
                    buffer[++posInStream] = uiColor.G;
                    buffer[++posInStream] = uiColor.R;
                    buffer[++posInStream] = uiColor.A;
                    //pixels[index >> PRECISION_SHIFT] = color;
                    index += inc;
                }
            }
        }

        /// <summary>
        /// Draws a colored line by connecting two points using a DDA algorithm (Digital Differential Analyzer).
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="stream"></param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="uiColor">The color for the line.</param>
        public static void DrawLineDDA(this WriteableBitmap bmp, Stream stream, int x1, int y1, int x2, int y2, Color uiColor)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            //int[] pixels = new int[w * h];

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine slope (absoulte value)
            int len = dy >= 0 ? dy : -dy;
            int lenx = dx >= 0 ? dx : -dx;
            if (lenx > len)
            {
                len = lenx;
            }

            // Prevent divison by zero
            if (len != 0)
            {
                // Init steps and start
                float incx = dx / (float)len;
                float incy = dy / (float)len;
                float x = x1;
                float y = y1;

                // Walk the line!
                for (int i = 0; i < len; i++)
                {
                    if (y < h && y >= 0 && x < w && x >= 0)
                    {
                        //pixels[(int)y * w + (int)x] = color;
                        int posInStream = ((int)y * w + (int)x) * 4;
                        stream.Position = posInStream;
                        stream.WriteByte(uiColor.B);
                        stream.WriteByte(uiColor.G);
                        stream.WriteByte(uiColor.R);
                        stream.WriteByte(uiColor.A);
                    }
                    x += incx;
                    y += incy;
                }
            }
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="stream"></param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        public static void DrawLine(this WriteableBitmap bmp, Stream stream, int x1, int y1, int x2, int y2, Color color)
        {
            // Add one to use mul and cheap bit shift for multiplicaltion
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            bmp.DrawLine(stream, x1, y1, x2, y2, col, color);
        }

        /// <summary>
        /// Method implementation for Drawline
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="stream"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        /// <param name="uiColor"></param>
        public static void DrawLine(this WriteableBitmap bmp,Stream stream, int x1, int y1, int x2, int y2, int color, Color uiColor)
        {
            DrawLine(stream, bmp.PixelWidth, bmp.PixelHeight, x1, y1, x2, y2, uiColor);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA. 
        /// Uses the pixels array and the width directly for best performance.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="uiColor"></param>
        public static void DrawLine(Stream stream, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, Color uiColor)
        {
            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            const int PRECISION_SHIFT = 8;

            // Determine slope (absoulte value)
            int lenX, lenY;
            if (dy >= 0)
            {
                lenY = dy;
            }
            else
            {
                lenY = -dy;
            }

            if (dx >= 0)
            {
                lenX = dx;
            }
            else
            {
                lenX = -dx;
            }

            if (lenX > lenY)
            { // x increases by +/- 1
                if (dx < 0)
                {
                    int t = x1;
                    x1 = x2;
                    x2 = t;
                    t = y1;
                    y1 = y2;
                    y2 = t;
                }

                // Init steps and start
                int incy = (dy << PRECISION_SHIFT) / dx;

                int y1s = y1 << PRECISION_SHIFT;
                int y2s = y2 << PRECISION_SHIFT;
                int hs = pixelHeight << PRECISION_SHIFT;

                if (y1 < y2)
                {
                    if (y1 >= pixelHeight || y2 < 0)
                    {
                        return;
                    }
                    if (y1s < 0)
                    {
                        if (incy == 0)
                        {
                            return;
                        }
                        int oldy1s = y1s;
                        // Find lowest y1s that is greater or equal than 0.
                        y1s = incy - 1 + ((y1s + 1) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s >= hs)
                    {
                        if (incy != 0)
                        {
                            // Find highest y2s that is less or equal than ws - 1.
                            // y2s = y1s + n * incy. Find n.
                            y2s = hs - 1 - (hs - 1 - y1s) % incy;
                            x2 = x1 + (y2s - y1s) / incy;
                        }
                    }
                }
                else
                {
                    if (y2 >= pixelHeight || y1 < 0)
                    {
                        return;
                    }
                    if (y1s >= hs)
                    {
                        if (incy == 0)
                        {
                            return;
                        }
                        int oldy1s = y1s;
                        // Find highest y1s that is less or equal than ws - 1.
                        // y1s = oldy1s + n * incy. Find n.
                        y1s = hs - 1 + (incy - (hs - 1 - oldy1s) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s < 0)
                    {
                        if (incy != 0)
                        {
                            // Find lowest y2s that is greater or equal than 0.
                            // y2s = y1s + n * incy. Find n.
                            y2s = y1s % incy;
                            x2 = x1 + (y2s - y1s) / incy;
                        }
                    }
                }

                if (x1 < 0)
                {
                    y1s -= incy * x1;
                    x1 = 0;
                }
                if (x2 >= pixelWidth)
                {
                    x2 = pixelWidth - 1;
                }

                int ys = y1s;

                // Walk the line!
                int y = ys >> PRECISION_SHIFT;
                int previousY = y;
                int index = x1 + y * pixelWidth;
                int k = incy < 0 ? 1 - pixelWidth : 1 + pixelWidth;
                for (int x = x1; x <= x2; ++x)
                {
                    int posInStream = index * 4;
                    stream.Position = posInStream;
                    stream.WriteByte(uiColor.B);
                    stream.WriteByte(uiColor.G);
                    stream.WriteByte(uiColor.R);
                    stream.WriteByte(uiColor.A);
                    //pixels[index] = color;
                    ys += incy;
                    y = ys >> PRECISION_SHIFT;
                    if (y != previousY)
                    {
                        previousY = y;
                        index += k;
                    }
                    else
                    {
                        ++index;
                    }
                }
            }
            else
            {
                // Prevent divison by zero
                if (lenY == 0)
                {
                    return;
                }
                if (dy < 0)
                {
                    int t = x1;
                    x1 = x2;
                    x2 = t;
                    t = y1;
                    y1 = y2;
                    y2 = t;
                }

                // Init steps and start
                int x1s = x1 << PRECISION_SHIFT;
                int x2s = x2 << PRECISION_SHIFT;
                int ws = pixelWidth << PRECISION_SHIFT;

                int incx = (dx << PRECISION_SHIFT) / dy;

                if (x1 < x2)
                {
                    if (x1 >= pixelWidth || x2 < 0)
                    {
                        return;
                    }
                    if (x1s < 0)
                    {
                        if (incx == 0)
                        {
                            return;
                        }
                        int oldx1s = x1s;
                        // Find lowest x1s that is greater or equal than 0.
                        x1s = incx - 1 + ((x1s + 1) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s >= ws)
                    {
                        if (incx != 0)
                        {
                            // Find highest x2s that is less or equal than ws - 1.
                            // x2s = x1s + n * incx. Find n.
                            x2s = ws - 1 - (ws - 1 - x1s) % incx;
                            y2 = y1 + (x2s - x1s) / incx;
                        }
                    }
                }
                else
                {
                    if (x2 >= pixelWidth || x1 < 0)
                    {
                        return;
                    }
                    if (x1s >= ws)
                    {
                        if (incx == 0)
                        {
                            return;
                        }
                        int oldx1s = x1s;
                        // Find highest x1s that is less or equal than ws - 1.
                        // x1s = oldx1s + n * incx. Find n.
                        x1s = ws - 1 + (incx - (ws - 1 - oldx1s) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s < 0)
                    {
                        if (incx != 0)
                        {
                            // Find lowest x2s that is greater or equal than 0.
                            // x2s = x1s + n * incx. Find n.
                            x2s = x1s % incx;
                            y2 = y1 + (x2s - x1s) / incx;
                        }
                    }
                }

                if (y1 < 0)
                {
                    x1s -= incx * y1;
                    y1 = 0;
                }
                if (y2 >= pixelHeight)
                {
                    y2 = pixelHeight - 1;
                }

                int index = x1s + ((y1 * pixelWidth) << PRECISION_SHIFT);

                // Walk the line!
                var inc = (pixelWidth << PRECISION_SHIFT) + incx;
                for (int y = y1; y <= y2; ++y)
                {
                    int posInStream = (index >> PRECISION_SHIFT) * 4;
                    stream.Position = posInStream;
                    stream.WriteByte(uiColor.B);
                    stream.WriteByte(uiColor.G);
                    stream.WriteByte(uiColor.R);
                    stream.WriteByte(uiColor.A);
                    //pixels[index >> PRECISION_SHIFT] = color;
                    index += inc;
                }
            }
        }

        #endregion

        #region DrawLine Anti-aliased

        /// <summary> 
        /// Draws an anti-aliased line, using an optimized version of Gupta-Sproull algorithm 
        /// From http://nokola.com/blog/post/2010/10/14/Anti-aliased-Lines-And-Optimizing-Code-for-Windows-Phone-7e28093First-Look.aspx
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="stream"></param>
        /// <param name="pixels"></param>
        /// </summary> 
        public static void DrawLineAa(this WriteableBitmap bmp, Stream stream, int x1, int y1, int x2, int y2, Color color, List<int> pixels)
        {
            // Add one to use mul and cheap bit shift for multiplicaltion
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            bmp.DrawLineAa(stream, x1, y1, x2, y2, col, pixels, color);
        }

        /// <summary> 
        /// Draws an anti-aliased line, using an optimized version of Gupta-Sproull algorithm 
        /// From http://nokola.com/blog/post/2010/10/14/Anti-aliased-Lines-And-Optimizing-Code-for-Windows-Phone-7e28093First-Look.aspx
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
       /// <param name="pixels"></param>
        /// <param name="stream"></param>
        /// <param name="uiColor"></param>
        /// </summary> 
        public static void DrawLineAa(this WriteableBitmap bmp, Stream stream, int x1, int y1, int x2, int y2, int color, List<int> pixels, Color uiColor)
        {
            //DrawLineAa(stream, bmp.PixelWidth, bmp.PixelHeight, x1, y1, x2, y2, color, pixels, uiColor);
        }

        /// <summary> 
        /// Draws an anti-aliased line, using an optimized version of Gupta-Sproull algorithm 
        /// From http://nokola.com/blog/post/2010/10/14/Anti-aliased-Lines-And-Optimizing-Code-for-Windows-Phone-7e28093First-Look.aspx
        /// <param name="pixels">An array containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// </summary> 
        public static void DrawLineAa(int[] pixels, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color)
        {
            if ((x1 == x2) && (y1 == y2)) return; // edge case causing invDFloat to overflow, found by Shai Rubinshtein

            if (x1 < 1) x1 = 1;
            if (x1 > pixelWidth - 2) x1 = pixelWidth - 2;
            if (y1 < 1) y1 = 1;
            if (y1 > pixelHeight - 2) y1 = pixelHeight - 2;

            if (x2 < 1) x2 = 1;
            if (x2 > pixelWidth - 2) x2 = pixelWidth - 2;
            if (y2 < 1) y2 = 1;
            if (y2 > pixelHeight - 2) y2 = pixelHeight - 2;

            var addr = y1 * pixelWidth + x1;
            var dx = x2 - x1;
            var dy = y2 - y1;

            int du;
            int dv;
            int u;
            int v;
            int uincr;
            int vincr;

            // Extract color
            var a = (color >> 24) & 0xFF;
            var srb = (uint)(color & 0x00FF00FF);
            var sg = (uint)((color >> 8) & 0xFF);

            // By switching to (u,v), we combine all eight octants 
            int adx = dx, ady = dy;
            if (dx < 0) adx = -dx;
            if (dy < 0) ady = -dy;

            if (adx > ady)
            {
                du = adx;
                dv = ady;
                u = x2;
                v = y2;
                uincr = 1;
                vincr = pixelWidth;
                if (dx < 0) uincr = -uincr;
                if (dy < 0) vincr = -vincr;
            }
            else
            {
                du = ady;
                dv = adx;
                u = y2;
                v = x2;
                uincr = pixelWidth;
                vincr = 1;
                if (dy < 0) uincr = -uincr;
                if (dx < 0) vincr = -vincr;
            }

            var uend = u + du;
            var d = (dv << 1) - du;        // Initial value as in Bresenham's 
            var incrS = dv << 1;    // &#916;d for straight increments 
            var incrD = (dv - du) << 1;    // &#916;d for diagonal increments

            var invDFloat = 1.0 / (4.0 * Math.Sqrt(du * du + dv * dv));   // Precomputed inverse denominator 
            var invD2DuFloat = 0.75 - 2.0 * (du * invDFloat);   // Precomputed constant

            const int PRECISION_SHIFT = 10; // result distance should be from 0 to 1 << PRECISION_SHIFT, mapping to a range of 0..1 
            const int PRECISION_MULTIPLIER = 1 << PRECISION_SHIFT;
            var invD = (int)(invDFloat * PRECISION_MULTIPLIER);
            var invD2Du = (int)(invD2DuFloat * PRECISION_MULTIPLIER * a);
            var zeroDot75 = (int)(0.75 * PRECISION_MULTIPLIER * a);

            var invDMulAlpha = invD * a;
            var duMulInvD = du * invDMulAlpha; // used to help optimize twovdu * invD 
            var dMulInvD = d * invDMulAlpha; // used to help optimize twovdu * invD 
            //int twovdu = 0;    // Numerator of distance; starts at 0 
            var twovduMulInvD = 0; // since twovdu == 0 
            var incrSMulInvD = incrS * invDMulAlpha;
            var incrDMulInvD = incrD * invDMulAlpha;

            do
            {
                AlphaBlendNormalOnPremultiplied(pixels, addr, (zeroDot75 - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                AlphaBlendNormalOnPremultiplied(pixels, addr + vincr, (invD2Du + twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                AlphaBlendNormalOnPremultiplied(pixels, addr - vincr, (invD2Du - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);

                if (d < 0)
                {
                    // choose straight (u direction) 
                    twovduMulInvD = dMulInvD + duMulInvD;
                    d += incrS;
                    dMulInvD += incrSMulInvD;
                }
                else
                {
                    // choose diagonal (u+v direction) 
                    twovduMulInvD = dMulInvD - duMulInvD;
                    d += incrD;
                    dMulInvD += incrDMulInvD;
                    v++;
                    addr += vincr;
                }
                u++;
                addr += uincr;
            } while (u < uend);
        }

        /// <summary>
        /// Method implementation for DrawLine
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="pixels"></param>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="uiColor"></param>
        /// <param name="leftThickness"></param>
        /// <param name="rightThickness"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        public static void DrawLineAa(this WriteableBitmap bmp, byte[] pixels, int pixelWidth, int pixelHeight, int x1, int y1,
            int x2, int y2, Color uiColor,List<int> seriesPixels)
        {
            if (x1 == 0 && x2 == 0) return;
            if ((x1 == x2) && (y1 == y2)) return; // edge case causing invDFloat to overflow, found by Shai Rubinshtein

            if (y1 < 0 && y2 < 0) return;

            if (y1 > pixelHeight && y2 > pixelHeight) return;

            var alp = uiColor.A + 1;
            var color = (uiColor.A << 24)
                     | ((byte)((uiColor.R * alp) >> 8) << 16)
                     | ((byte)((uiColor.G * alp) >> 8) << 8)
                     | ((byte)((uiColor.B * alp) >> 8));

            double sy1 = y1;
            double sy2 = y2;
            double sx1 = x1;
            double sx2 = x2;

            double slope;

            double intersect;
            if (y2 < 0)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx2, sy2, slope);
                    x2 = (int)((-intersect) / slope);
                }
                y2 = 0;
            }
            if (y1 < 0)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx1, sy1, slope);
                    x1 = (int)((-intersect) / slope);
                }
                y1 = 0;
            }
            if (x1 < 0)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx1, sy1, slope);
                    y1 = (int)(intersect);
                }
                x1 = 0;
            }

            if (x2 < 0)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx2, sy2, slope);
                    y2 = (int)(intersect);
                }
                x2 = 0;
            }
            if (x1 > pixelWidth)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx1, sy1, slope);
                    y1 = (int)(slope * (pixelWidth) + intersect);
                }
                x1 = pixelWidth;
            }
            if (x2 > pixelWidth)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx2, sy2, slope);
                    y2 = (int)(slope * (pixelWidth) + intersect);
                }
                x2 = pixelWidth;
            }


            if (y1 > pixelHeight)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx1, sy1, slope);
                    x1 = (int)(((pixelHeight) - intersect) / slope);
                }
                y1 = pixelHeight;
            }
            if (y2 > pixelHeight)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx2, sy2, slope);
                    x2 = (int)(((pixelHeight) - intersect) / slope);
                }
                y2 = pixelHeight;
            }
            var addValue = y1 * pixelWidth + x1;
            var addr = addValue >= 0 ? addValue : 1;
            var dx = x2 - x1;
            var dy = y2 - y1;

            int du;
            int dv;
            int u;
            int v;
            int uincr;
            int vincr;

            // Extract color
            var a = (color >> 24) & 0xFF;
            var srb = (uint)(color & 0x00FF00FF);
            var sg = (uint)((color >> 8) & 0xFF);

            // By switching to (u,v), we combine all eight octants 
            int adx = dx, ady = dy;
            if (dx < 0) adx = -dx;
            if (dy < 0) ady = -dy;

            if (adx > ady)
            {
                du = adx;
                dv = ady;
                u = x2;
                v = y2;
                uincr = 1;
                vincr = pixelWidth;
                if (dx < 0) uincr = -uincr;
                if (dy < 0) vincr = -vincr;
            }
            else
            {
                du = ady;
                dv = adx;
                u = y2;
                v = x2;
                uincr = pixelWidth;
                vincr = 1;
                if (dy < 0) uincr = -uincr;
                if (dx < 0) vincr = -vincr;
            }

            var uend = u + du;
            var d = (dv << 1) - du;        // Initial value as in Bresenham's 
            var incrS = dv << 1;    // &#916;d for straight increments 
            var incrD = (dv - du) << 1;    // &#916;d for diagonal increments

            var invDFloat = 1.0 / (4.0 * Math.Sqrt(du * du + dv * dv));   // Precomputed inverse denominator 
            var invD2DuFloat = 0.75 - 2.0 * (du * invDFloat);   // Precomputed constant

            const int PRECISION_SHIFT = 10; // result distance should be from 0 to 1 << PRECISION_SHIFT, mapping to a range of 0..1 
            const int PRECISION_MULTIPLIER = 1 << PRECISION_SHIFT;
            var invD = (int)(invDFloat * PRECISION_MULTIPLIER);
            var invD2Du = (int)(invD2DuFloat * PRECISION_MULTIPLIER * a);
            var zeroDot75 = (int)(0.75 * PRECISION_MULTIPLIER * a);

            var invDMulAlpha = invD * a;
            var duMulInvD = du * invDMulAlpha; // used to help optimize twovdu * invD 
            var dMulInvD = d * invDMulAlpha; // used to help optimize twovdu * invD 
            //int twovdu = 0;    // Numerator of distance; starts at 0 
            var twovduMulInvD = 0; // since twovdu == 0 
            var incrSMulInvD = incrS * invDMulAlpha;
            var incrDMulInvD = incrD * invDMulAlpha;

            do
            {
                var addrPlus = (addr + vincr) >= 0 ? (addr + vincr) : 0;
                var addrMinus = (addr - vincr) >= 0 ? (addr - vincr) : 0;
                AlphaBlendNormalOnPremultiplied(pixels, addrPlus, (invD2Du + twovduMulInvD) >> PRECISION_SHIFT, srb, sg, false, uiColor, seriesPixels);
                AlphaBlendNormalOnPremultiplied(pixels, addr, (zeroDot75 - twovduMulInvD) >> PRECISION_SHIFT, srb, sg, false, uiColor, seriesPixels);
                AlphaBlendNormalOnPremultiplied(pixels, addrMinus, (invD2Du - twovduMulInvD) >> PRECISION_SHIFT, srb, sg, false, uiColor, seriesPixels);

                if (d < 0)
                {
                    // choose straight (u direction) 
                    twovduMulInvD = dMulInvD + duMulInvD;
                    d += incrS;
                    dMulInvD += incrSMulInvD;
                }
                else
                {
                    // choose diagonal (u+v direction) 
                    twovduMulInvD = dMulInvD - duMulInvD;
                    d += incrD;
                    dMulInvD += incrDMulInvD;
                    v++;
                    addr += vincr;
                }
                u++;
                addr += uincr;
            } while (u < uend);
        }

        /// <summary>
        /// Draws an anti-aliased line
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="pixels"></param>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="uiColor"></param>
        /// <param name="seriesPixels"></param>
        /// <param name="clip"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        public static void DrawLineAa(this WriteableBitmap bmp, byte[] pixels, int pixelWidth, int pixelHeight, int x1, int y1,
            int x2, int y2, Color uiColor,List<int> seriesPixels,Rect clip)
        {
            var ct = (int)clip.Top;
            var cb = (int)clip.Bottom;
            var cl = (int)clip.Left;
            var cr = (int)clip.Right;
            if (x1 == 0 && x2 == 0) return;
            if ((x1 == x2) && (y1 == y2)) return; // edge case causing invDFloat to overflow, found by Shai Rubinshtein

            if (y1 < 0 && y2 < 0) return;

            if (y1 > pixelHeight && y2 > pixelHeight) return;

            var alp = uiColor.A + 1;
            var color = (uiColor.A << 24)
                     | ((byte)((uiColor.R * alp) >> 8) << 16)
                     | ((byte)((uiColor.G * alp) >> 8) << 8)
                     | ((byte)((uiColor.B * alp) >> 8));

            double sy1 = y1;
            double sy2 = y2;
            double sx1 = x1;
            double sx2 = x2;

            double slope;

            double intersect;
            if (y2 < 0)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx2, sy2, slope);
                    x2 = (int)((-intersect) / slope);
                }
                y2 = 0;
            }
            if (y1 < 0)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx1, sy1, slope);
                    x1 = (int)((-intersect) / slope);
                }
                y1 = 0;
            }
            if (x1 < 0)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx1, sy1, slope);
                    y1 = (int)(intersect);
                }
                x1 = 0;
            }

            if (x2 < 0)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx2, sy2, slope);
                    y2 = (int)(intersect);
                }
                x2 = 0;
            }
            if (x1 > pixelWidth)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx1, sy1, slope);
                    y1 = (int)(slope * (pixelWidth) + intersect);
                }
                x1 = pixelWidth;
            }
            if (x2 > pixelWidth)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx2, sy2, slope);
                    y2 = (int)(slope * (pixelWidth) + intersect);
                }
                x2 = pixelWidth;
            }


            if (y1 > pixelHeight)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx1, sy1, slope);
                    x1 = (int)(((pixelHeight) - intersect) / slope);
                }
                y1 = pixelHeight;
            }
            if (y2 > pixelHeight)
            {
                slope = Slope(sx1, sy1, sx2, sy2);
                if (!double.IsInfinity(slope))
                {
                    intersect = Intersect(sx2, sy2, slope);
                    x2 = (int)(((pixelHeight) - intersect) / slope);
                }
                y2 = pixelHeight;
            }
            if (y1 > cb)
                y1 = cb;
            if (y2 < ct)
                y2 = ct;
            if (y1 < ct)
                y1 = ct;
            if (y2 > cb)
                y2 = cb;

            if (x1 > cr)
                x1 = cr;
            if (x1 < cl)
                x1 = cl;
            if (x2 < cl)
                x2 = cl;
            if (x2 > cr)
                x2 = cr;
            var addValue = y1 * pixelWidth + x1;
            var addr = addValue >= 0 ? addValue : 1;
            var dx = x2 - x1;
            var dy = y2 - y1;

            int du;
            int dv;
            int u;
            int v;
            int uincr;
            int vincr;

            // Extract color
            var a = (color >> 24) & 0xFF;
            var srb = (uint)(color & 0x00FF00FF);
            var sg = (uint)((color >> 8) & 0xFF);

            // By switching to (u,v), we combine all eight octants 
            int adx = dx, ady = dy;
            if (dx < 0) adx = -dx;
            if (dy < 0) ady = -dy;

            if (adx > ady)
            {
                du = adx;
                dv = ady;
                u = x2;
                v = y2;
                uincr = 1;
                vincr = pixelWidth;
                if (dx < 0) uincr = -uincr;
                if (dy < 0) vincr = -vincr;
            }
            else
            {
                du = ady;
                dv = adx;
                u = y2;
                v = x2;
                uincr = pixelWidth;
                vincr = 1;
                if (dy < 0) uincr = -uincr;
                if (dx < 0) vincr = -vincr;
            }

            var uend = u + du;
            var d = (dv << 1) - du;        // Initial value as in Bresenham's 
            var incrS = dv << 1;    // &#916;d for straight increments 
            var incrD = (dv - du) << 1;    // &#916;d for diagonal increments

            var invDFloat = 1.0 / (4.0 * Math.Sqrt(du * du + dv * dv));   // Precomputed inverse denominator 
            var invD2DuFloat = 0.75 - 2.0 * (du * invDFloat);   // Precomputed constant

            const int PRECISION_SHIFT = 10; // result distance should be from 0 to 1 << PRECISION_SHIFT, mapping to a range of 0..1 
            const int PRECISION_MULTIPLIER = 1 << PRECISION_SHIFT;
            var invD = (int)(invDFloat * PRECISION_MULTIPLIER);
            var invD2Du = (int)(invD2DuFloat * PRECISION_MULTIPLIER * a);
            var zeroDot75 = (int)(0.75 * PRECISION_MULTIPLIER * a);

            var invDMulAlpha = invD * a;
            var duMulInvD = du * invDMulAlpha; // used to help optimize twovdu * invD 
            var dMulInvD = d * invDMulAlpha; // used to help optimize twovdu * invD 
            //int twovdu = 0;    // Numerator of distance; starts at 0 
            var twovduMulInvD = 0; // since twovdu == 0 
            var incrSMulInvD = incrS * invDMulAlpha;
            var incrDMulInvD = incrD * invDMulAlpha;

            do
            {
                var addrPlus = (addr + vincr) >= 0 ? (addr + vincr) : 0;
                var addrMinus = (addr - vincr) >= 0 ? (addr - vincr) : 0;
                AlphaBlendNormalOnPremultiplied(pixels, addrPlus, (invD2Du + twovduMulInvD) >> PRECISION_SHIFT, srb, sg, false, uiColor,seriesPixels);
                AlphaBlendNormalOnPremultiplied(pixels, addr, (zeroDot75 - twovduMulInvD) >> PRECISION_SHIFT, srb, sg, false, uiColor,seriesPixels);
                AlphaBlendNormalOnPremultiplied(pixels, addrMinus, (invD2Du - twovduMulInvD) >> PRECISION_SHIFT, srb, sg, false, uiColor,seriesPixels);

                if (d < 0)
                {
                    // choose straight (u direction) 
                    twovduMulInvD = dMulInvD + duMulInvD;
                    d += incrS;
                    dMulInvD += incrSMulInvD;
                }
                else
                {
                    // choose diagonal (u+v direction) 
                    twovduMulInvD = dMulInvD - duMulInvD;
                    d += incrD;
                    dMulInvD += incrDMulInvD;
                    v++;
                    addr += vincr;
                }
                u++;
                addr += uincr;
            } while (u < uend);
        }

        /// <summary> 
        /// Blends a specific source color on top of a destination premultiplied color 
        /// </summary> 
        /// <param name="pixels">Array containing destination color</param> 
        /// <param name="index">Index of destination pixel</param> 
        /// <param name="sa">Source alpha (0..255)</param> 
        /// <param name="srb">Source non-premultiplied red and blue component in the format 0x00rr00bb</param> 
        /// <param name="sg">Source green component (0..255)</param> 
        private static void AlphaBlendNormalOnPremultiplied(int[] pixels, int index, int sa, uint srb, uint sg)
        {
            var destPixel = (uint)pixels[index];

            var da = (destPixel >> 24);
            var dg = ((destPixel >> 8) & 0xff);
            var drb = destPixel & 0x00FF00FF;

            // blend with high-quality alpha and lower quality but faster 1-off RGBs 
            pixels[index] = (int)(
               ((sa + ((da * (255 - sa) * 0x8081) >> 23)) << 24) | // aplha 
               (((sg - dg) * sa + (dg << 8)) & 0xFFFFFF00) | // green 
               (((((srb - drb) * sa) >> 8) + drb) & 0x00FF00FF) // red and blue 
            );
        }

        /// <summary> 
        /// Blends a specific source color on top of a destination premultiplied color 
        /// </summary> 
        /// <param name="buffer">Array containing destination color</param> 
        /// <param name="index">Index of destination pixel</param> 
        /// <param name="sa">Source alpha (0..255)</param> 
        /// <param name="srb">Source non-premultiplied red and blue component in the format 0x00rr00bb</param> 
        /// <param name="sg">Source green component (0..255)</param>
        /// <param name="isOpaque"></param>
        /// <param name="uiColor"></param> 
        private static void AlphaBlendNormalOnPremultiplied(byte[] buffer, int index, int sa, uint srb, uint sg, bool isOpaque,
            Color uiColor,List<int> seriesPixels)
        {
            index *= 4;

            if (buffer.Length <= index + 3 || index < 0) return;

            int col = ConvertColor(buffer[index], buffer[index + 1], buffer[index + 2], buffer[index + 3]);

            var destPixel = (uint)col;

            var da = (destPixel >> 24);
            var dg = ((destPixel >> 8) & 0xff);
            var drb = destPixel & 0x00FF00FF;

            int color = (int)(
               ((sa + ((da * (255 - sa) * 0x8081) >> 23)) << 24) | // aplha 
               (((sg - dg) * sa + (dg << 8)) & 0x0000FF00) | // green 
               (((((srb - drb) * sa) >> 8) + drb) & 0x00FF00FF) // red and blue 
               );

            if (isOpaque)
            {
                int position = index + 3;
                buffer[position] = 255;
                seriesPixels.Add(position);

                position = index + 2;
                buffer[position] = uiColor.R;
                seriesPixels.Add(position);

                position = index + 1;
                buffer[position] = uiColor.G;
                seriesPixels.Add(position);

                buffer[index] = uiColor.B;
                seriesPixels.Add(position);
            }
            else
            {
                int position = index + 3;
                byte colorCode = (byte)((color >> 24) & 0xff);
                buffer[position] = colorCode;
                seriesPixels.Add(position);

                position = index + 2;
                colorCode = (byte)((color >> 16) & 0xff);
                buffer[position] = colorCode;
                seriesPixels.Add(position);

                position = index + 1;
                colorCode = (byte)((color >> 8) & 0xff);
                buffer[position] = colorCode;
                seriesPixels.Add(position);

                colorCode = (byte)((color) & 0xff);
                buffer[index] = (byte)((color) & 0xff);
                seriesPixels.Add(position);
            }
        }

        #endregion

        #region Rectangle

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        private static int ConvertColor(Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            return col;
        }

        private static int ConvertColor(byte b, byte g, byte r, byte a)
        {
            var alp = a + 1;
            var col = (a << 24)
                     | ((byte)((r * alp) >> 8) << 16)
                     | ((byte)((g * alp) >> 8) << 8)
                     | ((byte)((b * alp) >> 8));
            return col;
        }

        /// <summary>
        /// Draws a filled rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="height"></param>
        /// <param name="x1">The x-coordinate of the bounding rectangle's left side.</param>
        /// <param name="y1">The y-coordinate of the bounding rectangle's top side.</param>
        /// <param name="x2">The x-coordinate of the bounding rectangle's right side.</param>
        /// <param name="y2">The y-coordinate of the bounding rectangle's bottom side.</param>
        /// <param name="color">The color.</param>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        public static void FillRectangle(this WriteableBitmap bmp, byte[] buffer, int width, int height, int x1, int y1, int x2, int y2,
            Color color,List<int> seriesPixels)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            var w = width;
            var h = height;

            // Check boundaries
            if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)
             || (x1 >= w && x2 >= w) || (y1 >= h && y2 >= h))
            {
                return;
            }

            // Clamp boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 > w) { x1 = w; }
            if (y1 > h) { y1 = h; }
            if (x2 > w) { x2 = w; }
            if (y2 > h) { y2 = h; }

            // Fill first line
            var startY = y1 * w;
            var incrementOffset = w * 4;
            var startYPlusX1 = (startY + x1) * 4;
            var endOffset = (startY + x2) * 4;
            if (startYPlusX1 == endOffset)
                endOffset += 4;        
            int position = 0;
            for (var x = startYPlusX1; x < endOffset; x += 4)
            {
                position = x;
                buffer[position] = color.B;
                seriesPixels.Add(position);

                position = x + 1;
                buffer[position] = color.G;
                seriesPixels.Add(position);

                position = x + 2;
                buffer[position] = color.R;
                seriesPixels.Add(position);

                position = x + 3;
                buffer[position] = color.A;
                seriesPixels.Add(position);

            }

            // Copy first line
            var len = (x2 > x1) ? (x2 - x1) * 4 : (x1 - x2) * 4;//Argument out of range exception is thrown in FastHiloOpenClose series when using logarithmic axis dynamically-WRT-4869
            if (len == 0)
                len = 4;
            var offset2 = ((y2 - 1) * w + x1) * 4;
            for (var y = startYPlusX1 + incrementOffset; y <= offset2; y += incrementOffset)
            {
                System.Buffer.BlockCopy(buffer, startYPlusX1, buffer, y, len);

                for (int i = y; i < y+len; i++)
                {
                     seriesPixels.Add(i);
                }
            }
           
        }

        /// <summary>
        /// Draws a rectangle.
        /// x2 has to be greater than x1 and y2 has to be greater than y1.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the bounding rectangle's left side.</param>
        /// <param name="y1">The y-coordinate of the bounding rectangle's top side.</param>
        /// <param name="x2">The x-coordinate of the bounding rectangle's right side.</param>
        /// <param name="y2">The y-coordinate of the bounding rectangle's bottom side.</param>
        /// <param name="color">The color.</param>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height">
        /// </param>
        public static void DrawRectangle(this WriteableBitmap bmp, byte[] buffer, int width, int height, int x1, int y1, int x2, int y2, Color color, List<int> seriesPixels)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            var w = width;
            var h = height;

            // Check boundaries
            if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)
             || (x1 >= w && x2 >= w) || (y1 >= h && y2 >= h))
            {
                return;
            }

            // Clamp boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 > w) { x1 = w; }
            if (y1 > h) { y1 = h; }
            if (x2 > w) { x2 = w; }
            if (y2 > h) { y2 = h; }

            var startY = y1 * w;
            var endY = y2 * w;

            var incrementOffset = w * 4;
            var offset2 = ((endY - w) + x1) * 4;
            var endOffset = (startY + x2) * 4;
            var startYPlusX1 = (startY + x1) * 4;

            var len = (x2 > x1) ? (x2 - x1) * 4 : (x1 - x2) * 4;
            var offset3 = ((y2 - 1) * w + x1) * 4;

            // top and bottom horizontal scanlines
            for (var x = startYPlusX1; x < endOffset; x+=4)
            {
                buffer[x] = color.B;
                buffer[x + 1] = color.G;
                buffer[x + 2] = color.R;
                buffer[x + 3] = color.A;

                buffer[offset2] = color.B;
                buffer[++offset2] = color.G;
                buffer[++offset2] = color.R;
                buffer[++offset2] = color.A;
                offset2++;
            }

            // offset2 == endY + x2

            // vertical scanlines
            endOffset = startYPlusX1 + (w * 4);
            offset2 -= (w * 4);

            for (var y = (startY + x2 - 1 + w) * 4; y < offset2; y += incrementOffset)
            {
                buffer[y] = color.B;
                buffer[y + 1] = color.G;
                buffer[y + 2] = color.R;
                buffer[y + 3] = color.A;

                buffer[endOffset] = color.B;
                buffer[endOffset + 1] = color.G;
                buffer[endOffset + 2] = color.R;
                buffer[endOffset + 3] = color.A;
                endOffset += (w * 4);              
            }          
            for (var y = startYPlusX1 + incrementOffset; y <= offset3; y += incrementOffset)
            {             
                for (int i = y; i < y + len; i++)
                {
                    seriesPixels.Add(i);
                }
            }
        }

        #endregion

        #region ellips

        /// <summary>
        /// Uses a different parameter representation than DrawEllipse().
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="xc">The x-coordinate of the ellipses center.</param>
        /// <param name="yc">The y-coordinate of the ellipses center.</param>
        /// <param name="xr">The radius of the ellipse in x-direction.</param>
        /// <param name="yr">The radius of the ellipse in y-direction.</param>
        /// <param name="color">The color for the line.</param>
        public static void FillEllipseCentered(this WriteableBitmap bmp, byte[] buffer, int height, int width, int xc, int yc, int xr, int yr, 
            Color color,List<int> seriesPixels)
        {
            int w = width;
            int h = height;

            //WPF-28768 FastScatterBitmapSeries segments(ellipse) are not correct in size. 
            //Dividing the scatterwidth and scatterheight by 2.
            xr = xr >> 1;
            yr = yr >> 1;

            // Avoid endless loop
            if (xr < 1 || yr < 1)
            {
                return;
            }

            // Init vars
            int uh, lh, uy, ly, lx, rx;
            int x = xr;
            int y = 0;
            int xrSqTwo = (xr * xr) << 1;
            int yrSqTwo = (yr * yr) << 1;
            int xChg = yr * yr * (1 - (xr << 1)) * 4;
            int yChg = xr * xr * 4;
            int err = 0;
            int xStopping = yrSqTwo * xr;
            int yStopping = 0;
            int position = 0;

            // Draw first set of points counter clockwise where tangent line slope > -1.
            while (xStopping >= yStopping)
            {
                // Draw 4 quadrant points at once
                uy = yc + y;                  // Upper half
                ly = yc - y;                  // Lower half
                if (uy < 0) uy = 0;          // Clip
                if (uy >= h) uy = h - 1;      // ...
                if (ly < 0) ly = 0;
                if (ly >= h) ly = h - 1;
                uh = uy * w * 4;                  // Upper half
                lh = ly * w * 4;                  // Lower half

                rx = (xc + x);
                lx = (xc - x);
                if (rx < 0) rx = 0;          // Clip
                if (rx >= w) rx = (w - 1);      // ...
                if (lx < 0) lx = 0;
                if (lx >= w) lx = w - 1;
                
                // Draw line
                for (int i = lx * 4; i <= rx * 4; i += 4)
                {
                    position = i+uh;
                    buffer[position] = color.B;      // Quadrant II to I (Actually two octants)
                    seriesPixels.Add(position);

                    position = i + uh + 1;
                    buffer[position] = color.G;
                    seriesPixels.Add(position);

                    position = i + uh + 2;
                    buffer[position] = color.R;
                    seriesPixels.Add(position);

                    position = i + uh + 3;
                    buffer[position] = color.A;
                    seriesPixels.Add(position);

                    position = i + lh;
                    buffer[position] = color.B;
                    seriesPixels.Add(position);

                    position = i + lh + 1;
                    buffer[position] = color.G;
                    seriesPixels.Add(position);

                    position = i + lh + 2;
                    buffer[position] = color.R;
                    seriesPixels.Add(position);

                    position = i + lh + 3;
                    buffer[position] = color.A;
                    seriesPixels.Add(position);
                }

                y++;
                yStopping += xrSqTwo;
                err += yChg;
                yChg += xrSqTwo * 4;
                if ((xChg + (err << 1)) > 0)
                {
                    x--;
                    xStopping -= yrSqTwo;
                    err += xChg;
                    xChg += yrSqTwo;
                }
            }

            // ReInit vars
            x = 0;
            y = yr;
            uy = yc + y;                  // Upper half
            ly = yc - y;                  // Lower half
            if (uy < 0) uy = 0;          // Clip
            if (uy >= h) uy = h - 1;      // ...
            if (ly < 0) ly = 0;
            if (ly >= h) ly = h - 1;
            uh = uy * w * 4;                  // Upper half
            lh = ly * w * 4;                  // Lower half
            xChg = yr * yr * 4;
            yChg = xr * xr * (1 - (yr << 1)) * 4;
            err = 0;
            xStopping = 0;
            yStopping = xrSqTwo * yr;

            // Draw second set of points clockwise where tangent line slope < -1.
            while (xStopping <= yStopping)
            {
                // Draw 4 quadrant points at once
                rx = (xc + x);
                lx = (xc - x);
                if (rx < 0) rx = 0;          // Clip
                if (rx >= w) rx = w - 1;      // ...
                if (lx < 0) lx = 0;
                if (lx >= w) lx = w - 1;

                // Draw line
                for (int i = lx * 4; i <= rx * 4; i += 4)
                {
                    position = i + uh;
                    buffer[position] = color.B;      // Quadrant II to I (Actually two octants)
                    seriesPixels.Add(position);

                    position = i + uh + 1;
                    buffer[position] = color.G;
                    seriesPixels.Add(position);

                    position = i + uh + 2;
                    buffer[position] = color.R;
                    seriesPixels.Add(position);

                    position = i + uh + 3;
                    buffer[position] = color.A;
                    seriesPixels.Add(position);

                    position = i + lh;
                    buffer[position] = color.B;
                    seriesPixels.Add(position);

                    position = i + lh + 1;
                    buffer[position] = color.G;
                    seriesPixels.Add(position);

                    position = i + lh + 2;
                    buffer[position] = color.R;
                    seriesPixels.Add(position);

                    position = i + lh + 3;
                    buffer[position] = color.A;   // Quadrant III to IV
                    seriesPixels.Add(position);
                }

                x++;
                xStopping += yrSqTwo;
                err += xChg;
                xChg += yrSqTwo * 4;
                if ((yChg + (err << 1)) > 0)
                {
                    y--;
                    uy = yc + y;                  // Upper half
                    ly = yc - y;                  // Lower half
                    if (uy < 0) uy = 0;          // Clip
                    if (uy >= h) uy = h - 1;      // ...
                    if (ly < 0) ly = 0;
                    if (ly >= h) ly = h - 1;
                    uh = uy * w * 4;                  // Upper half
                    lh = ly * w * 4;                  // Lower half
                    yStopping -= xrSqTwo;
                    err += yChg;
                    yChg += xrSqTwo;
                }
            }
        }

        /// <summary>
        /// Uses a different parameter representation than GetEllipse().
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="xc"></param>
        /// <param name="yc"></param>
        /// <param name="xr"></param>
        /// <param name="yr"></param>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public static List<int> GetEllipseCentered(this WriteableBitmap bmp,int height, int width, int xc, int yc, int xr, int yr,List<int> pixels)
        {
            int w = width;
            int h = height;

            // Avoid endless loop
            if (xr < 1 || yr < 1)
            {
                return pixels;
            }

            // Init vars
            int uh, lh, uy, ly, lx, rx;
            int x = xr;
            int y = 0;
            int xrSqTwo = (xr * xr) << 1;
            int yrSqTwo = (yr * yr) << 1;
            int xChg = yr * yr * (1 - (xr << 1)) * 4;
            int yChg = xr * xr * 4;
            int err = 0;
            int xStopping = yrSqTwo * xr;
            int yStopping = 0;

            // Draw first set of points counter clockwise where tangent line slope > -1.
            while (xStopping >= yStopping)
            {
                // Draw 4 quadrant points at once
                uy = yc + y;                  // Upper half
                ly = yc - y;                  // Lower half
                if (uy < 0) uy = 0;          // Clip
                if (uy >= h) uy = h - 1;      // ...
                if (ly < 0) ly = 0;
                if (ly >= h) ly = h - 1;
                uh = uy * w * 4;                  // Upper half
                lh = ly * w * 4;                  // Lower half

                rx = (xc + x);
                lx = (xc - x);
                if (rx < 0) rx = 0;          // Clip
                if (rx >= w) rx = (w - 1);      // ...
                if (lx < 0) lx = 0;
                if (lx >= w) lx = w - 1;

                // Draw line
                for (int i = lx * 4; i <= rx * 4; i += 4)
                {
                    pixels.Add(i + uh);
                    pixels.Add(i + uh + 1);
                    pixels.Add(i + uh + 2);
                    pixels.Add(i + uh + 3);

                    pixels.Add(i + lh);
                    pixels.Add(i + lh + 1);
                    pixels.Add(i + lh + 2);
                    pixels.Add(i + lh + 3);
                }

                y++;
                yStopping += xrSqTwo;
                err += yChg;
                yChg += xrSqTwo * 4;
                if ((xChg + (err << 1)) > 0)
                {
                    x--;
                    xStopping -= yrSqTwo;
                    err += xChg;
                    xChg += yrSqTwo;
                }
            }

            // ReInit vars
            x = 0;
            y = yr;
            uy = yc + y;                  // Upper half
            ly = yc - y;                  // Lower half
            if (uy < 0) uy = 0;          // Clip
            if (uy >= h) uy = h - 1;      // ...
            if (ly < 0) ly = 0;
            if (ly >= h) ly = h - 1;
            uh = uy * w * 4;                  // Upper half
            lh = ly * w * 4;                  // Lower half
            xChg = yr * yr * 4;
            yChg = xr * xr * (1 - (yr << 1)) * 4;
            err = 0;
            xStopping = 0;
            yStopping = xrSqTwo * yr;

            // Draw second set of points clockwise where tangent line slope < -1.
            while (xStopping <= yStopping)
            {
                // Draw 4 quadrant points at once
                rx = (xc + x);
                lx = (xc - x);
                if (rx < 0) rx = 0;          // Clip
                if (rx >= w) rx = w - 1;      // ...
                if (lx < 0) lx = 0;
                if (lx >= w) lx = w - 1;

                // Draw line
                for (int i = lx * 4; i <= rx * 4; i += 4)
                {
                    pixels.Add(i + uh);
                    pixels.Add(i + uh + 1);
                    pixels.Add(i + uh + 2);
                    pixels.Add(i + uh + 3);

                    pixels.Add(i + lh);
                    pixels.Add(i + lh + 1);
                    pixels.Add(i + lh + 2);
                    pixels.Add(i + lh + 3);
                }

                x++;
                xStopping += yrSqTwo;
                err += xChg;
                xChg += yrSqTwo * 4;
                if ((yChg + (err << 1)) > 0)
                {
                    y--;
                    uy = yc + y;                  // Upper half
                    ly = yc - y;                  // Lower half
                    if (uy < 0) uy = 0;          // Clip
                    if (uy >= h) uy = h - 1;      // ...
                    if (ly < 0) ly = 0;
                    if (ly >= h) ly = h - 1;
                    uh = uy * w * 4;                  // Upper half
                    lh = ly * w * 4;                  // Lower half
                    yStopping -= xrSqTwo;
                    err += yChg;
                    yChg += xrSqTwo;
                }
            }

            return pixels;
        }

        /// <summary>
        ///Uses a different parameter representation than GetRectangle().
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        /// <param name="pixelsAndColors"></param>
        public static List<int> GetRectangle(this WriteableBitmap bmp, int width, int height, int x1, int y1, int x2, int y2,
           List<int> pixels)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            var w = width;
            var h = height;

            // Check boundaries
            if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)
             || (x1 >= w && x2 >= w) || (y1 >= h && y2 >= h))
            {
                return pixels;
            }

            // Clamp boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 > w) { x1 = w; }
            if (y1 > h) { y1 = h; }
            if (x2 > w) { x2 = w; }
            if (y2 > h) { y2 = h; }

            // Fill first line
            var startY = y1 * w;
            var incrementOffset = w * 4;
            var startYPlusX1 = (startY + x1) * 4;
            var endOffset = (startY + x2) * 4;
            for (var x = startYPlusX1; x < endOffset; x += 4)
            {
                pixels.Add(x);

                pixels.Add(x + 1);

                pixels.Add(x + 2);

                pixels.Add(x + 3);

            }

            // Copy first line
            var len = (x2 - x1) * 4;
            var offset2 = ((y2 - 1) * w + x1) * 4;
            for (var y = startYPlusX1 + incrementOffset; y <= offset2; y += incrementOffset)
            {
                for (int i = y; i < y + len; i++)
                {
                        pixels.Add(i);
                }
            }

            return pixels;
        }

        internal static List<int> GetDrawRectangle(this WriteableBitmap bmp, int width, int height, int x1, int y1, int x2, int y2,
           List<int> pixels)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            var w = width;
            var h = height;

            // Check boundaries
            if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)
             || (x1 >= w && x2 >= w) || (y1 >= h && y2 >= h))
            {
                return pixels;
            }

            // Clamp boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 > w) { x1 = w; }
            if (y1 > h) { y1 = h; }
            if (x2 > w) { x2 = w; }
            if (y2 > h) { y2 = h; }

            var startY = y1 * w;
            var endY = y2 * w;

            var incrementOffset = w * 4;
            var offset2 = ((endY - w) + x1) * 4;
            var endOffset = (startY + x2) * 4;
            var startYPlusX1 = (startY + x1) * 4;

            // top and bottom horizontal scanlines
            for (var x = startYPlusX1; x < endOffset; x += 4)
            {
                pixels.Add(x);
                pixels.Add(x + 1);
                pixels.Add(x + 2);
                pixels.Add(x + 3);

                pixels.Add(offset2);
                pixels.Add(++offset2);
                pixels.Add(++offset2);
                pixels.Add(++offset2);
                offset2++;
            }

            // vertical scanlines
            endOffset = startYPlusX1 + (w * 4);
            offset2 -= (w * 4);

            for (var y = (startY + x2 - 1 + w) * 4; y < offset2; y += incrementOffset)
            {
                pixels.Add(y);
                pixels.Add(y + 1);
                pixels.Add(y + 2);
                pixels.Add(y + 3);

                pixels.Add(endOffset);
                pixels.Add(endOffset + 1);
                pixels.Add(endOffset + 2);
                pixels.Add(endOffset + 3);

                endOffset += (w * 4);
            }

            return pixels;
        }

        /// <summary>
        /// Draws a filled polygon. Add the first point also at the end of the array if the line should be closed.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="points">The points of the polygon in x and y pairs, therefore the array is interpreted as (x1, y1, x2, y2, ..., xn, yn).</param>
        /// <param name="color">The color for the line.</param>
        public static void FillPolygon(this WriteableBitmap bmp, byte[] buffer, int[] points, int width, int height, Color color, List<int> seriesPixels)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            int w = width;
            int h = height;
            int pn = points.Length;
            int pnh = points.Length >> 1;
            int[] intersectionsX = new int[pnh];

            // Find y min and max (slightly faster than scanning from 0 to height)
            int yMin = h;
            int yMax = 0;
            for (int i = 1; i < pn; i += 2)
            {
                int py = points[i];
                if (py < yMin) yMin = py;
                if (py > yMax) yMax = py;
            }
            if (yMin < 0) yMin = 0;
            if (yMax >= h) yMax = h - 1;


            // Scan line from min to max
            for (int y = yMin; y <= yMax; y++)
            {
                // Initial point x, y
                float vxi = points[0];
                float vyi = points[1];

                // Find all intersections
                // Based on http://alienryderflex.com/polygon_fill/
                int intersectionCount = 0;
                for (int i = 2; i < pn; i += 2)
                {
                    // Next point x, y
                    float vxj = points[i];
                    float vyj = points[i + 1];

                    // Is the scanline between the two points
                    if (vyi < y && vyj >= y
                        || vyj < y && vyi >= y)
                    {
                        // Compute the intersection of the scanline with the edge (line between two points)
                        intersectionsX[intersectionCount++] = (int)(vxi + (y - vyi) / (vyj - vyi) * (vxj - vxi));
                    }
                    vxi = vxj;
                    vyi = vyj;
                }

                // Sort the intersections from left to right using Insertion sort 
                // It's faster than Array.Sort for this small data set
                int t, j;
                for (int i = 1; i < intersectionCount; i++)
                {
                    t = intersectionsX[i];
                    j = i;
                    while (j > 0 && intersectionsX[j - 1] > t)
                    {
                        intersectionsX[j] = intersectionsX[j - 1];
                        j = j - 1;
                    }
                    intersectionsX[j] = t;
                }

                // Fill the pixels between the intersections
                for (int i = 0; i < intersectionCount - 1; i += 2)
                {
                    int x0 = intersectionsX[i];
                    int x1 = intersectionsX[i + 1];

                    // Check boundary
                    if (x1 > 0 && x0 < w)
                    {
                        if (x0 < 0) x0 = 0;
                        if (x1 >= w) x1 = w - 1;

                        // Fill the pixels
                        for (int x = x0; x <= x1; x++)
                        {
                            int index = y * w + x;
                            var pos = index * 4;
                            buffer[pos] = color.B;
                            seriesPixels.Add(pos);
                            buffer[pos + 1] = color.G;
                            seriesPixels.Add(pos + 1);
                            buffer[pos + 2] = color.R;
                            seriesPixels.Add(pos + 2);
                            buffer[pos + 3] = color.A;
                            seriesPixels.Add(pos + 3);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set background to the polygon
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="buffer"></param>
        /// <param name="points"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        /// <param name="seriesPixels"></param>
        /// <param name="clip"></param>
        public static void FillPolygon(this WriteableBitmap bmp, byte[] buffer, int[] points, int width, int height, Color color, List<int> seriesPixels,Rect clip)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            int w = width;
            int h = height;
            var ct = (int)clip.Top;
            var cb = (int)clip.Bottom;
            var cl = (int)clip.Left;
            var cr = (int)clip.Right;
            int pn = points.Length;
            int pnh = points.Length >> 1;
            int[] intersectionsX = new int[pnh];

            // Find y min and max (slightly faster than scanning from 0 to height)
            int yMin = h;
            int yMax = 0;
            for (int i = 1; i < pn; i += 2)
            {
                int py = points[i];
                if (py < yMin) yMin = py;
                if (py > yMax) yMax = py;
            }
            if (yMin < 0) yMin = 0;
            if (yMax >= h) yMax = h - 1;
            if (yMin < ct) yMin = ct;
            if (yMax >= cb) yMax = cb - 1;


            // Scan line from min to max
            for (int y = yMin; y <= yMax; y++)
            {
                // Initial point x, y
                float vxi = points[0];
                float vyi = points[1];

                // Find all intersections
                // Based on http://alienryderflex.com/polygon_fill/
                int intersectionCount = 0;
                for (int i = 2; i < pn; i += 2)
                {
                    // Next point x, y
                    float vxj = points[i];
                    float vyj = points[i + 1];

                    // Is the scanline between the two points
                    if (vyi < y && vyj >= y
                        || vyj < y && vyi >= y)
                    {
                        // Compute the intersection of the scanline with the edge (line between two points)
                        intersectionsX[intersectionCount++] = (int) (vxi + (y - vyi)/(vyj - vyi)*(vxj - vxi));
                    }
                    vxi = vxj;
                    vyi = vyj;
                }

                // Sort the intersections from left to right using Insertion sort 
                // It's faster than Array.Sort for this small data set
                int t, j;
                for (int i = 1; i < intersectionCount; i++)
                {
                    t = intersectionsX[i];
                    j = i;
                    while (j > 0 && intersectionsX[j - 1] > t)
                    {
                        intersectionsX[j] = intersectionsX[j - 1];
                        j = j - 1;
                    }
                    intersectionsX[j] = t;
                }

                // Fill the pixels between the intersections
                for (int i = 0; i < intersectionCount - 1; i += 2)
                {
                    int x0 = intersectionsX[i];
                    int x1 = intersectionsX[i + 1];

                    // Check boundary
                    if (x1 > 0 && x0 < w && x1 > cl && x0 < cr)
                    {
                        if (x0 < 0) x0 = 0;
                        if (x1 >= w) x1 = w - 1;
                        if (x0 < cl) x0 = cl;
                        if (x1 >= cr) x1 = cr - 1;

                        // Fill the pixels
                        for (int x = x0; x <= x1; x++)
                        {
                            int index = y*w + x;
                            var pos = index*4;
                            buffer[pos] = color.B;
                            seriesPixels.Add(pos);
                            buffer[pos + 1] = color.G;
                            seriesPixels.Add(pos + 1);
                            buffer[pos + 2] = color.R;
                            seriesPixels.Add(pos + 2);
                            buffer[pos + 3] = color.A;
                            seriesPixels.Add(pos + 3);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
