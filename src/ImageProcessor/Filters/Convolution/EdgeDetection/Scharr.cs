﻿// <copyright file="Scharr.cs" company="James South">
// Copyright (c) James South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor.Filters
{
    /// <summary>
    /// The Scharr operator filter.
    /// <see href="http://en.wikipedia.org/wiki/Sobel_operator#Alternative_operators"/>
    /// </summary>
    public class Scharr : Convolution2DFilter
    {
        /// <summary>
        /// Gets the horizontal gradient operator.
        /// </summary>
        public override float[,] KernelX => new float[,]
        {
            { -3, 0, 3 },
            { -10, 0, 10 },
            { -3, 0, 3 }
        };

        /// <summary>
        /// Gets the vertical gradient operator.
        /// </summary>
        public override float[,] KernelY => new float[,]
        {
            { 3, 10, 3 },
            { 0, 0, 0 },
            { -3, -10, -3 }
        };
    }
}
