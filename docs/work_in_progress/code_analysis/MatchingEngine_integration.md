# MatchingEngine Integration Analysis

## Overview

Analysis of `MatchingEngineMatchingService.cs` in the namespace `SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine`.

**File Location:** `src/Data/Services/Matching/MatchingEngine/MatchingEngineMatchingService.cs`

## Method: MatchAsync

### Purpose

`MatchAsync` is the main entry point for matching submissions against existing works in the ISWC system. It takes a collection of submissions and attempts to find matching works in the database using different strategies based on the source type.

### Method Signature

```csharp
public async Task<IEnumerable<Submission>> MatchAsync(
    IEnumerable<Submission> submissions,
    string source)
```

### High-Level Flow

The method has two main execution paths:

#### 1. Non-Usage Sources (lines 32-144)

For sources like agency submissions, EDI files, etc. (anything except "Usage"):

**Step 1: Initial Matching (lines 34-39)**

- Maps submissions to `InputWorkInfo` objects
- Calls the matching API via `GetMatches`
- Filters out provisional matches (ISWC Status 1) with "PRS_" prefixes

**Step 2: Label Matching Fallback (lines 41-80)**
If initial matching finds no matches OR contains CUR (update) transactions:

- Creates a deep copy of submissions
- Strips out interested party identifiers (IP numbers and names) to match on metadata only
- Sets `SubSource` to "Label" for broader matching
- For definite matches with ISWC Status 2, adds them as provisional matches
- This provides a fallback when contributor information doesn't match exactly

**Step 3: FSQ Transaction Filter (lines 82-97)**
For "First Search Query" transactions:

- Removes all matches with ISWC Status 2 (provisional ISWCs)
- Only keeps matches with confirmed ISWCs (Status 1)

**Step 4: Non-Definite Match Filter (lines 99-131)**
For non-"Search" sources:

- Removes matches that are not definite matches (unless they skip contributor count rules)
- This ensures only high-confidence matches are returned
- **Note:** Lines 116-131 appear to be duplicate logic

**Step 5: CUR (Update) Matching (lines 143, 162-177)**
Special handling for update transactions:

- Strips work numbers from the input
- Sets `ExcludeMatchesOTBelowSimilarity = true`
- Adds additional matches found to existing results

**Step 6: Map Results (lines 133-141)**

- Maps matching results back to the submission objects
- Stores them in `submission.MatchedResult`

#### 2. Usage Sources (lines 146-158)

For usage/performance data:

- Maps submissions to `UsageWorkGroup` objects
- Uses a different matching endpoint (`UsageWorks/Match`)
- Maps results back to submissions

### Key Concepts

**Transaction Types:**

- **CUR** (Update): Existing work updates requiring special matching rules
- **FSQ** (First Search Query): Initial searches that exclude provisional ISWCs

**Match Types:**

- **Definite matches**: High-confidence matches meeting all criteria
- **Provisional matches**: Lower confidence matches that may need review

**ISWC Status:**

- **Status 1**: Confirmed ISWC
- **Status 2**: Provisional/temporary ISWC

**Matching Strategies:**

- **Full matching**: Uses all metadata including contributors
- **Label matching**: Strips contributor info to match on work metadata only

---

## Code Improvement Suggestions

### 1. Extract Smaller, Focused Methods

**Issue:** The method is ~170 lines with multiple responsibilities, violating Single Responsibility Principle.

**Suggestion:** Break it down into smaller methods:

```csharp
public async Task<IEnumerable<Submission>> MatchAsync(IEnumerable<Submission> submissions, string source)
{
    if (!submissions.Any())
        return submissions;

    var dictionary = submissions.ToDictionary(x => x.SubmissionId);

    if (source == "Usage")
        return await ProcessUsageMatches(submissions, dictionary);

    return await ProcessNonUsageMatches(submissions, dictionary, source);
}

private async Task<IEnumerable<Submission>> ProcessNonUsageMatches(
    IEnumerable<Submission> submissions,
    Dictionary<int, Submission> dictionary,
    string source)
{
    var submissionsForMatching = MapToInputWorkInfo(submissions, source);
    var matchResults = await GetInitialMatches(submissionsForMatching);

    matchResults = await ApplyLabelMatchingFallback(submissions, matchResults, source);
    matchResults = FilterByTransactionType(submissions, matchResults, source);

    MapResultsToSubmissions(matchResults, dictionary);

    if (submissions.Any(x => x.TransactionType == TransactionType.CUR))
        dictionary = await AddCURMatches(submissions, dictionary, source);

    return dictionary.Values;
}

private async Task<IEnumerable<Submission>> ProcessUsageMatches(
    IEnumerable<Submission> submissions,
    Dictionary<int, Submission> dictionary)
{
    var submissionsForUsageMatching = mapper.Map<IEnumerable<Submission>, IEnumerable<UsageWorkGroup>>(submissions);
    var usageMatchResults = await GetMatches(submissionsForUsageMatching);

    foreach (var usageMatchResult in usageMatchResults)
    {
        if (usageMatchResult.Error == null &&
            usageMatchResult.MatchedEntities != null &&
            usageMatchResult.MatchedEntities.Any())
        {
            dictionary[(int)usageMatchResult.ID].MatchedResult =
                mapper.Map<Bdo.MatchingEngine.MatchResult>(usageMatchResult);
        }
    }

    return dictionary.Values;
}
```

### 2. Remove Code Duplication

**Issue:** Lines 99-114 and 116-131 contain nearly identical filtering logic.

**Current Code:**

```csharp
// Lines 99-114
if (matchResults.Any() && source != "Search")
{
    foreach (var matchResult in matchResults.Where(m => string.IsNullOrEmpty(m.ErrorMessage) && m.Matches != null && m.Matches.Any()))
    {
        var matchesList = matchResult.Matches.ToList();
        for (int i = 0; i < matchesList.Count; i++)
        {
            if (!matchesList[i].SkipContributorCountRules && !matchesList[i].IsDefinite)
            {
                matchesList.RemoveAt(i);
                i--;
            }
        }
        matchResult.Matches = matchesList;
    };
}

// Lines 116-131 - DUPLICATE!
```

**Suggested Fix:**

```csharp
private void RemoveNonDefiniteMatches(IEnumerable<MatchResult> matchResults)
{
    foreach (var matchResult in matchResults.Where(m =>
        string.IsNullOrEmpty(m.ErrorMessage) &&
        m.Matches != null &&
        m.Matches.Any()))
    {
        matchResult.Matches = matchResult.Matches
            .Where(m => m.SkipContributorCountRules || m.IsDefinite)
            .ToList();
    }
}

// Usage:
if (matchResults.Any() && source != "Search" && source != "Usage")
{
    RemoveNonDefiniteMatches(matchResults);
}
```

### 3. Replace Magic Numbers with Constants/Enums

**Issue:** Magic numbers make code unclear (e.g., what does `IswcStatusID == 2` mean?).

**Current Code:**

```csharp
if (matchesList[i].IswcStatusID != null && matchesList[i].IswcStatusID == 2)
{
    matchesList.RemoveAt(i);
    i--;
}
```

**Suggested Fix:**

```csharp
// Create enum
public enum IswcStatus
{
    Confirmed = 1,
    Provisional = 2
}

// Usage:
if (match.IswcStatusID == IswcStatus.Provisional)
{
    // Remove provisional matches
}
```

### 4. Replace String Literals with Enums

**Issue:** String comparisons are error-prone and not type-safe.

**Current Code:**

```csharp
if (source != "Usage")
{
    // ...
}

if (source != "Search")
{
    // ...
}
```

**Suggested Fix:**

```csharp
public enum SubmissionSource
{
    Usage,
    Label,
    Search,
    Agency,
    Edi
}

// Update method signature:
public async Task<IEnumerable<Submission>> MatchAsync(
    IEnumerable<Submission> submissions,
    SubmissionSource source)

// Usage:
if (source != SubmissionSource.Usage)
{
    // ...
}
```

### 5. Fix Inefficient Loop Pattern

**Issue:** Removing items from a list while iterating with an index is error-prone and inefficient.

**Current Code (lines 87-94, 104-110, 121-128):**

```csharp
for (int i = 0; i < matchesList.Count; i++)
{
    if (matchesList[i].IswcStatusID != null && matchesList[i].IswcStatusID == 2)
    {
        matchesList.RemoveAt(i);
        i--;  // Dangerous pattern!
    }
}
```

**Suggested Fix:**

```csharp
// Use LINQ for cleaner, safer filtering:
matchResult.Matches = matchResult.Matches
    .Where(m => m.IswcStatusID != IswcStatus.Provisional)
    .ToList();

// Or use RemoveAll:
matchesList.RemoveAll(m => m.IswcStatusID == IswcStatus.Provisional);
```

### 6. Improve Variable Naming

**Issue:** Single-letter variables reduce code readability.

**Current Code:**

```csharp
submissionsForMatching = mapper.Map<IEnumerable<Submission>, IEnumerable<InputWorkInfo>>(
    labelSubmissions.Select(x =>
    {
        if (x.Model.InterestedParties.All(y => !string.IsNullOrEmpty(y.IpBaseNumber)))
        {
            x.Model.InterestedParties = x.Model.InterestedParties.DistinctBy(y => new { y.IpBaseNumber, y.CisacType })
                .Select(c => { c.IPNameNumber = null; c.IpBaseNumber = null; c.Name = null; return c; }).ToList();
        }
        // ...
    })
```

**Suggested Fix:**

```csharp
submissionsForMatching = mapper.Map<IEnumerable<Submission>, IEnumerable<InputWorkInfo>>(
    labelSubmissions.Select(submission =>
    {
        if (submission.Model.InterestedParties.All(party => !string.IsNullOrEmpty(party.IpBaseNumber)))
        {
            submission.Model.InterestedParties = submission.Model.InterestedParties
                .DistinctBy(party => new { party.IpBaseNumber, party.CisacType })
                .Select(contributor =>
                {
                    contributor.IPNameNumber = null;
                    contributor.IpBaseNumber = null;
                    contributor.Name = null;
                    return contributor;
                })
                .ToList();
        }
        // ...
    })
```

### 7. Add Guard Clauses & Early Returns

**Issue:** Deep nesting makes code harder to read.

**Current Code:**

```csharp
if (source != "Usage")
{
    // 100+ lines of code
}
else
{
    // 12 lines of code
}
```

**Suggested Fix:**

```csharp
if (!submissions.Any())
    return submissions;

if (source == "Usage")
    return await ProcessUsageMatches(submissions);

// Continue with non-usage logic without deep nesting
```

### 8. Move Local Functions to Private Methods

**Issue:** Lines 162-196 define nested local functions that are hard to test and reuse.

**Current Code:**

```csharp
public async Task<IEnumerable<Submission>> MatchAsync(...)
{
    // ...

    async Task<Dictionary<int, Submission>> GetMatchesForCUR()
    {
        // 15 lines
    }

    Dictionary<int, Submission> GetUpdatedMatches(IEnumerable<MatchResult> additionalMatches)
    {
        // 17 lines
    }
}
```

**Suggested Fix:**

```csharp
private async Task<Dictionary<int, Submission>> GetMatchesForCUR(
    IEnumerable<Submission> submissions,
    Dictionary<int, Submission> dictionary,
    string source)
{
    var updates = submissions.Where(s => s.TransactionType.Equals(TransactionType.CUR));

    if (!updates.Any())
        return dictionary;

    var updatesToMatch = MapToInputWorkInfo(updates, source);
    updatesToMatch = PrepareForCURMatching(updatesToMatch);

    var updateMatches = await GetMatches(updatesToMatch);

    return MergeUpdatedMatches(updateMatches, dictionary);
}

private Dictionary<int, Submission> MergeUpdatedMatches(
    IEnumerable<MatchResult> additionalMatches,
    Dictionary<int, Submission> dictionary)
{
    foreach (var matchedUpdate in additionalMatches)
    {
        var newMatches = mapper.Map<Bdo.MatchingEngine.MatchResult>(matchedUpdate);

        if (newMatches == null || !newMatches.Matches.Any())
            continue;

        var submissionId = (int)matchedUpdate.InputWorkId;
        if (dictionary.ContainsKey(submissionId))
        {
            dictionary[submissionId].MatchedResult.Matches =
                dictionary[submissionId].MatchedResult.Matches.Concat(newMatches.Matches);
        }
    }

    return dictionary;
}
```

### 9. Add Error Handling

**Issue:** No try-catch blocks despite multiple async HTTP calls that could fail.

**Current Code:**

```csharp
var matchResults = await GetMatches(submissionsForMatching);
```

**Suggested Fix:**

```csharp
private async Task<IEnumerable<MatchResult>> GetMatchesWithErrorHandling(
    IEnumerable<InputWorkInfo> inputs,
    string context)
{
    try
    {
        return await GetMatches(inputs);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Failed to get matches for {Context}", context);
        throw new MatchingServiceException($"Failed to retrieve matches: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error during matching for {Context}", context);
        throw;
    }
}
```

### 10. Reduce Performance Issues

**Issue:** Multiple performance anti-patterns.

**Problems:**

- `DeepCopy()` on line 44 (expensive operation)
- Multiple `.ToList()` calls creating unnecessary copies
- Inefficient loop with `RemoveAt()`

**Suggested Fixes:**

```csharp
// Instead of DeepCopy, consider:
var labelSubmissions = submissions.Select(s => CloneSubmissionForLabelMatching(s));

// Reduce ToList() calls:
var matchesList = matchResults.ToList(); // Do once
// Then work with matchesList

// Use LINQ instead of RemoveAt:
matchResult.Matches = matchResult.Matches
    .Where(predicate)
    .ToList();
```

### 11. Add XML Documentation

**Issue:** Complex business logic lacks documentation.

**Suggested Fix:**

```csharp
/// <summary>
/// Matches submissions against existing works using a multi-stage matching pipeline.
/// </summary>
/// <param name="submissions">Collection of submissions to match against existing works.</param>
/// <param name="source">Source type that determines the matching strategy (Usage, Label, Search, etc.).</param>
/// <returns>Submissions with populated MatchedResult properties.</returns>
/// <remarks>
/// Matching stages for non-Usage sources:
/// 1. Initial matching with full metadata (work info + contributors)
/// 2. Label fallback matching (metadata-only, no contributor matching)
/// 3. Transaction-specific filtering (FSQ removes provisional, CUR adds relaxed matches)
/// 4. Definite match filtering (removes low-confidence matches)
///
/// For Usage sources:
/// - Uses specialized UsageWorkGroup matching endpoint
/// - Different matching algorithm optimized for performance data
/// </remarks>
/// <exception cref="MatchingServiceException">Thrown when matching API calls fail.</exception>
public async Task<IEnumerable<Submission>> MatchAsync(
    IEnumerable<Submission> submissions,
    string source)
{
    // Implementation
}
```

### 12. Consider Strategy Pattern

**Issue:** Different sources have different matching logic hardcoded in if/else blocks.

**Suggested Fix:**

```csharp
public interface IMatchingStrategy
{
    Task<IEnumerable<Submission>> Match(
        IEnumerable<Submission> submissions,
        Dictionary<int, Submission> dictionary);
}

public class UsageMatchingStrategy : IMatchingStrategy
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public async Task<IEnumerable<Submission>> Match(
        IEnumerable<Submission> submissions,
        Dictionary<int, Submission> dictionary)
    {
        // Usage-specific matching logic
    }
}

public class StandardMatchingStrategy : IMatchingStrategy
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public async Task<IEnumerable<Submission>> Match(
        IEnumerable<Submission> submissions,
        Dictionary<int, Submission> dictionary)
    {
        // Standard matching logic with label fallback
    }
}

public class MatchingEngineMatchingService : IMatchingService
{
    private readonly Dictionary<string, IMatchingStrategy> _strategies;

    public MatchingEngineMatchingService(
        IHttpClientFactory httpClientFactory,
        IMapper mapper)
    {
        _strategies = new Dictionary<string, IMatchingStrategy>
        {
            ["Usage"] = new UsageMatchingStrategy(httpClientFactory, mapper),
            ["Standard"] = new StandardMatchingStrategy(httpClientFactory, mapper)
        };
    }

    public async Task<IEnumerable<Submission>> MatchAsync(
        IEnumerable<Submission> submissions,
        string source)
    {
        if (!submissions.Any())
            return submissions;

        var dictionary = submissions.ToDictionary(x => x.SubmissionId);
        var strategyKey = source == "Usage" ? "Usage" : "Standard";

        return await _strategies[strategyKey].Match(submissions, dictionary);
    }
}
```

### 13. Fix Complex LINQ Expression (Line 39)

**Issue:** Line 39 is hard to read and understand.

**Current Code:**

```csharp
matchResults.ToList().ForEach(x => x.Matches = x.Matches?.Where(m =>
    !(m.IswcStatusID == 1 && m.Numbers.Any(n => n.Number.StartsWith("PRS_")))));
```

**Suggested Fix:**

```csharp
private const string PRS_PREFIX = "PRS_";

private bool IsProvisionalMatchWithPrsPrefix(MatchingWork match)
{
    return match.IswcStatusID == IswcStatus.Confirmed &&
           match.Numbers.Any(n => n.Number.StartsWith(PRS_PREFIX));
}

private void FilterOutPrsProvisionalMatches(IEnumerable<MatchResult> matchResults)
{
    foreach (var result in matchResults)
    {
        if (result.Matches == null) continue;

        result.Matches = result.Matches
            .Where(match => !IsProvisionalMatchWithPrsPrefix(match))
            .ToList();
    }
}

// Usage:
FilterOutPrsProvisionalMatches(matchResults);
```

---

## Priority Recommendations

### High Priority (Address First)

1. **Extract methods** - Reduces cyclomatic complexity from ~20+ to manageable levels
2. **Remove duplicate code** - Lines 99-131 duplication
3. **Fix inefficient loops** - Replace `RemoveAt(i); i--;` pattern with LINQ
4. **Replace magic numbers** - Use IswcStatus enum
5. **Add error handling** - Protect against HTTP failures

### Medium Priority (Address Next)

6. **Use enums for source types** - Type safety and IntelliSense support
7. **Improve variable naming** - Better code readability
8. **Extract local functions** - Improve testability
9. **Add guard clauses** - Reduce nesting depth

### Low Priority (Future Improvements)

10. **Add XML documentation** - Better maintainability
11. **Consider design patterns** - Strategy pattern for extensibility
12. **Performance optimizations** - Reduce DeepCopy and ToList() calls

---

## Estimated Impact

### Impact Assessment by Improvement

| Improvement             | Priority   | Readability | Maintainability | Performance | Testability | Overall Impact    |
| ----------------------- | ---------- | ----------- | --------------- | ----------- | ----------- | ----------------- |
| [Extract methods](#1-extract-smaller-focused-methods)         | 🔴 **HIGH** | 🔥 Critical  | 🔥 Critical      | ⚪ Neutral   | 🔥 Critical  | 🏆 **Very High**   |
| [Remove duplication](#2-remove-code-duplication)      | 🔴 **HIGH** | ⬆️ High      | ⬆️ High          | ⬇️ Low       | ⬆️ High      | ⭐ **High**        |
| [Fix loops](#5-fix-inefficient-loop-pattern)               | 🔴 **HIGH** | ⬆️ High      | ⬆️ High          | ➡️ Medium    | ➡️ Medium    | ⭐ **High**        |
| [Constants/Enums](#3-replace-magic-numbers-with-constantsenums)         | 🔴 **HIGH** | ⬆️ High      | ⬆️ High          | ⚪ Neutral   | ➡️ Medium    | ⭐ **High**        |
| [Error handling](#9-add-error-handling)          | 🔴 **HIGH** | ⬇️ Low       | ⬆️ High          | ⚪ Neutral   | ⬆️ High      | ✅ **Medium-High** |
| [Extract local functions](#8-move-local-functions-to-private-methods) | 🟡 MEDIUM   | ➡️ Medium    | ⬆️ High          | ⚪ Neutral   | 🔥 Critical  | ✅ **Medium-High** |
| [Use source enums](#4-replace-string-literals-with-enums)        | 🟡 MEDIUM   | ⬆️ High      | ⬆️ High          | ⚪ Neutral   | ➡️ Medium    | 👍 **Medium**      |
| [Improve naming](#6-improve-variable-naming)          | 🟡 MEDIUM   | ⬆️ High      | ⬆️ High          | ⚪ Neutral   | ⬇️ Low       | 👍 **Medium**      |
| [Performance opts](#10-reduce-performance-issues)        | 🟡 MEDIUM   | ⬇️ Low       | ⬇️ Low           | ⬆️ High      | ⬇️ Low       | 👍 **Medium**      |
| [Add documentation](#11-add-xml-documentation)       | 🟢 LOW      | ⬇️ Low       | ⬆️ High          | ⚪ Neutral   | ⬇️ Low       | 📝 **Low-Medium**  |
| [Strategy pattern](#12-consider-strategy-pattern)        | 🟢 LOW      | ➡️ Medium    | 🔥 Critical      | ⚪ Neutral   | 🔥 Critical  | 👍 **Medium**      |

### Impact Legend

**Priority Levels:**

- 🔴 **HIGH**: Address first, highest ROI
- 🟡 **MEDIUM**: Address next, solid improvements
- 🟢 **LOW**: Future enhancements, strategic value

**Impact Ratings:**

- 🔥 **Critical**: Fundamental improvement, transforms code quality
- ⬆️ **High**: Significant positive impact, highly recommended
- ➡️ **Medium**: Noticeable improvement, worthwhile to implement
- ⬇️ **Low**: Minor improvement, nice-to-have
- ⚪ **Neutral**: No significant impact in this dimension

**Overall Impact:**

- 🏆 **Very High**: Must-do refactoring
- ⭐ **High**: Strong recommendation
- ✅ **Medium-High**: Recommended
- 👍 **Medium**: Consider for next iteration
- 📝 **Low-Medium**: Nice to have

### Key Insights

1. **Extract methods** has the highest overall impact - reduces complexity, improves readability, maintainability, and testability simultaneously
2. **Remove duplication** and **Fix loops** are quick wins with high ROI
3. **Strategy pattern** offers long-term benefits but requires more upfront effort
4. **Performance optimizations** should be measured before implementing (profile first)
5. Most improvements focus on maintainability and readability - the code works but is hard to understand and modify

## Next Steps

1. Create unit tests for existing behavior before refactoring
2. Implement high-priority improvements incrementally
3. Review and test each change independently
4. Consider code review with team before major refactoring
5. Update integration tests to cover edge cases

---

**Document Status:** Draft
**Last Updated:** 2025-10-27
**Author:** Code Analysis
