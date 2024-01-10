using System.Windows;
using System.Windows.Controls;
using BlogWPF.ViewModel;


namespace BlogWPF
{
    /// <summary>
    /// Logica di interazione per RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Window
    {
        private RegisterPageViewModel _viewModel;
        public RegisterPage()
        {
            InitializeComponent();
            _viewModel = (RegisterPageViewModel)LayoutGrid.DataContext;
            _viewModel.Register += OnRegister;
            _viewModel.Back += OnBack;
            _viewModel.SetView(this);
            
        }

        private void OnBack(object sender, EventArgs e)
        {
            var loginPage = new LoginPage();
            loginPage.Show();

            Close();
        }

        private void OnRegister(object sender, EventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.SetUsername(_viewModel.UserName);
            mainWindow.SetPassword(_viewModel.Password);

            Close();
        }
    }
}
