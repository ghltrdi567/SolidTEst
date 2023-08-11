using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using SolidBrokerTest.Repository.DB.Entities;
using Test2.Repository.DB;
using Test2.Repository.DB.Entities;

namespace Test2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        

        public int DISPLAY_MODE = 0;   //1 - по валюте(и датам), 2 по дате (и всем валютам на дату)

        string Display_Currency_ID = "R01235";

        public DateOnly Display_Currency_Date = new DateOnly(2015, 12, 24);

        public List<CurrencyWithRateEntity> CurrencyToDate = new List<CurrencyWithRateEntity>();

        public List<CurrencyEntity> CurrencyList = new List<CurrencyEntity>(); 


        public Repository.DB.Entities.DynamicCurrency? DynamicCurrency { get; set; } = null;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async void OnGet()
        {
            SQLHandlers.CreateCurrencyTable();
            SQLHandlers.CreateRateTable();

            CurrencyList = SQLHandlers.GetAllCurrency();

            if (Request.Query.ContainsKey("DISPLAY_MODE"))
            {
                
                StringValues someInt22;
                Request.Query.TryGetValue("DISPLAY_MODE", out someInt22);
                DISPLAY_MODE = Convert.ToInt32(someInt22);

            }

            if (Request.Query.ContainsKey("DispalayСurrencyID"))
            {

                StringValues someInt22;
                Request.Query.TryGetValue("DispalayСurrencyID", out someInt22);
                Display_Currency_ID = someInt22.ToString();

            }

            if (Request.Query.ContainsKey("DispalayСurrencyDate"))
            {

                StringValues someInt22;
                Request.Query.TryGetValue("DispalayСurrencyDate", out someInt22);

                if (ParseDate(someInt22) != null) Display_Currency_Date = ParseDate(someInt22) ?? new DateOnly();
                

            }

            


            //по валюте(и датам)
            if (DISPLAY_MODE == 1)
            {
                
                DynamicCurrency = DBEntityHandlers.GetAllCurrencyRate(Display_Currency_ID);

            }
            //дате (и всем валютам на дату)
            if (DISPLAY_MODE == 2)
            {

                CurrencyToDate = DBEntityHandlers.GetCurrencyRatesToDate(Display_Currency_Date);

               

            }




            //var yul = await SolidBrokerTest.Repository.XML.XMLData.GetDailyDataAsync(new DateOnly(2002, 12, 24));

            //var yul1 = await SolidBrokerTest.Repository.XML.XMLData.GetDynamicDataAsync(new DateOnly(2002, 12, 24), new DateOnly(2002, 12, 26), "R01010");


        }

        public void OnPostRecord()
        {
            var start_str = ParseDate(Request.Form["bd_start_date"].ToString());
            var end_str = ParseDate(Request.Form["bd_end_date"].ToString());


            //DBEntityHandlers.RecordData(start_str, end_str);
            DBEntityHandlers.RecordDataByDates(start_str?? new DateOnly(), end_str ?? new DateOnly());

            Response.Redirect(Request.Path);
        }

        
        /// <summary>
        /// Парсит строку с датой в формате 2023-08-12
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private DateOnly? ParseDate(string? str)
        {
            if (str == null) return null;
            if(str.Length !=10) return null;

            return new DateOnly(Convert.ToInt32(str.Substring(0, 4)), Convert.ToInt32(str.Substring(5, 2)), Convert.ToInt32(str.Substring(8, 2)));


        }
    }
}