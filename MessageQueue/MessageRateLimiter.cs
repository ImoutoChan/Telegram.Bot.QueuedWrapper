using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Bert.RateLimiters;

namespace MessageQueue
{
    public class MessageRateLimiter
    {
        private readonly LeakyTokenBucket _bucket;

        public MessageRateLimiter(int limit, TimeSpan forInterval)
        {
            _bucket = new StepDownTokenBucket(limit,  
                1, 
                (int)forInterval.TotalMilliseconds, 
                1, 
                1, 
                (int)(forInterval.TotalMilliseconds / limit));
        }

        public async Task Wait()
        {
            if (_bucket.ShouldThrottle(out var waitTime))
            {
                Debug.WriteLine(waitTime);
                await Task.Delay(waitTime);
            }
        }
    }

    public class QueueBasedMessageRateLimiter
    {
        private readonly string _name;
        private readonly int _limit;
        private readonly TimeSpan _forInterval;
        private readonly LinkedList<DateTime> _requestTimes = new LinkedList<DateTime>();
        private readonly object _locker = new object();

        public QueueBasedMessageRateLimiter(string name, int limit, TimeSpan forInterval)
        {
            _name = name;
            _limit = limit;
            _forInterval = forInterval;
        }

        public async Task Wait()
        {
            var waitTime = GetWaitTime();

            if (waitTime > TimeSpan.Zero)
            {
                Debug.WriteLine($"{_name} | Wait time: {waitTime}");
                await Task.Delay(waitTime);
            }
        }

        private TimeSpan GetWaitTime()
        {
            Debug.WriteLine($"{_name} | GetWaitTime");
            lock (_locker)
            {
                var lastTime = _requestTimes.Count == 0 || _requestTimes.Last.Value.AddMilliseconds(1) < DateTime.Now
                                  ? DateTime.Now
                                  : _requestTimes.Last.Value.AddMilliseconds(1);
                
                Debug.WriteLine($"{_name} | Current: {lastTime} | Count: {_requestTimes.Count}");
                var leftTime = lastTime.Add(-_forInterval);
                Debug.WriteLine($"{_name} | Left: {leftTime}");

                var count = CountAndClean(leftTime);
                Debug.WriteLine($"{_name} | calculatedCount: {count}");

                var time = count >= _limit 
                    ? _requestTimes.First.Value - DateTime.Now.Add(-_forInterval)
                    : TimeSpan.Zero;
                Debug.WriteLine($"{_name} | firstValue: {_requestTimes.First?.Value}");

                var current = DateTime.Now.Add(time);
                _requestTimes.AddLast(current);
                Debug.WriteLine($"{_name} | added: {current}");

                
                Debug.WriteLine($"{_name} | result: {time}");
                return time;
            }
        }

        private int CountAndClean(DateTime leftTime)
        {
            if (_requestTimes.Count == 0)
            {
                return 0;
            }

            var node = _requestTimes.Last;
            var counter = 0;

            // Count all greater then provided time
            while (node.Value > leftTime)
            {
                counter++;

                node = node.Previous;
                if (node == null) 
                    break;
            }

            // Remove all others
            while (node != null)
            {
                var next = node.Previous;
                _requestTimes.Remove(node);
                node = next;
            }

            return counter;
        }
    }
}