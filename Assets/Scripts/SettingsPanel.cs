using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Slider volumeSlider;

    void Start()
    {
        closeButton?.onClick.AddListener(Close);

        // set slider to current volume
        if (volumeSlider != null)
            volumeSlider.value = AudioListener.volume;

        volumeSlider?.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    void Close()
    {
        gameObject.SetActive(false);
    }
}