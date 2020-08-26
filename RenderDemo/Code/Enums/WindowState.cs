namespace RenderDemo
{
	/// <summary>
	/// What to do with TF2's window after it launches.
	/// </summary>
	public enum WindowState
	{
		/// <summary>
		/// Fix the window.
		/// </summary>
		Fixed = 0,

		/// <summary>
		/// Leave it as it is (off the screen).
		/// </summary>
		Broken = 1,

		/// <summary>
		/// Hide the window completely.
		/// </summary>
		Hidden = 2
	}
}
