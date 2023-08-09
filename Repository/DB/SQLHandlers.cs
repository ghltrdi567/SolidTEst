using Microsoft.Data.SqlClient;
using SolidBrokerTest.Repository.DB.Entities;
using System.Data;


namespace Test2.Repository.DB
{
    public class SQLHandlers
    {
        private static string ConnectionString { get; set; } = "";

        public static void SetConnectionString(string str) { ConnectionString = str; }

        public static void CreateCurrencyTable()
        {
            try
            {
                
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {

                    string query = @"IF OBJECT_ID(N'Currency', N'U') IS NULL
                                    CREATE TABLE Currency (
	                                ID NVARCHAR(10) NOT NULL,
	                                 NumCode NVARCHAR(5),
	                                 CharCode NVARCHAR(5),
	                                 Name NVARCHAR(50),
	                                 PRIMARY KEY (ID)
                                    );";
                    
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    var a = cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //display error message
                Console.WriteLine("Exception: " + ex.Message);
            }



        }

        public static void CreateRateTable()
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {

                    string query = @"IF OBJECT_ID(N'Rate', N'U') IS NULL
                                    CREATE TABLE Rate (
	                                ValuteID NVARCHAR(10) NOT NULL,
	                                Nominal INT,
	                                Value FLOAT,
	                                Date DATE,
	                                 FOREIGN KEY (ValuteID) REFERENCES Currency(ID)
                                );";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    var a = cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //display error message
                Console.WriteLine("Exception: " + ex.Message);
            }



        }

        //Валюты
        public static List<CurrencyEntity> GetAllCurrency()
        {
            var result = new List<CurrencyEntity>();
            try
            {
                
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {

                    
                    string query = @"SELECT e.ID,e.NumCode,e.CharCode,e.Name
                                     FROM Currency e
                                     ORDER BY ID ASC";
                    


                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                       
                        conn.Open();

                        
                        SqlDataReader dr = cmd.ExecuteReader();

                        
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                //var ID = dr.GetString(0);
                                //var NumCode = dr.GetString(1);
                                //var CharCode = dr.GetString(2);
                                //var Name = dr.GetString(3);

                                result.Add(new CurrencyEntity(dr.GetString(0), dr.GetString(1), dr.GetString(2), dr.GetString(3)));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Не найдено данных о валютах.");
                        }

                        
                        dr.Close();

                        
                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Exception: " + ex.Message);
            }



            return result;
        }

        public static CurrencyEntity? GetCurrency(string ID)
        {
            CurrencyEntity? result = null;

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"SELECT e.ID,e.NumCode,e.CharCode,e.Name
                                     FROM Currency e WHERE e.ID=@ID
                                     ";



                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 10).Value = ID;
                        conn.Open();


                        SqlDataReader dr = cmd.ExecuteReader();


                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                result = new CurrencyEntity(dr.GetString(0), dr.GetString(1), dr.GetString(2), dr.GetString(3));
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Не найдено данных о валюте с ID={ID}.");
                            
                        }


                        dr.Close();


                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }



            return result;
        }

        public static void AddCurrency(string ID, string NumCode, string CharCode, string Name)
        {

            try
            {
                
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {

                    
                    string query = @"INSERT INTO Currency (ID,NumCode,CharCode,Name)
                                    VALUES (@ID,@NumCode,@CharCode,@Name)";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 10).Value = ID;
                        cmd.Parameters.Add("@NumCode", SqlDbType.NVarChar, 5).Value = NumCode;
                        cmd.Parameters.Add("@CharCode", SqlDbType.NVarChar, 10).Value = CharCode;
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = Name;

                        
                        conn.Open();

                        var com = cmd.ExecuteNonQuery();

                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Exception: " + ex.Message);
            }




        }

        public static void AddCurrencyIfNotExists(string ID, string NumCode, string CharCode, string Name)
        {

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"BEGIN
                                       IF NOT EXISTS (SELECT * FROM Currency 
                                                       WHERE ID = @ID)
                                       BEGIN
                                           INSERT INTO Currency (ID,NumCode,CharCode,Name)
                                           VALUES (@ID,@NumCode,@CharCode,@Name)
                                       END
                                    END";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 10).Value = ID;
                        cmd.Parameters.Add("@NumCode", SqlDbType.NVarChar, 5).Value = NumCode;
                        cmd.Parameters.Add("@CharCode", SqlDbType.NVarChar, 10).Value = CharCode;
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = Name;


                        conn.Open();

                        var com = cmd.ExecuteNonQuery();

                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }




        }

        public static List<CurrencyEntity> GetCurrencyToDate(DateOnly Date)
        {


            var result = new List<CurrencyEntity>();

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"SELECT e.ID,e.NumCode,e.CharCode,e.Name
                                     FROM Currency e WHERE e.Date=@Date
                                     ";



                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@Date", SqlDbType.Date, 50).Value = new DateTime(Date.Year, Date.Month, Date.Day);
                        conn.Open();


                        SqlDataReader dr = cmd.ExecuteReader();


                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                result.Add(new CurrencyEntity(dr.GetString(0), dr.GetString(1), dr.GetString(2), dr.GetString(3)));
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Не найдено данных о валютах на дату с ID={Date.ToString()}.");

                        }


                        dr.Close();


                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }



            return result;





        }


        public static List<RateEntity> GetRatesToDate(DateOnly Date)
        {

            var result = new List<RateEntity>();

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"SELECT e.ValuteID,e.Nominal,e.Value,e.Date
                                     FROM Rate e WHERE e.Date=@Date ORDER BY ValuteID ASC, Date
                                     ";



                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@Date", SqlDbType.Date, 50).Value = new DateTime(Date.Year, Date.Month, Date.Day);
                        conn.Open();


                        SqlDataReader dr = cmd.ExecuteReader();


                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                result.Add(new RateEntity(dr.GetString(0), dr.GetInt32(1), (decimal)dr.GetDouble(2), new DateOnly(Date.Year, Date.Month, Date.Day)));

                                
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Не найдено данных о валютах на дату с ID={Date.ToString()}.");

                        }


                        dr.Close();


                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }



            return result;





        }

        public static void AddCurrencyIfNotExists(CurrencyEntity input)
        {
            AddCurrencyIfNotExists(input.ID, input.NumCode, input.CharCode?? "", input.Name ?? "");

            
        }


        public static void DeleteCurrency(string ID)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {

                    string query = @"DELETE FROM Currency WHERE ID=@ID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 10).Value = ID;
                        
                        conn.Open();

                        var com = cmd.ExecuteNonQuery();

                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }


        }


       //Курсы валют
        public static List<RateEntity> GetAllRate()
        {
            var result = new List<RateEntity>();
            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"SELECT e.ValuteID,e.Nominal,e.Value,e.Date
                                     FROM Rate e
                                     ORDER BY ValuteID ASC, Date";



                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        conn.Open();


                        SqlDataReader dr = cmd.ExecuteReader();


                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                
                                DateTime date = dr.GetDateTime(3);

                                result.Add(new RateEntity(dr.GetString(0), dr.GetInt32(1), dr.GetDecimal(2), new DateOnly(date.Year, date.Month, date.Day)));

                                
                            }
                        }
                        else
                        {
                            Console.WriteLine("Не найдено данных о курсах валют!");
                        }


                        dr.Close();


                        conn.Close();
                        
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }



            return result;
        }

        public static RateEntity? GetRate(string ValuteID, DateOnly Date)
        {
            RateEntity? result = null;

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"SELECT e.ValuteID,e.Nominal,e.Value,e.Date
                                     FROM Rate e WHERE e.ValuteID=@ValuteID AND e.Date=@Date
                                     ";



                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ValuteID", SqlDbType.NVarChar, 10).Value = ValuteID;
                        cmd.Parameters.Add("@Date", SqlDbType.Date, 50).Value = new DateTime(Date.Year, Date.Month, Date.Day);
                        conn.Open();


                        SqlDataReader dr = cmd.ExecuteReader();


                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                DateTime date = dr.GetDateTime(3);

                                result = new RateEntity(dr.GetString(0), dr.GetInt32(1), dr.GetDecimal(2), new DateOnly(date.Year, date.Month, date.Day));
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Не найдено данных о валюте с ID {ValuteID} и датой {Date}.");

                        }


                        dr.Close();


                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }



            return result;
        }


        public static List<RateEntity> GetRates(string ValuteID)
        {
            List<RateEntity> result = new List<RateEntity>();

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"SELECT e.ValuteID,e.Nominal,e.Value,e.Date
                                     FROM Rate e WHERE e.ValuteID=@ValuteID
                                     ";



                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ValuteID", SqlDbType.NVarChar, 10).Value = ValuteID;
                        
                        conn.Open();


                        SqlDataReader dr = cmd.ExecuteReader();


                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                DateTime date = dr.GetDateTime(3);

                                result.Add( new RateEntity(dr.GetString(0), dr.GetInt32(1), (decimal)dr.GetDouble(2), new DateOnly(date.Year, date.Month, date.Day)));
                                
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Не найдено данных о Курсе валюты с ID {ValuteID}.");

                        }


                        dr.Close();


                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }



            return result;
        }

        public static void AddRate(string ValuteID, int Nominal, decimal Value, DateOnly Date)
        {

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"INSERT INTO Rate (ValuteID,Nominal,Value,Date)
                                    VALUES ((SELECT ID from Currency WHERE ID=@ValuteID),@Nominal,@Value,@Date)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ValuteID", SqlDbType.NVarChar, 10).Value = ValuteID;
                        cmd.Parameters.Add("@Nominal", SqlDbType.Int, 5).Value = Nominal;
                        cmd.Parameters.Add("@Value", SqlDbType.Decimal, 10).Value = Value;
                        cmd.Parameters.Add("@Date", SqlDbType.Date, 50).Value = new DateTime(Date.Year,Date.Month, Date.Day);


                        conn.Open();

                        var com = cmd.ExecuteNonQuery();

                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }




        }


        public static void AddRateIfNotExists(string ValuteID, int Nominal, decimal Value, DateOnly Date)
        {

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {


                    string query = @"BEGIN
                                       IF NOT EXISTS (SELECT * FROM Rate
                                                       WHERE ValuteID = @ValuteID
                                                       AND Date = @Date)
                                       BEGIN
                                           INSERT INTO Rate (ValuteID, Nominal, Value, Date)
                                           VALUES (@ValuteID, @Nominal, @Value, @Date)
                                       END
                                    END";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ValuteID", SqlDbType.NVarChar, 10).Value = ValuteID;
                        cmd.Parameters.Add("@Nominal", SqlDbType.Int, 5).Value = Nominal;
                        cmd.Parameters.Add("@Value", SqlDbType.Float, 10).Value = Value;
                        cmd.Parameters.Add("@Date", SqlDbType.Date, 50).Value = new DateTime(Date.Year, Date.Month, Date.Day);


                        conn.Open();

                        var com = cmd.ExecuteNonQuery();

                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }




        }

        public static void AddRateIfNotExists(RateEntity input)
        {
            AddRateIfNotExists(input.ValuteID, input.Nominal, input.Rate, input.Date);
        }

        public static void DeleteRate(string ValuteID, DateOnly Date)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {

                    string query = @"DELETE FROM Rate WHERE ValuteID=@ValuteID AND Date = @Date";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@ValuteID", SqlDbType.NVarChar, 10).Value = ValuteID;
                        cmd.Parameters.Add("@Date", SqlDbType.Date, 50).Value = new DateTime(Date.Year, Date.Month, Date.Day);

                        conn.Open();

                        var com = cmd.ExecuteNonQuery();

                        conn.Close();

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
            }


        }



       

        


    }
}
