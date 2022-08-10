using LayerBottom;
using UnityEngine;

namespace LayerTop
{
    public class VisRecording : MonoBehaviour
    {
        DatRecording datRecording;
        public void Setup(DatRecording datRecording)
        {
            this.datRecording = datRecording;
        }
    }
}
