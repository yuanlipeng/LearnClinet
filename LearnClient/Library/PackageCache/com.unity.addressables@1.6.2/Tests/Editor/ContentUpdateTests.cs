using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace UnityEditor.AddressableAssets.Tests
{
    public class ContentUpdateTests : AddressableAssetTestBase
    {
        protected override bool PersistSettings { get { return true; } }

        [Test]
        public void CanCreateContentStateData()
        {
            var group = Settings.CreateGroup("LocalStuff", false, false, false, null);
            var schema = group.AddSchema<BundledAssetGroupSchema>();
            schema.BuildPath.SetVariableByName(Settings, AddressableAssetSettings.kLocalBuildPath);
            schema.LoadPath.SetVariableByName(Settings, AddressableAssetSettings.kLocalLoadPath);
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            group.AddSchema<ContentUpdateGroupSchema>().StaticContent = true;

            Settings.CreateOrMoveEntry(m_AssetGUID, group);
            var context = new AddressablesDataBuilderInput(Settings);

            var op = Settings.ActivePlayerDataBuilder.BuildData<AddressablesPlayerBuildResult>(context);

            Assert.IsTrue(string.IsNullOrEmpty(op.Error), op.Error);
            var tempPath = Path.GetDirectoryName(Application.dataPath) + "/Library/com.unity.addressables/StreamingAssetsCopy/" + PlatformMappingService.GetPlatform() + "/addressables_content_state.bin";
            var cacheData = ContentUpdateScript.LoadContentState(tempPath);
            Assert.NotNull(cacheData);
            Settings.RemoveGroup(group);
        }

        [Test]
        public void PrepareContentUpdate()
        {
            var group = Settings.CreateGroup("LocalStuff2", false, false, false, null);
            var schema = group.AddSchema<BundledAssetGroupSchema>();
            schema.BuildPath.SetVariableByName(Settings, AddressableAssetSettings.kLocalBuildPath);
            schema.LoadPath.SetVariableByName(Settings, AddressableAssetSettings.kLocalLoadPath);
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            group.AddSchema<ContentUpdateGroupSchema>().StaticContent = true;

            var entry = Settings.CreateOrMoveEntry(m_AssetGUID, group);
            entry.address = "test";

            var context = new AddressablesDataBuilderInput(Settings);

            Settings.ActivePlayerDataBuilder.BuildData<AddressablesPlayerBuildResult>(context);

            var path = AssetDatabase.GUIDToAssetPath(m_AssetGUID);
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            obj.GetComponent<Transform>().SetPositionAndRotation(new Vector3(10, 10, 10), Quaternion.identity);
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SavePrefabAsset(obj);
#else
            EditorUtility.SetDirty(obj);
#endif
            AssetDatabase.SaveAssets();
            var tempPath = Path.GetDirectoryName(Application.dataPath) + "/Library/com.unity.addressables/StreamingAssetsCopy/" + PlatformMappingService.GetPlatform() + "/addressables_content_state.bin";
            var modifiedEntries = ContentUpdateScript.GatherModifiedEntries(Settings, tempPath);
            Assert.IsNotNull(modifiedEntries);
            Assert.GreaterOrEqual(modifiedEntries.Count, 1);
            ContentUpdateScript.CreateContentUpdateGroup(Settings, modifiedEntries, "Content Update");
            var contentGroup = Settings.FindGroup("Content Update");
            Assert.IsNotNull(contentGroup);
            var movedEntry = contentGroup.GetAssetEntry(m_AssetGUID);
            Assert.AreSame(movedEntry, entry);
            Settings.RemoveGroup(group);
        }

        [Test]
        public void BuildContentUpdate()
        {
            var group = Settings.CreateGroup("LocalStuff3", false, false, false, null);
            Settings.BuildRemoteCatalog = true;
            Settings.RemoteCatalogBuildPath = new ProfileValueReference();
            Settings.RemoteCatalogBuildPath.SetVariableByName(Settings, AddressableAssetSettings.kRemoteBuildPath);
            Settings.RemoteCatalogLoadPath = new ProfileValueReference();
            Settings.RemoteCatalogLoadPath.SetVariableByName(Settings, AddressableAssetSettings.kRemoteLoadPath);
            var schema = group.AddSchema<BundledAssetGroupSchema>();
            schema.BuildPath.SetVariableByName(Settings, AddressableAssetSettings.kLocalBuildPath);
            schema.LoadPath.SetVariableByName(Settings, AddressableAssetSettings.kLocalLoadPath);
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            group.AddSchema<ContentUpdateGroupSchema>().StaticContent = true;
            Settings.CreateOrMoveEntry(m_AssetGUID, group);
            var context = new AddressablesDataBuilderInput(Settings);

            var op = Settings.ActivePlayerDataBuilder.BuildData<AddressablesPlayerBuildResult>(context);

            Assert.IsTrue(string.IsNullOrEmpty(op.Error), op.Error);
            var tempPath = Path.GetDirectoryName(Application.dataPath) + "/Library/com.unity.addressables/StreamingAssetsCopy/" + PlatformMappingService.GetPlatform() + "/addressables_content_state.bin";
            var buildOp = ContentUpdateScript.BuildContentUpdate(Settings, tempPath);
            Assert.IsNotNull(buildOp);
            Assert.IsTrue(string.IsNullOrEmpty(buildOp.Error));
            Settings.RemoveGroup(group);
        }

        [Test]
        public void IsCacheDataValid_WhenNoPreviousRemoteCatalogPath_ReturnsFalseWithError()
        {
            AddressablesContentState cacheData = new AddressablesContentState();
            cacheData.editorVersion = Application.unityVersion;
            Assert.IsFalse(ContentUpdateScript.IsCacheDataValid(Settings, cacheData));
            LogAssert.Expect(LogType.Error, new Regex("Previous build had 'Build Remote Catalog' disabled.*"));
        }

        [Test]
        public void IsCacheDataValid_WhenRemoteCatalogDisabled_ReturnsFalseWithError()
        {
            AddressablesContentState cacheData = new AddressablesContentState();
            cacheData.editorVersion = Application.unityVersion;
            cacheData.remoteCatalogLoadPath = "somePath";
            var oldSetting = Settings.BuildRemoteCatalog;
            Settings.BuildRemoteCatalog = false;
            Assert.IsFalse(ContentUpdateScript.IsCacheDataValid(Settings, cacheData));
            LogAssert.Expect(LogType.Error, new Regex("Current settings have 'Build Remote Catalog' disabled.*"));
            Settings.BuildRemoteCatalog = oldSetting;
        }

        [Test]
        public void IsCacheDataValid_WhenMismatchedCatalogPaths_ReturnsFalseWithError()
        {
            AddressablesContentState cacheData = new AddressablesContentState();
            cacheData.editorVersion = Application.unityVersion;
            cacheData.remoteCatalogLoadPath = "somePath";
            var oldSetting = Settings.BuildRemoteCatalog;
            Settings.BuildRemoteCatalog = true;
            Assert.IsFalse(ContentUpdateScript.IsCacheDataValid(Settings, cacheData));
            LogAssert.Expect(LogType.Error, new Regex("Current 'Remote Catalog Load Path' does not match load path of original player.*"));
            Settings.BuildRemoteCatalog = oldSetting;
        }

        [Test]
        public void IsCacheDataValid_WhenMismatchedEditorVersions_LogsWarning()
        {
            AddressablesContentState cacheData = new AddressablesContentState();
            cacheData.editorVersion = "invalid";
            Assert.IsFalse(ContentUpdateScript.IsCacheDataValid(Settings, cacheData));
            LogAssert.Expect(LogType.Warning, new Regex(".*with version `" + cacheData.editorVersion + "`.*"));
            LogAssert.Expect(LogType.Error, new Regex("Previous.*"));
        }

        [Test]
        public void BuildContentUpdate_DoesNotDeleteBuiltData()
        {
            var group = Settings.CreateGroup("LocalStuff3", false, false, false, null);
            var schema = group.AddSchema<BundledAssetGroupSchema>();
            schema.BuildPath.SetVariableByName(Settings, AddressableAssetSettings.kLocalBuildPath);
            schema.LoadPath.SetVariableByName(Settings, AddressableAssetSettings.kLocalLoadPath);
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            group.AddSchema<ContentUpdateGroupSchema>().StaticContent = true;
            Settings.CreateOrMoveEntry(m_AssetGUID, group);
            var context = new AddressablesDataBuilderInput(Settings);

            var op = Settings.ActivePlayerDataBuilder.BuildData<AddressablesPlayerBuildResult>(context);

            Assert.IsTrue(string.IsNullOrEmpty(op.Error), op.Error);
            var tempPath = Path.GetDirectoryName(Application.dataPath) + "/Library/com.unity.addressables/StreamingAssetsCopy/" + PlatformMappingService.GetPlatform() + "/addressables_content_state.bin";
            ContentUpdateScript.BuildContentUpdate(Settings, tempPath);
            Assert.IsTrue(Directory.Exists(Addressables.BuildPath));
        }

        [Test]
        public void ContentUpdateScenes_PackedTogether_MarksAllScenesModified()
        {
            AddressableAssetGroup group = Settings.CreateGroup("SceneGroup", false, false, false, null, typeof(BundledAssetGroupSchema));
            group.GetSchema<BundledAssetGroupSchema>().BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;

            AddressableAssetEntry scene1 = new AddressableAssetEntry(m_SceneGuids[0], "scene1", group, false);
            AddressableAssetEntry scene2 = new AddressableAssetEntry(m_SceneGuids[1], "scene2", group, false);
            AddressableAssetEntry scene3 = new AddressableAssetEntry(m_SceneGuids[2], "scene3", group, false);

            group.AddAssetEntry(scene1, false);
            group.AddAssetEntry(scene2, false);
            group.AddAssetEntry(scene3, false);

            List<AddressableAssetEntry> modifedEnteries = new List<AddressableAssetEntry>()
            {
                scene1
            };

            ContentUpdateScript.AddAllDependentScenesFromModifiedEnteries(modifedEnteries);

            Assert.AreEqual(3, modifedEnteries.Count);
            Assert.AreEqual(scene1, modifedEnteries[0]);
            Assert.AreEqual(scene2, modifedEnteries[1]);
            Assert.AreEqual(scene3, modifedEnteries[2]);

            Settings.RemoveGroup(group);
        }

        [Test]
        public void ContentUpdateScenes_PackedTogetherByLabel_MarksAllScenesModifiedWithSharedLabel()
        {
            AddressableAssetGroup group = Settings.CreateGroup("SceneGroup", false, false, false, null, typeof(BundledAssetGroupSchema));
            group.GetSchema<BundledAssetGroupSchema>().BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;

            AddressableAssetEntry scene1 = new AddressableAssetEntry(m_SceneGuids[0], "scene1", group, false);
            AddressableAssetEntry scene2 = new AddressableAssetEntry(m_SceneGuids[1], "scene2", group, false);
            AddressableAssetEntry scene3 = new AddressableAssetEntry(m_SceneGuids[2], "scene3", group, false);

            group.AddAssetEntry(scene1, false);
            group.AddAssetEntry(scene2, false);
            group.AddAssetEntry(scene3, false);

            scene1.SetLabel("label", true);
            scene3.SetLabel("label", true);

            List<AddressableAssetEntry> modifedEnteries = new List<AddressableAssetEntry>()
            {
                scene1
            };

            ContentUpdateScript.AddAllDependentScenesFromModifiedEnteries(modifedEnteries);

            Assert.AreEqual(2, modifedEnteries.Count);
            Assert.AreEqual(scene1, modifedEnteries[0]);
            Assert.AreEqual(scene3, modifedEnteries[1]);

            Settings.RemoveGroup(group);
        }

        [Test]
        public void ContentUpdateScenes_PackedSeperately_MarksNoAdditionalScenes()
        {
            AddressableAssetGroup group = Settings.CreateGroup("SceneGroup", false, false, false, null, typeof(BundledAssetGroupSchema));
            group.GetSchema<BundledAssetGroupSchema>().BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;

            AddressableAssetEntry scene1 = new AddressableAssetEntry(m_SceneGuids[0], "scene1", group, false);
            AddressableAssetEntry scene2 = new AddressableAssetEntry(m_SceneGuids[1], "scene2", group, false);
            AddressableAssetEntry scene3 = new AddressableAssetEntry(m_SceneGuids[2], "scene3", group, false);

            group.AddAssetEntry(scene1, false);
            group.AddAssetEntry(scene2, false);
            group.AddAssetEntry(scene3, false);

            List<AddressableAssetEntry> modifedEnteries = new List<AddressableAssetEntry>()
            {
                scene1
            };

            ContentUpdateScript.AddAllDependentScenesFromModifiedEnteries(modifedEnteries);

            Assert.AreEqual(1, modifedEnteries.Count);
            Assert.AreEqual(scene1, modifedEnteries[0]);

            Settings.RemoveGroup(group);
        }
    }
}