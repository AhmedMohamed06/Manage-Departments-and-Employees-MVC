using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Demo.Pl.Helper;
using Demo.Pl.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Pl.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper)
		// Ask Clr  For Object  From Class Implement Interface
		{

			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}


		#region Actions
		//BaseUrl/Employee/Index
		public async Task<IActionResult> Index(string SearchValue)
		{
			IEnumerable<Employee> employees;
			if (string.IsNullOrEmpty(SearchValue))
			{
				employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
			}
			else
			{
				employees = await _unitOfWork.EmployeeRepository.GetEmployeeByName(SearchValue);
			}
			var MappedEmployee = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);
			return View(MappedEmployee);
		}
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			/// هنا لازم تاخد نسخه او اوبجكت من الاقسام عشان يتم عرضهم فى الفيو بتاع الانشاء للموظفيين
			/// لازم تحط async + await + Task 
			/// لان فكرة انه يروح للداتا بيز ويرجع ممكن تستهلك وقت فلو مستخدمتهاش الابليكشن بتاعك هيفضل واقف لحد ما العملية دي تتم 
			// وبعدين يعرض الفيو وده هيكلفك وقت وغير ان الابليكشن هيفضل واقف لحد ما العملية دي تتم 

			ViewBag.Departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
			return View(new EmployeeViewModel());
		}
		[HttpPost]
		public async Task<IActionResult> Create(EmployeeViewModel employeeView)
		{
			if (ModelState.IsValid)// Server  Side Validation
			{
				//Manaul Mapping 
				//var MappedEmployee = new Employee
				//{
				//    Name = employeeView.Name,
				//    Age = employeeView.Age,
				//    Address = employeeView.Address,
				//    Phone = employeeView.Phone,
				//    Email = employeeView.Email,
				//};
				// Employee employee = (Employee)employeeView;
				employeeView.ImageName = DocumentSettings.UpLoadFile(employeeView.Image, "Images");

				var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeView);
				await _unitOfWork.EmployeeRepository.AddAsync(MappedEmployee);
				//UPDATE
				//Delete
				//update
				int count = await _unitOfWork.CompleteAsync();
				if (count > 0)
				{
					TempData["Message"] = "Employee Added!";

				}
				else
				{
					TempData["Message"] = "Employee Dose not Added!";
				}
				return RedirectToAction(nameof(Index));

			}
			return View(employeeView);
		}
		//BaseUrl / Department/Details/100
		public async Task<IActionResult> Details(int? id, string ViewName = "Details")

		{
			if (id is null)
				return BadRequest();// Status Code 400

			var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id.Value);
			if (employee is null)
				return NotFound();
			var MappedEmployee = _mapper.Map<Employee, EmployeeViewModel>(employee);
			//Must write it here to render in html View
			ViewBag.Departments = await _unitOfWork.DepartmentRepository.GetAllAsync();


			///here use TempData to container the name of Image Old and Send it to Action Edite if have any change or Update New Image 
			// remove old Image and Send New Image
			TempData["ImageName"]= employee.ImageName as string;

			return View(ViewName, MappedEmployee);

		}

		[HttpGet]
		public async Task<IActionResult> Edit(int? id)
		{
            //if (id is null)
            //    return BadRequest();
            //var department = _departmentRepository.GetById(id.Value);
            //if (department is null)
            //    return NotFound();
            //return View(department);
            ViewBag.Departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
				
            return await Details(id, "Edit");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EmployeeViewModel employeeView, [FromRoute] int id)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if(employeeView.Image is not null)
					{
						var EmployeeImageBeforeUpdate = TempData["ImageName"] as string;


                        DocumentSettings.DeleteFile(EmployeeImageBeforeUpdate, "Images");
                        employeeView.ImageName =DocumentSettings.UpLoadFile(employeeView.Image, "Images");
					}
					var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeView);
					_unitOfWork.EmployeeRepository.Update(MappedEmployee);
					await _unitOfWork.CompleteAsync();
					return RedirectToAction(nameof(Index));
				}
				catch (System.Exception ex)
				{
					//1- Log Exceptions
					// 2- Form
					ModelState.AddModelError(string.Empty, ex.Message);

				}

			}
            //Must write it here to render in html View
            ViewBag.Departments = await _unitOfWork.DepartmentRepository.GetAllAsync();

            return View(employeeView);
		}
		public async Task<IActionResult> Delete(int? id)
		{
			return await Details(id, "Delete");

		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(EmployeeViewModel employeeView, [FromRoute] int id)
		{
			if ((id != employeeView.Id))
			{
				return BadRequest();
			}
			try
			{
				var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeView);
				_unitOfWork.EmployeeRepository.Delete(MappedEmployee);
				var Result = await _unitOfWork.CompleteAsync();
				if (Result > 0 && employeeView.ImageName is not null)
				{
					DocumentSettings.DeleteFile(employeeView.ImageName, "Images");
				}
				return RedirectToAction(nameof(Index));
			}
			catch (System.Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);

				return View(employeeView);
			}
		}
		#endregion

	}
}
