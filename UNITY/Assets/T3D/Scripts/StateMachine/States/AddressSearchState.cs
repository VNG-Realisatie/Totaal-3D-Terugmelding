using System;
using System.Collections;
using System.Collections.Generic;
using ConvertCoordinates;
using Netherlands3D.T3D.Uitbouw;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AddressSearchState : State
{
    [SerializeField]
    private InputField inputField;
    private JSONNode searchResults;
    [SerializeField]
    private GameObject resultButtonsParent;
    private AddressSuggestionButton[] resultSuggestionButtons;
    //private JSONNode selectedAddress;

    [SerializeField]
    private GameObject backgroundImage;
    [SerializeField]
    private SelectedAddressPanel selectedAddressPanel;

    [SerializeField]
    private Button nextButton;

    protected override void Awake()
    {
        base.Awake();
        resultSuggestionButtons = resultButtonsParent.GetComponentsInChildren<AddressSuggestionButton>();
    }

    private void OnEnable()
    {
        inputField.onValueChanged.AddListener(InputFieldValueChanged);
    }

    private void Update()
    {
        nextButton.interactable = RestrictionChecker.ActiveBuilding.BuildingDataIsProcessed && RestrictionChecker.ActivePerceel.IsLoaded;
    }

    private IEnumerator GetAddress(string uri)
    {
        //var uri = @"https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressen?zoekresultaatIdentificatie=" + id;
        var uwr = UnityWebRequest.Get(uri);
        uwr.SetRequestHeader("X-Api-Key", T3DInit.X_API_KEY);
        uwr.SetRequestHeader("Accept-Crs", T3DInit.ACCEPT_CRS);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            var node = JSONNode.Parse(uwr.downloadHandler.text);
            var addressNode = node["_embedded"]["adressen"][0];
            var bagId = addressNode["pandIdentificaties"][0];
            ServiceLocator.GetService<T3DInit>().HTMLData.BagId = bagId;

            selectedAddressPanel.gameObject.SetActive(true);
            selectedAddressPanel.SetText(addressNode);
            backgroundImage.SetActive(false);

            var adresseerbaarObjectIdentificatie = addressNode["adresseerbaarObjectIdentificatie"];
            StartCoroutine(GetBuildingCoordinates(adresseerbaarObjectIdentificatie));
            //SetAddress(uwr.downloadHandler.text);
        }
    }

    public void RequestExtensiveAddressInfo(JSONNode node)
    {
        //selectedAddress = node;
        inputField.SetTextWithoutNotify(node["omschrijving"]);
        resultButtonsParent.transform.parent.gameObject.SetActive(false);

        var extensiveAddressInfoUri = node["_links"]["adres"]["href"];
        StartCoroutine(GetAddress(extensiveAddressInfoUri));
    }

    private void DeselectAddress()
    {
        backgroundImage.SetActive(true);
        selectedAddressPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        inputField.onValueChanged.RemoveAllListeners();
    }

    private void InputFieldValueChanged(string newString)
    {
        DeselectAddress();
        RestrictionChecker.ActiveBuilding.ResetBuilding();
        RestrictionChecker.ActivePerceel.ResetPerceel();
        if (newString != string.Empty)
        {
            StartCoroutine(SearchAddress(newString));
        }
    }

    private IEnumerator SearchAddress(string input)
    {
        var uri = @"https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressen/zoek?zoek=" + input + @"&page=1&pageSize=" + resultSuggestionButtons.Length;
        var uwr = UnityWebRequest.Get(uri);
        uwr.SetRequestHeader("X-Api-Key", T3DInit.X_API_KEY);
        uwr.SetRequestHeader("Accept-Crs", T3DInit.ACCEPT_CRS);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            UpdateResultsUI(uwr.downloadHandler.text);
        }
    }

    private void UpdateResultsUI(string jsonResults)
    {
        searchResults = JSON.Parse(jsonResults);

        if (searchResults["_embedded"] == null)
        {
            resultButtonsParent.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            //if (searchResults["_embedded"]["zoekresultaten"].Count == 1)
            //{
            //    resultButtonsParent.SetActive(false);
            //    inputField.SetTextWithoutNotify(searchResults["_embedded"]["zoekresultaten"][0]["omschrijving"].ToString());

            //    return;
            //}

            resultButtonsParent.transform.parent.gameObject.SetActive(true);
            for (int i = 0; i < resultSuggestionButtons.Length; i++)
            {
                var r = searchResults["_embedded"]["zoekresultaten"][i];
                resultSuggestionButtons[i].gameObject.SetActive(r != null);
                resultSuggestionButtons[i].SetSimpleAddressNode(r);
            }
        }
    }

    private IEnumerator GetBuildingCoordinates(string adresseerbaarObjectIdentificatie)
    {
        var uri = $"https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/verblijfsobjecten/{adresseerbaarObjectIdentificatie}";
        var uwr = UnityWebRequest.Get(uri);
        uwr.SetRequestHeader("X-Api-Key", T3DInit.X_API_KEY);
        uwr.SetRequestHeader("Accept-Crs", T3DInit.ACCEPT_CRS);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            var node = JSONNode.Parse(uwr.downloadHandler.text);
            if (node["verblijfsobject"] != null)
            {
                var bagCoordinates = node["verblijfsobject"]["geometrie"]["punt"]["coordinates"];
                var pos = new Vector3RD(bagCoordinates[0], bagCoordinates[1], bagCoordinates[2]);
                ServiceLocator.GetService<T3DInit>().HTMLData.RDPosition = pos;
                StartCoroutine(GetMonumentStatus(pos));
                StartCoroutine(GetProtectedStatus(pos));
                ServiceLocator.GetService<T3DInit>().LoadBuilding();
            }
            else
            {
                Debug.LogError("verblijfsobject is null");
            }
        }
        //this.getPerceel(this.bagcoordinates[0], this.bagcoordinates[1]);
    }

    private IEnumerator GetMonumentStatus(Vector3RD position)
    {
        var bbox = $"{ position.x - 0.5},{ position.y - 0.5},{ position.x + 0.5},{ position.y + 0.5}";
        var uri = $"https://services.rce.geovoorziening.nl/rce/wfs?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=rce:NationalListedMonuments&STARTINDEX=0&COUNT=1&SRSNAME=EPSG:28992&BBOX={bbox}&outputFormat=json";
        var uwr = UnityWebRequest.Get(uri);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            print(uwr.downloadHandler.text);
            var node = JSONNode.Parse(uwr.downloadHandler.text);
            var isMonument = node["features"].Count > 0;
            var data = ServiceLocator.GetService<T3DInit>().HTMLData;
            data.IsMonument = isMonument;
            if (isMonument)
            {
                selectedAddressPanel.FormatExtraInfoText(selectedAddressPanel.oorspronkelijkBouwjaar, selectedAddressPanel.perceelOppervlakte, isMonument, data.IsBeschermd);
            }
        }
    }

    private IEnumerator GetProtectedStatus(Vector3RD position)
    {
        var bbox = $"{ position.x - 0.5},{ position.y - 0.5},{ position.x + 0.5},{ position.y + 0.5}";
        var uri = $"https://services.rce.geovoorziening.nl/rce/wfs?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=rce:ArcheologicalMonuments&STARTINDEX=0&COUNT=1&SRSNAME=EPSG:28992&BBOX={bbox}&outputFormat=json";
        var uwr = UnityWebRequest.Get(uri);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            var node = JSONNode.Parse(uwr.downloadHandler.text);
            var isBeschermd = node["features"].Count > 0;
            var data = ServiceLocator.GetService<T3DInit>().HTMLData;
            data.IsBeschermd = isBeschermd;
            if (isBeschermd)
            {
                selectedAddressPanel.FormatExtraInfoText(selectedAddressPanel.oorspronkelijkBouwjaar, selectedAddressPanel.perceelOppervlakte, data.IsMonument, isBeschermd);
            }
        }
    }
}
