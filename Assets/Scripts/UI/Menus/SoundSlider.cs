using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    /// <summary>
    /// Slider which changes volume.
    /// </summary>
    private Slider _volumeSlider;

    private void Start()
    {
        _volumeSlider = GetComponent<Slider>();

        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    /// <summary>
    /// Called to change the volume value.
    /// </summary>
    public void ChangeVolume()
    {
        AudioListener.volume = _volumeSlider.value;
        Save();
    }

    /// <summary>
    /// Called to load the volume value saved.
    /// </summary>
    private void Load()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    /// <summary>
    /// Called to save the new volume value.
    /// </summary>
    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", _volumeSlider.value);
    }
}
