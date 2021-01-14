// lab2.cpp : Этот файл содержит функцию "main". Здесь начинается и заканчивается выполнение программы.
//

#include <stdio.h>
#include <cmath>
#include <ctime>
#include <malloc.h>
#include <xmmintrin.h>
#include <clocale>
#include <windows.h>

// вариант 1 (TODO переделать на функцию)

//Вычисляем функцию через cpp: y = sqrt(x + 0.5)
void ComputeArrayCpp(
	float* pArray, // [in] исходный массив
	float* pResult, // [out] массив результатов
	int nSize) // [in] размер массивов
{
	int i;

	float* pSource = pArray;
	float* pDest = pResult;

	for (i = 0; i < nSize; i++)
	{
		*pDest = (float)sqrt((*pSource) + 0.5f);

		//Перемещаемся к следующему элементу
		pSource++;
		pDest++;
	}
}
//Вычисляем функцию через sse: y = sqrt(x + 0.5)
void ComputeArrayCppSSE(
	float* pArray, // [in] исходный массив
	float* pResult, // [out] массив результатов
	int nSize) // [in] размер массивов
{
	//Определяем кол-во итераций цикла:
	//sizeof(__m128)/sizeof(float) => 4
	int nLoop = nSize / 4;

	__m128 m1;

	//Преобразуем указатели на float к типу __m128
	__m128* pSrc = (__m128*) pArray;
	__m128* pDest = (__m128*) pResult;


	__m128 m0_5 = _mm_set_ps1(0.5f); // m0_5[0, 1, 2, 3] = 0.5

	for (int i = 0; i < nLoop; i++)
	{
		m1 = _mm_add_ps(*pSrc, m0_5); // m1 = *pSrc + 0.5
		*pDest = _mm_sqrt_ps(m1); // *pDest = sqrt(m1)

		//Перемещаемся к следующему элементу __m128
		//Тем самым пропускаем 4 float из массива
		pSrc++;
		pDest++;
	}
}
//Инициализируем массив начальными занчениями sin(rnd())
void init(float* a, int size)
{
	for (int i = 0; i < size; i++)
	{
		float x = (float)rand() / RAND_MAX;
		a[i] = sin(x);
	}
}
int main(int argc, char* argv[])
{
	//Используем русскую локаль
	setlocale(LC_ALL, "Russian");

	const int MAX_SIZE = 10000000;

	//Тем самым пропускаем 4 float из массива
	float* x = (float*)_aligned_malloc(sizeof(float) * MAX_SIZE, 16);
	float* y = (float*)_aligned_malloc(sizeof(float) * MAX_SIZE, 16);
	DWORD startTime, endTime;
	//Получаем время начала работы
	startTime = GetTickCount();
	init(x, MAX_SIZE);
	//Получаем время окончания работы
	endTime = GetTickCount();

	printf("Инициализация массивов: %d мс\n", endTime - startTime);

	startTime = GetTickCount();

	//Выполняем вычисления с помощью обычных функций c++
	ComputeArrayCpp(x, y, MAX_SIZE);

	endTime = GetTickCount();

	printf("Вычисление средствами C++: %d мс\n", endTime - startTime);

	startTime = GetTickCount();

	//Выполняем вычисления с помощью SSE
	ComputeArrayCppSSE(x, y, MAX_SIZE);

	endTime = GetTickCount();

	printf("Вычисление средствами SSE: %d мс\n", endTime - startTime);

	//Освобождаем память массивов
	_aligned_free(x);
	_aligned_free(y);

	return 0;
}

