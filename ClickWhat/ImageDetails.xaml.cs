using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ClickWhat
{
    public partial class Page1 : PhoneApplicationPage

    {
        private int selectedImageIndex;
        public Page1()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            selectedImageIndex = int.Parse(NavigationContext.QueryString["imageindex"]);
            imageTitle.Text = ((App)App.Current).photos.ElementAt(int.Parse(NavigationContext.QueryString["imageindex"])).PhotoTile;
            fullImage.Source = ((App)App.Current).photos.ElementAt(int.Parse(NavigationContext.QueryString["imageindex"])).PhotoSource;
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ImagePanorama.xaml?selectedImage=" + selectedImageIndex, UriKind.Relative));
            base.OnNavigatedFrom(e);
        }
    }
}