using UnityEditor;

namespace Sorter.Editor
{
    public static class Tools
    {
        [MenuItem("Tools/Generate/UI Types", false, 1)]
        public static void GenerateUITypesProvider()
        {
            UITypesProviderGenerator.Generate();
        }
    }
}