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

        public Vertex(Vector3 colour, Vector3 position, Vector3 normal) {
            this.Colour = colour;
            this.Position = position;
            this.Normal = normal;
        }
    }
}
