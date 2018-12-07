using MongoDB.Driver;
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
        public static IEnumerable<Equipment> ToEquipmentsAsync(this DataTable data)
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

        public static Reason GetReasonByEquipmentName(List<Equipment> equipments, string equipmentName, WorkResultInfo resultInfo, int numberLine)
        {
            Equipment equipment = null;
            equipmentName = equipmentName.ToLower().Trim();

            foreach (var el in equipments)
            {
                if (el.Names.Where(i => !String.IsNullOrWhiteSpace(i) && equipmentName.Trim().ToLower().IndexOf(i.Trim().ToLower()) >= 0).Any())
                {
                    equipment = el;
                    break;
                }
            }

            if (equipment == null)
            {
                resultInfo.Errors.Add($"Причина и рекомендация не найдена для строки №{numberLine + 1} ({equipmentName})");
                return new Reason();
            }

            var reason = equipment.Reasons.Where(i => !i.WasUsed).FirstOrDefault();
            if (reason == null)
            {
                equipment.Reasons.ForEach(i => i.WasUsed = false);
                reason = equipment.Reasons[0];
            }

            reason.WasUsed = true;
            return reason;
        }
    }
}
