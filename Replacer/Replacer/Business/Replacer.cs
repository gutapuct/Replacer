using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;
using Replacer.Models;
using Replacer.Extensions;

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

        public async Task<IEnumerable<EquipmentShot>> GetAllEquipmentsAsync()
        {
            return await collection.AsQueryable().ToEquipmentShotModel();
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

        public async Task<ResultMessage> AddEquipmentAsync(string typeName)
        {
            typeName = typeName.ToLower().Trim();
            var resultMessage = new ResultMessage();

            try
            {
                if (!string.IsNullOrWhiteSpace(typeName))
                {
                    if (await collection.CountDocumentsAsync(i => i.TypeName.ToLower() == typeName) != 0)
                    {
                        resultMessage.Errors.Add($"Оборудование \"{typeName}\" уже есть в базе");
                    }
                    else
                    {
                        await collection.InsertOneAsync(new Equipment
                        {
                            TypeName = typeName,
                            Names = new List<EquipmentsName> { new EquipmentsName { Name = typeName } }
                        });
                    }
                }
                else
                {
                    resultMessage.Errors.Add("Значение не должно быть пустым");
                }
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
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
