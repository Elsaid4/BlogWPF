using BlogWPF.ViewModel;
using System.Windows;

namespace BlogWPF
{
    /// <summary>
    /// Logica di interazione per LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private LoginPageViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            _viewModel = (LoginPageViewModel)LayoutGrid.DataContext;
            _viewModel.LoggedIn += OnLoggedIn;
            _viewModel.Register += OnRegister;
            _viewModel.SetView(this);
        }

        private void OnRegister(object sender, EventArgs e)
        {
            var registerPage = new RegisterPage();
            registerPage.Show();

            Close();
        }

        private void OnLoggedIn(object sender, EventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.SetUsername(_viewModel.UserName);
            mainWindow.SetPassword(_viewModel.Password);

            Close();
        }
    }
}
