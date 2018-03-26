using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Lamp
{
    class Program
    {
        static void Main(string[] args)
        {
            LampManager Lm = new LampManager();

            Lm.AddLamp(LampAdapterFactory.LampFactory(LampVendors.LampVendorA));
            Lm.AddLamp(LampAdapterFactory.LampFactory(LampVendors.LampVendorB));
            Lm.AddLamp(LampAdapterFactory.LampFactory(LampVendors.LampVendorA));

            Console.WriteLine();
            Console.WriteLine();

            Lm.RunLamp();
        }
    }

    public class VendorALight
    {
        public void TurnOn()
        {
        }
        public void TurnOff()
        {
        }
    }

    public class VendorBLight
    {
        public void TurnLight(bool on)
        {
        }
    }

    public interface Lamp
    {
        void TurnLamp(bool enable);
    }

    public class AdapterLampVendorA : Lamp
    {
        VendorALight Lamp { get; set; }

        public AdapterLampVendorA()
        {
            Lamp = new VendorALight();
        }

        public void TurnLamp(bool enable)
        {
            if (enable)
                Lamp.TurnOn();
            else
                Lamp.TurnOff();
        }
    }

    public class AdapterLampVendorB : Lamp
    {
        VendorBLight Lamp { get; set; }

        public AdapterLampVendorB()
        {
            Lamp = new VendorBLight();
        }

        public void TurnLamp(bool enable)
        {
            Lamp.TurnLight(enable);
        }
    }

    public enum LampVendors
    {
        LampVendorA,
        LampVendorB
    }

    public class LampAdapterFactory
    {
        public static Lamp LampFactory(LampVendors lv)
        {
            if (lv == LampVendors.LampVendorA)
                return new AdapterLampVendorA();
            else if (lv == LampVendors.LampVendorB)
                return new AdapterLampVendorB();

            return null;
        }
    }

    public class LampsData
    {
        static LampsData _instance;
        static object _locker = new object();
        List<Lamp> _lampsData;

        private LampsData()
        {
            _lampsData = new List<Lamp>();
        }

        public static LampsData GetInstance()
        {
            lock (_locker)
            {
                if (_instance == null)
                {
                    _instance = new LampsData();
                }
            }

            return _instance;
        }

        public List<Lamp> GetLampsData()
        {
            return _lampsData;
        }
    }

    public class LampManager
    {
        LampsData Lamps = LampsData.GetInstance();
        object _locker = new object();

        public void AddLamp(Lamp l)
        {
            List<Lamp> _lampsData;

            lock (_locker)
            {
                _lampsData = Lamps.GetLampsData();
                _lampsData.Add(l);
            }

            Console.WriteLine("Lamp added!");
        }

        public void RunLamp()
        {
            var currentIndex = 0;
            List<Lamp> _lampsData;

            lock (_locker)
            {
                _lampsData = Lamps.GetLampsData();
            }

            while (true)
            {
                if (currentIndex >= _lampsData.Count)
                {
                    currentIndex = 0;
                }

                for (int i = 0; i < _lampsData.Count; i++)
                {
                    _lampsData[i].TurnLamp(false);
                }

                Console.Write("Turning lamp # " + currentIndex + " ");

                _lampsData[currentIndex].TurnLamp(true);

                currentIndex++;

                Console.WriteLine();
                Thread.Sleep(1000);
            }
        }
    }
}
