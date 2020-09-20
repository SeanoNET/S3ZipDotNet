using System.Collections.Generic;
using System.IO;

namespace S3ZipSharp.Interface
{
    internal interface IObjectZipper
    {
        void CreateZip();
        bool ZipObject(string objectName, Stream data);
    }
}