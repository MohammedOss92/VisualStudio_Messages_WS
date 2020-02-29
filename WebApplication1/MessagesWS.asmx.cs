using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for MessagesWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MessagesWS : System.Web.Services.WebService
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }


      


        [WebMethod]
        public void getCountOfMessagesByTitle(string typeID)
        {

            String strJson;
            HttpContext context = this.Context;

            try
            {

                //this is the bridge to database
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["messages"].ConnectionString);
                string strSQL = "SELECT count(*) from messages where typeid=@x  ";
                conn.Open();

                SqlCommand cmd = new SqlCommand(strSQL, conn);
                cmd.Parameters.AddWithValue("@x", typeID);
                string strCount = cmd.ExecuteScalar().ToString();
              
               
                conn.Close();


                // we took object

               

                strJson = "{" + "\"" + "result" + "\":" + strCount + "}";

                Context.Response.Write(strJson);

            }
            catch (Exception ex)
            {


            }


        }



       

        [WebMethod]
        public void readTitle()
        {

            String strJson;
            HttpContext context = this.Context;

            try
            {

                //this is the bridge to database
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["messages"].ConnectionString);
                string strSQL = "SELECT *   FROM MessageTypes  ";
                conn.Open();

                SqlCommand cmd = new SqlCommand(strSQL, conn);
      
               
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();



     

                // we took object
             
                MsgTypes[] u = new MsgTypes[dt.Rows.Count];
                int counter = 0;
                foreach (DataRow row in dt.Rows)
                {


                    u[counter] = new MsgTypes();
                    u[counter].typeID = row.Field<int>("TypeID");
                    u[counter].typeDescription = row.Field<string>("TypeDescription");
                    u[counter].newMsg = row.Field<int>("newMsg");
                    //strSQL = "SELECT count(*) as c from messages where typeDescription=@x  ";
                    conn.Open();

                    SqlCommand cmd2 = new SqlCommand(strSQL, conn);
                    cmd2.Parameters.AddWithValue("@x", u[counter].typeID);
                    string strCount = cmd2.ExecuteScalar().ToString();
                    

                    conn.Close();
                    counter++;
                }

               // Context.Response.ContentType = "application/json";
               //Context.Response.ContentEncoding = Encoding.UTF8;

                //Context.Response.Write(strJson);
                
                strJson = "{" + "\"" + "MyPersons" + "\":" + serializer.Serialize(u) + "}";
                //   string base64 = Base64.encodeToString(data, Base64.DEFAULT);



                byte[] s = System.Text.Encoding.UTF8.GetBytes(strJson);

                Context.Response.ContentType = "application/json";
                Context.Response.ContentEncoding = Encoding.UTF8;
                                Context.Response.AddHeader("Content-Length", s.Length.ToString());

                 Context.Response.BinaryWrite(s);

                Context.Response.Write(strJson);

                strJson = "{" + "\"" + "MyTitles" + "\":" + serializer.Serialize(u) + "}";

                Context.Response.Write(strJson);
             
            }
            catch (Exception ex)
            {


            }


        }

        [WebMethod]
        public void InsertTitle(string TDesc)
        {


            HttpContext context = this.Context;
            string strJson = "";
            try
            {
                //this is the bridge to database
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["messages"].ConnectionString);
                string strSQL = "insert into MessageTypes(TypeDescription) values(@x)";
                conn.Open();

                SqlCommand cmd = new SqlCommand(strSQL, conn);
                cmd.Parameters.AddWithValue("@x", TDesc);
                //cmd.Parameters.AddWithValue("@y", newMsg);
                cmd.ExecuteNonQuery();
                conn.Close();

                strJson = "{" + "\"" + "result" + "\":1" + "}";






                context.Response.Write(strJson);
            }
            catch (Exception ex)
            {
                strJson = "{" + "\"" + "result" + "\":0" + "}";
                context.Response.Write(strJson);
            }

        }

        


        [WebMethod]
        public void InsertMessagesByTitle(string MsgDescription, int TypeDes, int newMsg)
        {


            HttpContext context = this.Context;
            string strJson = "";
            try
            {
                //this is the bridge to database
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["messages"].ConnectionString);
                string strSQL = "INSERT INTO Message(MsgDescription,TypeDescription,newMsg) values(@x,@y,@n)";
                conn.Open();

                SqlCommand cmd = new SqlCommand(strSQL, conn);
                
                cmd.Parameters.AddWithValue("@x", MsgDescription);
                cmd.Parameters.AddWithValue("@y", TypeDes);
                cmd.Parameters.AddWithValue("@n", newMsg);
                cmd.ExecuteNonQuery();
                conn.Close();

                strJson = "{" + "\"" + "result" + "\":1" + "}";






                context.Response.Write(strJson);
            }
            catch (Exception ex)
            {
                strJson = "{" + "\"" + "result" + "\":0" + "}";
                context.Response.Write(strJson);
            }

        }





        [WebMethod]
        public void readMessagesByTitle(string typeID)
        {

            String strJson;
            HttpContext context = this.Context;

            try
            {

                //this is the bridge to database
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["messages"].ConnectionString);
                string strSQL = "SELECT *   FROM Message   where TypeDescription=@x";
                conn.Open();

                SqlCommand cmd = new SqlCommand(strSQL, conn);
                cmd.Parameters.AddWithValue("@x", typeID);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();


                Messages[] u = new Messages[dt.Rows.Count];
                int counter = 0;
                foreach (DataRow row in dt.Rows)
                {


                    // u[counter].MsgID               ===> From Class
                    //row.Field<int>("MsgID");        ===> From DB

                    u[counter] = new Messages();
                    u[counter].TypeID = row.Field<int>("TypeDescription");
                    u[counter].MsgID = row.Field<int>("MsgID");
                    u[counter].MsgDescription = row.Field<string>("MsgDescription");
                    u[counter].newMsg = row.Field<int>("newMsg");
                    counter++;
                }

                // Context.Response.ContentType = "application/json";
                //Context.Response.ContentEncoding = Encoding.UTF8;

                //Context.Response.Write(strJson);

                strJson = "{" + "\"" + "MyPersons" + "\":" + serializer.Serialize(u) + "}";
                //   string base64 = Base64.encodeToString(data, Base64.DEFAULT);



                byte[] s = System.Text.Encoding.UTF8.GetBytes(strJson);

                Context.Response.ContentType = "application/json";
                Context.Response.ContentEncoding = Encoding.UTF8;
                Context.Response.AddHeader("Content-Length", s.Length.ToString());

                Context.Response.BinaryWrite(s);

                Context.Response.Write(strJson);
                

                strJson = "{" + "\"" + "MyMessages" + "\":" + serializer.Serialize(u) + "}";

                Context.Response.Write(strJson);
                // return strPhone;


                // return strPhone;
            }
            catch (Exception ex)
            {


            }


        }




        [WebMethod]
        public void readMessagesByTitleAndFilter(string typeID,string filterValue)
        {

            String strJson;
            HttpContext context = this.Context;

            try
            {

                //this is the bridge to database
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["messages"].ConnectionString);
                string strSQL = "SELECT *   FROM Messages   where TypeDescription=@x and MsgDescription like'%" + filterValue + "%'";
                conn.Open();

                SqlCommand cmd = new SqlCommand(strSQL, conn);
                cmd.Parameters.AddWithValue("@x", typeID);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();


                Messages[] u = new Messages[dt.Rows.Count];
                int counter = 0;
                foreach (DataRow row in dt.Rows)
                {


                    // u[counter].MsgID               ===> From Class
                    //row.Field<int>("MsgID");        ===> From DB

                    u[counter] = new Messages();
                    u[counter].TypeID = row.Field<int>("TypeDescription");
                    u[counter].MsgID = row.Field<int>("MsgID");
                    u[counter].MsgDescription = row.Field<string>("MsgDescription");
                    u[counter].newMsg = row.Field<int>("newMsg");
                    counter++;
                }

                strJson = "{" + "\"" + "MyMessages" + "\":" + serializer.Serialize(u) + "}";

                Context.Response.Write(strJson);
                // return strPhone;


                // return strPhone;
            }
            catch (Exception ex)
            {


            }
        }


        public object typeDescription { get; set; }
    }
}
