namespace JpCalendar;

public
#if NET6_0_OR_GREATER
    readonly
#endif
    struct Era
{
    public
#if NET8_0_OR_GREATER
        required
#endif
        string Name
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET8_0_OR_GREATER
        required
#endif
        string Abbreviation
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }

    public
#if NET8_0_OR_GREATER
        required
#endif
        char Symbol
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set;
#endif
    }
}
