using System;
using System.Collections.Generic;

namespace lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            // Алгоритма SPT через алгоритм RR
            int KL = 3; //длительность кванта времени, уделяемого одной задаче
            int L = 5; //максимальная длительность решения сложной задачи
            int R = 40; //вероятность прихода сложной заявки
            Queue<int> A = new Queue<int>(); //очередь длительностей процессов            
            Random rnd = new Random();
            int ticks = 0; //общее количество тактов
            int smallTimes = 0; //общее время ожидания маленьких работ
            int smallWorks = 0;
            int processedSmallWorks = 0; //количество обработанных малых работ
            for (int i = 0; i < 10; i++) //инициализация очереди
            {
                if (rnd.Next(100) < R) //с вероятностью R приходит большая заявка
                {
                    A.Enqueue(L);
                }
                else
                {
                    int work = rnd.Next(1, 5); //с вероятностью 60% приходит средняя или минимальная заявка
                    A.Enqueue(work);
                    if (work <= KL) smallWorks++;
                }
            }
            while (A.Count != 0)
            {
                int nextWork = A.Dequeue(); //берем задачу из очереди
                if (nextWork > KL) //если задача выполняется дольше такта
                {
                    nextWork -= KL;
                    A.Enqueue(nextWork); //даем ей поработать и откладываем на потом
                }
                else
                {
                    if (processedSmallWorks < smallWorks) //чтобы не учитывать "обрезанные" большие работы
                    {
                        processedSmallWorks++;
                        smallTimes += ticks; //для маленьких задач подсчитываем время ожидания
                    }
                }
                ticks++;
            }
            if (smallWorks > 0) //если маленьких работ не было, то не вычисляем эту характеристику
            {
                double middleTimeOfMinimumWorks = (double)smallTimes / smallWorks; //среднее время ожидания маленькой заявки в тактах
                Console.WriteLine("Среднее время ожидания маленьких заявак(такт) {0:N}", middleTimeOfMinimumWorks);
            }
        }
    }
}
