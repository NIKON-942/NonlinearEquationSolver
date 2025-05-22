using ScottPlot;
using ScottPlot.WPF;

namespace WPF
{
    class PlotController
    {
        private WpfPlot _current;
        private Equation _equation;
        private double[] _dataX;

        private const int CountOfSteps = 200000;

        // Стандартні межі для побудови графіку, при їх зміні генеруватимуться нові значення х для побудови.
        private double _left = -100;
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

        private double _right = 100;
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

        // Стандартний крок для побудови графіку по точках, при його зміні генеруватимуться нові значення х для побудови.
        private double _step;

        // Створення об'єкту для побудови графіку equation.
        public PlotController(WpfPlot current, Equation equation)
        {
            this._current = current;
            this._equation = equation;
            _step = (_right - _left) / CountOfSteps;
            _dataX = Generate.Range(_left, _right, _step);
        }

        // Додати вісі абсцис і ординат до графіку
        private void AddAxis()
        {
            var crosshair = _current.Plot.Add.Crosshair(0, 0);
            crosshair.LineColor = Colors.Black;
            crosshair.LineWidth = 1.5f;
        }

        // Оновити графік з новими значеннями х.
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

        // Повернути обмеження малювання графіку за замовчуванням.
        public void ResetLimits()
        {
            _left = -100;
            _right = 100;
            _step = (_right - _left) / CountOfSteps;
            _dataX = Generate.Range(_left, _right, _step);
        }

        // Очистити графік залишивши лише вісі.
        public void Clear()
        {
            _current.Plot.Clear();
            AddAxis();
            _current.Refresh();
        }
    }
}
