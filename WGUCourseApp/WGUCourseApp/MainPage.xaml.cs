using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGUCourseApp.Classes;
using Xamarin.Forms;

namespace WGUCourseApp
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public List<Term> terms = new List<Term>();
        public List<Course> courses = new List<Course>();
        public List<Assessment> assessments = new List<Assessment>();
        public MainPage main;
        bool firstTime = true;
        public MainPage()
        {
            InitializeComponent();
            termsListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            main = this;
        }

        private async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            await Navigation.PushAsync(new TermPage(term, main));
        }

        protected override void OnAppearing()
        {
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.CreateTable<Term>();
                con.CreateTable<Course>();
                con.CreateTable<Assessment>();
                terms = con.Table<Term>().ToList();
            }
            if (terms.Any() && firstTime)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.DropTable<Assessment>();
                    con.DropTable<Course>();
                    con.DropTable<Term>();

                    con.CreateTable<Term>();
                    con.CreateTable<Course>();
                    con.CreateTable<Assessment>();

                    ExampleData(1);
                }
                firstTime = false;
                alertNotifications();
            }
            else if (firstTime)
            {

                ExampleData(1);

                firstTime = false;
                alertNotifications();
            }
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                terms = con.Table<Term>().ToList();
                termsListView.ItemsSource = terms;
            }

            base.OnAppearing();
        }

        private void alertNotifications()
        {
            foreach (Term t in terms)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    var courses = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{t.Id}'");
                    foreach (Course c in courses)
                    {
                        // Check for courses starting within 3 days
                        if ((c.Start - DateTime.Now).TotalDays < 3 && c.Notify == 1)
                        {
                            CrossLocalNotifications.Current.Show("Course Starting Soon", $"{c.CourseName} is starting on {c.Start.Date.ToString()}");
                        }
                        // Check for courses ending within 7 days
                        if ((c.End - DateTime.Now).TotalDays < 7 && c.Notify == 1)
                        {
                            CrossLocalNotifications.Current.Show("Course Ending Soon", $"{c.CourseName} is ending on {c.End.Date.ToString()}");
                        }

                        // Check for assessments that are coming up within 3 days
                        var assessments = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{c.Id}'");
                        foreach (Assessment a in assessments)
                        {
                            if ((a.DueDate - DateTime.Now).TotalDays < 3 && a.Notify == 1)
                            {
                                CrossLocalNotifications.Current.Show("Assessment Due Soon", $"{a.AssessName} is starting on {a.DueDate.Date.ToString()}");
                            }
                        }

                    }
                }
            }
        }

        // Sample data for testing 
        private void ExampleData(int termNumber)
        {
            // Term Info
            Term t = new Term();
            t.TermName = "Term " + termNumber.ToString();
            t.Start = new DateTime(2021, 03, 1);
            t.End = new DateTime(2021, 08, 31);
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(t);
            }
            // Course Info
            Course c = new Course();
            c.Term = t.Id;
            c.CourseName = "Mobile Application Development Using C# - C971";
            c.CourseStatus = "In Progress";
            c.Start = new DateTime(2021, 03, 1);
            c.End = new DateTime(2021, 06, 21);
            c.InstructorName = "Lauren Gilroy";
            c.InstructorEmail = "lauren.gilroy@wgu.edu";
            c.InstructorPhone = "385-428-8921";
            c.Notes = "Read content, finish all labs, then take assessments.";
            c.Notify = 1;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(c);
            }
            // Objective Assessment Info
            Assessment obj = new Assessment();
            obj.AssessName = "LDV1";
            obj.StartDate = new DateTime(2021, 02, 20);
            obj.DueDate = new DateTime(2021, 02, 20);
            obj.AssessType = "Objective";
            obj.Course = c.Id;
            obj.Notify = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(obj);
            }
            // Performance Assessment Info
            Assessment perf = new Assessment();
            perf.AssessName = "LAP1";
            perf.StartDate = new DateTime(2021, 03, 1);
            perf.DueDate = new DateTime(2021, 06, 21);
            perf.AssessType = "Performance";
            perf.Course = c.Id;
            perf.Notify = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(perf);
            }
        }

        private async void newTermBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddTerm(this));
        }
    }
}
