﻿using System.Collections;
using System.Collections.Generic;
using WeeControl.SharedKernel.BasicSchemas.Employee;
using WeeControl.SharedKernel.BasicSchemas.Employee.Enums;

namespace WeeControl.Server.Application.Test.Territory.V1.Queries
{
    public class GetTerritoriesV1HandlerTestData : IEnumerable<object[]>
    {
        private readonly IEmployeeLists values = new EmployeeLists();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { values.GetClaimType(ClaimTypeEnum.HumanResources), values.GetClaimTag(ClaimTagEnum.Read), false };
            yield return new object[] { values.GetClaimType(ClaimTypeEnum.HumanResources), values.GetClaimTag(ClaimTagEnum.Add), true };
            yield return new object[] { values.GetClaimType(ClaimTypeEnum.Territory), values.GetClaimTag(ClaimTagEnum.Read), true };
            yield return new object[] { "", values.GetClaimTag(ClaimTagEnum.Read), true };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
