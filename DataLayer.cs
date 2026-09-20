using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

/*I used ADO.NET to connect my MySQL database to my C# program. This 
 * was done because I Entity Framework is not compatible with MySQL 
 * or with the ODBC connection that I used to connect MySQL to Visual Studio.
 * In order to use Entity Framework I would have to have used a different 
 * database management system, such as MSSQL. I have not tested with 
 * PostGres.
 * */

namespace BookLog
{

    internal class DataLayer
    {
        private static string connectionString = "Dsn=MySQL_VS2026_booklog;auto_is_null=1;big_packets=1;database=booklog;db=booklog;found_rows=1;multi_statements=1;no_schema=1;port=3306;server=localhost;uid=root;user=root";
        //private static string connectionString = "DSN=MySQL_VS2026booklog2;UID=root;PWD=24Horses!3Roses;";
        /* From Slides
        //fetch data sample
        public void getData(string table)
        {
            string sql = $"SELECT * FROM {table}";
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            { 
                

                OdbcCommand command = new OdbcCommand(sql, connection);
                connection.Open();

                OdbcDataReader reader=command.ExecuteReader();
            }
        }
        //add data sample
        public void addData(string table, string column, string input)
        {
            string sql = $"INSERT INTO {table} ({column}) VALUES ('{input}')";
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {               
                OdbcCommand command = new OdbcCommand(sql, connection);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} row(s) inserted into {table}.");
            }
        }
        //update data sample
        public void updateData(string table, string column, string input, int ID)
        {
            string sql = $"UPDATE {table} SET {column} = '{input}' WHERE {ID}";
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} row(s) updated in {table}.");
            }
        }
        */
        public void GetSingleBookRecord(int book_Id)
        {
            Console.WriteLine("GET SINGLE BOOK RECORD");

            string sql =
                "SELECT * " +
                "FROM Book " +
                "WHERE Book_Id = ?";

            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);

                // Add a value for the parameter.
                command.Parameters.AddWithValue("?", book_Id);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    /*
                     *I used AI for debugging an ODBC  issues: Odbc does not support named parameters
                     * Console.WriteLine(
                        $"Inside connection and reader.bookID= {reader["Book_Id"]}"
                        );*/
                    Console.WriteLine(
                        $"{reader["Book_Id"]} - " +
                        $"{reader["Title"]} - " +
                        //$"Length: {reader["LengthText"]} - " +
                        $"Narration Style: {reader["Narration_Style"]} - " +
                        $"Series: {reader["Series_Name"]} - " +
                        $"{reader["Num_in_Series"]}"
                        );
                }
                else
                {
                    Console.WriteLine($"No record found for Book_Id = {book_Id}");
                }

                // reader.Close();
            }

            Console.WriteLine("END GET SINGLE BOOK RECORD");
        }
        public int GetRecordCount(string table)
        {
            //Console.WriteLine("GET RECORD Count");

            string sql= $"SELECT COUNT(*) " +
                $"FROM {table}";

            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);

                // Add a value for the parameter.
                // command.Parameters.AddWithValue("?", table);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    /*
                     *I used AI for debugging an ODBC  issues: Odbc does not support named parameters
                     * Console.WriteLine(
                        $"Inside connection and reader.bookID= {reader["Book_Id"]}"
                        );*/
                    int recordCount = Convert.ToInt32(reader[0]);
                    //Console.WriteLine($"{recordCount} - " );
                    return recordCount;
                }
                else
                {
                    Console.WriteLine($"No record found for table = {table}");
                    return 0;
                }
            }

            //Console.WriteLine("END GET RECORD Count");

        }
        private static void Nope()
        {
            Console.WriteLine("Type any character to return to the main menu");
            string choice11 = Console.ReadLine();
        }
        public void GetAllRecords(string table)
        {
           switch (table)
            {
                case "1":
                    table = "log";
                    break;
                case "2":
                    table = "book";
                    break;
                case "3":
                    table = "author";
                    break;
                case "4":
                    table = "narrator";
                    break;
            }
            //Console.WriteLine("GET ALL RECORDS ")

            string sql;
            if (table.ToLower() == "author") {
                sql =
                  "SELECT a.author_ID AS author_ID, a.first AS first, a.middle AS middle, a.last AS last, b.title AS title " +
                  "FROM Book_Author_Junct baj " +
                  "JOIN book b ON baj.book_ID = b.book_ID " +
                  "JOIN author a ON baj.author_ID = a.author_ID";
            } else if(table.ToLower() == "narrator")
            {
                sql =
                   "SELECT n.narrator_ID AS narrator_ID, n.first AS first, n.middle AS middle, n.last AS last, b.title AS title " +
                   "FROM Book_Narrator_Junct bnaj " +
                   "JOIN book b ON bnaj.book_ID = b.book_ID " +
                   "JOIN narrator n ON bnaj.narrator_ID = n.narrator_ID";

            } else if (table.ToLower() == "book")
            {
                sql =
                    "SELECT  b.book_ID as Book_Id, b.title AS title, b.narration_style AS narration_style, b.series_name AS series_name, b.num_in_series AS num_in_series, a.first AS first, a.middle AS middle, a.last AS last " +
                    "FROM Book_Author_Junct baj " +
                    "JOIN book b ON baj.book_ID = b.book_ID " +
                    "JOIN author a ON baj.author_ID = a.author_ID";

            }
            else
            {
                sql = $"SELECT * " +
                    $"FROM {table}";
            }
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);

                // Add a value for the parameter.
                // command.Parameters.AddWithValue("?", table);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                { 
                    switch (table.ToLower())
                    {
                        case "3":
                        case "author":
                            Console.WriteLine("Author Id- Author Name - Book Titles");
                            while (reader.Read())
                            {
                                Console.WriteLine(
                                    $"{reader["author_ID"]} - " +
                                    $"{reader["first"]} " +
                                    $"{reader["middle"]} " +
                                    $"{reader["last"]} - " +
                                    $"{reader["title"]}"
                                );
                            }
                            break;
                        case "2":
                        case "book":
                            Console.WriteLine("Book Id - Title -  Narration Style - Series Name - Number in Series - Authors");
                            while (reader.Read())
                            {
                                Console.WriteLine(
                                    $"{reader["Book_Id"]} - " +
                                    $"{reader["Title"]} - " +
                                    //$"{reader["Length"]} - " +
                                    $"{reader["Narration_Style"]} - " +
                                    $"{reader["Series_Name"]} - " +
                                    $"{reader["Num_in_Series"]} - " +
                                    $"{reader["First"]} {reader["Middle"]} {reader["Last"]}"
                                );
                            }
                            break;
                        case "1":
                        case "log":
                            Console.WriteLine("Start Date - End Date - Book ID - Rating");
                            while (reader.Read())
                            {
                                DateTime startDate = Convert.ToDateTime(reader["Start_Date"]);
                                DateTime endDate = Convert.ToDateTime(reader["End_Date"]);
                                Console.WriteLine(
                                    $"{startDate:yyyy-MM-dd} - " +
                                    $"{endDate:yyyy-MM-dd} - " +
                                    $"{reader["Book_Id"]} - " +
                                    $"{reader["Rating"]}"
                                );
                            }
                            break;
                        case "4":
                        case "narrator":
                            Console.WriteLine("Narrator Id- Narrator Name - Book Titles");
                            while (reader.Read())
                            {
                                    Console.WriteLine(
                                    $"{reader["Narrator_ID"]} - " +
                                    $"{reader["First"]} " +
                                    $"{reader["Middle"]} " +
                                    $"{reader["Last"]} - " +
                                    $"{reader["Title"]}"
                                    );
                                }
                                break;
                            default:
                                Console.WriteLine("The selected table is not availabe");
                                break;
                        }
                        Console.WriteLine("\n\n");

                 }  else
                {
                    Console.WriteLine($"No record found for table = {table}");
                    //return 0;
                }
            }
        }
        public List<String> GetAllRecordsAndID(string table)
        {
            switch (table)
            {
                case "1":
                    table = "log";
                    break;
                case "2":
                    table = "book";
                    break;
                case "3":
                    table = "author";
                    break;
                case "4":
                    table = "narrator";
                    break;
            }

            //Console.WriteLine("GET ALL RECORDS ");
            List<string> IdList = new List<string>();
            string sql;
            if (table.ToLower() == "author")
            {
                sql =
                  "SELECT a.author_ID AS author_ID, a.first AS first, a.middle AS middle, a.last AS last, b.title AS title " +
                  "FROM Book_Author_Junct baj " +
                  "JOIN book b ON baj.book_ID = b.book_ID " +
                  "JOIN author a ON baj.author_ID = a.author_ID";
            }
            else if (table.ToLower() == "narrator")
            {
                sql =
                   "SELECT n.narrator_ID AS narrator_ID, n.first AS first, n.middle AS middle, n.last AS last, b.title AS title " +
                   "FROM Book_Narrator_Junct bnaj " +
                   "JOIN book b ON bnaj.book_ID = b.book_ID " +
                   "JOIN narrator n ON bnaj.narrator_ID = n.narrator_ID";

            }
            else if (table.ToLower() == "book")
            {
                sql =
                    "SELECT  b.book_ID as Book_Id, b.title AS title, b.narration_style AS narration_style, b.series_name AS series_name, b.num_in_series AS num_in_series, a.first AS first, a.middle AS middle, a.last AS last " +
                    "FROM Book_Author_Junct baj " +
                    "JOIN book b ON baj.book_ID = b.book_ID " +
                    "JOIN author a ON baj.author_ID = a.author_ID";

            }
            else
            {
                sql = $"SELECT * " +
                    $"FROM {table}";
            }

            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);

                // Add a value for the parameter.
                // command.Parameters.AddWithValue("?", table);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.HasRows)
                    {
                        switch (table.ToLower())
                        {
                            case "3":
                            case "author":
                                Console.WriteLine("Author Id- Author Name");
                                while (reader.Read())
                                {
                                    IdList.Add($"{reader["Author_ID"]}");

                                    Console.WriteLine(
                                    $"{reader["author_ID"]} - " +
                                    $"{reader["first"]} " +
                                    $"{reader["middle"]} " +
                                    $"{reader["last"]} - " +
                                    $"{reader["title"]}"
                                );
                                }
                                break;
                            case "2":
                            case "book":
                                Console.WriteLine("Book Id - Title -  Narration Style - Series Name - Number in Series");
                                while (reader.Read())
                                {
                                    IdList.Add($"{reader["Book_Id"]}");
                                    //eliminating length variable after failure to debug ODBC issues with Time, TimeSpan, and cast as char
                                    //TimeSpan bookLength = (TimeSpan)reader["Length"];
                                    Console.WriteLine(
                                    $"{reader["Book_Id"]} - " +
                                    $"{reader["Title"]} - " +
                                    //$"{reader["Length"]} - " +
                                    $"{reader["Narration_Style"]} - " +
                                    $"{reader["Series_Name"]} - " +
                                    $"{reader["Num_in_Series"]} - " +
                                    $"{reader["First"]} {reader["Middle"]} {reader["Last"]}"
                                );
                                }
                                break;
                            case "1":
                            case "log":
                                Console.WriteLine("Start Date - End Date - Book ID - Rating");
                                while (reader.Read())
                                {
                                    DateTime startDate = Convert.ToDateTime(reader["Start_Date"]);
                                    DateTime endDate = Convert.ToDateTime(reader["End_Date"]);
                                    Console.WriteLine(
                                        $"{startDate:yyyy-MM-dd} - " +
                                        $"{endDate:yyyy-MM-dd} - " +
                                        $"{reader["Book_Id"]} - " +
                                        $"{reader["Rating"]}"
                                    );
                                }
                                break;
                            case "4":
                            case "narrator":
                                Console.WriteLine("Narrator Id- Narrator Name");
                                while (reader.Read())
                                {
                                    IdList.Add($"{reader["Narrator_ID"]}");
                                    Console.WriteLine(
                                    $"{reader["Narrator_ID"]} - " +
                                    $"{reader["First"]} " +
                                    $"{reader["Middle"]} " +
                                    $"{reader["Last"]} - " +
                                    $"{reader["Title"]}"
                                    );
                                }
                                break;
                            default:
                                Console.WriteLine("The selected table is not availabe");
                                break;
                        }
                        Console.WriteLine("\n\n");

                    }
                }
                else
                {
                    Console.WriteLine($"No record found for table = {table}");
                    //return 0;
                }

            }
            return IdList;


        }

        /*Insertion functions*/
        public void InsertLogData( string startDate, string bookID, string endDate= "1000-01-01", string rating="0")
        { 
            string sql =
                $"INSERT INTO log " +
                $"(start_Date, end_Date, book_Id, rating)" +
                $"VALUES"+
                $"(?, ?, ?, ?)";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
                command.Parameters.AddWithValue("?", startDate);
                command.Parameters.AddWithValue("?", endDate);
                command.Parameters.AddWithValue("?", bookID);
                command.Parameters.AddWithValue("?", rating);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected}");
                    GetAllRecords("log");
                    Nope();                }
                else { 
                    Console.WriteLine($"Unable to insert log entry with Start Date {startDate}, Book ID {bookID}, EndDate {endDate} please try again");
                    Nope();
                }

            }

        }
        public void InsertBookData(string title, string narrationStyle, string series="",string numInSeries="0")
        {
            string bookIDsql = "SELECT MAX(book_ID) FROM book";
            int currentID;
            
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command1 = new OdbcCommand(bookIDsql, connection);

                connection.Open();
                OdbcDataReader reader = command1.ExecuteReader();
                if (reader.Read())
                {
                    currentID = Convert.ToInt32(reader[0]);
                    currentID++;
                    Console.WriteLine($"{currentID} - \n\n" );

                    //insert
                    string insertSQL = $"INSERT INTO book " +
                    $"(book_ID, title,length, narration_style, series_name, num_in_series)" +
                    $"VALUES" +
                    $"(?, ?, '1:00', ?, ?, ?)";
                    OdbcCommand command2 = new OdbcCommand(insertSQL, connection);


                    command2.Parameters.AddWithValue("?", currentID);
                    command2.Parameters.AddWithValue("?", title);
                    //command2.Parameters.AddWithValue("?", length);
                    command2.Parameters.AddWithValue("?", narrationStyle);
                    command2.Parameters.AddWithValue("?", series);
                    command2.Parameters.AddWithValue("?", numInSeries);
                    
                    int rowsAffected = command2.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Rows inserted: {rowsAffected}\n");
                        GetAllRecords("book");
                        Nope();
                    }
                    else
                    {
                        Console.WriteLine($"Unable to insert book entry with Title {title}, , Narration Style {narrationStyle}, Series {series}, Number in Series {numInSeries} please try again");
                        Nope();
                    }

                }
                else
                {
                    Console.WriteLine($"No records found for latest ID");
                    Nope();
                }

                
            }
        }
        public void InsertAuthorData(string first,string middle,string last)
        {
            string authorIDsql = "SELECT MAX(author_ID) FROM Author";
            int currentID;

            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command1 = new OdbcCommand(authorIDsql, connection);

                connection.Open();
                OdbcDataReader reader = command1.ExecuteReader();

                if (reader.Read())
                {
                    currentID = Convert.ToInt32(reader[0]);
                    currentID++;
                    Console.WriteLine($"Current ID={currentID} - \n\n");

                    //insert
                    string insertSQL = $"INSERT INTO author " +
                    $"(author_Id, first, middle, last)" +
                    $"VALUES" +
                    $"(?, ?, ?, ?)";

                    OdbcCommand command2 = new OdbcCommand(insertSQL, connection);


                    command2.Parameters.AddWithValue("?", currentID);
                    command2.Parameters.AddWithValue("?", first);
                    command2.Parameters.AddWithValue("?", middle);
                    command2.Parameters.AddWithValue("?", last);
                   

                    int rowsAffected = command2.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Rows inserted: {rowsAffected}");
                        GetAllRecords("author");
                        Nope();
                    }
                    else
                    {
                        Console.WriteLine($"Unable to insert author entry with name {first} {middle} {last} please try again");
                        Nope();
                    }

                }
                else
                {
                    Console.WriteLine($"No records found for latest ID");
                    Nope();
                }


            }
        }
        public void InsertNarratorData(string first, string middle, string last)
        {
            string authorIDsql = "SELECT MAX(narrator_ID) FROM narrator";
            int currentID;

            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command1 = new OdbcCommand(authorIDsql, connection);

                connection.Open();
                OdbcDataReader reader = command1.ExecuteReader();

                if (reader.Read())
                {
                    currentID = Convert.ToInt32(reader[0]);
                    currentID++;
                    Console.WriteLine($"Current ID={currentID} - \n\n");

                    //insert
                    string insertSQL = $"INSERT INTO narrator " +
                    $"(narrator_Id, first, middle, last)" +
                    $"VALUES" +
                    $"(?, ?, ?, ?)";

                    OdbcCommand command2 = new OdbcCommand(insertSQL, connection);


                    command2.Parameters.AddWithValue("?", currentID);
                    command2.Parameters.AddWithValue("?", first);
                    command2.Parameters.AddWithValue("?", middle);
                    command2.Parameters.AddWithValue("?", last);


                    int rowsAffected = command2.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Rows inserted: {rowsAffected}");
                        GetAllRecords("narrator");
                        Nope();
                    }
                    else
                    {
                        Console.WriteLine($"Unable to insert narrator entry with name {first} {middle} {last} please try again");
                        Nope();
                    }

                }
                else
                {
                    Console.WriteLine($"No records found for latest ID");
                    Nope();
                }


            }
        }
        /*Update functions*/

        public void UpdateLogData(string editColumn, string updateStart, string updateID , string newVal)
        {
            string sql =
                $"UPDATE log " +
                $"SET {editColumn} = {newVal} "+
                $"WHERE Start_Date = '{updateStart}' AND Book_ID = {updateID}";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter. I kept gettign errors, so I did not use parameterized queries for this one
               /* 
                command.Parameters.AddWithValue("?", newVal);
                command.Parameters.AddWithValue("?", updateStart);
                command.Parameters.AddWithValue("?", updateID);
               */

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows Update {rowsAffected} with Start Date {updateStart} and Book ID {updateID} ");
                    GetAllRecords("log");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update log entry with Start Date {updateStart} and Book ID {updateID}, please try again");
                    Nope();
                }

            }

        }
        public void UpdateBookData(string editColumn, string updateID, string newVal)
        {
            string sql =
                 $"UPDATE book " +
                 $"SET {editColumn}= ? " +
                 $"WHERE book_ID = ?";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
                command.Parameters.AddWithValue("?", newVal);
                command.Parameters.AddWithValue("?", updateID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with  Book ID {updateID}\n\n");
                    GetAllRecords("book");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update Book with  Book ID {updateID}, please try again");
                    Nope();
                }


            }
        }
        public void UpdateAuthorData(string editColumn, string updateID, string newVal)
        {
            string sql =
                  $"UPDATE author " +
                  $"SET {editColumn}= ? " +
                  $"WHERE author_ID = ?";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
                command.Parameters.AddWithValue("?", newVal);
                command.Parameters.AddWithValue("?", updateID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected}with  Author_ID {updateID}\n\n");
                    GetAllRecords("author");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update Author with  Author_ID {updateID}, please try again");
                    Nope();
                }
            }
        }
        public void UpdateNarratorData(string editColumn, string updateID, string newVal)
        {
            string sql =
                  $"UPDATE narrator " +
                  $"SET {editColumn}= ? " +
                  $"WHERE narrator_ID = ?";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
                command.Parameters.AddWithValue("?", newVal);
                command.Parameters.AddWithValue("?", updateID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with narrator_ID {updateID}\n\n");
                    GetAllRecords("narrator");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update Narrator with narrator_ID {updateID}, please try again");
                    Nope();
                }


            }
        }


        public void UpdateAuthorBookConnect(string updateBookID, string updateAuthorID)
        {
            string sql =
                  $"INSERT INTO Book_Author_Junct (book_ID, author_ID) " +
                  $"VALUES (?, ?)";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
                command.Parameters.AddWithValue("?", updateBookID);
                command.Parameters.AddWithValue("?", updateAuthorID);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with book_ID {updateBookID} and author_ID {updateAuthorID}\n\n");
                    GetAllRecords("author");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update book_author_junct with book_ID {updateBookID} and author_ID {updateAuthorID}, please try again");
                    Nope();
                }
            }
        }
        public void UpdateNarratorBookConnect(string updateBookID, string updateNarratorID)
        {
            string sql =
                  $"INSERT INTO Book_Narrator_Junct (book_ID, narrator_ID) " +
                  $"VALUES (?, ?)";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
                command.Parameters.AddWithValue("?", updateBookID);
                command.Parameters.AddWithValue("?", updateNarratorID);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with book_ID {updateBookID} and narrator_ID {updateNarratorID}\n\n");
                    GetAllRecords("narrator");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update book_narrator_junct with book_ID {updateBookID} and narrator_ID {updateNarratorID}, please try again");
                    Nope();
                }
            }
        }
        /*Deletefunctions*/

        public void DeleteLogData( string updateStart, string updateID)
        {
            string sql =
                $"DELETE FROM log " +
                $"WHERE Start_Date = '{updateStart}' AND Book_ID = {updateID}";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter. I kept gettign errors, so I did not use parameterized queries for this one
                /* 
                 command.Parameters.AddWithValue("?", newVal);
                 command.Parameters.AddWithValue("?", updateStart);
                 command.Parameters.AddWithValue("?", updateID);
                */

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows Update {rowsAffected} with Start Date {updateStart} and Book ID {updateID} ");
                    GetAllRecords("log");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update log entry with Start Date {updateStart} and Book ID {updateID}, please try again");
                    Nope();
                }

            }

        }
        public void DeleteBookData(string updateID)
        {
            string sql =
                 $"DELETE FROM book " +
                 $"WHERE book_ID = ?";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
               
                command.Parameters.AddWithValue("?", updateID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with  Book ID {updateID}\n\n");
                    GetAllRecords("book");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update Book with  Book ID {updateID}, please try again");
                    Nope();
                }


            }
        }
        public void DeleteAuthorData( string updateID)
        {
            string sql =
                  $"DELETE FROM author " +
                  
                  $"WHERE author_ID = ?";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
               
                command.Parameters.AddWithValue("?", updateID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with  Author_ID {updateID}\n\n");
                    GetAllRecords("author");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update Author with  Author_ID {updateID}, please try again");
                    Nope();
                }
            }
        }
        public void DeleteNarratorData(string updateID )
        {
            string sql =
                  $"DELETE FROM narrator " +
                
                  $"WHERE narrator_ID = ?";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.
               
                command.Parameters.AddWithValue("?", updateID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with narrator_ID {updateID}\n\n");
                    GetAllRecords("narrator");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update Narrator with narrator_ID {updateID}, please try again");
                    Nope();
                }


            }
        }
        public void DeleteAuthorBookConnect(string updateBookID, string updateAuthorID) {
            string sql =
                      $"DELETE FROM book_author_junct " +

                      $"WHERE book_ID = ? AND author_ID = ?";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.

                command.Parameters.AddWithValue("?", updateBookID);
                command.Parameters.AddWithValue("?", updateAuthorID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with book_ID {updateBookID} and author_ID {updateAuthorID}\n\n");
                    GetAllRecords("author");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update book_author_junct with book_ID {updateBookID} and author_ID {updateAuthorID}, please try again");
                    Nope();
                }


            }
        }
        public void DeleteNarratorBookConnect(string updateBookID, string updateNarratorID) {
            string sql =
                      $"DELETE FROM book_narrator_junct " +

                      $"WHERE book_ID = ? AND narrator_ID = ?";
            using (OdbcConnection connection = new OdbcConnection(DataLayer.connectionString))
            {
                OdbcCommand command = new OdbcCommand(sql, connection);
                // Add a value for the parameter.

                command.Parameters.AddWithValue("?", updateBookID);
                command.Parameters.AddWithValue("?", updateNarratorID);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Rows inserted: {rowsAffected} with book_ID {updateBookID} and narrator_ID {updateNarratorID}\n\n");
                    GetAllRecords("narrator");
                    Nope();
                }
                else
                {
                    Console.WriteLine($"Unable to update book_narrator_junct with book_ID {updateBookID} and narrator_ID {updateNarratorID}, please try again");
                    Nope();
                }


            }
        }
    }

}

