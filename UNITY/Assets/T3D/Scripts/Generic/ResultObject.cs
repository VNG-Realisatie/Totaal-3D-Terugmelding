public class CoString
{
    public string val;

    public static implicit operator string(CoString co)
    {
        return co.val;
    }

    public override string ToString()
    {
        return val;
    }

}

public class CoBool
{
    public bool val;

    public static implicit operator bool(CoBool co)
    {
        return co.val;
    }

}