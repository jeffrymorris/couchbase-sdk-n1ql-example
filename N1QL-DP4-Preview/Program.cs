using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;

namespace N1QL_DP4_Preview
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var cluster = new Cluster())
            {
                using (var bucket = cluster.OpenBucket("beer-sample"))
                {
                    //CreateIndex(bucket);
                    //PositionalParameters(bucket);
                    NamedParameters(bucket);
                    Console.ReadKey();
                }
            }
        }

        static void CreateIndex(IBucket bucket)
        {
            IQueryResult<dynamic> result;
            var indexQuery = new QueryRequest().Statement("SELECT name FROM system:keyspaces");
            if (bucket.Query<dynamic>(indexQuery).Rows.Any(index => index.name == "beer-sample"))
            {
                var deleteIndexQuery = new QueryRequest().Statement("DROP PRIMARY INDEX ON `beer-sample`");
                result = bucket.Query<dynamic>(deleteIndexQuery);
                Console.WriteLine("PRIMARY Index on `beer-sample` was deleted: {0}", result.Success);
            }
            var createIndexQuery = new QueryRequest().Statement("CREATE PRIMARY INDEX ON `beer-sample`");
            result = bucket.Query<dynamic>(createIndexQuery);
            Console.WriteLine("PRIMARY Index on `beer-sample` was created: {0}", result.Success);
        }

        static void PositionalParameters(IBucket bucket)
        {
            var queryRequest = new QueryRequest()
                .Statement("SELECT * FROM `beer-sample` WHERE type=$1 LIMIT $2")
                .AddPositionalParameter("beer")
                .AddPositionalParameter(10);

            var result = bucket.Query<dynamic>(queryRequest);
            foreach (var row in result.Rows)
            {
                Console.WriteLine(row);
            }
        }

        static void NamedParameters(IBucket bucket)
        {
            var queryRequest = new QueryRequest()
                .Statement("SELECT * FROM `beer-sample` WHERE type=$type LIMIT $limit")
                .AddNamedParameter("limit", 10)
                .AddNamedParameter("type", "beer");

            var result = bucket.Query<dynamic>(queryRequest);
            foreach (var row in result.Rows)
            {
                Console.WriteLine(row);
            }
        }
    }
}
