using KRACHEL.Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KRACHEL.Core
{
    public interface IVideoBuilder
    {
        Task<string> ExtractAudioAsync(string videoFilePath, string audioFilePath);
        string FileAnalyze(string filePath);
        IDictionary<string, string> GetSupportedVideoFormatsList();
        IDictionary<string, string> GetSupportedAudioFormatsList();
        IDictionary<string, string> GetSupportedImageFormatsList();
        Task<string> CreateVideoWithOnePicture(string audioFilePath, string pictureFilePath, string outputFilePath);
        Task<string> CreateVideoWithMultiplePicture(string audioFilePath, IEnumerable<VideoPartDTO> videoParts, string outputFilePath);
    }
}
