# N+1 Query Problem in PipelineManager

## 📍 Location

**File:** `Api.Publisher\Managers\PipelineManager.cs`  
**Methods:** `SortForPostMatcing()`, `SortForProcessing()`, `SortSearchForPostMatcing()`

---

## 🔴 The Problem

### What is an N+1 Query Problem?

An N+1 problem occurs when you execute 1 query to get a parent collection (N items), then execute N additional queries (one for each parent) to get related data. This results in **N+1 total queries** instead of a more efficient approach.

### Where it Happens in This Code

#### **Example 1: `SortForPostMatcing()` - Lines 115-150**

```csharp
var children = submissions.Where(x => x.MultipleAgencyWorkCodesChild).GroupBy(x => x.SubmissionParentId);

foreach (var child in children)
{
    // N+1 PROBLEM: FirstOrDefault() is called for EACH group iteration
    var parent = submissions?.FirstOrDefault(x => x.SubmissionId == child.FirstOrDefault()?.SubmissionParentId);
    var parentId = child.FirstOrDefault()?.SubmissionParentId;

    // Multiple .Where() queries are executed for each child group
    submissions?.Where(x => x.SubmissionParentId == parentId || x.SubmissionId == parentId && ...)
    submissions?.Where(x => x.SubmissionParentId == parentId).Select(...)
}
```

**Problem:** For each child group (N groups), the code queries the entire `submissions` collection multiple times using `FirstOrDefault()` and `Where()`. This creates:

- 1 initial grouping query
- N queries to find parents (`FirstOrDefault`)
- N×M queries to filter submissions (`Where`)

#### **Example 2: `SortSearchForPostMatcing()` - Lines 187-218**

```csharp
foreach (var parentSubmission in parentSubmissions)
{
    // N+1 PROBLEM: For each parent, query all submissions to find children
    var childSubmissions = submissions.Where(x => x.SubmissionParentId == parentSubmission.SubmissionId);

    foreach (var childSubmission in childSubmissions)
    {
        // Processing each child...
    }
}
```

**Problem:** For each parent submission (N parents), the code scans the entire `submissions` collection to find matching children.

---

## 🟢 The Solution

### Strategy: Pre-compute Lookups with Dictionaries

Instead of querying the collection repeatedly, create dictionaries/lookups **once** at the beginning.

### **Fixed Version of `SortForPostMatcing()`**

```csharp
void SortForPostMatcing()
{
    // ✅ Pre-compute lookups ONCE
    var submissionById = submissions.ToDictionary(x => x.SubmissionId);
    var childrenByParentId = submissions
        .Where(x => x.MultipleAgencyWorkCodesChild)
        .GroupBy(x => x.SubmissionParentId)
        .ToDictionary(g => g.Key, g => g.ToList());

    foreach (var (parentId, children) in childrenByParentId)
    {
        // ✅ O(1) lookup instead of O(N) scan
        if (!submissionById.TryGetValue(parentId, out var parent))
            continue;

        if (parent.Rejection == null)
        {
            var update = children.FirstOrDefault(x =>
                x.TransactionType == Bdo.Edi.TransactionType.CUR && x.Rejection == null) ?? parent;

            if (update != null)
            {
                // ✅ Process only affected submissions (already filtered)
                var affectedSubmissions = children.Where(x => x.SubmissionId != update.SubmissionId).ToList();

                if (parent.Rejection == null)
                {
                    foreach (var sub in affectedSubmissions)
                    {
                        sub.ExistingWork = update.ExistingWork ?? sub.ExistingWork;
                        sub.MatchedResult = update.MatchedResult ?? sub.MatchedResult;
                        sub.Model.PreferredIswc = update.Model.PreferredIswc ?? string.Empty;
                    }
                }
                else
                {
                    foreach (var sub in children)
                    {
                        sub.SkipProcessing = true;
                    }
                }
            }
        }
        else
        {
            foreach (var child in children)
            {
                child.SkipProcessing = true;
            }
        }
    }
}
```

### **Fixed Version of `SortSearchForPostMatcing()`**

```csharp
void SortSearchForPostMatcing()
{
    var parentSubmissions = submissions.Where(x => x.SearchWorks.Any()).ToList();

    // ✅ Pre-group children by parent ONCE
    var childrenByParentId = submissions
        .Where(x => x.SubmissionParentId.HasValue)
        .GroupBy(x => x.SubmissionParentId.Value)
        .ToDictionary(g => g.Key, g => g.ToList());

    var consolidatedSubmissions = new List<Submission>();

    foreach (var parentSubmission in parentSubmissions)
    {
        // ✅ O(1) lookup instead of O(N) scan
        if (!childrenByParentId.TryGetValue(parentSubmission.SubmissionId, out var childSubmissions))
        {
            consolidatedSubmissions.Add(parentSubmission);
            continue;
        }

        var combinedSearchResults = new List<IswcModel>();

        foreach (var childSubmission in childSubmissions)
        {
            if (childSubmission.Rejection != null)
            {
                parentSubmission.Rejection = childSubmission.Rejection;
            }
            else
            {
                if (childSubmission.SearchedIswcModels != null && childSubmission.SearchedIswcModels.Any())
                {
                    combinedSearchResults.AddRange(childSubmission.SearchedIswcModels);
                }

                if (childSubmission.IsrcMatchedResult != null && childSubmission.IsrcMatchedResult.Matches.Any())
                {
                    parentSubmission.IsrcMatchedResult = childSubmission.IsrcMatchedResult;
                }
            }
        }

        parentSubmission.SearchedIswcModels = combinedSearchResults.Distinct().OrderByDescending(x => x.RankScore).ToList();
        consolidatedSubmissions.Add(parentSubmission);
    }

    submissions = consolidatedSubmissions;
}
```

---

## 📊 Performance Impact

### Before (N+1 Pattern)

- **Time Complexity:** O(N × M) where N = number of groups/parents, M = total submissions
- **Operations:** Hundreds to thousands of linear scans through the collection
- **Example:** 100 parents × 1000 submissions = **100,000 operations**

### After (Dictionary Lookup)

- **Time Complexity:** O(N + M) - one pass to build dictionary, one pass to process
- **Operations:** Dictionary creation + lookups
- **Example:** 1000 submissions + 100 lookups = **~1,100 operations**

### **Performance Improvement: ~90-99% reduction in operations** 🚀

---

## 🎯 Key Takeaways

1. **Avoid querying collections inside loops** - Pre-compute lookups
2. **Use `Dictionary` or `ToDictionary()`** for O(1) lookups by key
3. **Use `GroupBy().ToDictionary()`** for one-to-many relationships
4. **The `.Where().Select().ToList()` pattern in loops is a red flag** - it triggers full collection scans

---

## 🔧 Additional Issues Found

### Inefficient `.Select().ToList()` Pattern

The code uses this pattern incorrectly:

```csharp
submissions?.Where(...).Select(x => { x.Property = value; return x; }).ToList();
```

**Problem:** The `.ToList()` creates a new list but the result is **discarded**. The mutations happen on the original objects, but the list creation is wasteful.

**Fix:** Use `foreach` instead:

```csharp
foreach (var x in submissions.Where(...))
{
    x.Property = value;
}
```

This is more readable and avoids unnecessary list allocation.

---

## 📝 Recommended Action Items

1. ✅ Replace `SortForPostMatcing()` with dictionary-based lookups
2. ✅ Replace `SortSearchForPostMatcing()` with dictionary-based lookups
3. ✅ Replace `SortForProcessing()` with dictionary-based lookups
4. ✅ Replace `.Select().ToList()` mutation pattern with `foreach` loops
5. ✅ Add performance tests to validate improvements
6. ✅ Consider caching submission lookups at class level if used across methods

---

**Severity:** 🔴 **High** - Significant performance impact with large submission batches  
**Effort:** 🟡 **Medium** - Straightforward refactoring, requires testing  
**Impact:** 🟢 **High** - 90-99% performance improvement potential
