using PrimeTween;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace ElfSoft.Framework
{
    public static class UIElementEx
    {
        /// <summary>
        /// 从资产克隆元素
        /// </summary>
        public static AsyncOperationHandle<VisualTreeAsset> CloneTreeAsset(VisualElement target, object key)
        {
            var handle = Addressables.LoadAssetAsync<VisualTreeAsset>(key);
            var tree = handle.WaitForCompletion();
            tree.CloneTree(target);
            return handle;
        }

        public static Tween TweenScale(VisualElement element, float startValue = 1, float endValue = 1.2f, float duration = 0.16f, Ease ease = Ease.InOutBounce)
        {
            return Tween.Custom(element, startValue, endValue, duration, (e, newVal) => e.style.scale = new Vector2(newVal, newVal), ease)
                .OnComplete(element, e => e.style.scale = StyleKeyword.Null);
        }

        public static async Awaitable MoveElementToAsync(VisualElement element, VisualElement target)
        {
            await Awaitable.NextFrameAsync();
            MoveElementTo(element, target);
        }

        public static void MoveElementTo(VisualElement element, VisualElement target)
        {
            var rect = target.LocalToWorld(target.contentRect);
            rect = element.parent.WorldToLocal(rect);
            element.style.left = rect.xMin - element.parent.resolvedStyle.borderLeftWidth;
            element.style.top = rect.yMin - element.parent.resolvedStyle.borderTopWidth;
        }
    }
}
