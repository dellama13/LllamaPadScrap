using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SIGPLUSLib;


namespace LllamaPadScrap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SigPlus LlamaSig = new SigPlus();
        Byte ImageMaybe;
        
        public MainWindow()
        {
            
            InitializeComponent();
            
            LlamaSig.InitSigPlus();
            LlamaSig.TabletComPort = 6;
            LlamaSig.TabletInvisible = false;
            LlamaSig.ImageFileFormat = 0;
            
            LlamaSig.SetSigWindow(0, 0, 0, 0, 0);

            
            LlamaSig.TabletState = 1;
            LlamaSig.EnableTabletCapture();
            //doesn't fire. Woo.
            LlamaSig.PenDown += LlamaSig_PenDown;
            
            //ImageMaybe = LlamaSig.GetBitmapBufferBytes();
        }

        private void LlamaSig_PenDown()
        {
            
        }

        private void SubmitBtn(Object Snder, EventArgs E)
        {
            LlamaSig.AboutBox();
            var XSize = LlamaSig.TabletLogicalXSize;
            var YSize = LlamaSig.TabletLogicalYSize;
            MessageBox.Show("Tablet X/Y Size \nResults: " + "XSize = " + XSize.ToString() + "\nYSize = " + YSize.ToString(), "SigPad", MessageBoxButton.OK);
        }

        private void BtnClicked(Object Sender, EventArgs E)
        {
            var bob = LlamaSig.TabletComPort;
            MessageBox.Show("Tablet Query Clicked? \nResults: " + bob.ToString(), "SigPad", MessageBoxButton.OK);

        }

        private void ClearSigBtn(Object Sender, EventArgs E)
        {
            var test = LlamaSig.TabletComTest;
            MessageBox.Show("Connection Query Clicked? \nResults: " + test.ToString(), "SigPad", MessageBoxButton.OK);

        }

        private void LlamaSig_Clicked()
        {
            MessageBox.Show("SigPad Clicked?", "SigPad", MessageBoxButton.OK);


        }
    }
}
