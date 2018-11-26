using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer {
    class Matrix2x2 {

        public readonly float M00, M01, M10, M11;

        public Matrix2x2() {

        }

        public Matrix2x2(Vector3 a, Vector3 b) {
            this.M00 = a.X;
            this.M01 = b.X;
            this.M10 = a.Y;
            this.M11 = b.Y;
        }

        public Matrix2x2(float a, float b, float c, float d) {
            this.M00 = a;
            this.M01 = b;
            this.M10 = c;
            this.M11 = d;
        }

        public static Matrix2x2 operator *(Matrix2x2 m, float k) {
            return new Matrix2x2(m.M00 * k, m.M01 * k, m.M10 * k, m.M11 * k);

        }
        public static Matrix2x2 operator *(float k, Matrix2x2 m) {
            return m * k;

        }
        public static Vector2 operator *(Matrix2x2 m, Vector3 v) {
            return new Vector2(m.M00 * v.X + m.M01 * v.Y, m.M10 * v.X + m.M11 * v.Y);
        }

        public static Matrix2x2 Invert(Matrix2x2 m) {
            return new Matrix2x2(m.M11, -m.M01, -m.M10, m.M00) * (1 / (m.M00 * m.M11 - m.M01 * m.M10));

        } 
    }
}
