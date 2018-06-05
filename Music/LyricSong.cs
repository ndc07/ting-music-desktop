﻿
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lyric
{
    public class Lyric
    {
        private static Lyric instance = new Lyric();
        private HtmlDocument document;
        private HtmlWeb html;
        public Lyric()
        {
            html = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
        }

        public static Lyric Instance
        {
            get
            {
                return instance;
            }
        }

        public string GetLyric(string songName, string artist = null)
        {
            if (InternetConnection.IsConnectedToInternet() == false)
                return string.Empty;
            if (string.IsNullOrWhiteSpace(artist))
                document = html.LoadFromBrowser("https://mp3.zing.vn/tim-kiem/bai-hat.html?q=" + songName);
            else
                document = html.LoadFromBrowser("https://mp3.zing.vn/tim-kiem/bai-hat.html?q=" + songName + ' ' + artist);
            string link = null;
            try
            {
                var data = document.DocumentNode.QuerySelector(".fl .title-song h3 a");
                link = @"https://mp3.zing.vn" + data.Attributes["href"].DeEntitizeValue;
                document = html.LoadFromBrowser(link);

                //if (string.IsNullOrWhiteSpace(artist))
                //{
                //    var data = document.DocumentNode.QuerySelector(".fl .title-song h3 a");
                //    link = @"https://mp3.zing.vn" + data.Attributes["href"].DeEntitizeValue;
                //    document = html.LoadFromBrowser(link);
                //}
                //else
                //{
                //    var data = document.DocumentNode.QuerySelectorAll(".fl .title-song h3 a");
                //    foreach (var item in data)
                //    {
                //        if (item.InnerText.Contains(artist))
                //        {
                //            link = @"https://mp3.zing.vn" + item.Attributes["href"].DeEntitizeValue;
                //            document = html.LoadFromBrowser(link);
                //            break;
                //        }
                //    }
                //    if (link == null)
                //    {
                //        document = null;
                //        return string.Empty;
                //    }

                //}
                var lyric = document.DocumentNode.QuerySelector(".fn-container [id] p");
                document = null;
                if (lyric == null)
                    return string.Empty;
                else
                {
                    return lyric.InnerHtml.Replace("<br>", "\n");
                }
            }
            catch
            {
                document = null;
                return string.Empty;
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}
