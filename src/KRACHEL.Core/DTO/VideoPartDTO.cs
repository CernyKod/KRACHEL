using System;
using System.Collections.Generic;
using System.Text;

namespace KRACHEL.Core.DTO
{
    public class VideoPartDTO
    {
        public string FilePath { get; set; }

        public TimeSpan InTime { get; set; }

        public double Duration { get; set; }
    }
}
