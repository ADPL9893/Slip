using ExcelDataReader;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Slip.Utility
{
    public class CommonMethods
    {
        /// <summary>
        /// Gets a Inverted DataTable
        /// </summary>
        /// <param name="table">DataTable do invert</param>
        /// <param name="columnX">X Axis Column</param>
        /// <param name="nullValue">null Value to Complete the Pivot Table</param>
        /// <param name="columnsToIgnore">Columns that should be ignored in the pivot 
        /// process (X Axis column is ignored by default)</param>
        /// <returns>C# Pivot Table Method  - Felipe Sabino</returns>

        public static DataTable GetInversedDataTable(DataTable table, string columnX,
                                                     params string[] columnsToIgnore)
        {
            //Create a DataTable to Return
            DataTable returnTable = new DataTable();

            if (columnX == "")
                columnX = table.Columns[0].ColumnName;

            //Add a Column at the beginning of the table

            returnTable.Columns.Add(columnX);

            //Read all DISTINCT values from columnX Column in the provided DataTale
            List<string> columnXValues = new List<string>();

            //Creates list of columns to ignore
            List<string> listColumnsToIgnore = new List<string>();
            if (columnsToIgnore.Length > 0)
                listColumnsToIgnore.AddRange(columnsToIgnore);

            if (!listColumnsToIgnore.Contains(columnX))
                listColumnsToIgnore.Add(columnX);

            foreach (DataRow dr in table.Rows)
            {
                string columnXTemp = dr[columnX].ToString();
                //Verify if the value was already listed
                if (!columnXValues.Contains(columnXTemp))
                {
                    //if the value id different from others provided, add to the list of 
                    //values and creates a new Column with its value.
                    columnXValues.Add(columnXTemp);
                    returnTable.Columns.Add(columnXTemp);
                }
                else
                {
                    //Throw exception for a repeated value
                    throw new Exception("The inversion used must have " +
                                        "unique values for column " + columnX);
                }
            }

            //Add a line for each column of the DataTable

            foreach (DataColumn dc in table.Columns)
            {
                if (
                    !listColumnsToIgnore.Contains(dc.ColumnName))
                {
                    DataRow dr = returnTable.NewRow();
                    dr[0] = dc.ColumnName;
                    returnTable.Rows.Add(dr);
                }
            }

            //Complete the datatable with the values
            for (int i = 0; i < returnTable.Rows.Count; i++)
            {
                for (int j = 1; j < returnTable.Columns.Count; j++)
                {
                    returnTable.Rows[i][j] =
                      table.Rows[j - 1][returnTable.Rows[i][0].ToString()].ToString();
                }
            }

            return returnTable;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public static string GetCurrentYearWiseAlphabet(string CurYear)
        {

            string NewString = "";
            if (CurYear == "2020")
            {
                NewString = "A";
            }
            else if (CurYear == "2021")
            {
                NewString = "B";
            }
            else if (CurYear == "2022")
            {
                NewString = "C";
            }
            else if (CurYear == "2023")
            {
                NewString = "D";
            }
            else if (CurYear == "2024")
            {
                NewString = "E";
            }
            else if (CurYear == "2025")
            {
                NewString = "F";
            }
            else if (CurYear == "2026")
            {
                NewString = "G";
            }
            else if (CurYear == "2027")
            {
                NewString = "H";
            }
            else if (CurYear == "2028")
            {
                NewString = "I";
            }
            else if (CurYear == "2029")
            {
                NewString = "J";
            }
            else if (CurYear == "2030")
            {
                NewString = "K";
            }
            else if (CurYear == "2031")
            {
                NewString = "L";
            }
            else if (CurYear == "2032")
            {
                NewString = "M";
            }
            else if (CurYear == "2033")
            {
                NewString = "N";
            }
            else if (CurYear == "2034")
            {
                NewString = "O";
            }
            else if (CurYear == "2035")
            {
                NewString = "P";
            }
            else if (CurYear == "2036")
            {
                NewString = "Q";
            }
            else if (CurYear == "2037")
            {
                NewString = "R";
            }
            else if (CurYear == "2038")
            {
                NewString = "S";
            }
            else if (CurYear == "2039")
            {
                NewString = "T";
            }
            else if (CurYear == "2040")
            {
                NewString = "U";
            }
            else if (CurYear == "2041")
            {
                NewString = "V";
            }
            else if (CurYear == "2042")
            {
                NewString = "W";
            }
            else if (CurYear == "2043")
            {
                NewString = "X";
            }
            else if (CurYear == "2044")
            {
                NewString = "Y";
            }
            else if (CurYear == "2045")
            {
                NewString = "Z";
            }
            return NewString;

        }
        public static string GetCurrentMonthWiseAlphabet(string CurMonth)
        {

            string NewString = "";
            if (CurMonth == "Jan")
            {
                NewString = "A";
            }
            else if (CurMonth == "Feb")
            {
                NewString = "B";
            }
            else if (CurMonth == "Mar")
            {
                NewString = "C";
            }
            else if (CurMonth == "Apr")
            {
                NewString = "D";
            }
            else if (CurMonth == "May")
            {
                NewString = "E";
            }
            else if (CurMonth == "Jun")
            {
                NewString = "F";
            }
            else if (CurMonth == "Jul")
            {
                NewString = "G";
            }
            else if (CurMonth == "Aug")
            {
                NewString = "H";
            }
            else if (CurMonth == "Sep")
            {
                NewString = "I";
            }
            else if (CurMonth == "Oct")
            {
                NewString = "J";
            }
            else if (CurMonth == "Nov")
            {
                NewString = "K";
            }
            else if (CurMonth == "Dec")
            {
                NewString = "L";
            }
            return NewString;

        }
        public static string GetQuotedString(string input)
        {
            input = input.Trim();
            string NewString = "";
            if (input != "")
            {
                var arr = input.Split(',');
                foreach (var item in arr)
                {
                    NewString += "'" + item + "',";
                }
                NewString = NewString.TrimEnd(',');
            }
            return NewString;

        }
        public static XmlDocument ConvertToXml(Object list)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(list.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, list);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc;
            }
        }

        //public static Get_MST_EmailTamplate_Data_Result GetTemplate(string Name)
        //{
        //    HKESalesERPEntities context = new HKESalesERPEntities();
        //    return context.Get_MST_EmailTamplate_Data().Where(x => x.Active == true && x.TamplateName == Name).FirstOrDefault();
        //}

        public static bool SendOTPCode(string mobile, string message)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://login.redsms.in/API/SendMessage.ashx?user=dhavalpokiya&password=pokiya155&phone=" + mobile.Trim() + "&text=" + message + "&type=t&senderid=DBCACC");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            reader.Close();
            if (result.Contains("sent"))
            {
                return true;
            }
            else
            {
                return false;
            }
            //return !result.Contains("<error-code>");
        }
        public static string GetHPHTTagNo(int SrNo)
        {

            string NewString = "";
            if (SrNo == 1)
            {
                NewString = "A";
            }
            else if (SrNo == 2)
            {
                NewString = "B";
            }
            else if (SrNo == 3)
            {
                NewString = "C";
            }
            else if (SrNo == 4)
            {
                NewString = "D";
            }
            else if (SrNo == 5)
            {
                NewString = "E";
            }
            else if (SrNo == 6)
            {
                NewString = "F";
            }
            else if (SrNo == 7)
            {
                NewString = "G";
            }
            else if (SrNo == 8)
            {
                NewString = "H";
            }
            else if (SrNo == 9)
            {
                NewString = "I";
            }
            else if (SrNo == 10)
            {
                NewString = "J";
            }
            else if (SrNo == 11)
            {
                NewString = "K";
            }
            else if (SrNo == 12)
            {
                NewString = "L";
            }
            else if (SrNo == 13)
            {
                NewString = "M";
            }
            else if (SrNo == 14)
            {
                NewString = "N";
            }
            else if (SrNo == 15)
            {
                NewString = "O";
            }
            else if (SrNo == 16)
            {
                NewString = "P";
            }
            else if (SrNo == 17)
            {
                NewString = "Q";
            }
            else if (SrNo == 18)
            {
                NewString = "R";
            }
            else if (SrNo == 19)
            {
                NewString = "S";
            }
            else if (SrNo == 20)
            {
                NewString = "T";
            }
            else if (SrNo == 21)
            {
                NewString = "U";
            }
            else if (SrNo == 22)
            {
                NewString = "V";
            }
            else if (SrNo == 23)
            {
                NewString = "W";
            }
            else if (SrNo == 24)
            {
                NewString = "X";
            }
            else if (SrNo == 25)
            {
                NewString = "Y";
            }
            else if (SrNo == 25)
            {
                NewString = "Z";
            }
            return NewString;

        }
        public static int RendomNumber()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999);
        }

        public static String Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            //  Dim toEncryptArray As Byte() = UTF32Encoding.UTF32.GetBytes(toEncrypt)
            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //  Get the key from config file
            string key = Convert.ToString((settingsReader.GetValue("SecurityKey", typeof(String))));


            // key = "AdeF5ty6Fr456Mw###"
            // System.Windows.Forms.MessageBox.Show(key)
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                // keyArray = hashmd5.ComputeHash(UTF32Encoding.UTF32.GetBytes(key))
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
                // keyArray = UTF32Encoding.UTF32.GetBytes(key)
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static String Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get your key from config file to open the lock!
            string key = Convert.ToString((settingsReader.GetValue("SecurityKey", typeof(String))));

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                // keyArray = hashmd5.ComputeHash(UTF32Encoding.UTF32.GetBytes(key))
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
                // keyArray = UTF32Encoding.UTF32.GetBytes(key)
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static DataTable ReadExcelFile(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Read the Excel file into a DataSet
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true // Treat the first row as column headers
                        }
                    });

                    // Get the first (and usually only) table from the DataSet
                    DataTable dataTable = result.Tables[0];

                    return dataTable;
                }
            }
        }
        public static DataTable GetExcelfiledata(string filePath)
        {
            string strcon = string.Empty;
            string StrFileType = Path.GetExtension(filePath);
            if (StrFileType == ".xls")
            {
                strcon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                                + filePath +
                                ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";
            }
            else
            {
                strcon = @"Provider=Microsoft.ACE.OLEDB.12.0;";
                strcon += @"Data Source=" + filePath + ";";
                strcon += @"Extended Properties=""Excel 12.0 xml;HDR=YES;Imex=1;""";
                strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                              + filePath +
                              ";Extended Properties=\"Excel 12.0 xml;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";

                strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath +
                            ";Extended Properties=\"Excel 12.0 xml;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";
            }
            DataTable dtexcel = new DataTable();

            using (OleDbConnection excelCon = new OleDbConnection(strcon))
            {
                try
                {
                    ErrorLogger.ErrorLog("connection open");
                    excelCon.Open();
                    ErrorLogger.ErrorLog("connection open start");
                    //Start::Get Sheet Name Dynamically
                    DataTable dtSheetNames = excelCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dtSheetNames == null)
                    {
                        return null;
                    }
                    string strselect = "Select * from [" + dtSheetNames.Rows[0]["TABLE_NAME"] + "]";

                    using (OleDbDataAdapter exDA = new OleDbDataAdapter(strselect, excelCon))
                    {
                        exDA.Fill(dtexcel);
                    }
                }
                catch (OleDbException oledb)
                {
                    throw new Exception(oledb.Message.ToString());
                }
                finally
                {
                    excelCon.Close();
                }
            }

            return dtexcel;
        }

        public static string GetFinancialYear(DateTime VoucherDate)
        {
            int CurrentYear = Convert.ToInt32(VoucherDate.ToString("yy"));
            int PreviousYear = Convert.ToInt32(VoucherDate.ToString("yy")) - 1;
            int NextYear = Convert.ToInt32(VoucherDate.ToString("yy")) + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (VoucherDate.Month > 3)
                FinYear = CurYear + NexYear;
            else
                FinYear = PreYear + CurYear;
            return FinYear.Trim();

        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataSet ToDataSet<T>(IList<T> list)
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                t.Columns.Add(propInfo.Name, ColType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }

                t.Rows.Add(row);
            }

            return ds;
        }

        public static string NumbersToWords(int inputNumber)
        {
            int inputNo = inputNumber;

            if (inputNo == 0)
                return "ZERO";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("MINUS ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"ONE ", "TWO ", "THREE ", "FOUR ",
            "FIVE " ,"SIX ", "SEVEN ", "EIGHT ", "NINE "};
            string[] words1 = {"TEN ", "ELEVEN ", "TWELVE ", "THIRTEEN ", "FOURTEEN ",
            "FIFTEEN ","SIXTEEN ","SEVENTEEN ","EIGHTEEN ", "NINETEEN "};
            string[] words2 = {"TWENTY ", "THIRTY ", "FORTY ", "FIFTY ", "SIXTY ",
            "SEVENTY ","EIGHTY ", "NINETY "};
            string[] words3 = { "THOUSAND ", "LAKH ", "CRORE " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "HUNDRED ");
                if (u > 0 || t > 0)
                {
                    //if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        //  public static string SpellDecimal(decimal number)
        //  {
        //      string[] digit =
        //{
        //      "", "one", "two", "three", "four", "five", "six",
        //      "seven", "eight", "nine", "ten", "eleven", "twelve",
        //      "thirteen", "fourteen", "fifteen", "sixteen",
        //      "seventeen", "eighteen", "nineteen"
        //};

        //      string[] baseten =
        //{
        //      "", "", "twenty", "thirty", "fourty", "fifty",
        //      "sixty", "seventy", "eighty", "ninety"
        //};

        //      string[] expo =
        //{
        //      "", "thousand", "million", "billion", "trillion",
        //      "quadrillion", "quintillion"
        //};

        //      if (number == Decimal.Zero)
        //          return "zero";

        //      decimal n = Decimal.Truncate(number);
        //      decimal cents = Decimal.Truncate((number - n) * 100);

        //      StringBuilder sb = new StringBuilder();
        //      int thousands = 0;
        //      decimal power = 1;

        //      if (n < 0)
        //      {
        //          sb.Append("minus ");
        //          n = -n;
        //      }

        //      for (decimal i = n; i >= 1000; i /= 1000)
        //      {
        //          power *= 1000;
        //          thousands++;
        //      }

        //      bool sep = false;
        //      for (decimal i = n; thousands >= 0; i %= power, thousands--, power /= 1000)
        //      {
        //          int j = (int)(i / power);
        //          int k = j % 100;
        //          int hundreds = j / 100;
        //          int tens = j % 100 / 10;
        //          int ones = j % 10;

        //          if (j == 0)
        //              continue;

        //          if (hundreds > 0)
        //          {
        //              if (sep)
        //                  sb.Append(", ");

        //              sb.Append(digit[hundreds]);
        //              sb.Append(" hundred");
        //              sep = true;
        //          }

        //          if (k != 0)
        //          {
        //              if (sep)
        //              {
        //                  sb.Append(" and ");
        //                  sep = false;
        //              }

        //              if (k < 20)
        //                  sb.Append(digit[k]);
        //              else
        //              {
        //                  sb.Append(baseten[tens]);
        //                  if (ones > 0)
        //                  {
        //                      sb.Append("-");
        //                      sb.Append(digit[ones]);
        //                  }
        //              }
        //          }

        //          if (thousands > 0)
        //          {
        //              sb.Append(" ");
        //              sb.Append(expo[thousands]);
        //              sep = true;
        //          }
        //      }

        //      sb.Append(" and ");
        //      if (cents < 10) sb.Append("0");
        //      sb.Append(cents);
        //      sb.Append("/100");

        //      return sb.ToString();
        //  }

        //  public static string SpellDecimal(decimal number)
        //  {
        //      string[] digit =
        //{
        //      "", "one", "two", "three", "four", "five", "six",
        //      "seven", "eight", "nine", "ten", "eleven", "twelve",
        //      "thirteen", "fourteen", "fifteen", "sixteen",
        //      "seventeen", "eighteen", "nineteen"
        //};

        //      string[] baseten =
        //{
        //      "", "", "twenty", "thirty", "fourty", "fifty",
        //      "sixty", "seventy", "eighty", "ninety"
        //};

        //      string[] expo =
        //{
        //      "", "thousand", "million", "billion", "trillion",
        //      "quadrillion", "quintillion"
        //};

        //      StringBuilder sb = new StringBuilder();
        //      int thousands = 0;
        //      decimal power = 1;

        //      if (number < 0)
        //      {
        //          sb.Append("minus ");
        //          number = -number;
        //      }

        //      decimal n = Decimal.Truncate(number);
        //      decimal cents = Decimal.Truncate((number - n) * 100);

        //      if (n == Decimal.Zero)
        //          sb.Append("zero");

        //      for (decimal i = n; i >= 1000; i /= 1000)
        //      {
        //          power *= 1000;
        //          thousands++;
        //      }

        //      bool sep = false;
        //      for (decimal i = n; thousands >= 0; i %= power, thousands--, power /= 1000)
        //      {
        //          int j = (int)(i / power);
        //          int k = j % 100;
        //          int hundreds = j / 100;
        //          int tens = j % 100 / 10;
        //          int ones = j % 10;

        //          if (j == 0)
        //              continue;

        //          if (hundreds > 0)
        //          {
        //              if (sep)
        //                  sb.Append(", ");

        //              sb.Append(digit[hundreds]);
        //              sb.Append(" hundred");
        //              sep = true;
        //          }

        //          if (k != 0)
        //          {
        //              if (sep)
        //              {
        //                  sb.Append(" and ");
        //                  sep = false;
        //              }

        //              if (k < 20)
        //                  sb.Append(digit[k]);
        //              else
        //              {
        //                  sb.Append(baseten[tens]);
        //                  if (ones > 0)
        //                  {
        //                      sb.Append("-");
        //                      sb.Append(digit[ones]);
        //                  }
        //              }
        //          }

        //          if (thousands > 0)
        //          {
        //              sb.Append(" ");
        //              sb.Append(expo[thousands]);
        //              sep = true;
        //          }
        //      }

        //      sb.Append(" and ");
        //      if (cents < 10) sb.Append("0");
        //      sb.Append(cents);
        //      sb.Append("/100");

        //      return sb.ToString();
        //  }
        public class XMLReader
        {
            public List<commanprop> Product_Comman_Type(String type)
            {
                List<commanprop> _prop = new List<commanprop>();
                string xmlData = HttpContext.Current.Server.MapPath("~/Utility/xml_data/BlueStartStaticData.xml");
                int i = 0;
                XmlNodeList xmlnode;
                XmlDataDocument xmldoc = new XmlDataDocument();
                System.IO.FileStream fs = new System.IO.FileStream(xmlData, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                xmldoc.Load(fs);
                xmlnode = xmldoc.GetElementsByTagName(type.ToString());
                for (i = 0; i <= xmlnode.Count - 1; i++)
                {
                    //  xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                    _prop.Add(new commanprop() { id = Convert.ToInt32(xmlnode[i].ChildNodes.Item(0).InnerText.Trim()), name = xmlnode[i].ChildNodes.Item(1).InnerText.Trim(), type = xmlnode[i].ChildNodes.Item(2).InnerText.Trim() });
                }
                fs.Close();
                fs.Dispose();
                return _prop;
            }
        }



        public static string SpellDecimal(string input)
        {

            //string input = "123466265.123";

            // take decimal part of input. convert it to word. add it at the end of method.
            string decimals = "";

            if (input.Contains("."))
            {
                decimals = input.Substring(input.IndexOf(".") + 1);
                // remove decimal part from input
                input = input.Remove(input.IndexOf("."));
            }

            // Convert input into words. save it into strWords
            string strWords = GetWords(input);


            if (decimals.Length > 0)
            {
                // if there is any decimal part convert it to words and add it to strWords.
                strWords += " And " + GetWords(decimals) + " Cents";
            }
            return strWords.ToUpper();
            //Console.WriteLine(strWords);
        }

        public static string GetWords(string input)
        {
            // these are seperators for each 3 digit in numbers. you can add more if you want convert beigger numbers.
            string[] seperators = { "", " Thousand ", " Million ", " Billion " };

            // Counter is indexer for seperators. each 3 digit converted this will count.
            int i = 0;

            string strWords = "";

            while (input.Length > 0)
            {
                // get the 3 last numbers from input and store it. if there is not 3 numbers just use take it.
                string _3digits = input.Length < 3 ? input : input.Substring(input.Length - 3);
                // remove the 3 last digits from input. if there is not 3 numbers just remove it.
                input = input.Length < 3 ? "" : input.Remove(input.Length - 3);

                int no = int.Parse(_3digits);
                // Convert 3 digit number into words.
                _3digits = GetWord(no);

                // apply the seperator.
                _3digits += seperators[i];
                // since we are getting numbers from right to left then we must append resault to strWords like this.
                strWords = _3digits + strWords;

                // 3 digits converted. count and go for next 3 digits
                i++;
            }
            return strWords;
        }

        // your method just to convert 3digit number into words.
        private static string GetWord(int no)
        {
            string[] Ones =
        {
            "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
            "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Ninteen"
        };

            string[] Tens = { "Ten", "Twenty", "Thirty", "Fourty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninty" };

            string word = "";

            if (no > 99 && no < 1000)
            {
                int i = no / 100;
                word = word + Ones[i - 1] + " Hundred ";
                no = no % 100;
            }

            if (no > 19 && no < 100)
            {
                int i = no / 10;
                word = word + Tens[i - 1] + " ";
                no = no % 10;
            }

            if (no > 0 && no < 20)
            {
                word = word + Ones[no - 1];
            }

            return word;
        }

        [Serializable]
        [XmlRoot("Product_Comman_Type"), XmlType("Product_Comman_Type")]
        public class commanprop
        {
            public int id { set; get; }
            public string name { set; get; }
            public string type { set; get; }
        }

        public static double ToDouble(object pObj)
        {
            double Answer = 0;
            if (pObj == null) return Answer;
            if (double.TryParse(pObj.ToString(), out Answer))
            {
                return Answer;
            }
            else
            {
                return Answer;
            }
        }

        public static double Val(object pObj)
        {
            return ToDouble(pObj);
        }

        public static string FormatWithSeperatorWithCRDR(object Expression)
        {
            if (Expression == null)
            {
                return "";
            }
            double Answer = Val(Expression.ToString());

            if (Answer == 0)
            {
                return "";
            }

            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo(); // Use For Number Formate Saperator;
            info.NumberGroupSizes = new int[] { 3, 2 }; // Use For Number Formate Saperator;      

            //return Answer < 0 ? String.Format("{0:N2}", Math.Round((Answer * -1), 2), " Cr") : String.Format("{0:N2}", Math.Round(Answer, 2), " Dr");
            return Answer < 0 ? String.Format(info, "{0:#,#.#0}", Math.Round((Answer * -1), 2), "") + " Cr" : String.Format(info, "{0:#,#.#0}", Math.Round((Answer), 2), "") + " Dr";
        }

        public static string FormatWithSeperator(object Expression)
        {
            string strReturn = "";


            if (Expression == null)
            {
                return "";
            }
            double Answer = Val(Expression.ToString());
            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo(); // Use For Number Formate Saperator;
            info.NumberGroupSizes = new int[] { 3, 2 }; // Use For Number Formate Saperator;      

            if (Answer == 0)
            {
                return "";
            }
            else if (Answer >= -1 && Answer < 0)
            {
                strReturn = Answer.ToString();
            }
            else if (Answer > 0 && Answer <= 1)
            {
                strReturn = Answer.ToString();
            }
            else
            {
                strReturn = String.Format(info, "{0:#,#0.#0}", Math.Round(Answer, 2), "");
            }
            //return Answer < 0 ? String.Format("{0:N2}", Math.Round((Answer * -1), 2), " Cr") : String.Format("{0:N2}", Math.Round(Answer, 2), " Dr");
            return strReturn; // +" /-";
        }

        public static string FormatWithSeperatorWithOutRound(object Expression)
        {
            string strReturn = "";

            if (Expression == null)
            {
                return "";
            }
            double Answer = ToDouble(Expression.ToString());

            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo(); // Use For Number Formate Saperator;
            info.NumberGroupSizes = new int[] { 3, 2 }; // Use For Number Formate Saperator;   

            if (Answer == 0)
            {
                return "";
            }
            else if (Answer >= -1 && Answer < 0)
            {
                strReturn = Answer.ToString();
            }
            else if (Answer > 0 && Answer <= 1)
            {
                strReturn = Answer.ToString();
            }
            else
            {
                //return Answer < 0 ? String.Format("{0:N2}", Math.Round((Answer * -1), 2), " Cr") : String.Format("{0:N2}", Math.Round(Answer, 2), " Dr");
                return String.Format(info, "{0:#,#0.#0}", Answer, "");// +" /-";
            }

            return strReturn;
        }

        public static string FormatWithSeperatorWithRoundUp(object Expression)
        {
            if (Expression == null)
            {
                return "";
            }
            double Answer = ToDouble(Expression.ToString());
            if (Answer == 0)
            {
                return "";
            }

            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo(); // Use For Number Formate Saperator;
            info.NumberGroupSizes = new int[] { 3, 2 }; // Use For Number Formate Saperator;      

            //return Answer < 0 ? String.Format("{0:N2}", Math.Round((Answer * -1), 2), " Cr") : String.Format("{0:N2}", Math.Round(Answer, 2), " Dr");
            return String.Format(info, "{0:#,#0.#0}", Math.Round(Answer, 0), "").Replace(".00", "");// +" /-";
        }
        //kiran
        public static string FormatWithSeperatorWithRound_Four(object Expression)
        {
            if (Expression == null)
            {
                return "";
            }
            double Answer = ToDouble(Expression.ToString());
            if (Answer == 0)
            {
                return "";
            }

            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo(); // Use For Number Formate Saperator;
            info.NumberGroupSizes = new int[] { 5, 4 }; // Use For Number Formate Saperator;      

            //return Answer < 0 ? String.Format("{0:N2}", Math.Round((Answer * -1), 2), " Cr") : String.Format("{0:N2}", Math.Round(Answer, 2), " Dr");
            return String.Format(info, "{0:0.####}", Math.Round(Answer, 0), "").Replace(".0000", "");// +" /-";
        }
        private static readonly string[] Units =
{
        "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
        "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
    };

        private static readonly string[] Tens =
        {
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

        private static string ConvertLessThanOneThousand(int number)
        {
            string result;

            if (number % 100 < 20)
            {
                result = Units[number % 100];
                number /= 100;
            }
            else
            {
                result = Units[number % 10];
                number /= 10;

                result = Tens[number % 10] + " " + result;
                number /= 10;
            }

            if (number == 0)
                return result;

            return Units[number] + " Hundred " + result;
        }
        public static string ConvertToWords(decimal number)
        {
            if (number == 0)
                return "Zero";

            long numberInLong = (long)number;

            string numberString = numberInLong.ToString();
            string decimalString = number.ToString().Split('.')[1];

            string words = "";

            if (numberInLong / 1000000000000 > 0)
            {
                words += ConvertLessThanOneThousand((int)(numberInLong / 1000000000000)) + " Trillion ";
                numberInLong %= 1000000000000;
            }

            if (numberInLong / 10000000 > 0)
            {
                words += ConvertLessThanOneThousand((int)(numberInLong / 10000000)) + " Crore ";
                numberInLong %= 10000000;
            }

            if (numberInLong / 100000 > 0)
            {
                words += ConvertLessThanOneThousand((int)(numberInLong / 100000)) + " Lakh ";
                numberInLong %= 100000;
            }

            if (numberInLong / 1000 > 0)
            {
                words += ConvertLessThanOneThousand((int)(numberInLong / 1000)) + " Thousand ";
                numberInLong %= 1000;
            }

            if (numberInLong > 0)
            {
                words += ConvertLessThanOneThousand((int)numberInLong);
            }

            words = words.Trim();

            words += " And ";
            words += ConvertLessThanOneThousand((int)Convert.ToInt32(decimalString));
            words += " Only ";
            return words;
        }
        //public static string ConvertToWords(decimal number)
        //{
        //    if (number == 0)
        //        return "Zero";

        //    long numberInLong = (long)number;

        //    string numberString = numberInLong.ToString();
        //    string decimalString = number.ToString().Split('.')[1];

        //    string words = "";

        //    if (numberInLong / 1000000000000 > 0)
        //    {
        //        words += ConvertLessThanOneThousand((int)(numberInLong / 1000000000000)) + " Trillion ";
        //        numberInLong %= 1000000000000;
        //    }

        //    if (numberInLong / 1000000000 > 0)
        //    {
        //        words += ConvertLessThanOneThousand((int)(numberInLong / 1000000000)) + " Crore ";
        //        numberInLong %= 1000000000;
        //    }

        //    if (numberInLong / 1000000 > 0)
        //    {
        //        words += ConvertLessThanOneThousand((int)(numberInLong / 1000000)) + " Lakh ";
        //        numberInLong %= 1000000;
        //    }

        //    if (numberInLong / 1000 > 0)
        //    {
        //        words += ConvertLessThanOneThousand((int)(numberInLong / 1000)) + " Thousand ";
        //        numberInLong %= 1000;
        //    }

        //    if (numberInLong > 0)
        //    {
        //        words += ConvertLessThanOneThousand((int)numberInLong);
        //    }

        //    words = words.Trim();

        //    words += " And ";
        //    words += ConvertLessThanOneThousand((int)Convert.ToInt32(decimalString));
        //    words += " Only ";
        //    return words;
        //}
        //public static int FindRow(ExcelWorksheet ws, int cell, string text)
        //{
        //    for (int row = 1; row <= ws.Dimension.End.Row; row++)
        //    {
        //        if ((ws.Cells[row, cell].Value + "").Trim() == text)
        //            return row;
        //    }
        //    return -1;
        //}

        public static int FindRow(ExcelWorksheet ws, int cell, string text)
        {
            for (int row = 1; row <= ws.Dimension.End.Row; row++)
            {
                string cellValue = ws.Cells[row, cell].Value?.ToString().Trim();

                if (!string.IsNullOrEmpty(cellValue) &&
                    cellValue.Contains(text))
                {
                    return row;
                }
            }
            return -1;
        }

        public static decimal GetDecimal(ExcelRange cell)
        {
            if (cell?.Value == null) return 0;
            decimal.TryParse(cell.Value.ToString(), out decimal val);
            return val;
        }
        public static int? ToNullableInt(object val)
        {
            if (val == null) return null;
            if (val.ToString().Trim() == "") return null;
            return Convert.ToInt32(val);
        }

        public static decimal? ToNullableDecimal(ExcelRange cell)
        {
            if (cell == null || cell.Value == null) return null;
            if (string.IsNullOrWhiteSpace(cell.Text)) return null;

            return Convert.ToDecimal(cell.Value);
        }
    }
}