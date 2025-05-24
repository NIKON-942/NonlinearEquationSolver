using System.IO;

namespace NonlinearEquationSolver
{
    public class ResultWriter
    {
        /// <summary>
        /// Рівняння, для якого записуються результати.
        /// </summary>
        private Equation _equation;

        /// <summary>
        /// Розв'язувач рівнянь, що містить параметри та результати обчислень.
        /// </summary>
        private EquationSolver _equationSolver;

        /// <summary>
        /// Константина назва вихідного файлу для збереження результатів.
        /// </summary>
        private const string FileName = "result.txt";

        /// <summary>
        /// Ініціалізує новий екземпляр класу.
        /// </summary>
        /// <param name="equation">Рівняння, результати розв'язання якого будуть записані</param>
        /// <param name="equationSolver">Розв'язувач рівнянь, що використовувався для знаходження розв'язку та містить відповідні дані</param>
        public ResultWriter(Equation equation, EquationSolver equationSolver) {
            _equation = equation;
            _equationSolver = equationSolver;
        }

        /// <summary>
        /// Записує знайдену відповідь у файл.
        /// </summary>
        /// <param name="method">Метод розв'язання</param>
        /// <param name="result">Результат наближення кореня</param>
        /// <param name="complexity">Чи потрібно записувати складність у файл</param>
        /// <param name="newFile">Чи потрібно перестворювати файл</param>
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