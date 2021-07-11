using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGUCourseApp.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUCourseApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermPage : ContentPage
    {
        public Term _term;
        public MainPage _main;

        public TermPage(Term term, MainPage main)
        {
            _term = term;
            _main = main;
            InitializeComponent();
            coursesListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            Title = term.TermName;
        }

        private async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Course course = (Course)e.Item;
            await Navigation.PushAsync(new CoursePage(_term, _main, course));
        }

        protected override void OnAppearing()
        {
            termStart.Text = _term.Start.ToString("MM/dd/yyyy");
            termEnd.Text = _term.End.ToString("MM/dd/yyyy");
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.CreateTable<Course>();
                var coursesForTerm = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                coursesListView.ItemsSource = coursesForTerm;
            }
        }

        private async void addBtn_Clicked(object sender, EventArgs e)
        {
            if (getCourseCount() < 6)
            {
                await Navigation.PushModalAsync(new AddCourse(_term, _main));
            }
            else
            {
                await Navigation.PushModalAsync(new CourseMaximumError());
            }
        }

        int getCourseCount()
        {
            int count = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                var courseCount = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                count = courseCount.Count;
            }

            return count;
        }

        private async void editBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditTermPage(_term, _main));
        }

        private async void deleteBtn_Clicked(object sender, EventArgs e)
        {
            var result = await this.DisplayAlert("Alert!", "Delete Term?", "Yes", "No");
            if (result)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    var courses = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                    foreach (Course c in courses)
                    {
                        var assessments = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{c.Id}'");
                        foreach (Assessment a in assessments)
                        {
                            con.Delete(a);
                        }
                        con.Delete(c);
                    }
                    con.Delete(_term);
                    await Navigation.PopToRootAsync();
                }

            }
        }
    }
}