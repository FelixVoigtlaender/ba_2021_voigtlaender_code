using System;
using LayerBottom;
using UnityEngine.UI;

namespace LayerTop
{
    public class VisTab : VisLogicElement
    {
        VRTab vrTab;
        Toggle toggle;
        VisLogicElement otherVisLogicElement;
        public event Action OnHide;

        public override bool IsType(VRLogicElement vrLogicElement)
        {
            return vrLogicElement is VRTab;
        }

        public void Hide()
        {
            OnHide?.Invoke();
        }

        public override void Setup(VRLogicElement element)
        {
            base.Setup(element);
            this.vrTab = (VRTab)element;

            OnIsActiveChanged(vrTab.IsActive);
        }

        public void OnIsActiveChanged(bool value)
        {
            gameObject.SetActive(value);
            vrTab.IsActive = value;
            if(toggle)
                toggle.SetIsOnWithoutNotify(value);
        }
        public void SetToggle(Toggle toggle)
        {
            this.toggle = toggle;
            toggle.onValueChanged.AddListener(OnIsActiveChanged);
            OnIsActiveChanged(vrTab.IsActive);
        }

        public void SetOtherVisElement(VisLogicElement visProperty)
        {
            this.otherVisLogicElement = visProperty;
        }

        public override void Trigger()
        {
            if (otherVisLogicElement)
                otherVisLogicElement.Trigger();
        }
    }
}
