using UnoApp3.MVVM.ViewModels;

namespace UnoApp3;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        this.DataContext = new MainViewModel();
    }
}
