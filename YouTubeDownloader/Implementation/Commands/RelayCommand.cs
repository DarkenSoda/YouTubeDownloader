using System;

namespace YouTubeDownloader.Implementation.Commands;
internal class RelayCommand : ICommand {
	private readonly Action command;

	public RelayCommand(Action command) {
		this.command = command;
	}

	public void Execute() => command?.Invoke();
}
