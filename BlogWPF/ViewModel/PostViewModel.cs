using BlogWPF.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace BlogWPF.ViewModel
{
    public class PostViewModel : INotifyPropertyChanged
    {
        #region  Membri

        private int _postId;
        private string _title;
        private string _content;
        private int _blogId;


        #endregion

        #region Proprietà

        public event PropertyChangedEventHandler PropertyChanged;

        public int PostId 
        { 
            get => _postId; 
            set 
            { 
                _postId = value; 
                OnPropertyChanged(nameof(PostId));
            } 
        }

        public string Title 
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(HasTitle));
            }
        }

        public string Content 
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
                OnPropertyChanged(nameof(HasContent));
            }
        }

        public int BlogId
        { 
            get => _blogId;
            set
            {
                _blogId = value;
                OnPropertyChanged(nameof(BlogId));
            }
        }

        public bool HasTitle => !string.IsNullOrWhiteSpace(Title);
        public bool HasContent => !string.IsNullOrWhiteSpace(Content);

        #endregion

        #region Costruttori

        public PostViewModel() { }

        public PostViewModel(Post post) : this()
        {
            if (post != null)
            {
                PostId      = post.PostId;
                Title       = post.Title;
                Content     = post.Content;
                BlogId      = post.BlogId;
            }
        }

        #endregion

        #region Metodi

        public Post ToPost()
        {
            var post = new Post()
            {
                PostId  = PostId,
                Title   = Title,
                Content = Content,
                BlogId  = BlogId
            };
            return post;
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        #endregion


    }
}
