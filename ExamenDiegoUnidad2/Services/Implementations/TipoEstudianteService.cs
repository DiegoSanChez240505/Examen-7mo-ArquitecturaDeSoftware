using Microsoft.EntityFrameworkCore;
using ExamenDiegoUnidad2.Data;
using ExamenDiegoUnidad2.Data.Models;
using ExamenDiegoUnidad2.Models.DTOs;
using ExamenDiegoUnidad2.Services.Interfaces;

namespace ExamenDiegoUnidad2.Services.Implementations
{
    public class TipoEstudianteService : ITipoEstudianteService
    {
        private readonly Data.AppContext _context;

        public TipoEstudianteService(Data.AppContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<TipoEstudianteDto>>> ObtenerTiposEstudianteAsync()
        {
            try
            {
                var tipos = await _context.TiposEstudiante
                    .Include(t => t.Estudiantes)
                    .Select(t => new TipoEstudianteDto
                    {
                        IdTipoEstudiante = t.IdTipoEstudiante,
                        NombreTipo = t.NombreTipo,
                        TotalEstudiantes = t.Estudiantes.Count
                    })
                    .OrderBy(t => t.NombreTipo)
                    .ToListAsync();

                return ApiResponse<List<TipoEstudianteDto>>.SuccessResponse(tipos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<TipoEstudianteDto>>.ErrorResponse($"Error al obtener tipos de estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TipoEstudianteDto>> ObtenerTipoEstudiantePorIdAsync(int id)
        {
            try
            {
                var tipo = await _context.TiposEstudiante
                    .Include(t => t.Estudiantes)
                    .FirstOrDefaultAsync(t => t.IdTipoEstudiante == id);

                if (tipo == null)
                    return ApiResponse<TipoEstudianteDto>.ErrorResponse("Tipo de estudiante no encontrado");

                var tipoDto = new TipoEstudianteDto
                {
                    IdTipoEstudiante = tipo.IdTipoEstudiante,
                    NombreTipo = tipo.NombreTipo,
                    TotalEstudiantes = tipo.Estudiantes.Count
                };

                return ApiResponse<TipoEstudianteDto>.SuccessResponse(tipoDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<TipoEstudianteDto>.ErrorResponse($"Error al obtener tipo de estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TipoEstudianteDto>> CrearTipoEstudianteAsync(TipoEstudianteCreateDto tipoDto)
        {
            try
            {
                // Validar que el nombre sea único
                var nombreExiste = await _context.TiposEstudiante
                    .AnyAsync(t => t.NombreTipo.ToLower() == tipoDto.NombreTipo.ToLower());

                if (nombreExiste)
                    return ApiResponse<TipoEstudianteDto>.ErrorResponse("Ya existe un tipo de estudiante con ese nombre");

                var tipo = new TiposEstudiante
                {
                    NombreTipo = tipoDto.NombreTipo
                };

                _context.TiposEstudiante.Add(tipo);
                await _context.SaveChangesAsync();

                var resultado = new TipoEstudianteDto
                {
                    IdTipoEstudiante = tipo.IdTipoEstudiante,
                    NombreTipo = tipo.NombreTipo,
                    TotalEstudiantes = 0
                };

                return ApiResponse<TipoEstudianteDto>.SuccessResponse(resultado, "Tipo de estudiante creado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<TipoEstudianteDto>.ErrorResponse($"Error al crear tipo de estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TipoEstudianteDto>> ActualizarTipoEstudianteAsync(int id, TipoEstudianteCreateDto tipoDto)
        {
            try
            {
                var tipo = await _context.TiposEstudiante.FindAsync(id);
                if (tipo == null)
                    return ApiResponse<TipoEstudianteDto>.ErrorResponse("Tipo de estudiante no encontrado");

                // Validar que el nombre sea único (excluyendo el tipo actual)
                var nombreExiste = await _context.TiposEstudiante
                    .AnyAsync(t => t.NombreTipo.ToLower() == tipoDto.NombreTipo.ToLower() && t.IdTipoEstudiante != id);

                if (nombreExiste)
                    return ApiResponse<TipoEstudianteDto>.ErrorResponse("Ya existe un tipo de estudiante con ese nombre");

                tipo.NombreTipo = tipoDto.NombreTipo;
                await _context.SaveChangesAsync();

                // Obtener tipo actualizado con conteo de estudiantes
                var tipoActualizado = await _context.TiposEstudiante
                    .Include(t => t.Estudiantes)
                    .FirstOrDefaultAsync(t => t.IdTipoEstudiante == id);

                var resultado = new TipoEstudianteDto
                {
                    IdTipoEstudiante = tipoActualizado!.IdTipoEstudiante,
                    NombreTipo = tipoActualizado.NombreTipo,
                    TotalEstudiantes = tipoActualizado.Estudiantes.Count
                };

                return ApiResponse<TipoEstudianteDto>.SuccessResponse(resultado, "Tipo de estudiante actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<TipoEstudianteDto>.ErrorResponse($"Error al actualizar tipo de estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> EliminarTipoEstudianteAsync(int id)
        {
            try
            {
                var tipo = await _context.TiposEstudiante
                    .Include(t => t.Estudiantes)
                    .FirstOrDefaultAsync(t => t.IdTipoEstudiante == id);

                if (tipo == null)
                    return ApiResponse<bool>.ErrorResponse("Tipo de estudiante no encontrado");

                // Verificar si tiene estudiantes asociados
                if (tipo.Estudiantes.Any())
                    return ApiResponse<bool>.ErrorResponse("No se puede eliminar el tipo de estudiante porque tiene estudiantes asociados");

                _context.TiposEstudiante.Remove(tipo);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Tipo de estudiante eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al eliminar tipo de estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ValidarNombreTipoUnicoAsync(string nombreTipo, int? idTipoExcluir = null)
        {
            try
            {
                var query = _context.TiposEstudiante.Where(t => t.NombreTipo.ToLower() == nombreTipo.ToLower());
                
                if (idTipoExcluir.HasValue)
                    query = query.Where(t => t.IdTipoEstudiante != idTipoExcluir.Value);

                var existe = await query.AnyAsync();
                return ApiResponse<bool>.SuccessResponse(!existe);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al validar nombre de tipo: {ex.Message}");
            }
        }
    }
}