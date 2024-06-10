using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MongoDB.Driver;
using MongoDB.Bson;
using MimeKit;
using MailKit.Net.Smtp;
using MvvmHelpers;

namespace PythoPlus.PopScreens
{
    public class RestoreViewModel : BaseViewModel
    {
        private string _email;
        private bool _isRestoreEnabled;
        private readonly MongoDbService _mongoDbService;

        public RestoreViewModel()
        {
            _mongoDbService = new MongoDbService();
            RestoreCommand = new Command(async () => await RestorePasswordAsync(), () => IsRestoreEnabled);
            BackCommand = new Command(async () => await BackToLoginAsync());
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Email))
                {
                    IsRestoreEnabled = !string.IsNullOrWhiteSpace(Email);
                    ((Command)RestoreCommand).ChangeCanExecute();
                }
            };
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public bool IsRestoreEnabled
        {
            get => _isRestoreEnabled;
            set => SetProperty(ref _isRestoreEnabled, value);
        }

        public ICommand RestoreCommand { get; }
        public ICommand BackCommand { get; }

        private async Task RestorePasswordAsync()
        {
            try
            {
                var accountsCollection = _mongoDbService.GetCollection("accounts");
                var filter = Builders<BsonDocument>.Filter.Eq("email", Email);
                var account = await accountsCollection.Find(filter).FirstOrDefaultAsync();

                if (account != null)
                {
                    var newPassword = GenerateRandomPassword(8);
                    var update = Builders<BsonDocument>.Update.Set("pass", newPassword);
                    await accountsCollection.UpdateOneAsync(filter, update);

                    await SendEmailAsync(Email, newPassword);

                    await Application.Current.MainPage.DisplayAlert("Успіх", "Новий пароль відправлено на вашу електронну скриню.", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Помилка", "Акаунт з такою електронною адресою не знайдено.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", $"Виникла помилка при відновленні паролю: {ex.Message}", "OK");
            }
        }

        private async Task BackToLoginAsync()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var newPassword = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                newPassword.Append(validChars[random.Next(validChars.Length)]);
            }

            return newPassword.ToString();
        }

        private async Task SendEmailAsync(string email, string newPassword)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", "restore.no-reply@pythoplus.online"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Відновлення паролю";
            message.Body = new TextPart("plain")
            {
                Text = $"Ваш новий пароль: {newPassword}"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.hostinger.com", 465, true); // Укажите ваш SMTP сервер
                await client.AuthenticateAsync("restore.no-reply@pythoplus.online", "PythoPlus_noreply1"); // Укажите ваши учетные данные
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
