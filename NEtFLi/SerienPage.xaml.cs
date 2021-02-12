using Netfli;
using S.toNoApi;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using S.toNoApi.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static S.toNoApi.Serie;
using Windows.UI.Xaml.Documents;
using System.Diagnostics;
using TheMovieDB;
using Windows.UI.Text;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace NEtFLi
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SerienPage : Page
    {
        public SerienPage()
        {
            this.InitializeComponent();
        }
        Serie serie = null;
        public async void back()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                Frame.Navigate(typeof(MainPage));
            });
        }

        private void layout_ImageOpened(object sender, RoutedEventArgs e)
        {
            this.BlurThisUI(background, layout);
        }

        private void backbtn_Click(object sender, RoutedEventArgs e)
        {
            back();
        }

        private void SeasonsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSeason();
        }
        private async void BlurThisUI(UIElement sourceElement, Image outputImage)
        {
            using (var stream = await sourceElement.RenderToRandomAccessStream())
            {
                var device = new CanvasDevice();
                var bitmap = await CanvasBitmap.LoadAsync(device, stream);

                var renderer = new CanvasRenderTarget(device,
                                                      bitmap.SizeInPixels.Width,
                                                      bitmap.SizeInPixels.Height,
                                                      bitmap.Dpi);

                using (var ds = renderer.CreateDrawingSession())
                {
                    var blur = new GaussianBlurEffect();
                    blur.BlurAmount = 5.0f;
                    blur.Source = bitmap;
                    ds.DrawImage(blur);
                }

                stream.Seek(0);
                await renderer.SaveAsync(stream, CanvasBitmapFileFormat.Png);

                BitmapImage image = new BitmapImage();
                image.SetSource(stream);
                outputImage.Source = image;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            serie = (Serie)e.Parameter; // get parametery<  
            Setup();
        }
        private void EpisodenListe_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            serie.lastseason = SeasonsCombo.SelectedIndex;
            serie.lastepisode = EpisodenListe.SelectedIndex;

            Frame.Navigate(typeof(VideoPlayer), serie);
        }
       
        void Setup()
        {
            Title.Text = serie.Title;
            bool tv = false;
            backbtn.Content = new FontIcon { FontFamily = new FontFamily("Segoe MDL2 Assets"), Foreground = new SolidColorBrush(Colors.White), Glyph = "\uE72B" };
            //serie.loadSeason();
            background.Source = new BitmapImage(new Uri(serie.TV  != null ? S.toNoApi.Serializer.TMDB.ImgPath + serie.TV.backdrop_path : "" ));
            layout.Source = background.Source;
            try
            {

                TheMovieDB.TMDB.GetTvEpisodes(serie.TV);
            }
            catch { }
            DateTime date = DateTime.Today;
            DateTime.TryParse(serie.TV.last_air_date, out date);
            int dend = 0;
            if (date != DateTime.Today)
                dend = date.Year;
            tv = serie.TV != null ? true : false;
            Description.Text = "Produced: (" + serie.productionStart + " - " + dend + ") " + "NextEpisode Release: " + ((tv && serie.TV.next_episode_to_air != null && serie.TV.next_episode_to_air.air_date != null) ? Verwaltung.NEpis(serie.TV.next_episode_to_air.air_date) : "No DATA") + "\n\n" + (tv && serie.TV.overview != null && serie.TV.overview != "" ? serie.TV.overview : "No info");
            foreach (SeasonSTO seasons in serie.Seasons)
            {
                SeasonsCombo.Items.Add(new ComboBoxItem { Content = seasons.name, Tag = seasons.Season });

            }
            
             SeasonsCombo.SelectedIndex = serie.lastseason   ;




            SelectedSeason();


            try
            {
                 EpisodenListe.SelectedIndex = serie.lastepisode  ;
                EpisodenListe.ScrollIntoView(EpisodenListe.SelectedItem);
            }
            catch
            {
                EpisodenListe.SelectedIndex = 0;
            }
            loading.Visibility = Visibility.Collapsed;
            EpisodenListe.Loaded += (s, e) =>
    EpisodenListe.ScrollIntoView(EpisodenListe.SelectedItem);


           
        }
        void SelectedSeason()
        {
            string data = "";
            EpisodenListe.Items.Clear();
                if(serie.Seasons[SeasonsCombo.SelectedIndex].Episodes == null)
                    serie.Seasons[SeasonsCombo.SelectedIndex].load();
            /*      try
                  {
                    data  = MakeAsyncRequest(tmdbbase + tmdbserie + "/season/" + (SeasonsCombo.SelectedIndex + 1), "text/html").Result;
                      //    Console.WriteLine(data);

                  }
                  catch
                  {
                      data = "";
                  }*/
            Season actual = null;
            int i = 0;
            int index = serie.Seasons[SeasonsCombo.SelectedIndex].Season;
            Debug.WriteLine("Index: " + index);

           // List<new_episode> new_s = New_EpisodeList.FindAll(x => x.series == serie.series.id && x.season == index);
          /*  Debug.WriteLine("new episodes: " + new_s.Count);
            Debug.WriteLine(getBetween(data, "<div class=\"episode_list\">", "<div class=\"episode_navigation\">"));
            string[] block = getBetween(data, "<div class=\"episode_list\">", "<div class=\"episode_navigation\">").Split(new string[] { "<div class=\"card\">" }, StringSplitOptions.None);*/
            if (serie.TV != null)
                actual = serie.TV.seasons.Find(x => x.season_number == index);


            if (actual != null)
                Debug.WriteLine("Index: " + actual.season_number);
            foreach (EpisodeSTO episodes in serie.Seasons[SeasonsCombo.SelectedIndex].Episodes)
            {
                StackPanel mainstack = new StackPanel { Orientation = Orientation.Vertical };
                StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Padding = new Thickness(0, 0, 10, 0), Spacing = 10 };

                Image img = new Image { Width = 300, Height = 150, Stretch = Stretch.UniformToFill, Margin = new Thickness(0, 0, 0, 0), VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
                TextBlock text;

                if (actual != null)
                {
                    Grid grid = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    img.Source = new BitmapImage(new Uri(S.toNoApi.Serializer.TMDB.ImgPath + actual.episodes[i].still_path));
                    text = new TextBlock { Text = (i + 1) + ". " + actual.episodes[i].name, FontSize = 24, TextWrapping = TextWrapping.Wrap, Width = EpisodenListe.Width - 350, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right };
                    text.Inlines.Add(new Run { Text = "\n" + actual.episodes[i].overview, FontSize = 18 });
                    stackPanel.Children.Add(text);
                    grid.Children.Add(img);
                    /*   if (new_s.Exists(x => x.episode == i + 1))
                       {
                           Debug.WriteLine("ADD NEW");
                           TextBlock newtext = new TextBlock { FontSize = 18, Foreground = GetSolidColorBrush("#FFE22B22"), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, };
                           newtext.Inlines.Add(new Run { Text = "NEW !", FontWeight = FontWeights.Bold });
                           Border newborder = new Border { Background = GetSolidColorBrush("#FF1C1C1C"), Opacity = 0.9, Width = 60, Height = 25, Margin = new Thickness(10, 10, 10, 10), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
                           newborder.Child = newtext;
                           grid.Children.Add(newborder);
                       }*/
                      if (serie.WatchEpisodes != null && serie.WatchEpisodes.Exists(x => x.Season + 1 == episodes.Season && x.Episode + 1 == episodes.Episode && x.watched))
                           {
                           Debug.WriteLine("ADD Watched");
                           TextBlock newtext = new TextBlock { FontSize = 18, Foreground = Verwaltung.GetSolidColorBrush("#FF6AE416"), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, };
                           newtext.Inlines.Add(new Run { Text = "Watched !", FontWeight = FontWeights.Bold });
                           Border newborder = new Border { Background = Verwaltung.GetSolidColorBrush("#FF1C1C1C"), Opacity = 0.9, Width = 90, Height = 30, Margin = new Thickness(10, 10, 10, 10), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
                           newborder.Child = newtext;
                           grid.Children.Add(newborder);
                       } 
                       stackPanel.Children.Add(grid);
                   }
                   else
                   {
                       Grid grid = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                       img.Source = new BitmapImage(new Uri(serie.TV != null ? S.toNoApi.Serializer.TMDB.ImgPath + serie.TV.backdrop_path : ""));
                       text = new TextBlock { Text = (i + 1) + ". " + episodes.german != "" ? episodes.german + $" ({episodes.english})" : episodes.english, FontSize = 24, TextWrapping = TextWrapping.Wrap, Width = EpisodenListe.Width - 350, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right };
                       text.Inlines.Add(new Run { Text = "\n" + $"German Hoster {episodes.German.Count} \nEnglish Hoster {episodes.English.Count} \nSub Hoster {episodes.Sub.Count}", FontSize = 18 });
                       stackPanel.Children.Add(text);
                       grid.Children.Add(img);
                    /*   if (new_s.Exists(x => x.episode == i + 1))
                       {
                           Debug.WriteLine("ADD NEW");
                           TextBlock newtext = new TextBlock { FontSize = 18, Foreground = GetSolidColorBrush("#FFE22B22"), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, };
                           newtext.Inlines.Add(new Run { Text = "NEW !", FontWeight = FontWeights.Bold });
                           Border newborder = new Border { Background = GetSolidColorBrush("#FF1C1C1C"), Opacity = 0.9, Width = 60, Height = 25, Margin = new Thickness(10, 10, 10, 10), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
                           newborder.Child = newtext;
                           grid.Children.Add(newborder);
                       }*/
                    if (serie.WatchEpisodes != null && serie.WatchEpisodes.Exists(x => x.Season+1 == episodes.Season && x.Episode+1 == episodes.Episode && x.watched) )
                        {
                        Debug.WriteLine("ADD Watched");
                        TextBlock newtext = new TextBlock { FontSize = 18, Foreground = Verwaltung.GetSolidColorBrush("#FF6AE416"), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, };
                        newtext.Inlines.Add(new Run { Text = "Watched !", FontWeight = FontWeights.Bold });
                        Border newborder = new Border { Background = Verwaltung.GetSolidColorBrush("#FF1C1C1C"), Opacity = 0.9, Width = 90, Height = 30, Margin = new Thickness(10, 10, 10, 10), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
                        newborder.Child = newtext;
                        grid.Children.Add(newborder);
                    } 
                    stackPanel.Children.Add(grid);

                }



                    mainstack.Children.Add(stackPanel);
                
                    if (serie.WatchEpisodes != null && serie.WatchEpisodes.Exists(x => x.Season+1 == episodes.Season && x.Episode+1 == episodes.Episode && x.timespan != 0))
                    {
                        var timevalues = serie.WatchEpisodes.Find(x => x.Season+1 == episodes.Season && x.Episode+1 == episodes.Episode);

                        ProgressBar progress = new ProgressBar { Width = EpisodenListe.Width, Value = (timevalues.timespan / timevalues.max) * 100, Foreground = new SolidColorBrush(Colors.Orange) };
                        mainstack.Children.Add(progress);
                    }
                
                EpisodenListe.Items.Add(mainstack);
                i++;
            }
        }
            private void EpisodenListe_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
