using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace CSharp7Console
{
    // TODO CS70 4.1 Pattern matching
    class PatternMatching
    {
        public static void RunSamples()
        {
            SimpleIsExpressions();

            PatternWithSwitch();

            GetAgeBlock();

            VarPatternWithAnonymousType();
        }

        
        public static void SimpleIsExpressions()
        {
            int i = 0;
            object obj = i;

            //Old style 'is' expression
            if (obj is int)
            {
                WriteLine("Is int");
            }
            // TODO CS70 4.1 is type expression, if true casts and assigns to variable
            if (obj is int intVal)
            {
                WriteLine($"o is int = {intVal}");
            }

            float floatNum = 0.2f;
            obj = floatNum;
            // TODO CS70 4.2 is type expression, declared variable visible outside if
            if (obj is int intVal2) 
            {
                WriteLine($"o is = {intVal2}");
            }
            //WriteLine($"o is = {intVal2}"); error CS0165: Use of unassigned local variable 'intVal2'

            var idStrTuple = (id: 1, str: "idStrTuple");

            // TODO CS70 2.0.1 assigning tuple with named fields to object variable, field names are lost
            obj = idStrTuple;

            //WriteLine(o.str);

            WriteLine(obj.GetType().Name); //ValueTuple`2

            // TODO CS70 2.0.2 var declaration maintains named fields
            var td = idStrTuple; 
            WriteLine(td.str);

            var numberValueTuple = (number:1, value: "numberValueTuple");
            numberValueTuple = idStrTuple;

            // TODOD CS70 2.0.3 Re-assigning to other tuple with named fields and same structure, renames fields
            WriteLine(numberValueTuple.value); //idStrTuple

            // TODO CS70 4.3 Pattern macthing with tuple can only be used with long form of ValueTuple, 
            // as seen in 2.0.1 assigning to object compiler looses track of named fields
            if (obj is ValueTuple<int, string> vt)
            {
                WriteLine(vt.Item2);
                (int id, string str) tn = vt;
                WriteLine(tn.str);
            }

            //is expression in if clause not working with 'when', but easy to do with && so no need
            if (obj is var tupleObj && tupleObj.GetType() == typeof((int, string))) 
            //if (obj.GetType() == typeof((int, string))) <- Enough with this no?
            {
                WriteLine((((int _, string str))tupleObj).str); //Casting tuple/ValueTuple - unboxing conversion

                //(((int id, string str))to).str = "tuple2"; error CS0445: Cannot modify the result of an unboxing conversion



                var temp = ((int id, string str))tupleObj; //Value copy of unboxing

                ref var refTemp = ref temp; //Creates ref to value type

                var temp2 = temp; //Value copy

                temp.str = "tuple2";


                WriteLine((((int _, string str))tupleObj).str); //idStrTuple

                WriteLine(temp2.str); //idStrTuple

                WriteLine(refTemp.str); //tuple2
            }
        }


        //switch statement
        public static object CreateShape(string shapeDescription)
        {
            switch (shapeDescription)
            {
                case "circle":
                    return (2, "circle");

                case "square":
                    return (4, "square");

                case "large-circle":
                    return (12, "large-circle");

                case string o when (o.Trim()?.Length ?? 0) == 0:
                    // white space
                    return null;
                case null:
                    return null;
                default:
                    return "invalid shape description";
            }
        }

        public static void VarPatternWithAnonymousType()
        {
            var sample = new(int id, string name, int age)[] { (1, "jonas", 50), (2, "frank", 48) };

            var f48 = from s in sample where s.age == 48 select new { Name = s.name, Age = s.age };

            switch(f48.FirstOrDefault())
            {
                case var choosen when choosen.Name == "frank":
                    WriteLine(choosen.Age);
                    break;
                case null:
                default:
                    WriteLine("not found");
                    break;
            }

        }
        
        public static void GetAgeBlock()
        {
            string ageBlock;
            object age = 83;
            switch (age)
            {
                case 50:
                    ageBlock = "the big five-oh";
                    break;
                case int testAge when (new List<int>() { 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 }).Contains(testAge):
                    ageBlock = "octogenarian";
                    break;
                case int testAge when ((testAge >= 90) & (testAge <= 99)):
                    ageBlock = "nonagenarian";
                    break;
                case int testAge when (testAge >= 100):
                    ageBlock = "centenarian";
                    break;
                default:
                    ageBlock = "just old";
                    break;
            }
            WriteLine(ageBlock);
        }
        public static void PatternWithSwitch()
        {
            int i = 1;
            //object o = new object();
            //object o = "string";
            object o = i;
            if (o is object obj)
            {
                //Never true when null
                WriteLine("o is object");
            }

            if(o is var oVar) //CAREFUL is var is always true, used to assign to variable of strong type and use in when, see switch
            {
                WriteLine("o 'is var' always true");
            }
            switch (o)
            {
                case int intVal when intVal == 0:
                    {
                        WriteLine($"o is int = {intVal}");
                        break;
                    }
                case int _: //Type test
                    {
                        WriteLine("Is int");
                        break;
                    }
                case object _:
                    {
                        WriteLine("is object");
                        break;
                    }
                case null:
                    {
                        throw new ArgumentNullException();
                    }
            }
        }
    }
}
