using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenderDemo
{
	/// <summary>
	/// Arguments passed to this application.
	/// </summary>
	public static class Args
	{

		#region Private Memebers

		/// <summary>
		/// Arguments to parse.
		/// </summary>
		private static List<string> mArgs;

		/// <summary>
		/// How many ticks the demo has.
		/// </summary>
		private static int mDemoLength;

		#endregion


		#region Public Properties

		/// <summary>
		/// Directory where SourceDemoRender is.
		/// </summary>
		public static string SdrDirectory { get; private set; }

		/// <summary>
		/// Path to TF2's hl2.exe.
		/// </summary>
		public static string ExePath { get; private set; }

		/// <summary>
		/// Directory where TF2 generates logs (game directory).
		/// </summary>
		public static string TfDirectory { get; private set; }

		/// <summary>
		/// Demo to render out.
		/// </summary>
		public static string DemoPath { get; private set; }

		/// <summary>
		/// Tick to start recording at.
		/// </summary>
		public static int StartTick { get; private set; }

		/// <summary>
		/// Tick to stop recording at.
		/// </summary>
		public static int EndTick { get; private set; }

		/// <summary>
		/// Path of the movie to be rendered.
		/// </summary>
		public static string OutputPath { get; private set; }

		/// <summary>
		/// Path of the cfg profile file.
		/// </summary>
		public static string ProfileName { get; private set; } = "both";

		/// <summary>
		/// Extra commands to execute before recording.
		/// </summary>
		public static string UserCommands { get; private set; } = String.Empty;

		/// <summary>
		/// Extra launch options to pass to TF2.
		/// </summary>
		public static string LaunchOptions { get; private set; } = String.Empty;

		/// <summary>
		/// If to restore TF2 window to normal position after launch.
		/// </summary>
		public static bool ShowWindow { get; private set; } = true;

		/// <summary>
		/// The action when additional user input is required.
		/// </summary>
		public static OverwriteAction OverwriteAction { get; private set; } = OverwriteAction.Ask;

		/// <summary>
		/// What to do with TF2's window after it launches.
		/// </summary>
		public static WindowState WindowState { get; private set; } = WindowState.Fixed;

		/// <summary>
		/// What output to show to the user.
		/// </summary>
		public static LogLevel LogLevel { get; private set; } = LogLevel.Info;

		/// <summary>
		/// In test mode, application pauses to let user check if his input is correct.
		/// </summary>
		public static bool IsTest { get; private set; } = false;


		#endregion


		#region Public Methods

		public static void Parse(string[] args)
		{
			mArgs = args.ToList();

			GetSdrDirectory();
			GetExePath();
			GetTfDirectory();
			GetDemoPath();
			GetStartTick();
			GetEndTick();
			GetOutputPath();
			GetProfile();
			GetUserCommands();
			GetLaunchOptions();
			GetOverwriteAction();
			GetWindowState();
			GetLogLevel();

			foreach (var arg in mArgs)
			{
				switch (arg)
				{
					case "-test":
						IsTest = true;
						continue;
					default:
						throw new InvalidInputException($"Invalid input: Unexpected argument '{arg}'.");
				}
			}
		}

		#endregion


		#region Private Helpers

		/// <summary>
		/// Get value of a input argument. Remove them from the arguments list.
		/// </summary>
		/// <param name="arg">The argument.</param>
		/// <returns>The value.</returns>
		private static string GetArgValue(string arg)
		{
			var index = mArgs.IndexOf(arg);

			if (index == -1)
				return null;

			if (index == mArgs.Count - 1)
				throw new InvalidInputException($"Invalid input: Missing value for '{mArgs[index]}'.");


			var value = mArgs[index + 1];

			// Remove parsed args.
			mArgs.RemoveAt(index);
			mArgs.RemoveAt(index);

			return value;
		}

		#endregion


		#region Private "Get X Argument" Functions

		/// <summary>
		/// Get value for <see cref="SdrDirectory"/> and make sure is's valid.
		/// </summary>
		private static void GetSdrDirectory()
		{
			string sdrDirectory = GetArgValue("-sdrdir");

			// If not specified.
			if (sdrDirectory == null)
			{
				// If SDR isn't in the working directory, error.
				if (!File.Exists(@"SDR\LauncherCLI.exe") || !File.Exists(@"SDR\Extensions\Enabled\MultiProcess.dll"))
					throw new InvalidInputException($"Couldn't find SourceDemoRender, try specifying its path with '-sdrdir path'.");

				SdrDirectory = Path.GetFullPath("SDR");
				return;
			}

			try
			{
				sdrDirectory = Path.GetFullPath(sdrDirectory);
			}
			// Invalid file path characters.
			catch (ArgumentException)
			{
				throw new InvalidInputException($"Invalid -sdrdir value: '{sdrDirectory}' is not a valid directory path.");
			}

			// Does the directory exist?
			if (!Directory.Exists(sdrDirectory))
				throw new InvalidInputException($"Invalid -sdrdir value: Couldn't find '{sdrDirectory}'.");

			// Does LauncherCLI.exe exist?
			var launcher = Path.Combine(sdrDirectory, "LauncherCLI.exe");
			if (!File.Exists(launcher))
				throw new InvalidInputException($"Invalid -sdrdir value: Couldn't find 'LauncherCLI.exe' in '{sdrDirectory}'.");

			// Is MultiProcess.dll enabled?
			var extension = Path.Combine(sdrDirectory, @"Extensions\Enabled\MultiProcess.dll");
			if (!File.Exists(extension))
				throw new InvalidInputException($"Invalid -sdrdir value: SDR MultiProcess extension is not enabled.");

			SdrDirectory = sdrDirectory;
		}

		/// <summary>
		/// Get value for <see cref="ExePath"/> and make sure is's valid.
		/// </summary>
		private static void GetExePath()
		{
			string exePath = GetArgValue("-exepath");

			if (exePath == null)
				throw new InvalidInputException("Invalid input: -exepath must be set.");

			if (Path.GetFileName(exePath) != "hl2.exe")
				throw new InvalidInputException("Invalid -exepath value: File name must be 'hl2.exe'.");

			if (!File.Exists(exePath))
				throw new InvalidInputException($"Invalid -exepath value: Couldn't find '{exePath}'.");

			ExePath = exePath;
		}

		/// <summary>
		/// Get value for <see cref="TfDirectory"/> and make sure is's valid.
		/// </summary>
		private static void GetTfDirectory()
		{
			string tfDirectory = GetArgValue("-tfdir");

			// If not default tf directory.
			if (tfDirectory == null)
				tfDirectory = "tf";

			// If not a full path, assume it's relative to hl2.exe's directory.
			try
			{
				if (Path.GetFullPath(tfDirectory) != tfDirectory)
					tfDirectory = Path.Combine(Path.GetDirectoryName(ExePath), tfDirectory);
			}
			catch (ArgumentException)
			{
				throw new InvalidInputException($"Invalid -tfdir value: '{tfDirectory}' is not a valid directory path.");
			}

			if (!Directory.Exists(tfDirectory))
				throw new InvalidInputException($"Invalid -tfdir value: Couldn't find '{tfDirectory}'.");

			TfDirectory = tfDirectory;
		}

		/// <summary>
		/// Get value for <see cref="DemoPath"/> and make sure is's valid.
		/// </summary>
		private static void GetDemoPath()
		{
			string demoPath = GetArgValue("-demo");

			if (demoPath == null)
				throw new InvalidInputException("Invalid input: -demo must be set.");

			// If extension wasn't explicitly set, assume .dem.
			if (Path.GetExtension(demoPath) == String.Empty)
				demoPath += ".dem";

			// If not a full path, assume it's relative to tf directory.
			try
			{
				if (Path.GetFullPath(demoPath) != demoPath)
					demoPath = Path.Combine(TfDirectory, demoPath);
			}
			catch (ArgumentException)
			{
				throw new InvalidInputException($"Invalid -demo value: '{demoPath}' is not a valid file path.");
			}

			if (!File.Exists(demoPath))
				throw new InvalidInputException($"Invalid -demo value: Couldn't find '{demoPath}'.");

			// Parse demo header.
			DemoInfo demoInfo;
			try
			{
				demoInfo = new DemoInfo(demoPath);
			}
			catch (Exception e)
			{
				throw new InvalidInputException($"Error parsing demo: {e.Message}.");
			}

			if (!demoInfo.IsDemo)
				throw new InvalidInputException($"Invalid -demo value: '{demoPath}' isn't a demo.");
			if (!demoInfo.IsTF2)
				throw new InvalidInputException($"Invalid -demo value: '{demoPath}' isn't a TF2 demo.");
			if (demoInfo.DemoLength == -1)
				throw new InvalidInputException($"Invalid -demo value: Demo is corrupted.");

			DemoPath = demoPath;
			mDemoLength = demoInfo.DemoLength;
		}

		/// <summary>
		/// Get value for <see cref="StartTick"/> and make sure is's valid.
		/// </summary>
		private static void GetStartTick()
		{
			string input = GetArgValue("-start");

			// If not default tf directory.
			if (input == null)
				throw new InvalidInputException("Invalid input: -start must be set.");

			if (!int.TryParse(input, out int startTick))
				throw new InvalidInputException($"Invalid -start value: '{input}' is not an integer.");

			if (startTick < 1)
				throw new InvalidInputException("Invalid -start value: Start tick must be greater than one.");

			// Must be lower than demo length.
			if (startTick >= mDemoLength)
				throw new InvalidInputException($"Invalid -start value: '{DemoPath}' has only {mDemoLength - 1} ticks.");

			StartTick = startTick;
		}

		/// <summary>
		/// Get value for <see cref="EndTick"/> and make sure is's valid.
		/// </summary>
		private static void GetEndTick()
		{
			string input = GetArgValue("-end");

			// If not default tf directory.
			if (input == null)
				throw new InvalidInputException("Invalid input: -end must be set.");

			int endTick;

			// 'max' means stop as late as possible.
			if (input == "max")
			{
				endTick = mDemoLength - 1;
			}
			// If not a keyword, has to be a number.
			else if (!int.TryParse(input, out endTick))
			{
				throw new InvalidInputException($"Invalid -end value: '{input}' is not an integer.");
			}

			if (endTick <= StartTick)
				throw new InvalidInputException("Invalid -end value: End tick must be greater than start tick.");

			// End tick must be lower than demo length.
			if (endTick >= mDemoLength)
				throw new InvalidInputException($"Invalid -end value: '{DemoPath}' has only {mDemoLength - 1} ticks.");

			EndTick = endTick;
		}

		/// <summary>
		/// Get value for <see cref="OutputPath"/> and make sure is's valid.
		/// </summary>
		private static void GetOutputPath()
		{
			string outputPath = GetArgValue("-out");

			if (outputPath == null)
				throw new InvalidInputException("Invalid input: -out must be set.");

			var extension = Path.GetExtension(outputPath);

			// If extension wasn't explicitly set, assume .avi.
			if (extension == String.Empty)
			{
				outputPath += ".avi";
			}
			// Is extension is valid?
			else if (extension != ".avi" && extension != ".mp4" && extension != ".mov" && extension != ".mkv")
			{
				throw new InvalidInputException($"Invalid -out value: Unsupported extension '{extension}', use one of AVI, MP4, MOV, MKV.");
			}

			// If not a full path, assume it's relative to working directory.
			try
			{
				outputPath = Path.GetFullPath(outputPath);
			}
			// Path had invalid characters.
			catch (ArgumentException)
			{
				throw new InvalidInputException($"Invalid -out value: '{outputPath}' is not a valid file path.");
			}

			// Make sure output is not in the root of a drive (SDR requirement).
			if (Path.GetPathRoot(outputPath) == Path.GetDirectoryName(outputPath))
				throw new InvalidInputException($"Invalid -out value: SourceDemoRender cannot ouput to the root of a drive.");

			OutputPath = outputPath;
		}

		/// <summary>
		/// Get value for <see cref="ProfileName"/> and make sure is's valid.
		/// </summary>
		private static void GetProfile()
		{
			string profile = GetArgValue("-profile");

			// Optional argument.
			if (profile == null)
				return;

			var profilePath = Path.Combine(Directory.GetCurrentDirectory(), @"config\cfg\renderdemo\profiles", profile + ".cfg");

			if (!File.Exists(profilePath))
				throw new InvalidInputException($"Invalid -profile value: Couldn't find '{profilePath}'.");

			ProfileName = profile;
		}

		/// <summary>
		/// Get value for <see cref="UserCommands"/>.
		/// </summary>
		private static void GetUserCommands()
		{
			string userCommands = GetArgValue("-cmd");

			// Optional argument.
			if (userCommands == null)
				return;

			// These will be in an alias, quotes would cause issues.
			if (userCommands.IndexOf('"') != -1)
				throw new InvalidInputException($"Invalid -cmd value: Commands cannot contain quote.");

			UserCommands = userCommands;
		}

		/// <summary>
		/// Get value for <see cref="LaunchOptions"/>.
		/// </summary>
		private static void GetLaunchOptions()
		{
			string launchOptions = GetArgValue("-launch");

			// Optional argument.
			if (launchOptions == null)
				return;

			LaunchOptions = launchOptions;
		}

		/// <summary>
		/// Get value for <see cref="OverwriteAction"/>.
		/// </summary>
		private static void GetOverwriteAction()
		{
			string overwriteAction = GetArgValue("-overwrite");

			switch (overwriteAction)
			{
				// If not specified, default to ask.
				case null:
				case "ask":
					OverwriteAction = OverwriteAction.Ask;
					return;
				case "yes":
					OverwriteAction = OverwriteAction.AutoYes;
					return;
				case "no":
					OverwriteAction = OverwriteAction.AutoNo;
					return;
				default:
					throw new InvalidInputException($"Invalid -overwrite value: Only accepted values are 'yes', 'no', or 'ask'.");
			}
		}

		/// <summary>
		/// Get value for <see cref="WindowState"/>.
		/// </summary>
		private static void GetWindowState()
		{
			string overwriteAction = GetArgValue("-window");

			switch (overwriteAction)
			{
				// If not specified, default to ask.
				case null:
				case "fixed":
					WindowState = WindowState.Fixed;
					return;
				case "broken":
					WindowState = WindowState.Broken;
					return;
				case "hidden":
					WindowState = WindowState.Hidden;
					return;
				default:
					throw new InvalidInputException($"Invalid -window value: Only accepted values are 'fixed', 'broken', or 'hidden'.");
			}
		}

		/// <summary>
		/// Get value for <see cref="LogLevel"/>.
		/// </summary>
		private static void GetLogLevel()
		{
			string logLevel = GetArgValue("-loglevel");

			switch (logLevel)
			{
				// If not specified, default to 'info'.
				case null:
				case "info":
					LogLevel = LogLevel.Info;
					return;
				case "debug":
					LogLevel = LogLevel.Debug;
					return;
				case "progress":
					LogLevel = LogLevel.Progress;
					return;
				case "error":
					LogLevel = LogLevel.Error;
					return;
				case "quiet":
					LogLevel = LogLevel.Quiet;
					return;
				case "brief":
					LogLevel = LogLevel.Brief;
					return;
				default:
					throw new InvalidInputException($"Invalid -loglevel value: Only accepted values are 'debug', 'info', 'progress', 'error', or 'quiet'.");
			}
		}



		#endregion
	}
}
