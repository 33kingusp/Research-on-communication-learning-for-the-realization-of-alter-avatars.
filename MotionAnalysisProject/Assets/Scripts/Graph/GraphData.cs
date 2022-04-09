using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Graph
{
    public class GraphData<T>
    {
        private readonly List<int> _xData;
        private readonly List<T> _yData;

        public string Label { get; private set; }
        public ReadOnlyCollection<int> XData { get; }
        public ReadOnlyCollection<T> YData { get; }
        public GraphType GraphType { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="label">グラフのラベル</param>
        /// <param name="graphType">グラフの種類</param>
        public GraphData(string label, GraphType graphType)
        {
            Label = label;
            GraphType = graphType;

            _xData = new List<int>();
            _yData = new List<T>();
            XData = new ReadOnlyCollection<int>(_xData);
            YData = new ReadOnlyCollection<T>(_yData);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="label">グラフのラベル</param>
        /// <param name="yData">縦軸のデータ配列</param>
        /// <param name="graphType">グラフの種類</param>
        public GraphData(string label, T[] yData, GraphType graphType)
        {
            Label = label;
            GraphType = graphType;

            _xData = new List<int>();
            _yData = new List<T>();

            _yData.AddRange(yData);
            for (int i = 0; i < _yData.Count; i++)
            {
                _xData.Add(i);
            }

            XData = new ReadOnlyCollection<int>(_xData);
            YData = new ReadOnlyCollection<T>(_yData);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="label">グラフのラベル</param>
        /// <param name="xData">横軸のデータ配列</param>
        /// <param name="yData">縦軸のデータ配列</param>
        /// <param name="graphType">グラフの種類</param>
        public GraphData(string label, int[] xData, T[] yData, GraphType graphType)
        {
            Label = label;
            GraphType = graphType;

            _xData = new List<int>();
            _yData = new List<T>();

            _xData.AddRange(xData);
            _yData.AddRange(yData);

            XData = new ReadOnlyCollection<int>(_xData);
            YData = new ReadOnlyCollection<T>(_yData);
        }

        /// <summary>
        /// データの追加
        /// </summary>
        /// <param name="value">縦軸の値</param>
        public void AddData(T value)
        {
            _xData.Add(_xData.Count);
            _yData.Add(value);
        }

        /// <summary>
        /// データの追加
        /// </summary>
        /// <param name="x">横軸の値</param>
        /// <param name="value">縦軸の値</param>
        public void AddData(int x, T value)
        {
            _xData.Add(x);
            _yData.Add(value);
        }

        /// <summary>
        /// CSVからデータを読み込みグラフデータの生成
        /// </summary>
        /// <param name="fileName">読み込むファイル名</param>
        /// <returns>グラフデータ</returns>
        public static GraphData<float>[] LoadDataFromCSV(string fileName)
        {
            GraphData<float>[] graphDatas;

            using (var sr = new StreamReader(fileName))
            {
                var headers = sr.ReadLine().Split(',');
                graphDatas = new GraphData<float>[headers.Length];

                for (int i = 0; i < headers.Length; i++)
                {
                    graphDatas[i] = new GraphData<float>(headers[i], GraphType.Line);
                }

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(',').Select(e => float.Parse(e)).ToArray();

                    for (int i = 0; i < graphDatas.Length; i++)
                    {
                        graphDatas[i].AddData(line[i]);
                    }
                }
                Debug.Log($"Loaded : {fileName}");
            }
            return graphDatas;
        }

        /// <summary>
        /// グラフデータをCSVとして保存
        /// </summary>
        /// <param name="fileName">書きこむファイル名</param>
        /// <param name="graphDatas">書きこむグラフデータ</param>
        public static void SaveDataToCSV(string fileName, params GraphData<float>[] graphDatas)
        {
            using (var sw = new StreamWriter(fileName))
            {
                var header = "frames," + graphDatas.Select(g => g.Label).Aggregate((a, b) => a + "," + b);
                sw.WriteLine(header);

                int dataCount = graphDatas.Select(g => g.YData.Count).Max();

                for (int i = 0; i < dataCount; i++)
                {
                    var line = graphDatas[0].XData[i] + "," + graphDatas.Select(g => g.YData[i].ToString()).Aggregate((a, b) => a + "," + b);
                    sw.WriteLine(line);
                }
                // Debug.Log($"Saved : {fileName}");
            }
        }
    }
}