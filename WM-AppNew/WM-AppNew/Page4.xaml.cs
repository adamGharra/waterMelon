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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WM_AppNew
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page4 : Page
    {
        public Page4()
        {
            this.InitializeComponent();
        }

        private void Welcome1_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            if (WM_AppNew.MainPage.range == "1")
            {
                Welcome1.Text = " The Rating Of Your \n Watermelon is \n 1 Out Of 5" + "\n" + " Don't Buy it";
            }

            if (WM_AppNew.MainPage.range == "2")
            {
                Welcome1.Text = " The Rating Of Your \n Watermelon is \n 2 Out Of 5" + "\n" + " Not Recommended";
            }

            if (WM_AppNew.MainPage.range == "3")
            {
                Welcome1.Text = " The Rating Of Your \n Watermelon is \n 3 Out Of 5" + "\n" + " Good";
            }

            if (WM_AppNew.MainPage.range == "4")
            {
                Welcome1.Text = " The Rating Of Your \n Watermelon is \n 4 Out Of 5" + "\n" + " Recommended";
            }

            if (WM_AppNew.MainPage.range == "5")
            {
                Welcome1.Text = " The Rating Of Your \n Watermelon is \n 5 Out Of 5" + "\n" + " Delicious";
            }
        }
    }
}
