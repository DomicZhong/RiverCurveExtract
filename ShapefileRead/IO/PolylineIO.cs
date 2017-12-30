using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using Shapefile.Service;

namespace Shapefile.IO
{
    class PolylineIO : ShapeFileIO.shpIO
    {
        /// <summary>
        /// 读折线数据
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public override ArrayList Read(BinaryReader br)
        {
            ArrayList polylines = new ArrayList();
            while (br.PeekChar() != -1)
            {
                Polyline polyline = new Polyline();
                uint RecordNum = br.ReadUInt32();
                int DataLength = br.ReadInt32();

                //读取第i个记录
                br.ReadInt32();
                polyline.Box[0] = br.ReadDouble();
                polyline.Box[1] = br.ReadDouble();
                polyline.Box[2] = br.ReadDouble();
                polyline.Box[3] = br.ReadDouble();
                polyline.numParts = br.ReadInt32();
                polyline.numPoints = br.ReadInt32();

                for (int i = 0; i < polyline.numParts; i++)
                {
                    int part = br.ReadInt32();
                    polyline.parts.Add(part);
                }

                for (int j = 0; j < polyline.numPoints; j++)
                {
                    Point point = new Point();
                    point.X = br.ReadDouble();
                    point.Y = br.ReadDouble();
                    polyline.points.Add(point);
                }
                polylines.Add(polyline);
            }
            return polylines;
        }
        /// <summary>
        /// 折线数据struct写入string
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="shp"></param>
        /// <returns></returns>
        public override string[] Tostring(Shapefile.IO.ShapeFileIO.shpfilestruct shp)
        {
            int line = 0;
            string[] saveString = new string[10000];
            ShapeFileIO.shpfileHeadWrite(ref shp, ref line, saveString); //写文件头

            int count = 1;
            foreach (Polyline p in shp.shpList)
            {
                for (int i = 0; i < p.numParts; i++)
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
                    saveString[line++] = "折线" + count.ToString() + ":";

                    for (int k = 0, j = startpoint; j < endpoint; j++, k++)
                    {
                        Point po = (Point)p.points[j];
                        saveString[line++] = "\t点:" + po.X + "," + po.Y;
                    }
                    count++;
                }
            }
            return saveString;
        }
    }
}
