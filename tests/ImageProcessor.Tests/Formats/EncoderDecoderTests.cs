﻿namespace ImageProcessor.Tests
{
    using System.Diagnostics;
    using System.IO;

    using Formats;

    using Xunit;

    public class EncoderDecoderTests
    {
        [Theory]
        [InlineData("../../TestImages/Formats/Jpg/Backdrop.jpg")]
        [InlineData("../../TestImages/Formats/Gif/a.gif")]
        [InlineData("../../TestImages/Formats/Gif/leaf.gif")]
        [InlineData("../../TestImages/Formats/Gif/ani.gif")]
        [InlineData("../../TestImages/Formats/Gif/ani2.gif")]
        [InlineData("../../TestImages/Formats/Gif/giphy.gif")]
        [InlineData("../../TestImages/Formats/Bmp/Car.bmp")]
        [InlineData("../../TestImages/Formats/Png/cmyk.png")]
        public void DecodeThenEncodeImageFromStreamShouldSucceed(string filename)
        {
            if (!Directory.Exists("Encoded"))
            {
                Directory.CreateDirectory("Encoded");
            }

            FileStream stream = File.OpenRead(filename);
            Stopwatch watch = Stopwatch.StartNew();
            Image image = new Image(stream);

            string encodedFilename = "Encoded/" + Path.GetFileName(filename);

            using (FileStream output = File.OpenWrite(encodedFilename))
            {
                image.Save(output);
            }

            Trace.WriteLine($"{filename} : {watch.ElapsedMilliseconds}ms");
        }

        [Theory]
        [InlineData("../../TestImages/Formats/Jpg/Backdrop.jpg")]
        [InlineData("../../TestImages/Formats/Bmp/Car.bmp")]
        [InlineData("../../TestImages/Formats/Png/cmyk.png")]
        public void QuantizedImageShouldPreserveMaximumColorPrecision(string filename)
        {
            if (!Directory.Exists("Quantized"))
            {
                Directory.CreateDirectory("Quantized");
            }

            Image image = new Image(File.OpenRead(filename));
            IQuantizer quantizer = new OctreeQuantizer();
            QuantizedImage quantizedImage = quantizer.Quantize(image);

            using (FileStream output = File.OpenWrite($"Quantized/{ Path.GetFileName(filename) }"))
            {
                quantizedImage.ToImage().Save(output);
            }
        }
    }
}