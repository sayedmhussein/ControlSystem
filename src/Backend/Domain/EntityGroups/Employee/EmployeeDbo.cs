﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeeControl.Backend.Domain.EntityGroups.Territory;
using WeeControl.Common.SharedKernel.EntityGroups.Employee;
using WeeControl.Common.SharedKernel.Interfaces;

namespace WeeControl.Backend.Domain.EntityGroups.Employee
{
    public class EmployeeDbo : BaseEmployee, IDatabaseObject
    {
        [Key]
        public Guid Id { get; set; }

        public virtual EmployeeDbo ReportTo { get; set; }
        public virtual TerritoryDbo Territory { get; set; }

        public virtual ICollection<EmployeeClaimDbo> Claims { get; set; }
        public virtual ICollection<EmployeeIdentityDbo> Identities { get; set; }
        public virtual ICollection<EmployeeSessionDbo> Sessions { get; set; }
    }
   
}
