using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            Random random1 = new Random();
            Random random2 = new Random();

            for (int i = 0; i < 10; i++) 
            {
                int rand_num1 = random1.Next(10);
                Console.WriteLine("rand_num1: {0}", rand_num1);

                int rand_num2 = random2.Next(10);
                Console.WriteLine("rand_num2: {0}", rand_num2);
            } */
            twoRandowms_sameThreead();

            Console.ReadKey();
        }

        private static void twoRandowms_sameThreead()
        {
            Random random1 = new Random();
            Random random2 = new Random();

            for (int i = 0; i < 10; i++)
            {
                int rand_num1 = random1.Next(10);
                Console.WriteLine("rand_num1: {0}", rand_num1);

                int rand_num2 = random2.Next(10);
                Console.WriteLine("rand_num2: {0}", rand_num2);
            }

        }

    }
}
