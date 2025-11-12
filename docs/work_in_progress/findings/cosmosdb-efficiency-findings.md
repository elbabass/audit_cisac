# CosmosDB Efficiency Issues - Audit Service

**Analysis Date**: 2025-11-12
**File**: `F:\Teragone\ISWC-2\src\Data\Services\Audit\CosmosDb\CosmosDbAuditService.cs`
**Status**: 🔴 CRITICAL - Multiple High-Cost Patterns Identified

---

## Executive Summary

The CosmosDbAuditService contains multiple critical efficiency issues that are likely causing excessive RU consumption. The primary culprits are:
- **N+1 query patterns** in loops (worst offender)
- **Cross-partition queries** without partition keys
- **Sequential operations** that could be batched
- **Queries inside nested loops** (extremely expensive)

**Estimated Impact**: These issues could be consuming **10-100x more RUs** than necessary.

---

## 🔴 CRITICAL Issues

### 1. N+1 Query Pattern in LogSubmissions - Nested Loop Queries
**Location**: Lines 207-240
**Severity**: 🔴 CRITICAL
**RU Impact**: Very High (potentially thousands of RUs per batch)

```csharp
foreach (var iswc in submission.Model.IswcsToMerge)
{
    // Query #1 in loop - Line 212-213
    merSubmission.Model.WorkNumber.Number = (await workRepository.FindManyAsyncOptimizedByPath(
        x => x.AgencyId == submission.Model.Agency && x.Iswc.Iswc1 == iswc, "Iswc"))
        ?.OrderByDescending(x => x.LastModifiedDate)?.FirstOrDefault()?.AgencyWorkCode;

    // Query #2 in loop - Line 215-218
    var mergeWorkflowInstanceId = (await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(
        x => x.Iswc.Iswc1 == iswc && x.LinkedToIswc == submission.IswcModel.Iswc && x.Status,
        "Iswc", "MergeRequestNavigation", "MergeRequestNavigation.WorkflowInstance"))...

    // Upsert - Line 220
    await auditRequestContainer.UpsertItemAsync(new AuditRequestModel { ... });
}
```

**Problem**: For each ISWC to merge, this executes 2-3 database queries. With 10 ISWCs, that's 20-30 queries instead of 1-2 bulk queries.

**Recommendation**:
- Collect all ISWCs first
- Execute bulk queries using `Contains()` or `IN` operators
- Cache results and map them in memory

---

### 2. N+1 Query Pattern - Work Numbers Loop
**Location**: Lines 241-282
**Severity**: 🔴 CRITICAL

```csharp
foreach (var number in submission.Model.WorkNumbersToMerge)
{
    // Query in loop - Line 246-247
    var childIswc = (await workRepository.FindAsyncOptimizedByPath(
        x => x.AgencyWorkCode == number.Number && x.AgencyId == number.Type, "Iswc"))?.Iswc;

    // Another query in loop - Line 250-253
    mergeWorkflowInstanceId = (await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(...))...

    // Upsert - Line 262
    await auditRequestContainer.UpsertItemAsync(new AuditRequestModel { ... });
}
```

**Problem**: Same N+1 pattern. Each work number triggers 2-3 database calls.

**Recommendation**: Batch query all work numbers at once before the loop.

---

### 3. N+1 Query Pattern in Main Submission Loop
**Location**: Lines 80-321
**Severity**: 🔴 CRITICAL

```csharp
foreach (var submission in submissions)
{
    // Line 95-98: Query for MER transactions
    workflowInstanceId = (await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(...))...

    // Line 103-104: Query for work numbers
    var childIswc = (await workRepository.FindAsyncOptimizedByPath(...))...

    // Line 107-110: Another workflow query
    workflowInstanceId = (await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(...))...

    // Line 115-120: Query for DMR
    var iswc = (await workRepository.FindAsyncOptimizedByPath(...))...
}
```

**Problem**: Each submission in the batch triggers 2-5 database queries. For 100 submissions, that's 200-500 queries!

**Recommendation**:
- Extract all required ISWCs/work numbers upfront
- Execute 1-2 bulk queries before the loop
- Use dictionary lookups inside the loop

---

### 4. Cross-Partition Query with Empty Partition Key
**Location**: Lines 380-381, 390-391
**Severity**: 🔴 CRITICAL
**RU Impact**: Extremely High

```csharp
// Line 380-381: Empty partition key = full scan
var allocatedSub = (await auditRequestContainer
    .GetItemsAsync(x => x.Work.PreferredIswc == preferredIswc
        && x.TransactionError == null
        && x.TransactionType == TransactionType.CAR
        && x.Work.AdditionalIdentifiers.Any(), $""));  // ⚠️ Empty partition key!

// Line 390-391: Another cross-partition query
results = results.Union(mapper.Map<IEnumerable<AuditHistoryResult>>(await auditRequestContainer
    .GetItemsAsync(x => x.Work.PreferredIswc == preferredIswc
        && x.TransactionError == null
        && x.TransactionType != TransactionType.FSQ, $"{preferredIswc}")));
```

**Problem**: Empty partition key forces CosmosDB to scan ALL partitions. This is one of the most expensive operations possible.

**Recommendation**:
- Always provide a partition key
- If you must query across partitions, use continuation tokens and limit results
- Consider redesigning partition key strategy to include `preferredIswc`

---

### 5. Expensive Cross-Partition Query in Statistics
**Location**: Lines 602-606
**Severity**: 🔴 CRITICAL

```csharp
var workflows = await workflowInstanceRepository
    .FindManyAsyncOptimizedByPath(x => x.WorkflowTask.Any(x =>
    relevantAgencies.Contains(x.AssignedAgency.AgencyId))  // Contains = cross-partition
    && relevantMonths.Contains(x.CreatedDate.Month)        // Contains = cross-partition
    && relevantYears.Contains(x.CreatedDate.Year)          // Contains = cross-partition
    && !x.IsDeleted, $"{nameof(DataModels.WorkflowTask)}");
```

**Problem**: Multiple `Contains()` operations without partition key = full database scan. This could consume thousands of RUs.

**Recommendation**:
- Iterate through agencies and execute targeted queries per agency
- Use proper partition keys
- Consider pre-aggregating this data

---

## 🟠 HIGH Priority Issues

### 6. No Batch Operations for Upserts
**Location**: Lines 70, 144, 167, 172, 220, 262, 298
**Severity**: 🟠 HIGH
**RU Impact**: High (2-3x more RUs than necessary)

```csharp
// Sequential upserts
await auditContainer.UpsertItemAsync(...);           // Line 70
await auditRequestContainer.UpsertItemAsync(...);    // Line 144
await auditRequestContainer.UpsertItemAsync(...);    // Line 172
// ... many more
```

**Problem**: Each `UpsertItemAsync` is a separate round-trip to CosmosDB. For 100 submissions with 3 upserts each = 300 separate calls.

**Recommendation**:
- Use `TransactionalBatch` for multiple operations on same partition
- Use bulk operations (`BulkExecutor` or Bulk support in SDK v3+)
- Collect all operations and execute in batches of 100

---

### 7. Sequential Upserts in Nested Loops
**Location**: Lines 135-169
**Severity**: 🟠 HIGH

```csharp
foreach (var worknum in submission.Model.AdditionalAgencyWorkNumbers)
{
    // Individual upsert per work number
    await auditRequestContainer.UpsertItemAsync(new AuditRequestModel { ... });
}
```

**Problem**: Each additional work number = separate database call.

**Recommendation**: Collect all models and use batch upsert.

---

### 8. Query Without Partition Key in Search
**Location**: Lines 402, 445-446
**Severity**: 🟠 HIGH

```csharp
// Line 402: maxItemCount limits results but still cross-partition
results = results.Union(mapper.Map<List<AuditReportResult>>(
    await auditRequestContainer.GetItemsAsync(predicate, 1000)));
```

**Problem**: Complex predicate with date ranges but no partition key = cross-partition query.

**Recommendation**:
- If partition key is `AgencyCode`, execute separate queries per agency
- Use continuation tokens for pagination
- Consider indexing strategy

---

### 9. Inefficient Statistics Query
**Location**: Lines 549-551
**Severity**: 🟠 HIGH

```csharp
var existingStatistics = (await agencyStatisticsContainer
    .GetItemsAsync(x => x.Day == statistics.Day,
        $"{agency}_{statistics.Month}_{statistics.Year}_{transactionSource}"))
    .FirstOrDefault();
```

**Problem**: This is called in nested loops (line 544). Better to use point reads.

**Recommendation**:
- If you know the ID, use `ReadItemAsync` (point read = 1 RU)
- `GetItemsAsync` with FirstOrDefault is inefficient (query = 2-5+ RUs)

---

## 🟡 MEDIUM Priority Issues

### 10. Over-fetching with Multiple Includes
**Location**: Lines 95-98, 103-104, 107-110, 115-120
**Severity**: 🟡 MEDIUM

```csharp
// Multiple navigation properties loaded
await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(
    x => ...,
    "Iswc",
    "MergeRequestNavigation",
    "MergeRequestNavigation.WorkflowInstance")
```

**Problem**: Loading multiple levels of navigation properties increases document size and RUs.

**Recommendation**:
- Only include properties you actually use
- Consider denormalizing frequently accessed data

---

### 11. Inefficient Grouping Strategy
**Location**: Line 544
**Severity**: 🟡 MEDIUM

```csharp
foreach (var monthlyAgencyChanges in agencyChangesByTransSource.GroupBy(x => x.CreatedDate.Day))
```

**Problem**: Grouping by Day creates many small groups, each triggering a query.

**Recommendation**: Consider aggregating at month level if possible.

---

### 12. Full Scan in Workflow Statistics
**Location**: Lines 592-593
**Severity**: 🟡 MEDIUM

```csharp
var iterator = agencyStatisticsContainer.GetItemsFeedIterator(
    x => x.WorkflowTasksAssignedPending > 0);  // No partition key
```

**Problem**: Scanning all statistics documents.

**Recommendation**:
- Use partition key if possible
- Consider maintaining a separate "pending" collection
- Use Change Feed instead

---

## 💡 Recommended Solutions

### Immediate Actions (Quick Wins)

1. **Fix Empty Partition Key Queries** (Lines 380-381)
   - Provide actual partition keys or redesign the query logic
   - **Estimated savings**: 50-80% RU reduction on these queries

2. **Implement Batch Upserts**
   ```csharp
   // Instead of:
   foreach (var item in items) {
       await container.UpsertItemAsync(item);
   }

   // Use:
   var tasks = items.Select(item => container.UpsertItemAsync(item));
   await Task.WhenAll(tasks);  // Parallel execution

   // Or better, use TransactionalBatch for same partition
   ```
   - **Estimated savings**: 40-60% RU reduction

3. **Convert Point Reads** (Line 549-551)
   ```csharp
   // Instead of GetItemsAsync + FirstOrDefault
   // Use ReadItemAsync if you know the ID
   try {
       var stat = await container.ReadItemAsync<AgencyStatisticsModel>(id, partitionKey);
   } catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound) {
       // Handle not found
   }
   ```
   - **Estimated savings**: 80% RU reduction on these reads

### Medium-term Refactoring

4. **Eliminate N+1 Patterns**
   ```csharp
   // Step 1: Collect all required lookups
   var allIswcs = submissions.SelectMany(s => s.Model.IswcsToMerge).Distinct();
   var allWorkNumbers = submissions.SelectMany(s => s.Model.WorkNumbersToMerge).Distinct();

   // Step 2: Bulk query (1-2 queries total)
   var iswcData = await repository.FindManyAsync(x => allIswcs.Contains(x.Iswc));
   var workData = await repository.FindManyAsync(x => allWorkNumbers.Contains(x.Number));

   // Step 3: Create lookup dictionaries
   var iswcLookup = iswcData.ToDictionary(x => x.Iswc);
   var workLookup = workData.ToDictionary(x => x.Number);

   // Step 4: Use lookups in loop (no database calls)
   foreach (var submission in submissions) {
       var data = iswcLookup[submission.Model.PreferredIswc];
       // ... process without database calls
   }
   ```
   - **Estimated savings**: 90-95% RU reduction on these operations

5. **Redesign Partition Key Strategy**
   - Current strategy seems inconsistent
   - Consider: `AgencyCode` or `{AgencyCode}_{ISWC}` as partition key
   - Ensure all queries can provide partition key

### Long-term Optimizations

6. **Implement Caching Layer**
   - Cache frequently accessed reference data (agencies, work numbers)
   - Use Redis or in-memory cache
   - **Estimated savings**: 30-50% overall RU reduction

7. **Use Change Feed Properly**
   - Line 516 already uses Change Feed (good!)
   - Consider using Change Feed for other statistics calculations

8. **Consider Pre-aggregation**
   - Pre-calculate statistics at write time instead of query time
   - Use materialized views pattern

9. **Implement Bulk Executor**
   ```csharp
   // For high-volume upserts
   var bulkOperations = items.Select(item =>
       new CosmosOperation(OperationType.Upsert, item, partitionKey));

   await container.BulkExecuteAsync(bulkOperations);
   ```

---

## Performance Optimization Checklist

- [ ] Remove all empty partition key queries (Lines 380-381)
- [ ] Eliminate N+1 patterns in LogSubmissions (Lines 207-282)
- [ ] Eliminate N+1 patterns in main loop (Lines 80-321)
- [ ] Convert queries to bulk operations with upfront data loading
- [ ] Implement batch upsert operations
- [ ] Convert GetItemsAsync + FirstOrDefault to ReadItemAsync point reads
- [ ] Add partition keys to all queries in Search methods
- [ ] Optimize Contains() queries in statistics (Lines 602-606)
- [ ] Implement parallel processing where possible
- [ ] Add RU consumption monitoring/logging
- [ ] Review and optimize partition key strategy
- [ ] Consider implementing caching layer
- [ ] Add integration tests to measure RU consumption per operation

---

## Estimated Impact

**Current State**: Likely consuming **500-2000+ RUs per batch** of submissions
**After Fixes**: Could reduce to **50-200 RUs per batch** (10x improvement)

**Monthly Savings**: Assuming 1M operations/month at current 1000 RU avg vs optimized 100 RU avg:
- Current: ~1 billion RUs/month
- Optimized: ~100 million RUs/month
- **Savings: 900 million RUs/month** (significant cost reduction)

---

## Next Steps

1. **Immediate**: Fix critical issues (empty partition keys, point reads)
2. **Week 1-2**: Eliminate N+1 patterns with bulk queries
3. **Week 3-4**: Implement batch operations
4. **Month 2**: Review partition key strategy and implement caching
5. **Ongoing**: Monitor RU consumption with Application Insights

---

## Additional Recommendations

### Enable RU Monitoring
```csharp
// Add RU tracking to understand actual consumption
var response = await container.UpsertItemAsync(item);
_logger.LogInformation($"Upsert RU consumption: {response.RequestCharge}");
```

### Add Performance Tests
- Create integration tests that measure RU consumption
- Set RU budgets per operation
- Fail tests if operations exceed budget

### Code Review Guidelines
- Always provide partition key
- Never query in loops (N+1)
- Use point reads when ID is known
- Batch operations when possible
- Monitor RU consumption

---

**Report Generated**: 2025-11-12
**Analyst**: Claude Code AI Analysis
**Confidence Level**: High - Based on well-known CosmosDB anti-patterns
