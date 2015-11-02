﻿// <copyright file="ColorMatrixFilter.cs" company="James South">
// Copyright (c) James South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor.Filters
{
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// The color matrix filter.
    /// </summary>
    public class MatrixFilter : ParallelImageProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixFilter"/> class.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix4x4"/> to apply.</param>
        public MatrixFilter(Matrix4x4 matrix)
        {
            this.Value = matrix;
        }

        /// <summary>
        /// Gets the matrix value.
        /// </summary>
        public Matrix4x4 Value { get; }

        /// <inheritdoc/>
        protected override void Apply(ImageBase target, ImageBase source, Rectangle targetRectangle, Rectangle sourceRectangle, int startY, int endY)
        {
            int sourceY = sourceRectangle.Y;
            int sourceBottom = sourceRectangle.Bottom;
            int startX = sourceRectangle.X;
            int endX = sourceRectangle.Right;
            Matrix4x4 matrix = this.Value;

            Parallel.For(
                startY,
                endY,
                y =>
                    {
                        if (y >= sourceY && y < sourceBottom)
                        {
                            for (int x = startX; x < endX; x++)
                            {
                                target[x, y] = ApplyMatrix(source[x, y], matrix);
                            }
                        }
                    });
        }

        /// <summary>
        /// Applies the color matrix against the given color.
        /// </summary>
        /// <param name="color">The source color.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        private static Color ApplyMatrix(Color color, Matrix4x4 matrix)
        {
            color = PixelOperations.ToLinear(color);

            float sr = color.R;
            float sg = color.G;
            float sb = color.B;

            color.R = (sr * matrix.M11) + (sg * matrix.M21) + (sb * matrix.M31) + matrix.M41;
            color.G = (sr * matrix.M12) + (sg * matrix.M22) + (sb * matrix.M32) + matrix.M42;
            color.B = (sr * matrix.M13) + (sg * matrix.M23) + (sb * matrix.M33) + matrix.M43;

            return PixelOperations.ToSrgb(color);
        }
    }
}
