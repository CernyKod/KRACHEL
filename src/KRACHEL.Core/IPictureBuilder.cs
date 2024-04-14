using System;
using System.Collections.Generic;
using System.Text;

namespace KRACHEL.Core
{
    public interface IPictureBuilder
    {
        public string CreatePicture();
        public string CreatePicture(string text);
    }
}
