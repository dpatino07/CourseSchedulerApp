using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WGUCourseApp.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUCourseApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourse : ContentPage
    {
        public Term termPage;
        public MainPage mainPage;
        Dictionary<string, bool> notificationsDict = new Dictionary<string, bool>
        {
            {"Yes",true},
            {"No",false}
        };
        public AddCourse(Term term, MainPage main)
        {
            termPage = term;
            mainPage = main;
            InitializeComponent();
        }

        private async void saveBtn_Clicked(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {

                Course course = new Course();
                course.CourseName = courseTitle.Text;
                course.CourseStatus = courseStatus.SelectedItem.ToString();
                course.Start = startDate.Date;
                course.End = endDate.Date;
                course.InstructorName = instName.Text;
                course.InstructorEmail = instEmail.Text;
                course.InstructorPhone = instPhone.Text;
                course.Notes = notes.Text;
                course.Notify = pickerNotifications.SelectedIndex;
                course.Term = termPage.Id;

                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.Insert(course);
                    mainPage.courses.Add(course);
                    await Navigation.PopModalAsync();
                }
        }
            else
            {
                await Navigation.PushModalAsync(new InputError());
            }
        }

        private void exitBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private bool ValidateUserInput()
        {
            bool valid = true;

            if (courseTitle == null ||
                courseStatus.SelectedItem == null ||
                startDate.Date == null ||
                endDate.Date == null ||
                endDate.Date < startDate.Date ||
                instName == null ||
                instEmail.Text == null ||
                instPhone == null ||
                pickerNotifications.SelectedItem == null
                )

            {
                return false;
            }

            if (instEmail.Text != null)
            {
                valid = ValidateEmail(instEmail.Text);
            }


            return valid;
        }

        private bool ValidateEmail(string email)
        {
            try
            {
                MailAddress mail = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}