using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.Notifications;
namespace S.toNoApi
{
    public static class Verwaltung
    {
        public static List<string> Hosters = new List<string> { "Vivo", "Vidoza", "VOE", "Streamtape", };

        public static string localfolder = ApplicationData.Current.LocalFolder.Path;

        public static Setting Settingv1 = new Setting();

        static string cookie = "";

       public static string[] linkname = { "Abenteuer", "Action", "Animation", "Anime", "Comedy", "Dokumentation", "Dokusoap", "Drama", "Dramedy", "Familie", "Fantasy", "History", "Horror", "Jugend", "Kinderserie", "Krankenhaus", "Krimi", "Mystery", "Romantik", "Science-Fiction", "Sitcom", "Telenovela", "Thriller", "Western", "Zeichentrick", "K-Drama", "Reality-Tv", "Netflix-Originals", "Amazon-Originals" };

        public static void Message(string s)
        {
            var content = new ToastContentBuilder()
          .AddToastActivationInfo("NEtFLi", ToastActivationType.Foreground)
          .AddText(s)
          .GetToastContent();

            // Create the notification
            var notif = new ToastNotification(content.GetXml());

            // And show it!
            ToastNotificationManager.CreateToastNotifier().Show(notif);
        }

        public static async Task<bool> login(string username, string password)
        {
            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>{{ "email", $"{username}" },
    { "password", $"{password}" }
};

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync($"https://s.to/login", content);

            cookie = response.Headers.GetValues("Set-Cookie").ToList()[1].Split(';')[0];
            var txt = await response.Content.ReadAsStringAsync();
            if (txt == "")
            {
                Settingv1.ssid = cookie;

                SaveSettings();
                return true;
            }
            else
            {
                return false;
            }
            Console.WriteLine(cookie);


        }

        public static void addWatch(Serie serie)
        {
          


            int index = MyWatchlist.Serien.FindIndex(x => x.Title == serie.Title);
            serie.DateTime = DateTime.Now;
            if (index != -1)
                MyWatchlist.Serien.RemoveAt(index);
            MyWatchlist.Serien.Insert(0, serie);



            Verwaltung.saveWatchlist();
        }

        public static SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }
        public static string NEpis(string date)
        {

            var data = DateTime.Parse(date);

            var output = data - DateTime.Now;

            var days = output.Days;




            return (days > 0) ? days.ToString() + " Day(s) " : " Today ";
        }
        static void Setup()
        {

        }
        public static string NextEpisodeRelease(string name)
        {
            string link = name;
            if (link == "One Piece")
                link += " Jp";
            try
            {
                string html = new WebClient().DownloadString("https://next-episode.net/site-search-" + link + ".html");
                string data0 = getBeetween(html, "<div id=\"next_episode\">", "<div id=\"middle_section_schedule\">");
                data0 = getBeetween(data0, ">\"", "\"<");
                if (data0 == "")
                {
                    data0 = getBeetween(html, "<div id=\"next_episode\">", "<div id=\"middle_section_schedule\">");
                    data0 = getBeetween(data0, "<span", "/span>");
                    data0 = getBeetween(data0, ">", "<");
                }
                return data0.Trim();
            }
            catch
            {
                return "No Info.";
            }
        }
        public static void laodSettings()
        {
            Debug.WriteLine(localfolder + "\\" + "Settings.json");
            if (File.Exists(localfolder + "\\" + "Settings.json"))
            {

                string data = File.ReadAllText(localfolder + "\\" + "Settings.json");
                Settingv1 = JsonConvert.DeserializeObject<Setting>(data);
            }
            else
            {
                File.WriteAllText(localfolder + "\\" + "Settings.json", "{}");
                ResetSettings();
                SaveSettings();
            }
        }

        public static Watchlist MyWatchlist ; 

        public static void laodWatchlist()
        {

            try {  
            Debug.WriteLine(localfolder + "\\" + "Watchlist.json");
            if (File.Exists(localfolder + "\\" + "Watchlist.json"))
            {

                string data = File.ReadAllText(localfolder + "\\" + "Watchlist.json");
                MyWatchlist = JsonConvert.DeserializeObject<Watchlist>(data);
            }
            else
            {
                File.WriteAllText(localfolder + "\\" + "Watchlist.json", "{\"Serien\":[]}");

            }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }
        public static void saveWatchlist()
        {
            
            string data = JsonConvert.SerializeObject(MyWatchlist);
            File.WriteAllText(localfolder + "\\" + "Watchlist.json", data);
        }

        public static void ResetSettings()
        {
            Settingv1 = new Setting();

            Settingv1.ssid = "";

            Settingv1.SelectedGenre = new List<string>();

            Settingv1.autoplay = false;
           

            SaveSettings();
        }
        public class Setting
        {

            public bool autoplay;
            public List<string> SelectedGenre { get; set; }
            public string ssid { get; set; }



        }

        public class Watchlist {
            
           public List<Serie> Serien { get; set; }

}
        public static string getBeetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        public static void SaveSettings()
        {

            string data = JsonConvert.SerializeObject(Settingv1);
            File.WriteAllText(localfolder + "\\" + "Settings.json", data);
            Debug.WriteLine(data);

        }
        public static async Task<string> loadsite(string url)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Cookie", Settingv1.ssid);
   

            var response = await client.GetAsync(url);

            var txt = await response.Content.ReadAsStringAsync();
         
            return txt;
        }









    }
}
