using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace InventorySystem
{
    public class InventorySwitcher : ITickable
    {
        private const float SWITCH_COOLDOWN = 0.15f;
        private readonly ItemSelector _itemSelector;

        private bool _canSwitch = true;
        private float _cooldownTimer = SWITCH_COOLDOWN;

        [Inject]
        public InventorySwitcher(ItemSelector itemSelector) => _itemSelector = itemSelector;

        public void Tick()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                SwitchLeft();
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                SwitchRight();

            if (!_canSwitch)
            {
                _cooldownTimer -= Time.deltaTime;
                if(_cooldownTimer <= 0)
                    _canSwitch = true;
            }
        }

        public void SwitchLeft()
        {
            if(!_canSwitch)
                return;
            
            _itemSelector.SelectItemPrevious();
            _cooldownTimer = SWITCH_COOLDOWN;
            _canSwitch = false;
        }

        public void SwitchRight()
        {
            if(!_canSwitch)
                return;
            
            _itemSelector.SelectItemNext();
            _cooldownTimer = SWITCH_COOLDOWN;
            _canSwitch = false;
        }
    }
}