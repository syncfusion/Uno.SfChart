// <copyright file="Graphics3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System.Collections.Generic;
    using Windows.Foundation;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Represents Graphics3D.
    /// </summary>
    public partial class Graphics3D
    {
        #region Fields

        private readonly BspTreeBuilder treeBuilder = new BspTreeBuilder();
        private ChartTransform.ChartTransform3D transform;
        private BspNode tree;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        public ChartTransform.ChartTransform3D Transform
        {
            get
            {
                return this.transform;
            }

            set
            {
                if (this.transform != value)
                {
                    this.transform = value;
                }
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Gets the visual count.
        /// </summary>
        /// <returns>Returns the count.</returns>
        public int GetVisualCount()
        {
            return this.treeBuilder.Count();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphics3D"/> class.
        /// </summary>
        /// <summary>
        /// Adds the polygon to the drawing.
        /// </summary>
        /// <param name="polygon">The <see cref="Polygon3D"/>.</param>
        /// <returns>Returns the last index.</returns>
        public int AddVisual(Polygon3D polygon)
        {
            if ((polygon == null) || (polygon.Test()))
            {
                return -1;
            }

            polygon.Graphics3D = this;
            return this.treeBuilder.Add(polygon);
        }

        /// <summary>
        /// Removes the specified polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        public void Remove(Polygon3D polygon)
        {
            this.treeBuilder.Remove(polygon);
        }

        /// <summary>
        /// clear the polygon from visual tree.
        /// </summary>
        public void ClearVisual()
        {
            this.treeBuilder.Clear();
        }

        /// <summary>
        /// Gets the visual.
        /// </summary>
        /// <returns>Returns the visual.</returns>
        public List<Polygon3D> GetVisual()
        {
            return this.treeBuilder.Polygons;
        }

        /// <summary>
        /// Computes the BSP tree.
        /// </summary>
        public void PrepareView()
        {
            this.tree = this.treeBuilder.Build();
        }

        /// <summary>
        /// Computes the BSP tree.
        /// </summary>
        /// <param name="perspectiveAngle">The Perspective Angle</param>
        /// <param name="depth">The Chart Depth</param>
        /// <param name="rotation">The Rotation Angle</param>
        /// <param name="tilt">The Tilt Angle</param>
        /// <param name="size">The Size</param>
        public void PrepareView(double perspectiveAngle, double depth, double rotation, double tilt, Size size)
        {
            if (this.transform == null)
            {
                this.transform = new ChartTransform.ChartTransform3D(size);
            }
            else
            {
                this.transform.mViewport = size;
            }

            this.transform.Rotation = rotation;
            this.transform.Tilt = tilt;
            this.transform.Depth = depth;
            this.transform.PerspectiveAngle = perspectiveAngle;
            this.transform.Transform();
            this.tree = this.treeBuilder.Build();
        }

        /// <summary>
        /// Draws the paths to the panel/>.
        /// </summary>
        /// <param name="panel">The Panel</param>
        public void View(Panel panel)
        {
            if (panel == null) return;
            panel.Children.Clear();
            var eye = new Vector3D(0, 0, short.MaxValue);
            this.DrawBspNode3D(this.tree, eye, panel);
        }
        
        /// <summary>
        /// Draws the polygons to the <see cref="System.Drawing.Graphics"/>.
        /// </summary>
        /// <param name="panel">The Panel</param>
        /// <param name="rotation">The Rotation Angle</param>
        /// <param name="tilt">The Tilt Angle</param>
        /// <param name="size">The Size</param>
        /// <param name="perspectiveAngle">The Perspective Angle</param>
        /// <param name="depth">The Depth</param>
        public void View(Panel panel, double rotation, double tilt, Size size, double perspectiveAngle, double depth)
        {
            if (panel == null)
            {
                return;
            }

            panel.Children.Clear();

            if (this.transform == null)
            {
                this.transform = new ChartTransform.ChartTransform3D(size);
            }
            else
            {
                this.transform.mViewport = size;
            }

            this.transform.Rotation = rotation;
            this.transform.Tilt = tilt;
            this.transform.Depth = depth;
            this.transform.PerspectiveAngle = perspectiveAngle;
            this.transform.Transform();
            var eye = new Vector3D(0, 0, short.MaxValue);
            this.DrawBspNode3D(this.tree, eye, panel);
        }

        #endregion

        #region Helper methods

        #region Private Methods

        /// <summary>
        /// Draws the BSP node in 3D.
        /// </summary>
        /// <param name="tree">The Tree.</param>
        /// <param name="eye">The Eye Position.</param>
        /// <param name="panel">The Panel.</param>
        private void DrawBspNode3D(BspNode tree, Vector3D eye, Panel panel)
        {
            if (tree == null || this.transform == null) return;
            while (true)
            {
                var r = tree.Plane.GetNormal(this.transform.Result) & eye;
                if (r > tree.Plane.D)
                {
                    if (tree.Front != null)
                    {
                        this.DrawBspNode3D(tree.Front, eye, panel);
                    }

                    tree.Plane.Draw(panel);

                    if (tree.Back != null)
                    {
                        tree = tree.Back;
                        continue;
                    }
                }
                else
                {
                    if (tree.Back != null)
                    {
                        this.DrawBspNode3D(tree.Back, eye, panel);
                    }

                    tree.Plane.Draw(panel);

                    if (tree.Front != null)
                    {
                        tree = tree.Front;
                        continue;
                    }
                }

                break;
            }
        }

        #endregion

        #endregion
    }
}
