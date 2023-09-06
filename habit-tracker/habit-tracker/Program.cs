using System.Data.SQLite;
using System.Globalization;

namespace habit_tracker
{
    class Program
    {
        static string connectionString = @"Data Source=C:\Users\brian\Desktop\c_sharp\Habit-Tracker\habit-tracker\habit-tracker\habit_tracker.db";
        static void Main(string[] args)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                //tableCmd.CommandText = @"DROP TABLE IF EXISTS drinking_water";
                //tableCmd.ExecuteNonQuery();
                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS drinking_water (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Quantity INTEGER
                        )";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }

            GetUserInput();
        }

        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\n======MAIN MENU======");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application");
                Console.WriteLine("\nType 1 to View All Records");
                Console.WriteLine("\nType 2 to Insert Record");
                Console.WriteLine("\nType 3 to Delete Record");
                Console.WriteLine("\nType 4 to Update Record");
                Console.WriteLine("---------------------------\n");

                string commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        Update();
                        break;
                    default:
                        Console.WriteLine("\nInvalid command. Please try again.\n");
                        break;
                }
            }
        }

        private static void Insert()
        {
            string date = GetDateInput();
            int quantity = GetNumberInput("\n\nPlease insert the number of glasses or another measure of your choice (no decimals allowed). Type 0 to return to the main menu\n\n");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                   $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        private static void GetAllRecords()
        {
            Console.Clear();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                   $"SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new();

                SQLiteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                        new DrinkingWater
                        {
                            id = reader.GetInt32(0),
                            date = DateTime.ParseExact(reader.GetString(1), "mm-dd-yy", new CultureInfo("en-US")),
                            quantity = reader.GetInt32(2)
                        }); 
                    }
                } else
                {
                    Console.WriteLine("No data to display");
                }

                connection.Close();

                Console.WriteLine("---------------------------\n");

                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.id} - {dw.date.ToString("mm-dd-yyyy")} - Quantity: {dw.quantity}");
                }

                Console.WriteLine("\n---------------------------\n");
            }
        }

        private static void Delete()
        {
            Console.Clear();
            GetAllRecords();
            var recordId = GetNumberInput("\n\nPlease type the ID of the record you want to delete, or type 0 to return to the main menu.\n\n");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"DELETE from drinking_water WHERE Id = '{recordId}'"; 

                int rowCount = tableCmd.ExecuteNonQuery();
                
                if (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with ID {recordId} doesn't exist.\n\n");
                    Delete();
                }

                Console.WriteLine($"\n\nRecord with ID {recordId} was deleted. \n\n");
                connection.Close();
            }
        }

        internal static void Update()
        {
            Console.Clear();
            GetAllRecords();
            var recordId = GetNumberInput("\n\nPlease type the ID of the record you want to update, or type 0 to return to the main menu.\n\n");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with ID {recordId} doesn't exist.\n\n");
                    connection.Close();
                    Update();
                }

                string date = GetDateInput();

                int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed)\n\n");

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = {quantity} WHERE Id = {recordId}";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: mm-dd-yy). Type 0 to return to the main menu\n\n");
            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main manu or try again:\n\n");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string numInput = Console.ReadLine();

            if (numInput == "0") GetUserInput();

            while (!Int32.TryParse(numInput, out _) || Convert.ToInt32(numInput) < 0)
            {
                Console.WriteLine("\n\nInvalid number. Try again.\n\n");
                numInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(numInput);

            return finalInput;
        }
    }
}

public class DrinkingWater
{
    public int id { get; set; }
    public DateTime date { get; set; }
    public int quantity { get; set; }
}