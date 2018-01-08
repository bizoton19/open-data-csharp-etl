using CPSC.OpenData.Shaper.Contracts;
using OpenData.Shaper.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static CPSC.OpenData.Shaper.Model.NeissReport;

namespace CPSC.OpenData.Shaper.Repositories.Csv
{
    public class NeissEntity 
    {
        public NeissEntity(string partitionKey, string rowKey, string description)
        {
            this.PartitionKey = string.IsNullOrEmpty(partitionKey) ? "MISSING VALUE" : partitionKey; ;
            this.RowKey = string.IsNullOrEmpty(rowKey)?"MISSING VALUE": rowKey;
            Description = string.IsNullOrEmpty(description) ? "MISSING DESC" : description; ;

        }
        public NeissEntity() { }
        public string Description { get; private set; }
        public string PartitionKey { get; private set; }
        public string RowKey { get; private set; }

       
    }
    public class NeissCodeLookupRepository : INeissCodeLookupRepository
{
        private const string FILE = @"E:\sparkData\Neiss\NeissLookupTable.typed.csv";
    public NeissCodeLookupRepository()
    {
        this.init();
    }
    
    private void init()
    {
        

    }
        private IEnumerable<string> ReadLines(string filename)
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

        public ILookupBase Get(int? code, string entityType)
        {
            throw new NotImplementedException();
        }
        


       public IList<NeissEntity> ReadRecords( string file = FILE)
        {
            return (from line in ReadLines(file).Skip(1)
                   let splitRow = line.Split(',')
                   select new NeissEntity(splitRow[0], splitRow[1], splitRow[3]
                                                                        .TrimStart()
                                                                        .TrimEnd()
                                                                        .Replace('"',default(char))
                                                                        .ToUpperInvariant()))
                                                                        .ToList();
               
                
                    
        }
        

       public void Add(ILookupBase code)
        {
            throw new NotImplementedException();
        }

        public void Remove(ILookupBase code)
        {
            throw new NotImplementedException();
        }

      

    private ILookupBase CreateLookUp(string entityType)
    {
        if (entityType == "Product")
        {
            return new Product();
        }
        if (entityType == "Gender")
        {
            return new Gender();
        }
        if (entityType == "BodyPart")
        {
            return new BodyPart();
        }
        if (entityType == "EventLocale")
        {
            return new EventLocale();
        }
        if (entityType == "Fire")
        {
            return new Fire();
        }
        if (entityType == "Race")
        {
            return new Race();
        }
        if (entityType == "InjuryDiagnosis")
        {
            return new InjuryDiagnonis();
        }
        if (entityType == "InjuryDisposition")
        {
            return new InjuryDisposition();
        }


        return null;
    }

    

    public IEnumerable<ILookupBase> Get(List<ILookupBase> lookupcodes)
    {
        List<ILookupBase> lookups = new List<ILookupBase>();
        lookupcodes.ForEach(c => lookups.Add(this.Get(c.Code, c.GetType().Name)));
        return lookups;
    }


}
}


