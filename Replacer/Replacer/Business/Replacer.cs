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
using System.Net.Http;
using System.IO;

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

        public async Task<ResultMessage> GetAllEquipmentsAsync()
        {
            var resultMessage = new ResultMessage();
            try
            {
                resultMessage.Object = await collection.AsQueryable().ToEquipmentShotModel();
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        public async Task<ResultMessage> GetEquipmentById(string id)
        {
            var resultMessage = new ResultMessage();
            var objectId = new ObjectId(id);

            try
            {
                var equipment = await (await collection.FindAsync(i => i.Id == objectId)).SingleOrDefaultAsync();

                if (equipment == null)
                {
                    resultMessage.Errors.Add("Оборудование не найдено!");
                }
                else
                {
                    resultMessage.Object = equipment;
                }
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }

            return resultMessage;
        }

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
                            Names = new List<string>() { typeName },
                            Reasons = new List<Reason>(),
                            Order = (int)(await collection.EstimatedDocumentCountAsync())
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

        public async Task<ResultMessage> ReplaceNames(EquipmentShot equipment)
        {
            var resultMessage = new ResultMessage();

            try
            {
                await collection.UpdateOneAsync(
                    i => i.TypeName == equipment.TypeName,
                    Builders<Equipment>.Update.Set(x => x.Names, equipment.Names.Where(i => i.Trim().Length > 0).Distinct()));
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        public async Task<ResultMessage> ReplaceReasons(string id, IEnumerable<Reason> reasons)
        {
            var resultMessage = new ResultMessage();
            var objectId = new ObjectId(id);

            try
            {
                await collection.UpdateOneAsync(
                    i => i.Id == objectId,
                    Builders<Equipment>.Update.Set(x =>
                        x.Reasons,
                        reasons.Where(i => i.NameReason.Trim().Length > 0 || i.NameRecommendation.Trim().Length > 0)));
            }
            catch(Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        public async Task<ResultMessage> ReplaceTypeNameById(string id, string newTypeName)
        {
            var resultMessage = new ResultMessage();
            var objectId = new ObjectId(id);

            try
            {
                if (await collection.CountDocumentsAsync(i => i.TypeName == newTypeName && i.Id != objectId) > 0)
                {
                    resultMessage.Errors.Add("В списке уже есть оборудование с таким именем. Назовите иначе.");
                }
                else
                {
                    await collection.UpdateOneAsync(
                        i => i.Id == objectId,
                        Builders<Equipment>.Update.Set(x => x.TypeName, newTypeName));
                }
            }
            catch(Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        public async Task<ResultMessage> DeleteEquipment(string id)
        {
            var resultMessage = new ResultMessage();
            var objectId = new ObjectId(id);

            try
            {
                var orderByDeletingEquipment = (await (await collection.FindAsync(i => i.Id == objectId)).SingleAsync()).Order;
                await collection.UpdateManyAsync(
                    i => i.Order > orderByDeletingEquipment,
                    Builders<Equipment>.Update.Inc(k => k.Order, -1));

                await collection.DeleteOneAsync(i => i.Id == objectId);
            }
            catch(Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        public async Task<ResultMessage> ChangeOrder(string id, int value)
        {
            var resultMessage = new ResultMessage();
            var objectId = new ObjectId(id);

            try
            {
                var equipment = await (await collection.FindAsync(i => i.Id == objectId)).FirstOrDefaultAsync();
                if (equipment == null)
                {
                    resultMessage.Errors.Add("Оборудование не найдено. Обратитесь к администратору.");
                }
                else
                {
                    await collection.UpdateManyAsync(
                        i => i.Order == equipment.Order + value,
                        Builders<Equipment>.Update.Inc(x => x.Order, -value));
                    await collection.UpdateOneAsync(
                        i => i.Id == objectId,
                        Builders<Equipment>.Update.Inc(i => i.Order, value));
                }
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        public async Task<ResultMessage> ImportDb(HttpContent content)
        {
            var resultMessage = new ResultMessage();

            try
            {
                var file = await GetFileByRequestContent(content);
                var equipments = ExcelHelper.GetData(file).ToEquipments();
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        private async Task<FileModel> GetFileByRequestContent(HttpContent content)
        {
            var provider = new MultipartMemoryStreamProvider();
            await content.ReadAsMultipartAsync(provider);

            var fileNameParam = provider.Contents[0].Headers.ContentDisposition.Parameters.FirstOrDefault(p => p.Name.ToLower() == "filename");

            return new FileModel
            {
                Data = await provider.Contents[0].ReadAsByteArrayAsync(),
                FileName = (fileNameParam == null) ? "" : fileNameParam.Value.Trim('"')
            };
        }

    }
}
