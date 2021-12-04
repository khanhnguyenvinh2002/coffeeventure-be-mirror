using coffeeventureAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Model.unitsOfWork
{
    public interface IUnitOfWork
    {
        /// </summary>
        CVDbContext DataContextRead { get; }

        /// <summary>
        /// Define a property of context write class
        /// </summary>
        CVDbContext DataContextWrite { get; }

        /// <summary>
        /// Get current user
        /// </summary>
        //public string GetCurrentUserId();

        /// <summary>
        /// Begin a database transaction
        /// </summary>
        /// <returns>Transaction</returns>
        IDbContextTransaction BeginTransaction();
        public string GetCurrentUserId();
        public CurrentUser GetCurrentUser();
        public void SetCurrentUserId(string id);

        /// <summary>
        /// Find entity by key values
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;

        /// <summary>
        /// Find async entity by key values
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;

        /// <summary>
        /// Select entity
        /// </summary>
        /// <returns></returns>
        DbSet<TEntity> Select<TEntity>() where TEntity : class;

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity"></param>
        void Insert<TEntity>(TEntity entity, bool saveChange = true);

        /// <summary>
        /// Bulk insert entities
        /// </summary>
        /// <param name="entities"></param>
        void BulkInsert<TEntity>(IEnumerable<TEntity> entities);

        /// <summary>
        /// Insert entity mapping from dto
        /// </summary>
        /// <param name="dto"></param>
        void Insert<TEntity, TDto>(TDto dto, bool saveChange = true) where TEntity : class;

        /// <summary>
        /// Bulk insert entities from list dto
        /// </summary>
        /// <param name="listDto"></param>
        void BulkInsert<TEntity, TDto>(IEnumerable<TDto> listDto) where TEntity : class;

        /// <summary>
        /// Insert async entity
        /// </summary>
        /// <param name="entity"></param>
        Task InsertAsync<TEntity>(TEntity entity, bool saveChange = true);

        /// <summary>
        /// Bulk insert async entities
        /// </summary>
        /// <param name="entities"></param>
        Task BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities);

        /// <summary>
        /// Insert async entity mapping from dto
        /// </summary>
        /// <param name="dto"></param>
        Task InsertAsync<TEntity, TDto>(TDto dto, bool saveChange = true) where TEntity : class;

        /// <summary>
        /// Bulk insert async entities from list dto
        /// </summary>
        /// <param name="listDto"></param>
        Task BulkInsertAsync<TEntity, TDto>(IEnumerable<TDto> listDto) where TEntity : class;

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        void Update<TEntity>(TEntity entity, bool saveChange = true) where TEntity : class;

        /// <summary>
        /// Bulk update entities
        /// </summary>
        /// <param name="entities"></param>
        void BulkUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        /// <summary>
        /// Update entity, specific fields
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="fields">Update fields</param>
        public void Update<TEntity>(TEntity entity, params string[] fields) where TEntity : class;

        /// <summary>
        /// Update entity, specific field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities">Entities</param>
        /// <param name="fields">Update fields</param>
        public void BulkUpdate<TEntity>(IEnumerable<TEntity> entities, params string[] fields) where TEntity : class;

        /// <summary>
        /// Update entity mapping from dto
        /// </summary>
        /// <param name="entity"></param>
        void Update<TEntity, TDto>(TDto entity, bool saveChange = true) where TEntity : class;

        /// <summary>
        /// Bulk update entities mapping from dto
        /// </summary>
        /// <param name="entities"></param>
        void BulkUpdate<TEntity, TDto>(IEnumerable<TDto> entities) where TEntity : class;

        /// <summary>
        /// Merge entity
        /// </summary>
        /// <param name="entity"></param>
        public TEntity Merge<TEntity>(TEntity entity, bool saveChange = true) where TEntity : class;

        /// <summary>
        /// Merge entity mapping from dto
        /// </summary>
        /// <param name="entity"></param>
        public TDto Merge<TEntity, TDto>(TDto dto, bool saveChange = true) where TEntity : class;
        public void Map<TDest, TSource>(TDest destObj, TSource sourceObj);
        /// <summary>
        /// Bulk merge entities
        /// </summary>
        /// <param name="entities"></param>
        void BulkMerge<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        /// <summary>
        /// Bulk merge entities
        /// </summary>
        /// <param name="entities"></param>
        void BulkMerge<TEntity, TDto>(IEnumerable<TDto> listDto) where TDto : class where TEntity : class;

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="saveChange"></param>
        public void Delete<TEntity>(TEntity entity, bool saveChange = true) where TEntity : class;

        /// <summary>
        /// Delete entity by ids
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="saveChange"></param>
        public void Delete<TEntity>(params string[] ids) where TEntity : class;

        /// <summary>
        /// Bulk delete entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="saveChange"></param>
        public void BulkDelete<TEntity>(IEnumerable<TEntity> entities, bool saveChange = true) where TEntity : class;

        /// <summary>
        /// Database save changes
        /// </summary>
        void Save();

        /// <summary>
        /// Database save changes async
        /// </summary>
        Task SaveAsync();

        /// <summary>
        /// Dispose async database context
        /// </summary>
        Task DisposeAsync();
    }
    //    IDbContextTransaction BeginTransaction();
    //    void BulkDelete<TEntity>(IEnumerable<TEntity> entities, bool saveChange = true) where TEntity : class;
    //    void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    //    void BulkInsert<TEntity, TDto>(IEnumerable<TDto> listDto) where TEntity : class;
    //    Task BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    //    Task BulkInsertAsync<TEntity, TDto>(IEnumerable<TDto> listDto) where TEntity : class;
    //    void BulkMerge<TEntity, TDto>(IEnumerable<TDto> listDto)
    //        where TEntity : class
    //        where TDto : class;
    //    void BulkMerge<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    //    //void BulkUpdate<TEntity>(IEnumerable<TEntity> entities, params string[] fields) where TEntity : class;
    //    void BulkUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    //    void BulkUpdate<TEntity, TDto>(IEnumerable<TDto> entities) where TEntity : class;
    //    //void Delete<TEntity>(params string[] ids) where TEntity : class;
    //    void Delete<TEntity>(TEntity entity, bool saveChange = true) where TEntity : class;
    //    Task DisposeAsync();
    //    TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;
    //    Task<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;
    //    //CurrentUser GetCurrentUser();
    //    void Insert<TEntity, TDto>(TDto dto, bool saveChange = true) where TEntity : class;
    //    void Insert<TEntity>(TEntity entity, bool saveChange = true) where TEntity : class;
    //    Task InsertAsync<TEntity, TDto>(TDto dto, bool saveChange = true) where TEntity : class;
    //    Task InsertAsync<TEntity>(TEntity entity, bool saveChange = true) where TEntity : class;
    //    TEntity Merge<TEntity>(TEntity entity, bool saveChange = true) where TEntity : class;
    //    TDto Merge<TEntity, TDto>(TDto dto, bool saveChange = true) where TEntity : class;
    //    void Save();
    //    Task SaveAsync();
    //    DbSet<TEntity> Select<TEntity>() where TEntity : class;
    //    void Update<TEntity>(TEntity entity, bool saveChange = true) where TEntity : class;
    //    void Update<TEntity, TDto>(TDto entity, bool saveChange = true) where TEntity : class;
    //    void Update<TEntity>(TEntity entity, params string[] fields) where TEntity : class;
    //}
}
