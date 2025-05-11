using System;
using System.IO;
using UnityEngine;

namespace HFantasy.Script.Configs
{
    public static class PathConfig
    {
        //public const string SaveFolder = "/Saves/";

        public const string ResRoot = "Assets/Res/";

        public const string AssetIndexTablePath = ResRoot +"AssetIndexTable.json";

        public const string PlayerAppearanceExportPath = ResRoot + "Tables/PlayerAppearanceConfig.csv";

        public const string PlayerSMLogicPath = ResRoot + "Tables/StateMachine/PlayerSMLogic.xml";

        public const string SHADER_PATH = "Assets/Res/ArtRes/Shaders";


        public static string SaveFolder
        {
            get
            {
                string folder = "";

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "F4ntasy");
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support", "F4ntasy");
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".config", "F4ntasy");
#else
        folder = Path.Combine(Application.persistentDataPath); // fallback
#endif

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                return folder;
            }
        }


    }
}