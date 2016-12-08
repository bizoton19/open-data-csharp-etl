using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using neiss.lookup.model;
using bigdata.filereader.Model;

namespace bigdata.filereader.AzureStorageRepositories
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
    public class NeissCodeLookupRepository: INeissCodeLookupRepository
    {
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

        public ILookupBase Get(int code, string entityType)
        {
            this.init();
            TableOperation getOp = TableOperation.Retrieve<NeissEntity>(entityType,code.ToString(),new List<string>() { "Description"}) ;
            ILookupBase lBase = CreateLookUp(entityType);
            TableResult resultOp = CodeTable.Execute(getOp);
            var result = (NeissEntity)resultOp.Result;
            lBase.Code = System.Convert.ToInt32(result.RowKey);
            lBase.Description = result.Description;
            return lBase;
        }

        private ILookupBase CreateLookUp(string entityType)
        {
            if (entityType == "Product")
            {
                return new Product();
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
