using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CustomMediaTransportControls2;
using S.toNoApi;
using Windows.Media.Playback;
using static S.toNoApi.Serie;
using Windows.UI;
using Windows.Media.Core;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Diagnostics;
// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace NEtFLi
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class VideoPlayer : Page
    {
        Serie _serie;
       // CustomMediaTransportControls2.CustomMediaTransportControls2 controller = null;
        public int SelectedEpisodes { get { return _serie.lastepisode; } set { _serie.lastepisode = value; } }
        public int SelectedSeason { get { return _serie.lastseason; } set { _serie.lastseason = value; } }
        EpisodeSTO SelectedEpisode => _serie.Seasons[SelectedSeason].Episodes[SelectedEpisodes];
   

        public VideoPlayer()
        {
            this.InitializeComponent();

        }
 

        private void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             
                ComboBoxItem sender2 = controller.LangComboBox.SelectedItem as ComboBoxItem;

                switch (sender2.Tag.ToString())
                {
                    case "1":
                        SelectedLang = SelectedEpisode.German;
                        break;
                    case "2":
                        SelectedLang = SelectedEpisode.English;
                        break;
                    case "3":
                        SelectedLang = SelectedEpisode.Sub;
                        break;
                }
                controller.HosterComboBox.Items.Clear();
                foreach (Hoster h in SelectedLang)
                {
                    controller.HosterComboBox.Items.Add(item(h.Hostername, $"{h.Hostername}${h.link}"));
                }
            controller.HosterComboBox.SelectedIndex = 0 ;
        }

        private  async void HosterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (controller.HosterComboBox.Items.Count > 0)
            {
                string url = "";
                var sender2 = controller.HosterComboBox.SelectedItem as ComboBoxItem;
                string[] data = sender2.Tag.ToString().Split('$');
                string xxx = await Verwaltung.loadsite(data[1]);
                switch (data[0])
                {
                    case "Vivo":
                        url = SetupNOAPI.vivo(xxx);
                        break;
                    case "VOE":
                        url = SetupNOAPI.voe(xxx);
                        break;
                    case "Vidoza":
                        url = SetupNOAPI.vidoza(xxx);
                        break;
                    case "Streamtape":
                        url = SetupNOAPI.streamtape(xxx);
                        break;
                }
                if (url != "")
                {
                    mediaplayer.Source = MediaSource.CreateFromUri(new Uri(url));
                    mediaplayer.MediaPlayer.Play();
                    Verwaltung.addWatch(_serie);
                
                }
                else
                {
                    var dialog = new MessageDialog("Hoster Failed");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Debug.WriteLine(_serie.WatchEpisodes != null && _serie.WatchEpisodes.Exists(x => x.Season == SelectedSeason && x.Episode == SelectedEpisodes && x.watched == false && x.timespan != 0));
          
            if (_serie.WatchEpisodes != null && _serie.WatchEpisodes.Exists(x => x.Season  == SelectedSeason && x.Episode  == SelectedEpisodes && x.watched == false && x.timespan != 0))
            {
                var data = _serie.WatchEpisodes.Find(x => x.Season == SelectedSeason && x.Episode == SelectedEpisodes && x.watched == false && x.timespan != 0);

                mediaplayer.MediaPlayer.PlaybackSession.Position = new TimeSpan(0, 0, 0, (int)data.timespan);
            }
            });
        }

        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
           
            next();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            mediaplayer.MediaPlayer.Source = null;
        }
        private void controller_Skipforward(object sender, EventArgs e)
        {
            mediaplayer.MediaPlayer.PlaybackSession.Position -= new TimeSpan(0, 0, 0, 0, 20000);
            mediaplayer.MediaPlayer.Play();

        }
          List<Hoster> SelectedLang = null;
        void load()
        {
            string title = SelectedEpisode.german;


            title = title != "" ? title : SelectedEpisode.english;
            controller.Title = title.Length >= 43 ? title.Substring(0, 40) + "..." : title;
            SelectedEpisode.load();
          
      
            controller.LangComboBox.Items.Clear();
            if (SelectedEpisode.German.Count > 0)
                controller.LangComboBox.Items.Add(item("German" ,"1"));
            if (SelectedEpisode.English.Count > 0)
                controller.LangComboBox.Items.Add(item("English", "2"));
            if (SelectedEpisode.Sub.Count > 0)
                controller.LangComboBox.Items.Add(item("German Sub", "3"));
            controller.LangComboBox.SelectedIndex = 0;
           
        }

        ComboBoxItem item(string content , string tag)
        {
             
            ComboBoxItem hoste = new ComboBoxItem { Content = content, Tag = tag };
            hoste.Background = Verwaltung.GetSolidColorBrush("#FF1C1C1C");
            hoste.Foreground = new SolidColorBrush(Colors.White);
            hoste.BorderBrush = Verwaltung.GetSolidColorBrush("#FF1C1C1C");
            hoste.FontSize = 24;
            return hoste;
        }

        


         
        private void controller_Backbtn(object sender, EventArgs e)
        {
          
            try
            {
                if (mediaplayer.MediaPlayer.PlaybackSession.Position.TotalSeconds / mediaplayer.MediaPlayer.PlaybackSession.NaturalDuration.TotalSeconds >= 0.8)
                {
                  _serie.Watched();


                    Verwaltung.addWatch(_serie);
                    
                }
                else
                {
                    _serie.WatchEpisodeAdd(mediaplayer.MediaPlayer.PlaybackSession.Position.TotalSeconds, mediaplayer.MediaPlayer.PlaybackSession.NaturalDuration.TotalSeconds);


                    Verwaltung.addWatch(_serie);
                }

            }
            catch (Exception ex)
            {

            }

            Frame.Navigate(typeof(SerienPage), _serie);
        }

       async void   error()
        {
            var dialog = new MessageDialog("An Error happend");
            await dialog.ShowAsync();
            Frame.Navigate(typeof(SerienPage), _serie);
        }
        private void controller_Nextbtn(object sender, EventArgs e)
        {
            //   Verwaltung.WatchEpisode(_serie.SeasonsList[SelectedSeason].episodes[SelectedEpisodes].id);
            //     lastTimeEpisodeWatcheds.RemoveAll(x => x.id == _serie.SeasonsList[SelectedSeason].episodes[SelectedEpisodes].id);
            //   saveEpsiodeStateList();
            //    loaded = false;

            next();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _serie = (Serie)e.Parameter; // get parameter
        
        }
        void next()
        {

            if (mediaplayer.MediaPlayer.PlaybackSession.Position.TotalSeconds / mediaplayer.MediaPlayer.PlaybackSession.NaturalDuration.TotalSeconds >= 0.8)
            {
                _serie.Watched();
                Verwaltung.addWatch(_serie);
            }
            if (SelectedEpisodes + 1 < _serie.Seasons[SelectedSeason].Episodes.Count)
            {
                SelectedEpisodes++;

                load();
            }
            else
            {
                if (SelectedSeason + 1 < _serie.Seasons.Count)
                {
                    if (_serie.Seasons[SelectedSeason + 1].name != "Special" || Verwaltung.Settingv1.autoplay)
                    {
                        SelectedSeason++;
                        SelectedEpisodes = 0;
                        load();
                    }
                    else
                        error();
                    //else do something else like Series finished or so 


                }
                else
                    error();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mediaplayer.SetMediaPlayer(new Windows.Media.Playback.MediaPlayer());
            mediaplayer.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaplayer.MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;


            controller.HosterComboBox.SelectionChanged += HosterComboBox_SelectionChanged;
            controller.LangComboBox.SelectionChanged += LangComboBox_SelectionChanged;
            load();
        }
    }
}
