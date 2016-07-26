using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.Networking.Proximity;
using Windows.Media.Capture;
using System.Threading.Tasks;

namespace WM_AppNew
{
    
    public class bluetoothConnectionParams
    {
        
        public RfcommDeviceService chatService;
        public StreamSocket chatSocket;
        public DataWriter chatWriter;
        public DataReader chatReader;
        public bool isConnectedToBluetooth;
    }


    sealed partial class App : Application
    {
        public static MobileServiceClient MobileService = new MobileServiceClient("http://adamarduino.azure-mobile.net/", "FEvTowmfUmRUmauPhFtdsVIGbpnbUm98");

        public static bluetoothConnectionParams connectionParams;
        public static int BTdata; // data that i recieve from saleh ( BT ) 



        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            connectionParams = new bluetoothConnectionParams();
            connectionParams.chatReader = null;
            connectionParams.chatService = null;
            connectionParams.chatSocket = null;
            connectionParams.chatWriter = null;
            connectionParams.isConnectedToBluetooth = false;

        }

        /********************************************************************************************************/

        public static async void sendBluetoothSignal(string str)
        {
            App.connectionParams.chatWriter.WriteString(str);
            await App.connectionParams.chatWriter.StoreAsync();
        }

        /********************************************************************************************************/

        public static async void ReceiveFromBluetoothSignal() // when i add a async word i revies an warnning
        {
            char ch = '\0';
            int fit = 0;
            while (ch != '\n')
            {
                uint sizeFieldCount;
                IAsyncOperation<uint> taskLoad = App.connectionParams.chatReader.LoadAsync(1);
                taskLoad.AsTask().Wait();
                sizeFieldCount = taskLoad.GetResults();
                if (sizeFieldCount != 1)
                {
                    App.connectionParams.isConnectedToBluetooth = false;
                    return; // the socket was closed before reading.
                }
                byte b = App.connectionParams.chatReader.ReadByte();
                ch = Convert.ToChar(b);
                if (ch != '\r' && ch != '\n' )
                {
                    fit *= 10;
                    fit += Convert.ToInt32(b) - '0';
                }
            }
            App.BTdata = fit;
        }

        /********************************************************************************************************/
        
        public static int getHue(int r, int g, int b)
        {
            int h = 0;
            double r1 = r / 255, g1 = g / 255, b1 = b / 255;

            //find min and max values out of r,g,b components
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);

            if(max == r)
            {
                //if red is the predominent color
                if ((max - min) == 0)
                {
                    h = (int)((g - b) / (1));
                }
                else
                {
                    h = (int)((g - b) / (max - min));
                }
            }
            else if(max == g){
                //if green is the predominent color
                if ((max - min) == 0)
                {
                    h = 2 + (int)((b - r) / (1));
                }
                else
                {
                    h = 2 + (int)((b - r) / (max - min));
                }
            }
            else if(max == b){
                //if blue is the predominent color
                if ((max - min) == 0)
                {
                    h = 4 + (int)((r - g) / (1));
                }
                else
                {
                    h = 4 + (int)((r - g) / (max - min));
                }
            }

            h = h*60; //find the sector of 60 degrees to which the color belongs
            //https://www.pathofexile.com/forum/view-thread/1246208/page/45 - hsl color wheel

            if(h >0)
            {
                //h is a positive angle in the color wheel
                return h;
            }
            else
            {
                //h is a negative angle.
                return (360 -h);
            }
        }
        
        /********************************************************************************************************/

        public static int GetGreenLevel(ref Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap, int threshold)
        {
            double wavelength, hue, sum = 0, avgWaveLength = 0;
            uint count = 0 ;

            unsafe
            {
                int ExtractionTotalRGB = 128;// equal to 128
                int TotalRGB ;
                IBuffer pixels = bitmap.PixelBuffer;
                byte[] arr = pixels.ToArray();
                for (int i = 0; i < pixels.Length; i += 3)
                {
                    var pixelLocation = bitmap.PixelWidth * i + 100;
                    if (pixelLocation > 20000000)
                    {
                        break;
                    }
                    TotalRGB = pixels.GetByte((uint)pixelLocation) + pixels.GetByte((uint)pixelLocation + 1) + pixels.GetByte((uint)pixelLocation + 2);

                    if (Math.Abs(TotalRGB - ExtractionTotalRGB) < threshold)
                    {
                        int b = (int)pixels.GetByte((uint)pixelLocation);
                        int g = (int)pixels.GetByte((uint)pixelLocation + 1);
                        int r = (int)pixels.GetByte((uint)pixelLocation + 2);
                        
                        //hue = (650 - wavelength) * 240 / (650 - 475);
                        hue = getHue(r,g,b);
                        wavelength = 650 - ((hue * (650 - 475)) / 240); // nm ( not Angestrum )
                        if (wavelength >= 495 && wavelength <= 570) // Green WaveLength
                        {
                            sum += wavelength;
                            count++;
                        }
                    }
                }
            }

            avgWaveLength = sum / count;
            if (avgWaveLength >= 495 && avgWaveLength < 510)
            {
                return 1;
            }
            if (avgWaveLength >= 510 && avgWaveLength < 525)
            {
                return 2;
            }
            if (avgWaveLength >= 525 && avgWaveLength < 540)
            {
                return 3;
            }
            if (avgWaveLength >= 540 && avgWaveLength < 555)
            {
                return 4;
            }
            if (avgWaveLength >= 555 && avgWaveLength <= 570)
            {
                return 5;
            }
            return -1;
        }


        /*
        Example:
        int greenLevel = GetGreenLevel(ref b, 100);
        */

        /********************************************************************************************************/

        public static double PixelToCentimeter(double Pixels)
        {
            double Centimeter = -1;
            Centimeter = (Pixels * 2.54d) / 96;
            while (Centimeter > 3)
            {
                Centimeter /= 3;
            }
            return (double)Centimeter;
        }

        /********************************************************************************************************/

        public static int DistanceBetweenLines(ref Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap)
        {
            double centimeter = 0, wavelength, hue;
            uint count1 = 0, count2 = 0,count3 = 0, countY1 = 0, countY2 = 0,
                countY3 = 0, flag1 = 1, flag2 = 0, flag3 = 0;

            unsafe
            {
                IBuffer pixels = bitmap.PixelBuffer;
                byte[] arr = pixels.ToArray();

                for (var x = 0; x < bitmap.PixelWidth; x++) 
                {
                    var pixelLocation = bitmap.PixelWidth * x + 100;
                    int b = (int)pixels.GetByte((uint)pixelLocation); // arr[i]
                    int g = (int)pixels.GetByte((uint)pixelLocation + 1);
                    int r = (int)pixels.GetByte((uint)pixelLocation + 2);

                    //Color clr = Color.FromArgb((int)r, (int)g, (int)b);
                    //hue = (650 - wavelength) * 240 / (650 - 475);
                    hue = getHue(r, g, b);
                    wavelength = 650 - ((hue * (650 - 475)) / 240); // nm ( not Angestrum )

                    if (wavelength >= 495 && wavelength <= 570 && flag1 == 1) // Green WaveLength, first line
                    {
                        count1++; // how much green pixels are
                        continue;
                    }

                    if (wavelength >= 590 && wavelength <= 610 ) // Yellow WaveLength
                    {
                        flag1 = 0;
                        countY1++;
                        if(countY1 > 30 ) // 30 pixels ( < 1 cm )
                        {
                            flag2 = 1;
                        }
                        continue; 
                    }

                    if (wavelength >= 495 && wavelength <= 570 && flag2 == 1) // Green WaveLength, second line
                    {
                        count2++; // how much green pixels are
                    }

                    if (wavelength >= 590 && wavelength <= 610) // Yellow WaveLength
                    {
                        flag2 = 0;
                        countY2++;
                        if (countY2 > 30) // 30 pixels ( < 1 cm )
                        {
                            flag3 = 1;
                        }
                        continue;
                    }

                    if (wavelength >= 495 && wavelength <= 570 && flag3 == 1) // Green WaveLength, third line
                    {
                        count3++; // how much green pixels are
                    }

                    if (wavelength >= 590 && wavelength <= 610) // Yellow WaveLength
                    {
                        flag3 = 0;
                        countY3++;
                        continue;
                    }
                }
            }
            double avgPixels = (count1 + count2 + count3) / 3;
            centimeter = PixelToCentimeter(avgPixels);
            
            // find the distanse ( max 3 CM ) 
            if (centimeter >= 0 && centimeter < 0.6)
            {
                return 1;
            }
            if (centimeter >= 0.6 && centimeter < 1.2)
            {
                return 2;
            }
            if (centimeter >= 1.2 && centimeter < 1.8)
            {
                return 3;
            }
            if (centimeter >= 1.8 && centimeter < 2.4)
            {
                return 4;
            }
            if (centimeter >= 2.4 && centimeter <= 3)
            {
                return 5;
            }
            return -1;
        }


        /*
        Example:
        int greenLevel = DistanceBetweenLines(ref b);
        */


        /********************************************************************************************************/

        public static int GetYellowLevel(ref Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap, int threshold)
        {
            double wavelength, hue, sum = 0, avgWaveLength = 0;
            uint count = 0 ;

            unsafe
            {
                int ExtractionTotalRGB = 570;
                int TotalRGB ;
                IBuffer pixels = bitmap.PixelBuffer;
                byte[] arr = pixels.ToArray();
                for (int i = 0; i < pixels.Length; i += 3)
                {
                    var pixelLocation = bitmap.PixelWidth * i + 100;
                    if (pixelLocation > 20000000)
                    {
                        break;
                    }
                    TotalRGB = pixels.GetByte((uint)pixelLocation) + pixels.GetByte((uint)pixelLocation + 1) + pixels.GetByte((uint)pixelLocation + 2);
                    if (Math.Abs(TotalRGB - ExtractionTotalRGB) < threshold)
                    {
                        int b = (int)pixels.GetByte((uint)pixelLocation);
                        int g = (int)pixels.GetByte((uint)pixelLocation + 1);
                        int r = (int)pixels.GetByte((uint)pixelLocation + 2);

                        //Color clr = Color.FromArgb((int)r, (int)g, (int)b);
                        //hue = (650 - wavelength) * 240 / (650 - 475);
                        hue = getHue(r, g, b);
                        wavelength = 650 - ((hue * (650 - 475)) / 240); // nm ( not Angestrum )
                        if (wavelength >= 590 && wavelength <= 610) // Green WaveLength
                        {
                            sum += wavelength;
                            count++;
                        }
                    }
                }
            }
            avgWaveLength = sum / count;
            if (avgWaveLength >= 590 && avgWaveLength < 594)
            {
                return 1;
            }
            if (avgWaveLength >= 594 && avgWaveLength < 598)
            {
                return 2;
            }
            if (avgWaveLength >= 598 && avgWaveLength < 602)
            {
                return 3;
            }
            if (avgWaveLength >= 602 && avgWaveLength < 606)
            {
                return 4;
            }
            if (avgWaveLength >= 606 && avgWaveLength < 610)
            {
                return 5;
            }
            return -1;
        }


        /*
        Example:
        int greenLevel = GetYellowLevel(ref b, 150);
        */

        /********************************************************************************************************/

        public static int GetDiameter(ref Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap, int threshold)
        {
            uint count = 0 , max = 0;

            unsafe
            {
                int ExtractionTotalRGB = 0; // equal to zero ( Black )
                int TotalRGB;
                IBuffer pixels = bitmap.PixelBuffer;
                byte[] arr = pixels.ToArray();
                int length = (int)(pixels.Length / 1000);
                for (int i = 0; i < pixels.Length; i += 3)
                {
                    TotalRGB = pixels.GetByte((uint)i) + pixels.GetByte((uint)i + 1) + pixels.GetByte((uint)i + 2);
                    if (Math.Abs(TotalRGB - ExtractionTotalRGB) < threshold)
                    {
                        count++;
                        if( count > max)
                        {
                            max = count;
                        }
                        continue;
                    }
                    count = 0;
                }
            }
            double centemeter = PixelToCentimeter(count);
            // find the diameter ( max 1.5 CM )
            if (centemeter >= 0 && centemeter < 0.3)
            {
                return 1;
            }
            if (centemeter >= 0.3 && centemeter < 0.6)
            {
                return 2;
            }
            if (centemeter >= 0.6 && centemeter < 0.9)
            {
                return 3;
            }
            if (centemeter >= 0.9 && centemeter < 1.2)
            {
                return 4;
            }
            if (centemeter >= 1.2 && centemeter <= 1.5)
            {
                return 5;
            }
            return -1;
        }


        /*
        Example:
        int greenLevel = GetYellowLevel(ref b, 200, Color.Yellow);
        */


        /********************************************************************************************************/






        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {


            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
