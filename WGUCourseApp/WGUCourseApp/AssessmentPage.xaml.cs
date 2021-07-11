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
    public partial class AssessmentPage : ContentPage
    {
        public Course _course;
        public MainPage _main;
        public AssessmentPage(Course course, MainPage main)
        {
            _course = course;
            _main = main;
            InitializeComponent();
            AssessListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            Title = course.CourseName;
        }

        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Assessment assessment = (Assessment)e.Item;
            await Navigation.PushAsync(new EditAssessmentPage(_course, _main, assessment));
        }

        protected override void OnAppearing()
        {
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.CreateTable<Assessment>();
                var assessmentsForCourse = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}'");
                AssessListView.ItemsSource = assessmentsForCourse;
            }
        }

        async void addBtn_Clicked(object sender, EventArgs e)
        {
            if (getAssessmentCount() < 2)
            {
                await Navigation.PushModalAsync(new AddAssessment(_course, _main));
            }

            else
            {          
                await Navigation.PushModalAsync(new AssessmentError());
            }
        }

        int getAssessmentCount()
        {
            int count = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                var assessmentCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}'");
                count = assessmentCount.Count;
            }

            return count;
        }
    }
}