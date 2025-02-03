using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.ResourcesLoading
{
    public static class ResourceLoader
    {
        public static List<T> Load<T>(string resourcePath) where T : Object
        {
            try
            {
                return Resources.LoadAll<T>(resourcePath).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while loading resources!\n Path: {resourcePath}\nException: {e.Message}");
                throw;
            }
        }

        public static void UnloadAsset<T>(T asset) where T : Object => Resources.UnloadAsset(asset);
    }
}