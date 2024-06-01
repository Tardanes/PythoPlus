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
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private string email;
        private string password;
        private string confirmPassword;
        private bool isRegisterEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
                ValidateRegistration();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
                ValidateRegistration();
            }
        }

        public string ConfirmPassword
        {
            get => confirmPassword;
            set
            {
                confirmPassword = value;
                OnPropertyChanged();
                ValidateRegistration();
            }
        }

        public bool IsRegisterEnabled
        {
            get => isRegisterEnabled;
            set
            {
                isRegisterEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand { get; }

        public RegistrationViewModel()
        {
            RegisterCommand = new Command(OnRegister);
        }

        private void OnRegister()
        {
            // Логика выполнения регистрации
        }

        private void ValidateRegistration()
        {
            IsRegisterEnabled = !string.IsNullOrEmpty(Email) &&
                                !string.IsNullOrEmpty(Password) &&
                                Password == ConfirmPassword;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
