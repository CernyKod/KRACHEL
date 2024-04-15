using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace KRACHEL.Infrastructure.WinImageGenerator
{
    internal class PictureGenerator
    {
        private const int _normatedResolutionHeight = 1080;

        private const int _normatedFontSizeMax = 60;

        private const int _normatedFontSizeMin = 24;

        private readonly ImageFormat _imageFormat = ImageFormat.Png;

        private int _width;

        private int _height;

        private string _backgroundHex;

        private string _foregroundHex;

        private string _fontFamily;

        private int _maxFontSize
        {
            get
            {
                return (int)Math.Round(Convert.ToDouble(_normatedFontSizeMax * _height / _normatedResolutionHeight));
            }
        }

        private int _minFontSize
        {
            get
            {
                return (int)Math.Round(Convert.ToDouble(_normatedFontSizeMin * _height / _normatedResolutionHeight));
            }
        }

        private int _textAreaWidth
        {
            get
            {
                return (int)Math.Round(_width * 0.9);
            }
        }

        private int _textAreaHeight
        {
            get
            {
                return (int)Math.Round(_height * 0.9);
            }
        }

        private int _borderWidth
        {
            get
            {
                return (_width - _textAreaWidth) / 2;
            }
        }

        private int _borderHeight
        {
            get
            {
                return (_height - _textAreaHeight) / 2;
            }
        }

        public void Generate(
            out MemoryStream result, out string resultFormatExtension,
            int width, int height, string background, string foreground, string font, string text)
        {
            _width = width;
            _height = height;
            _backgroundHex = background;
            _foregroundHex = foreground;
            _fontFamily = font;

            result = new MemoryStream();
            resultFormatExtension = _imageFormat.ToString();

            using (Bitmap bitmap = new Bitmap(_width, _height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(ColorTranslator.FromHtml(_backgroundHex));

                    if (text != null)
                    {
                        GenerateTextToGraphic(graphics, text);
                    }
                }

                bitmap.Save(result, _imageFormat);
            }
        }

        private void GenerateTextToGraphic(Graphics graphics, string text)
        {
            using (Brush brush = new SolidBrush(ColorTranslator.FromHtml(_foregroundHex)))
            {
                RectangleF textArea = new RectangleF(_borderWidth, _borderHeight, _textAreaWidth, _textAreaHeight);

                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                var currentfontSize = GetOptimalFont(text, graphics, textArea, format, _fontFamily, _maxFontSize, _minFontSize);

                graphics.DrawString(text, new Font(_fontFamily, currentfontSize), brush, textArea, format);
            }
        }

        private int GetOptimalFont(string text, Graphics graphics, RectangleF rect, StringFormat format, string fontFamily, int maxFontSize, int minFontSize)
        {
            while (maxFontSize > minFontSize)
            {
                using (var font = new Font(fontFamily, maxFontSize))
                {
                    var calc = graphics.MeasureString(text, font, (int)rect.Width, format);
                    if (calc.Height <= rect.Height)
                    {
                        break;
                    }
                }
                maxFontSize -= 1;
            }
            return maxFontSize;

        }
    }
}
