using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace LibraryRoomReserver_v2
{
    class Program
    {
        // ページを移行するには
        // 最初のページで受け取ったCookie:JSESSIONID
        // それぞれのページで受け取るTokenが必要になる
        // srcActionPathで前にActionしたスクリプトを参照しているらしく、
        // 順番にページを開かなければ弾かれてしまう
        // ID PASS TEL EMAIL ROOMNUM REASON PEOPLENUM FROMHOUR FROMMIN MINUTESSTAYFOR
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DateTime requestedTime;
            DateTime weekLaterFromToday = DateTime.Today.AddDays(7);

            // ログインしてセッションCookieを取得
            dynamic data = Reservation.Login(args[0], args[1]);
            Console.WriteLine("[{0}] Successfully Logged in.", sw.ElapsedMilliseconds);
            // グループ学習室の施設予約を開く
            Reservation.OpenReservationMenu(ref data);
            Console.WriteLine("[{0}] Preparing to reserve...", sw.ElapsedMilliseconds);
            // 新規施設予約を選択する
            Reservation.SetReservationFacilityAndData(ref data);
            // 日付を選択して進む
            Reservation.SetDate(ref data,
                new Person(args[2], args[3]),
                new ReservationInfo(int.Parse(args[4]), args[5], int.Parse(args[6])),
                new DateTime(weekLaterFromToday.Year, weekLaterFromToday.Month, weekLaterFromToday.Day, int.Parse(args[7]), int.Parse(args[8]), 00),
                int.Parse(args[9]));
            // 必要情報を入力して予約する
            Reservation.TryReserve(ref data);
            requestedTime = DateTime.Now;
            Console.WriteLine("[{0}] Attempting to reserve...", sw.ElapsedMilliseconds);
            // 予約を確定する
            Reservation.ConfirmReserve(ref data);
            sw.Stop();
            Console.WriteLine("[{0}] Reserved.", sw.ElapsedMilliseconds);
            Console.WriteLine("Reservation from {0}:{1} {2} for {3} minutes has been reserved successfully with {4} sec delay after the request.",
                args[7], args[8], weekLaterFromToday.ToShortDateString(), args[9], (double)sw.ElapsedTicks / (double)Stopwatch.Frequency);
            Console.WriteLine("[Notice] This program highly depends on your computer local time and network traffic." +
                " If you want to reserve more precisely, please look up your computer time settings and traffic condition.");
            Console.ReadLine();
        }


    }
}
