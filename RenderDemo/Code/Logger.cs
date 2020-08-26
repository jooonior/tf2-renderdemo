using System;

namespace RenderDemo
{
	public static class Logger
	{
		public static void Message(string message, LogLevel logLevel)
		{
			if (Args.LogLevel > logLevel)
				return;

			Console.WriteLine(message);
		}

		public static void Message(string message, LogLevel logLevel, ConsoleColor color)
		{
			Console.ForegroundColor = color;

			Message(message, logLevel);

			Console.ResetColor();
		}
	}
}
