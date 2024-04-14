using KRACHEL.Core;
using KRACHEL.Infrastructure.TemporaryStore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace KRACHEL.Infrastructure.WinImageGenerator
{
    public class WinImageGenerator : IPictureBuilder
    {
        private readonly IOptions<AppSettings.AppSettings> _appSettings;

        private readonly ITemporaryStore _temporaryStore;

        private readonly PictureGenerator _pictureGenerator = new PictureGenerator();

        public WinImageGenerator() { }

        public WinImageGenerator(IOptions<AppSettings.AppSettings> appSettings, ITemporaryStore temporaryStore) 
        {
            _appSettings = appSettings;
            _temporaryStore = temporaryStore;
        }
        public string CreatePicture()
        {
            return PictureGeneratorProcess();
        }

        public string CreatePicture(string text)
        {
            return PictureGeneratorProcess(text);
        }

        private string PictureGeneratorProcess(string text = null)
        {
            var resultStream = new MemoryStream();
            var resultExtensionFormat = string.Empty;
            
            _pictureGenerator.Generate(
                out resultStream,
                out resultExtensionFormat,
                _appSettings.Value.VideoResolutionWidth,
                _appSettings.Value.VideoResolutionHeight,
                _appSettings.Value.PictureBackgroundHexh,
                _appSettings.Value.PictureForegroundHexh,
                _appSettings.Value.PictureFontName,
                text);

            return _temporaryStore.SaveFileFromStream(resultStream, resultExtensionFormat);
        }
    }
}
