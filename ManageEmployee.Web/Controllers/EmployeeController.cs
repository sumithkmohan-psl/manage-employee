using ManageEmployee.Business.Services;
using ManageEmployee.Business.Validators;
using ManageEmployee.Models.DTOs;
using ManageEmployee.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeList()
        {
            try
            {
                var employees = await _employeeService.GetEmployees(null);

                var empViewModel = new EmployeeListViewModel
                {
                    EmployeeList = employees
                };

                return View(empViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employees.");

                ViewBag.ErrorMessage =
                    ex.Message;

                return View(new EmployeeListViewModel
                {
                    EmployeeList = new List<EmployeeDto>()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeList(string keyword)
        {
            try
            {
                if (keyword.Length > 100)
                {
                    throw new ApplicationException("Search keyword must be less than 100 characters");
                }

                var employees = await _employeeService.GetEmployees(keyword);

                var empViewModel = new EmployeeListViewModel
                {
                    EmployeeList = employees,
                    Keyword = keyword
                };

                return View(empViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employees.");

                ViewBag.ErrorMessage =
                    ex.Message;

                return View(new EmployeeListViewModel
                {
                    EmployeeList = new List<EmployeeDto>(),
                    Keyword = keyword
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TitleList()
        {
            try
            {
                var titles = await _employeeService.GetJobTitles();

                return View(titles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving job titles.");

                ViewBag.ErrorMessage =
                    ex.Message;

                return View(new List<TitleDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromForm] EmployeeDto request)
        {
            try
            {
                var errors = EmployeeValidator.Validate(request);

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                // DUPLICATE SSN CHECK
                bool isUnique = await _employeeService.IsUniqueSSN(request.SSN);

                if (!isUnique)
                {
                    ModelState.AddModelError(nameof(request.SSN), "SSN already exists.");
                }

                // FINAL CHECK
                if (!ModelState.IsValid)
                {
                    return View(request);
                }

                await _employeeService.AddEmployee(request);

                TempData["SuccessMessage"] = "Employee added successfully.";

                return RedirectToAction(nameof(AddEmployee));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding employee.");

                ViewBag.ErrorMessage = ex.Message;

                return View(request);
            }
        }
    }
}
