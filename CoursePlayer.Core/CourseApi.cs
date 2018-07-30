using System;
using System.Collections.Generic;
using CoursePlayer.Core.Models;

namespace CoursePlayer.Core
{
    public static class CourseApi
    {
        const string ssIndexFile = @"204304\ScreenShot\High\package.pak";
        const string ssDataFile = @"204304\ScreenShot\High\1.pak";
        const string wbImageIndexFile = @"204304\WB\1\VectorImage\package.pak";
        const string wbImageDataFile = @"204304\WB\1\VectorImage\1.pak";
        const string wbSequenceIndexFile = @"204304\WB\1\VectorSequence\package.pak";
        const string wbSequenceDataFile = @"204304\WB\1\VectorSequence\1.pak";

        // Screenshot
        private static List<Index> ssIndexList = new List<Index>();
        private static IDictionary<int, int> ssIndexMap = new Dictionary<int, int>();
        // Whiteboard
        private static List<Index> wbImageIndexList;
        private static IDictionary<int, int> wbImageIndex;
        private static List<Index> wbSequenceIndexList;
        private static IDictionary<int, int> wbSequenceIndex;

        public static List<SSImage> GetScreenshotData(int second) {
            if (ssIndexList == null || ssIndexList.Count == 0)
            {
                var buffer = FileApi.GetIndexFile(ssIndexFile);
                ssIndexList = FileApi.GetIndexList(buffer);

                ssIndexMap.Clear();
                for (int i = 0; i < ssIndexList.Count; i++)
                {
                    if (!ssIndexMap.ContainsKey(ssIndexList[i].TimeStamp))
                    {
                        ssIndexMap.Add(new KeyValuePair<int, int>(ssIndexList[i].TimeStamp, i));
                    }
                }
            }

            var ssIndex = FileApi.GetSSIndex(ssIndexList, ssIndexMap, second);
            return FileApi.GetSSData(ssDataFile, ssIndex);
        }

        public static WBData GetWhiteboardData(int second)
        {
            // get lines
            List<WBLine> lines = GetWBImageData(second);
            // get events
            List<WBEvent> events = GetWBSequenceData(second);
            // combine them to whiteboard data
            WBData wb = new WBData(second, lines, events);
            return wb;
        }

        private static List<WBLine> GetWBImageData(int second)
        {
            try
            {
                if (wbImageIndex == null)
                {
                    var buffer = FileApi.GetIndexFile(wbImageIndexFile);
                    wbImageIndexList = FileApi.GetIndexList(buffer);
                    wbImageIndex = FileApi.GetWBIndex(wbImageIndexList);
                }

                TimeSpan tspan = TimeSpan.FromSeconds(second);
                List<WBLine> lines = FileApi.GetWBImageData(wbImageDataFile, wbImageIndexList, wbImageIndex, WBLine.StreamSize, tspan);
                return lines;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static List<WBEvent> GetWBSequenceData(int second)
        {
            try
            {
                if (wbSequenceIndex == null)
                {
                    var buffer = FileApi.GetIndexFile(wbSequenceIndexFile);
                    wbSequenceIndexList = FileApi.GetIndexList(buffer);
                    wbSequenceIndex = FileApi.GetWBIndex(wbSequenceIndexList);
                }

                TimeSpan tspan = TimeSpan.FromSeconds(second);
                List<WBEvent> events = FileApi.GetWBSequenceData(wbSequenceDataFile, wbSequenceIndexList, wbSequenceIndex, WBEvent.StreamSize, tspan);
                return events;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Close()
        {
             FileApi.Close();
        }
    }
}
