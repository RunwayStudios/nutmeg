using TMPro;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Base
{
    public class PlaceablesShopListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text placeableNameTextField;
        [SerializeField] private TMP_Text placeablePriceTextField;

        private int index;
        private int price;
        
        public void Initialize(int index, int price, string displayName)
        {
            this.index = index;
            this.price = price;
            placeableNameTextField.text = displayName;
            placeablePriceTextField.text = price.ToString();
        }

        private void UpdateAffordability()
        {
            // todo show whether affordable
        }

        public void StartPlacing()
        {
            BaseManager.Main.StartPlacingObject(index);
        }
    }
}
