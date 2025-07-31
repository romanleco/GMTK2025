using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TMP_Text[] _resourcesTexts = new TMP_Text[5];
    [SerializeField] private Image _loopTimerImage;
    public void UpdateResourceCount(int resourceIndex, int value)
    {
        //0 = Wheat; 1 = Wood; 2 = Minerals; 3 = Units; 4 = Coins
        _resourcesTexts[resourceIndex].text = value.ToString();
    }

    public void UpdateLoopCompletion(float completionPercentage) => _loopTimerImage.fillAmount = completionPercentage;
}
