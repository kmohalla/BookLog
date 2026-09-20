using BookLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

/* FRAMEWORK AND SQL DATABASE MANAGEMENT SYSTEM CHOICES
 * I chose to use .NET Framework 4.7.2 for this project, because I'm 
 * more familiar with C++ than python and .NET Framework more closely
 * resembles that format.
 * 
 * I chose MySQL for building my database because MSSQL is not 
 * compatible with my computer and MySQL is a program that is 
 * compatible with both my computer and the instructor's.
 */

/*AI USAGE
 * I used Gemini via google search to get instructions for connecting
 * MySQL to Visual studio. Prior to using AI I tried searching both 
 * via google (without AI) and youtube, but the resources I found were
 * for outdated verions of either Visual Studio or MySQL or both. I 
 * also tried searching the MySQL documentation, but could only find 
 * information about connection strings.
 * 
 * For MySQL (and possibly postgres) connection must be done through 
 * Server Explorer rather than the SQL Server Object Explorer. By 
 * following the steps provided by Gemini, I was able to get MySQL 
 * connected by adding a system DSN connection directly to the 
 * database and then connecting that to the Server Explorer via 
 * Microsoft ODBC Data Source.
 * 
 * Server Explorer functions similarly to SQL Server Object Explorer
 *  except that it does not show keys, constraints, etc. for the 
 *  tables, only the columns. "Script as" and other functions are not
 *  available, but data can be shown and queries can be executed.
 */
namespace MinorProject1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataLayer dataLayer = new DataLayer();
            //method testing
            //dataLayer.GetSingleBookRecord(3000);
            //dataLayer.GetRecordCount("Book");
            //dataLayer.GetAllRecords("Author");

            Console.WriteLine("Welcome to the BookLog Database, your place to log information about the audiobooks you listen to." +
                "\n\n" +
                "There are hidden tables that link authors and narrators to books. This makes it possible to have multiple authors and/or narrators for each book, and also makes it possible to have an author or narrator contribute to multiple books." +
                "\n\n" +
                "If you would like to delete an author or narrator you may see a warning that they have contributed to more than one book." +
                "\n\n" +
                "To connect existing authors, narrators, and books use the update data option below. To remove a connection use the delete data option below" +
                "\n\n"
            );
            ShowDescription(dataLayer);
            String choice1 = Console.ReadLine();
            while (choice1 != "-1")
            {
                string choice2 = "";
               // string choice11 = "";
                switch (choice1.ToLower())
                {
                    case "1":
                    case "get data":
                        Console.WriteLine(" Which table would you like to view: (1) log, (2) book, (3) author, or (4) narrator?");
                        choice2 = Console.ReadLine();
                        dataLayer.GetAllRecords(choice2);
                        Nope();
                        break;
                    case "2":
                    case "add data":
                        Console.WriteLine(" Which table would you like to add to (1) log, (2) book, (3) author, or (4) narrator?");
                        choice2 = Console.ReadLine();
                        AddRecordsHelper(dataLayer, choice2);
                        break;
                    case "3":
                    case "update data":
                        Console.WriteLine(" Which table would you like to update (1) log, (2) book, (3) author, (4) narrator, (5) connect author to book, or (6) connect narrator to book?");
                        choice2 = Console.ReadLine();
                        UpdateRecordsHelper(dataLayer, choice2);
                        break;
                    case "4":
                    case "delete data":
                        Console.WriteLine(" Which table would you like to delete from (1) log, (2) book, (3) author, (4) narrator,(5) delete author from book, or (6) delete narrator from book?");
                        choice2 = Console.ReadLine();
                        DeleteRecordsHelper(dataLayer, choice2);
                        break;
                    default:
                        Console.WriteLine("That input didn't work. Please try again. You can either type the number in parentheses or the action to continue");
                        Nope();
                        break;
                }
                ShowDescription(dataLayer);
                choice1 = Console.ReadLine();
            }

        }

        private static void ShowDescription(DataLayer dataLayer)
        {
            Console.WriteLine(
                $"____________+_____________+____________________________________________________________________________________________\n" +
                $"Table       | Record count| Description\n" +
                $"____________+_____________+____________________________________________________________________________________________\n" +
                $"Reading Log |     {dataLayer.GetRecordCount("Log")}      | This table tracks the start and end date for each book you've read and what you rated them \n" +
                $"            |             | for each log entry. You can add to this table as long as you have a unique start date and \n" +
                $"            |             | book pair. If you skip adding a end date, the date will be '1000-01-01'. You can track \n" +
                $"            |             | multiple books that start on the same day and rereads here, but you can't log a book as \n" +
                $"            |             | started twice on the same day even with a different end date. From here you can see all of \n" +
                $"            |             | the information for books you've read.\n" +
                $"____________+_____________+____________________________________________________________________________________________\n" +
                $"Book        |     {dataLayer.GetRecordCount("Book")}      | This table tracks the title, narration style, series name, and number in series \n" +
                $"            |             | for each book added. These can be used in the log, but you can also add books to read later.\n" +
                $"            |             | All fields are optional here, but you need at least one to get started. When you add books \n" +
                $"            |             | to this table you'll be asked if you want to add authors or narrators as well, but this is \n" +
                $"            |             | also optional and you can always add them later if you'd like. Associated authors and  \n" +
                $"            |             | narrators will appear along with your books.\n" +
                $"____________+_____________+____________________________________________________________________________________________\n" +
                $"Author      |     {dataLayer.GetRecordCount("Author")}      | This table tracks first, middle, and last names of authors. You can add authors for books\n" +
                $"            |             |  you've in the Book table or authors whose work you want to read later. All fields are \n" +
                $"            |             | optional here but you need at least one to get started. When you add an author you'll be \n" +
                $"            |             | asked if you want to add that author to a book, but this is also optional and you can also\n" +
                $"            |             | add them later if you'd like. A list of titles that each author has contributed to will \n" +
                $"            |             | appear beside their name.\n" +
                $"____________+_____________+____________________________________________________________________________________________\n" +
                $"Narrator    |     {dataLayer.GetRecordCount("Narrator")}      | This table tracks first, middle, and last names of narrators. You can add narrators for \n" +
                $"            |             | books you've in the Book table or narrators whose work you want to read later. All fields \n" +
                $"            |             | are optional here but you need at least one to get started. When you add a narrator you'll \n" +
                $"            |             | be asked if you want to add that narrator to a book, but this is also optional and you can \n" +
                $"            |             | also add them later if you'd like. A list of titles a narrator has contributed to will \n" +
                $"            |             | appear beside their name.\n" +
                $"____________+_____________+____________________________________________________________________________________________\n\n"
            );

            Console.WriteLine("Would you like to (1) Get data, (2) Add Data, (3) Update data, (4) Delete data? (type -1 to end)");
        }
       private static void Nope()
        {
            Console.WriteLine("Type any character to return to the main menu");
            string choice11 = Console.ReadLine();
        }
        private static void AddRecordsHelper(DataLayer dataLayer, string table)
        {
            //all
            string confirmationStatus;

            //add log
            string startDate;
            string endDate;
            string bookStatus;
            string bookIDtoLog;
            string bookCompletionStatus ;
            string rating;

            //add book
            string bookTitle;
            //string bookLength;
            string narration;
            List<string> narrationOptions = new List<string> { "1", "single", "2", "duet", "3", "multiple", "4", "full cast", "5", "unknown" };
            string narrationStyle;
            string seriesCheck;
            string series;
            string seriesNo;

            //add contributors
            string firstName;
            string middleName;
            string lastName;

            switch (table.ToLower()){
                case "1":
                case "log":
                    //get start date
                    Console.WriteLine("What date did you start reading? (format: yyyy-mm-dd)");
                    startDate = Console.ReadLine();
                    
                    //get bookID
                    Console.WriteLine("Is this book already in your book table? (y/n)");
                    bookStatus= Console.ReadLine();
                    bookStatus = bookStatus.ToLower();
                    if (bookStatus == "y" || bookStatus == "yes")
                    {
                        List<String> booksAvailable=dataLayer.GetAllRecordsAndID("book");
                        Console.WriteLine("Which book would you like to add to your log?");
                        bookIDtoLog = Console.ReadLine();
                        if (!booksAvailable.Contains(bookIDtoLog)){
                            Console.WriteLine("The ID you entered does not match a book.");
                            Nope();
                            return;
                        }
                        
                        //handle: check if the book has already been logged with this start date
                        //get finish date
                        Console.WriteLine("Have you finished this book? (y/n)");
                        bookCompletionStatus = Console.ReadLine();
                        bookCompletionStatus = bookCompletionStatus.ToLower();
                        if (bookCompletionStatus == "y" || bookCompletionStatus == "yes")
                        {
                            Console.WriteLine("What date did you finish reading? (format: yyyy-mm-dd)");
                            endDate = Console.ReadLine();
                            Console.WriteLine("How would you rate the book? (1-5)");
                            rating=Console.ReadLine();
                            Console.WriteLine($" Would you like to add the folowing entry to the log? y/n \n Start Date {startDate}, Book ID {bookIDtoLog}, EndDate {endDate}, rating {rating}");
                            confirmationStatus= Console.ReadLine();
                            if (confirmationStatus== "y" || confirmationStatus == "yes")
                            {
                                Console.WriteLine("Adding Entry");
                                dataLayer.InsertLogData(startDate:startDate, bookID:bookIDtoLog, endDate:endDate, rating: rating);
                                //call func to insert into book
                            }
                            else
                            {
                                Nope();
                                return;
                            }


                        }
                        else if (bookCompletionStatus == "n" || bookCompletionStatus == "no")
                        {
                            Console.WriteLine($"No problem! Would you like to add the folowing entry to the log? y/n \n Start Date {startDate}, Book ID {bookIDtoLog}");
                            confirmationStatus = Console.ReadLine();
                            if (confirmationStatus == "y" || confirmationStatus == "yes")
                            {
                                Console.WriteLine("Adding Entry");
                                dataLayer.InsertLogData(startDate: startDate, bookID: bookIDtoLog);
                    
                            }
                            else
                            {
                                {
                                    Console.WriteLine("Returning to main menu");
                                    Nope();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input");
                            Nope();
                            return;
                        }
                    }
                    else if (bookStatus == "n" || bookStatus == "no")
                    {
                        Console.WriteLine("Please add the book first then return here.");
                        Nope();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("The response entered is not valid");
                        Nope();
                        return;

                    }
                    
                    break;

                case "2":
                case "book":
                    Console.WriteLine("What is the title of your book?");
                    bookTitle = Console.ReadLine();
                    //Console.WriteLine("How long is your book? (format: HH:MM:SS)");
                    //bookLength=Console.ReadLine();
                    Console.WriteLine("What style is the narration?  (1) single, (2) duet, (3) multiple, (4) full cast, (5) unknown");
                    narration=Console.ReadLine();

                    //validate narration style
                    if (!narrationOptions.Contains(narration)){
                        Console.WriteLine(" That option is not valid. Please restart and try again.");
                        Nope() ;
                        return;
                    }

                    //fix if number was used
                    switch (narration.ToLower())
                    {
                        case "1":
                            narrationStyle = "single";
                            break;
                        case "2":
                            narrationStyle = "duet";
                            break;
                        case "3":
                            narrationStyle = "multiple";
                            break;
                        case "4":
                            narrationStyle = "full cast";
                            break;
                         case "5":
                            narrationStyle = "unknown";
                            break;
                        default:
                            narrationStyle = narration;
                            break;
                    }

                    Console.WriteLine("Is your book part of a series? y/n");
                    seriesCheck = Console.ReadLine();
                    if (seriesCheck =="y" || seriesCheck == "yes") {
                        Console.WriteLine("At the moment a book can only be part of one series. What series is the book part of?");
                        series=Console.ReadLine();

                        Console.WriteLine("What number is this book in the series? You may use a decimal if appropriate");
                        seriesNo=Console.ReadLine();                        

                        Console.WriteLine($" Would you like to add the folowing book ? y/n \n Title {bookTitle},  Narration Style {narrationStyle}, Series {series}, Number in Series {seriesNo}");
                        confirmationStatus = Console.ReadLine();
                        if (confirmationStatus == "y" || confirmationStatus == "yes")
                        {
                            Console.WriteLine("Adding Entry");
                            dataLayer.InsertBookData(title: bookTitle,  narrationStyle: narrationStyle, series: series, numInSeries: seriesNo);
                        }
                        else
                        {
                            Nope();
                            return;
                        }
                    }
                    else if ( seriesCheck=="n" || seriesCheck == "no") {
                        Console.WriteLine($" Would you like to add the folowing book ? y/n \n Title {bookTitle}, Narration Style {narrationStyle}");
                        confirmationStatus = Console.ReadLine();
                        if (confirmationStatus == "y" || confirmationStatus == "yes")
                        {
                            Console.WriteLine("Adding Entry");
                            dataLayer.InsertBookData(title: bookTitle, narrationStyle: narrationStyle);
                            return;
                        }
                        else
                        {
                            Nope();
                            return;
                        }

                    }
                    else
                    {
                        Console.WriteLine(" That option is not valid. Please restart and try again.");
                        Nope();
                        return;
                    }

                    break;
                case "3":
                case "author":
                    Console.WriteLine("In order to add a author you must have a first middle or last name. To add an unknown author enter \"unknown\" as the first name and leave remaning fields blank.\n\n");
                    Console.WriteLine("What is the author's first name?");
                    firstName=Console.ReadLine();

                    Console.WriteLine("What is the author's middle name?");
                    middleName=Console.ReadLine();

                    Console.WriteLine("What is the author's last name?");
                    lastName=Console.ReadLine();

                    if(firstName=="" && middleName=="" && lastName == "")
                    {
                        Console.WriteLine(" You have not filled any fields. You must include the author's first, middle, or last name to add an author. Please restart and try again.");
                        Nope();
                        return;
                    }

                    Console.WriteLine($" Would you like to add the folowing author? y/n \n {firstName} {middleName} {lastName}");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Adding Entry");
                        dataLayer.InsertAuthorData(first: firstName, middle: middleName, last: lastName);
                    }
                    else
                    {
                        Nope();
                        return;
                    }
                    break;
                case "4":
                case "narrator":
                    Console.WriteLine("In order to add a narrator you must have a first middle or last name. To add an unknown narrator enter \"unknown\" as the first name and leave remaning fields blank\n\n");
                    Console.WriteLine("What is the narrator's first name?");
                    firstName = Console.ReadLine();

                    Console.WriteLine("What is the narrator's middle name?");
                    middleName = Console.ReadLine();

                    Console.WriteLine("What is the narrator's last name?");
                    lastName = Console.ReadLine();
                    
                    if (firstName == "" && middleName == "" && lastName == "")
                    {
                        Console.WriteLine(" You have not filled any fields. You must include the narrator's first, middle, or last name to add an author. Please restart and try again.");
                        Nope();
                        return;
                    }

                    Console.WriteLine($" Would you like to add the folowing narrator? y/n \n {firstName} {middleName} {lastName}");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Adding Entry");
                        dataLayer.InsertNarratorData(first: firstName, middle: middleName, last: lastName);
                    }
                    else
                    {
                        Nope();
                        return;
                    }
                    break;
                default:
                    Console.WriteLine("That is not a valid table.");
                    break;
            }
            return;
            
        }
        private static void UpdateRecordsHelper(DataLayer dataLayer, string table)
        {
            string editColumn;
            string editId1;
            string editId2;
            string newVal;
            string confirmationStatus;


            if (table != "5" && table != "6" && table != "connect author to book" && table != "connect narrator to book")
            {
                dataLayer.GetAllRecordsAndID(table);
            }
            Console.WriteLine("Please select a column to update:");
            
                switch (table.ToLower())
            {
                case "1":
                case "log":
                    Console.WriteLine("(1) Start_Date, (2) End_Date, (3) Book_ID, (4) Rating");
                    editColumn = Console.ReadLine();
                    
                    switch (editColumn.ToLower())
                    {
                        case "1":
                            editColumn = "start_date";
                            break;
                        case "2":
                            editColumn = "end_date";
                            break;
                        case "3":
                            editColumn = "book_id";
                            break;
                        case "4":
                            editColumn = "rating";
                            break;
    
                        default: 
                            if (!(editColumn == "start_date" || editColumn =="end_date" || editColumn=="book_id"|| editColumn == "rating"))
                            {
                                Console.WriteLine("The selected column is not valid. Please try again.");
                                Nope();
                                return;
                            }
                            break;
                    }

                    Console.WriteLine("Please enter the target start date. (yyyy-mm-dd)");
                    editId1= Console.ReadLine();

                    Console.WriteLine("Please enter the target Book ID");
                    editId2 = Console.ReadLine();

                    Console.WriteLine($"Please enter the new {editColumn} value for the log entry of {editId1},  {editId2}.");
                    newVal = Console.ReadLine();

                    //confirm
                    Console.WriteLine($" Would you like to update the folowing log ? y/n \n {editColumn}: {editId1} {editId2} with {newVal}");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Updating Entry");
                        //update the log
                        dataLayer.UpdateLogData(editColumn: editColumn, updateStart: editId1, updateID: editId2, newVal: newVal);
                    }
                    else
                    {
                        Nope();
                        return;
                    }

                    
                    break;

                case "2":
                case "book":
                    Console.WriteLine("(1) Title, (2) Narration_Style, (3) Series_Name, (4) Num_in_Series");
                    editColumn = Console.ReadLine();

                    switch (editColumn.ToLower())
                    {
                        case "1":
                            editColumn = "title";
                            break;
                        case "2":
                            editColumn = "narration_style";  
                            break;
                        case "3":
                            editColumn = "series_name";
                            break;
                        case "4":
                            editColumn = "num_in_series";
                            break;

                        default:
                            if (!(editColumn == "title" ||  editColumn == "narration_style" || editColumn == "series_name" || editColumn == "num_in_series"))
                            {
                                Console.WriteLine("The selected column is not valid. Please try again.");
                                Nope();
                                return;
                            }
                            break;
                    }

                    Console.WriteLine("Please enter the target book_ID.");
                    editId1 = Console.ReadLine();

                    Console.WriteLine($"Please enter the new {editColumn} value for Book# {editId1}.");
                    newVal = Console.ReadLine();

                    //confirm
                    Console.WriteLine($" Would you like to update the folowing book ? y/n \n {editColumn}: {editId1}  with {newVal}");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Updating Entry");
                        //update the log
                        dataLayer.UpdateBookData(editColumn: editColumn, updateID: editId1, newVal: newVal);
                    }
                    else
                    {
                        Nope();
                        return;
                    }


                    break;
                case "3":
                case "author":
                    Console.WriteLine("(1) First, (2) Middle, (3) Last");
                    editColumn = Console.ReadLine();

                    switch (editColumn.ToLower())
                    {
                        case "1":
                            editColumn = "first";
                                break;
                        case "2":
                            editColumn = "middle";
                            break;
                        case "3":
                            editColumn = "last";
                            break;
                        default:
                            if (!(editColumn == "first" || editColumn == "middle" || editColumn == "last"))
                            {
                                Console.WriteLine("The selected column is not valid. Please try again.");
                                Nope();
                                return;
                            }
                            break;
                    }

                    Console.WriteLine("Please enter the target Author_ID.");
                    editId1 = Console.ReadLine();

                    Console.WriteLine($"Please enter the new {editColumn} name for Author# {editId1}.");
                    newVal = Console.ReadLine();

                    //confirm
                    Console.WriteLine($" Would you like to update the folowing author ? y/n \n {editColumn}: {editId1}  with {newVal}");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Updating Entry");
                        //update the log
                        dataLayer.UpdateAuthorData(editColumn: editColumn, updateID: editId1, newVal: newVal);
                    }
                    else
                    {
                        Nope();
                        return;
                    }

                    break;


                case "4":
                case "narrator":
                    Console.WriteLine("(1) First, (2) Middle, (3) Last");
                    editColumn = Console.ReadLine();

                    switch (editColumn.ToLower())
                    {
                        case "1":
                            editColumn = "first";
                            break;
                        case "2":
                            editColumn = "middle";
                            break;
                        case "3":
                            editColumn = "last";
                            break;
                        default:
                            if (!(editColumn == "first" || editColumn == "middle" || editColumn == "last"))
                            {
                                Console.WriteLine("The selected column is not valid. Please try again.");
                                Nope();
                                return;
                            }
                            break;
                    }

                    Console.WriteLine("Please enter the target Narrator_ID.");
                    editId1 = Console.ReadLine();

                    Console.WriteLine($"Please enter the new {editColumn} name for Narrator# {editId1}.");
                    newVal = Console.ReadLine();

                    //confirm
                    Console.WriteLine($" Would you like to update the folowing author ? y/n \n {editColumn}: {editId1}  with {newVal}");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Updating Entry");
                        //update the log
                        dataLayer.UpdateNarratorData(editColumn: editColumn, updateID: editId1, newVal: newVal);
                    }
                    else
                    {
                        Nope();
                        return;
                    }

                    break;

                case "5":
                case "connect author to book":
                    dataLayer.GetAllRecords("book");
                    Console.WriteLine("Please enter the target book_ID.");
                    editId1 = Console.ReadLine();

                    dataLayer.GetAllRecords("author");
                    Console.WriteLine("Please enter the target Author_ID.");
                    editId2 = Console.ReadLine();

                    dataLayer.UpdateAuthorBookConnect(updateBookID: editId1, updateAuthorID: editId2);
                    break;
                case "6":
                case "connect narrator to book":
                    dataLayer.GetAllRecords("book");
                    Console.WriteLine("Please enter the target book_ID.");
                    editId1 = Console.ReadLine();

                    dataLayer.GetAllRecords("narrator");
                    Console.WriteLine("Please enter the target Narrator_ID.");
                    editId2 = Console.ReadLine();
                    dataLayer.UpdateNarratorBookConnect(updateBookID: editId1, updateNarratorID: editId2);
                    break;
                default:
                    Console.WriteLine("That is not a valid table.");
                    break;
            }
        }
        private static void DeleteRecordsHelper(DataLayer dataLayer, string table)
        {
           
            string editId1;
            string editId2;
           
            string confirmationStatus;


            if (table != "5" && table != "6" && table != "delete author from book" && table != "delete narrator from book")
            {
                dataLayer.GetAllRecordsAndID(table);
            }
            Console.WriteLine("Please select record to delete:");

            switch (table.ToLower())
            {
                case "1":
                case "log":
                  
                    Console.WriteLine("Please enter the target start date. (yyyy-mm-dd)");
                    editId1 = Console.ReadLine();

                    Console.WriteLine("Please enter the target Book ID");
                    editId2 = Console.ReadLine();

                   
                    //confirm
                    Console.WriteLine($" Would you like to update the folowing log ? y/n \n {editId1} {editId2}");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Updating Entry");
                        //update the log
                        dataLayer.DeleteLogData(updateStart: editId1, updateID: editId2);
                    }
                    else
                    {
                        Nope();
                        return;
                    }


                    break;

                case "2":
                case "book":
                   
                    Console.WriteLine("Please enter the target book_ID.");
                    editId1 = Console.ReadLine();


                    //confirm
                    Console.WriteLine($" Would you like to update the folowing book ? y/n \n {editId1} ");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Updating Entry");
                        //update the log
                        dataLayer.DeleteBookData(updateID: editId1);
                    }
                    else
                    {
                        Nope();
                        return;
                    }


                    break;
                case "3":
                case "author":
                    
                    Console.WriteLine("Please enter the target Author_ID.");
                    editId1 = Console.ReadLine();

                    //confirm
                    Console.WriteLine($" Would you like to update the folowing author ? y/n \n{editId1} ");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Updating Entry");
                        //update the log
                        dataLayer.DeleteAuthorData(updateID: editId1);
                    }
                    else
                    {
                        Nope();
                        return;
                    }

                    break;


                case "4":
                case "narrator":
                   
                    Console.WriteLine("Please enter the target Narrator_ID.");
                    editId1 = Console.ReadLine();

                    
                    //confirm
                    Console.WriteLine($" Would you like to update the folowing author ? y/n \n {editId1}  ");
                    confirmationStatus = Console.ReadLine();
                    if (confirmationStatus == "y" || confirmationStatus == "yes")
                    {
                        Console.WriteLine("Updating Entry");
                        //update the log
                        dataLayer.DeleteNarratorData( updateID: editId1);
                    }
                    else
                    {
                        Nope();
                        return;
                    }

                    break;
                case "5":
                case "delete author from book":
                    dataLayer.GetAllRecords("book");
                    Console.WriteLine("Please enter the target book_ID.");
                    editId1 = Console.ReadLine();

                    dataLayer.GetAllRecords("author");
                    Console.WriteLine("Please enter the target Author_ID.");
                    editId2 = Console.ReadLine();

                    dataLayer.DeleteAuthorBookConnect(updateBookID: editId1, updateAuthorID: editId2);
                    break;
                    
                case "6":
                case "delete narrator from book":
                    dataLayer.GetAllRecords("book");
                    Console.WriteLine("Please enter the target book_ID.");
                    editId1 = Console.ReadLine();

                    dataLayer.GetAllRecords("narrator");
                    Console.WriteLine("Please enter the target narrator_ID.");
                    editId2 = Console.ReadLine();

                    dataLayer.DeleteNarratorBookConnect(updateBookID: editId1, updateNarratorID: editId2);
                    break;

                default:
                    Console.WriteLine("That is not a valid table.");
                    break;
            }
        }
    }

}
/*switch(table.ToLower()){
                case "1":
                case "log":
                case "2":
                case "book":
                case "3":
                case "author":
                case "4":
                case "narrator":
                default:
                    break;
            }
 */