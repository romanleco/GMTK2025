using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TMP_Text[] _resourcesTexts = new TMP_Text[4];
    [SerializeField] private Image _loopTimerImage;
    [Space(10)][Header("Zone Menu")]
    [SerializeField] private GameObject _zoneMenu;
    [SerializeField] private Image _zoneIcon;
    [SerializeField] private TMP_Text _zoneNameText;
    [SerializeField] private Image _tribeColorImage;
    [SerializeField] private TMP_Text _zoneLevelText;
    [Header("Zone Upgrade")]
    [SerializeField] private GameObject _upgradeSection;
    [SerializeField] private Button _upgradeZoneButton;
    [SerializeField] private Image _resourceNeededForUpgradeIcon;
    [SerializeField] private TMP_Text _resourceNeededForUpgradeQuantityText;
    [Header("Resources Generated")]
    [SerializeField] private GameObject _resourcesGeneratedSection;
    [SerializeField] private Image _resourceGeneratedIcon;
    [SerializeField] private TMP_Text _resourceGeneratedQuantity;
    [Header("Units Generation")]
    [SerializeField] private GameObject _unitsGenerationSection;
    [SerializeField] private Button _generateUnitButton;
    [SerializeField] private TMP_Text _generatedUnitsText;
    [Header("Units In Zone")]
    [SerializeField] private TMP_Text _unitsInZoneText;
    [SerializeField] private Button[] _unitsButtons = new Button[8];
    [Space(10)]
    [Header("Sprite References")]
    [SerializeField] private Sprite[] _resourceIconSprites = new Sprite[4];
    [SerializeField] private Color[] _resourceIconColors = new Color[4];
    private Zone _selectedZone;


    void Start()
    {
        int buttonIndex = 0;
        foreach (Button uB in _unitsButtons)
        {
            int assignementInt = buttonIndex; //The reference to the local variable gets lost so it doesn't update the rest of the buttons
            uB.onClick.AddListener(() => UnitButton(assignementInt));
            buttonIndex++;
        }

        _upgradeZoneButton.onClick.AddListener(() => UpgradeZoneButton());
        _generateUnitButton.onClick.AddListener(() => GenerateUnitsButton());

        CloseZoneMenu();
    }

    public void UpdateResourceCount(int resourceIndex, int value)
    {
        //0 = Wheat; 1 = Wood; 2 = Minerals; 3 = Units; 4 = Coins
        _resourcesTexts[resourceIndex].text = value.ToString();
    }

    public void DisplayZoneBaseInfo(ZoneSO zoneInfo,
                                int unitsCount,
                                int level,
                                int tribeIndex,
                                Zone zoneScr)
    {
        if (_zoneMenu.activeSelf == false) _zoneMenu.SetActive(true);

        _zoneIcon.sprite = zoneInfo.zoneIconSprite;
        _zoneNameText.text = zoneInfo.zoneName;
        _tribeColorImage.color = GameManager.Singleton.GetTribeColor(tribeIndex);

        _zoneLevelText.text = "Lvl. " + (level + 1).ToString() + "/" + zoneInfo.maximumUnitsPerLevel.Length.ToString();
        _upgradeSection.SetActive(level + 1 < zoneInfo.maximumUnitsPerLevel.Length);

        _resourceNeededForUpgradeIcon.sprite = _resourceIconSprites[zoneInfo.resourceNeededForUpgrade[level]];
        _resourceNeededForUpgradeIcon.color = _resourceIconColors[zoneInfo.resourceNeededForUpgrade[level]];
        _resourceNeededForUpgradeQuantityText.text = "x" + zoneInfo.resourceQuantNeededForUpgrade[level].ToString();

        _unitsInZoneText.text = "Units " + unitsCount.ToString() + "/" + zoneInfo.maximumUnitsPerLevel[level];

        for (int i = 0; i < _unitsButtons.Length; i++)
        {
            _unitsButtons[i].gameObject.SetActive(i + 1 <= unitsCount);
        }

        _selectedZone = zoneScr;
    }

    public void DisplayResourceGeneratingZoneInfo(int resourceGeneratedIndex, int resourceGeneratedQuantity)
    {
        if (_unitsGenerationSection.activeSelf) _unitsGenerationSection.SetActive(false);
        if (_resourcesGeneratedSection.activeSelf == false) _resourcesGeneratedSection.SetActive(true);

        _resourceGeneratedIcon.sprite = _resourceIconSprites[resourceGeneratedIndex];
        _resourceGeneratedIcon.color = _resourceIconColors[resourceGeneratedIndex];
        _resourceGeneratedQuantity.text = "x" + resourceGeneratedQuantity.ToString(); 
    }

    public void DisplaySettlementZoneInfo(int unitsGeneratedCount, int maxUnitsToGenerate)
    {
        if (_resourcesGeneratedSection.activeSelf) _resourcesGeneratedSection.SetActive(false);
        if (_unitsGenerationSection.activeSelf == false) _unitsGenerationSection.SetActive(true);

        _generatedUnitsText.text = unitsGeneratedCount.ToString() + "/" + maxUnitsToGenerate.ToString();
    }

    public void RefreshZoneUI()
    {
        if (_selectedZone != null) _selectedZone.Select();
    }

    private void UnitButton(int buttonIndex)
    {
        _selectedZone.SelectUnit(buttonIndex);
    }

    private void UpgradeZoneButton() => _selectedZone.UpgradeZone(GameManager.PLAYER_TRIBE_INDEX);

    private void GenerateUnitsButton()
    {
        ZSettlement settlementZoneScr = _selectedZone.GetComponent<ZSettlement>();
        if (settlementZoneScr != null)
        {
            settlementZoneScr.GenerateUnit(GameManager.PLAYER_TRIBE_INDEX);
        }
    }

    public void CloseZoneMenu() => _zoneMenu.SetActive(false);
    public void UpdateLoopCompletion(float completionPercentage) => _loopTimerImage.fillAmount = completionPercentage;
}
