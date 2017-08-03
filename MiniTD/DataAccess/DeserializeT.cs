/**
Copyright(c) 2016 Menno van der Woude

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
**/

using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace MiniTD.DataAccess
{
    public class DeserializeT<T>
    {
        #region GZip Serialization

        public T DeSerializeGZip(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                return default(T);

            var t = default(T);

            try
            {
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using (var gz = new GZipStream(fs, CompressionMode.Decompress))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    t = (T)serializer.Deserialize(gz);
                }
                fs.Close();
            }
            catch(Exception e)
            {
                throw new NotImplementedException();
            }

            return t;
        }

        #endregion // GZip Serialization
    }
}
