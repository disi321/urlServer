using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace server
{
    public class Database
    {
        SqlConnection con;
        SqlCommand cmd;
        public Database()
        {
            con = new SqlConnection(String.Format("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30", Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DataBase1.mdf")));
            con.Open();

            cmd = new SqlCommand();
            cmd.Connection = con;
        }

        public string add(string url)
        {
            cmd.CommandText = String.Format("INSERT INTO urls(url) VALUES('{0}')", url);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT COUNT(*) FROM urls"; //get the new id
            Int32 count = (Int32) cmd.ExecuteScalar();
            return "www.leap4-test.com/" + count.ToString(); // string, the new url
        }
        public string getOne(string id)
        {
            string sql = String.Format("SELECT * FROM urls WHERE id LIKE {0}", id);
            using var cmd2 = new SqlCommand(sql, con);

            var rdr = cmd2.ExecuteReader();
            if (!rdr.HasRows)
            {
                rdr.Close();
                return "no url with this id";
            }
            rdr.Read();
            string s = rdr.GetString(1);
            rdr.Close();
            return s; // the (old) url that have this id
        }
        public string getAll()
        {
            List<KeyValuePair<int, string>> l = new List<KeyValuePair<int, string>>();
            string sql = "SELECT * FROM urls";
            using var cmd2 = new SqlCommand(sql, con);

            using SqlDataReader rdr = cmd2.ExecuteReader();

            string s = "";
            while (rdr.Read())
            {
                l.Add( new KeyValuePair<int, string>(rdr.GetInt32(0), rdr.GetString(1).ToString()));
                s += string.Format("{0} {1}\n", rdr.GetInt32(0), rdr.GetString(1));
            }
            rdr.Close();
            return s;
        }

        //cmd.CommandText = "DROP TABLE IF EXISTS urls";
        //cmd.ExecuteNonQuery();

        //cmd.CommandText = @"CREATE TABLE urls(
        //    id int identity(1,1) NOT NULL PRIMARY KEY,
        //    url VARCHAR(255) NOT NULL
        //)";
        //cmd.ExecuteNonQuery();
    }
}
