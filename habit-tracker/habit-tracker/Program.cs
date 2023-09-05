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
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    //case "3":
                    //    Delete();
                    //    break;
                    //case "4":
                    //    Update();
                    //    break;
                    default:
                        Console.WriteLine("\nInvalid command. Please try again.\n");
                        break;
                }
            }
        }

        private static void Insert()
        {
            string date = GetDateInput();
            int quantity = GetNumberInput();

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
                    Console.WriteLine("Table is empty");
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

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: mm-dd-yy). Type 0 to return to the main menu\n\n");
            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            return dateInput;
        }

        internal static int GetNumberInput()
        {
            Console.WriteLine("\n\nPlease insert the number of glasses or another measure of your choice (no decimals allowed). Type 0 to return to the main menu\n\n");
            string numInput = Console.ReadLine();

            if (numInput == "0") GetUserInput();

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