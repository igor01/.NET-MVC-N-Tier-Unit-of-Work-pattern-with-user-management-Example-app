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
    public class LambdaExpressionsHelper
    {
        public static Expression<Func<TClass, bool>> CreateLambdaDependencyFilterExpression<TClass>(string prop, string dependencyId)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");

            //p.Property
            var property = Expression.PropertyOrField(item, prop);
            var method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
            var argument = Expression.Constant(dependencyId);

            var bodyItem = Expression.Call(property, method, argument);

            return (Expression<Func<TClass, bool>>)Expression.Lambda(bodyItem, item);
        }

        public static Expression<Func<TClass, bool>> CreateLambdaDependencyFilterExpression<TClass>(string prop, Guid? dependencyId)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");

            Type t = typeof(TClass);
            PropertyInfo propInfo = t.GetProperty(prop);

            var type = propInfo.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(type);

            //p.FirstName
            Expression left = Expression.PropertyOrField(item, prop);

            Expression right;

            if (underlyingType == null)
            {
                right = Expression.Constant(dependencyId, typeof(Guid));
            }
            else
            {
                right = Expression.Constant(dependencyId, typeof(Guid?));
            }

            var bodyItem = Expression.Equal(left, right);

            return (Expression<Func<TClass, bool>>)Expression.Lambda(bodyItem, item);
        }

        public static Expression<Func<TClass, bool>> CreateLambdaDependencyFilterExpression<TClass>(string prop, int dependencyId)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");

            //p.FirstName
            Expression left = Expression.PropertyOrField(item, prop);
            Expression right = Expression.Constant(dependencyId, typeof(int));
            var bodyItem = Expression.Equal(left, right);

            return (Expression<Func<TClass, bool>>)Expression.Lambda(bodyItem, item);
        }

        public static Expression<Func<TClass, bool>> CreateLambdaFilterExpression<TClass>(JqGridSearchModel search)
        {
            if (search != null)
            {
                if (search.Filters != null)
                {
                    //lists for filters
                    List<MethodCallExpression> lambdaBodyArray = new List<MethodCallExpression>();
                    List<BinaryExpression> lambdaBinaryBodyArray = new List<BinaryExpression>();

                    //p =>
                    var item = Expression.Parameter(typeof(TClass), "p");

                    //create expression for each filter
                    for (var i = 0; i < search.Filters.Rules.Count; i++)
                    {
                        Type t = typeof(TClass);

                        PropertyInfo propInfo = t.GetProperty(search.Filters.Rules[i].Field);

                        var type = propInfo.PropertyType;
                        var underlyingType = Nullable.GetUnderlyingType(type);//will be null if type is not nullable

                        if (propInfo.PropertyType.FullName == typeof(System.Int32).ToString())
                        {
                            int value;
                            bool isParsed;

                            isParsed = int.TryParse(search.Filters.Rules[i].Data, out value);

                            if (isParsed)
                            {

                                if (search.Filters.Rules[i].Op == "lt")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(int));
                                    var bodyItem = Expression.LessThan(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else if (search.Filters.Rules[i].Op == "le")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(int));
                                    var bodyItem = Expression.LessThanOrEqual(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else if (search.Filters.Rules[i].Op == "gt")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(int));
                                    var bodyItem = Expression.GreaterThan(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else if (search.Filters.Rules[i].Op == "ge")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(int));
                                    var bodyItem = Expression.GreaterThanOrEqual(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else if (search.Filters.Rules[i].Op == "ne")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(int));
                                    var bodyItem = Expression.NotEqual(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else
                                {
                                    //p.Property
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(int));
                                    var bodyItem = Expression.Equal(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }

                            }
                        }
                        else if (propInfo.PropertyType.FullName == typeof(System.Double).ToString())
                        {
                            double value;
                            bool isParsed;

                            isParsed = double.TryParse(search.Filters.Rules[i].Data, out value);

                            if (isParsed)
                            {
                                if (search.Filters.Rules[i].Op == "lt")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(double));
                                    var bodyItem = Expression.LessThan(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else if (search.Filters.Rules[i].Op == "le")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(double));
                                    var bodyItem = Expression.LessThanOrEqual(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else if (search.Filters.Rules[i].Op == "gt")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(double));
                                    var bodyItem = Expression.GreaterThan(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else if (search.Filters.Rules[i].Op == "ge")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(double));
                                    var bodyItem = Expression.GreaterThanOrEqual(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else if (search.Filters.Rules[i].Op == "ne")
                                {
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(double));
                                    var bodyItem = Expression.NotEqual(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }
                                else
                                {
                                    //p.Property
                                    Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                    Expression right = Expression.Constant(value, typeof(double));
                                    var bodyItem = Expression.Equal(left, right);

                                    lambdaBinaryBodyArray.Add(bodyItem);
                                }

                            }
                        }
                        else if (propInfo.PropertyType.FullName == typeof(System.Guid).ToString())
                        {
                            //do nothing
                        }
                        else if (propInfo.PropertyType.FullName == typeof(System.DateTime).ToString())
                        {
                            bool isDateTime = false;
                            DateTime dt = new DateTime();

                            try
                            {
                                dt = Convert.ToDateTime(search.Filters.Rules[i].Data);
                                isDateTime = true;
                            }
                            catch (Exception e)
                            {

                            }

                            if (isDateTime && search.Filters.Rules[i].Op == "lt")
                            {
                                Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                Expression right = Expression.Constant(dt, typeof(DateTime));
                                var bodyItem = Expression.LessThan(left, right);

                                lambdaBinaryBodyArray.Add(bodyItem);
                            }

                            else if (isDateTime && search.Filters.Rules[i].Op == "le")
                            {
                                DateTime dt1 = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
                                Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                Expression right = Expression.Constant(dt1, typeof(DateTime));
                                var bodyItem = Expression.LessThanOrEqual(left, right);

                                lambdaBinaryBodyArray.Add(bodyItem);
                            }

                            else if (isDateTime && search.Filters.Rules[i].Op == "ne")
                            {
                                Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                Expression right = Expression.Constant(dt, typeof(DateTime));
                                var bodyItem = Expression.NotEqual(left, right);

                                lambdaBinaryBodyArray.Add(bodyItem);
                            }

                            else if (isDateTime && search.Filters.Rules[i].Op == "gt")
                            {
                                DateTime dt1 = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
                                Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                Expression right = Expression.Constant(dt1, typeof(DateTime));
                                var bodyItem = Expression.GreaterThan(left, right);

                                lambdaBinaryBodyArray.Add(bodyItem);
                            }

                            else if (isDateTime && search.Filters.Rules[i].Op == "ge")
                            {
                                Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                Expression right = Expression.Constant(dt, typeof(DateTime));
                                var bodyItem = Expression.GreaterThanOrEqual(left, right);

                                lambdaBinaryBodyArray.Add(bodyItem);
                            }

                            else if (isDateTime)
                            {
                                Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                Expression right = Expression.Constant(dt, typeof(DateTime));
                                var bodyItem = Expression.GreaterThanOrEqual(left, right);

                                DateTime dt1 = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
                                Expression right1 = Expression.Constant(dt1, typeof(DateTime));
                                var bodyItem1 = Expression.LessThanOrEqual(left, right1);

                                var combinedLambda = Expression.AndAlso(bodyItem, bodyItem1);

                                lambdaBinaryBodyArray.Add(combinedLambda);
                            }

                        }
                        else if (underlyingType != null && underlyingType.FullName == typeof(System.DateTime).ToString())
                        {
                            bool isDateTime = false;
                            DateTime dt = new DateTime();

                            try
                            {
                                dt = Convert.ToDateTime(search.Filters.Rules[i].Data);
                                isDateTime = true;
                            }
                            catch (Exception e)
                            {

                            }

                            if (isDateTime)
                            {
                                Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                Expression right = Expression.Constant(dt, typeof(DateTime?));
                                var bodyItem = Expression.GreaterThanOrEqual(left, right);

                                DateTime? dt1 = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
                                Expression right1 = Expression.Constant(dt1, typeof(DateTime?));
                                var bodyItem1 = Expression.LessThanOrEqual(left, right1);

                                var combinedLambda = Expression.AndAlso(bodyItem, bodyItem1);

                                lambdaBinaryBodyArray.Add(combinedLambda);
                            }

                        }
                        else
                        {
                            //p.Property

                            if (search.Filters.Rules[i].Op == "ne")
                            {
                                Expression left = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                Expression right = Expression.Constant(search.Filters.Rules[i].Data, typeof(string));
                                var bodyItem = Expression.NotEqual(left, right);

                                lambdaBinaryBodyArray.Add(bodyItem);
                            }

                            else
                            {

                                var property = Expression.PropertyOrField(item, search.Filters.Rules[i].Field);
                                var toLower = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
                                var method = GetMethod(search.Filters.Rules[i].Op);
                                var argument = Expression.Constant(search.Filters.Rules[i].Data);

                                var tempBodyItem = Expression.Call(property, toLower);
                                var bodyItem = Expression.Call(tempBodyItem, method, argument);

                                lambdaBodyArray.Add(bodyItem);
                            }
                        }
                    }

                    Expression tempLambdaBody = null;

                    //join filters to one expression
                    string searchString = search.Filters.Rules.FirstOrDefault().Data;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        GroupOperator ope = search.Filters.GroupOp;

                        if (ope == GroupOperator.AND)
                        {
                            if (lambdaBodyArray != null && lambdaBodyArray.Count != 0)
                            {
                                tempLambdaBody = lambdaBodyArray[0];

                                for (var j = 0; j < lambdaBodyArray.Count - 1; j++)
                                {
                                    var second = lambdaBodyArray[j + 1];
                                    var lambdaBody = Expression.AndAlso(tempLambdaBody, second);
                                    tempLambdaBody = lambdaBody;
                                }
                            }

                            if (lambdaBinaryBodyArray != null && lambdaBinaryBodyArray.Count > 0)
                            {
                                if (tempLambdaBody == null)
                                {
                                    tempLambdaBody = lambdaBinaryBodyArray[0];
                                }

                                for (var j = 0; j < lambdaBinaryBodyArray.Count - 1; j++)
                                {
                                    var binaryItem = lambdaBinaryBodyArray[j + 1];
                                    var lambdaBody = Expression.AndAlso(tempLambdaBody, binaryItem);
                                    tempLambdaBody = lambdaBody;
                                }
                            }
                        }
                        else
                        {
                            if (lambdaBodyArray != null && lambdaBodyArray.Count != 0)
                            {
                                tempLambdaBody = lambdaBodyArray[0];

                                for (var j = 0; j < lambdaBodyArray.Count - 1; j++)
                                {
                                    var second = lambdaBodyArray[j + 1];
                                    var lambdaBody = Expression.OrElse(tempLambdaBody, second);
                                    tempLambdaBody = lambdaBody;
                                }
                            }

                            if (lambdaBinaryBodyArray != null && lambdaBinaryBodyArray.Count > 0)
                            {
                                if (tempLambdaBody == null)
                                {
                                    tempLambdaBody = lambdaBinaryBodyArray[0];
                                }

                                for (var j = 0; j < lambdaBinaryBodyArray.Count; j++)
                                {
                                    var binaryItem = lambdaBinaryBodyArray[j];
                                    var lambdaBody = Expression.OrElse(tempLambdaBody, binaryItem);
                                    tempLambdaBody = lambdaBody;
                                }
                            }
                        }
                    }

                    //create final expression
                    if (tempLambdaBody != null)
                    {
                        var lambda = (Expression<Func<TClass, bool>>)Expression.Lambda(tempLambdaBody, item);

                        return lambda;
                    }
                }
            }
            return null;
        }

        public static Expression<Func<TClass, object>> CreateLambdaSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, object>>)Expression.Lambda<Func<TClass, object>>(property, item);

            return lambda;
        }



        public static Expression<Func<TClass, DateTime>> CreateLambdaDateTimeSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, DateTime>>)Expression.Lambda<Func<TClass, DateTime>>(property, item);

            return lambda;
        }

        public static Expression<Func<TClass, DateTime?>> CreateLambdaNullableDateTimeSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, DateTime?>>)Expression.Lambda<Func<TClass, DateTime?>>(property, item);

            return lambda;
        }

        public static Expression<Func<TClass, int>> CreateLambdaIntSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, int>>)Expression.Lambda<Func<TClass, int>>(property, item);

            return lambda;
        }

        public static Expression<Func<TClass, int?>> CreateLambdaNullableIntSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, int?>>)Expression.Lambda<Func<TClass, int?>>(property, item);

            return lambda;
        }

        public static Expression<Func<TClass, float>> CreateLambdaFloatSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, float>>)Expression.Lambda<Func<TClass, float>>(property, item);

            return lambda;
        }

        public static Expression<Func<TClass, float?>> CreateLambdaNullableFloatSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, float?>>)Expression.Lambda<Func<TClass, float?>>(property, item);

            return lambda;
        }

        public static Expression<Func<TClass, double>> CreateLambdaDoubleSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, double>>)Expression.Lambda<Func<TClass, double>>(property, item);

            return lambda;
        }

        public static Expression<Func<TClass, double?>> CreateLambdaNullableDoubleSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, double?>>)Expression.Lambda<Func<TClass, double?>>(property, item);

            return lambda;
        }

        public static Expression<Func<TClass, Guid>> CreateLambdaGuidSortExpression<TClass>(string sortColumn)
        {
            //p =>
            var item = Expression.Parameter(typeof(TClass), "p");
            var property = Expression.PropertyOrField(item, sortColumn);
            var lambda = (Expression<Func<TClass, Guid>>)Expression.Lambda<Func<TClass, Guid>>(property, item);

            return lambda;
        }

        private static MethodInfo GetMethod(string op)
        {
            if (op == "eq")
            {
                return typeof(string).GetMethod("Equals", new[] { typeof(string) });
            }

            else if (op == "bw")
            {
                return typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            }

            else if (op == "ew")
            {
                return typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            }

            return typeof(string).GetMethod("Contains", new[] { typeof(string) });
        }
    }
}
