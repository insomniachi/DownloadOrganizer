using DownloadOrganizer;

if (args is { Length : not 2})
{
	Console.WriteLine("Incorrect number of arguments");
	return;
}

var sourceDir = args[0];
var targetDir = args[1];

Organizer.Organize(sourceDir, targetDir);
