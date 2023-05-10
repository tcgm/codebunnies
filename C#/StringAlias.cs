using System;
using System.Collections;
using System.Collections.Generic;

public class StringAlias : IEquatable<StringAlias>
{
    public List<string> strings = new List<string>();

    public StringAlias(params string[] passedStrings)
    {
        strings.AddRange(passedStrings);
    }

    public void AddAlias(string alias)
    {
        strings.Add(alias);
    }

    public void RemoveAlias(string alias)
    {
        strings.Remove(alias);
    }

    public bool Equals(string other)
    {
        if (strings.Contains(other)) return true;
        return false;
    }

    public override bool Equals(object obj)
    {
        return (obj as StringAlias).Equals(this);
    }

    public override string ToString()
    {
        return "StringAlias" + strings.ToString();
    }

    public bool Equals(StringAlias other)
    {
        return other is not null &&
               EqualityComparer<List<string>>.Default.Equals(strings, other.strings);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(strings);
    }

    public static bool operator ==(StringAlias alias, string other)
    {
        return alias.Equals(other);
    }

    public static bool operator !=(StringAlias alias, string other)
    {
        return !alias.Equals(other);
    }

    public static bool operator ==(StringAlias left, StringAlias right)
    {
        return EqualityComparer<StringAlias>.Default.Equals(left, right);
    }

    public static bool operator !=(StringAlias left, StringAlias right)
    {
        return !(left == right);
    }
}
