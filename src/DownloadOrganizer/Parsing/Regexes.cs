using System.Text.RegularExpressions;

namespace DownloadOrganizer.Parsing;

internal static partial class Regexes
{
	[GeneratedRegex("AMZN")]
	public static partial Regex Amazon();

	[GeneratedRegex(@"MP3|DDP?\+?|Dual[\- ]Audio|LiNE|D[Tt][Ss](?:-?6[Cc][Hh])?(?:-?HD)?(?:[ \.]?MA)?|AAC(?:\.?2\.0)?|[Aa][Cc]-?3(?:\s?DD)?")]
	public static partial Regex Audio();

	[GeneratedRegex(@"[257][\s\.][01]")]
	public static partial Regex AudioChannels();

	[GeneratedRegex(@"((\d+)\s?bit)", RegexOptions.IgnoreCase)]
	public static partial Regex BitDepth();

	[GeneratedRegex("BLURRED")]
	public static partial Regex Blurred();

	[GeneratedRegex(@"xvid|[xh]\.?26[45]|hevc", RegexOptions.IgnoreCase)]
	public static partial Regex Codec();

	[GeneratedRegex("COMPLETE")]
	public static partial Regex Complete();

	[GeneratedRegex(@"[\s\.-](MKV|AVI|MP4|mkv|avi|mp4)")]
	public static partial Regex Container();

	[GeneratedRegex(@"ATMOS|Atmos\b")]
	public static partial Regex DolbyAtmos();

	[GeneratedRegex("DUBBED")]
	public static partial Regex Dubbed();

	[GeneratedRegex(@"([Eex]([0-9]{2})(?:[^0-9]|$))")]
	public static partial Regex Episode();

	[GeneratedRegex("EXTENDED")]
	public static partial Regex Extended();

	[GeneratedRegex(@"(- ?(?:.+\])?([^-\[]+)(?:\[.+\])?)$")]
	public static partial Regex Group();

	[GeneratedRegex(@"HDR(?:\s?10)?(?:[\s\.\]])")]
	public static partial Regex HDR();

	[GeneratedRegex("HC")]
	public static partial Regex HardCoded();

	[GeneratedRegex("([^A-Za-z0-9](3D)[^A-Za-z0-9])")]
	public static partial Regex Is3D();

	[GeneratedRegex("MULT[iI]-?(?:[0-9]+)?")]
	public static partial Regex MultipleLanguages();

	[GeneratedRegex("NF")]
	public static partial Regex Netflix();

	[GeneratedRegex("PROPER")]
	public static partial Regex Proper();

	[GeneratedRegex(@"(?:PPV\.)?[HP]DTV|(?:HD)?C[Aa][Mm]|B[DrR]R[iI][pP][sS]?|TS|(?:PPV )?WEB[- ]?DL(?: DVDRip)?|H[dD]Rip|DVDRip|DVDRiP|DVDRIP|TELESYNC|CamRip|W[EB]B[rR]ip|[Bb]lu[ -]?[Rr]ay|DvDScr|hdtv|UHD(?: B[Ll][Uu][- ]?[Rr][Aa][Yy])")]
	public static partial Regex Quality();

	[GeneratedRegex(@"R[0-9]")]
	public static partial Regex Region();

	[GeneratedRegex("REMASTERED")]
	public static partial Regex Remastered();

	[GeneratedRegex("REMUX")]
	public static partial Regex Remux();

	[GeneratedRegex("REPACK")]
	public static partial Regex Repack();

	[GeneratedRegex(@"(([0-9]{3,4}p))[^M]")]
	public static partial Regex Resolution();

	[GeneratedRegex(@"([Ss]([0-9]{1,2}))[Eex\s]")]
	public static partial Regex Season();

	[GeneratedRegex(@"(?:full|half)[-\s](?:sbs|ou)", RegexOptions.IgnoreCase)]
	public static partial Regex ThreeDFormat();

	[GeneratedRegex(@"TrueHD(?:[ \.]MA)?")]
	public static partial Regex TrueHD();

	[GeneratedRegex(@"^(\[ ?([^\]]+?) ?\])")]
	public static partial Regex Website();

	[GeneratedRegex(@"([\[\(]?((?:19|20)[0-9]{2})[\]\)]?)")]
	public static partial Regex Year();

	[GeneratedRegex(@"1400Mb|3rd Nov| ((Rip))| \[no rar\]|[\[\(]?[Rr][Ee][Qq][\]\)]?")]
	public static partial Regex Garbage();



	[GeneratedRegex(@"(([A-Za-z0-9]+))$")]
	public static partial Regex GroupAlternate();
}
