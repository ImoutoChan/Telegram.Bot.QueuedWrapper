using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TokenBucket.NetStandart;

namespace MessageQueue
{
    public class TelegramMessageQueue
    {
        private readonly ConcurrentDictionary<string, ITokenBucket> _buckets =
            new ConcurrentDictionary<string, ITokenBucket>();

        public TelegramMessageQueue()
        {
            Configure("default", TimeSpan.FromSeconds(1), 30);
        }

        private ITokenBucket Configure(string tag, TimeSpan interval, int limit)
        {
            return _buckets.AddOrUpdate(tag,
                                        s => TokenBuckets
                                           .Construct()
                                           .WithCapacity(limit)
                                           .WithFixedIntervalRefillStrategy(limit, interval)
                                           .Build(),
                                        (s, bucket) => bucket);
        }

        public async Task RunThroughQueue(Func<Task> task, string groupId = null)
        {
            await Throttle(groupId);

            try
            {
                await task();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<T> RunThroughQueue<T>(Func<Task<T>> task, string groupId = "group_default")
        {

            await Throttle(groupId);

            try
            {
                return await task();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task Throttle(string groupId)
        {
            var buckets = new List<ITokenBucket>();
            _buckets.TryGetValue("default", out var defaultBucket);

            buckets.Add(defaultBucket);
            if (groupId != null)
            {
                var min = groupId + "_min";
                var total = groupId + "_total";

                if (!_buckets.TryGetValue(min, out var groupMinBucket))
                {
                    groupMinBucket = Configure(min, TimeSpan.FromMilliseconds(1000), 1);
                }

                if (!_buckets.TryGetValue(total, out var groupTotalBucket))
                {
                    groupTotalBucket = Configure(total, TimeSpan.FromSeconds(60), 20);
                }

                buckets.Add(groupMinBucket);
                buckets.Add(groupTotalBucket);
                buckets.Reverse();
            }


            foreach (var bucket in buckets)
            {
                while (!bucket.TryConsume())
                {
                    await Task.Delay(10);
                }
            }
        }
    }
}