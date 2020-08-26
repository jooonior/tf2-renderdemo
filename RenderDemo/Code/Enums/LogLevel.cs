namespace RenderDemo
{
	/// <summary>
	/// What output to show to the user.
	/// </summary>
	public enum LogLevel
	{
		Debug,

		/// <summary>
		/// Default.
		/// </summary>
		Info,

		Brief,

		/// <summary>
		/// Only show progress.
		/// </summary>
		Progress,

		Error,
		Quiet
	}
}
