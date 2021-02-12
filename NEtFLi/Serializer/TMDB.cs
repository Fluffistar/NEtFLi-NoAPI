
using Netfli;
using Newtonsoft.Json;
using S.toNoApi;
using S.toNoApi.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
 

namespace TheMovieDB
{
    static class TMDB
    {
        private class Page
        {
            public int page { get; set; }
            public int total_results { get; set; }
            public int total_pages { get; set; }
            public TVShowMin[] results { get; set; }
        }

        public static string ImgPath = "https://image.tmdb.org/t/p/original/";

        public static async Task<TVShow> GetTVShow(Serie s)
        {
            string src = s.Title.Length <= 34 ? s.Title : s.Title.Substring(0, 34);
            string Json = GetJson($"https://api.themoviedb.org/3/search/tv?api_key={APIKEY.tmdb}&query={src}&language=de");

            Page page = JsonConvert.DeserializeObject<Page>(Json);
            if (page.total_results > 1)
            {
                foreach (TVShowMin tv in page.results)
                {
                    int x;
                    try
                    {
                        // x = Int32.Parse(tv.first_air_date.Split('-')[0]);
                        x =DateTime.Parse(tv.first_air_date).Year;
                    }
                    catch
                    {
                        x = 0;
                    }
                    Debug.WriteLine($"{ x} :: {s.productionStart}");
                    if (x == s.productionStart  )
                    {

                        return JsonConvert.DeserializeObject<TVShow>(GetJson($"https://api.themoviedb.org/3/tv/{tv.id}?api_key={APIKEY.tmdb}&language=de"));
                    }

                }
                return JsonConvert.DeserializeObject<TVShow>(GetJson($"https://api.themoviedb.org/3/tv/{page.results[0].id}?api_key={APIKEY.tmdb}&language=de"));
            }
            else
                return JsonConvert.DeserializeObject<TVShow>(GetJson($"https://api.themoviedb.org/3/tv/{page.results[0].id}?api_key={APIKEY.tmdb}&language=de"));


        }
        public static void GetTvEpisodes(TVShow tv)
        {
            Parallel.ForEach<TvSeason>(tv.seasons, (season) => {


                string Json = GetJson($"https://api.themoviedb.org/3/tv/{tv.id}/season/{season.season_number}?api_key={APIKEY.tmdb}&language=de");
                Debug.WriteLine(Json);
                season.episodes = JsonConvert.DeserializeObject<TvSeason>(Json).episodes;

            });



        }
        public static void GetTvEpisodes(TV tv)
        {
            Parallel.ForEach<Season>(tv.seasons, (season) => {


                string Json = GetJson($"https://api.themoviedb.org/3/tv/{tv.id}/season/{season.season_number}?api_key={APIKEY.tmdb}&language=de");
                Debug.WriteLine(Json);
                season.episodes = JsonConvert.DeserializeObject<Season>(Json).episodes;

            });



        }
        static string GetJson(string url)
        {


            var jsonString = new WebClient().DownloadString(url); ;

            Debug.WriteLine(jsonString);
            return jsonString;
        }
    }




    public class TVShowMin
    {
        public string original_name { get; set; }
        public int[] genreids { get; set; }
        public string name { get; set; }
        public double popularity { get; set; }
        public string[] origin_country { get; set; }
        public int vote_count { get; set; }
        public string first_air_date { get; set; }
        public string backdrop_path { get; set; }
        public string original_language { get; set; }
        public int id { get; set; }
        public double vote_average { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
    }
    public class TVShow
    {
        public string backdrop_path { get; set; }
        // public  created_by { get; set; }
        public int[] episode_run_time { get; set; }
        public string first_air_date { get; set; }
        public string homepage { get; set; }
        public int id { get; set; }
        public bool in_production { get; set; }
        public string[] languages { get; set; }
        public string last_air_date { get; set; }
        public string name { get; set; }
        public TvEpisode next_episode_to_air { get; set; }
        public int number_of_seasons { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public List<TvSeason> seasons { get; set; }
        public string status { get; set; }
        public double vote_average { get; set; }
    }
    public class TvSeason
    {
        public string air_date { get; set; }
        public int episode_count { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public int season_number { get; set; }
        public List<TvEpisode> episodes { get; set; }

    }
    public class TvEpisode
    {
        public string overview { get; set; }
        public string air_date { get; set; }
        public int episode_number { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int season_number { get; set; }
        public string still_path { get; set; }
    }
}
