using Netherlands3D;
using NUnit.Framework;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;
using WebGLFileUploaderExample;



public class UnitTests
{
    [Test]
    public void TestPass()
    {
        Assert.That(true);
    }

    [UnityTest]
    public IEnumerator TestUpload()
    {
        yield return Run().AsCoroutine();

        async Task Run()
        {
            string filepath = @"E:\T3D\Data\IFC\ASP9 - Nieuw.ifc";
            FileInfo finfo = new FileInfo(filepath);            
            //var url = ($"http://localhost:7071/api/uploadbim/{Uri.EscapeDataString(finfo.Name)}");            
            var url = ($"https://t3d-o-functions.azurewebsites.net/api/uploadbim/{Uri.EscapeDataString(finfo.Name)}");

            var result = await UploadBimUtility.UploadFile(url, filepath);
            Debug.Log(result);

            var json = JSON.Parse(result);
            var modeId = json["modelId"];

            var urlCheckVersion = ($"https://t3d-o-functions.azurewebsites.net/api/getbimversionstatus/{modeId}");

            //UploadBimUtility.CheckBimVersion(urlCheckVersion, () => { }, (string result) => { });

        }        
    }

    [UnityTest]
    public IEnumerator TestAsync()
    {
        yield return Run().AsCoroutine();

        async Task Run()
        {
            var result = await UploadBimUtility.AsyncGet();
            Debug.Log(result);
        }
    }

    [UnityTest]
    public IEnumerator TestPost()
    {
        yield return Run().AsCoroutine();

        async Task Run()
        {
            var result = await UploadBimUtility.AsyncPost();
            Debug.Log(result);
        }
    }


}
