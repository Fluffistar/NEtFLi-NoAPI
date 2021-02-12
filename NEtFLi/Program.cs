using Netfli;
using Newtonsoft.Json;
using S.toNoApi.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static S.toNoApi.Serie;

namespace S.toNoApi
{
    public class Serie
    {
        static string main = "https://s.to";
        string imdb_pattern = "href[^>]*>(.*?)IMDB</a>";
        public string poster_path  { get; set; } 
        public string url   { get; set; }
         public string Title   { get; set; }
        public TVResult TVResult   { get; set; } 
        public int lastepisode   { get; set; }
        public int lastseason   { get; set; }
        public TV TV { get; set; }
        public TheMovieDB.TVShow TVShow { get; set; }
        public int productionStart { get; set; }
        public string Genre { get; set; }
        public string imdb { get; set; }
        public List<SeasonSTO> Seasons { get; set; }

        public List<WatchEpisode> WatchEpisodes { get; set; } 

        public class WatchEpisode {

            public int Season { get; set; }

            public int Episode { get; set; }

            public double timespan { get; set; }

            public double max { get; set; }

            public bool watched { get; set; }

        }
        
        public void Watched()
        {
            if (WatchEpisodes == null)
                WatchEpisodes = new List<WatchEpisode>();
            int index = WatchEpisodes.FindIndex(x => x.Season == lastseason + 1 && x.Episode == lastepisode + 1);

            if (index != -1)
                WatchEpisodes.RemoveAt(index);
            WatchEpisodes.Add(new WatchEpisode { Season = lastseason, Episode = lastepisode, timespan = 0, max = 0 ,watched = true });
        }

        public void WatchEpisodeAdd(double time , double max)
        {
            if (WatchEpisodes == null )
                WatchEpisodes = new List<WatchEpisode>();
            int index = WatchEpisodes.FindIndex(x => x.Season == lastseason+1 && x.Episode == lastepisode + 1);
         
            if (index != -1)
                WatchEpisodes.RemoveAt(index);
            WatchEpisodes.Add(new WatchEpisode { Season = lastseason, Episode =  lastepisode, timespan = time , max = max });
        }
    public Serie()
        {

        }
    public Serie(Match match)
        {
            Title = getBeetween(match.Value, "anschauen\">", "</a>");
            url = getBeetween(match.Value, "href=\"", "\" title");


          

        }

       

    public void loadSeason()
        {
            Seasons = new List<SeasonSTO>();
            string season_pattern = "<a(.*?)href(.*?)</a>";
            string[] season_data = getBeetween(result.Replace("\n","") , "<div class=\"hosterSiteDirectNav\" id=\"stream\">", "<div class=\"cf\">").Split("<br>", StringSplitOptions.None);
            SeasonSTO last = null;
           foreach( Match match in Regex.Matches(season_data[0], season_pattern))
            {
                SeasonSTO season = new SeasonSTO(match,url);
                if (getBeetween(match.Value, "title=\"", "\">") != "Alle Filme")
                    Seasons.Add(season);
                else
                    last = season;
               
               

            }

            if (last != null)
                Seasons.Add(last);


        }
        public class SeasonSTO
        {
            public string name = "";

            public int Season = -1;
            public bool special = false ;
            string url = "";
            public List<EpisodeSTO> Episodes = new List<EpisodeSTO>();

            public SeasonSTO()
            {

            }
            public SeasonSTO(Match match , string _url)
            {
                url = _url;
                string title = getBeetween(match.Value, "title=\"", "\">");

                name = (title) != "Alle Filme" ? title.Replace("Staffel", "Season") : "Specials";

                if (name != "Specials")
                {
                    Season = int.Parse(getBeetween(match.Value, "\">", "</a>"));
                }
                else
                {
                    Season = 0;
                    special = true;
                }

                load();
            }

            public override string ToString()
            {
                return $"{name} Number: {Season} Episodes: {Episodes.Count} ";
            }
            string result = "";
            string season_pattern = "<a href(.*?)</a>";
            public void load()
            {
                result = new WebClient().DownloadString(main + url + ((Season == 0) ? "/filme" : $"/staffel-{Season}"));
                result = getBeetween(result.Replace("\n", ""), "<div class=\"hosterSiteDirectNav\" id=\"stream\">", "<div class=\"cf\">").Split("<br>", StringSplitOptions.None)[1];

            

                foreach (Match match in Regex.Matches(result.Split("<a href=\"http://discovernative.com",StringSplitOptions.None)[0], season_pattern))
                {
                    string data = getBeetween(match.Value, Season == 0 ? "film-" : "episode-", "\"> <" ); 
                    if(data != "")
                    if (!Episodes.Exists(x => x.Episode == int.Parse(data))) 
                    {
                    
                        Episodes.Add(new EpisodeSTO(match,Season));
                    }
                }
                
            }
        }


        public class EpisodeSTO
        {
            public int Episode = -1;
            public int Season = -1;
            public string german = "";
            public string english = "";
            public List<Hoster> German = new List<Hoster>();
            public List<Hoster> English = new List<Hoster>();
            public List<Hoster> Sub = new List<Hoster>();
            public string url = "";
            public EpisodeSTO()
            {

            }
            public EpisodeSTO(Match match , int _Season)
            {
                Season = _Season;
                if(Season !=0)
                    Episode = int.Parse(getBeetween(match.Value, "episode-", "\"> <"));
                else
                    Episode = int.Parse(getBeetween(match.Value, "film-", "\">"));
                url = getBeetween(match.Value, "href=\"", "\"> ");
                german = getBeetween(match.Value, "<strong>", "</strong>");
                english = getBeetween(match.Value, "<span>", "</span>");

               
            }

            string result = "";
            
            string hoster_pattern = "<li class=\"col-md-4 col-xs-12(.*?)</li>";
            public void load()
            {
                result = new WebClient().DownloadString(main+url);
                Debug.WriteLine(result);
                result = getBeetween(result.Replace("\n",""), "originalLinkTarget", "</ul");
                Debug.WriteLine("Mine");
                Debug.WriteLine(result);
                result = Regex.Replace(result, @"\t|\n|\r", "");
                foreach (Match match in Regex.Matches( result , hoster_pattern))
                {
                    // Console.WriteLine(match.Value);
                    if(Verwaltung.Hosters.Contains(getBeetween(match.Value, "<h4>", "</h4>")))
                        switch (int.Parse( getBeetween(match.Value, "data-lang-key=\"", "data-link-id").Replace("\"", "")))
                        {
                            case 1:
                                    German.Add(new Hoster(match));
                                break;
                            case 2:
                                English.Add(new Hoster(match));
                                break;
                            case 3:
                                Sub.Add(new Hoster(match));
                                break;
                        }
                        
                }
            }

            public override string ToString()
            {
                return $" {Episode}|{Season} {german} [{english}] German: {German.Count} Enlish: {English.Count} Subs: {Sub.Count}";
            }

        }

       public class Hoster
        {
            public string Hostername = "";
            public int Language = 0;
            public string link = "";
            public Hoster()
            {

            }
            public Hoster(Match match)
            {
                Hostername = getBeetween(match.Value,"<h4>","</h4>");
                string lang_string = getBeetween(match.Value, "data-lang-key=\"", "data-link-id").Replace("\"","");
                Language = int.Parse(lang_string);
                link =  main+ getBeetween(match.Value , "href=\"","\" target").Replace("\"", "");
            }

            public override string ToString()
            {
                return $"Hostername {Hostername} Language {Language} URL {link}";
            }
        }

        public Serie(Match match , string genre)
        {
            Title = getBeetween(match.Value, "anschauen\">", "</a>");
            url = getBeetween(match.Value, "href=\"", "\" title");

            Genre = getBeetween(genre, "<h3>", "</h3>");



        }
        public void loadTV()
        {

        }


        public async void load()
        {
            if (poster_path ==null|| poster_path =="") {
                imdb = getImdb();
                productionStart = getproductionstart();

                if (imdb != "")
                    TVResult = vResult(imdb);
                else {
                    try
                    {
                        TVShow = await TheMovieDB.TMDB.GetTVShow(this);
                        poster_path = TMDB.ImgPath + TVShow.poster_path;
                    }catch(Exception ex) {

                        poster_path = main + getBeetween(result, "<noscript><img src=\"", "\" alt=\"");
                        if(poster_path == "")
                            Debug.WriteLine(result);
                    }
                }
                if (poster_path == "https://image.tmdb.org/t/p/original/" || poster_path == null)
                    poster_path = main+ getBeetween(result, "<noscript><img src=\"", "\" alt=\"");
                if (poster_path == "")
                    Debug.WriteLine(result);
                Debug.WriteLine(this);
            }
        }

       int getproductionstart() {
            int p = 0;
            string pdate = Regex.Match(result, "<a href=\"https://s.to/serien/jahr/(.*?)>").Value.Replace("<a href=\"https://s.to/serien/jahr/", "");
            Debug.WriteLine(pdate);
            int.TryParse(pdate.Replace("\">", ""), out p);
        
            return p;
        
        }

       TVResult vResult(string imdb)
        {
            FindResult findResult = JsonConvert.DeserializeObject<FindResult>(new WebClient().DownloadString($"https://api.themoviedb.org/3/find/{imdb}?api_key={APIKEY.tmdb}&external_source=imdb_id&language=de"));
            if (findResult.tv_results.Count > 0)
            {
                poster_path = TMDB.ImgPath + findResult.tv_results[0].poster_path;
                return findResult.tv_results[0];
            }
            else
                return null;
        }
        string result = "";

        public DateTime DateTime { get;   set; }

        string getImdb()
        {
            result = new WebClient().DownloadString(main+url) ;
            var match = Regex.Match(result, imdb_pattern);

            string imdb_id = getBeetween(match.Value, "https://www.imdb.com/title/", "\" title");
             
            
            
            return imdb_id;
        }

        
        public bool getTV()
        {
           
           
             if(TVResult != null)
               TV = JsonConvert.DeserializeObject<TV>(new WebClient().DownloadString($"https://api.themoviedb.org/3/tv/{TVResult.id}?api_key={APIKEY.tmdb}&language=de"));
             else if(TVShow != null)
                TV =  JsonConvert.DeserializeObject<TV>(new WebClient().DownloadString($"https://api.themoviedb.org/3/tv/{TVShow.id}?api_key={APIKEY.tmdb}&language=de"));


            return TV != null ? true : false;
             
        }

        public override string ToString()
        {
            return $"{Title}";// Imdb {imdb} Poster {poster_path}"  ;
        }

        public static string getBeetween(string input , string start  , string end)
        {
            string pattern = $"(?<={start})(.*)(?={end})";
            var output = Regex.Match(input, pattern);

            return output.Value;
        }
    }
    static class SetupNOAPI
    {
        public static List<Serie> Serien = new List<Serie>();
        public static List<Serie> Beliebt = new List<Serie>();
          public static List<Serie> Neu = new List<Serie>();
        public static string all_pattern = "<a data-alternative-title=(.*?) href(.*?)</a>";
        public static async Task Load()
        {
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

         Verwaltung.laodWatchlist();

       

            string beliebt_pattern =  "(?<=<h3>)(.*)(?=<span)";
             

            string input = new WebClient().DownloadString("https://s.to/serien");
            input = input.Replace("\n", "").Split("id=\"seriesContainer\"", StringSplitOptions.None)[1];

            string input_beliebt = new WebClient().DownloadString("https://s.to/beliebte-serien");
            string input_neu = new WebClient().DownloadString("https://s.to/neu");


          /*  foreach (string match in input.Split("</ul>",StringSplitOptions.None))
            {
                string outs = match;
                Parallel.ForEach<Match>(Regex.Matches(outs, all_pattern), match2 =>
                {

                    Serien.Add(new Serie(match2, match));

                });
            }
          */

             
                 _ =       Parallel.ForEach<string>(input.Split("</ul>",StringSplitOptions.None), match =>
                    {
                   _=     Parallel.ForEach<Match>(Regex.Matches(match, all_pattern), match2 =>
                        {
                            Serie s = new Serie(match2, match);
                            Serien.Add(s);

                        });

                    }); 
           
               
            
            
           
         Parallel.ForEach<Match>(Regex.Matches(input_beliebt, beliebt_pattern), match =>
               {

                       Serie serie = Serien.Find(x => x.Title == match.Groups[1].Value);
                       if (serie != null)
                           Beliebt.Add(serie);


               });

             Parallel.ForEach<Match>(Regex.Matches(input_neu, beliebt_pattern), match =>
              {

                  Serie serie = Serien.Find(se => se.Title == match.Groups[1].Value);
                  if (serie != null)
                      Neu.Add(serie);


              });


        /*   Parallel.ForEach<Serie>(Beliebt, s =>
               {
                   s.load();
                    
               });

            Parallel.ForEach<Serie>(Neu, s =>
            {
                s.load();

            });*/

            stopwatch.Stop();
            TimeSpan stopwatchElapsed = stopwatch.Elapsed;
      /*      Console.WriteLine(Convert.ToInt32(stopwatchElapsed.TotalMilliseconds)/1000);
            Console.WriteLine(Serien.Count);
            int index = 0;
            foreach (Serie s in Beliebt)
            {

                Console.WriteLine($"{index}) {s}");
                index++;
            }
          var x =  Console.ReadLine();

            index = int.Parse(x);
            var selectd = Beliebt[index];
            selectd.loadSeason();
            index = 0 ;
            SeasonSTO selectedseason = null;
           foreach(SeasonSTO s in selectd.Seasons) {

                Console.WriteLine(s);
                index++;
            }
              x = Console.ReadLine();

            index = int.Parse(x);
            selectedseason = selectd.Seasons[index];
            foreach (EpisodeSTO episode in selectedseason.Episodes)
                Console.WriteLine(episode);
            x = Console.ReadLine();
            
            index = int.Parse(x);

            var selectdeepisode = selectedseason.Episodes[index];
            // selectdeepisode.load();
            selectdeepisode.load();
            Console.WriteLine(selectdeepisode);

            foreach (Hoster episode in selectdeepisode.German)
                Console.WriteLine(episode);
            x = Console.ReadLine();

            index = int.Parse(x);
           
            var hoster = selectdeepisode.German[index];

            string xxx = await Verwaltung.loadsite(hoster.link);
            //  xxx = Regex.Replace(xxx, @"\t|\n|\r", "");
            Debug.WriteLine(xxx);
            string url = "NULL";
            switch (hoster.Hostername)
            {
                case "Vivo":
                    url = vivo(xxx);
                    break;
                case "VOE":
                    url = voe(xxx);
                    break;
                case "Vidoza":
                    url = vidoza(xxx);
                    break;
                case "Streamtape":
                    url = streamtape(xxx);
                    break;
            }
            Console.WriteLine(url);*/
            
        }


        
        public static string voe(string html)
        {
            string videourl =  Verwaltung.getBeetween(html, "\"mp4\": ", ",").Replace("\"", "");

            Debug.WriteLine(videourl);

            return videourl.Trim();
        }
        public static string streamtape(string html)
        {

            string videourl = Verwaltung.getBeetween(html, "id=\"videolink\"", "iv>");
            Debug.WriteLine(videourl);
            videourl =  getBeetween(videourl, "//", "</d").Replace("amp;", "");
            Debug.WriteLine("https://" + videourl);
            // videourl = new WebClient().DownloadString("https://" + videourl);
            Debug.WriteLine(videourl);

            return "https://" + videourl.Trim();
        }
        public static string vidoza(string html)
        {
            string videourl = Verwaltung.getBeetween(html, "src: \"", "\",").Replace("\"", "");

            Debug.WriteLine(videourl);

            return videourl.Trim();
        }



        public static string  vivo(string html)
        {
            var input = html;
            Console.WriteLine(input);
            input = input.Replace("%5E", "-");
            input = input.Replace("i", ":");


            input = input.Replace("-", "/");

            // input = input.Replace("G%405", "vod");
            input = input.Replace("%5D", ".");
            input = input.Replace("%40", "o");
            input = input.Replace("%5C", "-");


            input = input.Replace("%3A", "i");
            input = input.Replace("%3F", "n");
            input = input.Replace("%3D", "l");


            input = input.Replace("%2A", "Y");
            input = input.Replace("%2B", "Z");
            input = input.Replace("%29", "X");
            input = input.Replace("%28", "W");
            input = input.Replace("%24", "S");
            input = input.Replace("%22", "Q");
            input = input.Replace("%21", "P");
            input = input.Replace("%26", "U");
            input = input.Replace("%3B", "j");
            input = input.Replace("%7D", "N");

            input = input.Replace("%27", "V");
            input = input.Replace("%3C", "k");
            input = input.Replace("%7E", "O");
            input = input.Replace("%7C", "M");
            input = input.Replace("%7B", "L");
            input = input.Replace("%25", "T");
            input = input.Replace("%3E", "m");
            input = input.Replace("%23", "R");
            input = Swap(input, "F", "u");
            input = input.Replace("%60", "1");

            //  input = input.Replace("6", "e");
            //  input = input.Replace("4", "c");
            //     input = input.Replace("5", "d");
            input = Swap(input, "I", "x");
            input = Swap(input, "D", "s");
            input = Swap(input, "C", "r");
            input = Swap(input, "A", "p");
            input = Swap(input, "K", "z");
            input = Swap(input, "y", "J");
            input = Swap(input, "B", "q");
            input = Swap(input, "_", "0");
            input = Swap(input, "E", "t");
            input = Swap(input, "2", "a");
            input = Swap(input, "H", "w");
            input = Swap(input, "G", "v");
            input = Swap(input, "b", "3");
            input = Swap(input, "d", "5");
            input = Swap(input, "e", "6");
            input = Swap(input, "f", "7");
            input = Swap(input, "g", "8");
            input = Swap(input, "h", "9");
            input = Swap(input, "c", "4");

            return input.Trim();
        }
        public static string Swap(string main, string a, string b)
        {
            string x = main;
            string p = "~";
            x = x.Replace(a, p);
            x = x.Replace(b, a);
            x = x.Replace(p, b);
            return x;
        }


        // play hoster with ssid

        // Create a Login simulation  with post s.to/login




    }

    
}
