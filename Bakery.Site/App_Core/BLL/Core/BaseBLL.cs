//#define Cache //定义是否开启缓存
#if Cache
using Microsoft.Extensions.Caching.Memory;
#endif
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bakery.BLL
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BaseBLL
    {
        protected static DbType _DbType { get; private set; }

        protected static string _ConnStr { get; private set; }

        public static SqlSugarClient DBClient
        {
            get
            {
                var db = new SqlSugarClient(new ConnectionConfig
                {
                    ConnectionString = _ConnStr,
                    DbType = _DbType,
                    IsAutoCloseConnection = true,
                    ConfigureExternalServices = new ConfigureExternalServices
                    {
#if Cache
                        DataInfoCacheService = new SqlSugarDataInfoCacheService(),
#endif
                        SerializeService = new SqlSugarSerializeService()
                    },
                    //MoreSettings = new ConnMoreSettings()
                });
                return db;
            }
        }

        public static void Init(DbType dbType, string connStr)
        {
            _DbType = dbType;
            switch (dbType)
            {
                case DbType.MySql:
                case DbType.SqlServer:
                case DbType.Oracle:
                case DbType.PostgreSQL:
                default:
                    _ConnStr = connStr;
                    break;
                case DbType.Sqlite:
                    _ConnStr = "Data Source=" + connStr+";Cache=Shared;";
                    //https://www.runoob.com/sqlite/sqlite-pragma.html
                    //https://blog.csdn.net/comhaqs/article/details/53518133
                    //https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.data.sqlite.sqliteconnectionstringbuilder?view=msdata-sqlite-5.0.0
                    //除了 Data Source=xxxx;Cache=Shared; 其它配置参数在Microsoft.Data.Sqlite下调用都报错
                    //_ConnStr = "Data Source=" + connStr + "Data.db;Cache=Shared;Journal Mode=Off;Synchronous=Normal;Pooling=True;Max Pool Size=100;";
                    break;
            }
        }
        
        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void InitDataBase()
        {
            using (var db = DBClient)
            {
                db.DbMaintenance.CreateDatabase();
            }
        }
        
        /// <summary>
        /// 初始化数据表
        /// </summary>
        public static void InitDataTables()
        {
            using (var db = DBClient)
            {
                var modelTypeList = System.Reflection.Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(p =>
                    {
                        return p.IsClass && p.IsPublic 
                                         && p.CustomAttributes.Any(a=>a.AttributeType == typeof(SqlSugar.SugarTable));
                    }).ToArray();
                    
                db.CodeFirst.InitTables(modelTypeList);
            }
        }
        
        internal class SortDic<T> : Dictionary<Expression<Func<T, object>>, OrderByType>
        {
            public SortDic()
            {
            }

            public SortDic(SortDic<T> dic)
            {
                if (dic?.Count > 0)
                    foreach (var item in dic)
                        Add(item.Key, item.Value);
            }
        }

        #region TableColumnEdit

        public static bool ColumnAdd(string tablename, string columnname, string sqlsbtype, bool nullenbale,
            string defaultvalue = null)
        {
            using (var conn = DBClient)
            {
                var sql1 =
                    "IF NOT EXISTS (SELECT 1 FROM syscolumns INNER JOIN sysobjects ON sysobjects.id = syscolumns.id WHERE syscolumns.name = '" +
                    columnname + "' AND sysobjects.name = '" + tablename + "') ";
                var sql2 = "ALTER TABLE [" + tablename + "] ADD [" + columnname + "]  " + sqlsbtype + " " +
                           (nullenbale ? "NULL" : "NOT NULL") + " " +
                           (defaultvalue != null ? "DEFAULT " + defaultvalue : "");

                return conn.Ado.ExecuteCommand(sql1 + sql2) > 0;
            }
        }

        public static bool ColumnDrop(string tablename, string columnname)
        {
            using (var conn = DBClient)
            {
                var sql1 =
                    "IF EXISTS (SELECT 1 FROM syscolumns INNER JOIN sysobjects ON sysobjects.id = syscolumns.id WHERE syscolumns.name = '" +
                    columnname + "' AND sysobjects.name = '" + tablename + "') ";
                var sql2 = "ALTER TABLE [" + tablename + "] DROP COLUMN [" + columnname + "]";

                return conn.Ado.ExecuteCommand(sql1 + sql2) > 0;
            }
        }

        public static bool ColumnEdit(string tablename, string columnname, string sqlsbtype, bool nullenbale,
            string defaultvalue = null)
        {
            using (var conn = DBClient)
            {
                var sql1 =
                    "IF EXISTS (SELECT 1 FROM syscolumns INNER JOIN sysobjects ON sysobjects.id = syscolumns.id WHERE syscolumns.name = '" +
                    columnname + "' AND sysobjects.name = '" + tablename + "') ";
                var sql2 = "ALTER TABLE [" + tablename + "] ALTER COLUMN [" + columnname + "]  " + sqlsbtype +
                           " " + (nullenbale ? "NULL" : "NOT NULL") + " " +
                           (defaultvalue != null ? "DEFAULT " + defaultvalue : "");
                return conn.Ado.ExecuteCommand(sql1 + sql2) > 0;
            }
        }

        public static bool ColumnRename(string tablename, string columnname, string columnnamenew)
        {
            using (var conn = DBClient)
            {
                var sql1 =
                    "IF EXISTS (SELECT 1 FROM syscolumns INNER JOIN sysobjects ON sysobjects.id = syscolumns.id WHERE syscolumns.name = '" +
                    columnname + "' AND sysobjects.name = '" + tablename + "') ";
                var sql2 = "EXEC sp_rename '[" + tablename + "].[" + columnname + "]' , '[" + columnnamenew + "]'";
                return conn.Ado.ExecuteCommand(sql1 + sql2) > 0;
            }
        }

        #endregion
    }

    public abstract class BaseBLL<T> : BaseBLL where T : class, new()
    {
        public static T Insert(T model)
        {
            return InsertAsync(model).Result;
        }

        public static Task<T> InsertAsync(T model)
        {
            using (var db = DBClient)
            {
                var query = db.Insertable(model);
#if Cache
                if (_EnableCache)
                {
                    query.RemoveDataCache();
                }
#endif

                return query.ExecuteReturnEntityAsync();
            }
        }

        public static T InsertRange(IEnumerable<T> modellist)
        {
            return InsertRangeAsync(modellist).Result;
        }

        public static Task<T> InsertRangeAsync(IEnumerable<T> modellist)
        {
            using (var db = DBClient)
            {
                var query = db.Insertable(modellist.ToList());
#if Cache
                if (_EnableCache)
                {
                    db.Utilities.RemoveCacheAll<T>();
                }
#endif
                return query.ExecuteReturnEntityAsync();
            }
        }

        public static bool Update(T model)
        {
            return UpdateAsync(model).Result;
        }

        public static Task<bool> UpdateAsync(T model)
        {
            using (var db = DBClient)
            {
                var query = db.Updateable(model);
#if Cache
                if (_EnableCache)
                {
                    //db.Utilities.RemoveCacheAll<T>();
                    query.RemoveDataCache();
                }
#endif
                return query.ExecuteCommandHasChangeAsync();
            }
        }

        public static bool UpdateRange(IEnumerable<T> modellist)
        {
            return UpdateRangeAsync(modellist).Result;
        }

        public static Task<bool> UpdateRangeAsync(IEnumerable<T> modellist)
        {
            using (var db = DBClient)
            {
                var query = db.Updateable(modellist.ToList());
#if Cache
                if (_EnableCache)
                {
                    query.RemoveDataCache();
                }
#endif
                return query.ExecuteCommandHasChangeAsync();
            }
        }

        /// <summary>
        ///     根据条件更新数据
        /// </summary>
        /// <param name="wexpr"></param>
        /// <param name="sexprs">
        ///     更新多列的情况:it => new Student() { Name = it.Name+1 , Age = 17 }
        /// </param>
        /// <returns></returns>
        public static bool Update(Expression<Func<T, bool>> wexpr, Expression<Func<T, T>> sexprs)
        {
            return UpdateAsync(wexpr, sexprs).Result;
        }

        /// <summary>
        ///     根据条件更新数据
        /// </summary>
        /// <param name="wexpr"></param>
        /// <param name="sexprs">
        ///     更新多列的情况:it => new Student() { Name = it.Name+1 , Age = 17 }
        /// </param>
        /// <returns></returns>
        public static Task<bool> UpdateAsync(Expression<Func<T, bool>> wexpr, Expression<Func<T, T>> sexprs)
        {
            using (var db = DBClient)
            {
                var query = db.Updateable<T>().SetColumns(sexprs).Where(wexpr);
#if Cache
                if (_EnableCache)
                {
                    query.RemoveDataCache();
                }
#endif
                return query.ExecuteCommandHasChangeAsync();
            }
        }

        /// <summary>
        ///     根据条件更新数据
        /// </summary>
        /// <param name="wexpr"></param>
        /// <param name="sexprs">
        ///     更新一列的情况:it => it.Name == it.Name + 1
        /// </param>
        /// <returns></returns>
        public static bool Update(Expression<Func<T, bool>> wexpr, Expression<Func<T, bool>> sexprs)
        {
            return UpdateAsync(wexpr, sexprs).Result;
        }

        /// <summary>
        ///     根据条件更新数据
        /// </summary>
        /// <param name="wexpr"></param>
        /// <param name="sexprs">
        ///     更新一列的情况:it => it.Name == it.Name + 1
        /// </param>
        /// <returns></returns>
        public static Task<bool> UpdateAsync(Expression<Func<T, bool>> wexpr, Expression<Func<T, bool>> sexprs)
        {
            using (var db = DBClient)
            {
                var query = db.Updateable<T>().SetColumns(sexprs).Where(wexpr);
#if Cache
                if (_EnableCache)
                {
                    query.RemoveDataCache();
                }
#endif
                return query.ExecuteCommandHasChangeAsync();
            }
        }

        public static bool Delete(T model)
        {
            return DeleteAsync(model).Result;
        }

        public static Task<bool> DeleteAsync(T model)
        {
            using (var db = DBClient)
            {
                var query = db.Deleteable(model);
#if Cache
                if (_EnableCache)
                {
                    query.RemoveDataCache();
                }
#endif
                return query.ExecuteCommandHasChangeAsync();
            }
        }

        public static bool DeleteRange(IEnumerable<T> modellist)
        {
            return DeleteRangeAsync(modellist).Result;
        }

        public static Task<bool> DeleteRangeAsync(IEnumerable<T> modellist)
        {
            using (var db = DBClient)
            {
                var query = db.Deleteable(modellist.ToList());
#if Cache
                if (_EnableCache)
                {
                    query.RemoveDataCache();
                }
#endif
                return query.ExecuteCommandHasChangeAsync();
            }
        }

        public static bool DeleteById(dynamic id)
        {
            return DeleteByIdAsync(id).Result;
        }

        public static Task<bool> DeleteByIdAsync(dynamic id)
        {
            using (var db = DBClient)
            {
                var client = db.GetSimpleClient<T>();
#if Cache
                if (_EnableCache)
                {
                    db.Utilities.RemoveCacheAll<T>();
                }
#endif
                return Task.FromResult(client.DeleteById(id));
            }
        }

        public static bool DeleteByIds(params dynamic[] idlist)
        {
            return DeleteByIdsAsync(idlist).Result;
        }

        public static Task<bool> DeleteByIdsAsync(params dynamic[] idlist)
        {
            using (var db = DBClient)
            {
                var client = db.GetSimpleClient<T>();
#if Cache
                if (_EnableCache)
                {
                    db.Utilities.RemoveCacheAll<T>();
                }
#endif
                return Task.FromResult(client.DeleteByIds(idlist));
            }
        }

        public static bool Delete(Expression<Func<T, bool>> wexpr)
        {
            return DeleteAsync(wexpr).Result;
        }

        /// <summary>
        ///     根据条件删除数据
        /// </summary>
        /// <param name="wexpr"></param>
        /// <returns></returns>
        public static Task<bool> DeleteAsync(Expression<Func<T, bool>> wexpr)
        {
            using (var db = DBClient)
            {
                var query = db.Deleteable<T>().Where(wexpr);
#if Cache
                if (_EnableCache)
                {
                    query.RemoveDataCache();
                }
#endif
                return query.ExecuteCommandHasChangeAsync();
            }
        }

        public static List<T> QueryList(Expression<Func<T, bool>> wexpr = null,
            Dictionary<Expression<Func<T, object>>, OrderByType> oexprs = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            return QueryListAsync(wexpr, oexprs
#if Cache
            , _CacheSecond
#endif
            ).Result;
        }

        public static Task<List<T>> QueryListAsync(Expression<Func<T, bool>> wexpr = null,
            Dictionary<Expression<Func<T, object>>, OrderByType> oexprs = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            return QueryListAsync(-1, wexpr, oexprs
#if Cache
            , cachesecond
#endif
            );
        }

        public static List<T> QueryList(int limit, Expression<Func<T, bool>> wexpr = null,
            Dictionary<Expression<Func<T, object>>, OrderByType> oexprs = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            return QueryListAsync(limit, wexpr, oexprs
#if Cache
            , _CacheSecond
#endif
            ).Result;
        }

        public static Task<List<T>> QueryListAsync(int limit,
            Expression<Func<T, bool>> wexpr = null,
            Dictionary<Expression<Func<T, object>>, OrderByType> oexprs = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            using (var db = DBClient)
            {
                var query = db.Queryable<T>();
                if (wexpr != null) query.Where(wexpr);
#if Cache
                if (_EnableCache)
                {
                    query.WithCache(cachesecond);
                }
#endif
                if (oexprs != null && oexprs.Count > 0)
                    foreach (var item in oexprs)
                        query.OrderBy(item.Key, item.Value);

                if (limit > 0) query.Take(limit);

                return query.ToListAsync();
            }
        }

        public static int QueryCount(Expression<Func<T, bool>> wexpr = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            return QueryCountAsync(wexpr
#if Cache
            , _CacheSecond
#endif
            ).Result;
        }

        public static Task<int> QueryCountAsync(Expression<Func<T, bool>> wexpr
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            using (var db = DBClient)
            {
                var query = db.Queryable<T>().Where(wexpr);
#if Cache
                if (_EnableCache)
                {
                    query.WithCache(cachesecond);
                }
#endif
                return query.CountAsync();
            }
        }

        public static T QueryModel(Expression<Func<T, bool>> wexpr = null,
            Dictionary<Expression<Func<T, object>>, OrderByType> oexpr = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            return QueryModelAsync(wexpr, oexpr
#if Cache
            , _CacheSecond
#endif
            ).Result;
        }

        /// <summary>
        ///     查找对象
        /// </summary>
        /// <param name="wexpr"></param>
        /// <param name="oexprs"></param>
        /// <param name="cachesecond"></param>
        /// <returns></returns>
        public static Task<T> QueryModelAsync(Expression<Func<T, bool>> wexpr,
            Dictionary<Expression<Func<T, object>>, OrderByType> oexprs = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            using (var db = DBClient)
            {
                var query = db.Queryable<T>().Where(wexpr);
                if (oexprs != null && oexprs.Count > 0)
                    foreach (var item in oexprs)
                        query.OrderBy(item.Key, item.Value);
#if Cache
                if (_EnableCache)
                {
                    query.WithCache(cachesecond);
                }
#endif
                return query.FirstAsync();
            }
        }

        public static T QueryModelById(dynamic id)
        {
            return QueryModelByIdAsync(id).Result;
        }

        public static Task<T> QueryModelByIdAsync(dynamic id)
        {
            using (var db = DBClient)
            {
                var client = db.GetSimpleClient<T>();

                return Task.FromResult(client.GetById(id));
            }
        }

        public static PageList<T> QueryPageList(int pageindex, int pagesize,
            Expression<Func<T, bool>> wexpr = null,
            Dictionary<Expression<Func<T, object>>, OrderByType> oexprs = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            return QueryPageListAsync(pageindex, pagesize, wexpr, oexprs
#if Cache
            , _CacheSecond
#endif
            ).Result;
        }

        /// <summary>
        ///     查找分页数据
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="wexpr"></param>
        /// <param name="oexprs"></param>
        /// <param name="cachesecond"></param>
        /// <returns>{ Data = data, Total = total }</returns>
        public static Task<PageList<T>> QueryPageListAsync(int pageindex, int pagesize,
            Expression<Func<T, bool>> wexpr = null,
            Dictionary<Expression<Func<T, object>>, OrderByType> oexprs = null
#if Cache
            , int cachesecond = _CacheSecond
#endif
        )
        {
            using (var db = DBClient)
            {
                var query = db.Queryable<T>();
                if (wexpr != null) query.Where(wexpr);
                if (oexprs != null && oexprs.Count > 0)
                    foreach (var item in oexprs)
                        query.OrderBy(item.Key, item.Value);
#if Cache
                if (_EnableCache)
                {
                    query.WithCache(cachesecond);
                }
#endif

                RefAsync<int> total = 0;
                var data = query.ToPageListAsync(pageindex, pagesize, total);
                return Task.FromResult(new PageList<T>(data.Result, total));
            }
        }

        public static int ExecuteCommand(string sql, params SugarParameter[] parameters)
        {
            return ExecuteCommandAsync(sql, parameters).Result;
        }

        public static Task<int> ExecuteCommandAsync(string sql, params SugarParameter[] parameters)
        {
            using (var db = DBClient)
            {
                return db.Ado.ExecuteCommandAsync(sql, parameters);
            }
        }

        public static List<T> ExecuteQueryList(string sql, params SugarParameter[] parameters)
        {
            return ExecuteQueryListAsync(sql, parameters).Result;
        }

        public static Task<List<T>> ExecuteQueryListAsync(string sql, params SugarParameter[] parameters)
        {
            using (var db = DBClient)
            {
                return db.Ado.SqlQueryAsync<T>(sql, parameters);
            }
        }

        public static T ExecuteQuerySingle(string sql, params SugarParameter[] parameters)
        {
            return ExecuteQuerySingleAsync(sql, parameters).Result;
        }

        public static Task<T> ExecuteQuerySingleAsync(string sql, params SugarParameter[] parameters)
        {
            using (var db = DBClient)
            {
                return db.Ado.SqlQuerySingleAsync<T>(sql, parameters);
            }
        }


#if Cache
        protected static bool _EnableCache
        {
            get { return true; }
        }

        protected const int _CacheSecond = 900;

        public static void CacheClear()
        {
            if (_EnableCache)
            {
                using (var db = DbClient)
                {
                    db.Utilities.RemoveCacheAll();
                }
            }
        }

#endif
    }

    internal class SqlSugarSerializeService : ISerializeService
    {
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public string SugarSerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }

#if Cache
    public class SqlSugarDataInfoCacheService : ICacheService
    {
        public void Add<V>(string key, V value)
        {
            SqlSugarDataInfoCacheCore<V>.GetInstance().Add(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            SqlSugarDataInfoCacheCore<V>.GetInstance().Add(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return SqlSugarDataInfoCacheCore<V>.GetInstance().ContainsKey(key);
        }

        public V Get<V>(string key)
        {
            return SqlSugarDataInfoCacheCore<V>.GetInstance().Get(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            return SqlSugarDataInfoCacheCore<V>.GetInstance().GetAllKey();
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            return SqlSugarDataInfoCacheCore<V>.GetInstance().GetOrCreate(cacheKey, create);
        }

        public void Remove<V>(string key)
        {
            SqlSugarDataInfoCacheCore<V>.GetInstance().Remove(key);
        }
    }

    public class SqlSugarDataInfoCacheCore<V>
    {
        private readonly System.Collections.Concurrent.ConcurrentBag<string> _keys =
 new System.Collections.Concurrent.ConcurrentBag<string>();

        private readonly MemoryCache _cacher = new MemoryCache(new MemoryCacheOptions() { });

        private static SqlSugarDataInfoCacheCore<V> _instance = null;
        private static readonly object _instanceLock = new object();

        public bool ContainsKey(string key)
        {
            return _keys.Contains(key);
        }

        public V this[string key]
        {
            get
            {
                return Get(key);
            }
        }

        public V Get(string key)
        {
            object value = null;
            if (!_cacher.TryGetValue(key, out value))
            {
                _keys.TryTake(out key);
                value = default(V);
            }
            return (V)value;
        }

        public static SqlSugarDataInfoCacheCore<V> GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                    {
                        _instance = new SqlSugarDataInfoCacheCore<V>();
                        //Action addItem = () => { SqlSugarDataInfoCacheCore<V>.GetInstance().RemoveAllCache(); };
                        //ReflectionInoHelper.AddRemoveFunc(addItem);
                    }
            return _instance;
        }

        public void Add(string key, V value)
        {
            _keys.Add(key);
            var op = new MemoryCacheEntryOptions();
            op.RegisterPostEvictionCallback(new PostEvictionDelegate((p_key, p_value, p_reason, p_state) =>
            {
                string tempkey = p_key.ToString();
                _keys.TryTake(out tempkey);
            }));
            _cacher.Set(key, value, op);
        }

        public void Add(string key, V value, int cacheDurationInSeconds)
        {
            _keys.Add(key);
            var op = new MemoryCacheEntryOptions();
            op.SlidingExpiration = TimeSpan.FromSeconds(cacheDurationInSeconds);
            op.RegisterPostEvictionCallback(new PostEvictionDelegate((p_key, p_value, p_reason, p_state) =>
            {
                string tempkey = p_key.ToString();
                _keys.TryTake(out tempkey);
            }));
            _cacher.Set(key, value, op);
        }

        public void Remove(string key)
        {
            _keys.TryTake(out key);
            _cacher.Remove(key);
        }

        public void RemoveAllCache()
        {
            foreach (var key in GetAllKey())
            {
                _cacher.Remove(key);
            }
        }

        public IEnumerable<string> GetAllKey()
        {
            return _keys;
        }

        public V GetOrCreate(string cacheKey, Func<V> create)
        {
            if (ContainsKey(cacheKey)) return Get(cacheKey);
            else
            {
                var reval = create();
                Add(cacheKey, reval);
                return reval;
            }
        }
    }
#endif
    public class Pagination
    {
        private const int DefaultPageSize = 25;

        public Pagination(int pageIndex, int pageSize)
        {
            pageSize = pageSize == 0 ? DefaultPageSize : pageSize;
            if (pageIndex > 0)
            {
                Offset = (pageIndex - 1) * pageSize;
                Limit = pageSize;

                PageIndex = pageIndex;
                PageSize = pageSize;
            }
            else
            {
                throw new Exception("分页数据错误!");
            }
        }

        public int Offset { get; }
        public int Limit { get; }
        public int PageIndex { get; }
        public int PageSize { get; }

        public static Pagination Generate(string pageIndex, string pageSize)
        {
            int size = 0, index = 0;
            if (!string.IsNullOrWhiteSpace(pageSize) && !string.IsNullOrWhiteSpace(pageIndex))
            {
                int.TryParse(pageSize, out size);
                int.TryParse(pageIndex, out index);
            }

            return new Pagination(index, size);
        }
    }

    public class PageList<T>
    {
        public PageList(List<T> rows, int total = 0)
        {
            Rows = rows;
            Total = total == 0 ? rows.Count : total;
        }


        /// <summary>
        ///     当前页数据
        /// </summary>
        public List<T> Rows { get; }

        /// <summary>
        ///     总条数
        /// </summary>
        public int Total { get; }

        /// <summary>
        ///     附加数据
        /// </summary>
        public dynamic Extra { get; set; }
    }
}