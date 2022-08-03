using System;


namespace Bakery.BLL
{
    public class AdminLogBLL : BaseBLL<Model.AdminLogModel>
    {
        private static SortDic<Model.AdminLogModel> SortCreateDateDesc = new SortDic<Model.AdminLogModel>()
        {
            [p => p.CreateDate] = SqlSugar.OrderByType.Desc,
        };

        //public static PageList<Model.AdminLog> QueryPageListByAdminId(int AdminId, DateTime begin, DateTime end, int pageindex, int pagesize)
        //{
        //    if (AdminId == 0)
        //        return QueryPageList(pageindex, pagesize, p => p.CreateDate >= begin && p.CreateDate <= end, SortCreateDateDesc);
        //    return QueryPageList(pageindex, pagesize, p => p.AdminId == AdminId && p.CreateDate >= begin && p.CreateDate <= end, SortCreateDateDesc);
        //}

        //public static PageList<Model.AdminLog> QueryPageListByAdminIdAndType(int AdminId, EAdminLogType type, DateTime begin, DateTime end, int pageindex, int pagesize)
        //{
        //    if (AdminId == 0)
        //        return QueryPageList(pageindex, pagesize, p => p.AdminLogType == type && p.CreateDate >= begin && p.CreateDate <= end, SortCreateDateDesc);
        //    return QueryPageList(pageindex, pagesize, p => p.AdminId == AdminId && p.AdminLogType == type && p.CreateDate >= begin && p.CreateDate <= end, SortCreateDateDesc);
        //}

        public static PageList<Model.AdminLogModel> QueryPageListByAdminIdAndType(int AdminId, int type, DateTime begin, DateTime end, int pageindex, int pagesize)
        {
            using (var db = DBClient)
            {
                var count = 0;
                var list = db.Queryable<Model.AdminLogModel, Model.AdminModel>((p, s) => p.AdminId == s.AdminId)
                    .Where((p, s) => p.CreateDate >= begin && p.CreateDate <= end)
                    .WhereIF(AdminId != 0, (p, s) => p.AdminId == AdminId)
                    .WhereIF(type != 0, (p, s) => p.AdminLogType == (EAdminLogType)type)
                    .OrderBy((p, s) => p.CreateDate, SqlSugar.OrderByType.Desc)
                    .Select((p, s) => new Model.AdminLogModel() { AdminId = p.AdminId, AdminLogType = p.AdminLogType, Account = s.Account, ClientIP = p.ClientIP, CreateDate = p.CreateDate,  AdminLogId = p.AdminLogId })
                    .ToPageList(pageindex, pagesize, ref count);
                return new PageList<Model.AdminLogModel>(list, count);
            }
        }
    }
}
