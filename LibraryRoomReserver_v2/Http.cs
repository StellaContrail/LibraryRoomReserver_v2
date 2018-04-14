using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;

namespace LibraryRoomReserver_v2
{
    class Http
    {
        public static string SubmitPOST(Hashtable vals, string targetURL, CookieContainer cookies)
        {
            string param = "";
            foreach (string k in vals.Keys)
            {
                param += String.Format("{0}={1}&", k, vals[k]);
            }
            byte[] data = Extensions.encoder.GetBytes(param);

            // HTTP要求を作成
            HttpWebRequest postRequest = WebRequest.Create(targetURL) as HttpWebRequest;
            // POST送信
            postRequest.Method = "POST";
            postRequest.ContentType = "application/x-www-form-urlencoded";
            postRequest.ContentLength = data.Length;
            // ページ移行するにはログインしたときのCookieが必要になる
            postRequest.CookieContainer = cookies;

            // POST送信を実行
            Stream requestStream = postRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            // レスポンスの受信
            WebResponse postResponse = postRequest.GetResponse();
            // 取得したレスポンスの中身を取得
            Stream responseStream = postResponse.GetResponseStream();

            return responseStream.GetToken();
        }

        public static dynamic SubmitGET(string targetURL)
        {
            // Cookieを入れる変数を用意
            CookieContainer cookies = new CookieContainer();
            // HTTP要求を作成
            HttpWebRequest getRequest = WebRequest.Create(targetURL) as HttpWebRequest;
            // GET送信
            getRequest.Method = "GET";
            // 取得したCookieを変数ccに格納するように設定
            // ログイン時にCookie:JSESSIONIDが必要になる
            getRequest.CookieContainer = cookies;

            // サーバにリクエストの送信、レスポンスの受信
            WebResponse getResponse = getRequest.GetResponse();
            // 取得したレスポンスの中身を取得
            Stream reponseStream = getResponse.GetResponseStream();
            string token = reponseStream.GetToken();
            reponseStream.Close();

            return new
            {
                Cookies = cookies,
                Token = token
            };
        }
    }
}
