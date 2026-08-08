using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.Common;

namespace AlchemicalRefinement.RecipeSystem.Recipes
{
    public class RecipeCalcinator : IByteSerializable, IApparatusRecipeBase<RecipeCalcinator>
    {

        public int RecipeID;
        public void ToBytes(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            throw new System.NotImplementedException();
        }

        public AssetLocation Name { get; set; }
        public string Code { get; set; }
        public bool Enabled { get; set; } = true;
        public long JoulesPerCraft { get; set; }
        public int CraftTemperatureMin { get; set; }
        public int CraftTemperatureMax { get; set; }
        [JsonProperty]
        [JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes { get; set; }
        
        public CraftingRecipeIngredient[] Ingredients;
        //public ARRecipeVariableOutput[] Outputs;
        public RecipeVariableOutput[] Outputs;

        IRecipeIngredient[] IApparatusRecipeBase<RecipeCalcinator>.Ingredients
        {
            get { return Ingredients; }
        }

        IRecipeOutput[] IApparatusRecipeBase<RecipeCalcinator>.Outputs
        {
            get { return Outputs; }
        }
        
        public Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world)
        {
            throw new System.NotImplementedException();
        }

        public bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
        {
            throw new System.NotImplementedException();
        }

        public RecipeCalcinator Clone()
        {
            throw new System.NotImplementedException();
        }

        //public IRecipeIngredient[] Ingredients { get; }
        
        public bool SatisfiesAsIngredient(int index, ItemStack inputStack, bool checkStacksize = true)
        {
            throw new System.NotImplementedException();
        }

        public ItemStack GetResolvedInput(int index)
        {
            throw new System.NotImplementedException();
        }

        public ItemStack GetResolvedOutput(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}