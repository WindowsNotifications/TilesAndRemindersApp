using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TilesAndRemindersLibrary
{
    public abstract class SingleWorker
    {
        protected SingleWorker() { }

        private Task _currentTask;
        private object _lock = new object();
        private CancellationTokenSource _currentCancellationTokenSource;

        protected abstract Task Execute(CancellationToken cancellationToken);

        /// <summary>
        /// Cancels the previous work if it's currently executing, and then once cancelled, starts the work.
        /// </summary>
        /// <returns></returns>
        public Task Start()
        {
            Task newTask;

            lock (_lock)
            {
                if (_currentCancellationTokenSource != null)
                    _currentCancellationTokenSource.Cancel();

                _currentCancellationTokenSource = new CancellationTokenSource();

                _currentTask = CreateNextTask(_currentTask, _currentCancellationTokenSource.Token);
                newTask = _currentTask;
            }

            return newTask;
        }

        private async Task CreateNextTask(Task prevTask, CancellationToken newCancellationToken)
        {
            // Wait till the previous task is cancelled (which throws an OperationCanceledException) or completed
            // We catch all exceptions though, since even if it threw a different exception, the new work needs to start regardless
            if (prevTask != null)
            {
                try
                {
                    await prevTask;
                }

                catch { }
            }

            await Execute(newCancellationToken);
        }
    }
}
