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
    public class LoginViewModel : INotifyPropertyChanged
    {
        private LocalDbService _localDbService;

        private string email;
        private string password;
        private bool isLoginEnabled;
        private string userId;

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

        public string UserId
        {
            get => userId;
            private set
            {
                userId = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await OnLogin());
            _localDbService = new LocalDbService(FileSystem.AppDataDirectory);
        }

        private async Task OnLogin()
        {
            // принудительное решение от падений приложения
            try
            {
#if ANDROID
                Console.WriteLine("Login starting...");
#endif
                // Проверка, что подключение установлено
                var accounts = await _localDbService.GetCollectionAsync<Account>("accounts");

                var account = accounts.FirstOrDefault(acc => acc.Email == Email && acc.Password == Password);

                if (account != null)
                {
                    UserId = account.Id;
                    Application.Current.Resources["UserId"] = UserId; // Сохранение ID пользователя в динамических ресурсах
                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                    Shell.SetNavBarIsVisible(Shell.Current.CurrentItem, true);
                    LoginSuccessful?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Помилка входу", "Невірний email чи пароль.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки и показ сообщения пользователю
                Console.WriteLine($"Ошибка при выполнении входа: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Помилка", "Виникла помилка при з'єднанні. Перевірте ваш Інтернет та спробуйте пізніше.", "OK");
            }
        }

        public async Task UpdateUserStats()
        {
            var stats = await _localDbService.GetCollectionAsync<UserStats>("mat_stat");
            var userStats = stats.FirstOrDefault(stat => stat.UserId == UserId);

            if (userStats == null)
            {
                userStats = new UserStats
                {
                    UserId = UserId,
                    LoginsCount = 1,
                    TotalTimeSpent = 0,
                    TotalAnswers = 0,
                    CorrectAnswers = 0
                };
                await _localDbService.AddRecordAsync("mat_stat", userStats);
            }
            else
            {
                userStats.LoginsCount++;
                await _localDbService.UpdateRecordAsync("mat_stat", stat => stat.UserId == UserId, userStats);
            }
        }

        private void ValidateLogin()
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            bool isEmailValid = !string.IsNullOrEmpty(Email) && Regex.IsMatch(Email, emailPattern);
            bool isPasswordValid = !string.IsNullOrEmpty(Password) && Password.Length >= 8;

            IsLoginEnabled = isEmailValid && isPasswordValid;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
