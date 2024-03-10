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

        private readonly IOptions<AppSettings.AppSettings> _appSettings;

        public CommandBuilder() { }

        public CommandBuilder(IOptions<AppSettings.AppSettings> appSettings) 
        { 
            _appSettings = appSettings;
        }

        public Command FileAnalyzeCommand(string filePath)
        {
            var program = _appSettings.Value.FFprobePath;
            var arguments = $"-i {GenerateFilePath(filePath)} -show_entries format=duration -v quiet -of csv=\"p=0\" -sexagesimal";

            return Build(program, arguments);

        }

        public Command ExtractAudioCommand(string videoFilePath, string audioFilePath)
        {
            var programm = _appSettings.Value.FFmpegPath;
            var arguments = $"-y -i {GenerateFilePath(videoFilePath)} -q:a 0 -map a {GenerateFilePath(audioFilePath)}";

            return Build(programm, arguments);
        }

        public Command MergeAudiWithOnePicture(string audioFilePath, string pictureFilePath, string outputFilePath)
        {
            var program = _appSettings.Value.FFmpegPath;
            var arguments = $"-y -loop 1 -i {GenerateFilePath(pictureFilePath)} -i {GenerateFilePath(audioFilePath)} -c:v libx264 -pix_fmt yuv420p -c:a copy -shortest {GenerateFilePath(outputFilePath)}";

            return Build(program, arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioFilePath"></param>
        /// <param name="videoParts"></param>
        /// <param name="outputFilePath"></param>
        /// <returns></returns>
        /// <remarks>
        ///     -y 
        ///     -loop 1 -t 10 -i C:\temp\ff\images\img600_1.png 
        ///     -loop 1 -t 10 -i C:\temp\ff\images\img600_2.png 
        ///     -loop 1 -t 10 -i C:\temp\ff\images\img600_3.png 
        ///     -i C:\temp\ff\RACHEL.mp3 
        ///     -filter_complex "[0][1]xfade=transition=hrslice:duration=1:offset=9[f];[f][2]xfade=transition=hrslice:duration=1:offset=18" 
        ///     -pix_fmt
        ///     yuv420p
        ///     C:\temp\ff\result.mp4
        /// </remarks>
        public Command MergeAudiWithMultiplePicture(string audioFilePath, VideoPartDTO[] videoParts, string outputFilePath, int resolutionWidth, int resolutionHeight)
        {
            //force YES
            var arguments = $"{_forceYes}";

            //input file & duration
            videoParts.ToList().ForEach(vp => arguments += $" {GenerateArgumentLoop(vp.FilePath, vp.Duration + 1)}");

            //audio input file
            arguments += $" -i {GenerateFilePath(audioFilePath)}";

            //filter complex (transition)
            var offsetResolver = new XFadeOffsetReslover();
            double transitionDuration;
            double transitionOffset;
            var streamOutputName = "f";
            var scaleVarPrefix = "s";
            arguments += " -filter_complex";
            arguments += " \"";
            for (int i = 0; i < videoParts.Count(); i++)
            {
                arguments += GenerateFilterComplexScale(i.ToString(), $"{scaleVarPrefix}{i}", resolutionWidth, resolutionHeight);
            }
            for (int i = 1; i < videoParts.Count(); i++)
            {
                offsetResolver.ComputeOffsetData(videoParts[i-1].Duration+1, out transitionDuration, out transitionOffset);
                if(i == 1)
                {
                    arguments += $"{GenerateFilterComplexFade($"{scaleVarPrefix}0", $"{scaleVarPrefix}1", transitionDuration, transitionOffset, i == videoParts.Count() - 1 ? null : streamOutputName)}";
                    continue;
                }

                //if(i == videoParts.Count() - 1)
                //{
                //    arguments += $"{GenerateFilterComplexFade(streamOutputName, $"{scaleVarPrefix}{i}", transitionDuration, transitionOffset)}";
                //    continue;
                //}
                
                arguments += $"{GenerateFilterComplexFade(streamOutputName, $"{scaleVarPrefix}{i}", transitionDuration, transitionOffset, i == videoParts.Count() - 1 ? null : streamOutputName)}";
            }
            arguments += "\"";

            //video format properties
            arguments += $" {_videoProperties}";

            //output file path
            arguments += $" {GenerateFilePath(outputFilePath)}";
            
            return Build(_appSettings.Value.FFmpegPath, arguments);
        }

        private string GenerateFilePath(string filePath)
        {
            return $"\"{filePath}\"";
        }

        private string GenerateArgumentLoop(string inputFilePath, double duration)
        {
            return $"-loop 1 -t {duration} -i {GenerateFilePath(inputFilePath)}";
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
    }
}
