using UnityEngine;

namespace ElfSoft.ActorSystem
{
    public static class StringHash
    {
        public static readonly int Idle = To(nameof(Idle));
        public static readonly int Move = To(nameof(Move));
        public static readonly int Walk = To(nameof(Walk));
        public static readonly int Run = To(nameof(Run));
        public static readonly int RunStart = To(nameof(RunStart));
        public static readonly int RunLoop = To(nameof(RunLoop));
        public static readonly int RunStop = To(nameof(RunStop));

        public static readonly int Horizontal = To(nameof(Horizontal));

        private static int To(string name) => Animator.StringToHash(name);
    }
}
