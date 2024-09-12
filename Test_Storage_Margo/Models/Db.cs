using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace Test_Storage_Margo.Models
{
    public class Db
    {
        private static Db instance;
        private MySqlConnection connection;
        string connectionStr;

        private Db()
        {
            string con = "Database=projectdb;Data Source=localhost;User Id=root;Password=jgklsdfjkl";
            this.connectionStr = con;
            MySqlConnection myConnection = new MySqlConnection(con);
            connection = myConnection;

            MySqlCommand myCommand1 = new MySqlCommand("CREATE TABLE IF NOT EXISTS Pallets (Id INT PRIMARY KEY NOT NULL AUTO_INCREMENT, Width INT NOT NULL, Height INT NOT NULL, Depth INT NOT NULL, Weight INT NOT NULL);", connection);
            MySqlCommand myCommand2 = new MySqlCommand("CREATE TABLE IF NOT EXISTS Boxes (Id INT PRIMARY KEY NOT NULL AUTO_INCREMENT, Width INT NOT NULL, Height INT NOT NULL, Depth INT NOT NULL, Weight INT NOT NULL, Production_Date DATE, Expiration_Date DATE NOT NULL, Id_Pallet INT, FOREIGN KEY (Id_Pallet) REFERENCES Pallets(Id));", connection);
            connection.Open();
            myCommand1.ExecuteNonQuery();
            myCommand2.ExecuteNonQuery();
            connection.Close();

            DataSet ds = new DataSet();
            string CommandText = "SELECT * FROM Pallets LIMIT 5;";
            MySqlDataAdapter adapter = new MySqlDataAdapter(CommandText, this.connection);
            this.connection.Open();
            adapter.Fill(ds);
            this.connection.Close();
            RandomObj random;
            if (ds.Tables[0].Rows.Count == 0)
            {
                random = new RandomObj(7, "Pallet");
                string command = "";
                foreach (Pallet pallet in random.randPallets)
                {
                    string ins = $"INSERT INTO Pallets (Width, Height, Depth, Weight) Values ({pallet.Width.ToString()}, {pallet.Height.ToString()}, {pallet.Depth.ToString()}, {pallet.Weight.ToString()}); ";
                    command += ins;
                }
                MySqlCommand myCommand3 = new MySqlCommand(command, connection);
                connection.Open();
                myCommand3.ExecuteNonQuery();
                connection.Close();
            }
            DataSet dsb = new DataSet();
            CommandText = "SELECT * FROM Boxes LIMIT 10;";
            adapter = new MySqlDataAdapter(CommandText, this.connection);
            this.connection.Open();
            adapter.Fill(dsb);
            this.connection.Close();
            
            if (dsb.Tables[0].Rows.Count == 0)
            {
                CommandText = "SELECT Id FROM Pallets;";
                adapter = new MySqlDataAdapter(CommandText, this.connection);
                this.connection.Open();
                adapter.Fill(ds);
                this.connection.Close();
                DataTable dt = ds.Tables[0];
                Random rnd = new Random();

                random = new RandomObj(30, "Box");
                string command = "";
                foreach (Box box in random.randBoxes)
                {
                    string ins = $"INSERT INTO Boxes (Width, Height, Depth, Weight, Production_Date, Expiration_Date, Id_Pallet) Values ({box.Width.ToString()}, {box.Height.ToString()}, {box.Depth.ToString()}, {box.Weight.ToString()}, NUll, \"{box.ExpirationDate.ToString("yyyy-MM-dd")}\", {dt.Rows[rnd.Next(0, dt.Rows.Count - 1)]["Id"].ToString()}); ";
                    command += ins;
                }
                MySqlCommand myCommand4 = new MySqlCommand(command, connection);
                connection.Open();
                myCommand4.ExecuteNonQuery();
                connection.Close();
            }
        }

        public static Db getInstance()
        {
            if (instance == null)
                instance = new Db();
            return instance;
        }

        public int AddRow(string table, Dictionary<string, string> parametrs)
        {
            string columns = "";
            string value = "";
            switch (table)
            {
                case "Boxes":
                    columns = "Width, Height, Depth, Weight, Production_Date, Expiration_Date, Id_Pallet";
                    value = "@Width, @Height, @Depth, @Weight, @Production_Date, @Expiration_Date, @Id_Pallet";
                    break;
                case "Pallets":
                    columns = "Width, Height, Depth, Weight";
                    value = "@Width, @Height, @Depth, @Weight";
                    break;
            }

            string CommandText = $"INSERT INTO {table} ({columns}) VALUES ({value}) RETURNING ID;";
            MySqlCommand myCommand = new MySqlCommand(CommandText, this.connection);
            if (parametrs != null)
            {
                foreach (var item in parametrs)
                {
                    myCommand.Parameters.AddWithValue(item.Key, item.Value);
                }
            }

            this.connection.Open();
            int id = (Int32)myCommand.ExecuteScalar();
            this.connection.Close();
            return id;

        }
        public DataTable TakePalletsOrderByExp()
        {
            DataSet ds = new DataSet();
            string CommandText = @"SELECT z.Exp_Date_Pallet, CAST(JSON_ARRAYAGG(JSON_OBJECT('Id', z.Id, 'Weight_Pallet_With_Boxes', z.Weight_Pallet_With_Boxes) ORDER BY z.Weight_Pallet_With_Boxes aSC) AS CHAR) AS Pallets FROM (SELECT Pallets.Id, (SELECT Expiration_Date FROM Boxes WHERE Id_Pallet = Pallets.Id ORDER BY Expiration_Date aSC limit 1) as Exp_Date_Pallet, (Pallets.Weight + (SELECT SUM(Boxes.Weight) FROM Boxes WHERE Id_Pallet = Pallets.Id)) as Weight_Pallet_With_Boxes from Pallets) z GROUP BY z.Exp_Date_Pallet ORDER BY z.Exp_Date_Pallet aSC";
            MySqlDataAdapter adapter = new MySqlDataAdapter(CommandText, this.connection);
            this.connection.Open();
            adapter.Fill(ds);
            this.connection.Close();
            if (ds == null)
            {
                return null;
            }
            else
            {
                DataTable dt = ds.Tables[0];
                return dt;
            }
        }
        public DataTable Take3Pallets ()
        {
            DataSet ds = new DataSet();
            string CommandText = "SELECT Pallets.Id, (SELECT Expiration_Date FROM Boxes WHERE Id_Pallet = Pallets.Id ORDER BY Expiration_Date DEsc LIMIT 1) AS Exp_Date_Pallet, Pallets.Width * Pallets.Height * Pallets.Depth + (SELECT SUM(Boxes.Width * Boxes.Height * Boxes.Depth) FROM Boxes WHERE Id_Pallet = Pallets.Id) AS Volume FROM Pallets ORDER BY Exp_Date_Pallet Desc, Volume aSC LIMIT 3;";
            MySqlDataAdapter adapter = new MySqlDataAdapter(CommandText, this.connection);
            this.connection.Open();
            adapter.Fill(ds);
            this.connection.Close();
            if (ds == null)
            {
                return null;
            }
            else
            {
                DataTable dt = ds.Tables[0];
                return dt;
            }
        }
        public DataTable CheckPallet (int id)
        {
            DataSet ds = new DataSet();
            string CommandText = $"SELECT Width, Depth FROM Pallets WHERE Id = {id};";
            MySqlDataAdapter adapter = new MySqlDataAdapter(CommandText, this.connection);
            this.connection.Open();
            adapter.Fill(ds);
            this.connection.Close();
            if (ds == null)
            {
                return null;
            }
            else
            {
                DataTable dt = ds.Tables[0];
                return dt;
            }
        }
        public DataTable TakeBoxes (int id)
        {
            DataSet ds = new DataSet();
            string CommandText = $"SELECT * Boxes WHERE Id_Pallet = {id};";
            MySqlDataAdapter adapter = new MySqlDataAdapter(CommandText, this.connection);
            this.connection.Open();
            adapter.Fill(ds);
            this.connection.Close();
            if (ds == null)
            {
                return null;
            }
            else
            {
                DataTable dt = ds.Tables[0];
                return dt;
            }
        }

    }
}
