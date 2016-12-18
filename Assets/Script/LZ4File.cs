using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace LZ4
{
    public class LZ4File
    {

        Dictionary<string, LZ4Entry> dict = new Dictionary<string, LZ4Entry>();

        public void AddEntry(string name, byte[] bytes)
        {
            dict[name] = new LZ4Entry(name, bytes);
        }

        public bool ContainsEntry(string name)
        {
            return dict.ContainsKey(name);
        }

        public void Save(System.IO.Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = new System.IO.BinaryWriter(memoryStream);
            writer.Write(dict.Count);
            foreach (KeyValuePair<string, LZ4Entry> pair in dict)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.bytes.Length);
                writer.Write(pair.Value.bytes);
            }
            using (LZ4Stream lz4Stream = new LZ4Stream(stream, CompressionMode.Compress, LZ4StreamFlags.HighCompression))
            {
                lz4Stream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            }
         
        }

        public static LZ4File Read(System.IO.Stream stream)
        {
            LZ4File file = new LZ4File();
            LZ4Stream lz4Stream = new LZ4Stream(stream, CompressionMode.Decompress);
            var reader = new System.IO.BinaryReader(lz4Stream);
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                var length = reader.ReadInt32();
                var bytes = reader.ReadBytes(length);

                file.dict[name] = new LZ4Entry(name, bytes);
            }
            return file;
        }
        public LZ4Entry this[string index]
        {
            get
            {
                LZ4Entry v;
                dict.TryGetValue(index, out v);
                return v;
            }
        }

        public ICollection<string> EntryFileNames
        {
            get { return dict.Keys; }
        }
        public void Dispose()
        {
        }
    }

    public class LZ4Entry
    {
        internal string name;
        internal byte[] bytes;

        public LZ4Entry(string name, byte[] bytes)
        {
            this.name = name;
            this.bytes = bytes;
        }

        public void Extract(System.IO.Stream stream)
        {
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}


