using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SanSaludAPI.DataAccess
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly SanSaludDbContext _context;

        public TurnoRepository(SanSaludDbContext context)
        {
            _context = context;
        }

        public async Task<Turno> CreateAsync(Turno turno)
        {
            turno.Id = Guid.NewGuid();
            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(turno.Id) ?? turno;
        }

        public async Task<Turno?> GetByIdAsync(Guid id)
        {
            return await _context.Turnos
                .AsNoTracking()
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Turno>> GetAllAsync()
        {
            return await _context.Turnos
                .AsNoTracking()
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .ToListAsync();
        }

        public async Task<IEnumerable<Turno>> GetByPacienteIdAsync(Guid pacienteId)
        {
            return await _context.Turnos
                .AsNoTracking()
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .Where(t => t.PacienteId == pacienteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Turno>> GetByMedicoIdAsync(Guid medicoId)
        {
            return await _context.Turnos
                .AsNoTracking()
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .Where(t => t.MedicoId == medicoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Turno>> GetByMedicoAndDateRangeAsync(Guid medicoId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Turnos
                .AsNoTracking()
                .Where(t => t.MedicoId == medicoId && t.FechaHora >= fechaInicio && t.FechaHora <= fechaFin)
                .ToListAsync();
        }

        public async Task<Turno> UpdateAsync(Turno turno)
        {
            _context.Turnos.Update(turno);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(turno.Id) ?? turno;
        }

        public async Task DeleteAsync(Guid id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno != null)
            {
                _context.Turnos.Remove(turno);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Turnos.AnyAsync(t => t.Id == id);
        }
    }
}
