using chat.Modelos;
using Chat.Mvc.Models;
using Chat.Mvc.Proxies;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

public class ChatApiProxy : IChatApiProxy
{
    private readonly HttpClient _httpClient;

    public ChatApiProxy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateGrupoAsync(Grupo grupo)
    {
        try
        {
            var payload = new
            {
                Id = grupo.Id,
                Name = grupo.Name,
                FechaCreacion = grupo.FechaCreacion,
                Users = grupo.Users.Select(u => new { u.Id, u.Name }).ToList()
            };

            var jsonPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
            Console.WriteLine("JSON enviado desde el proxy al backend:");
            Console.WriteLine(jsonPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Grupoes", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error al enviar la solicitud: {response.StatusCode} - {response.ReasonPhrase}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en CreateGrupoAsync: {ex.Message}");
            return false;
        }
    }
    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Users"); 
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<User>>(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en GetUsersAsync: {ex.Message}");
            return new List<User>();
        }
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"Users/{id}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<User>(content);
    }

    public async Task<List<Mensaje>> GetMensajesAsync()
    {
        var response = await _httpClient.GetAsync("Mensajes");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Mensaje>>(content);
    }

    public async Task<bool> CreateMensajeAsync(Mensaje mensaje)
    {
        try
        {
            var jsonData = JsonConvert.SerializeObject(mensaje);
            Console.WriteLine($"JSON enviado al proxy: {jsonData}");

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Mensajes", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Mensaje enviado correctamente.");
                return true;
            }
            else
            {
                Console.WriteLine($"Error al enviar mensaje. Código: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en CreateMensajeAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CheckNewMessagesAsync()
    {
        var response = await _httpClient.GetAsync("Mensajes/NuevosMensajes");
        response.EnsureSuccessStatusCode();
        return bool.Parse(await response.Content.ReadAsStringAsync());
    }

    public async Task<bool> MarcarMensajesComoLeidosAsync(List<Mensaje> mensajes)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(mensajes), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("Mensajes/MarcarComoLeidos", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al marcar mensajes como leídos: {ex.Message}");
            return false;
        }
    }
    public async Task<List<Grupo>> GetGruposAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Grupoes"); 
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Grupo>>(content) ?? new List<Grupo>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en GetGruposAsync: {ex.Message}");
            return new List<Grupo>(); 
        }
    }
    public async Task<bool> DeleteGrupoAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Grupoes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en DeleteGrupoAsync: {ex.Message}");
            return false;
        }
    }
    public async Task<List<MensajeConNombres>> BuscarMensajesPorContenidoAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("El parámetro de búsqueda no puede estar vacío.", nameof(query));
        }

        var response = await _httpClient.GetAsync($"/api/Mensajes/BuscarPorContenido?query={Uri.EscapeDataString(query)}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error al buscar mensajes: {response.ReasonPhrase}");
        }

        var content = await response.Content.ReadAsStringAsync();

        // Asegúrate de deserializar como una lista de `MensajeConNombres`
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<MensajeConNombres>>>(content);

        if (apiResponse == null || !apiResponse.Success)
        {
            throw new Exception("Error al procesar la respuesta de la API.");
        }

        return apiResponse.Mensajes ?? new List<MensajeConNombres>();
    }




    //public async Task<List<Grupo>> GetGruposAsync()
    //{
    //    try
    //    {

    //        var response = await _httpClient.GetAsync("Grupos");
    //        response.EnsureSuccessStatusCode();


    //        var content = await response.Content.ReadAsStringAsync();
    //        var grupos = JsonConvert.DeserializeObject<List<Grupo>>(content);

    //        if (grupos == null || grupos.Count == 0)
    //        {
    //            Console.WriteLine("No se encontraron grupos disponibles.");
    //            return new List<Grupo>();
    //        }

    //        return grupos;
    //    }
    //    catch (HttpRequestException ex)
    //    {

    //        Console.WriteLine($"Error al obtener grupos: {ex.Message}");
    //    }
    //    catch (JsonException ex)
    //    {

    //        Console.WriteLine($"Error al procesar la respuesta de la API: {ex.Message}");
    //    }
    //    catch (Exception ex)
    //    {

    //        Console.WriteLine($"Error inesperado: {ex.Message}");
    //    }


    //    Console.WriteLine("No se pudieron cargar los grupos debido a un error.");
    //    return new List<Grupo>();
    //}

    public async Task<bool> MarcarMensajesGrupoComoLeidosAsync(int grupoId)
    {
        var response = await _httpClient.PutAsync($"Mensajes/MarcarMensajesGrupoComoLeidos/{grupoId}", null);
        return response.IsSuccessStatusCode;
    }
    public async Task<bool> CreateUserAsync(User user)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Users", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error al crear usuario: {response.StatusCode} - {response.ReasonPhrase}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en CreateUserAsync: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Users/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en DeleteUserAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<Mensaje> GetMensajeByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Mensajes/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var mensaje = JsonConvert.DeserializeObject<Mensaje>(content);

            if (mensaje == null)
            {
                Console.WriteLine($"Mensaje con ID {id} no encontrado.");
            }

            return mensaje;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al obtener el mensaje con ID {id}: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error al procesar el mensaje con ID {id}: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado al obtener el mensaje con ID {id}: {ex.Message}");
        }

        return null; 
    }

    public async Task<bool> UpdateMensajeAsync(Mensaje mensaje)
    {
        try
        {
            var jsonData = JsonConvert.SerializeObject(mensaje);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Mensajes/{mensaje.Id}", content);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error al actualizar el mensaje con ID {mensaje.Id}: {response.StatusCode} - {response.ReasonPhrase}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error de red al actualizar el mensaje con ID {mensaje.Id}: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado al actualizar el mensaje con ID {mensaje.Id}: {ex.Message}");
        }

        return false;
    }
    public async Task<bool> DeleteMensajeAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Mensajes/{id}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error al eliminar el mensaje con ID {id}: {response.StatusCode} - {response.ReasonPhrase}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error de red al eliminar el mensaje con ID {id}: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado al eliminar el mensaje con ID {id}: {ex.Message}");
        }

        return false;
    }

    public async Task<Grupo> GetGrupoByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Grupoes/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Grupo>(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en GetGrupoByIdAsync: {ex.Message}");
            return null;
        }
    }



}