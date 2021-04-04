using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Extensions
{
    public static class ActionDebounceExtensions
    {
        public static Action<T> Debounce<T> (this Action<T> action, int delayInMilliseconds)
        {
            CancellationTokenSource cancellationToken = null;

            // 'arg' is the parameter passed to the Action
            return arg =>
            {
                // Cancel() should dispose of the previous CancellationTokenSource
                cancellationToken?.Cancel();
                cancellationToken = new CancellationTokenSource();

                Task.Delay(delayInMilliseconds, cancellationToken.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                        {
                            action(arg);
                        }
                    });
            };
        }
    }
}
