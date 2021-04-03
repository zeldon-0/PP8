using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PP6;

namespace PP8
{
    class Program
    {
        private static int[,] _matrix;
        private static int[] _vector;
        private static int[] _result;
        private static List<int> _resultList = new List<int>();

        static void Main(string[] args)
        {
            //int size = 10000;
            int maxDegreesOfParallelism = 4;
            //CalculateInOrder(size);
            //CalculateWithParallelFor(size, maxDegreesOfParallelism);
            foreach (var i in Enumerable.Range(0, 11))
            {
                int size;
                if (i == 0)
                {
                    size = 100;
                }
                else
                {
                    size = i * 1000;
                }
                Console.WriteLine(size);
                CalculateWithTasks(size, maxDegreesOfParallelism);
            }
        }

        private static void CalculateInOrder(int size)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeInputData(size);
            _result = MultiplicationHelper.Multiply(_matrix, _vector);
            stopWatch.Stop();
            Console.WriteLine($"[In-Order Algorithm] Time elapsed: {stopWatch.Elapsed} ms");
        }

        private static void CalculateWithParallelFor(int size, int degreesOfParallelism)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeInputData(size);
            _result = new int[size];

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = degreesOfParallelism
            };

            Parallel.For(0, size, options, (index) =>
            {
                var matrixRow = Enumerable.Range(0, _matrix.GetLength(1))
                    .Select(x => _matrix[index, x])
                    .ToArray();
                _result[index] = MultiplicationHelper.Multiply(matrixRow, _vector);
            });
            stopWatch.Stop();
            Console.WriteLine($"[Algorithm with ParallelFor] Time elapsed: {stopWatch.Elapsed} ms");
        }

        private static void CalculateWithTasks(int size, int degreesOfParallelism)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeInputData(size);
            _result = new int[size];
            List<Task> tasks = new List<Task>();
            ThreadPool.SetMaxThreads(degreesOfParallelism, degreesOfParallelism);
            for (int i = 0; i < size; i++)
            {
                var task = CalculateForMatrixRow(i);
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            stopWatch.Stop();
            Console.WriteLine($"[Algorithm with Tasks] Time elapsed: {stopWatch.Elapsed} ms");
        }

        private static Task CalculateForMatrixRow(int index)
        {
            var task = Task.Run(() =>
                {
                    var matrixRow = Enumerable.Range(0, _matrix.GetLength(1))
                        .Select(x => _matrix[index, x])
                        .ToArray();
                    _result[index] = MultiplicationHelper.Multiply(matrixRow, _vector);
                }
            );
            return task;
        }

        private static void InitializeInputData(int vectorLength)
        {
            _matrix = DataGenerator.GenerateMatrix(vectorLength);
            _vector = DataGenerator.GenerateVector(vectorLength);
        }

        private static void PrintInput()
        {
            Console.WriteLine("Initial matrix:");
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    Console.Write($"{_matrix[i, j]}\t");
                }
                Console.Write('\n');
            }

            Console.WriteLine("Initial vector:");
            for (int i = 0; i < _vector.Length; i++)
            {
                Console.Write($"{_vector[i]}\t");
            }
            Console.Write('\n');
        }

        private static void PrintOutput()
        {

            Console.WriteLine("Resulting vector:");
            for (int i = 0; i < _result.Length; i++)
            {
                Console.Write($"{_result[i]}\t");
            }
            Console.Write('\n');
        }
    }
}
