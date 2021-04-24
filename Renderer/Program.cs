using CommandLine;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{

	public record Options
	{
		[Option('f', "file", HelpText = "The geometry file to use for rendering", Required = true)]
		public string File { get; set; }
		[Option('w', "width", HelpText = "The width of the window", Default = 640)]
		public int Width { get; set; }
		[Option('h', "height", HelpText = "The height of the window", Default = 360)]
		public int Height { get; set; }
		[Option('r', "fps", HelpText = "Target framerate of the window", Default = 60)]
		public int FPS { get; set; }
	}
	static class Program
	{
		static void Main(string[] args) => Parser.Default.ParseArguments<Options>(args).WithParsed(MainWithOptions);

		static void MainWithOptions(Options options)
		{
			Raylib.InitWindow(options.Width, options.Height, "Renderer");
			Raylib.SetTargetFPS(options.FPS);
			Raylib.SetWindowState(ConfigFlag.FLAG_WINDOW_RESIZABLE);
			Renderer.Init(options);
			Stopwatch sw = Stopwatch.StartNew();
			while (!Raylib.WindowShouldClose())
			{
				TimeSpan dt = sw.Elapsed;
				sw.Restart();
				Loop(dt.TotalSeconds);
			}
			Raylib.CloseWindow();
		}

		static void Loop(double dt)
		{
			Raylib.BeginDrawing();
			Raylib.ClearBackground(Color.WHITE);
			Renderer.GameLoop(dt);
			Raylib.DrawFPS(0, 0);
			Raylib.EndDrawing();
		}
	}
}
