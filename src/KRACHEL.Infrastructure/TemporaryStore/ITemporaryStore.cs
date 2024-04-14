using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KRACHEL.Infrastructure.TemporaryStore
{
    public interface ITemporaryStore
    {
        public void Clear();

        public string SaveFileFromStream(MemoryStream stream, string fileFormatExtension);
    }
}
