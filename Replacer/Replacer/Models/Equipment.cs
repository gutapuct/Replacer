using MongoDB.Bson;
using System.Collections.Generic;

namespace Replacer.Models
{
    public class Equipment
    {
        public ObjectId Id { get; set; }
        public string TypeName { get; set; }
        public List<EquipmentsName> Names { get; set; }
        public List<Reason> Reasons { get; set; }
    }
}
