namespace Test2.Repository.DB.Entities
{
    public class Metall
    {
        public DateOnly Date { get; set; }

        public MetallCodes Codes { get; set; }

        public string? BuyPrice { get; set; }

        public string? SellPrice { get; set; }

        public Metall(DateOnly date, MetallCodes codes, string? buyPrice, string? sellPrice)
        {
            Date = date;
            Codes = codes;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
        }
    }

    

    public enum MetallCodes
    { 
        Gold = 1,
        Silver = 2,
        Platinum = 3,
        Palladium = 4

    }
}
