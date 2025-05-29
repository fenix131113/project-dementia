using Photon.Pun;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core.Network.Menu
{
    public class SettingsManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject buttonsPanel;
        public GameObject settingsPanel;
        public GameObject confirmPanel;

        [Header("Buttons")]
        public Button saveSettingsButton;
        public Button exitToMenuButton;
        public Button exitGameButton;
        public Button applySettingsButton;
        public Button continueButton;

        [Header("Sliders")]
        public Slider fovSlider;
        public Slider sensitivitySlider;
        public Slider overallVolumeSlider;
        public Slider soundVolumeSlider;
        public Slider musicVolumeSlider;

        [Header("Slider Values")]
        public TMP_Text fovValueText;
        public TMP_Text sensitivityValueText;
        public TMP_Text overallVolumeValueText;
        public TMP_Text soundVolumeValueText;
        public TMP_Text musicVolumeValueText;

        private Camera mainCamera;
        private CameraController cameraController;
        private Player.PlayerController playerController;

        private const string FOV_KEY = "FOV";
        private const string SENSITIVITY_KEY = "Sensitivity";
        private const string OVERALL_VOLUME_KEY = "OverallVolume";
        private const string SOUND_VOLUME_KEY = "SoundVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";

        private void Start()
        {
            mainCamera = Camera.main;
            cameraController = FindObjectOfType<CameraController>();
            playerController = FindObjectOfType<Player.PlayerController>();

            LoadSettingsToMemory();
            LoadSlidersFromMemory();

            saveSettingsButton.onClick.AddListener(OnSaveSettingsClicked);
            exitToMenuButton.onClick.AddListener(OnExitToMenuButtonClicked);
            exitGameButton.onClick.AddListener(OnExitGameButtonClicked);
            applySettingsButton.onClick.AddListener(OnApplyConfirmed);
            continueButton.onClick.AddListener(OnContinueButtonClicked);

            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivityInRealTime);
            fovSlider.onValueChanged.AddListener(value => fovValueText.text = Mathf.RoundToInt(value).ToString());
            overallVolumeSlider.onValueChanged.AddListener(value => overallVolumeValueText.text = Mathf.RoundToInt(value).ToString());
            soundVolumeSlider.onValueChanged.AddListener(value => soundVolumeValueText.text = Mathf.RoundToInt(value).ToString());
            musicVolumeSlider.onValueChanged.AddListener(value => musicVolumeValueText.text = Mathf.RoundToInt(value).ToString());

            SetAllPanels(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (settingsPanel.activeSelf || confirmPanel.activeSelf)
                {
                    SetAllPanels(false);
                    buttonsPanel.SetActive(true);
                    EnableMainButtonsOnly();
                    UpdatePlayerControl(false);
                }
                else if (buttonsPanel.activeSelf)
                {
                    buttonsPanel.SetActive(false);
                    UpdatePlayerControl(true);
                }
                else
                {
                    buttonsPanel.SetActive(true);
                    EnableMainButtonsOnly();
                    UpdatePlayerControl(false);
                }
            }
        }

        private void SetAllPanels(bool state)
        {
            buttonsPanel.SetActive(state);
            settingsPanel.SetActive(state);
            confirmPanel.SetActive(state);
            UpdatePlayerControl(!state);
        }

        private void EnableMainButtonsOnly()
        {
            saveSettingsButton.gameObject.SetActive(true);
            exitToMenuButton.gameObject.SetActive(true);
            exitGameButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);

            settingsPanel.SetActive(false);
            confirmPanel.SetActive(false);
        }

        private void UpdatePlayerControl(bool enabled)
        {
            if (playerController != null)
                playerController.Controll = enabled;
        }

        private float GetValueOrDefault(string key, float defaultValue)
        {
            return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
        }

        private void LoadSettingsToMemory()
        {
            fovSlider.value = PlayerPrefs.HasKey(FOV_KEY) ? PlayerPrefs.GetFloat(FOV_KEY) : fovSlider.value;
            sensitivitySlider.value = PlayerPrefs.HasKey(SENSITIVITY_KEY) ? PlayerPrefs.GetFloat(SENSITIVITY_KEY) : sensitivitySlider.value;
            overallVolumeSlider.value = PlayerPrefs.GetFloat(OVERALL_VOLUME_KEY, 100f);
            soundVolumeSlider.value = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 100f);
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 100f);
        }


        private void LoadSlidersFromMemory()
        {
            fovValueText.text = Mathf.RoundToInt(fovSlider.value).ToString();
            sensitivityValueText.text = Mathf.RoundToInt(sensitivitySlider.value).ToString();
            overallVolumeValueText.text = Mathf.RoundToInt(overallVolumeSlider.value).ToString();
            soundVolumeValueText.text = Mathf.RoundToInt(soundVolumeSlider.value).ToString();
            musicVolumeValueText.text = Mathf.RoundToInt(musicVolumeSlider.value).ToString();
        }

        private void ApplySettings()
        {
            if (mainCamera != null)
            {
                float scaledFOV = Mathf.Lerp(60f, 120f, fovSlider.value / 100f);
                mainCamera.fieldOfView = scaledFOV;
            }

            if (cameraController != null)
            {
                float scaledSensitivity = Mathf.Lerp(0.1f, 10f, sensitivitySlider.value / 100f);
                cameraController.UpdateSensitivity(scaledSensitivity);
            }

            AudioListener.volume = overallVolumeSlider.value / 100f;
        }

        private void UpdateSensitivityInRealTime(float newValue)
        {
            sensitivityValueText.text = Mathf.RoundToInt(newValue).ToString();

            if (cameraController != null)
            {
                float scaled = Mathf.Lerp(0.1f, 10f, newValue / 100f);
                cameraController.UpdateSensitivity(scaled);
            }
        }

        private void OnSaveSettingsClicked()
        {
            confirmPanel.SetActive(true);
        }

        public void OnApplyConfirmed()
        {
            PlayerPrefs.SetFloat(FOV_KEY, fovSlider.value);
            PlayerPrefs.SetFloat(SENSITIVITY_KEY, sensitivitySlider.value);
            PlayerPrefs.SetFloat(OVERALL_VOLUME_KEY, overallVolumeSlider.value);
            PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, soundVolumeSlider.value);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolumeSlider.value);
            PlayerPrefs.Save();

            ApplySettings();

            confirmPanel.SetActive(false);
            settingsPanel.SetActive(true);
            buttonsPanel.SetActive(false);
        }

        public void OnCancelConfirmed()
        {
            LoadSettingsToMemory();
            LoadSlidersFromMemory();
            ApplySettings();
            confirmPanel.SetActive(false);
        }

        private void OnContinueButtonClicked()
        {
            buttonsPanel.SetActive(false);
            UpdatePlayerControl(true);
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnExitToMenuButtonClicked()
        {
            Time.timeScale = 1f;
            PhotonNetwork.LeaveRoom();
        }

        private void OnExitGameButtonClicked()
        {
            Application.Quit();
        }
    }
}