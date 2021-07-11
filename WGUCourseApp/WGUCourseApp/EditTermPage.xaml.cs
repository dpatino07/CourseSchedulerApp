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
    public partial class EditTermPage : ContentPage
    {
        public Term _term;
        public MainPage _main;
        public EditTermPage(Term term, MainPage main)
        {
            _term = term;
            _main = main;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            termTitle.Text = _term.TermName;
            startDate.Date = _term.Start.Date;
            endDate.Date = _term.End.Date;
        }

        private async void saveBtn_Clicked(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {

                _term.TermName = termTitle.Text;
                _term.Start = startDate.Date;
                _term.End = endDate.Date;
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.Update(_term);
                    await Navigation.PopToRootAsync();
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

            if (termTitle.Text == null ||
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
            Navigation.PopAsync();
        }
    }
}