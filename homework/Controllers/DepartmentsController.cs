using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using homework.Models;

namespace homework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.Where(d => d.IsDeleted == false || d.IsDeleted == null).ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null || department.IsDeleted == true)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            var olddep = await _context.Department.FindAsync(id);
            olddep.DateModified = DateTime.Now;

            _context.Entry(department).State = EntityState.Modified;


            try
            {
                await _context.Department.FromSqlRaw("EXECUTE dbo.Department_Update {0},{1}, {2}, {3}, {4},{5}", department.DepartmentId, department.Name, department.Budget, department.StartDate, department.InstructorId, department.RowVersion).ToListAsync();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            //_context.Department.Add(department);

            var result = await  _context.Department.FromSqlRaw("EXECUTE dbo.Department_Insert {0}, {1}, {2}, {3}", department.Name,department.Budget,department.StartDate,department.InstructorId).ToListAsync();
            //await _context.SaveChangesAsync();


            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {

            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            //await _context.Department.FromSqlRaw("EXECUTE dbo.Department_Delete @DepartmentID", id).ToListAsync();

            //_context.Department.Remove(department);
            department.IsDeleted = true;

            await _context.SaveChangesAsync();

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
