namespace EcoToss.Core
{
    /// <summary>
    /// Tipos de caneca según el código de colores de Colombia (Resolución 2184 de 2019).
    /// </summary>
    public enum BinType
    {
        /// <summary>Caneca blanca: plástico, vidrio, metal, papel y cartón limpios.</summary>
        Recyclable,

        /// <summary>Caneca verde: restos de comida y residuos orgánicos.</summary>
        Organic,

        /// <summary>Caneca negra: papel higiénico, servilletas y residuos contaminados.</summary>
        NonRecyclable
    }
}
