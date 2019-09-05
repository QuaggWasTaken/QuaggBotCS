using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuaggBotCS
{
    public class UserRepo
    {
        public string connectionString;

        public UserRepo(string connString)
        {
            connectionString = connString;
        }

        public List<User> GetUsers()
        {
            MySqlConnection UserConn = new MySqlConnection(connectionString);
            MySqlConnection NameConn = new MySqlConnection(connectionString);
            MySqlConnection ServerConn = new MySqlConnection(connectionString);
            using (UserConn)
            {
                UserConn.Open();
                using (NameConn)
                {
                    NameConn.Open();
                    using (ServerConn)
                    {
                        ServerConn.Open();
                        MySqlCommand UserCmd = UserConn.CreateCommand();
                        UserCmd.CommandText = "SELECT `UserID`, `Discriminator` FROM `users`;";
                        MySqlDataReader UserReader = UserCmd.ExecuteReader();
                        MySqlCommand NameCmd = NameConn.CreateCommand();
                        NameCmd.CommandText = "SELECT `Name`, `ServerID` FROM `names`;";
                        MySqlDataReader NameReader = NameCmd.ExecuteReader();
                        List<User> Users = new List<User>();
                        while (UserReader.Read())
                        {
                            List<Nickname> Nicks = new List<Nickname>();
                            while (NameReader.Read())
                            {
                                MySqlCommand ServerCmd = ServerConn.CreateCommand();
                                ServerCmd.CommandText = $"SELECT `ServerName`, `ServerID` FROM `servers` WHERE `ServerID` LIKE '{NameReader.GetUInt64("ServerID")}';";
                                MySqlDataReader ServerReader = ServerCmd.ExecuteReader();
                                while (ServerReader.Read())
                                {
                                    Nickname nick = new Nickname()
                                    {
                                        Name = NameReader.GetString("Name"),
                                        Guild = new Server
                                        {
                                            ServerID = NameReader.GetUInt64("ServerID"),
                                            ServerName = ServerReader.GetString("ServerName")
                                        }
                                    };
                                    Nicks.Add(nick);
                                }
                                ServerReader.Close();
                            }
                            NameReader.Close();
                            User user = new User()
                            {
                                Snowflake = UserReader.GetUInt64("UserID"),
                                Names = Nicks,
                                Discriminator = UserReader.GetString("Discriminator")
                            };
                            Users.Add(user);
                        }
                        UserReader.Close();
                        UserConn.Close();
                        NameConn.Close();
                        ServerConn.Close();
                        return Users;
                    }
                }
            }
        }

        public async Task UpdateUsers()
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (User user in DataHandler.Users)
                {
                    if (i == 0)
                    {
                        if (user.New)
                        {
                            await NonQuery($"INSERT INTO users(UserID, Discriminator) VALUES ({user.Snowflake}, '{user.Discriminator}');");
                        }
                        else if (user.Update)
                        {
                            await NonQuery($"UPDATE users SET Discriminator = '{user.Discriminator}' WHERE `UserID` = {user.Snowflake};");
                        }
                    }
                    if (i == 1)
                    {
                        foreach (Nickname name in user.Names)
                        {
                            if (name.New)
                            {
                                await NonQuery($"INSERT INTO servers(ServerID, ServerName) VALUES ({name.Guild.ServerID}, '{name.Guild.ServerName}');");
                            }
                            else if (name.Update)
                            {
                                await NonQuery($"UPDATE servers SET ServerName = '{name.Guild.ServerName}' WHERE `ServerID` = {name.Guild.ServerID};");
                            }
                        }
                    }
                    if (i == 2)
                    {
                        foreach (Nickname name in user.Names)
                        {
                            if (name.New)
                            {
                                await NonQuery($"INSERT INTO names(Name, UserID, ServerID) VALUES ('{name.Name}', {user.Snowflake}, {name.Guild.ServerID});");
                            }
                            else if (name.Update)
                            {
                                await NonQuery($"UPDATE names SET Name = '{name.Name}' WHERE `ServerID` = {name.Guild.ServerID} AND `UserID` = {user.Snowflake};");
                            }
                        }
                    }
                }
            }
        }

        private async Task NonQuery(string input)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            using (conn)
            {
                await conn.OpenAsync();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = input;
                await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
        }
    }


    public class User
    {
        public ulong Snowflake { get; set; }
        public List<Nickname> Names { get; set; }
        public string Discriminator { get; set; }
        public bool Update { get; set; } = false;
        public bool New { get; set; } = false;
    }

    public class Nickname
    {
        public Server Guild { get; set; }
        public string Name { get; set; }
        public bool Update { get; set; } = false;
        public bool New { get; set; } = false;
    }

    public class Server
    {
        public ulong ServerID { get; set; }
        public string ServerName { get; set; }
        public bool Update { get; set; } = false;
        public bool New { get; set; } = false;
    }
}
