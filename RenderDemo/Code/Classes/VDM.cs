using System.IO;

namespace RenderDemo
{
	class VDM
	{
		private string _content = "demoactions\n{\n";
		private int _counter = 1;

		/// <summary>
		/// Append a new VDM PlayCommands entry
		/// </summary>
		/// <param name="tick"></param>
		/// <param name="commands"></param>
		/// <param name="name"></param>
		public void Add(int tick, string commands, string name = null)
		{
			// Default name if not specified
			if (name == null)
				name = $"Entry {_counter}";

			// Append a new VDM entry
			_content +=
				$"\t\"{_counter}\"\n" +
				 "\t{\n" +
				 "\t\tfactory \"PlayCommands\"\n" +
				$"\t\tname \"{name}\"\n" +
				$"\t\tstarttick \"{tick}\"\n" +
				$"\t\tcommands \"{commands}\"\n" +
				 "\t}\n";

			// Increment counter
			_counter++;
		}

		/// <summary>
		/// Write the VDM to disk.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>An exception if one happeneded.</returns>
		public void Save(string path)
		{
			// Add the closing bracket
			_content += "}";

			// Save it
			File.WriteAllText(path, _content);
		}

	}
}
