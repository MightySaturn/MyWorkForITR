using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Line
    {
        /// <summary>
        /// Координата X начала линии
        /// </summary>
        public double StartX { get; set; }

        /// <summary>
        /// Координата Y начала линии
        /// </summary>
        public double StartY { get; set; }

        /// <summary>
        /// Координата X конца линии 
        /// </summary>
        public double EndX { get; set; }

        /// <summary>
        /// Координата Y конца линии
        /// </summary>
        public double EndY { get; set; }

        /// <summary>
        /// Длина отрезка по OX
        /// </summary>
        public double LengthX { get; set; }

        /// <summary>
        /// Длина отрезка по OY
        /// </summary>
        public double LengthY { get; set; }

        /// <summary>
        /// Является ли прямая вертикальной в пространстве
        /// </summary>
        public bool IsVertical { get; set; }

        /// <summary>
        /// Является ли прямая горизонтальной в пространстве
        /// </summary>
        public bool IsHorizontal { get; set; }

        /// <summary>
        /// Находится ли прямая в общем положении (под углом)
        /// </summary>
        public bool IsCommon { get; set; }

        public Line(double x1, double y1, double x2, double y2)
        {
            StartX = x1;
            StartY = y1;
            EndX = x2;
            EndY = y2;

            LengthX = EndX - StartX;
            LengthY = EndY - StartY;
        }
    }
}

