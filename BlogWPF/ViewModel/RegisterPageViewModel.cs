using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlogWPF.Model;

namespace BlogWPF.ViewModel
{
    class RegisterPageViewModel : INotifyPropertyChanged
    {
        #region Membri

        private string _userName;
        private string _name;
        private string _password;

        private RegisterPage _view;

        #endregion

        #region Proprietà

        public event EventHandler Register;
        public event EventHandler Back;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
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

        public RelayCommand RegisterCommand { get; private set; }
        public RelayCommand BackCommand  { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Costruttori

        public RegisterPageViewModel()
        {
            RegisterCommand     = new RelayCommand(RegisterCommandExecute, RegisterCommandCanExecute);
            BackCommand         = new RelayCommand(BackCommandExecute, BackCommandCanExecute);
        }

        #endregion


        #region Metodi


        public void SetView(RegisterPage view)
        {
            _view = view;
        }

        private bool RegisterCommandCanExecute() => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Name);
        private void RegisterCommandExecute()
        {
            Password = _view.PasswordBox.Password;
            if(!string.IsNullOrEmpty(Password))
            {
                var dbWorker = new DbWorker();
                var result = dbWorker.AddUser(UserName, Name, Password);
                if (result.Added)
                {
                    MessageBox.Show("Registrazione avvenuta con successo");
                    Register?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Registrazione fallita");
                }
            }
            else
            {
                MessageBox.Show("Password non valida");
            }
        }

        private bool BackCommandCanExecute() => true;
        private void BackCommandExecute()
        {
            Back?.Invoke(this, EventArgs.Empty);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
