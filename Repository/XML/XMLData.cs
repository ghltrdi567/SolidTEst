

using System.Net;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.CompilerServices;
using SolidBrokerTest.Repository.DB.Entities;
using System;
using Test2.Repository.DB.Entities;

namespace SolidBrokerTest.Repository.XML
{
    public class XMLData
    {
        
        public static readonly string DailyDataBaseURL = "http://www.cbr.ru/scripts/XML_daily.asp";

        
        public static readonly string DinamicDataBaseURL = "https://www.cbr.ru/scripts/XML_dynamic.asp";


        public static async Task<SolidBrokerTest.Repository.XML.Daily.ValCurs?> GetDailyDataAsync(DateOnly date)
        {
            SolidBrokerTest.Repository.XML.Daily.ValCurs? result = null;
            using (var client = new HttpClient())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SolidBrokerTest.Repository.XML.Daily.ValCurs));
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var content = await client.GetAsync(DailyDataBaseURL + $"?date_req={date.Day.ToString("00")}/{date.Month.ToString("00")}/{date.Year.ToString("0000")}");

                if(content.Content != null)
                {
                    using (var sr = new StreamReader(await content.Content.ReadAsStreamAsync(), Encoding.GetEncoding("windows-1251")))
                    {
                         result = serializer.Deserialize(sr.BaseStream) as SolidBrokerTest.Repository.XML.Daily.ValCurs;

                    }


                }

                


            }



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


        public static DynamicCurrencyEnity GetDynamicCurrency(Dynamic.ValCurs valCurs)
        {

            List<RateEntity> rates = new List<DB.Entities.RateEntity> ();

            for (int i = 0; i < valCurs.Record.Length; i++)
            {
                rates.Add(new RateEntity(valCurs.Record[i].Id, Convert.ToInt32(valCurs.Record[i].Nominal), Convert.ToDecimal(valCurs.Record[i].Value), ParseDate(valCurs.Record[i].Date) ?? new DateOnly()));


            }

            return new DynamicCurrencyEnity(valCurs.ID, rates);

        }


        public static List<CurrencyWithRateEntity> GetCurrencyToDate(Daily.ValCurs curs)
        {
            var Currensies = new List<CurrencyWithRateEntity>();

            if (curs.Valute == null) return Currensies;

            for (int i = 0; i < curs.Valute.Length; i++)
            {

                Currensies.Add(new CurrencyWithRateEntity(curs.Valute[i].ID, curs.Valute[i].NumCode, curs.Valute[i].CharCode, curs.Valute[i].Name, Convert.ToInt32(curs.Valute[i].Nominal), Convert.ToDecimal(curs.Valute[i].Value), ParseDate(curs.Date) ?? new DateOnly()));


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


        


        public static async Task<List<CurrencyWithRateEntity>> GetCurrencyToDate(DateOnly begin, DateOnly End)
        {

            
            var result = new List<CurrencyWithRateEntity>();

            

            int daysInScope = End.DayNumber  - begin.DayNumber;

            if (daysInScope < 0)
            {
                Console.WriteLine("Ошибка, дата начала должна быть раньше даты конца");

                return result;

            }

            for (int i = 0; i < daysInScope; i++)
            {
                var first = GetDailyDataAsync(begin.AddDays(i)).Result;
                if (first == null) continue;
                result.AddRange(GetCurrencyToDate( first));

                
            }

            return result;
        }
    }
}
