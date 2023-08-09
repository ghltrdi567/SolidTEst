using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace SolidBrokerTest.Repository.DB.Entities
{
    public class CurrencyEntity
    {
        /// <summary>
        /// ID, указанный в XML документе
        /// </summary>
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string? CharCode { get; set; }
        public string? Name { get; set; }

        public CurrencyEntity(string iD, string numCode, string? charCode, string? name)
        {
            ID = iD;
            NumCode = numCode;
            CharCode = charCode;
            Name = name;
        }
    }
}
