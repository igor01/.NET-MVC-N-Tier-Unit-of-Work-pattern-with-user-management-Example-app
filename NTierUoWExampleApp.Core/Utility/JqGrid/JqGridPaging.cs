using NTierUoWExampleApp.Core.BindingModels.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Utility.JqGrid
{
    public class JqGridPaging<T, TViewModel>
         where T : class
         where TViewModel : class
    {

        public JqGridPaging(string identityName, JqGridSearchModel searchModel)
        {
            this.searchModel = searchModel;
            this.JqGridPagingHelper = new JqGridPagingHelper<TViewModel>(searchModel);
            this.identityName = identityName;
        }

        public JqGridPaging(IQueryable<T> queryable, string identityName, JqGridSearchModel searchModel)
        {
            this.searchModel = searchModel;
            this.JqGridPagingHelper = new JqGridPagingHelper<TViewModel>(searchModel);
            this.queryable = queryable;
            this.identityName = identityName;
        }

        public JqGridPaging(IQueryable<T> queryable, string identityName, JqGridSearchModel searchModel, string dependencyProperty, string dependencyIdTypeofString)
        {
            this.searchModel = searchModel;
            this.JqGridPagingHelper = new JqGridPagingHelper<TViewModel>(searchModel);
            this.queryable = queryable;
            this.identityName = identityName;
            this.dependencyProperty = dependencyProperty;
            this.dependencyIdTypeofString = dependencyIdTypeofString;
        }

        public JqGridPaging(IQueryable<T> queryable, string identityName, JqGridSearchModel searchModel, string dependencyProperty, Guid dependencyIdTypeofGuid)
        {
            this.searchModel = searchModel;
            this.JqGridPagingHelper = new JqGridPagingHelper<TViewModel>(searchModel);
            this.queryable = queryable;
            this.identityName = identityName;
            this.dependencyProperty = dependencyProperty;
            this.dependencyIdTypeofGuid = dependencyIdTypeofGuid;
        }

        public JqGridPaging(IQueryable<T> queryable, string identityName, JqGridSearchModel searchModel, string dependencyProperty, int dependencyIdTypeofInt)
        {
            this.searchModel = searchModel;
            this.JqGridPagingHelper = new JqGridPagingHelper<TViewModel>(searchModel);
            this.queryable = queryable;
            this.identityName = identityName;
            this.dependencyProperty = dependencyProperty;
            this.dependencyIdTypeofInt = dependencyIdTypeofInt;
        }

        private IQueryable<T> queryable { get; set; }
        private IQueryable<T> queryable1 { get; set; }
        private List<T> model { get; set; }
        private JqGridPagingHelper<TViewModel> JqGridPagingHelper { get; set; }
        private JqGridResultViewModel<T> JqGridResultViewModel { get; set; }
        private JqGridSearchModel searchModel { get; set; }
        private int totalRows = 0;


        private string identityName { get; set; }


        private string dependencyIdTypeofString = null;
        private Guid dependencyIdTypeofGuid = Guid.Empty;
        private int dependencyIdTypeofInt = 0;
        private string dependencyProperty { get; set; }




        private Expression<Func<T, bool>> dependencyFilterExpression = null;

        private Expression<Func<T, bool>> filterExpression = null;


        public Expression<Func<T, bool>> filter = null;
        public int Skip { get; set; }
        public int Take { get; set; }


        public void SetJoinPagingParameters()
        {
            Skip = JqGridPagingHelper.Skip;
            Take = JqGridPagingHelper.PageSize;
            filter = LambdaExpressionsHelper.CreateLambdaFilterExpression<T>(searchModel);
        }

        public List<T> GetResult()
        {

            if (dependencyIdTypeofString != null)
            {
                dependencyFilterExpression = LambdaExpressionsHelper.CreateLambdaDependencyFilterExpression<T>(dependencyProperty, dependencyIdTypeofString);
            }
            if (dependencyIdTypeofGuid != null && dependencyIdTypeofGuid != Guid.Empty)
            {
                dependencyFilterExpression = LambdaExpressionsHelper.CreateLambdaDependencyFilterExpression<T>(dependencyProperty, dependencyIdTypeofGuid);
            }
            if (dependencyIdTypeofInt != 0)
            {
                dependencyFilterExpression = LambdaExpressionsHelper.CreateLambdaDependencyFilterExpression<T>(dependencyProperty, dependencyIdTypeofInt);
            }


            if (searchModel != null)
            {
                if (searchModel.Search && (filterExpression = LambdaExpressionsHelper.CreateLambdaFilterExpression<T>(searchModel)) != null)
                {
                    //check if dependency exist
                    if (dependencyFilterExpression == null)
                    {
                        //check for sort column
                        if (!String.IsNullOrWhiteSpace(searchModel.SortColumn))
                        {
                            //find type of sort column
                            Type t = typeof(T);
                            PropertyInfo propInfo = t.GetProperty(searchModel.SortColumn);

                            var type = propInfo.PropertyType;
                            var underlyingType = Nullable.GetUnderlyingType(type);//will be null if type is not nullable

                            //sort can be string, int or DateTime
                            if (propInfo.PropertyType.FullName == typeof(System.DateTime).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get sort of DateTime ascending
                                    GetByFilterAndSortedOfTypeDateTimeAscending();
                                }
                                else
                                {
                                    //get by sort of DateTime descending
                                    GetByFilterAndSortedOfTypeDateTimeDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.DateTime).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get sort of DateTime ascending
                                    GetByFilterAndSortedOfTypeNullableDateTimeAscending();
                                }
                                else
                                {
                                    //get by sort of DateTime descending
                                    GetByFilterAndSortedOfTypeNullableDateTimeDescending();
                                }
                            }
                            else if (propInfo.PropertyType.FullName == typeof(System.Int32).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByFilterAndSortedOfTypeIntAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByFilterAndSortedOfTypeIntDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.Int32).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByFilterAndSortedOfTypeNullableIntAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByFilterAndSortedOfTypeNullableIntDescending();
                                }
                            }
                            else if (propInfo.PropertyType.FullName == typeof(System.Double).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByFilterAndSortedOfTypeDoubleAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByFilterAndSortedOfTypeDoubleDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.Double).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByFilterAndSortedOfTypeNullableDoubleAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByFilterAndSortedOfTypeNullableDoubleDescending();
                                }
                            }
                            else
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByFilterAndSortedOfTypeStringAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByFilterAndSortedOfTypeStringDescending();
                                }
                            }
                        }
                        else
                        {
                            //there is no sort so get by filter and default sort by id in db

                            var identityNameType = GetType<T>(identityName);

                            if (identityNameType.Equals(typeof(int)))
                            {
                                //Get by dependency and default sort of type int
                                GetByFilterAndDefaultSortOfTypeInt();
                            }
                            if (identityNameType.Equals(typeof(double)))
                            {
                                //Get by dependency and default sort of type int
                                GetByFilterAndDefaultSortOfTypeDouble();
                            }
                            if (identityNameType.Equals(typeof(string)))
                            {
                                //Get by dependency and default sort of type string
                                GetByFilterAndDefaultSortOfTypeString();
                            }
                            if (identityNameType.Equals(typeof(Guid)))
                            {
                                //Get by dependency and default sort of type Guid
                                GetByFilterAndDefaultSortOfTypeGuid();
                            }
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(searchModel.SortColumn))
                        {
                            //find type of sort column
                            Type t = typeof(T);
                            PropertyInfo propInfo = t.GetProperty(searchModel.SortColumn);

                            var type = propInfo.PropertyType;
                            var underlyingType = Nullable.GetUnderlyingType(type);//will be null if type is not nullable

                            //sort can be string, int or DateTime
                            if (propInfo.PropertyType.FullName == typeof(System.DateTime).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by dependency && sort of DateTime ascending
                                    GetByDependencyAndFilterAndSortOfTypeDateTimeAscending();
                                }
                                else
                                {
                                    //get by dependency && sort of DateTime descending
                                    GetByDependencyAndFilterAndSortOfTypeDateTimeDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.DateTime).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by dependency && sort of DateTime ascending
                                    GetByDependencyAndFilterAndSortOfTypeNullableDateTimeAscending();
                                }
                                else
                                {
                                    //get by dependency && sort of DateTime descending
                                    GetByDependencyAndFilterAndSortOfTypeNullableDateTimeDescending();
                                }
                            }
                            else if (propInfo.PropertyType.FullName == typeof(System.Int32).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByDependencyAndFilterAndSortOfTypeIntAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByDependencyAndFilterAndSortOfTypeIntDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.Int32).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByDependencyAndFilterAndSortOfTypeNullableIntAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByDependencyAndFilterAndSortOfTypeNullableIntDescending();
                                }
                            }
                            else if (propInfo.PropertyType.FullName == typeof(System.Double).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByDependencyAndFilterAndSortOfTypeDoubleAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByDependencyAndFilterAndSortOfTypeDoubleDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.Double).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByDependencyAndFilterAndSortOfTypeNullableDoubleAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByDependencyAndFilterAndSortOfTypeNullableDoubleDescending();
                                }
                            }
                            else
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by dependency && sort of string ascending
                                    GetByDependencyAndFilterAndSortOfTypeStringAscending();
                                }
                                else
                                {
                                    //get by dependency && sort of string descending
                                    GetByDependencyAndFilterAndSortOfTypeStringDescending();
                                }
                            }
                        }
                        else
                        {
                            //there is no sort so get by dependency and filter and use default sort by id in db
                            var identityNameType = GetType<T>(identityName);

                            if (identityNameType.Equals(typeof(int)))
                            {
                                //Get by dependency and default sort of type int
                                GetByDependencyAndFilterAndDefaultSortOfTypeInt();
                            }
                            if (identityNameType.Equals(typeof(string)))
                            {
                                //Get by dependency and default sort of type string
                                GetByDependencyAndFilterAndDefaultSortOfTypeString();
                            }
                            if (identityNameType.Equals(typeof(Guid)))
                            {
                                //Get by dependency and default sort of type Guid
                                GetByDependencyAndFilterAndDefaultSortOfTypeGuid();
                            }
                        }
                    }

                }
                else
                {
                    /////////////////    there is no search     ///////////////////////

                    //check if dependency exist
                    if (dependencyFilterExpression == null)
                    {
                        //check for sort column
                        if (!String.IsNullOrWhiteSpace(searchModel.SortColumn))
                        {
                            //find type of sort column
                            Type t = typeof(T);
                            PropertyInfo propInfo = t.GetProperty(searchModel.SortColumn);

                            var type = propInfo.PropertyType;
                            var underlyingType = Nullable.GetUnderlyingType(type);//will be null if type is not nullable
                            var returnType = underlyingType ?? type;

                            //sort can be string, int or DateTime
                            if (propInfo.PropertyType.FullName == typeof(System.DateTime).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get sort of DateTime ascending
                                    GetSortedOfTypeDateTimeAscending();
                                }
                                else
                                {
                                    //get by sort of DateTime descending
                                    GetSortedOfTypeDateTimeDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.DateTime).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get sort of DateTime ascending
                                    GetSortedOfTypeNullableDateTimeAscending();
                                }
                                else
                                {
                                    //get by sort of DateTime descending
                                    GetSortedOfTypeNullableDateTimeDescending();
                                }
                            }
                            else if (propInfo.PropertyType.FullName == typeof(System.Int32).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetSortedOfTypeIntAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetSortedOfTypeIntDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.Int32).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetSortedOfTypeNullableIntAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetSortedOfTypeNullableIntDescending();
                                }
                            }
                            else if (propInfo.PropertyType.FullName == typeof(System.Double).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetSortedOfTypeDoubleAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetSortedOfTypeDoubleDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.Double).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetSortedOfTypeNullableDoubleAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetSortedOfTypeNullableDoubleDescending();
                                }
                            }
                            else
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetSortedOfTypeStringAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetSortedOfTypeStringDescending();
                                }
                            }
                        }
                        else
                        {
                            //get by default sort
                            var identityNameType = GetType<T>(identityName);

                            if (identityNameType.Equals(typeof(int)))
                            {
                                //Get by dependency and default sort of type int
                                GetByDefaultSortOfTypeInt();
                            }
                            if (identityNameType.Equals(typeof(double)))
                            {
                                //Get by dependency and default sort of type int
                                GetByDefaultSortOfTypeDouble();
                            }
                            else if (identityNameType.Equals(typeof(string)))
                            {
                                //Get by dependency and default sort of type string
                                GetByDefaultSortOfTypeString();
                            }
                            else if (identityNameType.Equals(typeof(Guid)))
                            {
                                //Get by dependency and default sort of type Guid
                                GetByDefaultSortOfTypeGuid();
                            }
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(searchModel.SortColumn))
                        {
                            //find type of sort column
                            Type t = typeof(T);
                            PropertyInfo propInfo = t.GetProperty(searchModel.SortColumn);

                            var type = propInfo.PropertyType;
                            var underlyingType = Nullable.GetUnderlyingType(type);//will be null if type is not nullable
                            var returnType = underlyingType ?? type;

                            //sort can be string, int or DateTime
                            if (underlyingType != null && underlyingType.FullName == typeof(System.DateTime).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by dependency && sort of DateTime ascending
                                    GetByDependencyAndSortOfTypeNullableDateTimeAscending();
                                }
                                else
                                {
                                    //get by dependency && sort of DateTime descending
                                    GetByDependencyAndSortOfTypeNullableDateTimeDescending();
                                }
                            }

                            else if (returnType.FullName == typeof(System.DateTime).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by dependency && sort of DateTime ascending
                                    GetByDependencyAndSortOfTypeDateTimeAscending();
                                }
                                else
                                {
                                    //get by dependency && sort of DateTime descending
                                    GetByDependencyAndSortOfTypeDateTimeDescending();
                                }
                            }
                            else if (propInfo.PropertyType.FullName == typeof(System.Int32).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByDependencyAndSortOfTypeIntAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByDependencyAndSortOfTypeIntDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.Int32).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByDependencyAndSortOfTypeNullableIntAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByDependencyAndSortOfTypeNullableIntDescending();
                                }
                            }
                            else if (propInfo.PropertyType.FullName == typeof(System.Double).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByDependencyAndSortOfTypeDoubleAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByDependencyAndSortOfTypeDoubleDescending();
                                }
                            }
                            else if (underlyingType != null && underlyingType.FullName == typeof(System.Double).ToString())
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by sort of string ascending
                                    GetByDependencyAndSortOfTypeNullableDoubleAscending();
                                }
                                else
                                {
                                    //get by sort of string descending
                                    GetByDependencyAndSortOfTypeNullableDoubleDescending();
                                }
                            }
                            else if (propInfo.PropertyType.BaseType.FullName == typeof(System.Enum).ToString())
                            {

                            }
                            else
                            {
                                //check for sort order
                                if (searchModel.SortOrder == GridSortOrder.ASC)
                                {
                                    //get by dependency && sort of string ascending
                                    GetByDependencyAndSortOfTypeStringAscending();
                                }
                                else
                                {
                                    //get by dependency && sort of string descending
                                    GetByDependencyAndSortOfTypeStringDescending();
                                }
                            }
                        }
                        else
                        {
                            //there is no sort so get by dependency and default sort by id in db
                            var identityNameType = GetType<T>(identityName);

                            if (identityNameType.Equals(typeof(int)))
                            {
                                //Get by dependency and default sort of type int
                                GetByDependencyAndDefaultSortOfTypeInt();
                            }
                            if (identityNameType.Equals(typeof(string)))
                            {
                                //Get by dependency and default sort of type string
                                GetByDependencyAndDefaultSortOfTypeString();
                            }
                            if (identityNameType.Equals(typeof(Guid)))
                            {
                                //Get by dependency and default sort of type Guid
                                GetByDependencyAndDefaultSortOfTypeGuid();
                            }
                        }
                    }
                }
                return model;
            }
            return null;
        }

        public JqGridResultViewModel<TViewModel> GetJqGridResult(List<TViewModel> model)
        {
            JqGridPagingHelper.SetResult(model, totalRows);
            return JqGridPagingHelper.Result;
        }


        #region helper methods

        ///////////////  sort methods  ////////////////  

        // DateTime
        private void GetSortedOfTypeDateTimeAscending()
        {
            model = (from m in queryable
                    .OrderBy(LambdaExpressionsHelper.CreateLambdaDateTimeSortExpression<T>(searchModel.SortColumn))
                    .Skip(JqGridPagingHelper.Skip)
                    .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeNullableDateTimeAscending()
        {
            model = (from m in queryable
                    .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableDateTimeSortExpression<T>(searchModel.SortColumn))
                    .Skip(JqGridPagingHelper.Skip)
                    .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeDateTimeDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeNullableDateTimeDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }


        //int
        private void GetSortedOfTypeIntAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeNullableIntAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableIntSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeIntDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeNullableIntDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableIntSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }


        //double
        private void GetSortedOfTypeDoubleAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeNullableDoubleAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableDoubleSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeDoubleDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeNullableDoubleDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableDoubleSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }


        //string
        private void GetSortedOfTypeStringAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetSortedOfTypeStringDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(searchModel.SortColumn))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        //defaulr sort

        private void GetByDefaultSortOfTypeInt()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(identityName))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetByDefaultSortOfTypeDouble()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(identityName))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetByDefaultSortOfTypeString()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(identityName))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        private void GetByDefaultSortOfTypeGuid()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaGuidSortExpression<T>(identityName))
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Count();
        }

        ///////////////  dependency and sort methods  ////////////////  

        // DateTime

        private void GetByDependencyAndSortOfTypeDateTimeAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeNullableDateTimeAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeDateTimeDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeNullableDateTimeDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }


        //int
        private void GetByDependencyAndSortOfTypeIntAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeNullableIntAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableIntSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeIntDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeNullableIntDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableIntSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }


        //double
        private void GetByDependencyAndSortOfTypeDoubleAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeNullableDoubleAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeDoubleDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeNullableDoubleDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }


        //string
        private void GetByDependencyAndSortOfTypeStringAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndSortOfTypeStringDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }


        //default
        private void GetByDependencyAndDefaultSortOfTypeInt()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(identityName))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndDefaultSortOfTypeString()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(identityName))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }

        private void GetByDependencyAndDefaultSortOfTypeGuid()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaGuidSortExpression<T>(identityName))
                     .Where(dependencyFilterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Count();
        }


        ///////////////  sort and filter methods  ////////////////  

        // DateTime
        private void GetByFilterAndSortedOfTypeDateTimeAscending()
        {
            model = (from m in queryable
                    .OrderBy(LambdaExpressionsHelper.CreateLambdaDateTimeSortExpression<T>(searchModel.SortColumn))
                    .Where(filterExpression)
                    .Skip(JqGridPagingHelper.Skip)
                    .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeNullableDateTimeAscending()
        {
            model = (from m in queryable
                    .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableDateTimeSortExpression<T>(searchModel.SortColumn))
                    .Where(filterExpression)
                    .Skip(JqGridPagingHelper.Skip)
                    .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeDateTimeDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeNullableDateTimeDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }


        //int
        private void GetByFilterAndSortedOfTypeIntAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeNullableIntAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableIntSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeIntDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeNullableIntDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableIntSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        //double
        private void GetByFilterAndSortedOfTypeDoubleAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeNullableDoubleAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeDoubleDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeNullableDoubleDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        //string

        private void GetByFilterAndSortedOfTypeStringAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndSortedOfTypeStringDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(searchModel.SortColumn))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }

        private void GetByFilterAndDefaultSortOfTypeInt()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(identityName))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }
        private void GetByFilterAndDefaultSortOfTypeDouble()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(identityName))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }
        private void GetByFilterAndDefaultSortOfTypeString()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(identityName))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }
        private void GetByFilterAndDefaultSortOfTypeGuid()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaGuidSortExpression<T>(identityName))
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(filterExpression).Count();
        }


        ///////////////  dependency, filter and sort methods  ////////////////  

        // DateTime

        private void GetByDependencyAndFilterAndSortOfTypeDateTimeAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeNullableDateTimeAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeDateTimeDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeNullableDateTimeDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableDateTimeSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }


        //int
        private void GetByDependencyAndFilterAndSortOfTypeIntAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeNullableIntAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableIntSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeIntDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeNullableIntDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableIntSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        //double
        private void GetByDependencyAndFilterAndSortOfTypeDoubleAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeNullableDoubleAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaNullableDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeDoubleDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeNullableDoubleDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaNullableDoubleSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }


        //string
        private void GetByDependencyAndFilterAndSortOfTypeStringAscending()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndSortOfTypeStringDescending()
        {
            model = (from m in queryable
                     .OrderByDescending(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(searchModel.SortColumn))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        //default

        private void GetByDependencyAndFilterAndDefaultSortOfTypeInt()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaIntSortExpression<T>(identityName))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndDefaultSortOfTypeString()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaSortExpression<T>(identityName))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private void GetByDependencyAndFilterAndDefaultSortOfTypeGuid()
        {
            model = (from m in queryable
                     .OrderBy(LambdaExpressionsHelper.CreateLambdaGuidSortExpression<T>(identityName))
                     .Where(dependencyFilterExpression)
                     .Where(filterExpression)
                     .Skip(JqGridPagingHelper.Skip)
                     .Take(JqGridPagingHelper.PageSize)
                     select m).ToList();

            totalRows = queryable.Where(dependencyFilterExpression).Where(filterExpression).Count();
        }

        private Type GetType<T>(string value)
        {
            Type t = typeof(T);
            return t.GetProperty(value).PropertyType;
        }


        #endregion

    }
}
