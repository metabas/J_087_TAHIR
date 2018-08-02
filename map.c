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
            var longititude = result.Substring(0, 9);
            var latitude = result.Substring(10, 9);
            var point = new PointLatLng(Convert.ToDouble(longititude), Convert.ToDouble(latitude));

           
            AddMarker(point);
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

        public string GetCoordinates()
        {

            using (SshClient ssh = new SshClient("10.130.1.1", 
                "root", "dragino"))
            {               
                ssh.Connect();
                var result = "";
                while (string.IsNullOrEmpty(result))
                      result = ssh.RunCommand("telnet localhost 6571").Result;
                return result;
               // ssh.Disconnect();
            }
            
        }

        void AddMarker(PointLatLng x)
        {
            //set Marker

            Mkr = new GMap.NET.WindowsPresentation.GMapMarker(x);
            BitmapImage bti = new BitmapImage();
            ToolTip tt = new ToolTip();

            tt.Content = "Name: " + "Location";
            Mkr.Shape = new Ellipse
            {
                  
                Width = 20,
                Height = 20,
                Stroke = Brushes.Red,
                StrokeThickness = 10,
                ToolTip = tt,


            };
            gMapControl1.Markers.Add(Mkr);

        }


    }
}
