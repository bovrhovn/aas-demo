using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace aas.client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Reading service from url http://localhost:5000 ....");
            var query = @"
                    EVALUATE
                      TOPN(
                        1001,
                        SUMMARIZECOLUMNS('Product'[Name], ""SumListPrice"", CALCULATE(SUM('Product'[ListPrice]))),
                        [SumListPrice],
                        0,
                        'Product'[Name],
                        1
                      )

                    ORDER BY
                      [SumListPrice] DESC, 'Product'[Name]
                    ";
            using (var client = new HttpClient {BaseAddress = new Uri("http://localhost:5000")})
            {
                Console.WriteLine("Getting data from service...");
                var result = await client.GetStringAsync($"query/data/{query}");
                Console.WriteLine(result);
            }
            
            Console.WriteLine("Done!");
        }
    }
}