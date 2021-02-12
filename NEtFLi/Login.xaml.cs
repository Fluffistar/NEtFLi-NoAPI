using S.toNoApi;
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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace NEtFLi
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
            if (Verwaltung.Settingv1.ssid != "")
            {  
                logged.Visibility = Visibility.Visible;
            }
        }

        private async void loginbtn_Click(object sender, RoutedEventArgs e)
        {
            if ( await Verwaltung.login(email.Text, password.Password))
            {
                logged.Visibility = Visibility.Visible;
            }
            else
            {
                info.Text = "Wrong Email or Password";
            }

        }

        private void restlog_Click(object sender, RoutedEventArgs e)
        {
            Verwaltung.Settingv1.ssid = "";
            Verwaltung.SaveSettings();
            logged.Visibility = Visibility.Collapsed;
        }
    }
}
