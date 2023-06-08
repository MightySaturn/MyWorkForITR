using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossingLineSolverConsole
{
	internal class Program
	{
		static void Main()
		{
			List<Line> lines = GetLines();

			// Перебор каждой прямой с каждой
			for (int i = 0; i < lines.Count; i++)
			{
				for (int j = i + 1; j < lines.Count; j++)
				{
					CheckCrossing(lines[i], lines[j], i + 1, j + 1);
				}
			}

			Console.ReadLine();
		}

		/// <summary>
		/// Возвращает список отрезков заданных пользователем
		/// </summary>
		/// <returns></returns>
		private static List<Line> GetLines()
		{
			List<Line> res = new List<Line>();

			bool listen = true;
			while (listen)
			{
				if (GetCoords("Введите координаты начала отрезка: ", out (double x, double y) coord1) == false)
				{
					listen = false;
					continue;
				};

				if (GetCoords("Введите координаты конца отрезка: ", out (double x, double y) coord2) == false)
				{
					listen = false;
					continue;
				};

				Line line = new Line(coord1.x, coord1.y, coord2.x, coord2.y);

				res.Add(line);
			}

			return res;
		}

		/// <summary>
		/// Возвращает флаг корректности координат, значения возвращаются таплом через out
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="res"></param>
		/// <returns></returns>
		private static bool GetCoords(string msg, out (double, double) res)
		{
			res = (0, 0);

			Console.Write(msg);

			string conLine = Console.ReadLine();
			string[] parts = conLine.Split(',');

			if (parts.Length != 2) return false;

			for (int i = 0; i < parts.Length; i++)
			{
				parts[i] = parts[i].Replace('.', ',');
			}
			res = (Convert.ToDouble(parts[0]), Convert.ToDouble(parts[1]));

			return true;
		}

		private static void CheckCrossing(Line mainLine, Line otherLine, int mainIndex, int otherIndex)
		{
			// Проверка на совпадение и нахождение на 1 прямой соотвественно (отсекаем эти случаи сразу)
			if (mainLine.IsEqual(otherLine))
			{
				Console.WriteLine($"Прямые {mainIndex} и {otherIndex} совпадают! \n");
				Console.WriteLine($"Точки пересечения соответственно: ({mainLine.StartX}, {mainLine.StartY}) и ({mainLine.EndX}, {mainLine.EndY})");

				return;
			}

			OnOneLineType check = mainLine.CheckOneLine(otherLine);
			if (check == OnOneLineType.NotOnOneLine)
			{
				// Если не совпадают и не лежат на 1 прямой - применяем основной алгоритм
				// проводим вектора для проверки пересечения

				// т.к. работа ведётся в 2х мерном пространстве, то при векторном произведении компонента x и y обнулится
				// останется только z, её матрицу и вычисляем

				// Геометрическая проверка для двух линий, условие должно выполнятся для каждой из них
				if (mainLine.CheckMatrix(otherLine, false, out double z1, out double z2) &&
					otherLine.CheckMatrix(mainLine, true, out _, out _))
				{
					Console.WriteLine($"Прямые {mainIndex} и {otherIndex} пересекаются!\n");

					(double crossX, double crossY) = FindCrossingPoint(otherLine, z1, z2);

					Console.WriteLine($"Координаты точки пересечения: ({crossX}, {crossY})\n");

					return;
				}
			}
			else
			{
				List<(double x, double y)> answer = FindCrossingOnOneLine(mainLine, otherLine, check);
				if (answer.Any())
				{
					Console.WriteLine($"Прямые {mainIndex} и {otherIndex} пересекаются!\n");
					if (answer.Count > 1)
					{
						Console.WriteLine($"Координаты точек пересечения: ({answer[0].x}, {answer[0].y}, {answer[1].x}, {answer[1].y})\n");
					}
					else
					{
						Console.WriteLine($"Координаты точки пересечения: ({answer[0].x}, {answer[0].y})\n");
					}

					return;
				}
			}

			Console.WriteLine($"Прямые {mainIndex} и {otherIndex} НЕ пересекаются!\n");
		}

		/// <summary>
		/// Нахождение пересечения в общем случае
		/// </summary>
		/// <param name="otherLine"></param>
		/// <param name="z1"></param>
		/// <param name="z2"></param>
		/// <returns></returns>
		private static (double, double) FindCrossingPoint(Line otherLine, double z1, double z2)
		{
			// По формуле идут модули z (длины векторов), но т.к. у нас кроме компоненты z ничего и нет,
			// то по сути мы берём модуль
			double abs = Math.Abs(z1) / Math.Abs(z2 - z1);

			double crossX = otherLine.StartX + otherLine.LengthX * abs;
			double crossY = otherLine.StartY + otherLine.LengthY * abs;

			return (crossX, crossY);
		}

		/// <summary>
		/// Нахождение пересечения на одной линии
		/// </summary>
		/// <param name="line"></param>
		/// <param name="otherLine"></param>
		/// <param name="check"></param>
		/// <returns></returns>
		private static List<(double, double)> FindCrossingOnOneLine(Line line, Line otherLine, OnOneLineType check)
		{
			// Определимся с порядком прямых, сделаем так, чтобы более левые прямые рассматривались как первая, правые - вторая
			// для вертикальной прямой первая - нижняя, вторая - верхняя

			(Line firstOne, Line secondOne) = GetLinesOrder(line, otherLine, check);

			// При реализации с помощью классов я не смог найти решения для избегания 2х почти идентичных циклов
			// для вертикальной прямой случай 1, для наклонной и горизонтальной - другой
			// отличается лишь координата, по которой идёт сравнение

			List<(double, double)> res = new List<(double, double)>();

			// Здесь, зная какого типа прямой лежат 2 отрезка, будем находить точки пересечения
			if (check == OnOneLineType.OnOneVerticalLine)
			{
				if (firstOne.EndY < secondOne.StartY) return res;

				if (firstOne.EndY == secondOne.StartY)
				{
					res.Add((firstOne.EndX, firstOne.EndY));
					return res;
				}

				if (firstOne.StartY == secondOne.StartY || firstOne.EndY == secondOne.EndY ||
					(secondOne.EndY > firstOne.EndY && secondOne.StartY < firstOne.EndY))
				{
					res.Add((firstOne.EndX, firstOne.EndY));
					res.Add((secondOne.StartX, secondOne.StartY));
					return res;
				}

				if (secondOne.StartY > firstOne.StartY && secondOne.StartY < firstOne.EndY &&
					secondOne.EndY < firstOne.EndY)
				{
					res.Add((secondOne.StartX, secondOne.StartY));
					res.Add((secondOne.EndX, secondOne.EndY));
					return res;
				}
			}
			else
			{
				if (firstOne.EndX < secondOne.StartX) return res;

				if (firstOne.EndX == secondOne.StartX)
				{
					res.Add((firstOne.EndX, firstOne.EndY));
					return res;
				}

				if (firstOne.StartX == secondOne.StartX || firstOne.EndX == secondOne.EndX ||
					(secondOne.EndX > firstOne.EndX && secondOne.StartX < firstOne.EndX))
				{
					res.Add((firstOne.EndX, firstOne.EndY));
					res.Add((secondOne.StartX, secondOne.StartY));
					return res;
				}

				if (secondOne.StartX > firstOne.StartX && secondOne.StartX < firstOne.EndX &&
					secondOne.EndX < firstOne.EndX)
				{
					res.Add((secondOne.StartX, secondOne.StartY));
					res.Add((secondOne.EndX, secondOne.EndY));
					return res;
				}
			}

			return res;
		}

		/// <summary>
		/// Возвращает порядок отрезков относительно направления
		/// </summary>
		/// <param name="line"></param>
		/// <param name="otherLine"></param>
		/// <param name="check"></param>
		/// <returns></returns>
		private static (Line, Line) GetLinesOrder(Line line, Line otherLine, OnOneLineType check)
		{
			bool res;
			if (check == OnOneLineType.OnOneVerticalLine)
			{
				res = line.StartY < line.StartY || (line.StartY == otherLine.StartY && line.EndY < otherLine.EndY);
			}
			else
			{
				res = line.StartX < otherLine.StartX || (line.StartX == otherLine.StartX && line.EndX < otherLine.EndX);
			}

			return res ? (line, otherLine) : (otherLine, line);
		}
	}
}
