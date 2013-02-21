using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ClickWhat.Resources;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;

namespace ClickWhat
{
    public partial class MainPage : PhoneApplicationPage
    {
        Dictionary<String, String> urlTitle;
        Photo P;
        private ObservableCollection<Photo> photos = ((App)App.Current).photos;
        //private String apiURL = "http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key=6317d8e0db3a555288202fe04b4f4972&radius=20&per_page=50&format=rest";
        private String apiURL = "http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key=6317d8e0db3a555288202fe04b4f4972&radius=30&per_page=20&format=rest&lat=37.3041&lon=-121.8727";
        private String lat;
        private String lon;

        public MainPage()
        {
            InitializeComponent();
            //getLocation();
            GetAllPhotos();
        }

        private ImageSource GetImageSource(string fileName)
        {
            return new BitmapImage(new Uri(fileName, UriKind.Absolute));
        }
        private void GetAllPhotos()
        {

            WebClient proxy = new WebClient();
            /*if (lat != "" && lon != "")
            {
                apiURL = apiURL+"&lat="+lat+"&lon="+lon;
            }*/
            try
            {
                proxy.DownloadStringAsync(new Uri(apiURL));
                proxy.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(proxy_DownloadStringCompleted);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went horribly wrong! you may not have data.");
            }

        }

        void proxy_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {

            urlTitle = new Dictionary<string, string>();
            urlTitle = ParseCustomXML(e.Result);
            photos.Clear();
            foreach (var iter in urlTitle)
            {
                P = new Photo { PhotoSource = GetImageSource(iter.Key), PhotoTile = iter.Value };
                photos.Add(P);
            }
            lstImage.ItemsSource = photos;
        }
        private Dictionary<String, String> ParseCustomXML(string xmlData)
        {
            var urls = new Dictionary<String, String>();
            XDocument doc = XDocument.Parse(xmlData);
            var photo = from r in doc.Descendants("photo")
                        select new
                        {
                            farm = r.Attribute("farm").Value,
                            server = r.Attribute("server").Value,
                            photoid = r.Attribute("id").Value,
                            secret = r.Attribute("secret").Value,
                            title = r.Attribute("title").Value

                        };
            foreach (var image in photo)
            {
                string url = "http://farm" + image.farm + ".staticflickr.com/" + image.server + "/" + image.photoid + "_" + image.secret + ".jpg";
                urls.Add(url, image.title);
            }
            return urls;
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        private async void getLocation()
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 10000;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );
                lat = geoposition.Coordinate.Latitude.ToString("0.00");
                lon = geoposition.Coordinate.Longitude.ToString("0.00");
                GetAllPhotos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("location  is disabled in phone settings.");
                if ((uint)ex.HResult == 0x80004004)
                {
                    MessageBox.Show("location  is disabled in phone settings.");
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            getLocation();
            //GetAllPhotos();

        }

        private void lstImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedIndex = ((ListBox)sender).SelectedIndex;
            NavigationService.Navigate(new Uri("/ImagePanorama.xaml?selectedImage=" + selectedIndex, UriKind.Relative));
        }

        private void AdvancedSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            AppBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Pivot_LoadingPivotItem_1(object sender, PivotItemEventArgs e)
        {
            if (e.Item == AdvancedSearch)
            {
                ApplicationBar.IsVisible = false;
            }
            else
            {
                ApplicationBar.IsVisible = true;
            }
        }

        private void AppBar_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if ((e.Orientation & PageOrientation.Portrait) == PageOrientation.Portrait)
            {
                TitlePanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TitlePanel.Visibility = System.Windows.Visibility.Collapsed;
            } 
        }
    }
}