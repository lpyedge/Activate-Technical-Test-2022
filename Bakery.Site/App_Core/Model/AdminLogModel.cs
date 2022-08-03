using SqlSugar;
using System;

namespace Bakery.Model
{
    [Serializable]
    [SqlSugar.SugarTable(nameof(AdminLogModel),IsDisabledUpdateAll=true)]
    public class AdminLogModel
    {
        /// <summary>
        ///编号
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, Length = 25)]
        public string AdminLogId { get; set; }

        /// <summary>
        ///管理员编号
        /// </summary>
        public int AdminId { get; set; }

        /// <summary>
        ///注释
        /// </summary>
        [SqlSugar.SugarColumn(Length = 4000)]
        public string Memo { get; set; }

        /// <summary>
        ///客户端IP
        /// </summary>
        [SqlSugar.SugarColumn(Length = 45)]
        public string ClientIP { get; set; }

        /// <summary>
        ///客户端信息
        /// </summary>
        [SqlSugar.SugarColumn(Length = 500)]
        public string UserAgent { get; set; }
        
        /// <summary>
        ///客户端语言
        /// </summary>
        [SqlSugar.SugarColumn(Length = 100)]
        public string AcceptLanguage { get; set; }
        
        /// <summary>
        ///商户日志类型
        /// </summary>
        [SqlSugar.SugarColumn(IndexGroupNameList = new []{nameof(AdminLogModel)})]
        public EAdminLogType AdminLogType { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        [SqlSugar.SugarColumn(IndexGroupNameList = new []{nameof(AdminLogModel)})]
        public DateTime CreateDate { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string Account { get; set; }
    }
}