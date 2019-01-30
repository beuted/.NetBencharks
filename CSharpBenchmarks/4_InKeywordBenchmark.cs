using BenchmarkDotNet.Attributes;

namespace MyBenchmarks
{
    /// <summary>
    /// The in-modifier is a way to pass the argument via readonly reference. Under the hood, the argument is passed
    /// by reference with a special attribute (System.Runtime.CompilerServices.IsReadOnlyAttribute), and the compiler
    /// makes sure that the method does not modify the parameter.
    /// https://blogs.msdn.microsoft.com/seteplia/2018/03/07/the-in-modifier-and-the-readonly-structs-in-c/
    ///
    ///                     Method |      Mean |     Error |    StdDev |
    ///--------------------------- |----------:|----------:|----------:|
    ///    ReadonlyStructWithoutIn | 8.8973 ns | 0.2024 ns | 0.3325 ns |
    ///       ReadonlyStructWithIn | 0.3083 ns | 0.0300 ns | 0.0266 ns |
    ///
    /// </summary>
    public class InKeywordBenchmark
    {
        private readonly struct ReadonlyStruct
        {
            private readonly long l1, l2, l3, l4;
            public readonly int Id;

            public ReadonlyStruct(int id)
            {
                Id = id;
                l1 = 0;
                l2 = 0;
                l3 = 0;
                l4 = 0;
            }
        }

        private readonly ReadonlyStruct _readonlyStruct = new ReadonlyStruct(1);

        [Benchmark]
        public int ReadonlyStructWithoutIn()
        {
            var value = SomeFunctionReadonlyStructWithoutIn(_readonlyStruct);
            if (value == 1)
            {
                return 1;
            }

            return 0;
        }

        [Benchmark]
        public int ReadonlyStructWithIn()
        {
            var value = SomeFunctionReadonlyStructWithIn(in _readonlyStruct);
            if (value == 1)
            {
                return 1;
            }

            return 0;
        }

        private int SomeFunctionReadonlyStructWithoutIn(ReadonlyStruct myStruct)
        {
            if (myStruct.Id == 1)
            {
                return 0;
            }
            return 1;
        }
        /*
         JIT - Release mode - https://sharplab.io/
        .method private hidebysig
            instance int32 SomeFunctionReadonlyStructWithoutIn (
                valuetype MyBenchmarks.ReadonlyStruct myStruct
            ) cil managed
        {
            // Method begins at RVA 0x20a1
            // Code size 13 (0xd)
            .maxstack 8

            IL_0000: ldarg.1
            IL_0001: ldfld int32 MyBenchmarks.ReadonlyStruct::Id
            IL_0006: ldc.i4.1
            IL_0007: bne.un.s IL_000b

            IL_0009: ldc.i4.0
            IL_000a: ret

            IL_000b: ldc.i4.1
            IL_000c: ret
        } // end of method InKeywordBenchmark::SomeFunctionReadonlyStructWithoutIn
        */

        private int SomeFunctionReadonlyStructWithIn(in ReadonlyStruct myStruct)
        {
            if (myStruct.Id == 1)
            {
                return 0;
            }
            return 1;
        }

        /*
         JIT - Release mode - https://sharplab.io/
        .method private hidebysig
            instance int32 SomeFunctionReadonlyStructWithIn (
                [in] valuetype MyBenchmarks.ReadonlyStruct& myStruct
            ) cil managed
        {
            .param [1]
            .custom instance void [mscorlib]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (
                01 00 00 00
            )
            // Method begins at RVA 0x20a1
            // Code size 13 (0xd)
            .maxstack 8

            IL_0000: ldarg.1
            IL_0001: ldfld int32 MyBenchmarks.ReadonlyStruct::Id
            IL_0006: ldc.i4.1
            IL_0007: bne.un.s IL_000b

            IL_0009: ldc.i4.0
            IL_000a: ret

            IL_000b: ldc.i4.1
            IL_000c: ret
        } // end of method InKeywordBenchmark::SomeFunctionReadonlyStructWithIn
        */
    }
}