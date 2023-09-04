using Syncfusion.Blazor;

namespace Inventory_Management;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddSyncfusionBlazor();

        return builder.Build();
	}
}