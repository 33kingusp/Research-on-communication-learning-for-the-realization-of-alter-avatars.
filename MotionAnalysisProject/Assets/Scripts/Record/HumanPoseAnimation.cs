using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Record
{
    public class HumanPoseAnimation
    {
        private readonly List<Vector3> positions = new List<Vector3>();
        private readonly List<Quaternion> rotations = new List<Quaternion>();
        private readonly List<float[]> muscles = new List<float[]>();

        public int FrameCount { get; private set; } = 0;
        public ReadOnlyCollection<Vector3> Positions { get; }
        public ReadOnlyCollection<Quaternion> Rotations { get; }
        public ReadOnlyCollection<float[]> Muscles { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HumanPoseAnimation()
        {
            Positions = new ReadOnlyCollection<Vector3>(positions);
            Rotations = new ReadOnlyCollection<Quaternion>(rotations);
            Muscles = new ReadOnlyCollection<float[]>(muscles);
        }

        /// <summary>
        /// アニメーションにフレームを追加
        /// </summary>
        /// <param name="position">BodyPosition</param>
        /// <param name="rotation">BodyRotation</param>
        /// <param name="muscle">Muscle</param>
        public void AddFrame(Vector3 position, Quaternion rotation, float[] muscle)
        {
            var mus = new float[muscle.Length];
            muscle.CopyTo(mus, 0);

            positions.Add(new Vector3(position.x, position.y, position.z));
            rotations.Add(new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w));
            muscles.Add(mus);
            FrameCount++;
        }

        /// <summary>
        /// 指定フレームのポーズを取得
        /// </summary>
        /// <param name="frameIndex">フレーム</param>
        /// <returns>HumanPose</returns>
        public HumanPose GetHumanPose(int frameIndex)
        {
            var humanPose = new HumanPose
            {
                bodyPosition = Positions[frameIndex],
                bodyRotation = Rotations[frameIndex],
                muscles = Muscles[frameIndex]
            };

            return humanPose;
        }

        /// <summary>
        /// CSV出力用のヘッダーを取得
        /// </summary>
        /// <returns>ヘッダー文字列</returns>
        private static string GetMuscleHeader()
        {
            var muscleNames = HumanTrait.MuscleName.Aggregate((a, b) => a + "," + b);
            return $"position.x,position.y,position.z,rotation.x,rotation.y,rotation.z,rotation.w,{muscleNames}";
        }

        /// <summary>
        /// アニメーションをCSV形式で保存
        /// </summary>
        /// <param name="animation">アニメーション</param>
        /// <param name="fileName">出力ファイル名</param>
        public static void SaveAnimationFile(HumanPoseAnimation animation, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine(GetMuscleHeader());

                for (int frame_i = 0; frame_i < animation.FrameCount; frame_i++)
                {
                    var pos = animation.positions[frame_i];
                    var rot = animation.rotations[frame_i];
                    var muscle = animation.muscles[frame_i]
                        .Select(a => a.ToString())
                        .Aggregate((a, b) => a + "," + b);

                    sw.WriteLine($"{pos.x},{pos.y},{pos.z},{rot.x},{rot.y},{rot.z},{rot.w},{muscle}");
                }
            }
        }

        /// <summary>
        /// ファイルからアニメーションを読み込み
        /// </summary>
        /// <param name="fileName">入力ファイル名</param>
        /// <returns></returns>
        public static HumanPoseAnimation LoadAnimationFile(string fileName)
        {
            var animation = new HumanPoseAnimation();

            using (StreamReader sr = new StreamReader(fileName))
            {
                // ヘッダーを捨てる
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(',').Select(e => float.Parse(e)).ToArray();

                    var pos = new Vector3(line[0], line[1], line[2]);
                    var rot = new Quaternion(line[3], line[4], line[5], line[6]);
                    var muscle = line.Skip(7).ToArray();
                    animation.AddFrame(pos, rot, muscle);
                }
            }

            return animation;
        }
    }
}