using System.IO;
using WPF;

namespace NonlinearEquationSolver
{
    public class ResultWriter
    {
        private Equation _equation;
        private EquationSolver _equationSolver;

        // Назва вихідного файлу.
        private const string FileName = "result.txt";

        public ResultWriter(Equation equation, EquationSolver equationSolver) {
            this._equation = equation;
            this._equationSolver = equationSolver;
        }

        // Записує знайдену відповідь у файл.
        public void WriteResult(Method method, double result, bool complexity, bool newFile)
        {
            using (StreamWriter writer = new StreamWriter(FileName, !newFile))
            {
                writer.WriteLine($"Рівняння: {_equation} = 0");
                writer.Write("Метод: ");

                // Запис вхідних даних для методів.
                switch (method)
                {
                    case Method.Bisection:
                        writer.WriteLine("Половинного ділення (бісекції)");
                        writer.WriteLine($"Межі: від {_equationSolver.FirstValue} до {_equationSolver.SecondValue}");
                        break;
                    case Method.Newton:
                        writer.WriteLine($"Ньютона (дотичних)");
                        writer.WriteLine($"Початкове наближення: {_equationSolver.FirstValue}");
                        break;
                    case Method.Secant:
                        writer.WriteLine($"Січних");
                        writer.WriteLine($"Початкові наближення: {_equationSolver.FirstValue} та {_equationSolver.SecondValue}");
                        break;
                }
                writer.WriteLine($"Точність: {_equationSolver.Precision}");

                // Запис практичної складності у файл за потреби.
                if (complexity)
                {
                    writer.WriteLine($"Витрачено часу: {_equationSolver.ElapsedMilliseconds} мс");
                    writer.WriteLine($"Кількість ітерацій: {_equationSolver.CountOfIterations}");
                    writer.WriteLine($"Кількість обчислень функції: {_equationSolver.CountOfCalculations}");
                }
                writer.WriteLine($"Корінь: x = {result:G15}\n");
            }
        }
    }
}