using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

namespace _3D_Renderer {

    public partial class MainWindow : Window {

        public static WriteableBitmap WriteableBitmap { get; set; }
        public static float[] ZBuffer;
        /*
        Cube Cube = new Cube(new Vector3(1, 0, 0),
                                new Vector3(1, 0, 1),
                                new Vector3(1, 1, 1),
                                new Vector3(0, 1, 1),
                                new Vector3(0, 0, 1),
                                new Vector3(1, 1, 0));
                                */
        static Vector3[] Colours = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 0) };

        Cube Cube = new Cube(Colours);

        float alpha = 0;
        Stopwatch stopwatch = Stopwatch.StartNew();

        public MainWindow() {
            WriteableBitmap = new WriteableBitmap(800, 800, 96, 96, PixelFormats.Bgr32, null);
            InitializeComponent();
            MainImage.Width = 800;
            MainImage.Height = 800;
            ZBuffer = new float[800 * 800];
            for (int n = 0; n < ZBuffer.Length; n++) {
                ZBuffer[n] = float.PositiveInfinity;
            }

            CompositionTarget.Rendering += Rendering;
        }


        private void Rendering(object sender, EventArgs e) {

            int width = WriteableBitmap.PixelWidth;
            int height = WriteableBitmap.PixelHeight;

            int cc = 4; //Colour Channels
            byte[] ColourData = new byte[width * height * cc];

            //transform matrix
            Matrix4x4 move = Matrix4x4.CreateTranslation(new Vector3(0, 0, 5));
            //rotation matrix
            Matrix4x4 rotate = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(0, 0.2f, 0.2f)), alpha);
            Matrix4x4 rotate2 = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1, 0, 0)), alpha);

            Matrix4x4 transform = rotate2 * rotate * move;

            for (int i = 0; i < Cube.triangles.Length; i++) {

                var temp = Cube.triangles[i].Transform(transform);
               
                Vector3 A = ProjectTo2D(temp.A.Position);
                Vector3 B = ProjectTo2D(temp.B.Position);
                Vector3 C = ProjectTo2D(temp.C.Position);
                Vector3 AB = B - A;
                Vector3 AC = C - A;

                Matrix2x2 matrix = new Matrix2x2(AB, AC);
                Matrix2x2 Imatrix = Matrix2x2.Invert(matrix);

                //backface culling
                if (Vector3.Normalize(Vector3.Cross(AB, AC)).Z <= 0) {
                    continue;
                }

                var minX = (int) Math.Min(Math.Min(A.X, B.X), C.X);
                var minY = (int) Math.Min(Math.Min(A.Y, B.Y), C.Y);
                var maxX = (int) Math.Max(Math.Max(A.X, B.X), C.X);
                var maxY = (int) Math.Max(Math.Max(A.Y, B.Y), C.Y);

                if(minX > width || minY > height || maxX < 0 || maxY < 0) {
                    continue;
                }

                minX = minX < 0 ? 0 : minX;
                minY = minY < 0 ? 0 : minY;
                maxX = maxX > width ? width : maxX;
                maxY = maxY > height ? height : maxY;

                for (int y = minY; y < maxY; y++) {

                    for (int x = minX; x < maxX; x++) {

                        //TODO ZBuffer somewhere here?
                        Vector4 ColourA;
                        Vector4 ColourB;
                        Vector4 ColourC;
                        Vector4 ColourP;

                        Vector2 uv = Imatrix * (new Vector3(x, y, 1) - A);

                        //if x/y is on triangle
                        if (uv.X >= 0 && uv.Y >= 0 && uv.X + uv.Y < 1) {
                            
                            ColourA = new Vector4(temp.A.Colour / temp.A.Position.Z, 1 / temp.A.Position.Z);
                            ColourB = new Vector4(temp.B.Colour / temp.B.Position.Z, 1 / temp.B.Position.Z);
                            ColourC = new Vector4(temp.C.Colour / temp.C.Position.Z, 1 / temp.C.Position.Z);

                            ColourP = ColourA + uv.X * (ColourB - ColourA) + uv.Y * (ColourC - ColourA);
                            ColourP = ColourP / ColourP.W;

                            System.Windows.Media.Color c = System.Windows.Media.Color.FromScRgb(1, ColourP.X, ColourP.Y, ColourP.Z);
                            ColourData[(x * 4 + y * width * cc)] = c.B;
                            ColourData[(x * 4 + y * width * cc + 1)] = c.G;
                            ColourData[(x * 4 + y * width * cc + 2)] = c.R;
                        }
                    }
                }
            }
            alpha += (float) (stopwatch.ElapsedMilliseconds / 1000d * (20 * Math.PI / 180));
            stopwatch.Restart();

            WriteableBitmap.Lock();
            WriteableBitmap.WritePixels(new Int32Rect(0, 0, width, height), ColourData, width * cc, 0);
            WriteableBitmap.Unlock();
        }

        public Vector3 ProjectTo2D(Vector3 v) {
            int width = WriteableBitmap.PixelWidth;
            int height = WriteableBitmap.PixelHeight;
            return new Vector3(width / 2 + width * v.X / v.Z, height / 2 + width * v.Y / v.Z, 1);
        }
    }
}
