using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication3.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
namespace MvcApplication3.Controllers
{
 
    public class HOMEController : Controller
    {
        public string URI;
        public string STATE; 
        public struct Program_parametres
        {
            string id;
            string secret_id;
        }
        public ActionResult Registration()
        {
                 return View();
        }
        [HttpPost]
        public ActionResult RegistrationTrue(string myLogin,string myPass)
        {
     
  
            // создаем cookie
            Response.Cookies["cookieUser1"].Value = myLogin;

            // задаем срок истечения срока действия cookie
            Response.Cookies["cookieUser1"].Expires = DateTime.Now.AddDays(1);

            // задаем значения ключ/значение (key/value) в один cookie


                        string MyCookieValue;
            // сначала нам требуется проверить на null наличие cookie
                        if (Request.Cookies["cookieUser1"] != null)
                            MyCookieValue = Request.Cookies["cookieUser1"].Value;

                        if (Request.Cookies["cookieUser1"] != null)
            {
                // установка срока
             // Response.Cookies["cookieUser1"].Expires = DateTime.Now.AddDays(-1);
            }
            Response.Redirect("Yes");
            return View();
        }

        public ActionResult Index()
        {
            // сначала нам требуется проверить на null наличие cookie
            if (Request.Cookies["cookieUser1"] == null)
            {
                Response.Redirect("/Home/registration");

            }
            //Вычленяем ClientID uri
            String ClientId = Request.Params["ClientID"];
               // return View();

            URI = Request.Params["Redirect"];
            STATE = Request.Params["State"];

            #region Удаляем все записи для URI из таблицы

            string connectionStrin = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            string queryStrin = @"select pr_secret_key from Program_parametres";
            
            connectionStrin = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            queryStrin = @"DELETE FROM Redirect_uri";
            SqlConnection con = new SqlConnection(connectionStrin);
            SqlCommand cm = new SqlCommand(queryStrin, con);
            con.Open();
            SqlDataReader rd = cm.ExecuteReader();
            #endregion Удаляем все записи для URI из таблицы

            #region Добавляем новые параметры URI-а
            connectionStrin = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            queryStrin = @"INSERT INTO Redirect_uri VALUES ( '" + URI + "')";
            con = new SqlConnection(connectionStrin);
            cm = new SqlCommand(queryStrin, con);
            con.Open();
            rd = cm.ExecuteReader();
            #endregion Добавляем новые параметры URI-а


            #region Удаляем все записи для State из таблицы
            connectionStrin = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            queryStrin = @"DELETE FROM StateTable";
            con = new SqlConnection(connectionStrin);
            cm = new SqlCommand(queryStrin, con);
            con.Open();
            rd = cm.ExecuteReader();
            #endregion Удаляем все записи для State из таблицы

            #region Добавляем новые параметры State-а
            connectionStrin = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            queryStrin = @"INSERT INTO StateTable VALUES ( '" + STATE + "')";
            con = new SqlConnection(connectionStrin);
            cm = new SqlCommand(queryStrin, con);
            con.Open();
            rd = cm.ExecuteReader();
            #endregion Добавляем новые параметры State-а

            //Достаем Secret_key
            #region
            string connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            int[] ClientSKeyFromBase = new int[5];
            string queryString = @"select pr_secret_key from Program_parametres";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(queryString, conn);


            conn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            int i = 0;
            while (rdr.Read())
            {
                ClientSKeyFromBase[i] = int.Parse(rdr.GetValue(i).ToString());
                i++;
            }
              rdr.Close();
            conn.Close();
             #endregion 

            //Достаем Id
            #region
            connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            int[] ClientIdFromBase = new int[5];
            queryString = @"select pr_id from Program_parametres";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand(queryString, conn);


            conn.Open();
            rdr = cmd.ExecuteReader();
            i = 0;
            while (rdr.Read())
            {
                ClientIdFromBase[i] = int.Parse(rdr.GetValue(i).ToString());
                i++;
            }
            rdr.Close();
            conn.Close();
            #endregion 
           if (Convert.ToInt32(ClientId) != ClientIdFromBase[0])
                return Redirect("Home/No");
               //return new HttpStatusCodeResult(300);
            else
                return View();
        }
        [HttpGet]
        public ActionResult Yes()
        {
            //Достаем из базы 

            string connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            int[] results = new int[5];
            string queryString = @"select * from Program_parametres";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(queryString, conn);
            conn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            string[] SResults = new string[5];
            queryString = @"select * from Redirect_uri";
            conn = new SqlConnection(connectionString);
           cmd = new SqlCommand(queryString, conn);


            conn.Open();
            rdr = cmd.ExecuteReader();
            int i = 0;
            while (rdr.Read())
            {
                SResults[i] = (rdr.GetValue(i).ToString());
                i++;
            }

            rdr.Close();
            conn.Close();

            queryString = @"select * from StateTable";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand(queryString, conn);
            conn.Open();
            rdr = cmd.ExecuteReader();
            queryString = @"select * from StateTable";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand(queryString, conn);


            conn.Open();
            rdr = cmd.ExecuteReader();
            i = 0;
            while (rdr.Read())
            {
                SResults[1] = (rdr.GetValue(i).ToString());
                i++;
            }

            rdr.Close();
            conn.Close();

            Random rnd = new Random();
            int code = rnd.Next(1, 123565);


            connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            queryString = @"DELETE FROM tblCode";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand(queryString, conn);
            conn.Open();
            rdr = cmd.ExecuteReader();

            queryString = @"INSERT INTO tblCode VALUES ( '" + code + "' )";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand(queryString, conn);
            conn.Open();
            rdr = cmd.ExecuteReader();
           // SResults[0]

            return Redirect(SResults[0].Trim() + "?code=" + code  + "&state=" + SResults[1].Trim());
            //return Redirect(URI + "?code=" + code + "&state" + STATE);
          
        }
        [HttpPost]
        public string Buy(Purchase purchase)
        {

            purchase.Date = DateTime.Now;
            // добавляем информацию о покупке в базу данных
            return "Спасибо, за покупку!";
        }

    }
}
