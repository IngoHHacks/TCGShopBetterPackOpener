using TCGShopBetterPackOpener.Globals;

namespace TCGShopBetterPackOpener.Extensions;

public static class Vector3Extensions
{
    public static string FormattedString(this Vector3 vector)
    {
        return string.Format(NFP.Default, "{0} {1} {2}", vector.x, vector.y, vector.z);
    }
}