using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private MenuFadeController menuFadeController;
    [SerializeField] private string sceneToLoad = "Forest_1";

    private bool CheckTitleIdConfigured()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            SetStatus("PlayFab non configurato: imposta il Title ID in PlayFabSharedSettings.");
            return false;
        }
        return true;
    }

    private bool TryGetCredentials(out string email, out string password)
    {
        email = emailInput != null ? emailInput.text.Trim() : "";
        password = passwordInput != null ? passwordInput.text : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("Inserisci email e password.");
            return false;
        }
        return true;
    }

    public void OnLoginClicked()
    {
        if (!CheckTitleIdConfigured()) return;
        if (!TryGetCredentials(out string email, out string password)) return;

        SetInteractable(false);
        SetStatus("Accesso in corso...");

        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        }, OnLoginSuccess, OnLoginError);
    }

    public void OnRegisterClicked()
    {
        if (!CheckTitleIdConfigured()) return;
        if (!TryGetCredentials(out string email, out string password)) return;

        if (password.Length < 6)
        {
            SetStatus("La password deve avere almeno 6 caratteri.");
            return;
        }

        SetInteractable(false);
        SetStatus("Registrazione in corso...");

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        }, OnRegisterSuccess, OnRegisterError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        SetStatus("Accesso riuscito!");
        if (menuFadeController != null)
        {
            menuFadeController.CallFadeAndStartGame(sceneToLoad);
        }
    }

    private void OnLoginError(PlayFabError error)
    {
        SetInteractable(true);
        SetStatus("Accesso non riuscito: email o password errati, oppure utente non registrato.");
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        SetInteractable(true);
        SetStatus("Registrazione completata! Ora puoi effettuare il login.");
    }

    private void OnRegisterError(PlayFabError error)
    {
        SetInteractable(true);

        if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable || error.Error == PlayFabErrorCode.UsernameNotAvailable)
        {
            SetStatus("Utente gia registrato. Effettua il login.");
        }
        else
        {
            SetStatus("Registrazione non riuscita: " + error.ErrorMessage);
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }

    private void SetInteractable(bool value)
    {
        if (loginButton != null) loginButton.interactable = value;
        if (registerButton != null) registerButton.interactable = value;
    }
}
