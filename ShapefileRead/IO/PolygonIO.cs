using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using Shapefile.Service;

namespace Shapefile.IO
{
    class PolygonIO : ShapeFileIO.shpIO
    {
        /// <summary>
        /// 读多边形数据
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public override ArrayList Read(BinaryReader br)
        {
            ArrayList polygons = new ArrayList();
            while (br.PeekChar() != -1)
            {
                Polygon polygon = new Polygon();

                uint RecordNum = br.ReadUInt32();
                int DataLength = br.ReadInt32();

                //读取第i个记录
                br.ReadInt32();
                polygon.Box[0] = br.ReadDouble();
                polygon.Box[1] = br.ReadDouble();
                polygon.Box[2] = br.ReadDouble();
                polygon.Box[3] = br.ReadDouble();
                polygon.NumParts = br.ReadInt32();
                polygon.NumPoints = br.ReadInt32();

                for (int i = 0; i < polygon.NumParts; i++)
                {
                    int part = new int();
                    part = br.ReadInt32();
                    polygon.parts.Add(part);
                }

                for (int j = 0; j < polygon.NumPoints; j++)
                {
                    Point pointtemp = new Point();
                    pointtemp.X = br.ReadDouble();
                    pointtemp.Y = br.ReadDouble();
                    polygon.points.Add(pointtemp);
                }
                polygons.Add(polygon);
            }
            return polygons;
        }
        /// <summary>
        /// 多边形数据struct写入string
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="shp"></param>
        /// <returns></returns>
        public override string[] Tostring(Shapefile.IO.ShapeFileIO.shpfilestruct shp)
        {
            int line = 0;
            string[] saveString = new string[10000];
            ShapeFileIO.shpfileHeadWrite(ref shp, ref line, saveString);  //写文件头

            int count_ = 1;
            foreach (Polygon p in shp.shpList)
            {
                for (int i = 0; i < p.NumParts; i++)
                {
                    int startpoint;
                    int endpoint;
                    if (i == p.NumParts - 1)
                    {
                        startpoint = (int)p.parts[i];
                        endpoint = p.NumPoints;
                    }
                    else
                    {
                        startpoint = (int)p.parts[i];
                        endpoint = (int)p.parts[i + 1];
                    }
                    saveString[line++] = "多边形" + count_.ToString() + ":";

                    for (int k = 0, j = startpoint; j < endpoint; j++, k++)
                    {
                        Point ps = (Point)p.points[j];
                        saveString[line++] = "\t点：" + ps.X + "," + ps.Y;
                    }
                    count_++;
                }
            }
            return saveString;
        }
    }
}
