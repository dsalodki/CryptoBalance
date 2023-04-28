using CryptoBalance.Models;
using Dapper;
using Npgsql;
using Quartz;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace CryptoBalance.Utils
{
    public class UpdateBalanceJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (Balance.Data.Count == 0)
            {
                using var con = new NpgsqlConnection(
        connectionString: "Server=127.0.0.1;Port=5432;User Id=postgres;Password=qaz123QAZ!@#;Database=postgres;");
                con.Open();
                IEnumerable<Item> wallets = await con.QueryAsync<Item>($"SELECT * FROM public.\"Wallets\"");

                foreach (Item item in wallets)
                {
                    Balance.Data.Add(new(item.Id, item.Address), null);
                }
            }

            var partSize = 10;
            var partsCount = (Balance.Data.Count + partSize - 1) / partSize;
            using var httpClient = new HttpClient();

            for (var partNumber = 0; partNumber < partsCount; partNumber++)
            {

                List<string> addresses = new List<string>();
                int absoluteIndex = partNumber * partSize;
                for (var i = 0; i < partSize; i++)
                {
                    if(absoluteIndex >= Balance.Data.Count)
                    {
                        break;
                    }
                    addresses.Add(Balance.Data.ElementAt(absoluteIndex).Key.Address);
                    absoluteIndex++;
                }

                var response = await httpClient.PostAsJsonAsync("https://localhost:7040/api/Balance", addresses);
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<IEnumerable<decimal>>(content);

                absoluteIndex = partNumber * partSize;
                for (var i = 0; i < partSize; i++)
                {
                    if (absoluteIndex >= Balance.Data.Count)
                    {
                        break;
                    }
                    Balance.Data[(absoluteIndex, addresses[i])] = json.ElementAt(i);
                    absoluteIndex++;
                }
                Balance.IsSynchrozed = false;
            }
        }
    }
}
