using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Collections;

namespace SpanishPoint.Azure.Iswc.Bdo.Iswc
{
    public class IswcModel
    {
        public IswcModel()
        {
            LinkedIswc = new List<IswcModel>();
            VerifiedSubmissions = new List<VerifiedSubmissionModel>();
            LinkedIswcTree = new TreeNode<IswcModel>(this);
            Iswc = string.Empty;
            Agency = string.Empty;
        }

        public string Iswc { get; set; }
        public string Agency { get; set; }
        public long? IswcId { get; set; }
        public bool Status { get; set; }
        public IswcModel? ParentIswc { get; set; }
        public IswcModel? OverallParentIswc { get; set; }
        public ICollection<IswcModel> LinkedIswc { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? LastModifiedUser { get; set; }
        public int? RankScore { get; set; }
        public int? IswcStatusId { get; set; }
        public IEnumerable<VerifiedSubmissionModel> VerifiedSubmissions { get; set; }
        [JsonIgnore]
        public TreeNode<IswcModel> LinkedIswcTree { get; set; }
    }
}