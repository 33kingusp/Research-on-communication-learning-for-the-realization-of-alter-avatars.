using System;

namespace MatrixUtilities
{
    public class Matrix
    {
        public readonly int row;
        public readonly int col;
        public readonly double[,] matrix;

        /// <summary>
        /// 行列クラス
        /// コンストラクタ
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        public Matrix(int row, int col)
        {
            if (row < 0 || col < 0)
            {
                throw new ArgumentOutOfRangeException("Rows and columns must be greater than or equal to 1.");
            }

            this.row = row;
            this.col = col;
            matrix = new double[row, col];
        }

        /// <summary>
        /// 行列に値を代入する
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <param name="value">値</param>
        public void SetValue(int row, int col, double value)
        {
            if (row >= this.row)
            {
                throw new ArgumentOutOfRangeException($"SetValue: The variable 'row({row})' exceeds the number of rows({this.row}).");
            }
            if (col >= this.col)
            {
                throw new ArgumentOutOfRangeException($"SetValue: The variable 'col({col})' exceeds the number of columns({this.col}).");
            }

            matrix[row, col] = value;
        }

        /// <summary>
        /// 行列に行を代入する
        /// </summary>
        /// <param name="col">行</param>
        /// <param name="values">値</param>
        public void SetValuesRow(int col, double[] values)
        {
            if (col >= this.col)
            {
                throw new ArgumentOutOfRangeException($"SetValuesRow: The variable 'row({row})' exceeds the number of rows({this.row}).");
            }
            if (values.Length != this.row)
            {
                throw new ArgumentException($"");
            }
            for (int r_i = 0; r_i < values.Length; r_i++)
            {
                SetValue(r_i, col, values[r_i]);
            }
        }

        /// <summary>
        /// 行列に列を代入する
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="values">値</param>
        public void SetValuesCol(int row, double[] values)
        {
            if (row >= this.row)
            {
                throw new ArgumentOutOfRangeException($"SetValuesCol: The variable 'row({row})' exceeds the number of rows({this.row}).");
            }
            if (values.Length != this.col)
            {
                throw new ArgumentException($"");
            }
            for (int c_i = 0; c_i < values.Length; c_i++)
            {
                SetValue(row, c_i, values[c_i]);
            }
        }

        /// <summary>
        /// 行列の値を取得する
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <returns>行列の値</returns>
        public double GetValue(int row, int col)
        {
            if (row >= this.row)
            {
                throw new ArgumentOutOfRangeException($"GetValue: The variable 'row({row})' exceeds the number of rows({this.row}).");
            }
            if (col >= this.col)
            {
                throw new ArgumentOutOfRangeException($"GetValue: The variable 'col({col})' exceeds the number of columns({this.col}).");
            }

            return matrix[row, col];
        }

        /// <summary>
        /// 行列内の要素を文字列化
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            var str = "";

            for (int r_i = 0; r_i < row; r_i++)
            {
                for (int c_i = 0; c_i < col; c_i++)
                {
                    str += GetValue(r_i, c_i) + "\t";
                }
                str += "\n";
            }

            return str;
        }

        /// <summary>
        /// 加算を計算する
        /// </summary>
        /// <param name="left">行列A</param>
        /// <param name="right">行列B</param>
        /// <returns>結果行列</returns>
        public static Matrix operator +(Matrix left, Matrix right)
        {
            if (left.row != right.row || left.col != right.col)
            {
                throw new ArgumentException("The number of rows and columns of the A and B matrices must be the same.");
            }

            var resultMatrix = new Matrix(left.row, left.col);

            for (int r_i = 0; r_i < resultMatrix.row; r_i++)
            {
                for (int c_i = 0; c_i < resultMatrix.col; c_i++)
                {
                    resultMatrix.SetValue(r_i, c_i, left.GetValue(r_i, c_i) + right.GetValue(r_i, c_i));
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// 減算を計算する
        /// </summary>
        /// <param name="left">行列A</param>
        /// <param name="right">行列B</param>
        /// <returns>結果行列</returns>
        public static Matrix operator -(Matrix left, Matrix right)
        {
            if (left.row != right.row || left.col != right.col)
            {
                throw new ArgumentException("The number of rows and columns of the A and B matrices must be the same.");
            }

            var resultMatrix = new Matrix(left.row, left.col);

            for (int r_i = 0; r_i < resultMatrix.row; r_i++)
            {
                for (int c_i = 0; c_i < resultMatrix.col; c_i++)
                {
                    resultMatrix.SetValue(r_i, c_i, left.GetValue(r_i, c_i) - right.GetValue(r_i, c_i));
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// 積を計算する
        /// </summary>
        /// <param name="left">行列A</param>
        /// <param name="right">行列B</param>
        /// <returns>結果行列</returns>
        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.col != right.row)
            {
                throw new ArgumentException("The number of columns in A and the number of rows in B must be the same.");
            }
            var size = left.col;

            Matrix resultMatrix = new Matrix(left.row, right.col);

            for (int r_i = 0; r_i < left.row; r_i++)
            {
                for (int c_i = 0; c_i < right.col; c_i++)
                {
                    double subTotal = 0;

                    for (int s_i = 0; s_i < size; s_i++)
                    {
                        subTotal += left.GetValue(r_i, s_i) * right.GetValue(s_i, c_i);
                    }

                    resultMatrix.SetValue(r_i, c_i, subTotal);
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// 行列の右側に行列を結合する
        /// </summary>
        /// <param name="left">元になる行列</param>
        /// <param name="right">追加する行列</param>
        /// <returns>結果行列</returns>
        public static Matrix JoinToRight(Matrix left, Matrix right)
        {
            if (left.row != right.row)
            {
                throw new ArgumentException("The number of rows in the matrix must be the same.");
            }

            var resultMatrix = new Matrix(left.row, left.col + right.col);

            for (int r_i = 0; r_i < left.row; r_i++)
            {
                for (int c_i = 0; c_i < left.col; c_i++)
                {
                    resultMatrix.SetValue(r_i, c_i, left.GetValue(r_i, c_i));
                }
            }

            for (int r_i = 0; r_i < right.row; r_i++)
            {
                for (int c_i = 0; c_i < right.col; c_i++)
                {
                    resultMatrix.SetValue(r_i, left.col + c_i, right.GetValue(r_i, c_i));
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// 行列の下側に行列を結合する
        /// </summary>
        /// <param name="top">元になる行列</param>
        /// <param name="bottom">追加する行列</param>
        /// <returns>結果行列</returns>
        public static Matrix JoinToBottom(Matrix top, Matrix bottom)
        {
            if (top.col != bottom.col)
            {
                throw new ArgumentException("The number of rows in the matrix must be the same.");
            }

            var resultMatrix = new Matrix(top.row + bottom.row, top.col);

            for (int r_i = 0; r_i < top.row; r_i++)
            {
                for (int c_i = 0; c_i < top.col; c_i++)
                {
                    resultMatrix.SetValue(r_i, c_i, top.GetValue(r_i, c_i));
                }
            }

            for (int r_i = 0; r_i < bottom.row; r_i++)
            {
                for (int c_i = 0; c_i < bottom.col; c_i++)
                {
                    resultMatrix.SetValue(top.row + r_i, c_i, bottom.GetValue(r_i, c_i));
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// 転置行列を返す
        /// </summary>
        /// <param name="origin">元になる行列</param>
        /// <returns>結果行列</returns>
        public static Matrix Transpose(Matrix origin)
        {
            var resultMatrix = new Matrix(origin.col, origin.row);

            for (int c_i = 0; c_i < origin.col; c_i++)
            {
                for (int r_i = 0; r_i < origin.row; r_i++)
                {
                    resultMatrix.SetValue(c_i, r_i, origin.GetValue(r_i, c_i));
                }
            }

            return resultMatrix;
        }
    }
}