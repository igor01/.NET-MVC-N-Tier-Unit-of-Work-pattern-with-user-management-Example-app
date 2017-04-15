using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.JqGrid
{
    public class JqGridResultViewModel<T> where T : class
    {
        public int total { get; set; }

        public int page { get; set; }

        public int records { get; set; }

        public List<T> rows;

        public List<T> Data
        {
            get
            {
                return rows;
            }
            set
            {
                rows = value;
            }
        }
    }
}
