using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace MiniTD.DataAccess
{
    public class SerializeT<T>
    {
        #region GZip Serialization

        public bool SerializeGZip(string file, T t)
        {
            var result = true;
            try
            {
                var fs = new FileStream(file,
                                           FileMode.Create, FileAccess.Write);
                using (var gz = new GZipStream(fs, CompressionMode.Compress))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(gz, t);
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        #endregion // GZip Serialization
    }
}
