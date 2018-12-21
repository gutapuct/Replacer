using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MongoDB.Driver;
using static Replacer.Enums;

namespace Replacer.Models
{
    public static class WordHelper
    {
        public static string FolderForTemplate = "FolderForTemplate";
        private static object LockObject = new object();

        public async static Task CreateAllActsAsync(HttpContent content, string pathToTempFolder, List<Equipment> equipments, WorkResultInfo resultInfo, string connectionid)
        {
            var hasError = false;
            var provider = new MultipartMemoryStreamProvider();
            await content.ReadAsMultipartAsync(provider);

            var values = await ExcelHelper.GetDataExcelAsync(content, provider, 1);

            using (var stream = await provider.Contents[0].ReadAsStreamAsync())
            {
                var reporter = new WebReporter();
                var rowsCount = values.Rows.Count;

                for (var value = 1; value < rowsCount; value++)
                {
                    reporter.SendProgress(rowsCount-1, value, TypeProgressBar.CreatingActs, connectionid);

                    System.Threading.Thread.Sleep(20);
                    var date = DateTime.Now.ToString("yyyyMMddhhmmssffff");
                    var copyPath = $"{pathToTempFolder}\\{date}_{value}.docx";

                    lock (LockObject)
                    {
                        if (!Directory.Exists(pathToTempFolder))
                        {
                            Directory.CreateDirectory(pathToTempFolder);
                        }

                        using (FileStream fs = new FileStream(copyPath, FileMode.Create))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(fs);
                        }
                    }


                    var pairsForReplacings = new Dictionary<string, string>();

                    // Add name of columns in the dictionary
                    for (var d = 0; d < values.Rows[0].ItemArray.Length; d++)
                    {
                        if (pairsForReplacings.ContainsKey(values.Rows[0].ItemArray[d].ToString().Trim()))
                        {
                            throw new Exception("Some values consist more than one columns. Also, you can't use the same values in the different columns.");
                        }
                        pairsForReplacings.Add(values.Rows[0].ItemArray[d].ToString().Trim(), values.Rows[value].ItemArray[d].ToString().Trim());
                    }

                    CheckColumnName(pairsForReplacings, "reason");
                    CheckColumnName(pairsForReplacings, "recommendation");

                    if (pairsForReplacings.ContainsKey("Name") || pairsForReplacings.ContainsKey("name"))
                    {
                        var equipmentName = pairsForReplacings.ContainsKey("Name") ? pairsForReplacings["Name"] : pairsForReplacings["name"];
                        var reason = ReasonHelper.GetReasonByEquipmentName(equipments, equipmentName, resultInfo, value);
                        pairsForReplacings.Add("reason", reason.NameReason);
                        pairsForReplacings.Add("recommendation", reason.NameRecommendation);
                    }
                    else if(!hasError)
                    {
                        resultInfo.Errors.Add(@"We couldn't find the column named ""Name"" or ""name"". The reasons will not be replaced!");
                    }

                    using (var doc = WordprocessingDocument.Open(copyPath, true))
                    {
                        var body = doc.MainDocumentPart.Document.Body;
                        var texts = body.Descendants<Text>();

                        foreach (var pair in pairsForReplacings.Where(i => !String.IsNullOrWhiteSpace(i.Key)))
                        {
                            var tokenTexts = texts.Where(t => t.Text.Contains(pair.Key));
                            foreach (var token in tokenTexts)
                            {
                                token.Text = token.Text.Replace(pair.Key, pair.Value);
                            }
                        }

                        if (texts.Any())
                        {
                            Paragraph PageBreakParagraph = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
                            doc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);
                        }

                        doc.MainDocumentPart.Document.Save();
                    }
                }
                reporter.SendProgress(rowsCount - 1, rowsCount - 1, TypeProgressBar.CreatingActs, connectionid);

                resultInfo.CountActs = rowsCount - 1;
            }
        }

        private static void CheckColumnName(Dictionary<string, string> pairsForReplacings, string column)
        {
            if (pairsForReplacings.ContainsKey(column))
            {
                throw new Exception($"Your Values can't contain \"{column}\" column!");
            }
        }
    }
}
