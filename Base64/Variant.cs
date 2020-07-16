namespace Base64
{
    public enum Variant
    {
        Original = 1,
        OriginalNoPadding = Original | Mask.NoPadding,
        UrlSafe = 1 | Mask.UrlSafe,
        UrlSafeNoPadding = UrlSafe | Mask.NoPadding,
    }
}