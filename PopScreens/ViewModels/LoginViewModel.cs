using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PythoPlus.PopScreens
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private MongoDbService _mongoDbService;

        private string email;
        private string password;
        private bool isLoginEnabled;
        private ObjectId userId;

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

        public ObjectId UserId
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
        }

        private async Task OnLogin()
        {

            // принудительное решение от падений приложения
            try
            {
#if ANDROID
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                Shell.SetNavBarIsVisible(Shell.Current.CurrentItem, true);
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
                return;
#endif
                // Проверка, что подключение установлено
                _mongoDbService = new MongoDbService();
                var accountsCollection = _mongoDbService.GetCollection("accounts");
                if (accountsCollection == null)
                {
                    Console.WriteLine("Не удалось получить коллекцию 'accounts'");
                    await Application.Current.MainPage.DisplayAlert("Помилка", "База даних наразі недоступна. Спробуйте пізніше.", "OK");
                    return;
                }

                var filter = Builders<BsonDocument>.Filter.Eq("email", Email) & Builders<BsonDocument>.Filter.Eq("pass", Password);
                var account = await accountsCollection.Find(filter).FirstOrDefaultAsync();

                if (account != null)
                {
                    UserId = account["_id"].AsObjectId;
                    Application.Current.Resources["UserId"] = UserId.ToString(); // Сохранение ID пользователя в динамических ресурсах
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