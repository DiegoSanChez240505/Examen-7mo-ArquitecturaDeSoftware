using ExamenDiegoUnidad2.Models.DTOs;

namespace ExamenDiegoUnidad2.Services.Interfaces
{
    public interface ITipoEstudianteService
    {
        Task<ApiResponse<List<TipoEstudianteDto>>> ObtenerTiposEstudianteAsync();
        Task<ApiResponse<TipoEstudianteDto>> ObtenerTipoEstudiantePorIdAsync(int id);
        Task<ApiResponse<TipoEstudianteDto>> CrearTipoEstudianteAsync(TipoEstudianteCreateDto tipoDto);
        Task<ApiResponse<TipoEstudianteDto>> ActualizarTipoEstudianteAsync(int id, TipoEstudianteCreateDto tipoDto);
        Task<ApiResponse<bool>> EliminarTipoEstudianteAsync(int id);
        Task<ApiResponse<bool>> ValidarNombreTipoUnicoAsync(string nombreTipo, int? idTipoExcluir = null);
    }
}