using BlogWPF.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace BlogWPF.ViewModel
{
    public class BlogViewModel : INotifyPropertyChanged
    {
        #region Membri

        private int _blogId;
        private string _name;
        private string _url;
        private string _userName;
        private bool _isPopulated = false;
        private ObservableCollection<PostViewModel> _posts;

        #endregion

        #region Proprietà

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsPopulated
        {
            get => _isPopulated;
            set
            {
                _isPopulated = value;
                OnPropertyChanged(nameof(IsPopulated));
            }
        }
            
        public int BlogId {
            get => _blogId;
            set
            {
                _blogId = value;
                OnPropertyChanged(nameof(BlogId));
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

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public ObservableCollection<PostViewModel> Posts
        {
            get => _posts;
            set
            {
                _posts = value;
                OnPropertyChanged(nameof(Posts));
            }
        }

        #endregion

        #region Costruttori

        public BlogViewModel() { }

        public BlogViewModel(Blog blog) : this()
        {
            if (blog != null)
            {
                BlogId      = blog.BlogId;
                Name        = blog.Name;
                Url         = blog.Url;
                UserName    = blog.Username;
                Posts       = [];
            }
        }

        #endregion

        #region Metodi

        public Blog ToBlog()
        {
            var blog  = new Blog()
            { 
                BlogId      = _blogId,
                Name        = _name,
                Url         = _url,
                Username    = _userName,
            };
            return blog;
        }


        public void UpdatePosts()
        {
            if (Posts != null)
            {
                Posts.Clear();
            }

            var _dbWorker = new DbWorker();
            var posts = _dbWorker.GetPosts(BlogId);
            foreach (var post in posts)
            {
                Posts.Add(new PostViewModel(post));
            }
            IsPopulated = true;
        }

        public void CheckIsPopulated()
        {
            if (!IsPopulated) UpdatePosts();
            
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
