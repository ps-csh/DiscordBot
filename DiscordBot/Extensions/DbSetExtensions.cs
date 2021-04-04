using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DiscordBot.Extensions
{
    public static class DbSetExtensions
    {
        /// <summary>
        /// Adds an item to the DbSet only if an element matching
        /// the predicate doesn't already exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="entity">The item to add to the DbSet</param>
        /// <param name="predicate">The conditions for finding a matching element</param>
        /// <returns>EntityEntry for tracking the item if it was added, or null otherwise</returns>
        public static EntityEntry<T> AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null) where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            return !exists ? dbSet.Add(entity) : null;
        }

        /// <summary>
        /// Gets an items from the DbSet that matches the predicate or
        /// Adds the item to the DbSet if one a matching one wasn't found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="entity">The item to add to the DbSet</param>
        /// <param name="predicate">The conditions for finding a matching element</param>
        /// <returns>The Entity from the DbSet if a match was found, otherwise the Entity that was added</returns>
        public static T GetOrAddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var existing = predicate != null ? dbSet.FirstOrDefault(predicate) : dbSet.FirstOrDefault();
            return existing ?? dbSet.Add(entity).Entity;
        }
    }
}
