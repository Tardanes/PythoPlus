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
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private MongoDbService _mongoDbService;

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

        public RegistrationViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegister());
        }

        private async Task OnRegister()
        {
            try
            {
                _mongoDbService = new MongoDbService();
#if ANDROID
                await Task.Delay(20000);
#endif
                var accountsCollection = _mongoDbService.GetCollection("accounts");

                var filter = Builders<BsonDocument>.Filter.Eq("email", Email);
                var existingAccount = await accountsCollection.Find(filter).FirstOrDefaultAsync();

                if (existingAccount != null)
                {
                    // Учетная запись с таким email уже существует
                    await Application.Current.MainPage.DisplayAlert("Помилка", "Користувач з такою поштою вже існує.", "OK");
                    return;
                }

                // Создание нового документа пользователя
                var newAccount = new BsonDocument
                {
                    { "email", Email },
                    { "pass", Password }
                };

                // Вставка нового документа в коллекцию
                await accountsCollection.InsertOneAsync(newAccount);

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
    }
}
