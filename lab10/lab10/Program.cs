// вариант 9  (нечетный, алгоритм флойда)   (81 % 12)

using System;
using System.Collections.Generic;
using System.Linq;

namespace lab10
{
    class Program
    {
        static void Main(string[] args)
        {
            const int N = 6;
            int[][] mf = new int[N][]{
 new int[N] {1000,    1, 1000,    1, 1000, 1000},
 new int[N] {   1, 1000,    3,    2,    3,    3},
 new int[N] {1000,    3, 1000,    1, 1000,    2},
 new int[N] {   1,    2,    1, 1000,    2, 1000},
 new int[N] {1000,    3, 1000,    2, 1000,    3},
 new int[N] {1000,    3,    2, 1000,    3, 1000},
 };
            for (int a = 0; a < N; a++)
            {
                for (int b = 0; b < N; b++)
                    if (mf[a][b] != 1000)
                        for (int c = 0; c < N; c++)
                            if (mf[a][c] > mf[a][b] + mf[b][c])
                            {
                                mf[a][c] = mf[a][b] + mf[b][c]; 
                            } 
            }

            // таблица кратчайших маршрутов
            Console.Write("  ");
            for (int i = 1; i <= N; i++)
            {
                Console.Write("{0} ", i);
            }
            Console.WriteLine();
            for (int k = 0; k < N; k++)
            {
                Console.Write("{0} ", k+1);
                for (int i = 0; i < N; i++)
                {
                   
                    Console.Write("{0} ", (i != k) ? mf[i][k] : 0);
                }
                Console.WriteLine();
            }

        }
    }
}
