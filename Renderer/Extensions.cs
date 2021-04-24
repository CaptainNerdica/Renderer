using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
	internal static class Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Lerp(Vector3 a, Vector3 b, float v) => new Vector3(Lerp(a.X, b.X, v), Lerp(a.Y, b.Y, v), Lerp(a.Z, b.Z, v));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Lerp(float a, float b, float v) => (1 - v) * a + v * b;
	}
}
