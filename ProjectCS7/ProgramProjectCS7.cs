using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Threading;
using Newtonsoft.Json;
using static System.Console;

namespace ProjectCS7
{
    class ProgramProjectCS7
    {
        public static async Task Main(string[] args)
        {
            bool useAsyncPump = false;
            if (useAsyncPump)
            {
                AsyncPump.Run(async delegate
                {
                    await ProcessJson();
                });
            }
            else
                await ProcessJson();

            //ProtoTesting();
            //await Task.Delay(1);
        }

        private static async Task ProcessJson()
        {
            var jasonString = await GetJsonAsync("http://www.impostorprogrammer.com/articles/modern_csharp/sample.json");

            DocumentStruct tupleDoc = await DocumentFactory.CreateAsync<DocumentStruct>(jasonString);

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            List<Task> allTasks = new List<Task>();
            foreach (var culture in cultures)
            {
                Func<Task> at = async () =>
                {
                    await MakeCultureJson(tupleDoc, culture);
                    var cultureJson = await LoadCultureJsonAsync(tupleDoc, culture);
                    DocumentClass classDoc = await DocumentFactory.CreateAsync<DocumentClass>(cultureJson, culture);
                    if (!(classDoc is null))
                        Debug.Assert(tupleDoc == classDoc);
                };

                allTasks.Add(at());
            }
            await Task.WhenAll(allTasks);
        }

        private static async Task<string> LoadCultureJsonAsync(DocumentStruct tupleDoc, CultureInfo culture)
        {
            using (StreamReader reader = File.OpenText(MakeFileName(culture)))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static async Task MakeCultureJson(DocumentStruct tupleDoc, CultureInfo culture)
        {
            using (StreamWriter writer = File.CreateText(MakeFileName(culture)))
            {
                await writer.WriteAsync(tupleDoc.ToString(culture));
                WriteLine($"{culture.Name}");
            }
        }

        private static string MakeFileName(CultureInfo culture)
        {
            return $"sample_{culture.Name}.json";
        }

        private static async Task<string> GetJsonAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var stringTask = client.GetStringAsync(url);
                return await stringTask;
            }
        }
        private static void ProtoTesting()
        {
            dynamic dynamicJson = JsonConvert.DeserializeObject(jsonSample);

            DocumentStruct tupleDoc = DocumentFactory.Create<DocumentStruct>(dynamicJson);

            foreach (var t in tupleDoc.teachers)
            {
                WriteLine(t.name);
            }

            DocumentClass classDoc = DocumentFactory.Create<DocumentClass>(dynamicJson);

            foreach (var t in tupleDoc.teachers)
            {
                WriteLine(t.name);
            }

            Debug.Assert(tupleDoc == classDoc);
            Debug.Assert(classDoc == tupleDoc);

            DocumentStruct fromClassDoc = new DocumentStruct(classDoc);

            Debug.Assert(fromClassDoc == tupleDoc);
            Debug.Assert(fromClassDoc == classDoc);

            var invariantStr = fromClassDoc.ToString(CultureInfo.InvariantCulture);

            dynamic dynamicJson2 = JsonConvert.DeserializeObject(invariantStr);

            DocumentStruct tupleDoc2 = DocumentFactory.Create<DocumentStruct>(dynamicJson2, CultureInfo.InvariantCulture);

            WriteLine(tupleDoc2);
            Debug.Assert(fromClassDoc == tupleDoc2);

            var enusCulture = CultureInfo.CreateSpecificCulture("en-us");
            var strEnUs = fromClassDoc.ToString(enusCulture);
            WriteLine(strEnUs);

            var esesCulture = CultureInfo.CreateSpecificCulture("es-es");
            var strEsEs = fromClassDoc.ToString(esesCulture);
            WriteLine(strEsEs);

            dynamic dynamicJsonEs = JsonConvert.DeserializeObject(strEsEs);

            DocumentStruct tupleDocEs = DocumentFactory.Create<DocumentStruct>(dynamicJsonEs, esesCulture);
            Debug.Assert(fromClassDoc == tupleDocEs);

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            WriteLine($"{cultures.Length} Cultures available");

            foreach (var t in dynamicJson.pupils)
            {
                FormattableString pupilTemplate = $"{t.name} {t.birthdate} {t.weight}";
                WriteLine(pupilTemplate.ToString(enusCulture));
                WriteLine(pupilTemplate.ToString(esesCulture));
            }

            WriteLine(dynamicJson);
        }

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
    }
}

