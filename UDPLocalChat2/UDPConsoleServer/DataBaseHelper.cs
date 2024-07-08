using System;
using System.Data;
using System.Windows;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;

class DataBaseHelper
{
    private static string TableName = "users";
    private static string databaseName = "usersDB";
    private static string connectionString = "server=localhost;port=3306;database=usersDB;user=root;password=12344321;";
    public static bool DataBaseExists()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string databaseName = connection.Database;

                string sql = $"SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";
                MySqlCommand command = new MySqlCommand(sql, connection);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public static bool TableExists()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string databaseName = connection.Database;

                string sql = $"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = @databaseName AND TABLE_NAME = @tableName";
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@databaseName", databaseName);
                command.Parameters.AddWithValue("@tableName", TableName);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
    public static string GetPort(string name)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT IP FROM users WHERE Login = @name", connection);
            command.Parameters.AddWithValue("@name", name);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader["IP"].ToString();
                }
            }
        }
        return null;
    }


    public static void CreateDatabase()
    {
        try
        {
            if(!DataBaseExists())
            {
                string createDbConnectionString = "server=localhost;port=3306;user=root;password=12344321;";
                using (var connection = new MySqlConnection(createDbConnectionString))
                {
                    connection.Open();
                    string sql = $"CREATE DATABASE IF NOT EXISTS {databaseName}";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating database: {ex.Message}");
        }
    }
    public static void CreateTable()
    {
        try
        {
            if(!TableExists())
            {
                string query = @$"
                    CREATE TABLE IF NOT EXISTS {TableName} (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        Login VARCHAR(255) UNIQUE,
                        Password VARCHAR(255),
                        BirthDate VARCHAR(255),
                        Online INT DEFAULT 0,
                        IP VARCHAR(255)
                    );";
                ExecuteNonQuery(query);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating table: {ex.Message}");
        }
    }
    public static List<CurrentUser> LoadUsers(CurrentUser currentUser)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            string query = $"SELECT * FROM {databaseName}.{TableName} Where Login != '{currentUser.UserName}';";
            using (var adapter = new MySqlDataAdapter(query, connection))
            {
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "Users");

                List<CurrentUser> users = new List<CurrentUser>();
                DataTable dataTable = dataSet.Tables["Users"];

                foreach (DataRow row in dataTable.Rows)
                {
                    CurrentUser user = new CurrentUser
                    {
                        UserName = row["Login"].ToString(),
                        Password = row["Password"].ToString(),
                        BirthDate = row["BirthDate"].ToString(),
                        Online = int.Parse(row["Online"].ToString()),
                        IP = row["IP"].ToString()
                    };
                    users.Add(user);
                }
                return users;
            }
        }
    }
    public static void SignUp(CurrentUser user)
    {
        string sql = @$"INSERT INTO {TableName} (login, Password, BirthDate, Online, IP) 
                    VALUES (@userName, @password, @birthDate, @online, @ip)";
        ExecuteNonQuery(sql,
                        new MySqlParameter("@userName", user.UserName),
                        new MySqlParameter("@password", user.Password),
                        new MySqlParameter("@birthDate", user.BirthDate),
                        new MySqlParameter("@online", user.Online),
                        new MySqlParameter("@ip", user.IP));
    }

    public static void UpdatePort(string name, string newPort)
    {
        string sql = $"UPDATE {databaseName}.{TableName} SET IP = @newPort WHERE Login = @name";
        ExecuteNonQuery(sql,
                        new MySqlParameter("@newPort", newPort),
                        new MySqlParameter("@name", name));
    }

    public static bool UserExists(string login)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string sql = $"SELECT Login FROM {TableName} WHERE Login = @login";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@login", login);

                object result = command.ExecuteScalar();
                return result != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
    public static bool ValidatePasswordLogin(string login, string password)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string sql = $"SELECT Password FROM {TableName} WHERE Login = @login AND Password = @password";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                object result = command.ExecuteScalar();
                return result != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }

    public static DataSet ExecuteQuery(string query)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet;
        }
    }

    public static int GetOnlineUsersCount()
    {
        string query = "SELECT COUNT(*) FROM `Users` WHERE `Online` = 1";

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand(query, connection);
            int onlineUsersCount = Convert.ToInt32(command.ExecuteScalar());

            return onlineUsersCount;
        }
    }

    public static void Exit(string name)
    {
        string query = "UPDATE Users SET Online = 0 WHERE Login = @name";

        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                Console.WriteLine($"Error updating user status: {e.Message}");
            }
        }
    }

    public static void ExecuteNonQuery(string query, params MySqlParameter[] parameters)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    command.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                Console.WriteLine($"Error executing query: {e.Message}");
            }
        }
    }
}

