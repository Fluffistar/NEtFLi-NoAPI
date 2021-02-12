using S.toNoApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TheMovieDB;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace NEtFLi
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        

        public MainPage()
        {
            this.InitializeComponent();

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
         
            Verwaltung.laodSettings();
            if (SetupNOAPI.Serien.Count == 0)
                await SetupNOAPI.Load();
            if (Verwaltung.Settingv1.ssid == "")
            {
                content.Navigate(typeof(Login));
                content.Visibility = Visibility.Visible;
            }
            else
            {
              
                navbar.SelectedItem = navbar.MenuItems[1];
                home();
            }
            navbar.SelectionChanged += Navbar_SelectionChanged;
        }

        void create_gridview(List<Serie> serien, string header )
        {
            int width = 175;
            int height = 250;
            if (!header.Contains("Watch"))  
            Parallel.ForEach<Serie>(serien, s =>
            {
                s.load();

            });
            GridView gridView = new GridView { Name = header, Height = height, Header = new TextBlock { TextWrapping = TextWrapping.Wrap, Text = header + ":", Width = 190, FontSize = 26 }, Style = (Style)Resources["Scrollbar"] };

            
            foreach (Serie s in serien)
            {
                Grid item = new Grid { Width = width, Height = height, Background = new SolidColorBrush(Colors.Transparent) };
                Image img = new Image { Width = width, Height = height, Stretch = Stretch.Fill, Margin = new Thickness(0, 0, 0, 0), VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
                 
                img.Source = new BitmapImage(new Uri(s.poster_path));
                //add new if its in new episodes

                TextBlock text = new TextBlock { Text = s.Title.Length >= 18 ? s.Title.Substring(0, 15) + "..." : s.Title, FontSize = 18, Foreground = new SolidColorBrush(Colors.White), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center };
                Border border = new Border { Background = Verwaltung.GetSolidColorBrush("#FF1C1C1C"), Opacity = 0.75, Width = width, Height = 25, Margin = new Thickness(0, width, 0, 0), VerticalAlignment = VerticalAlignment.Bottom };

                item.Tag = s.Title;
                border.Child = text;
                item.Children.Add(img);
                item.Children.Add(border);
                /*      if (New_EpisodeList.FindAll(x => x.series == s.id).Count > 0)
                      {
                          TextBlock newtext = new TextBlock { FontSize = 18, Foreground = GetSolidColorBrush("#FFE22B22"), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, };
                          newtext.Inlines.Add(new Run { Text = "NEW !", FontWeight = FontWeights.Bold });
                          Border newborder = new Border { Background = GetSolidColorBrush("#FF1C1C1C"), Opacity = 0.9, Width = 60, Height = 25, Margin = new Thickness(10, 10, 10, 10), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
                          newborder.Child = newtext;
                          item.Children.Add(newborder);
                      }*/
                if (header.Contains("Watch"))
                {
                    item.Tapped += Item_TappedAsyncWatch;
                    item.DoubleTapped += Item_DoubleTappedWatch;
                }else
                {
                    item.Tapped += Item_TappedAsync;
                    item.DoubleTapped += Item_DoubleTapped;
                }
                // item.Tapped += Button_Click;
                gridView.Items.Add(item);
            }


            SerienList.Children.Add(gridView);

        }

        private void Item_TappedAsyncWatch(object sender, TappedRoutedEventArgs e)
        {
            string title = ((Grid)sender).Tag.ToString();
            bool tv = false;
            Serie serie = Verwaltung.MyWatchlist.Serien.Find(x => x.Title == title);
            try
            {
                tv = serie.getTV();
            }
            catch (Exception ex)
            {

            }
            info.Visibility = Visibility.Visible;
            Title.Text = serie.Title;
            //     try
            //    {
            imginfo.Source = new BitmapImage(new Uri(tv ? TMDB.ImgPath + serie.TV.backdrop_path : ""));

            DateTime date = DateTime.Today;
            DateTime.TryParse(serie.TV.last_air_date, out date);
            int dend = 0;
            if (date != DateTime.Today)
                dend = date.Year;

            Description.Text = "Produced: (" + serie.productionStart + " - " + dend + ") " + "NextEpisode Release: " + ((tv && serie.TV.next_episode_to_air != null && serie.TV.next_episode_to_air.air_date != null) ? Verwaltung.NEpis(serie.TV.next_episode_to_air.air_date) : "No DATA") + "\n\n" + (tv && serie.TV.overview != null && serie.TV.overview != "" ? serie.TV.overview : "No info");

        }

        private void Item_DoubleTappedWatch(object sender, DoubleTappedRoutedEventArgs e)
        {
            string title = ((Grid)sender).Tag.ToString();
            Serie serie = Verwaltung.MyWatchlist.Serien.Find(x => x.Title == title);
           
            Frame.Navigate(typeof(SerienPage), serie);
        }

        void home()
        {
            SerienList.Children.Clear();
            autobox.Visibility = Visibility.Collapsed;
        if (Verwaltung.MyWatchlist != null)
            create_gridview(Verwaltung.MyWatchlist.Serien.OrderByDescending(x => x.DateTime).ToList(), "   Watchlist");
             
            create_gridview(SetupNOAPI.Beliebt, "   Beliebt");
 
            create_gridview(SetupNOAPI.Neu, "   Neu");


            foreach (string s in Verwaltung.Settingv1.SelectedGenre)
            {
                create_gridview(SetupNOAPI.Serien.FindAll(x => x.Genre == s ).Take(30).ToList(), $"   {s}");
            }

        }
      
        private void Navbar_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (Verwaltung.Settingv1.ssid != "")
            {
                FrameNavigationOptions navOptions = new FrameNavigationOptions();
                navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
                content.Visibility = Visibility.Collapsed;
                if (args.IsSettingsSelected)
                {
                    content.Navigate(typeof(Settings));
                    content.Visibility = Visibility.Visible;
                }
                else
                {
                    var selectedItem = (NavigationViewItem)args.SelectedItem;
                    string selectedItemTag = ((string)selectedItem.Tag);
                    info.Visibility = Visibility.Collapsed;
                    autobox.Visibility = Visibility.Collapsed;




                    switch (selectedItemTag)
                    {
                        case "HomePage":
                            home();
                            break;
                       
                        case "SearchPage":
                            search();
                            break;
                     

                    }

 


                }
            }
           
               
        }
        void create_gridview(List<Serie> serien)
        {
            int width = 175;
            int height = 250;
            Parallel.ForEach<Serie>(serien, s =>
            {
                s.load();

            });
            GridView gridView = new GridView { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch /*, Style = (Style)Resources["Scrollbar"] */};


            foreach (Serie s in serien)
            {
                Image img = new Image { Width = width, Height = height, Stretch = Stretch.Fill, Margin = new Thickness(0, 0, 0, 0), VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };

                img.Source = new BitmapImage(new Uri(s.poster_path));
                TextBlock text = new TextBlock { Text = s.Title.Length >= 18 ? s.Title.Substring(0, 15) + "..." : s.Title, FontSize = 18, Foreground = new SolidColorBrush(Colors.White), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center };
                Border border = new Border { Background = Verwaltung.GetSolidColorBrush("#FF1C1C1C"), Opacity = 0.75, Width = width, Height = 25, Margin = new Thickness(0, width, 0, 0), VerticalAlignment = VerticalAlignment.Bottom };
                Grid item = new Grid { Width = width, Height = height, Background = new SolidColorBrush(Colors.Transparent) };
                item.Tag = s.Title;
                border.Child = text;
                item.Children.Add(img);
                item.Children.Add(border);
             /*   if (New_EpisodeList.FindAll(x => x.series == s.id).Count > 0)
                {
                    TextBlock newtext = new TextBlock { FontSize = 18, Foreground = GetSolidColorBrush("#FFE22B22"), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, };
                    newtext.Inlines.Add(new Run { Text = "NEW !", FontWeight = FontWeights.Bold });
                    Border newborder = new Border { Background = GetSolidColorBrush("#FF1C1C1C"), Opacity = 0.9, Width = 60, Height = 25, Margin = new Thickness(10, 10, 10, 10), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
                    newborder.Child = newtext;
                    item.Children.Add(newborder);
                }*/
                item.Tapped += Item_TappedAsync;
                item.DoubleTapped += Item_DoubleTapped;
                // item.Tapped += Button_Click;
                gridView.Items.Add(item);
            }


            SerienList.Children.Add(gridView);

        }
        private void Item_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            string id = ((Grid)sender).Tag.ToString();


            output(id);
        }

        async void output(string id)
        {

            Serie serie = SetupNOAPI.Serien.Find(x => x.Title == id);
            serie.loadSeason();
            Frame.Navigate(typeof(SerienPage), serie);
        }
        
        private async void Item_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            string title = ((Grid)sender).Tag.ToString();
            bool tv = false; 
            Serie serie = SetupNOAPI.Serien.Find(x => x.Title == title);
            try
            {
               tv =  serie.getTV();
            }catch(Exception ex)
            {

            }
            info.Visibility = Visibility.Visible;
            Title.Text = serie.Title;
       //     try
        //    {
                imginfo.Source = new BitmapImage(new Uri(tv ? TMDB.ImgPath + serie.TV.backdrop_path : ""));

            DateTime date = DateTime.Today;
            DateTime.TryParse(serie.TV.last_air_date, out date);
            int dend = 0;
            if (date != DateTime.Today)
                dend = date.Year;

                Description.Text = "Produced: (" +   serie.productionStart + " - " +  dend  + ") " + "NextEpisode Release: " + ((tv && serie.TV.next_episode_to_air != null && serie.TV.next_episode_to_air.air_date != null) ? Verwaltung.NEpis(serie.TV.next_episode_to_air.air_date) :  "No DATA") + "\n\n" + (tv && serie.TV.overview != null && serie.TV.overview != "" ? serie.TV.overview : "No info");
           // }catch(Exception ex) {

             //   imginfo.Source = new BitmapImage(new Uri("https://marketingland.com/wp-content/ml-loads/2015/08/movie-film-video-production-ss-1920.jpg"));
        //        Description.Text = "";
           // }
            /*         switch (serie.series.fsk)
                     {
                         case 0:
                             Fsk.Source = new BitmapImage(new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/1/17/FSK_0.svg/2000px-FSK_0.svg.png"));
                             break;
                         case 6:
                             Fsk.Source = new BitmapImage(new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/7/7b/FSK_6.svg/2000px-FSK_6.svg.png"));
                             break;
                         case 12:
                             Fsk.Source = new BitmapImage(new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/6/6e/FSK_12.svg/2000px-FSK_12.svg.png"));
                             break;
                         case 16:
                             Fsk.Source = new BitmapImage(new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/3/30/FSK_16.svg/2000px-FSK_16.svg.png"));
                             break;
                         case 18:
                             Fsk.Source = new BitmapImage(new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/5/5d/FSK_18.svg/2000px-FSK_18.svg.png"));
                             break;

                     }*/
        }
        void search()
        {
            autobox.Visibility = Visibility.Visible;
            SerienList.Children.Clear();

        }

     

        private void autobox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (autobox.Text.Length >= 3)
            {
                SerienList.Children.Clear();
                autobox.Text = sender.Text;
                create_gridview(SetupNOAPI.Serien.FindAll(x => x.Title.ToLower().Contains(autobox.Text.ToLower())).Take(25).ToList());
            }
            else
            {
                
            }
        }

        private void autobox_TextChanged_1(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {

                sender.ItemsSource = SetupNOAPI.Serien.FindAll(x => x.Title.ToLower().Contains(autobox.Text.ToLower())).Take(25).ToList();
            }
        }

        private void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedItem = (NavigationViewItem)sender;
            string selectedItemTag = ((string)selectedItem.Tag);
            switch (selectedItemTag)
            {
                case "AccountPage":
                    content.Navigate(typeof(Login));
                    content.Visibility = Visibility.Visible;
                    break;

            }
        }
    }
}
