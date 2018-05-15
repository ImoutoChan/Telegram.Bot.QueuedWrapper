using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Telegram.Bot.QueuedWrapper
{
    public class TelegramMessageQueue
    {
        class QueueElement
        {
            public string Target { get; set; }
        }
        
        private readonly Dictionary<string, QueueBasedMessageRateLimiter> _groupLimiters 
            = new Dictionary<string, QueueBasedMessageRateLimiter>();
        private readonly Dictionary<string, QueueBasedMessageRateLimiter> _chatLimiters 
            = new Dictionary<string, QueueBasedMessageRateLimiter>();
        private readonly QueueBasedMessageRateLimiter _baseLimiter 
            = new QueueBasedMessageRateLimiter("default", 30, TimeSpan.FromSeconds(1));

        public async Task RunThroughQueue(Func<Task> task, string target = "defaultTarget")
        {
            //Debug.WriteLine($"PermissionRequested: {DateTime.Now:O}");
            await GetPermission(target);
            Debug.WriteLine($"PermissionGranted: {DateTime.Now:O}");

            await task();
        }

        public async Task<T> RunThroughQueue<T>(Func<Task<T>> task, string target = "defaultTarget")
        {
            await GetPermission(target);

            return await task();
        }


        private async Task GetPermission(string target)
        {
            var element = new QueueElement
            {
                Target = target
            };

            await Delay(element);
        }
        private async Task Delay(QueueElement element)
        {
            if (IsGroup(element))
            {
                await DelayGroup(element);
            }
            else
            {
                await DelayChat(element);
            }
        }
        private async Task DelayChat(QueueElement element)
        {
            var limiter = GetChatLimiter(element);

            await limiter.Wait();

            await DelayBase(element);
        }
        private async Task DelayGroup(QueueElement element)
        {
            var limiter = GetGroupLimiter(element);

            await limiter.Wait();

            await DelayChat(element);
        }
        private async Task DelayBase(QueueElement element)
        {
            await _baseLimiter.Wait();
        }

        private QueueBasedMessageRateLimiter GetGroupLimiter(QueueElement element)
        {
            QueueBasedMessageRateLimiter limiter;
            lock (_groupLimiters)
            {
                if (!_groupLimiters.ContainsKey(element.Target))
                {
                    Debug.WriteLine($"Creater {element.Target}: {DateTime.Now:O}");
                    _groupLimiters.Add(element.Target, new QueueBasedMessageRateLimiter("group: " + element.Target, 20, TimeSpan.FromMinutes(1)));
                }

                limiter = _groupLimiters[element.Target];
            }

            return limiter;
        }
        private QueueBasedMessageRateLimiter GetChatLimiter(QueueElement element)
        {
            QueueBasedMessageRateLimiter limiter;
            lock (_chatLimiters)
            {
                if (!_chatLimiters.ContainsKey(element.Target))
                {
                    Debug.WriteLine($"Creater {element.Target}: {DateTime.Now:O}");
                    _chatLimiters.Add(element.Target, new QueueBasedMessageRateLimiter("chat: " + element.Target, 1, TimeSpan.FromSeconds(1)));
                }

                limiter = _chatLimiters[element.Target];
            }

            return limiter;
        }

        private bool IsGroup(QueueElement element) 
            => !Int64.TryParse(element.Target, out var id) || id < 0;
    }
}