using System.Collections.Generic;
using DG.Tweening;
using InventorySystem.Data;
using ItemsSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace InventorySystem.View
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private float selectedSize;
        [SerializeField] private float unselectedSize;
        [SerializeField] private List<Image> itemsCells;

        private Inventory _inventory;
        private ItemSelector _itemSelector;
        private readonly List<Vector3> _startCellsPositions = new();
        private readonly List<Tween> _tweens = new();
        
        [Inject]
        private void Construct(PlayersInventory playersInventory, ItemSelector itemSelector)
        {
            _inventory = playersInventory.GetPlayerInventory(PhotonNetwork.LocalPlayer);
            _itemSelector = itemSelector;
        }

        private void Start()
        {
            Bind();

            foreach (var cell in itemsCells)
                _startCellsPositions.Add(cell.rectTransform.position);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                Debug.Log(_itemSelector.SelectedItem.ItemName);
        }

        private void OnDestroy() => Expose();

        private void MoveInventoryCells()
        {
            // Cells move
            if (_itemSelector.LastChangeSide == InventorySelectionChangeSide.RIGHT)
                MoveCellsRight();
            else
                MoveCellsLeft();
        }

        private void DrawVisibleCells()
        {
            GetCentralCell().sprite = _itemSelector.SelectedItem.Icon;
            itemsCells[GetCentralCellIndex() - 1].sprite = GetInventoryItemBySelectedOffset(-1).Icon;
            itemsCells[GetCentralCellIndex() + 1].sprite = GetInventoryItemBySelectedOffset(1).Icon;
        }

        private void DrawInvisibleCells()
        {
            itemsCells[GetCentralCellIndex() - 2].sprite = GetInventoryItemBySelectedOffset(-2).Icon;
            itemsCells[GetCentralCellIndex() + 2].sprite = GetInventoryItemBySelectedOffset(2).Icon;
        }
        
        private void MoveCellsRight()
        {
            if (_tweens.Count > 0)
                _tweens.ForEach(t => t.Kill());
            
            GetCentralCell().rectTransform.DOSizeDelta(Vector2.one * unselectedSize, animationDuration);

            var newItemsCells = new List<Image> { itemsCells[^1] };

            for (var i = 0; i < itemsCells.Count - 1; i++)
                newItemsCells.Add(itemsCells[i]);

            for (var index = 0; index < newItemsCells.Count; index++)
            {
                var cell = newItemsCells[index];

                if (index == 0)
                    cell.rectTransform.position = _startCellsPositions[0];
                else
                {
                    var tween = cell.rectTransform.DOMoveX(_startCellsPositions[index].x, animationDuration);
                    _tweens.Add(tween);
                    tween.onComplete = () => _tweens.Remove(tween);
                }
            }
            itemsCells = newItemsCells;

            GetCentralCell().rectTransform.DOSizeDelta(Vector2.one * selectedSize, animationDuration);
        }

        private void MoveCellsLeft()
        {
            if (_tweens.Count > 0)
                _tweens.ForEach(t => t.Kill());

            GetCentralCell().rectTransform.DOSizeDelta(Vector2.one * unselectedSize, animationDuration);

            var newItemsCells = new List<Image>();

            for (var i = 1; i < itemsCells.Count; i++)
                newItemsCells.Add(itemsCells[i]);
            newItemsCells.Add(itemsCells[0]);

            for (var index = 0; index < newItemsCells.Count; index++)
            {
                var cell = newItemsCells[index];

                if (index == newItemsCells.Count - 1)
                    cell.rectTransform.position = _startCellsPositions[^1];
                else
                {
                    var tween = cell.rectTransform.DOMoveX(_startCellsPositions[index].x, animationDuration);
                    _tweens.Add(tween);
                    tween.onComplete = () => _tweens.Remove(tween);
                }
            }
            
            itemsCells = newItemsCells;

            GetCentralCell().rectTransform.DOSizeDelta(Vector2.one * selectedSize, animationDuration);
        }
        
        private Item GetInventoryItemBySelectedOffset(int offset)
        {
            if (_inventory.Items.Count == 0)
                return default;

            var newIndex = (_itemSelector.SelectedItemInventoryIndex + offset) % _inventory.Items.Count;

            if (newIndex < 0)
                newIndex += _inventory.Items.Count;

            return _inventory.Items[newIndex];
        }

        private Image GetCentralCell() => itemsCells[GetCentralCellIndex()];
        private int GetCentralCellIndex() => itemsCells.Count / 2;

        private void OnInventoryItemAdded(Item item)
        {
            if (_itemSelector.SelectedItem == null)
                return;
            
            DrawVisibleCells();
        }

        private void OnInventoryItemRemoved(Item item)
        {
            if (_itemSelector.SelectedItem == null)
                return;
            
            DrawVisibleCells();
        }

        private void OnFirstItemAdded()
        {
            DrawVisibleCells();
        }

        private void Bind()
        {
            _itemSelector.OnSelectedItemChanged += MoveInventoryCells;
            _itemSelector.OnSelectedItemFirstTimeAdded += OnFirstItemAdded;
            _itemSelector.OnBeforeSelectedItemChanged += DrawInvisibleCells;
            _inventory.OnItemAdded += OnInventoryItemAdded;
            _inventory.OnItemRemoved += OnInventoryItemRemoved;
        }

        private void Expose()
        {
            _itemSelector.OnSelectedItemChanged -= MoveInventoryCells;
            _itemSelector.OnSelectedItemFirstTimeAdded -= OnFirstItemAdded;
            _itemSelector.OnBeforeSelectedItemChanged -= DrawInvisibleCells;
            _inventory.OnItemAdded -= OnInventoryItemAdded;
            _inventory.OnItemRemoved -= OnInventoryItemRemoved;
        }
    }
}