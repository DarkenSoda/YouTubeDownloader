using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace YouTubeDownloader.ViewModels;
internal class PlaylistItem : DownloadableItem {
	private ObservableCollection<VideoItem> videoList = new();
	public required ObservableCollection<VideoItem> VideoList {
		get => videoList;
		set {
			videoList = value;
			SetVideoList();
		}
	}

	public void SetVideoList() {
		foreach (VideoItem video in VideoList) {
			video.PropertyChanged += CalculateProgress;
		}
	}

	private void CalculateProgress(object? sender, PropertyChangedEventArgs e) {
		double value = 0;

		foreach (VideoItem video in VideoList) {
			value += video.ProgressBar;
		}

		ProgressBar = value / VideoList.Count;
	}
}
