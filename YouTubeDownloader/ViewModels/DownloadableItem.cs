using YouTubeDownloader.Implementation.Commands;
using YouTubeDownloader.ViewModels.Framework;

namespace YouTubeDownloader.ViewModels;
internal abstract class DownloadableItem : ViewModelBase {
	public required string Title { get; set; }
	public required string Id { get; set; }
	public required string ThumbnailUrl { get; set; }
	public ICommand? StartDownload { get; set; }
	public ICommand? PauseDownload { get; set; }
	public ICommand? CancelDownload { get; set; }
	public bool IsSelectedForDownload { get; set; } = true;

	private double progress;
	public double ProgressBar { get => progress; set => Set(ref progress, value); }
}
