using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer {
    public class Triangle {
        public Vertex A, B, C;
        Vector3 Normal;

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

        public Triangle Transform(Matrix4x4 m) {

            return new Triangle(Vector3.Transform(A.Position, m), Vector3.Transform(B.Position, m), Vector3.Transform(C.Position, m), A.Colour, B.Colour, C.Colour);
        }
    }
}
