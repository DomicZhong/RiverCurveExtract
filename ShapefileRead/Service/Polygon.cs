using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Shapefile.Service
{
    class Polygon  //多边形类
    {
        public double[] Box = new double[4];
        public int NumParts;
        public int NumPoints;
        public ArrayList parts = new ArrayList();
        public ArrayList points = new ArrayList();
    }
}
