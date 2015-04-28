﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YCbCrColor.cs" company="James South">
//   Copyright © James South and contributors.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Represents an YCbCr (luminance, chroma, chroma) color conforming to the
//   ITU-R BT.601 standard used in digital imaging systems.
//   <see href="http://en.wikipedia.org/wiki/YCbCr" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Represents an YCbCr (luminance, chroma, chroma) color conforming to the 
    /// ITU-R BT.601 standard used in digital imaging systems.
    /// <see href="http://en.wikipedia.org/wiki/YCbCr"/>
    /// </summary>
    public struct YCbCrColor : IEquatable<YCbCrColor>
    {
        /// <summary>
        /// Represents a <see cref="YCbCrColor"/> that has Y, Cb, and Cr values set to zero.
        /// </summary>
        public static readonly YCbCrColor Empty = new YCbCrColor();

        /// <summary>
        /// Holds the Y luminance component.
        /// <remarks>A value ranging between 0 and 255.</remarks>
        /// </summary>
        public float Y;

        /// <summary>
        /// Holds the Cb chroma component.
        /// <remarks>A value ranging between 0 and 255.</remarks>
        /// </summary>
        public float Cb;

        /// <summary>
        /// Holds the Cr chroma component.
        /// <remarks>A value ranging between 0 and 255.</remarks>
        /// </summary>
        public float Cr;

        /// <summary>
        /// The epsilon for comparing floating point numbers.
        /// </summary>
        private const float Epsilon = 0.0001f;

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCrColor"/> struct.
        /// </summary>
        /// <param name="y">The y luminance component.</param>
        /// <param name="cb">The cb chroma component.</param>
        /// <param name="cr">The cr chroma component.</param> 
        public YCbCrColor(float y, float cb, float cr)
        {
            this.Y = y.Clamp(0, 255);
            this.Cb = cb.Clamp(0, 255);
            this.Cr = cr.Clamp(0, 255);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="YCbCrColor"/> is empty.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsEmpty
        {
            get
            {
                return Math.Abs(this.Y) < Epsilon
                    && Math.Abs(this.Cb) < Epsilon
                    && Math.Abs(this.Cr) < Epsilon;
            }
        }

        /// <summary>
        /// Compares two <see cref="YCbCrColor"/> objects. The result specifies whether the values
        /// of the <see cref="YCbCrColor.Y"/>, <see cref="YCbCrColor.Cb"/>, and <see cref="YCbCrColor.Cr"/>
        /// properties of the two <see cref="YCbCrColor"/> objects are equal.
        /// </summary>
        /// <param name="left">
        /// The <see cref="YCbCrColor"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="YCbCrColor"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(YCbCrColor left, YCbCrColor right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="YCbCrColor"/> objects. The result specifies whether the values
        /// of the <see cref="YCbCrColor.Y"/>, <see cref="YCbCrColor.Cb"/>, and <see cref="YCbCrColor.Cr"/>
        /// properties of the two <see cref="YCbCrColor"/> objects are unequal.
        /// </summary>
        /// <param name="left">
        /// The <see cref="YCbCrColor"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="YCbCrColor"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(YCbCrColor left, YCbCrColor right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Allows the implicit conversion of an instance of <see cref="Color"/> to a 
        /// <see cref="YCbCrColor"/>.
        /// </summary>
        /// <param name="color">
        /// The instance of <see cref="Color"/> to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="YCbCrColor"/>.
        /// </returns>
        public static implicit operator YCbCrColor(Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            float y = (float)((0.299 * r) + (0.587 * g) + (0.114 * b));
            float cb = 128 + (float)((-0.168736 * r) - (0.331264 * g) + (0.5 * b));
            float cr = 128 + (float)((0.5 * r) - (0.418688 * g) - (0.081312 * b));

            return new YCbCrColor(y, cb, cr);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (obj is YCbCrColor)
            {
                YCbCrColor color = (YCbCrColor)obj;

                return Math.Abs(this.Y - color.Y) < Epsilon
                    && Math.Abs(this.Cb - color.Cb) < Epsilon
                    && Math.Abs(this.Cr - color.Cr) < Epsilon;
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return this.GetHashCode(this);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            if (this.IsEmpty)
            {
                return "YCbCrColor [Empty]";
            }

            return string.Format("YCbCrColor [ Y={0:#0.##}, Cb={1:#0.##}, Cr={2:#0.##}]", this.Y, this.Cb, this.Cr);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// True if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(YCbCrColor other)
        {
            return this.Y.Equals(other.Y)
                && this.Cb.Equals(other.Cb)
                && this.Cr.Equals(other.Cr);
        }

        /// <summary>
        /// Returns the hash code for the given instance.
        /// </summary>
        /// <param name="color">
        /// The instance of <see cref="Color"/> to return the hash code for.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        private int GetHashCode(YCbCrColor color)
        {
            unchecked
            {
                int hashCode = color.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ color.Cb.GetHashCode();
                hashCode = (hashCode * 397) ^ color.Cr.GetHashCode();
                return hashCode;
            }
        }
    }
}
