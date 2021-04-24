using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Renderer.Extensions;

namespace Renderer
{
	internal static class Renderer
	{
		internal static Camera Camera { get; set; }
		internal static Vector3[] Points { get; set; }
		internal static int[] Triangles { get; set; }
		internal static float RotationScale { get; set; }
		internal static float MovementScale { get; set; }
		internal static int Width { get; set; }
		internal static int Height { get; set; }
		internal static ColorF[] FrameBuffer { get; set; }
		internal static Texture2D FrameBufferTexture { get; set; }
		public unsafe static void Init(Options options)
		{
			RotationScale = 0.5f;
			MovementScale = 1;
			Camera = new Camera(new(0, 0, -5), new(0, 0, 0), 75, 0.3f, 10);
			Points = new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0), new(0, 1, 1), new(1, 1, 1), new(1, 0, 1), new(0, 0, 1) };
			Triangles = new int[] { 0, 2, 1, 0, 3, 2, 2, 3, 4, 2, 4, 5, 1, 2, 5, 1, 5, 6, 0, 7, 4, 0, 4, 3, 5, 4, 7, 5, 7, 6, 0, 6, 7, 0, 1, 6 };
			FrameBuffer = new ColorF[options.Width * options.Height];
			Array.Fill(FrameBuffer, ColorF.White);
			Image image = new Image { width = options.Width, height = options.Height, format = PixelFormat.UNCOMPRESSED_R32G32B32A32, mipmaps = 1 };
			fixed (ColorF* c = FrameBuffer)
			{
				image.data = (nint)c;
				FrameBufferTexture = Raylib.LoadTextureFromImage(image);
			}
		}
		public static void GameLoop(double dt)
		{
			HandleInput((float)dt);
			Width = Raylib.GetScreenWidth();
			Height = Raylib.GetScreenHeight();

			if (Width * Height != FrameBuffer.Length)
			{
				Raylib.UnloadTexture(FrameBufferTexture);
				unsafe
				{
					FrameBuffer = new ColorF[Width * Height];
					Array.Fill(FrameBuffer, ColorF.White);
					Image image = new Image { width = Width, height = Height, format = PixelFormat.UNCOMPRESSED_R32G32B32A32, mipmaps = 1 };
					fixed (ColorF* c = FrameBuffer)
					{
						image.data = (nint)c;
						FrameBufferTexture = Raylib.LoadTextureFromImage(image);
					}
				}
			}
			Matrix4x4 worldView = Matrix4x4.CreateTranslation(-Camera.Position) *
				Matrix4x4.CreateRotationZ(Camera.Rotation.Z * Raylib.DEG2RAD) *
				Matrix4x4.CreateRotationY((Camera.Rotation.Y + 180) * Raylib.DEG2RAD) *
				Matrix4x4.CreateRotationX(Camera.Rotation.X * Raylib.DEG2RAD);
			Matrix4x4 perspective =
				Matrix4x4.CreatePerspectiveFieldOfView(Camera.FieldOfView * Raylib.DEG2RAD, (float)Width / Height, Camera.NearClipping, Camera.FarClipping);
			Render(worldView, perspective);
			Raylib.DrawText($"Camera Pos: {Camera.Position:N1}", 0, 25, 20, Color.BLACK);
			Raylib.DrawText($"Camera Rot: {Camera.Rotation:N1}", 0, 50, 20, Color.BLACK);
			Raylib.DrawText($"Rotation Scale: {RotationScale:N1}", 0, 75, 20, Color.BLACK);
			Raylib.DrawText($"Movement Scale: {MovementScale:N1}", 0, 100, 20, Color.BLACK);
		}

		static void HandleInput(float dt)
		{
			if (Raylib.IsKeyReleased(KeyboardKey.KEY_F11))
			{
				if (!Raylib.IsWindowFullscreen())
				{
					Raylib.SetWindowSize(Raylib.GetMonitorWidth(0), Raylib.GetMonitorHeight(0));
					Raylib.ToggleFullscreen();
				}
				else
				{
					Raylib.ToggleFullscreen();
					Raylib.SetWindowSize(640, 360);
				}
				Width = Raylib.GetScreenWidth();
				Height = Raylib.GetScreenHeight();
				Raylib.SetMousePosition(Width / 2, Height / 2);
			}


			Vector3 newRot = Camera.Rotation;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_HOME))
				RotationScale += 0.5f * dt;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_END))
				RotationScale -= 0.5f * dt;
			RotationScale = Math.Clamp(RotationScale, 0, 2);

			Vector2 center = new(Width / 2, Height / 2);
			Vector2 mouse = Raylib.GetMousePosition();
			if (Raylib.IsWindowFocused())
			{
				Raylib.HideCursor();
				Raylib.SetMousePosition(Width / 2, Height / 2);
				Vector2 offset = mouse - center;
				offset *= RotationScale;
				newRot += new Vector3(offset.Y, offset.X, 0);
			}
			else
				Raylib.ShowCursor();

			Vector3 newPos = Camera.Position;
			Vector3 dir = default;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_PAGE_UP))
				MovementScale += 0.5f * dt;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_PAGE_DOWN))
				MovementScale -= 0.5f * dt;
			MovementScale = Math.Clamp(MovementScale, 0, 3);

			if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
				dir.Z += 1;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
				dir.Z -= 1;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
				dir.X -= 1;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
				dir.X += 1;

			newPos += dt * MovementScale * new Vector3(dir.Z * MathF.Sin(-newRot.Y * Raylib.DEG2RAD) * MathF.Cos(-newRot.X * Raylib.DEG2RAD) + dir.X * MathF.Cos(-newRot.Y * Raylib.DEG2RAD), dir.Z * MathF.Sin(-newRot.X * Raylib.DEG2RAD), dir.Z * MathF.Cos(-newRot.Y * Raylib.DEG2RAD) * MathF.Cos(-newRot.X * Raylib.DEG2RAD) - dir.X * MathF.Sin(-newRot.Y * Raylib.DEG2RAD));

			if (newRot.Y > 180)
				newRot.Y -= 360;
			if (newRot.Y < -180)
				newRot.Y += 360;
			newRot.X = Math.Clamp(newRot.X, -90, 90);
			if (Raylib.IsKeyDown(KeyboardKey.KEY_Z))
			{
				newPos = new(0, 0, -5);
				newRot = default;
			}
			Camera = new Camera(newPos, newRot, Camera.FieldOfView, Camera.NearClipping, Camera.FarClipping);
		}

		unsafe static void Render(Matrix4x4 worldView, Matrix4x4 perspective)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool InBounds(Vector4 v) => v.X >= -1 && v.X <= 1 && v.Y >= -1 && v.Y <= 1 && v.Z >= -1 && v.Z <= 1 && v.W > 0;
			Vector4[] projected = ProjectToPlane(Points, worldView, perspective);

			float[] zBuffer = new float[Width * Height];
			Array.Fill(zBuffer, Camera.FarClipping);
			Array.Fill(FrameBuffer, ColorF.Transparent);
			Parallel.For(0, Triangles.Length / 3, i =>
			{
				Vector2 p1, p2, p3;
				int t1, t2, t3;
				t1 = Triangles[i * 3 + 0];
				t2 = Triangles[i * 3 + 1];
				t3 = Triangles[i * 3 + 2];
				p1 = new Vector2(projected[t1].X, projected[t1].Y);
				p2 = new Vector2(projected[t2].X, projected[t2].Y);
				p3 = new Vector2(projected[t3].X, projected[t3].Y);

				float xMin, xMax, yMin, yMax;
				xMin = Math.Clamp(Math.Min(Math.Min(p1.X, p2.X), p3.X), -1, 1);
				yMin = Math.Clamp(Math.Min(Math.Min(p1.Y, p2.Y), p3.Y), -1, 1);
				xMax = Math.Clamp(Math.Max(Math.Max(p1.X, p2.X), p3.X), -1, 1);
				yMax = Math.Clamp(Math.Max(Math.Max(p1.Y, p2.Y), p3.Y), -1, 1);
				int xMinI = Math.Clamp((int)Math.Floor(Width * (xMin + 1) / 2), 0, Width - 1);
				int xMaxI = Math.Clamp((int)Math.Ceiling(Width * (xMax + 1) / 2), 0, Width - 1);
				int yMaxI = Math.Clamp((int)Math.Ceiling(Height - Height * (yMin + 1) / 2), 0, Height - 1);
				int yMinI = Math.Clamp((int)Math.Floor(Height - Height * (yMax + 1) / 2), 0, Height - 1);
				Parallel.For(xMinI, xMaxI + 1, x =>
				{
					for (int y = yMinI; y <= yMaxI; ++y)
					{
						Vector2 p = new Vector2(2 * (x + 0.5f) / Width - 1, 2 * (Height - y + 0.5f) / Height - 1);
						if (PointInTriangle(p, p1, p2, p3, out float w1, out float w2, out float w3))
						{
							Vector4 v1, v2, v3;
							v1 = projected[t1];
							v2 = projected[t2];
							v3 = projected[t3];
							Vector4 v = w1 * v1 + w2 * v2 + w3 * v3;
							if (InBounds(v) && v.W < zBuffer[x + y * Width])
							{
								zBuffer[x + y * Width] = v.W;
								FrameBuffer[x + y * Width] = new ColorF(w1, w2, w3) * new Vector3(1 / (w1 * v1.Z + w2 * v2.Z + w3 * v3.Z));
							}
						}
						//else if (FrameBuffer[x + y * Width] == ColorF.White)
						//	FrameBuffer[x + y * Width] = new ColorF(1, 0, 0, 1);
					}
				});
			});
			fixed (void* c = FrameBuffer)
			{
				Raylib.UpdateTexture(FrameBufferTexture, (IntPtr)c);
				Raylib.DrawTexture(FrameBufferTexture, 0, 0, Color.WHITE);
			}
			for (int i = 0; i < projected.Length; ++i)
			{
				Vector4 v = projected[i];
				if (InBounds(v))
					Raylib.DrawCircleV(new Vector2(0, Height) - new Vector2(-Width, Height) * (new Vector2(v.X, v.Y) + new Vector2(1, 1)) / 2, 3, Color.BLACK);
				Raylib.DrawText($"{v:N3}", 0, 125 + 25 * i, 20, Color.BLACK);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float EdgeFunction(Vector2 a, Vector2 b, Vector2 c) => (c.X - a.X) * (b.Y - a.Y) - (c.Y - a.Y) * (b.X - a.X);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool PointInTriangle(Vector2 p, Vector2 v0, Vector2 v1, Vector2 v2, out float w0, out float w1, out float w2)
		{
			float area = EdgeFunction(v0, v1, v2);
			w0 = EdgeFunction(v1, v2, p);
			w1 = EdgeFunction(v2, v0, p);
			w2 = EdgeFunction(v0, v1, p);
			bool b = w0 <= 0 && w1 <= 0 && w2 <= 0;
			if (b)
			{
				w0 /= area;
				w1 /= area;
				w2 /= area;
			}
			return b;
		}

		static Vector4[] ProjectToPlane(Vector3[] points, Matrix4x4 worldView, Matrix4x4 perspective)
		{
			Vector4[] projected = new Vector4[Points.Length];
			for (int i = 0; i < projected.Length; ++i)
			{
				Vector4 point = Vector4.Transform(new Vector4(points[i], 1), worldView);
				point = Vector4.Transform(point, perspective);
				projected[i] = new Vector4(point.X / point.W, point.Y / point.W, point.Z / point.W * 2 - 1, point.W);
			}
			return projected;
		}
	}
}
