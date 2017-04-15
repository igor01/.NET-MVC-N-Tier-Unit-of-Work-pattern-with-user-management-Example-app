using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.BindingModels.JqGrid
{
    public class JqGridSearchModel
    {
        public bool Search { get; set; } //  _search - determines if filtering is used 
        public Filter Filters { get; set; } // if filtering is used, it provides information in JSON-format on its parameters
        public int Page { get; set; } // page - number of page
        public int PageSize { get; set; } //rows -number of rows in page that need to be returned
        public string SortColumn { get; set; } //sidx - sort column name default is ""
        public GridSortOrder SortOrder { get; set; } //sort - type of sort asc/desc


        // Calculated
        public int PageIndex { get { return (Convert.ToInt32(this.Page) - 1); } }
        public int SkipValue { get { return this.PageIndex * this.PageSize; } }
        private int totalPages { get; set; }



        public int GetTotalPages(int totalRecords)
        {
            this.totalPages = (int)Math.Ceiling((float)totalRecords / (float)this.PageSize);
            return this.totalPages;
        }
        public int GetPageIndex()
        {
            return (Convert.ToInt32(this.Page) - 1);
        }
    }

    public enum GridSortOrder
    {
        ASC,
        DESC
    }
    public class Filter
    {
        public GroupOperator GroupOp { get; set; } //operator which applies to a group of rules, Rules (AND and OR)
        public List<Rule> Rules { get; set; } // a set of rules
    }
    public enum GroupOperator
    {
        AND,
        OR
    }
    public class Rule
    {
        public string Field { get; set; } // index value of columns - field where filtering is done
        public string Op { get; set; } // an operation which the user selected
        public string Data { get; set; } // a filter which the user entered
    }
}
