Class- Program.cs
	//Run methods form other classes

	Methods called
		SensorPOC.writefile
		SensorPOC.close
		IngestorPOC.readfile
		IngestorPOC.close

Class- SensorPOC.cs
	//Record and write data every second to a CSV file
	//When 100 lines is reached on the CSV file, close and open new file
	//Create random numbers for Lat and Lon to be recorded

	Var
		file.name //SensorName_YYYYMMDD_Counter.csv
		file.line.counter
		timer //every second create entry
		max.lines
		output

	Methods
		create.new.file //SensorName_YYYYMMDD_Counter.csv
		create.random.lat.lon //random num given, can be null
		write.to.file //TimeStamp, SensorId, SensorName, Key1, Value1, Key2, Value2
		timer.enacted //create line items every 1 second
		close.file //when line items reach 100, close to create new file

Class- IngestorPOC.cs
	//Read completed CSV files and add line items to cloud data table (log entry for this task)
	//Run test function from TestCSVRead and delete CSV if function if true

	Var
		file.name //IngestorName_YYYYMMDD_Counter.txt
		sensor.file.name //SensorName_YYYYMMDD_Counter.csv
		output
		sensor.file.output
		amt.null.values	
		amt.line.read
		
	Methods
		create.new.file //IngestorName_YYYYMMDD_Counter.txt
		read.file //SensorName_YYYYMMDD_Counter.csv
		count.lines //count amount of lines in file
		count.null.values //count how many lines lat and lon values were null
		write.to.file //create log when sensor file is scanned
			“TimeStamp”: “2025-03-03T15:31:05.000Z”,
			“Source”: “SensorPOC”,
			“FileName” : “SensorPOC_20250303_101.csv”,
			“SuccessEntries”: 100, // entries that have values
			“FailedEntries”: 0, //entries that have empty/null values
			“Errors”: “”
		scan.for.file //scan to find unlocked
		close.file //when program ends, close file


Class- TestCSVFile.cs
	//Run test that verifies all line items were correctly uploaded from CSV to data table
	//Return true is done correctly
	//Return false an error if false

Class- MockAuth
	//Show Auth implemented for user usage of the app
	//Will not be functioning

Class- Could database integration
	//when file from Sensor is read, log and data ran within IngesterPOC is added to a database

Class- Docker
	//Create Docker for SensorPOC and IngestorPOC classes


When I first read this task I knew there needed to be a create.file, wirte.file, and timer in both classes. 
I also new the classes would be somewhat similar to each other as they both preform similar tasks. 
I choose to write the SensorPOC class first as I was not fully sure how I was wanting to implement the scan method in IngestorPOC.
As I finished the SensorPOC I new I had a template I could work off of to create IngestorPOC. That for the most part work well, 
though not everything could match as there was more complex happenings in IngestorPOC.
I at first wanted to have the scan.file to be a method that looked for unlocked documents (as the object was locked when being written to)
if it found an unlocked document it was to read it, log the data, and then delete the CSV file. 
I was guessing I would not have time for the Test class as stated above, so I knew deleting after logging was not viable, as I needed to test before deleting anything
I did work on finding a unlocked document, but I ran into several errors and issues
I decided I would then have the application look for the file that ended in the next file.name counter. 
This however I felt after doing research that that would not be the best path forward.
I chose the scan for new file ending in the correct file.name counter after x amount of time. Like with the SensorPOC timer.
As I know this is not the most efficient way, and there are better solutions, this was what I could present within the time and knowing I had at the time. 
Overall the app does do what is asked. I could absolutly have another look over, more efficient methods, as well as a completion of the other classes mentioned. 

For future improvements, I would add the testing, as stated, to ensure all files are writing, reading, and closing correctly. There is plenty of log.messages
but having test give the likelihood of logging errors minimal. 
The docker containers need to be implemented. 
Authentication for users and admin will need to be added to ensure correct usage and changes are made by the correct people. 
Deletion of csv file after read, tested that reading was correct, and the file is no longer needed. This would save on memory space, and cleaner file management.
