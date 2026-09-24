namespace CloudFileManager.Core;

public enum TagKind { Urgent, Work, Personal }
public static class TagCatalog
{
    public static string Color(TagKind tag) => tag switch
    {
        TagKind.Urgent => "紅", TagKind.Work => "藍", TagKind.Personal => "綠",
        _ => throw new ArgumentOutOfRangeException(nameof(tag))
    };
}
