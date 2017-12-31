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
                for (int i = 0; i < 1 /*p.numParts*/; i++)   //由于一个线文件中线太多，取线文件中第一条线
                {
                    int startPointIndex;
                    int endPointIndex;
                    if (i == p.numParts - 1)
                    {
                        startPointIndex = (int)p.parts[i];
                        endPointIndex = p.numPoints;
                    }
                    else
                    {
                        startPointIndex = (int)p.parts[i];
                        endPointIndex = (int)p.parts[i + 1];
                    }
                    //saveString[line++] = "折线" + count.ToString() + ":";

                    for (int j = startPointIndex; j < endPointIndex; j++)   //设置曲流颈起点
                    {
                        List<Point> FeaturePoint = new List<Point>();

                        Point startpo = (Point)p.points[j];                          //曲流颈起点
                        Point endpo = (Point)p.points[j];                            //曲流颈终点
                        Point CentralPoint = new Point();                            //曲流轴起点
                        Point AxisEndPoint = new Point();                            //曲流轴终点
                        double minRatio = 999.99, limitRatio = 0.005;
                        int endindex=j;
                        int axisEndPointIndex = j;
                        double maxdistance = 0;

                        for (int k = j + 1; k < endPointIndex; k++)   //找曲流颈终点
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

                            if (Ratio < minRatio)   //取最小比例的终点
                            {
                                minRatio = Ratio;
                                endpo = MarkPoint;
                                endindex = k;
                            }
                        }     //找到终点

                        CentralPoint.X = (startpo.X + endpo.X) / 2 ; CentralPoint.Y = (startpo.Y + endpo.Y) / 2;  //取曲流颈中点

                        //找曲流轴
                        for(int axispointindex = startPointIndex; axispointindex < endindex; axispointindex++)
                        {
                            Point temppo = (Point)p.points[axispointindex];
                            double temdis = Math.Pow(Math.Pow((temppo.X - CentralPoint.X), 2) + Math.Pow((temppo.Y - CentralPoint.Y), 2),0.5);

                            if (temdis > maxdistance)
                            {
                                maxdistance = temdis;
                                axisEndPointIndex = axispointindex;
                                AxisEndPoint = temppo;
                            }
                        }

                        if (minRatio < limitRatio)    //小于限制比例，将每个河曲四个特征点加到List里
                        {
                            FeaturePoint.Add(startpo);     
                            FeaturePoint.Add(endpo);
                            FeaturePoint.Add(CentralPoint);
                            FeaturePoint.Add(AxisEndPoint);

                            startEndList.Add(FeaturePoint);   //加到曲流起点终点数组中
                            j += (endindex - j) / 4;    //河曲起点往前推进四分之一点数
                        }

                    }
                }
            }
            return startEndList;
        }


    }
}
