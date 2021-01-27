using System;
using System.Threading;
using System.Threading.Tasks;

// 1 вариант
// станций - 5
// интенсивность передачи - 2,5 
// количество пакетов для каждой станции - 7 
namespace lab7
{
    class Program
    {
        static void Main(string[] args)
        {
            Chanel chanel = new Chanel(5);
            while (true)
            {
                chanel.tick();
            }
        }
    }

    class Station
    {
        public int id;
        static Random random = new Random();
        int timeForSend = 0;
        int packege = 7; // средняя длина пакета(по условию они все равны)
        int sendPackege = 0;
        bool collision = false;

        public Station(int _id)
        {          
            this.id = _id;
            timeForSend = random.Next(0,100);
        }

        public void Run(int currentTime, Chanel chanel)
        {
            if(currentTime >= timeForSend )
            {
                //если возникла коллизия при нашей отправке тогда надо отложить отправку на небольшое время
                if(collision && sendPackege > 0)
                {
                    timeForSend += random.Next(0, 20);
                    Console.WriteLine("Прервала отправку, станция {0}", id);
                    collision = false;
                    sendPackege = 0;
                    return;
                }
                if (sendPackege == packege)
                {
                    Console.WriteLine("Отправка завершена, станция {0}", id);
                    timeForSend += random.Next(1, 100);
                    sendPackege = 0;
                }
                collision = false;
                Console.WriteLine("Отправка пакета, станция {0}", id);
                chanel.Send(this.id);
                sendPackege++;
            }          
        }

        internal void Collision()
        {
            collision = true;
        }
    }

    class Chanel
    {
        Station[] stations;
        int time = 0;
        int currentStation = -1;

        public Chanel(int countStation)
        {
            stations = new Station[countStation];
            for(int i = 0; i< countStation; i++)
            {
                stations[i] = new Station(i);
            }
        }

        public void Send(int id)
        {
            if (currentStation != -1)
            {
                Console.WriteLine("Коллизия!");
                foreach (Station stantion in stations)
                {
                    stantion.Collision();
                }
            }
            currentStation = id;       
        }

        public void tick()
        {
            time++;
            foreach(Station stantion in stations)
            {
                stantion.Run(time, this);
            }
            currentStation = -1;
            Thread.Sleep(100);
        }
    }
}
