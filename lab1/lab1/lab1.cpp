#include <iostream>
#include <windows.h>

int main()
{
    DWORD startTime, endTime;
    startTime = GetTickCount();
    //выполенние операций
  
    endTime = GetTickCount();
    printf("Work time: %d\n", startTime);
    return 0;
}
