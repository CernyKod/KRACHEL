using System;
using System.Collections.Generic;
using System.Text;

namespace KRACHEL.Infrastructure.FFvideoBuilder
{
    internal class XFadeOffsetReslover
    {
        /// <summary>
        /// [s]
        /// </summary>
        private const double _transitionDuration = 1;

        private double _previousOffset = 0;


        public void ComputeOffsetData(double inputDuration, out double transitionDuratin, out double transitionOffset)
        {
            var currentOffset = inputDuration + _previousOffset - _transitionDuration;

            transitionDuratin = _transitionDuration;
            transitionOffset = currentOffset;

            _previousOffset = currentOffset;
        }

        public void Reset()
        {
            _previousOffset = 0;
        }

    }
}
