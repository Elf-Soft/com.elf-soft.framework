using System.Collections.Generic;

namespace ElfSoft.DialogueSystem
{
    public interface IView
    {
        public void SwitchNode(Node node);
        public void ShowText(string text);
        public void ShowChoices(IEnumerable<Option> options);
        public void ClearChoices();
    }
}
