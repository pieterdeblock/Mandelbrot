using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Mandelbrot_generator.Presentation;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace Mandelbrot_generator.Models
{
    class Logic : ILogic
    {
        public async Task<uint[,]> CalulateMandelbrot(CancellationToken token, double xOffset, double yOffset, uint maxRow, uint maxColumn, int maxIterations, double scalefactor)
        {
            uint[,] matrix = new uint[maxRow, maxColumn];
            double scale = 2 * 2.0d / Math.Min(maxRow, maxColumn);
            await Task.Run(() =>
            {
                ParallelOptions po = new ParallelOptions();
                po.CancellationToken = token;
                Parallel.For(0, maxRow, po, i =>
                {
                    double b = (maxRow / 2 - i) * scale / scalefactor + yOffset;
                    for (int j = 0; j < maxColumn; j++)
                    {
                        po.CancellationToken.ThrowIfCancellationRequested();
                        double a = (j - maxColumn / 2) * scale / scalefactor + xOffset;
                        matrix[i, j] = GenerateMandelbrot(a, b, maxIterations);
                    }
                });                
            });
            return matrix;
        }
        public uint GenerateMandelbrot(double a , double b, int maxIterations)
        {
            uint iteration = 0;
            double x = 0,
                y = 0,
                X = 0,
                Y = 0;
            do
            {
                X = x * x - y * y + a;
                Y = 2 * x * y + b;
                x = X;
                y = Y;
                iteration++;
            } while (x * x + y * y < 4 && iteration < maxIterations);
            return iteration;
        }
    }

}
