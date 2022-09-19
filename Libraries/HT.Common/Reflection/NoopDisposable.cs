using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HT.Common.Documentation;

namespace HT.Common.Reflection
{
    [Citation("https://github.com/StephenCleary/Disposables/blob/master/src/Nito.Disposables/NoopDisposable.cs")]
    public sealed class NoopDisposable : IDisposable
#if NETSTANDARD2_1
        , IAsyncDisposable
#endif
    {
        private NoopDisposable()
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Dispose()
        {
        }

#if NETSTANDARD2_1
        /// <summary>
        /// Does nothing.
        /// </summary>
        public ValueTask DisposeAsync() => new ValueTask();
#endif

        /// <summary>
        /// Gets the instance of <see cref="NoopDisposable"/>.
        /// </summary>
        public static NoopDisposable Instance { get; } = new NoopDisposable();
    }
}
