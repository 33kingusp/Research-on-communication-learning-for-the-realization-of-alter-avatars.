using System.Linq;

public static class ArrayExtension
{
    /// <summary>
    /// 最小値0、最大値1に正規化する
    /// </summary>
    /// <param name="array">入力データ</param>
    /// <returns>正規化された配列</returns>
    public static double[] Normalize(this double[] array)
    {
        var result = new double[array.Length];
        array.CopyTo(result, 0);

        var max = result.Max();
        var min = result.Min();
        result = result.Select(e => (e - min) / (max - min)).ToArray();

        return result;
    }

    /// <summary>
    /// 微分する
    /// (配列を前後要素の差分の配列にする)
    /// </summary>
    /// <param name="array">入力データ</param>
    /// <returns>微分された配列</returns>
    public static double[] Differentiate(this double[] array)
    {
        var result = new double[array.Length - 1];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = array[i + 1] - array[i];
        }

        return result;
    }
}