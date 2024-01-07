using YoutubeExplode.Videos;

namespace YouTubeDownloader.ViewModels;
internal class VideoItem : DownloadableItem {
	public required IVideo Video { get; set; }
}
