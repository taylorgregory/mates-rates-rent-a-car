using MRRCManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MRRCManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            //Fleet test = new MRRCManagement.Fleet();
            Fleet test = new Fleet();
            test.LoadFromFile();
            Console.WriteLine("Search Check:");
            test.Query("GPS OR sunROOF");
            //test.SaveToFile();     
            Console.ReadKey();
        }
    }
}
