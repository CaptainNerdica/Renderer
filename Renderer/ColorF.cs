using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using E = Renderer.Extensions;

namespace Renderer
{
	public struct ColorF : IEquatable<ColorF>
	{
		public float R { get; set; }
		public float G { get; set; }
		public float B { get; set; }
		public float A { get; set; }

		public ColorF(float r, float g, float b, float a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public ColorF(float r, float g, float b) : this(r, g, b, 1) { }

		public ColorF(Vector3 vec) : this(vec.X, vec.Y, vec.Z) { }

		public ColorF(Vector4 vec) : this(vec.X, vec.Y, vec.Z, vec.W) { }

		public static readonly ColorF Transparent = new ColorF(1f, 1f, 1f, 0f);
		public static readonly ColorF White = new ColorF(1f, 1f, 1f, 1f);
		public static readonly ColorF Black = new ColorF(0f, 0f, 0f, 1f);
		public static readonly ColorF Gray = new ColorF(0.5f, 0.5f, 0.5f, 1f);

		public static ColorF Lerp(ColorF a, ColorF b, float v) => new ColorF(E.Lerp(a.R, b.R, v), E.Lerp(a.G, b.G, v), E.Lerp(a.B, b.B, v), E.Lerp(a.A, b.A, v));


		public static ColorF operator *(ColorF c, float v) => new ColorF(c.R * v, c.G * v, c.B * v, c.A * v);
		public static ColorF operator *(ColorF l, ColorF r) => new ColorF(l.R * r.R, l.G * r.G, l.B * r.B, l.A * r.B);
		public static ColorF operator /(ColorF c, float v) => new ColorF(c.R / v, c.G * v, c.B / v, c.A / v);
		public static ColorF operator /(ColorF l, ColorF r) => new ColorF(l.R / r.R, l.G * r.G, l.B / r.B, l.A / r.B);

		public static implicit operator ColorF(Vector3 v) => new ColorF(v);
		public static implicit operator ColorF(Vector4 v) => new ColorF(v);
		public static explicit operator Vector3(ColorF c) => new Vector3(c.R, c.G, c.B);
		public static explicit operator Vector4(ColorF c) => new Vector4(c.R, c.G, c.B, c.A);

		public static bool operator ==(ColorF l, ColorF r) => l.R == r.R && l.G == r.G && l.B == r.B && l.A == r.A;
		public static bool operator !=(ColorF l, ColorF r) => !(l == r);


		public override bool Equals(object obj) => obj is ColorF c && Equals(c);
		public bool Equals(ColorF other) => this == other;

		public override int GetHashCode() => HashCode.Combine(R, G, B, A);
	}
}
