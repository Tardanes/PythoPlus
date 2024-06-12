using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace PythoPlus.PopScreens
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private LocalDbService _localDbService;

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
                ValidateRegistration();
                OnPropertyChanged();
            }
        }

        public string ConfirmPassword
        {
            get => confirmPassword;
            set
            {
                confirmPassword = value;
                ValidateRegistration();
                OnPropertyChanged();
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
        public ICommand BackCommand { get; }

        public RegistrationViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegister());
            BackCommand = new Command(async () => await OnBackButtonClicked());
            _localDbService = new LocalDbService(FileSystem.AppDataDirectory);
        }

        private async Task OnRegister()
        {
            try
            {
                var accounts = await _localDbService.GetCollectionAsync<Account>("accounts");

                var existingAccount = accounts.FirstOrDefault(acc => acc.Email == Email);

                if (existingAccount != null)
                {
                    // Учетная запись с таким email уже существует
                    await Application.Current.MainPage.DisplayAlert("Помилка", "Користувач з такою поштою вже існує.", "OK");
                    return;
                }

                // Создание нового аккаунта
                var newAccount = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = Email,
                    Password = Password
                };

                // Добавление нового аккаунта в коллекцию
                await _localDbService.AddRecordAsync("accounts", newAccount);

                // Уведомление пользователя об успешной регистрации
                await Application.Current.MainPage.DisplayAlert("Вдала реєстрація", "Тепер використайте ваші дані для входу!", "OK");

                // Перенаправление на страницу входа или другую страницу
                // Например, можно использовать Shell для навигации
                await Shell.Current.GoToAsync("Login");
            }
            catch (Exception ex)
            {
                // Логирование ошибки и показ сообщения пользователю
                Console.WriteLine($"Ошибка при выполнении регистрации: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Помилка під'єднання", "Стався збій програми. Перевірте з'єднання та спробуйте пізніше", "OK");
            }
        }

        private void ValidateRegistration()
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            bool isEmailValid = !string.IsNullOrEmpty(Email) && Regex.IsMatch(Email, emailPattern);
            bool isPasswordValid = !string.IsNullOrEmpty(Password) && Password.Length >= 8;

            IsRegisterEnabled = isEmailValid && isPasswordValid && Password == ConfirmPassword;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task OnBackButtonClicked()
        {
            // Навигация на предыдущую страницу
            await Shell.Current.Navigation.PopAsync();
        }
    }
}
