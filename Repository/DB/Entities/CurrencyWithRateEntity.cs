using SolidBrokerTest.Repository.DB.Entities;

namespace Test2.Repository.DB.Entities
{
    public class CurrencyWithRateEntity : CurrencyEntity
    {
        public CurrencyWithRateEntity(string iD, string numCode, string? charCode, string? name, int nominal, string rate, DateOnly date) : base(iD, numCode, charCode, name)
        {
            Nominal = nominal;
            Rate = rate;
            Date = date;

        }

        

        public CurrencyEntity GetCurrency() { return this; }
        public RateEntity GetRate() { return new RateEntity(ID, Nominal, Rate, Date); }
        

        public int Nominal { get; set; }

        public string Rate { get; set; }

        public DateOnly Date { get; set; }






    }
}
