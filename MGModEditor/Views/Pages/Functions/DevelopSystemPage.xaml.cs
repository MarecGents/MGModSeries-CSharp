using MGEditor.ControlsLookup;
using MGEditor.Resources;
using MGEditor.ViewModels.Pages.Functions;

namespace MGEditor.Views.Pages.Functions;

[GalleryPage(
    Translations.GalleryDescDevelop, 
    SymbolRegular.Guardian24
    )]
public partial class DevelopSystemPage : INavigableView<DevelopSystemViewModel>
{

    public DevelopSystemViewModel ViewModel { get; set; }
    public DevelopSystemPage(DevelopSystemViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}

