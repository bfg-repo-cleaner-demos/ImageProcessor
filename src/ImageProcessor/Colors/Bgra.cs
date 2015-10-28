﻿// <copyright file="Bgra.cs" company="James South">
// Copyright (c) James South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents an BGRA (blue, green, red, alpha) color.
    /// </summary>
    /// <remarks>
    /// This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
    /// as it avoids the need to create new values for modification operations.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public struct Bgra : IEquatable<Bgra>
    {
        /// <summary>
        /// Represents a <see cref="Bgra"/> that has B, G, R, and A values set to zero.
        /// </summary>
        public static readonly Bgra Empty = default(Bgra);

        /// <summary>
        /// Represents a transparent <see cref="Bgra"/> that has B, G, R, and A values set to 255, 255, 255, 0.
        /// </summary>
        public static readonly Bgra Transparent = new Bgra(255, 255, 255, 0);

        /// <summary>
        /// Represents a black <see cref="Bgra"/> that has B, G, R, and A values set to 0, 0, 0, 0.
        /// </summary>
        public static readonly Bgra Black = new Bgra(0, 0, 0, 255);

        /// <summary>
        /// Represents a white <see cref="Bgra"/> that has B, G, R, and A values set to 255, 255, 255, 255.
        /// </summary>
        public static readonly Bgra White = new Bgra(255, 255, 255, 255);

        /// <summary>
        /// Holds the blue component of the color
        /// </summary>
        [FieldOffset(0)]
        public readonly byte B;

        /// <summary>
        /// Holds the green component of the color
        /// </summary>
        [FieldOffset(1)]
        public readonly byte G;

        /// <summary>
        /// Holds the red component of the color
        /// </summary>
        [FieldOffset(2)]
        public readonly byte R;

        /// <summary>
        /// Holds the alpha component of the color
        /// </summary>
        [FieldOffset(3)]
        public readonly byte A;

        /// <summary>
        /// Permits the <see cref="Bgra"/> to be treated as a 32 bit integer.
        /// </summary>
        [FieldOffset(0)]
        public readonly int BGRA;

        /// <summary>
        /// The epsilon for comparing floating point numbers.
        /// </summary>
        private const float Epsilon = 0.0001f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgra"/> struct.
        /// </summary>
        /// <param name="b">
        /// The blue component of this <see cref="Bgra"/>.
        /// </param>
        /// <param name="g">
        /// The green component of this <see cref="Bgra"/>.
        /// </param>
        /// <param name="r">
        /// The red component of this <see cref="Bgra"/>.
        /// </param>
        public Bgra(byte b, byte g, byte r)
            : this()
        {
            this.B = b;
            this.G = g;
            this.R = r;
            this.A = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgra"/> struct.
        /// </summary>
        /// <param name="b">
        /// The blue component of this <see cref="Bgra"/>.
        /// </param>
        /// <param name="g">
        /// The green component of this <see cref="Bgra"/>.
        /// </param>
        /// <param name="r">
        /// The red component of this <see cref="Bgra"/>.
        /// </param>
        /// <param name="a">
        /// The alpha component of this <see cref="Bgra"/>.
        /// </param>
        public Bgra(byte b, byte g, byte r, byte a)
            : this()
        {
            this.B = b;
            this.G = g;
            this.R = r;
            this.A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgra"/> struct.
        /// </summary>
        /// <param name="bgra">
        /// The combined color components.
        /// </param>
        public Bgra(int bgra)
            : this()
        {
            this.BGRA = bgra;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgra"/> struct.
        /// </summary>
        /// <param name="hex">
        /// The hexadecimal representation of the combined color components arranged
        /// in rgb, rrggbb, or aarrggbb format to match web syntax.
        /// </param>
        public Bgra(string hex)
            : this()
        {
            // Hexadecimal representations are layed out AARRGGBB to we need to do some reordering.
            hex = hex.StartsWith("#") ? hex.Substring(1) : hex;

            if (hex.Length != 8 && hex.Length != 6 && hex.Length != 3)
            {
                throw new ArgumentException("Hexadecimal string is not in the correct format.", nameof(hex));
            }

            if (hex.Length == 8)
            {
                this.B = Convert.ToByte(hex.Substring(6, 2), 16);
                this.G = Convert.ToByte(hex.Substring(4, 2), 16);
                this.R = Convert.ToByte(hex.Substring(2, 2), 16);
                this.A = Convert.ToByte(hex.Substring(0, 2), 16);
            }
            else if (hex.Length == 6)
            {
                this.B = Convert.ToByte(hex.Substring(4, 2), 16);
                this.G = Convert.ToByte(hex.Substring(2, 2), 16);
                this.R = Convert.ToByte(hex.Substring(0, 2), 16);
                this.A = 255;
            }
            else
            {
                string b = char.ToString(hex[2]);
                string g = char.ToString(hex[1]);
                string r = char.ToString(hex[0]);

                this.B = Convert.ToByte(b + b, 16);
                this.G = Convert.ToByte(g + g, 16);
                this.R = Convert.ToByte(r + r, 16);
                this.A = 255;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Bgra"/> is empty.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsEmpty => this.B == 0 && this.G == 0 && this.R == 0 && this.A == 0;

        /// <summary>
        /// Allows the implicit conversion of an instance of <see cref="ColorVector"/> to a
        /// <see cref="Bgra"/>.
        /// </summary>
        /// <param name="color">
        /// The instance of <see cref="ColorVector"/> to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Bgra"/>.
        /// </returns>
        public static implicit operator Bgra(ColorVector color)
        {
            return new Bgra((255 * color.B).ToByte(), (255 * color.G).ToByte(), (255 * color.R).ToByte(), (255 * color.A).ToByte());
        }

        /// <summary>
        /// Allows the implicit conversion of an instance of <see cref="Hsv"/> to a
        /// <see cref="Bgra"/>.
        /// </summary>
        /// <param name="color">
        /// The instance of <see cref="Hsv"/> to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Bgra"/>.
        /// </returns>
        public static implicit operator Bgra(Hsv color)
        {
            float s = color.S / 100;
            float v = color.V / 100;

            if (Math.Abs(s) < Epsilon)
            {
                byte component = (byte)(v * 255);
                return new Bgra(component, component, component, 255);
            }

            float h = (Math.Abs(color.H - 360) < Epsilon) ? 0 : color.H / 60;
            int i = (int)Math.Truncate(h);
            float f = h - i;

            float p = v * (1.0f - s);
            float q = v * (1.0f - (s * f));
            float t = v * (1.0f - (s * (1.0f - f)));

            float r, g, b;
            switch (i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;

                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;

                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;

                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;

                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;

                default:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }

            return new Bgra((byte)Math.Round(b * 255), (byte)Math.Round(g * 255), (byte)Math.Round(r * 255));
        }

        /// <summary>
        /// Allows the implicit conversion of an instance of <see cref="YCbCr"/> to a
        /// <see cref="Bgra"/>.
        /// </summary>
        /// <param name="color">
        /// The instance of <see cref="YCbCr"/> to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Bgra"/>.
        /// </returns>
        public static implicit operator Bgra(YCbCr color)
        {
            float y = color.Y;
            float cb = color.Cb - 128;
            float cr = color.Cr - 128;

            byte b = (y + (1.772 * cb)).ToByte();
            byte g = (y - (0.34414 * cb) - (0.71414 * cr)).ToByte();
            byte r = (y + (1.402 * cr)).ToByte();

            return new Bgra(b, g, r, 255);
        }

        /// <summary>
        /// Allows the implicit conversion of an instance of <see cref="Cmyk"/> to a
        /// <see cref="Bgra"/>.
        /// </summary>
        /// <param name="cmykColor">
        /// The instance of <see cref="Cmyk"/> to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="Bgra"/>.
        /// </returns>
        public static implicit operator Bgra(Cmyk cmykColor)
        {
            int red = Convert.ToInt32((1 - (cmykColor.C / 100)) * (1 - (cmykColor.K / 100)) * 255.0);
            int green = Convert.ToInt32((1 - (cmykColor.M / 100)) * (1 - (cmykColor.K / 100)) * 255.0);
            int blue = Convert.ToInt32((1 - (cmykColor.Y / 100)) * (1 - (cmykColor.K / 100)) * 255.0);
            return new Bgra(blue.ToByte(), green.ToByte(), red.ToByte());
        }

        /// <summary>
        /// Compares two <see cref="Bgra"/> objects. The result specifies whether the values
        /// of the <see cref="Bgra.B"/>, <see cref="Bgra.G"/>, <see cref="Bgra.R"/>, and <see cref="Bgra.A"/>
        /// properties of the two <see cref="Bgra"/> objects are equal.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Bgra"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Bgra"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Bgra left, Bgra right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Bgra"/> objects. The result specifies whether the values
        /// of the <see cref="Bgra.B"/>, <see cref="Bgra.G"/>, <see cref="Bgra.R"/>, and <see cref="Bgra.A"/>
        /// properties of the two <see cref="Bgra"/> objects are unequal.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Bgra"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Bgra"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Bgra left, Bgra right)
        {
            return !left.Equals(right);
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
            if (obj is Bgra)
            {
                Bgra color = (Bgra)obj;

                return this.BGRA == color.BGRA;
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
            unchecked
            {
                int hashCode = this.B.GetHashCode();
                hashCode = (hashCode * 397) ^ this.G.GetHashCode();
                hashCode = (hashCode * 397) ^ this.R.GetHashCode();
                hashCode = (hashCode * 397) ^ this.A.GetHashCode();
                return hashCode;
            }
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
                return "Color [ Empty ]";
            }

            return $"Color [ B={this.B}, G={this.G}, R={this.R}, A={this.A} ]";
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// True if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Bgra other)
        {
            return this.BGRA == other.BGRA;
        }
    }
}
