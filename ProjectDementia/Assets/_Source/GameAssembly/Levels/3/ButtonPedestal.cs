using System.Collections;
using Interactable.Custom.ClickInteractions;
using UnityEngine;

namespace Levels._3
{
    public class ButtonPedestal : MonoBehaviour
    {
        public PartFigurines CurrentFigurines { get; private set; }

        [SerializeField] private ClickableButton button;
        [SerializeField] private ItemObjectPlaceZone itemPlaceZone;
        [SerializeField] private float clickCooldown = 0.25f;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnPedestalButtonClicked() // Invokes locally
        {
            if(!CurrentFigurines)
                return;
            
            button.SetInteractable(false);
            CurrentFigurines.SwitchPartPosition_RPC();
            StartCoroutine(ClickCooldown());
        }

        private void OnObjectPlacedToPedestal(PickableItem item)
        {
            CurrentFigurines = item.GetComponent<PartFigurines>();
            item.OnInteract += OnCurrentItemTaken;
        }

        private void OnCurrentItemTaken()
        {
            CurrentFigurines.GetComponent<PickableItem>().OnInteract -= OnCurrentItemTaken;
            CurrentFigurines = null;
        }
        
        private void Bind()
        {
            button.OnInteract += OnPedestalButtonClicked;
            itemPlaceZone.OnObjectPlaced += OnObjectPlacedToPedestal;
        }

        private void Expose()
        {
            button.OnInteract -= OnPedestalButtonClicked;
            itemPlaceZone.OnObjectPlaced -= OnObjectPlacedToPedestal;
        }

        private IEnumerator ClickCooldown()
        {
            yield return new WaitForSeconds(clickCooldown);
            
            button.SetInteractable(true);
        }
    }
}