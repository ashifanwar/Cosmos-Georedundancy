using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SumOfNConsecutiveAraay
{
    class Program
    {
        static void Main(string[] args)
        {
            var array = new int[] { 2, 5, 3, 4, 6 };
            var result = GetLargestSum(array, 2);
            Console.WriteLine(result);
            Console.ReadLine();
        }

        
        
        static int GetLargestSum(int[] array, int n)
        {
            var list = new List<int>();
            //for(int i=0;i<=array.Length - n; i++)
            //{
            //    var sum = array[i] + array[i + 1];
            //    list.Add(sum);
            //}
            var i = 1;
            foreach (var x in array)
            { 
                var sum = x + array[i];
                i++;
                list.Add(sum);
                if(i==array.Length)
                {
                    break;
                }
            }

            
            //Console.WriteLine(list.Max());
            //Console.ReadLine();
            return list.Max();
        }
    }
}
