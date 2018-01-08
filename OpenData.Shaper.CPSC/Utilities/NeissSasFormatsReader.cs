using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenData.Shaper.Contracts;
using CPSC.OpenData.Shaper.Model;
using static CPSC.OpenData.Shaper.Model.NeissReport;

namespace OpenData.Shaper.Utilities
{
    public class NeissSasFormatsReader
    {
        private string _file;
        public NeissSasFormatsReader(string file)
        {
            _file = file;
        }
        
        public IEnumerable<ILookupBase> ReadAll()
        {
            var lookupList = new List<ILookupBase>();
            IEnumerable<string> content = File.ReadAllLines(_file)
                                                                 .Where(line=> line.Trim().Length >1)
                                                                 .Where(line=>!line.Contains("*"))
                                                                 .Where(line=>!line.Contains("RUN;"));
      
            ILookupBase entitytpe = default(ILookupBase);
            string sastablename = default(string);
            foreach (string line in content)
            {
                
                if(line.Contains("VALUE")&& line.StartsWith(" "))
                {
                  sastablename = ParseSasTableName(line);
                  entitytpe=  ParseLookupEntityType(line);
                }
                if (line.Contains("=") && entitytpe != null)
                {
                    var lookuptoadd = ParseKeyValue(line, sastablename);
                    lookupList.Add(lookuptoadd);
                }
            }
            return lookupList;
        }

        private ILookupBase ParseLookupEntityType(string line)
        {
             return CreateLookupFrom(ParseSasTableName(line)); 
            
        }

        private string ParseSasTableName(string line)
        {
            return line.TrimStart().TrimEnd().Split(' ')[1];
        }

        private ILookupBase ParseKeyValue(string line,string sastablename)
        {
            ILookupBase lookup = CreateLookupFrom(sastablename);
            lookup.Code = Convert.ToInt32(line.Split('=')[0]);
            lookup.Description = line.Split('=')[1].Replace("'", "");
            return lookup;
        }

        private ILookupBase CreateLookupFrom(string sastablename)
        {
            ILookupBase lookup = null;
            switch (sastablename)
            {
                case "PRODUCT":
                    lookup = new NeissReport.Product();
                    break;
                case "BDYPT":
                    lookup = new BodyPart();
                    break;
                case "DIAG":
                    lookup = new InjuryDiagnonis();
                    break;
                case "DISP":
                    lookup = new InjuryDisposition();
                    break;
                case "FIRE":
                    lookup = new Fire();
                    break;
                case "LOCALE":
                    lookup = new EventLocale();
                    break;
                case "GENDER":
                    lookup = new Gender();
                    break;
                case "RACE":
                    lookup = new Race();
                    break;
                default:
                    break;
            }
            return lookup;
        }
    }
}
