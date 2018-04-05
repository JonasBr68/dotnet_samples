using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace CSharp7Console
{
    class Item
    {
        public int pos;
        public string value;
        public (int x, int y) coordinates;

        public ref (int x, int y) GetCoordinates()
        {
            return ref coordinates;
        }
    }

    class RefLocal
    {
        public static List<Item> SamplesItem { get; } = new List<Item> { new Item{ pos=0, value="item1", coordinates = (1,2) },
                                                                         new Item{ pos=1, value="item2", coordinates = (3,4) }};

        public static int[] SamplesInt { get; } = new int[] { 1, 2, 3, 4, 5, 6 };

        public static (int pos, string value)[] SamplesStruct { get; } = new(int pos, string value)[] { (1,"first"), (2, "second") };

        public static ref int GetRefAt(int pos)
        {
            return ref SamplesInt[pos];
        }

        public static ref (int pos, string value) FindTupleRef(string value)
        {
            //return ref SamplesStruct.Where(ref s => s.value == value).Select(ref s => ref s);
            for(int i=0;i<SamplesStruct.Length;i++)
            {
                ref var item = ref SamplesStruct[i];
                if (item.value == value)
                {
                    return ref item;
                }
            }

            throw new KeyNotFoundException();
        }

        // TODO CS6 Expression bodied member function
        public static (int pos, string value) FindTuple(string value) => SamplesStruct.First(s => s.value == value);

        public static void RefLocalCollection()
        {

            for(int i = 0;i<SamplesInt.Length;i++)
            {
                GetRefAt(i) = GetRefAt(i) - 1;
            }
            GetRefAt(0) = -1;
            WriteLine(SamplesInt[0]);

            foreach(int v in SamplesInt)
            {
                WriteLine(v);
            }

            //FindTuple("second").value = "changed"; //error CS1612: Cannot modify the return value of 'RefLocal.FindTuple(string)' because it is not a variable

            FindTupleRef("second").value = "changed";

            WriteLine(FindTuple("changed").value);

            SamplesItem[0].value = "changed1";
            SamplesItem[0].coordinates.x = -1;

            SamplesItem[1].GetCoordinates().y = -1; //ref returned

            WriteLine(SamplesItem[0].value);
            WriteLine(SamplesItem[0].coordinates.x);
        }
    }
}
