using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BSPLuaChecker
{
	public struct Header
	{
		public string ident;
		public int version;
		public Lump_t[] lumps;
		public Lump_t entlump;
		public int mapRevision;
	}

	public class Lump_t
	{
		public int fileoffset;
		public int filelength;
		public int version;
		public byte[] uncompressedLength;
		public byte[] chunk;
	}

	public class BSP
	{
		protected bool headerReady;
		protected Header header;
		protected string FileName;
		protected string FilePath;

		public BSP(string file_path)
		{
			if (!File.Exists(file_path))
				return;

			FilePath = file_path;
			FileName = Path.GetFileName(file_path);

			CreateHeader(file_path);
		}

		private void CreateHeader(string file_path)
		{
			using (BinaryReader br = new BinaryReader(File.Open(file_path, FileMode.Open, FileAccess.Read)))
			{
				string identity = Encoding.ASCII.GetString(br.ReadBytes(4));
				if (identity != "VBSP")
					throw new InvalidDataException("File is not a BSP or is malformed.");

				header = new Header()
				{
					ident = identity,
					version = br.ReadInt32(),
				};

				Lump_t lump = new Lump_t()
				{
					fileoffset = br.ReadInt32(),
					filelength = br.ReadInt32(),
					version = br.ReadInt32(),
					uncompressedLength = br.ReadBytes(4)
				};

				if (lump.fileoffset > 0 && lump.filelength > 0)
				{
					long curPosition = br.BaseStream.Position;
					br.BaseStream.Position = lump.fileoffset;
					lump.chunk = br.ReadBytes(lump.filelength - 1);
					br.BaseStream.Position = curPosition;
				}

				header.mapRevision = br.ReadInt32();
				header.entlump = lump;

				br.Close();
				headerReady = true;
			}
		}

		public string GetEntityString()
		{
			return Encoding.ASCII.GetString(header.entlump.chunk);
		}

		public List<KeyValueGroup> GetEntities()
		{
			return Parser.Parse(Encoding.ASCII.GetString(header.entlump.chunk));
		}
	}
}
