// <copyright file="Polygon.cs" company="Syncfusion. Inc">
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
#if WINDOWS_UAP
    using Windows.Foundation;
#endif
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;

    /// <summary>
    /// Represents chart polygon to create any shapes in 3D.
    /// </summary>
    public partial class Polygon3D
    {
        #region Fields

        /// <summary>
        /// The epsilon.
        /// </summary>
        public const double Epsilon = 0.00001;

        /// <summary>
        /// Points of polygon.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "This is a public property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a public property")]
        protected internal Vector3D[] VectorPoints;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a protected property")]
       
        /// <summary>
        /// The constant of plane.
        /// </summary>
        protected double d;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a protected property")]

        /// <summary>
        /// The normal of plane.
        /// </summary>
        protected Vector3D normal;
        
        private readonly double strokeThickness;

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        public Polygon3D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        public Polygon3D(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            CalcNormal(v1, v2, v3);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public Polygon3D(Vector3D[] points)
        {
            VectorPoints = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="index">The Index.</param>
        public Polygon3D(Vector3D[] points, int index)
            : this(points)
        {
            Index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <param name="d">The d.</param>
        public Polygon3D(Vector3D normal, double d)
        {
            this.normal = normal;
            this.d = d;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The Points</param>
        /// <param name="tag">The Tag</param>
        /// <param name="index">The Index</param>
        /// <param name="stroke">The Stroke</param>
        /// <param name="strokeThickness">The StrokeThickness</param>
        /// <param name="fill">The Fill Color</param>
        /// <param name="name">The Name</param>
        public Polygon3D(Vector3D[] points, DependencyObject tag, int index, Brush stroke, double strokeThickness, Brush fill, string name)
            : this(points)
        {
            Element = new Path();
            Index = index;
            this.Tag = tag;
            this.Name = name;
            this.Stroke = stroke;
            this.strokeThickness = strokeThickness;
            this.Fill = fill;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The Points.</param>
        /// <param name="tag">The Tag</param>
        /// <param name="index">The Index</param>
        /// <param name="stroke">The Stroke</param>
        /// <param name="strokeThickness">The Stroke Thickness</param>
        /// <param name="fill">The Fill</param>
        public Polygon3D(Vector3D[] points, DependencyObject tag, int index, Brush stroke, double strokeThickness, Brush fill)
            : this(points)
        {
            Element = new Path();
            Index = index;
            this.Tag = tag;
            this.Stroke = stroke;
            this.strokeThickness = strokeThickness;
            this.Fill = fill;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="polygon">The plane.</param>
        public Polygon3D(Vector3D[] points, Polygon3D polygon)
            : this(points)
        {
            polygon.Element = null;
            Element = new Path();
            Index = polygon.Index;
            Stroke = polygon.Stroke;
            Tag = polygon.Tag;
            Graphics3D = polygon.Graphics3D;
            Fill = polygon.Fill;
            strokeThickness = polygon.strokeThickness;
        }

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        public UIElement Element { get; set; }
        
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal
        {
            get
            {
                return normal;
            }
        }

        /// <summary>
        /// Gets the A component.
        /// </summary>
        /// <value>The A component.</value>
        public double A
        {
            get
            {
                return normal.X;
            }
        }

        /// <summary>
        /// Gets the B component.
        /// </summary>
        /// <value>The B component.</value>
        public double B
        {
            get
            {
                return normal.Y;
            }
        }

        /// <summary>
        /// Gets the C component.
        /// </summary>
        /// <value>The C component.</value>
        public double C
        {
            get
            {
                return normal.Z;
            }
        }

        /// <summary>
        /// Gets the D component.
        /// </summary>
        /// <value>The D component.</value>
        public double D
        {
            get
            {
                return d;
            }
        }

        /// <summary>
        /// Gets the points of polygon.
        /// </summary>
        /// <value>The points.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is a public property")]
        public virtual Vector3D[] Points
        {
            get
            {
                return VectorPoints;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        internal int Index { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        internal DependencyObject Tag { get; set; }

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        internal Brush Stroke { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the fill color.
        /// </summary>
        internal Brush Fill { get; set; }

        /// <summary>
        /// Gets or sets the graphics 3D.
        /// </summary>
        internal Graphics3D Graphics3D { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Tests this instance to the existing.
        /// </summary>
        /// <returns>Indicates whether Normal of Plane is valid or Not.</returns>
        public bool Test()
        {
            return !normal.IsValid;
        }
       
        /// <summary>
        /// Gets the point on the plane.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Returns Vector3D instance.</returns>
        public Vector3D GetPoint(double x, double y)
        {
            var z = -(A * x + B * y + D) / C;

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Gets the point of intersect ray with plane.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="ray">The ray.</param>
        /// <returns>Returns Vector3D instance.</returns>
        public Vector3D GetPoint(Vector3D position, Vector3D ray)
        {
            var dir = normal * (-d) - position;

            var sv = dir & normal;
            var sect = sv / (normal & ray);

            return position + ray * sect;
        }

        /// <summary>
        /// Transforms by the specified <see cref="Matrix3D"/>.
        /// </summary>
        /// <param name="matrix">The 3D Matrix</param>
        public virtual void Transform(Matrix3D matrix)
        {
            if (Points != null)
            {
                for (var i = 0; i < Points.Length; i++)
                {
                    Points[i] = matrix * Points[i];
                }

                CalcNormal();
            }
            else
            {
                var v = matrix * (normal * -d);
                normal = matrix & normal;
                normal.Normalize();
                d = -(normal & v);
            }
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Creates the UI element.
        /// </summary>
        /// <param name="position">The Position</param>
        /// <param name="element">The Element</param>
        /// <param name="xLen">The X Length</param>
        /// <param name="yLen">The Y Length</param>
        /// <param name="isFront">Is Front Position</param>
        /// <param name="leftShiftType">The Left Shift Type</param>
        /// <param name="topShiftType">The Top Shift Type</param>
        /// <returns>Returns the created <see cref="UIElement3D"/>.</returns>
        internal static UIElement3D CreateUIElement(Vector3D position, UIElement element, double xLen, double yLen, bool isFront, UIElementLeftShift leftShiftType, UIElementTopShift topShiftType)
        {
            Canvas.SetZIndex(element, 0);
            var vectorColl = new Vector3D[3];
            var x = position.X;
            var y = position.Y;
            var z = position.Z;
            var desiredWidth = element.DesiredSize.Width;
            var desiredHeight = element.DesiredSize.Height;
            if (isFront)
            {
                vectorColl[0] = new Vector3D(x, y, position.Z);
                vectorColl[1] = new Vector3D(x + desiredWidth, y + desiredHeight + yLen, position.Z);
                vectorColl[2] = new Vector3D(x + desiredWidth + xLen, y + desiredHeight + yLen, position.Z);
            }
            else
            {
                vectorColl[0] = new Vector3D(x, y, z);
                vectorColl[1] = new Vector3D(x, y + desiredHeight + yLen, z + desiredWidth);
                vectorColl[2] = new Vector3D(x, y + desiredHeight + yLen, z + desiredWidth + xLen);
            }

            var uiElement = new UIElement3D(element, vectorColl);
            uiElement.LeftShift = leftShiftType;
            uiElement.TopShift = topShiftType;

            return uiElement;
        }

        /// <summary>
        /// Creates the <see cref="PolyLine3D"/> with the specified values.
        /// </summary>
        /// <param name="points">The Points</param>
        /// <param name="element">The Path Element</param>
        /// <param name="isFront">The Front</param>
        /// <returns>Returns the Created <see cref="PolyLine3D"/>.</returns>
        internal static PolyLine3D CreatePolyline(List<Vector3D> points, Path element, bool isFront)
        {
            if (points.Count == 2)
            {
                var prePoint = points[1];
                if (isFront)
                    points.Add(new Vector3D(prePoint.X + 0.01, prePoint.Y + 0.01, prePoint.Z));
                else
                    points.Add(new Vector3D(prePoint.X, prePoint.Y + 0.01, prePoint.Z + 0.01));
            }

            return new PolyLine3D(element, points);
        }

        /// <summary>
        /// Creates the <see cref="Polygon3D"/> with the specified values.
        /// </summary>
        /// <param name="vector1">The First Vector</param>
        /// <param name="vector2">The Second Vector</param>
        /// <param name="tag">The Tag</param>
        /// <param name="index">The Index</param>
        /// <param name="graphics3D">The Graphics 3D</param>
        /// <param name="stroke">The Stroke</param>
        /// <param name="fill">The Fill Color</param>
        /// <param name="strokeThickness">The Stroke Thickness</param>
        /// <param name="inverse">The Inverse <see cref="bool"/></param>
        /// <param name="name">The Name</param>
        /// <returns>Returns the created <see cref="Polygon3D"/>.</returns>
        internal static Polygon3D[] CreateBox(
           Vector3D vector1,
           Vector3D vector2,
           DependencyObject tag,
           int index,
           Graphics3D graphics3D,
           Brush stroke,
           Brush fill,
           double strokeThickness,
           bool inverse,
           string name)
        {
            var res = new Polygon3D[6];

            var p1 = new[]
            {
                new Vector3D(vector1.X, vector1.Y, vector1.Z),
                new Vector3D(vector2.X, vector1.Y, vector1.Z),
                new Vector3D(vector2.X, vector2.Y, vector1.Z),
                new Vector3D(vector1.X, vector2.Y, vector1.Z)
            };

            var p2 = new[]
            {
                new Vector3D(vector1.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector2.Z),
                new Vector3D(vector1.X, vector2.Y, vector2.Z)
            };

            var p3 = new[]
            {
                new Vector3D(vector1.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector1.Y, vector1.Z),
                new Vector3D(vector1.X, vector1.Y, vector1.Z)
            };

            var p4 = new[]
            {
                new Vector3D(vector1.X, vector2.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector1.Z),
                new Vector3D(vector1.X, vector2.Y, vector1.Z)
            };

            var p5 = new[]
            {
                new Vector3D(vector1.X, vector1.Y, vector1.Z),
                new Vector3D(vector1.X, vector1.Y, vector2.Z),
                new Vector3D(vector1.X, vector2.Y, vector2.Z),
                new Vector3D(vector1.X, vector2.Y, vector1.Z)
            };

            var p6 = new[]
            {
                new Vector3D(vector2.X, vector1.Y, vector1.Z),
                new Vector3D(vector2.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector1.Z)
            };

            res[0] = new Polygon3D(p1, tag, index, stroke, strokeThickness, fill, string.Concat("0", name));
            res[0].CalcNormal(p1[0], p1[1], p1[2]);
            res[0].CalcNormal();

            res[1] = new Polygon3D(p2, tag, index, stroke, strokeThickness, fill, string.Concat("1", name));
            res[1].CalcNormal(p2[0], p2[1], p2[2]);
            res[1].CalcNormal();

            res[2] = new Polygon3D(p3, tag, index, stroke, strokeThickness, fill, string.Concat("2", name));
            res[2].CalcNormal(p3[0], p3[1], p3[2]);
            res[2].CalcNormal();

            res[3] = new Polygon3D(p4, tag, index, stroke, strokeThickness, fill, string.Concat("3", name));
            res[3].CalcNormal(p4[0], p4[1], p4[2]);
            res[3].CalcNormal();

            res[4] = new Polygon3D(p5, tag, index, stroke, strokeThickness, fill, string.Concat("4", name));
            res[4].CalcNormal(p5[0], p5[1], p5[2]);
            res[4].CalcNormal();

            res[5] = new Polygon3D(p6, tag, index, stroke, strokeThickness, fill, string.Concat("5", name));
            res[5].CalcNormal(p6[0], p6[1], p6[2]);
            res[5].CalcNormal();

            if (inverse)
            {
                graphics3D.AddVisual(res[0]);
                graphics3D.AddVisual(res[1]);
                graphics3D.AddVisual(res[2]);
                graphics3D.AddVisual(res[3]);
                graphics3D.AddVisual(res[4]);
                graphics3D.AddVisual(res[5]);
            }
            else
            {
                graphics3D.AddVisual(res[5]);
                graphics3D.AddVisual(res[4]);
                graphics3D.AddVisual(res[0]);
                graphics3D.AddVisual(res[1]);
                graphics3D.AddVisual(res[2]);
                graphics3D.AddVisual(res[3]);
            }

            return res;
        }

        /// <summary>
        /// Creates the <see cref="Polygon3D"/> with the specified values.
        /// </summary>
        /// <param name="vector1">The First Vector</param>
        /// <param name="vector2">The Second Vector</param>
        /// <param name="tag">The Tag</param>
        /// <param name="index">The Index</param>
        /// <param name="graphics3D">The Graphics 3D</param>
        /// <param name="stroke">The Stroke</param>
        /// <param name="fill">The Fill Color</param>
        /// <param name="strokeThickness">The Stroke Thickness</param>
        /// <param name="inverse">The Inverse</param>
        /// <returns>Returns the created <see cref="Polygon3D"/>.</returns>
        internal static Polygon3D[] CreateBox(
            Vector3D vector1,
            Vector3D vector2,
            DependencyObject tag,
            int index,
            Graphics3D graphics3D,
            Brush stroke,
            Brush fill,
            double strokeThickness,
            bool inverse)
        {
            var res = new Polygon3D[6];

            var p1 = new[]
            {
                new Vector3D(vector1.X, vector1.Y, vector1.Z),
                new Vector3D(vector2.X, vector1.Y, vector1.Z),
                new Vector3D(vector2.X, vector2.Y, vector1.Z),
                new Vector3D(vector1.X, vector2.Y, vector1.Z)
            };

            var p2 = new[]
            {
                new Vector3D(vector1.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector2.Z),
                new Vector3D(vector1.X, vector2.Y, vector2.Z)
            };

            var p3 = new[]
            {
                new Vector3D(vector1.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector1.Y, vector1.Z),
                new Vector3D(vector1.X, vector1.Y, vector1.Z)
            };

            var p4 = new[]
            {
                new Vector3D(vector1.X, vector2.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector1.Z),
                new Vector3D(vector1.X, vector2.Y, vector1.Z)
            };

            var p5 = new[]
            {
                new Vector3D(vector1.X, vector1.Y, vector1.Z),
                new Vector3D(vector1.X, vector1.Y, vector2.Z),
                new Vector3D(vector1.X, vector2.Y, vector2.Z),
                new Vector3D(vector1.X, vector2.Y, vector1.Z)
            };

            var p6 = new[]
            {
                new Vector3D(vector2.X, vector1.Y, vector1.Z),
                new Vector3D(vector2.X, vector1.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector2.Z),
                new Vector3D(vector2.X, vector2.Y, vector1.Z)
            };

            res[0] = new Polygon3D(p1, tag, index, stroke, strokeThickness, fill);
            res[0].CalcNormal(p1[0], p1[1], p1[2]);
            res[0].CalcNormal();

            res[1] = new Polygon3D(p2, tag, index, stroke, strokeThickness, fill);
            res[1].CalcNormal(p2[0], p2[1], p2[2]);
            res[1].CalcNormal();

            res[2] = new Polygon3D(p3, tag, index, stroke, strokeThickness, fill);
            res[2].CalcNormal(p3[0], p3[1], p3[2]);
            res[2].CalcNormal();

            res[3] = new Polygon3D(p4, tag, index, stroke, strokeThickness, fill);
            res[3].CalcNormal(p4[0], p4[1], p4[2]);
            res[3].CalcNormal();

            res[4] = new Polygon3D(p5, tag, index, stroke, strokeThickness, fill);
            res[4].CalcNormal(p5[0], p5[1], p5[2]);
            res[4].CalcNormal();

            res[5] = new Polygon3D(p6, tag, index, stroke, strokeThickness, fill);
            res[5].CalcNormal(p6[0], p6[1], p6[2]);
            res[5].CalcNormal();

            if (inverse)
            {
                graphics3D.AddVisual(res[0]);
                graphics3D.AddVisual(res[1]);
                graphics3D.AddVisual(res[2]);
                graphics3D.AddVisual(res[3]);
                graphics3D.AddVisual(res[4]);
                graphics3D.AddVisual(res[5]);
            }
            else
            {
                graphics3D.AddVisual(res[5]);
                graphics3D.AddVisual(res[4]);
                graphics3D.AddVisual(res[0]);
                graphics3D.AddVisual(res[1]);
                graphics3D.AddVisual(res[2]);
                graphics3D.AddVisual(res[3]);
            }

            return res;
        }

        /// <summary>
        /// Updates the box.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="visibility">The visibility.</param>
        internal static void UpdateBox(Polygon3D[] plan, Vector3D vector1, Vector3D vector2, Brush stroke, Visibility visibility)
        {
            if (plan.Length < 6) return;

            plan[0].Update(
                new[] 
                {
                    new Vector3D(vector1.X, vector1.Y, vector1.Z),
                    new Vector3D(vector2.X, vector1.Y, vector1.Z),
                    new Vector3D(vector2.X, vector2.Y, vector1.Z),
                    new Vector3D(vector1.X, vector2.Y, vector1.Z)
                },                
                stroke,
                visibility);

            plan[1].Update(
                new[]
                {
                    new Vector3D(vector1.X, vector1.Y, vector2.Z),
                    new Vector3D(vector2.X, vector1.Y, vector2.Z),
                    new Vector3D(vector2.X, vector2.Y, vector2.Z),
                    new Vector3D(vector1.X, vector2.Y, vector2.Z)
                },
                stroke,
                visibility);

            plan[2].Update(
                new[] 
                {
                    new Vector3D(vector1.X, vector1.Y, vector2.Z),
                    new Vector3D(vector2.X, vector1.Y, vector2.Z),
                    new Vector3D(vector2.X, vector1.Y, vector1.Z),
                    new Vector3D(vector1.X, vector1.Y, vector1.Z)
                },
                stroke, 
                visibility);

            plan[3].Update(
                new[]
                {
                    new Vector3D(vector1.X, vector2.Y, vector2.Z),
                    new Vector3D(vector2.X, vector2.Y, vector2.Z),
                    new Vector3D(vector2.X, vector2.Y, vector1.Z),
                    new Vector3D(vector1.X, vector2.Y, vector1.Z)
                }, 
                stroke, 
                visibility);

            plan[4].Update(
                new[] 
                {
                    new Vector3D(vector1.X, vector1.Y, vector1.Z),
                    new Vector3D(vector1.X, vector1.Y, vector2.Z),
                    new Vector3D(vector1.X, vector2.Y, vector2.Z),
                    new Vector3D(vector1.X, vector2.Y, vector1.Z)
                }, 
                stroke, 
                visibility);

            plan[5].Update(
                new[] 
                {
                    new Vector3D(vector2.X, vector1.Y, vector1.Z),
                    new Vector3D(vector2.X, vector1.Y, vector2.Z),
                    new Vector3D(vector2.X, vector2.Y, vector2.Z),
                    new Vector3D(vector2.X, vector2.Y, vector1.Z)
                }, 
                stroke, 
                visibility);
        }

        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="z1">The z1.</param>
        /// <param name="z2">The z2.</param>
        /// <param name="isfront">The front indication.</param>
        /// <returns>Returns the created<see cref="Line3D"/>.</returns>
        internal static Line3D CreateLine(Line line, double x1, double y1, double x2, double y2, double z1, double z2, bool isfront)
        {
            var strokeThickness = line.StrokeThickness;
            var vectorColl = new Vector3D[3];
            if (isfront)
            {
                vectorColl[0] = new Vector3D(x1, y1, z1);
                vectorColl[1] = new Vector3D(x1 + strokeThickness, y2 + strokeThickness, z1);
                vectorColl[2] = new Vector3D(x2, y2, z1);
            }
            else
            {
                vectorColl[0] = new Vector3D(x1, y1, z1);
                vectorColl[1] = new Vector3D(x1, y2 + strokeThickness, z1 + strokeThickness);
                vectorColl[2] = new Vector3D(x1, y2, z2);
            }

            return new Line3D(line, vectorColl);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D"/>.
        /// </summary>
        /// <param name="panel">The Panel</param>
        internal virtual void Draw(Panel panel)
        {
            if (VectorPoints == null || VectorPoints.Length <= 0) return;
            var transform = Graphics3D.Transform;
            var segmentPath = Element as Path;
            if (segmentPath == null) return;
            segmentPath.Tag = Tag;

            var actualBrush = (Fill is SolidColorBrush) ?
                   ((SolidColorBrush)Fill).Color :
                   ((Fill is LinearGradientBrush) && ((LinearGradientBrush)Fill).GradientStops.Count > 0) ?
                       ((LinearGradientBrush)Fill).GradientStops[0].Color :
                       new SolidColorBrush(Colors.Transparent).Color;

            if (Tag != null && Tag is ChartSegment3D)
            {
                ((ChartSegment3D)Tag).Polygons.Add(this);
            }

            if (!panel.Children.Contains(segmentPath))
                panel.Children.Add(segmentPath);

            var figure = new PathFigure();
            var segmentGeometry = new PathGeometry();
            if (transform != null)
            {
                figure.StartPoint = transform.ToScreen(VectorPoints[0]);
                foreach (var lineSegment in VectorPoints.Select(item => new WindowsLineSegment { Point = transform.ToScreen(item) }))
                {
                    figure.Segments.Add(lineSegment);
                }
            }

            segmentGeometry.Figures.Add(figure);
            segmentPath.Data = segmentGeometry;
            var lightCoefZ = (int)(2 * (Math.Abs(normal & new Vector3D(0, 0, 1)) - 1));
            var lightCoefY = (int)(2 * (Math.Abs(normal & new Vector3D(0, 1, 0)) - 1));
            var lightCoefX = (int)(2 * (Math.Abs(normal & new Vector3D(1, 0, 0)) - 1));

                if (lightCoefZ == lightCoefX && Fill != null)
                {
                    segmentPath.Fill = ApplyZLight(actualBrush);
                }
                else if (((lightCoefY == lightCoefZ) || (lightCoefZ != 0 && lightCoefY < lightCoefZ))
                    && !(Tag is LineSegment3D) && !(Tag is AreaSegment3D) && !(Tag is PieSegment3D) && Fill != null)
                {
                    segmentPath.Fill = Polygon3D.ApplyXLight(actualBrush);
                }
                else if (lightCoefZ < 0 && Fill != null)
                {
                    segmentPath.Fill = ApplyZLight(actualBrush);
                }
                else
                {
                    segmentPath.Fill = Fill;
                }

            segmentPath.StrokeThickness = strokeThickness;
            segmentPath.Stroke = Stroke;
        }

        /// <summary>
        /// Redraws the segments.
        /// </summary>
        internal void ReDraw()
        {
            if (VectorPoints == null || VectorPoints.Length <= 0) return;
            var transform = Graphics3D.Transform;
            var segmentPath = Element as Path;
            if (segmentPath == null) return;
            var figure = new PathFigure();
            var segmentGeometry = new PathGeometry();
            if (transform != null)
            {
                figure.StartPoint = transform.ToScreen(VectorPoints[0]);
                foreach (var lineSegment in VectorPoints.Select(item => new WindowsLineSegment { Point = transform.ToScreen(item) }))
                {
                    figure.Segments.Add(lineSegment);
                }
            }

            segmentGeometry.Figures.Add(figure);
            segmentPath.Data = segmentGeometry;
            var lightCoefZ = (int)(2 * (Math.Abs(normal & new Vector3D(0, 0, 1)) - 1));
            var lightCoefY = (int)(2 * (Math.Abs(normal & new Vector3D(0, 1, 0)) - 1));
            var lightCoefX = (int)(2 * (Math.Abs(normal & new Vector3D(1, 0, 0)) - 1));

            var actualBrush = (Fill is SolidColorBrush) ?
                  ((SolidColorBrush)Fill).Color :
                  ((Fill is LinearGradientBrush) && ((LinearGradientBrush)Fill).GradientStops.Count > 0) ?
                      ((LinearGradientBrush)Fill).GradientStops[0].Color :
                      new SolidColorBrush(Colors.Transparent).Color;

                if (lightCoefZ == lightCoefX && Fill != null)
                {
                    segmentPath.Fill = ApplyZLight(actualBrush);
                }
                else if (((lightCoefY == lightCoefZ) || (lightCoefZ != 0 && lightCoefY < lightCoefZ))
                    && !(Tag is LineSegment3D) && !(Tag is AreaSegment3D) && !(Tag is PieSegment3D) && Fill != null)
                {
                    segmentPath.Fill = Polygon3D.ApplyXLight(actualBrush);
                }
                else if (lightCoefZ < 0 && Fill != null)
                {
                    segmentPath.Fill = ApplyZLight(actualBrush);
                }
                else
                {
                    segmentPath.Fill = Fill;
                }

            segmentPath.StrokeThickness = strokeThickness;
            segmentPath.Stroke = Stroke;
        }

        /// <summary>
        /// Updates the polygon.
        /// </summary>
        /// <param name="updatedPoints">The Updated Points</param>
        /// <param name="interior">The Interior</param>
        /// <param name="visibility">The Visibility</param>
        internal void Update(Vector3D[] updatedPoints, Brush interior, Visibility visibility)
        {
            VectorPoints = updatedPoints;
            var segmentPath = Element as Path;
            if (segmentPath == null) return;
            segmentPath.Visibility = visibility;
            if (Graphics3D == null) return;
            var transform = Graphics3D.Transform;
            var figure = new PathFigure();
            var segmentGeometry = new PathGeometry();

            var actualBrush = (Fill is SolidColorBrush) ?
                  ((SolidColorBrush)Fill).Color :
                  ((Fill is LinearGradientBrush) && ((LinearGradientBrush)Fill).GradientStops.Count > 0) ?
                      ((LinearGradientBrush)Fill).GradientStops[0].Color :
                      new SolidColorBrush(Colors.Transparent).Color;

            if (transform != null)
            {
                figure.StartPoint = transform.ToScreen(VectorPoints[0]);
                foreach (var lineSegment in VectorPoints.Select(item => new WindowsLineSegment { Point = transform.ToScreen(item) }))
                {
                    figure.Segments.Add(lineSegment);
                }
            }

            segmentGeometry.Figures.Add(figure);
            var lightCoefZ = (int)(2 * (Math.Abs(normal & new Vector3D(0, 0, 1)) - 1));
            var lightCoefY = (int)(2 * (Math.Abs(normal & new Vector3D(0, 1, 0)) - 1));
            var lightCoefX = (int)(2 * (Math.Abs(normal & new Vector3D(1, 0, 0)) - 1));

                if (lightCoefZ == lightCoefX && interior != null)
                {
                    segmentPath.Fill = ApplyZLight(actualBrush);
                }
                else if (((lightCoefY == lightCoefZ) || (lightCoefZ != 0 && lightCoefY < lightCoefZ))
                    && !(Tag is LineSegment3D) && !(Tag is AreaSegment3D) && interior != null)
                {
                    segmentPath.Fill = Polygon3D.ApplyXLight(actualBrush);
                }
                else if (lightCoefZ < 0 && interior != null)
                {
                    segmentPath.Fill = ApplyZLight(actualBrush);
                }
                else
                {
                    segmentPath.Fill = interior;
                }

            segmentPath.Data = segmentGeometry;
        }

        /// <summary>
        /// Calculates the normal.
        /// </summary>
        internal void CalcNormal()
        {
            CalcNormal(Points[0], Points[1], Points[2]);

            for (var i = 3; (i < Points.Length) && (Test()); i++)
            {
                CalcNormal(Points[i], Points[0], Points[i / 2]);
            }
        }

        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns>Returns Vector3D instance.</returns>
        internal virtual Vector3D GetNormal(Matrix3D transform)
        {
            Vector3D norm;

            if (VectorPoints != null)
            {
                norm = ChartMath.GetNormal(
                    transform * VectorPoints[0],
                    transform * VectorPoints[1], 
                    transform * VectorPoints[2]);
                
                for (var i = 3; (i < VectorPoints.Length) && !norm.IsValid; i++)
                {
                    var v1 = transform * VectorPoints[i];
                    var v2 = transform * VectorPoints[0];
                    var v3 = transform * VectorPoints[i / 2];

                    norm = ChartMath.GetNormal(v1, v2, v3);
                }
            }
            else
            {
                norm = transform & normal;
                norm.Normalize();
            }

            return norm;
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Calculates the normal.
        /// </summary>
        /// <param name="vector1">The First Vector.</param>
        /// <param name="vector2">The Second Vector.</param>
        /// <param name="vector3">The Third Vector.</param>
        protected internal void CalcNormal(Vector3D vector1, Vector3D vector2, Vector3D vector3)
        {
            var n = (vector1 - vector2) * (vector3 - vector2); // Relative information of the points
            var l = n.GetLength(); // Get length of the vector

            if (l < Epsilon)
            {
                l = 1;
            }

            normal = new Vector3D(n.X / l, n.Y / l, n.Z / l); // Calculate normalization of the vector
            this.d = -(this.A * vector1.X + this.B * vector1.Y + this.C * vector1.Z); // Normalized values * 1st coordinates Coordinates - Depth of the plan
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Applies the z light.
        /// </summary>
        /// <param name="color">The Color</param>
        /// <returns>Returns the Z light.</returns>
        private static SolidColorBrush ApplyZLight(Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, (byte)(color.R * 0.9), (byte)(color.G * 0.9), (byte)(color.B * 0.9)));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the X light.
        /// </summary>
        /// <param name="color">The Color</param>
        /// <returns>Returns the X light.</returns>
        private static Brush ApplyXLight(Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, (byte)(color.R * 0.7), (byte)(color.G * 0.7), (byte)(color.B * 0.7)));
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Used to plot any UIElement in 3D view.
    /// </summary>
    public partial class UIElement3D : Polygon3D
    {
        #region Fields

        private readonly FrameworkElement element;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UIElement3D"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="points">The points.</param>
        public UIElement3D(UIElement element, Vector3D[] points)
            : base(points)
        {
            this.element = element as FrameworkElement;
            CalcNormal(points[0], points[1], points[2]);
            CalcNormal();
        }

        #endregion 

        #region Properties

        /// <summary>
        /// Gets or sets the left shift indication.
        /// </summary>
        internal UIElementLeftShift LeftShift { get; set; }

        /// <summary>
        /// Gets or sets the top shift indication.
        /// </summary>
        internal UIElementTopShift TopShift { get; set; }

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D" />.
        /// </summary>
        /// <param name="panel">The Panel</param>
        internal override void Draw(Panel panel)
        {
            if (element.Parent == null)
                panel.Children.Add(element);
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var transform = Graphics3D.Transform;
            if (transform == null) return;
            var actual3DPosition = transform.ToScreen(VectorPoints[0]);
            var x = actual3DPosition.X;
            var y = actual3DPosition.Y;

            if (element is TextBlock)
            {
                GetShift(ref x, ref y, element.ActualWidth, element.ActualHeight);
            }
            else
            {
                GetShift(ref x, ref y, element.DesiredSize.Width, element.DesiredSize.Height);
            }

            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the shift.
        /// </summary>
        /// <param name="x">The X Point</param>
        /// <param name="y">The Y Point</param>
        /// <param name="width">Updates The With</param>
        /// <param name="height">Updates The Height</param>
        private void GetShift(ref double x, ref double y, double width, double height)
        {
            if (LeftShift == UIElementLeftShift.LeftShift)
            {
                x = x - width;
            }
            else if (LeftShift == UIElementLeftShift.RightShift)
            {
                x = x + width;
            }
            else if (LeftShift == UIElementLeftShift.LeftHalfShift)
            {
                x = x - width / 2;
            }
            else if (LeftShift == UIElementLeftShift.RightHalfShift)
            {
                x = x + width / 2;
            }
            
            if (TopShift == UIElementTopShift.TopShift)
            {
                y = y - height;
            }
            else if (TopShift == UIElementTopShift.BottomShift)
            {
                y = y + height;
            }
            else if (TopShift == UIElementTopShift.TopHalfShift)
            {
                y = y - height * 0.5;
            }
            else if (TopShift == UIElementTopShift.BottomHalfShift)
            {
                y = y + height * 0.5;
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Used to draw line in 3D view.
    /// </summary>
    public partial class PolyLine3D : Polygon3D
    {
        #region Fields

        private readonly Path element;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLine3D"/> class.
        /// </summary>
        /// <param name="element">The Path Element.</param>
        /// <param name="vectors">The Vectors.</param>
        public PolyLine3D(Path element, List<Vector3D> vectors)
            : base(vectors.ToArray())
        {
            this.element = element;
            CalcNormal(vectors.ToArray()[0], vectors.ToArray()[1], vectors.ToArray()[2]);
            CalcNormal();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D" />.
        /// </summary>
        /// <param name="panel">The Panel</param>
        internal override void Draw(Panel panel)
        {
            if (((FrameworkElement)element).Parent == null)
                panel.Children.Add(element);
            var transform = Graphics3D.Transform;
            if (transform == null) return;

            var pathFigure = new PathFigure();
            var pathGeometry = new PathGeometry();

            pathGeometry.Figures.Add(pathFigure);
            element.Data = pathGeometry;
            pathFigure.StartPoint = transform.ToScreen(VectorPoints[0]);
            var segment = new PolyLineSegment();
            segment.Points = new PointCollection();
            foreach (var vectorPoint in VectorPoints)
            {
                segment.Points.Add(transform.ToScreen(vectorPoint));
            }

            pathFigure.Segments.Add(segment);
        }

        #endregion
    }

    /// <summary>
    /// Used to draw line in 3D view.
    /// </summary>
    public partial class Line3D : Polygon3D
    {
        #region Fields

        private readonly UIElement element;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Line3D"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="points">The points.</param>
        public Line3D(UIElement element, Vector3D[] points)
            : base(points)
        {
            this.element = element;

            CalcNormal(points[0], points[1], points[2]);
            CalcNormal();
        }

        #endregion

        #region Internal

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D" />.
        /// </summary>
        /// <param name="panel">The Panel</param>
        internal override void Draw(Panel panel)
        {
            if (((FrameworkElement)element).Parent == null)
                panel.Children.Add(element);
            var transform = Graphics3D.Transform;
            if (transform == null) return;
            var actual3DPosition1 = transform.ToScreen(VectorPoints[0]);
            var actual3DPosition2 = transform.ToScreen(VectorPoints[2]);
            var line = element as Line;
            if (line == null) return;
            line.X1 = actual3DPosition1.X;
            line.X2 = actual3DPosition2.X;
            line.Y1 = actual3DPosition1.Y;
            line.Y2 = actual3DPosition2.Y;
        }

        #endregion
    }
}
