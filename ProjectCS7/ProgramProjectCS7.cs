using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Console;

namespace ProjectCS7
{
    interface IDocFactory
    {
        //void FillMember(dynamic dynamicChild);
        void FillMember(dynamic dynamicChild);
    }

    interface IElementFactory<T>
    {
        T MakeItem(dynamic c);

    }
    //class ElementType
    //{
    //}
    //class TeacherTag : ElementType
    //{
    //    public static TeacherTag Tag { get; } = new TeacherTag();
    //}
    //class PupilTag : ElementType
    //{
    //    public static PupilTag Tag { get; } = new PupilTag();
    //}
    abstract class DocFactoryBase<TeacherType, PupilType> : IDocFactory
    {
        protected IElementFactory<TeacherType> TeacherFactory { get; }
        protected IElementFactory<PupilType> PupilFactory { get; }
        public DocFactoryBase(IElementFactory<TeacherType> teacherFactory, IElementFactory<PupilType> pupilFactory) => (TeacherFactory, PupilFactory) = (teacherFactory, pupilFactory);
        public IList<TeacherType> teachers { get; }
        public IList<PupilType> pupils { get; }
        public void FillMember(dynamic dynamicChild) 
        {
            WriteLine($"Filling {dynamicChild.Parent.Name}");
            switch (dynamicChild)
            {
                case JArray array when dynamicChild.Parent.Name == "pupils":
                    {
                        foreach (var c in dynamicChild)
                        {
                            var pupil = PupilFactory.MakeItem(c);
                            this.pupils.Add(pupil);
                        }
                        break;
                    }
                case JArray array when dynamicChild.Parent.Name == "teachers":
                    {
                        foreach (var c in dynamicChild)
                        {
                            var teacher = TeacherFactory.MakeItem(c);
                            this.pupils.Add(teacher);
                            //this.teachers.Add((c.name, c.sirname, c.birthdate, c.height, c.weight));
                        }
                        break;
                    }
                case null:
                    {
                        throw new ArgumentNullException(nameof(dynamicChild));
                    }
            }
            //foreach(var child in )
        }


        
    }
    class DocumentClass : DocFactoryBase<Teacher, Pupil>
    {
        public IList<Teacher> teachers { get; } = new List<Teacher>();
        public IList<Pupil> pupils { get; } = new List<Pupil>();

        class TeacherClassFactory : IElementFactory<Teacher>
        {
            public Teacher MakeItem(dynamic c)
            {
                throw new NotImplementedException();
            }
        }
        class PupilClassFactory : IElementFactory<Pupil>
        {
            public Pupil MakeItem(dynamic c)
            {
                throw new NotImplementedException();
            }
        }
        public DocumentClass() : base(new TeacherClassFactory(), new PupilClassFactory()) => (teachers, pupils) = (new List<Teacher>(), new List<Pupil>());
        //public IList<Teacher> teachers { get; } = new List<Teacher>();
        //public IList<Pupil> pupils { get; } = new List<Pupil>();

    }

    public class Pupil
    {
    }

    public class Teacher
    {
    }

    class DocumentStruct : DocFactoryBase<(string name, string sirname, DateTime birthdate, float height, float weight), (string name, string sirname, DateTime birthdate, float height, float weight)>
    {
        class TupleFactory : IElementFactory<(string name, string sirname, DateTime birthdate, float height, float weight)>
        {
            public (string name, string sirname, DateTime birthdate, float height, float weight) MakeItem(dynamic c)
            {
                return (c.name, c.sirname, c.birthdate, c.height, c.weight);
            }
            public static TupleFactory Instance { get; } = new TupleFactory();
        }

        public DocumentStruct() : base(TupleFactory.Instance, TupleFactory.Instance) { }
        //public IList<(string name, string sirname, DateTime birthdate, float height, float weight)> teachers { get; } = new List<(string name, string sirname, DateTime birthdate, float height, float weight)>();
        //public IList<(string name, string sirname, DateTime birthdate, float height, float weight)> pupils { get; } = new List<(string name, string sirname, DateTime birthdate, float height, float weight)>();

        public void FillMemberX(dynamic dynamicChild)
        {
            WriteLine($"Filling {dynamicChild.Parent.Name}");
            switch(dynamicChild)
            {
                case JArray array when dynamicChild.Parent.Name == "pupils":
                    {
                        foreach (var c in dynamicChild)
                        {
                            this.pupils.Add(this. MakeItem(c));
                        }
                        break;
                    }
                case JArray array when dynamicChild.Parent.Name == "teachers":
                    {
                        foreach (var c in dynamicChild)
                        {
                            this.teachers.Add((c.name, c.sirname, c.birthdate, c.height, c.weight));
                        }
                        break;
                    }
                case null:
                    {
                        throw new ArgumentNullException(nameof(dynamicChild));
                    }
            }
            //foreach(var child in )
        }

        private (string name, string sirname, DateTime birthdate, float height, float weight) MakeItem(dynamic c)
        {
            return (c.name, c.sirname, c.birthdate, c.height, c.weight);
        }
    }

    class DocumentFactory 
    {
        public static T Create<T>(dynamic dynamicJson) where T : IDocFactory, new()
        {
            T doc = new T();

            doc.FillMember(dynamicJson.teachers);
            doc.FillMember(dynamicJson.pupils);

            return doc;
        }


    }

    class ProgramProjectCS7
    {

        static async Task WriteTextAsync()
        {
            using (StreamWriter writer = File.CreateText("newfile.txt"))
            {
                await writer.WriteAsync("Example text as string");
            }
        }
        public static async Task Main(string[] args)
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


            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            WriteLine($"{cultures.Length} Cultures available");

            var enusCulture = CultureInfo.CreateSpecificCulture("en-us");
            var esesCulture = CultureInfo.CreateSpecificCulture("es-es");
            foreach (var t in dynamicJson.pupils)
            {
                FormattableString pupilTemplate = $"{t.name} {t.birthdate} {t.weight}";
                WriteLine(pupilTemplate.ToString(enusCulture));
                WriteLine(pupilTemplate.ToString(esesCulture));
            }

            WriteLine(dynamicJson);
            await Task.Delay(1);
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
