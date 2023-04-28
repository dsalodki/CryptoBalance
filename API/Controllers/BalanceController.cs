using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Util;
using Nethereum.Web3;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        [HttpPost]
        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IEnumerable<decimal>> GetBalances([FromBody] IEnumerable<string> addresses)
        {
            // https://mainnet.infura.io/v3/fe4610a7299e43829297838b0697061c
            // https://eth-mainnet.g.alchemy.com/v2/fmIFPFYmfPdiyi2up69qxiqtGFjcgCcC

            var web3 = new Web3("https://eth-mainnet.g.alchemy.com/v2/fmIFPFYmfPdiyi2up69qxiqtGFjcgCcC");

            var tasks = new List<Task<decimal>>();
            foreach (var address in addresses)
            {
                tasks.Add(GetBalance(web3, address));
            }

            return await Task.WhenAll(tasks);
        }

        private async Task<decimal> GetBalance(Web3 web3, string address)
        {
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
            return UnitConversion.Convert.FromWei(balance);
        }
    }
}
