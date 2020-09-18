using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// Think of a semaphore as a gate letting people into the city.
namespace Semaphore
{
    class Program
    {
        static readonly HttpClient _client = new HttpClient 
        { 
            Timeout = TimeSpan.FromSeconds(5)
        };
        static readonly SemaphoreSlim _gate = new SemaphoreSlim(20);

        static void Main(string[] args)
        {
            // Real world example.            
            Task.WaitAll(CreateCalls().ToArray());
        }

        /// <summary>
        /// Makes an asynchronous call to Google and prints the Status code
        /// to the Console.
        /// </summary>     
        public static async Task CallGoogle()
        {
            try
            {
                await _gate.WaitAsync();
                var response = await _client.GetAsync("https://google.com");
                _gate.Release();

                Print(response.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                Print(ex.Message);
            }

        }

        /// <summary>
        /// Calls a list of tasks to ping Google.
        /// </summary>
        /// <returns>A list of tasks.</returns>
        public static IEnumerable<Task> CreateCalls()
        {
            for (int i = 0; i < 200; i++)
            {
                yield return CallGoogle();
            }
        }


        #region Utility method and topic-introducing notes & methods.

        /// <summary>
        /// Print method for short-hand Console.WriteLine operations.
        /// </summary>
        /// <param name="msg">The string to write to the Console.</param>
        private static void Print(string msg) =>
            Console.WriteLine(msg);

        /// <summary>
        /// First example Jimmy provides with corresponding notes.
        /// </summary>
        private static async Task FirstExample()
        {
            // When we declare a semaphore, we need to pass a number into the constructor.
            //  The number indicates the "amount" that is allowed through the gate.
            // If we throw a 0 in the constructor, no work will be allowed done past that point.
            // If we pass a 1, work will be allowed up until the next gate.
            // If we pass a 2, work will be allowed past both gates.
            SemaphoreSlim gate = new SemaphoreSlim(2);

            Print("Start");

            await gate.WaitAsync();

            Print("Do some work");

            await gate.WaitAsync();

            Print("Finish" );
        }

        /// <summary>
        /// Second example Jimmy provides showing how to use 1 SemaphoreSlim to gate
        /// work but releasing it down the road.
        /// </summary>
        private static async Task SecondExample()
        {
            // Allows one worker through the gate.
            SemaphoreSlim gate = new SemaphoreSlim(1);

            for (int i = 0; i < 10; i++)
            {
                Print("Start");
                await gate.WaitAsync();

                Print("Do work");
                // We release the worker.
                gate.Release();

                Print("Finish");
            }
        }

        #endregion
    }
}
