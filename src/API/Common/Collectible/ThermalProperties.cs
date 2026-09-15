using Vintagestory.API;

namespace AlchemicalRefinement.API.Common
{
    /// <summary>
    /// Gives an Item the ThermalProperties needed for many apparatuses to work.
    /// </summary>
    /// <example>
    /// <code language="json">
    /// "attributes": {
    ///     "thermalPropsByType": {
    ///         *-malachite: {
    ///             "SpecificHeatCapacity": 0.84,
    ///             "WeightInKg": 3,
    ///             "SpecificBurnValueMin": 10000,
    ///             "SpecificBurnValueMax": 15000,
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class ThermalProperties
    {
        /// <summary>
        /// The amount of energy needed to increase Temperature in kJ/kg·K
        /// Equation K = kJ/(Weight * stacksize * SpecificHeatCapacity) 
        /// </summary>
        [DocumentAsJson("Recommended", "0.84")]
        public float SpecificHeatCapacity = 0.84f;
        
        /// <summary>
        /// the weight pr Item in kg
        /// </summary>
        [DocumentAsJson("Recommended", "1")]
        public float WeightInKg = 1f;
        
        /// <summary>
        /// The Minimum amount of energy pr kg generates in kJ/kg
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float SpecificBurnValueMin;
        
        /// <summary>
        /// The Maximum amount of energy pr kg generates in kJ/kg
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float SpecificBurnValueMax;
    }
}