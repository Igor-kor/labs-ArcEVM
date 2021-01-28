using System;
using System.Collections.Generic;
using System.Linq;

namespace lab5
{
    class Program
    {
        static void Main(string[] args)
        {

            ObcMPC.ModelWithCommonMemory();
            IndMPC.Run();
        }

        // с общей памятью
    }
    class ObcMPC
    {
        static int max = 100;
        static int countTask = 0;
        static double timeTasksWite = 0;
        static Queue<CPTask> copiesTasks;
        public class CPTask
        {
            public double TimeTask;
            public int TypeTask;
            public CPTask(double timeTask, int typeTask)
            {
                this.TimeTask = timeTask;
                this.TypeTask = typeTask;
            }
        }
        static public void AddSomeWork()
        {
            Random rnd = new Random();
            //вероятность прихода заявки 2/3.
            //если заявка не пришла, сразу же выходим
            if (rnd.Next(3) == 0) return;
            max--;
            if (max <= 0) return;
            countTask++;
            Console.WriteLine("Время ожидания для заявки {0}",TimeWite());
            timeTasksWite += TimeWite();
            //генерируем длительность заявки и ее тип
            copiesTasks.Enqueue(new ObcMPC.CPTask(rnd.Next(1, 10), 5));
            Console.WriteLine("Принята заявка ");
        }

        static public double TimeWite()
        {
            double time = 0;
            foreach (var task in copiesTasks)
            {
                time += task.TimeTask;
            }
            return time;
        }
        static public double ModelWithCommonMemory()
        {
            int processors = 2, controlTask;
            copiesTasks = new Queue<CPTask>();
            //для начала сгенерируем некоторое количество работы
            for (int i = 0; i < 10; i++) AddSomeWork();
            controlTask = copiesTasks.Count();
            double[] workingProcessors = new double[processors];
            CPTask currentTask = null;
            double minimalTask = 0;
            int taskDone = 0;

            int controlProcessor = 0;
            double totalTime = 0;
            double controlTime = 0;
            int tick = 0;
            while (copiesTasks.Count != 0 || max > 0)
            {
                tick++;
                AddSomeWork();
                totalTime += minimalTask;
                for (int i = 0; i < processors; ++i)
                {
                    if (workingProcessors[i] == 0 && copiesTasks.Count != 0)
                    {
                        ++taskDone;
                        if (taskDone == controlTask)
                        {
                            controlProcessor = i;
                        }
                        currentTask = copiesTasks.Dequeue();
                        workingProcessors[i] = currentTask.TimeTask;
                    }
                    else
                    {
                        workingProcessors[i] -= minimalTask;
                    }
                    if (taskDone >= controlTask && i == controlProcessor && workingProcessors[i] == 0) // проеверка на выполнение контрольной
                    {
                        controlTime = totalTime;
                    }

                }
                minimalTask = workingProcessors.Min();
                if (minimalTask == 0)
                {
                    double[] temp = new double[processors];
                    workingProcessors.CopyTo(temp, 0);
                    Array.Sort(temp);
                    for (int j = 0; j < processors; ++j)
                    {
                        if (temp[j] > 0)
                        {
                            minimalTask = temp[j];
                            break;
                        }
                    }
                }
            }
            Console.WriteLine("Время: {0}, {1}, среднее время ожидания заявки {2}", tick, totalTime, timeTasksWite/countTask);
            return tick;
        }
    }

    // модель МПС с индивидуальной памятью
    class IndMPC
    {
        static int countTask = 0;
        static double timeTasksWite = 0;
        static int max = 100;
        static int totalTime = 0;
        static int procCount = 2; //количество процессоров
        static int[] procs = new int[procCount]; //массив длин задач на процессорах
        static Queue<int>[] qs = new Queue<int>[procCount]; //массив очередей для каждого процессора
        static Random rnd = new Random();

        static void AddSomeWork()
        {
            //вероятность прихода заявки 2/3.
            //если заявка не пришла, сразу же выходим
            if (rnd.Next(3) == 0) return;
            max--;
            if (max <= 0) return;
            //генерируем длительность заявки и ее тип
            int work = rnd.Next(1, 10);
            int type = rnd.Next(procCount);
            Console.WriteLine("Принята заявка длиной {0}", work);
            //если процессор данного типа не занят,
            //сразу кидаем на него эту заявку
            //иначе просто ставим ее в очередь к этому процессору
            totalTime += work;
            countTask++;
            if (procs[type] <= 0)
            {
                procs[type] = work;
                Console.WriteLine("Отправлена на процессор {0}", type);
            }
            else
            {
                Console.WriteLine("Поставлена в очередь к процессору {0}", type);
                timeTasksWite += TimeWite(type);
                qs[type].Enqueue(work);
            }
        }
        static public int TimeWite(int proc)
        {
            int time = 0;
            foreach (var task in qs[proc])
            {
                time += task;
            }
            return time;
        }

        //проверка на то, что все заявки были обработаны
        static bool AllWorkDone()
        {
            foreach (Queue<int> q in qs) if (q.Count != 0) return false;
            foreach (int proc in procs) if (proc > 0) return false;
            return true;
        }

        static public void Run()
        {
            for (int i = 0; i < procCount; i++)
            {
                procs[i] = 0;
                qs[i] = new Queue<int>();
            }

            int ticks = 0;
            //для начала сгенерируем некоторое количество работы
            for (int i = 0; i < 10; i++) AddSomeWork();
            //ждем окончания обработки всех заявок
            while (!AllWorkDone() || max > 0)
            {
                //моделируем случайное событие прихода случайной заявки
                AddSomeWork();
                for (int i = 0; i < procCount; i++)
                {
                    //если процессор занят задачей,
                    //отнимаем от времени ее выполнения единицу.
                    //и тут же проверяем, не выполнилась ли задача
                    if (procs[i] != -1 && --procs[i] == 0)
                    {
                        //добавляем задачу из очереди, если еще остались
                        if (qs[i].Count() != 0) procs[i] = qs[i].Dequeue();
                        //отключаем процессор, очередь закончилась.
                        else procs[i] = -1;
                    }
                }
                ticks++;
            }
            Console.WriteLine("Время: {0}  {1}, {2}", ticks, totalTime, timeTasksWite / countTask);
            Console.ReadKey();
        }
    }
}
