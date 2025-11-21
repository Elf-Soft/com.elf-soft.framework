namespace ElfSoft.Framework
{
    /// <summary>
    /// 一个保存事件数据的类
    /// </summary>
    public abstract class EventData
    {
        protected internal abstract void Reset();
    }


    public class EventData<T> : EventData
    {
        public object Sender { get; private set; }
        public T Args { get; private set; }


        public void Init(object sender, T args)
        {
            Sender = sender;
            Args = args;
        }

        protected internal override void Reset()
        {
            Sender = null;
            Args = default;
        }
    }
}
