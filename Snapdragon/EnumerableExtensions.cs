namespace Snapdragon
{
    public static class EnumerableExtensions
    {
        // Originally by Stephen Toub, https://devblogs.microsoft.com/pfxteam/implementing-a-simple-foreachasync/
        public static Task ForEachAsync<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Task<TResult>> taskSelector,
            Action<TSource, TResult> resultProcessor,
            int maxParallelism = 200
        )
        {
            var semaphore = new SemaphoreSlim(maxParallelism, maxParallelism);
            return Task.WhenAll(
                from item in source
                select Task.Run(() => ProcessAsync(item, taskSelector, resultProcessor, semaphore))
            );
        }

        private static async Task ProcessAsync<TSource, TResult>(
            TSource item,
            Func<TSource, Task<TResult>> taskSelector,
            Action<TSource, TResult> resultProcessor,
            SemaphoreSlim semaphore
        )
        {
            await semaphore.WaitAsync();
            try
            {
                TResult result = await taskSelector(item);
                resultProcessor(item, result);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static Task ForEachAsync<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, Task> task,
            int maxParallelism = 200
        )
        {
            var semaphore = new SemaphoreSlim(maxParallelism, maxParallelism);
            return Task.WhenAll(
                from item in source
                select Task.Run(() => ProcessAsync(item, task, semaphore))
            );
        }

        private static async Task ProcessAsync<TSource>(
            TSource item,
            Func<TSource, Task> task,
            SemaphoreSlim semaphore
        )
        {
            await semaphore.WaitAsync();
            try
            {
                await task(item);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
