

using System.Net;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.CompilerServices;
using SolidBrokerTest.Repository.DB.Entities;
using System;
using Test2.Repository.DB.Entities;
using System.Xml.Schema;
using System.Globalization;

namespace SolidBrokerTest.Repository.XML
{
    public class XMLData
    {

        public static readonly string DailyDataBaseURL = "http://www.cbr.ru/scripts/XML_daily.asp";


        public static readonly string DinamicDataBaseURL = "https://www.cbr.ru/scripts/XML_dynamic.asp";

        public static readonly string MetalsBaseURL = "https://www.cbr.ru/scripts/xml_metall.asp";

        //https://www.cbr.ru/scripts/xml_metall.asp?date_req1=01/07/2001&date_req2=13/07/2001


        public static SolidBrokerTest.Repository.XML.Daily.ValCurs? GetDailyDataAsync(DateOnly date)
        {
            SolidBrokerTest.Repository.XML.Daily.ValCurs? result = null;

            result = LoadFromXmlWithDTD<Daily.ValCurs?>(DailyDataBaseURL + $"?date_req={date.Day.ToString("00")}/{date.Month.ToString("00")}/{date.Year.ToString("0000")}", validationCallBack: ValidationCallBack);

            return result;


        }

        public static SolidBrokerTest.Repository.XML.Metall.Metall GetMetallsAsync(DateOnly datefrom, DateOnly dateto)
        {
            SolidBrokerTest.Repository.XML.Metall.Metall? result = null;

            result = LoadFromXmlWithDTD<Metall.Metall>(MetalsBaseURL + $"?date_req1={datefrom.Day.ToString("00")}/{datefrom.Month.ToString("00")}/{datefrom.Year.ToString("0000")}&date_req2={dateto.Day.ToString("00")}/{dateto.Month.ToString("00")}/{dateto.Year.ToString("0000")}");

            return result;

        }

        public static async Task<SolidBrokerTest.Repository.XML.Dynamic.ValCurs?> GetDynamicDataAsync(DateOnly datefrom, DateOnly dateto, string ValuteID)
        {
            SolidBrokerTest.Repository.XML.Dynamic.ValCurs? result = null;
            using (var client = new HttpClient())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SolidBrokerTest.Repository.XML.Dynamic.ValCurs));
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var content = await client.GetAsync(DinamicDataBaseURL + $"?date_req1={datefrom.Day}/{datefrom.Month}/{datefrom.Year}&date_req2={dateto.Day}/{dateto.Month}/{dateto.Year}&VAL_NM_RQ={ValuteID}");

                if (content != null)
                {
                    using (var sr = new StreamReader(await content.Content.ReadAsStreamAsync(), Encoding.GetEncoding("windows-1251")))
                    {
                        result = serializer.Deserialize(sr.BaseStream) as SolidBrokerTest.Repository.XML.Dynamic.ValCurs;

                    }

                }

            }



            return result;
        }

        public static List<CurrencyWithRateEntity> GetCurrencyWithRateToDate(Daily.ValCurs? curs)
        {
            var Currensies = new List<CurrencyWithRateEntity>();

            if (curs.Valute == null) return Currensies;

            for (int i = 0; i < curs.Valute.Length; i++)
            {
               

                Currensies.Add(new CurrencyWithRateEntity(curs.Valute[i].ID, curs.Valute[i].NumCode, curs.Valute[i].CharCode, curs.Valute[i].Name, Convert.ToInt32(curs.Valute[i].Nominal), curs.Valute[i].Value, ParseDate(curs.Date) ?? new DateOnly()));


            }
            return Currensies;
        }



        /// <summary>
        /// Парсит строку даты вида 03.03.2001
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static DateOnly? ParseDate(string str)
        {
            if(str.Length != 10)
            {
                Console.WriteLine("Ошибка в преобразовании даты");

                return null;

            }

            return new DateOnly(Convert.ToInt32(str.Substring(6, 4)), Convert.ToInt32(str.Substring(3, 2)), Convert.ToInt32(str.Substring(0, 2)));

        }

        public static List<CurrencyWithRateEntity> GetCurrencyToDate(DateOnly begin, DateOnly End)
        {

            var result = new List<CurrencyWithRateEntity>();

            int daysInScope = End.DayNumber  - begin.DayNumber +1;

            if (daysInScope < 0)
            {
                Console.WriteLine("Ошибка, дата начала должна быть раньше даты конца");

                return result;

            }

            for (int i = 0; i < daysInScope; i++)
            {
                var first = GetDailyDataAsync(begin.AddDays(i));
                

                if (first == null) continue;
                result.AddRange(GetCurrencyWithRateToDate(first));

                
            }

            return result;
        }


        public static List<CurrencyWithRateEntity> GetCurrencyToDate(DateOnly Date)
        {
            var some = GetDailyDataAsync(Date);

            var second = GetMetallsAsync(Date, Date.AddDays(2));

            return GetCurrencyWithRateToDate(some);

        }


            public static T LoadFromXmlWithDTD<T>(string url, XmlSerializer serial = default, ValidationEventHandler validationCallBack = default)
            {
                var settings = new XmlReaderSettings
                {
                    
                    DtdProcessing = DtdProcessing.Parse,
                    IgnoreWhitespace = true,
                };
                settings.ValidationEventHandler += validationCallBack;
                serial = serial ?? new XmlSerializer(typeof(T));
                using (var reader = XmlReader.Create(url, settings))
                    return (T)serial.Deserialize(reader);
            }

        private static void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
                Console.WriteLine("Warning: Matching schema not found.  No validation occurred." + e.Message);
            else // Error
                Console.WriteLine("Validation error: " + e.Message);
        }


    }
}
