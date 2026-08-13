using MGEditor.ControlsLookup;
using MGEditor.Resources;
using MGEditor.ViewModels.Pages.Functions;

namespace MGEditor.Views.Pages.Functions;

[GalleryPage(
    Translations.GalleryDescEconomy, 
    SymbolRegular.CurrencyDollarEuro16
    )]
public partial class EconomySystemPage : INavigableView<EconomySystemViewModel>
{

    public EconomySystemViewModel ViewModel { get; set; }
    public EconomySystemPage(EconomySystemViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}

