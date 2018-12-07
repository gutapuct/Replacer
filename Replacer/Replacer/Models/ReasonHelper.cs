using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replacer.Models
{
    public static class ReasonHelper
    {
        public static IEnumerable<Equipment> ToEquipments(this DataTable data)
        {
            var result = new List<Equipment>();

            foreach (DataRow row in data.Rows)
            {
                if (row.ItemArray.Length == 0 || String.IsNullOrWhiteSpace(row.ItemArray[0].ToString()))
                    continue;

                var names = row.ItemArray[0]
                    .ToString()
                    .Split('|')
                    .Select(i => i.Trim())
                    .Distinct()
                    .ToList();

                var reasons = new List<Reason>();

                for (var column = 1; column < row.ItemArray.Length; column++)
                {
                    var reason = row.ItemArray[column].ToString();
                    if (!String.IsNullOrWhiteSpace(reason))
                    {
                        var reasonAndRecommendation = reason.Split('|');
                        if (reasonAndRecommendation.Where(i => i.Length > 0).Count() == 2)
                        {
                            reasons.Add(new Reason
                            {
                                NameReason = reasonAndRecommendation[0].Trim(),
                                NameRecommendation = reasonAndRecommendation[1].Trim(),
                            });
                        }
                    }
                }
                if (reasons.Count > 0)
                {
                    result.Add(new Equipment
                    {
                        Names = names,
                        TypeName = names[0],
                        Reasons = reasons,
                    });
                }
            }

            return result;
        }
    }
}
