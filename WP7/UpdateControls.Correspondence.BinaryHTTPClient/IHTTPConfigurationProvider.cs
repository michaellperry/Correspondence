
namespace UpdateControls.Correspondence.BinaryHTTPClient
{
	public interface IHTTPConfigurationProvider
	{
		HTTPConfiguration Configuration { get; }
        bool IsToastEnabled { get; }
        bool IsNotificationEnabled { get; }
	}
}
