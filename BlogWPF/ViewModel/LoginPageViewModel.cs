using BlogWPF.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;

namespace BlogWPF.ViewModel
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        #region Membri

        private string _userName;
        private string _password;
        private LoginPage _view;

        #endregion

        #region Proprietà

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public event EventHandler LoggedIn;
        public event EventHandler Register;
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand RegisterCommand { get; private set; }

        #endregion

        #region Costruttori

        public LoginPageViewModel()
        {
            LoginCommand = new RelayCommand(LoginCommandExecute, LoginCommandCanExecute);
            RegisterCommand = new RelayCommand(RegisterCommandExecute, RegisterCommandCanExecute);

            using (var context = new BloggingContext())
            {
                context.Database.Migrate();
            }
        }

        #endregion

        #region Metodi

        public void SetView(LoginPage view)
        {
            _view = view;
        }

        private bool LoginCommandCanExecute() => !string.IsNullOrWhiteSpace(UserName) && 
            !string.IsNullOrEmpty(_view.PasswordBox.Password);

        private void LoginCommandExecute()
        {
            Password = _view.PasswordBox.Password;

            var dbWorker = new DbWorker();
            if (dbWorker.Login(UserName, Password))
            {
                LoggedIn?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Login fallito");
            }
            

        }
        private bool RegisterCommandCanExecute() => true;
        private void RegisterCommandExecute()
        {
            Register?.Invoke(this, EventArgs.Empty);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
