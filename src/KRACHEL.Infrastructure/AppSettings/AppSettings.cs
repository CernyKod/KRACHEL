using System;
using System.Collections.Generic;
using System.Text;

namespace KRACHEL.Infrastructure.AppSettings
{
    public class AppSettings
    {
        public string FFmpegPath { get; set; }

        public string FFprobePath { get; set; }

        public int FFProcessTimeout { get; set; }   

        public int VideoResolutionWidth { get; set; }

        public int VideoResolutionHeight { get; set; }
    }
}
