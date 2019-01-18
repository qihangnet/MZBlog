using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace IPIP.Net
{
    public class Reader
    {
        private readonly int fileSize;
        private readonly int nodeCount;

        private readonly MetaData meta;
        private readonly byte[] data;

        private readonly int v4offset;

        public Reader(string name)
        {
            var file = new FileInfo(name);
            fileSize = (int)file.Length;

            data = File.ReadAllBytes(name);

            var metaLength = BytesToLong(
                data[0],
                data[1],
                data[2],
                data[3]
            );

            var metaBytes = new byte[metaLength];
            Array.Copy(data, 4, metaBytes, 0, metaLength);

            var meta = JsonConvert.DeserializeObject<MetaData>(Encoding.UTF8.GetString(metaBytes));

            nodeCount = meta.NodeCount;
            this.meta = meta;

            if ((meta.TotalSize + (int)metaLength + 4) != data.Length)
            {
                throw new InvalidDatabaseException("database file size error");
            }

            data = data.Skip((int)metaLength + 4).ToArray();

            if (v4offset == 0)
            {
                var node = 0;
                for (var i = 0; i < 96 && node < nodeCount; i++)
                {
                    if (i >= 80)
                    {
                        node = ReadNode(node, 1);
                    }
                    else
                    {
                        node = ReadNode(node, 0);
                    }
                }

                v4offset = node;
            }
        }

        public string[] Find(string addr, string language)
        {
            int off;
            try
            {
                off = meta.Languages[language];
            }
            catch (NullReferenceException)
            {
                return null;
            }

            byte[] ipv;

            if (addr.IndexOf(":") > 0)
            {
                try
                {
                    ipv = IPAddress.Parse(addr).GetAddressBytes();
                }
                catch (Exception)
                {
                    throw new IPFormatException("ipv6 format error");
                }

                if ((meta.IPVersion & 0x02) != 0x02)
                {
                    throw new IPFormatException("no support ipv6");
                }
            }
            else if (addr.IndexOf(".") > 0)
            {
                try
                {
                    ipv = IPAddress.Parse(addr).GetAddressBytes();
                }
                catch (Exception)
                {
                    throw new IPFormatException("ipv4 format error");
                }

                if ((meta.IPVersion & 0x01) != 0x01)
                {
                    throw new IPFormatException("no support ipv4");
                }
            }
            else
            {
                throw new IPFormatException("ip format error");
            }

            int node;
            try
            {
                node = FindNode(ipv);
            }
            catch (NotFoundException)
            {
                return null;
            }

            var data = Resolve(node);
            var dst = new string[meta.Fields.Length];
            Array.Copy(data.Split('\t'), off, dst, 0, meta.Fields.Length);
            return dst;
        }

        private int FindNode(byte[] binary)
        {
            var node = 0;

            var bit = binary.Length * 8;

            if (bit == 32)
            {
                node = v4offset;
            }

            for (var i = 0; i < bit; i++)
            {
                if (node > nodeCount)
                {
                    break;
                }

                node = ReadNode(node, 1 & ((0xFF & binary[i / 8]) >> 7 - (i % 8)));
            }

            if (node > nodeCount)
            {
                return node;
            }

            throw new NotFoundException("ip not found");
        }

        private string Resolve(int node)
        {
            var resolved = node - nodeCount + nodeCount * 8;
            if (resolved >= fileSize)
            {
                throw new InvalidDatabaseException("database resolve error");
            }

            byte b = 0;
            var size = (int)(BytesToLong(
                b,
                b,
                data[resolved],
                data[resolved + 1]
            ));

            if (data.Length < (resolved + 2 + size))
            {
                throw new InvalidDatabaseException("database resolve error");
            }

            return Encoding.UTF8.GetString(data, resolved + 2, size);
        }

        private int ReadNode(int node, int index)
        {
            var off = node * 8 + index * 4;

            return (int)(BytesToLong(
                data[off],
                data[off + 1],
                data[off + 2],
                data[off + 3]
            ));
        }

        private static long BytesToLong(byte a, byte b, byte c, byte d)
        {
            return Int2long((((a & 0xff) << 24) | ((b & 0xff) << 16) | ((c & 0xff) << 8) | (d & 0xff)));
        }

        private static long Int2long(int i)
        {
            var l = i & 0x7fffffffL;
            if (i < 0)
            {
                l |= 0x080000000L;
            }

            return l;
        }

        public MetaData MetaData
        {
            get
            {
                return meta;
            }
        }

        public int BuildUTCTime
        {
            get
            {
                return meta.Build;
            }
        }

        public string[] SupportFields
        {
            get
            {
                return meta.Fields;
            }
        }
    }
}