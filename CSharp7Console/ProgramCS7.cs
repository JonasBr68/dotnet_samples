using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using static System.Console;
using static System.Int32;



namespace CSharp7Console
{
    /// <summary>
    /// Illustration only, probably not a good pattern. 
    /// ValueTuple is a struct, meaning everything is returned by value (copied) in read-only or imutable code could be good.
    /// </summary>
    class DBEmulator
    {
        public DBEmulator() { }

        // TODO CS70 7.3 Expression bodied contructor
        public DBEmulator(IList<(string Name, IEnumerable<(int id, int Amount)> Orders)> customers) => 
                            (Customers, db_connection) = (customers, null); // TODO CS70 2.6.1  Uses tuple deconstrution into fields, 
                                                                            // TODO CS71 2.6.2 7.1 in C# 7.1 no extra cost, tuple construction/deconstruction optimized away
                                                                            // see https://stackoverflow.com/questions/41974143/c-sharp-7-expression-bodied-constructors

        public (string Name, IEnumerable<(int id, int Amount)> Orders) this[int key]
        {
            get => Customers[key]; // TODO CS70 7.4 Expression bodied get accessor in indexer 
            set
            {
                if (Customers.Count == key) //Only allows adding new for next 'free' position
                {
                    Customers.Add(value);
                }
                else
                    Customers[key] = value; //Will throw out of range if item does not exist
            }
        }
        public IList<(string Name, IEnumerable<(int id, int Amount)> Orders)> Customers { get; } = new List<(string, IEnumerable<(int id, int amount)>)>();

        public string db_connection { get; }

        public static DBEmulator Instance {
            get {
                return _emulator ?? InitSingleton();
            }
        }

        // TODO CS70 2.9.3 return tuples from linq query in method, with anonymous not possible to delcare the return type
        public IEnumerable<(string Name, int OrderTotal)> QueryCustomersOrdersOver(int orderValue)
        {
            return from c in (from cTotal in Customers select (Name: cTotal.Name, OrderTotal: cTotal.Orders.Sum(o => o.Amount))) where c.OrderTotal > 100 select c;
        }
        private static DBEmulator InitSingleton()
        {
            _emulator = new DBEmulator()
            {
                [0] = ("Bobby", new(int id, int amount)[] { (0, 100) }),
                [1] = ("Carlos", new(int id, int amount)[] { (0, 200), (1, 350) }),
                [2] = ("Amanda", new(int id, int amount)[] { (0, 20), (1, 50), (2,45) })
            };

            return _emulator;
        }
        private static DBEmulator _emulator = null;
    }
    static class DeconstructExtensions
    {
        // TODO CS70 2.6.9.4 Extension method deconstructor with expression body
        public static void Deconstruct(this DateTime This, out int day, out int month, out int year) 
                                    => (day, month, year) = (This.Day, This.Month, This.Year);

    }

    class TupleEnumerable<T> : IEnumerable<T>
    {
        private ITuple m_wrapped { get; }
        public TupleEnumerable(ITuple wrapped)
        {
            m_wrapped = wrapped;
        }

        public static TupleEnumerable<T> Create(ITuple wrapped)
        {
            return new TupleEnumerable<T>(wrapped);
        }


        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < m_wrapped.Length; i++)
            {
                if(m_wrapped[i] is T)
                    yield return (T)m_wrapped[i];
                else if(m_wrapped[i] is ITuple nestedITuple)
                {
                    TupleEnumerable<T> nested = new TupleEnumerable<T>(nestedITuple);
                    foreach(var n in nested)
                    {
                        yield return n;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    class Area
    {
        // TODO CS70 7.0 Expression bodied constructor, deconstructing tuple straight into member variables
        internal Area(in (int x, int y, int width, int height) areaTuple) => (X, Y, Width, Height) = areaTuple;
                                                                            //(areaTuple.x,
                                                                            //areaTuple.y,
                                                                            //areaTuple.width,
                                                                            //areaTuple.height);

        // TODO CS70 7.1 Expression-bodied finalizer
        ~Area() => Console.Error.WriteLine("Finalized!");

        internal Area(int x, int y, int width, int height) => (X, Y, Width, Height) = (x, y, width, height);
        //{
        //    X = x;
        //    Y = y;
        //    Width = width;
        //    Height = height;
        //}
        int X { get; } = 0;
        int Y { get; } = 0;

        int Width { get; } = 0;
        int Height { get; } = 0;

        // TODO CS70 2.6.5 User defined custom deconstructor
        internal void Deconstruct(out int x, out int y, out int width, out int height)
        {
            x = this.X;
            y = this.Y;
            width = this.Width;
            height = this.Height;
        }
        internal void Deconstruct(out int width, out int height)
        {
            height = this.Height;
            width = this.Width;
        }

        private string label;
        public string Label
        {
            get => label;
            // TODO CS70 8.0 throw expression, now throw possible in expression bodied members and properties
            set => label = value ?? throw new ArgumentNullException(paramName: nameof(Label), message: "New label must not be null");
        }

        internal void Deconstruct(out int width)
        {
            width = this.Width;
        }

        // TODO CS70 2.6.8.1 User defined conversion to tuple
        public static implicit operator (int x, int y, int width, int height) (Area a)
        {
            return (a.X, a.Y, a.Width, a.Height);
        }

        // TODO CS70 2.6.9.2 User defined implicit conversion operator tuple to object
        public static implicit operator Area(in (int x, int y, int width, int height) areaTuple)
        {
            return new Area(in areaTuple);
        }
    }

    class Program
    {
        static bool IsInteger(string input)
        {
            return int.TryParse(input, out _); // TODO CS70 3.1 Discards of out variable
        }
        static void OutVariables()
        {
            //Old way
            var input = "34";
            int numericResult;
            if (int.TryParse(input, out numericResult))
                WriteLine(numericResult);
            else
                WriteLine("Could not parse input");


            // TODO CS70 1.0 out variable declarations, in situ visible in current scope
            {
                if (int.TryParse(input, out var result))
                    WriteLine(result);
                else
                    WriteLine("Could not parse input" + result);
            }
            //WriteLine(result);
        }


        // TODO CS70 2.0   Tuples and Deconstrution 
        // In other languages Deconstruction = pattern matching

        //Not yet fully supported in linq and lambdas, suggestions are:
        //https://github.com/dotnet/csharplang/issues/189 ("let" and "from")
        //https://github.com/dotnet/csharplang/issues/258 ("lambda")

        // TODO CS70 2.1 Returns a ValueTuple<string,string,int> and aliased members 
        // Item1 = name, Item2 = sirname and Item3 = age
        static (string name, string sirname, int age) GetPerson(int id)
        {
            return ("Jonas" + id, "Brandel" + id, 50 + id);
        }

        // TODO CS70 2.1.0 Also returns a ValueTuple<string,string,int> and aliased members Item1 = name, Item2 = sirname and Item3 = age
        static (string name, string sirname, int age) GetPersonVT(int id)
        {
            return new ValueTuple<string,string, int>("Jonas" + id, "Brandel" + id, 50 + id);
        }

        // TODO CS70 2.1.01 Returns a ValueTuple<string,string,int> with standard/anonymous fields Item1, Item2 and Item3
        static (string, string, int) GetPersonVTAnonymous(int id)
        {
            return new ValueTuple<string, string, int>("Jonas" + id, "Brandel" + id, 50 + id);
        }

        // TODO CS70 2.2 Returns old style Tuple<string, string, int>
        static Tuple<string, string, int> GetPersonT(int id)
        {
            return new Tuple<string, string, int>("Jonas" + id, "Brandel" + id, 50 + id);
        }


        // TODO CS70 2.3 Convert old style Tuple<...> to ValueTuple<...> with compiler magic names
        static (string name, string sirname, int age) ConvertTupleToNamedValueTuple(Tuple<string, string, int> t)
        {
            return (t.Item1, t.Item2, t.Item3);
        }

        static void DeconstructionAndTuples()
        {
            // TODO CS70 2.4.0 New ValueTuple with name aliases for field items, custom field names exist only at compile time not in reflection
            var personValueTuple1 = GetPerson(0);
            WriteLine(personValueTuple1.name);
            WriteLine(personValueTuple1.GetType().Name); //ValueTuple`3


            //Old Tuple<...>
            var personOldStyleTuple = GetPersonT(1);
            WriteLine(personOldStyleTuple.Item1);
            WriteLine(personOldStyleTuple.GetType().Name); //Tuple`3


            // TODO CS70 2.5 Convert old Tuple to new ValueTuple with name aliases for Items
            var personValueTuple2 = ConvertTupleToNamedValueTuple(personOldStyleTuple);
            WriteLine(personValueTuple2.name);

            // TODO CS70 2.6 Deconstructs Tuple<> into named variables
            (string name, string sirname, int age) = GetPersonT(2);
            WriteLine(name);

            // TODO CS70 2.7 No automatic conversion of Tuple<> to tuple (ValueTuple)
            //(string name, string sirname, int age) t2 = GetPersonT(22); 

            // TODO CS70 2.7.1 Conversion of Tuple<> to tuple (ValueTuple) with conversion method
            (string name, string sirname, int age) personValueTuple3 = ConvertTupleToNamedValueTuple(GetPersonT(22));

            // TODO CS70 2.7.3 Conversion of Tuple<> to tuple (ValueTuple) with framework extension method
            (string, string, int) valueTuple = GetPersonT(33).ToValueTuple();
            (string name, string sirname, int age) valueTupleNamedFields = GetPersonT(44).ToValueTuple();

            // TODO CS70 2.7.4 Examples with 'raw' ValueTuple - same type
            (string name, string sirname, int age) t55 = new ValueTuple<string, string, int>("name", "sirname", 50);
            (string, string, int) t56 = new ValueTuple<string, string, int>("name", "sirname", 50);


            // TODO CS70 2.4.1 field name only respected when using 'var' declaration on left side
            var tuple = (name, SirName: sirname, age);

            WriteLine(tuple.Item1);
            WriteLine(tuple.SirName);

            // TODO CS71 3.0 inferred name from variable name
            WriteLine(tuple.name);

            // TODO CS70 2.4.2 named tuple field ignored, have to use var instead of tuple declaration on left side for that
            (string, string, int) tuple2 = ( name, SirName: sirname, age);


            // TODO CS70 2.4.3 Using cast names fields - NOTE casting requires double parantesis
            var castedTuple2 = ((string name, string SirName, int age))tuple2;
            WriteLine(castedTuple2.SirName);

            // TODO CS70 2.4.4 Cannot cast old Tuple<> BUT convert to ValueTuple and cast works fine and creates compiled named fields
            // ToValueTuple() is framework extension method
            var castedOldTuple = ((string name, string SirName, int age))personOldStyleTuple.ToValueTuple();

            // TODO CS70 2.6.2 variables can be reused in tuple deconstruction
            (name, sirname, age) = GetPerson(3); //Reusing and reassign variables in deconstrution

            // TODO CS70 3.0 Discards, is used to allow compiler to match up with a tuple structure, but value is not used - discarded
            var (_, _, age4) = GetPerson(4); 
            WriteLine(age4);

            // TODO CS70 2.1.1 the language tuple type is backed by ValueTuple from the framework
            ValueTuple<string, string, int> explicitValueTuple = GetPerson(5);

            // TODO CS70 2.1.2 C# language tuple is treated as same type as ValueTuple (identity conversion)
            var varDeclaredTuple = GetPerson(5);

            // TODO CS70 2.1.4 tuple with named fields same type as tuple without, declaring tuple this way looses names
            (string, string, int) personTupleNoNames = GetPerson(5); 

            Debug.Assert(explicitValueTuple.GetType() == varDeclaredTuple.GetType());
            Debug.Assert(varDeclaredTuple.GetType() == personTupleNoNames.GetType());

            WriteLine(varDeclaredTuple.name);

            //WriteLine(personTupleNoNames.name); //error CS1061: '(string, string, int)' does not contain a definition for 'name'



            // TODO CS70 2.0.3 Creates tuple 
            var tupleNumbers = (1, 2, 3);

            // TODO CS70 2.6.3 Tuple deconstruction into separate variables
            (int first, int second, int third) = tupleNumbers;


            // TODO CS70 2.1.3 Creates tuple with named fields
            var t = (firstValue: 1, secondValue: 2, thirdValue: 3);

            WriteLine(t.firstValue);

            // TODO CS70 2.1.4 Creates tuple with 'anonymous' fields, Item1, Item2, etc
            var t2 = (1, 2, 3);
            WriteLine(t2.Item2);
            WriteLine(t2.GetType().Name);


            // TODO CS70 2.1.5 Creates anonymous tuple in situ and extracts one field, x2
            WriteLine((x1: 1, x2: 2, x3: 3).x2);
        }

        static (int a, (int a, (int a, (int a, int b) b) b) b) MakeNested()
        {
            return (1, (2, (3, (4, 5))));
        }


        static void NestedTuplesAndDeconstruction()
        {
            // TODO CS70 2.8.0  Nested Anonymous types similar to nested tuples
            var ao = new { a = 1, b = new { a = 2, b = new { a = 3, b = new { a = 4, b = 5 } } } };
            var ao2 = new { a = 1, b = new { a = 2, b = new { a = 3, b = new { a = 4, b = 5 } } } };

            Debug.Assert(ao.GetType() == ao2.GetType()); //Same anonymous structure is same type, but...
            WriteLine(ao);

            // BUT Anonymous objects type cannot be declared, tuples can be declared on variables and fields/members

            var at = (a: 1, b: (a: 2, b: (a: 3, b: (a: 4, b: 5))));

            // TODO CS70 2.8.01 Method MakeNested() returns same type, and can be explicitly declared too
            at = MakeNested();

            (int a, (int a, (int a, (int a, int b) b) b) b) at2 = MakeNested();

            // TODO CS70 2.8.02 Same type
            Debug.Assert(at.GetType() == at2.GetType());


            // // TODO CS70 2.8.03 It is possible with reflection to create anoymous object of same type, but not to declare the type
            var so = new { a = 1 };
            var another_so = Activator.CreateInstance(so.GetType(), new object[] { 2 });

            Debug.Assert(so.GetType() == another_so.GetType());


            // TODO CS70 2.8.1 Nested tuples
            var t1 = (1, (2, (3, (4, 5))));
            WriteLine(t1);
            WriteLine((t1.Item1, 
                       t1.Item2.Item1, 
                       t1.Item2.Item2.Item1, 
                       t1.Item2.Item2.Item2.Item1, 
                       t1.Item2.Item2.Item2.Item2));

            // TODO CS70 2.8.2 Nested tuples with named fields
            var t2 = MakeNested();
            WriteLine(( t2.a, 
                        t2.b.a, 
                        t2.b.b.a, 
                        t2.b.b.b.a, 
                        t2.b.b.b.b));

            // TODO CS70 2.8.3 Deconstructing and discard of nested tuples with named fields
            (_, (_, (_, (int a, int b)))) = t2;
            WriteLine((a, b).ToString());


            // TODO CS70 2.8.4 Tuples might look similar to arrays
            var t3 = (1, 2, 3, 4, 5);

            // TODO CS70 2.8.5 Tuples might look similar to arrays
            var arr = new int[] { 1, 2, 3, 4, 5 };

            // TODO CS70 2.8.6 ITuple compiler/tools interface to enumerate over a tuple
            // Requires .NET 4.7.1 for a public ITuple interface
            ITuple e = t3;

            // TODO CS70 2.8.7 tuple enumerator example
            var tw = TupleEnumerable<int>.Create(t3);
            WriteLine(tw.Sum());

            var tw2 = TupleEnumerable<int>.Create(t1);
            WriteLine(tw2.Sum());
        }


        static void TupleArray()
        {
            //  TODO CS70 2.8.8 creating an array of tuples with named fields
            var tuples = new(int id, int value)[10];

            WriteLine(tuples[5].id + tuples[5].value);
        }

        // TODO CS70 2.9.1 Tuples in Linq
        static void TuplesAndLinq()
        {
            // Experiement...
            // TODO CS70 2.9.5 DBEmulator is the only explicitly declared type good/bad usage? 
            // With tuple being value type, no values can easily be changed - read only scenario

            var customers = DBEmulator.Instance.Customers;

            var first = customers.First();

            WriteLine($"Customer {first.Name} has {first.Orders.Count()} orders");

            var ordered = from c in customers where c.Orders.Count() > 0 orderby c.Name select c;

            var ordered1 = customers.Where(c => c.Orders.Count() > 0).OrderBy(c => c.Name);

            var firstOrdered = ordered.First();

            WriteLine(firstOrdered.Name);

            // TODO CS70 2.9.2 Returning tuples in linq instead of anonymous objects
            var totalOrders = from cTotal in ordered select (Name: cTotal.Name, OrderTotal:cTotal.Orders.Sum(o => o.Amount));


            // TODO CS70 2.9.3 Returning tuples in linq behind method not possible with anonymous objects (cannot declare type)
            // Now possible to declare method doing query and returning 'anonymous' object in shape of tuples
            foreach (var (customerName, customerOrderTotal) in DBEmulator.Instance.QueryCustomersOrdersOver(100))
            {
                WriteLine($"{customerName} has {customerOrderTotal} in orders");
            }

            // TODO CS70 2.9.4 Select creates new tuple from query with field of tuple summed up, and then sums up sum of the sums
            var total = DBEmulator.Instance.Customers.Select(c => (name:c.Name, orders:c.Orders.Sum(o => o.Amount))).Sum(o => o.orders);
            WriteLine(total);
        }


        // TODO CS70 2.9.0 Large tuples get nested on 8th field, compiler hides this somewhat
        static void TupleWith8OrMore()
        {
            // TODO CS70 2.9.01 tuple with 10 elements looks like normal 'linear' tuple
            var tuple10Values = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            WriteLine(tuple10Values.Item9);

            // TODO CS70 2.9.02 declaring with 'tuple' syntax also seems normal
            (int, int, int, int, int, int, int, int, int, int) t8_1= tuple10Values;

            // TODO CS70 2.9.03 BUT declaring with ValueTuple syntax it is not transparent any more
            ValueTuple<int, int, int, int, int, int, int, ValueTuple<int, int, int>> tuple10Values2 = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            // TODO CS70 2.9.03 still deconstructs transparently
            WriteLine(tuple10Values2.Item10);

            // TODO CS70 2.9.04 Deconstruction into 10 variables looks normal as well
            (_, _, _, _, _, _, _, int eight, _, _) = tuple10Values2;

            // TODO CS70 2.9.05 This with a nested tuple does not compile, altough intuitively one would expect so
            // ValueTuple<int, int, int, int, int, int, int, ValueTuple<int, int, int>> t8e2 = (1, 2, 3, 4, 5, 6, 7, (8, 9, 10));

            // TODO CS70 2.9.06 When structure is really nested, the compiler wraps the nested tuple in a single ValueTuple
            ValueTuple<int, int, int, int, int, int, int, ValueTuple<ValueTuple<int, int, int>>> tuple10Values3Nested = (1, 2, 3, 4, 5, 6, 7, (8, 9, 10));

            // TODO CS70 2.9.07 ValueTuple with single value exist, but not useful, really used by compiler only
            ValueTuple<int> tupleOneValue = new ValueTuple<int>(9);
            int v = tupleOneValue.Item1;


            var tuple10Values3Nested2 = (1, 2, 3, 4, 5, 6, 7, (8, 9, 10));


            // TODO CS70 2.9.08 Deconstruction of nested tuple with discards 
            (_, _, _, _, _, _, _, (int eightNested, _, _)) = tuple10Values3Nested2;

            

            Debug.Assert(tuple10Values.GetType() == tuple10Values2.GetType());
            Debug.Assert(tuple10Values.GetType() != tuple10Values3Nested2.GetType());
            Debug.Assert(tuple10Values3Nested.GetType() == tuple10Values3Nested2.GetType());


            // TODO CS70 2.9.09 - tuple type structure
            WriteLine(tuple10Values.GetType().ToString());

            /* System.ValueTuple`8
             * [
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.ValueTuple`3
             *      [
             *          System.Int32,
             *          System.Int32,
             *          System.Int32
             *      ]
             * ] 
             **/

            WriteLine(tuple10Values3Nested2.GetType().ToString());

            /* System.ValueTuple`8
             * [
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.Int32,
             *      System.ValueTuple`1
             *      [
             *          System.ValueTuple`3
             *          [
             *              System.Int32,
             *              System.Int32,
             *              System.Int32
             *          ]
             *      ]
             *]
             **/
        }

        static void CustomDeconstructor()
        {
            //Normal constructor
            var area = new Area(1, 1, 100, 200);

            // TODO CS70 2.6.4 Using custom deconstructor, and discards 
            var (_, _, width, _) = area;
            WriteLine(width);

            // TODO CS70 2.6.6 Using different custom deconstructor, and discard 
            (_, int height) = area;

            var area2 = new Area(1, 1, 300, 200);

            //Calling deconstructor like a normal method
            //var (w) = area2;
            area2.Deconstruct(out var width2); // TODO CS70 1.1 out variable declared in situ
            WriteLine(width2);


            // TODO CS70 2.6.7 Trick to convert from type to tuple if has Deconstructor
            (int x, int y, int width, int height) f = (_, _, _, _) = area2; 
            WriteLine(f.height);

            //(int x, int y, int width, int height) f2 = (int a, int b, int c, int d) = area2; //Does not compile ?

            // TODO CS70 2.6.8 User defined conversion - see 2.6.8.1
            (int x, int y, int width, int height) converted = area2; 
            WriteLine(converted);

            // TODO CS70 2.6.9 Constructor that takes a tuple
            Area constructed = new Area(in converted);

            // TODO CS70 2.6.9.1 User defined implicit conversion operator see 2.6.9.2
            Area convertedAgain = converted; 
        }

        static void ExtensionDeconstruct()
        {
            // TODO CS70 2.6.9.3 Extension method deconstructor see 2.6.9.4
            var (day, month, year) = DateTime.Now;

            WriteLine($"Today is {day}/{month}/{year}"); 
        }

        // TODO CS70 9.0 Improved literals

        // TODO CS70 9.1 Binary literals
        public const int One = 0b0001;
        public const int Two = 0b0010;
        public const int Four = 0b0100;
        public const int Eight = 0b1000;

        // TODO CS70 9.2 Binary literals with digit separator
        public const int Sixteen = 0b0001_0000;
        public const int ThirtyTwo = 0b0010_0000;
        public const int SixtyFour = 0b0100_0000;
        public const int OneHundredTwentyEight = 0b1000_0000;

        // TODO CS72 Leading digit separator _ in binary and hex literals
        public const int LeadingSeparatorBin = 0b_1010_0110_0000_1010_0110;
        public const int LeadingSeparatorHex = 0x_0A_60_A6;

        // TODO CS70 9.3 Large base 10 with digit separator
        public const long BillionsAndBillions = 100_000_000_000;

        // TODO CS70 9.4 Digit separator with double, decimal and float
        public const float AvogadroConstantFloat = 6.022_140_857_747_474e23f;
        public const double AvogadroConstant = 6.022_140_857_747_474e23d;
        public const decimal GoldenRatio = 1.618_033_988_749_894_848_204_586_834_365_638_117_720_309_179M;

        static void Main(string[] args)
        {

            OutVariables();

            DeconstructionAndTuples();

            TupleWith8OrMore();

            NestedTuplesAndDeconstruction();

            TupleArray();

            TuplesAndLinq();

            CustomDeconstructor();

            ExtensionDeconstruct();

            //Pattern macthing
            PatternMatching.RunSamples();

            //Ref local
            RefLocal.RefLocalCollection();

            var myFunc = LocalFunctions.ReturnLocalFunction();
            myFunc();

            var myLambda = LocalFunctions.ReturnLocalLambda();
            myLambda();

            var task = LocalFunctions.ThrowsAsync("Hello async");
            try
            {
                task.Wait();
            }
            catch(AggregateException ex)
            {
                WriteLine(ex.ToString());
            }


            try
            {
                var task2 = LocalFunctions.LocalFunctionAsync(null);
                try
                {
                    task.Wait();
                }
                catch (AggregateException ex)
                {
                    WriteLine(ex.ToString());
                }
            }
            catch (ArgumentNullException ex)
            {
                WriteLine(ex.ToString());
            }


            //C# 7.2
            RefValueTypes.RefReadOnly();


            NullableTypesAndDefault();
        }

        private static void NullableTypesAndDefault()
        {
            (int id, string name)? myTuple = null;

            var myNonNullTuple = myTuple ?? default((int id, string name)?);
            WriteLine(myNonNullTuple?.name);

            int? nullableInt = null;

            var nonNullInt = nullableInt ?? default(int?);
            nonNullInt = null;

            var nonNullInt2 = nullableInt ?? default(int);
            //nonNullInt2 = null; //error CS0037: Cannot convert null to 'int' because it is a non-nullable value type

            var nonNullInt3 = nullableInt ?? default; //null coalescing with new default literal resolves var to its non-the nullabel type

            //nonNullInt3 = null; //error CS0037: Cannot convert null to 'int' because it is a non-nullable value type

        }
    }
}
