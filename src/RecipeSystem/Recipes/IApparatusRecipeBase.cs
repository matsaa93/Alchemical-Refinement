using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace AlchemicalRefinement.RecipeSystem.Recipes
{

    public interface IApparatusRecipeBase<T>
    {
        /// <summary>
        /// Name typically is the path of the file that define the recipe.
        /// </summary>
        AssetLocation Name { get; set; }
        
        /// <summary>
        /// Recipe code mainly for tracking issues for debugging.
        /// </summary>
        string Code { get; set; }
        
        /// <summary>
        /// Disables/Enables Recipe in JSON if "enabled: false" recipe will not load.
        /// </summary>
        bool Enabled { get; set; }
        
        /// <summary>
        /// Amount of Energy needed Pr craft in Joules is Directly convertable to Watt
        /// </summary>
        long JoulesPerCraft { get; set; }
        
        /// <summary>
        /// The Required temperature for the craft to start
        /// </summary>
        int CraftTemperature { get; set; }
        
        /// <summary>
        /// Custom Attributes for the Apparatus<br/>
        /// that the recipe that the apparatus needs
        /// </summary>
        JsonObject Attributes {get; set;}
        
        /// <summary>
        /// this matches any wildcard * value to game codes.<br/>
        /// <u>Important:</u> when referencing items outside of the mod, use "domain:" on the item to map it to the right source.<br/>
        /// For Example: If one ingredient is any metal ingot from the base game, use "game:ingot-*" as the code.
        /// </summary>
        /// <param name="world"></param>
        /// <returns>Mapping of name to all allowed variants</returns>
        Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world);
        
        /// <summary>
        /// Turns Ingredients (and Outputs) into IItemStacks<br/>
        /// Also use this to process any custom recipe Attributes!
        /// </summary>
        /// <param name="world"></param>
        /// <param name="sourceForErrorLogging"></param>
        /// <returns>True if successful</returns>
        bool Resolve(IWorldAccessor world, string sourceForErrorLogging);
        
        /// <summary>
        /// Creates a copy of this recipe.
        /// </summary>
        /// <returns></returns>
        T Clone();

        /// <summary>
        /// Recipe Ingredients in any order.<br/>
        /// Typically of type CraftingRecipeIngredient unless you need custom features.
        /// </summary>
        IRecipeIngredient[] Ingredients { get; }

        /// <summary>
        /// Recipe Outputs in any order.<br/>
        /// Typically of type VERecipeVariableOuput for VE variable-output recipes.
        /// </summary>
        IRecipeOutput[] Outputs { get; }
        
        /// <summary>
        /// Determines if the given item stack matches the recipe ingredient.
        /// </summary>
        /// <param name="index">A valid index in <see cref="Ingredients"/></param>
        /// <param name="inputStack">The stack to match against the ingredient</param>
        /// <param name="checkStacksize">Whether the stack size (number of items) should be considered in the comparison</param>
        /// <returns>true if the inputStack matches</returns>
        bool SatisfiesAsIngredient(int index, ItemStack inputStack, bool checkStacksize = true);

        /// <summary>
        ///  Gets the resolved item corresponding to an input ingredient.
        /// </summary>
        /// <param name="index">A valid index in <see cref="Ingredients"/></param>
        /// <returns>resolved item, or null if the ingredient is a wildcard</returns>
        ItemStack GetResolvedInput(int index);

        /// <summary>
        ///  Gets the resolved item corresponding to a recipe output.
        /// </summary>
        /// <param name="index">A valid index in <see cref="Outputs"/></param>
        /// <returns>resolved item</returns>
        ItemStack GetResolvedOutput(int index);

    }
}