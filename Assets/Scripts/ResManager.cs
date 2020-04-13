using UnityEngine;
using System.Collections.Generic;
using System.IO;

public sealed class ResManager{
    public class BundleList : ScriptableObject {
        public List<BundleData> bundleDatas = new List<BundleData>();

        public class BundleData {
            public string resPath = string.Empty;
            public string bundlePath = string.Empty;
        }
    }


    AsyncOperation _isReleasing;
    static Dictionary<string, string> _resABmap = new Dictionary<string, string>();
    static Dictionary<string, AssetBundle> _bundleCache = new Dictionary<string, AssetBundle>();

    ResManager() {
        _isReleasing = null;
        BundleList list = Resources.Load<BundleList>("bundleList");
        if(list == null) {
            return;
        }
        foreach(var bundleData in list.bundleDatas) {
            _resABmap[bundleData.resPath] = bundleData.bundlePath;
        }
    }
    //加载预设
    public static T LoadAsset<T>(string path) where T:Object {
        string resPath = Path.Combine("Assets/Resources", path);
        if(typeof(T) == typeof(GameObject)) {
            resPath += ".prefab";
        }
        string bundlePath;
        if(_resABmap.TryGetValue(resPath, out bundlePath)) {
            AssetBundle assetBundle;
            if(!_bundleCache.TryGetValue(bundlePath, out assetBundle)) {
                assetBundle = _bundleCache[bundlePath] = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundlePath));
            }
            return assetBundle.LoadAsset<T>(resPath);
        }
        return Resources.Load<T>(path);
    }
    
    public void ReleaseAllRes() {
        if (null == _isReleasing) {
            _isReleasing = Resources.UnloadUnusedAssets();
        }
    }

    private static readonly ResManager _instance = new ResManager();
    public static ResManager GetInstance() {
        return _instance;
    }
}
