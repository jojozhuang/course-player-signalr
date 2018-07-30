using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace CoursePlayer.Core
{
    public static class CompressHelper
    {
        public static byte[] Serialize(Object inst)
        {
            Type t = inst.GetType();
            var dcs = new DataContractSerializer(t);
            var ms = new MemoryStream();
            dcs.WriteObject(ms, inst);
            return ms.ToArray();
        }

        public static Object Deserialize(Type t, byte[] objectData)
        {
            var dcs = new DataContractSerializer(t);
            var ms = new MemoryStream(objectData);
            return dcs.ReadObject(ms);
        }

        public static byte[] SerializeAndCompress(Object inst)
        {
            byte[] b = Serialize(inst);
            byte[] b2 = Compress(b);
            return b2;
        }

        public static Object DecompressAndDeserialize(Type t, byte[] bytData)
        {
            byte[] b = Decompress(bytData);
            Object o = Deserialize(t, b);
            return o;
        }

        public static byte[] Compress(string strInput)
        {
            try
            {
                byte[] bytData = Encoding.UTF8.GetBytes(strInput);
                var ms = new MemoryStream();
                var defl = new Deflater(9, false);
                Stream s = new DeflaterOutputStream(ms, defl);
                s.Write(bytData, 0, bytData.Length);
                //s.Close();
                byte[] compressedData = ms.ToArray();
                return compressedData;
            }
            catch 
            {
                throw;
            }
        }

        public static byte[] Compress(byte[] bytData)
        {
            try
            {
                var ms = new MemoryStream();
                var defl = new Deflater(9, false);
                Stream s = new DeflaterOutputStream(ms, defl);
                s.Write(bytData, 0, bytData.Length);
                //s.Close();
                byte[] compressedData = ms.ToArray();
                return compressedData;
            }
            catch
            {
                throw;
            }
        }

        public static byte[] Compress(byte[] bytData, params int[] ratio)
        {
            int compRatio = 9;
            if (ratio[0] > 0)
            {
                compRatio = ratio[0];
            }

            try
            {
                var ms = new MemoryStream();
                var defl = new Deflater(compRatio, false);
                Stream s = new DeflaterOutputStream(ms, defl);
                s.Write(bytData, 0, bytData.Length);
                //s.Close();
                byte[] compressedData = ms.ToArray();
                return compressedData;
            }
            catch
            {
                throw;
            }
        }

        public static byte[] Decompress(byte[] bytInput)
        {
            var ms = new MemoryStream(bytInput, 0, bytInput.Length);
            byte[] bytResult = null;
            string strResult = String.Empty;
            var writeData = new byte[4096];
            Stream steam = new InflaterInputStream(ms);
            try
            {
                bytResult = ReadFullStream(steam);
                return bytResult;
            }
            catch 
            {
                throw;
            }
        }

        public static byte[] ReadFullStream(Stream stream)
        {
            var buffer = new byte[32768];
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}