using System;

namespace ProjectCS7
{
    public class SchoolPerson
    {
        public SchoolPerson(string name, string sirname, DateTime birthdate, float height, float weight) =>
            (this.name, this.sirname, this.birthdate, this.height, this.weight) = (name, sirname, birthdate, height, weight);

        public string name { get; }
        public string sirname { get; }
        public DateTime birthdate { get; }
        public float height { get; }
        public float weight { get; }

        public void Deconstruct(out string name, out string sirname, out DateTime birthdate, out float height, out float weight) =>
                    (name, sirname, birthdate, height, weight) = (this.name, this.sirname, this.birthdate, this.height, this.weight);

    }

    public class Pupil : SchoolPerson
    {
        public Pupil(string name, string sirname, DateTime birthdate, float height, float weight) : base(name, sirname, birthdate, height, weight) { }

    }

    public class Teacher : SchoolPerson
    {
        public Teacher(string name, string sirname, DateTime birthdate, float height, float weight) : base(name, sirname, birthdate, height, weight) { }
    }

}

