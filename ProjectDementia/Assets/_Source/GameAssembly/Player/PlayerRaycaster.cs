using DG.Tweening;
using Interactable.Base;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerRaycaster : MonoBehaviour
    {
        [SerializeField] private float animCenterPointTime;
        [SerializeField] private float centerPointMultiplier;
        [SerializeField] private RectTransform centerPoint;
        [SerializeField] private GameObject interactHelper;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private float rayDistance;

        private float _startSize;
        private Tween _growTween;
        private Tween _backTween;

        private void Start()
        {
            _startSize = centerPoint.sizeDelta.x;
        }

        public void Update()
        {
            if (!Physics.Raycast(transform.position, transform.forward, out var hit, rayDistance,
                    interactableLayer))
            {
                interactHelper.gameObject.SetActive(false);
                
                if (_backTween != null && _backTween.IsActive())
                    return;
                
                _backTween = centerPoint.DOSizeDelta(new Vector2(_startSize, _startSize), animCenterPointTime);
                if(!_growTween.IsActive())
                    _growTween.Kill();

                return;
            }

            interactHelper.gameObject.SetActive(true);

            var growScale = _startSize * centerPointMultiplier;

            if (_growTween == null || !_growTween.IsActive())
            {
                _growTween = centerPoint.DOSizeDelta(new Vector2(growScale, growScale), animCenterPointTime);
                if(!_backTween.IsActive())
                    _backTween.Kill();
            }

            if (!Input.GetKeyDown(KeyCode.E))
                return;

            if (hit.transform.gameObject &&
                LayerService.CheckLayersEquality(hit.transform.gameObject.layer, interactableLayer))
                hit.transform.GetComponent<AInteractableObject>()?.Interact();
        }
    }
}