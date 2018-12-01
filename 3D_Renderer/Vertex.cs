using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer {

    public class Vertex {

        public Vector3 Colour;
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 st;

        public Vertex(Vector3 colour, Vector3 position, Vector3 normal) {
            this.Colour = colour;
            this.Position = position;
            this.Normal = normal;
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 st) {
            this.Position = position;
            this.Normal = normal;
            this.st = st;
        }
    }
}
