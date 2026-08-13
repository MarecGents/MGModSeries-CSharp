using MGEditor.ControlsLookup;
using MGEditor.Resources;
using MGEditor.ViewModels.Pages.Functions;

namespace MGEditor.Views.Pages.Functions;

[GalleryPage(
    Translations.GalleryDescContainer, 
    SymbolRegular.BoxArrowUp24
    )]
public partial class ContainerExpandPage : INavigableView<ContainerExpandViewModel>
{
    
    public ContainerExpandViewModel ViewModel { get; set; }
    public ContainerExpandPage(ContainerExpandViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}

