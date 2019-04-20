using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepLib
{
    public class ResultContainer
    {
        public List<Result> Results { get; }
        public int NumberOfSuccesses
        {
            get
            {
                return Results.Count(x => x.IsSuccess == true);
            }
        }

        public ResultContainer()
        {
            this.Results = new List<Result>();
        }

        public void Add(Result result)
        {
            Results.Add(result);
        }

        public void Merge(ResultContainer results)
        {
            Results.AddRange(results.Results);
        }

        public ResultContainer DeepCopy()
        {
            var copy = new ResultContainer();

            foreach(var r in Results)
            {
                copy.Add(r);
            }

            return copy;
        }

        public override string ToString()
        {
            if(Results.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach(var result in Results)
            {
                sb.AppendLine(result.ToString());
            }

            return sb.ToString();
        }
    }
}
