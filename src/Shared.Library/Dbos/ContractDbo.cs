﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySystem.Shared.Library.Base;

namespace MySystem.Shared.Library.Dbos
{
    public class ContractDbo : ContractBase, IDbo
    {
        [Key]
        public Guid Id { get; set; }

        public virtual ContractDbo Parent { get; set; }
        public ICollection<ContractDbo> Children { get; set; }


        public virtual EmployeeDbo Sales { get; set; }
        public virtual OfficeDbo Office { get; set; }
    }
}
