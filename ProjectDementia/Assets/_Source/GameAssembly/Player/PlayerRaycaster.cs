using DG.Tweening;
using Interactable.Base;
using TMPro;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerRaycaster : MonoBehaviour
    {
        [SerializeField] private float animCenterPointTime;
        [SerializeField] private float animHelperTextTime;
        [SerializeField] private float centerPointMultiplier;
        [SerializeField] private RectTransform centerPoint;
        [SerializeField] private TMP_Text interactHelper;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private float rayDistance;

        private float _startSize;
        private Tween _growTween;
        private Tween _backTween;
        private Tween _textAppearTween;
        private Tween _textDisappearTween;

        private void Start()
        {
            _startSize = centerPoint.sizeDelta.x;
        }

        public void Update()
        {
            if (!Physics.Raycast(transform.position, transform.forward, out var hit, rayDistance,
                    interactableLayer))
            {
                if (_backTween != null && _backTween.IsActive())
                    return;
                
                _backTween = centerPoint.DOSizeDelta(new Vector2(_startSize, _startSize), animCenterPointTime);
                if(!_growTween.IsActive())
                    _growTween.Kill();
                
                if (_textDisappearTween != null && _textDisappearTween.IsActive())
                    return;
                
                _textDisappearTween = interactHelper.DOFade(0f, animHelperTextTime);
                if(!_textAppearTween.IsActive())
                    _textAppearTween.Kill();

                return;
            }

            // If raycast object with given layer
            
            hit.transform.TryGetComponent<AInteractableObject>(out var interactable);
            
            interactHelper.text = interactable?.InteractText;

            var growScale = _startSize * centerPointMultiplier;

            if (_growTween == null || !_growTween.IsActive())
            {
                _growTween = centerPoint.DOSizeDelta(new Vector2(growScale, growScale), animCenterPointTime);
                if(!_backTween.IsActive())
                    _backTween.Kill();
            }

            if (_textAppearTween == null || !_textAppearTween.IsActive())
            {
                _textAppearTween = interactHelper.DOFade(1f, animHelperTextTime);
                if(!_textDisappearTween.IsActive())
                    _textDisappearTween.Kill();
            }

            if (!Input.GetKeyDown(KeyCode.E))
                return;

            if (hit.transform.gameObject &&
                LayerService.CheckLayersEquality(hit.transform.gameObject.layer, interactableLayer))
                interactable?.Interact();
        }
    }
}