// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;

namespace SkillFunctionalTests.Common
{
    public class TestCaseBuilder
    {
        public IEnumerable<T[]> CartesianProduct<T>(params IEnumerable<T>[] sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }))
                .Select(e =>
                {
                    var list = new List<T>();
                    list.AddRange(e);
                    return list.ToArray();
                });
        }
    }
}
