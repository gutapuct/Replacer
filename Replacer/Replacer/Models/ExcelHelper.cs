using ExcelDataReader;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Replacer.Models
{
    public static class ExcelHelper
    {
        public static async Task<DataTable> GetData(HttpContent content)
        {
            var provider = new MultipartMemoryStreamProvider();
            await content.ReadAsMultipartAsync(provider);

            var fileNameParam = provider.Contents[0].Headers.ContentDisposition.Parameters.FirstOrDefault(p => p.Name.ToLower() == "filename");

            using (var stream = await provider.Contents[0].ReadAsStreamAsync())
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
