using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YouTubeDownloader.Implementation.Commands;
using YouTubeDownloader.Implementation.DownloadStrategies;
using YouTubeDownloader.ViewModels;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloader.Implementation;
internal partial class Downloader {
	private readonly YoutubeClient youtubeClient;
	private readonly HttpClient httpClient;
	private readonly Semaphore downloadLimit;
	private IDownloadStrategy? downloadStrategy;
	private string outputDestination;

	public string OutputDestination {
		get => outputDestination;
		set {
			if (Directory.Exists(value)) {
				outputDestination = value;
			}
		}
	}

	public Downloader() {
		youtubeClient = new YoutubeClient();
		httpClient = new HttpClient();
		outputDestination = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\YoutubeDownloader";
		downloadLimit = new Semaphore(2, 2);
	}
}

internal partial class Downloader {
	public async Task<DownloadableItem?> GetVideoAsync(string url) {
		downloadStrategy = new VideoStrategy(youtubeClient, url);
		var downloadableItem = await downloadStrategy.GetDownloadableItem();

		if (downloadableItem is VideoItem video) {
			video.StartDownload = new RelayCommand(() => StartVideoDownload(video));

			return video;
		}

		return null;
	}

	public async Task<DownloadableItem?> GetPlaylistAsync(string url) {
		downloadStrategy = new PlaylistStrategy(youtubeClient, url);
		var downloadableItem = await downloadStrategy.GetDownloadableItem();

		if (downloadableItem is not PlaylistItem playlist)
			return null;

		playlist.StartDownload = new RelayCommand(() => StartPlaylistDownload(playlist));

		foreach (var video in playlist.VideoList) {
			video.StartDownload = new RelayCommand(() => StartVideoDownload(video));
		}

		return playlist;
	}
}

internal partial class Downloader {
	public void StartAllDownload(IEnumerable<DownloadableItem>? videos) {
		if (videos == null)
			return;

		foreach (var video in videos) {
			video.StartDownload?.Execute();
		}
	}

	private void StartVideoDownload(VideoItem video) {
		ThreadStart threadStart = new ThreadStart(
				async () => {
					downloadLimit.WaitOne();

					await Download(video);

					downloadLimit.Release();
				});

		Thread thread = new Thread(threadStart);
		thread.Start();
	}

	private void StartPlaylistDownload(PlaylistItem playlist) {
		foreach (var videoItem in playlist.VideoList) {
			videoItem.StartDownload?.Execute();
		}
	}

	private async Task Download(VideoItem video) {
		StreamManifest streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Video.Id);

		List<IStreamInfo> streamInfos = streamManifest
			.GetMuxedStreams()
			.Where(s => s.Container == Container.Mp4)
			.OrderBy(s => s.Bitrate)
			.Cast<IStreamInfo>()
			.ToList();

		if (!streamInfos.Any()) {
			Console.WriteLine("Couldn't find a suitable Stream");
			return;
		}

		IStreamInfo streamInfo = streamInfos.First();
		StreamClient streamClient = new StreamClient(httpClient);

		Directory.CreateDirectory(OutputDestination);

		var progressHandler = new Progress<double>(p => video.ProgressBar = p);
		await streamClient.DownloadAsync(streamInfo, GetOutputFilePath(video.Title, streamInfo.Container.ToString()), progressHandler);
	}

	private string SanitizeTitle(string title) => string.Join("_", title.Split(Path.GetInvalidFileNameChars()));

	private string GetOutputFilePath(string title, string extension) => Path.Combine(OutputDestination, $"{SanitizeTitle(title)}.{extension}");
}
