using System.Windows.Controls;
using System.Windows;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Views;
public class ListDataTemplateSelector : DataTemplateSelector {
	public required DataTemplate VideoItemTemplate { get; set; }
	public required DataTemplate PlaylistItemTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container) {
		if (item is VideoItem) {
			return VideoItemTemplate;
		} else if (item is PlaylistItem) {
			return PlaylistItemTemplate;
		}

		// Default template if the item type is not recognized
		return base.SelectTemplate(item, container);
	}
}