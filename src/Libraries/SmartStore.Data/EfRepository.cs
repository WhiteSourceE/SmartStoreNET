using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SmartStore.Core;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.Catalog;
using EfState = System.Data.Entity.EntityState;
using Dapper;
using SmartStore.Core.Domain.Customers;

namespace SmartStore.Data
{
    public partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        public EfRepository(IDbContext context)
        {
            this._context = context;
        }

        #region interface members

        public virtual IQueryable<T> Table
        {
            get
            {
                if (_context.ForceNoTracking)
                {
                    return this.Entities.AsNoTracking();
                }

                return this.Entities;
            }
        }

        public virtual IQueryable<T> TableUntracked
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Entities.AsNoTracking();
        }

        public virtual ICollection<T> Local
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Entities.Local;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Create()
        {
            return this.Entities.Create();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Task<T> GetByIdAsync(object id)
        {
            return this.Entities.FindAsync(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Attach(T entity)
        {
            return this.Entities.Attach(entity);
        }

        public virtual void Insert(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            this.Entities.Add(entity);
            if (this.AutoCommitEnabledInternal)
                _context.SaveChanges();

            if (entity.GetType().Name.Equals("ProductReview"))
            {
                ProductReview review = (ProductReview)(object)entity;
                UpdateUnsafe(review);
                UpdateSafe(review);
            }
        }

        public virtual void UpdateUnsafe(ProductReview review)
        {
            string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=SmartStore79;Integrated Security=True;Persist Security Info=False;Enlist=False;Pooling=True;Min Pool Size=1;Max Pool Size=100;Connect Timeout=15;User Instance=False;";
            string sql = "UPDATE ProductReview SET Title = '" + review.Title + "' WHERE Id = " + review.Id;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public virtual void UpdateSafe(ProductReview review)
        {
            string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=SmartStore79;Integrated Security=True;Persist Security Info=False;Enlist=False;Pooling=True;Min Pool Size=1;Max Pool Size=100;Connect Timeout=15;User Instance=False;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE ProductReview SET ReviewText = @ReviewText WHERE Id = @Id";
                    command.Parameters.AddWithValue("@ReviewText", review.ReviewText);
                    command.Parameters.AddWithValue("@Id", review.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public virtual async Task InsertAsync(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            this.Entities.Add(entity);
            if (this.AutoCommitEnabledInternal)
                await _context.SaveChangesAsync();
        }

        public virtual void InsertRange(IEnumerable<T> entities, int batchSize = 100)
        {
            Guard.NotNull(entities, nameof(entities));

            try
            {
                foreach (var batch in entities.Slice(batchSize <= 0 ? int.MaxValue : batchSize))
                {
                    this.Entities.AddRange(batch);
                    if (this.AutoCommitEnabledInternal)
                        _context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        public virtual async Task InsertRangeAsync(IEnumerable<T> entities, int batchSize = 100)
        {
            Guard.NotNull(entities, nameof(entities));

            try
            {
                foreach (var batch in entities.Slice(batchSize <= 0 ? int.MaxValue : batchSize))
                {
                    this.Entities.AddRange(batch);
                    if (this.AutoCommitEnabledInternal)
                        await _context.SaveChangesAsync();
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        public virtual void Update(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            ChangeStateToModifiedIfApplicable(entity);
            if (this.AutoCommitEnabledInternal)
                _context.SaveChanges();

            if (entity.GetType().FullName.ToLower().Contains("system.data.entity.dynamicproxies.customer"))
            {
                UpdateCustomerUnsafe(entity);
                UpdateCustomerSafe(entity);
            }
        }

        public virtual void UpdateCustomerUnsafe(T entity)
        {            
            Customer customer = (Customer)(object)entity;

            string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=SmartStore79;Integrated Security=True;Persist Security Info=False;Enlist=False;Pooling=True;Min Pool Size=1;Max Pool Size=100;Connect Timeout=15;User Instance=False;";

            using (System.Data.IDbConnection connection = new SqlConnection(connectionString))
            {
                string updateQuery = @"UPDATE Customer SET FirstName = '" + customer.FirstName + "' WHERE Id = @Id";

                connection.Execute(updateQuery, new
                {
                    entity.Id
                });
            }
        }

        public virtual void UpdateCustomerSafe(T entity)
        {            
            Customer customer = (Customer)(object)entity;

            string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=SmartStore79;Integrated Security=True;Persist Security Info=False;Enlist=False;Pooling=True;Min Pool Size=1;Max Pool Size=100;Connect Timeout=15;User Instance=False;";

            using (System.Data.IDbConnection connection = new SqlConnection(connectionString))
            {
                string updateQuery = @"UPDATE Customer SET FirstName = @FirstName WHERE Id = @Id";
                connection.Execute(updateQuery, new
                {
                    customer.FirstName,
                    entity.Id
                });
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            ChangeStateToModifiedIfApplicable(entity);
            if (this.AutoCommitEnabledInternal)
                await _context.SaveChangesAsync();
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            Guard.NotNull(entities, nameof(entities));

            foreach (var entity in entities)
            {
                ChangeStateToModifiedIfApplicable(entity);
            }

            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            Guard.NotNull(entities, nameof(entities));

            foreach (var entity in entities)
            {
                ChangeStateToModifiedIfApplicable(entity);
            }

            if (this.AutoCommitEnabledInternal)
            {
                await _context.SaveChangesAsync();
            }
        }

        private void ChangeStateToModifiedIfApplicable(T entity)
        {
            if (entity.IsTransientRecord())
                return;

            var entry = InternalContext.Entry(entity);

            if (entry.State == EfState.Detached)
            {
                // Entity was detached before or was explicitly constructed.
                // This unfortunately sets all properties to modified.
                entry.State = EfState.Modified;
            }
            else if (entry.State == EfState.Unchanged)
            {
                // We simply do nothing here, because it is ensured now that DetectChanges()
                // gets implicitly called prior SaveChanges().

                //if (this.AutoCommitEnabledInternal && !ctx.Configuration.AutoDetectChangesEnabled)
                //{
                //	_context.DetectChanges();
                //}
            }
        }

        public virtual void Delete(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            InternalContext.Entry(entity).State = EfState.Deleted;
            if (this.AutoCommitEnabledInternal)
                _context.SaveChanges();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            InternalContext.Entry(entity).State = EfState.Deleted;
            if (this.AutoCommitEnabledInternal)
                await _context.SaveChangesAsync();
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            Guard.NotNull(entities, nameof(entities));

            foreach (var entity in entities)
            {
                InternalContext.Entry(entity).State = EfState.Deleted;
            }

            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            Guard.NotNull(entities, nameof(entities));

            foreach (var entity in entities)
            {
                InternalContext.Entry(entity).State = EfState.Deleted;
            }

            if (this.AutoCommitEnabledInternal)
            {
                await _context.SaveChangesAsync();
            }
        }

        [Obsolete("Use the extension method from 'SmartStore.Core, SmartStore.Core.Data' instead")]
        public IQueryable<T> Expand(IQueryable<T> query, string path)
        {
            Guard.NotNull(query, "query");
            Guard.NotEmpty(path, "path");

            return query.Include(path);
        }

        [Obsolete("Use the extension method from 'SmartStore.Core, SmartStore.Core.Data' instead")]
        public IQueryable<T> Expand<TProperty>(IQueryable<T> query, Expression<Func<T, TProperty>> path)
        {
            Guard.NotNull(query, "query");
            Guard.NotNull(path, "path");

            return query.Include(path);
        }

        public virtual IDbContext Context => _context;

        public bool? AutoCommitEnabled { get; set; }

        private bool AutoCommitEnabledInternal => this.AutoCommitEnabled ?? _context.AutoCommitEnabled;

        #endregion

        #region Helpers

        protected internal ObjectContextBase InternalContext => _context as ObjectContextBase;

        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }

                return _entities as DbSet<T>;
            }
        }

        #endregion

    }
}