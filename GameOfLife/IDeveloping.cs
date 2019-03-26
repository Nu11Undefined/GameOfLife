using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    /// <summary>
    /// Определяет возможность перехода в следующее состояние
    /// </summary>
    public interface IDeveloping
    {
        /// <summary>
        /// Перейти на следующий этап развития
        /// </summary>
        void NextStage();
    }
}
