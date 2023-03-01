﻿using WeeControl.Core.DataTransferObject.Contexts.Essentials;
using WeeControl.Core.SharedKernel.Contexts.Essentials;

namespace WeeControl.Host.WebApiService.Contexts.Essentials;

public interface IUserService
{
    Task AddCustomer(CustomerRegisterDto dto);
    Task AddEmployee(EmployeeRegisterDto dto);
    
    
    
    Task<UserModel> GetUser();
    Task EditUser(object dto);
    Task ChangePassword(UserPasswordChangeRequestDto dto);
}