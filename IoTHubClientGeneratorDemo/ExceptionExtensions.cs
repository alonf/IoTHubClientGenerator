using System;
using System.Collections.Generic;

namespace IoTHubClientGeneratorDemo
{
    internal static class ExceptionExtensions
    {
        internal static IEnumerable<Exception> Unwind(this Exception exception, bool unwindAggregate = false)
        {
            while (exception != null)
            {
                yield return exception;

                if (!unwindAggregate)
                {
                    exception = exception.InnerException;
                    continue;
                }

                if (exception is AggregateException {InnerExceptions: { }} aggEx)
                {
                    foreach (Exception ex in aggEx.InnerExceptions)
                    {
                        foreach (Exception innerEx in ex.Unwind(true))
                        {
                            yield return innerEx;
                        }
                    }
                }

                exception = exception.InnerException;
            }
        }
    }
}