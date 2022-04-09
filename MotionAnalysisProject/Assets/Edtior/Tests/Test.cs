using Graph;
using MotionAnalysis;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Tests
{
    public class MotionAnalisysTest
    {
        [Test]
        public void SplitTest()
        {
            var fileName = "record.csv";
            var graphData = GraphData<float>.LoadDataFromCSV(Path.Combine(Application.streamingAssetsPath, fileName));

            // 値の変化がないデータは除く
            graphData = graphData
                .Where(data => (data.YData.Max() - data.YData.Min()) > 0)
                .ToArray();

            using (var sw = new StreamWriter(Path.Combine(Application.streamingAssetsPath, "ResultsData.csv")))
            {
                sw.WriteLine("CompressThreshold,MotionMaxRange,Points");

                for (int t_i = 0; t_i <= 5; t_i++)
                {
                    for (int ct_i = 0; ct_i <= 10; ct_i++)
                    {
                        var commpressedDatas = new GraphData<float>[graphData.Length];
                        var differentiatedDatas = new GraphData<float>[graphData.Length];
                        var secondDifferentiatedDatas = new GraphData<float>[graphData.Length];
                        List<int> allPoints = new List<int>();

                        double compressThreshold = 1.00 - ct_i * 0.01;
                        int motionMaxRange = t_i * 30;

                        for (int data_i = 0; data_i < graphData.Length; data_i++)
                        {
                            // 正規化
                            var norm = graphData[data_i].YData.Select(e => (double)e).ToArray().Normalize();
                            // 階段関数に近似、圧縮
                            var comp = norm.Compress(compressThreshold);
                            // 一階微分
                            var diff = comp.Differentiate();
                            // 二階微分
                            var diff2 = diff.Differentiate();
                            // 変曲点
                            var points = diff2.FindInflectionPoints();
                            allPoints.AddRange(points);

                            commpressedDatas[data_i] = new GraphData<float>($"{graphData[data_i].Label}_comp", comp.Select(e => (float)e).ToArray(), GraphType.Line);
                            differentiatedDatas[data_i] = new GraphData<float>($"{graphData[data_i].Label}_diff", diff.Select(e => (float)e).ToArray(), GraphType.Line);
                            secondDifferentiatedDatas[data_i] = new GraphData<float>($"{graphData[data_i].Label}_diff2", diff2.Select(e => (float)e).ToArray(), GraphType.Line);
                        }
                        var addName = $"{compressThreshold}_{t_i}";

                        var resultPoints = MotionAnalysisUtilities.SummarizePoints(allPoints.OrderBy(e => e).ToArray(), motionMaxRange);
                        var pointData = new GraphData<float>($"Points", resultPoints, new float[resultPoints.Length], GraphType.Point);
                        
                        GraphData<float>.SaveDataToCSV(Path.Combine(Application.streamingAssetsPath, $"pointData_{addName}.csv"), pointData);
                        GraphData<float>.SaveDataToCSV(Path.Combine(Application.streamingAssetsPath, $"commpressedData_{addName}.csv"), commpressedDatas);
                        GraphData<float>.SaveDataToCSV(Path.Combine(Application.streamingAssetsPath, $"differentiatedData_{addName}.csv"), differentiatedDatas);
                        GraphData<float>.SaveDataToCSV(Path.Combine(Application.streamingAssetsPath, $"secondDifferentiatedData_{addName}.csv"), secondDifferentiatedDatas);
                        
                        sw.WriteLine($"{compressThreshold},{t_i},{resultPoints.Length}");
                        //Debug.Log($"{addName} : Points : {resultPoints.Length}");
                    }
                }
            }
        }
    }
}