using DownloadOrganizer;


try
{
	if (args is { Length: 0 })
	{
		Organizer.Organize();
	}
	else if (args is { Length: 1 })
	{
		var completedDownload = args[0];
		if(string.IsNullOrEmpty(completedDownload))
		{
			return;
		}

		Organizer.Organize(completedDownload);
	}
	else if (args is { Length: 2 })
	{
		var sourceDir = args[0];
		var targetDir = args[1];
		if (string.IsNullOrEmpty(sourceDir) || string.IsNullOrEmpty(targetDir))
		{
			return;
		}

		Organizer.Organize(sourceDir, targetDir);
	}
}
catch (Exception ex)
{
	File.WriteAllText("DownloadOrganizerError.txt", ex.Message + Environment.NewLine + ex.StackTrace);
}



