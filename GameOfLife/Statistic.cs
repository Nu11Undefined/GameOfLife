using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class Statistic
    {
        int[] _values;
        public int Count { get; set; }
        public int Max => _values.Max();
        public int Sum => _values.Sum();
        public int this[int index]
        {
            get => _values[index];
        }
        public Statistic(int count)
        {
            Count = count;
            _values = new int[count];
        }
        /// <summary>
        /// Добавить к данному индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        public void Add(int index)
        {
            if (index < Count)_values[index]++;
        }
        public void Delete(int index)
        {
            if (index < Count) _values[index]--;
        }
        /// <summary>
        /// Учесть изменение ячейки
        /// </summary>
        /// <param name="a1">Начальный возраст</param>
        /// <param name="a2">Конечный возраст</param>
        public void Invert(int a1, int a2)
        {
            Delete(a1);
            Add(a2);
        }
        /// <summary>
        /// Обнулить
        /// </summary>
        public void Clear()
        {
            for(int i = 0;i<_values.Length;i++)
            {
                _values[i] = 0;
            }
        }
    }
}
