using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace AlchemicalRefinement.API.Common
{
    /// <summary>
    /// The type of Calcination for the collectible. This effects how the object is Calcinated.
    /// </summary>
    [DocumentAsJson]
    public enum EnumCalcinationType
    {
        /// <summary>
        /// Currently has no special behavior.
        /// </summary>
        Roast,
        /// <summary>
        /// Currently has no special behavior.
        /// </summary>
        Decomposistion,
        /// <summary>
        /// Currently has no special behavior.
        /// </summary>
        Reaction,
        /// <summary>
        /// Currently has no special behavior.
        /// </summary>
        Oxidation
    }
    public interface ICalcinatable
    {
        float GetCalcinationDuration(IWorldAccessor world, BlockPos blockPos);
        
    }
    /// <summary>
    /// Marks an item as Calcinationable, either by Roasting or Decomposistion.
    /// </summary>
    /// <example>
    /// Roast:
    /// <code language="json">
    ///"attributes": {
    ///     "calcinationPropsByType": {
    ///	        "powdered-ore-*-raw": {
    ///		        "CalcinationPoint": 150,
    ///		        "CalcinationEnergy": 30,
    ///		        "CalcinationRatio": 1,
    ///		        "CalcinationType": "Roast",
    ///		        "CalcinatedStack": { "type": "item","code": "powdered-ore-{ore}-calcinated", "quantity": 1 }
    ///	        }
    ///     },
    ///}
    /// </code>
    /// Oxidation:
    /// <code language="json">
    ///"attributes": {
    ///     "oxidationPropsByType": {
    ///	        "powdered-ore-*-raw": {
    ///		        "CalcinationPoint": 150,
    ///		        "CalcinationEnergy": 30,
    ///		        "CalcinationRatio": 1,
    ///		        "CalcinationType": "Oxidation",
    ///		        "CalcinatedStack": { "type": "item","code": "powdered-ore-{ore}-calcinated", "quantity": 1 },
    ///             "OxidizerStack": { "type": "item","code": "powdered-oxidizer", "quantity": 1 }
    ///	        }
    ///     },
    ///}
    /// </code>
    /// Reaction:
    /// <code language="json">
    ///"attributes": {
    ///     "reactionPropsByType": {
    ///	        "powdered-ore-*-raw": {
    ///		        "CalcinationPoint": 150,
    ///		        "CalcinationEnergy": 30,
    ///		        "CalcinationRatio": 1,
    ///		        "CalcinationType": "Reaction",
    ///		        "CalcinatedStack": { "type": "item","code": "powdered-ore-{ore}-calcinated", "quantity": 1 },
    ///             "CatalystStack": { "type": "item","code": "powdered-Catalyst", "quantity": 1 },
    ///             "OxidizerStack": { "type": "item","code": "powdered-oxidizer", "quantity": 1 }
    ///	        }
    ///     },
    ///}
    /// </code>
    /// 
    ///</example>
    [DocumentAsJson]
    public class CalcinationProperties
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// If set, this is the resulting itemstack once the CalcinationPoint has been reached for the supplied duration.
        /// </summary>
        [DocumentAsJson("Recommended", "0")]
        public JsonItemStack CalcinatedStack;
        
        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// If set, this itemstack is for the reaction to be able to complete if the CalcinationPoint is reached. 
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public JsonItemStack OxidizerStack;
        
        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>0</jsondefault>-->
        /// If set, this itemstack is for the reaction to be able to complete if the CalcinationPoint is reached. 
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public JsonItemStack CatalystStack;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// If there is a melting point, the max temperature it can reach. A value of 0 implies no limit.
        /// </summary>
        [DocumentAsJson("Recommended", "1200")]
        public float CalcinationMaxTemperature = 1200f;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// How many degrees celsius it takes to Calcinate/transform this collectible into another. Required if <see cref="CalcinatedStack"/> is set.
        /// </summary>
        [DocumentAsJson("Recommended", "600")]
        public float CalcinationPoint = 600;

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>0</jsondefault>-->
        /// How much energy needer after temperature is reached. Recommended if <see cref="CalcinatedStack"/> is set.
        /// This value is in kJ/kg and is dependant on the ThermalProperties Weight
        /// </summary>
        [DocumentAsJson("Recommended", "2000")]
        public float CalcinationEnergy = 2000;
        
        /// <summary>
        /// How many of this collectible are needed to calcinate into <see cref="CalcinatedStack"/>.
        /// </summary>
        [DocumentAsJson("Optional", "1")]
        public int CalcinationRatio = 1;
        
        /// <summary>
        /// Some Calcination types have specific functionality, and are also used for correct naming in the tool tip.
        /// If using <see cref="EnumCalcinationType.Roast"/>
        /// </summary>
        [DocumentAsJson("Recommended", "Roast")]
        public EnumCalcinationType CalcinationType;
        
        /*public CalcinationProperties Clone()
        {
            CalcinationProperties cloned = new CalcinationProperties();
            
            
            cloned.CalcinationMaxTemperature = CalcinationMaxTemperature;
            cloned.CalcinationPoint = CalcinationPoint;
            cloned.CalcinationDuration = CalcinationDuration;
            cloned.CalcinationRatio = CalcinationRatio;
            
            
            if (CalcinatedStack != null)
            {
                cloned.CalcinatedStack = CalcinatedStack.Clone();
            }

            return cloned;
        }*/
    }
}