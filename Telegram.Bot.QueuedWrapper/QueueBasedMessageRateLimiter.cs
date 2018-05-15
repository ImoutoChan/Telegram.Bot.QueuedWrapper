using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Telegram.Bot.QueuedWrapper
{
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
                var dateTimeNow = DateTime.Now;

                var lastCallTime = _requestTimes.Count == 0 
                                   || _requestTimes.Last.Value.AddMilliseconds(1) < dateTimeNow
                    ? dateTimeNow
                    : _requestTimes.Last.Value.AddMilliseconds(1);
                Debug.WriteLine($"{_name} | Current: {lastCallTime} | Count: {_requestTimes.Count}");

                var leftTime = lastCallTime.Add(-_forInterval);
                Debug.WriteLine($"{_name} | Left: {leftTime}");

                var count = CountAndClean(leftTime);
                Debug.WriteLine($"{_name} | calculatedCount: {count}");

                var time = count >= _limit 
                    ? _requestTimes.First.Value - dateTimeNow.Add(-_forInterval)
                    : TimeSpan.Zero;
                Debug.WriteLine($"{_name} | firstValue: {_requestTimes.First?.Value}");

                var addTo = dateTimeNow ;//> leftTime ? dateTimeNow : leftTime;
                Debug.WriteLine($"{_name} | addTo: {addTo}");

                var expectedToRun = addTo.Add(time);
                _requestTimes.AddLast(expectedToRun);
                Debug.WriteLine($"{_name} | added: {expectedToRun}");

                //WriteLinked(_requestTimes);
                
                Debug.WriteLine($"{_name} | result: {time}");
                return time;
            }
        }

        private void WriteLinked(LinkedList<DateTime> requestTimes)
        {
            var sb = new StringBuilder();
            sb.Append($"{_name} | DebugList: ");
            var node = requestTimes.First;

            while (node != null)
            {
                sb.Append($"{node.Value:mm:sss}\t");

                node = node.Next;
            }

            Debug.WriteLine(sb);
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
            while (node.Value >= leftTime)
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