using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker.Contract
{
    public class CircuitBreakerManager
    {
        private readonly int resetTimeOut;

        public CircuitBreakerManager(int resetTimeoutInMilliseconds)
        {
            this.resetTimeOut = resetTimeoutInMilliseconds;
        }

        public async Task Invoke(Func<Task> func, Func<Task> failAction)
        {
            var circuitBreaker = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromMilliseconds(resetTimeOut)
                );

            var policy = Policy
                .Handle<Exception>()
                .FallbackAsync(() => failAction())
                .Wrap(circuitBreaker);

            var val = policy.ExecuteAsync(() => func());
        }
    }
}
