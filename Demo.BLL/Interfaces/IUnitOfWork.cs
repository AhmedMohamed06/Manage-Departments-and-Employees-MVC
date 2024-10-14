﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        //Signature For Property For  Each And Every Repostory Interface

        public IEmployeeRepository EmployeeRepository { get; set; }
        public IDepartmentRepository DepartmentRepository { get; set; }
       Task< int> CompleteAsync();
        void Dispose();
    }
}
