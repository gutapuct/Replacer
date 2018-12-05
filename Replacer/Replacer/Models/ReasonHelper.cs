using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replacer.Models
{
    public static class ReasonHelper
    {
        public static IEnumerable<Equipment> ToEquipments(this string[,] data)
        {
            var result = new List<Equipment>();

            for (var line = 0; line < data.GetLength(1); line++)
            {
                if (String.IsNullOrWhiteSpace(data[0, line]))
                {
                    continue;
                }

                var names = data[0, line].Split('|');
                var equipment = new Equipment
                {
                    Names = names.Select(i => i.Trim()).Distinct().ToList(),
                    Reasons = new List<Reason>()
                };
                equipment.TypeName = equipment.Names[0];

                for (var column = 1; column < data.GetLength(0); column++)
                {
                    var reason = data[column, line];
                    if (!String.IsNullOrWhiteSpace(reason))
                    {
                        var reasonAndRecommendation = reason.Split('|');
                        if (reasonAndRecommendation.Where(i => i.Length > 0).Count() == 2)
                        {
                            reasons.Add(new Reason
                            {
                                NameReason = reasonAndRecommendation[0].Trim(),
                                NameRecommendation = reasonAndRecommendation[1].Trim(),
                                WasUsed = false
                            });
                        }
                    }
                }
                if (reasons.Count > 0)
                {
                    result.Add(new Equipment
                    {
                        //NamesOfEquipment = data[0, line].Split('|').Select(i => i = i.ToLower()).ToArray(),
                        //Reasons = reasons
                    });
                }
            }

            return result;
        }
    }
}
