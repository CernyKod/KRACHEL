using KRACHEL.Core.DTO;
using KRACHEL.Infrastructure.ValueObject;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRACHEL.Infrastructure.FFvideoBuilder
{
    public class FFVideoBuilder : Core.IVideoBuilder
    {
        private readonly ILogger<FFVideoBuilder> _logger;

        private readonly IOptions<AppSettings.AppSettings> _appSettings;

        private ProcessWorker _processWorker;

        private CommandBuilder _commandBuilder;

        private SupportedFormatsDb _supportedFormatsDb;

        private HumanErrorTranslator _humanErrorTranslator;

        public FFVideoBuilder() { }

        public FFVideoBuilder(ILogger<FFVideoBuilder> logger, IOptions<AppSettings.AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;

            _processWorker = new ProcessWorker(_logger, _appSettings.Value.FFProcessTimeout);
            _commandBuilder = new CommandBuilder(_appSettings);
            _supportedFormatsDb = new SupportedFormatsDb();
            _humanErrorTranslator = new HumanErrorTranslator();
        }

        public async Task<string> ExtractAudioAsync(string videoFilePath, string audioFilePath)
        {
            var command = _commandBuilder.ExtractAudioCommand(videoFilePath, audioFilePath);

            var processResult = await _processWorker.RunAsync(command);

            return ProcessResultResolver(processResult);
        }

        public string FileAnalyze(string filePath)
        {
            var commnad = _commandBuilder.FileAnalyzeCommand(filePath);

            var procesResult = _processWorker.Run(commnad);

            return $"Audio stopa v trvání {ProcessResultResolver(procesResult)}";
        }

        public IDictionary<string, string> GetSupportedAudioFormatsList()
        {
            return FileFormatsToDic(_supportedFormatsDb.GetAudioFormats());
        }

        public IDictionary<string, string> GetSupportedImageFormatsList()
        {
            return FileFormatsToDic(_supportedFormatsDb.GetImageFormats());
        }

        public IDictionary<string, string> GetSupportedVideoFormatsList()
        {
            return FileFormatsToDic(_supportedFormatsDb.GetVideoFormats());
        }

        private Dictionary<string, string> FileFormatsToDic(List<FileFormat> fileFormats) 
        {
            var supportedFormats = new Dictionary<string, string>();
            fileFormats.ForEach(f => supportedFormats.Add(f.ID, f.Description));

            return supportedFormats;
        }

        private string ProcessResultResolver(Tuple<int, string> processResult)
        {
            var exitCode = processResult.Item1;
            var output = processResult.Item2;

            if (exitCode != 0)
            {
                throw new Exception($"{_humanErrorTranslator.GetTranslation(output)}\n{output}");
            }

            return output;
        }

        public async Task<string> CreateVideoFromPictureParts(string audioFilePath, IEnumerable<VideoPartDTO> videoParts, string outputFilePath)
        {
            RecomputeDurationForXFade(videoParts);

            var command = _commandBuilder.MergeAudiWithPictureParts(audioFilePath, videoParts.ToArray(), outputFilePath, _appSettings.Value.VideoResolutionWidth, _appSettings.Value.VideoResolutionHeight);

            var processResult = await _processWorker.RunAsync(command);

            return ProcessResultResolver(processResult);
        }

        private void RecomputeDurationForXFade(IEnumerable<VideoPartDTO> pictureParts)
        {
            foreach (var part in pictureParts)
            {
                part.Duration += _appSettings.Value.VideoTransitionDuration;
            }
        }
    }
}
