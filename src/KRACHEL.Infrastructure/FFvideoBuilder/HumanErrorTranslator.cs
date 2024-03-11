using System;
using System.Collections.Generic;
using System.Text;

namespace KRACHEL.Infrastructure.FFvideoBuilder
{
    /// <summary>
    /// Překladač FF STDERR do "lidské podoby". Vychází se z řídících sekvencí obsažených v textu výstupu
    /// </summary>
    public class HumanErrorTranslator
    {
        /// <summary>
        /// Vstupní slovník dle řídící sekvence
        /// </summary>
        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>()
        {
            {"Stream map 'a' matches no streams", HumanErrorDictionary.NoStream },
            {"Error opening input: No such file or directory", HumanErrorDictionary.FileOpenFailed }
        };

        /// <summary>
        /// Překlad STDERR
        /// </summary>
        /// <param name="ffError"></param>
        /// <returns></returns>
        public string GetTranslation(string ffError)
        {
            var translation = new StringBuilder();

            foreach (var record in _dictionary)
            {
                if(ffError.Contains(record.Key))
                {
                    translation.AppendLine(record.Value);
                }
            }

            return translation.ToString();
        }
    }
}
