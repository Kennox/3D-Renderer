using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer {
    public class Triangle {
        public Vertex A, B, C;
        public Vector2 stA, stB, stC;
        Vector3 Normal;
        public Bitmap Texture;

        public Triangle(Vector3 a, Vector3 b, Vector3 c, Vector3 colour) {
            this.Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));

            this.A = new Vertex(colour, a, Normal);
            this.B = new Vertex(colour, b, Normal);
            this.C = new Vertex(colour, c, Normal);           
        }

        public Triangle(Vector3 a, Vector3 b, Vector3 c, Vector3 colourA, Vector3 colourB, Vector3 colourC) {
            this.Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));

            this.A = new Vertex(colourA, a, Normal);
            this.B = new Vertex(colourB, b, Normal);
            this.C = new Vertex(colourC, c, Normal);
        }

        public Triangle(Vector3 a, Vector3 b, Vector3 c, Vector3 colourA, Vector3 colourB, Vector3 colourC, Vector3 normalA, Vector3 normalB, Vector3 normalC) {
            this.Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));

            this.A = new Vertex(colourA, a, normalA);
            this.B = new Vertex(colourB, b, normalB);
            this.C = new Vertex(colourC, c, normalC);
        }

        public Triangle(Vector3 a, Vector3 b, Vector3 c, Vector2 stA, Vector2 stB, Vector2 stC, Bitmap texture) {
            this.Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            this.Texture = texture;
            this.stA = stA;
            this.stB = stB;
            this.stC = stC;
            this.A = new Vertex(a, Normal, stA);
            this.B = new Vertex(b, Normal, stB);
            this.C = new Vertex(c, Normal, stC);
        }

        public Triangle Transform(Matrix4x4 m) {
            var temp = new Triangle(Vector3.Transform(A.Position, m), Vector3.Transform(B.Position, m), Vector3.Transform(C.Position, m), A.Colour, B.Colour, C.Colour);

            //TODO find better solution for this
            temp.Texture = this.Texture;
            temp.stA = this.stA;
            temp.stB = this.stB;
            temp.stC = this.stC;
            temp.A.st = this.stA;
            temp.B.st = this.stB;
            temp.C.st = this.stC;
            temp.A.Normal = Vector3.TransformNormal(this.A.Normal, m);
            temp.B.Normal = Vector3.TransformNormal(this.B.Normal, m);
            temp.C.Normal = Vector3.TransformNormal(this.C.Normal, m);

            return temp;
            
        }
    }
}
