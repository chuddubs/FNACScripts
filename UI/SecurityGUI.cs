using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SecurityGUI : MonoBehaviour
{
    public GameObject camerasPanel;
    public GameObject ventsCamerasPanel;

    [SerializeField]
    private TextMeshProUGUI camNameDispay;

    [SerializeField]
    private GameObject genDoorButton;
    [SerializeField]
    private GameObject buttonOpen;
    [SerializeField]
    private GameObject buttonClose;
    public Button      firstSelected;
    private Button     currentSelected;
    public Button      firstSelected_vents;
    private Button     currentSelected_vents;
    private bool      showVents;
    public int     lastReg = 13;
    public int     lastVent = 11;

    private List<int>     ventIds => new List<int>{8, 9, 10, 11, 15};
    private Color normal = new Color(1f/255f, 1f/255f, 1f/255f, 0.1f);
    private Color selected = new Color(240f/255f, 240f/255f, 240f/255f, 130f/255f);

    private void Awake()
    {
        showVents = false;
        ViewManager.Instance.onCamSwitched += OnCamChanged;
        currentSelected = firstSelected;
        currentSelected_vents = firstSelected_vents;
        Highlight_Reg(firstSelected);
        Highlight_Vent(firstSelected_vents);
    }

    public void Highlight_Reg(Button clicked)
    {
        currentSelected.image.color = normal;
        clicked.image.color = selected;
        currentSelected = clicked;
    }

    public void Highlight_Vent(Button clicked)
    {
        currentSelected_vents.image.color = normal;
        clicked.image.color = selected;
        currentSelected_vents = clicked;
    }

    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;
        ViewManager.Instance.onCamSwitched -= OnCamChanged;
    }

    public void TogglePanels()
    {
        showVents = !showVents;
        camerasPanel.SetActive(!showVents);
        ventsCamerasPanel.SetActive(showVents);
        ViewManager.Instance.SetViewCam(showVents ? lastVent : lastReg);
    }

    public void ToggleGenDoorButton(bool open)
    {
        buttonOpen.SetActive(!open);
        buttonClose.SetActive(open);
    }

    private void OnCamChanged(int id)
    {
        if (id > 0)
        {
            camNameDispay.text = SoyGameController.Instance.IsCaca() ? FNACStatic.CamIdToCacaDisplayName[id] : FNACStatic.CamIdToDisplayName[id];
            if (ventIds.Contains(id))
                lastVent = id;
            else 
                lastReg = id;
        }
        genDoorButton.SetActive(id == 3);
    }
}
