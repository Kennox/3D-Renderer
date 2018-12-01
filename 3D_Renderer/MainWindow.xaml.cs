using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        public struct Buffer {
            public Triangle triangle;
            public Vector2 uv;
            public Vector3 Position;
            public float zPos;

            public Buffer(Triangle t, Vector2 uv, Vector3 position, float zPos) {
                this.triangle = t;
                this.uv = uv;
                this.Position = position;
                this.zPos = zPos;
            }
        }

        public static WriteableBitmap WriteableBitmap { get; set; }
        public static Buffer[,] ZBuffer;

        Vector3 LightPosition = new Vector3(0, 0, 0);
        Vector3 LightColour = Vector3.One;
        Vector3 Eye = Vector3.Zero;

        static Bitmap Dice = LoadImage("dicetexture.png");
        static Bitmap Wood = LoadImage("wood.jpg");
        static int width = 400;
        static int height = 400;

        //basic cube
        /*
        Cube Cube = new Cube(new Vector3(1, 0, 0),
                                new Vector3(1, 0, 1),
                                new Vector3(1, 1, 1),
                                new Vector3(0, 1, 1),
                                new Vector3(0, 0, 1),
                                new Vector3(1, 1, 0));
                                */
        //interpolated colours cube
        static Vector3[] Colours = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0),
                                                   new Vector3(0, 1, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 0) };
        //Cube Cube = new Cube(Colours);
        
        //texture cubes
        Cube Cube = new Cube(Dice, true);
        //Cube Cube = new Cube(Wood, false);

        float alpha = 0;
        Stopwatch stopwatch = Stopwatch.StartNew();

        public MainWindow() {
            WriteableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            InitializeComponent();
            MainImage.Width = 400;
            MainImage.Height = 400;
            ZBuffer = new Buffer[width, height];

            CompositionTarget.Rendering += Rendering;
        }


        private void Rendering(object sender, EventArgs e) {

            for (int n = 0; n < width; n++) {
                for (int m = 0; m < height; m++) {
                    ZBuffer[n, m] = new Buffer(null, Vector2.Zero, Vector3.Zero, float.PositiveInfinity);
                }
            }

            //int width = WriteableBitmap.PixelWidth;
            //int height = WriteableBitmap.PixelHeight;

            int cc = 4; //Colour Channels
            byte[] ColourData = new byte[width * height * cc];

            //transform matrix
            Matrix4x4 move = Matrix4x4.CreateTranslation(new Vector3(0, 0, 5));
            //rotation matrix
            Matrix4x4 rotate = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(0, 0.4f, 0.2f)), alpha);
            Matrix4x4 rotate2 = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(new Vector3(1, 0, 0)), alpha);

            Matrix4x4 transform = rotate * rotate2 * move;

            //Z-prepass
            for (int i = 0; i < Cube.triangles.Length; i++) {

                var triangle = Cube.triangles[i].Transform(transform);

                Vector3 A = ProjectTo2D(triangle.A.Position);
                Vector3 B = ProjectTo2D(triangle.B.Position);
                Vector3 C = ProjectTo2D(triangle.C.Position);
                Vector3 AB = B - A;
                Vector3 AC = C - A;

                Matrix2x2 matrix = new Matrix2x2(AB, AC);
                Matrix2x2 Imatrix = Matrix2x2.Invert(matrix);

                //backface culling
                if (Vector3.Normalize(Vector3.Cross(AB, AC)).Z <= 0) {
                    continue;
                }

                var minX = (int)Math.Min(Math.Min(A.X, B.X), C.X);
                var minY = (int)Math.Min(Math.Min(A.Y, B.Y), C.Y);
                var maxX = (int)Math.Max(Math.Max(A.X, B.X), C.X);
                var maxY = (int)Math.Max(Math.Max(A.Y, B.Y), C.Y);

                if (minX > width || minY > height || maxX < 0 || maxY < 0) {
                    continue;
                }

                minX = minX < 0 ? 0 : minX;
                minY = minY < 0 ? 0 : minY;
                maxX = maxX > width ? width : maxX;
                maxY = maxY > height ? height : maxY;

                for (int y = minY; y < maxY; y++) {

                    for (int x = minX; x < maxX; x++) {

                        Vector2 uv = Imatrix * (new Vector3(x, y, 1) - A);

                        //if x/y is on triangle
                        if (uv.X >= 0 && uv.Y >= 0 && uv.X + uv.Y < 1) {

                            //position interpolation
                            Vector4 PositionA = new Vector4(triangle.A.Position / triangle.A.Position.Z, 1 / triangle.A.Position.Z);
                            Vector4 PositionB = new Vector4(triangle.B.Position / triangle.B.Position.Z, 1 / triangle.B.Position.Z);
                            Vector4 PositionC = new Vector4(triangle.C.Position / triangle.C.Position.Z, 1 / triangle.C.Position.Z);
                            Vector4 PositionP = PositionA + uv.X * (PositionB - PositionA) + uv.Y * (PositionC - PositionA);
                            PositionP = PositionP / PositionP.W;
                            Vector3 Position = new Vector3(PositionP.X, PositionP.Y, PositionP.Z);

                            if (ZBuffer[x, y].zPos > PositionP.Z) {
                                ZBuffer[x, y] = new Buffer(triangle, uv, Position, PositionP.Z);
                            }
                        }
                    }
                }
            }

            //Rendering
            for (int y = 0; y < ZBuffer.GetLength(1); y++) {
                for (int x = 0; x < ZBuffer.GetLength(0); x++) {

                    if (ZBuffer[x, y].zPos == float.PositiveInfinity) {
                        continue;
                    }
                    var triangle = ZBuffer[x, y].triangle;
                    var uv = ZBuffer[x, y].uv;
                    var Position = ZBuffer[x, y].Position;
                    var zPos = ZBuffer[x, y].zPos;

                    Vector3 Colour;

                    if (triangle.Texture != null) {

                        //s/t interpolation
                        Vector3 stA = new Vector3(triangle.A.st / triangle.A.Position.Z, 1 / triangle.A.Position.Z);
                        Vector3 stB = new Vector3(triangle.B.st / triangle.B.Position.Z, 1 / triangle.B.Position.Z);
                        Vector3 stC = new Vector3(triangle.C.st / triangle.C.Position.Z, 1 / triangle.C.Position.Z);
                        Vector3 stP = stA + uv.X * (stB - stA) + uv.Y * (stC - stA);
                        stP = stP / stP.Z;

                        //bilinear filtering
                        float sBilinear = (stP.X * (triangle.Texture.Width - 1)) % (triangle.Texture.Width - 1);
                        float tBilinear = (stP.Y * (triangle.Texture.Height - 1)) % (triangle.Texture.Height - 1);

                        int sFloor = (int)Math.Floor(sBilinear);
                        int tFloor = (int)Math.Floor(tBilinear);
                        float kS = sBilinear - sFloor;
                        float kT = tBilinear - tFloor;

                        var c1 = ColourConverter(triangle.Texture.GetPixel(sFloor, tFloor)) * (1 - kT) + ColourConverter(triangle.Texture.GetPixel(sFloor, tFloor + 1)) * kT;
                        var c2 = ColourConverter(triangle.Texture.GetPixel(sFloor + 1, tFloor)) * (1 - kT) + ColourConverter(triangle.Texture.GetPixel(sFloor + 1, tFloor + 1)) * kT;
                        Colour = c1 * (1 - kS) + c2 * kS;

                    } else {

                        //colour interpolation
                        Vector4 ColourA = new Vector4(triangle.A.Colour / triangle.A.Position.Z, 1 / triangle.A.Position.Z);
                        Vector4 ColourB = new Vector4(triangle.B.Colour / triangle.B.Position.Z, 1 / triangle.B.Position.Z);
                        Vector4 ColourC = new Vector4(triangle.C.Colour / triangle.C.Position.Z, 1 / triangle.C.Position.Z);
                        Vector4 ColourP = ColourA + uv.X * (ColourB - ColourA) + uv.Y * (ColourC - ColourA);
                        ColourP = ColourP / ColourP.W;
                        Colour = new Vector3(ColourP.X, ColourP.Y, ColourP.Z);
                    }

                    //normal interpolation
                    Vector4 NormalA = new Vector4(triangle.A.Normal / triangle.A.Position.Z, 1 / triangle.A.Position.Z);
                    Vector4 NormalB = new Vector4(triangle.B.Normal / triangle.B.Position.Z, 1 / triangle.B.Position.Z);
                    Vector4 NormalC = new Vector4(triangle.C.Normal / triangle.C.Position.Z, 1 / triangle.C.Position.Z);
                    Vector4 NormalP = NormalA + uv.X * (NormalB - NormalA) + uv.Y * (NormalC - NormalA);
                    NormalP = NormalP / NormalP.W;
                    Vector3 Normal = new Vector3(NormalP.X, NormalP.Y, NormalP.Z);

                    //diffuse and specular
                    Vector3 Diffuse = Vector3.Zero;
                    Vector3 Specular = Vector3.Zero;

                    Vector3 L = Vector3.Normalize(LightPosition - Position);
                    float nL = Vector3.Dot(Vector3.Normalize(Normal), L) * -1;

                    if (nL >= 0) {
                        Diffuse = LightColour * Colour * nL;
                        Vector3 s = L - Vector3.Dot(L, Normal) * Normal;
                        Vector3 r = Vector3.Normalize(L - 2 * s);
                        Vector3 eh = Vector3.Normalize(Position - Eye);
                        if (Vector3.Dot(r, eh) < 0) {
                            Specular = LightColour * (float)Math.Pow(Vector3.Dot(r, eh), 10) * 0.5f;
                        }
                    }

                    Vector3 ColourFinal = Diffuse + Specular;

                    System.Windows.Media.Color c = System.Windows.Media.Color.FromScRgb(1, ColourFinal.X, ColourFinal.Y, ColourFinal.Z);
                    ColourData[(x * 4 + y * width * cc)] = c.B;
                    ColourData[(x * 4 + y * width * cc + 1)] = c.G;
                    ColourData[(x * 4 + y * width * cc + 2)] = c.R;
                }
            }

            alpha += (float)(stopwatch.ElapsedMilliseconds / 1000d * (20 * Math.PI / 180));
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

        public static Bitmap LoadImage(String Filename) {
            var Path = @"Resources\" + Filename;
            var Image = new Bitmap(Path);
            return Image;
        }

        public static Vector3 ColourConverter(System.Drawing.Color c) {
            return new Vector3(c.R / 255f, c.G / 255f, c.B / 255f);
        }
    }
}
