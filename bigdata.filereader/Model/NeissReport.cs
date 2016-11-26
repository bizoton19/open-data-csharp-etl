using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.Model
{
   public class NeissReport
    {
        public int CpscCaseNo { get; set; }
        public DateTime TreatmentDate { get; set; }
        public int PSU { get; set; }
        public float Weight { get; set; }
        public Char Stratum { get; set; }
        public int Age { get; set; }
        public int Sex { get; set; }
        public int Race { get; set; }
        public int RaceOther { get; set; }
        public int DiagnosisCode { get; set; }
        public int DiagnosisOther { get; set; }
        public int BodyPart { get; set; }

        public int Disposition { get; set; }
        public int Location { get; set; }
        public int Fmv { get; set; }
        public int ProductCode1{ get; set; }

        public int ProductCode2 { get; set; }
        public string Narrative1 { get; set; }
        public string Narrative2 { get; set; }
       
    }
}
