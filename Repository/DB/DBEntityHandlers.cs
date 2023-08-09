using SolidBrokerTest.Repository.DB.Entities;
using SolidBrokerTest.Repository.XML;
using Test2.Repository.DB.Entities;

namespace Test2.Repository.DB
{
    public class DBEntityHandlers
    {
        public async static void RecordData(DateOnly? start, DateOnly? end)
        {
            if (start == null || end == null) return;
            var dataFromXML = await XMLData.GetCurrencyToDate(start?? new DateOnly(), end ?? new DateOnly());

            foreach (var item in dataFromXML)
            {
                SQLHandlers.AddCurrencyIfNotExists(item.GetCurrency());

                SQLHandlers.AddRateIfNotExists(item.GetRate());

            }

        }

        public static Entities.DynamicCurrency? GetAllCurrencyRate(string Current_ID) {

            var rates = new List<RateEntity>();

            var Current_info = SQLHandlers.GetCurrency(Current_ID);

            if (Current_info == null)
            {
                Console.WriteLine($"Ошибка в получении курса валюты с ID {Current_ID}");
                return null;

            }

            foreach (var item in SQLHandlers.GetRates(Current_ID))
            {
                rates.Add(item);

            }

            return new Entities.DynamicCurrency(Current_info.ID, rates, Current_info.NumCode, Current_info.CharCode, Current_info.Name);
        
        }


        public static List<CurrencyWithRateEntity> GetCurrencyRatesToDate(DateOnly date)
        {
            var result = new List<CurrencyWithRateEntity>();

            var rateList = SQLHandlers.GetRatesToDate(date);

            

            foreach (var rate in rateList)
            {
                var currency = SQLHandlers.GetCurrency(rate.ValuteID);

                if (currency != null) { result.Add(new CurrencyWithRateEntity(currency.ID, currency.NumCode, currency.CharCode, currency.Name, rate.Nominal, rate.Rate, rate.Date)); }

            }

            return result;
        }



    }
}
