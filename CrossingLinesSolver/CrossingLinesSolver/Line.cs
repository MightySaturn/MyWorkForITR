using System;

namespace CrossingLineSolverConsole
{
	/// <summary>
	/// Тип нахождения прямых на одной прямой
	/// </summary>
	public enum OnOneLineType
	{
		NotOnOneLine,
		OnOneCommonLine,
		OnOneHorizontalLine,
		OnOneVerticalLine
	}

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

		/// <summary>
		/// Коэффициент наклона прямой
		/// </summary>
		public double KoefK { get; set; }

		/// <summary>
		/// Координата пересечения с осью OY
		/// </summary>
		public double KoefB { get; set; }

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		public Line(double x1, double y1, double x2, double y2)
		{
			StartX = x1;
			StartY = y1;
			EndX = x2;
			EndY = y2;

			LengthX = EndX - StartX;
			LengthY = EndY - StartY;

			IsVertical = StartX == EndX;

			IsHorizontal = StartY == EndY;

			IsCommon = !(IsVertical || IsHorizontal);

			Redirect();

			KoefK = (EndY - StartY) / (EndX - StartX);

			KoefB = StartY - StartX * KoefK;
		}

		/// <summary>
		/// Возвращает true если указанный отрезок имеет такие же точки
		/// </summary>
		/// <param name="otherLine"></param>
		/// <returns></returns>
		public bool IsEqual(Line otherLine)
		{
			return StartX == otherLine.StartX && StartY == otherLine.StartY && EndX == otherLine.EndX && EndY == otherLine.EndY;
		}

		/// <summary>
		/// Проверка отрезков на нахождение их на 1 прямой.
		/// </summary>
		/// <param name="otherLine"></param>
		/// <returns></returns>
		public OnOneLineType CheckOneLine(Line otherLine)
		{
			// Охарактеризуем прямые по их расположению, так проще будет понять на какой они одной линии (обычной - под углом,
			// горизонтальной или вертикальной)

			// Определяем тип нахождения на 1 линии
			if (IsCommon & otherLine.IsCommon)
			{
				bool res = (KoefK == otherLine.KoefK) && (KoefB == otherLine.KoefB);
				return res ? OnOneLineType.OnOneCommonLine : OnOneLineType.NotOnOneLine;
			}

			if (IsHorizontal & otherLine.IsHorizontal)
			{
				return StartY == otherLine.StartY ? OnOneLineType.OnOneHorizontalLine : OnOneLineType.NotOnOneLine;
			}

			if (IsVertical && otherLine.IsVertical)
			{
				return StartX == otherLine.StartX ? OnOneLineType.OnOneVerticalLine : OnOneLineType.NotOnOneLine;
			}

			return OnOneLineType.NotOnOneLine;
		}

		/// <summary>
		/// Перенаправление прямых слева-направо, снизу-вверх (координаты будут идти по порядку: началоX-началоY-конецX-конецY)
		/// </summary>
		private void Redirect()
		{
			double sqrtVector = Math.Sqrt(Math.Pow(LengthX, 2) + Math.Pow(LengthY, 2));

			double normX = LengthX / sqrtVector;
			double normY = LengthY / sqrtVector;
			if ((normX >= -1) && (normX <= 0) && (normY != 1))
			{
				// Если прямая направлена вниз (компонента вектора х == 0, y == -1) то нам необходимо только поменять y местами
				(StartY, EndY) = (EndY, StartY);
				LengthY *= -1;

				if ((normX == 0) && (normY == -1)) return;

				(StartX, EndX) = (EndX, StartX);
				LengthX *= -1;
			}

			return;
		}

		/// <summary>
		/// Возвращает true, если при вычислении матриц z1 и z2, любая матрица равна нулю или имеют разные знаки
		/// </summary>
		/// <param name="otherLine"></param>
		/// <param name="z1"></param>
		/// <param name="z2"></param>
		/// <returns></returns>
		public bool CheckMatrix(Line otherLine, bool endPoint, out double z1, out double z2)
		{
			double leftFirstVectorX = otherLine.StartX;
			double leftFirstVectorY = otherLine.StartY;
			double rightFirstVectorX = otherLine.EndX;
			double rightFirstVectorY = otherLine.EndY;
			if (endPoint)
			{
				leftFirstVectorX -= EndX;
				leftFirstVectorY -= EndY;
				rightFirstVectorX -= EndX;
				rightFirstVectorY -= EndY;
			}
			else
			{
				leftFirstVectorX -= StartX;
				leftFirstVectorY -= StartY;
				rightFirstVectorX -= StartX;
				rightFirstVectorY -= StartY;
			}

			z1 = LengthX * leftFirstVectorY - LengthY * leftFirstVectorX;
			z2 = LengthX * rightFirstVectorY - LengthY * rightFirstVectorX;

			return z1 == 0 || z2 == 0 || (z1 > 0 && z2 < 0) || (z1 < 0 && z2 > 0);
		}
	}
}