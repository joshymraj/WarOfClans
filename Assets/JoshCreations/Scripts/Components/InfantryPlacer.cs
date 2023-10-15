using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace WarOfClans
{
    public class InfantryPlacer : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private Image infantryImage;
        private CanvasGroup canvasGroup;

        public InfantryPlacementData PlacementData
        {
            get;
            private set;
        }

        public Action<InfantryPlacer, Vector2> OnDragged
        {
            get;
            set;
        }

        public Action<InfantryPlacer> OnTapped
        {
            get;
            set;
        }

        public Action<InfantryPlacer> OnTapReleased
        {
            get;
            set;
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragged?.Invoke(this, eventData.delta);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTapped?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnTapReleased?.Invoke(this);
        }

        public void Init(InfantryPlacementData infantryPlacementData)
        {
            PlacementData = infantryPlacementData;
            infantryImage.sprite = PlacementData.infantryUI;
        }

        public void Enable()
        {
            canvasGroup.DOFade(1, 0.5f);
        }

        public void Disable()
        {
            canvasGroup.DOFade(0.25f, 0.5f);
        }
    }
}
