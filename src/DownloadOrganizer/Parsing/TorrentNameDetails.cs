using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DownloadOrganizer.Parsing;

[DebuggerDisplay("{Name}")]
public sealed class TorrentNameDetails
{
    public bool Amazon { get; set; }
    public string Audio { get; set; }
    public string AudioChannels { get; set; }
    public int? BitDepth { get; set; }
    public bool Blurred { get; set; }
    public string Codec { get; set; }
    public bool Complete { get; set; }
    public string Container { get; set; }
    public bool DolbyAtmos { get; set; }
    public bool Dubbed { get; set; }
    public int? Episode { get; set; }
    public bool Extended { get; set; }
    public string Group { get; set; }
    public bool HDR { get; set; }
    public bool HardCoded { get; set; }
    public bool Is3D { get; set; }
    public bool MultipleLanguages { get; set; }
    public string Name { get; set; }
    public bool Netflix { get; set; }
    public bool Proper { get; set; }
    public string Quality { get; set; }
    public string Region { get; set; }
    public bool Remastered { get; set; }
    public bool Remux { get; set; }
    public bool Repack { get; set; }
    public string Resolution { get; set; }
    public int? Season { get; set; }
    public string ThreeDFormat { get; set; }
    public string Title { get; set; }
    public bool TrueHD { get; set; }
    public string Website { get; set; }
    public int Year { get; set; }

    /// <summary>
    /// This element is used in ignoring elements for parsing the title
    /// </summary>
    public string Garbage { get; set; }


    internal void SetValue(string property, string value)
    {
        switch (property)
        {
            case nameof(Audio):
                Audio = value;
                break;
            case nameof(AudioChannels):
                AudioChannels = value;
                break;
            case nameof(Codec):
                Codec = value;
                break;
            case nameof(Container):
                Container = value;
                break;
            case nameof(Group):
                Group = value;
                break;
            case nameof(Quality):
                Quality = value;
                break;
            case nameof(Region):
                Region = value;
                break;
            case nameof(Resolution):
                Resolution = value;
                break;
            case nameof(ThreeDFormat):
                ThreeDFormat = value;
                break;
            case nameof(Website):
                Website = value;
                break;
            case nameof(Garbage):
                Garbage = value;
                break;
        }
    }

    internal void SetValue(string property, int value)
    {
        switch (property)
        {
            case nameof(BitDepth):
                BitDepth = value;
                break;
            case nameof(Episode):
                Episode = value;
                break;
            case nameof(Season):
                Season = value;
                break;
            case nameof(Year):
                Year = value;
                break;
        }
    }

    internal void SetValue(string property, bool value)
    {
        switch (property)
        {
            case nameof(Amazon):
                Amazon = value;
                break;
            case nameof(Blurred):
                Blurred = value;
                break;
            case nameof(Complete):
                Complete = value;
                break;
            case nameof(DolbyAtmos):
                DolbyAtmos = value;
                break;
            case nameof(Dubbed):
                Dubbed = value;
                break;
            case nameof(Extended):
                Extended = value;
                break;
            case nameof(HDR):
                HDR = value;
                break;
            case nameof(HardCoded):
                HardCoded = value;
                break;
            case nameof(Is3D):
                Is3D = value;
                break;
            case nameof(MultipleLanguages):
                MultipleLanguages = value;
                break;
            case nameof(Netflix):
                Netflix = value;
                break;
            case nameof(Proper):
                Proper = value;
                break;
            case nameof(Remastered):
                Remastered = value;
                break;
            case nameof(Remux):
                Remux = value;
                break;
            case nameof(Repack):
                Repack = value;
                break;
            case nameof(TrueHD):
                TrueHD = value;
                break;
        }
    }

    internal static bool IsIntProperty(string property)
    {
        return property switch
        {
            nameof(BitDepth) or nameof(Episode) or nameof(Season) or nameof(Year) => true,
            _ => false
        };
    }

    internal static bool IsBoolProperty(string property)
    {
        return property switch
        {
            nameof(Amazon) or nameof(Blurred) or nameof(Complete) or nameof(DolbyAtmos) or nameof(Dubbed) or
            nameof(Extended) or nameof(HDR) or nameof(HardCoded) or nameof(Is3D) or nameof(MultipleLanguages) or
            nameof(Netflix) or nameof(Proper) or nameof(Remastered) or nameof(Remux) or nameof(Repack) or nameof(TrueHD) => true,
            _ => false
        };
    }

    internal static string[] GetProperties()
    {
        return
        [
            nameof(Amazon),
            nameof(Audio),
            nameof(AudioChannels),
            nameof(BitDepth),
            nameof(Blurred),
            nameof(Codec),
            nameof(Complete),
            nameof(Container),
            nameof(DolbyAtmos),
            nameof(Dubbed),
            nameof(Episode),
            nameof(Extended),
            nameof(Group),
            nameof(HDR),
            nameof(HardCoded),
            nameof(Is3D),
            nameof(MultipleLanguages),
            nameof(Netflix),
            nameof(Quality),
            nameof(Region),
            nameof(Remastered),
            nameof(Remux),
            nameof(Repack),
            nameof(Resolution),
            nameof(Season),
            nameof(ThreeDFormat),
            nameof(TrueHD),
            nameof(Website),
            nameof(Year),
            nameof(Garbage)
        ];
    }

    internal static Regex GetRegex(string property)
    {
        return property switch
        {
            nameof(Amazon) => Regexes.Amazon(),
            nameof(Audio) => Regexes.Audio(),
            nameof(AudioChannels) => Regexes.AudioChannels(),
            nameof(BitDepth) => Regexes.BitDepth(),
            nameof(Blurred) => Regexes.Blurred(),
            nameof(Codec) => Regexes.Codec(),
            nameof(Complete) => Regexes.Complete(),
            nameof(Container) => Regexes.Container(),
            nameof(DolbyAtmos) => Regexes.DolbyAtmos(),
            nameof(Dubbed) => Regexes.Dubbed(),
            nameof(Episode) => Regexes.Episode(),
            nameof(Extended) => Regexes.Extended(),
            nameof(Group) => Regexes.Group(),
            nameof(HDR) => Regexes.HDR(),
            nameof(HardCoded) => Regexes.HardCoded(),
            nameof(Is3D) => Regexes.Is3D(),
            nameof(MultipleLanguages) => Regexes.MultipleLanguages(),
            nameof(Netflix) => Regexes.Netflix(),
            nameof(Quality) => Regexes.Quality(),
            nameof(Region) => Regexes.Region(),
            nameof(Remastered) => Regexes.Remastered(),
            nameof(Remux) => Regexes.Remux(),
            nameof(Repack) => Regexes.Repack(),
            nameof(Resolution) => Regexes.Resolution(),
            nameof(Season) => Regexes.Season(),
            nameof(ThreeDFormat) => Regexes.ThreeDFormat(),
            nameof(TrueHD) => Regexes.TrueHD(),
            nameof(Website) => Regexes.Website(),
            nameof(Year) => Regexes.Year(),
            nameof(Garbage) => Regexes.Garbage(),
            _ => throw new NotSupportedException()
        };
    }

    internal static Regex GetAlternateRegex(string property)
    {
        return property is nameof(Group) ? Regexes.GroupAlternate() : null;
    }

    internal static string GetReplacements(string property)
    {
        return property switch
        {
            nameof(Audio) => "., ",
            nameof(AudioChannels) => " ,.",
            _ => ""
        };
    }
}