using UnityEngine;

namespace Graph
{
    public static class Texture2DExtension
    {
        /// <summary>
        /// テクスチャをクリア
        /// </summary>
        /// <param name="texture">描画対象テクスチャ</param>
        public static void Clear(this Texture2D texture)
        {
            texture.Clear(Color.clear);
        }

        /// <summary>
        /// テクスチャをクリア
        /// </summary>
        /// <param name="texture">描画対象テクスチャ</param>
        /// <param name="color">塗りつぶし色</param>
        public static void Clear(this Texture2D texture, Color color)
        {
            for (int x_i = 0; x_i < texture.width; x_i++)
            {
                for (int y_i = 0; y_i < texture.height; y_i++)
                {
                    texture.SetPixel(x_i, y_i, color);
                }
            }
        }

        /// <summary>
        /// テクスチャに1ピクセル書きこみ
        /// </summary>
        /// <param name="texture">描画対象テクスチャ</param>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <param name="color">描画色</param>
        public static void SetPixelSafe(this Texture2D texture, int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= texture.width || y >= texture.height)
            {
                return;
            }
            else texture.SetPixel(x, y, color);
        }

        /// <summary>
        /// テクスチャに直線書きこみ
        /// </summary>
        /// <param name="texture">描画対象テクスチャ</param>
        /// <param name="start">開始座標</param>
        /// <param name="end">終了座標</param>
        /// <param name="color">描画色</param>
        /// <param name="step">描画間隔（点線の描画）</param>
        public static void SetLine(this Texture2D texture, Vector2Int start, Vector2Int end, Color color, int step = 1)
        {
            int dx = end.x - start.x;
            int dy = end.y - start.y;
            int abs_dx = Mathf.Abs(dx);
            int abs_dy = Mathf.Abs(dy);
            Vector2Int from = new Vector2Int(start.x, start.y);
            Vector2Int to = new Vector2Int(end.x, end.y);

            // 左上から右下へ向かう直線ではないのなら、開始地点と終了地点を入れ替える
            if ((abs_dx > abs_dy && dx < 0) || (abs_dx < abs_dy && dy < 0))
            {
                from.x = end.x;
                from.y = end.y;
                to.x = start.x;
                to.y = start.y;

                dx = -dx;
                dy = -dy;
            }

            if (abs_dx == 0)
            {
                // 縦直線だったとき
                for (int y = from.y; y < to.y; y += step)
                {
                    texture.SetPixelSafe(from.x, y, color);
                }
            }
            else if (abs_dy == 0)
            {
                // 横直線だったとき
                for (int x = from.x; x < to.x; x += step)
                {
                    texture.SetPixelSafe(x, from.y, color);
                }
            }
            else if (abs_dx > abs_dy)
            {
                // 45度未満のとき
                int y = from.y;
                int add = (dy > 0) ? 1 : -1;
                float error = 0;

                for (int x = from.x; x < to.x; x += step)
                {
                    texture.SetPixelSafe(x, y, color);
                    error += 2 * dy * add;
                    if (error > dx)
                    {
                        y += add;
                        texture.SetPixelSafe(x, y, color);
                        error -= 2 * dx;
                    }
                }
            }
            else if (abs_dx < abs_dy)
            {
                // 45度より急のとき
                int x = from.x;
                int add = (dx > 0) ? 1 : -1;
                float error = 0;

                for (int y = from.y; y < to.y; y += step)
                {
                    texture.SetPixelSafe(x, y, color);
                    error += 2 * dx * add;
                    if (error > dy)
                    {
                        x += add;
                        texture.SetPixelSafe(x, y, color);
                        error -= 2 * dy;
                    }
                }
            }
            else
            {
                // 45度の斜めだったとき
                int add = (dy > 0) ? 1 : -1;

                for (int i = 0; i < abs_dx; i += step)
                {
                    texture.SetPixelSafe(from.x + i, from.y + i * add, color);
                }
            }

        }

        /// <summary>
        /// 点の描画
        /// </summary>
        /// <param name="texture">描画対象テクスチャ</param>
        /// <param name="point">描画位置</param>
        /// <param name="size">直径</param>
        /// <param name="color">描画色</param>
        public static void SetPoint(this Texture2D texture, Vector2Int point, int size, Color color)
        {
            int x0 = point.x;
            int y0 = point.y;
            int r = Mathf.Max(size / 2, 1) - 1;

            for (int y_i = -r; y_i < r + 1; y_i++)
            {
                for (int x_i = -r; x_i < r + 1; x_i++)
                {
                    texture.SetPixelSafe(x0 + x_i, y0 + y_i, color);
                }
            }
        }
    }
}