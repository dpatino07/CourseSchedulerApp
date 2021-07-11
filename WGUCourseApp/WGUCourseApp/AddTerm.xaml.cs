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
    public partial class AddTerm : ContentPage
    {
        public MainPage mainPage;
        public AddTerm(MainPage main)
        {
            mainPage = main;
            InitializeComponent();
        }

        private async void saveBtn_Clicked(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {


                Term term = new Term();
                term.TermName = termName.Text;
                term.Start = startDate.Date;
                term.End = endDate.Date;
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.Insert(term);

                    mainPage.terms.Add(term);
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                await Navigation.PushModalAsync(new InputError());
            }
        }

        private bool ValidateUserInput()
        {
            bool valid = true;

            if (termName.Text == null ||
                startDate.Date == null ||
                endDate.Date == null ||
                endDate.Date < startDate.Date
                )
            {
                return false;
            }
            return valid;
        }

        private void exitBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}