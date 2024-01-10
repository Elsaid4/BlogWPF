using BlogWPF.ViewModel;
using System.Windows;

namespace BlogWPF
{
    /// <summary>
    /// Logica di interazione per RicercaPage.xaml
    /// </summary>
    public partial class RicercaPage : Window
    {
        private RicercaPageViewModel _viewModel;

        public RicercaPage()
        {
            InitializeComponent();
            _viewModel = (RicercaPageViewModel)LayoutGrid.DataContext;
            _viewModel.Back += OnBack;
            _viewModel.SetView(this);
        }

        public RicercaPage(MainWindowViewModel mainWindowViewModel) : this()
        {
            _viewModel.MainWindowViewModel = mainWindowViewModel;
        }

        public void OnBack(object sender, EventArgs e)
        {
            Close();
        }

    }
}
