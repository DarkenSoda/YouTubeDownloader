using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using YouTubeDownloader.ViewModels;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace YouTubeDownloader.Implementation.DownloadStrategies;
internal class PlaylistStrategy : IDownloadStrategy {
	private readonly string videoUrl;
	private readonly YoutubeClient client;

	public PlaylistStrategy(YoutubeClient client, string videoUrl) {
		this.client = client;
		this.videoUrl = videoUrl;
	}

	public async Task<DownloadableItem?> GetDownloadableItem() {
		PlaylistItem? playlistItem = null;
		IEnumerable<IVideo> videos;
		try {
			Playlist playlist = await client.Playlists.GetAsync(videoUrl);
			videos = await client.Playlists.GetVideosAsync(videoUrl);

			ObservableCollection<VideoItem> videoItemList = new();
			foreach (var video in videos) {
				var videoItem = new VideoItem {
					Id = video.Id,
					Title = video.Title,
					ThumbnailUrl = video.Thumbnails[0].Url,
					Video = video
				};

				videoItemList.Add(videoItem);
			}

			playlistItem = new PlaylistItem {
				ThumbnailUrl = playlist.Thumbnails[0].Url,
				Title = playlist.Title,
				Id = playlist.Id,
				VideoList = videoItemList
			};
		} catch (Exception e) {
			Console.WriteLine("An error occurred while downloading the videos: " + e.Message);
		}

		return playlistItem;
	}
}

