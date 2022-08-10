using LayerBottom;

namespace LayerTop
{
    public class VisProperty : VisLogicElement
    {
        public override bool IsType(VRLogicElement vrLogicElement)
        {
            return vrLogicElement is VRProperty;
        }

    }
}
