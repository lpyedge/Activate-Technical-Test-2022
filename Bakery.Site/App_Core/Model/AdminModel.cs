using System;

namespace Bakery.Model
{
    [Serializable]
    [SqlSugar.SugarTable(nameof(AdminModel),IsDisabledUpdateAll=true)]
    public class AdminModel
    {
        [SqlSugar.SugarColumn(IsIdentity = true,IsPrimaryKey = true)]
        public int AdminId { get; set; }
        
        [SqlSugar.SugarColumn(Length = 25,IndexGroupNameList = new []{nameof(AdminModel)})]
        public string Account { get; set; }
        
        [SqlSugar.SugarColumn(Length = 40)]
        public string Password { get; set; }
        
        
        [SqlSugar.SugarColumn(Length = 10)]
        public string Salt { get; set; }
        
        
        [SqlSugar.SugarColumn(Length = 32)]
        public string ClientKey { get; set; }
        
        public DateTime CreateDate { get; set; } = DateTime.Now;
        
        public bool IsRoot { get; set; }
    }
}