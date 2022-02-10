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
