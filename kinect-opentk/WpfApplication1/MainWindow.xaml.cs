using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
// Windows Forms support
using System.Windows.Forms;
// OpenTK
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
// Kinect
using Microsoft.Research.Kinect.Nui;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        private GLControl glc;
        private Runtime nui;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the GLControl.
            glc = new GLControl();

            // Assign Load and Paint events of GLControl.
            glc.Load += new EventHandler(glc_Load);
            glc.Paint += new PaintEventHandler(glc_Paint);

            // Assign the GLControl as the host control's child.
            host.Child = glc;


            // Initiate Kinect runtime and streams
            nui = Runtime.Kinects[0];
            try
            {
                nui.Initialize(RuntimeOptions.UseColor);
                nui.VideoStream.Open(ImageStreamType.Video, 2,
                    ImageResolution.Resolution640x480, ImageType.Color);
            }
            catch (InvalidOperationException)
            {
                return;
            }
            // Assign stream events
            nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
        }

        void glc_Load(object sender, EventArgs e)
        {
            // Make background "chocolate"
            GL.ClearColor(System.Drawing.Color.Chocolate);

            int w = glc.Width;
            int h = glc.Height;

            // Set up initial modes
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1);
            GL.Viewport(0, 0, w, h);
        }

        void glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // Draw a little yellow triangle
            GL.Color3(System.Drawing.Color.Yellow);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(200, 50);
            GL.Vertex2(200, 200);
            GL.Vertex2(100, 50);
            GL.End();

            glc.SwapBuffers();
        }

        public void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // Dump video stream to the Image element
            PlanarImage Image = e.ImageFrame.Image;
            image.Source = BitmapSource.Create(Image.Width, Image.Height, 96, 96,
                PixelFormats.Bgr32, null, Image.Bits, Image.Width * Image.BytesPerPixel);
        }
    }
}
