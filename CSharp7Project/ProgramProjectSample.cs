using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace CSharp7Project
{
    interface IValue<T>
    {
        T Value { get; }
    }
    interface IRow<T>
    {
        IEnumerable<IValue<T>> Values { get; }
    }
    interface IBaseDoc<T>
    {
        IEnumerable<IRow<string>> Fields { get; }
        IEnumerable<IRow<T>> Rows { get; }
    }

    struct Doc<T> : IBaseDoc<T>
    {
        public IEnumerable<IRow<string>> Fields => throw new NotImplementedException();

        public IEnumerable<IRow<T>> Rows => throw new NotImplementedException();
    }


    class ProgramProjectSample
    {
        // json document downloadable from http://www.impostorprogrammer.com/articles/modern_csharp/sample.json
        const string jsonSample = 
            @"{
                'teachers': [
                    {
                        'name': 'Jonas',
                        'sirname': 'Brandel',
                        'birthdate': '1968-01-02T00:00:00.000Z',
                        'height': 1.8,
                        'weight': 87.9
                    }
                ],
                'pupils': [
                    {
                        'name': 'Ruben',
                        'sirname': 'Muñoz',
                        'birthdate': '1994-01-16T00:00:00.000Z',
                        'height': 1.78,
                        'weight': 77.3
                    },
                    {
                        'name': 'Bobby',
                        'sirname': 'Sinclair',
                        'birthdate': '1973-12-01T00:00:00.000Z',
                        'height': 1.88,
                        'weight': 97.3
                    },
                    {
                        'name': 'Eleanor',
                        'sirname': 'Rigby',
                        'birthdate': '1893-12-31T00:00:00.000Z',
                        'height': 1.68,
                        'weight': 57.4
                    }
                ]
            }";

        static async Task WriteTextAsync()
        {
            using (StreamWriter writer = File.CreateText("newfile.txt"))
            {
                await writer.WriteAsync("Example text as string");
            }
        }
        static void Main(string[] args)
        {
            var myDoc = new Doc<int>();

            dynamic stuff = JsonConvert.DeserializeObject(jsonSample);

            foreach(var t in stuff.teachers)
            {
                WriteLine(t.name);
            }

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            WriteLine($"{cultures.Length} Cultures available");

            var enusCulture = CultureInfo.CreateSpecificCulture("en-us");
            var esesCulture = CultureInfo.CreateSpecificCulture("es-es");
            foreach (var t in stuff.pupils)
            {
                FormattableString pupilTemplate = $"{t.name} {t.birthdate} {t.weight}";
                WriteLine(pupilTemplate.ToString(enusCulture));
                WriteLine(pupilTemplate.ToString(esesCulture));
            }

            WriteLine(stuff);

        }
    }
}
