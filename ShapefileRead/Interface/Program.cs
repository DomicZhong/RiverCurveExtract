using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shapefile.IO;
using Shapefile.Service;


namespace Shapefile.Interface
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName1 = @"典型河曲.shp";
            //string saveFileName1 = @"典型河曲.txt";
            List<List<Point>> StartEndList = new List<List<Point>>();

            ShapeFileIO shp1 = new ShapeFileIO();

            ShapeFileIO.shpfilestruct polylineStruct = shp1.ShpRead(fileName1);

            StartEndList = GetRiverCurve.GetRiverCurvePoint(polylineStruct); 

            foreach(List<Point> doublepointlist in StartEndList)
            {
                Console.WriteLine("起点：{0},{1} 终点：{2},{3},曲流轴起点：{4}，{5}，曲流轴中点{6}，{7}",doublepointlist[0].X.ToString(), doublepointlist[0].Y.ToString(), doublepointlist[1].X.ToString(), doublepointlist[1].Y.ToString(),doublepointlist[2].X.ToString(), doublepointlist[2].Y.ToString(), doublepointlist[3].X.ToString(), doublepointlist[3].Y.ToString());
            } 

            //if (TxtFileIO.TxtFileIO.TxtFileWrite(saveFileName1, shp1.ShpToString(polylineStruct), false))
            //{
            //    Console.WriteLine("完成");
            //}
            Console.ReadLine();
        }
    }
}
