using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Utilities
{
   public static class DelimitedFileParser
    {
        public static IEnumerable<IEnumerable<KeyValuePair<string,string>>> Parse(string datacategorytype, string filelocation, string filextension,char delimiter,int partition=5000)
        {
          

                var contentfix = (
                                    from file in Directory.GetFiles(filelocation, $"*.{filextension}")
                                    from line in ReadLines(file).Skip(1)
                                    select new KeyValuePair<string, string>(datacategorytype, line))
                                        .PartitionCollection(partition);
        return contentfix;


        }
        private static IEnumerable<string> ReadLines(string filename)
        {

            using (TextReader reader = File.OpenText(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
