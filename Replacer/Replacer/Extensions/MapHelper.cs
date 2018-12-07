using Replacer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Replacer.Extensions
{
    public static class MapHelper
    {
        public async static Task<IEnumerable<EquipmentShot>> ToEquipmentShotModel(this MongoDB.Driver.Linq.IMongoQueryable<Equipment> equipments)
        {
            var result = new List<EquipmentShot>();
            var list = await equipments.ToListAsync();
            
            foreach(var equipment in list.OrderBy(i => i.Order).ThenBy(i => i.TypeName))
            {
                result.Add(new EquipmentShot
                {
                    Id = equipment.Id,
                    Names = equipment.Names,
                    TypeName = equipment.TypeName,
                    Order = equipment.Order
                });
            }
            return result;
        }
    }
}
