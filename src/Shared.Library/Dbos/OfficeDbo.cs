﻿using System;
using System.ComponentModel.DataAnnotations;
using MySystem.Shared.Library.Base;

namespace MySystem.Shared.Library.Dbos
{
    public class OfficeDbo : OfficeBase, IDbo
    {
        [Key]
        [Display(Name = "ID")]
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }
        public virtual OfficeDbo Parent { get; set; }

        public OfficeDbo()
        {
        }

        public OfficeDbo(string name, string country) : this()
        {
            OfficeName = name;
            CountryId = country;
        }

        public OfficeDbo(string name, string country, Guid parentid) : this(name, country)
        {
            ParentId = parentid;
        }
    }
}
