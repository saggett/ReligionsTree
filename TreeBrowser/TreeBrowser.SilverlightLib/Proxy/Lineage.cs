using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace TreeBrowser.SilverlightLib.Proxy
{
    public class Lineage
    {
        
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentLineageId { get; set; }

        public int StartYear { get; set; }

        public int? EndYear { get; set; }

        private string _wikipediaArticleName;
        public string WikipediaArticleName
        {
            get { return _wikipediaArticleName ?? string.Empty; }
            set { _wikipediaArticleName = value; }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes ?? string.Empty; }
            set { _notes = value; }
        }

        public int? LineageGroupId { get; set; }

        public bool IsNew
        {
            get { return Id == 0; }
        }

        public int RealStartYear
        {
            get { return StartYear + ContractProxy.RootLineageStartYear; }
            set { StartYear = value - ContractProxy.RootLineageStartYear; }
        }

        public int? RealEndYear
        {
            get { return EndYear.HasValue ? EndYear + ContractProxy.RootLineageStartYear : null; }
            set { EndYear = value.HasValue ? value - ContractProxy.RootLineageStartYear : null; }
        }

        public string DisplayStartYear
        {
            get { return GenerateDisplayYear(RealStartYear); }
        }

        public string LineageGroupName
        {
            get { return LineageGroupId.HasValue ? Cache.LookupLineageGroupName(LineageGroupId.Value) : string.Empty; }
        }

        public static string GenerateDisplayYear(int realYear)
        {
            string adSuffix = realYear < 0 ? "BCE" : "CE";
            if (realYear < 0)
                realYear = realYear * -1;
            return String.Format("{0} {1}", realYear, adSuffix);
        }

        public Lineage Clone()
        {
            return new Lineage()
                       {
                           Id = this.Id,
                           EndYear = this.EndYear,
                           Name = this.Name,
                           ParentLineageId = this.ParentLineageId,
                           StartYear = this.StartYear,
                           WikipediaArticleName = this.WikipediaArticleName,
                           Notes = this.Notes,
                           LineageGroupId = this.LineageGroupId
                       };
        }

        public Uri WikipediaArticleUrl
        {
            get { return new Uri(String.Format("http://en.wikipedia.org/wiki/{0}", WikipediaArticleName)); }
        }

        public string FoundingText
        {
            get
            {
                var builder = new StringBuilder();
                if (LineageGroupId.HasValue)
                    builder.AppendFormat("Part of {0}. ", LineageGroupName);
                builder.AppendFormat("Introduced {0}", DisplayStartYear);
                if (EndYear.HasValue)
                    builder.AppendFormat(", ended {0}", DisplayEndYear);
                builder.Append(". ");
                return builder.ToString();
            }
        }

        public string DisplayEndYear    
        {
            get { return RealEndYear.HasValue ? GenerateDisplayYear(RealEndYear.Value) : string.Empty; }
        }


        public Lineage Parent
        {
            get
            {
                return ParentLineageId.HasValue ? Cache.Lineages.LookupLineage(ParentLineageId.Value) : null;
            }
        }

        public string ParentName
        {
            get
            {
                return Parent != null ? Parent.Name : string.Empty;
            }
        }

        public IEnumerable<Lineage> DirectChildren
        {
            get
            {
                return Cache.Lineages.Where(lin => lin.ParentLineageId == Id);
            }
        }

        public IList<Lineage> Children
        {
            get
            {
                var output = new List<Lineage>();
                foreach (Lineage lineage in DirectChildren)
                {
                    output.Add(lineage);
                    output.AddRange(lineage.Children);
                }
                return output;
            }
        }

        public int ChildCount
        {
            get { return Children.Count; }
        }

        public bool HasChildren
        {
            get { return Cache.Lineages.Any(lin => lin.ParentLineageId == Id); }
        }

    }
}