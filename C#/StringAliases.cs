using System.Collections;
using System.Collections.Generic;

public class StringAliases
{
    public List<StringAlias> aliases = new List<StringAlias>();

    public void AddAlias(params string[] strings)
    {
        StringAlias newAlias = new StringAlias(strings);
        aliases.Add(newAlias);
    }

    public void RemoveAlias(params string[] strings)
    {
        StringAlias newAlias = new StringAlias(strings);
        aliases.Remove(newAlias);
    }

    public bool Compare(string src, string alias)
    {
        foreach(StringAlias a in aliases)
        {
            if(a == src)
            {
                return a == alias;
            }
        }

        return false;
    }
}
