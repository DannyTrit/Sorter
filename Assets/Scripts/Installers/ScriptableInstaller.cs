using System.Collections.Generic;
using ModestTree;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Sorter.Installers
{
    [CreateAssetMenu(fileName = "ScriptableInstaller", menuName = "ScriptableObject/ScriptableInstaller")]
    public class ScriptableInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private ScriptableObject[] _scriptableObjects;

        public override void InstallBindings()
        {
            Container.BindInstances(_scriptableObjects);
        }
        
#if UNITY_EDITOR
        private void Reset()
        {
            var scriptableObjects = new List<ScriptableObject>();

            var assetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (var path in assetPaths)
            {
                var asset = AssetDatabase.LoadMainAssetAtPath(path);

                if (asset is ScriptableObject scriptableObject && scriptableObject.GetType().HasAttribute(typeof(InstallableScriptableObjectAttribute)))
                {
                    scriptableObjects.Add(scriptableObject);
                }
            }

            _scriptableObjects = scriptableObjects.ToArray();
        }
#endif
    }
}