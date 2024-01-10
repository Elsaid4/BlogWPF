namespace BlogWPF.Model
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        public Post()
        {
            Title = string.Empty;
            Content = string.Empty;
            Blog = new Blog();
        }
        public Post(int postId, string title, string content, int blogId, Blog blog)
        {
            PostId      = postId;
            Title       = title;
            Content     = content;
            BlogId      = blogId;
            Blog        = blog;
        }

        public Post(string title, string content, int blogId, Blog blog) : this()
        {
            Title       = title;
            Content     = content;
            BlogId      = blogId;
            Blog        = blog;
        }   
    }
}