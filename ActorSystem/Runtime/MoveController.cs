//using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ElfSoft.ActorSystem
{
    public class MoveController
    {
        private readonly MonoBehaviour movable;
        private readonly Transform transform;
        private readonly CharacterController cc;
        private Vector3 targetDirection;
        public const float DefaultGravity = -9.8f;


        public MoveController(MonoBehaviour movable)
        {
            this.movable = movable;
            transform = movable.transform;
            cc = movable.GetComponent<CharacterController>();
        }

        public void Move(Vector3 direction, float speed, float motionY = DefaultGravity)
        {
            speed *= Time.deltaTime;
            Vector3 motion = new(direction.x * speed, motionY, direction.z * speed);
            cc.Move(motion);
            //EventHub.SendEvent<MoveEvent>(e => e.Init(transform));
        }

        /// <summary>
        /// 检查是否需要执行转向
        /// </summary>
        private bool CheckRotation(Vector3 direction)
        {
            if (direction == Vector3.zero || direction == targetDirection) return false;
            targetDirection = direction;
            return true;
        }

        public void SetRotation(Vector3 direction)
        {
            if (!CheckRotation(direction)) return;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        public void SetRotationAsync(Vector3 direction, float rotSpeed = 15f)
        {
            if (!CheckRotation(direction)) return;
            //RotationAsync(direction).Forget();

            //async UniTaskVoid RotationAsync(Vector3 dir)
            //{
            //    while (transform.forward != dir)
            //    {
            //        if (dir != targetDirection || movable.destroyCancellationToken.IsCancellationRequested) return;
            //        var quaternion = Quaternion.LookRotation(dir);
            //        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, rotSpeed * Time.deltaTime);
            //        await UniTask.DelayFrame(1, PlayerLoopTiming.Update, movable.destroyCancellationToken);
            //    }
            //}

            _ = RotationAsync(direction);
            async Awaitable RotationAsync(Vector3 dir)
            {
                while (transform.forward != dir)
                {
                    if (dir != targetDirection || movable.destroyCancellationToken.IsCancellationRequested) return;
                    var quaternion = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, rotSpeed * Time.deltaTime);
                    await Awaitable.NextFrameAsync(movable.destroyCancellationToken);
                }
            }
        }
    }
}
