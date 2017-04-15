using Newtonsoft.Json;
using NTierUoWExampleApp.Core.BindingModels.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTierUoWExampleApp.Mvc.Models
{
    public class JqGridPostData
    {
        // For use this object in GET/POST request from 
        // in jQgrid need to set configuration  
        // what defines JSON POST object properties
        // prmNames: {
        //    search:'search',
        //    rows: 'pageSize',
        //    sort: 'sortColumn',
        //    order: 'sortOrder'
        //},

        public bool Search { get; set; } //  _search - determines if filtering is used 
        public string Filters { get; set; } // if filtering is used, it provides information in JSON-format on its parameters
        public int Page { get; set; } // page - number of page
        public int PageSize { get; set; } //rows -number of rows in page that need to be returned
        public string SortColumn { get; set; } //sidx - sort column name default is ""
        public GridSortOrder SortOrder { get; set; } //sord - type of sort asc/desc

        public Filter GetFilter()
        {

            if (this.Search)
            {
                var filter = JsonConvert.DeserializeObject<Filter>(Filters);
                return filter;
            }
            return null;
        }
    }
}