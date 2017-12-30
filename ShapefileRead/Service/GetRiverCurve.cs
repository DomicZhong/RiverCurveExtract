using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
//using System.Collections.Generic;
using Shapefile.IO;
using static Shapefile.IO.ShapeFileIO;

namespace Shapefile.Service
{
    class GetRiverCurve
    {
        public static List<List<Point>> GetRiverCurvePoint(shpfilestruct polylineStruct)
        {
            List<List<Point>> startEndList = new List<List<Point>>();

            foreach (Polyline p in polylineStruct.shpList)
            {
                for (int i = 0; i < 1 /*p.numParts*/; i++)
                {
                    int startpoint;
                    int endpoint;
                    if (i == p.numParts - 1)
                    {
                        startpoint = (int)p.parts[i];
                        endpoint = p.numPoints;
                    }
                    else
                    {
                        startpoint = (int)p.parts[i];
                        endpoint = (int)p.parts[i + 1];
                    }
                    //saveString[line++] = "折线" + count.ToString() + ":";

                    for (int j = startpoint; j < endpoint; j++)  //设置起点
                    {
                        List<Point> doublePoint = new List<Point>();

                        Point startpo = (Point)p.points[j];
                        Point endpo = (Point)p.points[j];                            //终点
                        double minRatio = 999.99, limitRatio = 0.005;

                        for (int k = j + 1; k < endpoint; k++)   //找终点
                        {
                            Point MarkPoint = (Point)p.points[k];
                            double D, L = 0;
                            double Ratio;
                            D = Math.Pow(Math.Pow(MarkPoint.X - startpo.X, 2) + Math.Pow(MarkPoint.Y - startpo.Y, 2), 0.5);  //求两点直接的距离

                            for (int range = j; range <= k; range++)   //求两点在河流中的长度
                            {
                                Point temppo = (Point)p.points[range];
                                L += Math.Pow(Math.Pow(temppo.X - startpo.X, 2) + Math.Pow(temppo.Y - startpo.Y, 2), 0.5);
                            }
                            Ratio = D / L;

                            if (Ratio < minRatio)
                            {
                                minRatio = Ratio;
                                endpo = MarkPoint;
                            }
                        }

                        if (minRatio < limitRatio)    //小于限制比例
                        {
                            doublePoint.Add(startpo);
                            doublePoint.Add(endpo);
                            startEndList.Add(doublePoint);   //加到曲流起点终点数组中
                        }
                    }
                }
            }
            return startEndList;
        }


    }
}
