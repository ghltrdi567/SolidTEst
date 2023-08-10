using Microsoft.EntityFrameworkCore;

namespace SolidBrokerTest.Repository.DB.Entities
{
    
    public class RateEntity
    {
        public string ValuteID { get; set; }
        public int Nominal { get; set; }

        public float Rate { get; set; }

        public DateOnly Date { get; set; }

        public RateEntity(string valuteid, int nominal, float rate, DateOnly date)
        {
            ValuteID = valuteid;
            Nominal = nominal;
            Rate = rate;
            Date = date;
        }
    }
}
