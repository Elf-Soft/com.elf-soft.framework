using UnityEditor;
using UnityEngine;

namespace DemonMaid.Framework.Editor
{
    public class FbxPostProcessor : AssetPostprocessor
    {
        private void OnPreprocessModel()
        {
            if (assetImporter is not ModelImporter importer) return;
            importer.animationRotationError = 0.01f;
            importer.animationPositionError = 0.01f;
            importer.animationScaleError = 0.01f;
        }

        private void OnPostprocessModel(GameObject gameObject)
        {
            //…Ë÷√π«˜¿Àı∑≈
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            foreach (var tf in transforms)
            {
                tf.localScale = Vector3.one;
            }
        }
    }
}
