// Copyright © 2018 3D Haven.  All Rights Reserved.
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Haven.PathPainter2
{
    internal class ImpMgr
    {
        private struct Target
        {
            public string Id;
            public string Msg;
            public Target(string id, string msg)
            {
                Id = id;
                Msg = msg;
            }
        }
        private const string PP_FOLDER = "f86f1c5645c7f9e43ad5e591a600864e";
        private static readonly Target[] TODEL = new Target[]
        {
            new Target("31b7acf7ac89a954a9056bfc59468025",          // b1/PP1 folder
                "Removed an earlier Path Painter " +
                "version to avoid Beta version 1 (v2.1.0b1)" +
                " conflicting with the latest version"),
            new Target("77f1c3ac1d6a395419b13f8628192d5c",          // pre v2.1.2
                "Removed a pre-v2.1.2 Path Painter " +
                "to replace with the new lean version"),
            new Target("34b88dec236c0524793023c5885007db",          // pre v2.1.5 stuff - UnityBug1373388.cs
                "Removed Unity Bug #1373388 workaround, " +
                "because Unity fixed the bug"),
            new Target("be3415658f36aac48a8c2a51a3d50a97",          // obsolete terrain
                "Removed obsolete Path Painter demo terrains " +
                "used in early Path Painter II versions"),
            new Target("5eaabb075983e8d4e8dc11fc4f741235", ""),     // obsolete terrain
            new Target("e6a6d1ba0727c24439dbca0b4f4bf854", ""),     // obsolete terrain
            new Target("eb34dc917159c77448183ed190861fe8", ""),     // obsolete terrain
            new Target("80f7e3193ece5564e925efac5f7210ea", ""),     // obsolete terrain
            new Target("33bea1de80d591f4cae7bcf766783e87", ""),     // obsolete terrain
            new Target("bfd47cccede788b41b6faebdf62ff695",          // obsolete layers
                "Removed obsolete Path Painter demo terrain " +
                "layers used in early Path Painter II versions"),
            new Target("bd0efcefc152189478814fdfebea2434",          // obsolete textures
                "Removed obsolete Path Painter demo terrain " +
                "textures used in early Path Painter II versions"),

            new Target("c2831093bba02b245bcabfc4000907a4", "")      // this
        };

        private static readonly string[] TOACT = new string[]
        {
            "d634e9b0e9c0bfd4784ecde5e306268e",
            "7d53a18955d33b54181c366fbbb3f82c",
            "23e3655f6408d7e488040dcd3be67c9f",
            "aa3072e4fb77d73478681dd393b02c06",
        };

#if !HAVEN_REL
        [InitializeOnLoadMethod]
        static void Onload()
        {
            Del();
            // Activate();
        } 
#endif

        /// <summary>
        /// Del
        /// </summary>
        private static void Del()
        {
            for (int i = 0; i < TODEL.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(TODEL[i].Id);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                AssetDatabase.DeleteAsset(path);
                if (!string.IsNullOrEmpty(TODEL[i].Msg))
                {
                    Debug.LogWarningFormat("[3D Haven]: {0} ({1}).", TODEL[i].Msg, path);
                }
            }
        }

        /// <summary>
        /// Rename
        /// </summary>
        public static void Rename()
        {
            string path = AssetDatabase.GUIDToAssetPath(PP_FOLDER);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogErrorFormat("[3D Haven]: Unable to locate Path Painter II Lean using [{0}]." +
                    " This error does not indicate any issue that affects functionality, " +
                    "but please provide the messegae to support.", PP_FOLDER);
                return;
            }
            AssetDatabase.RenameAsset(path, "Path Painter II");
        }

        /// <summary>
        /// Activate
        /// </summary>
        private static void Activate()
        {
            for (int i = 0; i < TOACT.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(TOACT[i]);
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning(string.Format("[3D Haven]: Missing Path Painter component: '{0}'. Try to import " +
                        "from the Asset Store and forward this message to support if it persist.", TOACT[i]));
                    continue;
                }
                PluginImporter importer = AssetImporter.GetAtPath(path) as PluginImporter;
                if (importer == null)
                {
                    Debug.LogWarning(string.Format("[3D Haven]: Corrupted Path Painter component: '{0}'. Try to import " +
                        "from the Asset Store and forward this message to support if it persist.", TOACT[i]));
                    continue;
                }
                List<string> constraints = new List<string>(importer.DefineConstraints);
                if (!constraints.Contains("PP_ACTIVATE_POST_IMPORT"))
                {
                    continue;
                }
                constraints.Remove("PP_ACTIVATE_POST_IMPORT");
                importer.DefineConstraints = constraints.ToArray();
                importer.SaveAndReimport();
            }
        }
    }
}
#endif
