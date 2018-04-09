using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ProjectCS7
{
    public abstract class DocFactoryBase<TeacherType, PupilType> : IDocFactory
    {
        protected IElementFactory<TeacherType> TeacherFactory { get; }
        protected IElementFactory<PupilType> PupilFactory { get; }

        public DocFactoryBase(IElementFactory<TeacherType> teacherFactory, IElementFactory<PupilType> pupilFactory) =>
                                        (TeacherFactory, PupilFactory) = (teacherFactory, pupilFactory);

        public IList<TeacherType> teachers { get; } = new List<TeacherType>();
        public IList<PupilType> pupils { get; } = new List<PupilType>();


        public void FillMember(dynamic dynamicChild, IFormatProvider culture)
        {
            switch (dynamicChild)
            {
                case JArray array when dynamicChild.Parent.Name == "pupils":
                    {
                        foreach (var c in dynamicChild)
                        {
                            PupilType pupil = PupilFactory.MakeItem(c, culture);
                            this.pupils.Add(pupil);
                        }
                        break;
                    }
                case JArray array when dynamicChild.Parent.Name == "teachers":
                    {
                        foreach (var c in dynamicChild)
                        {
                            TeacherType teacher = TeacherFactory.MakeItem(c, culture);
                            this.teachers.Add(teacher);
                        }
                        break;
                    }
                case null:
                    {
                        throw new ArgumentNullException(nameof(dynamicChild));
                    }
            }
        }

        public string ToString(IFormatProvider provider)
        {
            FormattableString res =
$@"{{
    'teachers': [
                {string.Join(",", teachers.Select(t => TeacherFactory.ToString(provider, t)).ToArray())}
                ], 
    'pupils':  [
                {string.Join(",", pupils.Select(t => PupilFactory.ToString(provider, t)).ToArray())} 
                ]
}}";
            return res.ToString(provider);
        }
    }
}

