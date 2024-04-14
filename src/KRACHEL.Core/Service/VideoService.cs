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

        private readonly IPictureBuilder _pictureBuilder;

        public VideoService() { }

        public VideoService(IVideoBuilder videoBuilder, IPictureBuilder pictureBuilder)
        {
            _videoBuilder = videoBuilder;
            _pictureBuilder = pictureBuilder;
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
            var picturePartsArr = OrderAndComputeDurationOfVideoParts(pictureParts);

            PrepaterVideoPartData(picturePartsArr);

            return _videoBuilder.CreateVideoFromPictureParts(audioFilePath, picturePartsArr, outputFilePath);
        }

        private VideoPartDTO[] OrderAndComputeDurationOfVideoParts(IEnumerable<VideoPartDTO> pictureParts)
        {
            var picturePartsArr = pictureParts.OrderBy(vp => vp.AtTime).ToArray();

            for (int i = 0; i < picturePartsArr.Count(); i++)
            {
                if (i != picturePartsArr.Count() - 1)
                {
                    picturePartsArr[i].Duration = picturePartsArr[i + 1].AtTime.Subtract(picturePartsArr[i].AtTime).TotalSeconds;
                }
                else
                {
                    picturePartsArr[i].Duration = double.NaN;
                }
            }

            return picturePartsArr;
        }

        private void PrepaterVideoPartData(VideoPartDTO[] videoParts)
        {
            foreach(VideoPartDTO videoPart in videoParts)
            {
                switch (videoPart.VideoPartType) 
                {
                    case Enumerations.VideoPartType.BlackScreenWithText:
                        videoPart.FilePath = _pictureBuilder.CreatePicture(videoPart.Text);
                        break;
                    case Enumerations.VideoPartType.BlankScreen:
                        videoPart.FilePath = _pictureBuilder.CreatePicture();
                        break;
                    default:
                        break;
                
                }
            }
        }
    }
}
