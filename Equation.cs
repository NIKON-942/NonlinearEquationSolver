namespace NonlinearEquationSolver
{
    public class Equation
    {
        /// <summary>
        /// Змінна, що містить коефіцієнти рівняння.
        /// </summary>
        private double[] _coefficients;

        /// <summary>
        /// Створення об'єкту для роботи з поліномом з максимальним степенем maxPower.
        /// </summary>
        /// <param name="maxPower">Максимальний степінь рівняння</param>
        /// <exception cref="ArgumentException">При введенні від'ємного максимальниго степеня</exception>
        public Equation(int maxPower)
        {
            if (maxPower < 0)
                throw new ArgumentException("Value of maxPower can`t be less than zero");
            _coefficients = new double[maxPower + 1];
        }

        /// <summary>
        /// Зміна або отримання коефіцієнту при х у степені power.
        /// </summary>
        /// <param name="power">Степінь при якому знаходиться коефіцієнт</param>
        /// <returns>Значення коефіцієнту при х у заданій степені</returns>
        /// <exception cref="ArgumentException">При виборі степеня, що виходить за межі даного рівняння</exception>
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

        /// <summary>
        /// Обчислити значення полінома у певній точці.
        /// </summary>
        /// <param name="x">Значення у якому обчислити функцію</param>
        /// <returns>Значення функції у точці х</returns>
        public double Calculate(double x)
        {
            double result = 0;
            for (int i = 0; i < _coefficients.Length; i++)
                result += _coefficients[i] * Math.Pow(x, i);
            return result;
        }

        /// <summary>
        /// Отримати нову функцію, що є похідною даного полінома.
        /// </summary>
        /// <returns>Похідну даної функції</returns>
        public Equation GetDerivative()
        {
            Equation derivative = new Equation(_coefficients.Length - 1);
            for (int i = _coefficients.Length - 1; i >= 1; i--)
                derivative[i - 1] = _coefficients[i] * i;
            return derivative;
        }

        /// <summary>
        /// Перевірка чи є функція константою (y = a).
        /// </summary>
        /// <returns><c>true</c>, якщо всі коефіцієнти, крім першого, дорівнюють нулю; в іншому випадку — <c>false</c></returns>
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

        /// <summary>
        /// Переведення функції у стрічку для запису у файл.
        /// </summary>
        /// <returns>Стрічкове представлення полінома</returns>
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
