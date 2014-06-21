﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LomographMatrixFilter.cs" company="James South">
//   Copyright (c) James South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Encapsulates methods with which to add a lomograph filter to an image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Imaging.Filters
{
    #region Using
    using System.Drawing;
    using System.Drawing.Imaging;
    using ImageProcessor.Processors;
    #endregion

    /// <summary>
    /// Encapsulates methods with which to add a lomograph filter to an image.
    /// </summary>
    internal class LomographMatrixFilter : MatrixFilterBase
    {
        /// <summary>
        /// Gets the <see cref="T:System.Drawing.Imaging.ColorMatrix"/> for this filter instance.
        /// </summary>
        public override ColorMatrix Matrix
        {
            get { return ColorMatrixes.Lomograph; }
        }

        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="factory">
        /// The current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class containing
        /// the image to process.
        /// </param>
        /// <param name="image">The current image to process</param>
        /// <param name="newImage">The new Image to return</param>
        /// <returns>
        /// The processed image from the current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class.
        /// </returns>
        public override Image TransformImage(ImageFactory factory, Image image, Image newImage)
        {
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(this.Matrix);

                    Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);
                    graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            // Add a vignette to finish the effect.
            factory.Image = newImage;
            Vignette vignette = new Vignette();
            newImage = vignette.ProcessImage(factory);

            // Reassign the image.
            image.Dispose();
            image = newImage;

            return image;
        }
    }
}
