namespace Playground
{
    internal class Program
    {
        public static async IAsyncEnumerator<int> GetRoutine()
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(100);
                yield return i;
            }
        }

        static void Main(string[] args)
        {
            var t = Task.Run(async () =>
            {
                var routine = GetRoutine();
                while (await routine.MoveNextAsync())
                {
                    Console.WriteLine(routine.Current);
                }
            });

            t.Wait();
            
        }
    }
}