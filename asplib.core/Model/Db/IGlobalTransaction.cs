using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace asplib.Model.Db
{
    /// <summary>
    /// Primarily for simple SetUp()/TearDown in integration tests:
    /// Add global BeginTransaction/CommitTransaction/RollbackTransaction for
    /// all contained DbContext objects. Call RetrieveDbContexts() immediately
    /// after ConfigureServices() for the recursive DbContext member retrieval.
    /// </summary>
    public interface IGlobalTransaction
    {
        List<DbContext> DbContexts { get; set; }
    }

    public static class GlobalTransactionExtension
    {
        /// <summary>
        /// Retrieve all DbContext instances for the DbContexts property.
        /// Call immediately after ConfigureServices()
        /// </summary>
        /// <param name="inst"></param>
        public static void RetrieveDbContexts(this IGlobalTransaction inst)
        {
            inst.DbContexts = DbContextMembers(inst).ToList();
        }

        public static void BeginTransaction(this IGlobalTransaction inst, IsolationLevel isolationLevel)
        {
            foreach (var d in inst.DbContexts)
            {
                d.Database.BeginTransaction(isolationLevel);
            }
        }

        public static void CommitTransaction(this IGlobalTransaction inst)
        {
            foreach (var d in inst.DbContexts)
            {
                d.Database.CommitTransaction();
            }
        }

        public static void RollbackTransaction(this IGlobalTransaction inst)
        {
            foreach (var d in inst.DbContexts)
            {
                d.Database.RollbackTransaction();
            }
        }

        /// <summary>
        /// Recursively get all contained DbContext objects
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        internal static IEnumerable<DbContext> DbContextMembers(this IGlobalTransaction inst)
        {
            return from m in Members(inst)
                   where m is DbContext
                   select (DbContext)m;
        }

        internal static IEnumerable<object> Members(object inst)
        {
            return Members(inst, new List<object>());   // recursion anchor
        }

        internal static IEnumerable<object> Members(object inst, List<object> acc)
        {
            var members = DirectMembers(inst, acc);  /// breadth first search
            acc.AddRange(members);
            acc.AddRange(from m in members
                         select Members(m, acc));       // recursion
            return acc;
        }

        /// <summary>
        /// Get all direct members except those already present in the recursion
        /// accumulator to avoid getting trapped in a cyclic reference tree.
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        internal static IEnumerable<object> DirectMembers(object inst, List<object> acc)
        {
            return from f in inst.GetType().GetFields(BindingFlags.FlattenHierarchy |
                                                       BindingFlags.Instance |
                                                       BindingFlags.Public |
                                                       BindingFlags.NonPublic)
                   where (!f.FieldType.IsPrimitive &&
                            f.GetValue(inst) != null &&
                            !ContainsObject(acc, inst))
                   select f.GetValue(inst);
        }

        internal static bool ContainsObject(IEnumerable<object> list, object obj)
        {
            foreach (var item in list)
            {
                if (Object.ReferenceEquals(item, obj)) return true;
            }
            return false;
        }
    }
}