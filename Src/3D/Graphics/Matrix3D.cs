// <copyright file="Matrix3D.cs" company="Syncfusion. Inc">
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

    /// <summary>
    /// Represents the matrix 4x4.
    /// </summary>
    public struct Matrix3D
    {
        #region Constants

        private const int MATRIXSIZE = 4;

        #endregion

        #region Fields

        private readonly double[][] mData;

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3D"/> struct.
        /// </summary>
        /// <param name="m11">The M11 element of matrix.</param>
        /// <param name="m12">The M12 element of matrix.</param>
        /// <param name="m13">The M13 element of matrix.</param>
        /// <param name="m14">The M14 element of matrix.</param>
        /// <param name="m21">The M21 element of matrix.</param>
        /// <param name="m22">The M22 element of matrix.</param>
        /// <param name="m23">The M23 element of matrix.</param>
        /// <param name="m24">The M24 element of matrix.</param>
        /// <param name="m31">The M31 element of matrix.</param>
        /// <param name="m32">The M32 element of matrix.</param>
        /// <param name="m33">The M33 element of matrix.</param>
        /// <param name="m34">The M34 element of matrix.</param>
        /// <param name="m41">The M41 element of matrix.</param>
        /// <param name="m42">The M42 element of matrix.</param>
        /// <param name="m43">The M43 element of matrix.</param>
        /// <param name="m44">The M44 element of matrix.</param>
        public Matrix3D(
                        double m11,
                        double m12, 
                        double m13,
                        double m14,
                        double m21,
                        double m22,
                        double m23, 
                        double m24,
                        double m31,
                        double m32, 
                        double m33,
                        double m34,
                        double m41,
                        double m42, 
                        double m43, 
                        double m44)
                        : this(MATRIXSIZE)
        {
            mData[0][0] = m11;
            mData[1][0] = m12;
            mData[2][0] = m13;
            mData[3][0] = m14;

            mData[0][1] = m21;
            mData[1][1] = m22;
            mData[2][1] = m23;
            mData[3][1] = m24;

            mData[0][2] = m31;
            mData[1][2] = m32;
            mData[2][2] = m33;
            mData[3][2] = m34;

            mData[0][3] = m41;
            mData[1][3] = m42;
            mData[2][3] = m43;
            mData[3][3] = m44;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3D"/> struct.
        /// </summary>
        /// <param name="size">The size.</param>
        private Matrix3D(int size)
        {
            mData = new double[size][];

            for (var i = 0; i < size; i++)
            {
                mData[i] = new double[size];
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        /// <value>The identity matrix.</value>
        public static Matrix3D Identity
        {
            get
            {
                return GetIdentity();
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether this matrix is affine.
        /// </summary>
        /// <value><c>true</c> if this matrix is affine; otherwise, <c>false</c>.</value>
        public bool IsAffine
        {
            get
            {
                return (mData[0][3] == 0) && (mData[1][3] == 0)
                  && (mData[2][3] == 0) && (mData[3][3] == 1);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> with the specified column and row.
        /// </summary>
        /// <param name="i">The I Value</param>
        /// <param name="j">The J Value</param>
        /// <returns>Returns the value at the specified location.</returns>
        public double this[int i, int j]
        {
            get
            {
                return mData[i][j];
            }

            set
            {
                mData[i][j] = value;
            }
        }

        #endregion

        #region Public methods

        #region Operators
        /// <summary>
        /// Add the matrixes.
        /// </summary>
        /// <param name="m1">The First Matrix</param>
        /// <param name="m2">The Second Matrix</param>
        /// <returns>Returns the result of the operator.</returns>
        public static Matrix3D operator +(Matrix3D m1, Matrix3D m2)
        {
            var m = new Matrix3D(4);

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    m[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return m;
        }

        /// <summary>
        /// Method used to returns x, y, z values.
        /// </summary>
        /// <param name="m1">The First Matrix</param>
        /// <param name="point">The Three Dimensional Vector</param>
        /// <returns>Returns the result of the operator.</returns>
        public static Vector3D operator *(Matrix3D m1, Vector3D point)
        {
            var x = m1.mData[0][0] * point.X + m1.mData[1][0] * point.Y + m1.mData[2][0] * point.Z + m1.mData[3][0];
            var y = m1.mData[0][1] * point.X + m1.mData[1][1] * point.Y + m1.mData[2][1] * point.Z + m1.mData[3][1];
            var z = m1.mData[0][2] * point.X + m1.mData[1][2] * point.Y + m1.mData[2][2] * point.Z + m1.mData[3][2];

            if (!m1.IsAffine)
            {
                var c = 1d / (m1.mData[0][3] * point.X + m1.mData[1][3] * point.Y + m1.mData[2][3] * point.Z + m1.mData[3][3]);
                x *= c;
                y *= c;
                z *= c;
            }

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Method used to returns x, y, z values.
        /// </summary>
        /// <param name="m1">The First Matrix</param>
        /// <param name="v1">The Three Dimensional Vector</param>
        /// <returns>Returns the result of the operator.</returns>
        public static Vector3D operator &(Matrix3D m1, Vector3D v1)
        {
            var x = m1.mData[0][0] * v1.X + m1.mData[1][0] * v1.Y + m1.mData[2][0] * v1.Z;
            var y = m1.mData[0][1] * v1.X + m1.mData[1][1] * v1.Y + m1.mData[2][1] * v1.Z;
            var z = m1.mData[0][2] * v1.X + m1.mData[1][2] * v1.Y + m1.mData[2][2] * v1.Z;

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Gets the multiplied matrix values.
        /// </summary>
        /// <param name="f1">The Double Value</param>
        /// <param name="m1">The Matrix</param>
        /// <returns>Returns the result of the operator.</returns>
        public static Matrix3D operator *(double f1, Matrix3D m1)
        {
            var length = m1.mData.Length;
            var res = new Matrix3D(length);

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    res.mData[i][j] = m1.mData[i][j] * f1;
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the multiplied matrix values.
        /// </summary>
        /// <param name="m1">The First Matrix</param>
        /// <param name="m2">The Second Matrix</param>
        /// <returns>Returns the result of the operator.</returns>
        public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
        {
            var res = GetIdentity();

            for (var i = 0; i < MATRIXSIZE; i++)
            {
                for (var j = 0; j < MATRIXSIZE; j++)
                {
                    double v = 0;

                    for (var k = 0; k < MATRIXSIZE; k++)
                    {
                        v += m1[k, j] * m2[i, k];
                    }

                    res[i, j] = v;
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the equality check <see cref="bool"/> value.
        /// </summary>
        /// <param name="m1">The First Matrix</param>
        /// <param name="m2">The Second Matrix</param>
        /// <returns>Returns the result of the operator.</returns>
        public static bool operator ==(Matrix3D m1, Matrix3D m2)
        {
            var res = true;

            for (var i = 0; i < m1.mData.Length; i++)
            {
                for (var j = 0; j < m1.mData.Length; j++)
                {
                    if (m1.mData[i][j] != m2.mData[i][j])
                    {
                        res = false;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the matrix equality <see cref="bool"/> value.
        /// </summary>
        /// <param name="m1">The First Matrix</param>
        /// <param name="m2">The Second Matrix</param>
        /// <returns>Returns the result of the operator.</returns>
        public static bool operator !=(Matrix3D m1, Matrix3D m2)
        {
            var res = true;

            for (var i = 0; i < m1.mData.Length; i++)
            {
                for (var j = 0; j < m1.mData.Length; j++)
                {
                    if (m1.mData[i][j] != m2.mData[i][j])
                    {
                        res = false;
                    }
                }
            }

            return !res;
        }

        #endregion

        /// <summary>
        /// Gets the determinant.
        /// </summary>
        /// <param name="matrix3D">The matrix.</param>
        /// <returns>Returns the determinant of the matrix.</returns>
        public static double GetD(Matrix3D matrix3D)
        {
            return GetDeterminant(matrix3D.mData);
        }

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        /// <returns>Returns the identity matrix.</returns>
        public static Matrix3D GetIdentity()
        {
            var m = new Matrix3D(MATRIXSIZE);

            for (var i = 0; i < MATRIXSIZE; i++)
            {
                m[i, i] = 1.0f;
            }

            return m;
        }

        /// <summary>
        /// Transforms the specified vector.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>Returns the transformed matrix.</returns>
        public static Matrix3D Transform(double x, double y, double z)
        {
            var res = GetIdentity(); // new Matrix3D( MATRIX_SIZE );

            res.mData[3][0] = x;
            res.mData[3][1] = y;
            res.mData[3][2] = z;

            return res;
        }

        /// <summary>
        /// Turns by the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>Returns the resultant turn matrix.</returns>
        public static Matrix3D Turn(double angle)
        {
            var res = GetIdentity();

            res[0, 0] = Math.Cos(angle);
            res[2, 0] = -Math.Sin(angle);
            res[0, 2] = Math.Sin(angle);
            res[2, 2] = Math.Cos(angle);

            return res;
        }

        /// <summary>
        /// Tilts by the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>Returns the resultant turn matrix.</returns>
        public static Matrix3D Tilt(double angle)
        {
            var res = GetIdentity();

            res[1, 1] = (Math.Cos(angle));
            res[2, 1] = (Math.Sin(angle));
            res[1, 2] = -(Math.Sin(angle));
            res[2, 2] = (Math.Cos(angle));

            return res;
        }

        /// <summary>
        /// Transposes the specified matrix.
        /// </summary>
        /// <param name="matrix3D">The matrix.</param>
        /// <returns>Returns the transposed matrix.</returns>
        public static Matrix3D Transposed(Matrix3D matrix3D)
        {
            var m = Identity;

            for (var i = 0; i < MATRIXSIZE; i++)
            {
                for (var j = 0; j < MATRIXSIZE; j++)
                {
                    m[i, j] = matrix3D[j, i];
                }
            }

            return m;
        }

        /// <summary>
        /// Shears the specified values.
        /// </summary>
        /// <param name="xy">The x y shear.</param>
        /// <param name="xz">The x z shear.</param>
        /// <param name="yx">The y x shear.</param>
        /// <param name="yz">The y z shear.</param>
        /// <param name="zx">The z x shear.</param>
        /// <param name="zy">The z y shear.</param>
        /// <returns>Returns the sheared values.</returns>
        public static Matrix3D Shear(double xy, double xz, double yx, double yz, double zx, double zy)
        {
            var res = Identity;

            res[1, 0] = xy;
            res[2, 0] = xz;
            res[0, 1] = yx;
            res[2, 1] = yz;
            res[0, 2] = zx;
            res[1, 2] = zy;

            return res;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if object and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var m1 = (Matrix3D)obj;
            var res = true;

            for (var i = 0; i < m1.mData.Length; i++)
            {
                for (var j = 0; j < m1.mData.Length; j++)
                {
                    if (m1.mData[i][j] != this.mData[i][j])
                    {
                        res = false;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return this.mData.GetHashCode();
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Intervals the matrix.
        /// </summary>
        /// <param name="matrix3D">The Matrix.</param>
        /// <returns>Returns the interval.</returns>
        internal static Matrix3D GetInvertal(Matrix3D matrix3D)
        {
            var m = Identity;

            for (var i = 0; i < MATRIXSIZE; i++)
            {
                for (var j = 0; j < MATRIXSIZE; j++)
                {
                    m[i, j] = GetMinor(matrix3D, i, j);
                }
            }

            m = Transposed(m);
            m = (1f / GetD(matrix3D)) * m;

            return m;
        }

        /// <summary>
        /// Gets the minor.
        /// </summary>
        /// <param name="dd">The matrix.</param>
        /// <param name="columnIndex">The index of column.</param>
        /// <param name="rowIndex">The index of row.</param>
        /// <returns>Returns the minor of the matrix.</returns>
        internal static double GetMinor(Matrix3D dd, int columnIndex, int rowIndex)
        {
            return (((columnIndex + rowIndex) % 2 == 0) ? 1 : -1) * GetDeterminant(GetMMtr(dd.mData, columnIndex, rowIndex));
        }

        /// <summary>
        /// Tilts by the specified angle in arbitrary axis direction.
        /// </summary>
        /// <param name="angle">The Angle</param>
        /// <param name="vector3D">The Vector3D</param>
        /// <returns>Returns the resultant turn matrix.</returns>
        internal static Matrix3D TiltArbitrary(double angle, Vector3D vector3D)
        {
            var res = GetIdentity();

            double x = vector3D.X;
            double y = vector3D.Y;
            double z = vector3D.Z;

            double x2 = x * x;
            double y2 = y * y;
            double z2 = z * z;
            double l = (x2 + y2 + z2);

            double sqrtL = Math.Sqrt(l);
            double cosA = Math.Cos(angle);
            double sinA = Math.Sin(angle);

            res[0, 0] = (x2 + (y2 + z2) * cosA) / l;
            res[0, 1] = (x * y * (1 - cosA) - z * sqrtL * sinA) / l;
            res[0, 2] = (x * z * (1 - cosA) + y * sqrtL * sinA) / l;
            res[0, 3] = 0.0;

            res[1, 0] = (x * y * (1 - cosA) + z * sqrtL * sinA) / l;
            res[1, 1] = (y2 + (x2 + z2) * cosA) / l;
            res[1, 2] = (y * z * (1 - cosA) - x * sqrtL * sinA) / l;
            res[1, 3] = 0.0;

            res[2, 0] = (x * z * (1 - cosA) - y * sqrtL * sinA) / l;
            res[2, 1] = (y * z * (1 - cosA) + x * sqrtL * sinA) / l;
            res[2, 2] = (z2 + (x2 + y2) * cosA) / l;
            res[2, 3] = 0.0;

            res[3, 0] = 0.0;
            res[3, 1] = 0.0;
            res[3, 2] = 0.0;
            res[3, 3] = 1.0;

            return res;
        }

        #endregion

        #region Helper methdos

        #region Private Static Methods        

        /// <summary>
        /// Calculates determinant row given matrix..
        /// </summary>
        /// <param name="dd">The matrix to calculate determinant.</param>
        /// <returns>Determinant of the given matrix.</returns>
        private static double GetDeterminant(IList<double[]> dd)
        {
            var count = dd.Count;
            double res = 0;

            if (count < 2)
            {
                res = dd[0][0];
            }
            else
            {
                var k = 1;

                for (var i = 0; i < count; i++)
                {
                    var dm = GetMMtr(dd, i, 0);

                    res += k * dd[i][0] * GetDeterminant(dm);
                    k = (k > 0) ? -1 : 1;
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the minor.
        /// </summary>
        /// <param name="dd">The matrix.</param>
        /// <param name="columnIndex">The index of column.</param>
        /// <param name="rowIndex">The index of row.</param>
        /// <returns>Returns the matrix.</returns>
        private static double[][] GetMMtr(IList<double[]> dd, int columnIndex, int rowIndex)
        {
            var count = dd.Count - 1;
            var d = new double[count][];

            for (var i = 0; i < count; i++)
            {
                var m = (i >= columnIndex) ? i + 1 : i;
                d[i] = new double[count];

                for (var j = 0; j < count; j++)
                {
                    var n = (j >= rowIndex) ? j + 1 : j;

                    d[i][j] = dd[m][n];
                }
            }

            return d;
        }

        #endregion

        #endregion
    }
}
