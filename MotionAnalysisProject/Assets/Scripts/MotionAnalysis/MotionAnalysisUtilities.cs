using System.Collections.Generic;
using WaveletUtilities;

namespace MotionAnalysis
{
    public static class MotionAnalysisUtilities
    {
        /// <summary>
        /// 階段関数に近似する
        /// </summary>
        /// <param name="data">入力データ</param>
        /// <param name="threshold">閾値</param>
        /// <returns>出力データ</returns>
        public static double[] Compress(this double[] data, double threshold)
        {
            var trans = Haar.Transform(data);
            var comp = Haar.Compress(trans, threshold);
            var result = Haar.InverseTransform(comp);
            return result;
        }

        /// <summary>
        /// 変曲点を求める
        /// </summary>
        /// <param name="data">2階微分済みデータ</param>
        /// <returns>出力データ</returns>
        public static int[] FindInflectionPoints(this double[] data)
        {
            var points = new List<int>();

            bool posiFlag = false;
            bool negaFlag = false;
            int tmpIndex = 0;

            // 変曲点を探す
            for (int i = 0; i < data.Length; i++)
            {
                if (!posiFlag || !negaFlag)
                {
                    if (data[i] > 0)
                    {
                        posiFlag = true;
                        tmpIndex = i;
                    }
                    if (data[i] < 0)
                    {
                        negaFlag = true;
                        tmpIndex = i;
                    }
                }
                else
                {
                    posiFlag = false;
                    negaFlag = false;
                    points.Add((tmpIndex + i) / 2);
                }

            }
            return points.ToArray();
        }

        /// <summary>
        /// 余分な点を除く
        /// </summary>
        /// <param name="points">x座標</param>
        /// <param name="range">範囲</param>
        /// <returns>出力データ</returns>
        public static int[] SummarizePoints(int[] points, int range)
        {
            var results = new List<int>();
            results.Add(points[0]);

            for (int i = 1; i < points.Length - 1; i++)
            {
                var diff = points[i + 1] - points[i];
                if (diff > range)
                {
                    results.Add(points[i]);
                }
            }

            return results.ToArray();
        }
    }
}