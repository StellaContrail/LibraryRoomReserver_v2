using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;

namespace LibraryRoomReserver_v2
{
    public struct Person
    {
        public string telNum;
        public string email;
        public Person(string _telNum, string _email) { telNum = _telNum; email = _email; }
    }
    public struct ReservationInfo
    {
        public int roomNum;
        public string reason;
        public int num;
        public ReservationInfo(int _roomNum, string _reason, int _num) { roomNum = _roomNum; reason = _reason; num = _num; }
    }

    class Reservation
    {
        static DateTime reservedDate;
        static int stayMinutes;
        static Person person;
        static ReservationInfo reservation;
        static Hashtable vals;
        public static dynamic Login(string id, string password)
        {
            reservedDate = new DateTime();
            stayMinutes = 60;
            person = default(Person);
            reservation = default(ReservationInfo);

            dynamic data = Http.SubmitGET("https://opac.lib.niigata-u.ac.jp/portal/");
            // POST送信するパラメータを作成
            vals = new Hashtable
            {
                ["biblioId"] = "",
                ["lang"] = "ja",
                ["lm"] = "",
                ["org.apache.struts.taglib.html.TOKEN"] = data.Token,
                ["p"] = "",
                ["portalId"] = id,
                ["portalPasswd"] = password,
                ["rftAtitle"] = "",
                ["rftAufirst"] = "",
                ["rftAulast"] = "",
                ["rftBtitle"] = "",
                ["rftDate"] = "",
                ["rftEpage"] = "",
                ["rftIsbn"] = "",
                ["rftIssn"] = "",
                ["rftIssue"] = "",
                ["rftJtitle"] = "",
                ["rftSpage"] = "",
                ["rftVolume"] = "",
                ["srcActionPath"] = "https://opac.lib.niigata-u.ac.jp/portal/portal/portalLogin.do",
                ["srcOpeId"] = "portal",
                ["srcScreenId"] = "portalLogin"
            };
            return new
            {
                data.Cookies,
                Token = Http.SubmitPOST(vals, "https://opac.lib.niigata-u.ac.jp/portal/portal/portalLogin/doLogin", data.Cookies)
            };
        }

        public static void OpenReservationMenu(ref dynamic data)
        {
            // POST送信するパラメータを作成
            vals = new Hashtable
            {
                ["dataCss"] = "",
                ["dataId"] = "",
                ["nalisContexPath"] = "/portal",
                ["org.apache.struts.taglib.html.TOKEN"] = data.Token,
                ["srcActionPath"] = "https://opac.lib.niigata-u.ac.jp/portal/portal/portalHome.do",
                ["srcOpeId"] = "portal",
                ["srcScreenId"] = "portalHome"
            };
            data = new
            {
                data.Cookies,
                Token = Http.SubmitPOST(vals,
                "https://opac.lib.niigata-u.ac.jp/portal/admin/selectMenu/doSelectPublicUseMainMenu?selectedMenuId=11&selectMenu=1",
                data.Cookies)
            };

        }

        public static void SetReservationFacilityAndData(ref dynamic data)
        {
            // POST送信するパラメータを作成
            // tblCurrent_length, tblHistory_lengthは謎
            vals = new Hashtable
            {
                ["boothNumber"] = "",
                ["chkStatus"] = "ALL",
                ["mylScreenId"] = "",
                ["nalisContextPath"] = "/portal",
                ["org.apache.struts.taglib.html.TOKEN"] = data.Token,
                ["requestId"] = "",
                ["selBoothlist"] = "",
                ["selReserveDateFrom"] = DateTime.Today.ToShortDateString(),
                ["selReserveDateTo"] = DateTime.Today.AddMonths(1).ToShortDateString(),
                ["srcActionPath"] = "https://opac.lib.niigata-u.ac.jp/portal/myl/scMylBO001.do",
                ["srcOpeId"] = "myl",
                ["srcScreenId"] = "scMylBO001",
                ["tabIndex"] = "0",
                ["tblCurrent_length"] = "10",
                ["tblHistory_length"] = "10"
            };
            data = new
            {
                data.Cookies,
                Token = Http.SubmitPOST(vals,
                "https://opac.lib.niigata-u.ac.jp/portal/myl/scMylBO001/doForwardNewRequest",
                data.Cookies)
            };
        }

        public static void SetDate(ref dynamic data, Person _person, ReservationInfo _reservation, DateTime _reservedDate, int _stayMinutes)
        {
            // POST送信するパラメータを作成
            _reservedDate = new DateTime(_reservedDate.Year, _reservedDate.Month, _reservedDate.Day, _reservedDate.Hour, (_reservedDate.Minute / 10) * 10, _reservedDate.Second);
            reservedDate = _reservedDate;
            stayMinutes = _stayMinutes;
            person = _person;
            reservation = _reservation;
            vals = new Hashtable
            {
                ["selSearchRoom"] = "",
                ["searchDate"] = reservedDate.ToShortDateString(),
                ["nalisContextPath"] = "/portal",
                ["org.apache.struts.taglib.html.TOKEN"] = data.Token,
                ["srcActionPath"] = "https://opac.lib.niigata-u.ac.jp/portal/myl/scMylBO002.do",
                ["srcOpeId"] = "myl",
                ["srcScreenId"] = "scMylBO002"
            };
            data = new
            {
                data.Cookies,
                Token = Http.SubmitPOST(vals,
                "https://opac.lib.niigata-u.ac.jp/portal/myl/scMylBO002/doForwardSearch",
                data.Cookies)
            };
        }

        public static void TryReserve(ref dynamic data)
        {
            // POST送信するパラメータを作成
            DateTime reservedDateTo = reservedDate.AddMinutes((stayMinutes / 10) * 10);
            vals = new Hashtable
            {
                ["selSearchRoom"] = "",
                ["searchDate"] = reservedDate.ToShortDateString(),
                ["selContact"] = person.telNum,
                ["selEmail"] = person.email,
                ["selRoom"] = "001" + (reservation.roomNum - 3).ToString(),
                ["reserveDate"] = reservedDate.ToShortDateString(),
                ["selReserveTimeFromH"] = reservedDate.ToString("HH"),
                ["selReserveTimeFromM"] = reservedDate.ToString("mm"),
                ["selReserveTimeToH"] = reservedDateTo.ToString("HH"),
                ["selReserveTimeToM"] = reservedDateTo.ToString("mm"),
                ["reserveName"] = reservation.reason,
                ["personCount"] = reservation.num,
                ["note"] = "",
                ["nalisContextPath"] = "/portal",
                ["org.apache.struts.taglib.html.TOKEN"] = data.Token,
                ["srcActionPath"] = "https://opac.lib.niigata-u.ac.jp/portal/myl/scMylBO002.do",
                ["srcOpeId"] = "myl",
                ["srcScreenId"] = "scMylBO002"
            };
            data = new
            {
                data.Cookies,
                Token = Http.SubmitPOST(vals,
                "https://opac.lib.niigata-u.ac.jp/portal/myl/scMylBO002/doForwardConfirm",
                data.Cookies)
            };
        }

        public static void ConfirmReserve(ref dynamic data)
        {
            // POST送信するパラメータを作成
            // 様々な情報を変数でとってくる必要あり
            vals = new Hashtable
            {
                ["nalisContextPath"] = "/portal",
                ["org.apache.struts.taglib.html.TOKEN"] = data.Token,
                ["srcActionPath"] = "https://opac.lib.niigata-u.ac.jp/portal/myl/scMylBO003.do",
                ["srcOpeId"] = "myl",
                ["srcScreenId"] = "scMylBO003"
            };
            data = new
            {
                data.Cookies,
                Token = Http.SubmitPOST(vals,
                "https://opac.lib.niigata-u.ac.jp/portal/myl/scMylBO003/doForwardConfirm",
                data.Cookies)
            };
        }
    }
}
