using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Renci.SshNet;

namespace TrackingMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GMap.NET.WindowsPresentation.GMapMarker Mkr;

        public MainWindow()
        {
            InitializeComponent();
            Start();          
        }

        
        void Start()
        {

            InitiMap();
            var result = GetCoordinates();
            while (string.IsNullOrEmpty(result.ToString()))
            {
               result = GetCoordinates();
            }
            var coords = result.ToString().Split(','); // .Substring(0, 9);
            var longititude = coords[0];
            var latitude = coords[1]; //Substring(10, 9);
            var distance = coords[3];
            var point = new PointLatLng(Convert.ToDouble(longititude.Substring(1)), Convert.ToDouble(latitude));
            AddMarker(point,distance[0]);
        }

     
        void InitiMap()
        {
            //Map Configurations
            GMap.NET.CacheProviders.SQLitePureImageCache ch = new GMap.NET.CacheProviders.SQLitePureImageCache();
            ch.CacheLocation = @"../../CashedMapDB/";
            gMapControl1.Manager.PrimaryCache = ch;
            gMapControl1.MapProvider = GMapProviders.OpenStreetMap;
            gMapControl1.Manager.Mode = AccessMode.CacheOnly;
            gMapControl1.DragButton = MouseButton.Left;
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 4;
        }

        public StringBuilder GetCoordinates()
        {
            SshClient sshclient = new SshClient("10.130.1.1", "root", "dragino");
            if (!sshclient.IsConnected)
            {
                sshclient.Connect();
            }
            
            //SshClient ssh = new SshClient("10.130.1.1", "root", "dragino");
            //ssh.Connect();
            //SshCommand sc = ssh.CreateCommand("telnet localhost 6571");
            //sc.Execute();
            //Thread.Sleep(2000);
            //sc.EndExecute();

            //var result = "";
            //    while (string.IsNullOrEmpty(result))
            //    {
            //        result = ssh.RunCommand("telnet localhost 6571 > ^]").Result;
            //    }
            //    ssh.Disconnect();
            //    return result;
            //}
            ShellStream stream = sshclient.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);
            var result = SendCommand("telnet localhost 6571", stream);

            return result;

        }

        public StringBuilder SendCommand(string customCMD , ShellStream stream)
        {
            StringBuilder answer;

            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            WriteStream(customCMD, writer, stream);
            answer = ReadStream(reader);
            return answer;
        }
        private void WriteStream(string cmd, StreamWriter writer, ShellStream stream)
        {
            writer.WriteLine(cmd);
            while (stream.Length == 0)
            {
                Thread.Sleep(500);
            }
        }

        private StringBuilder ReadStream(StreamReader reader)
        {
            StringBuilder result = new StringBuilder();

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    if (line[0] == 'S')
                    {
                        result.AppendLine(line);
                        break;
                    }
                }
              
            }
            return result;
        }
        void AddMarker(PointLatLng x , char dist)
        {
            //set Marker
            
            Mkr = new GMap.NET.WindowsPresentation.GMapMarker(x);

            BitmapImage bti = new BitmapImage();
            ToolTip tt = new ToolTip();

            tt.Content = "Name: " + "Location";
            if (dist == '3')
            {

                Mkr.Shape = new Ellipse
                {

                    Width = 20,
                    Height = 20,
                    Stroke = Brushes.Red,
                    StrokeThickness = 10,
                    ToolTip = tt,
                };
            }
            if (dist == '2')
            {

                Mkr.Shape = new Ellipse
                {

                    Width = 20,
                    Height = 20,
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 10,
                    ToolTip = tt,
                };
            }
            if (dist == '1')
            {

                Mkr.Shape = new Ellipse
                {

                    Width = 20,
                    Height = 20,
                    Stroke = Brushes.Green,
                    StrokeThickness = 10,
                    ToolTip = tt,
                };
            }
            gMapControl1.Markers.Add(Mkr);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Start();
        }

    }
}
