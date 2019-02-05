#if NET472
namespace MSDI.NamedServiceExtensions
{
    public static class HashCode
    {
        private const int MagicNumber = 397;

        public static int Combine(params object[] parts)
        {
            unchecked
            {
                if (parts.Length == 0)
                {
                    return 0;
                }

                var hashCode = parts[0]?.GetHashCode() ?? 0;
                for (var i = 1; i < parts.Length; i++)
                    hashCode = (hashCode * MagicNumber) ^ (parts[i]?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

    }
}
#endif
