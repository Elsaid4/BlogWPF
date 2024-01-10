using BlogWPF.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace BlogWPF.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Membri

        private IDbWorker _dbWorker;
        private string _username;
        private string _name;
        private string _blogName;
        private string _password;
        private bool _editName = false;
        private bool _editUsername = false;
        private bool _editPost = false;
        private bool _editBlog = false;
        private BlogViewModel _blogsSelectedItem;
        private PostViewModel _postsSelectedItem;

        #endregion

        #region Proprietà

        public event PropertyChangedEventHandler PropertyChanged;

        public EventHandler Logout;
        public EventHandler Ricerca;
        private bool _blogInserting;
        private bool _postInserting;

        public RelayCommand InserisciSalvaBlog { get; private set; }
        public RelayCommand EliminaAnnullaBlog { get; private set; }

        public RelayCommand InserisciSalvaPost { get; private set; }
        public RelayCommand EliminaAnnullaPost { get; private set; }

        public RelayCommand LogoutCommand { get; private set; }
        public RelayCommand RicercaCommand { get; private set; }
        public RelayCommand EditNameCommand { get; private set; }
        public RelayCommand AnnullaEditNameCommand { get; private set; }
        public RelayCommand EditUsernameCommand { get; private set; }
        public RelayCommand AnnullaEditUsernameCommand { get; private set; }

        public RelayCommand EditPostCommand { get; private set; }
        public RelayCommand AnnullaEditPostCommand { get; private set; }
        public RelayCommand EditBlogCommand { get; private set; }
        public RelayCommand AnnullaEditBlogCommand { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Username
        {
            get => _username;
            set
            {
                var dbWorker = new DbWorker();
                if (NotEditUsername)
                {
                    _username = dbWorker.GetUserName(value);
                    Name = dbWorker.GetName(Username);
                }
                else
                {
                    _username = value;

                }
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Username));
                UpdateBlogs();
            }
        }
        public string BlogName
        {
            get => _blogName;
            set
            {
                _blogName = value;
                OnPropertyChanged(nameof(BlogName));
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

        /// <summary>
        /// 
        /// Creo una ObservableCollection di blog, usando il modello di vista, nel quale avrò tutti i blog.
        ///
        /// Ogni volta che cambia la selezione del blog dalla ListBox, aggiorno la lista di post.
        ///  Cancello i vecchi post e contenuti qualora ci fossero.
        ///  
        /// </summary>
        public ObservableCollection<BlogViewModel> Blogs { get; private set; }
        public BlogViewModel BlogsSelectedItem
        {
            get => _blogsSelectedItem;
            set
            {
                _blogsSelectedItem = value;
                PostInserting = false;
                OnPropertyChanged(nameof(BlogsSelectedItem));
                BlogsSelectedItem?.CheckIsPopulated();
            }
        }
        public int? BlogsSelectedItemId
        {
            get => BlogsSelectedItem.BlogId;
            set
            {
                var blog = Blogs.Where(b => b.BlogId == value).FirstOrDefault();
                BlogsSelectedItem = blog;
                OnPropertyChanged(nameof(BlogsSelectedItemId));
            }
        }

        public ObservableCollection<PostViewModel> Posts { get; private set; }
        public PostViewModel PostsSelectedItem
        {
            get => _postsSelectedItem;
            set
            {
                _postsSelectedItem = value;
                PostInserting = false;
                OnPropertyChanged(nameof(PostsSelectedItem));
                
            }
        }
        public int? PostsSelectedItemId
        {
            get => PostsSelectedItem.PostId;
            set
            {
                var post = BlogsSelectedItem.Posts.Where(p => p.PostId == value).FirstOrDefault();
                PostsSelectedItem = post;
                OnPropertyChanged(nameof(PostsSelectedItemId));
            }
        }


        public bool EditName
        {
            get => _editName;
            set
            {
                _editName = value;
                OnPropertyChanged(nameof(EditName));
                OnPropertyChanged(nameof(NotEditName));
            }
        }
        public bool NotEditName => !EditName;

        public bool EditUsername
        {
            get => _editUsername;
            set
            {
                _editUsername = value;
                OnPropertyChanged(nameof(EditUsername));
                OnPropertyChanged(nameof(NotEditUsername));
            }
        }
        public bool NotEditUsername => !EditUsername;

        public bool BlogInserting
        {
            get => _blogInserting;
            set
            {
                _blogInserting = value;
                OnPropertyChanged(nameof(BlogInserting));
                OnPropertyChanged(nameof(BlogNotInserting));
            }
        }
        public bool BlogNotInserting => !BlogInserting;

        public bool PostInserting
        {
            get => _postInserting;
            set
            {
                _postInserting = value;
                OnPropertyChanged(nameof(PostInserting));
                OnPropertyChanged(nameof(PostNotInserting));
            }
        }
        public bool PostNotInserting => !PostInserting;

        public bool EditPost
        {
            get => _editPost;
            set
            {
                _editPost = value;
                OnPropertyChanged(nameof(EditPost));
                OnPropertyChanged(nameof(NotEditPost));
            }
        }
        public bool NotEditPost => !EditPost;

        public bool EditBlog
        {
            get => _editBlog;
            set
            {
                _editBlog = value;
                OnPropertyChanged(nameof(EditBlog));
                OnPropertyChanged(nameof(NotEditBlog));
            }
        }
        public bool NotEditBlog => !EditBlog;
        
        public string OldNames { get; private set; }
        public string Oldtitle { get; private set; }
        public string OldContent { get; private set; }

        #endregion


        #region Costruttori

        public MainWindowViewModel()
        {
            InserisciSalvaBlog          = new RelayCommand(InserisciSalvaBlogExecute,   InserisciSalvaBlogCanExecute);
            EliminaAnnullaBlog          = new RelayCommand(EliminaAnnullaBlogExecute,   EliminaAnnullaBlogCanExecute);

            InserisciSalvaPost          = new RelayCommand(InserisciSalvaPostExecute,   InserisciSalvaPostCanExecute);
            EliminaAnnullaPost          = new RelayCommand(EliminaAnnullaPostExecute,   EliminaAnnullaPostCanExecute);

            LogoutCommand               = new RelayCommand(LogoutCommandExecute);
            RicercaCommand              = new RelayCommand(RicercaCommandExecute);

            EditNameCommand             = new RelayCommand(EditNameCommandExecute,              EditNameCommandCanExecute);
            AnnullaEditNameCommand      = new RelayCommand(AnnullaEditNameCommandExecute,       AnnullaEditNameCommandCanExecute);
            EditUsernameCommand         = new RelayCommand(EditUsernameCommandExecute,          EditUsernameCommandCanExecute);
            AnnullaEditUsernameCommand  = new RelayCommand(AnnullaEditUsernameCommandExecute,   AnnullaEditUsernameCommandCanExecute);

            EditPostCommand             = new RelayCommand(EditPostCommandExecute,          EditPostCommandCanExecute);
            AnnullaEditPostCommand      = new RelayCommand(AnnullaEditPostCommandExecute,   AnnullaEditPostCommandCanExecute);
            EditBlogCommand             = new RelayCommand(EditBlogCommandExecute,          EditBlogCommandCanExecute);
            AnnullaEditBlogCommand      = new RelayCommand(AnnullaEditBlogCommandExecute,   AnnullaEditBlogCommandCanExecute);

            _dbWorker                   = new DbWorker();

            Blogs                       = new ObservableCollection<BlogViewModel>();
            Posts                       = new ObservableCollection<PostViewModel>();
        }

        #endregion


        #region Metodi

        /// <summary>
        /// Se non sto inserendo un blog, inizia l'inserimento, altrimenti salva il blog inserito.
        /// </summary>
        private bool InserisciSalvaBlogCanExecute() => NotEditBlog;
        private void InserisciSalvaBlogExecute()
        {
            if (BlogNotInserting)
            {
                BlogInserting = true;
            }
            else
            {
                var result = _dbWorker.AddBlog(Username, BlogName);
                if (result.Added)
                {
                    var blogVM = new BlogViewModel(result.Blog);
                    Blogs.Add(blogVM);
                    BlogsSelectedItem = blogVM;
                    BlogInserting = false;
                }
                else
                {
                    MessageBox.Show("Blog not added");
                }
            }
        }

        /// <summary>
        /// Se sto inserendo un blog, annulla l'inserimento, altrimenti elimina il blog selezionato.
        /// </summary>
        private bool EliminaAnnullaBlogCanExecute() => BlogsSelectedItem != null && NotEditBlog || (BlogInserting && NotEditBlog);
        private void EliminaAnnullaBlogExecute()
        {
            if(BlogInserting)
            {
                BlogInserting = false;
            }
            else
            {
                if (_dbWorker.DeleteBlog(BlogsSelectedItem.BlogId))
                {
                    MessageBox.Show($"Blog {BlogsSelectedItem.Name} eliminato correttamente");
                    Blogs.Remove(BlogsSelectedItem);
                    Posts.Clear();
                    PostsSelectedItem = null;
                }
                else
                {
                    MessageBox.Show("Errore, blog non eliminato");
                }
            }
        }

        private bool InserisciSalvaPostCanExecute() => BlogsSelectedItem != null && NotEditPost;
        private void InserisciSalvaPostExecute()
        {
            if (PostNotInserting)
            {
                var post = new PostViewModel()
                {
                    BlogId = BlogsSelectedItem.BlogId,
                    Title = "Nuovo post",
                    Content = "Inserisci contenuto"
                };
                //BlogsSelectedItem.Posts.Add(post);
                PostsSelectedItem = post;
                PostInserting = true;
            }
            else
            {
                var result = _dbWorker.AddPost(PostsSelectedItem.Title, PostsSelectedItem.Content, BlogsSelectedItem.BlogId);
                if (result.Added)
                {
                    var postVM = new PostViewModel(result.Post);
                    BlogsSelectedItem.Posts.Add(postVM);
                    PostsSelectedItem = postVM;
                    PostInserting = false;
                }
                else
                {
                    MessageBox.Show("Post not added");
                }
            }
        }

        private bool EliminaAnnullaPostCanExecute() => PostsSelectedItem != null && NotEditPost;
        private void EliminaAnnullaPostExecute()
        {
            if (PostInserting)
            {
                BlogsSelectedItem.Posts.Remove(PostsSelectedItem);
                PostsSelectedItem = null;
                PostInserting = false;
            }
            else
            {
                if (_dbWorker.DeletePost(PostsSelectedItem.PostId))
                {
                    MessageBox.Show($"Post {PostsSelectedItem.Title} eliminato correttamente");
                    BlogsSelectedItem.Posts.Remove(PostsSelectedItem);
                    PostsSelectedItem = null;
                    PostInserting = false;
                }
                else
                {
                    MessageBox.Show("Errore, post non eliminato");
                }
            }
        }

        private void LogoutCommandExecute()
        {
            Logout?.Invoke(this, EventArgs.Empty);
        }

        private void RicercaCommandExecute()
        {
            Ricerca?.Invoke(this, EventArgs.Empty);
        }

        private bool EditNameCommandCanExecute() => true;
        private void EditNameCommandExecute()
        {
            if (NotEditName)
            {
                OldNames = Name;
                EditName = true;
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(Name))
                {
                    var result = _dbWorker.EditName(Name, Username);
                    if (result.Edited)
                    {
                        MessageBox.Show("Modifica avvenuta con successo");

                    }
                    else
                    {
                        MessageBox.Show("Modifica fallita");
                    }
                    Name = result.Name;
                    EditName = false;
                }
                else
                {
                    MessageBox.Show("Nome non valido");
                }
            }
        }

        private bool AnnullaEditNameCommandCanExecute() => true;
        private void AnnullaEditNameCommandExecute()
        {
            Name = OldNames;
            EditName = false;
        }

        private bool EditUsernameCommandCanExecute() => true;
        private void EditUsernameCommandExecute()
        {
            if (NotEditUsername)
            {
                EditUsername = true;
                OldNames = Username;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Username))
                {
                    
                    if (_dbWorker.DeleteUser(OldNames))
                    {
                        MessageBox.Show($"Username {OldNames} eliminato correttamente");
                        MessageBox.Show($"passwporsd {Password} ");
                        var result = _dbWorker.AddUser(Username, Name, Password);
                        if (result.Added)
                        {
                            MessageBox.Show("Modifica avvenuta con successo");
                            Username = result.User.Username;
                        }
                        else
                        {
                            MessageBox.Show("Modifica fallita");
                        }
                    }
                    else
                    {
                        Username = OldNames;
                    }
                    EditName = false;
                }
                else
                {
                    MessageBox.Show("Nome non valido");
                }
                EditUsername = false;
            }
        }

        private bool AnnullaEditUsernameCommandCanExecute() => true;
        private void AnnullaEditUsernameCommandExecute()
        {
            Username = OldNames;
            EditUsername = false;
        }

        private bool EditPostCommandCanExecute() => PostsSelectedItem != null || (EditPost && PostInserting);
        private void EditPostCommandExecute()
        {
            if (NotEditPost)
            {
                EditPost = true;
                PostInserting = true;
                Oldtitle = PostsSelectedItem.Title;
                OldContent = PostsSelectedItem.Content;
            }
            else
            {
                if (_dbWorker.EditPost(PostsSelectedItem.PostId, PostsSelectedItem.Title, PostsSelectedItem.Content))
                {
                    MessageBox.Show("Modifica avvenuta con successo");
                }
                else
                {
                    PostsSelectedItem.Title = Oldtitle;
                    PostsSelectedItem.Content = OldContent;
                    MessageBox.Show("Modifica fallita");
                }
                EditPost = false;
                PostInserting = false;
            }
        }

        private bool AnnullaEditPostCommandCanExecute() => true;
        private void AnnullaEditPostCommandExecute()
        {
            PostsSelectedItem.Title = Oldtitle;
            PostsSelectedItem.Content = OldContent;
            EditPost = false;
            PostInserting = false;
        }

        private bool EditBlogCommandCanExecute() => BlogsSelectedItem != null || (EditBlog && BlogInserting);
        private void EditBlogCommandExecute()
        {
            if (NotEditBlog)
            {
                EditBlog        = true;
                BlogInserting   = true;
                BlogName        = BlogsSelectedItem.Name;
                Oldtitle        = BlogsSelectedItem.Name;
            }
            else
            {
                if (_dbWorker.EditBlog(BlogsSelectedItem.BlogId, BlogName))
                {
                    UpdateBlogs();
                    MessageBox.Show("Modifica avvenuta con successo");
                }
                else
                {
                    MessageBox.Show("Modifica fallita");
                }
                EditBlog = false;
                BlogInserting = false;
            }
        }
        
        private bool AnnullaEditBlogCommandCanExecute() => true;
        private void AnnullaEditBlogCommandExecute()
        {
            BlogsSelectedItem.Name = Oldtitle;
            BlogName = string.Empty;
            EditBlog = false;
            BlogInserting = false;
        }

        private void UpdateBlogs()
        {
            Blogs.Clear();
            var blogs = _dbWorker.GetBlogs(Username);
            foreach (var blog in blogs)
            {
                Blogs.Add(new BlogViewModel(blog));
            }
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}