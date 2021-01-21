using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lab8
{
    class Station
    {
        Random rnd = new Random();
        public Station(int id, int packetCount)
        {
            this.id = id;
            this.packetCount = packetCount;
            collisions++;
        }

        // увеличивает время ожидания при коллизиях
        public int delay()
        {
            nextTime = rnd.Next(1, 10) ;
            // удвоение времени ожидания после коллизии
            if (collisions > 0) nextTime = (int)Math.Pow(nextTime, collisions+1);
            // если уже превышаем максимальное время ожидания сообщаем о нарушении связи
            if(nextTime > maxtime)
            {
                Console.WriteLine("Stantion {0}, communication failure!",id);
            }
            return nextTime;
        }
        public int packetCount = 0;
        public int nextTime = 0;
        public int id;
        public int collisions = 0;
        public int maxtime = 100;
    }
    class Program
    {
        static void Main(string[] args)
        {
            // количество станций
            int n = 5;
            int t = 2;
            Station[] stations = new Station[n];
            //количество пакетов 7
            for (int i = 0; i < n; i++) stations[i] = new Station(i, 7);
            Random rnd = new Random();
            int globalTicks = 0;
            while (!AllPacketsSended(stations))
            {
                Thread.Sleep(50);
                Station stationToStartSending;
                int numberStationsReadyToSend = GetNumberStationsReadyToSend(stations,
                out stationToStartSending);
                // если станций 1 то коллизий нет
                if (numberStationsReadyToSend == 1)
                {
                    DecrementAllTimes(stations, t, ref globalTicks);
                    stationToStartSending.nextTime = 0;
                    stationToStartSending.packetCount--;
                    stationToStartSending.collisions = 0;
                    Console.WriteLine("Stantion {0} sended packet! {1} осталось",
                     stationToStartSending.id,
                    stationToStartSending.packetCount);
                }
                // если несколько станций отправляют то коллизии
                else if (numberStationsReadyToSend > 1)
                {
                    Console.WriteLine("Collision was detected!");
                    for (int i = 0; i < stations.Count(); i++)
                    {
                        if (stations[i].nextTime <= 0)
                        {
                            Console.WriteLine("Station {0} was delayed for {1} ticks",
                             stations[i].id, stations[i].delay());
                        }
                    }
                }
                // если никто не отправляет
                else if (numberStationsReadyToSend == 0)
                {
                    Station stationWithNearestTime =
                   FindStationWithNearestTime(stations);
                    Console.WriteLine("Go to time with {0}", stationWithNearestTime.nextTime);
                    DecrementAllTimes(stations,stationWithNearestTime.nextTime,
                   ref globalTicks);

                }
            }
            Console.WriteLine("Time: {0}", globalTicks);
            Console.ReadKey();
        }
        static Station FindStationWithNearestTime(Station[] stations)
        {
            Station minStation = stations[0];
            foreach (Station s in stations)
            {
                if (s.nextTime < minStation.nextTime && s.packetCount > 0)
                    minStation = s;
            }
            return minStation;
        }
        static void DecrementAllTimes(Station[] stations,
        int time,
        ref int globalTicks)
        {
            globalTicks += time;
            for (int i = 0; i < stations.Count(); i++)
            {
                stations[i].nextTime -= time;
            }
        }
        static int GetNumberStationsReadyToSend(Station[] stations, out Station st)
        {
            st = null;
            int count = 0;
            foreach (Station s in stations)
            {
                if (s.nextTime <= 0 && s.packetCount > 0)
                {
                    count++;
                    st = s;
                }
            }
            if (count != 1) st = null;
            return count;
        }
        static bool AllPacketsSended(Station[] stations)
        {
            foreach (Station s in stations)
            {
                if (s.packetCount != 0)
                {
                    Console.WriteLine("Not all packets was sended");
                    return false;
                }
            }
            Console.WriteLine("All packets was sended");
            return true;
        }
    }
}