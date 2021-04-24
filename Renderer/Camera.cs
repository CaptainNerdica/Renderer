using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
	internal struct Camera
	{
		public float NearClipping { get; set; }
		public float FarClipping { get; set; }
		public float FieldOfView { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }

		public Camera(Vector3 position, Vector3 rotation, float fov, float nearClipping, float farClipping)
		{
			Position = position;
			Rotation = rotation;
			FieldOfView = fov;
			NearClipping = nearClipping;
			FarClipping = farClipping;
		}
	}
}
