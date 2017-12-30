using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using Shapefile.Service;

namespace Shapefile.IO
{
    class PointIO : ShapeFileIO.shpIO
    {   
        /// <summary>
        /// 读点数据
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public override ArrayList Read(BinaryReader br)
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
        /// 点数据struct写入string
        /// </summary>
        /// <param name="points"></param>
        /// <param name="shp"></param>
        /// <returns></returns>
        public override string[] Tostring(Shapefile.IO.ShapeFileIO.shpfilestruct shp)
        {
            int line = 0;
            string[] saveString = new string[10000];
            ShapeFileIO.shpfileHeadWrite(ref shp, ref line, saveString);  //写文件头

            foreach (Point p in shp.shpList)
            {
                string tempstr = "点:" + String.Format(":{0:F7},", p.X) + String.Format("{0:F7}", p.Y);
                saveString[line++] = tempstr;
            }
            return saveString;
        }

    }
}
