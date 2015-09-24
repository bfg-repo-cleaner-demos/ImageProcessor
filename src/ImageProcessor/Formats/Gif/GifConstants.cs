﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GifConstants.cs" company="James South">
//   Copyright © James South and contributors.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Constants that define specific points within a gif.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Formats
{
    /// <summary>
    /// Constants that define specific points within a gif.
    /// </summary>
    internal sealed class GifConstants
    {
        /// <summary>
        /// The maximum comment length.
        /// </summary>
        public const int MaxCommentLength = 1024 * 8;

        /// <summary>
        /// The extension block introducer <value>!</value>.
        /// </summary>
        public const byte ExtensionIntroducer = 0x21;

        /// <summary>
        /// The terminator.
        /// </summary>
        public const byte Terminator = 0;

        /// <summary>
        /// The image label introducer <value>,</value>.
        /// </summary>
        public const byte ImageLabel = 0x2C;

        /// <summary>
        /// The end introducer trailer <value>;</value>.
        /// </summary>
        public const byte EndIntroducer = 0x3B;

        /// <summary>
        /// The application extension label.
        /// </summary>
        public const byte ApplicationExtensionLabel = 0xFF;

        /// <summary>
        /// The comment label.
        /// </summary>
        public const byte CommentLabel = 0xFE;

        /// <summary>
        /// The image descriptor label <value>,</value>.
        /// </summary>
        public const byte ImageDescriptorLabel = 0x2C;

        /// <summary>
        /// The plain text label.
        /// </summary>
        public const byte PlainTextLabel = 0x01;

        /// <summary>
        /// The graphic control label.
        /// </summary>
        public const byte GraphicControlLabel = 0xF9;
    }
}
