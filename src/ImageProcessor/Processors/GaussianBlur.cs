﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GaussianBlur.cs" company="James South">
//   Copyright (c) James South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Applies a Gaussian blur to the image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Processors
{
    using System.Collections.Generic;
    using System.Drawing;
    using ImageProcessor.Imaging;

    /// <summary>
    /// Applies a Gaussian blur to the image.
    /// </summary>
    public class GaussianBlur : IGraphicsProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianBlur"/> class.
        /// </summary>
        public GaussianBlur()
        {
            this.Settings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the DynamicParameter.
        /// </summary>
        public dynamic DynamicParameter { get; set; }

        /// <summary>
        /// Gets or sets any additional settings required by the processor.
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="factory">The current instance of the <see cref="T:ImageProcessor.ImageFactory" /> class containing
        /// the image to process.</param>
        /// <returns>
        /// The processed image from the current instance of the <see cref="T:ImageProcessor.ImageFactory" /> class.
        /// </returns>
        public Image ProcessImage(ImageFactory factory)
        {
            Bitmap newImage = null;
            Bitmap image = (Bitmap)factory.Image;

            try
            {
                newImage = new Bitmap(image);
                GaussianLayer gaussianLayer = this.DynamicParameter;

                Convolution convolution = new Convolution(gaussianLayer.Sigma) { Threshold = gaussianLayer.Threshold };
                double[,] kernel = convolution.CreateGuassianBlurFilter(gaussianLayer.Size);
                newImage = convolution.ProcessKernel(newImage, kernel);

                image.Dispose();
                image = newImage;
            }
            catch
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }
            }

            return image;
        }
    }
}