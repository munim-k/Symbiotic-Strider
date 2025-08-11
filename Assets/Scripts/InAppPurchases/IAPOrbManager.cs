using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine.Purchasing;

[Serializable]
public class PlayerPurchases
{
    public bool adsRemoved;
    public List<string> ownedSkins = new List<string>();
}

public class IAPOrbManager : MonoBehaviour
{
    public const string REMOVE_ADS = "REMOVE_ADS";
    public const string SKIN_RED = "SKIN_RED_ORB";
    public const string SKIN_BLUE = "SKIN_BLUE_ORB";

    private StoreController storeController;
    private PlayerPurchases purchases = new PlayerPurchases();

    async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await LoadPurchases();
        await InitializeIAP();
    }

    async System.Threading.Tasks.Task InitializeIAP()
    {
        storeController = UnityIAPServices.StoreController();

        storeController.OnPurchasePending += OnPurchasePending;
        storeController.OnPurchaseFailed += OnPurchaseFailed;
        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnPurchasesFetched += OnPurchasesFetched;

        await storeController.Connect();

        var productList = new List<ProductDefinition>
        {
            new ProductDefinition(REMOVE_ADS, ProductType.NonConsumable),
            new ProductDefinition(SKIN_RED, ProductType.NonConsumable),
            new ProductDefinition(SKIN_BLUE, ProductType.NonConsumable)
        };

        storeController.FetchProducts(productList);
    }

    void OnProductsFetched(List<Product> products)
    {
        Debug.Log("Products fetched.");
        storeController.FetchPurchases();
    }

    void OnPurchasesFetched(Orders orders)
    {
        foreach (var order in orders.ConfirmedOrders)
        {
            string id = order.Info.PurchasedProductInfo[0].productId;
            if (id == REMOVE_ADS)
                purchases.adsRemoved = true;
            else if (id.StartsWith("skin_") && !purchases.ownedSkins.Contains(id))
                purchases.ownedSkins.Add(id);
        }

        SavePurchases();
    }

    void OnPurchasePending(PendingOrder order)
    {
        string id = order.Info.PurchasedProductInfo[0].productId;
        Debug.Log($"Purchase pending: {id}");

        if (id == REMOVE_ADS)
            purchases.adsRemoved = true;
        else if (id.StartsWith("skin_") && !purchases.ownedSkins.Contains(id))
            purchases.ownedSkins.Add(id);

        SavePurchases();
        storeController.ConfirmPurchase(order);
    }

    void OnPurchaseFailed(FailedOrder order)
    {
        Debug.LogError($"Purchase failed: "+ order.Info.PurchasedProductInfo[0].productId);
    }

    public async void Buy(string productId)
    {
        try
        {
            storeController.Purchase(new Cart(new CartItem(storeController.GetProductById(productId))));
            Debug.Log("Purchase initiated for: " + productId);
        }
        catch (Exception e)
        {
            Debug.LogError($"Purchase error: {e.Message}");
        }
    }

    async System.Threading.Tasks.Task LoadPurchases()
    {
        try
        {
            var dict = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "purchases" });
            if (dict.TryGetValue("purchases", out var item))
            {
                string json = item.ToString(); 
                purchases = JsonUtility.FromJson<PlayerPurchases>(json);
            }
        }
        catch
        {
            purchases = new PlayerPurchases();
        }
    }
    async void SavePurchases()
    {
        string json = JsonUtility.ToJson(purchases);
        await CloudSaveService.Instance.Data.ForceSaveAsync(new Dictionary<string, object> { { "purchases", json } });
        Debug.Log("Saved purchases to Cloud Save");
    }

    public bool AdsRemoved => purchases.adsRemoved;
    public bool HasSkin(string skinId) => purchases.ownedSkins.Contains(skinId);
}