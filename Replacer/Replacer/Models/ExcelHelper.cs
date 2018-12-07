using ExcelDataReader;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Replacer.Models
{
    public static class ExcelHelper
    {
        public static async Task<DataTable> GetDataExcelAsync(HttpContent content, MultipartMemoryStreamProvider provider = null, int numberDoc = 0)
        {
            if (provider == null)
            {
                provider = new MultipartMemoryStreamProvider();
                await content.ReadAsMultipartAsync(provider);
            }

            var fileNameParam = provider.Contents[numberDoc].Headers.ContentDisposition.Parameters.FirstOrDefault(p => p.Name.ToLower() == "filename");

            using (var stream = await provider.Contents[numberDoc].ReadAsStreamAsync())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();
                    return result.Tables[0];
                }
            }
        }
    }
}
