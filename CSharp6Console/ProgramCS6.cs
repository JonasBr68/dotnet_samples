using System;
using System.Collections.Generic;
using System.Collections.Specialized;

// TODO CS6 3.0 using static injects all static methods into namespace
using static System.String;
using static CSharp6Console.MyStatics;
using static System.Linq.Enumerable; // TODO CS6 3.3 Observe that using System.Linq not specified, this imports it

// TODO CS6 3.1 using static to allow calling Assert(...)
using static System.Diagnostics.Debug;

using System.Runtime.CompilerServices;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Text;
using System.Collections;

namespace CSharp6Console
{
    public static class StudentExtensions
    {
        // TODO CS6 9.3 Extension method to enable Collection initializer of Enrollment
        public static void Add(this Enrollment e, Student s) => e.Enroll(s);

        public static void Add(this Enrollment e, string firstName, string lastName) => e.Enroll(new Student(firstName, lastName));
    }
    public class Enrollment : IEnumerable<Student>
    {
        private List<Student> allStudents = new List<Student>();

        public void Enroll(Student newStudent)
        {
            // TODO CS70 throw expressions new in 7.0 before only statements
            //newStudent = newStudent ?? throw new ArgumentNullException(nameof(newStudent)); 
            if (newStudent == null)
                throw new ArgumentNullException(nameof(newStudent)); // TODO CS6 7.0 nameof C# 6.0

            allStudents.Add(newStudent);
        }

        public Student this[int key]
        {
            get
            {
                return allStudents[key];
            }
            // TODO CS6 9.01 custom indexer to add items to allow using index initializer on Enrollment 
            set
            {
                if(allStudents.Count == key) //Only allows adding new for next 'free' position
                {
                    Enroll(value); 
                }
                else
                    allStudents[key] = value; //Will throw out of range if item does not exist
            }
        }

        public IEnumerator<Student> GetEnumerator()
        {
            return ((IEnumerable<Student>)allStudents).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Student>)allStudents).GetEnumerator();
        }
        
        public static void InitializeStudents()
        {
            // TODO CS6 9.2 Collection initializers - Extension Add method if type has different name 
            var classList = new Enrollment()
            {              
                new Student("Vicki", "Petty"),
                new Student("Ofelia", "Hobbs"),
                new Student("Leah", "Kinney"),
                new Student("Alton", "Stoker"),
                new Student("Luella", "Ferrell"),
                { "Lessie", "Crosby" }, // Different extension method
            };

            // TODO CS6 9.1 Index Initializers using custom indexer to allow using index initializer
            var classList2 = new Enrollment()
            {
                [0] = new Student("First", "Zero")
            };
        }
    }
    public static class ExceptionExtensions
    {
        public static string UnwrapMessage(this Exception exception)
        {
            if (exception is AggregateException)
                return (exception as AggregateException).UnwrapMessage();
            else
                return exception.ToString();
        }
        public static string UnwrapMessage(this AggregateException exception)
        {
            Exception current = exception.InnerException;
            while (current is AggregateException)
            {
                current = current.InnerException;
            }
            StringBuilder message = new StringBuilder(current.Message);
            while (current.InnerException != null)
            {
                message.AppendFormat(" ---> {0}\r\n\t", current.InnerException.Message);
                current = current.InnerException;
            }
            return message.ToString();
        }
    }

    public class Person
    {
        // TODO CS6 1.0 True immutable read-only properties
        public string LastName { get; }

        // TODO CS6 1.1 Read only properties can be initialized at declaraction or in constructor
        public string FirstName { get; } = "Unknown"; 

        public Person(string firstName, string lastName)
        {
            if (!IsNullOrWhiteSpace(firstName)) // TODO CS6 3.5 String.IsNullOrWhiteSpace injected with using static
                LastName = lastName; // TODO CS6 1.2 read only property only writeable in constructor
        }

        public void ChangeLastName(string newLastName)
        {
            // TODO CS6 1.5 Generates CS0200: Property or indexer cannot be assigned to -- it is read only
            // LastName = newLastName; //Immutable
        }

        
        public string NickName { get; private set; } // TODO CS6 1.4 Before C# 6 best option was private set
    }

    interface IStudent
    {
        ICollection<double> Grades { get; }
        int YearsInSchool { get; set; }

        string FullName { get; }

        string GetFormattedGradePoint();

        bool MakesDeansList();

        EventHandler Quitting { get; set; }

        void Leave();
    }

    public class Student : Person, IStudent
    {
        public Student(string firstName, string lastName) : base(firstName, lastName)
        {

        }

        public ICollection<double> Grades { get; } = new List<double>();


        // TODO CS6 1.3 auto properties initialized in declaration also with normal read/write automatic properties
        public int YearsInSchool { get; set; } = 0;

        // TODO CS6 2.0 Method with expression body
        public override string ToString() => $"{LastName}, {FirstName}"; // TODO CS6 5.0 interpolated strings

        // TODO CS6 2.1 read-only property with expression body 
        public string FullName => $"{LastName}, {FirstName}"; // TODO CS6 5.1 interpolated strings


        // TODO CS6 5.3 interpolated string with format specifier and method call
        public string GetFormattedGradePoint() => $"Name: {LastName}, {FirstName}. G.P.A: {Grades.Average():F2}";

        public bool MakesDeansList()
        {
            return Grades.All(g => g > 3.5) && Grades.Any();
            // TODO CS6 3.2 using static extension methods only visible when called with extension syntax
            // Code below generates CS0103: 
            // The name 'All' does not exist in the current context.
            //return All(Grades, g => g > 3.5) && Grades.Any();
        }

        public EventHandler Quitting { get; set; }
        public void Leave()
        {
            // TODO CS6 4.2 Null-conditional operator only call method/event handler when not null
            Quitting?.Invoke(this, null);
        }
    }

    public class StudentCS5 : Person, IStudent
    {
        public StudentCS5(string firstName, string lastName) : base(firstName, lastName)
        {
            Grades = new List<double>();
            YearsInSchool = 0;
        }
        public int YearsInSchool { get; set; }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public ICollection<double> Grades { get; private set; }
        public string GetFormattedGradePoint()
        {
            return string.Format("Name: {0}, {1}. G.P.A: {2:F1}", FirstName, LastName, Grades.Average());
        }

        public bool MakesDeansList()
        {
            return Grades.All(g => g > 3.5) && Grades.Any();
            // Code below generates CS0103: 
            // The name 'All' does not exist in the current context.
            //return All(Grades, g => g > 3.5) && Grades.Any();
        }
        public EventHandler Quitting { get; set; }
        public void Leave()
        {
            var handler = this.Quitting;
            if (handler != null)
                handler(this, null);
        }

    }


    class Program
    {
        static Action<string> Log = Console.WriteLine;

        //C# null coalescing operator
        static void NullCoalescing()
        {
            //Note, Normal Dictionary throws KeyNotFoundException on missing key 

            var Parameters = new StringDictionary { };
            var Settings = new StringDictionary { };
            var GlobalSetting = new StringDictionary { { "Name", "Choose me" } };

#pragma warning disable IDE0029 // Use coalesce expression
            string anybody2 = Parameters["Name"] != null ? Parameters["Name"]
                 : (Settings["Name"] != null) ? Settings["Name"]
                 : GlobalSetting["Name"];
#pragma warning restore IDE0029 // Use coalesce expression

            //Fallbacks
            string anybody = Parameters["Name"]
              ?? Settings["Name"]
              ?? GlobalSetting["Name"];

            Console.WriteLine(anybody);
        }


        ///null conditional operator 
        static void NullConditionalOperator()
        {
            IStudent nullStudent = null;

            //var name = nullStudent.FullName; //System.NullReferenceException

            // TODO CS6 4.0 Null-conditional operator with . (dot) operator
            var name = nullStudent?.FullName;

            string[] names = null;

            // TODO CS6 4.1 Null-conditional operator with [] (index) operator
            var firstName = names?[0]; //Only calls operator [] if names is not null

            // TODO CS6 3.4 Calling System.Diagnostics.Debug.Assert directly thanks to 'using static'
            Assert(name == null); 


            // TODO CS6 4.1 Null-conditional operator WITH null coalescing operator 
            var name2 = nullStudent?.FullName ?? "Unknown"; //If null, assign a default of your choosing, same type!

            IStudent student1 = new Student("First", "Last");

            student1.Quitting += (sender, args) =>
            {
                IStudent student = sender as IStudent;
                Console.WriteLine($"Student {student.FullName} is quitting");
            };

            student1.Leave();
            student1.Quitting = null;
            student1.Leave();


        }
        static void FormattableStrings()
        {
            FormattableString fStr2 = $"Now2 {DateTime.Now}"; // TODO CS6 5.4 Interpolated strings compiles to FormattableString if you want
            Console.WriteLine(fStr2.Format);
            Console.WriteLine(fStr2);

            // TODO CS6 5.5 Interpolated strings/FormattableString can be used and reused with different cultures
            Console.WriteLine(fStr2.ToString(CultureInfo.CreateSpecificCulture("en-us")));


            // TODO CS6 5.6 FormattableString can be used without interpolated strings and reused with different cultures as well
            FormattableString fStr = FormattableStringFactory.Create("Now1 {0:m}", DateTime.Now);
            Console.WriteLine(fStr);
            Console.WriteLine(fStr.ToString(CultureInfo.CreateSpecificCulture("en-us")));
            Console.WriteLine(fStr.ToString(CultureInfo.CreateSpecificCulture("es-es")));


            //Console.WriteLine(string.Format("Missing arg {0}")); // System.FormatException

        }
        static bool RethrowExceptions = false;

        // TODO CS6 6.3 Method used in Exception filter, when returning false handler not called and call stack is NOT unwound
        static bool LogExceptions(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return RethrowExceptions;
        }

        // TODO CS6 6.0 Exception Filters
        static void ExceptionFilters()
        {

            try
            {
                throw new HttpRequestException("Failed", new FormatException());
            }
            catch (HttpRequestException ex) when (ex.InnerException is FormatException) // TODO CS6 6.1 Exception filter on typeof InnerException
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                throw new HttpRequestException("Rethrow");
            }
            catch (Exception ex) when (Program.LogExceptions(ex)) // TODO CS6 6.2 Exception filter calling method
            {
                throw;
            }
            catch (Exception ex) when (!!System.Diagnostics.Debugger.IsAttached) // TODO CS6 6.4 Depending on debugger attached or not
            {
                Console.WriteLine("EXCEPTION NOT THROWN: " + ex.ToString());
            }
        }
        static async Task AsyncLog(string msg, Exception ex)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(300));
            Log(msg);
        }
        static void HttpClientExceptionFiltering()
        {

            var task2 = MakeAsyncRequest();
            Console.WriteLine(task2.Result);


            // TODO CS6 7.3 Call this instead to see use of nameof in exception
            //var task = AsyncHttpClientExceptionFiltering(""); 

            // TODO CS71 1.0 async Main new, instead of task.Wait() await task in Main
            var task = AsyncHttpClientExceptionFiltering("http://www.microsoft.fi"); 

            task.Wait();
            var res = task.Result;
            Console.WriteLine(res?.AbsoluteUri ?? "No redirect!");

        }
        static async Task<string> MakeAsyncRequest()
        {
            WebRequestHandler webRequestHandler = new WebRequestHandler();
            webRequestHandler.AllowAutoRedirect = false;
            using (HttpClient client = new HttpClient(webRequestHandler))
            {
                var streamTask = client.GetStringAsync("http://www.microsoft.se");
                try
                {
                    var responseText = await streamTask;
                    return responseText;
                }
                catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("301"))
                {
                    return "Site Moved";
                }
            }
        }
        static Task<Uri> AsyncHttpClientExceptionFiltering(string requestedUrl)
        {
            if (string.IsNullOrEmpty(requestedUrl))
            {
                // TODO CS6 7.2 nameof turns variable into its name as string, allows safe renaming/refactoring
                throw new ArgumentNullException(nameof(requestedUrl));
            }

            return MakeAsyncHttpRequest(requestedUrl);

        }

        private static async Task<Uri> MakeAsyncHttpRequest(string requestedUrl)
        {
            Uri redirected = null;
            WebRequestHandler webRequestHandler = new WebRequestHandler();
            webRequestHandler.AllowAutoRedirect = false;
            using (HttpClient client = new HttpClient(webRequestHandler))
            {
                HttpResponseMessage response = null;
                try
                {

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestedUrl);
                    response = await client.SendAsync(request);
                    //var stringResponse = await client.GetStringAsync(requestedUrl); //Silly to use as it looses the HttpResponseMessage with the status code

                    response.EnsureSuccessStatusCode();
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        if (response.StatusCode == HttpStatusCode.MovedPermanently ||
                            response.StatusCode == HttpStatusCode.Redirect ||
                            response.StatusCode == HttpStatusCode.TemporaryRedirect)
                        {
                            if (response.Headers.Location.IsAbsoluteUri)
                                redirected = response.Headers.Location;
                            else
                                redirected = new Uri(new Uri(requestedUrl, UriKind.Absolute), response.Headers.Location);
                            return redirected;
                        }
                        else
                        {
                            if ((int)response.StatusCode > 399)
                            {
                                throw new HttpException((int)response.StatusCode, string.Format("Resolving redirect for {0} returned {1}", requestedUrl, response.ReasonPhrase));
                            }
                        }
                    }
                    return redirected;

                }
                catch (AggregateException aggregate)
                {
                    throw new HttpException((int)404, aggregate.UnwrapMessage());
                }
                // TODO CS6 6.5 Exception filter on exception message contents
                catch (HttpRequestException ex) when (ex.Message.Contains(" 301 ") ||
                                                      ex.Message.Contains(" 302 ") ||
                                                      ex.Message.Contains(" 307 "))
                {
                    // TODO CS6 8.0 C# 6 now allows await in catch and finally blocks
                    await AsyncLog("Recovered from redirect", ex);
                    return response.Headers.Location;
                }
                catch (HttpRequestException)
                {
                    throw;//Rethrow
                }
                catch (Exception)
                {
                    throw;//Rethrow
                }

                throw new TimeoutException();
            }
        }

        static void IndexInitializers()
        {
            Dictionary<int, string> webErrorsOld = new Dictionary<int, string>
            {
                { 404, "Page not Found" },
                { 302, "Page moved, but left a forwarding address." },
                { 500, "The web server can't come out to play today." }
            };
            // TODO CS6 9.0 Index based Initializers - works on types where indexer can add new items
            Dictionary<int, string> webErrors = new Dictionary<int, string>
            {
                [404] = "Page not Found",
                [302] = "Page moved, but left a forwarding address.",
                [500] = "The web server can't come out to play today."
            };
            Assert(webErrors[404] == webErrorsOld[404]);

            string[] arr1 = { "Zero", "One", "Two" };
            Console.Write(arr1[0]);

            //string[] arr2 = new string[] { [0] = "Zero" };

            var list1 = new List<string>(arr1) { [0] = "0", [2] = "2" }; //TODO CS6 9.4 Does not initialize, calls indexer and (re-)assign values on existing items
            Console.WriteLine(list1[1]);

            //var list2 = new List<string> { [0] = "Zero" }; //ArgumentOutOfRangeException

            //webErrors = { [404] = "Status code 404, Page not Found }; //does not work but conceivably could
        }

        static void NameOf()
        {
            var culture = CultureInfo.CreateSpecificCulture("ar-sa");
            // TODO CS6 7.1 nameof turns variable into its name as string, allows safe renaming/refactoring
            var nameOfCalendar = nameof(culture.Calendar);
            Log($"{culture.Calendar.GetType().Name}.{nameOfCalendar}");
        }
        static void Main(string[] args)
        {
            var CSVersion = 6;

            //TODO CS6 5.01 string interpolation
            Console.WriteLine($"Hello C# {CSVersion}");

            try
            {
                //TODO CS6 5.02 string interpolation Helps avoid run-time errors for silly mistakes like this
                Console.WriteLine(string.Format("Hello C# {0}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            NullCoalescing();

            NullConditionalOperator();

            FormattableStrings();


            ExceptionFilters();

            HttpClientExceptionFiltering();

            NameOf();

            IndexInitializers();

            Enrollment.InitializeStudents();

            var person = new Person("Impostor", "Programmer");

            //error CS0272: The property or indexer 'Person.NickName' cannot be used in this context because the set accessor is inaccessible
            //person.NickName = "Nicky";

            //error CS0200: Property or indexer 'Person.FirstName' cannot be assigned to -- it is read only
            //person.FirstName = "Impostor";

            IStudent studentSC6 = new Student("CSharp", "Version6");
            studentSC6.Grades.Add(3.5d);
            studentSC6.Grades.Add(4.2);

            Console.WriteLine($"{studentSC6.FullName} {studentSC6.GetFormattedGradePoint()}");

            IStudent studentSC5 = new StudentCS5("CSharp", "Version5");

            var top = studentSC5.MakesDeansList();

            studentSC5.Grades.Add(3.51d);
            studentSC5.Grades.Add(4.2);

            top = studentSC5.MakesDeansList();

            Console.WriteLine($"{studentSC5.FullName} {studentSC5.GetFormattedGradePoint()}");

            FormattableString str = $"Average grade is {studentSC5.Grades.Average()}";
            Console.WriteLine(str);
            var gradeStr2 = str.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("de-de"));

            Console.WriteLine(gradeStr2);

            Console.WriteLine(EchoMe("Echo this"));
            Console.WriteLine(NestedStatics.EchoMeNested("Echo this"));

            var o = new NestedStatics();




        }
    }
}
