using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    internal class Program
    {
        /// <summary>
        /// Тип нахождения прямых на одной прямой
        /// </summary>
        private enum OnOneLineType
        {
            NotOnOneLine,
            OnOneCommonLine,
            OnOneHorizontalLine,
            OnOneVerticalLine
        }

        static void Main()
        {
            List<Line> lines = new List<Line>
            {
                new Line(7, 10, 3, 10),
                new Line(5.5, 10, 9.5, 10)
            };

            lines.ForEach(x => RedirectLine(x));

            //for (int i = 0; i < lines.Count; i++)
            //{
            //    RedirectLine(lines[i]);
            //}

            // Перебор каждой прямой с каждой
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    // Проверка на совпадение и нахождение на 1 прямой соотвественно (отсекаем эти случаи сразу)
                    Line mainLine = lines[i];
                    Line otherLine = lines[j];

                    if ((mainLine.StartX == otherLine.StartX) && (mainLine.StartY == otherLine.StartY) &&
                        (mainLine.EndX == otherLine.EndX) && (mainLine.EndY == otherLine.EndY))
                    {
                        Console.WriteLine($"Прямые {i + 1} и {j + 1} совпадают! \n");
                        Console.WriteLine($"Точки пересечения соответственно: ({mainLine.StartX}, {mainLine.StartY}) и ({mainLine.EndX}, {mainLine.EndY})");
                    }
                    else
                    {
                        OnOneLineType check = CheckOneLine(mainLine, otherLine);
                        if (check != OnOneLineType.NotOnOneLine)
                        {
                            List<double> answer = FindCrossingOnOneLine(mainLine, otherLine, check);
                            if (answer.Any() == false)
                            {
                                Console.WriteLine($"Прямые {i + 1} и {j + 1} НЕ пересекаются! \n");
                            }
                            else
                            {
                                Console.WriteLine($"Прямые {i + 1} и {j + 1} пересекаются!\n");
                                if (answer.Count == 4)
                                {
                                    Console.WriteLine($"Координаты точек пересечения: ({answer[0]}, {answer[1]}, {answer[2]}, {answer[3]})\n");
                                }
                                else
                                {
                                    Console.WriteLine($"Координаты точки пересечения: ({answer[0]}, {answer[1]})\n");
                                }
                            }
                        }
                        else
                        {
                            // Если не совпадают и не лежат на 1 прямой - применяем основной алгоритм
                            // проводим вектора для проверки пересечения
                            double leftFirstVectorX = otherLine.StartX - mainLine.StartX;
                            double leftFirstVectorY = otherLine.StartY - mainLine.StartY;
                            double rightFirstVectorX = otherLine.EndX - mainLine.StartX;
                            double rightFirstVectorY = otherLine.EndY - mainLine.StartY;

                            double leftSecondVectorX = mainLine.StartX - otherLine.EndX;
                            double leftSecondVectorY = mainLine.StartY - otherLine.EndY;
                            double rightSecondVectorX = mainLine.EndX - otherLine.EndX;
                            double rightSecondVectorY = mainLine.EndY - otherLine.EndY;

                            // т.к. работа ведётся в 2х мерном пространстве, то при векторном произведении компонента x и y обнулится
                            // останется только z, её матрицу и вычисляем

                            double z1 = mainLine.LengthX * leftFirstVectorY - mainLine.LengthY * leftFirstVectorX;
                            double z2 = mainLine.LengthX * rightFirstVectorY - mainLine.LengthY * rightFirstVectorX;

                            double z3 = otherLine.LengthX * leftSecondVectorY - otherLine.LengthY * leftSecondVectorX;
                            double z4 = otherLine.LengthX * rightSecondVectorY - otherLine.LengthY * rightSecondVectorX;

                            // Геометрическая проверка для двух линий, условие должно выполнятся для каждой из них
                            if (((z1 > 0) & (z2 < 0)) | ((z1 < 0) & (z2 > 0)) | (z1 == 0) | (z2 == 0))
                            {
                                if (((z3 > 0) & (z4 < 0)) | ((z3 < 0) & (z4 > 0)) | (z3 == 0) | (z4 == 0))
                                {
                                    Console.WriteLine($"Прямые {i + 1} и {j + 1} пересекаются!\n");

                                    (double crossX, double crossY) = FindCrossingPoint(lines[j], z1, z2);

                                    Console.WriteLine($"Координаты точки пересечения: ({crossX}, {crossY})\n");
                                }
                                else
                                {
                                    Console.WriteLine($"Прямые {i + 1} и {j + 1} НЕ пересекаются!\n");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Прямые {i + 1} и {j + 1} НЕ пересекаются!\n");
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Нахождение пересечения в общем случае
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="z1"></param>
        /// <param name="z2"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        static (double, double) FindCrossingPoint(Line otherLine, double z1, double z2)
        {
            // По формуле идут модули z (длины векторов), но т.к. у нас кроме компоненты z ничего и нет,
            // то по сути мы берём модуль 

            double crossX = otherLine.StartX + otherLine.LengthX * (Math.Abs(z1) / Math.Abs(z2 - z1));
            double crossY = otherLine.StartY + otherLine.LengthY * (Math.Abs(z1) / Math.Abs(z2 - z1));

            return (crossX, crossY);
        }

        /// <summary>
        /// проверка отрезков на нахождение их на 1 прямой.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        static OnOneLineType CheckOneLine(Line line1, Line line2)
        {
            // Охарактеризуем прямые по их расположению, так проще будет понять на какой они одной линии (обычной - под углом,
            // горизонтальной или вертикальной)

            // Первая прямая
            DetermineLineType(line1);

            // Вторая прямая
            DetermineLineType(line2);

            // Определяем тип нахождения на 1 линии
            if (line1.IsCommon & line2.IsCommon)
            {
                double k1 = GetK(line1);
                double k2 = GetK(line2);
                double b1 = GetB(line1, k1);
                double b2 = GetB(line2, k2);

                bool res = (k1 == k2) && (b1 == b2);
                return res ? OnOneLineType.OnOneCommonLine : OnOneLineType.NotOnOneLine;

                //int digit = 1;

                //if ((OnOneLineType)digit == OnOneLineType.NotOnOneLine)
                //{

                //}    
            }

            if (line1.IsHorizontal & line2.IsHorizontal)
            {
                return line1.StartY == line2.StartY ? OnOneLineType.OnOneHorizontalLine : OnOneLineType.NotOnOneLine;
            }

            if (line1.IsVertical && line2.IsVertical)
            {
                return line1.StartX == line2.StartX ? OnOneLineType.OnOneVerticalLine : OnOneLineType.NotOnOneLine;
            }

            return OnOneLineType.NotOnOneLine;
        }

        /// <summary>
        /// Определяет тип указанной линии (Выставляет флаги в объекте)
        /// </summary>
        /// <param name="line"></param>
        private static void DetermineLineType(Line line)
        {
            if (line.StartX == line.EndX)
            {
                line.IsVertical = true;
            }
            else if (line.StartY == line.EndY)
            {
                line.IsHorizontal = true;
            }
            else
            {
                line.IsCommon = true;
            }
        }

        /// <summary>
        /// Коэффициент наклона прямой
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static double GetK(Line line)
        {
            return (line.EndY - line.StartY) / (line.EndX - line.StartX);
        }

        /// <summary>
        /// Точка пересечения с осью OY
        /// </summary>
        /// <param name="line"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static double GetB(Line line, double k)
        {
            return line.StartY - line.StartX * k;
        }

        /// <summary>
        /// Нахождение пересечения на одной линии
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        static List<double> FindCrossingOnOneLine(Line line1, Line line2, OnOneLineType check)
        {
            // Определимся с порядком прямых, сделаем так, чтобы более левые прямые рассматривались как первая, правые - вторая
            // для вертикальной прямой первая - нижняя, вторая - верхняя

            (Line firstOne, Line secondOne) = GetLinesOrder(line1, line2, check);

            // При реализации с помощью классов я не смог найти решения для избегания 2х почти идентичных циклов
            // для вертикальной прямой случай 1, для наклонной и горизонтальной - другой
            // отличается лишь координата, по которой идёт сравнение

            // Здесь, зная, на какого типа прямой лежат 2 отрезка, будем находить точки пересечения
            List<double> res = new List<double>();
            if (check == OnOneLineType.OnOneVerticalLine)
            {
                if (firstOne.EndY < secondOne.StartY) return res;

                if (firstOne.EndY == secondOne.StartY)
                {
                    res.Add(firstOne.EndX);
                    res.Add(firstOne.EndY);
                    return res;
                }

                if ((firstOne.StartY == secondOne.StartY) || (firstOne.EndY == secondOne.EndY) || ((secondOne.EndY > firstOne.EndY) && (secondOne.StartY < firstOne.EndY)))
                {
                    res.Add(firstOne.EndX);
                    res.Add(firstOne.EndY);
                    res.Add(secondOne.StartX);
                    res.Add(secondOne.StartY);
                    return res;
                }

                if ((secondOne.StartY > firstOne.StartY) && (secondOne.StartY < firstOne.EndY) &&
                    (secondOne.EndY < firstOne.EndY))
                {
                    res.Add(secondOne.StartX);
                    res.Add(secondOne.StartY);
                    res.Add(secondOne.EndX);
                    res.Add(secondOne.EndY);
                    return res;
                }

                return res;
            }
            else
            {
                if (firstOne.EndX < secondOne.StartX) return res;

                if (firstOne.EndX == secondOne.StartX)
                {
                    res.Add(firstOne.EndX);
                    res.Add(firstOne.EndY);
                    return res;
                }

                if ((firstOne.StartX == secondOne.StartX) || (firstOne.EndX == secondOne.EndX) || ((secondOne.EndX > firstOne.EndX) && (secondOne.StartX < firstOne.EndX)))
                {
                    res.Add(firstOne.EndX);
                    res.Add(firstOne.EndY);
                    res.Add(secondOne.StartX);
                    res.Add(secondOne.StartY);
                    return res;
                }

                if ((secondOne.StartX > firstOne.StartX) && (secondOne.StartX < firstOne.EndX) &&
                    (secondOne.EndX < firstOne.EndX))
                {
                    res.Add(secondOne.StartX);
                    res.Add(secondOne.StartY);
                    res.Add(secondOne.EndX);
                    res.Add(secondOne.EndY);
                    return res;
                }

                return res;
            }
        }

        /// <summary>
        /// Возвращает порядок отрезков относительно направления
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        private static (Line, Line) GetLinesOrder(Line line1, Line line2, OnOneLineType check)
        {
            bool res;
            if (check == OnOneLineType.OnOneVerticalLine)
            {
                res = (line1.StartY < line1.StartY) || ((line1.StartY == line2.StartY) && (line1.EndY < line2.EndY));
            }
            else
            {
                res = (line1.StartX < line2.StartX) || ((line1.StartX == line2.StartX) && (line1.EndX < line2.EndX));
            }

            return res ? (line1, line2) : (line2, line1);
        }

        /// <summary>
        /// Перенаправление прямых слева-направо, снизу-вверх (координаты будут идти по порядку: началоX-началоY-конецX-конецY)
        /// </summary>
        /// <param name="line"></param>
        static void RedirectLine(Line line)
        {
            double sqrtVector = Math.Sqrt(Math.Pow(line.LengthX, 2) + Math.Pow(line.LengthY, 2));

            double normX = line.LengthX / sqrtVector;
            double normY = line.LengthY / sqrtVector;
            if ((normX >= -1) && (normX <= 0) && (normY != 1))
            {
                // Если прямая направлена вниз (компонента вектора х == 0, y == -1) то нам необходимо только поменять y местами
                (line.StartY, line.EndY) = (line.EndY, line.StartY);
                line.LengthY *= -1;

                if ((normX == 0) && (normY == -1)) return;

                (line.StartX, line.EndX) = (line.EndX, line.StartX);
                line.LengthX *= -1;
            }

            return;
        }
    }
}
