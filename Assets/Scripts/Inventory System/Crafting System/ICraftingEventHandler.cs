// Interfejs do obsługi zdarzeń związanych z craftingiem
public interface ICraftingEventHandler
{
    void OnCraftingSuccess(CraftingRecipe recipe); // sucses
    void OnCraftingFailure(CraftingRecipe recipe); // fa
}