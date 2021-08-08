using System.Collections.Generic;
using System.Linq;
using Assets.GetText.NET.GetText.Extractor.Template;
using Assets.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor
{
    public class ExtractGettextStringsEditorWindow : EditorWindow
    {
        [MenuItem("Window/Translation/Extract Translations...")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (ExtractGettextStringsEditorWindow)GetWindow(typeof(ExtractGettextStringsEditorWindow));
            window.Show();
        }

        private void OnGUI()
        {
            var clicked = GUILayout.Button("Extract translations in build-scenes");
            if (!clicked)
            {
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.LogWarning("Unable to extract translations if changes aren't saved first.");
                return;
            }

            var translationObjects = GetTranslationStringsForBuildScenes().Concat(GetTranslationStringsForPrefabs());

            WriteTranslationFile(translationObjects);
        }

        private static void WriteTranslationFile(IEnumerable<TranslationObject> translationObjects)
        {
            var catalog = new CatalogTemplate("Assets/Resources/Translation/Assets.pot");

            foreach (var translationObject in translationObjects)
            {
                catalog.AddOrUpdateEntry(
                    translationObject.Context,
                    translationObject.PrimaryString,
                    translationObject.PluralString,
                    null, // TODO: Add reference
                    translationObject.FormatString);
            }

            catalog.WriteAsync().Wait();
        }

        private static IEnumerable<TranslationObject> GetTranslationStringsForPrefabs()
        {
            var translationStrings = new List<TranslationObject>();

            var prefabsGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (var prefabGuid in prefabsGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                translationStrings.AddRange(GetTranslationStringsForGameObject(go));
            }

            return translationStrings;
        }

        private static IEnumerable<TranslationObject> GetTranslationStringsForBuildScenes()
        {
            var loadedScenePaths = GetLoadedScenePaths();
            LoadBuildScenes();

            var translationStrings = GetTranslationStringsForOpenScenes();

            LoadScenes(loadedScenePaths);

            return translationStrings;
        }

        private static IEnumerable<TranslationObject> GetTranslationStringsForOpenScenes()
        {
            var translationStrings = new List<TranslationObject>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var rootObjects = scene.GetRootGameObjects();
                foreach (var rootObject in rootObjects)
                {
                    translationStrings.AddRange(GetTranslationStringsForGameObject(rootObject));
                }
            }

            return translationStrings;
        }

        private static IEnumerable<string> GetLoadedScenePaths()
        {
            var scenePaths = new List<string>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                scenePaths.Add(scene.path);
            }

            return scenePaths;
        }

        private static void LoadBuildScenes()
        {
            var first = true;

            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(i), first ? OpenSceneMode.Single : OpenSceneMode.Additive);
                first = false;
            }
        }

        private static void LoadScenes(IEnumerable<string> scenePaths)
        {
            var first = true;

            foreach (var scenePath in scenePaths)
            {
                EditorSceneManager.OpenScene(scenePath, first ? OpenSceneMode.Single : OpenSceneMode.Additive);
                first = false;
            }
        }

        private static IEnumerable<TranslationObject> GetTranslationStringsForGameObject(GameObject go)
        {
            return go.GetComponentsInChildren<TranslateLabelString>(true).Select(x => x.GetTranslationObject());
        }
    }
}
