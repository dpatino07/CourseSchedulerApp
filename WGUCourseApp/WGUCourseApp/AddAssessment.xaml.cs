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
    public partial class AddAssessment : ContentPage
    {
        public Course _course;
        public MainPage _main;
        public AddAssessment(Course course, MainPage main)
        {
            _course = course;
            _main = main;
            InitializeComponent();
        }

        private void cancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void saveBtn_Clicked(object sender, EventArgs e)
        {
            if(validateUserInput())
            {
                Assessment assessment = new Assessment();
                assessment.AssessName = assessName.Text;
                assessment.AssessType = assessPicker.SelectedItem.ToString();
                assessment.DueDate = dueDate.Date;
                assessment.Notify = pickerNotifcations.SelectedIndex;
                assessment.Course = _course.Id;

                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    var objCount = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Objective'");
                    var perCount = con.Query<Assessment>($"SELECT * FROM ASsessments WHERE Course = '{_course.Id}' AND AssessType = 'Performance'");

                    if(assessment.AssessType.ToString() == "Objective" && objCount.Count == 0)
                    {
                        con.Insert(assessment);
                        _main.assessments.Add(assessment);
                        await Navigation.PopModalAsync();
                    }
                    else if(assessment.AssessType.ToString() == "Performance" && perCount.Count == 0)
                    {
                        con.Insert(assessment);
                        _main.assessments.Add(assessment);
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new AssessmentError());
                    }
                }
            }
            else
            {
                await Navigation.PushModalAsync(new InputError());
            }
        }

        private bool validateUserInput()
        {
            bool valid = true;

            if(assessName == null || dueDate.Date == null || pickerNotifcations.SelectedItem == null)
            {
                return false;
            }
            return valid;
        }
    }
}