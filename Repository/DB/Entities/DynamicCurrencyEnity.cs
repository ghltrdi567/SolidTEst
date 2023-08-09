using SolidBrokerTest.Repository.DB.Entities;

namespace Test2.Repository.DB.Entities
{
    public class DynamicCurrencyEnity
    {
        public string CurrencyID { get; set; }

        public List<RateEntity> Rates { get; set; }

        public DynamicCurrencyEnity(string currencyID, List<RateEntity> rates)
        {
            CurrencyID = currencyID;
            Rates = rates;
        }
    }

    public class DynamicCurrency : DynamicCurrencyEnity
    {
        public DynamicCurrency(string currencyID, List<RateEntity> rates, string numCode, string? charCode, string? name) : base(currencyID, rates)
        {
            NumCode = numCode;
            CharCode = charCode;
            Name = name;
        }


        public string NumCode { get; set; }
        public string? CharCode { get; set; }
        public string? Name { get; set; }




    }


}
