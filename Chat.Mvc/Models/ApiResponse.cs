namespace Chat.Mvc.Models
{

        public class ApiResponse<T>
        {
            public bool Success { get; set; } 
            public T Mensajes { get; set; } 
        }
    }

