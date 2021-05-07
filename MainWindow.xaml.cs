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
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using System.Management;
using WindowsDisplayAPI;
using WindowsDisplayAPI.DisplayConfig;
using WindowsDisplayAPI.Native.DisplayConfig;

namespace LllamaPadScrap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        
        AxSIGPLUSLib.AxSigPlus LlamaAxSig = new AxSigPlus();
        
        private Bitmap _SigTransparent;
        private string PDFPath;
        



        public MainWindow()
        {
            
        
            
           InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            var screen = Screen.AllScreens;
            Screen testscreen;
            
            testscreen = screen.Where(x => (x.WorkingArea.Width == 1024) && (x.WorkingArea.Height == 560)).FirstOrDefault();

            if (testscreen != null)
            {
                var workingArea = testscreen.WorkingArea;
                this.Left = workingArea.Left;
                this.Top = workingArea.Top;
                this.Width = workingArea.Width;
                this.Height = workingArea.Height;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Minimized;
                this.WindowState = WindowState.Normal;
            }
            //Need to work on Making this full screen borderless for the other app.


            


            LlamaPadArea.Child = LlamaAxSig;
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            var workingdir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            var tempPDFPath = workingdir + "\\resource\\TestNDA.pdf";
            //System.Windows.MessageBox.Show(test);
            if (File.Exists(tempPDFPath))
            {
                LlamaBrowser.Address = tempPDFPath;
                PDFPath = tempPDFPath;
                //System.Windows.MessageBox.Show("PDF Found");
            }
            else
            {
                System.Windows.MessageBox.Show("PDF Not Found :(");
            }

        }

        private void OpenFile(object sender, RoutedEventArgs E)
        {

            var workingdir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = workingdir;
            openFileDialog1.Title = "Select a PDF to use.";
            openFileDialog1.DefaultExt = "pdf";
            openFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LlamaBrowser.Address = openFileDialog1.FileName;

            }


        }


        private void SubmitBtn(object Snder, RoutedEventArgs E)
        {
            long mySize;
            byte[] myArray;
            var BackgroundRM = System.Drawing.Color.FromArgb(213, 213, 213);


            LlamaAxSig.SigCompressionMode = 1;
            LlamaAxSig.TabletState = 0; //turn off tablet
            LlamaAxSig.JustifyMode = 5; //zoom signature to fit image size
            LlamaAxSig.ImageXSize = 325; //image width in px
            LlamaAxSig.ImageYSize = 135; //image height in px
            LlamaAxSig.ImagePenWidth = 8; //image ink thickness in px
            LlamaAxSig.BackColor = BackgroundRM;

            LlamaAxSig.BitMapBufferWrite();
            mySize = LlamaAxSig.BitMapBufferSize();
            myArray = new byte[mySize];
            myArray = (byte[])LlamaAxSig.GetBitmapBufferBytes();
            LlamaAxSig.BitMapBufferClose();
            LlamaAxSig.ClearTablet();
            MemoryStream ms = new MemoryStream(myArray);

            System.Drawing.Image sigImage = System.Drawing.Image.FromStream(ms);

            Bitmap tempbmp = new Bitmap(sigImage);         
            tempbmp.MakeTransparent(BackgroundRM);
            _SigTransparent = tempbmp;


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
                        tempbmp.Save(fs, ImageFormat.Png);
                        break;
                   

                }

                fs.Close();
            }
            GeneratePDF();


            //sigImage.Save("C:\\mySig.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //sigImage.Save("C:\\mySig.jpg", System.Drawing.Imaging.ImageFormat.Jpeg); 
            //sigImage.Save("C:\\mySig.tif", System.Drawing.Imaging.ImageFormat.Tiff);
        }

        private void GeneratePDF()
        {
            try

            {
                double myX = 3 * 72;
                double myY = 7.5 * 72;
                double someWidth = 126;
                double someHeight = 36;
                string pdfloc = LlamaBrowser.Address.Substring(LlamaBrowser.Address.IndexOf(@"file:///")).Replace(@"file:///", "");
                var tempdocument = PdfReader.Open(pdfloc);
                //PdfDocument tempdocument = new PdfDocument(PDFPath);
                PdfPages temppages = tempdocument.Pages;
                int pagecount = temppages.Count;
                PdfPage temppage = temppages[0];
                
                
                if (pagecount == 1)
                {
                     temppage = temppages[0];
                }
                else if(pagecount > 1)
                {
                    int pageindex = (pagecount - 1);
                     temppage = temppages[pageindex];
                }

                
                
                temppage.Orientation = PageOrientation.Portrait;
                temppage.Width = XUnit.FromInch(8.5);
                temppage.Height = XUnit.FromInch(11);

                XGraphics gfx = XGraphics.FromPdfPage(temppage, XPageDirection.Downwards);

                MemoryStream stream = new MemoryStream();
                _SigTransparent.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                Byte[] bytes = stream.ToArray();


                gfx.DrawImage(XImage.FromStream(stream), myX, myY, someWidth, someHeight);
                //gfx.DrawImage(XImage.FromStream(_SigStream), 0, 0);
                SaveFileDialog LlamaSaves = new SaveFileDialog();
                LlamaSaves.Filter = "PDF |*.pdf";
                LlamaSaves.Title = "Save PDF";
                LlamaSaves.ShowDialog();

                if (LlamaSaves.FileName != "")
                {

                    FileStream fs = (FileStream)LlamaSaves.OpenFile();

                    switch (LlamaSaves.FilterIndex)
                    {
                        case 1:
                            tempdocument.Save(fs, true);
                            break;


                    }

                    fs.Close();
                }
               // _SigStream.Dispose();
                tempdocument.Dispose();
                gfx.Dispose();
                stream.Dispose();
                _SigTransparent.Dispose();
            }
            catch(Exception Err)
            {
                System.Windows.MessageBox.Show(Err.Message);

            }


        }




        private void BtnClicked(Object Sender, EventArgs E)
        {
           // var bob = LlamaAxSig.TabletComPort;
            LlamaAxSig.TabletState = 1;
            LlamaAxSig.SigCompressionMode = 0;
            LlamaAxSig.SetEnableColor(1);
            LlamaAxSig.ForeColor = System.Drawing.Color.Black;
            LlamaAxSig.BackColor = System.Drawing.Color.FromArgb(213, 213, 213);
            //MessageBox.Show("Tablet Query Clicked? \nResults: " + bob.ToString(), "SigPad", MessageBoxButton.OK);

        }

        private void ClearSigBtn(Object Sender, EventArgs E)
        {

            LlamaAxSig.ClearTablet();
            //MessageBox.Show("Connection Query Clicked? \nResults: " + test.ToString(), "SigPad", MessageBoxButton.OK);

        }



        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {

            var paths = WindowsDisplayAPI.DisplayConfig.PathInfo.GetActivePaths();


        }







    }
}
