using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;


public class ConnectionManager
{
    public string DatabaseName;
    private string path;
    
    private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    private IDataReader reader;
    
    public bool EnabledLogOfErrors = false;
    public bool GoodInit = true;

    private bool underDevelopment = false;
    
    public List<T> ConvertDataTable<T>(DataTable dt)  
    {  
        List<T> data = new List<T>();  
        foreach (DataRow row in dt.Rows)  
        {  
            T item = GetItem<T>(row);  
            data.Add(item);  
        }  
        return data;  
    }  
    private static T GetItem<T>(DataRow dr)  
    {  
        Type temp = typeof(T);  
        T obj = Activator.CreateInstance<T>();  
  
        foreach (DataColumn column in dr.Table.Columns)  
        {  
            foreach (PropertyInfo pro in temp.GetProperties())  
            {  
                if (pro.Name == column.ColumnName)  
                    pro.SetValue(obj, dr[column.ColumnName], null);  
                else  
                    continue;  
            }  
        }  
        return obj;  
    }

    public string ConvertTable2String(DataTable results)
    {
        return string.Join(Environment.NewLine, 
            results.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
    }

    public string ConvertDate2Sql(DateTime _toConvert)
    {
        StringBuilder date = new StringBuilder();
        date.Append(_toConvert.Year.ToString());
        date.Append("-");
        date.Append(_toConvert.Month.ToString());
        date.Append("-");
        date.Append(_toConvert.Day.ToString());
        return date.ToString();
    }
    
    public string RemoveSpecialCharacters(string str)
    {
        StringBuilder sb = new StringBuilder();
        str = str.Trim();
        //Escribir como una caso especial los carácteres especiales que podrán ser utilizados en la aplicación, es excluye de manera directa la ' para 
        //Evitar la inyeccion de Sql Malicioso en la aplicación, el resto de los caracteres no presentan un problema adicional para la misma, siendo
        //posible sustiturlo sin problemas con comillas "".
        foreach (var t in str.Where(t => (t >= '0' && t <= '9')
                                         || (t >= 'A' && t <= 'z'
                                             || (t == '.' || t == '_' 
                                        ||t==' '||t=='-' ||t=='ñ' ||t=='í'
                                            || t=='+' || t=='-'
                                        ||t=='@' ||t=='ó'))))
        {
            sb.Append(t);
        }
        return sb.ToString();
    }
    public ConnectionManager()
    {
    }

    public void Initialize(string databaseName, bool enabledLogOfErrors)
    {
        EnabledLogOfErrors = enabledLogOfErrors;
        DatabaseName = databaseName;
        var filepath = Application.persistentDataPath +"/"+ DatabaseName;
        if (Application.isEditor)
        {
            filepath = Application.dataPath + "/Plugins/" + DatabaseName;
        }

        try
        {
            conn = "URI=file:" + filepath;
            Debug.Log("Stablishing connection to: " + conn);
            dbconn = new SqliteConnection(conn);
            dbconn.Open();
            dbconn.Close();
            Debug.Log("Good initializaition");
        }
        catch (Exception e)
        {
            GoodInit = false;
            error2Log(e.Message);
        }
    }
    public void Initialize()
    {
        DatabaseName = "manual.s3db";
        path = Application.persistentDataPath +"/"+ DatabaseName;
        
        if (Application.isEditor)
        {
            path = Application.dataPath + "/Plugins/" + DatabaseName;
        }
        try
        {
            conn = "URI=file:" + path;
            Debug.Log("Stablishing connection to: " + conn);
            dbconn = new SqliteConnection(conn);
            dbconn.Open();
            dbconn.Close();
            Debug.Log("Good initializaition");
        }
        catch (Exception e)
        {
            GoodInit = false;
            error2Log(e.Message);
        }
    }

    public void Initialize(string databaseName)
    {
        path = Application.persistentDataPath +"/"+ DatabaseName;
        if (Application.isEditor)
        {
            path = Application.dataPath + "/Plugins/" + DatabaseName;
        }
        DatabaseName = databaseName;
        try
        {
            conn = "URI=file:" + path;
            Debug.Log("Stablishing connection to: " + conn);
            dbconn = new SqliteConnection(conn);
            dbconn.Open();
            dbconn.Close();
            Debug.Log("Ok, good database initialization");
        }
        catch (Exception e)
        {
            GoodInit = false;
            error2Log(e.Message);
        }
        
    }

    private void error2Log(string eMessage)
    {
        var filepath = Application.persistentDataPath;
        var messageError = $"Error on database {DateTime.Now.ToShortDateString()}: {eMessage}";
        var file = new StreamWriter("WriteLines2.txt", append: true);
        file.WriteLine(messageError);
        Debug.Log(messageError);
    }

    private void message2Log(string message2Log)
    {
        var filepath = Application.persistentDataPath;
        var messageError = $"Making an consult on database {DateTime.Now.ToShortDateString()}: {message2Log}";
        var file = new StreamWriter("MovementsInDatabase.txt", append: true);
        file.WriteLine(messageError);
        Debug.Log(messageError);
    }

    public string DataTable2Json(DataTable table2Translate)
    {
        return JsonUtility.ToJson(table2Translate);
    }
    
    public DataTable makeAnSqlConsult(string consultaSQL)
    {
        var dt = new DataTable();
        dbconn.Open();
        if (underDevelopment)
        {
            message2Log(consultaSQL);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            using (var command = dbconn.CreateCommand()) //Android modification to get the data
            {
                command.CommandText = consultaSQL;
                reader = command.ExecuteReader();

                var len = reader.FieldCount;
                //Creacion de las columnas
                for (int i = 0; i < len; i++)
                    dt.Columns.Add (reader.GetName (i), reader.GetFieldType (i));
                //Se inicia el llenado de la tabla de datos
                dt.BeginLoadData();
                var values = new object[len];
                while (reader.Read ()) {
                    for (int i = 0; i < len; i++)
                        values[i] = reader[i];

                    dt.Rows.Add (values);
                }
                dt.EndLoadData ();
                //Se termina con el llenado de datos y se liberan los recursos
                reader.Close();
                reader.Dispose();
            }
        }
        else //Under Windows, Linux or Mac
        {
            using (var command = dbconn.CreateCommand())
            {
                command.CommandText = consultaSQL;
                reader = command.ExecuteReader();
                dt.Load(reader);
            }
        }

        dbconn.Close();
        return dt;
    }
    
    private  DataTable FillDatatable (string consultaSQL)
    {
        var dt = new DataTable();
        dbconn.Open();
        using (var command = dbconn.CreateCommand())
        {
            command.CommandText = consultaSQL;
            reader = command.ExecuteReader();
            //Tabled.Load(lector);
            var len = reader.FieldCount;
            for (int i = 0; i < len; i++)
                dt.Columns.Add (reader.GetName (i), reader.GetFieldType (i));
            
            dt.BeginLoadData();
            var values = new object[len];
            while (reader.Read ()) {
                for (int i = 0; i < len; i++)
                    values[i] = reader[i];

                dt.Rows.Add (values);
            }
            dt.EndLoadData ();
            reader.Close();
            reader.Dispose();
        }
        dbconn.Close();
        return dt;
        
    }

    public bool ExecuteACommand(string SqlCom)
    {
        try
        {
            dbconn.Open();
            using (var command = dbconn.CreateCommand())
            {
                command.CommandText = SqlCom;
                if (underDevelopment)
                {
                    message2Log(SqlCom);
                }
                command.ExecuteNonQuery();
            }
            dbconn.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Error al ejecutar el comando {e.Message},\n {e.StackTrace}");
            return false;
        }
    }
   
    private static async Task ExampleAsync(string MessageError)
    {
        string filepath = Application.persistentDataPath;
        StreamWriter file = new StreamWriter("Errors.txt", append: true);
        await file.WriteLineAsync(MessageError+"\n");
    }
}
