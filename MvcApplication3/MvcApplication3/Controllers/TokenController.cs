using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http;
using System.Web.Helpers;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.ObjectModel;
using MvcApplication3.Models;
using System.Web.Script.Serialization;
using System.Collections.Specialized;

namespace MvcApplication3.Controllers
{
    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(obj);
        }
    }

    public class Generator
    {
        public static byte[] RollDice(int passLenght)
        {
            byte[] randomNumber = new byte[passLenght];
            byte[] password = new byte[passLenght];


            // Создайте новый экземпляр RNGCryptoServiceProvider
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();

            // Заполняем массив случайными значениями
            Gen.GetBytes(randomNumber);

            for (int i = 0; i < randomNumber.Length; i++)
            {
                if ((int)randomNumber[i] > 48 && (int)randomNumber[i] < 57 ||
                    (int)randomNumber[i] > 65 && (int)randomNumber[i] < 90 ||
                    (int)randomNumber[i] > 97 && (int)randomNumber[i] < 122)
                {
                    password[i] = randomNumber[i];
                }
                else
                {
                    Gen.GetBytes(randomNumber);
                    i--;
                }
            }
            return password;
        }
    }

    public class GetAllInfo
    {
        public string pages { get; set; }
        public string per_page { get; set; }
        public string total { get; set; }
    }
    public class GetAllVacansies2
    {
        public string id { set; get; }
        public string vacation_name { get; set; }

        public string vacation_cash { get; set; }

        public string description { get; set; }

    }

    public class GetAllVacansies
    {
        
        public string vacation_name { get; set; }

        public string vacation_cash { get; set; }

       

    }

    public class Jsoner
    {
        public GetAllVacansies[] Items { get; set; }
        public GetAllInfo Info {set; get;}
    }
    public class TokenQuestion
    {
        public string client_id { get; set; }

        public string secret_id { get; set; }

        public string code { get; set; }

        public string redirect { get; set; }

        public string state { get; set; }

        public string refresh_token { get; set; }
    }
    public class TokenAnswer
    {
        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public string expires { get; set; }
    }

    public class TokenController : ApiController
    {

        public TokenAnswer Post(TokenQuestion model)
        {
            if (model.refresh_token != null)
            {
                #region Достаем refresh_token ClientSRefrTokenFromBase[0]
                string connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
                string[] ClientSRefrTokenFromBase = new string[5];
                string queryString = @"select refresh_token from Token_table";
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(queryString, conn);


                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                int i = 0;
                while (rdr.Read())
                {
                    ClientSRefrTokenFromBase[i] = rdr.GetValue(i).ToString();
                    i++;
                }
                rdr.Close();
                conn.Close();
                #endregion
                byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(ClientSRefrTokenFromBase[0]);
                string returnValue1 = System.Convert.ToBase64String(toEncodeAsBytes);
                toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(model.refresh_token);
                string returnValue2 = System.Convert.ToBase64String(toEncodeAsBytes);
                returnValue1 = returnValue1.Replace("IA==", "");
                returnValue2 = returnValue2.Replace("IA==", "");
                if (returnValue1 == returnValue2)
                {
                    #region Устанавливаем данные для токенов с помощью криптографии
                    string acc_token = "";
                    string refr_token = "";
                    byte[] temp = Generator.RollDice(12);
                    for (int j = 0; j < temp.Length; j++)
                    {
                        acc_token += (char)temp[j];
                    }
                    temp = Generator.RollDice(12);
                    for (int k = 0; k < temp.Length; k++)
                    {
                        refr_token += (char)temp[k];
                    }

                    System.DateTime today = System.DateTime.Now;
                    System.TimeSpan duration = new System.TimeSpan(2, 0, 0);
                    System.DateTime exp = today.Add(duration);


                    #endregion Устанавливаем данные для токенов с помощью криптографии
                    #region Удаляем все записи для Token из таблицы
                    connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
                    queryString = @"DELETE FROM Token_table";
                    conn = new SqlConnection(connectionString);
                    cmd = new SqlCommand(queryString, conn);
                    conn.Open();
                    rdr = cmd.ExecuteReader();
                    #endregion Удаляем все записи для Token из таблицы

                    #region Добавляем новые параметры Token-а
                    connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
                    queryString = @"INSERT INTO Token_table VALUES ( '" + acc_token + "','" + refr_token + "','" + exp.ToString() + "' )";
                    conn = new SqlConnection(connectionString);
                    cmd = new SqlCommand(queryString, conn);
                    conn.Open();
                    rdr = cmd.ExecuteReader();
                    #endregion Добавляем новые параметры Token-а

                    return new TokenAnswer { access_token = acc_token, refresh_token = refr_token, expires = exp.ToString() };
                }
                else
                {
                    return new TokenAnswer { access_token = "Неверный", refresh_token = "refr_token" };
                }
            }
            #region Достаем Secret_key
            string connectionStrin = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            int[] ClientSKeyFromBase = new int[5];
            string queryStrin = @"select pr_secret_key from Program_parametres";
            SqlConnection con = new SqlConnection(connectionStrin);
            SqlCommand cm = new SqlCommand(queryStrin, con);


            con.Open();
            SqlDataReader rd = cm.ExecuteReader();
            int l = 0;
            while (rd.Read())
            {
                ClientSKeyFromBase[l] = int.Parse(rd.GetValue(l).ToString());
                l++;
            }
            rd.Close();
            con.Close();
            #endregion

            #region Достаем Id
            int[] ClientIdFromBase = new int[5];
            queryStrin = @"select pr_id from Program_parametres";
            con = new SqlConnection(connectionStrin);
            cm = new SqlCommand(queryStrin, con);


            con.Open();
            rd = cm.ExecuteReader();
            l = 0;
            while (rd.Read())
            {
                ClientIdFromBase[l] = int.Parse(rd.GetValue(l).ToString());
                l++;
            }
            rd.Close();
            con.Close();
            #endregion 

            #region Достаем Code
            int[] CodeFromBase = new int[5];
            queryStrin = @"select * from tblCode";
            con = new SqlConnection(connectionStrin);
            cm = new SqlCommand(queryStrin, con);
            con.Open();
            rd = cm.ExecuteReader();
            l = 0;
            l = 0;
            while (rd.Read())
            {
                CodeFromBase[l] = int.Parse(rd.GetValue(l).ToString());
                l++;
            }
            rd.Close();
            con.Close();
            #endregion 

            if ((model.client_id == ClientIdFromBase[0].ToString()) && (model.secret_id == ClientSKeyFromBase[0].ToString())
                && (model.code == CodeFromBase[0].ToString()))
            {

            #region Устанавливаем данные для токенов с помощью криптографии
                string acc_token = "";
            string refr_token = "";
            byte[] temp = Generator.RollDice(12);
            for (int j = 0; j < temp.Length; j++)
            {
                acc_token += (char)temp[j];
            }
            temp = Generator.RollDice(12);
            for (int k = 0; k < temp.Length; k++)
            {
                refr_token += (char)temp[k];
            }

            System.DateTime today = System.DateTime.Now;
            System.TimeSpan duration = new System.TimeSpan(2, 0, 0);
            System.DateTime exp = today.Add(duration);


            #endregion Устанавливаем данные для токенов с помощью криптографии

            #region Удаляем все записи для Token из таблицы
            connectionStrin = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            queryStrin = @"DELETE FROM Token_table";
            con = new SqlConnection(connectionStrin);
            cm = new SqlCommand(queryStrin, con);
            con.Open();
            rd = cm.ExecuteReader();
            #endregion Удаляем все записи для Token из таблицы

            #region Добавляем новые параметры Token-а
            connectionStrin = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            queryStrin = @"INSERT INTO Token_table VALUES ( '" + acc_token + "','" + refr_token + "','" + exp.ToString() + "' )";
            con = new SqlConnection(connectionStrin);
            cm = new SqlCommand(queryStrin, con);
            con.Open();
            rd = cm.ExecuteReader();
            #endregion Добавляем новые параметры Token-а
/*
            
 */

            return new TokenAnswer { access_token = acc_token, refresh_token = refr_token, expires = exp.ToString() };
            }
            return new TokenAnswer { access_token = "404", refresh_token = "404", expires = "404" };
        }
        

    }
    public class VacansiesController : ApiController
    {
        public HttpResponseMessage Get()
        {
            NameValueCollection coll;

            coll = HttpContext.Current.Request.Headers;

            String[] arr1 = coll.AllKeys;
            String[] arr2 = coll.GetValues(arr1[6]);


            #region Достаем refresh_token ClientSRefrTokenFromBase[0]
            string connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            string[] ClientSRefrTokenFromBase = new string[5];
            string queryString = @"select access_token from Token_table";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(queryString, conn);


            conn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            int i = 0;
            while (rdr.Read())
            {
                ClientSRefrTokenFromBase[i] = rdr.GetValue(i).ToString();
                i++;
            }
            rdr.Close();
            conn.Close();
            #endregion

            #region Достаем token
            connectionString = WebConfigurationManager.ConnectionStrings["Oauth2"].ConnectionString;
            string[] ClientSKeyFromBase = new string[5];
            queryString = @"select access_token from Token_table";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand(queryString, conn);


            conn.Open();
            rdr = cmd.ExecuteReader();
            i = 0;
            while (rdr.Read())
            {
                ClientSKeyFromBase[i] = rdr.GetValue(i).ToString();
                i++;
            }
            rdr.Close();
            conn.Close();
            #endregion

            #region Достаем expires
            string[] ClientSExpiresFromBase = new string[5];
            queryString = @"select expired from Token_table";
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand(queryString, conn);


            conn.Open();
            rdr = cmd.ExecuteReader();
            i = 0;
            while (rdr.Read())
            {
                ClientSExpiresFromBase[i] = rdr.GetValue(i).ToString();
                i++;
            }
            rdr.Close();
            conn.Close();
            #endregion

            String PageNum = "";
            String Num = "";
            String Token = "";
            String Per_page = "";
            PageNum = HttpContext.Current.Request.QueryString["Page"];
            Per_page = HttpContext.Current.Request.QueryString["per_page"];
            Token = HttpContext.Current.Request.QueryString["Token"];
            Num = HttpContext.Current.Request.QueryString["Num"];
            String TokenTmp = ClientSKeyFromBase[0];
            String ExpiresTmp = ClientSExpiresFromBase[0];

            DateTime ExspiresDate = DateTime.Parse(ExpiresTmp);
            if (DateTime.Now > ExspiresDate)
            {
                GetAllVacansies ErrExp = new GetAllVacansies();
                ErrExp.vacation_name = "Ошибка!";
                ErrExp.vacation_cash = "Токен просрочен!";
                string js = ErrExp.ToJSON();
                
                 return new HttpResponseMessage()
    {
        Content = new StringContent(js),
        StatusCode = System.Net.HttpStatusCode.Unauthorized
    };

            }


            arr2[0] = arr2[0].Replace("bearer ", "");
            byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(arr2[0]);
     
            string returnValue1 = System.Convert.ToBase64String(toEncodeAsBytes);
            toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(TokenTmp);
            string returnValue2 = System.Convert.ToBase64String(toEncodeAsBytes);
            returnValue2 = returnValue2.Replace("IA==", "");
            returnValue1 = returnValue1.Replace("IA==", "");
            if (returnValue1 == returnValue2)
            {
                if (PageNum == "") PageNum = "1";
                #region Определение данных
                GetAllVacansies[] T = new GetAllVacansies[25];
                T[0] = new GetAllVacansies(); T[0].vacation_name = "Турция"; T[0].vacation_cash = "50 000 рублей"; 
                T[1] = new GetAllVacansies(); T[1].vacation_name = "Турция"; T[1].vacation_cash = "50 000 рублей"; 
                T[2] = new GetAllVacansies(); T[2].vacation_name = "Египет"; T[2].vacation_cash = "60 000 рублей"; 
                T[3] = new GetAllVacansies(); T[3].vacation_name = "Польша"; T[3].vacation_cash = "70 000 рублей"; 
                T[4] = new GetAllVacansies(); T[4].vacation_name = "Литва"; T[4].vacation_cash = "80 000 рублей";
                T[5] = new GetAllVacansies(); T[5].vacation_name = "Латвия"; T[5].vacation_cash = "90 000 рублей";
                T[6] = new GetAllVacansies(); T[6].vacation_name = "Германия"; T[6].vacation_cash = "50 000 рублей";
                T[7] = new GetAllVacansies(); T[7].vacation_name = "Америка"; T[7].vacation_cash = "40 000 рублей";
                T[8] = new GetAllVacansies(); T[8].vacation_name = "Нидерланды"; T[8].vacation_cash = "20 000 рублей";
                T[9] = new GetAllVacansies(); T[9].vacation_name = "Бразилия"; T[9].vacation_cash = "60 000 рублей";
                T[10] = new GetAllVacansies(); T[10].vacation_name = "Армения"; T[10].vacation_cash = "70 000 рублей";
                T[11] = new GetAllVacansies(); T[11].vacation_name = "Грузия"; T[11].vacation_cash = "40 000 рублей";
                T[12] = new GetAllVacansies(); T[12].vacation_name = "Китай"; T[12].vacation_cash = "60 000 рублей";
                T[13] = new GetAllVacansies(); T[13].vacation_name = "Тайланд"; T[13].vacation_cash = "80 000 рублей";
                T[14] = new GetAllVacansies(); T[14].vacation_name = "КНДР"; T[14].vacation_cash = "80 000 рублей";
                T[15] = new GetAllVacansies(); T[15].vacation_name = "Япония"; T[15].vacation_cash = "100 000 рублей";
                T[16] = new GetAllVacansies(); T[16].vacation_name = "Австралия"; T[16].vacation_cash = "110 000 рублей";
                T[17] = new GetAllVacansies(); T[17].vacation_name = "Перу"; T[17].vacation_cash = "12 000 рублей";
                T[18] = new GetAllVacansies(); T[18].vacation_name = "Ватикан"; T[18].vacation_cash = "13 000 рублей";
                T[19] = new GetAllVacansies(); T[19].vacation_name = "Испания"; T[19].vacation_cash = "14 000 рублей";
                T[20] = new GetAllVacansies(); T[20].vacation_name = "Италия"; T[20].vacation_cash = "16 000 рублей";
                T[21] = new GetAllVacansies(); T[21].vacation_name = "Саудовская аравия"; T[21].vacation_cash = "300 000 рублей";
                T[22] = new GetAllVacansies(); T[22].vacation_name = "Мексика"; T[22].vacation_cash = "9 000 рублей";
                T[23] = new GetAllVacansies(); T[23].vacation_name = "Англия"; T[23].vacation_cash = "20 000 рублей";
                T[24] = new GetAllVacansies(); T[24].vacation_name = "Куба"; T[24].vacation_cash = "10 000 рублей";
                #endregion Определение данных

                #region bdl
                GetAllVacansies2[] TS = new GetAllVacansies2[25];
                TS[0] = new GetAllVacansies2(); TS[0].vacation_name = "Турция"; TS[0].vacation_cash = "50 000 рублей";
                TS[1] = new GetAllVacansies2(); TS[1].vacation_name = "Турция"; TS[1].vacation_cash = "50 000 рублей";
                TS[2] = new GetAllVacansies2(); TS[2].vacation_name = "Египет"; TS[2].vacation_cash = "60 000 рублей";
                TS[3] = new GetAllVacansies2(); TS[3].vacation_name = "Польша"; TS[3].vacation_cash = "70 000 рублей";
                TS[4] = new GetAllVacansies2(); TS[4].vacation_name = "Литва"; TS[4].vacation_cash = "80 000 рублей";
                TS[5] = new GetAllVacansies2(); TS[5].vacation_name = "Латвия"; TS[5].vacation_cash = "90 000 рублей";
                TS[6] = new GetAllVacansies2(); TS[6].vacation_name = "Германия"; TS[6].vacation_cash = "50 000 рублей";
                TS[7] = new GetAllVacansies2(); TS[7].vacation_name = "Америка"; TS[7].vacation_cash = "40 000 рублей";
                TS[8] = new GetAllVacansies2(); TS[8].vacation_name = "Нидерланды"; TS[8].vacation_cash = "20 000 рублей";
                TS[9] = new GetAllVacansies2(); TS[9].vacation_name = "Бразилия"; TS[9].vacation_cash = "60 000 рублей";
                TS[10] = new GetAllVacansies2(); TS[10].vacation_name = "Армения"; TS[10].vacation_cash = "70 000 рублей";
                TS[11] = new GetAllVacansies2(); TS[11].vacation_name = "Грузия"; TS[11].vacation_cash = "40 000 рублей";
                TS[12] = new GetAllVacansies2(); TS[12].vacation_name = "Китай"; TS[12].vacation_cash = "60 000 рублей";
                TS[13] = new GetAllVacansies2(); TS[13].vacation_name = "Тайланд"; TS[13].vacation_cash = "80 000 рублей";
                TS[14] = new GetAllVacansies2(); TS[14].vacation_name = "КНДР"; TS[14].vacation_cash = "80 000 рублей";
                TS[15] = new GetAllVacansies2(); TS[15].vacation_name = "Япония"; TS[15].vacation_cash = "100 000 рублей";
                TS[16] = new GetAllVacansies2(); TS[16].vacation_name = "Австралия"; TS[16].vacation_cash = "110 000 рублей";
                TS[17] = new GetAllVacansies2(); TS[17].vacation_name = "Перу"; TS[17].vacation_cash = "12 000 рублей";
                TS[18] = new GetAllVacansies2(); TS[18].vacation_name = "Ватикан"; TS[18].vacation_cash = "13 000 рублей";
                TS[19] = new GetAllVacansies2(); TS[19].vacation_name = "Испания"; TS[19].vacation_cash = "14 000 рублей";
                TS[20] = new GetAllVacansies2(); TS[20].vacation_name = "Италия"; TS[20].vacation_cash = "16 000 рублей";
                TS[21] = new GetAllVacansies2(); TS[21].vacation_name = "Саудовская аравия"; TS[21].vacation_cash = "300 000 рублей";
                TS[22] = new GetAllVacansies2(); TS[22].vacation_name = "Мексика"; TS[22].vacation_cash = "9 000 рублей";
                TS[23] = new GetAllVacansies2(); TS[23].vacation_name = "Англия"; TS[23].vacation_cash = "20 000 рублей";
                TS[24] = new GetAllVacansies2(); TS[24].vacation_name = "Куба"; TS[24].vacation_cash = "10 000 рублей";


                #endregion bdl
                for (int p = 0; p < 25; p++)
                {
                    TS[p].id = p.ToString();
                    TS[p].description = p.ToString() + "descr";
                }
                
                int CountOnStr = 5;
                if (Per_page != null)
                    CountOnStr = int.Parse(Per_page);

                GetAllVacansies[] Returnig = new GetAllVacansies[CountOnStr];
                GetAllVacansies2[] Returnig2 = new GetAllVacansies2[CountOnStr];


                Jsoner panic = new Jsoner();
                panic.Items = Returnig;
                


                int Tmp = Convert.ToInt32(PageNum) * CountOnStr;
                int j = 0;
                for (int k = Tmp; k < Tmp + CountOnStr; k++)
                {
                    panic.Items[j] = T[k];
                    //Returnig2[k] = TS[k];
                    j++;
                }

                GetAllInfo inf = new GetAllInfo();
                inf.pages = (25 / CountOnStr).ToString();
                inf.per_page = Per_page;
                inf.total = "25";


                var json = Returnig.ToJSON();
                var json_inf = inf.ToJSON();

                panic.Info = inf;
                var total = inf.ToJSON();

                if (Num != null)
                {
                    json = TS[Convert.ToInt32(Num)].ToJSON();



                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json)
                    };
                }
                else
                {


                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json + json_inf)
                        // Content = new StringContent(total)
                    };
                }
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("Неверный токен"),
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                };
            }
        } 
    }

}
