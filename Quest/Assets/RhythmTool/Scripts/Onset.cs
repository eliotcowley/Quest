[System.Serializable]
public class Onset
{
    /// <summary>
    /// Frame index of this onset.
    /// </summary>
    public int index;
    /// <summary>
    /// Strength of this onset.
    /// </summary>
    public float strength;
    /// <summary>
    /// Rank of this onset. Lower rank (5 is best) indicates surrounding onsets are stronger.
    /// </summary>
    public int rank;

    public Onset(int index, float strength, int rank)
    {
        this.index = index;
        this.rank = rank;
        this.strength = strength;
    }

    public static bool operator <(Onset x, Onset y)
    {
        if (x == null && y == null)
        {
            return false;
        }
        if (x == null)
        {
            return true;
        }
        if (y == null)
        {
            return false;
        }

        return x.strength < y.strength;
    }

    public static bool operator <(Onset x, float y)
    {        
        if (x == null)
        {
            return true; 
        }
        return x.strength < y;
    }

    public static bool operator >(Onset x, Onset y)
    {
        if (x == null && y == null)
        {
            return false;
        }
        if (x == null)
        {
            return false; 
        }
        if (y == null)
        {
            return true; 
        }

        return x.strength > y.strength;
    }

    public static bool operator >(Onset x, float y)
    {        
        if (x == null)
        {
            return false; 
        }
        
        return x.strength > y;
    }

    public static implicit operator float(Onset x)
    {
        if (x == null)
            return 0;

        return x.strength;
    }
}
