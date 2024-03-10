using KRACHEL.Core.DTO;
using KRACHEL.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRACHEL.Core.Service
{
    public class VideoService : IVideoService
    {
        private readonly IVideoBuilder _videoBuilder;

        public VideoService() { }

        public VideoService(IVideoBuilder videoBuilder)
        {
            _videoBuilder = videoBuilder;
        }        

        public Task<string> ExtractAudioAsync(string videoFilePath, string audioFilePath)
        {
            return _videoBuilder.ExtractAudioAsync(videoFilePath, audioFilePath);
        }

        public string FileAnalyze(string filePath)
        {
            return _videoBuilder.FileAnalyze(filePath);
        }

        public IDictionary<string, string> GetSupportedVideoFormatList()
        {
            return _videoBuilder.GetSupportedVideoFormatsList();
        }

        public IDictionary<string, string> GetSupportedAudioFormatList()
        {
            return _videoBuilder.GetSupportedAudioFormatsList();
        }

        public IDictionary<string, string> GetSupportedImageFormatList()
        {
            return _videoBuilder.GetSupportedImageFormatsList();
        }

        public Task<string> CreateVideoWithOnePicture(string audioFilePath, string pictureFilePath, string outputFilePath)
        {
            return _videoBuilder.CreateVideoWithOnePicture(audioFilePath, pictureFilePath, outputFilePath);
        }

        public Task<string> CreateVideoWithMiltiplePicture(string audioFilePath, IEnumerable<VideoPartDTO> videoParts, string outputFilePath)
        {
            var videoPartsArr = videoParts.OrderBy(vp => vp.InTime).ToArray();            

            for(int i = 0; i<videoPartsArr.Count(); i++)
            {
                if (i != videoPartsArr.Count() - 1)
                {
                    videoPartsArr[i].Duration = videoPartsArr[i + 1].InTime.Subtract(videoPartsArr[i].InTime).TotalSeconds;
                }
                else
                {
                    videoPartsArr[i].Duration = 1;
                }
            }            

            return _videoBuilder.CreateVideoWithMultiplePicture(audioFilePath, videoPartsArr, outputFilePath);
        }
    }
}
