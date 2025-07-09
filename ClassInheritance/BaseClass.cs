using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInheritance
{
    public class BaseClass
    {
        public int X;
        public BaseClass() 
        {
            Console.WriteLine("BaseClass() constructor");
            X = 1; 
        }

        public BaseClass(int i)
        {
            Console.WriteLine("BaseClass(int i) constructor");
            X = i;
        }

    }
}
