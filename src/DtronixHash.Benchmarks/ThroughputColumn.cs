using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DtronixHash.Benchmarks
{
    public class ThroughputColumn : IColumn
    {
        public string Id { get; }
        public string ColumnName { get; }

        public ThroughputColumn(string columnName)
        {
            ColumnName = columnName;
            Id = nameof(ThroughputColumn) + "." + ColumnName;
        }

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        {
            try
            {
                var dataLength = double.Parse(benchmarkCase.Parameters["DataSize"].ToString());
                var statistics = summary[benchmarkCase].ResultStatistics;


                return $"{(dataLength / 1024 / 1024) / (statistics.Mean * 0.000000001):N1}";
            }
            catch (Exception e)
            {
                return "-";
            }
        } 

        public bool IsAvailable(Summary summary) => true;
        public bool AlwaysShow => true;
        public ColumnCategory Category => ColumnCategory.Custom;
        public int PriorityInCategory => 0;
        public bool IsNumeric => false;
        public UnitType UnitType => UnitType.Dimensionless;
        public string Legend => $"Throughput {ColumnName} column.";
        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);
        public override string ToString() => ColumnName;
    }
}
