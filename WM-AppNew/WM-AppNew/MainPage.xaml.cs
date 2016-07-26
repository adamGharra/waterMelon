using System;
using System.Collections;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WM_AppNew
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Params
        public static int greenLevel;
        public static int yellowSpaceLevel;
        public static int mass;
        public static int volume;
        public static int distanceBetweenLines;
        public static int lowCircleDiameter;
        public static int exampleType;
        public static string range;


        public MainPage()
        {
            greenLevel = -1;
            yellowSpaceLevel = -1;
            mass = -1;
            volume = -1;
            distanceBetweenLines = -1;
            lowCircleDiameter = -1;
            exampleType = -1;
            range = (-1).ToString();

            this.InitializeComponent();
            MyFrame.Navigate(typeof(Page1));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.Navigate(typeof(Page1));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
        }

        private static readonly string blobStoreConnectionString = "DefaultEndpointsProtocol=https;AccountName=basastorage;AccountKey=RMFHqqfFO75/OUmfHLJA3QztTNc48ZEHIw2VmnBTbNsstROm45X5KlK1quWKGdNKEE731VjFuWZDGbidI8z2rw==";

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoForward)
            {
                MyFrame.GoForward();
            }
        }
    }
}