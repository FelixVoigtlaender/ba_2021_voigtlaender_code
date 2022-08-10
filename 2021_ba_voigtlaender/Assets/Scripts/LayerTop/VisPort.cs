using LayerBottom;
using UnityEngine;
using UnityEngine.UI;

namespace LayerTop
{
    public class VisPort : MonoBehaviour
    {
        public VRPort vrPort;
        public Image image;
        public TooltipContent tooltipContent;
        public void Setup(VRPort vrPort)
        {
            this.vrPort = vrPort;
            image.color = vrPort.dataType.GetColor();

            tooltipContent.description = vrPort.toolTip;
        }
    }
}
