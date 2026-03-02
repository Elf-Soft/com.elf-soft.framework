using UnityEngine;

namespace ElfSoft.Framework
{
    public static class Utils
    {
        /// <summary>
        /// ÍË³öÓÎÏ·
        /// </summary>
        public static void ExitGame()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
#endif
            Application.Quit();
        }

    }
}
