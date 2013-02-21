using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace ClickWhat
{
    public partial class PanoramaPage1 : PhoneApplicationPage
    {
        private ObservableCollection<Photo> photos = ((App)App.Current).photos;
        public PanoramaPage1()
        {
            InitializeComponent();

        }
        private ImageSource GetImageSource(string fileName)
        {
            return new BitmapImage(new Uri(fileName, UriKind.Absolute));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (panorama.Items.Count <= 0)
            {
                foreach (Photo p in photos)
                {
                    PanoramaItem pi = new PanoramaItem();
                    pi.Header = p.PhotoTile;
                    Image img = new Image();
                    img.Source = p.PhotoSource;
                    pi.Content = img;
                    panorama.Items.Add(pi);
                }
            }
            panorama.DefaultItem = panorama.Items[int.Parse(NavigationContext.QueryString["selectedImage"])];
            base.OnNavigatedTo(e);
        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            base.OnBackKeyPress(e);
        }
        
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            var selectedItem = ((PanoramaItem)panorama.Items[panorama.SelectedIndex]);
            var newTile = new StandardTileData()
            {
                Title = ((PanoramaItem)panorama.Items[panorama.SelectedIndex]).Header.ToString()
            };
            ShellTile.Create(new Uri("/ImagePanorama.xaml?selectedImage=" + panorama.SelectedIndex, UriKind.Relative), newTile);
        }

        private void panorama_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ImageDetails.xaml?imageindex="+panorama.SelectedIndex,UriKind.Relative));
        }
      
    }
}