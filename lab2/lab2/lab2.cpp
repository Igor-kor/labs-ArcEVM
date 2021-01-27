// все функции https://doc.rust-lang.org/core/arch/x86_64/fn._mm_sub_ps.html

#include <stdio.h>
#include <cmath>
#include <ctime>
#include <malloc.h>
#include <xmmintrin.h>
#include <clocale>
#include <windows.h>

// вариант 1 (TODO переделать на функцию)

//Вычисляем функцию через cpp: x^2+y-sqrt(z^2+1)
void ComputeArrayCpp(
	float* pArrayx, // [x] исходный массив
	float* pArrayy, // [y] исходный массив
	float* pArrayz, // [z] исходный массив
	float* pResult, // [out] массив результатов
	int nSize) // [in] размер массивов
{
	float* pSourcex = pArrayx;
	float* pSourcey = pArrayy;
	float* pSourcez = pArrayz;
	float* pDest = pResult;

	for (int i = 0; i + 2 < nSize; i++)
	{
		*pDest = (float)*(pSourcex + i) * *(pSourcex + i) + *(pSourcey + i) - sqrt((*(pSourcez + i)) * (*(pSourcez + i)) + 1.0f);
		//Перемещаемся к следующему элементу
		pDest++;
	}
}
//Вычисляем функцию через sse: /*y = sqrt(x + 0.5)*/  x^2+y-sqrt(z^2+1)
void ComputeArrayCppSSE(
	float* pArrayx, // [x] исходный массив
	float* pArrayy, // [y] исходный массив
	float* pArrayz, // [z] исходный массив
	float* pResult, // [out] массив результатов
	int nSize) // [in] размер массивов
{
	//Определяем кол-во итераций цикла:
	//sizeof(__m128)/sizeof(float) => 4
	int nLoop = nSize / 4;

	__m128 m2;

	//Преобразуем указатели на float к типу __m128
	__m128* pSrcx = (__m128*) pArrayx;
	__m128* pSrcy = (__m128*) pArrayy;
	__m128* pSrcz = (__m128*) pArrayz;
	__m128* pDest = (__m128*) pResult;


	__m128 m1 = _mm_set_ps1(1); // m0_5[0, 1, 2, 3] = 1

	for (int i = 0; i < nLoop; i++)
	{

		m2 = _mm_add_ps(_mm_mul_ps( *pSrcz, *pSrcz), m1); // m1 = *pSrcz * *pSrcz + 1
		*pDest = _mm_sub_ps(_mm_add_ps( _mm_mul_ps(*pSrcx, *pSrcx), *pSrcy ), _mm_sqrt_ps(m2)); // *pDest = sqrt(m2)

		//Перемещаемся к следующему элементу __m128
		//Тем самым пропускаем 4 float из массива
		pSrcx++;
		pSrcy++;
		pSrcz++;
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

	const int MAX_SIZE = 100000000;

	//Тем самым пропускаем 4 float из массива
	float* x = (float*)_aligned_malloc(sizeof(float) * MAX_SIZE, 16);
	float* y = (float*)_aligned_malloc(sizeof(float) * MAX_SIZE, 16);
	float* z = (float*)_aligned_malloc(sizeof(float) * MAX_SIZE, 16);
	float* out = (float*)_aligned_malloc(sizeof(float) * MAX_SIZE, 16);
	DWORD startTime, endTime;
	//Получаем время начала работы
	startTime = GetTickCount();
	init(x, MAX_SIZE);
	init(y, MAX_SIZE);
	init(z, MAX_SIZE);
	//Получаем время окончания работы
	endTime = GetTickCount();

	printf("Инициализация массивов: %d мс\n", endTime - startTime);

	startTime = GetTickCount();

	//Выполняем вычисления с помощью обычных функций c++
	ComputeArrayCpp(x, y, z, out, MAX_SIZE);

	endTime = GetTickCount();

	printf("Вычисление средствами C++: %d мс\n", endTime - startTime);

	startTime = GetTickCount();

	//Выполняем вычисления с помощью SSE
	ComputeArrayCppSSE(x, y, z, out, MAX_SIZE);

	endTime = GetTickCount();

	printf("Вычисление средствами SSE: %d мс\n", endTime - startTime);

	//Освобождаем память массивов
	_aligned_free(x);
	_aligned_free(y);
	_aligned_free(z);
	_aligned_free(out);

	return 0;
}

