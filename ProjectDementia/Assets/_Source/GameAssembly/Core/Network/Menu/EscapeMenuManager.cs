using Core.Initializing;
using Photon.Pun;
using Player;
using UnityEngine;
using VContainer;

public class EscapeMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject escapePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmationPanel;

    [Inject] private PlayerObjects _players;

    private PlayerController playerController;
    private bool isMenuOpen = false;

    private void Start()
    {
        playerController = _players.PlayerObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (confirmationPanel.activeSelf) return;

            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        escapePanel.SetActive(isMenuOpen);

        if (playerController != null)
            playerController.Controll = !isMenuOpen;

        if (isMenuOpen)
            ShowCursor();
        else
            HideCursor();
    }

    public void OnContinueClicked()
    {
        isMenuOpen = false;
        escapePanel.SetActive(false);

        if (playerController != null)
            playerController.Controll = true;

        HideCursor();
    }

    public void OnSettingsClicked() => settingsPanel.SetActive(true);
    public void OnApplyClicked() => confirmationPanel.SetActive(true);
    public void OnConfirmApplyClicked()
    {
        confirmationPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void OnBackFromSettings() => settingsPanel.SetActive(false);
    public void OnExitToMenuClicked()
    {
        Time.timeScale = 1f;
        PhotonNetwork.LeaveRoom();
    }
    public void OnExitGameClicked() => Application.Quit();

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}