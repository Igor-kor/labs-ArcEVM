/// Вариант 1 
/// процессоров 2, задач в пакете 5
using System;
using System.Collections.Generic;
using System.Linq;

namespace lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 10;
            int k = 2;
            int[] works = new int[n]; //массив заданий
            for (int i = 0; i < n; i++) works[i] = new Random().Next(1, 12);
            Macnoton.Run(works, k);
            LPT.Run(works, k);
        }

        static class Macnoton
        {
            public static void Run(int[] tasks, int procCount)
            {
                int[] timesOfTasks = (int[])tasks.Clone();
                //int L = 5; // кол-во заданий
                //int n = 2; // кол-во идентичных процессоров
                double theta; // вещественное значение нижней границы времени для алгоритма Макнотона
                              //лучше использовать целочисленное значение.
                double optimalT1;
                double wasteTime1;
                //int[] timesOfTasks = new int[L];
                //Random init = new Random();

                /* for (int i = 0; i < L; ++i)
                 {
                     timesOfTasks[i] = init.Next(1, 12); // случайная инициализация массива временем выполнения заданий
                 }*/
                Array.Sort(timesOfTasks); // Сортируем массив
                Array.Reverse(timesOfTasks); // делаем перестановку элементов, чтобы они были убывающими
                theta = Math.Max(timesOfTasks[0], middleOfSum(timesOfTasks, procCount));
                optimalT1 = theta;
                wasteTime1 = AlgorithmMaknoton(timesOfTasks, theta, procCount);

                Console.WriteLine("\nMacnoton Простой процессора - {0:0.0}, Оптимальное время(нижняя граница) - {1:0.0} ", wasteTime1, optimalT1);
            }
        }

        public static double AlgorithmMaknoton(int[] times, double thetaLevel, int n)
        {
            string[] table = new string[n];
            double temp = thetaLevel;
            int currentProcessor = n;
            for (int i = 0; i < times.Length; ++i)
            {
                if (temp <= 0 && currentProcessor > 1)
                {
                    temp += thetaLevel;
                    --currentProcessor;
                }
                temp -= times[i];
                if (temp >= 0)
                {
                    for (int h = 0; h < times[i]; h++)
                    {
                        table[currentProcessor - 1] += "*";
                    }
                }
                else
                {
                    for (int h = 0; h > temp; h--)
                    {
                        table[currentProcessor - 1] += " ";
                    }
                }


            }
            foreach (string tab in table)
            {
                Console.WriteLine(tab);
            }
            return temp;
        }
        public static double middleOfSum(int[] arr, int quantatityProcs)
        {
            int sum = 0;
            for (int i = 0; i < arr.Length; ++i)
            {
                sum += arr[i];
            }
            return (double)(sum / (double)quantatityProcs);
        }
    }

    static class LPT
    {
        public static void Run(int[] works, int procCount)
        {
            int q = 5; //время, уделяемое одной задаче

            List<int> p = new List<int>(); //массив процессоров
            for (int i = 0; i < procCount; i++) p.Add(0);
            Array.Sort(works);
            Array.Reverse(works);
            Queue<int> pack = new Queue<int>(); //отсортированная очередь заданий
            foreach (int work in works) pack.Enqueue(work);
            string[] table = new string[procCount];
            int ticks = 0; //количество тактов
            int prostoi = 0;
            do
            {
                for (int i = 0; i < p.Count(); i++) //перебираем все процессоры
                {
                    p[i] -= q;
                    if (ticks > 0)
                    {
                        for (int h = p[i]; h < (p[i] + q); h++)
                        {
                            if (h < 0)
                            {
                                table[i] += " ";
                                prostoi++;
                            }
                            else table[i] += "*";
                        }
                    }
                    if (p[i] <= 0) //если задача выполнена, берём следующую
                    {
                        if (pack.Count > 0) p[i] = pack.Dequeue();
                    }
                    ticks++;
                }
            } while (CheckProcessors(p)); //все ли процессоры выполнили все работу

            foreach (string tab in table)
            {
                Console.WriteLine(tab);
            }
            Console.WriteLine("Время LPT {0}, Простой процессора {1}", (ticks * q) / procCount, prostoi);
        }

        static bool CheckProcessors(List<int> processors)
        {
            foreach (int proc in processors) if (proc > 0) return true; //если хотя бы один занят, продолжаем
            return false;
        }
    }
}
