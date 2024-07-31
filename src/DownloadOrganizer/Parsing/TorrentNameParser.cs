using System.Text.RegularExpressions;
using System.Web;

namespace DownloadOrganizer.Parsing;

public static partial class TorrentNameParser
{
	public static TorrentNameDetails Parse(string name)
	{
		var result = new TorrentNameDetails();
		var end = name.Length;
		var start = 0;
		string clean;

		name = HttpUtility.HtmlDecode(name);

		if (string.IsNullOrEmpty(name))
		{
			return null;
		}

		foreach (var property in TorrentNameDetails.GetProperties())
		{
			var regex = TorrentNameDetails.GetRegex(property);
			var match = regex.Match(name);

			if (!match.Success && TorrentNameDetails.GetAlternateRegex(property) is { } altRegex)
			{
				match = altRegex.Match(name);
			}

			if (match.Success)
			{
				var cleanIndex = match.Groups.Count > 1 ? 2 : 0;
				clean = match.Groups[cleanIndex].Value;

				if (TorrentNameDetails.IsIntProperty(property))
				{
					result.SetValue(property, int.Parse(clean));
				}
				else if (TorrentNameDetails.IsBoolProperty(property))
				{
					result.SetValue(property, true);
				}
				else
				{
					if (property == nameof(TorrentNameDetails.Group))
					{
						clean = GroupReplaceRegex().Replace(clean, "");
					}

					var replacements = TorrentNameDetails.GetReplacements(property);
					if (!string.IsNullOrEmpty(replacements))
					{
						foreach (var replace in replacements.Split('|'))
						{
							var parts = replace.Split(',');
							if (parts.Length == 2)
							{
								clean = clean.Replace(parts[0], parts[1]);
							}
						}
					}

					result.SetValue(property, clean);
				}

				if (match.Index == 0)
				{
					start = match.Groups[0].Length;
				}
				else if (match.Index < end)
				{
					end = match.Index;
				}
			}
		}

		var raw = name[start..end].Split('(')[0];
		clean = HypenRegex().Replace(raw, "");
		if (!clean.Contains(' ') && clean.Contains('.'))
		{
			clean = DotRegex().Replace(clean, " ");
		}

		clean = UndersscoreOrDotRegex().Replace(clean, " ");
		clean = BracketsRegex().Replace(clean, "").Trim();
		result.Title = clean;
		result.Name = name;

		return result;
	}

	[GeneratedRegex(@" *\([^)]*\) *")]
	private static partial Regex GroupReplaceRegex();
	
	[GeneratedRegex(@"^ -")]
	private static partial Regex HypenRegex();
	
	[GeneratedRegex(@"\.")]
	private static partial Regex DotRegex();
	
	[GeneratedRegex(@"_|\.")]
	private static partial Regex UndersscoreOrDotRegex();
	
	[GeneratedRegex(@"([\(_]|- ?)$")]
	private static partial Regex BracketsRegex();
}
