using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ChatHubWPF
{
    public partial class FormVideo : Form
    {

        private static Socket sendSocket;
        private static byte[] sendBuffer;
        private static Socket sizeSenderSocket;
        private static byte[] sizeBuffer;
        //const string ip = "127.0.0.1";
        const string ip = "jnhjnjs";
        const int port = 9873;
        //const int port = 9786;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        IPEndPoint sizeSendEndPoint = new IPEndPoint(IPAddress.Parse(ip), 10002);
        IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 9786);
        IPEndPoint sizeReceiveEndPoint = new IPEndPoint(IPAddress.Any, 10002);

        private FilterInfoCollection videoDevicesList;
        private IVideoSource videoSource;
        public FormVideo()
        {
            InitializeComponent();
            Thread clientthread = new Thread(Listener);
            clientthread.Start();
            // get list of video devices
            videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoDevice in videoDevicesList)
            {
                cmbVideoSource.Items.Add(videoDevice.Name);
            }
            if (cmbVideoSource.Items.Count > 0)
            {
                cmbVideoSource.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // stop the camera on window close
            this.Closing += Form1_Closing;
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            // signal to stop
            //ClosingConnections();
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //   Thread.Sleep(1000);
            //Thread t = new Thread(VideoSender);
            //t.Start();
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = bitmap;
            //Thread listenerThread = new Thread(InvokingListener);
            //listenerThread.Start();
            if (pictureBox1.InvokeRequired) { pictureBox1.Invoke(new MethodInvoker(() => VideoSender())); } else VideoSender();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            GettingConnection();
            videoSource = new VideoCaptureDevice(videoDevicesList[cmbVideoSource.SelectedIndex].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
            //if (pictureBox1.InvokeRequired) { pictureBox1.Invoke(new MethodInvoker(() => VideoSender())); } else VideoSender();
            //VideoSender();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            videoSource.SignalToStop();
            ClosingConnections();
            if (videoSource != null && videoSource.IsRunning && pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
        }

        private void Listener()
        {
            byte[] buffer;
            byte[] sizeBuffer = new byte[4];
            using (Socket videoReceiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                using (Socket sizeReceiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    videoReceiveSocket.Bind(receiveEndPoint);
                    videoReceiveSocket.Listen(1);
                    sizeReceiveSocket.Bind(sizeReceiveEndPoint);
                    sizeReceiveSocket.Listen(1);
                    using (Socket client = videoReceiveSocket.Accept())
                    {
                        using (Socket sizeClient = sizeReceiveSocket.Accept())
                        {
                            MemoryStream stream;
                            while (true)
                            {
                                //Thread.Sleep(4);
                                sizeClient.Receive(sizeBuffer);
                                buffer = ReceiveLargeFile(client, BitConverter.ToInt32(sizeBuffer, 0));
                                stream = new MemoryStream(buffer);
                                pictureBox2.Image = Image.FromStream(stream);
                                stream.Position = 0;
                            }
                        }
                    }
                }
            }
        }
        private byte[] ReceiveLargeFile(Socket socket, int lenght)
        {
            // send first the length of total bytes of the data to server
            // create byte array with the length that you've send to the server.
            byte[] data = new byte[lenght];


            int size = lenght; // lenght to reveive
            var total = 0; // total bytes to received
            var dataleft = size; // bytes that havend been received 

            // 1. check if the total bytes that are received < than the size you've send before to the server.
            // 2. if true read the bytes that have not been receive jet
            while (total < size)
            {
                // receive bytes in byte array data[]
                // from position of total received and if the case data that havend been received.
                var recv = socket.Receive(data, total, dataleft, SocketFlags.None);
                if (recv == 0) // if received data = 0 than stop reseaving
                {
                    data = null;
                    break;
                }
                total += recv;  // total bytes read + bytes that are received
                dataleft -= recv; // bytes that havend been received
            }
            return data; // return byte array and do what you have to do whith the bytes.
        }
        private void VideoSender()
        {
            if (pictureBox1.Image != null)
            {
                //Thread.Sleep(4);
                var size = SizeCounter(pictureBox1.Image);
                sendBuffer = new byte[size];
                sizeBuffer = BitConverter.GetBytes(size);
                MemoryStream stream = new MemoryStream(sendBuffer);
                //pictureBox1.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                SaveJpeg(stream, pictureBox1.Image, 30);
                sendSocket.Send(sendBuffer);
                sizeSenderSocket.Send(sizeBuffer);
                stream.Position = 0;
            }
        }
        private int SizeCounter(Image img)
        {
            MemoryStream sizeStream = new MemoryStream();
            img.Save(sizeStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return sizeStream.ToArray().Length;
        }
        private void GettingConnection()
        {
            sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sendSocket.Connect(endPoint);
            sizeSenderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sizeSenderSocket.Connect(sizeSendEndPoint);
        }
        public void ClosingConnections()
        {
            sendSocket.Shutdown(SocketShutdown.Both);
            sendSocket.Close();
        }
        public static void SaveJpeg(MemoryStream stream, Image img, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

        // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
        // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(stream, jpegCodec, encoderParams);
        }
    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        // Get image codecs for all image formats 
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

        // Find the correct image codec 
        for (int i = 0; i < codecs.Length; i++)
            if (codecs[i].MimeType == mimeType)
                return codecs[i];

        return null;
    }
    }
}