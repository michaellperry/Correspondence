
namespace UpdateControls.Correspondence.POXClient
{
	public interface IPOXConfigurationProvider
	{
		POXConfiguration Configuration { get; }
        bool IsToastEnabled { get; }
	}
}
