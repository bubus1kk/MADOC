namespace MADOC.Domain.Validation.Lists
{
    public static class ListOptionKeyConverter
    {
        public static bool TryConvert(object? value, out ListOptionKey key)
        {
            if (value is ListOptionKey listOptionKey)
            {
                key = listOptionKey;
                return true;
            }

            key = default;
            return false;
        }
    }
}
