using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Threading;
using System.Diagnostics;

namespace CustomMediaTransportControls2
{
    public sealed class CustomMediaTransportControls2 : MediaTransportControls
    {
        private static int time = 0;
        // public event EventHandler<EventArgs> Liked;
        public event EventHandler<EventArgs> Skipforward;
        public event EventHandler<EventArgs> Backbtn;
        public event EventHandler<EventArgs> Nextbtn;
        static TextBlock title;
        static ComboBox Hostercombo;
        static ComboBox Langcombo;
        public string Title
        {
            set
            {

                title.Text = value;
            }


        }

        public ComboBox LangComboBox
        {
            get
            {
                return Langcombo;
            }
        }

        public ComboBox HosterComboBox
        {
            get
            {
                return Hostercombo;
            }
        }
        public CustomMediaTransportControls2()
        {
            this.DefaultStyleKey = typeof(CustomMediaTransportControls2);

        }
        Grid top;

        protected override void OnApplyTemplate()
        {
            // This is where you would get your custom button and create an event handler for its click method.
            title = GetTemplateChild("TitleTop") as TextBlock;
            //   Button likeButton = GetTemplateChild("LikeButton") as Button;
            Button skipforward = GetTemplateChild("SkipForwardButton") as Button;
            Button backbtn = GetTemplateChild("Backtbn") as Button;
            Button nextbtn = GetTemplateChild("NextTrackButton") as Button;
            nextbtn.Click += Nextbtn_Click;
            top = GetTemplateChild("TopGrid") as Grid;
            Langcombo = GetTemplateChild("LangCombo") as ComboBox;
            Hostercombo = GetTemplateChild("HosterCombo") as ComboBox;
            //   likeButton.Click += LikeButton_Click;
            skipforward.Click += Skipforward_Click;
            backbtn.Click += Backbtn_Click;
            base.OnApplyTemplate();
            Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;

            new Thread(() => { Starttimer(); }).Start();

        }

        private void Nextbtn_Click(object sender, RoutedEventArgs e)
        {
            Nextbtn?.Invoke(this, EventArgs.Empty);
        }

        private void CoreWindow_PointerMoved(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            time = 0;
            
            //set title stack visible
            if (top != null)
                top.Visibility = Visibility.Visible;
            //set pointer visible
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            Backbtn?.Invoke(this, EventArgs.Empty);
        }

        private void MainPage_PointerMovedAsync(object sender, PointerRoutedEventArgs e)
        {

        }

        public async void Starttimer()
        {
          
            Thread.Sleep(1000);
            time++;
            //    Window.Current.CoreWindow.PointerCursor = null;

            if (time > 3)
            {


                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {

                    top.Visibility = Visibility.Collapsed;
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = null;
                });


            }

            Starttimer();
         

        }
        private void Skipforward_Click(object sender, RoutedEventArgs e)
        {
            Skipforward?.Invoke(this, EventArgs.Empty);
        }

        /*  private void LikeButton_Click(object sender, RoutedEventArgs e)
          {
              // Raise an event on the custom control when 'like' is clicked
              Liked?.Invoke(this, EventArgs.Empty);
          }*/
    }
}
