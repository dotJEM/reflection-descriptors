using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadLocal<string> test = new ThreadLocal<string>();
            test.Value = "Hello World";

            Console.WriteLine(test.Value);

            ThreadLocal<string> other = new ThreadLocal<string>();
            Console.WriteLine(other.Value);


        }
    }
}
