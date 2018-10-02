using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace AutoAPI.IntegrationTests
{
    public class PriorityOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var sortedMethods = new List<KeyValuePair<int, TTestCase>>();

            foreach (TTestCase testCase in testCases)
            {
                int priority = 0;

                foreach (IAttributeInfo attr in testCase.TestMethod.Method.GetCustomAttributes((typeof(TestPriorityAttribute).AssemblyQualifiedName)))
                {
                    priority = attr.GetNamedArgument<int>("Priority");
                }

                sortedMethods.Add(new KeyValuePair<int, TTestCase>(priority, testCase));
            }

            return sortedMethods.OrderBy(x => x.Key).Select(x => x.Value);

        }

    }
}
