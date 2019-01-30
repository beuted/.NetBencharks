using System.ComponentModel.DataAnnotations;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace MyBenchmarks
{
    /// <summary>
    /// The significant difference in the results caused by a defensive copy that is happening each time the readonly
    /// field is used. You may have heard that the size of the struct should be relatively small to avoid the overhead
    /// of passing it around to other methods. But as you can see, you may get a performance hit even when a fairly
    /// large struct is stored in a readonly field and never passed to another method.
    ///
    ///                        Method |      Mean |     Error |    StdDev |
    /// ----------------------------- |----------:|----------:|----------:|
    ///  AggregateForNonReadOnlyField |  59.28 us | 0.5330 us | 0.4725 us |
    ///     AggregateForReadOnlyField | 102.39 us | 2.0290 us | 3.0985 us |
    ///
    ///
    /// https://blogs.msdn.microsoft.com/seteplia/2018/03/07/the-in-modifier-and-the-readonly-structs-in-c/
    /// </summary>
    public class StructWithReadOnlyPpties
    {
        private struct FairlyLargeStruct
        {
            private readonly long l1, l2, l3, l4;
            public int N { get; }
            public FairlyLargeStruct(int n) : this() => N = n;
        }

        private FairlyLargeStruct _nonReadOnlyStruct  = new FairlyLargeStruct(42);
        private readonly FairlyLargeStruct _readOnlyStruct  = new FairlyLargeStruct(42);

        private readonly int[] _data = Enumerable.Range(1, 100_000).ToArray();

        [Benchmark]
        public int AggregateForNonReadOnlyField()
        {
            int result = 0;
            foreach (int n in _data)
                result += n + _nonReadOnlyStruct.N;
            return result;
        }

        [Benchmark]
        public int AggregateForReadOnlyField()
        {
            int result = 0;
            foreach (int n in _data)
                result += n + _readOnlyStruct.N;
            return result;
        }
    }
}