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
using System.Data;
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
                resultMessage.Object = await collection.AsQueryable().ToEquipmentShotModelAsync();
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        public async Task<ResultMessage> GetEquipmentByIdAsync(string id)
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

        public async Task<ResultMessage> ReplaceNamesAsync(EquipmentShot equipment)
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

        public async Task<ResultMessage> ReplaceReasonsAsync(string id, IEnumerable<Reason> reasons)
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

        public async Task<ResultMessage> ReplaceTypeNameByIdAsync(string id, string newTypeName)
        {
            var resultMessage = new ResultMessage();
            var objectId = new ObjectId(id);

            try
            {
                if ((await collection.AsQueryable().ToListAsync())
                    .Where(i => i.TypeName.ToLower().Trim() == newTypeName.Trim().ToLower() && i.Id != objectId)
                    .Any())
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

        public async Task<ResultMessage> DeleteEquipmentAsync(string id)
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

        public async Task<ResultMessage> ChangeOrderAsync(string id, int value)
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

        public async Task<ResultMessage> ImportDbAsync(HttpContent content)
        {
            var equipmentsForSaving = new List<Equipment>();
            var equipmentsForSkipping = new List<Equipment>();
            var resultMessage = new ResultMessage();

            try
            {
                var equipmentsFromDb = (await collection.AsQueryable().ToListAsync()).Select(i => i.TypeName.ToLower()).ToList();
                var currentOrder = equipmentsFromDb.Count;

                var equipments = (await ExcelHelper.GetDataExcelAsync(content)).ToEquipmentsAsync();

                foreach (var equipment in equipments)
                {
                    equipment.Names = equipment.Names.Where(i => !String.IsNullOrWhiteSpace(i)).ToList();
                    if (equipmentsFromDb.Contains(equipment.TypeName.Trim().ToLower()))
                    {
                        equipmentsForSkipping.Add(equipment);
                    }
                    else
                    {
                        equipment.Order = currentOrder++;
                        equipmentsForSaving.Add(equipment);
                    }
                }

                var messages = new List<string>();
                if (equipmentsForSaving.Any())
                {
                    await collection.InsertManyAsync(equipmentsForSaving);
                    messages.Add($@"Ипортировано {equipmentsForSaving.Count} оборудования: {String.Join(", ", equipmentsForSaving.Select(i => i.TypeName))}.");
                }
                else
                {
                    messages.Add("Ничего не импортировано.");
                }

                if (equipmentsForSkipping.Any())
                {
                    equipmentsForSkipping.ForEach(eq =>
                        messages.Add($@"Оборудование ""{eq.TypeName}"" не импортировалось, так как ранее уже содержалось в базе.")
                    );
                }

                if (messages.Any())
                    resultMessage.Object = messages;
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            return resultMessage;
        }

        public async Task<ResultMessage> StartAsync(HttpContent content, string connectionid)
        {
            var resultMessage = new ResultMessage();
            var pathToTempFolder = $"{Environment.CurrentDirectory.Split(':')[0]}:\\temp\\{Guid.NewGuid().ToString()}";
            var resultInfo = new WorkResultInfo()
            {
                CountActs = 0,
                Errors = new List<string>(),
            };
            resultMessage.Object = resultInfo;

            try
            {
                var equipments = collection.AsQueryable().ToList();
                await WordHelper.CreateAllActsAsync(content, pathToTempFolder, equipments, resultInfo, connectionid);
                var files = FileHelper.GetStreamAllFiles(pathToTempFolder);
                FileHelper.SaveNewFile(files, connectionid);
            }
            catch (Exception ex)
            {
                resultMessage.AddErrorsFromException(ex);
            }
            finally
            {
                FileHelper.DeleteDirectoryAndFilesInThat(pathToTempFolder);
            }
            return resultMessage;
        }

    }
}
