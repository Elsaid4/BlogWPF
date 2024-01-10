using BlogWPF.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BlogWPF.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        #region Membri

        private string _userName;
        private string _password;
        private string _displaName;
        private ObservableCollection<BlogViewModel> _blogs;

        #endregion

        #region Proprietà

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string DisplayName
        {
            get => _displaName;
            set
            {
                _displaName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public ObservableCollection<BlogViewModel> Blogs
        {
            get => _blogs;
            set
            {
                _blogs = value;
                OnPropertyChanged(nameof(Blogs));
            }
        }

        #endregion

        #region Costruttori


        public UserViewModel() { }

        public UserViewModel(User user) : this()
        {
            UserName        = user.Username;
            Password        = user.Password;
            DisplayName     = user.DisplayName;
            Blogs           = [];
        }

        #endregion

        #region Metodi


        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
