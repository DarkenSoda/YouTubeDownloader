using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeDownloader.ViewModels;
using YoutubeExplode;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace YouTubeDownloader.Implementation.DownloadStrategies;
internal class VideoStrategy : IDownloadStrategy {
	private readonly string videoUrl;
	private readonly YoutubeClient client;

	public VideoStrategy(YoutubeClient client, string videoUrl) {
		this.client = client;
		this.videoUrl = videoUrl;
	}

	public async Task<DownloadableItem?> GetDownloadableItem() {
		VideoItem? videoItem = null;
		try {
			IVideo video = await client.Videos.GetAsync(videoUrl);
			
			videoItem = new VideoItem {
				Id = video.Id,
				Title = video.Title,
				ThumbnailUrl = video.Thumbnails[0].Url,
				Video = video,
			};
		} catch (Exception e) {
			Console.WriteLine("An error occurred while downloading the videos: " + e.Message);
		}

		return videoItem;
	}
}
