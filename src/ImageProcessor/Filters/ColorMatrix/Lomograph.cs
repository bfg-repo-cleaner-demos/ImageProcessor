﻿// <copyright file="Lomograph.cs" company="James South">
// Copyright © James South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor.Filters
{
    /// <summary>
    /// Converts the colors of the image recreating an old Lomograph effect.
    /// </summary>
    public class Lomograph : ColorMatrixFilter
    {
        /// <summary>
        /// The Lomograph matrix. Purely artistic in composition.
        /// TODO: Calculate a matrix that works in the linear color space.
        /// </summary>
        private static readonly ColorMatrix Matrix = new ColorMatrix(
            new[]
                {
                    new[] { 1.50f, 0, 0, 0, 0 },
                    new[] { 0, 1.45f, 0, 0, 0 },
                    new[] { 0, 0, 1.09f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new[] { -0.10f, 0.05f, -0.08f, 0, 1 }
                });

        /// <summary>
        /// Initializes a new instance of the <see cref="Lomograph"/> class.
        /// </summary>
        public Lomograph()
            : base(Matrix, false)
        {
        }
    }
}
