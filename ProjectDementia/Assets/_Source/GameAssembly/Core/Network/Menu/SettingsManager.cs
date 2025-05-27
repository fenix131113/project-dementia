using Photon.Pun;
using Settings;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

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

        private Camera mainCamera;
        private CameraController cameraController;
        private bool isButtonsPanelActive = false;

        private const string FOV_KEY = "FOV";
        private const string SENSITIVITY_KEY = "Sensitivity";
        private const string OVERALL_VOLUME_KEY = "OverallVolume";
        private const string SOUND_VOLUME_KEY = "SoundVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";

        private float savedFov;
        private float savedSensitivity;
        private float savedOverallVolume;
        private float savedSoundVolume;
        private float savedMusicVolume;

        private void Start()
        {
            mainCamera = Camera.main;
            cameraController = FindObjectOfType<CameraController>();

            LoadSettingsToMemory();
            LoadSlidersFromMemory();
            ApplySettings();

            saveSettingsButton.onClick.AddListener(OnSaveSettingsClicked);
            exitToMenuButton.onClick.AddListener(OnExitToMenuButtonClicked);
            exitGameButton.onClick.AddListener(OnExitGameButtonClicked);
            applySettingsButton.onClick.AddListener(OnApplyConfirmed);
            continueButton.onClick.AddListener(OnContinueButtonClicked);

            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivityInRealTime);

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
                }
                else
                {
                    ToggleButtonsPanel();
                }
            }
        }

        private void SetAllPanels(bool state)
        {
            buttonsPanel.SetActive(state);
            settingsPanel.SetActive(state);
            confirmPanel.SetActive(state);
        }

        private void ToggleButtonsPanel()
        {
            isButtonsPanelActive = !buttonsPanel.activeSelf;
            buttonsPanel.SetActive(isButtonsPanelActive);

            if (isButtonsPanelActive)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
            }
        }

        private void LoadSettingsToMemory()
        {
            savedFov = PlayerPrefs.GetFloat(FOV_KEY, 75);
            savedSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, 50);
            savedOverallVolume = PlayerPrefs.GetFloat(OVERALL_VOLUME_KEY, 100);
            savedSoundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 100);
            savedMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 100);
        }

        private void LoadSlidersFromMemory()
        {
            fovSlider.value = savedFov;
            sensitivitySlider.value = savedSensitivity;
            overallVolumeSlider.value = savedOverallVolume;
            soundVolumeSlider.value = savedSoundVolume;
            musicVolumeSlider.value = savedMusicVolume;
        }

        private void ApplySettings()
        {
            if (mainCamera != null)
            {
                mainCamera.fieldOfView = savedFov;
            }

            if (cameraController != null)
            {
                cameraController.UpdateSensitivity(savedSensitivity);
            }
        }

        private void UpdateSensitivityInRealTime(float newSensitivity)
        {
            if (cameraController != null)
            {
                cameraController.UpdateSensitivity(newSensitivity);
            }
        }

        private void OnSaveSettingsClicked()
        {
            confirmPanel.SetActive(true);
        }

        public void OnApplyConfirmed()
        {
            savedFov = fovSlider.value;
            savedSensitivity = sensitivitySlider.value;
            savedOverallVolume = overallVolumeSlider.value;
            savedSoundVolume = soundVolumeSlider.value;
            savedMusicVolume = musicVolumeSlider.value;

            PlayerPrefs.SetFloat(FOV_KEY, savedFov);
            PlayerPrefs.SetFloat(SENSITIVITY_KEY, savedSensitivity);
            PlayerPrefs.SetFloat(OVERALL_VOLUME_KEY, savedOverallVolume);
            PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, savedSoundVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, savedMusicVolume);
            PlayerPrefs.Save();

            ApplySettings();

            confirmPanel.SetActive(false);
        }

        public void OnCancelConfirmed()
        {
            LoadSlidersFromMemory();
            ApplySettings();
            confirmPanel.SetActive(false);
        }

        private void OnContinueButtonClicked()
        {
            buttonsPanel.SetActive(false);
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