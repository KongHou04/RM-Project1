﻿using BLL.Interfaces;
using DAL.Interfaces;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserSVC : IUserSVC
    {
        private readonly ImageHandlerSVC _imageHandlerSVC;
        private readonly IAccountRES _accountRES;
        private readonly IEmployeeRES _employeeRES;
        private readonly IRoleRES _roleRES;
        public UserSVC(IAccountRES accountRES, IEmployeeRES employeeRES, IRoleRES roleRES, ImageHandlerSVC imageHandlerSVC)
        {
            _accountRES = accountRES;
            _employeeRES = employeeRES;
            _roleRES = roleRES;
            _imageHandlerSVC = imageHandlerSVC;
        }

        public EmployeeDTO? Login(string username, string password)
        {
            var acc = _accountRES.GetByUserName(username);
            if (acc == null) return null;
            if (acc.HashPassword != password) return null;
            if (acc.EmployeeID == null) return null;
            var emp = _employeeRES.GetByID((int)acc.EmployeeID);
            if (emp == null) return null;
            if (emp.RoleID == null) return null;
            var role = _roleRES.GetByID((int)emp.RoleID);
            if (role == null) return null;
            return new EmployeeDTO()
            {
                ID = emp.EmployeeID,
                FullName = emp.FullName,
                BirthDate = emp.BirthDate,
                Email = emp.Email,
                Status = emp.Status,
                Avatar = _imageHandlerSVC.GetImageDirecory(emp.Avatar, true),
                Phone = emp.Phone,
                UserName = acc.UserName,
                RoleName = role.Name,
                RoleID = role.RoleID,
                AvatarBitMap = _imageHandlerSVC.GetBitMap(emp.Avatar, true),
            };
        }

        public bool UpdatePassword(string email, string newPassword)
        {
            var emp = _employeeRES.GetByEmail(email);
            if (emp == null) return false;
            var acc = _accountRES.GetByEmpID(emp.EmployeeID);
            if (acc == null) return false;
            acc.HashPassword = newPassword;
            return (_accountRES.Update(acc))? true: false;
        }
        
        public string? GetUserNameByEmail(string email)
        {
            var emp = _employeeRES.GetByEmail(email);
            if (emp == null) return null;
            var acc = _accountRES.GetByEmpID((int)emp.EmployeeID);
            if (acc == null) return null;
            if (emp.RoleID == null) return null;
            var role = _roleRES.GetByID((int)emp.RoleID);
            if (role == null) return null;
            return acc.UserName;
        }
    }
}
