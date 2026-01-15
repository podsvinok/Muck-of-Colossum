using Code.Gameplay.Player.Crafting;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.Gameplay.Recipe
{
    public class RecipeTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector] public CraftingRecipe recipe;
        
        [SerializeField] private Image icon;
        [SerializeField] private Image selectedFrame;
        [SerializeField] private Image availableVignette;
        [SerializeField] private Color unavailableColor;
        [SerializeField] private TMP_Text quantityText;
        
        private CraftingView view;

        public void Initialize(CraftingRecipe recipe, CraftingView view)
        {
            this.view = view;
            SetRecipe(recipe);
        }

        private void SetRecipe(CraftingRecipe recipe)
        {
            this.recipe = recipe;

            quantityText.text = recipe.result.quantity.ToString();
            icon.sprite = recipe.result.preset.icon;
            icon.color = Color.white;
        }

        public void UpdateAvailability(bool isAvailable) => 
            availableVignette.color = isAvailable ? Color.clear :  unavailableColor;

        public void OnPointerDown(PointerEventData eventData) => 
            view.Craft(recipe);

        public void OnPointerEnter(PointerEventData eventData)
        {
            selectedFrame.color = Color.white;
            view.OnTilePointEnter(recipe);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            selectedFrame.color = Color.clear;
            view.OnTilePointExit();
        }
    }
}