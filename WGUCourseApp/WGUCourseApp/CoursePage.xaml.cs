using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WGUCourseApp.Classes;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUCourseApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoursePage : ContentPage
    {
        public Course _course;
        public Term _term;
        public MainPage _main;
        public List<string> courseStatusPicker = new List<string> { "In Progress", "Completed", "Dropped", "Plan To Take" };
        public List<string> notifyStatusPicker = new List<string> { "Yes", "No" };
        public CoursePage(Term term, MainPage main, Course course)
        {
            _course = course;
            _term = term;
            _main = main;
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            courseStatus.ItemsSource = courseStatusPicker;
            courseStatus.SelectedIndex = courseStatusPicker.FindIndex(status => status == _course.CourseStatus);
            courseTitle.Text = _course.CourseName;
            courseStatus.SelectedItem = _course.CourseStatus;
            startDate.Date = _course.Start.Date;
            endDate.Date = _course.End.Date;
            instName.Text = _course.InstructorName;
            instEmail.Text = _course.InstructorEmail;
            instPhone.Text = _course.InstructorPhone;
            notes.Text = _course.Notes;
            if (_course.Notify == 0)
            {
                pickerNotifications.SelectedIndex = 0;
            }
            else
            {
                pickerNotifications.SelectedIndex = 1;
            }
            base.OnAppearing();
        }

        private void assessBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AssessmentPage(_course, _main));
        }

        private async void editBtn_Clicked(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {
                _course.CourseName = courseTitle.Text;
                _course.CourseStatus = courseStatus.SelectedItem.ToString();
                _course.Start = startDate.Date;
                _course.End = endDate.Date;
                _course.InstructorName = instName.Text;
                _course.InstructorEmail = instEmail.Text;
                _course.InstructorPhone = instPhone.Text;
                _course.Notes = notes.Text;
                _course.Notify = pickerNotifications.SelectedIndex;
                _course.Term = _term.Id;
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.Update(_course);

                    await Navigation.PopAsync();
                }
            }
            else
            {
                await Navigation.PushModalAsync(new InputError());
            }
        }

        private async void deleteBtn_Clicked(object sender, EventArgs e)
        {
            var result = await this.DisplayAlert("Alert!", "Are you sure you want to delete this course?", "Yes", "No");
            if (result)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    var assessments = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}'");
                    foreach (Assessment a in assessments)
                    {
                        con.Delete(a);
                    }

                    con.Delete(_course);

                    await Navigation.PopAsync();
                }
            }
        }

        private void cancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void shareNotes_Clicked(object sender, EventArgs e)
        {
            await ShareNotes();
        }

        private bool ValidateUserInput()
        {
            bool valid = true;

            if (String.IsNullOrEmpty(courseTitle.Text) ||
                courseStatus.SelectedItem == null ||
                startDate.Date == null ||
                endDate.Date == null ||
                endDate.Date < startDate.Date ||
                String.IsNullOrEmpty(instName.Text) ||
                String.IsNullOrEmpty(instEmail.Text) ||
                String.IsNullOrEmpty(instPhone.Text) ||
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

        public async Task ShareNotes()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = notes.Text,
                Title = "Share Notes"
            });
        }
    }
}