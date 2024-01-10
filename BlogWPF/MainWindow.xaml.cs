using BlogWPF.Model;
using BlogWPF.ViewModel;
using System.Windows;

namespace BlogWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowViewModel)LayoutGrid.DataContext;
            _viewModel.Logout += OnLogout;
            _viewModel.Ricerca += OnRicerca;
        }

        private void OnRicerca(object sender, System.EventArgs e)
        {
            var ricercaPage = new RicercaPage(_viewModel);
            ricercaPage.Show();
        }

        private void OnLogout(object sender, EventArgs e)
        {
            var loginPage = new LoginPage();
            loginPage.Show();

            Close();
        }

        internal void SetUsername(string userName)
        {
            _viewModel.Username = userName;
        }

        internal void SetPassword(string password)
        {
            _viewModel.Password = password;
        }

        internal void SetBlogSelectedItem(BlogViewModel blogSelectedItem)
        {
            _viewModel.BlogsSelectedItem = blogSelectedItem;
        }

        internal void SetPostSelectedItem(PostViewModel postSelectedItem)
        {
            _viewModel.PostsSelectedItem = postSelectedItem;
        }
    }

}