﻿// <copyright file="Contrast.cs" company="James South">
// Copyright (c) James South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor.Filters
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An <see cref="IImageProcessor"/> to change the contrast of an <see cref="Image"/>.
    /// </summary>
    public class Contrast : ParallelImageProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Contrast"/> class.
        /// </summary>
        /// <param name="contrast">The new contrast of the image. Must be between -100 and 100.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="contrast"/> is less than -100 or is greater than 100.
        /// </exception>
        public Contrast(int contrast)
        {
            Guard.MustBeBetweenOrEqualTo(contrast, -100, 100, nameof(contrast));
            this.Value = contrast;
        }

        /// <summary>
        /// Gets the contrast value.
        /// </summary>
        public int Value { get; }

        /// <inheritdoc/>
        protected override void Apply(ImageBase target, ImageBase source, Rectangle targetRectangle, Rectangle sourceRectangle, int startY, int endY)
        {
            float contrast = (100f + this.Value) / 100f;
            int sourceY = sourceRectangle.Y;
            int sourceBottom = sourceRectangle.Bottom;
            int startX = sourceRectangle.X;
            int endX = sourceRectangle.Right;

            Parallel.For(
                startY,
                endY,
                y =>
                    {
                        if (y >= sourceY && y < sourceBottom)
                        {
                            for (int x = startX; x < endX; x++)
                            {
                                target[x, y] = AdjustContrast(source[x, y], contrast);
                            }
                        }
                    });
        }

        /// <summary>
        /// Returns a <see cref="Color"/> with the contrast adjusted.
        /// </summary>
        /// <param name="color">The source color.</param>
        /// <param name="contrast">The contrast adjustment factor.</param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        private static Color AdjustContrast(Color color, float contrast)
        {
            color = PixelOperations.ToLinear(color);

            color.R -= 0.5f;
            color.R *= contrast;
            color.R += 0.5f;

            color.G -= 0.5f;
            color.G *= contrast;
            color.G += 0.5f;

            color.B -= 0.5f;
            color.B *= contrast;
            color.B += 0.5f;

            return PixelOperations.ToSrgb(color);
        }
    }
}
