using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace AlchemicalRefinement.API.Common
{
    public class ThermalFunctions
    {
        public void HeatItemstack(IWorldAccessor world, ItemStack stack, float temperature)
        {
            stack.Collectible.SetTemperature(world, stack, temperature);
        }

        public float GetItemTemperature(IWorldAccessor world, ItemStack stack)
        {
            return stack?.Collectible.GetTemperature(world, stack) ?? 20;
        }

        /// <summary>
        /// a Methode to Equalize the Temperature of all the Items In the Array,
        /// this methode will set the temperatures of the item
        /// and return the new temperature if it is needed as a variable.
        /// </summary>
        /// <param name="world"> Api.World</param>
        /// <param name="slots"> Array of slots to loop usually the Input Slots of the block</param>
        /// <param name="energyNeededForTemperatureIncrease"> energy that is needed to increase the temperature</param>
        /// <returns>the new temperature of the items so it can be set to a variable</returns>
        public float EqualizeItemTemperature(IWorldAccessor world, ItemSlot[] slots, float energyNeededForTemperatureIncrease)
        {
            // TODO: Find out what the hell is not working with this.
            ThermalProperties props;
            ItemStack stack;
            float storedEnergy = 0f;
            float temperature;
            foreach (var slot in slots)
            {
                if (!slot.Empty)
                {
                    stack = slot.Itemstack;
                    props = GetThermalProperties(stack);
                    if (props != null && GetItemTemperature(world, stack) > 20)
                    {
                        storedEnergy += CalculateStoredHeatEnergy(
                            GetItemTemperature(world, stack), props.WeightInKg,
                            stack.StackSize, props.SpecificHeatCapacity);
                    }
                }
            }
            temperature = IncreaseTemperature(storedEnergy, energyNeededForTemperatureIncrease);
            foreach (var slot in slots)
            {
                if (!slot.Empty)
                {
                    stack = slot.Itemstack;
                    HeatItemstack(world, stack, temperature);
                }
            }
            return temperature;
        }

        /// <summary>
        /// Calculates the stored energy
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="weight"></param>
        /// <param name="stackSize"></param>
        /// <param name="specificHeatCapacity"></param>
        /// <returns></returns>
        public float CalculateStoredHeatEnergy(float temperature, float weight, float stackSize, float specificHeatCapacity)
        {
            return temperature * weight * stackSize * specificHeatCapacity;
        }
        
        /// <summary>
        /// get the ThermalProperties of a ItemStack
        /// </summary>
        /// <param name="stack"> ItemStack to check</param>
        /// <returns></returns>
        public ThermalProperties GetThermalProperties(ItemStack stack)
        {
            return stack?.ItemAttributes?["thermalProps"].AsObject<ThermalProperties>();
        }

        public CombustibleProperties GetCombustibleProperties(IWorldAccessor world, ItemStack stack, BlockPos pos)
        {
            return stack?.Collectible.GetCombustibleProperties(world, stack, pos);
        }
        
        /// <summary>
        /// Methode used to calculate the temp increase for an item/block.
        /// </summary>
        /// <param name="inputEnergy"> The energy amount for temp increase in kJ</param>
        /// <param name="weight"> the Weight of the Item</param>
        /// <param name="stackSize">Amount of items in Stack</param>
        /// <param name="specificHeatCapacity"> the specific heat capacity in kJ/kg·K stored in ThermalProperties</param>
        /// <returns>float Temperature increase in ºK</returns>
        public float CalculateTemperatureIncrease(float inputEnergy, float weight, float stackSize, float specificHeatCapacity)
        {
            return inputEnergy / (weight * stackSize * specificHeatCapacity);
        }

        /// <summary>
        /// calculate the needed energy for temperature increase in kJ/ºK
        /// </summary>
        /// <param name="weight"> the Weight of the Item</param>
        /// <param name="stackSize">Amount of items in Stack</param>
        /// <param name="specificHeatCapacity"> the specific heat capacity in kJ/kg·K stored in ThermalProperties</param>
        /// <returns>float value kJ/ºK</returns>
        public float EnergyForTemperatureIncrease(float weight, float stackSize, float specificHeatCapacity)
        {
            return weight * stackSize * specificHeatCapacity;
        }

        /// <summary>
        /// Calculate Craft energy needed after temperature is reached.
        /// </summary>
        /// <param name="craftEnergy"></param>
        /// <param name="stackSize"></param>
        /// <returns>Returns energy needed for craft in kJ</returns>
        public float CraftEnergyNeeded(float craftEnergy, float stackSize)
        {
            return craftEnergy * stackSize;
        }
        
        /// <summary>
        /// Calculate Temperature based on needed energy to increase it
        /// </summary>
        /// <param name="inputEnergy"> The energy amount for temp increase in kJ</param>
        /// <param name="energyNeeded"></param>
        /// <returns>float Temperature increase in ºK</returns>
        public float IncreaseTemperature(float inputEnergy, float energyNeeded)
        {
            return inputEnergy / energyNeeded;
        }
        
        /// <summary>
        /// Calculate the current Specific BurnValue for the item 
        /// </summary>
        /// <param name="minBurnValue"> SpecificBurnValueMin stored in ThermalProperties</param>
        /// <param name="maxBurnValue">SpecificBurnValueMax stored in ThermalProperties</param>
        /// <param name="currentFuelBurnTemp">Fuel burn temperature at current moment same as BlockTemperature</param>
        /// <param name="maxFuelBurnTemp"></param>
        /// <returns>float value in kJ/kg</returns>
        public float CalculateBurnValue(float minBurnValue, float maxBurnValue, float currentFuelBurnTemp, float maxFuelBurnTemp)
        {
            return (maxBurnValue - minBurnValue) / maxFuelBurnTemp * currentFuelBurnTemp + minBurnValue;
        }
        
        /// <summary>
        /// calculate the energy generated pr sec in kJ/s or kW  
        /// </summary>
        /// <param name="burnDuration"> the total amount of time the Item burns for stored in CombustionProperties</param>
        /// <param name="weight"> the weight of the item is stored in ThermalProperties</param>
        /// <param name="specificBurnValue"> Energy stored pr kilo given in kJ/kg stored in ThermalProperties </param>
        /// <returns>float value in kJ/s or kW</returns>
        public float CalculateFuelEnergyOutput(float burnDuration, float weight, float specificBurnValue)
        {
            return specificBurnValue * weight / burnDuration;
        }
    }
    public interface IThermalProps
    {
        
        /// <summary>
        /// Calculated Fuel Energy Output when burning in kJ/s or kW
        /// </summary>
        float FuelEnergyOutput { get; set; }
        /// <summary>
        /// how fast the fuel increses temp in ºK/s
        /// </summary>
        float FuelTempIncrease { get; set; }
        float FuelBurnTime { get; set; }
        float FuelTemperatureMax { get; set; }

        float FuelTemperature { get; set; }
        
        /// <summary>
        /// The temperature increase for the block.
        /// </summary>
        float BlockTemperatureIncrease { get; set; }
        /// <summary>
        /// Max Temperature the block can be 
        /// </summary>
        float BlockTemperatureMax { get; set; }
        /// <summary>
        /// the current Specific BurnValue for the item in kJ/kg 
        /// </summary>
        float CurrentBurnValue { get; set; }
    }
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