using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskExtension
{
    public static IEnumerator AsCoroutine(this Task task)
    {
        while (!task.IsCompleted) yield return null;
        // if task is faulted, throws the exception
        task.GetAwaiter().GetResult();
    }
}
