using System.Threading.Tasks;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Implementation.DownloadStrategies;
internal interface IDownloadStrategy {
	public Task<DownloadableItem?> GetDownloadableItem();
}
