﻿// <copyright file="IImageFormat.cs" company="James South">
// Copyright (c) James South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor.Formats
{
    /// <summary>
    /// Encapsulates a supported image format, providing means to encode and decode an image.
    /// </summary>
    public interface IImageFormat
    {
        /// <summary>
        /// Gets the image encoder for encoding an image from a stream.
        /// </summary>
        IImageEncoder Encoder { get; }

        /// <summary>
        /// Gets the image decoder for decoding an image from a stream.
        /// </summary>
        IImageDecoder Decoder { get; }
    }
}
