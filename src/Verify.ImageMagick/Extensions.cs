using System;

static class Extensions
{
    public static bool HasValue(this object input)
    {
        if (input is ValueType)
        {
            var obj = Activator.CreateInstance(input.GetType());
            return !obj.Equals(input);
        }
        if (input is string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        return !(input is null);
    }
}