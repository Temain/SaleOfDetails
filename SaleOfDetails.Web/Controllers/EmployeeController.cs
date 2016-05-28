using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SaleOfDetails.Domain.Context;
using SaleOfDetails.Domain.DataAccess.Interfaces;
using SaleOfDetails.Domain.Models;

namespace SaleOfDetails.Web.Controllers
{
    public class EmployeeController : BaseApiController
    {
        public EmployeeController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        // GET: api/Employee
        public IQueryable<Employee> GetEmployees()
        {
            return UnitOfWork.Repository<Employee>().GetQ();
        }

        // GET: api/Employee/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult GetEmployee(int id)
        {
            Employee employee = UnitOfWork.Repository<Employee>().GetById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/Employee/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            UnitOfWork.Repository<Employee>().Update(employee);

            try
            {
                UnitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Employee
        [ResponseType(typeof(Employee))]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UnitOfWork.Repository<Employee>().Insert(employee);
            UnitOfWork.Save();

            return CreatedAtRoute("DefaultApi", new { id = employee.EmployeeId }, employee);
        }

        // DELETE: api/Employee/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult DeleteEmployee(int id)
        {
            Employee employee = UnitOfWork.Repository<Employee>().GetById(id);
            if (employee == null)
            {
                return NotFound();
            }

            UnitOfWork.Repository<Employee>().Delete(employee);
            UnitOfWork.Save();

            return Ok(employee);
        }

        private bool EmployeeExists(int id)
        {
            return UnitOfWork.Repository<Employee>().GetQ().Count(e => e.EmployeeId == id) > 0;
        }
    }
}