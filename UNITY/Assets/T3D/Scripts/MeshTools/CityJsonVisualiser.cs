using ConvertCoordinates;
using Netherlands3D.T3D.Uitbouw;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using T3D.LoadData;
using T3D.Uitbouw;
//using T3D.LoadData;
using UnityEngine;

public struct CityObjectIdentifier
{
    public string Key;
    public JSONNode Node;
    public int Lod;
    public bool FlipYZ;
    public CityObjectType Type;

    public CityObjectIdentifier(string key, CityObjectType type, JSONNode node, int lod, bool flipYZ)
    {
        Key = key;
        Type = type;
        Lod = lod;
        Node = node;
        FlipYZ = flipYZ;
    }

    public override bool Equals(object other)
    {
        if (!(other is CityObjectIdentifier)) return false;

        return Equals((CityObjectIdentifier)other);
    }

    public bool Equals(CityObjectIdentifier other)
    {
        return (Key == other.Key) && (Lod == other.Lod);
    }
    public override int GetHashCode()
    {
        //https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
        int hash = 17;
        hash = hash * 31 + Key.GetHashCode();
        hash = hash * 31 + Lod.GetHashCode();
        return hash;
    }
}

public static class CityJsonVisualiser //: MonoBehaviour, IUniqueService
{
    private static string[] definedNodes = { "type", "version", "CityObjects", "vertices", "extensions", "metadata", "transform", "appearance", "geometry-templates" };

    public static void AddExtensionNodes(JSONNode cityJsonNode)
    {
        foreach (var node in cityJsonNode)
        {
            if (definedNodes.Contains(node.Key))
                continue;

            CityJSONFormatter.AddExtensionNode(node.Key, node.Value);
        }
    }

    public static void RemoveExtensionNodes(JSONNode cityJsonNode)
    {
        foreach (var node in cityJsonNode)
        {
            if (definedNodes.Contains(node.Key))
                continue;

            CityJSONFormatter.RemoveExtensionNode(node.Key);
        }
    }

    public static JSONObject GetAttributes(JSONNode cityJsonNode)
    {
        var attributesNode = new JSONObject();
        foreach (KeyValuePair<string, JSONNode> co in cityJsonNode)
        {
            var attributes = co.Value["attributes"];
            foreach (var attr in attributes)
            {
                attributesNode[attr.Key] = attr.Value; //todo: might overwrite attributes due to merge
            }
        }
        return attributesNode;
    }

    public static Dictionary<CityObjectIdentifier, Mesh> ParseCityJson(string cityJson, Matrix4x4 localToWorldMatrix, bool flipYZ, bool useKeytoSetExportIdPrefix)
    {
        var cityJsonModel = new CityJsonModel(cityJson, new Vector3RD(), false);
        return ParseCityJson(cityJsonModel, localToWorldMatrix, flipYZ, useKeytoSetExportIdPrefix);
    }

    public static Dictionary<CityObjectIdentifier, Mesh> ParseCityJson(CityJsonModel cityJsonModel, Matrix4x4 localToWorldMatrix, bool flipYZ, bool useKeytoSetExportIdPrefix)
    {
        //var cityJsonModel = new CityJsonModel(cityJson, new Vector3RD(), false);
        var meshmaker = new CityJsonMeshUtility();

        Dictionary<CityObjectIdentifier, Mesh> meshes = new Dictionary<CityObjectIdentifier, Mesh>();

        foreach (KeyValuePair<string, JSONNode> co in cityJsonModel.cityjsonNode["CityObjects"])
        {
            var key = co.Key;
            if (useKeytoSetExportIdPrefix)
            {
                var bagId = ServiceLocator.GetService<T3DInit>().HTMLData.BagId;
                CityObject.IdPrefix = key.Split(bagId.ToCharArray())[0];
            }
            var geometries = meshmaker.CreateMeshes(key, localToWorldMatrix, cityJsonModel, co.Value, flipYZ);

            foreach (var g in geometries)
            {
                meshes.Add(g.Key, g.Value);
            }
        }

        return meshes;
    }

    public static Mesh CombineMeshes(List<Mesh> meshes, Matrix4x4 localToWorldMatrix)
    {
        //if (meshes.Any())
        //{
        CombineInstance[] combine = new CombineInstance[meshes.Count];

        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.CombineMeshes(combine);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
        //}
        //return null;
    }
}
