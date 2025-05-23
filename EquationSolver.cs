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
        private Equation _equation;
        private Stopwatch _timer;

        // Максимальна кількість ітерації, щоб уникнути "вічних" циклів.
        public const int MaxIterations = 1000000;

        // Допустима похибка для перевірки на корінь та на степінь 10.
        private const double Tolerance = 1e-3;

        // Значення необхідні для пошуку наближеного розв'язку.
        public double FirstValue { get; set; }
        public double SecondValue { get; set; }
        public double Precision { get; set; }

        // Обчислення значень для практичної складності.
        public int CountOfCalculations { get; private set; }
        public int CountOfIterations { get; private set; }

        // Отримання часу витраченого на пошук наближення.
        public double ElapsedMilliseconds {
            get
            {
                return _timer.Elapsed.TotalMilliseconds;
            } 
        }

        // Створення об'єкту для пошуку розв'язку рівняння equation.
        public EquationSolver(Equation equation)
        {
            this._equation = equation;
            _timer = new Stopwatch();
        }

        // Перевірка, чи застосовний метод бісекції для введених значень.
        public bool IsBisectionApplied()
        {
            return _equation.Calculate(FirstValue) * _equation.Calculate(SecondValue) < 0;
        }

        // Перевірка чи застосовний метод Ньютона для введених значень.
        public bool IsNewtonApplied()
        {
            return _equation.Calculate(FirstValue) * _equation.GetDerivative().GetDerivative().Calculate(FirstValue) > 0;
        }
        
        // Функція для пошуку наближеного розв'язку методом бісекції.
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

        // Функція для пошуку наближеного розв'язку методом Ньютона.
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

        // Функція для пошуку наближеного розв'язку методом січних.
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

        // Перевірка чи є значення наближеним коренем equation.
        public bool IsRoot(double value)
        {
            return _equation.Calculate(value - Precision * 4) * _equation.Calculate(value + Precision * 4) < 0 ||
                Math.Abs(_equation.Calculate(value)) < Tolerance;
        }

        // Перевірка чи є precision степенем 10.
        public static bool IsPowerOfTen(double precision)
        {
            double log10 = Math.Log10(precision);
            return Math.Abs(log10 - Math.Round(log10)) < Tolerance;
        }

        // Знайти кількість знаків precision після коми (кількість точних знаків розв'язку після коми).
        public static int GetDecimalPlacesOfPrecision(double precision)
        {
            string precisionString = precision.ToString("0.#############");
            return precisionString.Length - precisionString.IndexOf(',') - 1;
        }
    }
}
