﻿using Netherlands3D.Core;
using Netherlands3D;
using T3D.Uitbouw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[KeepSaveDataOnReset]
public class HTMLInitSaveData : SaveDataContainer
{
    public string SessionId; //SessionSaver
    public string Street; //SelectedAddressPanel
    public string City; //SelectedAddressPanel
    public string HouseNumber; //SelectedAddressPanel
    public string HouseNumberAddition; //SelectedAddressPanel
    public string ZipCode; //SelectedAddressPanel
    public bool HasFile; //SelectOptionState
    public Vector3RD RDPosition; //AddressSearchState
    public string BagId; //AddressSearchState
    public string BlobId; //todo
    public string ModelId; //todo
    public string ModelVersionId;//todo
    public string Date; //SubmitRequestState
    public bool IsUserFeedback; //SendFeedback
    public bool HasSubmitted; //SubmitRequestState
    public bool IsMonument; //AddressSearchState
    public bool IsBeschermd; //AddressSearchState
    public bool SnapToWall; //SelectOptionState
    public bool Add3DModel; //SelectOptionState

    public string simpleAddressJson; //AddressSearchState
}

public class T3DInit : MonoBehaviour, IUniqueService
{
    public const string X_API_KEY = "l772bb9814e5584919b36a91077cdacea7";
    public const string ACCEPT_CRS = "epsg:28992";

    public bool IsEditMode { get; private set; } = true;
   
    public HTMLInitSaveData HTMLData = null;

    public Netherlands3D.Rendering.RenderSettings RenderSettings;

    private void Awake()
    {
        HTMLData = new HTMLInitSaveData();
    }

    void Start()
    {
        ToggleQuality(true);
    }

    private void ToggleQuality(bool ishigh)
    {
        //postProcessingEffects
        RenderSettings.TogglePostEffects(ishigh);

        //antiAliasing
        RenderSettings.ToggleAA(ishigh);

        //ambientOcclusion
        RenderSettings.ToggleAO(ishigh);
    }
}
