using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace NonlinearEquationSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Змінна для об'єкту рівняння
        /// </summary>
        private Equation _equation;

        /// <summary>
        /// Змінна для об'єкту контролера графіком
        /// </summary>
        private PlotController _plot;

        /// <summary>
        /// Змінна для об'єкту розв'язувача рівняння
        /// </summary>
        private EquationSolver _equationSolver;

        /// <summary>
        /// Змінна для об'єкту запису у файл
        /// </summary>
        private ResultWriter _fileWriter;

        /// <summary>
        /// Межі на введення даних від користувача (окрім точності)
        /// </summary>
        private const double MinAbsoluteValue = 1e-3;
        private const double MaxAbsoluteValue = 1e3;

        /// <summary>
        /// Межі на введення точності від користувача
        /// </summary>
        private const double MinPrecisionValue = 1e-12;
        private const double MaxPrecisionValue = 1e-2;

        public MainWindow()
        {
            InitializeComponent();
            _equation = new Equation(10);
            _plot = new PlotController(Plot, _equation);
            _equationSolver = new EquationSolver(_equation);
            _fileWriter = new ResultWriter(_equation, _equationSolver);
            _plot.Clear();
            foreach (TextBox tb in Coefficients.Children.OfType<StackPanel>().Select(n => n.Children.OfType<TextBox>().First()))
                DataObject.AddPastingHandler(tb, TextPasteHandler);
            DataObject.AddPastingHandler(FirstValue, TextPasteHandler);
            DataObject.AddPastingHandler(SecondValue, TextPasteHandler);
            DataObject.AddPastingHandler(Precision, TextPasteHandler);
        }

        /// <summary>
        /// Обробляє зміну вибору степеня рівняння.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію</param>
        /// <param name="e">Параматри події</param>
        private void PowerSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Power0 == null)
                return;

            int count = 8 - PowerSelection.SelectedIndex;
            List<StackPanel> panels = Coefficients.Children.OfType<StackPanel>().ToList();
            panels.Reverse();
            foreach (StackPanel panel in panels[3..])
            {
                if (count-- <= 0)
                {
                    panel.Visibility = Visibility.Collapsed;
                    panel.Children.OfType<TextBox>().First().Text = "";
                }
                else
                {
                    panel.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Обробляє зміну вибору методу розв'язання рівняння.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію</param>
        /// <param name="e">Параматри події</param>
        private void MethodSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomValues == null)
                return;

            FirstValue.Text = "";
            _equationSolver.FirstValue = 0;
            SecondValue.Text = "";
            _equationSolver.SecondValue = 0;
            switch ((Method)MethodSelection.SelectedIndex)
            {
                case Method.Bisection:
                    FirstValueLabel.Text = "Від:";
                    SecondValueLabel.Text = "До:";
                    SecondValuePanel.Visibility = Visibility.Visible;
                    CustomValues.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                    break;
                case Method.Newton:
                    FirstValueLabel.Text = "Початкове наближення:";
                    SecondValuePanel.Visibility = Visibility.Collapsed;
                    CustomValues.ColumnDefinitions[1].Width = GridLength.Auto;
                    break;
                case Method.Secant:
                    FirstValueLabel.Text = "X0:";
                    SecondValueLabel.Text = "X1:";
                    SecondValuePanel.Visibility = Visibility.Visible;
                    CustomValues.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                    break;
            }
        }

        /// <summary>
        /// Обробляє введення тексту користувачем.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію</param>
        /// <param name="e">Параматри події</param>
        private void TextInputHandler(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            string currentText = textBox.Text;
            int caretIndex = textBox.CaretIndex;

            string potentialText = currentText.Remove(textBox.SelectionStart, textBox.SelectionLength).Insert(caretIndex, e.Text);

            bool isValid = double.TryParse(potentialText.Replace(".", ","), NumberStyles.Float, CultureInfo.CurrentCulture, out double parsedValue);

            if (!isValid && potentialText == "-")
                isValid = true;

            if (!isValid)
            {
                e.Handled = true;
                MessageBox.Show($"Дозволено вводити тільки дійсні числа у форматі \"123.45\".\nРоздільники: ',' та '.'",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (potentialText.Length >= 20)
            {
                e.Handled = true;
                MessageBox.Show($"Введено занадто довге число",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            parsedValue = Math.Abs(parsedValue);
            if (textBox != Precision && parsedValue != 0 && (parsedValue < MinAbsoluteValue || parsedValue > MaxAbsoluteValue))
            {
                e.Handled = true;
                MessageBox.Show($"Для коректної роботи програми використовуйте числа з діапазону [{-MaxAbsoluteValue:G15}; {-MinAbsoluteValue:G15}] або [{MinAbsoluteValue:G15}; {MaxAbsoluteValue:G15}]",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (textBox == Precision && parsedValue != 0 && (parsedValue < MinPrecisionValue || parsedValue > MaxPrecisionValue))
            {
                e.Handled = true;
                MessageBox.Show($"Для коректної роботи програми використовуйте точність у форматі \"0.00001\" з діапазону [{MinPrecisionValue:G15}; {MaxPrecisionValue:G15}]",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Обробляє вставку тексту.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію</param>
        /// <param name="e">Параматри події</param>
        private void TextPasteHandler(object sender, DataObjectPastingEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null || !e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string pastedText = (string)e.DataObject.GetData(typeof(string));
            string currentText = textBox.Text;
            string textAfterPaste = currentText.Remove(textBox.SelectionStart, textBox.SelectionLength).Insert(textBox.CaretIndex, pastedText).Replace(".", ",");

            if (!double.TryParse(textAfterPaste, NumberStyles.Any, CultureInfo.CurrentCulture, out double parsedValue))
            {
                e.CancelCommand();
                return;
            }

            if (textAfterPaste.Length >= 20)
            {
                e.CancelCommand();
                Dispatcher.BeginInvoke(new Action(() => MessageBox.Show($"Введено занадто довге число",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning)));
                return;
            }

            parsedValue = Math.Abs(parsedValue);
            if (textBox != Precision && parsedValue != 0 && (parsedValue < MinAbsoluteValue || parsedValue > MaxAbsoluteValue))
            {
                e.CancelCommand();
                Dispatcher.BeginInvoke(new Action(() => MessageBox.Show($"Для коректної роботи програми використовуйте числа з діапазону [{-MaxAbsoluteValue:G15}; {-MinAbsoluteValue:G15}] або [{MinAbsoluteValue:G15}; {MaxAbsoluteValue:G15}]",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning)));
            }
            else if (textBox == Precision && parsedValue != 0 && (parsedValue < MinPrecisionValue || parsedValue > MaxPrecisionValue))
            {
                e.CancelCommand();
                Dispatcher.BeginInvoke(new Action(() => MessageBox.Show($"Для коректної роботи програми використовуйте точність у форматі \"0.00001\" з діапазону [{MinPrecisionValue:G15}; {MaxPrecisionValue:G15}]",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning)));
            }
        }

        /// <summary>
        /// Оновлює коефіцієнти рівняння при зміні тексту в відповідному полі.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію</param>
        /// <param name="e">Параматри події</param>
        private void CoefficientsUpdate(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            int power = int.Parse(textBox.Name[11..]);
            if (double.TryParse(textBox.Text.Replace(".", ","), out double result))
                _equation[power] = result;
            else
                _equation[power] = 0;

            _plot.ResetLimits();
            _plot.Update();
        }

        /// <summary>
        /// Обробляє натискання кнопки "Обчислити".
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію</param>
        /// <param name="e">Параматри події</param>
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            double root = 0;
            Method selectedMethod = (Method)MethodSelection.SelectedIndex;
            string errorMessage = "";
            ResultLabel.Content = "";

            if (_equation.IsConstant())
                errorMessage += "Введіть рівняння, що не є константою\n";

            // Отримання потрібних значень від користувача і перевірка їх коректності.
            // Отримання першого параметру.
            if (!double.TryParse(FirstValue.Text.Replace(".", ","), out double first) || FirstValue.Text.Length == 0)
                errorMessage += "Некоректне значення першого параметру\n";
            else
                _equationSolver.FirstValue = first;

            // Отримання другого параметру.
            if ((!double.TryParse(SecondValue.Text.Replace(".", ","), out double second) || SecondValue.Text.Length == 0) && (selectedMethod == Method.Bisection || selectedMethod == Method.Secant))
                errorMessage += "Некоректне значення другого параметру\n";
            else if (selectedMethod == Method.Bisection && second < first)
                errorMessage += "Значення правої межі має бути більше за ліву\n";
            else if (selectedMethod == Method.Secant && second == first)
                errorMessage += "Значення початкових наближень мають бути різними\n";
            else
                _equationSolver.SecondValue = second;

            // Отримання точності.
            if (!double.TryParse(Precision.Text.Replace(".", ","), out double precision) || Precision.Text.Length == 0)
                errorMessage += "Некоректне значення точності\n";
            else if (!EquationSolver.IsPowerOfTen(precision))
                errorMessage += "Точність має бути степенем 10";
            else
                _equationSolver.Precision = precision;

            // Вивід повідомлення за умови некоректного вводу.
            if (errorMessage.Length > 0)
            {
                MessageBox.Show(errorMessage, "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Пошук кореня за допомогою відповідного методу.
            switch (selectedMethod)
            {
                case Method.Bisection:
                    if (_equationSolver.IsBisectionApplied())
                    {
                        root = _equationSolver.BisectionMethod();
                    } 
                    else
                    {
                        MessageBox.Show($"Для введених меж неможливо розв'язати задане рівняння методом половинного ділення. Спробуйте обрати інший метод або межі, з різними знаками функції на кінцях.",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    break;
                case Method.Newton:
                    if (_equationSolver.IsNewtonApplied())
                    {
                        root = _equationSolver.NewtonMethod();
                    }
                    else
                    {
                        MessageBox.Show($"Для введеного наближення неможливо розв'язати задане рівняння методом Ньютона. Спробуйте обрати інший метод або наближення, що відповідає умові: f(x) * f''(x) > 0.",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    break;
                case Method.Secant:
                    root = _equationSolver.SecantMethod();
                    break;
            }

            // Перевірка чи було знайдено корінь для методів Ньютона та січних, бо метод половинного ділення повністю збіжний і завжди знаходить корінь за умови знакозміни на кінцях відрізку.
            if (selectedMethod != Method.Bisection && (!_equationSolver.IsRoot(root) || _equationSolver.CountOfIterations > EquationSolver.MaxIterations))
            {
                MessageBox.Show($"Даний метод не знайшов коренів рівняння. Спробуйте обрати інший метод або наближення.",
                                "Некоректне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Добудова частини графіку, якщо корінь лежить поза межами побудованого графіку.
            _plot.ResetLimits();
            if (root > _plot.Right)
                _plot.Right = root + 50;
            else if (root < _plot.Left)
                _plot.Left = root - 50;
            _plot.Update();

            // Прибрати неточну частину відповіді.
            double factor = Math.Pow(10, EquationSolver.GetDecimalPlacesOfPrecision(precision));
            root = Math.Truncate(root * factor) / factor;

            // Вивід відповіді і інших даних користувачу.
            ResultLabel.Content += $"Відповідь: x = {root:G15}\n";
            if (ComplexityCheckBox.IsChecked == true)
            {
                ResultLabel.Content += $"Витрачено часу: {_equationSolver.ElapsedMilliseconds} мс\nКількість ітерацій: {_equationSolver.CountOfIterations}\nКількість обчислень функції: {_equationSolver.CountOfCalculations}\n";
            }
            if (FileCheckBox.IsChecked == true)
            {
                _fileWriter.WriteResult(selectedMethod, root, ComplexityCheckBox.IsChecked!.Value, NewFileCheckBox.IsChecked!.Value);
                ResultLabel.Content += "Результат записано у файл result.txt";
            }
        }

        /// <summary>
        /// Обробляє зміну стану чекбоксу "Новий файл".
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію</param>
        /// <param name="e">Параматри події</param>
        private void NewFileCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox writeToFile = sender as CheckBox;
            if (writeToFile == null)
                return;

            if (writeToFile.IsChecked == true)
            {
                NewFileCheckBox.Visibility = Visibility.Visible;
            }
            else if (writeToFile.IsChecked == false)
            {
                NewFileCheckBox.Visibility = Visibility.Collapsed;
                NewFileCheckBox.IsChecked = false;
            }
        }
    }
}