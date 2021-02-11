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
using AxSIGPLUSLib;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;

namespace LllamaPadScrap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        
        AxSIGPLUSLib.AxSigPlus LlamaAxSig = new AxSigPlus();
        
        



        public MainWindow()
        {
            
        
            
           InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            var screen = Screen.AllScreens;
            var testscreen = Screen.AllScreens[3];
            var workingArea = testscreen.WorkingArea;
            this.Left = workingArea.Left;
            this.Top = workingArea.Top;
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Minimized;
            this.WindowState = WindowState.Normal;
//Need to work on Making this full screen borderless for the other app.

            LlamaPadArea.Child = LlamaAxSig;
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void LlamaSig_PenDown()
        {
            
        }

        private void SubmitBtn(Object Snder, EventArgs E)
        {
            long mySize;
            byte[] myArray;
            LlamaAxSig.SigCompressionMode = 1;
            LlamaAxSig.TabletState = 0; //turn off tablet
            LlamaAxSig.JustifyMode = 5; //zoom signature to fit image size
            LlamaAxSig.ImageXSize = 325; //image width in px
            LlamaAxSig.ImageYSize = 135; //image height in px
            LlamaAxSig.ImagePenWidth = 8; //image ink thickness in px
 

            LlamaAxSig.BitMapBufferWrite();
            mySize = LlamaAxSig.BitMapBufferSize();
            myArray = new byte[mySize];
            myArray = (byte[])LlamaAxSig.GetBitmapBufferBytes();
            LlamaAxSig.BitMapBufferClose();
            LlamaAxSig.ClearTablet();
            MemoryStream ms = new MemoryStream(myArray);

            System.Drawing.Image sigImage = System.Drawing.Image.FromStream(ms);
            Bitmap tempbmp = new Bitmap(sigImage);
            var BackgroundRM = System.Drawing.Color.FromArgb(213, 213, 213);
            tempbmp.MakeTransparent(BackgroundRM);

            SaveFileDialog LlamaSaves = new SaveFileDialog();
            LlamaSaves.Filter = "Png Image|*.png";
            LlamaSaves.Title = "Save Signature";
            LlamaSaves.ShowDialog();

            if(LlamaSaves.FileName != "")
            {
                
                FileStream fs = (FileStream)LlamaSaves.OpenFile();
                    
                switch(LlamaSaves.FilterIndex)
                {
                    case 1 :
                        tempbmp.Save(fs, ImageFormat.Bmp);
                        break;
                    case 2:
                        tempbmp.Save(fs, ImageFormat.Png);
                        break;

                }

                fs.Close();
               



            }



            //sigImage.Save("C:\\mySig.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //sigImage.Save("C:\\mySig.jpg", System.Drawing.Imaging.ImageFormat.Jpeg); 
            //sigImage.Save("C:\\mySig.tif", System.Drawing.Imaging.ImageFormat.Tiff);
        }

        private void BtnClicked(Object Sender, EventArgs E)
        {
           // var bob = LlamaAxSig.TabletComPort;
            LlamaAxSig.TabletState = 1;
            LlamaAxSig.SigCompressionMode = 0;
            LlamaAxSig.BackColor = System.Drawing.Color.FromArgb(213, 213, 213);
            //MessageBox.Show("Tablet Query Clicked? \nResults: " + bob.ToString(), "SigPad", MessageBoxButton.OK);

        }

        private void ClearSigBtn(Object Sender, EventArgs E)
        {

            LlamaAxSig.ClearTablet();
            //MessageBox.Show("Connection Query Clicked? \nResults: " + test.ToString(), "SigPad", MessageBoxButton.OK);

        }

        private void LlamaSig_Clicked()
        {
            System.Windows.MessageBox.Show("SigPad Clicked?", "SigPad", MessageBoxButton.OK);


        }
    }
}
