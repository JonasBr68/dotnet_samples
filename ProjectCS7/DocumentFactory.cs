using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Console;
using static System.String;

namespace ProjectCS7
{
    class DocumentFactory
    {
        public static T Create<T>(dynamic dynamicJson, IFormatProvider culture = null) where T : IDocFactory, new()
        {
            T doc = new T();
            doc.FillMember(dynamicJson.teachers, culture);
            doc.FillMember(dynamicJson.pupils, culture);

            return doc;
        }

        internal static Task<T> CreateAsync<T>(string jsonSample, IFormatProvider culture = null) where T : class, IDocFactory, new()
        {
            if (IsNullOrEmpty(jsonSample))
                throw new ArgumentNullException(nameof(jsonSample));

            return DoAsync();

            async Task<T> DoAsync()
            {
                return await Task.Run(() =>
                {
                    dynamic dynamicJson = JsonConvert.DeserializeObject(jsonSample);
                    try
                    {
                        T tupleDoc = DocumentFactory.Create<T>(dynamicJson, culture);
                        return tupleDoc;
                    }
                    catch (FormatException ex)
                    {
                        WriteLine(ex.Message);
                        return null;
                    }
                });
            }

        }

    }
}

