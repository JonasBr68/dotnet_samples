using System;
using System.Linq;
using static ProjectCS7.Converter;

namespace ProjectCS7
{
    public class DocumentStruct : DocFactoryBase<(string name, string sirname, DateTime birthdate, float height, float weight), (string name, string sirname, DateTime birthdate, float height, float weight)>
    {
        public static bool operator ==(DocumentStruct c1, DocumentStruct c2) {
            if (c1 is null || c2 is null)
            {
                return (c1 is null && c2 is null);
            }
            return c1.Equals(c2);
        }
        public static bool operator ==(DocumentStruct c1, DocumentClass c2) {
            if(c1 is null || c2 is null)
            {
                return (c1 is null && c2 is null);
            }
            return c1.teachers.SequenceEqual(c2.teachers.Select(c2t => (c2t.name, c2t.sirname, c2t.birthdate, c2t.height, c2t.weight)));
        }
        public static bool operator ==(DocumentClass c2, DocumentStruct c1) => c1 == c2;

        public static bool operator !=(DocumentStruct c1, DocumentStruct c2) => !(c1 == c2);
        public static bool operator !=(DocumentStruct c1, DocumentClass c2) => !(c1 == c2);
        public static bool operator !=(DocumentClass c2, DocumentStruct c1) => !(c1 == c2);
        

        class TupleFactory : IElementFactory<(string name, string sirname, DateTime birthdate, float height, float weight)>
        {
            public (string name, string sirname, DateTime birthdate, float height, float weight) MakeItem(dynamic c, IFormatProvider culture)
            {
                return (c.name, c.sirname, MakeType<DateTime>(c.birthdate, culture), MakeType<float>(c.height, culture), MakeType<float>(c.weight, culture));
            }

            public string ToString(IFormatProvider provider, (string name, string sirname, DateTime birthdate, float height, float weight) item)
            {

                FormattableString res = 
$@"{{
    'name':'{item.name}',
    'sirname':'{item.sirname}',            
    'birthdate':'{item.birthdate}',
    'height':'{item.height}',
    'weight':'{item.weight}'
}}";
                return res.ToString(provider);
            }

        }

        public DocumentStruct() : base(new TupleFactory(), new TupleFactory()) { }

        public DocumentStruct(DocumentClass classDoc) : this()
        {
            foreach (var p in classDoc.pupils)
            {
                (string name, string sirname, DateTime birthdate, float height, float weight) pp = (_, _, _, _, _) = p;
                pupils.Add(pp);
            }

            foreach (var t in classDoc.teachers)
            {
                (string name, string sirname, DateTime birthdate, float height, float weight) tt = (_, _, _, _, _) = t;
                teachers.Add(tt);
            }
        }


        private (string name, string sirname, DateTime birthdate, float height, float weight) MakeItem(dynamic c, IFormatProvider culture)
        {
            return (c.name, c.sirname, Converter.MakeType(c.birthdate, culture), Converter.MakeType(c.height, culture), Converter.MakeType(c.weight, culture));
        }

        public override bool Equals(object other)
        {
            if (other is DocumentStruct otherStruct)
            {
                return this.teachers.SequenceEqual(otherStruct.teachers) && this.pupils.SequenceEqual(otherStruct.pupils);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

