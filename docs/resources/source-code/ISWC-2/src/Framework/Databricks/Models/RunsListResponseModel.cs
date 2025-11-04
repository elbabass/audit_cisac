using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Databricks.Models
{
    [ExcludeFromCodeCoverage]
    public partial class RunsListResponseModel
    {
        [JsonProperty("runs")]
        public List<Run> Runs { get; set; } = new List<Run>();

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
    }

    public partial class Run
    {
        [JsonProperty("job_id")]
        public long JobId { get; set; }

        [JsonProperty("run_id")]
        public long RunId { get; set; }

        [JsonProperty("number_in_job")]
        public long NumberInJob { get; set; }

        [JsonProperty("original_attempt_run_id")]
        public long OriginalAttemptRunId { get; set; }

        [JsonProperty("state")]
        public State? State { get; set; }

        [JsonProperty("task")]
        public RunTask Task { get; set; } = new RunTask();

        [JsonProperty("cluster_spec")]
        public ClusterSpec? ClusterSpec { get; set; }

        [JsonProperty("cluster_instance")]
        public ClusterInstance? ClusterInstance { get; set; }

        [JsonProperty("overriding_parameters")]
        public OverridingParameters OverridingParameters { get; set; } = new OverridingParameters();

        [JsonProperty("start_time")]
        public long StartTime { get; set; }

        [JsonProperty("setup_duration")]
        public long SetupDuration { get; set; }

        [JsonProperty("execution_duration")]
        public long ExecutionDuration { get; set; }

        [JsonProperty("cleanup_duration")]
        public long CleanupDuration { get; set; }

        [JsonProperty("trigger")]
        public string? Trigger { get; set; }

        [JsonProperty("creator_user_name")]
        public string? CreatorUserName { get; set; }

        [JsonProperty("run_name")]
        public string? RunName { get; set; }

        [JsonProperty("run_page_url")]
        public Uri? RunPageUrl { get; set; }

        [JsonProperty("run_type")]
        public string? RunType { get; set; }
    }

    public partial class ClusterInstance
    {
        [JsonProperty("cluster_id")]
        public string? ClusterId { get; set; }

        [JsonProperty("spark_context_id")]
        public string? SparkContextId { get; set; }
    }

    public partial class ClusterSpec
    {
        [JsonProperty("existing_cluster_id")]
        public string? ExistingClusterId { get; set; }
    }

    public partial class OverridingParameters
    {
        [JsonProperty("notebook_params")]
        public NotebookParameters NotebookParameters { get; set; } = new NotebookParameters();
    }

    public partial class State
    {
        [JsonProperty("life_cycle_state")]
        public string? LifeCycleState { get; set; }

        [JsonProperty("state_message")]
        public string? StateMessage { get; set; }
    }

    public partial class RunTask
    {
        [JsonProperty("notebook_task")]
        public NotebookTask NotebookTask { get; set; } = new NotebookTask();
    }

    public partial class NotebookTask
    {
        [JsonProperty("notebook_path")]
        public string? NotebookPath { get; set; }

        [JsonProperty("base_parameters")]
        public NotebookParameters BaseParameters { get; set; } = new NotebookParameters();
    }
}
