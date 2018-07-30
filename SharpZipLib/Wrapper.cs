using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ICSharpCode.SharpZipLib
{
    /// <summary>
    /// Summary description for Wrapper.
    /// </summary>
    public class Wrapper
    {
        public byte[] Serialize(Object inst)
        {
            Type t = inst.GetType();
            var dcs = new DataContractSerializer(t);
            var ms = new MemoryStream();
            dcs.WriteObject(ms, inst);
            return ms.ToArray();
        }

        public Object Deserialize(Type t, byte[] objectData)
        {
            var dcs = new DataContractSerializer(t);
            var ms = new MemoryStream(objectData);
            return dcs.ReadObject(ms);
        }

        public byte[] SerializeAndCompress(Object inst)
        {
            byte[] b = Serialize(inst);
            byte[] b2 = Compress(b);
            return b2;
        }

        public Object DecompressAndDeserialize(Type t, byte[] bytData)
        {
            byte[] b = Decompress(bytData);
            Object o = Deserialize(t, b);
            return o;
        }


        public byte[] Compress(string strInput)
        {
            try
            {
                byte[] bytData = Encoding.UTF8.GetBytes(strInput);
                var ms = new MemoryStream();
                var defl = new Deflater(9, false);
                Stream s = new DeflaterOutputStream(ms, defl);
                s.Write(bytData, 0, bytData.Length);
                s.Close();
                byte[] compressedData = ms.ToArray();
                return compressedData;
            }
            catch 
            {
                throw;
            }
        }

        public byte[] Compress(byte[] bytData)
        {
            try
            {
                var ms = new MemoryStream();
                var defl = new Deflater(9, false);
                Stream s = new DeflaterOutputStream(ms, defl);
                s.Write(bytData, 0, bytData.Length);
                s.Close();
                byte[] compressedData = ms.ToArray();
                return compressedData;
            }
            catch
            {
               
                throw;
            }
        }


        public byte[] Compress(byte[] bytData, params int[] ratio)
        {
            int compRatio = 9;
            try
            {
                if (ratio[0] > 0)

                {
                    compRatio = ratio[0];
                }
            }
            catch
            {
                throw;
            }


            try
            {
                var ms = new MemoryStream();
                var defl = new Deflater(compRatio, false);
                Stream s = new DeflaterOutputStream(ms, defl);
                s.Write(bytData, 0, bytData.Length);
                s.Close();
                byte[] compressedData = ms.ToArray();
                 return compressedData;
            }
            catch
            {
                throw;
               
            }
        }


        public byte[] Decompress(byte[] bytInput)
        {
            var ms = new MemoryStream(bytInput, 0, bytInput.Length);
            byte[] bytResult = null;
            string strResult = String.Empty;
            var writeData = new byte[4096];
            Stream s2 = new InflaterInputStream(ms);
            try
            {
                bytResult = ReadFullStream(s2);
                s2.Close();
                return bytResult;
            }
            catch 
            {
                throw;
            }
        }

        public byte[] ReadFullStream(Stream stream)
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