using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging;//add reference
using AForge.Imaging.Filters;//add reference
using AForge;//add reference
using AForge.Video.DirectShow;//add reference
using System.Drawing.Imaging;//add reference
using System.Threading;//add reference
using System.IO;//add reference
using AForge.Video;             //add reference

namespace Afoge3
{
    public partial class Form1 : Form
    {
        //reference sudah ditambahkan
        private bool DeviceExist = false;//setup adanya media webcam
        private FilterInfoCollection VideoCaptureDevices;//mengumpulkan setiap perangkat video yg terdeteksi
        private VideoCaptureDevice FinalVideoSource;//menampung perangkat video yang akan digunakan
        ColorFiltering colorFilter = new ColorFiltering();
        GrayscaleBT709 grayFilter = new GrayscaleBT709();
        // use two blob counters, so the could run in parallel in two threads
        Color color = Color.Black;
        BlobCounter blobCounter = new BlobCounter();
        int x = 0;
        public float x1, y1;

        int a,b;

        public Form1()
        {
            InitializeComponent();
        }
       


        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
           // colorDialog1.ShowDialog();
           // color = colorDialog1.Color;
            //tambahkan kode berikut untuk menentukan perangkat sumber dari video
            FinalVideoSource = new VideoCaptureDevice(VideoCaptureDevices[comboBox1.SelectedIndex].MonikerString);
            FinalVideoSource.NewFrame += new NewFrameEventHandler(FinalVideoSource_NewFrame);
            FinalVideoSource.DesiredFrameSize = new Size(160, 120);
            FinalVideoSource.Start();
            
            
            
   
        }
        // received frame from the 1st camera
        private void FinalVideoSource_NewFrame(object sender, ref Bitmap image)
        {
            
                

        }
        void FinalVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            
            
            //sekarang kita tambahkan kode berikut untuk menampilkan new frame event (frame dari webcam) untuk menampilkan gambar pada picturebox
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();//bitmap boxing
            Bitmap images = (Bitmap)eventArgs.Frame.Clone();
            pictureBox3.Image = images;
            pictureBox4.Image = images;
           
            // create filter
            //ColorFiltering colorFilter = new ColorFiltering();
           //EuclideanColorFiltering colorFilter = new EuclideanColorFiltering();
           //colorFilter.CenterColor = Color.FromArgb(color.ToArgb());
            // configure the filter
            colorFilter.Red =new IntRange(int.Parse(textBox4.Text)-10,int.Parse(textBox1.Text));
           colorFilter.Green = new IntRange(int.Parse(textBox5.Text) - 10, int.Parse(textBox2.Text));
          colorFilter.Blue = new IntRange(int.Parse(textBox6.Text) - 10, int.Parse(textBox3.Text));
            //apply the filter
            Bitmap objectImage = colorFilter.Apply(image);
           
            // create blob counter and configure it
           // BlobCounter blobCounter = new BlobCounter();
              blobCounter.FilterBlobs = true;               // filter blobs by size
            blobCounter.ObjectsOrder = ObjectsOrder.Size; // order found object by size
            // grayscaling
            Bitmap grayImage = grayFilter.Apply(objectImage);
            
            // locate blobs 
            blobCounter.ProcessImage(grayImage);
            Rectangle[] rects = blobCounter.GetObjectRectangles();
            // draw rectangle around the biggest blob
            if (rects.Length > 0)
            {
                Rectangle objectRect = rects[0];
                Graphics g = Graphics.FromImage(image);

                using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 3))
                {
                    g.DrawRectangle(pen, objectRect);
                }

                g.Dispose();

                x1 = objectRect.X + objectRect.Width / 2;
                y1 = objectRect.Y + objectRect.Height / 2;
                //map to [-1, 1] range

                // x1 = x1 *4;
                //  y1 = y1 *4;
                //camera1Acquired.Set( );


            }
            else
            {
                x1 = 0;
                y1 = 0;
            }
            pictureBox5.Image = grayImage;
            pictureBox1.Image = image;
            pictureBox2.Image = objectImage;
           
            // for the first camera, for example
           
            
            

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //pertama add reference semua Aforge.video.dll dan Aforge.video.directshow.dll
            VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //sekarang perangkat video sudah tersimpan pada array di atas
            foreach (FilterInfo VideoCaptureDevice in VideoCaptureDevices)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name);
            }
            //jalankan aplikasi ini maka akan didapatkan semua perangkat video yang terscan di pc
            comboBox1.SelectedIndex = 0;
            //sekarang akan ditambahkan kode untuk menjalankan webcam dan menampiljkan gambar pada picturebox
          
            // configure blob counters
     
            
   
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //kita tambahkan kode untuk membuat webcam berhenti bekerja
            //kapanpun form telah ditutup maka webcam akan berhenti bekerja secara automatis
            timer1.Stop();
            if (FinalVideoSource.IsRunning)
            {
                
                FinalVideoSource.Stop();
            }
            //selesai
            //sekarang kita jalankan
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
            float x = 0;
            float y = 0;
            x = x1;
            y = y1;
            label3.Text = x.ToString();
            label4.Text = y.ToString();
           
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
           colorDialog1.ShowDialog();
            color = colorDialog1.Color;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            blobCounter.MaxWidth = Convert.ToInt32(numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            blobCounter.MaxHeight = Convert.ToInt32(numericUpDown2.Value);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            blobCounter.MinWidth = Convert.ToInt32(numericUpDown3.Value);
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            blobCounter.MinHeight = Convert.ToInt32(numericUpDown4.Value);
        }

        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
           
            Bitmap bmp = (Bitmap)pictureBox3.Image;
            Color clr = bmp.GetPixel(e.X, e.Y);
            textBox1.Text = clr.R.ToString();
            textBox2.Text = clr.G.ToString();
            textBox3.Text = clr.B.ToString();
                
         
        }

        private void pictureBox4_MouseClick(object sender, MouseEventArgs e)
        {

            Bitmap bmp2 = (Bitmap)pictureBox3.Image;
            Color clrs = bmp2.GetPixel(e.X, e.Y);
            textBox4.Text = clrs.R.ToString();
            textBox5.Text = clrs.G.ToString();
            textBox6.Text = clrs.B.ToString();
         
        }

      
            
          



        
       
       
    }
}
