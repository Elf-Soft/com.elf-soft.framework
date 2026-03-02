using PrimeTween;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace ElfSoft.Framework
{
    [UxmlElement]
    public partial class PageBar : VisualElement
    {
        public readonly ClickElement clickElementPrevious, clickElementNext;
        private readonly TextElement pageElement, maxPageElement;
        private Tween previousTween, nextTween;
        private static readonly string ussClassName = "page-bar";
        private static readonly string buttonUssClassName = ussClassName + "__button";
        private static readonly string buttonPreviousUssClassName = buttonUssClassName + "--previous";
        private static readonly string buttonNextUssClassName = buttonUssClassName + "--next";
        private static readonly string iconUssClassName = buttonUssClassName + "__icon";
        private static readonly string textElementUssClassName = ussClassName + "__text-element";
        private static readonly string pageTextElementUssClassName = textElementUssClassName + "--page";
        private static readonly string slashTextElementUssClassName = textElementUssClassName + "--slash";
        private static readonly string maxPageTextElementUssClassName = textElementUssClassName + "--max-page";
        private int page, maxPage;
        public int Page
        {
            get => page;
            set
            {
                page = value;
                pageElement.text = value.ToString();
            }
        }
        public int MaxPage
        {
            get => maxPage;
            set
            {
                maxPage = value;
                maxPageElement.text = value.ToString();
            }
        }


        public PageBar()
        {
            AddToClassList(ussClassName);
            var handle = Addressables.LoadAssetAsync<StyleSheet>("UI/StyleSheet/PageBarStyles.uss");
            handle.Completed += h => styleSheets.Add(h.Result);

            clickElementPrevious = InitClickElement(buttonPreviousUssClassName);
            pageElement = InitTextElement("99", pageTextElementUssClassName);
            var slashElement = InitTextElement(@"/", slashTextElementUssClassName);
            maxPageElement = InitTextElement("99", maxPageTextElementUssClassName);
            clickElementNext = InitClickElement(buttonNextUssClassName);

            ClickElement InitClickElement(string ussClassName)
            {
                ClickElement elem = new();
                elem.AddToClassList(buttonUssClassName);
                elem.AddToClassList(ussClassName);
                var icon = new VisualElement();
                icon.AddToClassList(iconUssClassName);
                elem.Add(icon);
                Add(elem);
                return elem;
            }
            TextElement InitTextElement(string text, string ussClassName)
            {
                TextElement elem = new() { text = text };
                elem.AddToClassList(textElementUssClassName);
                elem.AddToClassList(ussClassName);
                Add(elem);
                return elem;
            }
            RegisterCallback<DetachFromPanelEvent>(e =>
            {
                var bar = e.target as PageBar;
                if (bar.previousTween.isAlive) bar.previousTween.Complete();
                if (bar.nextTween.isAlive) bar.nextTween.Complete();
            });
        }

        public void PreviousClickedAnimation()
        {
            if (previousTween.isAlive) previousTween.Complete();
            previousTween = UIElementEx.TweenScale(clickElementPrevious);
        }

        public void NextClickedAnimation()
        {
            if (nextTween.isAlive) nextTween.Complete();
            nextTween = UIElementEx.TweenScale(clickElementNext);
        }
    }
}
