using Microsoft.Extensions.Options;
using KRACHEL.Core.Model;
using KRACHEL.Infrastructure.ValueObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using KRACHEL.Core.DTO;

namespace KRACHEL.Infrastructure.FFvideoBuilder
{
    /// <summary>
    /// Třída odpovědná za tvorbu příkazů pro volání komponent FF
    /// </summary>
    internal class CommandBuilder
    {
        private const string _forceYes = "-y";

        private const string _videoProperties = "-pix_fmt yuv420p";

        private const string _partsAligmnetOption = "-shortest";

        private readonly IOptions<AppSettings.AppSettings> _appSettings;

        public CommandBuilder() { }

        public CommandBuilder(IOptions<AppSettings.AppSettings> appSettings) 
        { 
            _appSettings = appSettings;
        }

        public Command FileAnalyzeCommand(string filePath)
        {
            var program = _appSettings.Value.FFprobePath;
            var arguments = $"-i {FilePathFormater(filePath)} -show_entries format=duration -v quiet -of csv=\"p=0\" -sexagesimal";

            return Build(program, arguments);

        }

        public Command ExtractAudioCommand(string videoFilePath, string audioFilePath)
        {
            var programm = _appSettings.Value.FFmpegPath;
            var arguments = $"{_forceYes} {GenerateVerbosity()} -i {FilePathFormater(videoFilePath)} -q:a 0 -map a {FilePathFormater(audioFilePath)}";

            return Build(programm, arguments);
        }

        public Command MergeAudiWithOnePicture(string audioFilePath, string pictureFilePath, string outputFilePath)
        {
            var program = _appSettings.Value.FFmpegPath;
            var arguments = $"{_forceYes} {GenerateVerbosity()} -loop 1 -i {FilePathFormater(pictureFilePath)} -i {FilePathFormater(audioFilePath)} -c:v libx264 -pix_fmt yuv420p -c:a copy -shortest {FilePathFormater(outputFilePath)}";

            return Build(program, arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioFilePath"></param>
        /// <param name="pictureParts"></param>
        /// <param name="outputFilePath"></param>
        /// <returns></returns>
        public Command MergeAudiWithPictureParts(string audioFilePath, VideoPartDTO[] pictureParts, string outputFilePath, int resolutionWidth, int resolutionHeight)
        {
            //general
            var arguments = $"{_forceYes} {GenerateVerbosity()}";

            //input file & duration
            arguments += GenerateArgumentLoop(pictureParts);

            //audio input file
            arguments += $" -i {FilePathFormater(audioFilePath)}";

            //filter complex (transition)
            if(pictureParts.Count() > 1)
            {
                arguments += GenerateFilterComplex(pictureParts);
            }            

            //video format properties
            arguments += $" {_videoProperties}";

            //parts aligment
            arguments += $" {_partsAligmnetOption}";

            //output file path
            arguments += $" {FilePathFormater(outputFilePath)}";
            
            return Build(_appSettings.Value.FFmpegPath, arguments);
        }

        private string FilePathFormater(string filePath)
        {
            return $"\"{filePath}\"";
        }

        private string NumberFormatter(double number)
        {
            return number.ToString().Replace(',', '.');
        }

        private string GenerateArgumentLoop(VideoPartDTO[] pictureParts)
        {
            var argument = string.Empty;

            foreach(var part in pictureParts)
            {
                argument += " -loop 1";
                if(!double.IsNaN(part.Duration))
                {
                    //čas je posunut o dobu trvání přechodu. Nutné pro korektní výpočet přechodu XFade modulu
                    argument += $" -t {NumberFormatter(part.Duration)}";
                }
                argument += $" -i {FilePathFormater(part.FilePath)}";
            }

            return argument;
        }

        /// <summary>
        /// FilterComplex - lze generovat pouze pro více položek než 1
        /// </summary>
        /// <param name="pictureParts"></param>
        /// <returns></returns>
        private string GenerateFilterComplex(VideoPartDTO[] pictureParts)
        {
            var argument = string.Empty;
            var offsetResolver = new XFadeOffsetReslover(_appSettings.Value.VideoTransitionDuration);
            double transitionDuration;
            double transitionOffset;
            var streamOutputName = "f";
            var scaleVarPrefix = "s";
            argument += " -filter_complex";
            argument += " \"";
            for (int i = 0; i < pictureParts.Count(); i++)
            {
                argument += GenerateFilterComplexScale(i.ToString(), $"{scaleVarPrefix}{i}", _appSettings.Value.VideoResolutionWidth, _appSettings.Value.VideoResolutionHeight);
            }
            for (int i = 1; i < pictureParts.Count(); i++)
            {
                offsetResolver.ComputeOffsetData(pictureParts[i - 1].Duration, out transitionDuration, out transitionOffset);
                if (i == 1)
                {
                    argument += $"{GenerateFilterComplexFade($"{scaleVarPrefix}0", $"{scaleVarPrefix}1", transitionDuration, transitionOffset, i == pictureParts.Count() - 1 ? null : streamOutputName)}";
                    continue;
                }

                argument += $"{GenerateFilterComplexFade(streamOutputName, $"{scaleVarPrefix}{i}", transitionDuration, transitionOffset, i == pictureParts.Count() - 1 ? null : streamOutputName)}";
            }
            argument += "\"";

            return argument;
        }

        private string GenerateFilterComplexFade(
            string firstInputIndex, string secondInputIndex, 
            double transitionDuration, double transitionOffset, 
            string output = null, string transitionName = "fade")
        {
            return $"[{firstInputIndex}][{secondInputIndex}]" +
                $"xfade=transition={transitionName}:duration={transitionDuration}:offset={transitionOffset}" +
                (output != null ? $"[{output}];" : string.Empty);
        }

        private string GenerateFilterComplexScale(string firstInputName, string secondInputName, int resolutionWidth, int resolutionHeight)
        {
            return $"[{firstInputName}]scale={resolutionWidth}:{resolutionHeight}:force_original_aspect_ratio=decrease:eval=frame,pad={resolutionWidth}:{resolutionHeight}:-1:-1:eval=frame[{secondInputName}];";
        }

        private Command Build(string program, string arguments)
        {
            return new Command() { Program = program, Arguments = arguments };
        }

        private string GenerateVerbosity()
        {
            return $"-loglevel {_appSettings.Value.FFVerbosity}";
        }
    }
}
