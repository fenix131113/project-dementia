using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Settings;
using AudioSystem;

public class SettingsManager : MonoBehaviour
{
    [Header("Slider")]
    public Slider fovSlider;
    public Slider mouseSensSlider;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    [Header("Text")]
    public TMP_Text fovValueText;
    public TMP_Text mouseSensValueText;
    public TMP_Text masterVolumeText;
    public TMP_Text sfxVolumeText;
    public TMP_Text musicVolumeText;

    [Header("Panel/Audio/Controll")]
    private CameraController cameraController;
    private Camera playerCamera;
    private AudioManager audioManager;

    private void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
        playerCamera = Camera.main;
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        UpdateSliderTexts();

        fovSlider.onValueChanged.AddListener(_ => UpdateSliderTexts());
        mouseSensSlider.onValueChanged.AddListener(_ => UpdateSliderTexts());
        masterSlider.onValueChanged.AddListener(_ => UpdateSliderTexts());
        sfxSlider.onValueChanged.AddListener(_ => UpdateSliderTexts());
        musicSlider.onValueChanged.AddListener(_ => UpdateSliderTexts());
    }

    private void UpdateSliderTexts()
    {
        fovValueText.text = Mathf.RoundToInt(fovSlider.value).ToString();
        mouseSensValueText.text = mouseSensSlider.value.ToString("0.0");
        masterVolumeText.text = Mathf.RoundToInt(masterSlider.value).ToString();
        sfxVolumeText.text = Mathf.RoundToInt(sfxSlider.value).ToString();
        musicVolumeText.text = Mathf.RoundToInt(musicSlider.value).ToString();
    }

    public void ApplySettings()
    {
        var settings = new SettingsData
        {
            cameraFOV = fovSlider.value,
            mouseSensitivity = mouseSensSlider.value,
            masterVolume = masterSlider.value,
            sfxVolume = sfxSlider.value,
            musicVolume = musicSlider.value
        };

        cameraController?.UpdateSensitivity(settings.mouseSensitivity);
        if (playerCamera != null)
            playerCamera.fieldOfView = settings.cameraFOV;

        audioManager?.UpdateVolumes(settings);
    }
}
