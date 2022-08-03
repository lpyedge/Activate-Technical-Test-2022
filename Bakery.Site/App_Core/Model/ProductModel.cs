using System;

namespace Bakery.Model
{
    [Serializable]
    [SqlSugar.SugarTable(nameof(ProductModel),IsDisabledUpdateAll=true)]
    public class ProductModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, Length = 28)]
        public string Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SqlSugar.SugarColumn(IndexGroupNameList = new[] {nameof(ProductModel)})]
        public int Sort { get; set; }
        
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Memo
        /// </summary>
        [SqlSugar.SugarColumn(Length = 4000)]
        public string Memo { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [SqlSugar.SugarColumn(Length = 50)]
        public string Category { get; set; }


        /// <summary>
        /// Icon
        /// </summary>
        [SqlSugar.SugarColumn(Length = 100)]
        public string Icon { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public double Price { get; set; }
        
        /// <summary>
        /// Cost
        /// </summary>
        public double Cost { get; set; }
        
        /// <summary>
        /// IsShow
        /// </summary>
        [SqlSugar.SugarColumn(IndexGroupNameList = new []{nameof(ProductModel)})]
        public bool IsShow { get; set; }

    }
}