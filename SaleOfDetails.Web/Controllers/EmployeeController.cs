using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using SaleOfDetails.Domain.Context;
using SaleOfDetails.Domain.DataAccess.Interfaces;
using SaleOfDetails.Domain.Models;
using SaleOfDetails.Web.Models;

namespace SaleOfDetails.Web.Controllers
{
    public class EmployeeController : BaseApiController
    {
        public EmployeeController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        // GET: api/Employee
        public IEnumerable<EmployeeViewModel> GetEmployees()
        {
            var employees = UnitOfWork.Repository<Employee>()
                .Get(
                    orderBy: o => o.OrderBy(p => p.Person.LastName)
                        .ThenBy(p => p.Person.FirstName),
                    includeProperties: "Person");

            var employeeViewModels = Mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);

            return employeeViewModels;
        }

        // GET: api/Employee
        public ListViewModel<EmployeeViewModel> GetEmployees(int page, int pageSize = 10)
        {
            var employeesList = UnitOfWork.Repository<Employee>()
                .GetQ(
                    orderBy: o => o.OrderBy(p => p.Person.LastName)
                        .ThenBy(p => p.Person.FirstName),
                    includeProperties: "Person");
                
            var employees = employeesList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var employeeViewModels = Mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);
            var viewModel = new ListViewModel<EmployeeViewModel>
            {
                Items = employeeViewModels,
                ItemsCount = employeesList.Count(),
                PagesCount = (int)Math.Ceiling((double)employeesList.Count() / pageSize),
                SelectedPage = page
            };

            return viewModel;
        }

        // GET: api/Employee/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult GetEmployee(int id)
        {
            var employee = UnitOfWork.Repository<Employee>()
                .Get(x => x.EmployeeId == id, includeProperties: "Person")
                .SingleOrDefault();
            if (employee == null)
            {
                return NotFound();
            }

            var employeeViewModel = Mapper.Map<Employee, EmployeeViewModel>(employee);

            return Ok(employeeViewModel);
        }

        // PUT: api/Employee/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployee(EmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = UnitOfWork.Repository<Employee>()
                .Get(x => x.EmployeeId == viewModel.EmployeeId)
                .SingleOrDefault();
            if (employee == null)
            {
                return BadRequest();
            }

            Mapper.Map<EmployeeViewModel, Employee>(viewModel, employee);
            employee.Person.UpdatedAt = DateTime.Now;
            employee.UpdatedAt = DateTime.Now;

            UnitOfWork.Repository<Employee>().Update(employee);

            try
            {
                UnitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(viewModel.EmployeeId))
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
        public IHttpActionResult PostEmployee(EmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = Mapper.Map<EmployeeViewModel, Employee>(viewModel);
            employee.Person.CreatedAt = DateTime.Now;
            employee.CreatedAt = DateTime.Now;          

            UnitOfWork.Repository<Employee>().Insert(employee);
            UnitOfWork.Save();

            return Ok();
            // return CreatedAtRoute("DefaultApi", new { id = employee.EmployeeId }, employee);
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