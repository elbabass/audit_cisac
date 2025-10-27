using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Data.DataModels;

namespace SpanishPoint.Azure.Iswc.Data.Extensions
{
    internal static class DbContextExtensions
    {
        private static IswcModel _IswcModelToReturn;
        private static IList<string> _Parents = new List<string>();

        public static IQueryable Set(this DbContext context, Type T)
        {
            var method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);
            method = method.MakeGenericMethod(T);
            return method.Invoke(context, null) as IQueryable;
        }

        public static IswcModel FindOverallParentIswc(this IswcModel childIswc, DbSet<IswclinkedTo> iswclinkedToData, DbSet<DataModels.Iswc> dbSetIswc, IMapper mapper)
        {
            if (childIswc == null)
            {
                _Parents = new List<string>();
                throw new ArgumentNullException($"{nameof(childIswc)} is null in {nameof(DbContextExtensions)} - {nameof(FindOverallParentIswc)}");
            }

            var parentLinkedIswc = iswclinkedToData.Where(l => l.IswcId == childIswc.IswcId && l.Status).FirstOrDefault();

            if (parentLinkedIswc != null && !IsSelfReferencingIswclinkedTo(parentLinkedIswc))
            {
                var parentIswc = dbSetIswc.Where(i => i.Iswc1 == parentLinkedIswc.LinkedToIswc)
                                            .Select(m => mapper.Map<IswcModel>(m)).FirstOrDefault();

                parentIswc.FindOverallParentIswc(iswclinkedToData, dbSetIswc, mapper);
            }
            else
            {
                _IswcModelToReturn = childIswc;
                _Parents = new List<string>();
            }

            return childIswc.Iswc == _IswcModelToReturn.Iswc ? null : _IswcModelToReturn;
        }

        private static bool IsSelfReferencingIswclinkedTo(IswclinkedTo linkedIswc)
        {
            if (linkedIswc == null)
            {
                return false;
            }

            var isExistingParent = _Parents.Any(l => l == linkedIswc?.LinkedToIswc);

            _Parents.Add(linkedIswc.LinkedToIswc);

            return linkedIswc != null && linkedIswc?.LinkedToIswc == linkedIswc?.Iswc.Iswc1 || isExistingParent;
        }
    }
}
