using Microsoft.EntityFrameworkCore;
using ExamenDiegoUnidad2.Data;
using ExamenDiegoUnidad2.Data.Models;
using ExamenDiegoUnidad2.Models.DTOs;
using ExamenDiegoUnidad2.Services.Interfaces;

namespace ExamenDiegoUnidad2.Services.Implementations
{
    public class CarreraService : ICarreraService
    {
        private readonly Data.AppContext _context;

        public CarreraService(Data.AppContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CarreraDto>>> ObtenerCarrerasAsync()
        {
            try
            {
                var carreras = await _context.Carreras
                    .Include(c => c.Estudiantes)
                    .Select(c => new CarreraDto
                    {
                        IdCarrera = c.IdCarrera,
                        NombreCarrera = c.NombreCarrera,
                        TotalEstudiantes = c.Estudiantes.Count
                    })
                    .OrderBy(c => c.NombreCarrera)
                    .ToListAsync();

                return ApiResponse<List<CarreraDto>>.SuccessResponse(carreras);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CarreraDto>>.ErrorResponse($"Error al obtener carreras: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CarreraDto>> ObtenerCarreraPorIdAsync(int id)
        {
            try
            {
                var carrera = await _context.Carreras
                    .Include(c => c.Estudiantes)
                    .FirstOrDefaultAsync(c => c.IdCarrera == id);

                if (carrera == null)
                    return ApiResponse<CarreraDto>.ErrorResponse("Carrera no encontrada");

                var carreraDto = new CarreraDto
                {
                    IdCarrera = carrera.IdCarrera,
                    NombreCarrera = carrera.NombreCarrera,
                    TotalEstudiantes = carrera.Estudiantes.Count
                };

                return ApiResponse<CarreraDto>.SuccessResponse(carreraDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<CarreraDto>.ErrorResponse($"Error al obtener carrera: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CarreraDto>> CrearCarreraAsync(CarreraCreateDto carreraDto)
        {
            try
            {
                // Validar que el nombre sea único
                var nombreExiste = await _context.Carreras
                    .AnyAsync(c => c.NombreCarrera.ToLower() == carreraDto.NombreCarrera.ToLower());

                if (nombreExiste)
                    return ApiResponse<CarreraDto>.ErrorResponse("Ya existe una carrera con ese nombre");

                var carrera = new Carreras
                {
                    NombreCarrera = carreraDto.NombreCarrera
                };

                _context.Carreras.Add(carrera);
                await _context.SaveChangesAsync();

                var resultado = new CarreraDto
                {
                    IdCarrera = carrera.IdCarrera,
                    NombreCarrera = carrera.NombreCarrera,
                    TotalEstudiantes = 0
                };

                return ApiResponse<CarreraDto>.SuccessResponse(resultado, "Carrera creada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<CarreraDto>.ErrorResponse($"Error al crear carrera: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CarreraDto>> ActualizarCarreraAsync(int id, CarreraCreateDto carreraDto)
        {
            try
            {
                var carrera = await _context.Carreras.FindAsync(id);
                if (carrera == null)
                    return ApiResponse<CarreraDto>.ErrorResponse("Carrera no encontrada");

                // Validar que el nombre sea único (excluyendo la carrera actual)
                var nombreExiste = await _context.Carreras
                    .AnyAsync(c => c.NombreCarrera.ToLower() == carreraDto.NombreCarrera.ToLower() && c.IdCarrera != id);

                if (nombreExiste)
                    return ApiResponse<CarreraDto>.ErrorResponse("Ya existe una carrera con ese nombre");

                carrera.NombreCarrera = carreraDto.NombreCarrera;
                await _context.SaveChangesAsync();

                // Obtener carrera actualizada con conteo de estudiantes
                var carreraActualizada = await _context.Carreras
                    .Include(c => c.Estudiantes)
                    .FirstOrDefaultAsync(c => c.IdCarrera == id);

                var resultado = new CarreraDto
                {
                    IdCarrera = carreraActualizada!.IdCarrera,
                    NombreCarrera = carreraActualizada.NombreCarrera,
                    TotalEstudiantes = carreraActualizada.Estudiantes.Count
                };

                return ApiResponse<CarreraDto>.SuccessResponse(resultado, "Carrera actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<CarreraDto>.ErrorResponse($"Error al actualizar carrera: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> EliminarCarreraAsync(int id)
        {
            try
            {
                var carrera = await _context.Carreras
                    .Include(c => c.Estudiantes)
                    .FirstOrDefaultAsync(c => c.IdCarrera == id);

                if (carrera == null)
                    return ApiResponse<bool>.ErrorResponse("Carrera no encontrada");

                // Verificar si tiene estudiantes asociados
                if (carrera.Estudiantes.Any())
                    return ApiResponse<bool>.ErrorResponse("No se puede eliminar la carrera porque tiene estudiantes asociados");

                _context.Carreras.Remove(carrera);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Carrera eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al eliminar carrera: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ValidarNombreCarreraUnicoAsync(string nombreCarrera, int? idCarreraExcluir = null)
        {
            try
            {
                var query = _context.Carreras.Where(c => c.NombreCarrera.ToLower() == nombreCarrera.ToLower());
                
                if (idCarreraExcluir.HasValue)
                    query = query.Where(c => c.IdCarrera != idCarreraExcluir.Value);

                var existe = await query.AnyAsync();
                return ApiResponse<bool>.SuccessResponse(!existe);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al validar nombre de carrera: {ex.Message}");
            }
        }
    }
}