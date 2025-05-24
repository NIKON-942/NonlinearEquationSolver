using System.Diagnostics;

namespace NonlinearEquationSolver
{
    public enum Method
    {
        Bisection,
        Newton,
        Secant
    }

    public class EquationSolver
    {
        /// <summary>
        /// Змінна для об'єкту рівняння.
        /// </summary>
        private Equation _equation;

        /// <summary>
        /// Змінна для об'єкту таймера.
        /// </summary>
        private Stopwatch _timer;

        /// <summary>
        /// Максимальна кількість ітерації, щоб уникнути "вічних" циклів.
        /// </summary>
        public const int MaxIterations = 1000000;

        /// <summary>
        /// Допустима похибка для перевірки на корінь та на степінь 10.
        /// </summary>
        private const double Tolerance = 1e-3;

        /// <summary>
        /// Значення необхідні для пошуку наближеного розв'язку.
        /// </summary>
        public double FirstValue { get; set; }
        public double SecondValue { get; set; }
        public double Precision { get; set; }

        /// <summary>
        /// Змінні для обчислення значень для практичної складності.
        /// </summary>
        public int CountOfCalculations { get; private set; }
        public int CountOfIterations { get; private set; }

        /// <summary>
        /// Отримання часу витраченого на пошук наближення.
        /// </summary>
        public double ElapsedMilliseconds {
            get
            {
                return _timer.Elapsed.TotalMilliseconds;
            } 
        }

        /// <summary>
        /// Створення об'єкту для пошуку розв'язку рівняння equation.
        /// </summary>
        /// <param name="equation">Рівняння для розв'язку</param>
        public EquationSolver(Equation equation)
        {
            this._equation = equation;
            _timer = new Stopwatch();
        }

        /// <summary>
        /// Перевірка, чи застосовний метод бісекції для введених значень.
        /// </summary>
        /// <returns><c>true</c>, якщо значення функції <c>_equation</c> на кінцях інтервалу <c>[FirstValue, SecondValue]</c> мають різні знаки; в іншому випадку — <c>false</c></returns>
        public bool IsBisectionApplied()
        {
            return _equation.Calculate(FirstValue) * _equation.Calculate(SecondValue) < 0;
        }

        /// <summary>
        /// Перевірка чи застосовний метод Ньютона для введених значень.
        /// </summary>
        /// <returns><c>true</c>, якщо добуток значення функції <c>_equation</c> та її другої похідної в точці <c>FirstValue</c> є додатним; в іншому випадку — <c>false</c></returns>
        public bool IsNewtonApplied()
        {
            return _equation.Calculate(FirstValue) * _equation.GetDerivative().GetDerivative().Calculate(FirstValue) > 0;
        }

        /// <summary>
        /// Функція для пошуку наближеного розв'язку методом бісекції.
        /// </summary>
        /// <returns>Наближене значення кореня рівняння</returns>
        public double BisectionMethod()
        {
            double tempX, a = FirstValue, b = SecondValue, calculatedA;
            int maxIterations = MaxIterations;

            // Обнулення значень для обрахунку практичної складності.
            CountOfIterations = 0;
            CountOfCalculations = 0;
            _timer.Reset();
            _timer.Start();
            do 
            {
                tempX = (a + b) / 2;
                calculatedA = _equation.Calculate(a);
                CountOfCalculations++;
                if (calculatedA / Math.Abs(calculatedA) * _equation.Calculate(tempX) <= 0)
                    b = tempX;
                else
                    a = tempX;
                CountOfIterations++;
            } while (Math.Abs(b - a) > Precision && maxIterations-- > 0);

            tempX = (a + b) / 2;
            _timer.Stop();
            return tempX;
        }

        /// <summary>
        /// Функція для пошуку наближеного розв'язку методом Ньютона.
        /// </summary>
        /// <returns>Наближене значення кореня рівняння</returns>
        public double NewtonMethod()
        {
            double xk = FirstValue, xk1 = xk, difference;
            int maxIterations = MaxIterations;
            Equation derivative = _equation.GetDerivative();

            // Обнулення значень для обрахунку практичної складності.
            CountOfIterations = 0;
            CountOfCalculations = 0;
            _timer.Reset();
            _timer.Start();
            do
            {
                double derivativeValue = derivative.Calculate(xk);
                if (Math.Abs(derivativeValue) < Precision)
                    derivativeValue += Precision;
                difference = _equation.Calculate(xk) / derivativeValue;
                CountOfCalculations += 2;
                (xk1, xk) = (xk - difference, xk1);
                CountOfIterations++;
            } while (Math.Abs(difference) >= Precision && maxIterations-- > 0);
            _timer.Stop();
            return xk1;
        }

        /// <summary>
        /// Функція для пошуку наближеного розв'язку методом січних.
        /// </summary>
        /// <returns>Наближене значення кореня рівняння</returns>
        public double SecantMethod()
        {
            double xk0 = FirstValue, xk1 = SecondValue, xk2 = 0, difference;
            int maxIterations = MaxIterations;

            // Обнулення значень для обрахунку практичної складності.
            CountOfIterations = 0;
            CountOfCalculations = 0;
            _timer.Reset();
            _timer.Start();
            do
            {
                double calculatedXK1 = _equation.Calculate(xk1);
                double denominator = calculatedXK1 - _equation.Calculate(xk0);
                if (Math.Abs(denominator) < Precision)
                    denominator += Precision;
                difference = (xk1 - xk0) * calculatedXK1 / denominator;
                CountOfCalculations += 2;
                xk2 = xk1 - difference;
                (xk0, xk1) = (xk1, xk2);
                CountOfIterations++;
            } while (Math.Abs(difference) >= Precision && maxIterations-- > 0);
            _timer.Stop();
            return xk2;
        }

        /// <summary>
        /// Перевірка чи є значення наближеним коренем equation.
        /// </summary>
        /// <param name="value">Значення, яке перевіряється на те, чи є воно коренем</param>
        /// <returns><c>true</c>, якщо значення є коренем; в іншому випадку — <c>false</c></returns>
        public bool IsRoot(double value)
        {
            return _equation.Calculate(value - Precision * 4) * _equation.Calculate(value + Precision * 4) < 0 ||
                Math.Abs(_equation.Calculate(value)) < Tolerance;
        }

        /// <summary>
        /// Перевірка чи є precision степенем 10.
        /// </summary>
        /// <param name="precision">Значення точності, яке перевіряється</param>
        /// <returns><c>true</c>, якщо значення точності є коректним; в іншому випадку — <c>false</c></returns>
        public static bool IsPowerOfTen(double precision)
        {
            double log10 = Math.Log10(precision);
            return Math.Abs(log10 - Math.Round(log10)) < Tolerance;
        }

        /// <summary>
        /// Визначає кількість знаків після коми для заданого значення точності.
        /// </summary>
        /// <param name="precision">Значення точності</param>
        /// <returns>Кількість десяткових знаків у <paramref name="precision"/></returns>
        public static int GetDecimalPlacesOfPrecision(double precision)
        {
            string precisionString = precision.ToString("0.#############");
            return precisionString.Length - precisionString.IndexOf(',') - 1;
        }
    }
}
