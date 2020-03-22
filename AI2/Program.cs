using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie0
{
    class Grafika
    {
        private Bitmap img;
        private Double[][] kernel = new Double[3][];
        public Grafika()
        {

        }

        public void LoadRGBMatrixFromPath(String path)
        {
            try
            {
                img = new Bitmap(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during loading RBG Matrix: {ex.Message}");
            }
        }

        public void ApplyFilterAndSave(FilterKind kind, String outputFileName)
        {
            Bitmap imgN = new Bitmap(img.Width, img.Height);
            String filename = String.Empty;

            if (kind == FilterKind.Sharp)
            {
                filename = $"{outputFileName.ToUpper()}_FILTR_SHARP";
                kernel[0] = new Double[] { -1, -1, -1 };
                kernel[1] = new Double[] { -1, 8, -1 };
                kernel[2] = new Double[] { -1, -1, -1 };
            }
            else if (kind == FilterKind.Gauss)
            {
                filename = $"{outputFileName.ToUpper()}_FILTR_GAUSS";
                kernel[0] = new Double[] { 1, 2, 1 };
                kernel[1] = new Double[] { 2, 4, 2 };
                kernel[2] = new Double[] { 1, 2, 1 };
            }
            else if (kind == FilterKind.Blur)
            {
                filename = $"{outputFileName.ToUpper()}_FILTR_BLUR";
                kernel[0] = new Double[] { 1, 1, 1 };
                kernel[1] = new Double[] { 1, 1, 1 };
                kernel[2] = new Double[] { 1, 1, 1 };
            }

            double maskSum = 0;
            foreach (double[] el in kernel)
                foreach (double i in el)
                    maskSum += i;

            for (Int32 w = 1; w < img.Width - 1; w++)
            {
                for (Int32 h = 1; h < img.Height - 1; h++)
                {
                    Color pxl = img.GetPixel(w, h);

                    Int32 elR = 0;
                    Int32 elG = 0;
                    Int32 elB = 0;

                    Int32 currX = w;
                    Int32 currY = h;

                    for (Int32 i = -1; i < 2; i++)
                    {
                        for (Int32 j = -1; j < 2; j++)
                        {
                            Int32 mask = (Int32)GetMaskValueForNeighbor(i + 1, j + 1);

                            Int32 neiR = img.GetPixel(currX + i, currY + j).R;
                            elR += neiR * mask;

                            Int32 neiG = img.GetPixel(currX + i, currY + j).G;
                            elG += neiG * mask;

                            Int32 neiB = img.GetPixel(currX + i, currY + j).B;
                            elB += neiB * mask;
                        }
                    }

                    if (maskSum != 0)
                    {
                        elR /= (Int32)maskSum;
                        elG /= (Int32)maskSum;
                        elB /= (Int32)maskSum;
                    }

                    elR = Clamp<Int32>(elR, 0, 255);
                    elG = Clamp<Int32>(elG, 0, 255);
                    elB = Clamp<Int32>(elB, 0, 255);


                    imgN.SetPixel(currX, currY, Color.FromArgb(elR, elG, elB));
                    elR = elG = elB = 0;
                }
            }

            imgN.Save($"C:/Users/Intel/Desktop/{filename}.png");

        }

        private Double GetMaskValueForNeighbor(Int32 x, Int32 y)
        {
            return kernel[x][y];
        }

        static void PrintImageToTextFile(Bitmap bmp, String path)
        {
            Bitmap img = bmp;
            StringBuilder sb = new StringBuilder();
            for (Int32 w = 0; w < img.Width; w++)
            {
                for (Int32 h = 0; h < img.Height; h++)
                {
                    Color pxl = img.GetPixel(w, h);

                    String pixelData = $"({pxl.R.ToString().PadLeft(3, '0')} {pxl.G.ToString().PadLeft(3, '0')} {pxl.B.ToString().PadLeft(3, '0')})";

                    sb.Append(pixelData);
                }
                sb.AppendLine();
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
            {
                file.WriteLine(sb.ToString());
            }
        }

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }

    public enum FilterKind
    {
        Gauss = 1,
        Blur = 2,
        Sharp = 3
    }
}
