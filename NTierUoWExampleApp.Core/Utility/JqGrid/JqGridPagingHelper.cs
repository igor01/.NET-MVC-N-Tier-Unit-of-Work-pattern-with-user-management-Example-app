using NTierUoWExampleApp.Core.BindingModels.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Utility.JqGrid
{
    public class JqGridPagingHelper<T> where T : class
    {
        public JqGridPagingHelper(JqGridSearchModel searchModel)
        {
            PageSize = searchModel.PageSize;
            PageNumber = searchModel.Page;
            Skip = (PageSize * (PageNumber - 1)) < 0 ? 0 : PageSize * (PageNumber - 1);
            Result = new JqGridResultViewModel<T>();
        }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Skip { get; set; }

        public JqGridResultViewModel<T> Result { get; set; }


        public void SetResult(List<T> model, int totalRows)
        {
            Result.Data = model;
            Result.records = totalRows;
            Result.page = PageNumber;
            Result.total = (int)Math.Ceiling((float)totalRows / (float)PageSize);
        }
    }
}
