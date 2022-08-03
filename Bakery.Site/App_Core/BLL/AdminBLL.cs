using System.Collections.Generic;
using System.Linq;

namespace Bakery.BLL
{
    public class AdminBLL : BaseBLL<Model.AdminModel>
    {
        private static SortDic<Model.AdminModel> SortCreateDateDesc = new SortDic<Model.AdminModel>()
        {
            [p => p.CreateDate] = SqlSugar.OrderByType.Desc,
        };

        public static void DeleteById(int adminid)
        {
            Delete(p => p.AdminId == adminid);
        }
        public static Model.AdminModel QueryModelByAccount(string account)
        {
            return QueryModel(p => p.Account == account);
        }

        public static Model.AdminModel QueryModelById(int adminid)
        {
            return QueryModel(p => p.AdminId == adminid);
        }

        public static PageList<Model.AdminModel> QueryPageList(int pageindex, int pagesize)
        {
            using (var db = DBClient)
            {
                int total = 0;
                var data = db.Queryable<Model.AdminModel>()
                    .Select(p => new Model.AdminModel { Account = p.Account, AdminId = p.AdminId, IsRoot = p.IsRoot, CreateDate = p.CreateDate })
                    .OrderBy(SortCreateDateDesc.First().Key, SortCreateDateDesc.First().Value)
                    .ToPageList(pageindex, pagesize,ref total);
                return new PageList<Model.AdminModel>(data,total);
            }
        }
    }
}
