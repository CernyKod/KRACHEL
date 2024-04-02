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
        private double _transitionDuration = 0;

        private double _previousOffset = 0;

        public XFadeOffsetReslover() { }

        public XFadeOffsetReslover(double transitionDuration) 
        {
            _transitionDuration = transitionDuration;
        }

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
