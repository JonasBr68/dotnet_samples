using System;
using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSharp7Console
{
    class RefValueTypes
    {
        private static readonly (int X, int Y) s_origin = (0, 0);

        // TODO CS72 ref readonly method
        public static ref readonly (int X, int Y) GetROStruct(){

            return ref s_origin;
        }
        public static void RefReadOnly()
        {

            //ref var roStruct = ref GetROStruct(); //error CS8329: Cannot use method 'RefValueTypes.GetROStruct()' as a ref or out value because it is a readonly variable
            // TODO CS72 ref readonly local variable
            ref readonly var roStruct = ref GetROStruct();

            //roStruct.X = 1; //error CS0131: The left-hand side of an assignment must be a variable, property or indexer
            //s_origin.X = 1; //error CS1650: Fields of static readonly field 'RefValueTypes.s_origin' cannot be assigned to (except in a static constructor or a variable initializer)

            //(int X, int Y) endPoint = (3, 4);
            var endPoint = (3, 4);

            // TODO CS71 2.0 default literal
            (int deltaX, int deltaY, double Length) res = default;

            // TODO CS72 'in' = readonly reference
            CalcLength(in roStruct, in endPoint, ref res); //No copies allowed by compiler

            //var res1 = CalcLength(in roStruct, in (3,4)); //error CS8156: An expression cannot be used in this context because it may not be passed or returned by reference

            // TODO CS72 'in' at call sites prohibits compiler making copy 
            var res2 = CalcLength(in roStruct, (3, 2), ref res); //copies allowed by compiler if omitting 'in' specifier at call site

            WriteLine(res);


            PerfWithInArguments();

            //ref readonly var p2 = Test(roStruct); //error CS8172: Cannot initialize a by-reference variable with a value
            ref readonly var p3 = ref Test(in roStruct);

        }

        // TODO CS72 argument with in specifier passed by ref readonly, optimization possible with no copying 
        public static (int deltaX, int deltaY, double Length)  CalcLength(in (int X, int Y) origin, in (int X, int Y) end, ref (int deltaX, int deltaY, double Length) res)
        {
            (res.deltaX, res.deltaY) = ((end.X - origin.X), (end.Y - origin.Y));
            res.Length = Math.Sqrt(res.deltaX * res.deltaX + res.deltaY * res.deltaY);
            return res;
        }
        const int POOL_SIZE = 1_000;
        static (int X, int Y)[] static_pool = new(int X, int Y)[POOL_SIZE];

        private static int s_nextFree = 0;
        static int NextFree {
            get => s_nextFree < POOL_SIZE ? s_nextFree++ : throw new Exception();
        }

        public static ref (int, int) Test(in (int X, int Y) p)
        {
            //inPoint.X = -1;
            //return ref copy; //error CS8168: Cannot return local 'copy' by reference because it is not a ref local
            //return inPoint; //error CS8150: By-value returns may only be used in methods that return by value
            int nextFree = NextFree;
            (static_pool[nextFree].X, static_pool[nextFree].Y) = p;
            return ref static_pool[nextFree];
        }

        public static void AlterValueType(ref (int X, int Y) t)
        {
            t.X++;
            t.Y++;
        }

        // TODO CS72 readonly struct creation of immutable type
        readonly struct ROPoint
        {
            public ROPoint(int x, int y) => (X, Y) = (x, y);

            public readonly int X;
            public readonly int Y;
        }


        public static void PerfWithInArguments()
        {
            const int SIZE = 100_000_000;

            (int X, int Y)[] rwPoints = new(int X, int Y)[SIZE];

            for(int i=0;i< SIZE; i++)
            {
                (rwPoints[i].X, rwPoints[i].Y) = (i, i * (int)Math.Round(Math.Log(i)));
            }

            (int deltaX, int deltaY, double Length)[] distances = new(int deltaX, int deltaY, double Length)[SIZE];

            //One loop to warm up memory/cpu caching
            for (int i = 1; i < SIZE; i++)
            {
                CalcLength(in rwPoints[i - 1], in rwPoints[i], ref distances[i - 1]);
            }


            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 1; i < SIZE; i++)
            {
                //(int deltaX, int deltaY, double Length) distance = default;
                var start = (rwPoints[i - 1].X, rwPoints[i - 1].Y);
                var end = (rwPoints[i].X, rwPoints[i].Y);
                CalcLength(start, end , ref distances[i]);
                //distances[i] = distance;
            }

            sw.Stop();
            WriteLine("Length copy " + sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();
            for (int i = 1; i < SIZE; i++)
            {
                CalcLength(in rwPoints[i - 1], in rwPoints[i], ref distances[i]);
            }
            sw.Stop();
            WriteLine("Length nocopying " + sw.ElapsedMilliseconds);
        }
    }
}
