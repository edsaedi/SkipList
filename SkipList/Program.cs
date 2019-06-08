using System;

namespace SkipList
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SkipList<int> skiplist;
            skiplist = new SkipList<int>();

            skiplist.Add(5);
        }
    }
}
