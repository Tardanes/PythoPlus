using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PythoPlus.PopScreens
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string email;
        private string password;
        private bool isLoginEnabled;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler LoginSuccessful;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
                ValidateLogin();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
                ValidateLogin();
            }
        }

        public bool IsLoginEnabled
        {
            get => isLoginEnabled;
            set
            {
                isLoginEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLogin);
        }

        private void OnLogin()
        {
            


            // Логика выполнения входа
            bool loginSuccessful = !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password); // Пример проверки
            if (loginSuccessful)
            {
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                Shell.SetNavBarIsVisible(Shell.Current.CurrentItem, true);
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ValidateLogin()
        {
            IsLoginEnabled = !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
