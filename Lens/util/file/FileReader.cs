using System;
using System.IO;
using System.Text;

namespace Lens.util.file {
	public class FileReader {
		protected byte[] read;
		public byte[] Data => read;

		private int position;

		public int Position {
			get => position;

			set {
				position = Math.Max(0, Math.Min(read.Length - 1, value));
			}
		}
		
		public FileReader(string path) {
			if (path == null) {
				read = new byte[1];
				return;
			}

			ReadData(path);
		}

		protected virtual void ReadData(string path) {
			var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var stream = new BinaryReader(file);

			read = new byte[file.Length];

			for (var i = 0; i < file.Length; i++) {
				read[i] = (byte) file.ReadByte();
			}

			stream.Close();
		}

		public byte ReadByte() {
			if (read.Length == Position) {
				return 0;
			}
			
			return read[Position++];
		}
		
		public sbyte ReadSbyte() {
			return (sbyte) ReadByte();
		}

		public bool ReadBoolean() {
			return ReadByte() == 1;
		}

		public short ReadInt16() {
			return (short) ((ReadByte() << 8) | ReadByte());
		}

		public ushort ReadUint16() {
			return (ushort) ((ReadByte() << 8) | ReadByte());
		}

		public int ReadInt32() {
			return (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
		}

		public ushort ReadUInt16() {
			return (ushort) ((ReadByte() << 8) | ReadByte());
		}

		public uint ReadUInt32() {
			return (uint) ((ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte());
		}

		public string ReadString() {
			byte length = ReadByte();

			if (length == 0) {
				return null;
			}

			var result = new StringBuilder();

			for (int i = 0; i < length; i++) {
				result.Append((char) ReadByte());
			}

			return result.ToString();
		}

		public float ReadFloat() {
			return BitConverter.ToSingle(new[] { ReadByte(), ReadByte(), ReadByte(), ReadByte() }, 0);
		}

		public void SetData(byte[] data) {
			read = data;
			position = 0;
		}
	}
}
