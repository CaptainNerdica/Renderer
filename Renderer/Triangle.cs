using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
	public struct Triangle
	{
		public Vector3 Point1 { get; set; }
		public Vector3 Point2 { get; set; }
		public Vector3 Point3 { get; set; }

		public Triangle(Vector3 point1, Vector3 point2, Vector3 point3)
		{
			Point1 = point1;
			Point2 = point2;
			Point3 = point3;
		}

		public static implicit operator Triangle((Vector3 p1, Vector3 p2, Vector3 p3) t) => new Triangle(t.p1, t.p2, t.p3);
		public static implicit operator Triangle((
			(float x, float y, float z) p1,
			(float x, float y, float z) p2,
			(float x, float y, float z) p3) t) => new Triangle(
				new Vector3(t.p1.x, t.p1.y, t.p1.z),
				new Vector3(t.p2.x, t.p2.y, t.p2.z),
				new Vector3(t.p3.x, t.p3.y, t.p3.z));
	}
}