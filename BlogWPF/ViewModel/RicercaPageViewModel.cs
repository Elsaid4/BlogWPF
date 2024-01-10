using BlogWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlogWPF.ViewModel
{
    class RicercaPageViewModel : INotifyPropertyChanged
    {
        #region Membri

        private RicercaPage _view;
        private readonly DbWorker _dbWorker;
        private string _username;
        private string _ricercaBlog;
        private string _ricercaPost;


        private BlogViewModel _blogsSelectedItem;
        private PostViewModel _postsSelectedItem;
        private BlogViewModel _blogOfPostSelected;

        #endregion

        #region Proprietà

        public event PropertyChangedEventHandler PropertyChanged;

        public EventHandler Back;
        private MainWindowViewModel _mainWindowViewModel;

        public RelayCommand RicercaBlogCommand { get; private set; }
        public RelayCommand RicercaPostCommand { get; private set; }
        public RelayCommand InvioBlogCommand { get; private set; }
        public RelayCommand InvioPostCommand { get; private set; }
        public RelayCommand BackCommand { get; private set; }

        public MainWindowViewModel MainWindowViewModel 
        { 
            get => _mainWindowViewModel;
            set
            {
                _mainWindowViewModel = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Username => MainWindowViewModel?.Username;

        public string RicercaBlog
        {
            get => _ricercaBlog;
            set
            {
                _ricercaBlog = value;
                OnPropertyChanged(nameof(RicercaBlog));
            }
        }

        public string RicercaPost
        {
            get => _ricercaPost;
            set
            {
                _ricercaPost = value;
                OnPropertyChanged(nameof(RicercaPost));
            }
        }

        public ObservableCollection<BlogViewModel> Blogs { get; private set; }
        public BlogViewModel BlogsSelectedItem
        {
            get => _blogsSelectedItem;
            set
            {
                _blogsSelectedItem = value;
                OnPropertyChanged(nameof(BlogsSelectedItem));
                // BlogsSelectedItem?.CheckIsPopulated();
                MainWindowViewModel.BlogsSelectedItemId = value.BlogId;
            }
        }

        public ObservableCollection<PostViewModel> Posts { get; private set; }
        public PostViewModel PostsSelectedItem
        {
            get => _postsSelectedItem;
            set
            {
                _postsSelectedItem = value;
                OnPropertyChanged(nameof(PostsSelectedItem));
                if (PostsSelectedItem != null)
                {
                    var result = _dbWorker.GetBlogByPost(PostsSelectedItem.BlogId);
                    var blogVM = new BlogViewModel(result);
                    BlogOfPostSelected = blogVM;
                    MainWindowViewModel.BlogsSelectedItemId = blogVM.BlogId;
                    MainWindowViewModel.PostsSelectedItemId = PostsSelectedItem.PostId;
                }
            }
        }

        public BlogViewModel BlogOfPostSelected
        {
            get => _blogOfPostSelected;
            set
            {
                _blogOfPostSelected = value;
                OnPropertyChanged(nameof(BlogOfPostSelected));
            }
        }


        #endregion

        #region Costruttori

        public RicercaPageViewModel()
        {

            RicercaBlogCommand = new RelayCommand(RicercaBlogCommandExecute, RicercaBlogCommandCanExecute);
            RicercaPostCommand = new RelayCommand(RicercaPostCommandExecute, RicercaPostCommandCanExecute);
            InvioBlogCommand = new RelayCommand(InvioBlogCommandExecute, InvioBlogCommandCanExecute);
            InvioPostCommand = new RelayCommand(InvioPostCommandExecute, InvioPostCommandCanExecute);
            BackCommand = new RelayCommand(BackCommandExecute);

            _dbWorker = new DbWorker();
            Blogs = new ObservableCollection<BlogViewModel>();
            Posts = new ObservableCollection<PostViewModel>();
        }

        #endregion

        #region Metodi

        public void SetView(RicercaPage view)
        {
            _view = view;
        }

        private bool RicercaPostCommandCanExecute() => !string.IsNullOrEmpty(RicercaPost);
        private void RicercaPostCommandExecute()
        {
            Posts.Clear();
            var posts = _dbWorker.GetPostByTitle(RicercaPost, Username);
            foreach (var post in posts)
            {
                Posts.Add(new PostViewModel(post));
            }
        }

        private bool RicercaBlogCommandCanExecute() => !string.IsNullOrEmpty(RicercaBlog);
        private void RicercaBlogCommandExecute()
        {
            Blogs.Clear();
            var blogs = _dbWorker.GetBlogByName(RicercaBlog, Username);
            foreach (var blog in blogs)
            {
                Blogs.Add(new BlogViewModel(blog));
            }
        }

        private bool InvioBlogCommandCanExecute() => BlogsSelectedItem != null;
        private void InvioBlogCommandExecute()
        {
            Back?.Invoke(this, EventArgs.Empty);
        }

        private bool InvioPostCommandCanExecute() => PostsSelectedItem != null;
        private void InvioPostCommandExecute()
        {
            Back?.Invoke(this, EventArgs.Empty);
        }

        private void BackCommandExecute()
        {
            Back?.Invoke(this, EventArgs.Empty);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
