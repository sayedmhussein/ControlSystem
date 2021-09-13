﻿using System.Collections.Generic;
using WeeControl.SharedKernel.EntityGroups.Employee.Enums;
using WeeControl.SharedKernel.EntityGroups.Employee.Interfaces;
using WeeControl.SharedKernel.Helpers;

namespace WeeControl.SharedKernel.EntityGroups.Employee.Attributes
{
    public class EmployeeAppSetting : IEmployeeAttribute
    {
        private readonly AppSettingReader appSettingReader;
        
        private Dictionary<PersonalTitleEnum, string> personTitle;
        private Dictionary<PersonalGenderEnum, string> personGender;
        private Dictionary<IdentityTypeEnum, string> identityType;

        private Dictionary<ClaimTypeEnum, string> claimType;
        private Dictionary<ClaimTagEnum, string> claimTag;

        public EmployeeAppSetting()
        {
            appSettingReader = new AppSettingReader(typeof(EmployeeAppSetting).Namespace, "attributes.json");
        }
        public string GetClaimTag(ClaimTagEnum tag)
        {
            appSettingReader.PopulateAttribute(ref claimTag, "ClaimTags");
            return claimTag[tag];
        }

        public string GetClaimType(ClaimTypeEnum type)
        {
            appSettingReader.PopulateAttribute(ref claimType, "ClaimTypes");
            return claimType[type];
        }

        public string GetPersonalGender(PersonalGenderEnum gender)
        {
            appSettingReader.PopulateAttribute(ref personGender, "Genders");
            return personGender[gender];
        }

        public string GetPersonalIdentity(IdentityTypeEnum identity)
        {
            appSettingReader.PopulateAttribute(ref identityType, "IdentityTypes");
            return identityType[identity];
        }

        public string GetPersonalTitle(PersonalTitleEnum title)
        {
            appSettingReader.PopulateAttribute(ref personTitle, "Titles");
            return personTitle[title];
        }
    }
}