// UnityPlayerAccountsAuth.cs
// Place this in Assets/Scripts/Auth/

using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts; // PlayerAccountService APIs
using UnityEngine.SceneManagement;

/// <summary>
/// Handles sign-in with Unity Player Accounts and then signs into Unity Authentication
/// using the Player Account access token. Also supports linking/unlinking and sign-out.
/// Follows Unity docs flow: StartSignInAsync() -> listen for PlayerAccountService.SignedIn -> SignInWithUnityAsync(accessToken)
/// Docs: https://docs.unity.com/ugs/manual/authentication/manual/unity-player-accounts
/// </summary>
public class PlayerAuthentication : MonoBehaviour
{
    public static PlayerAuthentication Instance { get; private set; }

    // Events you can hook from UI
    public event Action OnSignedIn;               // Fired after successful auth with Unity Authentication
    public event Action<string> OnSignInFailed;   // Fired on error (error message)
    public event Action OnSignedOut;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await InitializeServices();
    }

    private async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("[Auth] Unity Services initialized.");

            // Register to Player Accounts SignedIn event **after** services are initialized.
            PlayerAccountService.Instance.SignedIn += OnPlayerAccountsSignedIn;
            //await StartPlayerAccountsSignInAsync();
        }
        catch (Exception e)
        {
            Debug.LogError("[Auth] Failed to initialize Unity Services: " + e);
            OnSignInFailed?.Invoke(e.Message);
        }
    }
    public void signInWithUnityAuth()
    {
        // Call the async version and wait for it to complete
        StartPlayerAccountsSignInAsync().ConfigureAwait(false);
    }
    /// <summary>
    /// Starts the Player Accounts browser sign-in flow.
    /// This will open the system browser and prompt the player to sign in.
    /// If the player is already signed into Player Accounts, SignInWithUnityAuthAsync() will be called directly.
    /// </summary>
    public async Task StartPlayerAccountsSignInAsync()
    {
        // If Player Accounts is already signed in, just sign into Unity Auth with the existing token.
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            Debug.Log("[Auth] PlayerAccountService already signed in — proceeding to Unity Auth.");
            await SignInWithUnityAuthAsync();
            return;
        }

        try
        {
            Debug.Log("[Auth] Starting Player Accounts sign-in (opens browser)...");
            await PlayerAccountService.Instance.StartSignInAsync();
            // The SignedIn event will fire after the browser flow completes.
        }
        catch (PlayerAccountsException ex)
        {
            Debug.LogException(ex);
            OnSignInFailed?.Invoke($"PlayerAccounts error: {ex.Message}");
        }
        catch (RequestFailedException rf)
        {
            Debug.LogException(rf);
            OnSignInFailed?.Invoke($"Request error: {rf.Message}");
        }
    }

    /// <summary>
    /// Callback: Player Accounts has signed in (browser returned with credentials).
    /// Proceed to sign in to Unity Authentication using the Player Accounts access token.
    /// </summary>
    private async void OnPlayerAccountsSignedIn()
    {
        Debug.Log("[Auth] Player Accounts SignedIn event received.");
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Uses PlayerAccountService.Instance.AccessToken to authenticate with Unity Authentication.
    /// </summary>
    
    public async Task SignInWithUnityAuthAsync()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                Debug.LogError("[Auth] No Player Accounts access token available.");
                OnSignInFailed?.Invoke("No access token found.");
                return;
            }

            Debug.Log("[Auth] Signing in with Unity Authentication using Player Accounts token...");
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            Debug.Log($"[Auth] Signed in to Unity Authentication. PlayerId: {AuthenticationService.Instance.PlayerId}");
            OnSignedIn?.Invoke();
        }
        catch (AuthenticationException authEx)
        {
            Debug.LogException(authEx);
            OnSignInFailed?.Invoke($"Authentication failed: {authEx.Message}");
        }
        catch (RequestFailedException rf)
        {
            Debug.LogException(rf);
            OnSignInFailed?.Invoke($"Request failed: {rf.Message}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnSignInFailed?.Invoke($"Unexpected error: {e.Message}");
        }
    }

    /// <summary>
    /// Link currently-signed-in Unity Authentication player with Player Accounts (useful when upgrading anonymous to account).
    /// Call StartPlayerAccountsSignInAsync() first to obtain Player Accounts access token, then call this to link.
    /// </summary>
    public async Task LinkWithUnityAsync()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                OnSignInFailed?.Invoke("No access token available to link.");
                return;
            }

            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Debug.Log("[Auth] LinkWithUnityAsync successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogError("[Auth] Account already linked to another player.");
            OnSignInFailed?.Invoke("Account already linked to another player.");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnSignInFailed?.Invoke(e.Message);
        }
    }

    /// <summary>
    /// Unlink the Player Accounts identity from the current Unity Authentication player.
    /// After unlink, if no other identity is linked, the player will become anonymous.
    /// </summary>
    public async Task UnlinkUnityAsync()
    {
        try
        {
            await AuthenticationService.Instance.UnlinkUnityAsync();
            Debug.Log("[Auth] UnlinkUnityAsync successful.");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnSignInFailed?.Invoke(e.Message);
        }
    }

    /// <summary>
    /// Sign out from both Unity Authentication and Player Accounts (optionally clear session token).
    /// </summary>
    /// <param name="clearSessionToken">If true, clears the local session token for Unity Authentication.</param>
    public void SignOut(bool clearSessionToken = false)
    {
        // Sign out of Unity Authentication (optionally clear session token)
        AuthenticationService.Instance.SignOut(clearSessionToken);

        // Sign out of Player Accounts (no session token persistence on PlayerAccountService)
        PlayerAccountService.Instance.SignOut();

        Debug.Log("[Auth] Signed out of Authentication and Player Accounts.");
        OnSignedOut?.Invoke();
    }

    private void OnDestroy()
    {
        try
        {
            if (PlayerAccountService.Instance != null)
                PlayerAccountService.Instance.SignedIn -= OnPlayerAccountsSignedIn;
        }
        catch { /* ignore if shutting down */ }
    }
}
