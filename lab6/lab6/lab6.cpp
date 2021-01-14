// 81%3 = 0
// 0) Обобщенная квадратурная формула прямоугольников.
// 1 вариант

#include <iostream>
#include <omp.h>

double InFunction(double x) { //Подынтегральная функция
	return exp(0.38 - x * x) / (2 + sin(1 / (1.5 + x * x)));
}

//  формула прямоугольноков
double CalcIntegral(double a, double b, int n) {
	double result = 0, h = (b - a) / n;

	for (int i = 0; i < n; i++) {
		result += InFunction(a + h * (i + 0.5));
	}

	result *= h;
	return result;
}



int main()
{

	//включаем поддержку русского языка в консоли
	setlocale(LC_ALL, "Russian");

	const int N = 100000000; 
	//отключение выбора количества потоков (нитей) по умолчанию
	omp_set_dynamic(false);
	//заданние числа используемых потоков
	omp_set_num_threads(2);
	//определение типа переменных блока распараллеливания
	int nTheads, theadNum;
#pragma omp parallel private(nTheads, theadNum)
	{
		nTheads = omp_get_num_threads();
		theadNum = omp_get_thread_num();
		//вывод на экран номера и количества используемых потоков
		printf("OpenMP поток №%d из %d потоков\n", theadNum, nTheads);
	}
	//сохраняем время начала теста
	double startTime = omp_get_wtime();

	double a = 0.4, b = 0.6;
	double result = 0, h = (b - a) / N;
	int i;
#pragma omp parallel for default(shared)\
private(i)\
reduction (+:result)
	for ( i = 0; i < N; i++) {
		result += InFunction(a + h * (i + 0.5));
	}
	result *= h;

	//сохраняем время окончания теста
	double endTime = omp_get_wtime();
	//выводим сообщения об окончании работы потоками
#pragma omp parallel private(theadNum)
	{
		theadNum = omp_get_thread_num();
		printf("Поток №%d окончил вычисления... \n", theadNum);
	}

	//выводим время работы теста на экран
	printf("Время вычисления %f\n", endTime - startTime);


	startTime = omp_get_wtime();
	double result2 = CalcIntegral(a, b, N);
	endTime = omp_get_wtime();
	//выводим время работы теста на экран
	printf("Время вычисления без распараллеливания %f\n", endTime - startTime);
}
