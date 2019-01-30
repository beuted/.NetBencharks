using System.ComponentModel.DataAnnotations;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace MyBenchmarks
{
    /// <summary>
    /// The struct being readonly does not change anything
    /// Extra time is due to pop variable of the stack and push value on the stack (????? TO CHECK)
    ///
    ///
    ///                                      Method |      Mean |     Error |    StdDev |
    /// ------------------------------------------- |----------:|----------:|----------:|
    ///                AggregateForNonReadOnlyField |  59.34 us | 0.4999 us | 0.4676 us |
    ///                   AggregateForReadOnlyField | 100.14 us | 1.3883 us | 1.0839 us |
    ///  AggregateForReadOnlyStructNonReadOnlyField |  59.05 us | 0.4898 us | 0.4342 us |
    ///     AggregateForReadOnlyStructReadOnlyField | 108.18 us | 2.0938 us | 4.0341 us |
    ///
    /// https://stackoverflow.com/questions/26700918/why-does-the-compiler-emit-a-stloc-followed-by-a-ldloca
    /// https://blogs.msdn.microsoft.com/seteplia/2018/03/07/the-in-modifier-and-the-readonly-structs-in-c/
    /// </summary>
    public class StructWithReadOnlyPpties2
    {
        private struct FairlyLargeStruct
        {
            private readonly long l1, l2, l3, l4;
            public int N { get; }
            public FairlyLargeStruct(int n) : this() => N = n;
        }

        private struct FairlyLargeReadOnlyStruct
        {
            private readonly long l1, l2, l3, l4;
            public int N { get; }
            public FairlyLargeReadOnlyStruct(int n) : this() => N = n;
        }

        private FairlyLargeStruct _nonReadOnlyStruct  = new FairlyLargeStruct(42);
        private readonly FairlyLargeStruct _readOnlyStruct  = new FairlyLargeStruct(42);

        private FairlyLargeReadOnlyStruct _readOnlyNonReadOnlyStruct  = new FairlyLargeReadOnlyStruct(42);
        private readonly FairlyLargeReadOnlyStruct _readOnlyReadOnlyStruct  = new FairlyLargeReadOnlyStruct(42);

        private readonly int[] _data = Enumerable.Range(1, 100_000).ToArray();

        [Benchmark]
        public int AggregateForNonReadOnlyField()
        {
            int result = 0;
            foreach (int n in _data)
                result += n + _nonReadOnlyStruct.N;
            return result;
        }
        /*
        // Method begins at RVA 0x2050
        // Code size 45 (0x2d)
        .maxstack 3
        .locals init (
            [0] int32,
            [1] int32[],
            [2] int32,
            [3] int32
        )

        IL_0000: ldc.i4.0
        IL_0001: stloc.0
        IL_0002: ldarg.0
        IL_0003: ldfld int32[] Program::_data
        IL_0008: stloc.1
        IL_0009: ldc.i4.0
        IL_000a: stloc.2
        // sequence point: hidden
        IL_000b: br.s IL_0025
        // loop start (head: IL_0025)
            IL_000d: ldloc.1
            IL_000e: ldloc.2
            IL_000f: ldelem.i4
            IL_0010: stloc.3
            IL_0011: ldloc.0
            IL_0012: ldloc.3
            IL_0013: ldarg.0
            IL_0014: ldflda valuetype Program/FairlyLargeStruct Program::_nonReadOnlyStruct
            IL_0019: call instance int32 Program/FairlyLargeStruct::get_N()
            IL_001e: add
            IL_001f: add
            IL_0020: stloc.0
            // sequence point: hidden
            IL_0021: ldloc.2
            IL_0022: ldc.i4.1
            IL_0023: add
            IL_0024: stloc.2

            IL_0025: ldloc.2
            IL_0026: ldloc.1
            IL_0027: ldlen
            IL_0028: conv.i4
            IL_0029: blt.s IL_000d
        // end loop

        IL_002b: ldloc.0
        IL_002c: ret
         */

        [Benchmark]
        public int AggregateForReadOnlyField()
        {
            int result = 0;
            foreach (int n in _data)
                result += n + _readOnlyStruct.N;
            return result;
        }
        /*
        // Method begins at RVA 0x208c
        // Code size 49 (0x31)
        .maxstack 3
        .locals init (
            [0] int32,
            [1] int32[],
            [2] int32,
            [3] int32,
            [4] valuetype Program/FairlyLargeStruct
        )

        IL_0000: ldc.i4.0
        IL_0001: stloc.0
        IL_0002: ldarg.0
        IL_0003: ldfld int32[] Program::_data
        IL_0008: stloc.1
        IL_0009: ldc.i4.0
        IL_000a: stloc.2
        // sequence point: hidden
        IL_000b: br.s IL_0029
        // loop start (head: IL_0029)
            IL_000d: ldloc.1
            IL_000e: ldloc.2
            IL_000f: ldelem.i4
            IL_0010: stloc.3
            IL_0011: ldloc.0
            IL_0012: ldloc.3
            IL_0013: ldarg.0
            IL_0014: ldfld valuetype Program/FairlyLargeStruct Program::_readOnlyStruct
            IL_0019: stloc.s 4  // pop top stack value and push it to index 4 //////////////////////////////////////////
            IL_001b: ldloca.s 4 // loads the address of a variable onto the stack (not the variable itself) ////////////
            IL_001d: call instance int32 Program/FairlyLargeStruct::get_N()
            IL_0022: add
            IL_0023: add
            IL_0024: stloc.0
            // sequence point: hidden
            IL_0025: ldloc.2
            IL_0026: ldc.i4.1
            IL_0027: add
            IL_0028: stloc.2

            IL_0029: ldloc.2
            IL_002a: ldloc.1
            IL_002b: ldlen
            IL_002c: conv.i4
            IL_002d: blt.s IL_000d
        // end loop

        IL_002f: ldloc.0
        IL_0030: ret
         */

        [Benchmark]
        public int AggregateForReadOnlyStructNonReadOnlyField()
        {
            int result = 0;
            foreach (int n in _data)
                result += n + _readOnlyNonReadOnlyStruct.N;
            return result;
        }
        /*
        // Method begins at RVA 0x20cc
        // Code size 45 (0x2d)
        .maxstack 3
        .locals init (
            [0] int32,
            [1] int32[],
            [2] int32,
            [3] int32
        )

        IL_0000: ldc.i4.0
        IL_0001: stloc.0
        IL_0002: ldarg.0
        IL_0003: ldfld int32[] Program::_data
        IL_0008: stloc.1
        IL_0009: ldc.i4.0
        IL_000a: stloc.2
        // sequence point: hidden
        IL_000b: br.s IL_0025
        // loop start (head: IL_0025)
            IL_000d: ldloc.1
            IL_000e: ldloc.2
            IL_000f: ldelem.i4
            IL_0010: stloc.3
            IL_0011: ldloc.0
            IL_0012: ldloc.3
            IL_0013: ldarg.0
            IL_0014: ldflda valuetype Program/FairlyLargeReadOnlyStruct Program::_readOnlyNonReadOnlyStruct
            IL_0019: call instance int32 Program/FairlyLargeReadOnlyStruct::get_N()
            IL_001e: add
            IL_001f: add
            IL_0020: stloc.0
            // sequence point: hidden
            IL_0021: ldloc.2
            IL_0022: ldc.i4.1
            IL_0023: add
            IL_0024: stloc.2

            IL_0025: ldloc.2
            IL_0026: ldloc.1
            IL_0027: ldlen
            IL_0028: conv.i4
            IL_0029: blt.s IL_000d
        // end loop

        IL_002b: ldloc.0
        IL_002c: ret
         */

        [Benchmark]
        public int AggregateForReadOnlyStructReadOnlyField()
        {
            int result = 0;
            foreach (int n in _data)
                result += n + _readOnlyReadOnlyStruct.N;
            return result;
        }
        /*
        // Method begins at RVA 0x2108
        // Code size 49 (0x31)
        .maxstack 3
        .locals init (
            [0] int32,
            [1] int32[],
            [2] int32,
            [3] int32,
            [4] valuetype Program/FairlyLargeReadOnlyStruct
        )

        IL_0000: ldc.i4.0
        IL_0001: stloc.0
        IL_0002: ldarg.0
        IL_0003: ldfld int32[] Program::_data
        IL_0008: stloc.1
        IL_0009: ldc.i4.0
        IL_000a: stloc.2
        // sequence point: hidden
        IL_000b: br.s IL_0029
        // loop start (head: IL_0029)
            IL_000d: ldloc.1
            IL_000e: ldloc.2
            IL_000f: ldelem.i4
            IL_0010: stloc.3
            IL_0011: ldloc.0
            IL_0012: ldloc.3
            IL_0013: ldarg.0
            IL_0014: ldfld valuetype Program/FairlyLargeReadOnlyStruct Program::_readOnlyReadOnlyStruct
            IL_0019: stloc.s 4  // pop top stack value and push it to index 4 //////////////////////////////////////////
            IL_001b: ldloca.s 4 // loads the address of a variable onto the stack (not the variable itself) ////////////
            IL_001d: call instance int32 Program/FairlyLargeReadOnlyStruct::get_N()
            IL_0022: add
            IL_0023: add
            IL_0024: stloc.0
            // sequence point: hidden
            IL_0025: ldloc.2
            IL_0026: ldc.i4.1
            IL_0027: add
            IL_0028: stloc.2

            IL_0029: ldloc.2
            IL_002a: ldloc.1
            IL_002b: ldlen
            IL_002c: conv.i4
            IL_002d: blt.s IL_000d
        // end loop

        IL_002f: ldloc.0
        IL_0030: ret
         */
    }
}