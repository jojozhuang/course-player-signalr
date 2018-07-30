using CoursePlayer.Core;
using CoursePlayer.Core.Models;
using CoursePlayer.SignalR.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace CoursePlayer.SignalR
{
    public class PlayerHub : Hub
    {
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void UpdateTime(string group, string second)
        {
            int currenttime = Convert.ToInt32(second);
            List<SSImage> images = CourseApi.GetScreenshotData(currenttime);
            List<ScreenImage> list = new List<ScreenImage>();

            // convert image from byte[] to base64 string.
            foreach (SSImage item in images)
            {
                if (item.Image == null)
                {
                    continue;
                }
                list.Add(new ScreenImage { Row = item.Row, Col = item.Col, ImageStream = Convert.ToBase64String(item.Image) });
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Clients.Group(group).broadcastDrawScreenshot(jss.Serialize(list));

            WBData wbData = CourseApi.GetWhiteboardData(currenttime);
            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            Clients.Group(group).broadcastDrawWhiteboard(jss2.Serialize(wbData));
        }
    }
}