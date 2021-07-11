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
    public partial class EditAssessmentPage : ContentPage
    {
        public Course _course;

        public Assessment _assessment;

        public MainPage _main;

        public List<string> assessPicker = new List<string> { "Objective", "Performance" };

        public List<string> notifyPicker = new List<string> { "Yes", "No" };
        public EditAssessmentPage(Course course, MainPage main, Assessment assessment)
        {
            _course = course;
            _assessment = assessment;
            _main = main;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            assessType.ItemsSource = assessPicker;
            assessType.SelectedIndex = assessPicker.FindIndex(status => status == _assessment.AssessType);
            assessName.Text = _assessment.AssessName; 
            dueDate.Date = _assessment.DueDate.Date;

            if (_assessment.Notify == 0)
            {
                pickerNotifications.SelectedIndex = 0;
            }
            else
            {
                pickerNotifications.SelectedIndex = 1;
            }
            base.OnAppearing();
        }

        private async void saveBtn_Clicked(object sender, EventArgs e)
        {
            bool changedAssessmentType = false;
            if (_assessment.AssessType.ToString() != assessType.SelectedItem.ToString())
            {
                changedAssessmentType = true;
            }
            _assessment.AssessName = assessName.Text;
            _assessment.DueDate= dueDate.Date;
            _assessment.Notify = pickerNotifications.SelectedIndex;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                var objectiveCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Objective'");
                var performanceCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Performance'");
                if (_assessment.AssessType.ToString() == "Objective" && objectiveCount.Count == 0)
                {
                    _assessment.AssessType = assessType.SelectedItem.ToString();
                    con.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else if (_assessment.AssessType.ToString() == "Performance" && performanceCount.Count == 0)
                {
                    _assessment.AssessType = assessType.SelectedItem.ToString();
                    con.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else if (((_assessment.AssessType.ToString() == "Performance" && performanceCount.Count == 1) ||
                         (_assessment.AssessType.ToString() == "Objective" && objectiveCount.Count == 1)) &&
                         !(String.IsNullOrEmpty(_assessment.Id.ToString())) &&
                          !changedAssessmentType)
                {
                    con.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else
                {
                    await Navigation.PushModalAsync(new AssessmentError());
                }
            }
        }

        private async void deleteBtn_Clicked(object sender, EventArgs e)
        {
            var result = await this.DisplayAlert("Alert!", "Do you really want to delete this assessment?", "Yes", "No");
            if (result)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.Delete(_assessment);
                    await Navigation.PopAsync();
                }

            }
        }

        private void cancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}