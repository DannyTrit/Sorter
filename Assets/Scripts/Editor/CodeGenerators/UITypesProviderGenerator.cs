using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sorter.Services.UI;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Sorter.Editor
{
    public static class UITypesProviderGenerator
    {
        private const string SCRIPT_PATH = "Assets/Scripts/Services/UISystem/UITypesProvider.cs";
        private const string CODEGEN_BEGIN = @"//UITypes Auto-generated code start";
        private const string CODEGEN_END = @"//UITypes Auto-generated code end";
        private const string PREFABS_PATH = "Assets/Content/UI/";

        public static void Generate()
        {
            if (File.Exists(SCRIPT_PATH) is false)
            {
                Debug.LogError($"Can't generate \"{nameof(Sorter.Services.UI.UITypesProvider)}\": file does not exist at path: {SCRIPT_PATH}");
                return;
            }
            
            var views = GetViewAssets().Select(CreateDataString).OrderBy(x => x);
            
            var ctorBuilder = new StringBuilder();

            ctorBuilder.AppendLine("\t\t" + CODEGEN_BEGIN);
            ctorBuilder.AppendLine("\t\tpublic " + nameof(Sorter.Services.UI.UITypesProvider) + "()");
            ctorBuilder.AppendLine("\t\t{");
            views.ForEach(x => ctorBuilder.AppendLine(x));
            ctorBuilder.AppendLine("\t\t}");
            ctorBuilder.AppendLine("\t\t" + CODEGEN_END);

            var content = File.ReadAllText(SCRIPT_PATH);
            var split = content.Split(new String[] { CODEGEN_BEGIN, CODEGEN_END }, StringSplitOptions.None);
            if (split.Length != 3)
            {
                Debug.LogError($"Can't generate '{nameof(Sorter.Services.UI.UITypesProvider)}': no keywords at script were found");
                return;
            }

            var scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine(split.First().TrimEnd());
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine(ctorBuilder.ToString());
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine($"\t\t{split.Last().TrimStart()}");

            File.WriteAllText(SCRIPT_PATH, scriptBuilder.ToString().Trim(), Encoding.UTF8);
            Debug.Log("Generation <color=lime>Completed</color>");
        }

        private static string CreateDataString(KeyValuePair<Type, ViewAsset> data)
        {
            var viewType = $"typeof({data.Key.FullName})";
            var viewModelType = $"typeof({data.Value.ViewModel.FullName})";
            var modelType = data.Value.Model == null ? "null" : $"typeof({data.Value.Model.FullName})";
            var viewAssetCtor = $"new {nameof(ViewAsset)}(\"{data.Value.Key}\", {viewModelType}, {modelType})";

            return $"\t\t\t_data.Add({viewType}, {viewAssetCtor});";
        }
        
        private static IReadOnlyDictionary<Type, ViewAsset> GetViewAssets()
        {
            var data = new Dictionary<Type, ViewAsset>();
            var prefabs = LoadAsset(x => x.EndsWith(".prefab") && x.StartsWith(PREFABS_PATH));
            var views = prefabs.Select(x => x.GetComponentInChildren<UIViewBase>(true)).Where(x => x != null).ToArray();

            foreach (var view in views)
            {
                var viewType = view.GetType();
                var viewBaseClassDefinition = typeof(UIViewBase<>);
                var viewBaseType = GetAbstractBaseClasses(viewType).FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == viewBaseClassDefinition);
                if (viewBaseType == null) throw new Exception($"Type {viewType} is have no {viewBaseClassDefinition} base class");

                var viewModelType = viewBaseType.GetGenericArguments().First();
                var viewModelBaseClassDefinition = typeof(UIViewModelBase<>);
                var viewModelBaseType = GetAbstractBaseClasses(viewModelType).FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == viewModelBaseClassDefinition);

                Type modelType = null;

                if (viewModelBaseType != null) modelType = viewModelBaseType.GetGenericArguments()[0];

                var assetPath = AssetDatabase.GetAssetPath(view);
                var assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                var assetEntry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(assetGUID);

                if (assetEntry != null)
                    data.Add(viewType, new ViewAsset(assetEntry.address, viewModelType, modelType));
                else
                    Debug.LogError($"View {viewType.Name} is not addressable", view);
            }
            
            return data;
        }

        private static GameObject[] LoadAsset(Predicate<string> pathSelector)
        {
            AssetDatabase.Refresh();

            var type = typeof(GameObject);
            var assetsPaths = AssetDatabase.GetAllAssetPaths().OrderBy(x => x).ToList();
            var pathPrefix = Path.GetFileName(Application.dataPath);
            var paths = assetsPaths.FindAll(path => path.StartsWith(pathPrefix) && pathSelector(path) && type.IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(path)));

            var objects = new List<GameObject>();

            for (var i = 0; i < paths.Count; i++)
            {
                var path = paths[i];
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path.Trim());

                if (obj == null) continue;

                var progress = (float)i / paths.Count;
                EditorUtility.DisplayProgressBar($"Loading asset ({i + 1}/{paths.Count})", path, progress);

                objects.Add(obj);
            }

            EditorUtility.ClearProgressBar();

            return objects.ToArray();
        }
        
        private static List<Type> GetAbstractBaseClasses(Type type)
        {
            var abstractBaseClasses = new List<Type>();

            while (type != null)
            {
                var baseType = type.BaseType;
                if (baseType != null && baseType.IsAbstract)
                {
                    abstractBaseClasses.Add(baseType);
                }
                type = baseType;
            }

            return abstractBaseClasses;
        }
    }
}