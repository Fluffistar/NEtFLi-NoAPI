using S.toNoApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
 
// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace NEtFLi
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();


            Setup();
        }

        void Setup()
        {
            GenreListView.Items.Clear();

            foreach (string genres in Verwaltung.linkname)
            {
                GenreListView.Items.Add(new ListViewItem { Content = new TextBlock { Text = genres, Foreground = new SolidColorBrush(Colors.White) }, Tag = genres, IsSelected = Verwaltung.Settingv1.SelectedGenre.Exists(x => x == genres) });

            }
            autoplaytoogle.IsOn = Verwaltung.Settingv1.autoplay;
        }

        private void showsublisttoogle_Toggled(object sender, RoutedEventArgs e)
        {

        }

        private void watchsynctoogle_Toggled(object sender, RoutedEventArgs e)
        {

        }

        private void autoplaytoogle_Toggled(object sender, RoutedEventArgs e)
        {
            Verwaltung.Settingv1.autoplay = autoplaytoogle.IsOn;
            Verwaltung.SaveSettings();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
        
            File.Delete(Verwaltung.localfolder + "\\" + "Settings.json");
            Verwaltung.ResetSettings();
            Verwaltung.SaveSettings();
            Verwaltung.Message("Reset Settings");
            Setup();
        }

        private void LanguageListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {

        }

        private void HostnameListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {

        }

        private void saveMygenres_Click(object sender, RoutedEventArgs e)
        {
            Verwaltung.Settingv1.SelectedGenre.Clear();
            foreach (ListViewItem l in GenreListView.Items)
            {
                string name = (l.Tag.ToString());
                

                if(l.IsSelected)
                 Verwaltung.Settingv1.SelectedGenre.Add(name) ;

            }
            Verwaltung.SaveSettings();
            Verwaltung.Message("Settings Updated");
        }
    }
}
