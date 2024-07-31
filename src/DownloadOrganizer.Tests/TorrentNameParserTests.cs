using DownloadOrganizer.Parsing;

namespace DownloadOrganizer.Tests;

public class TorrentNameParserTests
{
	[Theory]
	[InlineData("The Flash 2023 1080p V2 Cam X264 Will1869")]
	[InlineData("Iron Man 3 2013 REMASTERED 2160p UHD BluRay X265-IAMABLE")]
	[InlineData("John Wick 3 2019 1080p Bluray DTS-HD MA 5 1 x264-EVO")]
	[InlineData("Annabelle Comes Home 2019 1080p HC HDRip X264-EVO")]
	[InlineData("Suits S09E05 720p HDTV x264-aAF")]
	[InlineData("The Boys S01 2160p WEB-DL DDP5 1 HDR HEVC-NEOLUTiON")]
	public void ParseTest(string name)
	{
		var actual = TorrentNameParser.Parse(name);
		var expected = _expectedResults[name];

		Assert.Equal(expected.Amazon, actual.Amazon);
		Assert.Equal(expected.Audio, actual.Audio);
		Assert.Equal(expected.AudioChannels, actual.AudioChannels);
		Assert.Equal(expected.BitDepth, actual.BitDepth);
		Assert.Equal(expected.Blurred, actual.Blurred);
		Assert.Equal(expected.Codec, actual.Codec);
		Assert.Equal(expected.Complete, actual.Complete);
		Assert.Equal(expected.Container, actual.Container);
		Assert.Equal(expected.DolbyAtmos, actual.DolbyAtmos);
		Assert.Equal(expected.Dubbed, actual.Dubbed);
		Assert.Equal(expected.Episode, actual.Episode);
		Assert.Equal(expected.Extended, actual.Extended);
		Assert.Equal(expected.Group, actual.Group);
		Assert.Equal(expected.HDR, actual.HDR);
		Assert.Equal(expected.HardCoded, actual	.HardCoded);
		Assert.Equal(expected.Is3D, actual.Is3D);
		Assert.Equal(expected.MultipleLanguages, actual.MultipleLanguages);
		Assert.Equal(expected.Netflix, actual.Netflix);
		Assert.Equal(expected.Proper, actual.Proper);
		Assert.Equal(expected.Quality, actual.Quality);
		Assert.Equal(expected.Region, actual.Region);
		Assert.Equal(expected.Remastered, actual.Remastered);
		Assert.Equal(expected.Remux, actual.Remux);
		Assert.Equal(expected.Repack, actual.Repack);
		Assert.Equal(expected.Resolution, actual.Resolution);
		Assert.Equal(expected.Season, actual.Season);
		Assert.Equal(expected.ThreeDFormat, actual.ThreeDFormat);
		Assert.Equal(expected.Title, actual.Title);
		Assert.Equal(expected.TrueHD, actual.TrueHD);
		Assert.Equal(expected.Website, actual.Website);
		Assert.Equal(expected.Year, actual.Year);
		Assert.Equal(expected.Garbage, actual.Garbage);
	}

	private readonly Dictionary<string, TorrentNameDetails> _expectedResults = new()
	{
		// Movies
		["The Flash 2023 1080p V2 Cam X264 Will1869"] = new TorrentNameDetails
		{
			Title = "The Flash",
			Year = 2023,
			Resolution = "1080p",
			Quality = "Cam",
			Codec = "X264",
			Group = "Will1869"
		},

		["Iron Man 3 2013 REMASTERED 2160p UHD BluRay X265-IAMABLE"] = new TorrentNameDetails
		{
			Title = "Iron Man 3",
			Year = 2013,
			Remastered = true,
			Resolution = "2160p",
			Quality = "UHD BluRay",
			Codec = "X265",
			Group = "IAMABLE"
		},

		["John Wick 3 2019 1080p Bluray DTS-HD MA 5 1 x264-EVO"] = new TorrentNameDetails
		{
			Title = "John Wick 3",
			Year = 2019,
			Resolution = "1080p",
			Quality = "Bluray",
			Audio = "DTS-HD MA",
			AudioChannels = "5.1",
			Codec = "x264",
			Group = "EVO"
		},

		["Annabelle Comes Home 2019 1080p HC HDRip X264-EVO"] = new TorrentNameDetails
		{
			Title = "Annabelle Comes Home",
			Year = 2019,
			Resolution = "1080p",
			HardCoded = true,
			HDR = false,
			Quality = "HDRip",
			Codec = "X264",
			Group = "EVO"
		},



		// Series
		["Suits S09E05 720p HDTV x264-aAF"] = new TorrentNameDetails
		{
			Title = "Suits",
			Season = 9,
			Episode = 5,
			Resolution = "720p",
			Quality = "HDTV",
			Codec = "x264",
			Group = "aAF"
		},

		["The Boys S01 2160p WEB-DL DDP5 1 HDR HEVC-NEOLUTiON"] = new TorrentNameDetails
		{
			Title = "The Boys",
			Season = 1,
			Resolution = "2160p",
			Quality = "WEB-DL",
			Audio = "DDP",
			AudioChannels = "5.1",
			HDR = true,
			Codec = "HEVC",
			Group = "NEOLUTiON"
		}
	};
}
