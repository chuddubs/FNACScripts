using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button3D : MonoBehaviour
{

    public Function function;

    void OnMouseDown()
    {
        if (NightManager.Instance.inTransition || SoyGameController.Instance.inPause)
            return;
        switch (function)
        {
            case Function.deskLight:
                if (!OfficeController.Instance.flickering)
                    OfficeController.Instance.ToggleLight();
                break;
            case Function.door:
                OfficeController.Instance.OpenCloseDoor();
                break;
            case Function.ventsDoor:
                OfficeController.Instance.OpenCloseVentsDoor();
                break;
            case Function.tablet:
                FeralManager.Instance.GiveTablet();
                break;
            case Function.bowl:
                SquirrelManager.Instance.RefillBowl();
                break;
            case Function.phone:
                NightManager.Instance.StopPhoneCall();
                break;
            case Function.dada:
                if (NightManager.Instance.currNight == 4 || NightManager.Instance.currNight == 5)
                {
                    if (ViewManager.Instance.currId == 6)
                        NightManager.Instance.EndNight(false, true);
                }
                break;
        }
    }
}

public enum Function
{
    deskLight,
    door,
    ventsDoor,
    tablet,
    bowl,
    phone,
    dada
}