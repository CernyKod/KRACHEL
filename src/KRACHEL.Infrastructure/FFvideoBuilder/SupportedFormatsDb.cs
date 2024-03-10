using KRACHEL.Infrastructure.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace KRACHEL.Infrastructure.FFvideoBuilder
{
    internal class SupportedFormatsDb
    {
        public List<FileFormat> GetVideoFormats()
        {
            return new List<FileFormat>()
            {
                new FileFormat() { ID = "mp4", Description = "MP4 (MPEG-4 Part 14)"},
                new FileFormat() { ID = "avi", Description = "AVI (Audio Vide Interleaved)"},                
                new FileFormat() { ID = "mpeg", Description = "MPEG-1 Systems / MPEG program stream"}
            };
        }

        public List<FileFormat> GetImageFormats()
        {
            return new List<FileFormat>()
            {
                //new FileFormat() { ID = "gif", Description = "Graphics Interchange Format"},
                new FileFormat() { ID = "jpg", Description = "JPEG File Interchange Format"},
                new FileFormat() { ID = "jpeg", Description = "JPEG File Interchange Format"},
                new FileFormat() { ID = "png", Description = "Portable Network Graphics"},
                new FileFormat() { ID = "webp", Description = "WebP"}
            };
        }

        public List<FileFormat> GetAudioFormats()
        {
            return new List<FileFormat>()
            {
                new FileFormat() { ID = "mp3", Description = "MP3 (MPEG audio layer 3)"},
                new FileFormat() { ID = "wav", Description = "WAV / WAVE (Waveform Audio)"}
            };
        }
    }
}
