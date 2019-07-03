using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mastersLibrary
{
    public static class mfl
    {
        /// <summary>
        /// A Range function much like that of python
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static IEnumerable<int> Range(int start = 0, int end = 0, int increment = 1)
        {
            /*
                Test code: 
                var myArr = new int[]{ 1, 2, 3, 4, 5 };
                foreach (var i in myArr)
                    Console.WriteLine("Foreach: {0}", i);

                foreach (var i in mfl.Range(0,myArr.Length,1))
                     Console.WriteLine("Range: at {0} value is {1}", i,myArr[i]);
             */

            if (increment == 0)
                throw new Exception("Range cannot have an imcrement size of 0");
            
            int count = start;
            
            while((increment < 0 && count > end) || ( increment > 0 && count < end))
            {
                yield return count;
                count += increment;
            }
        }


        public static IEnumerable<int> primesSeive(int limit)
        {
            /* test code for primeSieve
            * foreach (var n in mfl.primesSeive(10000))
            {
                Console.WriteLine(n);
            }*/
            List<bool> a = new List<bool>(); //a list of flags denoting which numbers are prime
            for(int i = 0; i < limit; i++) //assign true to all numbers up to limit (this will change later...drastically)
                a.Add(true);

            a[0] = a[1] = false; //0 and 1 are not primes
            int count = 0; 
            bool isPrime = false;
            while (count < limit) //go through each numbers isPrime flag
            {
                isPrime = a[count]; //assign the current numbers primality to isPrime
                if(isPrime) 
                {
                    yield return count; //return the value of the prime
                    for(int n = count*count; n<limit; n+=count)// go through every remaining element in the list of prime flags (a) thats divisible by the current prime and set them to false 
                    {
                        a[n] = false;
                    }   
                }
                count++; //move to the next element
            }
        }

        /// <summary>
        /// Takes a files address and returns a list of strings containing each line of code
        /// </summary>
        /// <param name="fileAddress"></param>
        /// <returns></returns>
        public static List<string> loadDataFromFile(String fileAddress)
        {
            var tempList = new List<string>();
            StreamReader streamReader;

            try
            {
                streamReader = new StreamReader(fileAddress);
            }
            catch (Exception)
            {
                Console.WriteLine("File not found");
                return null;
            }

            string text;

            while (true)
            {
                text = streamReader.ReadLine();
                if (text == null)
                    break;
                else
                    tempList.Add(text);
            }
            if (tempList.Count == 0)
                tempList.Add("");

            streamReader.Close();
            return tempList;
        }



    }
}
