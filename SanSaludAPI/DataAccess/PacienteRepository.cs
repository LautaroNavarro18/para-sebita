using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SanSaludAPI.DataAccess
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly SanSaludDbContext _context;

        public PacienteRepository(SanSaludDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Paciente>> GetAllAsync()
        {
            return await _context.Pacientes.ToListAsync();
        }

        public async Task<Paciente?> GetByIdAsync(Guid id)
        {
            return await _context.Pacientes.FindAsync(id);
        }

        public async Task<Paciente> CreateAsync(Paciente paciente)
        {
            paciente.Id = Guid.NewGuid();
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return paciente;
        }
    }
}
