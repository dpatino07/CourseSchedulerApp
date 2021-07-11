using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUCourseApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentError : ContentPage
    {
        public AssessmentError()
        {
            InitializeComponent();
        }

        private void okBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}