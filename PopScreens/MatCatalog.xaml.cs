namespace PythoPlus.PopScreens;

public partial class MatCatalog : ContentPage
{
	public MatCatalog()
	{
		InitializeComponent();
        BindingContext = new MatCatalogViewModel();
    }
}