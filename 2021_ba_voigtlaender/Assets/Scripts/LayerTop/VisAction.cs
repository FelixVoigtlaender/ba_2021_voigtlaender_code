using LayerBottom;

namespace LayerTop
{
    public class VisAction : VisLogicElement
    {
        public VRAction vrAction;
        public string actionName;
        public override bool IsType(VRLogicElement vrLogicElement)
        {
            return vrLogicElement is VRAction;
        }

        public override void Init()
        {
            if (vrAction != null)
                return;

            vrAction = VRManager.instance.InitVRAction(actionName);
            Setup(vrAction);
        }
        public override void Setup(VRLogicElement element)
        {
            base.Setup(element);
            vrAction = (VRAction)element;
        }



    }
}
