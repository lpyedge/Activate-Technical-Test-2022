using System;
using System.Collections.Generic;
using System.Linq;

namespace Bakery.BLL
{
    public class ProductBLL : BaseBLL<Model.ProductModel>
    {
        private static SortDic<Model.ProductModel> SortAsc = new SortDic<Model.ProductModel>()
        {
            [p => p.Sort] = SqlSugar.OrderByType.Asc,
        };
        public static Model.ProductModel QueryModelByProductId(string productId)
        {
            return QueryModel(p => p.Id == productId);
        }
        
        public static List<Model.ProductModel> QueryListByStoreShow()
        {
            List<Model.ProductModel> data;
            using (var conn = DBClient)
            {
                data = conn.Queryable<Model.ProductModel>()
                    .Where(p => p.IsShow == true)
                    .OrderBy(p => p.Sort, SqlSugar.OrderByType.Asc)
                    .ToList();
            }

            return data;
        }

        public static void DeleteByIdsAndUserId(List<string> ids)
        {
            Delete(p => ids.Contains(p.Id));
        }
    }
}