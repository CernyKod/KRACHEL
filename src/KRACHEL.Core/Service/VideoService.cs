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

        public Task<string> CreateVideoFromPictureParts(string audioFilePath, IEnumerable<VideoPartDTO> pictureParts, string outputFilePath)
        {
            var picturePartsArr = pictureParts.OrderBy(vp => vp.InTime).ToArray();            

            for(int i = 0; i<picturePartsArr.Count(); i++)
            {
                if (i != picturePartsArr.Count() - 1)
                {
                    picturePartsArr[i].Duration = picturePartsArr[i + 1].InTime.Subtract(picturePartsArr[i].InTime).TotalSeconds;
                }
                else
                {
                    picturePartsArr[i].Duration = double.NaN;
                }
            }            

            return _videoBuilder.CreateVideoFromPictureParts(audioFilePath, picturePartsArr, outputFilePath);
        }
    }
}
