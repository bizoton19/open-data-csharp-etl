using CPSC.OpenData.Shaper.Model;
using OpenData.Shaper.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC.OpenData.Shaper.Services
{
   public class DelimitedFileService
    {
        private string _dataType;
        private string _fileExtension;
        private string _fileLocation;
        private char _delimiter;
        public DelimitedFileService(string datatype, string fileLocation ,string fileextension,char delimiter=',')
        {
            this._fileExtension = fileextension;
            this._dataType = datatype;
            this._fileLocation = fileLocation;
        }
        
        public void CreateModelFromDelimitedRecord()
        {
            
            var m = DelimitedFileParser.Parse(_dataType,_fileLocation, _fileExtension,_delimiter);
          

        }


    }
}
