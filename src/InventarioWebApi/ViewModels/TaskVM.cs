using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.ViewModels
{
    public class TaskVM
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}