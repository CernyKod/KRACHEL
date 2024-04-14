using KRACHEL.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace KRACHEL.Core.DTO
{
    public class VideoPartDTO
    {
        public string FilePath { get; set; }

        public string Text { get; set; }    

        public TimeSpan AtTime { get; set; }

        public double Duration { get; set; }

        public VideoPartType VideoPartType { get; set; }
    }
}
