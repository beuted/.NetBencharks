using System.ComponentModel.DataAnnotations;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace MyBenchmarks
{
    /// <summary>
    /// The in parameters are similar to the readonly fields. To make sure that the parameter's value
    /// stays the same the compiler make a defensive copy of the parameter every time a method/property is used
    ///
    /// https://blogs.msdn.microsoft.com/seteplia/2018/03/07/the-in-modifier-and-the-readonly-structs-in-c/
    ///
    ///                         Method |     Mean |     Error |    StdDev |
    ///------------------------------- |---------:|----------:|----------:|
    ///         AggregatePassedByValue | 56.98 us | 0.2079 us | 0.1843 us |
    ///            AggregatePassedByIn |*96.10 us*| 0.3311 us | 0.2935 us |
    /// ReadonlyAggregatePassedByValue | 58.13 us | 1.3327 us | 1.6855 us |
    ///    ReadonlyAggregatePassedByIn | 57.56 us | 0.7784 us | 0.7281 us |

    /// </summary>
    public class InKeywordBenchmark2
    {
        private struct FairlyLargeStruct
        {
            private readonly long l1, l2, l3, l4;
            public int N { get; }
            public FairlyLargeStruct(int n) : this() => N = n;
        }

        private readonly struct FairlyLargeReadOnlyStruct
        {
            private readonly long l1, l2, l3, l4;
            public int N { get; }
            public FairlyLargeReadOnlyStruct(int n) : this() => N = n;
        }

        private readonly FairlyLargeStruct _fairlyLargeStruct = new FairlyLargeStruct(42);
        private readonly FairlyLargeReadOnlyStruct _fairlyLargeReadOnlyStruct = new FairlyLargeReadOnlyStruct(42);

        private readonly int[] _data = Enumerable.Range(1, 100_000).ToArray();

        [Benchmark]
        public int AggregatePassedByValue()
        {
            return DoAggregateWithoutIn(_fairlyLargeStruct);
        }

        [Benchmark]
        public int AggregatePassedByIn()
        {
            return DoAggregateWithIn(in _fairlyLargeStruct);
        }

        [Benchmark]
        public int ReadonlyAggregatePassedByValue()
        {
            return DoAggregateWithoutIn(_fairlyLargeReadOnlyStruct);
        }

        [Benchmark]
        public int ReadonlyAggregatePassedByIn()
        {
            return DoAggregateWithIn(in _fairlyLargeReadOnlyStruct);
        }

        private int DoAggregateWithIn(in FairlyLargeStruct largeStruct)
        {
            int result = 0;
            foreach (int n in _data)
                result += n + largeStruct.N;
            return result;
        }

        private int DoAggregateWithoutIn(FairlyLargeStruct largeStruct)
        {
            int result = 0;
            foreach (int n in _data)
                result += n + largeStruct.N;
            return result;
        }

        private int DoAggregateWithIn(in FairlyLargeReadOnlyStruct largeStruct)
        {
            int result = 0;
            foreach (int n in _data)
                result += n + largeStruct.N;
            return result;
        }

        private int DoAggregateWithoutIn(FairlyLargeReadOnlyStruct largeStruct)
        {
            int result = 0;
            foreach (int n in _data)
                result += n + largeStruct.N;
            return result;
        }


    }
}