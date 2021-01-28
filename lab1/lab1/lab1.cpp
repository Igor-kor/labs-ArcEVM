#include <iostream>
#include <windows.h>
#include <math.h>

//6 вариант
int main()
{
	DWORD startTime, endTime;
	startTime = GetTickCount();
	//выполенние операций 
	for (long long y = 0; y < 10000000000; y++) {
		for (long long k = 0; k < 10000000000000000000; k++) {
			int sum = 0;
			for (int x = 1; x <= 10; x++) {
				for (int i = 1; i < 100000000; i++) {
					sum += pow(cos(i * x * x * x), 4);
				}
			}
		}
	}

	endTime = GetTickCount();
	printf("Work time: %d\n", endTime - startTime); 
	return 0;
}
