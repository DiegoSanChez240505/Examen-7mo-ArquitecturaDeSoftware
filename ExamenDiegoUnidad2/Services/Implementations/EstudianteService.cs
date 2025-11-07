using Microsoft.EntityFrameworkCore;
using ExamenDiegoUnidad2.Data;
using ExamenDiegoUnidad2.Data.Models;
using ExamenDiegoUnidad2.Models.DTOs;
using ExamenDiegoUnidad2.Services.Interfaces;
using ExamenDiegoUnidad2.Factories;

namespace ExamenDiegoUnidad2.Services.Implementations
{
    public class EstudianteService : IEstudianteService
    {
        private readonly Data.AppContext _context;

        public EstudianteService(Data.AppContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PaginatedResponse<EstudianteDto>>> ObtenerEstudiantesAsync(EstudianteFiltroDto filtro)
        {
            try
            {
                var query = _context.Estudiantes
                    .Include(e => e.IdCarreraFkNavigation)
                    .Include(e => e.IdTipoEstudianteFkNavigation)
                    .AsQueryable();

                // Aplicar filtros
                if (filtro.IdCarrera.HasValue)
                    query = query.Where(e => e.IdCarreraFk == filtro.IdCarrera.Value);

                if (filtro.Semestre.HasValue)
                    query = query.Where(e => e.Semestre == filtro.Semestre.Value);

                if (filtro.IdTipoEstudiante.HasValue)
                    query = query.Where(e => e.IdTipoEstudianteFk == filtro.IdTipoEstudiante.Value);

                if (!string.IsNullOrWhiteSpace(filtro.NombreCompleto))
                    query = query.Where(e => e.NombreCompleto.Contains(filtro.NombreCompleto));

                if (!string.IsNullOrWhiteSpace(filtro.Matricula))
                    query = query.Where(e => e.Matricula.Contains(filtro.Matricula));

                if (filtro.PromedioMinimo.HasValue)
                    query = query.Where(e => e.Promedio >= filtro.PromedioMinimo.Value);

                if (filtro.PromedioMaximo.HasValue)
                    query = query.Where(e => e.Promedio <= filtro.PromedioMaximo.Value);

                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalRecords / filtro.PageSize);

                var estudiantes = await query
                    .OrderBy(e => e.NombreCompleto)
                    .Skip((filtro.Page - 1) * filtro.PageSize)
                    .Take(filtro.PageSize)
                    .Select(e => new EstudianteDto
                    {
                        IdEstudiante = e.IdEstudiante,
                        Matricula = e.Matricula,
                        NombreCompleto = e.NombreCompleto,
                        Semestre = e.Semestre,
                        Promedio = e.Promedio,
                        IdCarrera = e.IdCarreraFk,
                        IdTipoEstudiante = e.IdTipoEstudianteFk,
                        NombreCarrera = e.IdCarreraFkNavigation.NombreCarrera,
                        TipoEstudiante = e.IdTipoEstudianteFkNavigation.NombreTipo
                    })
                    .ToListAsync();

                var response = new PaginatedResponse<EstudianteDto>
                {
                    Data = estudiantes,
                    TotalRecords = totalRecords,
                    Page = filtro.Page,
                    PageSize = filtro.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = filtro.Page < totalPages,
                    HasPreviousPage = filtro.Page > 1
                };

                return ApiResponse<PaginatedResponse<EstudianteDto>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaginatedResponse<EstudianteDto>>.ErrorResponse($"Error al obtener estudiantes: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstudianteDto>> ObtenerEstudiantePorIdAsync(int id)
        {
            try
            {
                var estudiante = await _context.Estudiantes
                    .Include(e => e.IdCarreraFkNavigation)
                    .Include(e => e.IdTipoEstudianteFkNavigation)
                    .FirstOrDefaultAsync(e => e.IdEstudiante == id);

                if (estudiante == null)
                    return ApiResponse<EstudianteDto>.ErrorResponse("Estudiante no encontrado");

                var estudianteDto = new EstudianteDto
                {
                    IdEstudiante = estudiante.IdEstudiante,
                    Matricula = estudiante.Matricula,
                    NombreCompleto = estudiante.NombreCompleto,
                    Semestre = estudiante.Semestre,
                    Promedio = estudiante.Promedio,
                    IdCarrera = estudiante.IdCarreraFk,
                    IdTipoEstudiante = estudiante.IdTipoEstudianteFk,
                    NombreCarrera = estudiante.IdCarreraFkNavigation.NombreCarrera,
                    TipoEstudiante = estudiante.IdTipoEstudianteFkNavigation.NombreTipo
                };

                return ApiResponse<EstudianteDto>.SuccessResponse(estudianteDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<EstudianteDto>.ErrorResponse($"Error al obtener estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstudianteDto>> CrearEstudianteAsync(EstudianteCreateDto estudianteDto)
        {
            try
            {
                // Validar que la matrícula sea única
                var matriculaExiste = await _context.Estudiantes
                    .AnyAsync(e => e.Matricula == estudianteDto.Matricula);

                if (matriculaExiste)
                    return ApiResponse<EstudianteDto>.ErrorResponse("La matrícula ya existe en el sistema");

                // Validar que la carrera exista
                var carreraExiste = await _context.Carreras
                    .AnyAsync(c => c.IdCarrera == estudianteDto.IdCarrera);

                if (!carreraExiste)
                    return ApiResponse<EstudianteDto>.ErrorResponse("La carrera especificada no existe");

                // Validar que el tipo de estudiante exista
                var tipoExiste = await _context.TiposEstudiante
                    .AnyAsync(t => t.IdTipoEstudiante == estudianteDto.IdTipoEstudiante);

                if (!tipoExiste)
                    return ApiResponse<EstudianteDto>.ErrorResponse("El tipo de estudiante especificado no existe");

                // Usar Factory Pattern para crear el estudiante
                var factory = EstudianteFactoryProvider.ObtenerFactory(estudianteDto.IdTipoEstudiante);
                var estudiante = factory.CrearEntidadEstudiante(
                    estudianteDto.Matricula,
                    estudianteDto.NombreCompleto,
                    estudianteDto.Semestre,
                    estudianteDto.Promedio,
                    estudianteDto.IdCarrera,
                    estudianteDto.IdTipoEstudiante
                );

                _context.Estudiantes.Add(estudiante);
                await _context.SaveChangesAsync();

                // Obtener el estudiante creado con sus relaciones
                var estudianteCreado = await _context.Estudiantes
                    .Include(e => e.IdCarreraFkNavigation)
                    .Include(e => e.IdTipoEstudianteFkNavigation)
                    .FirstOrDefaultAsync(e => e.IdEstudiante == estudiante.IdEstudiante);

                var resultado = new EstudianteDto
                {
                    IdEstudiante = estudianteCreado!.IdEstudiante,
                    Matricula = estudianteCreado.Matricula,
                    NombreCompleto = estudianteCreado.NombreCompleto,
                    Semestre = estudianteCreado.Semestre,
                    Promedio = estudianteCreado.Promedio,
                    IdCarrera = estudianteCreado.IdCarreraFk,
                    IdTipoEstudiante = estudianteCreado.IdTipoEstudianteFk,
                    NombreCarrera = estudianteCreado.IdCarreraFkNavigation.NombreCarrera,
                    TipoEstudiante = estudianteCreado.IdTipoEstudianteFkNavigation.NombreTipo
                };

                return ApiResponse<EstudianteDto>.SuccessResponse(resultado, "Estudiante creado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<EstudianteDto>.ErrorResponse($"Error al crear estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstudianteDto>> ActualizarEstudianteAsync(int id, EstudianteUpdateDto estudianteDto)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(id);
                if (estudiante == null)
                    return ApiResponse<EstudianteDto>.ErrorResponse("Estudiante no encontrado");

                // Validar que la matrícula sea única (excluyendo el estudiante actual)
                var matriculaExiste = await _context.Estudiantes
                    .AnyAsync(e => e.Matricula == estudianteDto.Matricula && e.IdEstudiante != id);

                if (matriculaExiste)
                    return ApiResponse<EstudianteDto>.ErrorResponse("La matrícula ya existe en el sistema");

                // Validar que la carrera exista
                var carreraExiste = await _context.Carreras
                    .AnyAsync(c => c.IdCarrera == estudianteDto.IdCarrera);

                if (!carreraExiste)
                    return ApiResponse<EstudianteDto>.ErrorResponse("La carrera especificada no existe");

                // Validar que el tipo de estudiante exista
                var tipoExiste = await _context.TiposEstudiante
                    .AnyAsync(t => t.IdTipoEstudiante == estudianteDto.IdTipoEstudiante);

                if (!tipoExiste)
                    return ApiResponse<EstudianteDto>.ErrorResponse("El tipo de estudiante especificado no existe");

                // Actualizar datos
                estudiante.Matricula = estudianteDto.Matricula;
                estudiante.NombreCompleto = estudianteDto.NombreCompleto;
                estudiante.Semestre = estudianteDto.Semestre;
                estudiante.Promedio = estudianteDto.Promedio;
                estudiante.IdCarreraFk = estudianteDto.IdCarrera;
                estudiante.IdTipoEstudianteFk = estudianteDto.IdTipoEstudiante;

                await _context.SaveChangesAsync();

                // Obtener el estudiante actualizado con sus relaciones
                var estudianteActualizado = await _context.Estudiantes
                    .Include(e => e.IdCarreraFkNavigation)
                    .Include(e => e.IdTipoEstudianteFkNavigation)
                    .FirstOrDefaultAsync(e => e.IdEstudiante == id);

                var resultado = new EstudianteDto
                {
                    IdEstudiante = estudianteActualizado!.IdEstudiante,
                    Matricula = estudianteActualizado.Matricula,
                    NombreCompleto = estudianteActualizado.NombreCompleto,
                    Semestre = estudianteActualizado.Semestre,
                    Promedio = estudianteActualizado.Promedio,
                    IdCarrera = estudianteActualizado.IdCarreraFk,
                    IdTipoEstudiante = estudianteActualizado.IdTipoEstudianteFk,
                    NombreCarrera = estudianteActualizado.IdCarreraFkNavigation.NombreCarrera,
                    TipoEstudiante = estudianteActualizado.IdTipoEstudianteFkNavigation.NombreTipo
                };

                return ApiResponse<EstudianteDto>.SuccessResponse(resultado, "Estudiante actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<EstudianteDto>.ErrorResponse($"Error al actualizar estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> EliminarEstudianteAsync(int id)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(id);
                if (estudiante == null)
                    return ApiResponse<bool>.ErrorResponse("Estudiante no encontrado");

                _context.Estudiantes.Remove(estudiante);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Estudiante eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al eliminar estudiante: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<EstudianteDto>>> ObtenerEstudiantesPorCarreraAsync(int idCarrera)
        {
            try
            {
                var estudiantes = await _context.Estudiantes
                    .Include(e => e.IdCarreraFkNavigation)
                    .Include(e => e.IdTipoEstudianteFkNavigation)
                    .Where(e => e.IdCarreraFk == idCarrera)
                    .Select(e => new EstudianteDto
                    {
                        IdEstudiante = e.IdEstudiante,
                        Matricula = e.Matricula,
                        NombreCompleto = e.NombreCompleto,
                        Semestre = e.Semestre,
                        Promedio = e.Promedio,
                        IdCarrera = e.IdCarreraFk,
                        IdTipoEstudiante = e.IdTipoEstudianteFk,
                        NombreCarrera = e.IdCarreraFkNavigation.NombreCarrera,
                        TipoEstudiante = e.IdTipoEstudianteFkNavigation.NombreTipo
                    })
                    .OrderBy(e => e.NombreCompleto)
                    .ToListAsync();

                return ApiResponse<List<EstudianteDto>>.SuccessResponse(estudiantes);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<EstudianteDto>>.ErrorResponse($"Error al obtener estudiantes por carrera: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<EstudianteDto>>> ObtenerEstudiantesPorSemestreAsync(sbyte semestre)
        {
            try
            {
                var estudiantes = await _context.Estudiantes
                    .Include(e => e.IdCarreraFkNavigation)
                    .Include(e => e.IdTipoEstudianteFkNavigation)
                    .Where(e => e.Semestre == semestre)
                    .Select(e => new EstudianteDto
                    {
                        IdEstudiante = e.IdEstudiante,
                        Matricula = e.Matricula,
                        NombreCompleto = e.NombreCompleto,
                        Semestre = e.Semestre,
                        Promedio = e.Promedio,
                        IdCarrera = e.IdCarreraFk,
                        IdTipoEstudiante = e.IdTipoEstudianteFk,
                        NombreCarrera = e.IdCarreraFkNavigation.NombreCarrera,
                        TipoEstudiante = e.IdTipoEstudianteFkNavigation.NombreTipo
                    })
                    .OrderBy(e => e.NombreCompleto)
                    .ToListAsync();

                return ApiResponse<List<EstudianteDto>>.SuccessResponse(estudiantes);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<EstudianteDto>>.ErrorResponse($"Error al obtener estudiantes por semestre: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ValidarMatriculaUnicaAsync(string matricula, int? idEstudianteExcluir = null)
        {
            try
            {
                var query = _context.Estudiantes.Where(e => e.Matricula == matricula);
                
                if (idEstudianteExcluir.HasValue)
                    query = query.Where(e => e.IdEstudiante != idEstudianteExcluir.Value);

                var existe = await query.AnyAsync();
                return ApiResponse<bool>.SuccessResponse(!existe);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al validar matrícula: {ex.Message}");
            }
        }
    }
}