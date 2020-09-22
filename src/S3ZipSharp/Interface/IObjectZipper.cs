using System.Collections.Generic;
using System.IO;

namespace S3ZipSharp.Interface
{
    public interface IObjectZipper
    {
        void CreateZip();
        bool ZipObject(string objectName, Stream data);
    }
}