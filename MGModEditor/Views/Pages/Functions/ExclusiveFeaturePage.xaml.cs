using MGEditor.ControlsLookup;
using MGEditor.Resources;
using MGEditor.ViewModels.Pages.Functions;

namespace MGEditor.Views.Pages.Functions;

[GalleryPage(
    Translations.GalleryDescFeature, 
    SymbolRegular.Premium12
    )]
public partial class ExclusiveFeaturePage : INavigableView<ExclusiveFeatureViewModel>
{
    public ExclusiveFeatureViewModel ViewModel { get; set; }
    public ExclusiveFeaturePage(ExclusiveFeatureViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}

