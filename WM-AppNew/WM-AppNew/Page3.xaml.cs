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
using Microsoft.WindowsAzure.MobileServices;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;
using System.Diagnostics;
using Windows.Networking.Proximity;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Http;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WM_AppNew
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page3 : Page
    {
        public Page3()
        {
            this.InitializeComponent();
        }

        private async void connectToBluetooth1(object sender, RoutedEventArgs e)
        {
            bool deviceFound = false;
            if (App.connectionParams.isConnectedToBluetooth)
            {
                return;
            }
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            var pairedDevices = await PeerFinder.FindAllPeersAsync();

            if (pairedDevices==null || pairedDevices.Count == 0)
            {
                Debug.WriteLine("No paired devices found.");
            }
            else
            {
                foreach (var device in pairedDevices)
                {
                    if (device.DisplayName == " WM\n") // HC-06 change to our bluetooth name
                    {
                        //Debug.WriteLine("current 2 = " + bluetoothStatus.Text);
                        App.connectionParams.chatSocket = new StreamSocket();
                        try
                        {
                            await App.connectionParams.chatSocket.ConnectAsync(device.HostName, "1");
                            App.connectionParams.chatWriter = new DataWriter(App.connectionParams.chatSocket.OutputStream);
                            App.connectionParams.chatReader = new DataReader(App.connectionParams.chatSocket.InputStream);
                            bluetoothStatus.Text = "Connected";
                            deviceFound = true;
                            App.connectionParams.isConnectedToBluetooth = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Cannot connect to " + device.DisplayName + "!");
                           // throw ex; // i add it
                        }
                    }
                }
                //return;
                if (App.connectionParams.chatSocket == null)
                {
                    Debug.WriteLine("HC-06 is Not Paired");
                }
                if (!deviceFound)
                {
                    bluetoothStatus.Text = "Cannot connect :(";
                }
            }
        }

        /********************************************************************************************************/

        public void handleStep(string str)
        {
            App.sendBluetoothSignal(str);
            App.ReceiveFromBluetoothSignal();
        }

        /********************************************************************************************************/

        public async Task<StorageFile> capturePhoto()
        {

            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Windows.Foundation.Size(10, 10);

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture
                return null;
            }
            return photo;
        }

        /********************************************************************************************************/

        public static async Task<Windows.UI.Xaml.Media.Imaging.WriteableBitmap> converStorageToWriteableBitmap(StorageFile photo)
        {
            IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
            Windows.Graphics.Imaging.BitmapDecoder decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(
                softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);
            softwareBitmap.CopyToBuffer(bitmap.PixelBuffer);
            return bitmap;
        }

        /********************************************************************************************************/

        private async void letStart_Click(object sender, RoutedEventArgs e)
        {
            var bluetoothError = new Windows.UI.Popups.MessageDialog("Cannot start.\nNot connected to bluetooth, click to connect and try again.");
            bluetoothError.Commands.Add(new Windows.UI.Popups.UICommand("Okay."));


            if (!App.connectionParams.isConnectedToBluetooth)
            {
                await bluetoothError.ShowAsync();
                return;
            }

            // picture 1    :    greenLevel   &   distanceBetweenLines
            handleStep("1");
            Debug.WriteLine("aaaaaa =>" + App.BTdata + "<=");
            if (App.BTdata == 1)
            {
                // Capture photo num 1
                StorageFile photo =  await capturePhoto();
                
                // converting Storage to SoftwareBitmap
                Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap = await converStorageToWriteableBitmap(photo);
                
                // Calculates the Params
                int greenLevel = App.GetGreenLevel(ref bitmap, 200);
                int distanceBetweenLines = App.DistanceBetweenLines(ref bitmap);

                WM_AppNew.MainPage.greenLevel = greenLevel;
                WM_AppNew.MainPage.distanceBetweenLines = distanceBetweenLines;
            }

            // picture 2    :    lowCircleDiameter
            handleStep("2");
            if (App.BTdata == 2)
            {
                // Capture photo num 2
                StorageFile photo = await capturePhoto();

                // converting Storage to SoftwareBitmap
                Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap = await converStorageToWriteableBitmap(photo);

                // Calculates the Params
                int diameter = App.GetDiameter(ref bitmap, 100);
                WM_AppNew.MainPage.lowCircleDiameter = diameter;
            }

            // picture 3   :    yellowSpaceLevel
            handleStep("3");
            if (App.BTdata == 3)
            {
                // Capture photo num 3
                StorageFile photo = await capturePhoto();

                // converting Storage to SoftwareBitmap
                Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap = await converStorageToWriteableBitmap(photo);

                // Calculates the Params
                int YellowLevel = App.GetYellowLevel(ref bitmap, 200);
                WM_AppNew.MainPage.yellowSpaceLevel = YellowLevel;
            }

            // recieve mass
            handleStep("4");
            WM_AppNew.MainPage.mass = App.BTdata;

            // recieve volume
            handleStep("5");
            WM_AppNew.MainPage.volume = App.BTdata;

            // call to Adam
            string x = await App.MobileService.InvokeApiAsync<string>("waterm/newwm",
              HttpMethod.Get,
              new Dictionary<string, string>() {
                { "greenLevel",WM_AppNew.MainPage.greenLevel.ToString() },
                { "yellowSpaceLevel",WM_AppNew.MainPage.yellowSpaceLevel.ToString() },
                { "mass",WM_AppNew.MainPage.mass.ToString() },
                { "volume",WM_AppNew.MainPage.volume.ToString() },
                { "distanceBetweenLines",WM_AppNew.MainPage.distanceBetweenLines.ToString() },
                { "lowCircleDiameter",WM_AppNew.MainPage.lowCircleDiameter.ToString() },
                { "sevok","0"},
                { "exampleType","0"}});

            WM_AppNew.MainPage.range = x; // the x is the return param from adam call
            Frame.Navigate(typeof(Page4));
        }

        /********************************************************************************************************/

        private async void Camera_Click(object sender, RoutedEventArgs e)
        {

            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Windows.Foundation.Size(200, 200);

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                return;
            }

            // converting Storage to SoftwareBitmap
            Windows.UI.Xaml.Media.Imaging.WriteableBitmap bitmap = await converStorageToWriteableBitmap(photo);

            // Calculates the Params
            int greenLevel = App.GetGreenLevel(ref bitmap, 50);
            int distanceBetweenLines = App.DistanceBetweenLines(ref bitmap);
            int YellowLevel = App.GetYellowLevel(ref bitmap, 100);
            Debug.WriteLine("aaaaa=>" + YellowLevel + "<=");

        }

        /********************************************************************************************************/

        private void checkBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            checker.Text = checkBox.IsChecked.ToString();
            if (checker.Text == "True")
             {
                 WM_AppNew.MainPage.exampleType = 0;
             }
            if (checker.Text == "False")
             {
                 WM_AppNew.MainPage.exampleType = 1;
             }
        }
    }
}