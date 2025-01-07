using MySql.Data.MySqlClient;
using Server.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Services
{
   
    public class DataBaseService
    {
        private static string connectionString 
        = "server=localhost;port=3306;database=social_network_db;user=root;password=12344321;";

        public async static Task setGroupHistory(Message message)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("update social_network_db.group_history " +
                    "SET TextLines = CONCAT(TextLines, @message) " +
                    "Where id_group = (" +
                    " SELECT id FROM social_network_db.`group` " +
                    "WHERE `Name` = @groupName)", connection);
                command.Parameters.AddWithValue("@message", message.Data + "\n");
                command.Parameters.AddWithValue("@groupName", message.ReciverIP);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task change_groupName(Message message)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("UPDATE social_network_db.`group` " +
                    "SET `Name` = @newName " +
                    "WHERE `Name` = @oldName;", connection);
                command.Parameters.AddWithValue("@newName", message.Data);
                command.Parameters.AddWithValue("@oldName", message.ReciverIP);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task deleteFromGroup(Message message)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("DELETE FROM social_network_db.groups_users " +
                    "WHERE id_group = (SELECT id FROM social_network_db.`group` WHERE `Name` = @GroupName) " +
                    "AND id_member = (SELECT id FROM social_network_db.users WHERE Login = @UserName);", connection);
                command.Parameters.AddWithValue("@GroupName", message.Data);
                command.Parameters.AddWithValue("@UserName", message.ReciverIP);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task addToGroup(Message message)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("INSERT INTO social_network_db.groups_users " +
                    "(id_group, id_member) " +
                    "SELECT " +
                    "(SELECT id FROM social_network_db.`group` WHERE `Name` = @GroupName), " +
                    "(SELECT id FROM social_network_db.users WHERE Login = @UserName);", connection);
                command.Parameters.AddWithValue("@GroupName", message.Data);
                command.Parameters.AddWithValue("@UserName", message.ReciverIP);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task createGroup(Message message)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("INSERT INTO social_network_db.`group` " +
                    "(`Name`, id_group_owner) " +
                    "Values ( @name, " +
                    "( Select id FROM social_network_db.users Where ip = @ownerIp ));", connection);
                command.Parameters.AddWithValue("@name", message.Data);
                command.Parameters.AddWithValue("@ownerIp", message.SenderIP);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task<bool> isOwner(Message message)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT `Name` FROM  social_network_db.`group` " +
                        "WHERE `Name` = @name AND id_group_owner = " +
                        "(Select id FROM social_network_db.users Where ip = @ownerIp);";

                    MySqlCommand command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@name", message.Data);
                    command.Parameters.AddWithValue("@ownerIp", message.SenderIP);

                    object result = await command.ExecuteScalarAsync();
                    return result != null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

        public async static Task deleteGroup(Message message)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("DELETE FROM social_network_db.`group` WHERE `Name` = @name;", connection);
                command.Parameters.AddWithValue("@name", message.Data);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task<string> GetIPAsync(string Name)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT ip FROM social_network_db.users WHERE Login = @name;", connection);
                command.Parameters.AddWithValue("@name", Name);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader["IP"].ToString();
                    }
                }
            }
            return "";
        }

        public async static Task<string> GetLoginByIPAsync(string IP)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT Login FROM social_network_db.users WHERE ip = @IP;", connection);
                command.Parameters.AddWithValue("@IP", IP);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader["Login"].ToString();
                    }
                }
            }
            return "";
        }

        public async static Task<List<string>> LoadUsersAsync(User user)
        {
            var users = new List<string>();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "SELECT Login FROM social_network_db.users AS U " +
                    "WHERE U.Login != @name",
                    connection);

                command.Parameters.AddWithValue("@name", user.Login);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(reader["Login"].ToString());
                    }
                }
            }

            return users;
        }

        public async static Task<List<string>> UsersNotInGroup(Message message)
        {
            var users = new List<string>();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "SELECT U.Login FROM social_network_db.users AS U " +
                    "WHERE U.id NOT IN " +
                    "( SELECT GU.id_member " +
                    "FROM social_network_db.groups_users GU " +
                    "JOIN social_network_db.`group` AS G ON G.id = GU.id_group " +
                    "WHERE G.`Name` = @name);",
                    connection);

                command.Parameters.AddWithValue("@name", message.Data);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(reader["Login"].ToString());
                    }
                }
            }

            return users;
        }

        public async static Task<List<string>> UsersInGroup(Message message)
        {
            var users = new List<string>();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "SELECT U.Login FROM social_network_db.users AS U" +
                    " JOIN social_network_db.groups_users GU ON U.id = GU.id_member" +
                    " JOIN social_network_db.`group` AS G ON G.id = GU.id_group" +
                    " WHERE G.`Name` = @name AND U.ip != @ipOwner",
                    connection);

                command.Parameters.AddWithValue("@name", message.Data);
                command.Parameters.AddWithValue("@ipOwner", message.SenderIP);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(reader["Login"].ToString());
                    }
                }
            }

            return users;
        }

        public async static Task<List<string>> getGroupHistory(Message message)
        {
            var history = new List<string>();
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "SELECT TextLines FROM social_network_db.group_history " +
                    "WHERE id_group = " +
                    "(select id FROM social_network_db.`group` WHERE `Name` = @name);",
                    connection);

                command.Parameters.AddWithValue("@name", message.Data);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        history.Add(reader["TextLines"].ToString());
                    }
                }
            }
            return history;
        }


        public async static Task<List<string>> LoadGroupsAsync(User user)
        {
            var groups = new List<string>();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "SELECT DISTINCT G.`Name` FROM social_network_db.`group` AS G " +
                    "JOIN social_network_db.groups_users AS GU ON G.id = GU.id_group " +
                    "WHERE G.id IN ( " +
                        "SELECT GU.id_group FROM social_network_db.groups_users AS GU " +
                        "JOIN social_network_db.users AS U ON GU.id_member = U.id " +
                        "WHERE U.Login = @name);",
                    connection);

                command.Parameters.AddWithValue("@name", user.Login);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        groups.Add(reader["Name"].ToString());
                    }
                }
            }

            return groups;
        }
        public async static Task<bool> IsConnectionExistsAsync(List<string> users)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var checkCommand = new MySqlCommand(
                    "SELECT COUNT(*) FROM social_network_db.chat_history " +
                    "WHERE (id_first = (SELECT id FROM social_network_db.users WHERE Login = @name1) " +
                    "AND id_second = (SELECT id FROM social_network_db.users WHERE Login = @name2)) " +
                    "OR (id_first = (SELECT id FROM social_network_db.users WHERE Login = @name2) " +
                    "AND id_second = (SELECT id FROM social_network_db.users WHERE Login = @name1));",
                    connection
                );

                checkCommand.Parameters.AddWithValue("@name1", users[0]);
                checkCommand.Parameters.AddWithValue("@name2", users[1]);

                var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                return count > 0;
            }
        }


        public async static Task ConectUsers(List<string> users)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("INSERT INTO social_network_db.chat_history  " +
                    "(id_first, id_second, TextLines) values( " +
                    "(SELECT id FROM social_network_db.users WHERE Login = @name), " +
                    "(SELECT id FROM social_network_db.users WHERE Login = @name2), " +
                    "'');", connection);

                command.Parameters.AddWithValue("@name", users[0]);
                command.Parameters.AddWithValue("@name2", users[1]);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task SetUsersHistory(Message message)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("CREATE TEMPORARY TABLE social_network_db.temp_table " +
                    "AS SELECT H.id FROM social_network_db.chat_history AS H " +
                    "JOIN social_network_db.users AS U1 ON H.id_first = U1.id " +
                    "JOIN social_network_db.users AS U2 ON H.id_second = U2.id " +
                    "WHERE (U1.ip = @IP1 AND U2.ip = @IP2) " +
                    "OR (U1.ip = @IP2 AND U2.ip = @IP1); " +
                    "UPDATE social_network_db.chat_history  " +
                    "SET TextLines = CONCAT(TextLines, @message) " +
                    "WHERE id IN (SELECT id FROM social_network_db.temp_table); " +
                    "DROP TEMPORARY TABLE social_network_db.temp_table;", connection);

                command.Parameters.AddWithValue("@IP1", message.ReciverIP);
                command.Parameters.AddWithValue("@IP2", message.SenderIP);
                command.Parameters.AddWithValue("@message", message.Data + "\n");

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task<List<string>> GetUsersHistory(List<string> users)
        {
            List<string> history = new List<string>();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT H.TextLines FROM social_network_db.chat_history AS H" +
                    " WHERE (H.id_first = (Select id from social_network_db.users WHERE Login = @name)" +
                    " AND H.id_second = (Select id from social_network_db.users WHERE Login = @name2))" +
                    " OR (H.id_first = (Select id from social_network_db.users WHERE Login = @name2) " +
                    " AND H.id_second = (Select id from social_network_db.users WHERE Login = @name))", connection);

                command.Parameters.AddWithValue("@name", users[0]);
                command.Parameters.AddWithValue("@name2", users[1]);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string text = reader["TextLines"]?.ToString();

                        if (!string.IsNullOrEmpty(text))
                        {
                            var lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var line in lines)
                            {
                                history.Add(line);
                            }
                        }
                    }
                }
            }
            return history;
        }

        

        public async static Task SignUpAsync(User user)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("INSERT INTO social_network_db.users (login, Password, BirthDate, Online, IP)" +
                    " VALUES(@name, @password, @birthDate, @online, @ip);", connection);
                command.Parameters.AddWithValue("@name", user.Login);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@birthDate", user.BirthDate);
                command.Parameters.AddWithValue("@online", user.Online);
                command.Parameters.AddWithValue("@ip", user.IP);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task UpdateIPAsync(User user, string newIP)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("UPDATE social_network_db.users SET IP = @newIP WHERE Login = @name", connection);

                command.Parameters.AddWithValue("@name", user.Login);
                command.Parameters.AddWithValue("@newIP", newIP);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task<bool> UserExistsAsync(User user)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT Login FROM social_network_db.users WHERE Login = @name AND password = @word;";

                    MySqlCommand command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@name", user.Login);
                    command.Parameters.AddWithValue("@word", user.Password);


                    object result = await command.ExecuteScalarAsync();
                    return result != null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

        public async static Task<int> GetOnlineUsersCountAsync()
        {

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM social_network_db.users WHERE online = 1";
                connection.Open();

                MySqlCommand command = new MySqlCommand(query, connection);
                int onlineUsersCount = Convert.ToInt32(await command.ExecuteScalarAsync());

                return onlineUsersCount;
            }
        }

        public async static Task SetOnlineAsync(User user)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    string query = "UPDATE social_network_db.users SET online = 1 WHERE Login = @name";
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", user.Login);
                    await command.ExecuteNonQueryAsync();
                }
                catch (MySqlException e)
                {
                    Console.WriteLine($"Error updating user status: {e.Message}");
                }
            }
        }

        public async static Task Exit(User user)
        {

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    string query = "UPDATE social_network_db.users SET online = 0 WHERE Login = @name";
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", user.Login);
                    await command.ExecuteNonQueryAsync();
                }
                catch (MySqlException e)
                {
                    Console.WriteLine($"Error updating user status: {e.Message}");
                }
            }
        }

        public async static Task<bool> isOnline(string ip)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT Login FROM social_network_db.users WHERE ip = @ip AND online = 1;";

                    MySqlCommand command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@ip", ip);

                    object result = await command.ExecuteScalarAsync();
                    return result != null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

       




    }
}
