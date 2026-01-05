using Code.Gameplay.Player.Crafting;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Gameplay.Recipe
{
    public class RecipeTile : MonoBehaviour, IPointerDownHandler
    {
        [HideInInspector] public CraftingRecipe recipe;
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text nameText;
        private CraftingView view;
        
        public void Initialize(CraftingRecipe recipe, CraftingView view)
        {
            this.view = view;
            SetRecipe(recipe);
        }

        private void SetRecipe(CraftingRecipe recipe)
        {
            this.recipe = recipe;
            
            icon.sprite = recipe.result.preset.icon;
            nameText.text = recipe.result.preset.itemName;
        }

        public void UpdateAvailability(bool isAvailable) => 
            background.color = isAvailable ? Color.green : Color.red;

        public void OnPointerDown(PointerEventData eventData) => 
            view.Craft(recipe);
    }
}