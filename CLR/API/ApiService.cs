using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void Get_ApiServiceData (string URL)
    {

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
        request.Method = "GET";
        //request.ContentLength = 0;
        //request.Credentials = CredentialCache.DefaultCredentials;
        request.ContentType = "application/json";
        request.Accept = "application/json";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream receiveStream = response.GetResponseStream();
        StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
        string strContent = readStream.ReadToEnd();
        DataSet data = JsonConvert.DeserializeObject<DataSet>(strContent);
        if (data.Tables.Count > 0)
        {
            DataTable dt = data.Tables[0];
            SqlPipe pipe = SqlContext.Pipe;

            SqlMetaData[] cols = new SqlMetaData[dt.Columns.Count];

            for (int col = 0; col < dt.Columns.Count; col++)
            {
                cols[col] = new SqlMetaData(dt.Columns[col].ColumnName, SqlDbType.NVarChar,4000);
            }
            SqlDataRecord record = new SqlDataRecord(cols);
            pipe.SendResultsStart(record);

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    record.SetSqlString(col, new SqlString(Convert.ToString(dt.Rows[row][dt.Columns[col].ColumnName])));
                   
                }
               
                pipe.SendResultsRow(record);
            }
           
          

            pipe.SendResultsEnd();

        }
       
    }



}

//ALTER DATABASE CLR SET TRUSTWORTHY ON
//go
//sp_configure 'clr strict security', 1;
//GO
//RECONFIGURE;
//GO
//sp_configure 'show advanced options', 1;
//GO
//RECONFIGURE;
//GO
//sp_configure 'clr enabled', 1;
//GO
//RECONFIGURE;
//GO

//CREATE ASSEMBLY [System.Runtime.Serialization]
//AUTHORIZATION dbo
//FROM  N'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Runtime.Serialization.dll'
//WITH PERMISSION_SET = UNSAFE--external_access

//CREATE ASSEMBLY [Newtonsoft.Json]
//AUTHORIZATION dbo
//FROM  N'D:\Development\CLR\CLR\dll/Newtonsoft.Json.dll'
//WITH PERMISSION_SET = UNSAFE

//create ASSEMBLY [ApiService]
//AUTHORIZATION dbo
//FROM  N'D:\Development\CLR\CLR\bin\Debug\CLR.dll'
//WITH PERMISSION_SET = UNSAFE--external_access


//CREATE PROCEDURE [dbo].[Get_ApiServiceData]
//@URL NVARCHAR(MAX) NULL
//AS EXTERNAL NAME [ApiService].[StoredProcedures].[Get_ApiServiceData]?
