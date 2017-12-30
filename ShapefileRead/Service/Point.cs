using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shapefile.Service
{
    class Point   //点类
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double M { get; set; }

        public Point(int x, int y, int z,int m)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.M = m;
        }
        public Point()
        {
        }
    }

}
