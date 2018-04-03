using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class RegisterGroupViewModel : BaseViewModel
    {
        string groupAuthCode;
        public string GroupAuthCode
        {
            get { return groupAuthCode; }
            set { SetProperty(ref groupAuthCode, value); }
        }

        public RegisterGroupViewModel()
        {
            Title = "Register group";

            RegisterCompanyCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand RegisterCompanyCommand { get; }

        private void RegisterGroup()
        {

        }
    }

    
}