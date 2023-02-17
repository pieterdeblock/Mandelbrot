using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mandelbrot_generator.Models
{
    public interface ILogic
    {
        public Task<uint[,]> CalulateMandelbrot(CancellationToken token, double xOffset, double yOffset, uint maxRow, uint maxColumn, int maxIterations, double scalefactor);
    }
}
