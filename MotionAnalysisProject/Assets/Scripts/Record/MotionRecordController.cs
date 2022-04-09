using System.Collections;
using System.IO;
using UnityEngine;

namespace Record
{
    public class MotionRecordController : MonoBehaviour
    {
        [SerializeField] private Animator _animator = default;
        [SerializeField] private string _fileName = default;
        [SerializeField] private float _recordTime = default;
        [SerializeField] private bool _skipRecord = default;

        /// <summary>
        /// ボタン入力待ちコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            if (!_skipRecord)
            {
                Debug.Log("BeginRecord");
                yield return RecordMuscleCoroutine(Path.Combine(Application.streamingAssetsPath, _fileName), _recordTime);
                Debug.Log("EndRecord");
            }

            Debug.Log("BeginPlay");
            _animator.runtimeAnimatorController = null;
            yield return PlayMuscleCoroutine(Path.Combine(Application.streamingAssetsPath, _fileName));
            Debug.Log("EndPlay");
        }

        /// <summary>
        /// モーション収録コルーチン
        /// </summary>
        /// <param name="fileName">出力ファイル名</param>
        /// <param name="limitTime">収録時間（0で無限）</param>
        /// <returns></returns>
        private IEnumerator RecordMuscleCoroutine(string fileName, float limitTime = 0)
        {
            HumanPoseAnimation animation = new HumanPoseAnimation();
            var handler = new HumanPoseHandler(_animator.avatar, _animator.transform);
            var humanPose = new HumanPose();
            var time = 0.0f;

            while (true)
            {
                handler.GetHumanPose(ref humanPose);

                animation.AddFrame(humanPose.bodyPosition, humanPose.bodyRotation, humanPose.muscles);

                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;

                if (limitTime > 0 && time >= limitTime)
                {
                    break;
                }
            }
            HumanPoseAnimation.SaveAnimationFile(animation, fileName);
        }

        /// <summary>
        /// モーション再生コルーチン
        /// </summary>
        /// <param name="fileName">入力ファイル名</param>
        /// <returns></returns>
        private IEnumerator PlayMuscleCoroutine(string fileName)
        {
            var handler = new HumanPoseHandler(_animator.avatar, _animator.transform);
            var animation = HumanPoseAnimation.LoadAnimationFile(fileName);

            for (int frame_i = 0; frame_i < animation.FrameCount; frame_i++)
            {
                var humanPose = animation.GetHumanPose(frame_i);

                handler.SetHumanPose(ref humanPose);

                yield return new WaitForFixedUpdate();
            }
        }
    }
}