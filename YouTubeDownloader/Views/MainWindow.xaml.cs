using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YouTubeDownloader.Implementation;
using YouTubeDownloader.ViewModels;

namespace YouTubeDownloader.Views;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
	private readonly Downloader downloader = new Downloader();

	public MainWindow() {
		InitializeComponent();
	}

	private async void Button_Click(object sender, RoutedEventArgs e) {
		var downloadableItem = await downloader.GetVideoAsync(textbox.Text) ?? await downloader.GetPlaylistAsync(textbox.Text);

		if (downloadableItem == null) {
			Console.WriteLine("Cannot find results");
			return;
		}

		if (!videoListView.Items.Cast<DownloadableItem>().Where(v => v.Id == downloadableItem.Id).Any())
			videoListView.Items.Add(downloadableItem);
	}

	private void DownloadButton_Click(object sender, RoutedEventArgs e) {
		List<DownloadableItem> itemsToDownload = videoListView.Items
			.Cast<DownloadableItem>()
			.Where(i => i.IsSelectedForDownload && i.ProgressBar < 1)
			.ToList();

		downloader.StartAllDownload(itemsToDownload);
	}

	private void CheckBox_Checked(object sender, RoutedEventArgs e) {

	}

	private void CheckBox_Unchecked(object sender, RoutedEventArgs e) {

	}

	private void Expander_Expanded(object sender, RoutedEventArgs e) {

	}
}
