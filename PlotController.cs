using ScottPlot;
using ScottPlot.WPF;

namespace NonlinearEquationSolver
{
    class PlotController
    {
        /// <summary>
        /// Посилання на графік у графічному інтерфейсі.
        /// </summary>
        private WpfPlot _current;

        /// <summary>
        /// Змінна для об'єкту рівняння.
        /// </summary>
        private Equation _equation;

        /// <summary>
        /// Масив значень по осі X, що використовуються для побудови графіка.
        /// </summary>
        private double[] _dataX;

        /// <summary>
        /// Визначає кількість кроків, що використовуються для генерації даних для побудови графіка функції.
        /// </summary>
        private const int CountOfSteps = 100000;

        /// <summary>
        /// Внутрішнє поле для зберігання лівої межі графіка.
        /// </summary>
        private double _left = -100;

        /// <summary>
        /// Отримує або встановлює ліву межу для побудови графіка.
        /// </summary>
        public double Left
        {
            set
            {
                if (value > _right)
                    throw new ArgumentException("Incorrect left limit value");
                _left = value;
                _right = 100;
                _step = (_right - _left) / CountOfSteps;
                _dataX = Generate.Range(_left, _right, _step);
            }
            get
            {
                return _left;
            }
        }

        /// <summary>
        /// Внутрішнє поле для зберігання правої межі графіка.
        /// </summary>
        private double _right = 100;

        /// <summary>
        /// Отримує або встановлює праву межу для побудови графіка.
        /// </summary>
        public double Right
        {
            set
            {
                if (value < _left)
                    throw new ArgumentException("Incorrect left limit value");
                _right = value;
                _left = -100;
                _step = (_right - _left) / CountOfSteps;
                _dataX = Generate.Range(_left, _right, _step);
            }
            get
            {
                return _right;
            }
        }

        /// <summary>
        /// Крок для побудови графіку по точках.
        /// </summary>
        private double _step;

        /// <summary>
        /// Створення об'єкту для побудови графіку equation.
        /// </summary>
        /// <param name="current">Елемент на якому буде відображатися графік</param>
        /// <param name="equation">Рівняння, графік якого потрібно побудувати</param>
        public PlotController(WpfPlot current, Equation equation)
        {
            this._current = current;
            this._equation = equation;
            _step = (_right - _left) / CountOfSteps;
            _dataX = Generate.Range(_left, _right, _step);
        }

        /// <summary>
        /// Додає осі координат (абсцис та ординат) до графіка у вигляді перехрестя.
        /// </summary>
        private void AddAxis()
        {
            var crosshair = _current.Plot.Add.Crosshair(0, 0);
            crosshair.LineColor = Colors.Black;
            crosshair.LineWidth = 1.5f;
        }

        /// <summary>
        /// Оновлює графік, перераховуючи значення функції для поточного діапазону та перемальовує його на елементі.
        /// </summary>
        public void Update()
        {
            List<double> dataY = [];
            foreach (double x in _dataX)
                dataY.Add(_equation.Calculate(x));
            _current.Plot.Clear();
            _current.Plot.Add.ScatterLine(_dataX, dataY.ToArray());
            AddAxis();
            _current.Refresh();
        }

        /// <summary>
        /// Скидає межі відображення графіка.
        /// </summary>
        public void ResetLimits()
        {
            _left = -100;
            _right = 100;
            _step = (_right - _left) / CountOfSteps;
            _dataX = Generate.Range(_left, _right, _step);
        }

        /// <summary>
        /// Очищує область графіка.
        /// </summary>
        public void Clear()
        {
            _current.Plot.Clear();
            AddAxis();
            _current.Refresh();
        }
    }
}