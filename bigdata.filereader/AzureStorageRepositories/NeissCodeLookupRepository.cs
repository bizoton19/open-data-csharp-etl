using System;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using OpenData.Shaper.Contracts;
using OpenData.Shaper.Model;
using static OpenData.Shaper.Model.NeissReport;
/// <summary>
/// PLugin module that implements the INeissCodeLookupRepository contract. This module performs CRUD operation on Azure storage tables
/// </summary>
namespace OpenData.Shaper.Repositories.AzureStorage
{
    public class NeissEntity : TableEntity
    {
        public NeissEntity(ILookupBase neissLookup)
        {
            this.PartitionKey = neissLookup.GetType().Name;
            this.RowKey = neissLookup.Code.ToString();
            Description = neissLookup.Description;

        }
        public NeissEntity() { }
        public string Description { get; set; }
    }
    public class      NeissCodeLookupRepository: INeissCodeLookupRepository
    {
        public NeissCodeLookupRepository()
        {
            this.init();
        }
        private CloudTable CodeTable;
        CloudTableClient tbleClient;
        private void init()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            
            //create table if it doesn't exist
             tbleClient = storageAccount.CreateCloudTableClient();

            //get a ref to the table 
             CodeTable = tbleClient.GetTableReference("NeissLookupTable");

            //create table if it doesn't exist
            CodeTable.CreateIfNotExists();

        }

        public ILookupBase Get(int? code, string entityType)
        {

           
            TableOperation getOp = TableOperation.Retrieve<NeissEntity>(entityType,code.ToString(),new List<string>() { "Description"}) ;
            ILookupBase lBase = CreateLookUp(entityType);
            TableResult resultOp = CodeTable.Execute(getOp);
            var result = (NeissEntity)resultOp.Result;
            lBase.Code = result==null ? default(int) : int.Parse(result.RowKey);
            lBase.Description = result==null? string.Empty:result.Description;
            return lBase;
        }
      
    private ILookupBase CreateLookUp(string entityType)
        {
            if (entityType == "Product")
            {
                return new NeissReport.Product();
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

        public void Add(ILookupBase code)
        {
            this.init();
            NeissEntity prod = new NeissEntity(code);
            
            TableOperation addOp = TableOperation.Insert(prod);
            CodeTable.Execute(addOp);

        }

        public void Remove(ILookupBase code)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ILookupBase> Get(List<ILookupBase> lookupcodes)
        {
            List<ILookupBase> lookups = new List<ILookupBase>();
            lookupcodes.ForEach(c => lookups.Add(this.Get(c.Code, c.GetType().Name)));
            return lookups; 
        }

        
    }
}
