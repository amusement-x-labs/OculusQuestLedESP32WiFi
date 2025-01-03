using Oculus.Interaction;
using UnityEngine;

namespace Core.Scripts
{
    public class ControlMenuHandler : MonoBehaviour
    {
        [SerializeField] private PointableUnityEventWrapper turnOnButton;
        [SerializeField] private PointableUnityEventWrapper turnOffButton;
        [Space(20)]
        [SerializeField] private WebBasedController ctrl;
        [SerializeField] private MeshRenderer mesh;
        [Space(20)]
        [SerializeField] private Color onColor = Color.red;
        [SerializeField] private Color offColor = new Color(0x76, 0x76, 0x76);

        private void Start()
        {
            mesh.materials[0].color = offColor;
            ctrl.OnLedStatusChanged += LedStatusChanged;

            turnOnButton.WhenRelease.AddListener(TurnOnHandler);
            turnOffButton.WhenRelease.AddListener(TurnOffHandler);
        }

        private void OnDestroy()
        {
            ctrl.OnLedStatusChanged -= LedStatusChanged;

            turnOnButton.WhenRelease.RemoveListener(TurnOnHandler);
            turnOffButton.WhenRelease.RemoveListener(TurnOffHandler);
        }

        private void LedStatusChanged(bool status)
        {
            if(mesh)
                mesh.materials[0].color = status == true ? onColor : offColor;
        }

        private void TurnOnHandler(PointerEvent arg)
        {
            if(ctrl)
                ctrl.TurnLedOnButton();
        }

        private void TurnOffHandler(PointerEvent arg)
        {
            if (ctrl)
                ctrl.TurnLedOffButton();
        }
    }
}
