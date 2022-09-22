using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class AddressSuggestionButton : MonoBehaviour
{
    private Button button;
    private Text buttonText;
    private AddressSearchState state;
    private JSONNode addressNode;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(ChooseAddress);
        state = GetComponentInParent<AddressSearchState>();
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void ChooseAddress()
    {
        state.SetAddress(addressNode);
    }

    public void SetAddressNode(JSONNode node)
    {
        buttonText.text = node["omschrijving"];
        addressNode = node;
    }
}
