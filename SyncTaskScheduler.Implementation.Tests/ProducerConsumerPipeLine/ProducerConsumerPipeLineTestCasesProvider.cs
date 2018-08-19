using System.Collections.Generic;
using System.Linq;

namespace SyncTaskScheduler.Implementation.Tests.ProducerConsumerPipeLine
{
    public class ProducerConsumerPipeLineTestCasesProvider
    {
        private static readonly IEnumerable<int> TestActionDurationMsTestCaseValues = new List<int>()
        {
            500, 50
        };

        private static readonly IEnumerable<int> NumberOfProducersTestCaseValues = new List<int>()
        {
            10, 3
        };

        private static readonly IEnumerable<int> MaximumPipelineCapacityTestCaseValues = new List<int>()
        {
            1, 3, 5
        };

        // testActionDurationMs : numberOfProducers : maximumPipelineCapacity
        public static IEnumerable<object> CommonTestCaseSource()
        {
            var allTestParameterCombinations =
                from first in TestActionDurationMsTestCaseValues
                from second in NumberOfProducersTestCaseValues
                from third in MaximumPipelineCapacityTestCaseValues
                select new[] { first, second, third };

            return allTestParameterCombinations;
        }
    }
}
