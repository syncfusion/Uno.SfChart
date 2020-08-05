// <copyright file="BspTree.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System.Collections.Generic;

    #region Enums
    /// <summary>
    /// Specifies the point location by the plane.
    /// </summary>
    internal enum ClassifyPointResult
    {
        /// <summary>
        /// Point is in the front of plane.
        /// </summary>
        OnFront,

        /// <summary>
        /// Point is at the back of plane.
        /// </summary>
        OnBack,

        /// <summary>
        /// Point is on the plane.
        /// </summary>
        OnPlane
    }

    /// <summary>
    /// Specifies the polygon location by the plane.
    /// </summary>
    internal enum ClassifyPolyResult
    {
        /// <summary>
        /// Polygon is on the plane.
        /// </summary>
        OnPlane,

        /// <summary>
        /// Polygon is from right of the plane.
        /// </summary>
        ToRight,

        /// <summary>
        /// Polygon is from left of the plane.
        /// </summary>
        ToLeft,

        /// <summary>
        /// Location of polygon is unknown.
        /// </summary>
        Unknown
    }
    #endregion

    /// <summary>
    /// This class contains methods to compute the Binary Space Partitioning (BSP) tree.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Already Implemented With Public Properties")]   
    internal sealed class BspTreeBuilder
    {
        #region Fields

        internal readonly List<Polygon3D> Polygons = new List<Polygon3D>();

        #endregion

        #region Constants

        private const double EPSILON = 0.0005;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see>
        /// <cref>PiePrototype.Polygon</cref>
        /// </see>
        /// at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Polygon3D"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>Returns the polygon 3D.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Already Implemented With Public Properties")]
        public Polygon3D this[int index]
        {
            get
            {
                return this.Polygons[index];
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified poly.
        /// </summary>
        /// <param name="poly">The poly.</param>
        /// <returns>Returns the last index.</returns>
        public int Add(Polygon3D poly)
        {
            this.Polygons.Add(poly);
            return this.Polygons.Count - 1;
        }

        /// <summary>
        /// Removes the specified polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        public void Remove(Polygon3D polygon)
        {
            this.Polygons.Remove(polygon);
        }

        /// <summary>
        /// Clear the polygons.
        /// </summary>
        public void Clear()
        {
            this.Polygons.Clear();
        }

        /// <summary>
        /// Calculate the available polygons.
        /// </summary>
        /// <returns>Returns the polygon count.</returns>
        public int Count()
        {
           return this.Polygons.Count;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Returns the built <see cref="BspNode"/>.</returns>
        public BspNode Build()
        {
            return this.Build(this.Polygons);
        }

        /// <summary>
        /// Builds the specified collection of polygons.
        /// </summary>
        /// <param name="arlist">The collection of polygons.</param>
        /// <returns>Returns the built <see cref="BspNode"/>.</returns>
        public BspNode Build(List<Polygon3D> arlist)
        {
            if (arlist.Count < 1)
            {
                return null;
            }

            var bspNode = new BspNode();
            var plane = arlist[0];
            bspNode.Plane = plane;
            var arleft = new List<Polygon3D>(arlist.Count);
            var arright = new List<Polygon3D>(arlist.Count);

            for (int i = 1, len = arlist.Count; i < len; i++)
            {
                var pln = arlist[i];

                if (pln == plane)
                {
                    continue;
                }

                var r = ClassifyPolygon(plane, pln);

                switch (r)
                {
                    case ClassifyPolyResult.OnPlane:
                    case ClassifyPolyResult.ToRight:
                        arright.Add(pln);
                        break;

                    case ClassifyPolyResult.ToLeft:
                        arleft.Add(pln);
                        break;

                    case ClassifyPolyResult.Unknown:
                        if (pln is Line3D || pln is UIElement3D)
                        {
                            arleft.Add(pln);
                        }
                        else if (pln is PolyLine3D)
                        {
                            arright.Add(pln);
                        }
                        else
                        {
                            Polygon3D[] ps1, ps2;
                            BspTreeBuilder.SplitPolygon(pln, plane, out ps1, out ps2);
                            arleft.AddRange(ps1);
                            arright.AddRange(ps2);
                        }

                        break;
                }
            }

            if (arleft.Count > 0)
            {
                bspNode.Back = this.Build(arleft);
            }

            if (arright.Count > 0)
            {
                bspNode.Front = this.Build(arright);
            }

            return bspNode;
        }

        /// <summary>
        /// Gets the node count.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <returns>Returns the node count.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Already Implemented With Public Properties")]
        public int GetNodeCount(BspNode el)
        {
            return (el == null) ? 0 : 1 + this.GetNodeCount(el.Back) + this.GetNodeCount(el.Front);
        }

        #endregion

        #region Helper methods

        #region Private Static Methods
        
        /// <summary>
        /// Cuts the out back polygon.
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <param name="vwiwc">The Vertical Index Classification.</param>
        /// <param name="points">The points.</param>
        private static void CutOutBackPolygon(List<Vector3DIndexClassification> polyPoints, Vector3DIndexClassification vwiwc, ICollection<Vector3D> points)
        {
            points.Clear();

            var curVW = vwiwc;

            while (true)
            {
                curVW.AlreadyCuttedBack = true;
                points.Add(curVW.Vector);

                var curVWPair = polyPoints[curVW.CuttingBackPairIndex];

                if (curVW.CuttingBackPoint)
                {
                    if (!curVWPair.AlreadyCuttedBack)
                    {
                        curVW = curVWPair;
                    }
                    else
                    {
                        var curVWPrev = polyPoints[BspTreeBuilder.GetNext(curVW.Index - 1, polyPoints.Count)];
                        var curVWNext = polyPoints[BspTreeBuilder.GetNext(curVW.Index + 1, polyPoints.Count)];

                        if ((curVWPrev.Result == ClassifyPointResult.OnBack) && !curVWPrev.AlreadyCuttedBack)
                        {
                            curVW = curVWPrev;
                        }
                        else
                            if ((curVWNext.Result == ClassifyPointResult.OnBack) && !curVWNext.AlreadyCuttedBack)
                        {
                            curVW = curVWNext;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    var curVWPrev = polyPoints[BspTreeBuilder.GetNext(curVW.Index - 1, polyPoints.Count)];
                    var curVWNext = polyPoints[BspTreeBuilder.GetNext(curVW.Index + 1, polyPoints.Count)];

                    if ((curVWPrev.Result != ClassifyPointResult.OnFront) && !curVWPrev.AlreadyCuttedBack)
                    {
                        curVW = curVWPrev;
                    }
                    else
                        if ((curVWNext.Result != ClassifyPointResult.OnFront) && !curVWNext.AlreadyCuttedBack)
                    {
                        curVW = curVWNext;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Cuts the out front polygon.
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <param name="vwiwc">The Vertical Index Classification.</param>
        /// <param name="points">The points.</param>
        private static void CutOutFrontPolygon(List<Vector3DIndexClassification> polyPoints, Vector3DIndexClassification vwiwc, List<Vector3D> points)
        {
            points.Clear();

            var curVW = vwiwc;

            while (true)
            {
                curVW.AlreadyCuttedFront = true;
                points.Add(curVW.Vector);

                var curVWPair = polyPoints[curVW.CuttingFrontPairIndex];

                if (curVW.CuttingFrontPoint)
                {
                    if (!curVWPair.AlreadyCuttedFront)
                    {
                        curVW = curVWPair;
                    }
                    else
                    {
                        var curVWPrev = polyPoints[BspTreeBuilder.GetNext(curVW.Index - 1, polyPoints.Count)];
                        var curVWNext = polyPoints[BspTreeBuilder.GetNext(curVW.Index + 1, polyPoints.Count)];

                        if ((curVWPrev.Result == ClassifyPointResult.OnFront) && !curVWPrev.AlreadyCuttedFront)
                        {
                            curVW = curVWPrev;
                        }
                        else
                            if ((curVWNext.Result == ClassifyPointResult.OnFront) && !curVWNext.AlreadyCuttedFront)
                        {
                            curVW = curVWNext;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    var curPrev = polyPoints[BspTreeBuilder.GetNext(curVW.Index - 1, polyPoints.Count)];
                    var curNext = polyPoints[BspTreeBuilder.GetNext(curVW.Index + 1, polyPoints.Count)];

                    if ((curPrev.Result != ClassifyPointResult.OnBack) && !curPrev.AlreadyCuttedFront)
                    {
                        curVW = curPrev;
                    }
                    else
                        if ((curNext.Result != ClassifyPointResult.OnBack) && !curNext.AlreadyCuttedFront)
                    {
                        curVW = curNext;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
        
        /// <summary>
        /// Get the next item.
        /// </summary>
        /// <param name="index">The Index</param>
        /// <param name="count">The Count</param>
        /// <returns>Returns the next index.</returns>
        private static int GetNext(int index, int count)
        {
            if (index >= count)
            {
                return index - count;
            }

            if (index < 0)
            {
                return index + count;
            }

            return index;
        }

        /// <summary>
        /// Classify the polygon.
        /// </summary>
        /// <param name="polygon1">The First Polygon.</param>
        /// <param name="polygon2">The Second Polygon.</param>
        /// <returns>Returns the classified result.</returns>
        private static ClassifyPolyResult ClassifyPolygon(Polygon3D polygon1, Polygon3D polygon2)
        {
            var res = ClassifyPolyResult.Unknown;
            var points = polygon2.Points;

            if (points == null)
            {
                return res;
            }

            var onBack = 0;
            var onFront = 0;
            var onPlane = 0;
            var normal = polygon1.Normal; // root node normailized value perpendicular direction
            var d = polygon1.D; // constant of the plan or depth

            for (int i = 0, len = points.Length; i < len; i++)
            {
                var r = -d - (points[i] & normal); // Comparision of Plane point depth with the other nodes

                if (r > EPSILON)
                {
                    onBack++;
                }
                else if (r < -EPSILON)
                {
                    onFront++;
                }
                else
                {
                    onPlane++;
                }

                if ((onBack > 0) && (onFront > 0))
                {
                    break;
                }
            }

            if (onPlane == points.Length)
            {
                res = ClassifyPolyResult.OnPlane;
            }
            else if (onFront + onPlane == points.Length)
            {
                res = ClassifyPolyResult.ToRight;
            }
            else if (onBack + onPlane == points.Length)
            {
                res = ClassifyPolyResult.ToLeft;
            }
            else
            {
                res = ClassifyPolyResult.Unknown;
            }

            return res;
        }

        /// <summary>
        /// Classifies the point.
        /// </summary>
        /// <param name="point3D">The 3D Point</param>
        /// <param name="polygon3D">The 3D Polygon</param>
        /// <returns>Returns the classified point result.</returns>
        private static ClassifyPointResult ClassifyPoint(Vector3D point3D, Polygon3D polygon3D)
        {
            var res = ClassifyPointResult.OnPlane;
            var sv = -polygon3D.D - (point3D & polygon3D.Normal);

            if (sv > EPSILON)
            {
                res = ClassifyPointResult.OnBack;
            }
            else if (sv < -EPSILON)
            {
                res = ClassifyPointResult.OnFront;
            }

            return res;
        }
        
        /// <summary>
        /// Split the polygon.
        /// </summary>
        /// <param name="poly">The Polygon</param>
        /// <param name="part">The Part</param>
        /// <param name="backPoly">The Back Polygon</param>
        /// <param name="frontPoly">The Front Polygon</param>
        private static void SplitPolygon(Polygon3D poly, Polygon3D part, out Polygon3D[] backPoly, out Polygon3D[] frontPoly)
        {
            var backP = new List<Polygon3D>();
            var frontP = new List<Polygon3D>();

            // this code looks for points which lie on the part plane and divide polygon into two parts
            if (poly.Points != null)
            {
                var polyPoints = new List<Vector3DIndexClassification>();
                var backPartPoints = new List<Vector3DIndexClassification>();
                var frontPartPoints = new List<Vector3DIndexClassification>();

                var outpts = new List<Vector3D>();
                var inpts = new List<Vector3D>();

                var count = poly.Points.Length;
                for (var i = 0; i < count; i++)
                {
                    var ptB = poly.Points[i];
                    var ptC = poly.Points[BspTreeBuilder.GetNext(i + 1, count)];
                    var sideB = ClassifyPoint(ptB, part);
                    var sideC = ClassifyPoint(ptC, part);

                    var vwiwcB = new Vector3DIndexClassification(ptB, polyPoints.Count, sideB);
                    polyPoints.Add(vwiwcB);

                    if ((sideB != sideC) && (sideB != ClassifyPointResult.OnPlane) &&
                        (sideC != ClassifyPointResult.OnPlane))
                    {
                        var v = ptB - ptC;
                        var dir = part.Normal * (-part.D) - ptC;

                        var sv = dir & part.Normal;
                        var sect = sv / (part.Normal & v);
                        var ptP = ptC + v * sect;
                        var vwiwc = new Vector3DIndexClassification(
                            ptP,
                            polyPoints.Count,
                            ClassifyPointResult.OnPlane);

                        polyPoints.Add(vwiwc);
                        backPartPoints.Add(vwiwc);
                        frontPartPoints.Add(vwiwc);
                    }
                    else
                        if (sideB == ClassifyPointResult.OnPlane)
                    {
                        var ptA = poly.Points[BspTreeBuilder.GetNext(i - 1, count)];
                        var sideA = ClassifyPoint(ptA, part);
                        if ((sideA == sideC))
                        {
                            continue;
                        }

                        if ((sideA != ClassifyPointResult.OnPlane) && (sideC != ClassifyPointResult.OnPlane))
                        {
                            backPartPoints.Add(vwiwcB);
                            frontPartPoints.Add(vwiwcB);
                        }
                        else
                            if (sideA == ClassifyPointResult.OnPlane)
                        {
                            switch (sideC)
                            {
                                case ClassifyPointResult.OnBack:
                                    backPartPoints.Add(vwiwcB);
                                    break;
                                case ClassifyPointResult.OnFront:
                                    frontPartPoints.Add(vwiwcB);
                                    break;
                            }
                        }
                        else
                                if (sideC == ClassifyPointResult.OnPlane)
                        {
                            switch (sideA)
                            {
                                case ClassifyPointResult.OnBack:
                                    backPartPoints.Add(vwiwcB);
                                    break;
                                case ClassifyPointResult.OnFront:
                                    frontPartPoints.Add(vwiwcB);
                                    break;
                            }
                        }
                    }
                }

                if ((frontPartPoints.Count != 0) || (backPartPoints.Count != 0))
                {
                    for (var i = 0; i < backPartPoints.Count - 1; i += 2)
                    {
                        var vwiwc1 = backPartPoints[i];
                        var vwiwc2 = backPartPoints[i + 1];
                        vwiwc1.CuttingBackPoint = true;
                        vwiwc2.CuttingBackPoint = true;
                        vwiwc1.CuttingBackPairIndex = vwiwc2.Index;
                        vwiwc2.CuttingBackPairIndex = vwiwc1.Index;
                    }

                    for (var i = 0; i < frontPartPoints.Count - 1; i += 2)
                    {
                        var vwiwc1 = frontPartPoints[i];
                        var vwiwc2 = frontPartPoints[i + 1];
                        vwiwc1.CuttingFrontPoint = true;
                        vwiwc2.CuttingFrontPoint = true;
                        vwiwc1.CuttingFrontPairIndex = vwiwc2.Index;
                        vwiwc2.CuttingFrontPairIndex = vwiwc1.Index;
                    }

                    for (var i = 0; i < backPartPoints.Count - 1; i++)
                    {
                        var vwiwc = backPartPoints[i];
                        if (vwiwc.AlreadyCuttedBack)
                        {
                            continue;
                        }

                        BspTreeBuilder.CutOutBackPolygon(polyPoints, vwiwc, outpts);

                        if (outpts.Count > 2)
                        {
                            var points = outpts.ToArray();
                            var polygon = new Polygon3D(points, poly);
                            polygon.CalcNormal(points[0], points[1], points[2]);
                            polygon.CalcNormal();
                            backP.Add(polygon);
                        }
                    }

                    for (var i = 0; i < frontPartPoints.Count - 1; i++)
                    {
                        var vwiwc = frontPartPoints[i];
                        if (vwiwc.AlreadyCuttedFront)
                        {
                            continue;
                        }

                        BspTreeBuilder.CutOutFrontPolygon(polyPoints, vwiwc, inpts);
                        if (inpts.Count > 2)
                        {
                            var points = inpts.ToArray();
                            var polygon = new Polygon3D(points, poly);
                            polygon.CalcNormal(points[0], points[1], points[2]);
                            polygon.CalcNormal();
                            frontP.Add(polygon);
                        }
                    }
                }
            }
            else
            {
                backP.Add(poly);
                frontP.Add(poly);
            }

            backPoly = backP.ToArray();
            frontPoly = frontP.ToArray();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for <see cref="Vector3DIndexClassification"/>.
    /// </summary>
    internal partial class Vector3DIndexClassification
    {
        #region Fields

        private int index;
        private ClassifyPointResult result;

        private bool isCuttingBackPoint;
        private int cuttingBackPairIndex;
        private bool alreadyCuttedBack;

        private bool isCuttingFrontPoint;
        private int cuttingFrontPairIndex;
        private bool alreadyCuttedFront;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3DIndexClassification"/> class.
        /// </summary>
        /// <param name="point">The Vector3D point.</param>
        /// <param name="ind">The index.</param>
        /// <param name="res">The ClassifyPointResult.</param>
        public Vector3DIndexClassification(Vector3D point, int ind, ClassifyPointResult res)
        {
            this.Vector = point;
            this.index = ind;
            this.result = res;
        }

        #endregion

        #region Public Properties
        
        /// <summary>
        /// Gets or sets the vector.
        /// </summary>
        /// <value>The vector.</value>
        public Vector3D Vector { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Already Implemented With Public Properties")]
        public int Index
        {
            get
            {
                return this.index;
            }

            set
            {
                if (this.index != value)
                {
                    this.index = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the classify result.
        /// </summary>
        /// <value>The classify result.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Already Implemented With Public Properties")]
        public ClassifyPointResult Result
        {
            get
            {
                return result;
            }

            set
            {
                if (this.result != value)
                {
                    this.result = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cutting back point].
        /// </summary>
        /// <value><c>true</c> if [cutting back point]; otherwise, <c>false</c>.</value>
        /// <internalonly/>
        public bool CuttingBackPoint
        {
            get
            {
                return this.isCuttingBackPoint;
            }

            set
            {
                if (this.isCuttingBackPoint != value)
                {
                    this.isCuttingBackPoint = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cutting front point].
        /// </summary>
        /// <value><c>true</c> if [cutting front point]; otherwise, <c>false</c>.</value>
        /// <internalonly/>
        public bool CuttingFrontPoint
        {
            get
            {
                return this.isCuttingFrontPoint;
            }

            set
            {
                if (this.isCuttingFrontPoint != value)
                {
                    this.isCuttingFrontPoint = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the cutting back pair.
        /// </summary>
        /// <value>The index of the cutting back pair.</value>
        /// <internalonly/>
        public int CuttingBackPairIndex
        {
            get
            {
                return this.cuttingBackPairIndex;
            }

            set
            {
                if (this.cuttingBackPairIndex != value)
                {
                    cuttingBackPairIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the cutting front pair.
        /// </summary>
        /// <value>The index of the cutting front pair.</value>
        /// <internalonly/>
        public int CuttingFrontPairIndex
        {
            get
            {
                return this.cuttingFrontPairIndex;
            }

            set
            {
                if (this.cuttingFrontPairIndex != value)
                {
                    this.cuttingFrontPairIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [already cut back].
        /// </summary>
        /// <value><c>true</c> if [already cut back]; otherwise, <c>false</c>.</value>
        /// <internalonly/>
        public bool AlreadyCuttedBack
        {
            get
            {
                return this.alreadyCuttedBack;
            }

            set
            {
                if (this.alreadyCuttedBack != value)
                {
                    this.alreadyCuttedBack = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [already cut front].
        /// </summary>
        /// <value><c>true</c> if [already cut front]; otherwise, <c>false</c>.</value>
        /// <internalonly/>
        public bool AlreadyCuttedFront
        {
            get
            {
                return this.alreadyCuttedFront;
            }

            set
            {
                if (this.alreadyCuttedFront != value)
                {
                    this.alreadyCuttedFront = value;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Class Implementation for <see cref="BspNode"/>
    /// </summary>
    internal sealed class BspNode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the back node.
        /// </summary>
        /// <value>The back node.</value>
        public BspNode Back { get; set; }

        /// <summary>
        /// Gets or sets the front node.
        /// </summary>
        /// <value>The front node.</value>
        public BspNode Front { get; set; }

        /// <summary>
        /// Gets or sets the plane.
        /// </summary>
        /// <value>The plane.</value>
        public Polygon3D Plane { get; set; }

        #endregion
    }
}
