using AlSatBot.services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace AlSatBot.Controllers
{
    public class HomeController : Controller
    {
        RsiServices _services;

        public HomeController(RsiServices services)
        {
            _services = services;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("getmyresult")]
        
        public IActionResult GetOutput(FormModel model)
        {
            string[] Coins = SplitAndAppendUSDT(model.Coins);
            string test = _services.RsiIndicator(Coins, model.rsiBuyThreshold, model.rsiSellThreshold, model.hourPeriod, model.ApiKey, model.SecretKey);
            return Json(test); 
        }

        public string[] SplitAndAppendUSDT(string input)
        {
            string[] parts = input.Split(',');
            string[] result = new string[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                result[i] = parts[i].Trim() + "_USDT";
            }
            return result;
        }

    }
}
