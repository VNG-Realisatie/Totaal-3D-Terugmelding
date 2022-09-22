using System;
using System.Collections;
using System.Collections.Generic;
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
    private JSONNode selectedAddress;

    protected override void Awake()
    {
        base.Awake();
        resultSuggestionButtons = resultButtonsParent.GetComponentsInChildren<AddressSuggestionButton>();
    }

    private void OnEnable()
    {
        inputField.onValueChanged.AddListener(InputFieldValueChanged);
    }

    public void SetAddress(JSONNode node)
    {
        selectedAddress = node;
        inputField.text = node["omschrijving"];
        print("selected: " + node.ToString());
    }

    private void OnDisable()
    {
        inputField.onValueChanged.RemoveAllListeners();
    }

    private void InputFieldValueChanged(string newString)
    {
        if (newString != string.Empty)
        {
            StartCoroutine(SearchAddress(newString));
        }
    }

    private IEnumerator SearchAddress(string input)
    {
        var uri = @"https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressen/zoek?zoek=" + input + @"&page=1&pageSize=" + resultSuggestionButtons.Length;
        var uwr = UnityWebRequest.Get(uri);
        uwr.SetRequestHeader("X-Api-Key", "l772bb9814e5584919b36a91077cdacea7");
        uwr.SetRequestHeader("Accept-Crs", "epsg:28992");
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
            resultButtonsParent.SetActive(false);
        }
        else
        {
            if (searchResults["_embedded"]["zoekresultaten"].Count == 1)
            {
                resultButtonsParent.SetActive(false);
                inputField.SetTextWithoutNotify(searchResults["_embedded"]["zoekresultaten"][0]["omschrijving"].ToString());

                return;
            }

            resultButtonsParent.SetActive(true);
            for (int i = 0; i < resultSuggestionButtons.Length; i++)
            {
                var r = searchResults["_embedded"]["zoekresultaten"][i];
                resultSuggestionButtons[i].gameObject.SetActive(r != null);
                resultSuggestionButtons[i].SetAddressNode(r);
            }
        }
    }
}
