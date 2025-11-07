using ExamenDiegoUnidad2.Models.DTOs;

namespace ExamenDiegoUnidad2.Services.Interfaces
{
    public interface ICarreraService
    {
        Task<ApiResponse<List<CarreraDto>>> ObtenerCarrerasAsync();
        Task<ApiResponse<CarreraDto>> ObtenerCarreraPorIdAsync(int id);
        Task<ApiResponse<CarreraDto>> CrearCarreraAsync(CarreraCreateDto carreraDto);
        Task<ApiResponse<CarreraDto>> ActualizarCarreraAsync(int id, CarreraCreateDto carreraDto);
        Task<ApiResponse<bool>> EliminarCarreraAsync(int id);
        Task<ApiResponse<bool>> ValidarNombreCarreraUnicoAsync(string nombreCarrera, int? idCarreraExcluir = null);
    }
}