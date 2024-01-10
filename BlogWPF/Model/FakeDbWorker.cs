namespace BlogWPF.Model
{
    public class FakeDbWorker : IDbWorker
    {
        public User User1 { get; set; }
        public List<Blog> blogs1 { get; set; }
        public List<Post> posts1 { get; set; }
        public User User2 { get; set; }
        public List<Blog> blogs2 { get; set; }
        public List<Post> posts2 { get; set; }



        public FakeDbWorker()
        {
            User1 = new User("Elsaid Rexha", "Elsaid", "aaa");
            blogs1 =
            [
                new Blog(1, "Blog1E", "http:/Blog1E.com", new List<Post>(), User1),
                new Blog(2, "Blog2E", "http:/Blog2E.com", new List<Post>(), User1),
            ];
            posts1 =
            [
                new Post(1, "Post1E", "Contenuto1E", 1, blogs1[0]),
                new Post(2, "Post2E", "Contenuto2E", 2, blogs1[1]),
            ];

            User2 = new User("Pinco Pallino", "Pippo", "Aaa");
            blogs2 =
            [
                new Blog(3, "Blog1P", "http:/Blog1P.com", new List<Post>(), User2),
                new Blog(4, "Blog2P", "http:/Blog2P.com", new List<Post>(), User2),
                new Blog(5, "Blog3P", "http:/Blog3P.com", new List<Post>(), User2),
            ];
            posts2 =
            [
                new Post(3, "Post1P", "Contenuto1P", 3, blogs2[0]),
                new Post(4, "Post2P", "Contenuto2P", 3, blogs2[0]),
                new Post(5, "Post2P", "Contenuto2P", 4, blogs2[1]),
            ];

            blogs1[0].Posts.Add(posts1[0]);
            blogs1[1].Posts.Add(posts1[1]);
            blogs2[0].Posts.Add(posts2[0]);
            blogs2[0].Posts.Add(posts2[1]);
            blogs2[1].Posts.Add(posts2[2]);
        }

        public bool Login(string userName, string pass) => User1.Username == userName || User2.Username == userName;


        public List<Blog> GetBlogs(string userName)
        {
            return userName == User1.Username ? blogs1 : blogs2;
        }

        public List<Post> GetPosts(int blogId)
        {

            var posts = blogs1.Where(b => b.BlogId == blogId).ToList();
            if (posts.Count() == 0)
            {
                posts = blogs2.Where(b => b.BlogId == blogId).ToList();
            }

            return posts.FirstOrDefault().Posts;

        }

        public string GetContent(int postId)
        {
            var posts = blogs1.SelectMany(b => b.Posts).Concat(blogs2.SelectMany(b => b.Posts));
            var post = posts.Where(p => p.PostId == postId).FirstOrDefault();
            return post.Content;
        }

        public (bool Added, Blog Blog) AddBlog(string blogName, string userName)
        {
            throw new NotImplementedException();
        }

        public string GetTitle(int postId)
        {
            throw new NotImplementedException();
        }
        public (bool Added, Post Post) AddPost(string title, string content, int blogId)
        {
            throw new NotImplementedException();
        }

        public string GetUserName(string userName)
        {
            throw new NotImplementedException();
        }
        public bool DeleteBlog(int blogId)
        {
            throw new NotImplementedException();
        }
        public bool DeletePost(int postId)
        {
            throw new NotImplementedException();
        }

        public string GetName(string userName)
        {
            throw new NotImplementedException();
        }

        public (bool Edited, string Name) EditName(string newName, string username)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string userName)
        {
            throw new NotImplementedException();
        }

        public (bool Added, User User) AddUser(string userName, string name, string pass)
        {
            throw new NotImplementedException();
        }

        public List<Blog> GetBlogByName(string blogName, string username)
        {
            throw new NotImplementedException();
        }

        List<Post> IDbWorker.GetPostByTitle(string postTitle, string username)
        {
            throw new NotImplementedException();
        }

        public Blog GetBlogByPost(int postId)
        {
            throw new NotImplementedException();
        }

        public bool EditPost(int postId, string newTitle, string newContent)
        {
            throw new NotImplementedException();
        }

        public bool EditBlog(int blogId, string newBlogName)
        {
            throw new NotImplementedException();
        }
    }
}
