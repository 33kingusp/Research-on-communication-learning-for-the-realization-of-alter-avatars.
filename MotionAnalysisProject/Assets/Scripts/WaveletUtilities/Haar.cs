using System;
using System.Linq;

namespace WaveletUtilities
{
    public static class Haar
    {
        /// <summary>
        /// ハールウェーブレットで変換
        /// </summary>
        /// <param name="data">一次元配列データ</param>
        /// <param name="m">レベル</param>
        /// <returns>出力データ</returns>
        public static double[] Transform(double[] data)
        {
            // 変換する配列は、入力データの長さを満たす2の乗数
            int resultLength = (int)Math.Pow(2, (int)Math.Log(data.Length, 2) + 1);
            var result = new double[resultLength];
            data.CopyTo(result, 0);
            var m = (int)Math.Log(result.Length, 2) - 1;

            for (int m_i = m; m_i >= 0; m_i--)
            {
                int n = (int)Math.Pow(2, m_i);
                int step = result.Length / n;

                for (int i = 0; i < result.Length; i += step)
                {
                    var left = result[i];
                    var right = result[i + step / 2];

                    var avg = (left + right) / Math.Sqrt(2);
                    var dif = (left - right) / Math.Sqrt(2);

                    /*
                    var avg = (left + right) * 0.5;
                    var dif = (left - right) * 0.5;
                    */

                    result[i] = avg;
                    result[i + step / 2] = dif;
                }
            }
            return result;
        }

        /// <summary>
        /// ハールウェーブレットで逆変換
        /// </summary>
        /// <param name="data">一次元配列データ</param>
        /// <param name="m">レベル</param>
        /// <returns>出力データ</returns>
        public static double[] InverseTransform(double[] data)
        {
            var result = new double[data.Length];
            data.CopyTo(result, 0);

            var m = (int)Math.Log(result.Length, 2);

            for (int m_i = 0; m_i < m; m_i++)
            {
                int n = (int)Math.Pow(2, m_i);
                int step = result.Length / n;

                for (int i = 0; i < result.Length; i += step)
                {
                    var left = result[i];
                    var right = result[i + step / 2];

                    
                    result[i] = (left + right) / Math.Sqrt(2);
                    result[i + step / 2] = (left - right) / Math.Sqrt(2);
                    
                    /*
                    result[i] = (left + right);
                    result[i + step / 2] = (left - right);
                    */
                }
            }
            return result;
        }

        /// <summary>
        /// 変換済みデータを圧縮する
        /// </summary>
        /// <param name="transformedData">変換済みデータ</param>
        /// <param name="threshold">圧縮率(0.0 ~ 1.0) </param>
        /// <returns>出力データ</returns>
        public static double[] Compress(double[] transformedData, double threshold)
        {
            var result = new double[transformedData.Length];
            transformedData.CopyTo(result, 0);

            // 上位閾値%の値を取得し、その値をスレッショルドに設定
            var th = result.Select(x => Math.Abs(x))
                .OrderByDescending(x => x)
                .Skip((int)(transformedData.Length * (1 - threshold)))
                .FirstOrDefault();

            // 閾値未満の値を0に置き換え
            result = result
                .Select(x => Math.Abs(x) < th ? 0 : x)
                .ToArray();

            return result;
        }
    }
}