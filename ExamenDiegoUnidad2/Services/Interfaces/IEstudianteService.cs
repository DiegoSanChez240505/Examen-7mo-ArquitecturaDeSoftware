using ExamenDiegoUnidad2.Models.DTOs;

namespace ExamenDiegoUnidad2.Services.Interfaces
{
    public interface IEstudianteService
    {
        Task<ApiResponse<PaginatedResponse<EstudianteDto>>> ObtenerEstudiantesAsync(EstudianteFiltroDto filtro);
        Task<ApiResponse<EstudianteDto>> ObtenerEstudiantePorIdAsync(int id);
        Task<ApiResponse<EstudianteDto>> CrearEstudianteAsync(EstudianteCreateDto estudianteDto);
        Task<ApiResponse<EstudianteDto>> ActualizarEstudianteAsync(int id, EstudianteUpdateDto estudianteDto);
        Task<ApiResponse<bool>> EliminarEstudianteAsync(int id);
        Task<ApiResponse<List<EstudianteDto>>> ObtenerEstudiantesPorCarreraAsync(int idCarrera);
        Task<ApiResponse<List<EstudianteDto>>> ObtenerEstudiantesPorSemestreAsync(sbyte semestre);
        Task<ApiResponse<bool>> ValidarMatriculaUnicaAsync(string matricula, int? idEstudianteExcluir = null);
    }
}