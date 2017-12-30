using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Shapefile.Service;
using System.Collections;

namespace Shapefile.IO
{
    class ShapeFileIO
    {
        string[] saveString = new string[10000];  
        /// <summary>
        /// shp文件结构体
        /// </summary>
        public struct shpfilestruct
        {
            //public int fileCode;
            public int fileLength;
            public int version;
            public int shapeType;
            public double Xmin, Ymin, Xmax, Ymax,Zmin,Zmax,Mmin,Mmax;
            public ArrayList shpList;
        }
        /// <summary>
        /// 读shp文件函数
        /// </summary>
        /// <param name="shpName"></param>
        /// <returns></returns>
        public shpfilestruct ShpRead(string shpName)
        {
            shpfilestruct shp = new shpfilestruct();
            FileStream fs = File.Open(shpName, FileMode.Open);

            if (File.Exists(shpName))     //文件存在
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    //文件头部分
                    br.ReadBytes(24);
                    int FileLength = br.ReadInt32();  //<0代表数据长度未知
                    shp.fileLength= Big2LittleEndian(FileLength); //大端模式转小端模式
                    shp.version = br.ReadInt32();  //版本
                    shp.shapeType = br.ReadInt32();
                    shp.Xmin = br.ReadDouble();
                    shp.Ymin = br.ReadDouble();
                    shp.Xmax = br.ReadDouble();
                    shp.Ymax = br.ReadDouble();
                    br.ReadBytes(32);

                    //Record部分
                    switch (shp.shapeType)
                    {
                        case 1:  //点
                            shp.shpList=PointRead(br);
                            break;
                        case 3:  //折线
                            shp.shpList = PolylineRead(br);
                            break;
                        case 5:  //多边形
                            shp.shpList = PolygonRead(br);
                            break;
                    }  //case
                    br.Close();
                }
            }  //if 
            return shp;
        }

        //abstract void shpread(binaryreader br)
        //{

        //}

        public string[] ShpToString(shpfilestruct shp)
        {
            int type=shp.shapeType;
            switch (type)
            {
                case 1:
                    return PointWrite(shp);
                case 3:
                    return PolylineWrite(shp);
                case 5:
                    return PolygonWrite(shp);
                default :
                    return null;
            }
        }
        /// <summary>
        /// 读多边形数据
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private static ArrayList PolygonRead(BinaryReader br)
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
        /// 读折线数据
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private static ArrayList PolylineRead(BinaryReader br)
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
        /// 读点数据
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private static ArrayList PointRead(BinaryReader br)
        {
            ArrayList points = new ArrayList();

            while (br.PeekChar() != -1)
            {
                uint RecordNum = br.ReadUInt32();
                int DataLength = br.ReadInt32();

                //读取第i个记录 ,并将点存入points数组中
                Point point = new Point();
                br.ReadInt32();   //shape type
                point.X = br.ReadDouble();
                point.Y = br.ReadDouble();
                points.Add(point);
            }
            return points;
        }

        /// <summary>
        /// 多边形数据写入string
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="shp"></param>
        /// <returns></returns>
        private static string[] PolygonWrite(shpfilestruct shp)
        {
            int line = 0;
            string[] saveString = new string[10000];
            shpfileHeadWrite(ref shp, ref line, saveString);  //写文件头

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
        /// <summary>
        /// 折线数据写入string
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="shp"></param>
        /// <returns></returns>
        private static string[] PolylineWrite(shpfilestruct shp)
        {
            int line = 0;
            string[] saveString = new string[10000];
            shpfileHeadWrite(ref shp, ref line, saveString); //写文件头

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

                    for (int j = startpoint; j < endpoint; j++)
                    {
                        Point po = (Point)p.points[j];
                        saveString[line++] = "\t点:" + po.X + "," + po.Y;
                    }
                    count++;
                }
            }
            return saveString;
        }

        /// <summary>
        /// 点数据写入string
        /// </summary>
        /// <param name="points"></param>
        /// <param name="shp"></param>
        /// <returns></returns>
        private static string[] PointWrite(shpfilestruct shp)
        {
            int line = 0;
            string[] saveString = new string[10000];
            shpfileHeadWrite(ref shp, ref line, saveString);  //写文件头

            foreach (Point p in shp.shpList)
            {
                string tempstr = "点:" + String.Format(":{0:F7},", p.X) + String.Format("{0:F7}", p.Y);
                saveString[line++] = tempstr;
            }
            return saveString;
        }

        /// <summary>
        /// 写文件头
        /// </summary>
        /// <param name="shp"></param>
        /// <param name="line"></param>
        /// <param name="saveString"></param>
        private static void shpfileHeadWrite(ref shpfilestruct shp, ref int line, string[] saveString)
        {
            saveString[line++] = "文件长度：" + shp.fileLength.ToString() + " * 16 bits";
            saveString[line++] = "版本：" + shp.version.ToString();
            saveString[line++] = "shape类型：" + shp.shapeType.ToString();
            saveString[line++] = "xmin：" + shp.Xmin.ToString();
            saveString[line++] = "ymin：" + shp.Ymin.ToString();
            saveString[line++] = "xmax：" + shp.Xmax.ToString();
            saveString[line++] = "ymax：" + shp.Ymax.ToString();
        } 
 
        /// <summary>
        /// 大端模式转小端模式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int Big2LittleEndian(int data)
        {
            byte[] bData = BitConverter.GetBytes(data);

            if (BitConverter.IsLittleEndian) // 若为 小端模式
            {
                Array.Reverse(bData); // 转换为 大端模式               
            }

            return BitConverter.ToInt32(bData,0);
        }

    }
}

