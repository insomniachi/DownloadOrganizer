using ConsoleAppFramework;
using DownloadOrganizer;

var app = ConsoleApp.Create();

app.Add("mvd", Commands.MoveDirectory);
app.Add("", Commands.OrganizeDownloadsDirectory);
app.Add("run", Commands.Run);

try
{
	app.Run(args);
}
catch (Exception ex)
{
	File.WriteAllText("DownloadOrganizerError.txt", ex.Message + Environment.NewLine + ex.StackTrace);
}
