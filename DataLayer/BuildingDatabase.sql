/*
* These queries were used to create the database and create, verify and load the data into tables.
* For each table, I created the table, ran describe to verify the table had been created as expected, 
* and then loaded the data and verified it using a SELECT * query before moving on to the next.
* Because these queries were designed for use on MySQL this file can not be run as is to generate a database for MSSQL.
*/

/*
* I had the data in an excel sheet for a book I'm knitting. It's currently in a monster table and I'd 
* like to eventually put it into a database and build an app to run CRUD operations on the database 
* and automate the calculations for the project. In designing the database structure I wanted to make 
* the data more robust than what I currently have in my spreadsheet, so I added a narrator table. For 
* filling the table I chose a portion of the spreadsheet data that would include some books with 
* multiple authors as well as some with multiple narrators. I built the database using MySQL and the 
* code in MySQLDatabaseCreation.txt and the input can be found in the  .csv files provided.
*/


--Creating Database
  SHOW DATABASES;
  CREATE DATABASE BookLog;
  USE BookLog;
  SHOW TABLES;


--Authors Table
 CREATE TABLE Author (
	Author_Id INT NOT NULL AUTO_INCREMENT, 
	First VARCHAR(50), 
    Middle VARCHAR(100), 
	 Last VARCHAR(100),
	PRIMARY KEY(Author_Id)
	) AUTO_INCREMENT = 1000;

	--verify table creation
	DESCRIBE Author;

	--populate table (change path to target file path for the .csv files on your machine)
	LOAD DATA LOCAL INFILE 'C:/Users/kmoha/OneDrive/Desktop/BookBlanket/MinorProject1/CSCE547Data_Author.csv' INTO TABLE author FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r\n' IGNORE 1 ROWS;

	--verify data loaded correctly
	SELECT * FROM Author;



--Narrators Table
  CREATE TABLE Narrator (
	Narrator_Id INT NOT NULL AUTO_INCREMENT, 
	First VARCHAR(50), 
    Middle VARCHAR(100), 
	Last VARCHAR(100),
	PRIMARY KEY(Narrator_Id)
	) AUTO_INCREMENT = 2000;
	
	--verify table creation
	DESCRIBE Narrator;
	
	--populate table (change path to target file path for the .csv files on your machine)
	LOAD DATA LOCAL INFILE 'C:/Users/kmoha/OneDrive/Desktop/BookBlanket/MinorProject1/CSCE547Data_Narrator.csv' INTO TABLE narrator FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r\n' IGNORE 1 ROWS;
	
	--verify data loaded correctly
	SELECT * FROM Narrator;

--Books Table
  CREATE TABLE Book(
	Book_Id INT NOT NULL AUTO_INCREMENT,
	Title VARCHAR(250),
	Length TIME,
	Narration_Style ENUM('single', 'duet', 'multiple', 'full cast', 'unknown'),
	Series_Name VARCHAR(100) ,
	Num_In_Series FLOAT,
	PRIMARY KEY(Book_Id)
	) AUTO_INCREMENT = 3000;
	
	--verify table creation
	DESCRIBE Book;

	--populate table (change path to target file path for the .csv files on your machine)
	LOAD DATA LOCAL INFILE 'C:/Users/kmoha/OneDrive/Desktop/BookBlanket/MinorProject1/CSCE547Data_Book.csv' INTO TABLE book FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r\n' IGNORE 1 ROWS;

	--verify data loaded correctly
	SELECT * FROM Book;

--Hidden Junction Table for Book and Author
  CREATE TABLE Book_Author_Junct(
	Book_id INT NOT NULL,
	Author_id INT NOT NULL,
	PRIMARY KEY (Book_Id, Author_Id),
	FOREIGN KEY(Book_Id) REFERENCES Book(Book_Id)ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY(Author_Id) REFERENCES Author(Author_Id) ON UPDATE CASCADE ON DELETE CASCADE
	);
	
	--verify table creation
	DESCRIBE Book_Author_Junct;

	--populate table (change path to target file path for the .csv files on your machine)
	LOAD DATA LOCAL INFILE 'C:/Users/kmoha/OneDrive/Desktop/BookBlanket/MinorProject1/CSCE547Data_Book_Author_Junct.csv' INTO TABLE book_author_junct FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r\n' IGNORE 1 ROWS;

	--verify data loaded correctly
	SELECT * FROM Book_Author_Junct;

--Hidden Junction Table for Book and Narrator
  CREATE TABLE Book_Narrator_Junct(
	Book_id INT,
	Narrator_id INT,
	PRIMARY KEY (Book_Id, Narrator_Id),
	FOREIGN KEY(Book_Id) REFERENCES Book(Book_Id) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY(Narrator_Id) REFERENCES Narrator(Narrator_Id) ON UPDATE CASCADE ON DELETE CASCADE
	);

	--verify table creation
	DESCRIBE Book_Narrator_Junct;

	--populate table (change path to target file path for the .csv files on your machine)
	LOAD DATA LOCAL INFILE 'C:/Users/kmoha/OneDrive/Desktop/BookBlanket/MinorProject1/CSCE547Data_Book_Narrator_Junct.csv' INTO TABLE book_narrator_junct FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r\n' IGNORE 1 ROWS;

	--verify data loaded correctly
	SELECT * FROM Book_Narrator_Junct;

--Log Table
  CREATE TABLE Log(
	Start_Date DATE DEFAULT '1000-01-01',
	End_Date DATE,
	Book_Id INT NOT NULL,
	Rating INT,
	PRIMARY KEY(Start_Date, Book_Id),
	FOREIGN KEY(Book_Id) REFERENCES Book(Book_Id)  ON UPDATE CASCADE ON DELETE RESTRICT
	);
	
	--verify table creation  
	DESCRIBE Log;

	--populate table (change path to target file path for the .csv files on your machine)
	LOAD DATA LOCAL INFILE 'C:/Users/kmoha/OneDrive/Desktop/BookBlanket/MinorProject1/CSCE547Data_log.csv' INTO TABLE log FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r\n' IGNORE 1 ROWS;

	--verify data loaded correctly
