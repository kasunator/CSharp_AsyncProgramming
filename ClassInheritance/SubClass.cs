using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInheritance
{
    public class SubClass : BaseClass
    {
        int myNumber;
        public SubClass()
        {

        } 

        public SubClass( int i):base(1)
        {
            myNumber = i;
            Console.WriteLine("SubClass() constructor");
            Console.WriteLine(X);
        }



    }



}
