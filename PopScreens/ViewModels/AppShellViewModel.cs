using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PythoPlus
{
    public class AppShellViewModel : INotifyPropertyChanged
    {
        private bool isMainPageVisible;

        public Command LogoutCommand { get; }

        public AppShellViewModel()
        {
            LogoutCommand = new Command(OnLogout);
            UpdateVisibility();
        }

        public bool IsMainPageVisible
        {
            get => isMainPageVisible;
            set
            {
                if (isMainPageVisible != value)
                {
                    isMainPageVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private async void OnLogout()
        {
            // Очистить состояние пользователя и перенаправить на страницу входа
            await Shell.Current.GoToAsync("Login");
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
            Shell.SetNavBarIsVisible(Shell.Current.CurrentItem, false);
        }

        private void UpdateVisibility()
        {
            try
            {
                IsMainPageVisible = Shell.Current.CurrentPage is not MainPage;
            }
            catch
            {
                IsMainPageVisible = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
