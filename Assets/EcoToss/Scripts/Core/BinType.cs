namespace EcoToss.Core
{
    /// <summary>
    /// Waste bin categories based on Colombia's waste separation color code (Resolution 2184 of 2019).
    /// </summary>
    public enum BinType
    {
        /// <summary>White bin: clean recyclables such as plastic, glass, metal, paper, and cardboard.</summary>
        Recyclable,

        /// <summary>Green bin: food scraps and organic waste.</summary>
        Organic,

        /// <summary>Black bin: non-recyclables, toilet paper, napkins, and contaminated items.</summary>
        NonRecyclable
    }
}
