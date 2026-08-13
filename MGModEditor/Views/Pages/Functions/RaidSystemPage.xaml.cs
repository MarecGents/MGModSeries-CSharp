using MGEditor.ControlsLookup;
using MGEditor.Resources;
using MGEditor.ViewModels.Pages.Functions;

namespace MGEditor.Views.Pages.Functions;

[GalleryPage(
    Translations.GalleryDescRaid, 
    SymbolRegular.PersonWalking16
    )]
public partial class RaidSystemPage : INavigableView<RaidSystemViewModel>
{
    public RaidSystemViewModel ViewModel {  get; set; }
    public RaidSystemPage(RaidSystemViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}

