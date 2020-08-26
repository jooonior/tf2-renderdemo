using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderDemo
{
	/// <summary>
	/// Gets basic information about the demo.
	/// </summary>
	class DemoInfo
	{
		public bool IsDemo { get; private set; } = false;
		public bool IsTF2 { get; private set; } = false;
		public int DemoLength { get; private set; } = -1;
		public DemoInfo(string demoPath)
		{
			var reader = new BinaryReader(File.OpenRead(demoPath));

			IsDemo = new string(reader.ReadChars(8)) == "HL2DEMO\0";

			if (!IsDemo)
				return;

			// Skip bytes.
			reader.ReadBytes(4 + 4 + 260 + 260 + 260);

			// Read game directory.
			var game = reader.ReadChars(260);

			// Should be tf + nulls
			if (game[0] != 't' || game[1] != 'f')
				return;

			for (var i = 2; i < 260; i++)
			{
				if (game[i] != '\0')
					return;
			}

			IsTF2 = true;

			reader.ReadBytes(4);

			DemoLength = reader.ReadInt32();

			reader.Dispose();
		}
	}
}
