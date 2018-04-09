using System;
using System.Linq;
using static ProjectCS7.Converter;

namespace ProjectCS7
{
    public class DocumentClass : DocFactoryBase<Teacher, Pupil>
    {
        public static bool operator ==(DocumentClass c1, DocumentClass c2)
        {
            return c1.Equals(c2);
        }
        public static bool operator !=(DocumentClass c2, DocumentClass c1) => !(c1 == c2);

        class TeacherClassFactory : IElementFactory<Teacher>
        {
            public Teacher MakeItem(dynamic c, IFormatProvider culture)
            {
                return new Teacher((string)c.name, (string)c.sirname, MakeType<DateTime>(c.birthdate, culture), MakeType<float>(c.height, culture), Converter.MakeType<float>(c.weight, culture));
            }

            public string ToString(IFormatProvider provider, Teacher item)
            {
                throw new NotImplementedException();
            }
        }
        class PupilClassFactory : IElementFactory<Pupil>
        {
            public Pupil MakeItem(dynamic c, IFormatProvider culture)
            {
                return new Pupil((string)c.name, (string)c.sirname, MakeType<DateTime>(c.birthdate, culture), MakeType<float>(c.height, culture), MakeType<float>(c.weight, culture));
            }

            public string ToString(IFormatProvider provider, Pupil item)
            {
                throw new NotImplementedException();
            }
        }
        public DocumentClass() : base(new TeacherClassFactory(), new PupilClassFactory()) { }

        public override bool Equals(object other)
        {
            if (other is DocumentClass otherClass)
            {
                return this.teachers.SequenceEqual(otherClass.teachers) && this.pupils.SequenceEqual(otherClass.pupils);
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

