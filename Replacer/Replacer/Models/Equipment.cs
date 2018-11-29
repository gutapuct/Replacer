using MongoDB.Bson;
using System.Collections.Generic;

namespace Replacer.Models
{
    public abstract class EquipmentAbstract
    {
        public ObjectId Id { get; set; }
        public string TypeName { get; set; }
        public List<string> Names { get; set; }
        public int Order { get; set; }
    }

    public class EquipmentShot : EquipmentAbstract
    {
        public bool IsShowNames { get; } = false;
    }

    public class Equipment : EquipmentAbstract
    {
        public List<Reason> Reasons { get; set; }
    }

    
}
