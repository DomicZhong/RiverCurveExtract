using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Shapefile.Service
{
    class Polyline //折线类
    {
        public double[] Box=new double[4];
        public int numParts;
        public int numPoints;
        public ArrayList parts=new ArrayList();
        public ArrayList points = new ArrayList();
    }

}
