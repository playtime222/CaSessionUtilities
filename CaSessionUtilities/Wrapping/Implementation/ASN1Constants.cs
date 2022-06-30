namespace CaSessionUtilities.Wrapping.Implementation;

public static class ASN1Constants
{

    /** Universal tag class. */
    public const int UNIVERSAL_CLASS = 0;            /* 00 x xxxxx */

    /** Application tag class. */
    public const int APPLICATION_CLASS = 1;        /* 01 x xxxxx */

    /** Context specific tag class. */
    public const int CONTEXT_SPECIFIC_CLASS = 2; /* 10 x xxxxx */

    /** Private tag class. */
    public const int PRIVATE_CLASS = 3;          /* 11 x xxxxx */
}