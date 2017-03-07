using UnityEngine;
using System;
using System.Collections;
using System.Data;
using MySql.Data.MySqlClient;

public class SqlHelper
{
    // Global variables
    public static MySqlConnection dbConnection;
    //Just like MyConn.conn in StoryTools before
    private static string host = "192.168.1.35";
    private static string uname = "albert";
    private static string pwd = "albert";
    //    private static string database = "CaptainSonar";
    private static string database = "test";

    public static DataSet ds;
    public static MySqlDataReader dr;
    public static MySqlCommand cmd;

    static SqlHelper ()
    {
    }


    /*
    public  DataSet GetDataSet (string sqlString)
    {
        //string sql = UnicodeAndANSI.UnicodeAndANSI.UnicodeToUtf8(sqlString);


        DataSet ds = new DataSet ();
        try {
            MySqlDataAdapter da = new MySqlDataAdapter (sqlString, dbConnection);
            da.Fill (ds);

        } catch (Exception ee) {

            throw new Exception ("SQL:" + sqlString + "\n" + ee.Message.ToString ());
        }
        return ds;

    }

    */


    /// <summary>
    /// 执行SQL语句，返回reader
    /// </summary>
    /// <returns>The sql.</returns>
    /// <param name="commStr">Comm string.</param>
    public static MySqlDataReader ExSql (string commStr)
    {
        Debug.Log ("ExSql:" + commStr);
        if (dbConnection == null) {
            SqlHelper.OpenDB ();
        } else {
            if (!dbConnection.State.Equals (ConnectionState.Closed)) {
                dbConnection.Close ();
            }
            SqlHelper.OpenDB ();
        }

        cmd = dbConnection.CreateCommand ();
        cmd.CommandText = commStr;
        dr = cmd.ExecuteReader ();
        return dr;
    }

    private static void OpenDB ()
    {
        Debug.Log ("OpenDB");
        try {
            string connectionString = string.Format ("Server = {0}; Database = {1}; User ID = {2}; Password = {3};", host, database, uname, pwd);
            dbConnection = new MySqlConnection (connectionString);
            dbConnection.Open ();
        } catch (Exception e) {
            throw new Exception ("服务器连接失败，请重新检查是否打开MySql服务。" + e.Message.ToString ());  

        }
        Debug.Log ("dbConnection.State :" + dbConnection.State.ToString ());
    }

    public static void CloseDB ()
    {
        Debug.Log ("CloseDB");
        if (dr != null)
            dr.Close ();
        
        cmd.Dispose ();
        if (!dbConnection.State.Equals (ConnectionState.Closed)) {
            dbConnection.Close ();
        }
        dr = null;
        cmd = null;
        dbConnection = null;
    }


}