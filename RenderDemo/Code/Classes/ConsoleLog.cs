using System;
using System.IO;
using System.Text;
using System.Threading;

namespace RenderDemo
{
	/// <summary>
	/// Class for reading TF2's console.log.
	/// </summary>
	class ConsoleLog : IDisposable
	{
		#region Private Members

		private readonly FileStream _stream;
		private readonly StreamReader _reader;

		#endregion

		#region Public Properties

		/// <summary>
		/// True if there aren't any new unread lines.
		/// </summary>
		public bool EndOfStream => _reader.EndOfStream;

		#endregion

		#region Constructor

		/// <summary>
		/// Opens a console log for reading.
		/// </summary>
		/// <param name="logFilePath">The log file to read.</param>
		public ConsoleLog(string logFilePath)
		{
			// Open the file for reading
			_stream = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);

			// Skip what's already there
			/*
			 * This is useful when reading from default tf/console.log ,
			 * that can already contain lines from before we started.
			 * However since we initialize ConsoleLog after TF2 starts,
			 * we actually need to read those lines (we're not reading
			 * default console.log so it's fine).
			 */
			//_stream.Seek(0, SeekOrigin.End);

			// Start the reader
			_reader = new StreamReader(_stream, Encoding.UTF8);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Reads a line from the log file.
		/// </summary>
		/// <returns>The line.</returns>
		public string ReadLine()
		{
			// Return the next line
			return _reader.ReadLine();
		}

		/// <summary>
		/// Closes the log file and releases all resources.
		/// </summary>
		public void Dispose()
		{
			_reader.Dispose();
			_stream.Dispose();
		}

		#endregion
	}
}
