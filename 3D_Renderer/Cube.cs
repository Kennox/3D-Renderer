﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer {
    public class Cube {

        public readonly static Vector3[] points = new Vector3[] {
            new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1), //top
            new Vector3(-1, -1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, 1), new Vector3(-1, 1, 1)  //bottom
        };

        public Vector3 top, bottom, left, right, front, back;
        public Vector3[] Colours;

        public Triangle[] triangles;

        public Bitmap Texture;

        public Cube(Vector3 top, Vector3 bottom, Vector3 left, Vector3 right, Vector3 front, Vector3 back) {

            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
            this.front = front;
            this.back = back;

            triangles = new Triangle[]{
                    new Triangle(points[0], points[1], points[2], top), new Triangle(points[0], points[2], points[3], top), //top
                    new Triangle(points[7], points[6], points[5], bottom), new Triangle(points[7], points[5], points[4], bottom), //bottom
                    new Triangle(points[0], points[3], points[7], left), new Triangle(points[0], points[7], points[4], left), //left
                    new Triangle(points[2], points[1], points[5], right), new Triangle(points[2], points[5], points[6], right), //right
                    new Triangle(points[3], points[2], points[6], front), new Triangle(points[3], points[6], points[7], front), //front
                    new Triangle(points[1], points[0], points[4], back), new Triangle(points[1], points[4], points[5], back)  //back
            };
        }

        public Cube(Vector3[] colours) {

            this.Colours = colours;

            triangles = new Triangle[]{
                    new Triangle(points[0], points[1], points[2], top), new Triangle(points[0], points[2], points[3], top), //top
                    new Triangle(points[7], points[6], points[5], bottom), new Triangle(points[7], points[5], points[4], bottom), //bottom
                    new Triangle(points[0], points[3], points[7], left), new Triangle(points[0], points[7], points[4], left), //left
                    new Triangle(points[2], points[1], points[5], right), new Triangle(points[2], points[5], points[6], right), //right
                    new Triangle(points[3], points[2], points[6], front), new Triangle(points[3], points[6], points[7], front), //front
                    new Triangle(points[1], points[0], points[4], back), new Triangle(points[1], points[4], points[5], back)  //back
            };
            
            int i = 0;
            int j = Colours.Length;
            while (i < 36) {
                if(i % 3 == 0) {
                    triangles[i / 3].A.Colour = Colours[i % j];
                } else if (i % 3 == 1) {
                    triangles[i / 3].B.Colour = Colours[i % j];
                } else {
                    triangles[i / 3].C.Colour = Colours[i % j];
                }
                i++;
            }
        }

        public Cube(Bitmap texture, bool isTextureSixSided) {
            this.Texture = texture;
            if (isTextureSixSided) {
                //TODO something is off here, need to investigate (maybe s and t need to be switched?)
                triangles = new Triangle[]{
                    new Triangle(points[0], points[1], points[2], new Vector2(0.25f, 0.75f), new Vector2(0.5f, 0.75f), new Vector2(0.5f, 0.5f), texture),
                    new Triangle(points[0], points[2], points[3], new Vector2(0.25f, 0.75f), new Vector2(0.5f, 0.5f), new Vector2(0.25f, 0.5f), texture), //top
                    new Triangle(points[7], points[6], points[5], new Vector2(0.25f, 0.25f), new Vector2(0.5f, 0.25f), new Vector2(0.5f, 0), texture),
                    new Triangle(points[7], points[5], points[4], new Vector2(0.25f, 0.25f), new Vector2(0.5f, 0), new Vector2(0.25f, 0), texture), //bottom
                    new Triangle(points[0], points[3], points[7], new Vector2(0, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.25f, 0.25f), texture),
                    new Triangle(points[0], points[7], points[4], new Vector2(0, 0.5f), new Vector2(0.25f, 0.25f), new Vector2(0, 0.25f), texture), //left
                    new Triangle(points[2], points[1], points[5], new Vector2(0.5f, 0.5f), new Vector2(0.75f, 0.5f), new Vector2(0.75f, 0.25f), texture),
                    new Triangle(points[2], points[5], points[6], new Vector2(0.5f, 0.5f), new Vector2(0.75f, 0.25f), new Vector2(0.5f, 0.25f), texture), //right
                    new Triangle(points[3], points[2], points[6], new Vector2(0.25f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.25f), texture),
                    new Triangle(points[3], points[6], points[7], new Vector2(0.25f, 0.5f), new Vector2(0.5f, 0.25f), new Vector2(0.25f, 0.25f), texture), //front
                    new Triangle(points[1], points[0], points[4], new Vector2(0.75f, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.25f), texture),
                    new Triangle(points[1], points[4], points[5], new Vector2(0.75f, 0.5f), new Vector2(1, 0.25f), new Vector2(0.75f, 0.25f), texture)  //back
                };
            } else {
                triangles = new Triangle[]{
                    new Triangle(points[0], points[1], points[2], new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), texture),
                    new Triangle(points[0], points[2], points[3], new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0), texture), //top
                    new Triangle(points[7], points[6], points[5], new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), texture),
                    new Triangle(points[7], points[5], points[4], new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0), texture), //bottom
                    new Triangle(points[0], points[3], points[7], new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), texture),
                    new Triangle(points[0], points[7], points[4], new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0), texture), //left
                    new Triangle(points[2], points[1], points[5], new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), texture),
                    new Triangle(points[2], points[5], points[6], new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0), texture), //right
                    new Triangle(points[3], points[2], points[6], new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), texture),
                    new Triangle(points[3], points[6], points[7], new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0), texture), //front
                    new Triangle(points[1], points[0], points[4], new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), texture),
                    new Triangle(points[1], points[4], points[5], new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0), texture)  //back
                };
            }
        }
    }
}
