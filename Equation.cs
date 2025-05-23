namespace NonlinearEquationSolver
{
    public class Equation
    {
        private double[] _coefficients;

        // Створення об'єкту для роботи з поліномом з максимальним степенем maxPower.
        public Equation(int maxPower)
        {
            if (maxPower < 0)
                throw new ArgumentException("Value of maxPower can`t be less than zero");
            _coefficients = new double[maxPower + 1];
        }

        // Зміна або отримання коефіцієнту при х у степені power.
        public double this[int power]
        {
            get
            {
                if (power < 0 || power > _coefficients.Length)
                    throw new ArgumentException("Incorrect power value");
                return _coefficients[power];
            }
            set
            {
                if (power < 0 || power > _coefficients.Length)
                    throw new ArgumentException("Incorrect power value");
                _coefficients[power] = value;
            }
        }

        // Обчислити значення функції у певній точці.
        public double Calculate(double x)
        {
            double result = 0;
            for (int i = 0; i < _coefficients.Length; i++)
                result += _coefficients[i] * Math.Pow(x, i);
            return result;
        }

        // Отримати нову функцію, що є похідною даної функції.
        public Equation GetDerivative()
        {
            Equation derivative = new Equation(_coefficients.Length - 1);
            for (int i = _coefficients.Length - 1; i >= 1; i--)
                derivative[i - 1] = _coefficients[i] * i;
            return derivative;
        }

        // Перевірка чи є функція константою (y = a).
        public bool IsConstant()
        {
            bool result = true;
            for (int i = 1; i < _coefficients.Length; i++)
                if (_coefficients[i] != 0)
                {
                    result = false;
                    break;
                }
            return result;
        }

        // Переведення функції у стрічку для запису у файл.
        public override string ToString()
        {
            string result = "";
            for (int i = _coefficients.Length - 1; i >= 0; i--)
            {
                if (_coefficients[i] != 0 && i > 1)
                {
                    if (result.Length > 0)
                    {
                        result += _coefficients[i] > 0 ? "+ " : "- ";
                        result += $"{Math.Abs(_coefficients[i])} * x^{i} ";
                    } else
                    {
                        result += $"{_coefficients[i]} * x^{i} ";
                    }
                }
                else if (_coefficients[i] != 0 && i == 1)
                {
                    if (result.Length > 0)
                    {
                        result += _coefficients[i] > 0 ? "+ " : "- ";
                        result += $"{Math.Abs(_coefficients[i])} * x ";
                    }
                    else
                    {
                        result += $"{_coefficients[i]} * x ";
                    }
                }
                else if (_coefficients[i] != 0)
                {
                    if (result.Length > 0)
                    {
                        result += _coefficients[i] > 0 ? "+ " : "- ";
                        result += $"{Math.Abs(_coefficients[i])}";
                    }
                    else
                    {
                        result += $"{_coefficients[i]}";
                    }
                }
            }
            return result;
        }
    }
}
