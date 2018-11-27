using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;
using Replacer.Models;

namespace Replacer.Business
{
    public class Replacer
    {
        private readonly MongoClient client;
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<Equipment> collection;
        private string connectionString { get; } = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;

        public Replacer()
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("Replacer");
            collection = database.GetCollection<Equipment>("Equipments");
        }

        public async Task<IEnumerable<Equipment>> GetAllReasonsAsync()
        {
            return collection.AsQueryable();
        }

        //public async Task<string> GetReasonAsync(int id)
        //{
        //    if (reasons.Count > id && id >= 0)
        //    {
        //        return reasons[id];
        //    }
        //    else
        //    {
        //        return "Error. We couldn't find that reason.";
        //    }
        //}

        public async Task<bool> AddReasonAsync(Equipment value)
        {
            if (!string.IsNullOrWhiteSpace(value.TypeName))
            {
                await collection.InsertOneAsync(value);
                return true;
            }
            return false;
        }

        //public async Task<bool> ChangeReasonAsync(int id, string value)
        //{
        //    if (reasons.Count > id && id >= 0)
        //    {
        //        reasons[id] = value;
        //        return true;
        //    }
        //    return false;
        //}

        //public async Task<bool> DeleteReasonAsync(int id)
        //{
        //    if (reasons.Count > id && id >= 0)
        //    {
        //        reasons.RemoveAt(id);
        //        return true;
        //    }
        //    return false;
        //}
    }
}
