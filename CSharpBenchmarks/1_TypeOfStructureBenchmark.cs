using BenchmarkDotNet.Attributes;

namespace MyBenchmarks
{
    /// <summary>
    /// In the Microsoft implementation of C# on the desktop CLR, value types are stored on the stack when the value is
    /// a local variable or temporary that is not a closed-over local variable of a lambda or anonymous method, and the
    /// method body is not an iterator block, and the jitter chooses to not enregister the value.
    ///
    /// Struct here are more efficient because stored on the stack.
    ///
    ///                 Method |     Mean |     Error |    StdDev |
    /// ---------------------- |---------:|----------:|----------:|
    ///      ReadonlyClassTest | 6.102 ns | 0.1024 ns | 0.0958 ns |
    ///  NonReadonlyStructTest | 1.463 ns | 0.0636 ns | 0.0531 ns |
    ///     ReadonlyStructTest | 1.465 ns | 0.0622 ns | 0.0639 ns |
    ///
    /// https://docs.microsoft.com/en-us/dotnet/csharp/write-safe-efficient-code
    /// </summary>
    public class TypeOfStructureBenchmark
    {
        private struct NonReadonlyStruct
        {
            private readonly long l1, l2, l3, l4;
            public readonly int Id;

            public NonReadonlyStruct(int id)
            {
                Id = id;
                l1 = 0;
                l2 = 0;
                l3 = 0;
                l4 = 0;
            }
        }

        private class ReadonlyClass
        {
            private readonly long l1, l2, l3, l4;
            public readonly int Id;

            public ReadonlyClass(int id)
            {
                Id = id;
                l1 = 0;
                l2 = 0;
                l3 = 0;
                l4 = 0;
            }
        }

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

        [Benchmark]
        public int ReadonlyClassTest()
        {
            var readonlyClass = new ReadonlyClass(1);
            if (readonlyClass.Id == 1)
            {
                return 0;
            }
            return 1;
        }
        /*
        .method public hidebysig
            instance int32 ReadonlyClassTest () cil managed
        {
            // Method begins at RVA 0x2050
            // Code size 18 (0x12)
            .maxstack 8

            IL_0000: ldc.i4.1
            IL_0001: newobj instance void MyBenchmarks.TypeOfStructureBenchmark/ReadonlyClass::.ctor(int32)
            IL_0006: ldfld int32 MyBenchmarks.TypeOfStructureBenchmark/ReadonlyClass::Id
            IL_000b: ldc.i4.1
            IL_000c: bne.un.s IL_0010

            IL_000e: ldc.i4.0
            IL_000f: ret

            IL_0010: ldc.i4.1
            IL_0011: ret
        } // end of method TypeOfStructureBenchmark::ReadonlyClassTest
        */

        [Benchmark]
        public int NonReadonlyStructTest()
        {
            var nonReadonlyStruct = new NonReadonlyStruct(1);
            if (nonReadonlyStruct.Id == 1)
            {
                return 0;
            }
            return 1;
        }
        /*
        .method public hidebysig
            instance int32 NonReadonlyStructTest () cil managed
        {
            // Method begins at RVA 0x2063
            // Code size 18 (0x12)
            .maxstack 8

            IL_0000: ldc.i4.1
            IL_0001: newobj instance void MyBenchmarks.TypeOfStructureBenchmark/NonReadonlyStruct::.ctor(int32)
            IL_0006: ldfld int32 MyBenchmarks.TypeOfStructureBenchmark/NonReadonlyStruct::Id
            IL_000b: ldc.i4.1
            IL_000c: bne.un.s IL_0010

            IL_000e: ldc.i4.0
            IL_000f: ret

            IL_0010: ldc.i4.1
            IL_0011: ret
        } // end of method TypeOfStructureBenchmark::NonReadonlyStructTest
        */

        [Benchmark]
        public int ReadonlyStructTest()
        {
            var readonlyStruct = new ReadonlyStruct(1);
            if (readonlyStruct.Id == 1)
            {
                return 0;
            }
            return 1;
        }
        /*
        .method public hidebysig
            instance int32 ReadonlyStructTest () cil managed
        {
            // Method begins at RVA 0x2076
            // Code size 18 (0x12)
            .maxstack 8

            IL_0000: ldc.i4.1
            IL_0001: newobj instance void MyBenchmarks.TypeOfStructureBenchmark/ReadonlyStruct::.ctor(int32)
            IL_0006: ldfld int32 MyBenchmarks.TypeOfStructureBenchmark/ReadonlyStruct::Id
            IL_000b: ldc.i4.1
            IL_000c: bne.un.s IL_0010

            IL_000e: ldc.i4.0
            IL_000f: ret

            IL_0010: ldc.i4.1
            IL_0011: ret
        } // end of method TypeOfStructureBenchmark::ReadonlyStructTest
        */
    }
}