using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.BotFrameworkFunctionalTests.SimpleHostBot.Utils
{
    public class Semaphore
    {
        private int _max;
        private int _counter = 0;
        private List<dynamic> _waiting = new List<dynamic>();

        public Semaphore(int max)
        {
            _max = max;
        }

        private void Take()
        {
            if (_waiting.Count > 0 && _counter < _max)
            {
                _counter++;
                var task = _waiting.FirstOrDefault();
                _waiting.RemoveAt(0);
                task.Resolve.Start();
            }
        }

        public Task Acquire()
        {
            if (_counter < _max)
            {
                _counter++;
                return Task.CompletedTask;
            }else
            {
                var task = new Task(() => { });
                _waiting.Add(new { Resolve = task });
                return promise.Task;
            }
        }
    }
}
