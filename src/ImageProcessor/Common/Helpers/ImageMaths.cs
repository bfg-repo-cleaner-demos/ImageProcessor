﻿// <copyright file="ImageMaths.cs" company="James South">
// Copyright (c) James South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor
{
    using System;

    /// <summary>
    /// Provides common mathematical methods.
    /// </summary>
    internal static class ImageMaths
    {
        /// <summary>
        /// Represents PI, the ratio of a circle's circumference to its diameter.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const float PI = 3.1415926535897931f;

        /// <summary>
        /// Returns the result of a B-C filter against the given value.
        /// <see href="http://www.imagemagick.org/Usage/filter/#cubic_bc"/>
        /// </summary>
        /// <param name="x">The value to process.</param>
        /// <param name="b">The B-Spline curve variable.</param>
        /// <param name="c">The Cardinal curve variable.</param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float GetBcValue(float x, float b, float c)
        {
            float temp;

            if (x < 0)
            {
                x = -x;
            }

            temp = x * x;
            if (x < 1)
            {
                x = ((12 - (9 * b) - (6 * c)) * (x * temp)) + ((-18 + (12 * b) + (6 * c)) * temp) + (6 - (2 * b));
                return x / 6;
            }

            if (x < 2)
            {
                x = ((-b - (6 * c)) * (x * temp)) + (((6 * b) + (30 * c)) * temp) + (((-12 * b) - (48 * c)) * x) + ((8 * b) + (24 * c));
                return x / 6;
            }

            return 0;
        }

        /// <summary>
        /// Gets the result of a sine cardinal function for the given value.
        /// </summary>
        /// <param name="x">
        /// The value to calculate the result for.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float SinC(float x)
        {
            const float Epsilon = .00001f;

            if (Math.Abs(x) > Epsilon)
            {
                x *= PI;
                return Clean((float)Math.Sin(x) / x);
            }

            return 1.0f;
        }

        /// <summary>
        /// Ensures that any passed double is correctly rounded to zero
        /// </summary>
        /// <param name="x">The value to clean.</param>
        /// <returns>
        /// The <see cref="float"/>
        /// </returns>.
        private static float Clean(float x)
        {
            const float Epsilon = .00001f;

            if (Math.Abs(x) < Epsilon)
            {
                return 0f;
            }

            return x;
        }
    }
}
