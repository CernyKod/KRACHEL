using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KRACHEL.Infrastructure.TemporaryStore
{
    public class DirectoryStore : ITemporaryStore
    {
        private const string _storePath = @"tmpStore";

        public bool IsInitialized { get; private set; } = false;

        public DirectoryStore() 
        {
            IsInitialized = Initialize();            
        }
        public void Clear()
        {
            if (Directory.Exists(_storePath))
            {
                Directory.Delete(_storePath, true);
            }
        }

        public string SaveFileFromStream(MemoryStream stream, string fileFormatExtension)
        {
            string filePathInStore = null;
            using (var fileStream = new FileStream(GenerateNewFilePath(fileFormatExtension), FileMode.Create, FileAccess.Write))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
                filePathInStore = fileStream.Name;
            }

            return filePathInStore;
        }

        private string GenerateNewFilePath(string fileFormatExtension)
        {
            return @$"{_storePath}\{Guid.NewGuid()}.{fileFormatExtension.ToLower()}";
        }

        private bool Initialize()
        {
            Clear();

            Directory.CreateDirectory(_storePath);

            return true;
        }
    }
}
