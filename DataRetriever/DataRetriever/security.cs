using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRetriever
{
    class security
    {
        public string Name;
        public string Code;
        public string Index;
        public string Sector;
        public Dictionary<string, double> historicalClose = new Dictionary<string, double>();

        public void Print()
        {
            Console.WriteLine("Name;Code;Index;Sector;Date;Price");
            for(int i = 0; i < historicalClose.Count; i++)
            {
                Console.WriteLine(Name + ";" + Code + ";" + Index + ";" + Sector + ";" + historicalClose.Keys.ToList()[i].ToString() + ";" + historicalClose.Values.ToList()[i].ToString());
            }
        }
    }
}
