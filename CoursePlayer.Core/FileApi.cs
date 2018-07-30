using System;
using System.Collections.Generic;
using System.IO;
using CoursePlayer.Core.Models;

namespace CoursePlayer.Core
{
    public static class FileApi
    {
        private static readonly FileHelper _fileHelper = new FileHelper();

        public static byte[] GetIndexFile(string originalFile) {
            byte[] bytes = _fileHelper.ReadBytes(originalFile);
            // decompress
            byte[] decompressedBytes = CompressHelper.Decompress(bytes);
            return decompressedBytes;
        }

        public static List<Index> GetIndexList(byte[] indexbuf)
        {
            List<Index> listIndex = new List<Index>();
            //read data to index list
            MemoryStream stream = new MemoryStream(indexbuf);
            BinaryReader breader = new BinaryReader(stream);
            for (int i = 0; i < indexbuf.Length / Index.StreamSize; i++)
            {
                Index item = new Index((ushort)breader.ReadInt16(), breader.ReadByte(), breader.ReadInt32(), (uint)breader.ReadInt32());
                listIndex.Add(item);
            }

            for (int i = 0; i < listIndex.Count; i++)
            {
                //dataffset ==-1 is the point to same block as previous one
                if (listIndex[i].Offset == -1 && i > 0)
                {
                    listIndex[i].Offset = listIndex[i - 1].Offset;
                    listIndex[i].DataLength = listIndex[i - 1].DataLength;
                }
            }

            listIndex.Sort();
           
            return listIndex;
        }

        // screenshot
        public static List<Index> GetSSIndex(List<Index> ssIndexList, IDictionary<int, int> mapIndex, int second)
        {
            bool[] foundset = new bool[Constants.MAX_ROW_NO * Constants.MAX_COL_NO];

            List<Index> res = new List<Index>();

            int firstItem = 0;
            int firstSecond = second;
            for (; firstSecond >= 0; firstSecond--)
            {
                if (mapIndex.ContainsKey(firstSecond))
                {
                    firstItem = mapIndex[firstSecond];
                    break;
                }
            }

            while (firstItem < ssIndexList.Count && ssIndexList[firstItem].TimeStamp == firstSecond)
            {
                firstItem++;
            }

            if (firstItem > 0)
            {
                for (int i = firstItem - 1; i >= 0; i--)
                {
                    int value = ssIndexList[i].Row * Constants.MAX_ROW_NO + ssIndexList[i].Col;
                    if (!foundset[value])
                    {
                        foundset[value] = true;
                        res.Add(ssIndexList[i]);
                    }
                    if (res.Count == Constants.MAX_ROW_NO * Constants.MAX_COL_NO)
                    {
                        break;
                    }
                }
            }
            return res;
        }

        public static List<SSImage> GetSSData(string imagedatafile, List<Index> ssIndex)
        {
            List<SSImage> imageList = new List<SSImage>();
            foreach (Index index in ssIndex)
            {
                byte[] buf = _fileHelper.Seek(imagedatafile, index.Offset, (int)index.DataLength);
                imageList.Add(new SSImage(index.Row, index.Col, buf));
            }

            return imageList;
        }

        // whiteboard
        public static IDictionary<int, int> GetWBIndex(List<Index> indexs)
        {
            if (indexs == null || indexs.Count == 0)
                return null;

            IDictionary<int, int> minute_index_map = new Dictionary<int, int>();

            for (int i = 0; i < indexs.Count; i++)
            {
                if (!minute_index_map.ContainsKey(indexs[i].TimeStamp))
                {
                    minute_index_map.Add(new KeyValuePair<int, int>(indexs[i].TimeStamp, i));
                }
            }

            return minute_index_map;
        }

        public static List<WBLine> GetWBImageData(string wbImageDataFile, List<Index> wbIndexList, IDictionary<int, int> wbImageIndex, int streamSize, TimeSpan tspan)
        {
            List<WBLine> wblines = new List<WBLine>();

            Index indeximage = null;
            if (wbImageIndex.ContainsKey((int)tspan.TotalMinutes))
                indeximage = wbIndexList[wbImageIndex[(int)tspan.TotalMinutes]];
            try
            {
                if (indeximage != null && indeximage.DataLength > 0)
                {
                    byte[] buf = _fileHelper.Seek(wbImageDataFile, indeximage.Offset, (int)indeximage.DataLength);
                    if (buf.Length == indeximage.DataLength)
                    {
                        //read data to index list
                        MemoryStream stream = new MemoryStream(buf);
                        BinaryReader breader = new BinaryReader(stream);
                        for (int i = 0; i < buf.Length / streamSize; i++)
                        {
                            wblines.Add(new WBLine((ushort)breader.ReadInt16(), (ushort)breader.ReadInt16(), (ushort)breader.ReadInt16(),
                                (ushort)breader.ReadInt16(), (short)breader.ReadInt16(), (ushort)breader.ReadInt16()));
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return wblines;
        }

        public static List<WBEvent> GetWBSequenceData(string wbSequenceDataFile, List<Index> wbIndexList, IDictionary<int, int> wbSequenceIndex, int streamSize, TimeSpan tspan)
        {
            List<WBEvent> wbEvents = new List<WBEvent>();

            Index indeximage = null;
            if (wbSequenceIndex.ContainsKey((int)tspan.TotalMinutes))
                indeximage = wbIndexList[wbSequenceIndex[(int)tspan.TotalMinutes]];
            try
            {
                if (indeximage != null && indeximage.DataLength > 0)
                {
                    byte[] buf = _fileHelper.Seek(wbSequenceDataFile, indeximage.Offset, (int)indeximage.DataLength);
                    if (buf.Length == indeximage.DataLength)
                    {
                        //read data to index list
                        MemoryStream stream = new MemoryStream(buf);
                        BinaryReader breader = new BinaryReader(stream);
                        for (int i = 0; i < buf.Length / streamSize; i++)
                        {
                            wbEvents.Add(new WBEvent((uint)(breader.ReadUInt16()), (ushort)breader.ReadInt16(), (int)breader.ReadInt16(), (int)breader.ReadInt16()));
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return wbEvents;
        }

        public static void Close()
        {
            if (_fileHelper != null)
                _fileHelper.Close();
        }
    }
}
