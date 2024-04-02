using KRACHEL.Core.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KRACHEL.Core.Service
{
    public interface IVideoService
    {
        Task<string> ExtractAudioAsync(string videoFilePath, string audioFilePath);
        string FileAnalyze(string filePath);

        IDictionary<string, string> GetSupportedVideoFormatList();

        IDictionary<string, string> GetSupportedAudioFormatList();

        IDictionary<string, string> GetSupportedImageFormatList();

        Task<string> CreateVideoFromPictureParts(string audioFilePath, IEnumerable<VideoPartDTO> pictureParts, string outputFilePath);
    }
}